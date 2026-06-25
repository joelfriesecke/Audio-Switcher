namespace Loupedeck.AudioSwitcherPlugin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

internal readonly record struct AudioDeviceInfo(String Key, String Name);

internal static class AudioDeviceManager
{
    private static readonly Regex GuidPattern =
        new Regex(@"[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}", RegexOptions.Compiled);

    private readonly record struct Endpoint(String EndpointId, String Key, String Name);

    public static IReadOnlyList<AudioDeviceInfo> GetDevices(EDataFlow flow)
    {
        try
        {
            return Enumerate(flow)
                .Select(e => new AudioDeviceInfo(e.Key, e.Name))
                .ToList();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Failed to enumerate audio devices: {ex.Message}");
            return [];
        }
    }

    public static AudioDeviceInfo? GetDefaultDevice(EDataFlow flow)
    {
        try
        {
            return GetDefaultEndpoint(flow) is { } e ? new AudioDeviceInfo(e.Key, e.Name) : null;
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Failed to get default audio device: {ex.Message}");
            return null;
        }
    }

    public static Boolean SetDefault(String deviceKey)
    {
        if (String.IsNullOrEmpty(deviceKey))
        {
            return false;
        }

        return RunOnStaThread(() =>
        {
            var endpoint = Enumerate(EDataFlow.All)
                .Where(e => String.Equals(e.Key, deviceKey, StringComparison.OrdinalIgnoreCase))
                .Select(e => (Endpoint?)e)
                .FirstOrDefault();

            if (endpoint is not { } match)
            {
                PluginLog.Warning($"No active audio device matches id: {deviceKey}");
                return false;
            }

            return SetDefaultEndpoint(match.EndpointId);
        });
    }

    public static Boolean CycleInDirection(EDataFlow flow, Int32 direction) =>
        RunOnStaThread(() =>
        {
            var ordered = Enumerate(flow)
                .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(e => e.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ordered.Count == 0)
            {
                PluginLog.Warning($"No active audio devices to cycle for flow: {flow}");
                return false;
            }

            var currentKey = GetDefaultEndpoint(flow)?.Key;
            var currentIndex = ordered.FindIndex(
                e => String.Equals(e.Key, currentKey, StringComparison.OrdinalIgnoreCase));
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var step = direction >= 0 ? 1 : -1;
            var nextIndex = ((currentIndex + step) % ordered.Count + ordered.Count) % ordered.Count;
            return SetDefaultEndpoint(ordered[nextIndex].EndpointId);
        });

    public static Boolean CycleToNext(EDataFlow flow) => CycleInDirection(flow, 1);

    private static List<Endpoint> Enumerate(EDataFlow flow)
    {
        List<Endpoint> devices = [];
        IMMDeviceEnumerator enumerator = null;
        IMMDeviceCollection collection = null;
        try
        {
            enumerator = (IMMDeviceEnumerator)CreateComObject(NativeMethods.CLSID_MMDeviceEnumerator);
            Marshal.ThrowExceptionForHR(enumerator.EnumAudioEndpoints(flow, NativeMethods.DEVICE_STATE_ACTIVE, out collection));
            Marshal.ThrowExceptionForHR(collection.GetCount(out var count));

            for (UInt32 i = 0; i < count; i++)
            {
                IMMDevice device = null;
                try
                {
                    Marshal.ThrowExceptionForHR(collection.Item(i, out device));
                    if (ReadEndpoint(device) is { } endpoint)
                    {
                        devices.Add(endpoint);
                    }
                }
                finally
                {
                    Release(device);
                }
            }
        }
        finally
        {
            Release(collection);
            Release(enumerator);
        }

        return devices;
    }

    private static Endpoint? GetDefaultEndpoint(EDataFlow flow)
    {
        IMMDeviceEnumerator enumerator = null;
        IMMDevice device = null;
        try
        {
            enumerator = (IMMDeviceEnumerator)CreateComObject(NativeMethods.CLSID_MMDeviceEnumerator);

            var hr = enumerator.GetDefaultAudioEndpoint(flow, ERole.Console, out device);
            if (hr == NativeMethods.E_NOTFOUND || device == null)
            {
                return null;
            }

            Marshal.ThrowExceptionForHR(hr);
            return ReadEndpoint(device);
        }
        finally
        {
            Release(device);
            Release(enumerator);
        }
    }

    private static Endpoint? ReadEndpoint(IMMDevice device)
    {
        Marshal.ThrowExceptionForHR(device.GetId(out var endpointId));

        var key = ExtractDeviceKey(endpointId);
        return key == null
            ? null
            : new Endpoint(endpointId, key, GetFriendlyName(device) ?? endpointId);
    }

    private static Boolean SetDefaultEndpoint(String endpointId)
    {
        IPolicyConfig policyConfig = null;
        try
        {
            policyConfig = (IPolicyConfig)CreateComObject(NativeMethods.CLSID_PolicyConfigClient);

            Marshal.ThrowExceptionForHR(policyConfig.SetDefaultEndpoint(endpointId, ERole.Console));
            Marshal.ThrowExceptionForHR(policyConfig.SetDefaultEndpoint(endpointId, ERole.Multimedia));
            Marshal.ThrowExceptionForHR(policyConfig.SetDefaultEndpoint(endpointId, ERole.Communications));
            return true;
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Failed to set default device: {ex.Message}");
            return false;
        }
        finally
        {
            Release(policyConfig);
        }
    }

    private static String GetFriendlyName(IMMDevice device)
    {
        IPropertyStore store = null;
        try
        {
            if (device.OpenPropertyStore(NativeMethods.STGM_READ, out store) != NativeMethods.S_OK)
            {
                return null;
            }

            var key = NativeMethods.PKEY_Device_FriendlyName;
            if (store.GetValue(ref key, out var value) != NativeMethods.S_OK)
            {
                return null;
            }

            try
            {
                return value.GetString();
            }
            finally
            {
                NativeMethods.PropVariantClear(ref value);
            }
        }
        finally
        {
            Release(store);
        }
    }

    private static String ExtractDeviceKey(String endpointId)
    {
        if (String.IsNullOrEmpty(endpointId))
        {
            return null;
        }

        var match = GuidPattern.Match(endpointId);
        return match.Success && Guid.TryParse(match.Value, out var guid)
            ? guid.ToString()
            : null;
    }

    private static Boolean RunOnStaThread(Func<Boolean> action)
    {
        var result = false;
        var thread = new Thread(() =>
        {
            try
            {
                result = action();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, $"Audio device operation failed: {ex.Message}");
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        return result;
    }

    private static Object CreateComObject(Guid clsid) => Activator.CreateInstance(Type.GetTypeFromCLSID(clsid));

    private static void Release(Object comObject)
    {
        if (comObject != null && Marshal.IsComObject(comObject))
        {
            Marshal.FinalReleaseComObject(comObject);
        }
    }
}

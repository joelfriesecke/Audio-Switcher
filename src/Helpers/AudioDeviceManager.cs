namespace Loupedeck.AudioSwitcherPlugin;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

internal readonly struct AudioDeviceInfo
{
    public AudioDeviceInfo(String id, String name)
    {
        this.Id = id;
        this.Name = name;
    }

    public String Id { get; }

    public String Name { get; }
}

internal static class AudioDeviceManager
{
    private static readonly Regex GuidPattern =
        new Regex(@"[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}", RegexOptions.Compiled);

    public static IReadOnlyList<AudioDeviceInfo> GetDevices(EDataFlow flow)
    {
        var result = new List<AudioDeviceInfo>();
        try
        {
            foreach (var device in Enumerate(flow))
            {
                result.Add(new AudioDeviceInfo(device.Key, device.Name));
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Failed to enumerate audio devices: {ex.Message}");
        }

        return result;
    }

    public static Boolean SetDefault(String deviceKey)
    {
        if (String.IsNullOrEmpty(deviceKey))
        {
            return false;
        }

        // IPolicyConfig.SetDefaultEndpoint is more reliable from an STA thread; the whole
        // operation (including enumeration) runs there to keep a single apartment.
        return RunOnStaThread(() =>
        {
            var endpointId = ResolveEndpointId(deviceKey);
            if (endpointId == null)
            {
                PluginLog.Warning($"No active audio device matches id: {deviceKey}");
                return false;
            }

            return SetDefaultEndpoint(endpointId);
        });
    }

    private static List<(String EndpointId, String Key, String Name)> Enumerate(EDataFlow flow)
    {
        var devices = new List<(String, String, String)>();
        IMMDeviceEnumerator enumerator = null;
        IMMDeviceCollection collection = null;
        try
        {
            enumerator = (IMMDeviceEnumerator)new MMDeviceEnumeratorComObject();
            Marshal.ThrowExceptionForHR(enumerator.EnumAudioEndpoints(flow, NativeMethods.DEVICE_STATE_ACTIVE, out collection));
            Marshal.ThrowExceptionForHR(collection.GetCount(out var count));

            for (UInt32 i = 0; i < count; i++)
            {
                IMMDevice device = null;
                try
                {
                    Marshal.ThrowExceptionForHR(collection.Item(i, out device));
                    Marshal.ThrowExceptionForHR(device.GetId(out var endpointId));

                    var key = ExtractDeviceKey(endpointId);
                    if (key == null)
                    {
                        continue;
                    }

                    var name = GetFriendlyName(device) ?? endpointId;
                    devices.Add((endpointId, key, name));
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

    private static String ResolveEndpointId(String deviceKey)
    {
        foreach (var device in Enumerate(EDataFlow.All))
        {
            if (String.Equals(device.Key, deviceKey, StringComparison.OrdinalIgnoreCase))
            {
                return device.EndpointId;
            }
        }

        return null;
    }

    private static Boolean SetDefaultEndpoint(String endpointId)
    {
        IPolicyConfig policyConfig = null;
        try
        {
            policyConfig = (IPolicyConfig)new PolicyConfigClientComObject();

            // Console + Multimedia set the default device, Communications the default comms device.
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

    // Mirrors AudioSwitcher's Device.Id (the GUID embedded in the endpoint string), so
    // actions configured with the old library keep resolving without re-configuration.
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

    private static void Release(Object comObject)
    {
        if (comObject != null && Marshal.IsComObject(comObject))
        {
            // These RCWs are created locally and never shared, so we hold the only
            // reference - release it fully rather than just decrementing the count.
            Marshal.FinalReleaseComObject(comObject);
        }
    }
}

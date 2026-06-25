namespace Loupedeck.AudioSwitcherPlugin;

using System;
using System.Runtime.InteropServices;

// vtable method order matches the IDL and is significant - do not reorder.

public enum EDataFlow
{
    Render = 0,
    Capture = 1,
    All = 2,
}

internal enum ERole
{
    Console = 0,
    Multimedia = 1,
    Communications = 2,
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct PropertyKey
{
    public Guid FormatId;
    public Int32 PropertyId;

    public PropertyKey(Guid formatId, Int32 propertyId)
    {
        this.FormatId = formatId;
        this.PropertyId = propertyId;
    }
}

// Size is pinned to the native 64-bit PROPVARIANT size; only VT_LPWSTR is read.
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal struct PropVariant
{
    private const UInt16 VtLpwstr = 31;

    [FieldOffset(0)]
    public UInt16 ValueType;

    [FieldOffset(8)]
    public IntPtr PointerValue;

    public readonly String GetString() =>
        this.ValueType == VtLpwstr && this.PointerValue != IntPtr.Zero
            ? Marshal.PtrToStringUni(this.PointerValue)
            : null;
}

[ComImport]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceEnumerator
{
    [PreserveSig]
    Int32 EnumAudioEndpoints(EDataFlow dataFlow, UInt32 stateMask, out IMMDeviceCollection devices);

    [PreserveSig]
    Int32 GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice endpoint);

    [PreserveSig]
    Int32 GetDevice([MarshalAs(UnmanagedType.LPWStr)] String id, out IMMDevice device);

    [PreserveSig]
    Int32 RegisterEndpointNotificationCallback(IntPtr client);

    [PreserveSig]
    Int32 UnregisterEndpointNotificationCallback(IntPtr client);
}

[ComImport]
[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceCollection
{
    [PreserveSig]
    Int32 GetCount(out UInt32 count);

    [PreserveSig]
    Int32 Item(UInt32 index, out IMMDevice device);
}

[ComImport]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDevice
{
    [PreserveSig]
    Int32 Activate(ref Guid iid, UInt32 clsCtx, IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out Object instance);

    [PreserveSig]
    Int32 OpenPropertyStore(UInt32 stgmAccess, out IPropertyStore properties);

    [PreserveSig]
    Int32 GetId([MarshalAs(UnmanagedType.LPWStr)] out String id);

    [PreserveSig]
    Int32 GetState(out UInt32 state);
}

[ComImport]
[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IPropertyStore
{
    [PreserveSig]
    Int32 GetCount(out UInt32 count);

    [PreserveSig]
    Int32 GetAt(UInt32 index, out PropertyKey key);

    [PreserveSig]
    Int32 GetValue(ref PropertyKey key, out PropVariant value);

    [PreserveSig]
    Int32 SetValue(ref PropertyKey key, ref PropVariant value);

    [PreserveSig]
    Int32 Commit();
}

// Undocumented but stable (Windows 7 - 11) interface used to set the default endpoint.
// The 10 leading methods are placeholders: only their vtable slot count matters so that
// SetDefaultEndpoint lands on the correct slot. They must never be called.
[ComImport]
[Guid("F8679F50-850A-41CF-9C72-430F290290C8")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IPolicyConfig
{
    [PreserveSig]
    Int32 GetMixFormat();

    [PreserveSig]
    Int32 GetDeviceFormat();

    [PreserveSig]
    Int32 ResetDeviceFormat();

    [PreserveSig]
    Int32 SetDeviceFormat();

    [PreserveSig]
    Int32 GetProcessingPeriod();

    [PreserveSig]
    Int32 SetProcessingPeriod();

    [PreserveSig]
    Int32 GetShareMode();

    [PreserveSig]
    Int32 SetShareMode();

    [PreserveSig]
    Int32 GetPropertyValue();

    [PreserveSig]
    Int32 SetPropertyValue();

    [PreserveSig]
    Int32 SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] String deviceId, ERole role);

    [PreserveSig]
    Int32 SetEndpointVisibility([MarshalAs(UnmanagedType.LPWStr)] String deviceId, Int32 visible);
}

[ComImport]
[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
internal class MMDeviceEnumeratorComObject
{
}

[ComImport]
[Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
internal class PolicyConfigClientComObject
{
}

internal static class NativeMethods
{
    public const UInt32 DEVICE_STATE_ACTIVE = 0x00000001;
    public const UInt32 STGM_READ = 0x00000000;
    public const Int32 S_OK = 0;
    public const Int32 E_NOTFOUND = unchecked((Int32)0x80070490);

    public static readonly PropertyKey PKEY_Device_FriendlyName =
        new PropertyKey(new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), 14);

    [DllImport("ole32.dll")]
    public static extern Int32 PropVariantClear(ref PropVariant value);
}

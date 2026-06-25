namespace Loupedeck.AudioSwitcherPlugin;

using System;

public abstract class AudioSwitchCommandBase : ActionEditorCommand
{
    private const String DeviceControlName = "Device";
    private const String DeviceDisplayName = "Select Device:";

    protected AudioSwitchCommandBase(String displayName, String description)
    {
        this.DisplayName = displayName;
        this.Description = description;
        this.GroupName = "Audio";

        this.ActionEditor.AddControlEx(
            new ActionEditorListbox(DeviceControlName, DeviceDisplayName));
        this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
    }

    protected abstract EDataFlow DataFlow { get; }

    private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
    {
        if (!e.ControlName.EqualsNoCase(DeviceControlName))
        {
            return;
        }

        try
        {
            foreach (var device in AudioDeviceManager.GetDevices(this.DataFlow))
            {
                e.AddItem(device.Key, device.Name, device.Name);
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Failed to list devices: {ex.Message}");
        }
    }

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        if (!actionParameters.TryGetString(DeviceControlName, out var deviceId))
        {
            return false;
        }

        try
        {
            if (AudioDeviceManager.SetDefault(deviceId))
            {
                return true;
            }

            PluginLog.Warning($"Device not found or switch failed: {deviceId}");
            return false;
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Switch failed: {ex.Message}");
            return false;
        }
    }
}

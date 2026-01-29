namespace Loupedeck.AudioSwitcherPlugin;

using System;
using System.Collections.Generic;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

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

    protected abstract IEnumerable<CoreAudioDevice> GetDevices(CoreAudioController controller);

    private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
    {
        if (!e.ControlName.EqualsNoCase(DeviceControlName))
        {
            return;
        }

        try
        {
            var controller = AudioSwitcherPlugin.Controller;
            if (controller == null)
            {
                PluginLog.Error("CoreAudioController is null during device listing.");
                return;
            }

            var devices = this.GetDevices(controller);
            if (devices == null)
            {
                return;
            }

            foreach (var device in devices)
            {
                e.AddItem(device.Id.ToString(), device.FullName, device.FullName);
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
            var controller = AudioSwitcherPlugin.Controller;
            if (controller == null)
            {
                PluginLog.Error("CoreAudioController is null during switch.");
                return false;
            }

            if (Guid.TryParse(deviceId, out var id))
            {
                var device = controller.GetDevice(id);
                if (device != null)
                {
                    device.SetAsDefault();
                    return true;
                }
            }
            
            PluginLog.Warning($"Device not found or invalid ID: {deviceId}");
            return false;
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Switch failed: {ex.Message}");
            return false;
        }
    }
}

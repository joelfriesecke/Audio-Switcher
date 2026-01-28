namespace Loupedeck.AudioSwitcherPlugin
{
    using System;
    using System.Collections.Generic;
    using AudioSwitcher.AudioApi;
    using AudioSwitcher.AudioApi.CoreAudio;

    public abstract class AudioSwitchCommandBase : ActionEditorCommand
    {
        private const string DeviceControlName = "Device";

        protected AudioSwitchCommandBase(string displayName, string description)
        {
            this.DisplayName = displayName;
            this.Description = description;
            this.GroupName = "Audio";

            this.ActionEditor.AddControlEx(
                new ActionEditorListbox(DeviceControlName, "Select Device:"));
            this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
        }

        protected abstract IEnumerable<CoreAudioDevice> GetDevices();

        private void OnListboxItemsRequested(object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (!e.ControlName.EqualsNoCase(DeviceControlName))
                return;

            try
            {
                var devices = this.GetDevices();
                foreach (var device in devices)
                {
                    e.AddItem(device.Id.ToString(), device.FullName, device.FullName);
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to list devices: {ex.Message}");
            }
        }

        protected override bool RunCommand(ActionEditorActionParameters actionParameters)
        {
            if (!actionParameters.TryGetString(DeviceControlName, out var deviceId))
                return false;

            if (AudioSwitcherPlugin.Instance.Controller == null)
            {
                PluginLog.Error("Audio controller is not initialized");
                return false;
            }

            try
            {
                if (Guid.TryParse(deviceId, out var id))
                {
                    var device = AudioSwitcherPlugin.Instance.Controller.GetDevice(id);
                    if (device != null)
                    {
                        device.SetAsDefault();
                        return true;
                    }
                }
                
                PluginLog.Warning($"Device not found: {deviceId}");
                return false;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Switch failed: {ex.Message}");
                return false;
            }
        }
    }
}

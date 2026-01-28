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

        protected abstract IEnumerable<CoreAudioDevice> GetDevices(CoreAudioController controller);

        private void OnListboxItemsRequested(object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (!e.ControlName.EqualsNoCase(DeviceControlName))
                return;

            try
            {
                using (var controller = new CoreAudioController())
                {
                    var devices = this.GetDevices(controller);
                    foreach (var device in devices)
                    {
                        e.AddItem(device.Id.ToString(), device.FullName, device.FullName);
                    }
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

            try
            {
                using (var controller = new CoreAudioController())
                {
                    if (Guid.TryParse(deviceId, out var id))
                    {
                        var device = controller.GetDevice(id);
                        if (device != null)
                        {
                            device.SetAsDefault();
                            return true;
                        }
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

namespace Loupedeck.AudioSwitcherPlugin
{
    using System;
    using System.Collections.Generic;
    using AudioSwitcher.AudioApi;
    using AudioSwitcher.AudioApi.CoreAudio;

    public abstract class AudioSwitchCommandBase : ActionEditorCommand
    {
        private const string DeviceControlName = "Device";

        private CoreAudioController _controller;
        private readonly Dictionary<string, CoreAudioDevice> _devices = new Dictionary<string, CoreAudioDevice>();

        protected AudioSwitchCommandBase(string displayName, string description)
        {
            this.DisplayName = displayName;
            this.Description = description;
            this.GroupName = "Audio";

            this.ActionEditor.AddControlEx(
                new ActionEditorListbox(DeviceControlName, "Select Device:"));
            this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;

            this.InitializeDevices();
        }

        protected abstract IEnumerable<CoreAudioDevice> GetDevices(CoreAudioController controller);

        private void InitializeDevices()
        {
            try
            {
                this._controller = new CoreAudioController();
                foreach (var device in this.GetDevices(this._controller))
                {
                    var id = device.Id.ToString();
                    this._devices[id] = device;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Init failed: {ex.Message}");
            }
        }

        private void OnListboxItemsRequested(object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (!e.ControlName.EqualsNoCase(DeviceControlName))
                return;

            foreach (var kvp in this._devices)
            {
                e.AddItem(kvp.Key, kvp.Value.FullName, kvp.Value.FullName);
            }
        }

        protected override bool RunCommand(ActionEditorActionParameters actionParameters)
        {
            if (!actionParameters.TryGetString(DeviceControlName, out var deviceId))
                return false;

            if (!this._devices.TryGetValue(deviceId, out var device))
                return false;

            try
            {
                device.SetAsDefault();
                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Switch failed: {ex.Message}");
                return false;
            }
        }
    }
}

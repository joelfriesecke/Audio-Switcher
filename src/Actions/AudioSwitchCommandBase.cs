namespace Loupedeck.AudioSwitcherPlugin
{
    using System;
    using System.Collections.Generic;
    using AudioSwitcher.AudioApi;
    using AudioSwitcher.AudioApi.CoreAudio;

    public abstract class AudioSwitchCommandBase : PluginDynamicCommand
    {
        private CoreAudioController _controller;
        private readonly Dictionary<String, CoreAudioDevice> _devices = new Dictionary<String, CoreAudioDevice>();

        protected AudioSwitchCommandBase(String displayName, String description) : base()
        {
            this.DisplayName = displayName;
            this.Description = description;
            this.GroupName = "Audio";

            this.InitializeDevices();
            this.MakeProfileAction("list;Select Device:");
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
                    this.AddParameter(id, device.FullName, "Devices");
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Init failed: {ex.Message}");
            }
        }

        protected override void RunCommand(String actionParameter)
        {
            if (String.IsNullOrEmpty(actionParameter) || !this._devices.TryGetValue(actionParameter, out var device))
                return;

            try
            {
                device.SetAsDefault();
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Switch failed: {ex.Message}");
            }
        }
    }
}

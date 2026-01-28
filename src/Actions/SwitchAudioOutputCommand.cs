namespace Loupedeck.AudioSwitcherPlugin
{
    using System.Collections.Generic;
    using AudioSwitcher.AudioApi;
    using AudioSwitcher.AudioApi.CoreAudio;

    public class SwitchAudioOutputCommand : AudioSwitchCommandBase
    {
        public SwitchAudioOutputCommand() : base("Audio Output", "Switch audio output device")
        {
        }

        protected override IEnumerable<CoreAudioDevice> GetDevices() => 
            AudioSwitcherPlugin.Instance.Controller.GetPlaybackDevices(DeviceState.Active);
    }
}

namespace Loupedeck.AudioSwitcherPlugin;

using System.Collections.Generic;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

public class SwitchAudioInputCommand : AudioSwitchCommandBase
{
    public SwitchAudioInputCommand() : base("Audio Input", "Switch audio input device")
    {
    }

    protected override IEnumerable<CoreAudioDevice> GetDevices(CoreAudioController controller) => 
        controller.GetCaptureDevices(DeviceState.Active);
}

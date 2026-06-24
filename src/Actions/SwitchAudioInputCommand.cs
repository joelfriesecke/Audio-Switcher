namespace Loupedeck.AudioSwitcherPlugin;

public class SwitchAudioInputCommand : AudioSwitchCommandBase
{
    public SwitchAudioInputCommand() : base("Audio Input", "Switch audio input device")
    {
    }

    protected override EDataFlow DataFlow => EDataFlow.Capture;
}

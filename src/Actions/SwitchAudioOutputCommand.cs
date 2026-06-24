namespace Loupedeck.AudioSwitcherPlugin;

public class SwitchAudioOutputCommand : AudioSwitchCommandBase
{
    public SwitchAudioOutputCommand() : base("Audio Output", "Switch audio output device")
    {
    }

    protected override EDataFlow DataFlow => EDataFlow.Render;
}

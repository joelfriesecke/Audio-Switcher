namespace Loupedeck.AudioSwitcherPlugin;

public class CycleAudioOutputCommand : CycleAudioCommandBase
{
    public CycleAudioOutputCommand() : base("Cycle Audio Output", "Switch to the next audio output device")
    {
    }

    protected override EDataFlow DataFlow => EDataFlow.Render;
}

namespace Loupedeck.AudioSwitcherPlugin;

public class CycleAudioInputCommand : CycleAudioCommandBase
{
    public CycleAudioInputCommand() : base("Cycle Audio Input", "Switch to the next audio input device")
    {
    }

    protected override EDataFlow DataFlow => EDataFlow.Capture;
}

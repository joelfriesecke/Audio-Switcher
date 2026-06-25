namespace Loupedeck.AudioSwitcherPlugin;

public class AudioInputAdjustment : AudioDeviceAdjustmentBase
{
    public AudioInputAdjustment() : base("Audio Input (Dial)", "Cycle audio input device with a dial")
    {
    }

    protected override EDataFlow DataFlow => EDataFlow.Capture;
}

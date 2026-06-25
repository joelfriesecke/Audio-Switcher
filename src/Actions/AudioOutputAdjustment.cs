namespace Loupedeck.AudioSwitcherPlugin;

public class AudioOutputAdjustment : AudioDeviceAdjustmentBase
{
    public AudioOutputAdjustment() : base("Audio Output (Dial)", "Cycle audio output device with a dial")
    {
    }

    protected override EDataFlow DataFlow => EDataFlow.Render;
}

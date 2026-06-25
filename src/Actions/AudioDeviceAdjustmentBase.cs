namespace Loupedeck.AudioSwitcherPlugin;

using System;

public abstract class AudioDeviceAdjustmentBase : PluginDynamicAdjustment
{
    protected AudioDeviceAdjustmentBase(String displayName, String description)
        : base(displayName, description, "Audio", hasReset: false)
    {
    }

    protected abstract EDataFlow DataFlow { get; }

    protected override void ApplyAdjustment(String actionParameter, Int32 diff)
    {
        if (diff == 0)
        {
            return;
        }

        try
        {
            AudioDeviceManager.CycleInDirection(this.DataFlow, diff);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Audio device dial adjustment failed: {ex.Message}");
        }

        this.AdjustmentValueChanged();
    }

    protected override String GetAdjustmentValue(String actionParameter)
    {
        var current = AudioDeviceManager.GetDefaultDevice(this.DataFlow);
        return current?.Name ?? "—";
    }

    protected override void RunCommand(String actionParameter)
    {
    }
}

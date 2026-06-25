namespace Loupedeck.AudioSwitcherPlugin;

using System;

public abstract class CycleAudioCommandBase : PluginDynamicCommand
{
    protected CycleAudioCommandBase(String displayName, String description)
        : base(displayName, description, "Audio")
    {
    }

    protected abstract EDataFlow DataFlow { get; }

    protected override void RunCommand(String actionParameter)
    {
        try
        {
            if (!AudioDeviceManager.CycleToNext(this.DataFlow))
            {
                PluginLog.Warning("Cycle audio device failed or no devices available.");
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, $"Cycle audio device failed: {ex.Message}");
        }

        this.ActionImageChanged();
    }

    protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
    {
        var current = AudioDeviceManager.GetDefaultDevice(this.DataFlow);
        return current?.Name ?? base.GetCommandDisplayName(actionParameter, imageSize);
    }
}

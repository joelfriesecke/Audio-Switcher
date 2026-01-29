namespace Loupedeck.AudioSwitcherPlugin;

using System;
using AudioSwitcher.AudioApi.CoreAudio;

public class AudioSwitcherPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;
    public override Boolean HasNoApplication => true;

    private static CoreAudioController _controller;

    public static CoreAudioController Controller => _controller ??= new CoreAudioController();

    public static void InvalidateController()
    {
        try
        {
            _controller?.Dispose();
        }
        catch
        {
        }
        _controller = null;
    }

    public AudioSwitcherPlugin()
    {
        PluginLog.Init(this.Log);
    }

    public override void Load()
    {
        var _ = Controller;
    }

    public override void Unload()
    {
        InvalidateController();
    }
}

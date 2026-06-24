namespace Loupedeck.AudioSwitcherPlugin;

public class AudioSwitcherPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;
    public override Boolean HasNoApplication => true;

    public AudioSwitcherPlugin()
    {
        PluginLog.Init(this.Log);
    }
}

namespace Loupedeck.AudioSwitcherPlugin
{
    using System;

    public class AudioSwitcherPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;
        public override Boolean HasNoApplication => true;

        public AudioSwitcherPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }
    }
}


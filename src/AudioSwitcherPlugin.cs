namespace Loupedeck.AudioSwitcherPlugin
{
    using System;
    using AudioSwitcher.AudioApi.CoreAudio;

    public class AudioSwitcherPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;
        public override Boolean HasNoApplication => true;

        public static AudioSwitcherPlugin Instance { get; private set; }

        public CoreAudioController Controller { get; private set; }

        public AudioSwitcherPlugin()
        {
            Instance = this;
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
            try
            {
                this.Controller = new CoreAudioController();
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to initialize CoreAudioController: {ex.Message}");
            }
        }

        public override void Unload()
        {
            this.Controller?.Dispose();
            this.Controller = null;
        }
    }
}


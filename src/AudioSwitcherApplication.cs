namespace Loupedeck.AudioSwitcherPlugin
{
    using System;

    public class AudioSwitcherApplication : ClientApplication
    {
        protected override String GetProcessName() => string.Empty;

        protected override String GetBundleName() => string.Empty;

        public override ClientApplicationStatus GetApplicationStatus() => ClientApplicationStatus.Unknown;
    }
}

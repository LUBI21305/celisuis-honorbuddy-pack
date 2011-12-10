using System.IO;
using Styx;
using Styx.Helpers;

namespace GuildInviter2
{
    public class GISettings : Settings
    {
        public static readonly GISettings Instance = new GISettings();

        public GISettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"Plugins/GuildInviter2/GI2-S-{0}.xml", StyxWoW.Me.Name)))
        {
        }

        [Setting, DefaultValue("Hey there, please join my guild!")]
        public string whispertext { get; set; }

        [Setting, DefaultValue(1)]
        public int MinLevel { get; set; }

        [Setting, DefaultValue(85)]
        public int MaxLevel { get; set; }
    }
}
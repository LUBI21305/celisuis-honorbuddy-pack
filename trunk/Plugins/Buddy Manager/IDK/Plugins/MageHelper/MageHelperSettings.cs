using System.IO;
using Styx;
using Styx.Helpers;

namespace MageHelper
{
    public class MageHelperSettings : Settings
    {
        public static readonly MageHelperSettings Instance = new MageHelperSettings();

        public MageHelperSettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"Settings/MageHelper/MageHelper-Settings-{0}.xml", StyxWoW.Me.Name)))
        {
        }

        [Setting, DefaultValue(true)]
        public bool UseBlink { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseSlowFall { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseInvis { get; set; }

        [Setting, DefaultValue(3)]
        public int InvisAdds { get; set; }

        [Setting, DefaultValue(20)]
        public int InvisHP { get; set; }

        }

    }



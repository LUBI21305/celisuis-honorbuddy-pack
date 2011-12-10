using System.IO;
using Styx;
using Styx.Helpers;

namespace HazzDruid
{
    public class HazzDruidSettings : Settings
    {
        public static readonly HazzDruidSettings Instance = new HazzDruidSettings();

        public HazzDruidSettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"CustomClasses/HazzDruid/Config/HazzDruid-Settings-{0}.xml", StyxWoW.Me.Name)))
        {
        }

        [Setting, DefaultValue(false)]
        public bool UseTree { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseRebirth { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseRevive { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseRemoveCurse { get; set; }

        [Setting, DefaultValue(65)]
        public int SwiftmendPercent { get; set; }

        [Setting, DefaultValue(50)]
        public int RegrowthPercent { get; set; }

        [Setting, DefaultValue(95)]
        public int RejuvenationPercent { get; set; }

        [Setting, DefaultValue(00)]
        public int TranquilityPercent { get; set; }

        [Setting, DefaultValue(55)]
        public int HealingTouchPercent { get; set; }

        [Setting, DefaultValue(50)]
        public int WildGrowthPercent { get; set; }

        [Setting, DefaultValue(00)]
        public int NaturesPercent { get; set; }
		
		[Setting, DefaultValue(3)]
        public int LifebloomPercent { get; set; }

        [Setting, DefaultValue(0)]
        public int ManaPercent { get; set; }

        [Setting, DefaultValue(0)]
        public int HealthPercent { get; set; }


    }
}
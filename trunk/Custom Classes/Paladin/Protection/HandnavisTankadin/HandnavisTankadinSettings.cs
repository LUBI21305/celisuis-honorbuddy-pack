using System.IO;
using Styx;
using Styx.Helpers;

namespace HandnavisTankadin
{
    public class HandnavisTankadinSettings : Settings
    {
        public static readonly HandnavisTankadinSettings Instance = new HandnavisTankadinSettings();

        public HandnavisTankadinSettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"CustomClasses/Config/HandnavisTankadin-Settings-{0}.xml", StyxWoW.Me.Name)))
        {
        }

	[Setting, DefaultValue(0)]
        public int Seal { get; set; }


	[Setting, DefaultValue(0)]
        public int Movement { get; set; }

	[Setting, DefaultValue(0)]
        public int Faceing { get; set; }




       [Setting, DefaultValue(true)]
        public bool UseTaunt { get; set; }

       [Setting, DefaultValue(false)]
       public bool UseAura { get; set; }


	[Setting, DefaultValue(false)]
        public bool UseLay { get; set; }



	[Setting, DefaultValue(false)]
        public bool DivinePleaCD { get; set; }


	[Setting, DefaultValue(false)]
        public bool UseExo { get; set; }



        [Setting, DefaultValue(15)]
        public int LayonHandsPercent { get; set; }

	[Setting, DefaultValue(30)]
        public int DivinePleaPercent { get; set; }

	[Setting, DefaultValue(30)]
        public int GuardianoftheancientKingsPercent { get; set; }


[Setting, DefaultValue(20)]
        public int ArdentDefenderPercent { get; set; }


[Setting, DefaultValue(50)]
        public int DivineProtectionPercent { get; set; }


[Setting, DefaultValue(90)]
        public int HolyShieldPercent { get; set; }



[Setting, DefaultValue(30)]
        public int ManaPercent { get; set; }
[Setting, DefaultValue(30)]
        public int HealthPercent { get; set; }

    }
}
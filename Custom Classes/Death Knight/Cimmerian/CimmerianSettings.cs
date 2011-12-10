using System.IO;
using Styx;
using Styx.Helpers;

namespace Cimmerian
{
    public class CimmerianSettings : Styx.Helpers.Settings
    {
        public static readonly CimmerianSettings Instance = new CimmerianSettings();

        public CimmerianSettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"CustomClasses/Cimmerian/CimmerianSettings_{0}.xml", StyxWoW.Me.Name)))
        {
        }

        #region Pull

        [Setting, DefaultValue(true)]
        public bool OpenWithDeathGrip { get; set; }

        [Setting, DefaultValue(false)]
        public bool OpenWithIcyTouch { get; set; }

        [Setting, DefaultValue(false)]
        public bool OpenWithDarkCommand { get; set; }

        [Setting, DefaultValue(20)]
        public int IcyTouchRange { get; set; }

        [Setting, DefaultValue(true)]
        public bool OpenWithIcyTouchBackup { get; set; }

        [Setting, DefaultValue(false)]
        public bool AlertPlayers { get; set; }

        [Setting, DefaultValue(false)]
        public bool PlayerAlert { get; set; }

        [Setting, DefaultValue(20)]
        public int PlayerDetectorRange { get; set; }

        [Setting, DefaultValue(false)]
        public bool AlertPlayersLog { get; set; }
        
        #endregion

        #region Blood

        [Setting, DefaultValue(false)]
        public bool UseVampiricBlood { get; set; }

        [Setting, DefaultValue(false)]
        public bool VampiricBloodAdds { get; set; }

        [Setting, DefaultValue(0)]
        public int VampiricBloodHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseDrw { get; set; }

        [Setting, DefaultValue(false)]
        public bool DrwAdds { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseBoneShield { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseRuneTap { get; set; }

        [Setting, DefaultValue(0)]
        public int RuneTapHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseBloodTap { get; set; }

        #endregion

        #region Frost

        [Setting, DefaultValue(false)]
        public bool UseIceboundFortitude { get; set; }

        [Setting, DefaultValue(false)]
        public bool IceboundFortitudeAdds { get; set; }

        [Setting, DefaultValue(0)]
        public int IceboundFortitudeHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UsePillar { get; set; }

        [Setting, DefaultValue(0)]
        public int PillarHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool PillarAdds { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseErw { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseErwRunes { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseErwAdds { get; set; }

        [Setting, DefaultValue(false)]
        public bool RimeIcyTouch { get; set; }

        [Setting, DefaultValue(false)]
        public bool RimeHb { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseRuneStrike { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseHc { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseHowlingBlast { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseLichborne { get; set; }

        [Setting, DefaultValue(30)]
        public int LichbornHealth { get; set; }

        #endregion

        #region Unholy

        [Setting, DefaultValue(false)]
        public bool UseHorn { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseOutbreak { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseSummonGargoyle { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseDarkTransformation { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseUnholyFrenzy { get; set; }
        
        #endregion

        #region Pet

        [Setting, DefaultValue(false)]
        public bool UseRaiseDead { get; set; }

        [Setting, DefaultValue(0)]
        public int DeathPactHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseDeathPact { get; set; }

        #endregion

        #region Racials

        [Setting, DefaultValue(false)]
        public bool UseAt { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseNaaru { get; set; }

        [Setting, DefaultValue(0)]
        public int NaaruHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseStoneForm { get; set; }

        [Setting, DefaultValue(0)]
        public int StoneFormHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseEm { get; set; }

        [Setting, DefaultValue(false)]
        public bool NaaruAdds { get; set; }

        [Setting, DefaultValue(false)]
        public bool SfAdds { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseWarStomp { get; set; }

        [Setting, DefaultValue(0)]
        public int WarStompHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool WarStompAdds { get; set; }

        [Setting, DefaultValue(false)]
        public bool WarStompCasters { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseLifeBlood { get; set; }

        [Setting, DefaultValue(false)]
        public bool LifeBloodAdds { get; set; }

        [Setting, DefaultValue(0)]
        public int LifebloodHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseBloodFury { get; set; }

        [Setting, DefaultValue(false)]
        public bool BloodFuryAdds { get; set; }

        [Setting, DefaultValue(0)]
        public int BloodFuryHealth { get; set; }

        #endregion

        #region Misc

        [Setting, DefaultValue(50)]
        public int RestHealth { get; set; }

        [Setting, DefaultValue(false)]
        public bool UsePoF { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseStrangulate { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseStrangulateMelee { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseMindFreeze { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseDeathGripInterupt { get; set; }

        [Setting, DefaultValue(false)]
        public bool IgnoreAdds { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseAntiMagicShell { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseChainsOfIce { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseDeathGripRunners { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseDarkCommandRunners { get; set; }

        [Setting, DefaultValue(2)]
        public int AddsCount { get; set; }

        [Setting, DefaultValue(20)]
        public int CooldownHealth { get; set; }

        [Setting, DefaultValue(0)]
        public int UseDsssHealth { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseBloodPresence { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseFrostPresence { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseUnholyPresence { get; set; }

        #endregion

    }
}

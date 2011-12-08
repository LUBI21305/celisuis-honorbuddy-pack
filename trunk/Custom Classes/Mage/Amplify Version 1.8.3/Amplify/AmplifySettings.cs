using System.IO;
using Styx;
using Styx.Helpers;

namespace Amplify
{
    public class AmplifySettings : Settings
    {
        public static readonly AmplifySettings Instance = new AmplifySettings();

        public AmplifySettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"CustomClasses/Config/Amplify-Settings-{0}.xml", StyxWoW.Me.Name)))
        {
        }

        #region Rest


        [Setting, DefaultValue(40)]
        public int RestHealthPercentage { get; set; }

        [Setting, DefaultValue(40)]
        public int RestManaPercentage { get; set; }
        //0 for auto, 1 for Molten, 2 for Frost, 3 for Mage
        [Setting, DefaultValue("Auto")]
        public string ArmorSelect { get; set; }

        [Setting, DefaultValue(false)]
        public bool Use_Wand { get; set; }

        [Setting, DefaultValue(false)]
        public bool FrostElemental { get; set; }

        [Setting, DefaultValue(false)]
        public bool Freeze { get; set; }
        #endregion


        #region Spells

        [Setting, DefaultValue(true)]
        public bool Use_FireBlast { get; set; }

        [Setting, DefaultValue(30)]
        public int FireBlast_Hp_Percent { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_Fireball_Low { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_Frostbolt_Low { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_ProcArcaneMissles_Low { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_ProcArcaneMissles { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_FrostNova_Low { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_CounterSpell { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_CounterSpellArcane { get; set; }
        #endregion


        #region Spells

        [Setting, DefaultValue(true)]
        public bool Use_ManaShield { get; set; }

        [Setting, DefaultValue("Always")]
        public string ManaShieldArcane { get; set; }

        [Setting, DefaultValue(30)]
        public int ManaShield_Hp_Percent { get; set; }

        [Setting, DefaultValue(true)]
        public bool Pull_ProcArcaneMissles { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_FrostNova { get; set; }

        [Setting, DefaultValue(30)]
        public int FrostNova_Mob_Hp_Above { get; set; }

        [Setting, DefaultValue(true)] 
        public bool Use_Polymorph { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_Evocation { get; set; }

        [Setting, DefaultValue(30)]
        public int Evocation_MP_Percent { get; set; }

        [Setting, DefaultValue(30)]
        public int Evocation_HP_Percent { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_IceBlock { get; set; }

        [Setting, DefaultValue(30)]
        public int IceBlock_hP_Percent { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_ManaGems{ get; set; }

        [Setting, DefaultValue(30)]
        public int ManaGems_MP_Percent { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_FireballOnBF { get; set; }

        [Setting, DefaultValue("Blizzard")]
        public string AoE { get; set; }

        [Setting, DefaultValue(3)]
        public int BlizzardwhenXAdds { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_MirrorImage { get; set; }

        [Setting, DefaultValue(3)]
        public int MirrorImagewhenXAdds { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_FrostFireboltOnBF { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_DFIceLanceOnFoF { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_ColdSnap { get; set; }
        #endregion

        [Setting, DefaultValue(true)]
        public bool IceBarrierBeforePull { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_IceBarrier { get; set; }

        [Setting, DefaultValue(60)]
        public int IceBarrierWhenBelow_Hp_Percent { get; set; }

        [Setting, DefaultValue(30)]
        public int ManaPotPercent { get; set; }

        [Setting, DefaultValue(20)]
        public int HealthPotPercent { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_ArcaneBarrage { get; set; }

        [Setting, DefaultValue(30)]
        public int ArcaneBarrage_Hp_Percent { get; set; }

        [Setting, DefaultValue(3)]
        public int ArcaneBlastStacks { get; set; }

        [Setting, DefaultValue("Always")]
        public string PresenceofMind { get; set; }

        [Setting, DefaultValue("Automatic")]
        public string FrostNova { get; set; }

        [Setting, DefaultValue("On")]
        public string Spellsteal { get; set; }

        [Setting, DefaultValue(true)]
        public bool Use_IcyVeins { get; set; }

        [Setting, DefaultValue(3)]
        public int IcyVeinswhenXAdds { get; set; }

        [Setting, DefaultValue("Frostbolt")]
        public string FrostSpamSpell { get; set; }
        

        [Setting, DefaultValue("Fireball")]
        public string FireSpamSpell { get; set; }

        [Setting, DefaultValue(true)]
        public bool FFS_Enable { get; set; }

        [Setting, DefaultValue("Frostbolt")]
        public string PullSpellSelect { get; set; }

        [Setting, DefaultValue("Never")]
        public string IcyVeinSettings { get; set; }

        [Setting, DefaultValue("Never")]
        public string TimeWarp { get; set; }

        [Setting, DefaultValue("Always")]
        public string MirrorImage { get; set; }

        [Setting, DefaultValue("Never")]
        public string FlameOrbSelection { get; set; }

        [Setting, DefaultValue("Fire Blast")]
        public string FrostFinisher { get; set; }

        [Setting, DefaultValue(true)]
        public bool CriticalMass { get; set; }

        [Setting, DefaultValue(true)]
        public bool LivingBomb { get; set; }

        [Setting, DefaultValue(true)]
        public bool BuffParty { get; set; }

        [Setting, DefaultValue(true)]
        public bool DeCurseParty { get; set; }
        
        [Setting, DefaultValue(false)]
        public bool IsConfigured { get; set; }

        [Setting, DefaultValue(false)]
        public bool MoveDisable { get; set; }
    }
}

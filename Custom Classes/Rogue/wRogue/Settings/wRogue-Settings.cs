using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Styx;
using Styx.Helpers;
using Styx.Combat.CombatRoutine;

namespace wRogue
{
    public class SSSettings : Settings
    {
        public static readonly SSSettings Instance = new SSSettings();

        public SSSettings()
            : base(Logging.ApplicationPath + "\\Settings\\SSSettings_" + StyxWoW.Me.Name + ".xml")
        {
            Load();
        }

        ~SSSettings()
        {
            Save();
        }

        [Setting("UseRest", "Rest when below the set hit points amount"), DefaultValue(true)]
        public bool UseRest { get; set; }

        [Setting("restHealth", "Percentage of health to rest at."), DefaultValue(55)]
        public int restHealth { get; set; }

        [Setting("UsePoisons", "Use poisons when needed"), DefaultValue(true)]
        public bool UsePoisons { get; set; }

        [Setting("poisonToMain", "Poison Mainhand - 1 for Instance - 2 for Deadly - 3 for Crippling - 4 for Wound"), DefaultValue(1)]
        public int poisonToMain { get; set; }

        [Setting("poisonToOff", "Poison Offhand - 1 for Instance - 2 for Deadly - 3 for Crippling - 4 for Wound"), DefaultValue(2)]
        public int poisonToOff { get; set; }

        [Setting("PullType", "1 for Cheap Shot - 2 for Ambush - 3 for Garrote - 4 for Melee - 5 for Throwing"), DefaultValue(2)]
        public int PullType { get; set; }

        [Setting("UseStealthToPull", "Use stealth when able before pulling."), DefaultValue(true)]
        public bool UseStealthToPull { get; set; }

        [Setting("UseSprintPull", "Use sprint when able before pulling."), DefaultValue(true)]
        public bool UseSprintPull { get; set; }

        [Setting("UseDistract", "Use distract when able before pulling."), DefaultValue(true)]
        public bool UseDistract { get; set; }

        [Setting("UsePickPocket", "Use pick pocket when able before pulling."), DefaultValue(true)]
        public bool UsePickPocket { get; set; }

        [Setting("UseStealthAlways", "Use stealth when ever able. Useful for Battlegrounds."), DefaultValue(false)]
        public bool UseStealthAlways { get; set; }

        [Setting("UseLifeblood", "Use Lifeblood when hurt."), DefaultValue(true)]
        public bool UseLifeblood { get; set; }

        [Setting("UsePremeditation", "(Subtlety) Use Premeditation to pull."), DefaultValue(true)]
        public bool UsePremeditation { get; set; }

        [Setting("UsePreparation", "(Subtlety) Use Preparation to reset cooldowns."), DefaultValue(true)]
        public bool UsePreparation { get; set; }

        [Setting("UseShadowDance", "(Subtlety) Used in a special attack."), DefaultValue(true)]
        public bool UseShadowDance { get; set; }

        [Setting("UseAdrenalineRush", "(Combat) Used on adds."), DefaultValue(true)]
        public bool UseAdrenalineRush { get; set; }

        [Setting("UseKillingSpree", "(Combat) Used on adds."), DefaultValue(true)]
        public bool UseKillingSpree { get; set; }

        [Setting("UseColdBlood", "(Assassination) Used before offensive abilities."), DefaultValue(true)]
        public bool UseColdBlood { get; set; }

        [Setting("UseVendetta", "(Assassination) Used whenever available during combat."), DefaultValue(true)]
        public bool UseVendetta { get; set; }

        [Setting("UseLifeBloodAt", "Use Lifeblood at this percent of hit points."), DefaultValue(50)]
        public int UseLifebloodAt { get; set; }

        [Setting("UseBandages", "Use bandages when hurt in combat."), DefaultValue(true)]
        public bool UseBandages { get; set; }

        [Setting("UseBandageAt", "Use bandages when hit points fall below this amount"), DefaultValue(30)]
        public int UseBandagesAt { get; set; }

        [Setting("DontWasteBandage", "Don't use bandages if target is below this percent of hit points"), DefaultValue(20)]
        public int DontWasteBandage { get; set; }

        [Setting("UseHealthPotions", "Use health potions when hurt in combat"), DefaultValue(true)]
        public bool UseHealthPotions { get; set; }

        [Setting("UseHealthPotionsAt", "Use health potions when hit points fall below this amount"), DefaultValue(15)]
        public int UseHealthPotionsAt { get; set; }

        [Setting("DontWastePotion", "Don't use potions when target health below this amount"), DefaultValue(10)]
        public int DontWastePotion { get; set; }

        [Setting("UseSliceAndDice", "Use Slice and Dice for haste when needed."), DefaultValue(true)]
        public bool UseSliceAndDice { get; set; }

        [Setting("UseRecuperate", "Use recuperate to heal when needed."), DefaultValue(true)]
        public bool UseRecuperate { get; set; }

        [Setting("UseBladeFlurry", "Use blade flurry on adds."), DefaultValue(true)]
        public bool UseBladeFlurry { get; set; }

        [Setting("UseFanOfKnives", "Use fan of knives on adds."), DefaultValue(true)]
        public bool UseFanOfKnives { get; set; }

        [Setting("UseAddManagement", "Use blind and gouge to manage adds."), DefaultValue(true)]
        public bool UseAddManagement { get; set; }

        [Setting("UseRedirect", "Use redirect to transfer combo points to new target when needed."), DefaultValue(true)]
        public bool UseRedirect { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Styx.Helpers;

namespace Amplify
{
    public partial class AmpConfig : Form
    {
        public AmpConfig()
        {
            InitializeComponent();
        }

        private void AmpConfig_Load(object sender, EventArgs e)
        {
            Logging.Write("Settings Panel Opened");
            AmplifySettings.Instance.Load();
            propertyGrid1.SelectedObject = AmplifySettings.Instance;
            FrostSpamSpell.SelectedItem = AmplifySettings.Instance.FrostSpamSpell;
            Use_CounterSpellArcane.Checked = AmplifySettings.Instance.Use_CounterSpellArcane;
            Use_Evocation.Checked = AmplifySettings.Instance.Use_Evocation;
            PullSpellSelect.SelectedItem = AmplifySettings.Instance.PullSpellSelect;
            FrostNova.SelectedItem = AmplifySettings.Instance.FrostNova;
            AoE.SelectedItem = AmplifySettings.Instance.AoE;
            FireSpamSpell.SelectedItem = AmplifySettings.Instance.FireSpamSpell;
            IcyVeinSettings.SelectedItem = AmplifySettings.Instance.IcyVeinSettings;
            FlameOrbSelection.SelectedItem = AmplifySettings.Instance.FlameOrbSelection;
            PresenceofMind.SelectedItem = AmplifySettings.Instance.PresenceofMind;
            Evocation_MP_Percent.Value = new decimal(AmplifySettings.Instance.Evocation_MP_Percent);
            Evocation_HP_Percent.Value = new decimal(AmplifySettings.Instance.Evocation_HP_Percent);
            RestManaPercentage.Value = new decimal(AmplifySettings.Instance.RestManaPercentage);
            RestHealthPercentage.Value = new decimal(AmplifySettings.Instance.RestHealthPercentage);
            ArmorSelect.SelectedItem = AmplifySettings.Instance.ArmorSelect;
            ManaShield_Hp_Percent.Value = new decimal(AmplifySettings.Instance.ManaShield_Hp_Percent);
            IceBarrierWhenBelow_Hp_Percent.Value = new decimal(AmplifySettings.Instance.IceBarrierWhenBelow_Hp_Percent);
            IceBlock_hP_Percent.Value = new decimal(AmplifySettings.Instance.IceBlock_hP_Percent);
            FrostFinisher.SelectedItem = AmplifySettings.Instance.FrostFinisher;
            MirrorImage.SelectedItem = AmplifySettings.Instance.MirrorImage;
            TimeWarp.SelectedItem = AmplifySettings.Instance.TimeWarp;
            Spellsteal.SelectedItem = AmplifySettings.Instance.Spellsteal;
            Use_ManaShield.Checked = AmplifySettings.Instance.Use_ManaShield;
            Use_IceBarrier.Checked = AmplifySettings.Instance.Use_IceBarrier;
            Use_IceBlock.Checked = AmplifySettings.Instance.Use_IceBlock;
            Use_Wand.Checked = AmplifySettings.Instance.Use_Wand;
            CriticalMass.Checked = AmplifySettings.Instance.CriticalMass;
            LivingBomb.Checked = AmplifySettings.Instance.LivingBomb;
            Freeze.Checked = AmplifySettings.Instance.Freeze;
            FrostElemental.Checked = AmplifySettings.Instance.FrostElemental;

            MoveDisable.Checked = AmplifySettings.Instance.MoveDisable;

            HealthPotPercent.Value = new decimal(AmplifySettings.Instance.HealthPotPercent);
            ManaPotPercent.Value = new decimal(AmplifySettings.Instance.ManaPotPercent);

            BuffParty.Checked = AmplifySettings.Instance.BuffParty;
            DeCurseParty.Checked = AmplifySettings.Instance.DeCurseParty;
            ArcaneBlastStacks.Value = new decimal(AmplifySettings.Instance.ArcaneBlastStacks); 
        }
        
        private void save_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "Are you sure you want to save settings? if Settings Still do not take effect, you may need to restart honorbuddy.",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (dr == DialogResult.No)
            {
                Logging.Write(Color.Red,
                                    "Really? Not saving settings? thats too bad. ");
            }
            if (dr == DialogResult.Yes)
            {
                
                AmplifySettings.Instance.Save();
                AmplifySettings.Instance.Load();
                if (AmplifySettings.Instance.IsConfigured == false)
                {
                    Logging.Write(Color.Red,
                                  "Thank you for Configuring Amplify, If You Experiance Issues Such as Honorbuddy not Pulling Please Restart your Instance of Honorbuddy to allow the Settings to Take Effect.");
                    AmplifySettings.Instance.IsConfigured = true;
                    Logging.Write(Color.Blue,
                                  "I have walked the edge of the abyss. I have seen your future. And I have learned!");
                    AmplifySettings.Instance.Save();
                }
                else
                {
                    Logging.Write(Color.Red,
                                    "Some Settings May require you to restart honorbuddy to take effect.");
                }
            }
        }


        private void IcyVeinSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.IcyVeinSettings = IcyVeinSettings.SelectedItem.ToString();
        }

        private void FlameOrbSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.FlameOrbSelection = FlameOrbSelection.SelectedItem.ToString();
        }

        private void PresenceofMind_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.PresenceofMind = PresenceofMind.SelectedItem.ToString();
        }

        private void Use_CounterSpellArcane_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Use_CounterSpellArcane = Use_CounterSpellArcane.Checked;
        }

        private void Use_Evocation_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Use_Evocation = Use_Evocation.Checked;
        }

        private void Evocation_HP_Percent_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Evocation_HP_Percent = int.Parse(Evocation_HP_Percent.Value.ToString());
        }

        private void Evocation_MP_Percent_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Evocation_MP_Percent = int.Parse(Evocation_MP_Percent.Value.ToString());
        }

        private void RestHealthPercentage_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.RestHealthPercentage = int.Parse(RestHealthPercentage.Value.ToString());
        }

        private void RestManaPercentage_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.RestManaPercentage = int.Parse(RestManaPercentage.Value.ToString());
        }

        private void ArmorSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.ArmorSelect = ArmorSelect.SelectedItem.ToString();
        }

        private void Use_ManaShield_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Use_ManaShield = Use_ManaShield.Checked;
        }

        private void ManaShield_Hp_Percent_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.ManaShield_Hp_Percent = int.Parse(ManaShield_Hp_Percent.Value.ToString());
        }

        private void Use_IceBarrier_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Use_IceBarrier = Use_IceBarrier.Checked;
        }

        private void IceBarrierWhenBelow_Hp_Percent_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.IceBarrierWhenBelow_Hp_Percent = int.Parse(IceBarrierWhenBelow_Hp_Percent.Value.ToString());
        }

        private void Use_IceBlock_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Use_IceBlock = Use_IceBlock.Checked;
        }

        private void IceBlock_hP_Percent_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.IceBlock_hP_Percent = int.Parse(IceBlock_hP_Percent.Value.ToString());
        }

        private void Use_Wand_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Use_Wand = Use_Wand.Checked;
        }


        private void FrostFinisher_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.FrostFinisher = FrostFinisher.SelectedItem.ToString();
        }

        private void MirrorImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.MirrorImage = MirrorImage.SelectedItem.ToString();
        }

        private void PullSpellSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.PullSpellSelect = PullSpellSelect.SelectedItem.ToString();
        }

        private void FrostSpamSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.FrostSpamSpell = FrostSpamSpell.SelectedItem.ToString();
        }

        private void TimeWarp_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.TimeWarp = TimeWarp.SelectedItem.ToString();
        }

        private void FrostNova_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.FrostNova = FrostNova.SelectedItem.ToString();
        }

        private void Spellsteal_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Spellsteal = Spellsteal.SelectedItem.ToString();
        }

        private void FrostElemental_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.FrostElemental = FrostElemental.Checked;
        }

        private void Freeze_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.Freeze = Freeze.Checked;
        }

        private void AoE_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.AoE = AoE.SelectedItem.ToString();
        }

        private void FireSpamSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.FireSpamSpell = FireSpamSpell.SelectedItem.ToString();
        }

        private void CriticalMass_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.CriticalMass = CriticalMass.Checked;
        }

        private void LivingBomb_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.LivingBomb = LivingBomb.Checked;
        }

        private void HealthPotPercent_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.HealthPotPercent = int.Parse(HealthPotPercent.Value.ToString());
        }

        private void ManaPotPercent_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.ManaPotPercent = int.Parse(ManaPotPercent.Value.ToString());
        }

        private void MoveDisable_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.MoveDisable = MoveDisable.Checked;
        }

        private void BuffParty_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.BuffParty = BuffParty.Checked;
        }

        private void DeCurseParty_CheckedChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.DeCurseParty = DeCurseParty.Checked;
        }

 
        private void ArcaneBlastStacks_ValueChanged(object sender, EventArgs e)
        {
            AmplifySettings.Instance.ArcaneBlastStacks = int.Parse(ArcaneBlastStacks.Value.ToString());
        }


  

   




    }
}

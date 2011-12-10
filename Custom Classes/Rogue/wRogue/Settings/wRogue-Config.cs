using System;
//using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Styx;
using Styx.Helpers;

namespace wRogue
{
    public partial class wRogueConfig : Form
    {
        public wRogueConfig()
        {
            InitializeComponent();
            BandageAt_Check.Checked = SSSettings.Instance.UseBandages;
            BandageAt_Numeric.Value = new decimal(SSSettings.Instance.UseBandagesAt);
            RestAt_Check.Checked = SSSettings.Instance.UseRest;
            RestAt_Numeric.Value = new decimal(SSSettings.Instance.restHealth);
            UsePremeditation.Checked = SSSettings.Instance.UsePremeditation;
            ShadowDance_Check.Checked = SSSettings.Instance.UseShadowDance;
            AdrenalineRush_Check.Checked = SSSettings.Instance.UseAdrenalineRush;
            KillingSpree_Check.Checked = SSSettings.Instance.UseKillingSpree;
            Preparation_Check.Checked = SSSettings.Instance.UsePreparation;
            ColdBlood_Check.Checked = SSSettings.Instance.UseColdBlood;
            Vendetta_Check.Checked = SSSettings.Instance.UseVendetta;
            SliceAndDice_Check.Checked = SSSettings.Instance.UseSliceAndDice;
            Recuperate_Check.Checked = SSSettings.Instance.UseRecuperate;
            BladeFlurry_Check.Checked = SSSettings.Instance.UseBladeFlurry;
            FanOfKnives_Check.Checked = SSSettings.Instance.UseFanOfKnives;
            AddManagement_Check.Checked = SSSettings.Instance.UseAddManagement;
            LifebloodAt_Check.Checked = SSSettings.Instance.UseLifeblood;
            LifebloodAt_Numeric.Value = new decimal(SSSettings.Instance.UseLifebloodAt);
            PotionAt_Check.Checked = SSSettings.Instance.UseHealthPotions;
            PotionAt_Numeric.Value = new decimal(SSSettings.Instance.UseHealthPotionsAt);
            Pick_Pocket_Check.Checked = SSSettings.Instance.UsePickPocket;
            Distract_Check.Checked = SSSettings.Instance.UseDistract;
            Sprint_Check.Checked = SSSettings.Instance.UseSprintPull;
            Stealth_Check.Checked = SSSettings.Instance.UseStealthToPull;
            Stealth_Always_Check.Checked = SSSettings.Instance.UseStealthAlways;
            Use_Poisons_Check.Checked = SSSettings.Instance.UsePoisons;
            Redirect_Check.Checked = SSSettings.Instance.UseRedirect;
            if (SSSettings.Instance.PullType == 1)
            {
                Pull_Cheapshot.Checked = true;
            }
            if (SSSettings.Instance.PullType == 2)
            {
                Pull_Ambush.Checked = true;
            }
            if (SSSettings.Instance.PullType == 3)
            {
                Pull_Garrote.Checked = true;
            }
            if (SSSettings.Instance.PullType == 4)
            {
                Pull_Melee.Checked = true;
            }
            if (SSSettings.Instance.PullType == 5)
            {
                Pull_Throw.Checked = true;
            }
            if (SSSettings.Instance.poisonToMain == 1)
            {
                Main_Instant_Radio.Checked = true;
            }
            if (SSSettings.Instance.poisonToMain == 2)
            {
                Main_Deadly_Radio.Checked = true;
            }
            if (SSSettings.Instance.poisonToMain == 3)
            {
                Main_Crippling_Radio.Checked = true;
            }
            if (SSSettings.Instance.poisonToMain == 4)
            {
                Main_Wound_Radio.Checked = true;
            }
            if (SSSettings.Instance.poisonToOff == 1)
            {
                Off_Instant_Radio.Checked = true;
            }
            if (SSSettings.Instance.poisonToOff == 2)
            {
                Off_Deadly_Radio.Checked = true;
            }
            if (SSSettings.Instance.poisonToOff == 3)
            {
                Off_Crippling_Radio.Checked = true;
            }
            if (SSSettings.Instance.poisonToOff == 4)
            {
                Off_Wound_Radio.Checked = true;
            }
        }

        private void wRogueConfig_Load(object sender, EventArgs e)
        {
            SSSettings.Instance.Load();
        }

        private void FormConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dr = MessageBox.Show(
                "Save before closing?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
                SSSettings.Instance.Save();
        }

        private void Main_Save_Button_Click_1(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "Save before closing?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
                SSSettings.Instance.Save();
            Close();
        }

        private void RestAt_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseRest = RestAt_Check.Checked;
        }

        private void RestAt_Numeric_ValueChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.restHealth = int.Parse(RestAt_Numeric.Value.ToString());
        }

        private void PotionAt_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseHealthPotions = PotionAt_Check.Checked;
        }

        private void PotionAt_Numeric_ValueChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseHealthPotionsAt = int.Parse(PotionAt_Numeric.Value.ToString());
        }

        private void BandageAt_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseBandages = BandageAt_Check.Checked;
        }

        private void BandageAt_Numeric_ValueChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseBandagesAt = int.Parse(BandageAt_Numeric.Value.ToString());
        }

        private void LifebloodAt_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseLifeblood = LifebloodAt_Check.Checked;
        }

        private void LifebloodAt_Numeric_ValueChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseLifebloodAt = int.Parse(LifebloodAt_Numeric.Value.ToString());
        }

        private void Stealth_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseStealthToPull = Stealth_Check.Checked;
        }

        private void Sprint_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseSprintPull = Sprint_Check.Checked;
        }

        private void Distract_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseDistract = Distract_Check.Checked;
        }

        private void Pick_Pocket_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UsePickPocket = Pick_Pocket_Check.Checked;
        }

        private void Use_Poisons_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UsePoisons = Use_Poisons_Check.Checked;
        }

        private void Main_Instant_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToMain = 1;
        }

        private void Main_Deadly_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToMain = 2;
        }

        private void Main_Crippling_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToMain = 3;
        }

        private void Main_Wound_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToMain = 4;
        }

        private void Off_Instant_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToOff = 1;
        }

        private void Off_Deadly_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToOff = 2;
        }

        private void Off_Crippling_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToOff = 3;
        }

        private void Off_Wound_Radio_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.poisonToOff = 4;
        }

        private void Pull_Cheapshot_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.PullType = 1;
        }

        private void Pull_Ambush_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.PullType = 2;
        }

        private void Pull_Garrote_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.PullType = 3;
        }

        private void Pull_Melee_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.PullType = 4;
        }

        private void Pull_Throw_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.PullType = 5;
        }

        private void Stealth_Always_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseStealthAlways = Stealth_Always_Check.Checked;
        }

        private void UsePremeditation_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UsePremeditation = UsePremeditation.Checked;
        }

        private void Preparation_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UsePreparation = Preparation_Check.Checked;
        }

        private void ShadowDance_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseShadowDance = ShadowDance_Check.Checked;
        }

        private void AdrenalineRush_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseAdrenalineRush = AdrenalineRush_Check.Checked;
        }

        private void KillingSpree_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseKillingSpree = KillingSpree_Check.Checked;
        }

        private void ColdBlood_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseColdBlood = ColdBlood_Check.Checked;
        }

        private void Vendetta_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseVendetta = Vendetta_Check.Checked;
        }

        private void SliceAndDice_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseSliceAndDice = SliceAndDice_Check.Checked;
        }

        private void Recuperate_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseRecuperate = Recuperate_Check.Checked;
        }

        private void BladeFlurry_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseBladeFlurry = BladeFlurry_Check.Checked;
        }

        private void FanOfKnives_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseFanOfKnives = FanOfKnives_Check.Checked;
        }

        private void AddManagement_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseAddManagement = AddManagement_Check.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "Save before closing?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
                SSSettings.Instance.Save();
            Close();
        }

        private void Redirect_Check_CheckedChanged(object sender, EventArgs e)
        {
            SSSettings.Instance.UseRedirect = Redirect_Check.Checked;
        }
    }
}

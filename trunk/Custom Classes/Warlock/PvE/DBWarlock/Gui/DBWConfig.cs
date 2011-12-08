using System;
using System.Drawing;
using System.Windows.Forms;
using Styx;
using Styx.Logic.Combat;
using Styx.WoWInternals;

#pragma warning disable

namespace DBWarlock.Gui
{
    public partial class DBWConfig : Form
    {
        public static int DBWLevel=0;
        public DBWConfig()
        {

            InitializeComponent();


            if (!Warlock.isRunning)
            {
                MessageBox.Show("Please Start Honorbuddy first");
                return;
            }

            if (DBWLevel == 0)
                DBWLevel = ObjectManager.Me.Level;

            Warlock._settings.Load();


            ckUseHowlOfTerror.Checked = Warlock._settings.useHowlOfTerror;
            ckUseFear.Checked = Warlock._settings.useFear;

            ArmorBox.Text = Warlock._settings.armorName;

            ckUseDrainMana.Checked = Warlock._settings.useDrainMana;
            ckUseDrainSoul.Checked = Warlock._settings.useDrainSoul;
            ckDotAdds.Checked = Warlock._settings.dotAdds;
            useIncinerate.Checked = Warlock._settings.useIncinerate;
            ckDebug.Checked = Warlock._settings.showDebug;
            metamorphosisMinimumAggros.Value = Warlock._settings.metamorphosisMinimumAggros;
            useMetamorphosis.Checked = Warlock._settings.useMetamorphosis;
            useSoulFire.Checked = Warlock._settings.useSoulFire;
            useSearingOfPain.Checked = Warlock._settings.useSearingOfPain;

            if (Warlock._settings.useSummon)
            {
                switch (Warlock._settings.summonEntry)
                {
                    case 0:
                        summonAuto.Checked = true;
                        break;
                    case 416:
                        summonImp.Checked = true;
                        break;
                    case 1860:
                        summonVoidWalker.Checked = true;
                        break;
                    case 1863:
                        summonSuccubs.Checked = true;
                        break;
                    case 17252:
                        summonFelGuard.Checked = true;
                        break;
                    default:
                        summonOff.Checked = true;
                        break;

                }
            }
            else
            {
                summonOff.Checked = true;
            }

            statusTimer.Start();
            ckUsehealthFunnel.Checked = Warlock._settings.useHealthFunnel;
            hfMinHealthPercent.Value = (decimal)Warlock._settings.hfMinPlayerHealth;
            hfPetMinHealthPercent.Value = (decimal)Warlock._settings.hfPetMinHealth;
            hfMaxPetHealthPercent.Value = (decimal)Warlock._settings.hfPetMaxHealth;

            ckUseDeathCoil.Checked = Warlock._settings.useDeathCoil;

            deatCoilMaxHealthPercent.Value = (decimal)Warlock._settings.dcMaxHealth;

            healthStoneHP.Value = (decimal)Warlock._settings.healthStoneHealthPercent;

            petAttackDelay.Value = (decimal)Warlock._settings.petAttackDelay;

            ckUseFelDomination.Checked = Warlock._settings.useFelDomination;


            maxSoulShards.Value = new decimal(Warlock._settings.maxSoulShards);
            restHealthPercent.Value = new decimal(Warlock._settings.restHealthPercent);
            restManaPercent.Value = new decimal(Warlock._settings.restManaPercent);
            ckUseHealthStone.Checked = Warlock._settings.useHealthStone;
            ckUseSoulstone.Checked = Warlock._settings.useSoulstone;
            if (Warlock._settings.useFirestone)
            {
                weaponStone.Text = "Firestone";
            }
            else if (Warlock._settings.useSpellstone)
            {
                weaponStone.Text = "Spellstone";
            }
            else
                weaponStone.Text = "Off";

            avrLag.Value = new decimal(Warlock._settings.myLag);

            ckShadowBolt.Checked = Warlock._settings.useShadowBolt;
            ckImmolate.Checked = Warlock._settings.useImmolate;
            ckCorruption.Checked = Warlock._settings.useCorruption;

            ckLifeTap.Checked = Warlock._settings.useLifeTap;
            lfTapMinHealth.Enabled = ckLifeTap.Checked;
            lfTapMinHealth.Value = (decimal)Warlock._settings.lfTapMinHealth;
            lfTapMaxMana.Enabled = ckLifeTap.Checked;
            lfTapMaxMana.Value = (decimal)Warlock._settings.lfTapMaxMana;

            ckDrainLife.Checked = Warlock._settings.useDrainLife;
            dLifeMaxHealth.Enabled = ckDrainLife.Checked;
            dLifeMaxHealth.Value = (decimal)Warlock._settings.drainLifeMaxHealth;
            dLifeStopHealth.Enabled = ckDrainLife.Checked;
            dLifeStopHealth.Value = (decimal)Warlock._settings.drainLifeStopHealth;
            ckDemonicEmpowerment.Checked = Warlock._settings.useDemonicEmpowerment;

            ckWand.Checked = Warlock._settings.useWand;


            if (Warlock._settings.useCurse)
            {
                /*switch (Warlock._settings.curseName)
                {
                    case "Auto":
                        CurseAuto.Checked = true;
                        break;
                    case "Bane of Agony":
                        CurseAgony.Checked = true;
                        break;
                    case "Curse of the Elements":
                        CurseElements.Checked = true;
                        break;
                    case "Curse of Tongues":
                        CurseTongues.Checked = true;
                        break;
                    case "Curse of Weakness":
                        CurseWeaknes.Checked = true;
                        break;
                    default:
                        CurseOff.Checked=true;
                        break;
                }*/
                CurseBox.Text = Warlock._settings.curseName;

            }
            else
            {
                CurseBox.Text = "Off";
            }
            checkEnabledSpells();


        }

        public void checkEnabledSpells()
        {
            ckUseHealthStone.Enabled = SpellManager.Spells.ContainsKey("Create Healthstone");
            dcBox.Enabled = SpellManager.Spells.ContainsKey("Death Coil");
            summonFelGuard.Enabled = SpellManager.Spells.ContainsKey("Summon Felguard");
            summonVoidWalker.Enabled = SpellManager.Spells.ContainsKey("Summon Voidwalker");
            summonSuccubs.Enabled = SpellManager.Spells.ContainsKey("Summon Succubus");
            summonImp.Enabled = SpellManager.Spells.ContainsKey("Summon Imp");
            metamorphosisBox.Enabled = SpellManager.Spells.ContainsKey("Metamorphosis");
        }

        private void maxSoulShards_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.maxSoulShards = int.Parse(maxSoulShards.Value.ToString());
        }

        private void restHealthPercent_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.restHealthPercent = int.Parse(restHealthPercent.Value.ToString());

        }

        private void restManaPercent_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.restManaPercent = int.Parse(restManaPercent.Value.ToString());
        }

        private void DBWConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            statusTimer.Stop();
           // if (Warlock.isRunning)
                Warlock._settings.Save();
        }

        private void ckUseHealthStone_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useHealthStone = ckUseHealthStone.Checked;

        }

        private void ckUseSoulstone_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useSoulstone = ckUseSoulstone.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.myLag = int.Parse(avrLag.Value.ToString());
        }

        private void ckShadowBolt_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useShadowBolt = ckShadowBolt.Checked;

        }

        private void ckImmolate_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useImmolate = ckImmolate.Checked;
        }

        private void ckCorruption_CheckedChanged(object sender, EventArgs e)
        {
            ckCorruption.Checked = Warlock._settings.useCorruption;
        }


        private void CheckPage(object sender, EventArgs e)
        {
            if (aboutTab.SelectedIndex == 0)
                statusTimer.Start();
            else
                statusTimer.Stop();

        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ObjectManager.Me.Level != DBWLevel)
                {
                    checkEnabledSpells();
                    DBWLevel = ObjectManager.Me.Level;
                }

                healthBar.Value = (int)ObjectManager.Me.HealthPercent;
                charHealthPercent.Text = healthBar.Value.ToString() + "%";
                manaBar.Value = (int)ObjectManager.Me.ManaPercent;
                charManaPercent.Text = manaBar.Value.ToString() + "%";
                lvl.Text = ObjectManager.Me.Level.ToString();

                if (ObjectManager.Me.GotAlivePet)
                {
                    summonHealthBar.Value = (int)ObjectManager.Me.Pet.HealthPercent;
                    summonHealthPercent.Text = summonHealthBar.Value.ToString() + "%";
                    summonManaBar.Value = (int)ObjectManager.Me.Pet.ManaPercent;
                    summonManaPercent.Text = summonManaBar.Value.ToString() + "%";
                    summonName.Text = ObjectManager.Me.Pet.Name;
                }
                else
                {
                    summonHealthBar.Value = 0;
                    summonHealthPercent.Text = summonHealthBar.Value.ToString() + "%";
                    summonManaBar.Value = 0;
                    summonManaPercent.Text = summonManaBar.Value.ToString() + "%";
                    summonName.Text = "";
                }


                if (ObjectManager.Me.GotTarget)
                {
                    tgtHealthBar.Value = (int)ObjectManager.Me.CurrentTarget.HealthPercent;
                    tgtHealthPercent.Text = tgtHealthBar.Value.ToString() + "%";

                    if (ObjectManager.Me.CurrentTarget.ManaPercent > 0)
                        tgtManaBar.Value = (int)ObjectManager.Me.CurrentTarget.ManaPercent;
                    else
                        tgtManaBar.Value = 0;
                    tgtManaPercent.Text = tgtManaBar.Value.ToString() + "%";

                    tgtName.Text = ObjectManager.Me.CurrentTarget.Name;
                    tgtLevel.Text = ObjectManager.Me.CurrentTarget.Level.ToString();
                    tgtType.Text = ObjectManager.Me.CurrentTarget.CreatureType.ToString();

                    if (ObjectManager.Me.CurrentTarget.IsHostile)
                    {
                        groupBox8.BackColor = Color.Red;
                        combatType.Text = "Hostile";
                    }
                    else
                        if (ObjectManager.Me.CurrentTarget.IsFriendly)
                        {
                            groupBox8.BackColor = Color.Green;
                            combatType.Text = "Friendly";
                        }
                        else
                        {
                            groupBox8.BackColor = Color.Yellow;
                            if (ObjectManager.Me.CurrentTarget.IsNeutral)
                                combatType.Text = "Neutral";
                            else
                                combatType.Text = "Unknown";
                        }

                }
                else
                {
                    groupBox8.BackColor = Color.Transparent;
                    tgtHealthBar.Value = 0;
                    tgtHealthPercent.Text = tgtHealthBar.Value.ToString() + "%";
                    tgtManaBar.Value = 0;
                    tgtManaPercent.Text = tgtManaBar.Value.ToString() + "%";
                    tgtName.Text = "";
                    tgtLevel.Text = "";
                    tgtType.Text = "";
                    combatType.Text = "";

                }
            }
            catch (Exception ex)
            {
                statusTimer.Stop();
               //DBWarlock.slog(ex.Message + "\n" +ex.StackTrace);
            }



        }

        private void ckLifeTap_CheckedChanged(object sender, EventArgs e)
        {
            lfTapMinHealth.Enabled = ckLifeTap.Checked;
            lfTapMaxMana.Enabled = ckLifeTap.Checked;
            Warlock._settings.useLifeTap = ckLifeTap.Checked;
        }

        private void lfTapMinHealth_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.lfTapMinHealth = (int)lfTapMinHealth.Value;
        }

        private void lfTapMaxMana_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.lfTapMaxMana = (int)lfTapMaxMana.Value;
        }

        private void ckDrainLife_CheckedChanged(object sender, EventArgs e)
        {
            dLifeMaxHealth.Enabled = ckDrainLife.Checked;
            dLifeStopHealth.Enabled = ckDrainLife.Checked;
            Warlock._settings.useDrainLife = ckDrainLife.Checked;

        }

        private void dLifeMaxHealth_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.drainLifeMaxHealth = (int)dLifeMaxHealth.Value;
        }

        private void dLifeStopHealth_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.drainLifeStopHealth = (int)dLifeStopHealth.Value;
        }

        private void ckWand_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useWand = ckWand.Checked;
        }

        private void healthStoneHP_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.healthStoneHealthPercent = (int)healthStoneHP.Value;

        }

        private void petAttackDelay_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.petAttackDelay = (int)petAttackDelay.Value;
        }

        private void ckUseFelDomination_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useFelDomination = ckUseFelDomination.Checked;
        }

        private void deatCoilMaxHealthPercent_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.dcMaxHealth = (int)deatCoilMaxHealthPercent.Value;
        }

        private void ckUseDeathCoil_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useDeathCoil = ckUseDeathCoil.Checked;
        }

        private void ckUsehealthFunnel_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useHealthFunnel = ckUsehealthFunnel.Checked;
        }

        private void hfMinHealthPercent_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.hfMinPlayerHealth = (int)hfMinHealthPercent.Value;
        }

        private void hfPetMinHealthPercent_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.hfPetMinHealth=(int)hfPetMinHealthPercent.Value;

        }

        private void hfMaxPetHealthPercent_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.hfPetMaxHealth = (int)hfMaxPetHealthPercent.Value;
        }

        private void summon_CheckedChanged(object sender, EventArgs e)
        {
            if (summonOff.Checked)
            {
                Warlock._settings.useSummon = false;
                Warlock._settings.summonEntry = 0;
                return;
            }

            if (summonAuto.Checked)
            {
                Warlock._settings.useSummon = true;
                Warlock._settings.summonEntry = 0;
                return;
            }

            if (summonImp.Checked)
            {
                Warlock._settings.useSummon = true;
                Warlock._settings.summonEntry = 416;
                return;
            }

            if (summonVoidWalker.Checked)
            {
                Warlock._settings.useSummon = true;
                Warlock._settings.summonEntry = 1860;
                return;
            }
            if (summonFelGuard.Checked)
            {
                Warlock._settings.useSummon = true;
                Warlock._settings.summonEntry = 17252;
                return;
            }
            if (summonSuccubs.Checked)
            {
                Warlock._settings.useSummon = true;
                Warlock._settings.summonEntry = 1863; // "Succubus";
                return;
            }
        }

     
        private void useSoulFire_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useSoulFire = useSoulFire.Checked;
        }

        private void useSearingOfPain_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useSearingOfPain = useSearingOfPain.Checked;
        }

        private void useMetamorphosis_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useMetamorphosis = useMetamorphosis.Checked;
        }

        private void metamorphosisMinimumAggros_ValueChanged(object sender, EventArgs e)
        {
            Warlock._settings.metamorphosisMinimumAggros = (int)metamorphosisMinimumAggros.Value;
        }

        private void ckDebug_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.showDebug = ckDebug.Checked;
        }

        private void useIncinerate_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useIncinerate = useIncinerate.Checked;
        }

        private void CurseBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Warlock._settings.curseName = CurseBox.Text;
            Warlock._settings.useCurse = !CurseBox.Text.Equals("Off");

        }

        private void weaponStone_SelectedIndexChanged(object sender, EventArgs e)
        {
            Warlock._settings.useFirestone = weaponStone.SelectedItem.Equals("Firestone");
            Warlock._settings.useSpellstone = weaponStone.SelectedItem.Equals("Spellstone");

        }

        private void ckDotAdds_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.dotAdds = ckDotAdds.Checked;
        }

        private void ckUseDrainSoul_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useDrainSoul = ckUseDrainSoul.Checked;
        }

        private void ckUseDrainMana_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useDrainMana = ckUseDrainMana.Checked;
        }

        private void ArmorBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            Warlock._settings.armorName = ArmorBox.Text;
            if (ArmorBox.Text.Equals("Off"))
                Warlock._settings.useArmor = false;

        }

        private void ckUseFear_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useFear = ckUseFear.Checked;
        }

        private void ckUseHowlOfTerror_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useHowlOfTerror = ckUseHowlOfTerror.Checked;
        }

        private void ckDemonicEmpowerment_CheckedChanged(object sender, EventArgs e)
        {
            Warlock._settings.useDemonicEmpowerment = ckDemonicEmpowerment.Checked;
        }


    

      


     

     
     

   

    
    }
}
#pragma warning restore

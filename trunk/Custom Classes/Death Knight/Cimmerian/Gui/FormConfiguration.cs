using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Cimmerian.Gui
{
    public partial class CimmerianForm : Form
    {
        public CimmerianForm()
        {
            InitializeComponent();
        }

        private void LoadSettings_Click_1(object sender, EventArgs e)
        {

            var ofd = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    CimmerianSettings.Instance.LoadFromXML(XElement.Load(fileName));
                    UpdateGuiSettings();
                }
            }

        }

        private void SaveSettings_Click_1(object sender, EventArgs e)
        {

            var sfd = new SaveFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select where to save file"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfd.FileName;
                CimmerianSettings.Instance.SaveToFile(fileName);
            }

        }

        private void CimmerianForm_Load(object sender, EventArgs e)
        {
            CimmerianSettings.Instance.Load();
            RegisterSettingsEventHandlers();
            UpdateGuiSettings();

        }

        private void CimmerianForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CimmerianSettings.Instance.Save();

        }

        private void RegisterSettingsEventHandlers()
        {
            #region Pull

            radioButton1.CheckedChanged += (sender, args) => CimmerianSettings.Instance.OpenWithDeathGrip = radioButton1.Checked;
            radioButton5.CheckedChanged += (sender, args) => CimmerianSettings.Instance.OpenWithIcyTouch = radioButton5.Checked;
            radioButton2.CheckedChanged += (sender, args) => CimmerianSettings.Instance.OpenWithDarkCommand = radioButton2.Checked;
            checkBox6.CheckedChanged += (sender, args) => CimmerianSettings.Instance.OpenWithIcyTouchBackup = checkBox6.Checked;
            numericUpDown4.ValueChanged += (sender, args) => CimmerianSettings.Instance.IcyTouchRange = (int)numericUpDown4.Value;

            #endregion

            #region Player Detector

            checkBox31.CheckedChanged += (sender, args) => CimmerianSettings.Instance.AlertPlayers = checkBox31.Checked;
            checkBox61.CheckedChanged += (sender, args) => CimmerianSettings.Instance.AlertPlayersLog = checkBox61.Checked;
            numericUpDown10.ValueChanged += (sender, args) => CimmerianSettings.Instance.PlayerDetectorRange = (int)numericUpDown10.Value;

            #endregion

            #region Blood

            checkBox23.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseVampiricBlood = checkBox23.Checked;
            checkBox46.CheckedChanged += (sender, args) => CimmerianSettings.Instance.VampiricBloodAdds = checkBox46.Checked;
            numericUpDown22.ValueChanged += (sender, args) => CimmerianSettings.Instance.VampiricBloodHealth = (int)numericUpDown22.Value;

            checkBox22.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseDrw = checkBox22.Checked;
            checkBox47.CheckedChanged += (sender, args) => CimmerianSettings.Instance.DrwAdds = checkBox47.Checked;

            checkBox1.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseBoneShield = checkBox1.Checked;

            checkBox25.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseRuneTap = checkBox25.Checked;
            numericUpDown25.ValueChanged += (sender, args) => CimmerianSettings.Instance.RuneTapHealth = (int)numericUpDown25.Value;

            checkBox28.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseBloodTap = checkBox28.Checked;

            #endregion

            #region Frost

            checkBox26.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseIceboundFortitude = checkBox26.Checked;
            checkBox51.CheckedChanged += (sender, args) => CimmerianSettings.Instance.IceboundFortitudeAdds = checkBox51.Checked;
            numericUpDown27.ValueChanged += (sender, args) => CimmerianSettings.Instance.IceboundFortitudeHealth = (int)numericUpDown27.Value;

            checkBox33.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UsePillar = checkBox33.Checked;
            checkBox27.CheckedChanged += (sender, args) => CimmerianSettings.Instance.PillarAdds = checkBox27.Checked;
            numericUpDown23.ValueChanged += (sender, args) => CimmerianSettings.Instance.PillarHealth = (int)numericUpDown23.Value;

            checkBox50.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseErw = checkBox50.Checked;
            checkBox5.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseErwRunes = checkBox5.Checked;
            checkBox7.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseErwAdds = checkBox7.Checked;

            radioButton11.CheckedChanged += (sender, args) => CimmerianSettings.Instance.RimeIcyTouch = radioButton11.Checked;
            radioButton10.CheckedChanged += (sender, args) => CimmerianSettings.Instance.RimeHb = radioButton10.Checked;

            checkBox36.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseRuneStrike = checkBox36.Checked;

            checkBox24.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseHc = checkBox24.Checked;

            checkBox37.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseHowlingBlast = checkBox37.Checked;

            checkBox4.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseLichborne = checkBox4.Checked;
            numericUpDown3.ValueChanged += (sender, args) => CimmerianSettings.Instance.LichbornHealth = (int)numericUpDown3.Value;


            #endregion

            #region Unholy

            checkBox39.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseSummonGargoyle = checkBox39.Checked;

            checkBox2.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseHorn = checkBox2.Checked;

            checkBox44.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseOutbreak = checkBox44.Checked;

            checkBox3.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseDarkTransformation = checkBox3.Checked;

            checkBox8.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseUnholyFrenzy = checkBox8.Checked;

            #endregion

            #region Pet

            checkBox38.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseRaiseDead = checkBox38.Checked;

            checkBox59.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseDeathPact = checkBox59.Checked;

            numericUpDown35.ValueChanged += (sender, args) => CimmerianSettings.Instance.DeathPactHealth = (int)numericUpDown35.Value;

            #endregion

            #region Racials

            radioButton12.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseEm = radioButton12.Checked;

            radioButton15.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseAt = radioButton15.Checked;

            radioButton14.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseNaaru = radioButton14.Checked;
            checkBox29.CheckedChanged += (sender, args) => CimmerianSettings.Instance.NaaruAdds = checkBox29.Checked;
            numericUpDown19.ValueChanged += (sender, args) => CimmerianSettings.Instance.NaaruHealth = (int)numericUpDown19.Value;

            radioButton13.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseStoneForm = radioButton12.Checked;
            checkBox30.CheckedChanged += (sender, args) => CimmerianSettings.Instance.SfAdds = checkBox30.Checked;
            numericUpDown18.ValueChanged += (sender, args) => CimmerianSettings.Instance.StoneFormHealth = (int)numericUpDown18.Value;

            radioButton27.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseBloodFury = radioButton27.Checked;
            checkBox52.CheckedChanged += (sender, args) => CimmerianSettings.Instance.BloodFuryAdds = checkBox52.Checked;
            numericUpDown17.ValueChanged += (sender, args) => CimmerianSettings.Instance.BloodFuryHealth = (int)numericUpDown17.Value;

            radioButton22.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseBloodFury = radioButton22.Checked;
            checkBox41.CheckedChanged += (sender, args) => CimmerianSettings.Instance.BloodFuryAdds = checkBox41.Checked;
            numericUpDown13.ValueChanged += (sender, args) => CimmerianSettings.Instance.BloodFuryHealth = (int)numericUpDown13.Value;
            checkBox42.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseHowlingBlast = checkBox42.Checked;

            checkBox48.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseLifeBlood = checkBox48.Checked;
            checkBox49.CheckedChanged += (sender, args) => CimmerianSettings.Instance.LifeBloodAdds = checkBox49.Checked;
            numericUpDown16.ValueChanged += (sender, args) => CimmerianSettings.Instance.LifebloodHealth = (int)numericUpDown16.Value;

            #endregion

            #region Misc

            checkBox21.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseMindFreeze = checkBox21.Checked;
            checkBox20.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseStrangulate = checkBox20.Checked;
            checkBox57.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseStrangulateMelee = checkBox57.Checked;
            checkBox19.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseDeathGripInterupt = checkBox19.Checked;
            checkBox35.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseAntiMagicShell = checkBox35.Checked;

            radioButton23.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseChainsOfIce = radioButton23.Checked;
            radioButton24.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseDeathGripRunners = radioButton24.Checked;
            radioButton25.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseDarkCommandRunners = radioButton25.Checked;

            numericUpDown28.ValueChanged += (sender, args) => CimmerianSettings.Instance.AddsCount = (int)numericUpDown28.Value;

            numericUpDown1.ValueChanged += (sender, args) => CimmerianSettings.Instance.RestHealth = (int)numericUpDown1.Value;

            checkBox54.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UsePoF = checkBox54.Checked;

            numericUpDown2.ValueChanged += (sender, args) => CimmerianSettings.Instance.CooldownHealth = (int)numericUpDown2.Value;

            radioButton6.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseBloodPresence = radioButton6.Checked;
            radioButton4.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseFrostPresence = radioButton4.Checked;
            radioButton3.CheckedChanged += (sender, args) => CimmerianSettings.Instance.UseUnholyPresence = radioButton3.Checked;


            #endregion

        }

        private void UpdateGuiSettings()
        {

            #region Pull

            radioButton1.Checked  = CimmerianSettings.Instance.OpenWithDeathGrip;
            radioButton5.Checked  = CimmerianSettings.Instance.OpenWithIcyTouch;
            radioButton2.Checked  = CimmerianSettings.Instance.OpenWithDarkCommand;
            checkBox6.Checked  = CimmerianSettings.Instance.OpenWithIcyTouchBackup;
            numericUpDown4.Value = CimmerianSettings.Instance.IcyTouchRange;

            #endregion

            #region Player Detector

            checkBox31.Checked = CimmerianSettings.Instance.AlertPlayers;
            checkBox61.Checked = CimmerianSettings.Instance.AlertPlayersLog;
            numericUpDown10.Value = CimmerianSettings.Instance.PlayerDetectorRange;

            #endregion

            #region Blood

            checkBox23.Checked = CimmerianSettings.Instance.UseVampiricBlood;
            checkBox46.Checked = CimmerianSettings.Instance.VampiricBloodAdds;
            numericUpDown22.Value = CimmerianSettings.Instance.VampiricBloodHealth;

            checkBox22.Checked = CimmerianSettings.Instance.UseDrw;
            checkBox47.Checked = CimmerianSettings.Instance.DrwAdds;

            checkBox1.Checked = CimmerianSettings.Instance.UseBoneShield;

            checkBox25.Checked = CimmerianSettings.Instance.UseRuneTap;
            numericUpDown25.Value = CimmerianSettings.Instance.RuneTapHealth;

            checkBox28.Checked = CimmerianSettings.Instance.UseBloodTap;

            #endregion

            #region Frost

            checkBox26.Checked = CimmerianSettings.Instance.UseIceboundFortitude;
            checkBox51.Checked = CimmerianSettings.Instance.IceboundFortitudeAdds;
            numericUpDown27.Value = CimmerianSettings.Instance.IceboundFortitudeHealth;

            checkBox33.Checked = CimmerianSettings.Instance.UsePillar;
            checkBox27.Checked = CimmerianSettings.Instance.PillarAdds;
            numericUpDown23.Value = CimmerianSettings.Instance.PillarHealth;

            checkBox50.Checked = CimmerianSettings.Instance.UseErw;
            checkBox5.Checked = CimmerianSettings.Instance.UseErwRunes;
            checkBox7.Checked = CimmerianSettings.Instance.UseErwAdds;

            radioButton11.Checked = CimmerianSettings.Instance.RimeIcyTouch;
            radioButton10.Checked = CimmerianSettings.Instance.RimeHb;

            checkBox36.Checked = CimmerianSettings.Instance.UseRuneStrike;

            checkBox24.Checked = CimmerianSettings.Instance.UseHc;

            checkBox37.Checked = CimmerianSettings.Instance.UseHowlingBlast;

            checkBox4.Checked = CimmerianSettings.Instance.UseLichborne;
            numericUpDown3.Value = CimmerianSettings.Instance.LichbornHealth;

            #endregion

            #region Unholy

            checkBox39.Checked = CimmerianSettings.Instance.UseSummonGargoyle;

            checkBox2.Checked = CimmerianSettings.Instance.UseHorn;

            checkBox44.Checked = CimmerianSettings.Instance.UseOutbreak;

            checkBox3.Checked = CimmerianSettings.Instance.UseDarkTransformation;

            checkBox8.Checked = CimmerianSettings.Instance.UseUnholyFrenzy;

            #endregion

            #region Pet

            checkBox38.Checked = CimmerianSettings.Instance.UseRaiseDead;

            checkBox59.Checked = CimmerianSettings.Instance.UseDeathPact;

            numericUpDown35.Value = CimmerianSettings.Instance.DeathPactHealth;

            #endregion

            #region Racials

            radioButton12.Checked = CimmerianSettings.Instance.UseEm;

            radioButton15.Checked = CimmerianSettings.Instance.UseAt;

            radioButton14.Checked = CimmerianSettings.Instance.UseNaaru;
            checkBox29.Checked = CimmerianSettings.Instance.NaaruAdds;
            numericUpDown19.Value = CimmerianSettings.Instance.NaaruHealth;

            radioButton13.Checked = CimmerianSettings.Instance.UseStoneForm;
            checkBox30.Checked = CimmerianSettings.Instance.SfAdds;
            numericUpDown18.Value = CimmerianSettings.Instance.StoneFormHealth;

            radioButton27.Checked = CimmerianSettings.Instance.UseBloodFury;
            checkBox52.Checked = CimmerianSettings.Instance.BloodFuryAdds;
            numericUpDown17.Value = CimmerianSettings.Instance.BloodFuryHealth;

            radioButton22.Checked = CimmerianSettings.Instance.UseBloodFury;
            checkBox41.Checked = CimmerianSettings.Instance.BloodFuryAdds;
            numericUpDown13.Value = CimmerianSettings.Instance.BloodFuryHealth;
            checkBox42.Checked = CimmerianSettings.Instance.UseHowlingBlast;

            checkBox48.Checked = CimmerianSettings.Instance.UseLifeBlood;
            checkBox49.Checked = CimmerianSettings.Instance.LifeBloodAdds;
            numericUpDown16.Value = CimmerianSettings.Instance.LifebloodHealth;

            #endregion

            #region Misc

            checkBox21.Checked = CimmerianSettings.Instance.UseMindFreeze;
            checkBox20.Checked = CimmerianSettings.Instance.UseStrangulate;
            checkBox57.Checked = CimmerianSettings.Instance.UseStrangulateMelee;
            checkBox19.Checked = CimmerianSettings.Instance.UseDeathGripInterupt;
            checkBox35.Checked = CimmerianSettings.Instance.UseAntiMagicShell;

            radioButton23.Checked = CimmerianSettings.Instance.UseChainsOfIce;
            radioButton24.Checked = CimmerianSettings.Instance.UseDeathGripRunners;
            radioButton25.Checked = CimmerianSettings.Instance.UseDarkCommandRunners;

            numericUpDown28.Value = CimmerianSettings.Instance.AddsCount;

            numericUpDown1.Value = CimmerianSettings.Instance.RestHealth;

            checkBox54.Checked = CimmerianSettings.Instance.UsePoF;

            numericUpDown2.Value = CimmerianSettings.Instance.CooldownHealth;

            radioButton6.Checked = CimmerianSettings.Instance.UseBloodPresence;
            radioButton4.Checked = CimmerianSettings.Instance.UseFrostPresence;
            radioButton3.Checked = CimmerianSettings.Instance.UseUnholyPresence;

            #endregion


        }
        

    }
}

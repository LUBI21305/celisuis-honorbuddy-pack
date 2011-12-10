using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace HandnavisTankadin
{

    public partial class HandnavisTankadinConfig : Form
    {
        public HandnavisTankadinConfig()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.Load();
            
            usetaunt.Checked = HandnavisTankadinSettings.Instance.UseTaunt;
	    uselay.Checked = HandnavisTankadinSettings.Instance.UseLay;
            checkBox1.Checked = HandnavisTankadinSettings.Instance.UseExo;
            divinepleacd.Checked = HandnavisTankadinSettings.Instance.DivinePleaCD;

            manapercent.Value = new decimal(HandnavisTankadinSettings.Instance.ManaPercent);
            healthpercent.Value = new decimal(HandnavisTankadinSettings.Instance.HealthPercent);
            checkBox2.Checked = HandnavisTankadinSettings.Instance.UseAura;
            layonhandspercent.Value = new decimal(HandnavisTankadinSettings.Instance.LayonHandsPercent);
            divinepleapercent.Value = new decimal(HandnavisTankadinSettings.Instance.DivinePleaPercent);
            guardianoftheancientkingspercent.Value = new decimal(HandnavisTankadinSettings.Instance.GuardianoftheancientKingsPercent);
            ardentdefenderpercent.Value = new decimal(HandnavisTankadinSettings.Instance.ArdentDefenderPercent);
            divineprotectionpercent.Value = new decimal(HandnavisTankadinSettings.Instance.DivineProtectionPercent);
            holyshieldpercent.Value = new decimal(HandnavisTankadinSettings.Instance.HolyShieldPercent);


            if (HandnavisTankadinSettings.Instance.Seal == 0)
            {
                radioButton1.Select();
            }
            else if (HandnavisTankadinSettings.Instance.Seal == 1)
            {
                radioButton2.Select();
            }
            else if (HandnavisTankadinSettings.Instance.Seal == 2)
            {
                radioButton3.Select();
            }
            else if (HandnavisTankadinSettings.Instance.Seal == 3)
            {
                radioButton4.Select();
            }


            if (HandnavisTankadinSettings.Instance.Movement == 0)
            {
                radioButton5.Select();
            }
            else if (HandnavisTankadinSettings.Instance.Movement == 1)
            {
                radioButton6.Select();
            }
            else if (HandnavisTankadinSettings.Instance.Movement == 2)
            {
                radioButton7.Select();
            }


            if (HandnavisTankadinSettings.Instance.Faceing == 0)
            {
                radioButton8.Select();
            }
            else if (HandnavisTankadinSettings.Instance.Faceing == 1)
            {
                radioButton9.Select();
            }
            else if (HandnavisTankadinSettings.Instance.Faceing == 2)
            {
                radioButton10.Select();
            }



        }




        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Seal = 0;
            }
            else if (radioButton2.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Seal = 1;
            }
            else if (radioButton3.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Seal = 2;
            }
            else if (radioButton4.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Seal = 3;
            }
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton5.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Movement = 0;
            }
            else if (radioButton6.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Movement = 1;
            }
            else if (radioButton7.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Movement = 2;
            }

        }


        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton8.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Faceing = 0;
            }
            else if (radioButton9.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Faceing = 1;
            }
            else if (radioButton10.Checked == true)
            {
                HandnavisTankadinSettings.Instance.Faceing = 2;
            }
        }











        private void manapercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.ManaPercent = (int)manapercent.Value;
        }
        private void healthpercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.HealthPercent = (int)healthpercent.Value;
        }

        private void usetaunt_CheckedChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.UseTaunt = usetaunt.Checked;
        }



        private void uselay_CheckedChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.UseLay = uselay.Checked;
        }


        private void divinepleacd_CheckedChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.DivinePleaCD = divinepleacd.Checked;
        }


	private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.UseExo = checkBox1.Checked;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.Save();
            Logging.Write("Config saved");
            Close();
        }



        private void usetaunt_CheckedChanged_1(object sender, EventArgs e)
        {
            if (usetaunt.Checked == true)
            {
                HandnavisTankadinSettings.Instance.UseTaunt = true;
            }
            else
            {
                HandnavisTankadinSettings.Instance.UseTaunt = false;
            }
        }

	
	        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
		 if (checkBox1.Checked == true)
            {
                HandnavisTankadinSettings.Instance.UseExo = true;
            }
            else
            {
                HandnavisTankadinSettings.Instance.UseExo = false;
            }
        }










        private void divinepleacd_CheckedChanged_1(object sender, EventArgs e)
        {
            if (divinepleacd.Checked == true)
            {
                HandnavisTankadinSettings.Instance.DivinePleaCD = true;
            }
            else
            {
                HandnavisTankadinSettings.Instance.DivinePleaCD = false;
            }
        }




        private void layonhandspercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.LayonHandsPercent = (int)layonhandspercent.Value;
        }






        private void divinepleapercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.DivinePleaPercent = (int)divinepleapercent.Value;
        }





        private void guardianoftheancientkingspercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.GuardianoftheancientKingsPercent = (int)guardianoftheancientkingspercent.Value;
        }





        private void ardentdefenderpercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.ArdentDefenderPercent = (int)ardentdefenderpercent.Value;
        }


        private void divineprotectionpercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.DivineProtectionPercent = (int)divineprotectionpercent.Value;
        }


        private void holyshieldpercent_ValueChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.HolyShieldPercent = (int)holyshieldpercent.Value;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            HandnavisTankadinSettings.Instance.UseAura = checkBox2.Checked;
        }





    }
}

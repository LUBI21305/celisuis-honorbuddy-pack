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

namespace HazzDruid
{
    public partial class HazzDruidConfig : Form
    {
        public HazzDruidConfig()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.Load();
            checkBox1.Checked = HazzDruidSettings.Instance.UseTree;
            checkBox2.Checked = HazzDruidSettings.Instance.UseRebirth;
            checkBox3.Checked = HazzDruidSettings.Instance.UseRevive;
            checkBox4.Checked = HazzDruidSettings.Instance.UseRemoveCurse;
			trackBar10.Value = HazzDruidSettings.Instance.LifebloomPercent;
            trackBar8.Value = HazzDruidSettings.Instance.HealthPercent;
            trackBar9.Value = HazzDruidSettings.Instance.ManaPercent;
            trackBar1.Value = HazzDruidSettings.Instance.SwiftmendPercent;
            trackBar2.Value = HazzDruidSettings.Instance.RegrowthPercent;
            trackBar3.Value = HazzDruidSettings.Instance.RejuvenationPercent;
            trackBar4.Value = HazzDruidSettings.Instance.TranquilityPercent;
            trackBar5.Value = HazzDruidSettings.Instance.HealingTouchPercent;
            trackBar6.Value = HazzDruidSettings.Instance.WildGrowthPercent;
            trackBar7.Value = HazzDruidSettings.Instance.NaturesPercent;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.UseTree = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.UseRebirth = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.UseRevive = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.UseRemoveCurse = checkBox4.Checked;
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.HealthPercent = trackBar8.Value;
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.ManaPercent = trackBar9.Value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.RejuvenationPercent = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.RegrowthPercent = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.RejuvenationPercent = trackBar3.Value;
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.TranquilityPercent = trackBar4.Value;
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.HealingTouchPercent = trackBar5.Value;
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.WildGrowthPercent = trackBar6.Value;
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.NaturesPercent = trackBar7.Value;
        }
		
		private void trackBar10_Scroll(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.LifebloomPercent = trackBar8.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HazzDruidSettings.Instance.Save();
            Close();
        }

    }
}

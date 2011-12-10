using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MageHelper
{
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent(); 
        }
        private void MHelperConfig_Load(object sender, EventArgs e)
        {
            MageHelperSettings.Instance.Load();
            useblink.Checked = MageHelperSettings.Instance.UseBlink;
            useslowfall.Checked = MageHelperSettings.Instance.UseSlowFall;
            useinvis.Checked = MageHelperSettings.Instance.UseInvis;
            invisadds.Value = new decimal(MageHelperSettings.Instance.InvisAdds);
            invishp.Value = new decimal(MageHelperSettings.Instance.InvisHP);
        }
        private void useblink_CheckedChanged(object sender, EventArgs e)
        {
            MageHelperSettings.Instance.UseBlink = useblink.Checked;
        }
        private void useslowfall_CheckedChanged(object sender, EventArgs e)
        {
            MageHelperSettings.Instance.UseSlowFall = useslowfall.Checked;
        }
        private void useinvis_CheckedChanged(object sender, EventArgs e)
        {
            MageHelperSettings.Instance.UseInvis = useinvis.Checked;
        }
        private void invisadds_ValueChanged(object sender, EventArgs e)
        {
            MageHelperSettings.Instance.InvisAdds = int.Parse(invisadds.Value.ToString());
        }
        private void invishp_ValueChanged(object sender, EventArgs e)
        {
            MageHelperSettings.Instance.InvisHP = int.Parse(invishp.Value.ToString());
        }
        private void Save_Click(object sender, EventArgs e)
        {
            MageHelperSettings.Instance.Save();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

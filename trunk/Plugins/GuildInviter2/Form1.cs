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

namespace GuildInviter2
{
    public partial class cfg : Form
    {
        public cfg()
        {
            InitializeComponent();
        }

        private void cfg_Load(object sender, EventArgs e)
        {
            GISettings.Instance.Load();
            MinLevel.Value = new decimal(GISettings.Instance.MinLevel);
            MaxLevel.Value = new decimal(GISettings.Instance.MaxLevel);
            WhisperText.Text = GISettings.Instance.whispertext;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (MinLevel.Value > MaxLevel.Value)
            {
                MessageBox.Show("Minimum level has to be lower than maximum level!");
            }
            else if (WhisperText.Text.Contains("€") | WhisperText.Text.Contains("£") | WhisperText.Text.Contains("$") | WhisperText.Text.Contains("%") | WhisperText.Text.Contains("^") | WhisperText.Text.Contains("&") | WhisperText.Text.Contains("*"))
            {
                MessageBox.Show("Please do not use the following characters: \r\n €£$%^&*");
            }
            else
            {
                GISettings.Instance.MinLevel = (int)MinLevel.Value;
                GISettings.Instance.MaxLevel = (int)MaxLevel.Value;
                GISettings.Instance.whispertext = WhisperText.Text;

                GISettings.Instance.Save();
                Logging.Write("GuildInviter config saved");
                Close();
            }
        }
    }
}

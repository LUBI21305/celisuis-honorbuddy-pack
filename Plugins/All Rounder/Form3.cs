using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using Styx;
using Allrounder;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins.PluginClass;
using Styx.Logic.Pathing;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.MailBox;
using System.Text.RegularExpressions;
using Styx.Logic.Profiles;

namespace Allrounder
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInksde = new System.Windows.Forms.CheckBox();
            this.btnGemsde = new System.Windows.Forms.CheckBox();
            this.btndarkde = new System.Windows.Forms.CheckBox();
            this.btnLeatherde = new System.Windows.Forms.CheckBox();
            this.btnEtenalsde = new System.Windows.Forms.CheckBox();
            this.btnDisenchantde = new System.Windows.Forms.CheckBox();
            this.btnClothde = new System.Windows.Forms.CheckBox();
            this.btnFortunede = new System.Windows.Forms.CheckBox();
            this.Savebtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInksde
            // 
            this.btnInksde.AutoSize = true;
            this.btnInksde.Location = new System.Drawing.Point(13, 13);
            this.btnInksde.Name = "btnInksde";
            this.btnInksde.Size = new System.Drawing.Size(85, 17);
            this.btnInksde.TabIndex = 0;
            this.btnInksde.Text = "Deposit Inks";
            this.btnInksde.UseVisualStyleBackColor = true;
            // 
            // btnGemsde
            // 
            this.btnGemsde.AutoSize = true;
            this.btnGemsde.Location = new System.Drawing.Point(13, 37);
            this.btnGemsde.Name = "btnGemsde";
            this.btnGemsde.Size = new System.Drawing.Size(92, 17);
            this.btnGemsde.TabIndex = 1;
            this.btnGemsde.Text = "Deposit Gems";
            this.btnGemsde.UseVisualStyleBackColor = true;
            // 
            // btndarkde
            // 
            this.btndarkde.AutoSize = true;
            this.btndarkde.Location = new System.Drawing.Point(13, 61);
            this.btndarkde.Name = "btndarkde";
            this.btndarkde.Size = new System.Drawing.Size(144, 17);
            this.btndarkde.TabIndex = 2;
            this.btndarkde.Text = "Deposit Darkmoon Cards";
            this.btndarkde.UseVisualStyleBackColor = true;
            // 
            // btnLeatherde
            // 
            this.btnLeatherde.AutoSize = true;
            this.btnLeatherde.Location = new System.Drawing.Point(13, 85);
            this.btnLeatherde.Name = "btnLeatherde";
            this.btnLeatherde.Size = new System.Drawing.Size(101, 17);
            this.btnLeatherde.TabIndex = 3;
            this.btnLeatherde.Text = "Deposit Leather";
            this.btnLeatherde.UseVisualStyleBackColor = true;
            // 
            // btnEtenalsde
            // 
            this.btnEtenalsde.AutoSize = true;
            this.btnEtenalsde.Location = new System.Drawing.Point(13, 109);
            this.btnEtenalsde.Name = "btnEtenalsde";
            this.btnEtenalsde.Size = new System.Drawing.Size(103, 17);
            this.btnEtenalsde.TabIndex = 4;
            this.btnEtenalsde.Text = "Deposit Eternals";
            this.btnEtenalsde.UseVisualStyleBackColor = true;
            // 
            // btnDisenchantde
            // 
            this.btnDisenchantde.AutoSize = true;
            this.btnDisenchantde.Location = new System.Drawing.Point(13, 133);
            this.btnDisenchantde.Name = "btnDisenchantde";
            this.btnDisenchantde.Size = new System.Drawing.Size(131, 17);
            this.btnDisenchantde.TabIndex = 5;
            this.btnDisenchantde.Text = "Deposit Disenchanted";
            this.btnDisenchantde.UseVisualStyleBackColor = true;
            // 
            // btnClothde
            // 
            this.btnClothde.AutoSize = true;
            this.btnClothde.Location = new System.Drawing.Point(13, 157);
            this.btnClothde.Name = "btnClothde";
            this.btnClothde.Size = new System.Drawing.Size(89, 17);
            this.btnClothde.TabIndex = 6;
            this.btnClothde.Text = "Deposit Cloth";
            this.btnClothde.UseVisualStyleBackColor = true;
            // 
            // btnFortunede
            // 
            this.btnFortunede.AutoSize = true;
            this.btnFortunede.Location = new System.Drawing.Point(13, 181);
            this.btnFortunede.Name = "btnFortunede";
            this.btnFortunede.Size = new System.Drawing.Size(131, 17);
            this.btnFortunede.TabIndex = 7;
            this.btnFortunede.Text = "Deposit Fortune Cards";
            this.btnFortunede.UseVisualStyleBackColor = true;
            // 
            // Savebtn
            // 
            this.Savebtn.Location = new System.Drawing.Point(69, 227);
            this.Savebtn.Name = "Savebtn";
            this.Savebtn.Size = new System.Drawing.Size(75, 23);
            this.Savebtn.TabIndex = 8;
            this.Savebtn.Text = "Save";
            this.Savebtn.UseVisualStyleBackColor = true;
            this.Savebtn.Click += new System.EventHandler(this.Savebtn_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 262);
            this.Controls.Add(this.Savebtn);
            this.Controls.Add(this.btnFortunede);
            this.Controls.Add(this.btnClothde);
            this.Controls.Add(this.btnDisenchantde);
            this.Controls.Add(this.btnEtenalsde);
            this.Controls.Add(this.btnLeatherde);
            this.Controls.Add(this.btndarkde);
            this.Controls.Add(this.btnGemsde);
            this.Controls.Add(this.btnInksde);
            this.Name = "Form3";
            this.Text = "Guild Deposit";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox btnInksde;
        private System.Windows.Forms.CheckBox btnGemsde;
        private System.Windows.Forms.CheckBox btndarkde;
        private System.Windows.Forms.CheckBox btnLeatherde;
        private System.Windows.Forms.CheckBox btnEtenalsde;
        private System.Windows.Forms.CheckBox btnDisenchantde;
        private System.Windows.Forms.CheckBox btnClothde;
        private System.Windows.Forms.CheckBox btnFortunede;
        private System.Windows.Forms.Button Savebtn;

        public void Guilddeposit_Shown(object sender, EventArgs e)
        {
            Depositsettings settings = new Depositsettings();
            settings.LoadSetings();
            btnInksde.Checked = settings.Inkde;
            btnGemsde.Checked = settings.Gemsde;
            btndarkde.Checked = settings.Darkde;
            btnLeatherde.Checked = settings.Leatherde;
            btnEtenalsde.Checked = settings.Eternalde;
            btnDisenchantde.Checked = settings.Disenchantde;
            btnClothde.Checked = settings.Clothde;
            btnFortunede.Checked = settings.Fortunede;
        }

        public void Guilddeposit_Activated(object sender, EventArgs e)
        {
            Depositsettings settings = new Depositsettings();
            btnInksde.Checked = settings.Inkde;
            btnGemsde.Checked = settings.Gemsde;
            btndarkde.Checked = settings.Darkde;
            btnLeatherde.Checked = settings.Leatherde;
            btnEtenalsde.Checked = settings.Eternalde;
            btnDisenchantde.Checked = settings.Disenchantde;
            btnClothde.Checked = settings.Clothde;
            btnFortunede.Checked = settings.Fortunede;
        }

        private void Savebtn_Click(object sender, EventArgs e)
        {
            Depositsettings settings = new Depositsettings();
            settings.SaveSettings(btnInksde.Checked, btnGemsde.Checked, btndarkde.Checked, btnLeatherde.Checked, btnEtenalsde.Checked, btnDisenchantde.Checked, btnClothde.Checked, btnFortunede.Checked);
            btnInksde.Checked = settings.Inkde;
            btnGemsde.Checked = settings.Gemsde;
            btndarkde.Checked = settings.Darkde;
            btnLeatherde.Checked = settings.Leatherde;
            btnEtenalsde.Checked = settings.Eternalde;
            btnDisenchantde.Checked = settings.Disenchantde;
            btnClothde.Checked = settings.Clothde;
            btnFortunede.Checked = settings.Fortunede;
            Logging.Write(Color.SandyBrown, "Guild Deposit Settings Saved");
            Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Depositsettings settings = new Depositsettings();
            settings.LoadSetings();
            btnInksde.Checked = settings.Inkde;
            btnGemsde.Checked = settings.Gemsde;
            btndarkde.Checked = settings.Darkde;
            btnLeatherde.Checked = settings.Leatherde;
            btnEtenalsde.Checked = settings.Eternalde;
            btnDisenchantde.Checked = settings.Disenchantde;
            btnClothde.Checked = settings.Clothde;
            btnFortunede.Checked = settings.Fortunede;
        }


    }
    public partial class Form3 : Form
    {
        #region HBStuff
        public bool useDefault = false;
        public bool settingsLoaded = false;
        public bool Inkde { get; set; }
        public bool Gemsde { get; set; }
        public bool Darkde { get; set; }
        public bool Leatherde { get; set; }
        public bool Eternalde { get; set; }
        public bool Disenchantde { get; set; }
        public bool Clothde { get; set; }
        public bool Fortunede { get; set; }
        public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Allrounder"));
        #endregion

        public Form3()
        {
            InitializeComponent();
        }

        public void DefaultSettings()
        {
            string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guilddeposit.xml");
            File.Delete(settingsPath);
            File.Copy(Path.Combine(PluginFolderPath, "Guilddeposit.xml"), settingsPath);
            Logging.Write(Color.Blue, "Allrounder Guilddeposit default settings loaded, please use settings button to select your options.");
            useDefault = true;
        }

        public bool PulseLoadsettings3()
        {
            string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guilddeposit.xml");
            XElement elm = XElement.Load(settingsPath);
            XElement[] options = elm.Elements("Option").ToArray();
            bool _Inkde;
            bool _Gemsde;
            bool _Darkde;
            bool _Leatherde;
            bool _Eternalde;
            bool _Disenchatde;
            bool _Clothde;
            bool _Fortunede;
            foreach (XElement opt in options)
            {
                var value = opt.Attributes("Value").ToList();
                var set = opt.Attributes("Set").ToList();
                if (value[0].Value == "Inkde")
                {
                    bool.TryParse(set[0].Value, out _Inkde);
                    Inkde = _Inkde;
                }
                if (value[0].Value == "Gemsde")
                {
                    bool.TryParse(set[0].Value, out _Gemsde);
                    Gemsde = _Gemsde;
                }
                if (value[0].Value == "Darkde")
                {
                    bool.TryParse(set[0].Value, out _Darkde);
                    Darkde = _Darkde;
                }
                if (value[0].Value == "Leatherde")
                {
                    bool.TryParse(set[0].Value, out _Leatherde);
                    Leatherde = _Leatherde;
                }
                if (value[0].Value == "Eternalde")
                {
                    bool.TryParse(set[0].Value, out _Eternalde);
                    Eternalde = _Eternalde;
                }
                if (value[0].Value == "Disenchantde")
                {
                    bool.TryParse(set[0].Value, out _Disenchatde);
                    Disenchantde = _Disenchatde;
                }
                if (value[0].Value == "Clothde")
                {
                    bool.TryParse(set[0].Value, out _Clothde);
                    Clothde = _Clothde;
                }
                if (value[0].Value == "Fortunede")
                {
                    bool.TryParse(set[0].Value, out _Fortunede);
                    Fortunede = _Fortunede;
                }
            }
            return true;
        }

        #region Settings
        class Depositsettings
        {
            public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Allrounder"));
            public bool Inkde { get; set; }
            public bool Gemsde { get; set; }
            public bool Darkde { get; set; }
            public bool Leatherde { get; set; }
            public bool Eternalde { get; set; }
            public bool Disenchantde { get; set; }
            public bool Clothde { get; set; }
            public bool Fortunede { get; set; }
            public void LoadSetings()
            {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guilddeposit.xml");
                if (File.Exists(settingsPath))
                {
                }
                else
                {
                    File.Copy(Path.Combine(PluginFolderPath, "Guilddeposit.xml"), settingsPath);
                }
                XElement elm = XElement.Load(settingsPath);
                XElement[] options = elm.Elements("Option").ToArray();
                bool _Inkde;
                bool _Gemsde;
                bool _Darkde;
                bool _Leatherde;
                bool _Eternalde;
                bool _Disenchatde;
                bool _Clothde;
                bool _Fortunede;
                foreach (XElement opt in options)
                {
                    var value = opt.Attributes("Value").ToList();
                    var set = opt.Attributes("Set").ToList();
                    if (value[0].Value == "Inkde")
                    {
                        bool.TryParse(set[0].Value, out _Inkde);
                        Inkde = _Inkde;
                    }
                    if (value[0].Value == "Gemsde")
                    {
                        bool.TryParse(set[0].Value, out _Gemsde);
                        Gemsde = _Gemsde;
                    }
                    if (value[0].Value == "Darkde")
                    {
                        bool.TryParse(set[0].Value, out _Darkde);
                        Darkde = _Darkde;
                    }
                    if (value[0].Value == "Leatherde")
                    {
                        bool.TryParse(set[0].Value, out _Leatherde);
                        Leatherde = _Leatherde;
                    }
                    if (value[0].Value == "Eternalde")
                    {
                        bool.TryParse(set[0].Value, out _Eternalde);
                        Eternalde = _Eternalde;
                    }
                    if (value[0].Value == "Disenchantde")
                    {
                        bool.TryParse(set[0].Value, out _Disenchatde);
                        Disenchantde = _Disenchatde;
                    }
                    if (value[0].Value == "Clothde")
                    {
                        bool.TryParse(set[0].Value, out _Clothde);
                        Clothde = _Clothde;
                    }
                    if (value[0].Value == "Fortunede")
                    {
                        bool.TryParse(set[0].Value, out _Fortunede);
                        Fortunede = _Fortunede;
                    }
                }
            }
            public void SaveSettings(
                bool _Inkde,
                bool _Gemsde,
                bool _Darkde,
                bool _Leatherde,
                bool _Eternalde,
                bool _Disenchantde,
                bool _Clothde,
                bool _Fortunede)
            {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guilddeposit.xml");
                if (File.Exists(settingsPath))
                {
                }
                else
                {
                    File.Copy(Path.Combine(PluginFolderPath, "Guilddeposit.xml"), settingsPath);
                }
                XElement saveElm = File.Exists(settingsPath) ? XElement.Load(settingsPath) : new XElement("Settings");
                XElement[] options = saveElm.Elements("Option").ToArray();
                foreach (XElement opt in options)
                {
                    var value = opt.Attributes("Value").ToList();
                    if (value[0].Value == "Inkde")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Inkde"), new XAttribute("Set", _Inkde.ToString())));
                    }
                    if (value[0].Value == "Gemsde")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Gemsde"), new XAttribute("Set", _Gemsde.ToString())));
                    }
                    if (value[0].Value == "Darkde")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Darkde"), new XAttribute("Set", _Darkde.ToString())));
                    }
                    if (value[0].Value == "Leatherde")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Leatherde"), new XAttribute("Set", _Leatherde.ToString())));
                    }
                    if (value[0].Value == "Eternalde")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Eternalde"), new XAttribute("Set", _Eternalde.ToString())));
                    }
                    if (value[0].Value == "Disenchantde")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Disenchantde"), new XAttribute("Set", _Disenchantde.ToString())));
                    }
                    if (value[0].Value == "Clothde")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Clothde"), new XAttribute("Set", _Clothde.ToString())));
                    }
                    if (value[0].Value == "Fortunede")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Fortune"), new XAttribute("Set", _Fortunede.ToString())));
                    }
                    saveElm.Save(settingsPath);
        #endregion
                }
            }
        }
    }
}

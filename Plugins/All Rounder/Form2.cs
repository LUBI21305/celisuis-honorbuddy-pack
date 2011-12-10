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
    partial class Guildwithdraw
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
            this.btnHerbs = new System.Windows.Forms.CheckBox();
            this.btnOres = new System.Windows.Forms.CheckBox();
            this.btnGems = new System.Windows.Forms.CheckBox();
            this.btnInks = new System.Windows.Forms.CheckBox();
            this.btnEternals = new System.Windows.Forms.CheckBox();
            this.btnLeather = new System.Windows.Forms.CheckBox();
            this.btnLockboxes = new System.Windows.Forms.CheckBox();
            this.btnSave2 = new System.Windows.Forms.Button();
            this.btndarkmoonm = new System.Windows.Forms.CheckBox();
            this.btnfortunem = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnHerbs
            // 
            this.btnHerbs.AutoSize = true;
            this.btnHerbs.Location = new System.Drawing.Point(13, 13);
            this.btnHerbs.Name = "btnHerbs";
            this.btnHerbs.Size = new System.Drawing.Size(54, 17);
            this.btnHerbs.TabIndex = 0;
            this.btnHerbs.Text = "Herbs";
            this.btnHerbs.UseVisualStyleBackColor = true;
            // 
            // btnOres
            // 
            this.btnOres.AutoSize = true;
            this.btnOres.Location = new System.Drawing.Point(13, 37);
            this.btnOres.Name = "btnOres";
            this.btnOres.Size = new System.Drawing.Size(48, 17);
            this.btnOres.TabIndex = 1;
            this.btnOres.Text = "Ores";
            this.btnOres.UseVisualStyleBackColor = true;
            // 
            // btnGems
            // 
            this.btnGems.AutoSize = true;
            this.btnGems.Location = new System.Drawing.Point(13, 61);
            this.btnGems.Name = "btnGems";
            this.btnGems.Size = new System.Drawing.Size(53, 17);
            this.btnGems.TabIndex = 2;
            this.btnGems.Text = "Gems";
            this.btnGems.UseVisualStyleBackColor = true;
            // 
            // btnInks
            // 
            this.btnInks.AutoSize = true;
            this.btnInks.Location = new System.Drawing.Point(13, 85);
            this.btnInks.Name = "btnInks";
            this.btnInks.Size = new System.Drawing.Size(69, 17);
            this.btnInks.TabIndex = 3;
            this.btnInks.Text = "Pigments";
            this.btnInks.UseVisualStyleBackColor = true;
            // 
            // btnEternals
            // 
            this.btnEternals.AutoSize = true;
            this.btnEternals.Location = new System.Drawing.Point(96, 61);
            this.btnEternals.Name = "btnEternals";
            this.btnEternals.Size = new System.Drawing.Size(64, 17);
            this.btnEternals.TabIndex = 4;
            this.btnEternals.Text = "Eternals";
            this.btnEternals.UseVisualStyleBackColor = true;
            // 
            // btnLeather
            // 
            this.btnLeather.AutoSize = true;
            this.btnLeather.Location = new System.Drawing.Point(96, 13);
            this.btnLeather.Name = "btnLeather";
            this.btnLeather.Size = new System.Drawing.Size(62, 17);
            this.btnLeather.TabIndex = 5;
            this.btnLeather.Text = "Leather";
            this.btnLeather.UseVisualStyleBackColor = true;
            // 
            // btnLockboxes
            // 
            this.btnLockboxes.AutoSize = true;
            this.btnLockboxes.Location = new System.Drawing.Point(96, 37);
            this.btnLockboxes.Name = "btnLockboxes";
            this.btnLockboxes.Size = new System.Drawing.Size(78, 17);
            this.btnLockboxes.TabIndex = 6;
            this.btnLockboxes.Text = "Lockboxes";
            this.btnLockboxes.UseVisualStyleBackColor = true;
            // 
            // btnSave2
            // 
            this.btnSave2.Location = new System.Drawing.Point(12, 211);
            this.btnSave2.Name = "btnSave2";
            this.btnSave2.Size = new System.Drawing.Size(75, 23);
            this.btnSave2.TabIndex = 7;
            this.btnSave2.Text = "Save";
            this.btnSave2.UseVisualStyleBackColor = true;
            this.btnSave2.Click += new System.EventHandler(this.btnSave2_Click);
            // 
            // btndarkmoonm
            // 
            this.btndarkmoonm.AutoSize = true;
            this.btndarkmoonm.Location = new System.Drawing.Point(13, 135);
            this.btndarkmoonm.Name = "btndarkmoonm";
            this.btndarkmoonm.Size = new System.Drawing.Size(101, 17);
            this.btndarkmoonm.TabIndex = 8;
            this.btndarkmoonm.Text = "Darkmoon Mats";
            this.btndarkmoonm.UseVisualStyleBackColor = true;
            // 
            // btnfortunem
            // 
            this.btnfortunem.AutoSize = true;
            this.btnfortunem.Location = new System.Drawing.Point(13, 159);
            this.btnfortunem.Name = "btnfortunem";
            this.btnfortunem.Size = new System.Drawing.Size(88, 17);
            this.btnfortunem.TabIndex = 9;
            this.btnfortunem.Text = "Fortune Mats";
            this.btnfortunem.UseVisualStyleBackColor = true;
            // 
            // Guildwithdraw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 246);
            this.Controls.Add(this.btnfortunem);
            this.Controls.Add(this.btndarkmoonm);
            this.Controls.Add(this.btnSave2);
            this.Controls.Add(this.btnLockboxes);
            this.Controls.Add(this.btnLeather);
            this.Controls.Add(this.btnEternals);
            this.Controls.Add(this.btnInks);
            this.Controls.Add(this.btnGems);
            this.Controls.Add(this.btnOres);
            this.Controls.Add(this.btnHerbs);
            this.Name = "Guildwithdraw";
            this.Text = "Guild Withdraw";
            this.Load += new System.EventHandler(this.Guildwithdraw_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox btnHerbs;
        private CheckBox btnOres;
        private CheckBox btnGems;
        private CheckBox btnInks;
        private CheckBox btnEternals;
        private CheckBox btnLeather;
        private Button btnSave2;
        private CheckBox btndarkmoonm;
        private CheckBox btnfortunem;
        private CheckBox btnLockboxes;

        public void Guildwithdraw_Shown(object sender, EventArgs e)
        {
            this.Activate();
            Withdrawsettings settings = new Withdrawsettings();
            settings.Loadsettings();
            btnHerbs.Checked = settings.Herbs;
            btnOres.Checked = settings.Ores;
            btnGems.Checked = settings.Gems;
            btnInks.Checked = settings.Inks;
            btnEternals.Checked = settings.Eternals;
            btnLeather.Checked = settings.Leathers;
            btnLockboxes.Checked = settings.Lockboxes;
            btndarkmoonm.Checked = settings.Darkmoonmats;
            btnfortunem.Checked = settings.Fortunemats;
        }

        public void Guildwithdraw_Activated(object sender, EventArgs e)
        {
            this.Activate();
            Withdrawsettings settings = new Withdrawsettings();
            btnHerbs.Checked = settings.Herbs;
            btnOres.Checked = settings.Ores;
            btnGems.Checked = settings.Gems;
            btnInks.Checked = settings.Inks;
            btnEternals.Checked = settings.Eternals;
            btnLeather.Checked = settings.Leathers;
            btnLockboxes.Checked = settings.Lockboxes;
            btndarkmoonm.Checked = settings.Darkmoonmats;
            btnfortunem.Checked = settings.Fortunemats;
        }

        public void btnSave2_Click(object sender, EventArgs e)
        {
            Withdrawsettings settings = new Withdrawsettings();
            settings.Savesettings(btnHerbs.Checked, btnOres.Checked, btnGems.Checked, btnInks.Checked, btnEternals.Checked, btnLeather.Checked, btnLockboxes.Checked, btndarkmoonm.Checked, btnfortunem.Checked);
            btnHerbs.Checked = settings.Herbs;
            btnOres.Checked = settings.Ores;
            btnGems.Checked = settings.Gems;
            btnInks.Checked = settings.Inks;
            btnEternals.Checked = settings.Eternals;
            btnLeather.Checked = settings.Leathers;
            btnLockboxes.Checked = settings.Lockboxes;
            btndarkmoonm.Checked = settings.Darkmoonmats;
            btnfortunem.Checked = settings.Fortunemats;
            Logging.Write(Color.Purple, "Withdraw Settings Saved");
            Close();
        }
    }
    public partial class Guildwithdraw : Form
    {
        #region HBStuff
        public bool useDefault = false;
        public bool settingsLoaded = false;
        public bool Herbs { get; set; }
        public bool Ores { get; set; }
        public bool Gems { get; set; }
        public bool Inks { get; set; }
        public bool Eternals { get; set; }
        public bool Leathers { get; set; }
        public bool Lockboxes { get; set; }
        public bool Darkmoonmats { get; set; }
        public bool Fortunemats { get; set; }
        public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Allrounder"));
        #endregion

        public Guildwithdraw()
        {
            InitializeComponent();
        }

        public void DefaultSettings()
        {
            string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guildwithdraw.xml");
            File.Delete(settingsPath);
            File.Copy(Path.Combine(PluginFolderPath, "Guildwithdraw.xml"), settingsPath);
            Logging.Write(Color.Blue, "Allrounder Guildwithdraw default settings loaded, please use settings button to select your options.");
            useDefault = true;
        }

        public bool PulsesettingsLoad2()
        {
            string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guildwithdraw.xml");
            XElement elm = XElement.Load(settingsPath);
            XElement[] options = elm.Elements("Option").ToArray();
            bool _Ores;
            bool _Herbs;
            bool _Gems;
            bool _Inks;
            bool _Eternals;
            bool _Leathers;
            bool _Lockboxes;
            bool _Darkmoonmats;
            bool _Fortunemats;
            foreach (XElement opt in options)
            {
                var value = opt.Attributes("Value").ToList();
                var set = opt.Attributes("Set").ToList();
                if (value[0].Value == "Herbs")
                {
                    bool.TryParse(set[0].Value, out _Herbs);
                    Herbs = _Herbs;
                }
                if (value[0].Value == "Ores")
                {
                    bool.TryParse(set[0].Value, out _Ores);
                    Ores = _Ores;
                }
                if (value[0].Value == "Gems")
                {
                    bool.TryParse(set[0].Value, out _Gems);
                    Gems = _Gems;
                }
                if (value[0].Value == "Inks")
                {
                    bool.TryParse(set[0].Value, out _Inks);
                    Inks = _Inks;
                }
                if (value[0].Value == "Eternals")
                {
                    bool.TryParse(set[0].Value, out _Eternals);
                    Eternals = _Eternals;
                }
                if (value[0].Value == "Leathers")
                {
                    bool.TryParse(set[0].Value, out _Leathers);
                    Leathers = _Leathers;
                }
                if (value[0].Value == "Lockboxes")
                {
                    bool.TryParse(set[0].Value, out _Lockboxes);
                    Lockboxes = _Lockboxes;
                }
                if (value[0].Value == "Darkmoonmats")
                {
                    bool.TryParse(set[0].Value, out _Darkmoonmats);
                    Darkmoonmats = _Darkmoonmats;
                }
                if (value[0].Value == "Fortunemats")
                {
                    bool.TryParse(set[0].Value, out _Fortunemats);
                    Fortunemats = _Fortunemats;
                }
                
            }
            return true;
        }

        class Withdrawsettings
        #region settings
        {
            public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Allrounder"));
            public bool Herbs { get; set; }
            public bool Ores { get; set; }
            public bool Gems { get; set; }
            public bool Inks { get; set; }
            public bool Eternals { get; set; }
            public bool Leathers { get; set; }
            public bool Lockboxes { get; set; }
            public bool Darkmoonmats { get; set; }
            public bool Fortunemats { get; set; }
            public void Loadsettings()
            {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guildwithdraw.xml");
                if (File.Exists(settingsPath))
                {
                }
                else
                {
                    File.Copy(Path.Combine(PluginFolderPath, "Guildwithdraw.xml"), settingsPath);
                }
                XElement elm = XElement.Load(settingsPath);
                XElement[] options = elm.Elements("Option").ToArray();
                bool _Ores;
                bool _Herbs;
                bool _Gems;
                bool _Inks;
                bool _Eternals;
                bool _Leathers;
                bool _Lockboxes;
                bool _Darkmoonmats;
                bool _Fortunemats;
                foreach (XElement opt in options)
                {
                    var value = opt.Attributes("Value").ToList();
                    var set = opt.Attributes("Set").ToList();
                    if (value[0].Value == "Herbs")
                    {
                        bool.TryParse(set[0].Value, out _Herbs);
                        Herbs = _Herbs;
                    }
                    if (value[0].Value == "Ores")
                    {
                        bool.TryParse(set[0].Value, out _Ores);
                        Ores = _Ores;
                    }
                    if (value[0].Value == "Gems")
                    {
                        bool.TryParse(set[0].Value, out _Gems);
                        Gems = _Gems;
                    }
                    if (value[0].Value == "Inks")
                    {
                        bool.TryParse(set[0].Value, out _Inks);
                        Inks = _Inks;
                    }
                    if (value[0].Value == "Eternals")
                    {
                        bool.TryParse(set[0].Value, out _Eternals);
                        Eternals = _Eternals;
                    }
                    if (value[0].Value == "Leathers")
                    {
                        bool.TryParse(set[0].Value, out _Leathers);
                        Leathers = _Leathers;
                    }
                    if (value[0].Value == "Lockboxes")
                    {
                        bool.TryParse(set[0].Value, out _Lockboxes);
                        Lockboxes = _Lockboxes;
                    }
                    if (value[0].Value == "Darkmoonmats")
                    {
                        bool.TryParse(set[0].Value, out _Darkmoonmats);
                        Darkmoonmats = _Darkmoonmats;
                    }
                    if (value[0].Value == "Fortunemats")
                    {
                        bool.TryParse(set[0].Value, out _Fortunemats);
                        Fortunemats = _Fortunemats;
                    }
                }
            }

            public void Savesettings(
                bool _Herbs,
                bool _Ores,
                bool _Gems,
                bool _Inks,
                bool _Eternals,
                bool _Leathers,
                bool _Lockboxes,
                bool _Darkmoonmats,
                bool _Fortunemats)
            {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Guildwithdraw.xml");
                if (File.Exists(settingsPath))
                {
                }
                else
                {
                    File.Copy(Path.Combine(PluginFolderPath, "Guildwithdraw.xml"), settingsPath);
                }
                XElement saveElm = File.Exists(settingsPath) ? XElement.Load(settingsPath) : new XElement("Settings");
                XElement[] options = saveElm.Elements("Option").ToArray();
                foreach (XElement opt in options)
                {
                    var value = opt.Attributes("Value").ToList();
                    if (value[0].Value == "Herbs")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Herbs"), new XAttribute("Set", _Herbs.ToString())));
                    }
                    if (value[0].Value == "Ores")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Ores"), new XAttribute("Set", _Ores.ToString())));
                    }
                    if (value[0].Value == "Gems")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Gems"), new XAttribute("Set", _Gems.ToString())));
                    }
                    if (value[0].Value == "Inks")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Inks"), new XAttribute("Set", _Inks.ToString())));
                    }
                    if (value[0].Value == "Eternals")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Eternals"), new XAttribute("Set", _Eternals.ToString())));
                    }
                    if (value[0].Value == "Leathers")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Leathers"), new XAttribute("Set", _Leathers.ToString())));
                    }
                    if (value[0].Value == "Lockboxes")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Lockboxes"), new XAttribute("Set", _Lockboxes.ToString())));
                    }
                    if (value[0].Value == "Darkmoonmats")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Darkmoonmats"), new XAttribute("Set", _Darkmoonmats.ToString())));
                    }
                    if (value[0].Value == "Fortunemats")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "Fortunemats"), new XAttribute("Set", _Fortunemats.ToString())));
                    }
                    saveElm.Save(settingsPath);
        #endregion
                }
            }
        }
    }
}

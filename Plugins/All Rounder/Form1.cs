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
    partial class AllrounderConfig
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
            this.btnSewingHelp = new System.Windows.Forms.CheckBox();
            this.btnSkinningHelp = new System.Windows.Forms.CheckBox();
            this.btnMilling = new System.Windows.Forms.CheckBox();
            this.btnInking = new System.Windows.Forms.CheckBox();
            this.btnProspecting = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnMail = new System.Windows.Forms.CheckBox();
            this.btnDisenchant = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGuild = new System.Windows.Forms.CheckBox();
            this.btnDarkmoon = new System.Windows.Forms.CheckBox();
            this.btnGuilddeposit = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAutosmelt = new System.Windows.Forms.CheckBox();
            this.btnFortune = new System.Windows.Forms.CheckBox();
            this.btnPapervend = new System.Windows.Forms.CheckBox();
            this.btnInkVend = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSewingHelp
            // 
            this.btnSewingHelp.AutoSize = true;
            this.btnSewingHelp.Location = new System.Drawing.Point(228, 16);
            this.btnSewingHelp.Name = "btnSewingHelp";
            this.btnSewingHelp.Size = new System.Drawing.Size(106, 17);
            this.btnSewingHelp.TabIndex = 0;
            this.btnSewingHelp.Text = "Use Sewing help";
            this.btnSewingHelp.UseVisualStyleBackColor = true;
            // 
            // btnSkinningHelp
            // 
            this.btnSkinningHelp.AutoSize = true;
            this.btnSkinningHelp.Location = new System.Drawing.Point(16, 62);
            this.btnSkinningHelp.Name = "btnSkinningHelp";
            this.btnSkinningHelp.Size = new System.Drawing.Size(112, 17);
            this.btnSkinningHelp.TabIndex = 0;
            this.btnSkinningHelp.Text = "Use Skinning help";
            this.btnSkinningHelp.UseVisualStyleBackColor = true;
            // 
            // btnMilling
            // 
            this.btnMilling.AutoSize = true;
            this.btnMilling.Location = new System.Drawing.Point(16, 39);
            this.btnMilling.Name = "btnMilling";
            this.btnMilling.Size = new System.Drawing.Size(77, 17);
            this.btnMilling.TabIndex = 0;
            this.btnMilling.Text = "Use Milling";
            this.btnMilling.UseVisualStyleBackColor = true;
            // 
            // btnInking
            // 
            this.btnInking.AutoSize = true;
            this.btnInking.Location = new System.Drawing.Point(16, 16);
            this.btnInking.Name = "btnInking";
            this.btnInking.Size = new System.Drawing.Size(77, 17);
            this.btnInking.TabIndex = 1;
            this.btnInking.Text = "Use Inking";
            this.btnInking.UseVisualStyleBackColor = true;
            // 
            // btnProspecting
            // 
            this.btnProspecting.AutoSize = true;
            this.btnProspecting.Location = new System.Drawing.Point(228, 67);
            this.btnProspecting.Name = "btnProspecting";
            this.btnProspecting.Size = new System.Drawing.Size(104, 17);
            this.btnProspecting.TabIndex = 2;
            this.btnProspecting.Text = "Use Prospecting";
            this.btnProspecting.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(286, 306);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnMail
            // 
            this.btnMail.AutoSize = true;
            this.btnMail.Location = new System.Drawing.Point(245, 231);
            this.btnMail.Name = "btnMail";
            this.btnMail.Size = new System.Drawing.Size(67, 17);
            this.btnMail.TabIndex = 8;
            this.btnMail.Text = "Use Mail";
            this.btnMail.UseVisualStyleBackColor = true;
            // 
            // btnDisenchant
            // 
            this.btnDisenchant.AutoSize = true;
            this.btnDisenchant.Location = new System.Drawing.Point(228, 39);
            this.btnDisenchant.Name = "btnDisenchant";
            this.btnDisenchant.Size = new System.Drawing.Size(102, 17);
            this.btnDisenchant.TabIndex = 9;
            this.btnDisenchant.Text = "Use Disenchant";
            this.btnDisenchant.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(273, 349);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Allrounder By Alpha";
            // 
            // btnGuild
            // 
            this.btnGuild.AutoSize = true;
            this.btnGuild.Location = new System.Drawing.Point(16, 254);
            this.btnGuild.Name = "btnGuild";
            this.btnGuild.Size = new System.Drawing.Size(114, 17);
            this.btnGuild.TabIndex = 11;
            this.btnGuild.Text = "Use Guildwithdraw";
            this.btnGuild.UseVisualStyleBackColor = true;
            // 
            // btnDarkmoon
            // 
            this.btnDarkmoon.AutoSize = true;
            this.btnDarkmoon.Location = new System.Drawing.Point(16, 85);
            this.btnDarkmoon.Name = "btnDarkmoon";
            this.btnDarkmoon.Size = new System.Drawing.Size(105, 17);
            this.btnDarkmoon.TabIndex = 15;
            this.btnDarkmoon.Text = "Make Darkmoon";
            this.btnDarkmoon.UseVisualStyleBackColor = true;
            // 
            // btnGuilddeposit
            // 
            this.btnGuilddeposit.AutoSize = true;
            this.btnGuilddeposit.Location = new System.Drawing.Point(245, 254);
            this.btnGuilddeposit.Name = "btnGuilddeposit";
            this.btnGuilddeposit.Size = new System.Drawing.Size(106, 17);
            this.btnGuilddeposit.TabIndex = 18;
            this.btnGuilddeposit.Text = "Use Guilddeposit";
            this.btnGuilddeposit.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 339);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 19;
            this.button1.Text = "Withdraw";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(97, 339);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 20;
            this.button2.Text = "Deposit";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 311);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Guild Controls";
            // 
            // btnAutosmelt
            // 
            this.btnAutosmelt.AutoSize = true;
            this.btnAutosmelt.Location = new System.Drawing.Point(228, 90);
            this.btnAutosmelt.Name = "btnAutosmelt";
            this.btnAutosmelt.Size = new System.Drawing.Size(94, 17);
            this.btnAutosmelt.TabIndex = 22;
            this.btnAutosmelt.Text = "Use Autosmelt";
            this.btnAutosmelt.UseVisualStyleBackColor = true;
            // 
            // btnFortune
            // 
            this.btnFortune.AutoSize = true;
            this.btnFortune.Location = new System.Drawing.Point(16, 108);
            this.btnFortune.Name = "btnFortune";
            this.btnFortune.Size = new System.Drawing.Size(84, 17);
            this.btnFortune.TabIndex = 24;
            this.btnFortune.Text = "Use Fortune";
            this.btnFortune.UseVisualStyleBackColor = true;
            // 
            // btnPapervend
            // 
            this.btnPapervend.AutoSize = true;
            this.btnPapervend.Location = new System.Drawing.Point(16, 231);
            this.btnPapervend.Name = "btnPapervend";
            this.btnPapervend.Size = new System.Drawing.Size(118, 17);
            this.btnPapervend.TabIndex = 25;
            this.btnPapervend.Text = "Use Paper Vendors";
            this.btnPapervend.UseVisualStyleBackColor = true;
            // 
            // btnInkVend
            // 
            this.btnInkVend.AutoSize = true;
            this.btnInkVend.Location = new System.Drawing.Point(16, 208);
            this.btnInkVend.Name = "btnInkVend";
            this.btnInkVend.Size = new System.Drawing.Size(105, 17);
            this.btnInkVend.TabIndex = 26;
            this.btnInkVend.Text = "Use Ink Vendors";
            this.btnInkVend.UseVisualStyleBackColor = true;
            // 
            // AllrounderConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 374);
            this.Controls.Add(this.btnInkVend);
            this.Controls.Add(this.btnPapervend);
            this.Controls.Add(this.btnFortune);
            this.Controls.Add(this.btnAutosmelt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGuilddeposit);
            this.Controls.Add(this.btnDarkmoon);
            this.Controls.Add(this.btnGuild);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDisenchant);
            this.Controls.Add(this.btnMail);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnProspecting);
            this.Controls.Add(this.btnInking);
            this.Controls.Add(this.btnMilling);
            this.Controls.Add(this.btnSkinningHelp);
            this.Controls.Add(this.btnSewingHelp);
            this.Name = "AllrounderConfig";
            this.Text = "Allrounder";
            this.Load += new System.EventHandler(this.Form1_Activated);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox btnSewingHelp;
        private CheckBox btnSkinningHelp;
        private CheckBox btnMilling;
        private CheckBox btnInking;
        private CheckBox btnProspecting;
        private CheckBox btnMail;
        private CheckBox btnDisenchant;
        private Label label1;
        private CheckBox btnGuild;
        private CheckBox btnDarkmoon;
        private CheckBox btnGuilddeposit;
        private Button button1;
        private Button button2;
        private Label label2;
        private CheckBox btnAutosmelt;
        private CheckBox btnFortune;
        private CheckBox btnPapervend;
        private CheckBox btnInkVend;
        private Button btnSave;
    }

    public partial class AllrounderConfig : Form
    {
        public AllrounderConfig()
        {
            InitializeComponent();
            this.Activate();
            Logging.Write(Color.White, "[Allrounder]:Welcome to Allrounder v3 : Time to craft some SHIT!!");
        }
        public void button1_Click(object sender, EventArgs e)
        {
            Guildwithdraw form2 = new Guildwithdraw();
            form2.ShowDialog();
            form2.Dispose();
        }
        public void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
            form3.Dispose();
        }
        AllrounderSettings settings;
        public void Form1_Shown(object sender, EventArgs e)
        {
            this.Activate();
            AllrounderSettings settings = new AllrounderSettings();
            settings.LoadSettings();          
            btnSewingHelp.Checked = settings.useSewingHelp;
            btnSkinningHelp.Checked = settings.useSkinningHelp;
            btnMilling.Checked = settings.useMilling;
            btnInking.Checked = settings.useInking;
            btnProspecting.Checked = settings.useProspecting;                     
            btnMail.Checked = settings.useMail;
            btnDisenchant.Checked = settings.useDisenchant;
            btnGuild.Checked = settings.useGuildw;
            btnGuilddeposit.Checked = settings.useGuilddeposit;
            btnDarkmoon.Checked = settings.useDarkmoon;
            btnAutosmelt.Checked = settings.useAutosmelt;
            btnFortune.Checked = settings.useFortune;
        }
        public void Form1_Activated(object sender, EventArgs e)
        {
            this.Activate();
            AllrounderSettings settings = new AllrounderSettings();
            settings.LoadSettings();
            
            btnSewingHelp.Checked = settings.useSewingHelp;
            btnSkinningHelp.Checked = settings.useSkinningHelp;
            btnMilling.Checked = settings.useMilling;
            btnInking.Checked = settings.useInking;
            btnProspecting.Checked = settings.useProspecting;
            btnMail.Checked = settings.useMail;
            btnDisenchant.Checked = settings.useDisenchant;
            btnGuild.Checked = settings.useGuildw;
            btnGuilddeposit.Checked = settings.useGuilddeposit;
            btnDarkmoon.Checked = settings.useDarkmoon;
            btnAutosmelt.Checked = settings.useAutosmelt;
            btnFortune.Checked = settings.useFortune;
        }
        public void btnSave_Click(object sender, EventArgs e)
        {
            AllrounderSettings settings = new AllrounderSettings();
            settings.SaveSettings(btnSewingHelp.Checked, btnSkinningHelp.Checked, btnInking.Checked,  btnMilling.Checked, btnProspecting.Checked, btnMail.Checked, btnDisenchant.Checked, btnGuild.Checked, btnDarkmoon.Checked, btnGuilddeposit.Checked, btnAutosmelt.Checked, btnFortune.Checked, btnInkVend.Checked, btnPapervend.Checked);            
            btnSewingHelp.Checked = settings.useSewingHelp;
            btnSkinningHelp.Checked = settings.useSkinningHelp;
            btnMilling.Checked = settings.useMilling;
            btnInking.Checked = settings.useInking;
            btnProspecting.Checked = settings.useProspecting;
            btnMail.Checked = settings.useMail;
            btnDisenchant.Checked = settings.useDisenchant;
            btnGuild.Checked = settings.useGuildw;
            btnGuilddeposit.Checked = settings.useGuilddeposit;
            btnDarkmoon.Checked = settings.useDarkmoon;
            btnAutosmelt.Checked = settings.useAutosmelt;
            btnFortune.Checked = settings.useFortune;
            Logging.Write(Color.DarkGreen, "Allrounder Settings Saved");
            Close();
        }

        #region HBStuff
        public class Allrounder : HBPlugin
        {      
            public static AllrounderConfig Form = new AllrounderConfig();
            public override string Name { get { return "Allrounder"; } }
            public override string Author { get { return "Alpha"; } }
            public override Version Version { get { return _version; } }
            private readonly Version _version = new Version(3, 1);
            public override string ButtonText { get { return "Settings"; } }
            public override bool WantButton { get { return true; } }
            public override void OnButtonPress()
            {
                Form.ShowDialog();
            }
            public WoWUnit Me = Styx.StyxWoW.Me;
            public static WoWPoint Finish = new WoWPoint(-8903.335, 630.2755, 99.60458);
            
            public bool useDefault = false;
            public bool useSewingHelp { get; set; }
            public bool useSkinningHelp { get; set; }
            public bool useMilling { get; set; }
            public bool useInking { get; set; }
            public bool useProspecting { get; set; }
            public bool useMail { get; set; }
            public bool useDisenchant { get; set; }
            public bool useGuildw { get; set; }
            public bool useDarkmoon { get; set; }
            public bool useGuilddeposit { get; set; }
            public bool useAutosmelt { get; set; }
            public bool useEternals { get; set; }
            public bool useFortune { get; set; }
            public bool usePapervend { get; set; }
            public bool useInkvend { get; set; }
            public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Allrounder"));
            #endregion
            public override void Pulse()
            {
                while (!StyxWoW.Me.Combat && !StyxWoW.Me.Dead && !StyxWoW.Me.IsInInstance && !StyxWoW.Me.IsFlying && !StyxWoW.Me.IsFalling && StyxWoW.Me.IsAlive)
                {
                    if(useSewingHelp || useSkinningHelp || useGuildw || useMail || useMilling || useInking || useProspecting || useEternals || useDisenchant || useAutosmelt || useDarkmoon || useGuilddeposit || useFortune || usePapervend || useInkvend)
                        ObjectManager.Update();

                    if (useDefault)
                    {
                        DefaultSettings();
                    }
                    
                    Guildwithdraw gw = new Guildwithdraw();
                    Form3 dw = new Form3();
                    Pulsesettingsload();

                    while (Me.Location != Finish && useGuildw || useGuilddeposit)
                    {
                        Logging.Write("Moving To Start Point");
                        Navigator.MoveTo(Finish);
                        if (Me.Location.Distance(Finish) < 10)
                        {
                            break;
                        }
                    }

                    if (useGuildw)
                    {
                        gw.PulsesettingsLoad2();
                        Guildwith.Guildwithspell();
                    }

                    if (useMail)
                    {
                        Mail.Mailspell();
                    }
                    if (useMilling)
                    {
                        Milling.milling();
                    }
                    if (useInking)
                    {
                        Inking.Inkingspell();
                    }
                    if (useSewingHelp)
                    {
                        Sewing.SewingDo();
                    }
                    if (useSkinningHelp)
                    {
                        Skinning.SkinningDo();
                    }
                    if (useProspecting)
                    {
                        Prospecting.Prospectingspell();
                    }
                    if (useDisenchant)
                    {
                        Disenchant.Disenchantspell();
                    }
                    if (useAutosmelt)
                    {
                        Autosmelt.UseNearestAnvil();
                        Autosmelt.Autosmeltspell();
                    }
                    if (usePapervend)
                    {
                        DarkVendors.PaperVendors();
                    }
                    if (useInkvend)
                    {
                        DarkVendors.Inkvendor();
                    }
                    if (useDarkmoon)
                    {
                        Darkmoon.Darkmoonspell();
                    }
                    if (useFortune)
                    {
                        Fortunespell.useFortunespell();
                    }

                    while (Me.Location != Finish && useGuildw || useGuilddeposit)
                    {
                        if (Me.Location.Distance(Finish) > 5)
                        {
                            Navigator.MoveTo(Finish);
                            Logging.Write("Moving To Finish Point");
                            if (Me.Location.Distance(Finish) < 10)
                            {
                                break;
                            }
                        }
                    }
                    if (useGuilddeposit)
                    {
                        dw.PulseLoadsettings3();
                        Guilddeposit.Guilddepositspell();
                    }
                }
            }   
                
            

            public void DefaultSettings()
            {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Settings.xml");
                File.Delete(settingsPath);
                File.Copy(Path.Combine(PluginFolderPath, "Settings.xml"), settingsPath);
                Logging.Write(Color.Blue, "Allrounder default settings loaded, please use settings button to select your options.");
                useDefault = true;
            }

            public void Pulsesettingsload()
            {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Settings.xml");
                XElement elm = XElement.Load(settingsPath);
                XElement[] options = elm.Elements("Option").ToArray();
                bool _useSewingHelp;
                bool _useSkinningHelp;
                bool _useMilling;
                bool _useInking;
                bool _useProspecting;              
                bool _useMail;
                bool _useDisenchant;
                bool _useGuildw;              
                bool _useDarkmoon;                
                bool _useGuilddeposit;
                bool _useAutosmelt;
                bool _useFortune;
                bool _useInkvend;
                bool _usePapervend;
               
                foreach (XElement opt in options)
                {
                    var value = opt.Attributes("Value").ToList();
                    var set = opt.Attributes("Set").ToList();

                    if (value[0].Value == "usePapervend")
                    {
                        bool.TryParse(set[0].Value, out _usePapervend);
                        usePapervend = _usePapervend;
                    } 
                    if (value[0].Value == "useInkvend")
                    {
                        bool.TryParse(set[0].Value, out _useInkvend);
                        useInkvend = _useInkvend;
                    }
                    if (value[0].Value == "useFortune")
                    {
                        bool.TryParse(set[0].Value, out _useFortune);
                        useFortune = _useFortune;
                    }
                    if (value[0].Value == "useSewingHelp")
                    {
                        bool.TryParse(set[0].Value, out _useSewingHelp);
                        useSewingHelp = _useSewingHelp;
                    }
                    if (value[0].Value == "useSkinningHelp")
                    {
                        bool.TryParse(set[0].Value, out _useSkinningHelp);
                        useSkinningHelp = _useSkinningHelp;
                    }
                    if (value[0].Value == "useMilling")
                    {
                        bool.TryParse(set[0].Value, out _useMilling);
                        useMilling = _useMilling;
                    }
                    if (value[0].Value == "useInking")
                    {
                        bool.TryParse(set[0].Value, out _useInking);
                        useInking = _useInking;
                    }
                    if (value[0].Value == "useProspecting")
                    {
                        bool.TryParse(set[0].Value, out _useProspecting);
                        useProspecting = _useProspecting;
                    }
                    if (value[0].Value == "useMail")
                    {
                        bool.TryParse(set[0].Value, out _useMail);
                        useMail = _useMail;
                    }
                    if (value[0].Value == "useDisenchant")
                    {
                        bool.TryParse(set[0].Value, out _useDisenchant);
                        useDisenchant = _useDisenchant;
                    }
                    if (value[0].Value == "useGuildw")
                    {
                        bool.TryParse(set[0].Value, out _useGuildw);
                        useGuildw = _useGuildw;
                    }
                    if (value[0].Value == "useDarkmoon")
                    {
                        bool.TryParse(set[0].Value, out _useDarkmoon);
                        useDarkmoon = _useDarkmoon;
                    }
                    if (value[0].Value == "useGuilddeposit")
                    {
                        bool.TryParse(set[0].Value, out _useGuilddeposit);
                        useGuilddeposit = _useGuilddeposit;
                    }
                    if (value[0].Value == "useAutosmelt")
                    {
                        bool.TryParse(set[0].Value, out _useAutosmelt);
                        useAutosmelt = _useAutosmelt;
                    }
                    
                }
            }
        }

        class AllrounderSettings
        #region Settings
        {
            public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Allrounder"));
           
            public bool useSewingHelp { get; set; }
            public bool useSkinningHelp { get; set; }
            public bool useMilling { get; set; }
            public bool useInking { get; set; }
            public bool useProspecting { get; set; }
            public bool useMail { get; set; }
            public bool useDisenchant { get; set; }
            public bool useGuildw { get; set; }
            public bool useDarkmoon { get; set; }
            public bool useGuilddeposit { get; set; }
            public bool useAutosmelt { get; set; }
            public bool useFortune { get; set; }
            public bool usePapervend { get; set; }
            public bool useInkvend { get; set; }
            public void LoadSettings()
            {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Settings.xml");
                if (File.Exists(settingsPath))
                {
                }
                else
                {
                    File.Copy(Path.Combine(PluginFolderPath, "Settings.xml"), settingsPath);
                }
                XElement elm = XElement.Load(settingsPath);
                XElement[] options = elm.Elements("Option").ToArray();
                
                bool _useSewingHelp;
                bool _useSkinningHelp;
                bool _useInking;
                bool _useMilling;
                bool _useProspecting;
                bool _useMail;
		        bool _useDisenchant;
                bool _useGuildw;
                bool _useDarkmoon;
                bool _useGuilddeposit;
                bool _useAutosmelt;
                bool _useFortune;
                bool _usePapervend;
                bool _useInkvend;
                foreach (XElement opt in options)
                {
                    var value = opt.Attributes("Value").ToList();
                    var set = opt.Attributes("Set").ToList();

                    if (value[0].Value == "usePapervend")
                    {
                        bool.TryParse(set[0].Value, out _usePapervend);
                        usePapervend = _usePapervend;
                    }
                    if (value[0].Value == "useInkvend")
                    {
                        bool.TryParse(set[0].Value, out _useInkvend);
                        useInkvend = _useInkvend;
                    }
                    if (value[0].Value == "useFortune")
                    {
                        bool.TryParse(set[0].Value, out _useFortune);
                        useFortune = _useFortune;
                    }
                    if (value[0].Value == "useDisenchant")
                    {
                        bool.TryParse(set[0].Value, out _useDisenchant);
                        useDisenchant = _useDisenchant;
                    }
                    if (value[0].Value == "useSewingHelp")
                    {
                        bool.TryParse(set[0].Value, out _useSewingHelp);
                        useSewingHelp = _useSewingHelp;
                    }
                    if (value[0].Value == "useSkinningHelp")
                    {
                        bool.TryParse(set[0].Value, out _useSkinningHelp);
                        useSkinningHelp = _useSkinningHelp;
                    }
                    if (value[0].Value == "useMilling")
                    {
                        bool.TryParse(set[0].Value, out _useMilling);
                        useMilling = _useMilling;
                    }
                    if (value[0].Value == "useInking")
                    {
                        bool.TryParse(set[0].Value, out _useInking);
                        useInking = _useInking;
                    }
                    if (value[0].Value == "useProspecting")
                    {
                        bool.TryParse(set[0].Value, out _useProspecting);
                        useProspecting = _useProspecting;
                    }
                    if (value[0].Value == "useMail")
                    {
                        bool.TryParse(set[0].Value, out _useMail);
                        useMail = _useMail;
                    }
                    if (value[0].Value == "useGuildw")
                    {
                        bool.TryParse(set[0].Value, out _useGuildw);
                        useGuildw = _useGuildw;
                    }
                    if (value[0].Value == "useDarkmoon")
                    {
                        bool.TryParse(set[0].Value, out _useDarkmoon);
                        useDarkmoon = _useDarkmoon;
                    }
                    if (value[0].Value == "useGuilddeposit")
                    {
                        bool.TryParse(set[0].Value, out _useGuilddeposit);
                        useGuilddeposit = _useGuilddeposit;
                    }
                    if (value[0].Value == "useAutosmelt")
                    {
                        bool.TryParse(set[0].Value, out _useAutosmelt);
                        useAutosmelt = _useAutosmelt;
                    }
                    
                }
            }
            public void SaveSettings(
                bool _useSewingHelp,
                bool _useSkinningHelp,
                bool _useInking,
                bool _useMilling,
                bool _useProspecting,
                bool _useMail,
                bool _useDisenchant,
                bool _useGuildw,
                bool _useDarkmoon,
                bool _useGuilddeposit,
                bool _useAutosmelt,
                bool _useFortune,
                bool _useInkvend,
                bool _usePapervend)
                {
                string settingsPath = Path.Combine(PluginFolderPath, StyxWoW.Me.Name + "-Settings.xml");
                if (File.Exists(settingsPath))
                {
                }
                else
                {
                    File.Copy(Path.Combine(PluginFolderPath, "Settings.xml"), settingsPath);
                }
                XElement saveElm = File.Exists(settingsPath) ? XElement.Load(settingsPath) : new XElement("Settings");
                XElement[] options = saveElm.Elements("Option").ToArray();
                foreach (XElement opt in options)
                {
                    var value = opt.Attributes("Value").ToList();

                    if (value[0].Value == "usePapervend")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "usePapervend"), new XAttribute("Set", _usePapervend.ToString())));
                    } 
                    if (value[0].Value == "useInkvend")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useInkvend"), new XAttribute("Set", _useInkvend.ToString())));
                    }
                    if (value[0].Value == "useFortune")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useFortune"), new XAttribute("Set", _useFortune.ToString())));
                    }
                    if (value[0].Value == "useSewingHelp")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useSewingHelp"), new XAttribute("Set", _useSewingHelp.ToString())));
                    }
                    if (value[0].Value == "useSkinningHelp")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useSkinningHelp"), new XAttribute("Set", _useSkinningHelp.ToString())));
                    }
                    if (value[0].Value == "useMilling")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useMilling"), new XAttribute("Set", _useMilling.ToString())));
                    }
                    if (value[0].Value == "useInking")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useInking"), new XAttribute("Set", _useInking.ToString())));
                    }
                    if (value[0].Value == "useProspecting")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useProspecting"), new XAttribute("Set", _useProspecting.ToString())));
                    }
                    if (value[0].Value == "useMail")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useMail"), new XAttribute("Set", _useMail.ToString())));
                    }
                    if (value[0].Value == "useDisenchant")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useDisenchant"), new XAttribute("Set", _useDisenchant.ToString())));
                    }
                    if (value[0].Value == "useGuildw")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useGuildw"), new XAttribute("Set", _useGuildw.ToString())));
                    }
                    if (value[0].Value == "useDarkmoon")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useDarkmoon"), new XAttribute("Set", _useDarkmoon.ToString())));
                    }
                    if (value[0].Value == "useGuilddeposit")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useGuilddeposit"), new XAttribute("Set", _useGuilddeposit.ToString())));
                    }
                    if (value[0].Value == "useAutosmelt")
                    {
                        opt.ReplaceWith(new XElement("Option", new XAttribute("Value", "useAutosmelt"), new XAttribute("Set", _useAutosmelt.ToString())));
                    }
                    saveElm.Save(settingsPath);
        #endregion
                }
            }
        }
    }
}

       






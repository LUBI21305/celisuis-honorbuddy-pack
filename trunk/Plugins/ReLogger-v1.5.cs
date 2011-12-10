using System;
using System.Threading;
using System.Windows.Forms;
using Styx;
using Styx.Helpers;
using Styx.Plugins;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;
using Styx.Logic.BehaviorTree;
using System.Drawing;

namespace Relogger
{

    #region form

    public class SettingsForm : Form
    {
        private FlowLayoutPanel panel = new FlowLayoutPanel();
        private Label LabelUsername = new Label();
        private Label LabelPassword = new Label();
        private Label LabelAccount = new Label();
        private Label LabelRealm = new Label();
        private Label LabelCharacter = new Label();
        private Label LabelTries = new Label();
        private TextBox TextUsername = new TextBox();
        private TextBox TextPassword = new TextBox();
        private TextBox TextAccount = new TextBox();
        private TextBox TextRealm = new TextBox();
        private TextBox TextCharacter = new TextBox();
        private TextBox TextTries = new TextBox();

        public SettingsForm()
        {
            LabelUsername.Text = "Username:";
            LabelPassword.Text = "Password:";
            LabelAccount.Text = "Account:";
            LabelRealm.Text = "Realm:";
            LabelCharacter.Text = "Character:";
            LabelTries.Text = "Tries:";
            TextUsername.Text = ReLogger.settings.Username;
            TextUsername.Width = 190;
            TextPassword.Text = ReLogger.settings.Password;
            TextPassword.Width = 190;
            TextAccount.Text = ReLogger.settings.Account;
            TextAccount.Width = 190;
            TextRealm.Text = ReLogger.settings.Realm;
            TextRealm.Width = 190;
            TextCharacter.Text = ReLogger.settings.Character;
            TextCharacter.Width = 190;
            TextTries.Text = ReLogger.settings.Tries.ToString();
            TextTries.Width = 190;
            panel.Dock = DockStyle.Fill;
            panel.Controls.Add(LabelUsername);
            panel.Controls.Add(TextUsername);
            panel.Controls.Add(LabelPassword);
            panel.Controls.Add(TextPassword);
            panel.Controls.Add(LabelAccount);
            panel.Controls.Add(TextAccount);
            panel.Controls.Add(LabelRealm);
            panel.Controls.Add(TextRealm);
            panel.Controls.Add(LabelCharacter);
            panel.Controls.Add(TextCharacter);
            panel.Controls.Add(LabelTries);
            panel.Controls.Add(TextTries);
            this.Text = "Relogger Settings";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Height = 190;
            this.Width = 320;
            this.Controls.Add(panel);
        }

        protected override void Dispose(bool disposing)
        {
            ReLogger.settings.Username = TextUsername.Text;
            ReLogger.settings.Password = TextPassword.Text;
            ReLogger.settings.Account = TextAccount.Text;
            ReLogger.settings.Realm = TextRealm.Text;
            ReLogger.settings.Character = TextCharacter.Text;
            try 
            {
                ReLogger.settings.Tries = int.Parse(TextTries.Text);
            } catch(Exception) 
            {
                ReLogger.settings.Tries = 10;
            }
            ReLogger.settings.Save();
            base.Dispose(disposing);
        }
    }

    #endregion

    #region settings

    class ReloggerSettings : Settings
    {
        public ReloggerSettings() : base(Logging.ApplicationPath + "\\Settings\\Relogger_" + StyxWoW.Me.Name + ".xml") 
        {
            Load();
        }

        [Setting, DefaultValue("")]
        public string Username { get; set; }

        [Setting, DefaultValue("")]
        public string Password { get; set; }

        [Setting, DefaultValue("")]
        public string Account { get; set; }

        [Setting, DefaultValue("")]
        public string Realm { get; set; }

        [Setting, DefaultValue("")]
        public string Character { get; set; }

        [Setting, DefaultValue(10)]
        public int Tries { get; set; }  
    }

    #endregion

    #region relogger

    class ReLogger : HBPlugin
    {

        #region variables

        public static ReloggerSettings settings = new ReloggerSettings();
        private Thread _t;
        private bool wasloggedout = false;

        #endregion

        #region lua

        #region is

        private bool IsLoginScreen() {
            return Lua.GetReturnVal<bool>("if AccountLoginUI then return AccountLoginUI:IsVisible() else return false end ", 0);
        }

        private bool IsAccountSelectScreen()
        {
            return Lua.GetReturnVal<bool>("if WoWAccountSelectDialog then return WoWAccountSelectDialog:IsShown() else return false end ", 0);
        }

        private bool IsRealmSelectScreen()
        {
            return Lua.GetReturnVal<bool>("if RealmList then return RealmList:IsVisible() else return false end ", 0);
        }

        private bool IsCharacterSelectScreen()
        {
            return Lua.GetReturnVal<bool>("if CharacterSelectUI then return CharacterSelectUI:IsVisible() else return false end ", 0);
        }

        #endregion

        #region do

        private bool DoHideFrame(string frame)
        {
            bool b = Lua.GetReturnVal<bool>("local b = false " +
                                            "if " + frame + " and " + frame + ":IsVisible() then " +
                                                " b = 1 " +
                                                frame + ":Hide() " +
                                            "end " + 
                                            "return b ", 0);
            return b;
        }

        private void DoLogin()
        {
            Lua.DoString("DefaultServerLogin(\"" + settings.Username + "\", \"" + settings.Password + "\")");
        }

        private bool DoAccountSelect()
        {
            bool b = Lua.GetReturnVal<bool>
                        ("local b = false " +
                        "for i = 1, GetNumGameAccounts() do " +
                            "if string.lower(GetGameAccountInfo(i)) == string.lower(\"" + settings.Account + "\") then " +
                                "WoWAccountSelect_SelectAccount(i) " +
                                "b = 1 " +
                                "break " +
                            "end " +
                        "end " +
                        "return b; ", 0);
            return b;
        }

        private bool DoRealmSelect()
        {
            bool b = Lua.GetReturnVal<bool>
                        ("local b = false " + 
                         "for i = 1, select('#',GetRealmCategories()) do " +
                            "for j = 1, GetNumRealms(i) do " +
                                "if string.lower(GetRealmInfo(i, j)) == string.lower(\"" + settings.Realm + "\") then " +
                                    "RealmList:Hide() " +
                                    "ChangeRealm(i, j) " +
                                    "b = 1 " +
                                    "break " +
                                "end " +
                             "end " +
                         "end " +
                         "return b; ", 0);
            return b;
        }

        private bool DoCharacterSelect()
        {
            bool b = Lua.GetReturnVal<bool>
                       ("local b = false " + 
                        "if string.lower(GetServerName()) == string.lower(\"" + settings.Realm + "\") then " +
                            "if GetNumCharacters() > 0 then " +
                                "for i = 1,GetNumCharacters() do " +
                                    "if string.lower(GetCharacterInfo(i)) == string.lower(\"" + settings.Character + "\") then " +
                                        "CharacterSelect_SelectCharacter(i) " +
                                        "EnterWorld() " +
                                        "b = 1 " +
                                        "break " +
                                    "end " +
                                "end " +
                            "end " + 
                        "else " +
                            "RequestRealmList(1) " +
                            "b = 1 " +
                        "end " +
                        "return b; ", 0);
            return b;
        }

        #endregion

        #endregion

        #region helper

        private void log(string s)
        {
            Logging.Write(Color.DarkCyan, "[" + Name + "] " + s);
        }

        #endregion
        
        #region overrides

        public override void Dispose()
        {
            if (_t != null)
            {
                log("Cleaning up");
                try
                {
                    _t.Interrupt();
                    _t.Abort();
                }
                catch (Exception) { }
                _t = null;
            }
        }

        public override string Author
        {
            get { return "eXemplar"; }
        }

        public override string Name
        {
            get { return "ReLogger"; }
        }

        public override System.Version Version
        {
            get { return new System.Version(1, 5); }
        }

        public override void Pulse()
        {
            if (IsPluginEnabled())
            {
                if (_t == null || !_t.IsAlive)
                {
                    log("Creating thread");
                    _t = new Thread(new ThreadStart(run));
                    _t.IsBackground = true;
                    _t.Start();
                }
            }
            else
            {
                if (_t != null && _t.IsAlive)
                {
                    log("Stoppping thread");
                    _t.Interrupt();
                    //_t.Abort();
                }
            }
        }

        public override bool WantButton
        {
            get
            {
                Pulse();
                return IsPluginEnabled();
            }
        }

        public override string ButtonText
        {
            get
            {
                return "Settings";
            }
        }

        public override void OnButtonPress()
        {
            new SettingsForm().Show();
        }

        #endregion

        #region thread

        private void run()
        {
            int tries = 0;
            while (IsPluginEnabled())
            {
                try
                {
                    Thread.Sleep(10000);
                    if (DoHideFrame("ScriptErrorsFrame"))
                    {
                        log("Hiding ScriptErrorsFrame");
                    }
                    if (StyxWoW.IsInWorld)
                    {
                        if (wasloggedout && !TreeRoot.IsRunning)
                        {
                            log("Restarting Bot");
                            wasloggedout = false;
                            TreeRoot.Start();
                            continue;
                        }
                        if (tries > 0)
                        {
                            log("Resetting tries");
                            tries = 0;
                        }
                    }
                    if (StyxWoW.IsInGame)
                    {
                        continue;
                    }
                    wasloggedout = true;
                    if (tries >= settings.Tries)
                    {
                        log("Too many tries, aborting - " + tries);
                        GetPlugin().Enabled = false;
                        break;
                    }
                    if (TreeRoot.IsRunning)
                    {
                        log("Stopping Bot");
                        TreeRoot.Stop();
                        continue;
                    }
                    if (DoHideFrame("StaticPopup1"))
                    {
                        log("Hiding StaticPopup");
                        continue;
                    }
                    if (IsAccountSelectScreen())
                    {
                        log("Account selection - " + settings.Account);
                        if (DoAccountSelect())
                        {
                            log("Account selection - OK!");
                        }
                        else
                        {
                            log("Account selection - ERROR!");
                            GetPlugin().Enabled = false;
                            break;
                        }
                    }
                    else if (IsLoginScreen())
                    {
                        tries++;
                        log("Logging in - " + settings.Username + " (" + tries  + ")");
                        DoLogin();
                    }
                    else if (IsRealmSelectScreen())
                    {
                        log("Realm selection - " + settings.Realm);
                        if (DoRealmSelect())
                        {
                            log("Realm selection - OK!");
                        }
                        else
                        {
                            log("Realm selection - ERROR!");
                            GetPlugin().Enabled = false;
                            break;
                        }
                    }
                    else if (IsCharacterSelectScreen())
                    {
                        log("Character selection - " + settings.Character);
                        if (DoCharacterSelect())
                        {
                            log("Character selection - OK!");
                        }
                        else
                        {
                            log("Character selection - ERROR!");
                            GetPlugin().Enabled = false;
                            break;
                        }
                    }
                }
                catch (Exception) { }
            }
            log("Thread finished");
        }

        #endregion

        #region plugin

        private PluginContainer GetPlugin() {
            foreach (PluginContainer p in PluginManager.Plugins)
            {
                if (p != null && p.Name == Name)
                {
                    return p;
                }
            }
            return null;
        }

        private bool IsPluginEnabled()
        {
            PluginContainer p = GetPlugin();
            if (p == null)
            {
                return false;
            }
            return p.Enabled;
        }

        #endregion

    }

    #endregion

}

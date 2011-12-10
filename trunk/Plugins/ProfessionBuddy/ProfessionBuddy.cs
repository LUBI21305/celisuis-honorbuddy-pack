//!CompilerOption:Optimize:On
//!CompilerOption:AddRef:WindowsBase.dll

// Professionbuddy plugin by HighVoltz

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Packaging;

using Styx;
using TreeSharp;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.Logic;
using Styx.Logic.Combat;
using System.Diagnostics;
using Styx.Patchables;
using Styx.Plugins;
using Styx.Plugins.PluginClass;
using Styx.Logic.Pathing;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals.WoWObjects;
using CommonBehaviors.Actions;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Styx.Combat.CombatRoutine;
using Styx.Logic.POI;
using HighVoltz.Composites;
using System.Reflection;
using Styx.Logic.Profiles;
using System.Collections.Specialized;
using Action = TreeSharp.Action;
using ObjectManager = Styx.WoWInternals.ObjectManager;
using System.Globalization;
using System.Security.Cryptography;

namespace HighVoltz
{
    public partial class Professionbuddy : HBPlugin
    {
        #region Declarations
        public ProfessionBuddySettings MySettings;
        public List<TradeSkill> TradeSkillList { get; private set; }
        // <itemId,count>
        public DataStore DataStore { get; private set; }
        // dictionary that keeps track of material list using item ID for key and number required as value
        public Dictionary<uint, int> MaterialList { get; private set; }
        public List<uint> ProtectedItems { get; private set; }
        public bool IsTradeSkillsLoaded { get; private set; }

        static string _pluginPath;
        public string PluginPath { get { return _pluginPath; } }

        static string _profilePath;
        public string ProfilePath { get { return _profilePath; } }

        static string _tempFolder;
        public string TempFolder { get { return _tempFolder; } }

        public event EventHandler OnTradeSkillsLoaded;
        public readonly LocalPlayer Me = ObjectManager.Me;
        Svn _svn = new Svn();
        // DataStore is an addon for WOW thats stores bag/ah/mail item info and more.
        public bool HasDataStoreAddon { get { return DataStore != null ? DataStore.HasDataStoreAddon : false; } }
        // profile Settings.
        public PbProfileSettings ProfileSettings { get; private set; }
        public bool IsRunning = false;
        // singleton instance
        public static Professionbuddy Instance { get; private set; }

        // test some culture specific stuff.
        public Professionbuddy()
        {
            Instance = this;
            new Thread((ThreadStart)delegate
                {
                    try
                    {
                        var mod = Process.GetCurrentProcess().MainModule;
                        using (HashAlgorithm hashAlg = new SHA1Managed())
                        {
                            using (Stream file = new FileStream(mod.FileName, FileMode.Open, FileAccess.Read))
                            {
                                byte[] hash = hashAlg.ComputeHash(file);
                                Logging.WriteDebug("H: {0}", BitConverter.ToString(hash));
                            }
                        }
                        var vInfo = mod.FileVersionInfo;
                        Logging.WriteDebug("V: {0}", vInfo.FileVersion);
                    }
                    catch { }
                }).Start();
        }
        #endregion

        #region Overrides
        static readonly string _name = "ProfessionBuddy";
        public override string Name
        {
            get { return _name; }
        }
        public override string Author { get { return "HighVoltz"; } }

        public override Version Version { get { return new Version(1, _svn.Revision); } }

        public override bool WantButton { get { return true; } }

        public override string ButtonText { get { return Name; } }

        public override void OnButtonPress()
        {
            try
            {
                if (!PluginManager.IsInitialized)
                    MessageBox.Show("You must wait until plugins are initialized");
                else if (IsEnabled)
                {
                    if (!MainForm.IsValid)
                        new MainForm().Show();
                    else
                        MainForm.Instance.Activate();
                }
                else
                    MessageBox.Show("You must enable Professionbuddy before you can use it");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        public bool IsEnabled
        {
            get { return PluginManager.Plugins.Any(p => p.Name == Name && p.Author == Author && p.Enabled); }
        }

        public override void Dispose()
        {
            BotEvents.OnBotChanged -= BotEvents_OnBotChanged;
            IsRunning = false;
            //BotBaseCleanUp(null);
            Lua.Events.DetachEvent("BAG_UPDATE", OnBagUpdate);
            Lua.Events.DetachEvent("SKILL_LINES_CHANGED", OnSkillUpdate);
            Lua.Events.DetachEvent("SPELLS_CHANGED", OnSpellsChanged);
            Lua.Events.DetachEvent("BANKFRAME_OPENED", Util.OnBankFrameOpened);
            Lua.Events.DetachEvent("BANKFRAME_CLOSED", Util.OnBankFrameClosed);
            if (MainForm.IsValid)
                MainForm.Instance.Close();
        }

        public override void Pulse()
        {
            // Due to some non-thread safe HB api I need to make sure the callbacks are executed from main thread
            // Throttling the events so the callback doesn't get called repeatedly in a short time frame.
            if (OnBagUpdateSpamSW.ElapsedMilliseconds >= 1000)
            {
                OnBagUpdateSpamSW.Stop();
                OnBagUpdateSpamSW.Reset();
                OnBagUpdateTimerCB(null);
            }
            if (OnSkillUpdateSpamSW.ElapsedMilliseconds >= 1000)
            {
                OnSkillUpdateSpamSW.Stop();
                OnSkillUpdateSpamSW.Reset();
                OnSkillUpdateTimerCB(null);
            }
            if (OnSpellsChangedSpamSW.ElapsedMilliseconds >= 1000)
            {
                OnSpellsChangedSpamSW.Stop();
                OnSpellsChangedSpamSW.Reset();
                OnSpellsChangedCB(null);
            }
        }

        public override void Initialize()
        {
            Init();
        }
        #endregion

        #region Callbacks

        #region OnBagUpdate
        Stopwatch OnBagUpdateSpamSW = new Stopwatch();
        public void OnBagUpdate(object obj, LuaEventArgs args)
        {
            if (!OnBagUpdateSpamSW.IsRunning)
            {
                Lua.Events.DetachEvent("BAG_UPDATE", OnBagUpdate);
                OnBagUpdateSpamSW.Start();
            }
        }

        void OnBagUpdateTimerCB(Object stateInfo)
        {
            try
            {
                Lua.Events.AttachEvent("BAG_UPDATE", OnBagUpdate);
                foreach (TradeSkill ts in TradeSkillList)
                {
                    ts.PulseBags();
                }
                UpdateMaterials();
                if (MainForm.IsValid)
                {
                    MainForm.Instance.RefreshTradeSkillTabs();
                    MainForm.Instance.RefreshActionTree(typeof(CastSpellAction));
                }
            }
            catch (Exception ex) { Err(ex.ToString()); }
        }
        #endregion

        #region OnSkillUpdate
        Stopwatch OnSkillUpdateSpamSW = new Stopwatch();
        public void OnSkillUpdate(object obj, LuaEventArgs args)
        {
            foreach (object o in args.Args)
                Debug("spell changed {0}", o);
            if (!OnSkillUpdateSpamSW.IsRunning)
            {
                Lua.Events.DetachEvent("SKILL_LINES_CHANGED", OnSkillUpdate);
                OnSkillUpdateSpamSW.Start();
            }
        }

        void OnSkillUpdateTimerCB(Object stateInfo)
        {
            Lua.Events.AttachEvent("SKILL_LINES_CHANGED", OnSkillUpdate);
            try
            {
                UpdateMaterials();
                // check if there was any tradeskills added or removed.
                WoWSkill[] skills = SupportedTradeSkills;
                bool changed = skills.
                    Count(s => TradeSkillList.Count(l => l.SkillLine == (SkillLine)s.Id) == 1) != TradeSkillList.Count ||
                    skills.Length != TradeSkillList.Count;
                if (changed)
                {
                    Debug("A profession was added or removed. Reloading Tradeskills (OnSkillUpdateTimerCB)");
                    LoadTradeSkills();
                    if (MainForm.IsValid)
                        MainForm.Instance.InitTradeSkillTab();
                }
                else
                {
                    Debug("Updated tradeskills from OnSkillUpdateTimerCB");
                    foreach (TradeSkill ts in TradeSkillList)
                    {
                        ts.PulseSkill();
                    }
                    if (MainForm.IsValid)
                    {
                        MainForm.Instance.RefreshTradeSkillTabs();
                        MainForm.Instance.RefreshActionTree(typeof(CastSpellAction));
                    }
                }
            }
            catch (Exception ex) { Err(ex.ToString()); }
        }

        #endregion

        #region OnSpellsChanged
        Stopwatch OnSpellsChangedSpamSW = new Stopwatch();
        public void OnSpellsChanged(object obj, LuaEventArgs args)
        {
            if (!OnSpellsChangedSpamSW.IsRunning)
            {
                Lua.Events.DetachEvent("SPELLS_CHANGED", OnSpellsChanged);
                OnSpellsChangedSpamSW.Start();
            }
        }
        public void OnSpellsChangedCB(Object stateInfo)
        {
            try
            {
                Lua.Events.AttachEvent("SPELLS_CHANGED", OnSpellsChanged);
                Debug("Pulsing Tradeskills from OnSpellsChanged");
                foreach (TradeSkill ts in TradeSkillList)
                {
                    TradeSkillFrame.Instance.UpdateTradeSkill(ts, true);
                }
                if (MainForm.IsValid)
                {
                    MainForm.Instance.InitTradeSkillTab();
                    MainForm.Instance.RefreshActionTree(typeof(CastSpellAction));
                }
            }
            catch (Exception ex)
            { Err(ex.ToString()); }
        }
        #endregion

        #region OnBotChanged

        void BotEvents_OnBotChanged(BotEvents.BotChangedEventArgs args)
        {
            if (TreeRoot.Current.Root is PrioritySelector)
            {
                PrioritySelector botbase = TreeRoot.Current.Root as PrioritySelector;
                BotBaseCleanUp(botbase);
                botbase.InsertChild(0, Root);
            }
        }
        #endregion

        #endregion

        #region Behavior Tree
        PBIdentityComposite root;
        public TreeSharp.Composite Root
        {
            get
            {
                return root ?? (root = new PBIdentityComposite(CurrentProfile.Branch));
            }
            set
            {
                root = (PBIdentityComposite)value;
            }
        }
        PbProfile _currentProfile;
        public PbProfile CurrentProfile
        {
            get
            {
                return _currentProfile ?? (_currentProfile = new PbProfile());
            }
            private set
            {
                _currentProfile = value;
            }
        }
        #endregion

        #region Misc
        // remove any occurance of IdentityComposite in the current BotBase, used on dispose or botbase change
        void BotBaseCleanUp(PrioritySelector bot)
        {
            PrioritySelector botbase = null;
            if (bot != null)
                botbase = bot;
            else if (TreeRoot.Current.Root is PrioritySelector)
                botbase = TreeRoot.Current.Root as PrioritySelector;
            // check if we already injected into the BotBase
            if (botbase != null)
            {
                bool isRunning = botbase.IsRunning;
                if (isRunning)
                    TreeRoot.Stop();
                for (int i = botbase.Children.Count - 1; i >= 0; i--)
                {
                    //if (botbase.Children[i] is IdentityComposite ) // this will not work after a recompile because the types are now in different assemblies
                    if (botbase.Children[i].GetType().Name.Contains("PBIdentityComposite"))
                    {
                        botbase.Children.RemoveAt(i);
                    }
                }
            }
        }

        bool _init = false;
        public void Init()
        {
            try
            {
                if (!_init)
                {

                    Debug("Initializing ...");
                    _pluginPath = Logging.ApplicationPath + @"\Plugins\" + _name;
                    _profilePath = Environment.UserName == "highvoltz" ?
                        @"C:\Users\highvoltz\Desktop\Buddy\Projects\Professionbuddy\Professionbuddy\Profiles" :
                        Path.Combine(_pluginPath, "Profiles");
                    _tempFolder = Path.Combine(_pluginPath, "Temp");

                    if (!Directory.Exists(PluginPath))
                        Directory.CreateDirectory(PluginPath);
                    WipeTempFolder();
                    // force Tripper.Tools.dll to load........
                    new Tripper.Tools.Math.Vector3(0, 0, 0);

                    MySettings = new ProfessionBuddySettings (
                        Path.Combine(Logging.ApplicationPath, string.Format(@"Settings\{0}\{0}[{1}-{2}].xml",
                        Name, Me.Name, Lua.GetReturnVal<string>("return GetRealmName()", 0)))
                    );

                    IsTradeSkillsLoaded = false;
                    IsRunning = MySettings.IsRunning;
                    MaterialList = new Dictionary<uint, int>();
                    TradeSkillList = new List<TradeSkill>();
                    Instance.ProfileSettings = new PbProfileSettings();
                    LoadProtectedItems();
                    LoadTradeSkills();
                    DataStore = new DataStore();
                    DataStore.ImportDataStore();
                    BotEvents.OnBotChanged += BotEvents_OnBotChanged;
                    Lua.Events.AttachEvent("BAG_UPDATE", OnBagUpdate);
                    Lua.Events.AttachEvent("SKILL_LINES_CHANGED", OnSkillUpdate);
                    Lua.Events.AttachEvent("SPELLS_CHANGED", OnSpellsChanged);
                    Lua.Events.AttachEvent("BANKFRAME_OPENED", Util.OnBankFrameOpened);
                    Lua.Events.AttachEvent("BANKFRAME_CLOSED", Util.OnBankFrameClosed);
                    BotEvents_OnBotChanged(null);
                    if (!string.IsNullOrEmpty(MySettings.LastProfile))
                    {
                        if (IsRunning)
                        {
                            PreChangeBot();
                            PreLoadHbProfile();
                        }
                        LoadProfile(MySettings.LastProfile);
                    }
                    else
                        ProfileSettings = new PbProfileSettings();
                    _init = true;
                }
            }
            catch (Exception ex) { Logging.Write(System.Drawing.Color.Red, ex.ToString()); }
        }

        WoWSkill[] SupportedTradeSkills
        {
            get
            {
                IEnumerable<WoWSkill> skillList = from skill in TradeSkillFrame.SupportedSkills
                                                  where skill != SkillLine.Archaeology && Me.GetSkill(skill).MaxValue > 0
                                                  select Me.GetSkill(skill);
                return skillList.ToArray();
            }
        }
        public void LoadTradeSkills()
        {
            try
            {
                lock (TradeSkillList)
                {
                    TradeSkillList.Clear();
                    foreach (WoWSkill skill in SupportedTradeSkills)
                    {
                        Log("Adding TradeSkill {0}", skill.Name);
                        TradeSkill ts = TradeSkillFrame.Instance.GetTradeSkill((SkillLine)skill.Id, true);
                        if (ts != null)
                        {
                            TradeSkillList.Add(ts);
                        }
                        else
                        {
                            IsTradeSkillsLoaded = false;
                            Log("Unable to load tradeskill {0}", (SkillLine)skill.Id);
                            return;
                        }
                    }
                }
                IsTradeSkillsLoaded = true;
                if (OnTradeSkillsLoaded != null)
                {
                    OnTradeSkillsLoaded(this, null);
                }
            }
            catch (Exception ex) { Logging.Write(System.Drawing.Color.Red, ex.ToString()); }
        }

        public void UpdateMaterials()
        {
            try
            {
                lock (MaterialList)
                {
                    MaterialList.Clear();
                    List<CastSpellAction> castSpellList =
                        CastSpellAction.GetCastSpellActionList(CurrentProfile.Branch);
                    if (castSpellList != null)
                    {
                        foreach (CastSpellAction ca in castSpellList)
                        {
                            if (ca.IsRecipe)
                            {
                                foreach (var ingred in ca.Recipe.Ingredients)
                                {
                                    MaterialList[ingred.ID] = MaterialList.ContainsKey(ingred.ID) ?
                                        MaterialList[ingred.ID] + (ca.CalculatedRepeat > 0 ?
                                        (int)ingred.Required * (ca.CalculatedRepeat - ca.Casted) : 0) :

                                        (ca.CalculatedRepeat > 0 ? (int)ingred.Required * (ca.CalculatedRepeat - ca.Casted) : 0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { Err(ex.ToString()); }
        }

        public static bool LoadProfile(string path)
        {
            if (File.Exists(path))
            {
                Log("Loading profile {0}", path);
                PBIdentityComposite idComp = Instance.CurrentProfile.LoadFromFile(path);
                if (idComp != null)
                {
                    if (MainForm.IsValid)
                    {
                        MainForm.Instance.InitActionTree();
                        MainForm.Instance.RefreshTradeSkillTabs();
                    }
                    Instance.MySettings.LastProfile = path;
                    Instance.Root = idComp;
                    Instance.ProfileSettings.Load();
                    Instance.GenorateDynamicCode();
                    Instance.UpdateMaterials();
                }
                else
                    return false;
            }
            else
            {
                Err("Profile: {0} does not exist", path);
                Instance.MySettings.LastProfile = path;
                return false;
            }
            if (MainForm.IsValid)
                MainForm.Instance.UpdateControls();
            Instance.MySettings.Save();
            return true;
        }

        public static void PreLoadHbProfile()
        {
            if (!string.IsNullOrEmpty(Instance.CurrentProfile.ProfilePath) && Instance.CurrentProfile.Branch != null)
            {
                Dictionary<string, Uri> dict = new Dictionary<string, Uri>();
                PbProfile.GetHbprofiles(Instance.CurrentProfile.ProfilePath, Instance.CurrentProfile.Branch, dict);
                if (dict.Count > 0)
                {
                    foreach (var kv in dict)
                    {
                        if (!string.IsNullOrEmpty(kv.Key))
                        {
                            ProfileManager.LoadNew(kv.Key);
                            return;
                        }
                    }
                }
            }
            if (ProfileManager.CurrentProfile == null)
                ProfileManager.LoadEmpty();
        }

        public static void PreChangeBot()
        {
            List<ChangeBotAction> cbaList = GetListOfActionsByType<ChangeBotAction>(Instance.CurrentProfile.Branch, null);
            if (cbaList.Count > 0 && !BotManager.Current.Name.Contains((cbaList[0].BotName)))
            {
                Professionbuddy.Debug("Changing to Bot {0}", cbaList[0].BotName);
                cbaList[0].ChangeBot();
            }

        }

        static internal List<T> GetListOfActionsByType<T>(Composite comp, List<T> list) where T : Composite
        {
            if (list == null)
                list = new List<T>();
            if (comp.GetType() == typeof(T))
            {
                list.Add((T)comp);
            }
            if (comp is GroupComposite)
            {
                foreach (Composite c in ((GroupComposite)comp).Children)
                {
                    GetListOfActionsByType<T>(c, list);
                }
            }
            return list;
        }

        void LoadProtectedItems()
        {
            List<uint> tempList = null;
            string path = Path.Combine(PluginPath, "Protected Items.xml");
            if (File.Exists(path))
            {
                XElement xml = XElement.Load(path);
                tempList = xml.Elements("Item").Select(x => x.Attribute("Entry").Value.ToUint()).Distinct().ToList();
            }
            ProtectedItems = tempList != null ? tempList : new List<uint>();
        }
        #endregion

        #region Utilies
        public static void Err(string format, params object[] args)
        {
            Logging.Write(System.Drawing.Color.Red, "Err: " + format, args);
        }

        public static void Log(string format, params object[] args)
        {
            Logging.Write(System.Drawing.Color.SaddleBrown, string.Format("PB {0}:", Instance.Version) + format, args);
        }

        public static void Debug(string format, params object[] args)
        {
            Logging.WriteDebug(System.Drawing.Color.SaddleBrown, string.Format("PB {0}:", Instance.Version) + format, args);
        }

        #endregion

    }
}

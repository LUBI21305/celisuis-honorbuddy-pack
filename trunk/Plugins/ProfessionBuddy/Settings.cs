using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Styx.WoWInternals;
using System.Xml;
using System.Runtime.Serialization;
using Styx.Helpers;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz
{
    public class ProfessionBuddySettings : Styx.Helpers.Settings
    {
        public static ProfessionBuddySettings Instance{get;private set;}
        public ProfessionBuddySettings(string settingsPath)
            : base(settingsPath) {
                Instance = this;
            Load();
        }
        [Setting, DefaultValue("")]
        public string LastProfile { get; set; }

        [Setting, DefaultValue(false)]
        public bool IsRunning { get; set; }

        [Setting, DefaultValue(null)]
        public string DataStoreTable { get; set; }
        
        [Setting, DefaultValue("")]
        public string WowVersion { get; set; }

        [Setting, DefaultValue(0u)]
        public uint TradeskillFrameOffset { get; set; }       
    }

    public class PbProfileSettings
    {
        public Dictionary<string, object> Settings { get; private set; }
        public Dictionary<string, string> Summaries { get; private set; }

        public PbProfileSettings() {
            Settings = new Dictionary<string, object>();
            Summaries = new Dictionary<string, string>();
        }
        public object this[string name] {
            get {
                return Settings.ContainsKey(name) ? Settings[name] : null;
            }
            set {
                Settings[name] = value;
                if (Professionbuddy.Instance.CurrentProfile != null)
                    Save();
            }
        }
        string ProfileName {
            get {
                return Professionbuddy.Instance.CurrentProfile != null ?
                    Path.GetFileNameWithoutExtension(Professionbuddy.Instance.CurrentProfile.XmlPath) : "";
            }
        }
        string SettingsPath {
            get {
                return Path.Combine(Logging.ApplicationPath,
                    string.Format("Settings\\ProfessionBuddy\\{0}[{1}-{2}].xml", ProfileName,
                    ObjectManager.Me.Name, Lua.GetReturnVal<string>("return GetRealmName()", 0)));
            }
        }
        public void Save() {
            if (Professionbuddy.Instance.CurrentProfile != null)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(SettingsPath, settings))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>));
                    serializer.WriteObject(writer, Settings);
                }
            }
        }
        public void Load() {

            if (Professionbuddy.Instance.CurrentProfile != null)
            {
                if (File.Exists(SettingsPath))
                {
                    using (XmlReader reader = XmlReader.Create(SettingsPath))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>));
                        var temp = (Dictionary<string, object>)serializer.ReadObject(reader);
                        if (temp != null)
                        {
                            Settings = new Dictionary<string, object>();
                            Summaries = new Dictionary<string, string>();
                            foreach (var kv in temp)
                            {
                                Settings[kv.Key] = kv.Value;
                            }
                        }
                    }
                }
                LoadDefaultValues();
            }
        }

        public void LoadDefaultValues() {
            List<Composites.Settings> settingsList = new List<Composites.Settings>();
            GetDefaultSettings(Professionbuddy.Instance.CurrentProfile.Branch, settingsList);
            foreach (var setting in settingsList)
            {
                if (!Settings.ContainsKey(setting.SettingName))
                {
                    object o = GetValue(setting.Type, setting.DefaultValue);
                    Settings[setting.SettingName] = o;
                    Summaries[setting.SettingName] = setting.Summary;
                }
            }
        }

        object GetValue(TypeCode code, string value) {
            try
            {
                switch (code)
                {
                    case TypeCode.Boolean:
                        return bool.Parse(value);
                    case TypeCode.Byte:
                        return byte.Parse(value);
                    case TypeCode.Char:
                        return char.Parse(value);
                    case TypeCode.DateTime:
                        return DateTime.Parse(value);
                    case TypeCode.Decimal:
                        return decimal.Parse(value);
                    case TypeCode.Double:
                        return double.Parse(value);
                    case TypeCode.Int16:
                        return short.Parse(value);
                    case TypeCode.Int32:
                        return int.Parse(value);
                    case TypeCode.Int64:
                        return long.Parse(value);
                    case TypeCode.SByte:
                        return sbyte.Parse(value);
                    case TypeCode.Single:
                        return float.Parse(value);
                    case TypeCode.String:
                        return value;
                    case TypeCode.UInt16:
                        return ushort.Parse(value);
                    case TypeCode.UInt32:
                        return uint.Parse(value);
                    case TypeCode.UInt64:
                        return ulong.Parse(value);
                    default:
                        return new object();
                }
            }
            catch (Exception ex) { Logging.WriteException(ex);return null; }
        }

        public void GetDefaultSettings(Composite comp, List<Composites.Settings> list) {
            if (comp is Composites.Settings)
                list.Add(comp as Composites.Settings);
            if (comp is GroupComposite)
            {
                foreach (var child in ((GroupComposite)comp).Children)
                    GetDefaultSettings(child, list);
            }
        }
    }
}

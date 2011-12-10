using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Xml;
using Styx;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Helpers;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz.Composites
{
    public class Settings : PBAction
    {
        public string DefaultValue
        {
            get { return (string)Properties["DefaultValue"].Value; }
            set { Properties["DefaultValue"].Value = value; }
        }
        
        public TypeCode Type
        {
            get { return (TypeCode)Properties["Type"].Value; }
            set { Properties["Type"].Value = value; }
        }
        public string SettingName
        {
            get { return (string)Properties["Name"].Value; }
            set { Properties["Name"].Value = value; }
        }
        public string Summary
        {
            get { return (string)Properties["Summary"].Value; }
            set { Properties["Summary"].Value = value; }
        }
        public Settings()
        {
            Properties["DefaultValue"] = new MetaProp("DefaultValue", typeof(string), new DisplayNameAttribute("Default Value"));
            Properties["Type"] = new MetaProp("Type", typeof(TypeCode));
            Properties["Name"] = new MetaProp("Name", typeof(string));
            Properties["Summary"] = new MetaProp("Summary", typeof(string));
            DefaultValue = "true";
            Type = TypeCode.Boolean;
            SettingName = "SettingName";
            Summary = "This is a summary of what this setting does";
        }

        public override object Clone()
        {
            return new Settings() { DefaultValue = this.DefaultValue,
                SettingName = this.SettingName,Type = this.Type,Summary = this.Summary};
        }
        public override string Help
        {
            get
            {
                return "This action adds a user customizable setting to Professionbuddy profiles";
            }
        }
        public override string Name
        {
            get
            {
                return "Settings";
            }
        }
        public override string Title
        {
            get
            {
                return string.Format("{0}: {1} {2}={3}",Name,Type,SettingName,DefaultValue);
            }
        }
        public override bool IsDone
        {
            get
            {
                return true;
            }
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            Type = (TypeCode)Enum.Parse(typeof(TypeCode), reader["Type"]);
            DefaultValue = reader["DefaultValue"];
            SettingName = reader["Name"];
            Summary = reader["Summary"];
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Type", Type.ToString());
            writer.WriteAttributeString("DefaultValue", DefaultValue);
            writer.WriteAttributeString("Name", SettingName);
            writer.WriteAttributeString("Summary", Summary);
        }
        #endregion
    }
}

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
    public class Comment : PBAction
    {
        public string Text
        {
            get { return (string)Properties["Text"].Value; }
            set { Properties["Text"].Value = value; }
        }
        public Comment()
        {
            Properties["Text"] = new MetaProp("Text", typeof(string), new DisplayNameAttribute("Comment"));
        }
        public Comment(string comment) : this()
        {
            Text = comment;
        }
        public override System.Drawing.Color Color { get { return System.Drawing.Color.DarkGreen; } }
        public override object Clone()
        {
            return new Comment() { Text = this.Text };
        }
        public override string Help
        {
            get
            {
                return "This does nothing but add a comment";
            }
        }
        public override string Name
        {
            get
            {
                return "Comment";
            }
        }
        public override string Title
        {
            get
            { 
               return string.Format("// {0}", Text);
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
            Text = reader["Text"];
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Text", Text);
        }
        #endregion
    }
}

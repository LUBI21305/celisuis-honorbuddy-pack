using System;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = TreeSharp.Action;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text.RegularExpressions;
using Styx;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic.Pathing;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz.Composites
{
    public interface IPBComposite : ICloneable, IXmlSerializable
    {
        PropertyBag Properties { get; }
        string Name { get; }
        string Title { get; }
        System.Drawing.Color Color { get; }
        string Help { get; }
        void Reset();
        bool IsDone { get; }
    }

    #region PBAction
    public abstract class PBAction : Action, IPBComposite, IXmlSerializable
    {
        protected Professionbuddy Pb;
        protected LocalPlayer me = ObjectManager.Me;
        protected PBAction()
        {
            HasRunOnce = false;
            Pb = Professionbuddy.Instance;
            Properties = new PropertyBag();
        }
        virtual public string Help { get { return ""; } }
        virtual public string Name { get { return "PBAction"; } }
        virtual public string Title { get { return string.Format("({0})", Name); } }
        virtual public System.Drawing.Color Color { get { return System.Drawing.Color.Black; } }
        protected PropertyGrid propertyGrid { get { return MainForm.IsValid ? MainForm.Instance.ActionGrid : null; } }
        protected void RefreshPropertyGrid()
        {
            if (propertyGrid != null)
            {
                propertyGrid.Refresh();
            }
        }
        public virtual bool IsDone { get; protected set; }
        protected bool HasRunOnce { get; set; }
        /// <summary>
        /// If overriding this method call base method or set HasRunOnce to false.
        /// </summary>
        protected virtual void RunOnce()
        {
            HasRunOnce = true;
        }
        public virtual PropertyBag Properties { get; protected set; }

        public virtual object Clone()
        {
            return this;
        }
        public virtual void Reset()
        {
            IsDone = false;
            HasRunOnce = false;
        }
        #region XmlSerializer
        virtual public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
        }
        virtual public void WriteXml(XmlWriter writer)
        {
        }
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        #endregion

    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TreeSharp;
using System.Xml;
using Styx.Helpers;

namespace HighVoltz.Composites
{
    class SubRoutine : GroupComposite, IPBComposite, IXmlSerializable
    {
        virtual public string SubRoutineName
        {
            get { return (string)Properties["SubRoutineName"].Value; }
            set { Properties["SubRoutineName"].Value = value; }
        }
        public SubRoutine()
            : base()
        {
            Properties = new PropertyBag();
            Properties["SubRoutineName"] = new MetaProp("SubRoutineName", typeof(string));
            SubRoutineName = "";
        }

        virtual public System.Drawing.Color Color { get { return System.Drawing.Color.Blue; } }
        virtual public string Name { get { return "SubRoutine"; } }
        virtual public string Title { get { return string.Format("Sub {0}", SubRoutineName); } }

        virtual public PropertyBag Properties { get; private set; }

        // credits to Apoc http://code.google.com/p/treesharp/source/browse/trunk/TreeSharp/PrioritySelector.cs
        protected override IEnumerable<RunStatus> Execute(object context)
        {
            //lock (Locker)
            //{
            if (context == null || !(context is string) || (string)context != SubRoutineName)
            {
                yield return RunStatus.Failure;
                yield break;
            }
            foreach (Composite node in Children)
            {
                node.Start(context);
                // Keep stepping through the enumeration while it's returing RunStatus.Running
                // or until CanRun() returns false if IgnoreCanRun is false..
                while (node.Tick(context) == RunStatus.Running)
                {
                    Selection = node;
                    yield return RunStatus.Running;
                }

                Selection = null;
                node.Stop(context);
                if (node.LastStatus == RunStatus.Success)
                {
                    yield return RunStatus.Success;
                    yield break;
                }
            }
            yield return RunStatus.Failure;
            yield break;
            //}
        }

        virtual public void Reset()
        {
            recursiveReset(this);
        }
        void recursiveReset(GroupComposite gc)
        {
            foreach (IPBComposite comp in gc.Children)
            {
                comp.Reset();
                if (comp is GroupComposite)
                    recursiveReset(comp as GroupComposite);
            }
        }
        public bool IsDone
        {
            get
            {
                return (Children.Count(c => ((IPBComposite)c).IsDone) == Children.Count);
            }
        }

        public virtual object Clone()
        {
            SubRoutine pd = new SubRoutine()
            {
                SubRoutineName = this.SubRoutineName,
            };
            return pd;
        }

        virtual public string Help { get { return "SubRoutine can contain multiple actions which which you can execute using the 'Call SubRoutine' action"; } }

        #region XmlSerializer
        virtual public void ReadXml(XmlReader reader)
        {
            SubRoutineName = reader["SubRoutineName"];
            reader.ReadStartElement();

            while (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Comment)
            {
                if (reader.NodeType == XmlNodeType.Comment)
                {
                    AddChild(new Comment(reader.Value));
                    reader.Skip();
                }
                else
                {
                    Type type = Type.GetType("HighVoltz.Composites." + reader.Name);
                    if (type != null)
                    {
                        IPBComposite comp = (IPBComposite)Activator.CreateInstance(type);
                        if (comp != null)
                        {
                            comp.ReadXml(reader);
                            AddChild((Composite)comp);
                        }
                    }
                    else
                    {
                        Logging.Write(System.Drawing.Color.Red, "Failed to load type {0}", type.Name);
                    }
                }
            }
            if (reader.NodeType == XmlNodeType.EndElement)
                reader.ReadEndElement();
        }

        virtual public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("SubRoutineName", SubRoutineName);

            foreach (IPBComposite comp in Children)
            {
                if (comp is Comment)
                {
                    writer.WriteComment(((Comment)comp).Text);
                }
                else
                {
                    writer.WriteStartElement(comp.GetType().Name);
                    ((IXmlSerializable)comp).WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
        }
        virtual public System.Xml.Schema.XmlSchema GetSchema() { return null; }
        #endregion
    }
}

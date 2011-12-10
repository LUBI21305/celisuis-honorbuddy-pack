using System;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Styx.Helpers;
using TreeSharp;
using System.Diagnostics;
using PrioritySelector = TreeSharp.PrioritySelector;
using System.Windows.Forms;
using System.Collections.Generic;


namespace HighVoltz.Composites
{

    public class If : GroupComposite, ICSharpCode, IPBComposite, ICloneable
    {
        #region Properties
        virtual public PropertyBag Properties { get; private set; }
        protected PropertyGrid propertyGrid { get { return MainForm.IsValid ? MainForm.Instance.ActionGrid : null; } }
        protected void RefreshPropertyGrid()
        {
            if (propertyGrid != null)
            {
                propertyGrid.Refresh();
            }
        }
        #endregion
        protected readonly static object _lockObject = new object();
        virtual public CanRunDecoratorDelegate CanRunDelegate { get; set; }
        virtual public string Condition
        {
            get { return (string)Properties["Condition"].Value; }
            set { Properties["Condition"].Value = value; }
        }

        virtual public bool IgnoreCanRun
        {
            get { return (bool)Properties["IgnoreCanRun"].Value; }
            set { Properties["IgnoreCanRun"].Value = value; }
        }

        public If()
            : base()
        {
            Properties = new PropertyBag();
            Properties["IgnoreCanRun"] = new MetaProp("IgnoreCanRun", typeof(bool), new DisplayNameAttribute("Ignore Condition until done"));
            Properties["Condition"] = new MetaProp("Condition", typeof(string));
            Properties["CompileError"] = new MetaProp("CompileError", typeof(string), new ReadOnlyAttribute(true));

            this.CanRunDelegate = c => false;
            Condition = "";
            CompileError = "";
            Properties["CompileError"].Show = false;

            Properties["Condition"].PropertyChanged += new EventHandler(Condition_PropertyChanged);
            Properties["CompileError"].PropertyChanged += new EventHandler(CompileError_PropertyChanged);
            IgnoreCanRun = true;
        }

        void Condition_PropertyChanged(object sender, EventArgs e)
        {
            Professionbuddy.Instance.CodeWasModified = true;
        }

        string lastError = "";
        void CompileError_PropertyChanged(object sender, EventArgs e)
        {
            if (CompileError != "" || (CompileError == "" && lastError != ""))
                MainForm.Instance.RefreshActionTree(this);
            if (CompileError != "")
                Properties["CompileError"].Show = true;
            else
                Properties["CompileError"].Show = false;
            RefreshPropertyGrid();
            lastError = CompileError;
        }

        protected virtual bool CanRun(object context)
        {
            try
            {
                return CanRunDelegate(context);
            }
            catch (Exception ex)
            {
                Professionbuddy.Err("If Condition: {0} ,Err:{1}", Condition, ex);
                return false;
            }
        }

        protected override IEnumerable<RunStatus> Execute(object context)
        {
            // genorates some exeption.... besides I'm only accessing this from one thread
            //lock (Locker)
            //{
            if (IsDone)
            {
                yield return RunStatus.Failure;
                yield break;
            }
            foreach (Composite node in Children)
            {
                // Keep stepping through the enumeration while it's returing RunStatus.Running
                // or until CanRun() returns false if IgnoreCanRun is false..
                node.Start(context);
                while ((IgnoreCanRun || (CanRun(null) && !IgnoreCanRun)) && node != null &&
                    node.Tick(context) == RunStatus.Running)
                {
                    Selection = node;
                    yield return RunStatus.Running;
                }

                Selection = null;
                //node.Stop(context);
                if (node.LastStatus == RunStatus.Success)
                {
                    yield return RunStatus.Success;
                    //yield break; don't break iteration.. While Condition return sucess if at end of loop
                }
            }
            yield return RunStatus.Failure;
            yield break;
            //}
        }

        //private IEnumerator<RunStatus> _current;
        //public override RunStatus Tick(object context)
        //{
        //    //lock (Locker)
        //    //{
        //        if (LastStatus.HasValue && LastStatus != RunStatus.Running)
        //        {
        //            return LastStatus.Value;
        //        }
        //        if (_current == null)
        //        {
        //            LastStatus = null;
        //            _current = Execute(context).GetEnumerator();
        //        }
        //        if (_current.MoveNext())
        //        {
        //            LastStatus = _current.Current;
        //        }
        //        //else
        //        //{
        //        //    throw new ApplicationException("Nothing to run? Somethings gone terribly, terribly wrong!");
        //        //}

        //        //if (LastStatus != RunStatus.Running)
        //        //{
        //        //    Stop(context);
        //        //}
        //        return LastStatus ?? RunStatus.Failure ;
        //     //}
        //}

        public virtual Delegate CompiledMethod
        {
            get { return CanRunDelegate; }
            set { CanRunDelegate = (CanRunDecoratorDelegate)value; }
        }

        public virtual string Code
        {
            get { return Condition; }
            set { Condition = value; }
        }

        public int CodeLineNumber { get; set; }

        public string CompileError
        {
            get { return (string)Properties["CompileError"].Value; }
            set { Properties["CompileError"].Value = value; }
        }

        public CsharpCodeType CodeType { get { return CsharpCodeType.BoolExpression; } }

        virtual public void Reset()
        {
            recursiveReset(this);
        }
        void recursiveReset(If gc)
        {
            foreach (IPBComposite comp in gc.Children)
            {
                comp.Reset();
                if (comp is If)
                    recursiveReset(comp as If);
            }
        }
        virtual public bool IsDone
        {
            get
            {
                return Children.Count(c => ((IPBComposite)c).IsDone) == Children.Count || !CanRun(null);
            }
        }

        virtual public System.Drawing.Color Color
        {
            get { return string.IsNullOrEmpty(CompileError) ? System.Drawing.Color.Blue : System.Drawing.Color.Red; }
        }

        virtual public string Name { get { return "If Condition"; } }
        virtual public string Title
        {
            get
            {
                return string.Format("If {0}",
                    string.IsNullOrEmpty(Condition) ? "Condition" : "(" + Condition + ")");
            }
        }


        public virtual object Clone()
        {
            If pd = new If()
            {
                CanRunDelegate = this.CanRunDelegate,
                Condition = this.Condition,
                IgnoreCanRun = this.IgnoreCanRun
            };
            return pd;
        }

        virtual public string Help { get { return "'If Condition' will execute the actions it contains if the specified condition is true. 'Ignore Condition until done' basically will ignore the Condition if any of the actions it contains is running.If you need to repeat a set of actions then use 'While Condition' or nest this within a 'While Condition'"; } }

        #region XmlSerializer
        virtual public void ReadXml(XmlReader reader)
        {
            Condition = reader["Condition"];
            bool boolVal;
            bool.TryParse(reader["IgnoreCanRun"], out boolVal);
            IgnoreCanRun = boolVal;
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
            writer.WriteAttributeString("Condition", Condition);
            writer.WriteAttributeString("IgnoreCanRun", IgnoreCanRun.ToString());
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

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        #endregion
    }



    //public class If : CsharpCondition
    //{
    //    virtual public bool IgnoreCanRun
    //    {
    //        get { return (bool)Properties["IgnoreCanRun"].Value; }
    //        set { Properties["IgnoreCanRun"].Value = value; }
    //    }
    //    public If()
    //        : base(CsharpCodeType.BoolExpression)
    //    {
    //        Properties["IgnoreCanRun"] = new MetaProp("IgnoreCanRun", typeof(bool), new DisplayNameAttribute("Ignore Condition until done"));
    //        IgnoreCanRun = true;
    //    }


    //    override public string Name { get { return "If Condition"; } }
    //    override public string Title
    //    {
    //        get
    //        {
    //            return string.Format("If {0}",
    //                string.IsNullOrEmpty(Condition) ? "Condition" : "(" + Condition + ")");
    //        }
    //    }


    //    public override RunStatus Tick(object context)
    //    {
    //        if ((LastStatus == RunStatus.Running && IgnoreCanRun) || CanRun(null))
    //        {
    //            if (!DecoratedChild.IsRunning)
    //                DecoratedChild.Start(null);
    //            LastStatus = DecoratedChild.Tick(null);
    //        }
    //        else
    //        {
    //            LastStatus = RunStatus.Failure;
    //        }
    //        return (RunStatus)LastStatus;
    //    }

    //    public override object Clone()
    //    {
    //        If pd = new If()
    //        {
    //            CanRunDelegate = this.CanRunDelegate,
    //            Condition = this.Condition,
    //            IgnoreCanRun = this.IgnoreCanRun
    //        };
    //        return pd;
    //    }

    //    override public string Help { get { return "'If Condition' will execute the actions it contains if the specified condition is true. 'Ignore Condition until done' basically will ignore the Condition if any of the actions it contains is running.If you need to repeat a set of actions then use 'While Condition' or nest this within a 'While Condition'"; } }

    //    #region XmlSerializer
    //    override public void ReadXml(XmlReader reader)
    //    {
    //        PrioritySelector ps = (PrioritySelector)DecoratedChild;
    //        Condition = reader["Condition"];
    //        bool boolVal;
    //        bool.TryParse(reader["IgnoreCanRun"], out boolVal);
    //        IgnoreCanRun = boolVal;
    //        reader.MoveToAttribute("ChildrenCount");
    //        int count = reader.ReadContentAsInt();
    //        reader.ReadStartElement();
    //        if (count > 0)
    //        {
    //            for (int i = 0; i < count; i++)
    //            {
    //                Type type = Type.GetType("HighVoltz.Composites." + reader.Name);
    //                if (type != null)
    //                {
    //                    IPBComposite comp = (IPBComposite)Activator.CreateInstance(type);
    //                    if (comp != null)
    //                    {
    //                        comp.ReadXml(reader);
    //                        ps.AddChild((Composite)comp);
    //                    }
    //                }
    //                else
    //                {
    //                    Logging.Write(System.Drawing.Color.Red, "Failed to load type {0}", type.Name);
    //                }
    //            }
    //            if (reader.NodeType == XmlNodeType.EndElement)
    //                reader.ReadEndElement();
    //        }
    //    }

    //    override public void WriteXml(XmlWriter writer)
    //    {
    //        PrioritySelector ps = (PrioritySelector)DecoratedChild;
    //        writer.WriteAttributeString("Condition", Condition);
    //        writer.WriteAttributeString("IgnoreCanRun", IgnoreCanRun.ToString());
    //        writer.WriteStartAttribute("ChildrenCount");
    //        writer.WriteValue(ps.Children.Count);
    //        writer.WriteEndAttribute();
    //        foreach (IPBComposite comp in ps.Children)
    //        {
    //            writer.WriteStartElement(comp.GetType().Name);
    //            ((IXmlSerializable)comp).WriteXml(writer);
    //            writer.WriteEndElement();
    //        }
    //    }

    //    #endregion
    //}
}

using System;
using System.Xml;
using Styx.Helpers;
using TreeSharp;


namespace HighVoltz.Composites
{
    #region CustomAction
    class CustomAction : CsharpAction
    {
        public System.Action<object> Action { get; set; }
        override public string Code
        {
            get { return (string)Properties["Code"].Value; }
            set { Properties["Code"].Value = value; }
        }

        public CustomAction():base(CsharpCodeType.Statements)
        {
            this.Action = c => { ;};
            Properties["Code"] = new MetaProp("Code", typeof(string));
            Code = "";
            Properties["Code"].PropertyChanged += new EventHandler(CustomAction_PropertyChanged);
        }

        void CustomAction_PropertyChanged(object sender, EventArgs e)
        {
            Pb.CodeWasModified = true;
        }

        protected override RunStatus Run(object context)
        {
            try
            {
                if (!IsDone)
                {
                    try
                    {
                        Action(this);
                    }
                    catch (Exception ex)
                    {
                        Professionbuddy.Err("Custom:({0})\n{1}", Code, ex);
                    }
                    IsDone = true;
                }
                return RunStatus.Failure;
            }
            catch (Exception ex) { Logging.Write(System.Drawing.Color.Red, "There was an exception while executing a CustomAction\n{0}", ex); }
            return RunStatus.Failure;
        }
        public override string Name { get { return "Custom Action"; } }
        public override string Title
        {
            get
            {
                return string.Format("{0}",
                    string.IsNullOrEmpty(Code) ? "(" + Name + ")" : "(" + Code + ")");
            }
        }
        public override string Help
        {
            get
            {
                return "This action will execute a valid C# code once before moving to next action.";
            }
        }
        public override object Clone()
        {
            return new CustomAction() { Code = this.Code };
        }

        public override Delegate CompiledMethod
        {
            get
            {
                return Action;
            }
            set
            {
                Action = (System.Action<object>)value;
            }
        }

        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            Code = reader["Code"];
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Code", Code.ToString());
        }
        #endregion
    }
    #endregion
}

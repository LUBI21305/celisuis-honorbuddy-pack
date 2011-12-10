using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HighVoltz.Composites
{
    //this is a PBAction derived abstract class that adds functionallity for dynamically compiled Csharp expression/statement

    public abstract class CsharpAction : PBAction, ICSharpCode
    {
        public CsharpAction()
        {
            CodeType = CsharpCodeType.Statements;
            Properties["CompileError"] = new MetaProp("CompileError", typeof(string), new ReadOnlyAttribute(true));
            
            CompileError = "";

            Properties["CompileError"].Show = false;
            Properties["CompileError"].PropertyChanged += new EventHandler(CompileError_PropertyChanged);
        }
        public CsharpAction(CsharpCodeType t):this()
        {
            CodeType = t;
        }

        string lastError = "";
        void CompileError_PropertyChanged(object sender, EventArgs e)
        {
            if (CompileError != "" || (CompileError == "" && lastError != ""))
            {
                MainForm.Instance.RefreshActionTree(this);
                Properties["CompileError"].Show = true;
            }
            else
                Properties["CompileError"].Show = false;
            RefreshPropertyGrid();
            lastError = CompileError;
        }

        public int CodeLineNumber { get; set; }

        public string CompileError
        {
            get { return (string)Properties["CompileError"].Value; }
            set { Properties["CompileError"].Value = value; }
        }

        public CsharpCodeType CodeType { get; private set; }

        virtual public string Code { get; set; }
        public override System.Drawing.Color Color
        {
            get { return string.IsNullOrEmpty(CompileError) ? base.Color : System.Drawing.Color.Red; }
        }

        virtual public Delegate CompiledMethod { get; set; }

    }
}

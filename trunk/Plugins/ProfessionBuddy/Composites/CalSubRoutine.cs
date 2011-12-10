using System;
using System.Xml;
using Styx.Helpers;
using TreeSharp;


namespace HighVoltz.Composites
{
    #region CallSubRoutine
    class CallSubRoutine : PBAction
    {
        SubRoutine _sub;
        public string SubRoutineName {
            get { return (string)Properties["SubRoutineName"].Value; }
            set { Properties["SubRoutineName"].Value = value; }
        }

        public CallSubRoutine() {
            Properties["SubRoutineName"] = new MetaProp("SubRoutineName", typeof(string));
            SubRoutineName = "";
        }

        bool ranonce = false;
        protected override RunStatus Run(object context) {
            if (!IsDone)
            {
                if (_sub == null)
                {
                    if (!GetSubRoutine())
                    {
                        Professionbuddy.Err("Unable to find Subroutine with name: {0}.", SubRoutineName);
                        IsDone = true;
                    }
                }
                if (!ranonce)
                {
                    // make sure all actions within the subroutine are reset before we start.
                    _sub.Reset();
                    ranonce = true;
                }
                if (_sub != null)
                {
                    if (!_sub.IsRunning)
                        _sub.Start(SubRoutineName);
                    try
                    {
                        _sub.Tick(SubRoutineName);
                    }
                    catch { IsDone = true; _sub.Reset(); return RunStatus.Failure; }
                    IsDone = _sub.IsDone;
                    // we need to reset so calls to the sub from other places can
                    if (!IsDone)
                        return RunStatus.Running;
                }
            }
            return RunStatus.Failure;
        }

        public override void Reset() {
            base.Reset();
            ranonce = false;
        }
        bool GetSubRoutine() {
            _sub = FindSubRoutineByName(SubRoutineName, Pb.CurrentProfile.Branch);
            return _sub != null;
        }

        SubRoutine FindSubRoutineByName(string subName, Composite comp) {

            if (comp is SubRoutine && ((SubRoutine)comp).SubRoutineName == subName)
                return (SubRoutine)comp;
            if (comp is GroupComposite)
            {
                foreach (var c in ((GroupComposite)comp).Children)
                {
                    SubRoutine temp = FindSubRoutineByName(subName, c);
                    if (temp != null)
                        return temp;
                }
            }
            return null;
        }

        public override string Name { get { return "Call SubRoutine"; } }
        public override string Title { get { return string.Format("{0}: {1}()", Name, SubRoutineName); } }
        public override string Help {
            get {
                return "This action will execute a SubRoutine";
            }
        }
        public override object Clone() {
            return new CallSubRoutine() { SubRoutineName = this.SubRoutineName };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader) {
            SubRoutineName = reader["SubRoutineName"];
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("SubRoutineName", SubRoutineName.ToString());
        }
        #endregion
    }
    #endregion
}

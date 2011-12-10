using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Globalization;
using Styx;
using Styx.Logic;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Styx.Logic.Profiles;
using Styx.Logic.BehaviorTree;
using System.Reflection;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz.Composites
{
    #region FlyToAction
    public class FlyToAction : PBAction
    {
        public bool Dismount {
            get { return (bool)Properties["Dismount"].Value; }
            set { Properties["Dismount"].Value = value; }
        }
        WoWPoint loc;
        public string Location {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }
        public FlyToAction() {
            Properties["Dismount"] = new MetaProp("Dismount", typeof(bool), new DisplayNameAttribute("Dismount on Arrival"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));

            Location = loc.ToInvariantString();
            Dismount = true;

            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
        }

        void LocationChanged(object sender, EventArgs e) {
            MetaProp mp = (MetaProp)sender;
            loc = Util.StringToWoWPoint((string)((MetaProp)sender).Value);
            Properties["Location"].PropertyChanged -= new EventHandler(LocationChanged);
            Properties["Location"].Value = string.Format(CultureInfo.InvariantCulture, "<{0}, {1}, {2}>", loc.X, loc.Y, loc.Z);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            RefreshPropertyGrid();
        }

        protected override RunStatus Run(object context) {
            if (!IsDone)
            {
                if (ObjectManager.Me.Location.Distance(loc) > 6)
                {
                    Flightor.MoveTo(loc);
                    TreeRoot.StatusText = string.Format("Flying to location {0}", loc);
                }
                else
                {
                    if (Dismount)
                        Mount.Dismount("Dismounting flying mount");
                        //Lua.DoString("Dismount() CancelShapeshiftForm()");
                    IsDone = true;
                    TreeRoot.StatusText = string.Format("Arrived at location {0}", loc);
                }
                return RunStatus.Running;
            }
            return RunStatus.Failure;
        }

        public override string Name { get { return "Fly To"; } }
        public override string Title {
            get {
                return string.Format("{0}: {1} ", Name, Location);
            }
        }
        public override string Help {
            get {
                return "This action relies on Flightor, the 3d navigator used by Gatherbuddy2. This action will fly your character to the selected location and dismount on arrival if Dismount is set to true.Be sure to make the target location outdoors and not underneath any obsticles.";
            }
        }
        public override object Clone() {
            return new FlyToAction() { Location = this.Location, Dismount = this.Dismount };
        }

        public override void Reset() {
            base.Reset();
        }

        #region XmlSerializer
        public override void ReadXml(XmlReader reader) {
            bool dismount;
            bool.TryParse(reader["Dismount"], out dismount);
            Dismount = dismount;
            float x, y, z;
            x = reader["X"].ToSingle();
            y = reader["Y"].ToSingle();
            z = reader["Z"].ToSingle();
            loc = new WoWPoint(x, y, z);
            Location = loc.ToInvariantString();
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Dismount", Dismount.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
        }
        #endregion
    }
    #endregion
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Styx;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz.Composites
{
    #region MoveToAction
    public class MoveToAction : PBAction
    {
        public enum MoveToType
        {
            Location,
            NearestMailbox,
            NearestVendor,
            NearestFlight,
            NearestAH,
            NearestRepair,
            NearestReagentVendor,
            NearestBanker,
            NearestGB,
            NpcByID
        }
        public enum NavigationType { Navigator, ClickToMove };

        WoWPoint loc;
        public string Location
        {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }

        public MoveToType MoveType
        {
            get { return (MoveToType)Properties["MoveType"].Value; }
            set { Properties["MoveType"].Value = (MoveToType)value; }
        }
        public NavigationType Pathing
        {
            get { return (NavigationType)Properties["Pathing"].Value; }
            set { Properties["Pathing"].Value = (NavigationType)value; }
        }
        public uint Entry
        {
            get { return (uint)Properties["Entry"].Value; }
            set { Properties["Entry"].Value = (uint)value; }
        }
        private WoWPoint locationDb = WoWPoint.Zero;

        public MoveToAction()
        {
            Properties["Entry"] = new MetaProp("Entry", typeof(uint), new EditorAttribute(typeof(PropertyBag.EntryEditor), typeof(UITypeEditor)));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));
            Properties["MoveType"] = new MetaProp("MoveType", typeof(MoveToType), new DisplayNameAttribute("MoveTo Type"));
            Properties["Pathing"] = new MetaProp("Pathing", typeof(NavigationType), new DisplayNameAttribute("Use"));

            Entry = 0u;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            MoveType = MoveToType.Location;
            Pathing = NavigationType.Navigator;

            Properties["Entry"].Show = false;
            Properties["MoveType"].PropertyChanged += new EventHandler(MoveToAction_PropertyChanged);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
        }

        void LocationChanged(object sender, EventArgs e)
        {
            MetaProp mp = (MetaProp)sender;
            loc = Util.StringToWoWPoint(Location);
            Properties["Location"].PropertyChanged -= new EventHandler(LocationChanged);
            Properties["Location"].Value = string.Format("<{0}, {1}, {2}>", loc.X, loc.Y, loc.Z);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            RefreshPropertyGrid();
        }

        void MoveToAction_PropertyChanged(object sender, EventArgs e)
        {
            switch (MoveType)
            {
                case MoveToType.Location:
                    Properties["Entry"].Show = false;
                    Properties["Location"].Show = true;
                    Properties["Pathing"].Show = true;
                    break;
                case MoveToType.NpcByID:
                    Properties["Entry"].Show = true;
                    Properties["Location"].Show = false;
                    Properties["Pathing"].Show = false;
                    break;
                default:
                    Properties["Entry"].Show = false;
                    Properties["Location"].Show = false;
                    Properties["Pathing"].Show = false;
                    break;
            }
            RefreshPropertyGrid();
        }

        Stopwatch concludingSw = new Stopwatch();
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {

                if (MoveType != MoveToType.Location)
                {
                    loc = GetLocationFromType(MoveType, Entry);
                    if (loc == WoWPoint.Zero)
                    {
                        if (locationDb == WoWPoint.Zero)
                        {
                            locationDb = GetLocationFromDB(MoveType, Entry);
                        }
                        loc = locationDb;
                    }
                    if (loc == WoWPoint.Zero)
                    {
                        Professionbuddy.Err("MoveToAction Failed.. Unable to find location from Database");
                        IsDone = true;
                        return RunStatus.Failure;
                    }
                }
                if (Entry > 0 && (!ObjectManager.Me.GotTarget || ObjectManager.Me.CurrentTarget.Entry != Entry))
                {
                    WoWUnit unit = ObjectManager.GetObjectsOfType<WoWUnit>(true).FirstOrDefault(u => u.Entry == Entry);
                    if (unit != null)
                        unit.Target();
                }
                if (ObjectManager.Me.Location.Distance(loc) > 4.5)
                {
                    if (Pathing == NavigationType.ClickToMove)
                        WoWMovement.ClickToMove(loc);
                    else
                        Util.MoveTo(loc);
                }
                else
                {
                    if (!concludingSw.IsRunning)
                        concludingSw.Start();
                    else if (concludingSw.ElapsedMilliseconds >= 2000)
                    {
                        IsDone = true;
                        Professionbuddy.Log("MoveTo Action completed for type {0}", MoveType);
                        concludingSw.Stop();
                        concludingSw.Reset();
                    }
                }
                if (!IsDone)
                    return RunStatus.Running;
            }
            return RunStatus.Failure;
        }

        static public WoWPoint GetLocationFromType(MoveToType type, uint entry)
        {
            WoWObject obj = null;
            if (entry != 0)
            {
                obj = ObjectManager.ObjectList.FirstOrDefault(o => o.Entry == entry);
            }
            if (obj == null)
            {
                switch (type)
                {
                    case MoveToType.NearestAH:
                        obj = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsAuctioneer && u.IsAlive).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                    case MoveToType.NearestBanker:
                        obj = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsBanker && u.IsAlive).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                    case MoveToType.NearestFlight:
                        obj = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsFlightMaster && u.IsFriendly && u.IsAlive).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                    case MoveToType.NearestGB:
                        obj = ObjectManager.ObjectList.Where(u => {
                            if (u is WoWUnit)
                            {
                                WoWUnit un = (WoWUnit)u;
                                if (un.IsGuildBanker)
                                    return true;
                            }
                            else if (u is WoWGameObject)
                            {
                                WoWGameObject go = (WoWGameObject)u;
                                if (go.SubType == WoWGameObjectType.GuildBank)
                                    return true;
                            }
                            return false;
                        }).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                    case MoveToType.NearestMailbox:
                        obj = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(u => u.SubType == WoWGameObjectType.Mailbox).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                    case MoveToType.NearestReagentVendor:
                        obj = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsReagentVendor && u.IsAlive).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                    case MoveToType.NearestRepair:
                        obj = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsRepairMerchant && u.IsAlive).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                    case MoveToType.NearestVendor:
                        obj = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsAnyVendor && u.IsAlive).OrderBy(u => u.Distance).FirstOrDefault();
                        break;
                }
            }
            if (obj != null)
            {
                if (obj is WoWUnit && (!ObjectManager.Me.GotTarget || ObjectManager.Me.CurrentTarget != obj))
                {
                    entry = ((WoWUnit)obj).Entry;
                    ((WoWUnit)obj).Target();
                }
                return obj.Location;
            }
            else return WoWPoint.Zero;
        }

        static public WoWPoint GetLocationFromDB(MoveToType type, uint entry)
        {
            NpcResult npcResults = null;
            switch (type)
            {
                case MoveToType.NearestAH:
                    npcResults = NpcQueries.GetNearestNpc(ObjectManager.Me.FactionTemplate, ObjectManager.Me.MapId,
                                                 ObjectManager.Me.Location, UnitNPCFlags.Auctioneer);
                    break;
                case MoveToType.NearestBanker:
                    npcResults = NpcQueries.GetNearestNpc(ObjectManager.Me.FactionTemplate, ObjectManager.Me.MapId,
                                                 ObjectManager.Me.Location, UnitNPCFlags.Banker);
                    break;
                case MoveToType.NearestFlight:
                    npcResults = NpcQueries.GetNearestNpc(ObjectManager.Me.FactionTemplate, ObjectManager.Me.MapId,
                                                 ObjectManager.Me.Location, UnitNPCFlags.Flightmaster);
                    break;
                case MoveToType.NearestGB:
                    npcResults = NpcQueries.GetNearestNpc(ObjectManager.Me.FactionTemplate, ObjectManager.Me.MapId,
                                                 ObjectManager.Me.Location, UnitNPCFlags.GuildBanker);
                    break;
                case MoveToType.NearestReagentVendor:
                    npcResults = NpcQueries.GetNearestNpc(ObjectManager.Me.FactionTemplate, ObjectManager.Me.MapId,
                                                 ObjectManager.Me.Location, UnitNPCFlags.ReagentVendor);
                    break;
                case MoveToType.NearestRepair:
                    npcResults = NpcQueries.GetNearestNpc(ObjectManager.Me.FactionTemplate, ObjectManager.Me.MapId,
                                                 ObjectManager.Me.Location, UnitNPCFlags.Repair);
                    break;
                case MoveToType.NearestVendor:
                    npcResults = NpcQueries.GetNearestNpc(ObjectManager.Me.FactionTemplate, ObjectManager.Me.MapId,
                                                 ObjectManager.Me.Location, UnitNPCFlags.Vendor);
                    break;
                case MoveToType.NpcByID:
                    npcResults = NpcQueries.GetNpcById(entry);
                    break;
            }
            if (npcResults != null)
                return npcResults.Location;
            else
                return WoWPoint.Zero;
        }
        public override string Name { get { return "Move To"; } }
        public override string Title
        {
            get
            {
                return string.Format("{0}: {1} " +
                    ((MoveType == MoveToType.Location) ? Location.ToString() : ""), Name, MoveType);
            }
        }
        public override string Help
        {
            get
            {
                return "This action will move your character to the selected location or if set to go to NearestXXX it will search the object manager and NPC database for the specific unit/object and move to it. It will also target NPCs so the interact action can be used right after this action finishes without having to specify an Npc Entry in it.";
            }
        }
        public override object Clone()
        {
            return new MoveToAction() { MoveType = this.MoveType, Entry = this.Entry, loc = this.loc, Location = this.Location, Pathing = this.Pathing };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            MoveType = (MoveToType)Enum.Parse(typeof(MoveToType), reader["MoveType"]);
            if (reader.MoveToAttribute("Pathing"))
                Pathing = (NavigationType)Enum.Parse(typeof(NavigationType), reader["Pathing"]);
            uint num;
            uint.TryParse(reader["Entry"], out num);
            Entry = num;
            float x, y, z;
            x = reader["X"].ToSingle();
            y = reader["Y"].ToSingle();
            z = reader["Z"].ToSingle();
            loc = new WoWPoint(x, y, z);
            Location = loc.ToInvariantString();
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("MoveType", MoveType.ToString());
            writer.WriteAttributeString("Pathing", Pathing.ToString());
            writer.WriteAttributeString("Entry", Entry.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
        }
        #endregion
    }
    #endregion
}

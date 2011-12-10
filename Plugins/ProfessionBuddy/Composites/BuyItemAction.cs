using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using System.Diagnostics;

using System.Linq;
using Styx;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Styx.Helpers;
using System.Collections.Generic;

namespace HighVoltz.Composites
{
    #region BuyItemAction
    class BuyItemAction : PBAction
    {
        public enum BuyItemActionType
        {
            SpecificItem,
            Material,
        }
        public uint NpcEntry {
            get { return (uint)Properties["NpcEntry"].Value; }
            set { Properties["NpcEntry"].Value = value; }
        }
        WoWPoint loc;
        public string Location {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }
        public string ItemID {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }
        public BuyItemActionType BuyItemType {
            get { return (BuyItemActionType)Properties["BuyItemType"].Value; }
            set { Properties["BuyItemType"].Value = value; }
        }
        public uint Count {
            get { return (uint)Properties["Count"].Value; }
            set { Properties["Count"].Value = value; }
        }
        public bool BuyAdditively
        {
            get { return (bool)Properties["BuyAdditively"].Value; }
            set { Properties["BuyAdditively"].Value = value; }
        }
        public BuyItemAction() {
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));
            Properties["NpcEntry"] = new MetaProp("NpcEntry", typeof(uint), new EditorAttribute(typeof(PropertyBag.EntryEditor), typeof(UITypeEditor)));
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string));
            Properties["Count"] = new MetaProp("Count", typeof(uint));
            Properties["BuyItemType"] = new MetaProp("BuyItemType", typeof(BuyItemActionType), new DisplayNameAttribute("Buy"));
            Properties["BuyAdditively"] = new MetaProp("BuyAdditively", typeof(bool), new DisplayNameAttribute("Buy Additively"));

            ItemID = "";
            Count = 1u;
            BuyItemType = BuyItemActionType.Material;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            NpcEntry = 0u;
            BuyAdditively = true;

            Properties["ItemID"].Show = false;
            Properties["Count"].Show = false;
            Properties["BuyAdditively"].Show = false;
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            Properties["BuyItemType"].PropertyChanged += new EventHandler(BuyItemAction_PropertyChanged);
        }
        void LocationChanged(object sender, EventArgs e) {
            MetaProp mp = (MetaProp)sender;
            loc = Util.StringToWoWPoint((string)((MetaProp)sender).Value);
            Properties["Location"].PropertyChanged -= new EventHandler(LocationChanged);
            Properties["Location"].Value = string.Format("<{0}, {1}, {2}>", loc.X, loc.Y, loc.Z);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            RefreshPropertyGrid();
        }
        void BuyItemAction_PropertyChanged(object sender, EventArgs e) {
            switch (BuyItemType)
            {
                case BuyItemActionType.Material:
                    Properties["ItemID"].Show = false;
                    Properties["Count"].Show = false;
                    Properties["BuyAdditively"].Show = false;
                    break;
                case BuyItemActionType.SpecificItem:
                    Properties["ItemID"].Show = true;
                    Properties["Count"].Show = true;
                    Properties["BuyAdditively"].Show = true;
                    break;
            }
            RefreshPropertyGrid();
        }
        Stopwatch _concludingSw = new Stopwatch();// add pause at the end to give objectmanager a chance to update.
        protected override RunStatus Run(object context) {
            if (!IsDone)
            {
                if (MerchantFrame.Instance == null || !MerchantFrame.Instance.IsVisible)
                {
                    WoWPoint movetoPoint = loc;
                    WoWUnit unit = null;
                    unit = ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.Entry == NpcEntry).
                        OrderBy(o => o.Distance).FirstOrDefault();
                    if (unit != null)
                        movetoPoint = WoWMathHelper.CalculatePointFrom(me.Location, unit.Location, 3);
                    else if (movetoPoint == WoWPoint.Zero)
                        movetoPoint = MoveToAction.GetLocationFromDB(MoveToAction.MoveToType.NpcByID, NpcEntry);
                    if (movetoPoint != WoWPoint.Zero && ObjectManager.Me.Location.Distance(movetoPoint) > 4.5)
                    {
                        Util.MoveTo(movetoPoint);
                    }
                    else if (unit != null)
                    {
                        unit.Target();
                        unit.Interact();
                    }
                    if (GossipFrame.Instance != null && GossipFrame.Instance.IsVisible &&
                        GossipFrame.Instance.GossipOptionEntries != null)
                    {
                        foreach (GossipEntry ge in GossipFrame.Instance.GossipOptionEntries)
                        {
                            if (ge.Type == GossipEntry.GossipEntryType.Vendor)
                            {
                                GossipFrame.Instance.SelectGossipOption(ge.Index);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // check if we have merchant frame open at correct NPC
                    if (NpcEntry > 0 && me.GotTarget && me.CurrentTarget.Entry != NpcEntry)
                    {
                        MerchantFrame.Instance.Close();
                        return RunStatus.Running;
                    }
                    if (!_concludingSw.IsRunning)
                    {
                        if (BuyItemType == BuyItemActionType.SpecificItem)
                        {
                            List<uint> idList = new List<uint>();
                            string[] entries = ItemID.Split(',');
                            if (entries != null && entries.Length > 0) {
                                foreach (var entry in entries) {
                                    uint temp = 0;
                                    uint.TryParse(entry.Trim(), out temp);
                                    idList.Add(temp);
                                }
                            }
                            else {
                                Professionbuddy.Err("No ItemIDs are specified");
                                IsDone = true;
                                return RunStatus.Failure;
                            }
                            foreach (uint id in idList)
                                buyItem(id, !BuyAdditively ? Count - Util.GetCarriedItemCount(id) : Count);
                        }
                        else if (BuyItemType == BuyItemActionType.Material)
                        {
                            foreach (var kv in Pb.MaterialList)
                            {
                                // only buy items if we don't have enough in bags...
                                int amount = kv.Value - (int)Ingredient.GetInBagItemCount(kv.Key);
                                if (amount > 0)
                                    buyItem(kv.Key, (uint)amount);
                            }
                        }
                        _concludingSw.Start();
                    }
                    if (_concludingSw.ElapsedMilliseconds >= 2000)
                    {
                        Professionbuddy.Log("BuyItemAction Completed ");
                        IsDone = true;
                    }
                }
                if (!IsDone)
                    return RunStatus.Running;
            }
            return RunStatus.Failure;
        }

        static public void buyItem(uint id, uint count) {
            bool found = false;
            foreach (MerchantItem mi in MerchantFrame.Instance.GetAllMerchantItems())
            {
                if (mi.ItemId == id)
                {
                    // since BuyItem can only by up to 20 items we need to run it multiple times when buying over 20 items
                    int stacks = (int)(count / 20);
                    int leftovers = (int)(count % 20);
                    if (count >= 20)
                    {
                        //using (new FrameLock()) // framelock was causing DCs
                        //{
                            for (int i = 0; i < stacks; i++)
                                MerchantFrame.Instance.BuyItem(mi.Index, 20);
                            if (leftovers > 0)
                                MerchantFrame.Instance.BuyItem(mi.Index, leftovers);
                        //}
                    }
                    else
                        MerchantFrame.Instance.BuyItem(mi.Index, leftovers);
                    found = true;
                    break;
                }
            }
            Professionbuddy.Log("item {0} {1}", id, found ? "bought " : "not found");
        }

        public override void Reset() {
            base.Reset();
            _concludingSw = new Stopwatch();
        }
        public override string Name {
            get { return "Buy Item"; }
        }
        public override string Title {
            get { return string.Format("{0}: " + (BuyItemType == BuyItemActionType.SpecificItem ? "{1} x{2}" : "{3}"), Name, ItemID, Count, BuyItemType); }
        }

        public override string Help {
            get {
                return "This action will buy items from a merchant frame, either a specific item or any materials the NPC has that are needed for recipes in the action tree. BuyAdditively if set to true will buy axact amount of items regardless of item count player has in bags";
            }
        }

        public override object Clone() {
            return new BuyItemAction()
            {
                Count = this.Count,
                ItemID = this.ItemID,
                BuyItemType = this.BuyItemType,
                Location = this.Location,
                NpcEntry = this.NpcEntry,
                BuyAdditively = this.BuyAdditively
            };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader) {
            uint val;
            if (reader.MoveToAttribute("ItemID"))
                ItemID = reader["ItemID"];
            else if (reader.MoveToAttribute("Entry"))
                ItemID = reader["Entry"]; 
            uint.TryParse(reader["Count"], out val);
            Count = val;
            BuyItemType = (BuyItemActionType)Enum.Parse(typeof(BuyItemActionType), reader["BuyItemType"]);
            if (reader.MoveToAttribute("NpcEntry"))
            {
                uint.TryParse(reader["NpcEntry"], out val);
                NpcEntry = val;
            }
            if (reader.MoveToAttribute("BuyAdditively"))
            {
                bool boolVal = false;
                bool.TryParse(reader["BuyAdditively"], out boolVal);
                BuyAdditively = boolVal;
            }
            if (reader.MoveToAttribute("X"))
            {
                float x, y, z;
                x = reader["X"].ToSingle();
                y = reader["Y"].ToSingle();
                z = reader["Z"].ToSingle();
                loc = new WoWPoint(x, y, z);
                Location = loc.ToInvariantString();
            }
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("NpcEntry", NpcEntry.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
            writer.WriteAttributeString("ItemID", ItemID);
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("BuyItemType", BuyItemType.ToString());
            writer.WriteAttributeString("BuyAdditively", BuyAdditively.ToString());
        }
        #endregion
    }
    #endregion
}

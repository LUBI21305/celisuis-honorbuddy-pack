using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Xml;
using Styx;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;
using System.Reflection;


namespace HighVoltz.Composites
{
    #region GetItemfromBankAction
    public class GetItemfromBankAction : PBAction
    {
        // number of times the recipe will be crafted
        public enum BankWithdrawlItemType
        {
            SpecificItem,
            Materials,
        }
        public BankType Bank
        {
            get { return (BankType)Properties["Bank"].Value; }
            set { Properties["Bank"].Value = value; }
        }
        public uint MinFreeBagSlots
        {
            get { return (uint)Properties["MinFreeBagSlots"].Value; }
            set { Properties["MinFreeBagSlots"].Value = value; }
        }
        public BankWithdrawlItemType GetItemfromBankType
        {
            get { return (BankWithdrawlItemType)Properties["GetItemfromBankType"].Value; }
            set { Properties["GetItemfromBankType"].Value = value; }
        }
        public string ItemID
        {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }
        public uint NpcEntry
        {
            get { return (uint)Properties["NpcEntry"].Value; }
            set { Properties["NpcEntry"].Value = value; }
        }
        public int Amount
        {
            get { return (int)Properties["Amount"].Value; }
            set { Properties["Amount"].Value = value; }
        }
        public bool AutoFindBank
        {
            get { return (bool)Properties["AutoFindBank"].Value; }
            set { Properties["AutoFindBank"].Value = value; }
        }
        public bool WithdrawAdditively
        {
            get { return (bool)Properties["WithdrawAdditively"].Value; }
            set { Properties["WithdrawAdditively"].Value = value; }
        }
        WoWPoint loc;
        public string Location
        {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }
        public GetItemfromBankAction()
        {
            Properties["Amount"] = new MetaProp("Amount", typeof(int));
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string));

            Properties["MinFreeBagSlots"] = new MetaProp("MinFreeBagSlots", typeof(uint), new DisplayNameAttribute("Min Free Bagslots"));
            Properties["GetItemfromBankType"] = new MetaProp("GetItemfromBankType", typeof(BankWithdrawlItemType), new DisplayNameAttribute("Items to Withdraw"));
            Properties["Bank"] = new MetaProp("Bank", typeof(BankType));
            Properties["AutoFindBank"] = new MetaProp("AutoFindBank", typeof(bool), new DisplayNameAttribute("Auto find Bank"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));
            Properties["NpcEntry"] = new MetaProp("NpcEntry", typeof(uint), new EditorAttribute(typeof(PropertyBag.EntryEditor), typeof(UITypeEditor)));
            Properties["WithdrawAdditively"] = new MetaProp("WithdrawAdditively", typeof(bool), new DisplayNameAttribute("Withdraw Additively"));

            Amount = 1;
            ItemID = "";
            MinFreeBagSlots = 2u;
            GetItemfromBankType = BankWithdrawlItemType.SpecificItem;
            Bank = BankType.Personal;
            AutoFindBank = true;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            NpcEntry = 0u;
            WithdrawAdditively = true;

            Properties["Location"].Show = false;
            Properties["NpcEntry"].Show = false;
            Properties["AutoFindBank"].PropertyChanged += new EventHandler(AutoFindBankChanged);
            Properties["GetItemfromBankType"].PropertyChanged += new EventHandler(GetItemfromBankAction_PropertyChanged);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
        }

        #region Callbacks
        void LocationChanged(object sender, EventArgs e)
        {
            MetaProp mp = (MetaProp)sender;
            loc = Util.StringToWoWPoint((string)((MetaProp)sender).Value);
            Properties["Location"].PropertyChanged -= new EventHandler(LocationChanged);
            Properties["Location"].Value = string.Format("<{0}, {1}, {2}>", loc.X, loc.Y, loc.Z);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            RefreshPropertyGrid();
        }

        void AutoFindBankChanged(object sender, EventArgs e)
        {
            if (AutoFindBank)
            {
                Properties["Location"].Show = false;
                Properties["NpcEntry"].Show = false;
            }
            else
            {
                Properties["Location"].Show = true;
                Properties["NpcEntry"].Show = true;
            }
            RefreshPropertyGrid();
        }
        void GetItemfromBankAction_PropertyChanged(object sender, EventArgs e)
        {
            switch (GetItemfromBankType)
            {
                case BankWithdrawlItemType.SpecificItem:
                    Properties["Amount"].Show = true;
                    Properties["ItemID"].Show = true;
                    Properties["WithdrawAdditively"].Show = true;
                    break;
                case BankWithdrawlItemType.Materials:
                    Properties["Amount"].Show = false;
                    Properties["ItemID"].Show = false;
                    Properties["WithdrawAdditively"].Show = false;
                    break;
            }
            RefreshPropertyGrid();
        }

        #endregion

        // key= itemID; value amount to withdrawl
        Dictionary<uint, int> ItemList = null;
        bool IsGbankFrameVisible { get { return Lua.GetReturnVal<int>("if GuildBankFrame and GuildBankFrame:IsVisible() then return 1 else return 0 end ", 0) == 1; } }
        Stopwatch _itemsSW;
        Stopwatch _gbankItemThrottleSW = new Stopwatch();
        const long _gbankItemThrottle = 167; // 6 times per sec.. round up to nearest 1.
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                if ((Bank == BankType.Guild && !IsGbankFrameVisible) ||
                    (Bank == BankType.Personal && !Util.IsBankFrameOpen))
                {
                    MoveToBanker();
                }
                else
                {
                    if (_itemsSW == null)
                    {
                        _itemsSW = new Stopwatch();
                        _itemsSW.Start();
                    }
                    else if (_itemsSW.ElapsedMilliseconds < Util.WoWPing * 3)
                        return RunStatus.Running;
                    if (ItemList == null)
                        ItemList = BuildItemList();
                    // no bag space... 
                    if (me.FreeNormalBagSlots <= MinFreeBagSlots || ItemList.Count == 0)
                        IsDone = true;
                    else
                    {
                        KeyValuePair<uint, int> kv = ItemList.FirstOrDefault();
                        bool done = false;
                        if (Bank == BankType.Personal)
                            done = GetItemFromBank(kv.Key, kv.Value);
                        else
                        {
                            // throttle the amount of items being withdrawn from gbank per sec
                            if (!_gbankItemThrottleSW.IsRunning)
                                _gbankItemThrottleSW.Start();
                            if (_gbankItemThrottleSW.ElapsedMilliseconds < _gbankItemThrottle)
                                return RunStatus.Success;
                            else
                            {
                                _gbankItemThrottleSW.Reset();
                                _gbankItemThrottleSW.Start();
                            }
                            done = GetItemFromGBank(kv.Key, kv.Value);
                        }
                        if (done)
                            ItemList.Remove(kv.Key);
                    }
                    _itemsSW.Reset();
                    _itemsSW.Start();
                }
                if (IsDone)
                {
                    Professionbuddy.Log("Removed Item:[{0}] from {1} Bank", ItemID, Bank);
                }
                else
                    return RunStatus.Running;
            }
            return RunStatus.Failure;
        }

        WoWObject GetLocalBanker()
        {
            WoWObject bank = null;
            List<WoWObject> bankers = null;
            if (Bank == BankType.Guild)
                bankers = (from banker in ObjectManager.ObjectList
                           where (banker is WoWGameObject && ((WoWGameObject)banker).SubType == WoWGameObjectType.GuildBank) ||
                             (banker is WoWUnit && ((WoWUnit)banker).IsGuildBanker && ((WoWUnit)banker).IsAlive && ((WoWUnit)banker).CanSelect)
                           select banker).ToList();
            else
                bankers = (from banker in ObjectManager.ObjectList
                           where (banker is WoWUnit &&
                                ((WoWUnit)banker).IsBanker &&
                                ((WoWUnit)banker).IsAlive &&
                                ((WoWUnit)banker).CanSelect)
                           select banker).ToList();

            if (bankers != null)
            {
                if (!AutoFindBank && NpcEntry != 0)
                    bank = bankers.Where(b => b.Entry == NpcEntry).OrderBy(o => o.Distance).FirstOrDefault();
                else if (AutoFindBank || loc == WoWPoint.Zero || NpcEntry == 0)
                    bank = bankers.OrderBy(o => o.Distance).FirstOrDefault();
                else if (ObjectManager.Me.Location.Distance(loc) <= 90)
                {
                    bank = bankers.Where(o => o.Location.Distance(loc) < 10).
                        OrderBy(o => o.Distance).FirstOrDefault();
                }
            }
            return bank;
        }

        void MoveToBanker()
        {
            WoWPoint movetoPoint = loc;
            WoWObject bank = GetLocalBanker();
            if (bank != null)
                movetoPoint = WoWMathHelper.CalculatePointFrom(me.Location, bank.Location, 3);
            // search the database
            else if (movetoPoint == WoWPoint.Zero)
            {
                if (Bank == BankType.Personal)
                    movetoPoint = MoveToAction.GetLocationFromDB(MoveToAction.MoveToType.NearestBanker, NpcEntry);
                else
                    movetoPoint = MoveToAction.GetLocationFromDB(MoveToAction.MoveToType.NearestGB, NpcEntry);
            }
            if (movetoPoint == WoWPoint.Zero)
            {
                IsDone = true;
                Professionbuddy.Err("Unable to find bank");
            }
            if (movetoPoint.Distance(ObjectManager.Me.Location) > 4)
            {
                Util.MoveTo(movetoPoint);
            }
            // since there are many personal bank replacement addons I can't just check if frame is open and be generic.. using events isn't reliable
            else if (bank != null)
            {
                bank.Interact();
            }
            else
            {
                IsDone = true;
                Logging.Write(System.Drawing.Color.Red, "Unable to find a banker at location. aborting");
            }
        }

        Dictionary<uint, int> BuildItemList()
        {
            Dictionary<uint, int> items = new Dictionary<uint, int>();
            switch (GetItemfromBankType)
            {
                case BankWithdrawlItemType.SpecificItem:
                    //List<uint> idList = new List<uint>();
                    string[] entries = ItemID.Split(',');
                    if (entries != null && entries.Length > 0)
                    {
                        foreach (var entry in entries)
                        {
                            uint temp = 0;
                            uint.TryParse(entry.Trim(), out temp);
                            items.Add(temp, !WithdrawAdditively ? Amount - (int)Util.GetCarriedItemCount(temp) : Amount);
                        }
                    }
                    else
                    {
                        Professionbuddy.Err("No ItemIDs are specified");
                        IsDone = true;
                    }
                    break;
                case BankWithdrawlItemType.Materials:
                    foreach (var kv in Pb.MaterialList)
                        items.Add(kv.Key, kv.Value);
                    break;
            }
            return items;
        }

        // returns true when done. supports pulsing.
        static Stopwatch queueServerSW;
        public bool GetItemFromGBank(uint id, int amount)
        {
            if (queueServerSW == null)
            {
                queueServerSW = new Stopwatch();
                queueServerSW.Start();
                Lua.DoString("for i=GetNumGuildBankTabs(), 1, -1 do QueryGuildBankTab(i) end ");
                Professionbuddy.Log("Queuing server for gbank info");
                return false;
            }
            if (queueServerSW.ElapsedMilliseconds < 2000)
            {
                return false;
            }
            string lua = string.Format(
                "local tabnum = GetNumGuildBankTabs() " +
                "local bagged = 0 " +
                "local amount = {1} " +
                "if GuildBankFrame and GuildBankFrame:IsVisible() then " +
                   "for tab = 1,tabnum do " +
                      "local _,_,iv,_,_, rw = GetGuildBankTabInfo(tab) " +
                      "if iv then " +
                         "SetCurrentGuildBankTab(tab) " +
                         "local  sawItem = 0  " +
                         "for slot = 1, 98 do " +
                            "local _,c,l=GetGuildBankItemInfo(tab, slot) " +
                            "local id = tonumber(string.match(GetGuildBankItemLink(tab, slot) or '','|Hitem:(%d+)')) " +
                            "if l == nil and id == {0} then " +
                               "sawItem = 1 " +
                               "if c + bagged <= amount then " +
                                  "AutoStoreGuildBankItem(tab, slot) " +
                                  "bagged = bagged + c " +
                               "else " +
                                  "local itemf  = GetItemFamily(id) " +
                                  "for bag =4 ,0 ,-1 do " +
                                     "local fs,bfamily = GetContainerNumFreeSlots(bag) " +
                                     "if fs > 0 and (bfamily == 0 or bit.band(itemf, bfamily) > 0) then " +
                                        "local freeSlots = GetContainerFreeSlots(bag) " +
                                        "SplitGuildBankItem(tab, slot, amount-bagged) " +
                                        "PickupContainerItem(bag, freeSlots[1]) " +
                                        "return bagged " +
                                     "end " +
                                  "end " +
                               "end " +
                            "end " +
                         "end " +
                      "end " +
                   "end " +
                   "if sawItem == 0 then return -1 else return bagged end " +
                "end " +
                "return -2 "
            , id, amount);
            Professionbuddy.Log("Attempting to withdraw items");
            int retVal = Lua.GetReturnVal<int>(lua, 0);
            // -1 means no item was found.
            if (retVal == -1)
            {
                Professionbuddy.Log("No items with entry {0} could be found in gbank", id);
            }
            else if (retVal == -2) // frame was not visible
            {
                Professionbuddy.Log("Guildbank frame was not visible, skipping withdrawl");
            }
            queueServerSW = null;
            return true;
        }

        public bool GetItemFromBank(uint id, int amount)
        {
            string lua = string.Format(
                "local numSlots = GetNumBankSlots() " +
                "local splitUsed = 0 " +
                "local bagged = 0 " +
                "local amount = {1} " +
                "local bag1 = numSlots + 4  " +
                "while bag1 >= -1 do " +
                   "if bag1 == 4 then " +
                      "bag1 = -1 " +
                   "end " +
                   "for slot1 = 1, GetContainerNumSlots(bag1) do " +
                      "local _,c,l=GetContainerItemInfo(bag1, slot1) " +
                      "local id = GetContainerItemID(bag1,slot1) " +
                      "if l ~= 1 and  id == {0} then " +
                         "if c + bagged <= amount  then " +
                            "UseContainerItem(bag1,slot1) " +
                            "bagged = bagged + c " +
                         "else " +
                            "local itemf  = GetItemFamily(id) " +
                            "for bag2 = 0,4 do " +
                               "local fs,bfamily = GetContainerNumFreeSlots(bag2) " +
                               "if fs > 0 and (bfamily == 0 or bit.band(itemf, bfamily) > 0) then " +
                                  "local freeSlots = GetContainerFreeSlots(bag2) " +
                                  "SplitContainerItem(bag1,slot1,amount - bagged) " +
                                  "if bag2 == 0 then PutItemInBackpack() else PutItemInBag(bag2) end " +
                                  "return " +
                               "end " +
                               "bag2 = bag2 -1 " +
                            "end " +
                         "end " +
                         "if bagged >= amount then return end " +
                      "end " +
                   "end " +
                   "bag1 = bag1 -1 " +
                "end "
            , id, amount);
            Lua.DoString(lua);
            return true;
        }

        public override void Reset()
        {
            base.Reset();
            queueServerSW = null;
            ItemList = null;
            _itemsSW = null;
        }

        public override string Name { get { return "Withdraw Item From Bank"; } }
        public override string Title
        {
            get
            {
                return string.Format("{0}: " + (GetItemfromBankType == BankWithdrawlItemType.SpecificItem ?
                    " {1} {2}" : ""), Name, ItemID, Amount);
            }
        }
        public override string Help
        {
            get
            {
                return "This action will withdraw the specified item from your personal or guild bank, it can also withdraw items needed for your recipes in the action tree.WithdrawAdditively if set to true will buy axact amount of items regardless of item count player has in bags";
            }
        }
        public override object Clone()
        {
            return new GetItemfromBankAction()
            {
                ItemID = this.ItemID,
                Amount = this.Amount,
                Bank = this.Bank,
                GetItemfromBankType = this.GetItemfromBankType,
                loc = this.loc,
                AutoFindBank = this.AutoFindBank,
                NpcEntry = this.NpcEntry,
                Location = this.Location,
                MinFreeBagSlots = this.MinFreeBagSlots,
                WithdrawAdditively = this.WithdrawAdditively
            };
        }

        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            uint id;
            if (reader.MoveToAttribute("ItemID"))
                ItemID = reader["ItemID"];
            else if (reader.MoveToAttribute("Entry"))
                ItemID = reader["Entry"];

            uint.TryParse(reader["Amount"], out id);
            Amount = (int)id;
            Bank = (BankType)Enum.Parse(typeof(BankType), reader["Bank"]);
            GetItemfromBankType = (BankWithdrawlItemType)Enum.Parse(typeof(BankWithdrawlItemType), reader["GetItemfromBankType"]);
            uint.TryParse(reader["NpcEntry"], out id);
            NpcEntry = id;
            bool boolVal;
            bool.TryParse(reader["AutoFindBank"], out boolVal);
            AutoFindBank = boolVal;
            float x, y, z;
            x = reader["X"].ToSingle();
            y = reader["Y"].ToSingle();
            z = reader["Z"].ToSingle();
            loc = new WoWPoint(x, y, z);
            Location = loc.ToInvariantString();
            if (reader.MoveToAttribute("MinFreeBagSlots"))
            {
                uint.TryParse(reader["MinFreeBagSlots"], out id);
                MinFreeBagSlots = id;
            }
            if (reader.MoveToAttribute("WithdrawAdditively"))
            {
                bool.TryParse(reader["WithdrawAdditively"], out boolVal);
                WithdrawAdditively = boolVal;
            }
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ItemID", ItemID);
            writer.WriteAttributeString("Amount", Amount.ToString());
            writer.WriteAttributeString("Bank", Bank.ToString());
            writer.WriteAttributeString("GetItemfromBankType", GetItemfromBankType.ToString());
            writer.WriteAttributeString("NpcEntry", NpcEntry.ToString());
            writer.WriteAttributeString("AutoFindBank", AutoFindBank.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
            writer.WriteAttributeString("MinFreeBagSlots", MinFreeBagSlots.ToString());
            writer.WriteAttributeString("WithdrawAdditively", WithdrawAdditively.ToString());
        }
        #endregion
    }
    #endregion

}

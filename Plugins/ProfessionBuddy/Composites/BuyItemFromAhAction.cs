using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Styx;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWCache;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz.Composites
{
    struct BuyItemEntry
    {
        public string Name;     // localized name
        public uint Id;                // item ID 
        public uint BuyAmount;            // amount to buy
    }

    #region BuyItemFromAhAction
    class BuyItemFromAhAction : PBAction
    {
        public enum ItemType
        {
            Item,
            RecipeMats,
            MaterialList,
        }
        public ItemType ItemListType
        {
            get { return (ItemType)Properties["ItemListType"].Value; }
            set { Properties["ItemListType"].Value = value; }
        }
        public string ItemID
        {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }

        public PropertyBag.GoldEditor MaxBuyout
        {
            get { return (PropertyBag.GoldEditor)Properties["MaxBuyout"].Value; }
            set { Properties["MaxBuyout"].Value = value; }
        }

        public uint Amount
        {
            get { return (uint)Properties["Amount"].Value; }
            set { Properties["Amount"].Value = value; }
        }
        public bool BuyAdditively
        {
            get { return (bool)Properties["BuyAdditively"].Value; }
            set { Properties["BuyAdditively"].Value = value; }
        }

        public bool AutoFindAh
        {
            get { return (bool)Properties["AutoFindAh"].Value; }
            set { Properties["AutoFindAh"].Value = value; }
        }
        public bool BidOnItem
        {
            get { return (bool)Properties["BidOnItem"].Value; }
            set { Properties["BidOnItem"].Value = value; }
        }
        WoWPoint loc;
        public string Location
        {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }

        public BuyItemFromAhAction()
        {
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string));
            Properties["MaxBuyout"] = new MetaProp("MaxBuyout", typeof(PropertyBag.GoldEditor),
                new DisplayNameAttribute("Max Buyout"), new TypeConverterAttribute(typeof(PropertyBag.GoldEditorConverter)));
            Properties["Amount"] = new MetaProp("Amount", typeof(uint));
            Properties["ItemListType"] = new MetaProp("ItemListType", typeof(ItemType), new DisplayNameAttribute("Buy ..."));
            Properties["AutoFindAh"] = new MetaProp("AutoFindAh", typeof(bool), new DisplayNameAttribute("Auto find AH"));
            Properties["BuyAdditively"] = new MetaProp("BuyAdditively", typeof(bool), new DisplayNameAttribute("Buy Additively"));

            Properties["BidOnItem"] = new MetaProp("BidOnItem", typeof(bool), new DisplayNameAttribute("Bid on Item"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));

            ItemID = "";
            Amount = 1u;
            ItemListType = ItemType.Item;
            AutoFindAh = true;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            MaxBuyout = new PropertyBag.GoldEditor("100g0s0c");
            BidOnItem = false;
            BuyAdditively = true;

            Properties["AutoFindAh"].PropertyChanged += new EventHandler(AutoFindAHChanged);
            Properties["ItemListType"].PropertyChanged += new EventHandler(BuyItemFromAhAction_PropertyChanged);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            Properties["Amount"].Show = true;
            Properties["Location"].Show = false;
        }
        void LocationChanged(object sender, EventArgs e)
        {
            MetaProp mp = (MetaProp)sender;
            loc = Util.StringToWoWPoint((string)((MetaProp)sender).Value);
            Properties["Location"].PropertyChanged -= new EventHandler(LocationChanged);
            Properties["Location"].Value = string.Format("<{0}, {1}, {2}>", loc.X, loc.Y, loc.Z);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            RefreshPropertyGrid();
        }

        void AutoFindAHChanged(object sender, EventArgs e)
        {
            if (AutoFindAh)
            {
                Properties["Location"].Show = false;
            }
            else
            {
                Properties["Location"].Show = true;
            }
            RefreshPropertyGrid();
        }
        void BuyItemFromAhAction_PropertyChanged(object sender, EventArgs e)
        {
            if (ItemListType == ItemType.MaterialList)
            {
                Properties["ItemID"].Show = false;
                Properties["Amount"].Show = false;
                Properties["BuyAdditively"].Show = false;
            }
            else
            {
                Properties["ItemID"].Show = true;
                Properties["Amount"].Show = true;
                Properties["BuyAdditively"].Show = true;
            }
            RefreshPropertyGrid();
        }

        List<BuyItemEntry> ToQueueNameList;
        List<BuyItemEntry> ToBuyList;
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                using (new FrameLock())
                {
                    if (Lua.GetReturnVal<int>("if AuctionFrame and AuctionFrame:IsVisible() then return 1 else return 0 end ", 0) == 0)
                    {
                        MovetoAuctioneer();
                    }
                    else
                    {
                        if (ToQueueNameList == null)
                        {
                            ToQueueNameList = BuildItemList();
                            ToBuyList = new List<BuyItemEntry>();
                        }
                        if ((ToQueueNameList == null || ToQueueNameList.Count == 0) && ToBuyList.Count == 0)
                        {
                            IsDone = true;
                            return RunStatus.Failure;
                        }
                        if (ToQueueNameList.Count > 0)
                        {
                            string name = GetLocalName(ToQueueNameList[0].Id);
                            if (!string.IsNullOrEmpty(name))
                            {
                                var item = ToQueueNameList[0];
                                item.Name = name;
                                ToBuyList.Add(item);
                                ToQueueNameList.RemoveAt(0);
                            }
                        }
                        if (ToBuyList.Count > 0)
                        {
                            if (BuyFromAH(ToBuyList[0]))
                            {
                                ToBuyList.RemoveAt(0);
                            }
                        }
                    }
                }
                if (!IsDone)
                    return RunStatus.Running;
            }
            return RunStatus.Failure;
        }

        Stopwatch queueTimer = new Stopwatch();
        int totalAuctions = 0;
        int counter = 0;
        int page = 0;
        bool BuyFromAH(BuyItemEntry bie)
        {
            bool done = false;
            if (!queueTimer.IsRunning)
            {
                string lua = string.Format("QueryAuctionItems(\"{0}\" ,nil,nil,nil,nil,nil,{1}) return 1",
                    bie.Name.ToFormatedUTF8(), page);
                Lua.GetReturnVal<int>(lua, 0);
                Professionbuddy.Debug("Searching AH for {0}", bie.Name);
                queueTimer.Start();
            }
            else if (queueTimer.ElapsedMilliseconds <= 10000)
            {
                if (Lua.GetReturnVal<int>("if CanSendAuctionQuery('list') == 1 then return 1 else return 0 end ", 0) == 1)
                {
                    totalAuctions = Lua.GetReturnVal<int>("return GetNumAuctionItems('list')", 1);
                    queueTimer.Stop();
                    queueTimer.Reset();
                    if (totalAuctions > 0)
                    {
                        string lua = string.Format("local A,totalA= GetNumAuctionItems('list') local amountBought={0} local want={1} local each={3} local useBid={4} local buyPrice=0 for index=1, A do local name, _, count,_,_,_,_,minBid,minInc, buyoutPrice,bidNum,isHighBidder,_,_ = GetAuctionItemInfo('list', index) if useBid == 1 and buyoutPrice > each*count and isHighBidder == nil then if bidNum == nil then buyPrice =minBid + minInc else buyPrice = bidNum + minInc end else buyPrice = buyoutPrice end if name == \"{2}\" and buyPrice > 0 and buyPrice <= each*count and amountBought < want then amountBought = amountBought + count PlaceAuctionBid('list', index,buyPrice) end if amountBought >=  want then return -1 end end return amountBought",
                            counter, bie.BuyAmount, bie.Name.ToFormatedUTF8(), MaxBuyout.TotalCopper, BidOnItem == true ? 1 : 0);
                        counter = Lua.GetReturnVal<int>(lua, 0);
                        if (counter == -1 || ++page >= (int)Math.Ceiling((double)totalAuctions / 50))
                            done = true;
                    }
                    else
                        done = true;
                }
            }
            else
            {
                done = true;
            }
            if (done)
            {
                queueTimer = new Stopwatch();
                totalAuctions = 0;
                counter = 0;
                page = 0;
            }
            return done;
        }

        void MovetoAuctioneer()
        {
            WoWPoint movetoPoint = loc;
            WoWUnit auctioneer;
            if (AutoFindAh || movetoPoint == WoWPoint.Zero)
            {
                auctioneer = ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.IsAuctioneer && o.IsAlive)
                    .OrderBy(o => o.Distance).FirstOrDefault();
            }
            else
            {
                auctioneer = ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.IsAuctioneer
                    && o.Location.Distance(loc) < 5)
                    .OrderBy(o => o.Distance).FirstOrDefault();
            }
            if (auctioneer != null)
                movetoPoint = WoWMathHelper.CalculatePointFrom(me.Location, auctioneer.Location, 3);
            else if (movetoPoint == WoWPoint.Zero)
                movetoPoint = MoveToAction.GetLocationFromDB(MoveToAction.MoveToType.NearestAH, 0);
            if (movetoPoint == WoWPoint.Zero)
            {
                Logging.Write("Unable to location Auctioneer, Maybe he's dead?");
            }
            if (movetoPoint.Distance(ObjectManager.Me.Location) > 4.5)
            {
                Util.MoveTo(movetoPoint);
            }
            else if (auctioneer != null)
            {
                auctioneer.Interact();
            }
        }

        string GetLocalName(uint id)
        {
            Professionbuddy.Debug("Queueing server for Item: {0}", id);
            return TradeSkillFrame.GetItemCacheName(id);
        }

        List<BuyItemEntry> BuildItemList()
        {
            List<BuyItemEntry> list = new List<BuyItemEntry>();
            List<uint> idList = new List<uint>();
            string[] entries = ItemID.Split(',');
            if (entries != null && entries.Length > 0)
            {
                foreach (var entry in entries)
                {
                    uint temp = 0;
                    uint.TryParse(entry.Trim(), out temp);
                    idList.Add(temp);
                }
            }
            else
            {
                Professionbuddy.Err("No ItemIDs are specified");
                IsDone = true;
            }

            switch (ItemListType)
            {
                case ItemType.Item:
                    foreach (uint id in idList)
                        list.Add(new BuyItemEntry()
                        {
                            Id = id,
                            BuyAmount = !BuyAdditively ? Amount - Util.GetCarriedItemCount(id) : Amount
                        });
                    break;
                case ItemType.MaterialList:
                    foreach (var kv in Pb.MaterialList)
                    {
                        list.Add(new BuyItemEntry() { Id = kv.Key, BuyAmount = (uint)kv.Value });
                    }
                    break;
                case ItemType.RecipeMats:
                    foreach (uint id in idList)
                    {
                        Recipe recipe = (from tradeskill in Pb.TradeSkillList
                                         where tradeskill.Recipes.ContainsKey(id)
                                         select tradeskill.Recipes[id]).FirstOrDefault();
                        if (recipe != null)
                            foreach (var ingred in recipe.Ingredients)
                            {
                                // subtract whatever material we have in bags already
                                int toBuyAmount = (int)((ingred.Required * Amount) - Ingredient.GetInBagItemCount(ingred.ID));
                                if (toBuyAmount > 0)
                                    list.Add(new BuyItemEntry() { Id = ingred.ID, BuyAmount = (uint)toBuyAmount });
                            }
                    }
                    break;
            }
            return list;
        }

        public override void Reset()
        {
            base.Reset();
            ToQueueNameList = null;
            ToBuyList = null;
        }
        public override string Name
        {
            get { return "Buy Item From AH"; }
        }

        public override string Title
        {
            get
            {
                return string.Format("{0}: {1} " + (ItemListType != ItemType.MaterialList ? "x" + Amount : ""), Name,
                    ItemListType != ItemType.MaterialList ? ItemID.ToString() : "Material List");
            }
        }
        public override string Help
        {
            get
            {
                return "This action will buy a either a specified item, ingredients that a recipe requires or whatevers in the material list from the Auction house if item is below specified maximum price. The ItemID is the id of the item or the recipe Id if buying materials for a recipe.'Bid On Item', if set to true will bid on an item if buyout price > maxBuyout and minBid <= maxBuyout. ItemID accepts a comma separated list of Item IDs. BuyAdditively if set to true will buy axact amount of items regardless of item count player has in bags";
            }
        }

        public override object Clone()
        {
            return new BuyItemFromAhAction()
            {
                ItemID = this.ItemID,
                MaxBuyout = new PropertyBag.GoldEditor(this.MaxBuyout.ToString()),
                Amount = this.Amount,
                ItemListType = this.ItemListType,
                AutoFindAh = this.AutoFindAh,
                Location = this.Location,
                BidOnItem = this.BidOnItem,
                BuyAdditively = this.BuyAdditively,
            };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            uint val;
            if (reader.MoveToAttribute("ItemID"))
                ItemID = reader["ItemID"];
            else if (reader.MoveToAttribute("Entry"))
                ItemID = reader["Entry"];

            MaxBuyout = new PropertyBag.GoldEditor(reader["MaxBuyout"]);
            uint.TryParse(reader["Amount"], out val);
            Amount = val;
            ItemListType = (ItemType)Enum.Parse(typeof(ItemType), reader["ItemListType"]);
            bool boolVal;
            bool.TryParse(reader["AutoFindAh"], out boolVal);
            AutoFindAh = boolVal;

            if (reader.MoveToAttribute("BuyAdditively"))
            {
                bool.TryParse(reader["BuyAdditively"], out boolVal);
                BuyAdditively = boolVal;
            }

            float x, y, z;
            x = reader["X"].ToSingle();
            y = reader["Y"].ToSingle();
            z = reader["Z"].ToSingle();
            loc = new WoWPoint(x, y, z);
            Location = loc.ToInvariantString();
            if (reader.MoveToAttribute("BidOnItem"))
            {
                bool.TryParse(reader["BidOnItem"], out boolVal);
                BidOnItem = boolVal;
            }
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ItemID", ItemID);
            writer.WriteAttributeString("MaxBuyout", MaxBuyout.ToString());
            writer.WriteAttributeString("Amount", Amount.ToString());
            writer.WriteAttributeString("ItemListType", ItemListType.ToString());
            writer.WriteAttributeString("AutoFindAh", AutoFindAh.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
            writer.WriteAttributeString("BidOnItem", BidOnItem.ToString());
            writer.WriteAttributeString("BuyAdditively", BuyAdditively.ToString());
        }
        #endregion
    }
    #endregion
}

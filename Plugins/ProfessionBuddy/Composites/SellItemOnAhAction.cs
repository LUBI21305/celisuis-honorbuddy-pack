using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using Styx;
using TreeSharp;
using Styx.Helpers;
using Styx.WoWInternals;
using System.Diagnostics;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;
using System.Xml;
using System.Drawing.Design;

using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz.Composites
{
    
    #region SellItemOnAhAction
    class SellItemOnAhAction : PBAction
    {
        public enum RunTimeType { _12_Hours = 1, _24_Hours, _48_Hours, }
        public enum AmountBasedType { Amount, Everything }
        public bool UseCategory
        {
            get { return (bool)Properties["UseCategory"].Value; }
            set { Properties["UseCategory"].Value = value; }
        }
        public WoWItemClass Category
        {
            get { return (WoWItemClass)Properties["Category"].Value; }
            set { Properties["Category"].Value = value; }
        }
        public object SubCategory
        {
            get { return (object)Properties["SubCategory"].Value; }
            set { Properties["SubCategory"].Value = value; }
        }
        public RunTimeType RunTime
        {
            get { return (RunTimeType)Properties["RunTime"].Value; }
            set { Properties["RunTime"].Value = value; }
        }
        public AmountBasedType AmountType
        {
            get { return (AmountBasedType)Properties["AmountType"].Value; }
            set { Properties["AmountType"].Value = value; }
        }
        public string ItemID
        {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }
        public PropertyBag.GoldEditor MinBuyout
        {
            get { return (PropertyBag.GoldEditor)Properties["MinBuyout"].Value; }
            set { Properties["MinBuyout"].Value = value; }
        }
        public PropertyBag.GoldEditor MaxBuyout
        {
            get { return (PropertyBag.GoldEditor)Properties["MaxBuyout"].Value; }
            set { Properties["MaxBuyout"].Value = value; }
        }
        public uint StackSize
        {
            get { return (uint)Properties["StackSize"].Value; }
            set { Properties["StackSize"].Value = value; }
        }

        public uint IgnoreStackSizeBelow
        {
            get { return (uint)Properties["IgnoreStackSizeBelow"].Value; }
            set { Properties["IgnoreStackSizeBelow"].Value = value; }
        }

        public uint Amount
        {
            get { return (uint)Properties["Amount"].Value; }
            set { Properties["Amount"].Value = value; }
        }
        public float BidPrecent
        {
            get { return (float)Properties["BidPrecent"].Value; }
            set { Properties["BidPrecent"].Value = value; }
        }
        public float UndercutPrecent
        {
            get { return (float)Properties["UndercutPrecent"].Value; }
            set { Properties["UndercutPrecent"].Value = value; }
        }
        public bool AutoFindAh
        {
            get { return (bool)Properties["AutoFindAh"].Value; }
            set { Properties["AutoFindAh"].Value = value; }
        }
        public bool PostIfBelowMinBuyout
        {
            get { return (bool)Properties["PostIfBelowMinBuyout"].Value; }
            set { Properties["PostIfBelowMinBuyout"].Value = value; }
        }
        WoWPoint loc;
        public string Location
        {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }

        public SellItemOnAhAction()
        {
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string), new DisplayNameAttribute("Item ID List"));
            Properties["RunTime"] = new MetaProp("RunTime", typeof(RunTimeType), new DisplayNameAttribute("Auction Duration"));
            Properties["MinBuyout"] = new MetaProp("MinBuyout", typeof(PropertyBag.GoldEditor),
                new DisplayNameAttribute("Min Buyout"), new TypeConverterAttribute(typeof(PropertyBag.GoldEditorConverter)));
            Properties["MaxBuyout"] = new MetaProp("MaxBuyout", typeof(PropertyBag.GoldEditor),
                new DisplayNameAttribute("Max Buyout"), new TypeConverterAttribute(typeof(PropertyBag.GoldEditorConverter)));
            Properties["Amount"] = new MetaProp("Amount", typeof(uint));
            Properties["StackSize"] = new MetaProp("StackSize", typeof(uint));
            Properties["IgnoreStackSizeBelow"] = new MetaProp("IgnoreStackSizeBelow", typeof(uint), new DisplayNameAttribute("Ignore StackSize Below"));
            Properties["AmountType"] = new MetaProp("AmountType", typeof(AmountBasedType), new DisplayNameAttribute("Sell"));
            Properties["AutoFindAh"] = new MetaProp("AutoFindAh", typeof(bool), new DisplayNameAttribute("Auto find AH"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));
            Properties["BidPrecent"] = new MetaProp("BidPrecent", typeof(float));
            Properties["UndercutPrecent"] = new MetaProp("UndercutPrecent", typeof(float));
            Properties["UseCategory"] = new MetaProp("UseCategory", typeof(bool), new DisplayNameAttribute("Use Category"));
            Properties["Category"] = new MetaProp("Category", typeof(WoWItemClass), new DisplayNameAttribute("Item Category"));
            Properties["SubCategory"] = new MetaProp("SubCategory", typeof(WoWItemTradeGoodsClass), new DisplayNameAttribute("Item SubCategory"));
            Properties["PostIfBelowMinBuyout"] = new MetaProp("PostIfBelowMinBuyout", typeof(bool), new DisplayNameAttribute("Post if Below MinBuyout"));

            ItemID = "";
            MinBuyout = new PropertyBag.GoldEditor("0g10s0c");
            MaxBuyout = new PropertyBag.GoldEditor("100g0s0c");
            RunTime = RunTimeType._24_Hours;
            Amount = 10u;
            StackSize = 20u;
            IgnoreStackSizeBelow = 1u;
            AmountType = AmountBasedType.Everything;
            AutoFindAh = true;
            BidPrecent = 95f;
            UndercutPrecent = 0.1f;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            UseCategory = true;
            Category = WoWItemClass.TradeGoods;
            SubCategory = WoWItemTradeGoodsClass.None;
            PostIfBelowMinBuyout = true;

            Properties["AutoFindAh"].PropertyChanged += new EventHandler(AutoFindAHChanged);
            Properties["AmountType"].PropertyChanged += new EventHandler(SellItemToAhAction_PropertyChanged);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            Properties["UseCategory"].PropertyChanged += UseCategoryChanged;
            Properties["Category"].PropertyChanged += CategoryChanged;

            Properties["ItemID"].Show = false;
            Properties["Amount"].Show = false;
            Properties["Location"].Show = false;
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

        void AutoFindAHChanged(object sender, EventArgs e)
        {
            if (AutoFindAh)
                Properties["Location"].Show = false;
            else
                Properties["Location"].Show = true;
            RefreshPropertyGrid();
        }

        void SellItemToAhAction_PropertyChanged(object sender, EventArgs e)
        {
            if (AmountType == AmountBasedType.Everything)
                Properties["Amount"].Show = false;
            else
                Properties["Amount"].Show = true;
            RefreshPropertyGrid();
        }

        void UseCategoryChanged(object sender, EventArgs e)
        {
            if (UseCategory)
            {
                Properties["ItemID"].Show = false;
                Properties["Category"].Show = true;
                Properties["SubCategory"].Show = true;
            }
            else
            {
                Properties["ItemID"].Show = true;
                Properties["Category"].Show = false;
                Properties["SubCategory"].Show = false;
            }
            RefreshPropertyGrid();
        }

        void CategoryChanged(object sender, EventArgs e)
        {
            object subCategory = Callbacks.GetSubCategory(Category);
            Properties["SubCategory"] = new MetaProp("SubCategory", subCategory.GetType(),
                new DisplayNameAttribute("Item SubCategory"));
            SubCategory = subCategory;
            RefreshPropertyGrid();
        }
        #endregion

        List<AuctionEntry> ToScanItemList;
        List<AuctionEntry> ToSellItemList;
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                if (Lua.GetReturnVal<int>("if AuctionFrame and AuctionFrame:IsVisible() then return 1 else return 0 end ", 0) == 0)
                {
                    MoveToAh();
                }
                else
                {
                    if (ToScanItemList == null)
                    {
                        ToScanItemList = BuildScanItemList();
                        ToSellItemList = new List<AuctionEntry>();
                    }
                    if (ToScanItemList.Count == 0 && ToSellItemList.Count == 0)
                    {
                        ToScanItemList = null;
                        IsDone = true;
                        return RunStatus.Failure;
                    }
                    if (ToScanItemList.Count > 0)
                    {
                        AuctionEntry ae = ToScanItemList[0];
                        bool scanDone = ScanAh(ref ae);
                        ToScanItemList[0] = ae; // update
                        if (scanDone)
                        {
                            uint lowestBo = ae.LowestBo;
                            if (lowestBo > MaxBuyout.TotalCopper)
                                ae.Buyout = MaxBuyout.TotalCopper;
                            else if (lowestBo < MinBuyout.TotalCopper)
                                ae.Buyout = MinBuyout.TotalCopper;
                            else
                                ae.Buyout = lowestBo - (uint)Math.Ceiling(((double)(lowestBo * UndercutPrecent) / 100d));
                            ae.Bid = (uint)((double)(ae.Buyout * BidPrecent) / 100d);
                            bool enoughItemsPosted = AmountType == AmountBasedType.Amount && ae.myAuctions >= Amount;
                            bool tooLowBuyout = !PostIfBelowMinBuyout && lowestBo < MinBuyout.TotalCopper;

                            Professionbuddy.Debug("PB: PostIfBelowMinBuyout:{0} ", PostIfBelowMinBuyout, MinBuyout.TotalCopper);
                            Professionbuddy.Debug("PB: lowestBo:{0}  MinBuyout.TotalCopper: {1}", lowestBo, MinBuyout.TotalCopper);
                            Professionbuddy.Debug("PB: tooLowBuyout:{0} enoughItemsPosted: {1}", enoughItemsPosted, enoughItemsPosted);

                            if (!enoughItemsPosted && !tooLowBuyout)
                            {
                                ToSellItemList.Add(ae);
                            }
                            else
                                Professionbuddy.Log("Skipping {0} since {1}",
                                    ae.Name, tooLowBuyout ? string.Format("lowest buyout:{0} is below MinBuyout:{1}",
                                    AuctionEntry.GoldString(lowestBo), MinBuyout) :
                                    string.Format("{0} items from me are already posted. Max amount is {1}",
                                    ae.myAuctions, Amount));
                            ToScanItemList.RemoveAt(0);
                        }
                        if (ToScanItemList.Count == 0)
                            Professionbuddy.Debug("Finished scanning for items");
                    }
                    if (ToSellItemList.Count > 0)
                    {
                        if (SellOnAh(ToSellItemList[0]))
                        {
                            Professionbuddy.Log("Selling {0}", ToSellItemList[0]);
                            ToSellItemList.RemoveAt(0);
                        }
                    }
                }
                return RunStatus.Running;
            }
            return RunStatus.Failure;
        }

        #region Auction House

        Stopwatch queueTimer = new Stopwatch();
        int totalAuctions = 0;
        int page = 0;
        bool ScanAh(ref AuctionEntry ae)
        {
            bool scanned = false;
            if (!queueTimer.IsRunning)
            {
                string lua = string.Format("QueryAuctionItems(\"{0}\" ,nil,nil,nil,nil,nil,{1}) return 1",
                    ae.Name.ToFormatedUTF8(), page);
                Lua.GetReturnVal<int>(lua, 0);
                Professionbuddy.Debug("Searching AH for {0}", ae.Name);
                queueTimer.Start();
            }
            else if (queueTimer.ElapsedMilliseconds <= 10000)
            {
                using (new FrameLock())
                {
                    if (Lua.GetReturnVal<int>("if CanSendAuctionQuery('list') == 1 then return 1 else return 0 end ", 0) == 1)
                    {
                        queueTimer.Stop();
                        queueTimer.Reset();
                        totalAuctions = Lua.GetReturnVal<int>("return GetNumAuctionItems('list')", 1);
                        string lua = string.Format("local A,totalA= GetNumAuctionItems('list') local me = GetUnitName('player') local auctionInfo = {{{0},{1}}} for index=1, A do local name, _, count,_,_,_,_,minBid,_, buyoutPrice,_,_,owner,_ = GetAuctionItemInfo('list', index) if name == \"{2}\" and owner ~= me and count >= {3} and buyoutPrice > 0 and buyoutPrice/count <  auctionInfo[1] then auctionInfo[1] = floor(buyoutPrice/count) end if owner == me then auctionInfo[2] = auctionInfo[2] + 1 end end return unpack(auctionInfo) ",
                            ae.LowestBo, ae.myAuctions, ae.Name.ToFormatedUTF8(),IgnoreStackSizeBelow);
                        //Logging.Write("****Copy Below this line****");
                        //Logging.Write(lua);
                        //Logging.Write("****End of copy/paste****");
                        List<string> retVals = Lua.GetReturnValues(lua);
                        uint.TryParse(retVals[0], out ae.LowestBo);
                        uint.TryParse(retVals[1], out ae.myAuctions);
                        if (++page >= (int)Math.Ceiling((double)totalAuctions / 50))
                            scanned = true;
                    }
                }
            }
            else
            {
                scanned = true;
            }
            // reset to default values in preparations for next scan
            if (scanned)
            {
                Professionbuddy.Debug("lowest buyout {0}", ae.LowestBo);
                queueTimer.Stop();
                queueTimer.Reset();
                totalAuctions = 0;
                page = 0;
            }
            return scanned;
        }

        bool _posted = false;
        uint _leftOver = 0;
        bool SellOnAh(AuctionEntry ae)
        {
            if (!_posted)
            {
                uint subAmount = AmountType == AmountBasedType.Amount ? Amount - ae.myAuctions : Amount;
                uint amount = AmountType == AmountBasedType.Everything ?
                    (_leftOver == 0 ? int.MaxValue : _leftOver) :
                    (_leftOver == 0 ? subAmount : _leftOver);
                string lua = string.Format(
                    "local itemID = {0} " +
                    "local amount = {1} " +
                    "local bid = {3} " +
                    "local bo = {4} " +
                    "local runtime = {5} " +
                    "local stack = {2} " +
                    "local sold = 0 " +
                    "local leftovers = 0 " +
                    "local numItems = GetItemCount(itemID) " +
                    "if numItems == 0 then return -1 end " +
                    "if AuctionProgressFrame:IsVisible() == nil then " +
                        "AuctionFrameTab3:Click() " +
                        "local _,_,_,_,_,_,_,maxStack= GetItemInfo(itemID) " +
                        "if maxStack < stack then stack = maxStack end " +
                        "if amount * stack > numItems then " +
                          "amount = floor(numItems/stack) " +
                          "if amount <= 0 then " +
                             "amount = 1 " +
                             "stack = numItems " +
                          "else " +
                             "leftovers = numItems-(amount*stack) " +
                          "end " +
                       "end " +
                       "for bag = 0,4 do " +
                          "for slot=GetContainerNumSlots(bag),1,-1 do " +
                             "local id = GetContainerItemID(bag,slot) " +
                             "local _,c,l = GetContainerItemInfo(bag, slot) " +
                             "if id == itemID and l == nil then " +
                                "PickupContainerItem(bag, slot) " +
                                "ClickAuctionSellItemButton() " +
                                "StartAuction(bid*stack, bo*stack, runtime,stack,amount) " +
                                "return leftovers " +
                             "end " +
                          "end " +
                       "end " +
                    "else " +
                       "return -1 " +
                    "end", ae.Id, amount, StackSize, ae.Bid, ae.Buyout, (int)RunTime);
                int ret = Lua.GetReturnVal<int>(lua, 0);
                if (ret != -1) // returns -1 if waiting for auction to finish posting..
                    _leftOver = (uint)ret;
                if (_leftOver == 0)
                    _posted = true;
            }
            //wait for auctions to finish listing before moving on
            if (_posted)
            {
                bool ret = Lua.GetReturnVal<int>("if AuctionProgressFrame:IsVisible() == nil then return 1 else return 0 end ", 0) == 1;
                if (ret) // we're done listing this item so reset to default values
                {
                    _posted = false;
                    _leftOver = 0;
                }
                return ret;
            }
            else
                return false;
        }

        #endregion

        void MoveToAh()
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

        List<AuctionEntry> BuildScanItemList()
        {
            var tmpItemlist = new List<AuctionEntry>();
            List<WoWItem> itemList;
            if (UseCategory)
            {
                itemList = ObjectManager.Me.BagItems.
                    Where(i => !i.IsSoulbound && !i.IsConjured && !i.IsDisabled &&
                        !Pb.ProtectedItems.Contains(i.Entry) &&
                        i.ItemInfo.ItemClass == Category && subCategoryCheck(i)).ToList();
                foreach (var item in itemList)
                {
                    if (!containsItem(item, tmpItemlist))
                        tmpItemlist.Add(new AuctionEntry(item.Name, item.Entry, 0, 0));
                }
            }
            else
            {
                string[] entries = ItemID.Split(',');
                if (entries != null && entries.Length > 0)
                {
                    foreach (var entry in entries)
                    {
                        uint id = 0;
                        uint.TryParse(entry.Trim(), out id);
                        itemList = ObjectManager.Me.BagItems.Where(i => !i.IsSoulbound && !i.IsConjured && i.Entry == id).ToList();
                        if (itemList != null && itemList.Count > 0)
                        {
                            tmpItemlist.Add(new AuctionEntry(itemList[0].Name, itemList[0].Entry, 0, 0));
                        }
                    }
                }
                else
                {
                    Professionbuddy.Err("No ItemIDs are specified");
                    IsDone = true;
                }
            }
            return tmpItemlist;
        }

        Func<WoWItem, IEnumerable<AuctionEntry>, bool> containsItem = (i, en) => { return en.Count(ae => ae.Id == i.Entry) > 0; };

        bool subCategoryCheck(WoWItem item)
        {
            int sub = (int)SubCategory;
            if (sub == -1 || sub == 0)
                return true;
            object val = item.ItemInfo.GetType().GetProperties().FirstOrDefault(t => t.PropertyType == SubCategory.GetType()).GetValue(item.ItemInfo, null);
            if (val != null && (int)val == sub)
                return true;
            else
                return false;
        }

        public override void Reset()
        {
            base.Reset();
            ToScanItemList = null;
            ToSellItemList = null;
        }
        public override string Name
        {
            get { return "Sell Item To AH"; }
        }
        public override string Title
        {
            get
            {
                return string.Format("{0}: {1}{2}", Name, UseCategory ?
                    string.Format("{0} {1}", Category,
                    (SubCategory != null && (int)SubCategory != -1 && (int)SubCategory != 0) ?
                    "(" + SubCategory + ")" : "") : ItemID,
                    AmountType == AmountBasedType.Amount ? " x" + Amount.ToString() : "");
            }
        }
        public override string Help
        {
            get
            {
                return "This action will sell a specific item that matches ID or all items that belong to a category and optionally sub catagory to the Auction house. If Cheapest item on AH is below minimum buyout then the item is listed at min buyout, else if the cheapest item is higher then the max buyout then the item is listed at max buyout, otherwise it's listed at lowest buyout minus undercut precent.ItemID takes a comma separated list of item IDs. All stacks of items below the IgnoreStackSizeBelow are ignored when undercutting auctions";
            }
        }
        public override object Clone()
        {
            return new SellItemOnAhAction()
            {
                ItemID = this.ItemID,
                MinBuyout = new PropertyBag.GoldEditor(this.MinBuyout.ToString()),
                MaxBuyout = new PropertyBag.GoldEditor(this.MaxBuyout.ToString()),
                Amount = this.Amount,
                StackSize = this.StackSize,
                IgnoreStackSizeBelow = this.IgnoreStackSizeBelow,
                AmountType = this.AmountType,
                AutoFindAh = this.AutoFindAh,
                Location = this.Location,
                UndercutPrecent = this.UndercutPrecent,
                BidPrecent = this.BidPrecent,
                RunTime = this.RunTime,
                UseCategory = this.UseCategory,
                Category = this.Category,
                SubCategory = this.SubCategory,
                PostIfBelowMinBuyout = this.PostIfBelowMinBuyout,
            };
        }

        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            uint val;
            if (reader.MoveToAttribute("ItemID"))
                ItemID = reader["ItemID"];
            else if (reader.MoveToAttribute("ItemName"))
                ItemID = reader["ItemName"];
            MinBuyout = new PropertyBag.GoldEditor(reader["MinBuyout"]);
            MaxBuyout = new PropertyBag.GoldEditor(reader["MaxBuyout"]);
            RunTime = (RunTimeType)Enum.Parse(typeof(RunTimeType), reader["RunTime"]);
            uint.TryParse(reader["Amount"], out val);
            Amount = val;
            uint.TryParse(reader["StackSize"], out val);
            StackSize = val;
            if (reader.MoveToAttribute("IgnoreStackSizeBelow"))
            {
                uint.TryParse(reader["IgnoreStackSizeBelow"], out val);
                IgnoreStackSizeBelow = val;
            }

            AmountType = (AmountBasedType)Enum.Parse(typeof(AmountBasedType), reader["AmountType"]);
            bool boolVal;
            bool.TryParse(reader["AutoFindAh"], out boolVal);
            AutoFindAh = boolVal;
            BidPrecent = reader["BidPrecent"].ToSingle();
            UndercutPrecent = reader["UndercutPrecent"].ToSingle();
            if (reader.MoveToAttribute("UseCategory"))
            {
                bool.TryParse(reader["UseCategory"], out boolVal);
                UseCategory = boolVal;
            }
            if (reader.MoveToAttribute("Category"))
            {
                Category = (WoWItemClass)Enum.Parse(typeof(WoWItemClass), reader["Category"]);
            }
            string subCatType = "";
            if (reader.MoveToAttribute("SubCategoryType"))
            {
                subCatType = reader["SubCategoryType"];
            }
            if (reader.MoveToAttribute("SubCategory") && !string.IsNullOrEmpty(subCatType))
            {
                Type t;
                if (subCatType != "SubCategoryType")
                {
                    string typeName = string.Format("Styx.{0}", subCatType);
                    t = Assembly.GetEntryAssembly().GetType(typeName);
                }
                else
                    t = typeof(SubCategoryType);
                object subVal = Activator.CreateInstance(t);
                subVal = Enum.Parse(t, reader["SubCategory"]);
                SubCategory = subVal;
            }

            float x, y, z;
            x = reader["X"].ToSingle();
            y = reader["Y"].ToSingle();
            z = reader["Z"].ToSingle();
            loc = new WoWPoint(x, y, z);
            Location = loc.ToInvariantString();
            if (reader.MoveToAttribute("PostIfBelowMinBuyout"))
            {
                bool.TryParse(reader["PostIfBelowMinBuyout"], out boolVal);
                PostIfBelowMinBuyout = boolVal;
            }
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ItemID", ItemID);
            writer.WriteAttributeString("MinBuyout", MinBuyout.ToString());
            writer.WriteAttributeString("MaxBuyout", MaxBuyout.ToString());
            writer.WriteAttributeString("RunTime", RunTime.ToString());
            writer.WriteAttributeString("Amount", Amount.ToString());
            writer.WriteAttributeString("StackSize", StackSize.ToString());
            writer.WriteAttributeString("IgnoreStackSizeBelow", IgnoreStackSizeBelow.ToString());
            writer.WriteAttributeString("AmountType", AmountType.ToString());
            writer.WriteAttributeString("AutoFindAh", AutoFindAh.ToString());
            writer.WriteAttributeString("BidPrecent", BidPrecent.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("UndercutPrecent", UndercutPrecent.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("UseCategory", UseCategory.ToString());
            writer.WriteAttributeString("Category", Category.ToString());
            writer.WriteAttributeString("SubCategoryType", SubCategory.GetType().Name);
            writer.WriteAttributeString("SubCategory", SubCategory.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
            writer.WriteAttributeString("PostIfBelowMinBuyout", PostIfBelowMinBuyout.ToString());
        }
        #endregion
    }
    #endregion
}

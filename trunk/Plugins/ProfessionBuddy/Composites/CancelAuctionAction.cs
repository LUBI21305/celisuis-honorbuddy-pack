
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx.Logic.Pathing;
using Styx;
using System.ComponentModel;
using System.Drawing.Design;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Helpers;
using TreeSharp;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using Styx.WoWInternals.WoWCache;

namespace HighVoltz.Composites
{
    public class CancelAuctionAction : PBAction
    {
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

        public string ItemID
        {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }
        public bool AutoFindAh
        {
            get { return (bool)Properties["AutoFindAh"].Value; }
            set { Properties["AutoFindAh"].Value = value; }
        }
        public PropertyBag.GoldEditor MinBuyout
        {
            get { return (PropertyBag.GoldEditor)Properties["MinBuyout"].Value; }
            set { Properties["MinBuyout"].Value = value; }
        }
        WoWPoint loc;
        public string Location
        {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }

        public CancelAuctionAction()
        {
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string), new DisplayNameAttribute("Item ID List"));
            Properties["AutoFindAh"] = new MetaProp("AutoFindAh", typeof(bool), new DisplayNameAttribute("Auto find AH"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));
            Properties["UseCategory"] = new MetaProp("UseCategory", typeof(bool), new DisplayNameAttribute("Use Category"));
            Properties["Category"] = new MetaProp("Category", typeof(WoWItemClass), new DisplayNameAttribute("Item Category"));
            Properties["SubCategory"] = new MetaProp("SubCategory", typeof(WoWItemTradeGoodsClass), new DisplayNameAttribute("Item SubCategory"));
            Properties["MinBuyout"] = new MetaProp("MinBuyout", typeof(PropertyBag.GoldEditor),
                new DisplayNameAttribute("Min Buyout"), new TypeConverterAttribute(typeof(PropertyBag.GoldEditorConverter)));

            ItemID = "0";
            AutoFindAh = true;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            UseCategory = false;
            Category = WoWItemClass.TradeGoods;
            SubCategory = WoWItemTradeGoodsClass.None;
            MinBuyout = new PropertyBag.GoldEditor("0g0s0c");

            Properties["AutoFindAh"].PropertyChanged += new EventHandler(AutoFindAHChanged);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            Properties["UseCategory"].PropertyChanged += UseCategoryChanged;
            Properties["Category"].PropertyChanged += CategoryChanged;

            Properties["Category"].Show = false;
            Properties["SubCategory"].Show = false;
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
        List<AuctionEntry> ToCancelItemList;
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                if (Lua.GetReturnVal<int>("if AuctionFrame and AuctionFrame:IsVisible() == 1 then return 1 else return 0 end ", 0) == 0)
                {
                    MoveToAh();
                }
                else if (Lua.GetReturnVal<int>("if CanSendAuctionQuery('owner') == 1 then return 1 else return 0 end ", 0) == 1)
                {
                    if (ToScanItemList == null)
                    {
                        ToScanItemList = BuildScanItemList();
                        ToCancelItemList = new List<AuctionEntry>();
                    }

                    if (ToScanItemList.Count > 0)
                    {
                        AuctionEntry ae = ToScanItemList[0];
                        bool scanDone = ScanAh(ref ae);
                        ToScanItemList[0] = ae; // update
                        if (scanDone)
                        {
                            ToCancelItemList.Add(ae);
                            ToScanItemList.RemoveAt(0);
                        }
                        if (ToScanItemList.Count == 0)
                            Professionbuddy.Debug("Finished scanning for items");
                    }
                    else
                    {
                        if (ToCancelItemList.Count == 0)
                        {
                            ToScanItemList = null;
                            IsDone = true;
                            return RunStatus.Failure;
                        }
                        else if (CancelAuction(ToCancelItemList[0]))
                        {
                            ToCancelItemList.RemoveAt(0);
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
                        queueTimer.Reset();
                        totalAuctions = Lua.GetReturnVal<int>("return GetNumAuctionItems('list')", 1);
                        string lua = string.Format("local A,totalA= GetNumAuctionItems('list') local me = GetUnitName('player') local auctionInfo = {{{0},{1}}} for index=1, A do local name, _, count,_,_,_,_,minBid,_, buyoutPrice,_,_,owner,_ = GetAuctionItemInfo('list', index) if name == \"{2}\" and owner ~= me and buyoutPrice > 0 and buyoutPrice/count <  auctionInfo[1] then auctionInfo[1] = floor(buyoutPrice/count) end if owner == me then auctionInfo[2] = auctionInfo[2] + 1 end end return unpack(auctionInfo) ",
                            ae.LowestBo, ae.myAuctions, ae.Name.ToFormatedUTF8());

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
                queueTimer.Reset();
                totalAuctions = 0;
                page = 0;
            }
            return scanned;
        }

        bool CancelAuction(AuctionEntry ae)
        {
            int numCanceled = Lua.GetReturnVal<int>(String.Format("local A =GetNumAuctionItems('owner') local cnt=0 for i=A,1,-1 do local name,_,cnt,_,_,_,_,_,_,buyout,_,_,_,sold,id=GetAuctionItemInfo('owner', i) if id == {0} and sold ~= 1 and {2} > {1} and (buyout/cnt) > {2} then CancelAuction(i) cnt=cnt+1 end end return cnt",
                ae.Id,MinBuyout.TotalCopper,ae.LowestBo),0);
            if (numCanceled > 0)
            {
                Professionbuddy.Log("Canceled {0} x{1}", ae.Name, numCanceled);
            }
            return true;
        }

        Dictionary<uint, string> GetMyAuctions()
        {
            string rawString = Lua.GetReturnVal<string>("local A =GetNumAuctionItems('owner') local myAucs = {} for i=1,A do local name,_,_,_,_,_,_,_,_,_,_,_,_,sold,id=GetAuctionItemInfo('owner', i) if sold ~= 1 then myAucs[id]=name end end local ret ='' for k,v in pairs(myAucs) do  ret = ret..k..','..v..'|' end return ret",
                0);
            Dictionary<uint, string> ret = new Dictionary<uint, string>();
            if (rawString != null)
            {
                string[] myAucs = rawString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string rawEntry in myAucs)
                {
                    uint itemId;
                    string[] entry = rawEntry.Split(',');
                    uint.TryParse(entry[0], out itemId);
                    ret.Add(itemId, entry[1]);
                }
            }
            return ret;
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
                Professionbuddy.Err("Unable to location Auctioneer, Maybe he's dead?");
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
            Dictionary<uint, string> myAucs = GetMyAuctions();

            if (UseCategory)
            {
                using (new FrameLock())
                {
                    foreach (var aucKV in myAucs)
                    {
                        ItemInfo info = ItemInfo.FromId(aucKV.Key);
                        if (info != null)
                        {
                            if (info.ItemClass == Category && subCategoryCheck(info.SubClassId))
                            {
                                tmpItemlist.Add(new AuctionEntry(aucKV.Value, aucKV.Key, 0, 0));
                            }
                        }
                        else
                            Professionbuddy.Err("item cache of {0} is null", aucKV.Value);
                    }
                }
            }
            else
            {
                if (ItemID == "0" || ItemID == "")
                {
                    foreach (var kv in myAucs)
                        tmpItemlist.Add(new AuctionEntry(kv.Value, kv.Key, 0, 0));
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
                            if (myAucs.ContainsKey(id))
                                tmpItemlist.Add(new AuctionEntry(myAucs[id], id, 0, 0));
                        }
                    }
                }

            }
            return tmpItemlist;
        }

        Func<WoWItem, IEnumerable<AuctionEntry>, bool> containsItem = (i, en) => { return en.Count(ae => ae.Id == i.Entry) > 0; };

        bool subCategoryCheck(object subCat)
        {
            int sub = (int)SubCategory;
            if (sub == -1 || sub == 0 || (int)subCat == sub)
                return true;
            else
                return false;
        }

        public override void Reset()
        {
            base.Reset();
            ToScanItemList = null;
            ToCancelItemList = null;
        }
        public override string Name
        {
            get { return "Cancel Auction"; }
        }
        public override string Title
        {
            get
            {
                return string.Format("{0}: {1}", Name, UseCategory ?
                    string.Format("{0} {1}", Category,
                    (SubCategory != null && (int)SubCategory != -1 && (int)SubCategory != 0) ?
                    "(" + SubCategory + ")" : "") : ItemID);
            }
        }
        public override string Help
        {
            get
            {
                return "This action will cancel a specific auction that matches ID or all auctions that belong to a category and optionally sub category if they have been undercut. ItemID takes a comma separated list of item IDs. Set 'Use Category' to false and leave ItemId blank to cancel all undercut auctions. If the competition is at or below 'MinBuyout' than that auction is skipped. ";
            }
        }
        public override object Clone()
        {
            return new CancelAuctionAction()
            {
                ItemID = this.ItemID,
                AutoFindAh = this.AutoFindAh,
                Location = this.Location,
                UseCategory = this.UseCategory,
                Category = this.Category,
                SubCategory = this.SubCategory,
            };
        }

        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            ItemID = reader["ItemID"];
            MinBuyout = new PropertyBag.GoldEditor(reader["MinBuyout"]);
            bool boolVal;
            bool.TryParse(reader["AutoFindAh"], out boolVal);
            AutoFindAh = boolVal;
            bool.TryParse(reader["UseCategory"], out boolVal);
            UseCategory = boolVal;
            Category = (WoWItemClass)Enum.Parse(typeof(WoWItemClass), reader["Category"]);
            string subCatType = "";
            subCatType = reader["SubCategoryType"];
            if (!string.IsNullOrEmpty(subCatType))
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
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ItemID", ItemID);
            writer.WriteAttributeString("AutoFindAh", AutoFindAh.ToString());
            writer.WriteAttributeString("UseCategory", UseCategory.ToString());
            writer.WriteAttributeString("Category", Category.ToString());
            writer.WriteAttributeString("SubCategoryType", SubCategory.GetType().Name);
            writer.WriteAttributeString("SubCategory", SubCategory.ToString());
            writer.WriteAttributeString("MinBuyout", MinBuyout.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
        }
        #endregion
    }
}

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
using System.Reflection;
using ObjectManager = Styx.WoWInternals.ObjectManager;


namespace HighVoltz.Composites
{
    #region PutItemInBankAction
    public class PutItemInBankAction : PBAction
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

        public BankType Bank
        {
            get { return (BankType)Properties["Bank"].Value; }
            set { Properties["Bank"].Value = value; }
        }

        public string ItemID
        {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }
        public uint GuildTab
        {
            get { return (uint)Properties["GuildTab"].Value; }
            set { Properties["GuildTab"].Value = value; }
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
        WoWPoint loc;
        public string Location
        {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }
        public PutItemInBankAction()
        {
            Properties["Amount"] = new MetaProp("Amount", typeof(int));
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string));
            Properties["Bank"] = new MetaProp("Bank", typeof(BankType));
            Properties["AutoFindBank"] = new MetaProp("AutoFindBank", typeof(bool), new DisplayNameAttribute("Auto find Bank"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));
            Properties["NpcEntry"] = new MetaProp("NpcEntry", typeof(uint), new EditorAttribute(typeof(PropertyBag.EntryEditor), typeof(UITypeEditor)));
            Properties["GuildTab"] = new MetaProp("GuildTab", typeof(uint));
            Properties["UseCategory"] = new MetaProp("UseCategory", typeof(bool), new DisplayNameAttribute("Use Category"));
            Properties["Category"] = new MetaProp("Category", typeof(WoWItemClass), new DisplayNameAttribute("Item Category"));
            Properties["SubCategory"] = new MetaProp("SubCategory", typeof(WoWItemTradeGoodsClass), new DisplayNameAttribute("Item SubCategory"));

            Amount = 0;
            ItemID = "";
            Bank = BankType.Personal;
            AutoFindBank = true;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            NpcEntry = 0u;
            GuildTab = 0u;
            UseCategory = true;
            Category = WoWItemClass.TradeGoods;
            SubCategory = WoWItemTradeGoodsClass.None;

            Properties["ItemID"].Show = false;
            Properties["Location"].Show = false;
            Properties["NpcEntry"].Show = false;
            Properties["GuildTab"].Show = false;

            Properties["AutoFindBank"].PropertyChanged += new EventHandler(AutoFindBankChanged);
            Properties["Bank"].PropertyChanged += new EventHandler(PutItemInBankAction_PropertyChanged);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            Properties["UseCategory"].PropertyChanged += UseCategoryChanged;
            Properties["Category"].PropertyChanged += CategoryChanged;
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

        void PutItemInBankAction_PropertyChanged(object sender, EventArgs e)
        {
            if (Bank == BankType.Personal)
            {
                Properties["GuildTab"].Show = false;
            }
            else
            {
                Properties["GuildTab"].Show = true;
            }
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
        Dictionary<uint, int> ItemList = null;
        //bool _switchingTabs = false;
        Stopwatch _gbankItemThrottleSW = new Stopwatch();
        const long _gbankItemThrottle = 167; // 6 times per sec.. round up to nearest 1.
        int _numOfItemsDepositedInGB = 0;
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
                    if (ItemList.Count == 0)
                        IsDone = true;
                    else
                    {
                        KeyValuePair<uint, int> kv = ItemList.FirstOrDefault();
                        bool done = false;
                        if (Bank == BankType.Personal)
                            done = PutItemInBank(kv.Key, kv.Value);
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
                            int ret = PutItemInGBank(kv.Key, kv.Value, GuildTab);
                            if (ret == -1 || _numOfItemsDepositedInGB + ret >= Amount)
                                done = true;
                            else
                            {
                                _numOfItemsDepositedInGB += ret;
                                done = false;
                            }
                        }
                        if (done)
                        {
                            Professionbuddy.Debug("Done Depositing Item:{0} to bank", kv.Key);
                            ItemList.Remove(kv.Key);
                        }
                        _itemsSW.Reset();
                        _itemsSW.Start();
                    }
                }
                if (IsDone)
                {
                    Professionbuddy.Log("Deposited Items:[{0}] to {1} Bank", ItemID, Bank);
                }
                else
                    return RunStatus.Running;
            }
            return RunStatus.Failure;
        }

        void MoveToBanker()
        {
            WoWPoint movetoPoint = loc;
            WoWObject bank = GetLocalBanker();
            if (bank != null)
                movetoPoint = WoWMathHelper.CalculatePointFrom(me.Location, bank.Location, 4);
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
            Dictionary<uint, int> itemList = new Dictionary<uint, int>();
            IEnumerable<WoWItem> tmpItemlist = from item in me.BagItems
                                               where !item.IsConjured && !item.IsSoulbound && !item.IsDisabled
                                               select item;
            if (UseCategory)
                foreach (WoWItem item in tmpItemlist)
                {
                    if (!Pb.ProtectedItems.Contains(item.Entry) && item.ItemInfo.ItemClass == Category &&
                        subCategoryCheck(item) && !itemList.ContainsKey(item.Entry))
                    {
                        itemList.Add(item.Entry, Amount);
                    }
                }
            else
            {
                string[] entries = ItemID.Split(',');
                if (entries != null && entries.Length > 0)
                {
                    foreach (var entry in entries)
                    {
                        uint temp = 0;
                        uint.TryParse(entry.Trim(), out temp);
                        itemList.Add(temp, Amount);
                    }
                }
                else
                {
                    Professionbuddy.Err("No ItemIDs are specified");
                    IsDone = true;
                }
            }
            Professionbuddy.Debug("List of items to deposit to bank");
            foreach (var item in itemList)
            {
                Professionbuddy.Debug("Item:{0} Amount:{1}", item.Key, item.Value);
            }
            Professionbuddy.Debug("End of list");
            return itemList;
        }

        bool subCategoryCheck(WoWItem item)
        {
            int sub = (int)SubCategory;
            if (sub == -1 || sub == 0)
                return true;
            object val = item.ItemInfo.GetType().GetProperties()
                .FirstOrDefault(t => t.PropertyType == SubCategory.GetType()).GetValue(item.ItemInfo, null);
            if (val != null && (int)val == sub)
                return true;
            else
                return false;
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
                else if (AutoFindBank || loc == WoWPoint.Zero)
                    bank = bankers.OrderBy(o => o.Distance).FirstOrDefault();
                else if (ObjectManager.Me.Location.Distance(loc) <= 90)
                {
                    bank = bankers.Where(o => o.Location.Distance(loc) < 10).
                        OrderBy(o => o.Distance).FirstOrDefault();
                }
            }
            return bank;
        }

        bool IsGbankFrameVisible { get { return Lua.GetReturnVal<int>("if GuildBankFrame and GuildBankFrame:IsVisible() then return 1 else return 0 end ", 0) == 1; } }
        Stopwatch queueServerSW;
        Stopwatch _itemsSW;
        int _currentBag = -1;
        int _currentSlot = 1;
        // returns number of items deposited.. -1 if done...
        public int PutItemInGBank(uint id, int amount, uint tab)
        {
            if (queueServerSW == null)
            {
                queueServerSW = new Stopwatch();
                queueServerSW.Start();
                Lua.DoString("for i=GetNumGuildBankTabs(), 1, -1 do QueryGuildBankTab(i) end ");
                Professionbuddy.Log("Queuing server for gbank info");
                return 0;
            }
            else if (queueServerSW.ElapsedMilliseconds < 2000)
                return 0;
            string lua = string.Format(
                "local tabnum = GetNumGuildBankTabs() " +
                "local bagged = 0 " +
                "local tabInfo = {{0}} " +
                "local tab = 0 " +
                "local i = 1 " +
                "local _,_,_,_,_,_,_,maxStack = GetItemInfo({0}) " +
                "while tab <= tabnum do " +
                   "local_,_,v,d =GetGuildBankTabInfo(tab) " +
                   "if v == 1 and d == 1 then " +
                   //"SetCurrentGuildBankTab(tab) " +
                      "for slot=1, 98 do " +
                         "local _,c,l=GetGuildBankItemInfo(tab, slot) " +
                         "if c > 0 and l == nil then " +
                            "local id = tonumber(string.match(GetGuildBankItemLink(tab,slot), 'Hitem:(%d+)')) " +
                            "if id == {0} and c < maxStack then " +
                               "tabInfo[i] = {{tab,slot,maxStack-c}} " +
                               "i = i +1 " +
                            "end " +
                         "elseif c == 0 then " +
                            "tabInfo[i] = {{tab,slot,maxStack}} " +
                            "i = i +1 " +
                         "end " +
                      "end " +
                   "end " +
                   "tab = tab + 1 " +
                "end " +
                "i = 1 " +
                "if GetCurrentGuildBankTab() ~= tabInfo[1][1] then " +
                    "SetCurrentGuildBankTab(tabInfo[1][1]) " +
                    "return 0 " +
                "end " +
                "for bag = 0,4 do " +
                   "for slot=1,GetContainerNumSlots(bag) do " +
                      "if i > #tabInfo then return end " +
                      "local id = GetContainerItemID(bag,slot) " +
                      "local _,c,l = GetContainerItemInfo(bag, slot) " +
                      "if id == {0} and l == nil then  " +
                         "if GetCurrentGuildBankTab() ~= tabInfo[i][1] then " +
                            "return 0 " +
                         "end " +
                         "if c + bagged <= {1} and c <= tabInfo[i][3] then " +
                            "PickupContainerItem(bag,slot) " +
                            "PickupGuildBankItem(tabInfo[i][1] ,tabInfo[i][2]) " +
                            "bagged = bagged + c " +
                         "else " +
                            "local cnt = {1}-bagged " +
                            "if cnt > tabInfo[i][3] then cnt = tabInfo[i][3] end " +
                            "SplitContainerItem(bag,slot, cnt) " +
                            "PickupGuildBankItem(tabInfo[i][1] ,tabInfo[i][2]) " +
                            "bagged = bagged + cnt " +
                         "end " +
                         "return c " +
                      "end " +
                      "i=i+1 " +
                      "if bagged >= {1} then return 1 end " +
                   "end " +
                "end " +
                "return -1"
                , id, amount <= 0 ? int.MaxValue : amount, tab, _currentBag, _currentSlot);
           return Lua.GetReturnVal<int>(lua,0) ;
        }

        public bool PutItemInBank(uint id, int amount)
        {
            string lua = string.Format(
                "local bagged = 0 " +
                "local bagInfo = {{0}} " +
                "local bag = -1 " +
                "local i=1; " +
                "local _,_,_,_,_,_,_,maxStack = GetItemInfo({0}) " +
                "while bag <= 11 do " +
                   "local itemf  = GetItemFamily({0}) " +
                   "local fs,bfamily = GetContainerNumFreeSlots(bag) " +
                   "if fs > 0 and (bfamily == 0 or bit.band(itemf, bfamily) > 0) then " +
                      "for slot=1, GetContainerNumSlots(bag) do " +
                         "local _,c,l = GetContainerItemInfo(bag, slot) " +
                         "local id = GetContainerItemID(bag, slot) or 0 " +
                         "if c == nil then " +
                            "bagInfo[i]={{bag,slot,maxStack}} " +
                            "i=i+1 " +
                         "elseif l == nil and id == {0} and c < maxStack then " +
                            "bagInfo[i]={{bag,slot,maxStack-c}} " +
                            "i=i+1 " +
                         "end " +
                      "end " +
                   "end " +
                   "bag = bag + 1 " +
                   "if bag == 0 then bag = 5 end " +
                "end " +
                "i=1 " +
                "for bag = 0,4 do " +
                   "for slot=1,GetContainerNumSlots(bag) do " +
                      "if i > #bagInfo then return end " +
                      "local id = GetContainerItemID(bag,slot) or 0 " +
                      "local _,c,l = GetContainerItemInfo(bag, slot) " +
                      "local _,_,_,_,_,_,_, maxStack = GetItemInfo(id) " +
                      "if id == {0} and l == nil then " +
                         "if c + bagged <= {1} and c <= bagInfo[i][3] then " +
                            "PickupContainerItem(bag, slot) " +
                            "PickupContainerItem(bagInfo[i][1], bagInfo[i][2]) " +
                            "bagged = bagged + c " +
                         "else " +
                            "local cnt = {1}-bagged " +
                            "if cnt > bagInfo[i][3] then cnt = bagInfo[i][3] end " +
                            "SplitContainerItem(bag,slot, cnt) " +
                            "PickupContainerItem(bagInfo[i][1], bagInfo[i][2]) " +
                            "bagged = bagged + cnt " +
                         "end " +
                         "i=i+1 " +
                      "end " +
                      "if bagged == {1} then return end " +
                   "end " +
                "end return "
                , id, amount <= 0 ? int.MaxValue : amount);
            Lua.DoString(lua);
            return true;
        }
        public override string Name { get { return "Deposit Item in Bank"; } }
        public override string Title
        {
            get
            {
                return string.Format("{0}: {1} {2}", Name,
                    UseCategory ? string.Format("{0} {1}", Category, SubCategory) : ItemID.ToString(),
                    Amount > 0 ? Amount.ToString() : "");
            }
        }
        public override string Help
        {
            get
            {
                return "This action will deposit the specified item/s into your personal or guild bank. Set Amount to 0 if you want to deposit all items that match Entry or Category. Set GuildTab to 0 to deposit in whichever tab has room";
            }
        }
        public override void Reset()
        {
            base.Reset();
            queueServerSW = null;
            ItemList = null;
            _itemsSW = null;
            _currentBag = -1;
            _currentSlot = 1;
            _numOfItemsDepositedInGB = 0;
        }
        public override object Clone()
        {
            return new PutItemInBankAction()
            {
                ItemID = this.ItemID,
                Amount = this.Amount,
                Bank = this.Bank,
                NpcEntry = this.NpcEntry,
                loc = this.loc,
                GuildTab = this.GuildTab,
                AutoFindBank = this.AutoFindBank,
                Parent = this.Parent,
                Location = this.Location,
                UseCategory = this.UseCategory,
                Category = this.Category,
                SubCategory = this.SubCategory,
            };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            uint id;
            uint.TryParse(reader["Amount"], out id);
            Amount = (int)id;
            if (reader.MoveToAttribute("ItemID"))
                ItemID = reader["ItemID"];
            else if (reader.MoveToAttribute("Entry"))
                ItemID = reader["Entry"];
            uint.TryParse(reader["NpcEntry"], out id);
            NpcEntry = id;
            uint.TryParse(reader["GuildTab"], out id);
            GuildTab = id;
            bool boolVal = false;
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

            bool autoFind;
            bool.TryParse(reader["AutoFindBank"], out autoFind);
            AutoFindBank = autoFind;
            Bank = (BankType)Enum.Parse(typeof(BankType), reader["Bank"]);
            float x, y, z;
            x = reader["X"].ToSingle();
            y = reader["Y"].ToSingle();
            z = reader["Z"].ToSingle();
            loc = new WoWPoint(x, y, z);
            Properties["Location"].Value = loc.ToInvariantString();
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Amount", Amount.ToString());
            writer.WriteAttributeString("ItemID", ItemID);
            writer.WriteAttributeString("NpcEntry", NpcEntry.ToString());
            writer.WriteAttributeString("GuildTab", GuildTab.ToString());
            writer.WriteAttributeString("AutoFindBank", AutoFindBank.ToString());
            writer.WriteAttributeString("UseCategory", UseCategory.ToString());
            writer.WriteAttributeString("Category", Category.ToString());
            writer.WriteAttributeString("SubCategoryType", SubCategory.GetType().Name);
            writer.WriteAttributeString("SubCategory", SubCategory.ToString());
            writer.WriteAttributeString("Bank", Bank.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
        }
        #endregion
    }
    #endregion
}

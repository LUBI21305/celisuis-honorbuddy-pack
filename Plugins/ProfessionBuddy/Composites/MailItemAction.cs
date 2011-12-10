using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Styx;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.Helpers;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;
using System.Reflection;

namespace HighVoltz.Composites {
    #region MailItemAction
    class MailItemAction : PBAction {
        public enum SubCategoryType { None }; // use as a placeholder for item categories with no sub categories

        public bool UseCategory {
            get { return (bool)Properties["UseCategory"].Value; }
            set { Properties["UseCategory"].Value = value; }
        }
        public WoWItemClass Category {
            get { return (WoWItemClass)Properties["Category"].Value; }
            set { Properties["Category"].Value = value; }
        }
        public object SubCategory {
            get { return (object)Properties["SubCategory"].Value; }
            set { Properties["SubCategory"].Value = value; }
        }
        public string ItemID {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }
        public bool AutoFindMailBox {
            get { return (bool)Properties["AutoFindMailBox"].Value; }
            set { Properties["AutoFindMailBox"].Value = value; }
        }

        WoWPoint loc;
        public string Location {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }
        public MailItemAction() {
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string));
            Properties["AutoFindMailBox"] = new MetaProp("AutoFindMailBox", typeof(bool), new DisplayNameAttribute("Automatically find Mailbox"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));
            Properties["UseCategory"] = new MetaProp("UseCategory", typeof(bool), new DisplayNameAttribute("Use Category"));
            Properties["Category"] = new MetaProp("Category", typeof(WoWItemClass), new DisplayNameAttribute("Item Category"));
            Properties["SubCategory"] = new MetaProp("SubCategory", typeof(WoWItemTradeGoodsClass), new DisplayNameAttribute("Item SubCategory"));

            ItemID = "";
            AutoFindMailBox = true;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            UseCategory = true;
            Category = WoWItemClass.TradeGoods;
            SubCategory = WoWItemTradeGoodsClass.None;

            Properties["Location"].Show = false;
            Properties["ItemID"].Show = false;
            Properties["AutoFindMailBox"].PropertyChanged += new EventHandler(AutoFindMailBoxChanged);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            Properties["UseCategory"].PropertyChanged += UseCategoryChanged;
            Properties["Category"].PropertyChanged += CategoryChanged;
        }

        #region Callbacks
        void LocationChanged(object sender, EventArgs e) {
            MetaProp mp = (MetaProp)sender;
            loc = Util.StringToWoWPoint((string)((MetaProp)sender).Value);
            Properties["Location"].PropertyChanged -= new EventHandler(LocationChanged);
            Properties["Location"].Value = string.Format("<{0}, {1}, {2}>", loc.X, loc.Y, loc.Z);
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
            RefreshPropertyGrid();
        }
        void UseCategoryChanged(object sender, EventArgs e) {
            if (UseCategory) {
                Properties["ItemID"].Show = false;
                Properties["Category"].Show = true;
                Properties["SubCategory"].Show = true;
            }
            else {
                Properties["ItemID"].Show = true;
                Properties["Category"].Show = false;
                Properties["SubCategory"].Show = false;
            }
            RefreshPropertyGrid();
        }

        void CategoryChanged(object sender, EventArgs e) {
            object subCategory = Callbacks.GetSubCategory(Category);
            Properties["SubCategory"] = new MetaProp("SubCategory", subCategory.GetType(),
                new DisplayNameAttribute("Item SubCategory"));
            SubCategory = subCategory;
            RefreshPropertyGrid();
        }

        void AutoFindMailBoxChanged(object sender, EventArgs e) {
            if (AutoFindMailBox)
                Properties["Location"].Show = false;
            else
                Properties["Location"].Show = true;
            RefreshPropertyGrid();
        }

        #endregion

        WoWGameObject _mailbox;
        protected override RunStatus Run(object context) {
            if (!IsDone) {
                WoWPoint movetoPoint = loc;
                if (MailFrame.Instance == null || !MailFrame.Instance.IsVisible) {
                    if (AutoFindMailBox || movetoPoint == WoWPoint.Zero) {
                        _mailbox = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.SubType == WoWGameObjectType.Mailbox)
                            .OrderBy(o => o.Distance).FirstOrDefault();
                    }
                    else {
                        _mailbox = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.SubType == WoWGameObjectType.Mailbox
                            && o.Location.Distance(loc) < 10)
                            .OrderBy(o => o.Distance).FirstOrDefault();
                    }
                    if (_mailbox != null)
                        movetoPoint = WoWMathHelper.CalculatePointFrom(me.Location, _mailbox.Location, 3);
                    if (movetoPoint == WoWPoint.Zero)
                        return RunStatus.Failure;
                    if (movetoPoint.Distance(ObjectManager.Me.Location) > 4.5)
                        Util.MoveTo(movetoPoint);
                    else if (_mailbox != null) {
                        _mailbox.Interact();
                    }
                    return RunStatus.Running;
                }
                else {
                    List<WoWItem> ItemList = BuildItemList();
                    if (ItemList == null || ItemList.Count == 0) {
                        IsDone = true;
                        return RunStatus.Failure;
                    }
                    using (new FrameLock()) {
                        MailFrame.Instance.SwitchToSendMailTab();
                        foreach (WoWItem item in ItemList) {
                            item.UseContainerItem();
                        }
                        Lua.DoString(string.Format("SendMail ('{0}',' ','');SendMailMailButton:Click();", CharacterSettings.Instance.MailRecipient.ToFormatedUTF8()));
                    }
                    if (IsDone) {
                        Professionbuddy.Log("Done sending {0} via mail",
                            UseCategory ? string.Format("Items that belong to category {0} and subcategory {1}", Category, SubCategory) :
                            string.Format("Items that match Id of {0}", ItemID));
                    }
                    else
                        return RunStatus.Running;
                }
            }
            return RunStatus.Failure;
        }

        List<WoWItem> BuildItemList() {
            IEnumerable<WoWItem> tmpItemlist = from item in me.BagItems
                                               where !item.IsConjured && !item.IsSoulbound && !item.IsDisabled
                                               select item;
            if (UseCategory)
                return tmpItemlist.Where(i => !Pb.ProtectedItems.Contains(i.Entry) &&
                    i.ItemInfo.ItemClass == Category && subCategoryCheck(i)).Take(12).ToList();
            else {
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
                }
                return tmpItemlist.Where(i => idList.Contains(i.Entry)).Take(12).ToList();
            }
        }

        bool subCategoryCheck(WoWItem item) {
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

        public override void Reset() {
            base.Reset();
        }
        public override string Name {
            get {
                return "Mail Item";
            }
        }
        public override string Title {
            get {
                return string.Format("{0}: to:{1} {2} ", Name, CharacterSettings.Instance.MailRecipient,
                    UseCategory ? string.Format("{0} {1}", Category, SubCategory) : ItemID.ToString());
            }
        }
        public override string Help {
            get {
                return "This action will mail either all items that match Item ID or by item category.Setting Count to 0 will mail all items that match Entry. Note: Count = axact number, not stacks. This mails items to the 'Mail Recipient' from Honorbuddy settings. ";
            }
        }
        public override object Clone() {
            return new MailItemAction() {
                ItemID = this.ItemID,
                //Recipient = this.Recipient,
                loc = this.loc,
                AutoFindMailBox = this.AutoFindMailBox,
                Location = this.Location,
                UseCategory = this.UseCategory,
                Category = this.Category,
                SubCategory = this.SubCategory,
            };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader) {
            if (reader.MoveToAttribute("ItemID"))
                ItemID = reader["ItemID"];
            else if (reader.MoveToAttribute("Entry"))
                ItemID = reader["Entry"];
            bool autofind;
            bool.TryParse(reader["AutoFindMailBox"], out autofind);
            AutoFindMailBox = autofind;
            bool boolVal = false;
            if (reader.MoveToAttribute("UseCategory")) {
                bool.TryParse(reader["UseCategory"], out boolVal);
                UseCategory = boolVal;
            }
            if (reader.MoveToAttribute("Category")) {
                Category = (WoWItemClass)Enum.Parse(typeof(WoWItemClass), reader["Category"]);
            }
            string subCatType = "";
            if (reader.MoveToAttribute("SubCategoryType")) {
                subCatType = reader["SubCategoryType"];
            }
            if (reader.MoveToAttribute("SubCategory") && !string.IsNullOrEmpty(subCatType)) {
                Type t;
                if (subCatType != "SubCategoryType") {
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
        public override void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Entry", ItemID.ToString());
            writer.WriteAttributeString("AutoFindMailBox", AutoFindMailBox.ToString());

            writer.WriteAttributeString("UseCategory", UseCategory.ToString());
            writer.WriteAttributeString("Category", Category.ToString());
            writer.WriteAttributeString("SubCategoryType", SubCategory.GetType().Name);
            writer.WriteAttributeString("SubCategory", SubCategory.ToString());

            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
        }
        #endregion
    }
    #endregion

}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Xml;
using Styx;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Helpers;
using TreeSharp;
using ObjectManager = Styx.WoWInternals.ObjectManager;
using System.Collections.Generic;


namespace HighVoltz.Composites
{
    #region GetMailAction
    class GetMailAction : PBAction
    {
        public enum GetMailActionType
        {
            AllItems,
            Specific,
        }
        public GetMailActionType GetMailType
        {
            get { return (GetMailActionType)Properties["GetMailType"].Value; }
            set { Properties["GetMailType"].Value = value; }
        }
        public string ItemID
        {
            get { return (string)Properties["ItemID"].Value; }
            set { Properties["ItemID"].Value = value; }
        }
        public bool CheckNewMail
        {
            get { return (bool)Properties["CheckNewMail"].Value; }
            set { Properties["CheckNewMail"].Value = value; }
        }
        public uint MinFreeBagSlots
        {
            get { return (uint)Properties["MinFreeBagSlots"].Value; }
            set { Properties["MinFreeBagSlots"].Value = value; }
        }
        public bool AutoFindMailBox
        {
            get { return (bool)Properties["AutoFindMailBox"].Value; }
            set { Properties["AutoFindMailBox"].Value = value; }
        }
        WoWPoint loc;
        public string Location
        {
            get { return (string)Properties["Location"].Value; }
            set { Properties["Location"].Value = value; }
        }
        public GetMailAction()
        {
            //CheckNewMail
            Properties["ItemID"] = new MetaProp("ItemID", typeof(string));
            Properties["MinFreeBagSlots"] = new MetaProp("MinFreeBagSlots", typeof(uint), new DisplayNameAttribute("Min Free Bagslots"));
            Properties["CheckNewMail"] = new MetaProp("CheckNewMail", typeof(bool), new DisplayNameAttribute("Check for New Mail"));
            Properties["GetMailType"] = new MetaProp("GetMailType", typeof(GetMailActionType), new DisplayNameAttribute("Get Mail"));
            Properties["AutoFindMailBox"] = new MetaProp("AutoFindMailBox", typeof(bool), new DisplayNameAttribute("Auto find Mailbox"));
            Properties["Location"] = new MetaProp("Location", typeof(string), new EditorAttribute(typeof(PropertyBag.LocationEditor), typeof(UITypeEditor)));

            ItemID = "";
            CheckNewMail = true;
            GetMailType = (GetMailActionType)GetMailActionType.AllItems;
            AutoFindMailBox = true;
            loc = WoWPoint.Zero;
            Location = loc.ToInvariantString();
            MinFreeBagSlots = 2u;

            Properties["GetMailType"].PropertyChanged += new EventHandler(GetMailAction_PropertyChanged);
            Properties["AutoFindMailBox"].PropertyChanged += new EventHandler(AutoFindMailBoxChanged);
            Properties["ItemID"].Show = false;
            Properties["Location"].Show = false;
            Properties["Location"].PropertyChanged += new EventHandler(LocationChanged);
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

        void AutoFindMailBoxChanged(object sender, EventArgs e)
        {
            if (AutoFindMailBox)
                Properties["Location"].Show = false;
            else
                Properties["Location"].Show = true;
            RefreshPropertyGrid();
        }
        void GetMailAction_PropertyChanged(object sender, EventArgs e)
        {
            if (GetMailType == GetMailActionType.AllItems)
                Properties["ItemID"].Show = false;
            else
                Properties["ItemID"].Show = true;
            RefreshPropertyGrid();
        }
        WoWGameObject mailbox;
        Stopwatch WaitForContentToShowSW = new Stopwatch();
        Stopwatch ConcludingSW = new Stopwatch();
        Stopwatch TimeoutSW = new Stopwatch();
        Stopwatch _refreshInboxSW = new Stopwatch();

        List<uint> _idList ;
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                if (!TimeoutSW.IsRunning)
                    TimeoutSW.Start();
                if (TimeoutSW.ElapsedMilliseconds > 300000)
                    IsDone = true;
                WoWPoint movetoPoint = loc;
                if (MailFrame.Instance == null || !MailFrame.Instance.IsVisible)
                {
                    if (AutoFindMailBox || movetoPoint == WoWPoint.Zero)
                    {
                        mailbox = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.SubType == WoWGameObjectType.Mailbox)
                            .OrderBy(o => o.Distance).FirstOrDefault();
                    }
                    else
                    {
                        mailbox = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.SubType == WoWGameObjectType.Mailbox
                            && o.Location.Distance(loc) < 10)
                            .OrderBy(o => o.Distance).FirstOrDefault();
                    }
                    if (mailbox != null)
                        movetoPoint = WoWMathHelper.CalculatePointFrom(me.Location, mailbox.Location, 3);
                    if (movetoPoint == WoWPoint.Zero)
                        return RunStatus.Failure;
                    if (movetoPoint.Distance(ObjectManager.Me.Location) > 4.5)
                        Util.MoveTo(movetoPoint);
                    else if (mailbox != null)
                    {
                        mailbox.Interact();
                    }
                    return RunStatus.Running;
                }
                else
                {
                    if (_idList == null) {
                        _idList = BuildItemList();
                    }
                    if (!_refreshInboxSW.IsRunning)
                        _refreshInboxSW.Start();
                    if (!WaitForContentToShowSW.IsRunning)
                        WaitForContentToShowSW.Start();
                    if (WaitForContentToShowSW.ElapsedMilliseconds < 3000)
                        return RunStatus.Running;
                    uint freeslots =  ObjectManager.Me.FreeNormalBagSlots;

                    if (!ConcludingSW.IsRunning)
                    {
                        if (_refreshInboxSW.ElapsedMilliseconds < 61000)
                        {
                            if (GetMailType == GetMailActionType.AllItems)
                            {
                                string lua = string.Format("local totalItems,numItems = GetInboxNumItems() local foundMail=0 for index=1,numItems do local _,_,sender,subj,gold,cod,_,itemCnt,_,_,hasText=GetInboxHeaderInfo(index) if sender ~= nil and cod == 0 and itemCnt == nil and gold == 0 and hasText == nil then DeleteInboxItem(index) end if cod == 0 and ((itemCnt and itemCnt >0) or (gold and gold > 0)) then AutoLootMailItem(index) foundMail = foundMail + 1 break end end local beans = BeanCounterMail and BeanCounterMail:IsVisible() if foundMail == 0 {0}and totalItems == numItems and beans ~= 1 then return 1 else return 0 end ",
                                    CheckNewMail ? "and HasNewMail() == nil " : "");
                                //freeslots / 2 >= MinFreeBagSlots ? (freeslots - MinFreeBagSlots) / 2 : 1);
                                if (Lua.GetReturnValues(lua)[0] == "1" || ObjectManager.Me.FreeNormalBagSlots <= MinFreeBagSlots)
                                    ConcludingSW.Start();
                            }
                            else
                            {
                                for (int i = 0; i < _idList.Count; i++)
                                {
                                    string lua = string.Format("local totalItems,numItems = GetInboxNumItems() local foundMail=0 for index=1,numItems do local _,_,sender,subj,gold,cod,_,itemCnt,_,_,hasText=GetInboxHeaderInfo(index) if sender ~= nil and cod == 0 and itemCnt == nil and gold == 0 and hasText == nil then DeleteInboxItem(index) end if cod == 0 and itemCnt and itemCnt >0  then for i2=1, ATTACHMENTS_MAX_RECEIVE do local itemlink = GetInboxItemLink(index, i2) if itemlink ~= nil and string.find(itemlink,'{0}') then foundMail = foundMail + 1 TakeInboxItem(index, i2) break end end end end if (foundMail == 0 {1})  or (foundMail == 0 and (numItems == 50 and totalItems >= 50)) then return 1 else return 0 end ",
                                        //, Entry, freeslots / 2 >= MinFreeBagSlots ? (freeslots - MinFreeBagSlots) / 2 : 1);
                                    _idList[i], CheckNewMail ? "and HasNewMail() == nil " : "");

                                    if (Lua.GetReturnValues(lua)[0] == "1" || ObjectManager.Me.FreeNormalBagSlots <= MinFreeBagSlots)
                                        _idList.RemoveAt(i);
                                }
                                if (_idList.Count == 0)
                                    ConcludingSW.Start();
                            }
                        }
                        else
                        {
                            _refreshInboxSW.Reset();
                            MailFrame.Instance.Close();
                        }
                    }
                    if (ConcludingSW.ElapsedMilliseconds > 2000)
                        IsDone = true;
                    if (IsDone)
                    {
                        Professionbuddy.Log("Mail retrieval of items:{0} finished", GetMailType);
                    }
                    else
                        return RunStatus.Running;
                }
            }
            return RunStatus.Failure;
        }

        List<uint> BuildItemList() {
            List<uint> list = new List<uint>();
            string[] entries = ItemID.Split(',');
            if (entries != null && entries.Length > 0) {
                foreach (var entry in entries) {
                    uint temp = 0;
                    uint.TryParse(entry.Trim(), out temp);
                    list.Add(temp);
                }
            }
            else {
                Professionbuddy.Err("No ItemIDs are specified");
                IsDone = true;
            }
            return list;
        }

        public override void Reset()
        {
            base.Reset();
            WaitForContentToShowSW = new Stopwatch();
            ConcludingSW = new Stopwatch();
            TimeoutSW = new Stopwatch();
            _refreshInboxSW = new Stopwatch();
        }
        public override string Name
        {
            get
            {
                return "Get Mail";
            }
        }
        public override string Title
        {
            get
            {
                return string.Format("{0}: {1} " + (GetMailType == GetMailActionType.Specific ? " - " +
                    ItemID.ToString() : ""), Name, GetMailType);
            }
        }
        public override string Help
        {
            get
            {
                return "This action retrieves all mail in the mailbox, or only items that match the ID.Since mailboxes are not in the NPC database you need to be within 100 yards of a mailbox to 'autofind' it";
            }
        }
        public override object Clone()
        {
            return new GetMailAction()
            {
                ItemID = this.ItemID,
                GetMailType = this.GetMailType,
                loc = this.loc,
                AutoFindMailBox = this.AutoFindMailBox,
                Location = this.Location,
                MinFreeBagSlots = this.MinFreeBagSlots,
                CheckNewMail = this.CheckNewMail,
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
            if (reader.MoveToAttribute("CheckNewMail"))
            {
                bool boolVal;
                bool.TryParse(reader["CheckNewMail"], out boolVal);
                CheckNewMail = boolVal;
            }
            GetMailType = (GetMailActionType)Enum.Parse(typeof(GetMailActionType), reader["GetMailType"]);
            bool autofind;
            bool.TryParse(reader["AutoFindMailBox"], out autofind);
            AutoFindMailBox = autofind;
            float x, y, z;
            x = reader["X"].ToSingle();
            x = reader["X"].ToSingle();
            y = reader["Y"].ToSingle();
            z = reader["Z"].ToSingle();
            loc = new WoWPoint(x, y, z);
            Location = loc.ToInvariantString();
            if (reader.MoveToAttribute("MinFreeBagSlots"))
            {
                uint.TryParse(reader["MinFreeBagSlots"], out val);
                MinFreeBagSlots = val;
            }
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ItemID", ItemID);
            writer.WriteAttributeString("CheckNewMail", CheckNewMail.ToString());
            writer.WriteAttributeString("GetMailType", GetMailType.ToString());
            writer.WriteAttributeString("AutoFindMailBox", AutoFindMailBox.ToString());
            writer.WriteAttributeString("X", loc.X.ToString());
            writer.WriteAttributeString("Y", loc.Y.ToString());
            writer.WriteAttributeString("Z", loc.Z.ToString());
            writer.WriteAttributeString("MinFreeBagSlots", MinFreeBagSlots.ToString());
        }
        #endregion
    }
    #endregion
}

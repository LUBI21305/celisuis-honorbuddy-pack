using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Timers;
using System.Xml.Linq;
using System.Xml;

using Styx;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Database;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins.PluginClass;
using Styx.Logic.Pathing;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Inventory.Frames.Merchant;
using System.Text.RegularExpressions;
using Styx.Logic.Profiles;

namespace Allrounder
{
    class DestroyJunk
    {

        static DestroyJunk()
        {
            LoadProtectedItems();
        }
        static uint LastUpdate = 0;
       static public List<uint> ProtectedItems;
       static void LoadProtectedItems()
       {
            List<uint> tempList = null;
            string path = Path.Combine(Logging.ApplicationPath, "Protected Items.xml");
            if (File.Exists(path))
            {
                XElement xml = XElement.Load(path);
                tempList = xml.Elements("Item").Select(x => (uint)(x.Attribute("Entry").Value.ToInt32())).Distinct().ToList();
            }
            ProtectedItems = tempList != null ? tempList : new List<uint>();
       }
       static uint LastSellTry = 0;
       public static void DoDestroyJunk()
       {
            if(LastUpdate + 2000 < (uint)Environment.TickCount)
            {
                LastUpdate = (uint)Environment.TickCount;
            } else
                return;

            WoWSpell mount = null;
            uint FNBS = Me.FreeNormalBagSlots;
//            Logging.Write("SpellManager.HasSpell(61425):" + SpellManager.HasSpell(61425));
            if(Mount.CanMount() && ((uint)Environment.TickCount - LastSellTry > 30000 ) && (FNBS <= 2))
            {
                if (Me.Mounted)         // optional
                {
                    Mount.Dismount();
                    Thread.Sleep(4400); // Gnimo does not disappear instantly
                }
//                if () // Sell to mamoth
                { 
                    mount = WoWSpell.FromId(61425);
                    mount.Cast();
                    Thread.Sleep(500);
                    if(Me.IsCasting)
                    {
                        Thread.Sleep(2500);
                        ObjectManager.Update();
                    }
                }
                LastSellTry = (uint)Environment.TickCount;
                if( Me.Mounted )
                {
                    // todo: debug sell procedure
                    IsDone = false; SellDone = false;
                        while(!SellDone && ((uint)Environment.TickCount - LastSellTry < 8000 ))
                        {
                            Logging.Write("attempting trade..." + (uint)Environment.TickCount);
                            SellDone = InteractAndSell(32639,SellItemActionType.Whites,0,0); // SellItemActionType.Whites
                            Thread.Sleep(1000);
                        }
                }
            } else
            {
                List<WoWItem> targetItems = ObjectManager.Me.BagItems;

                uint TotalCount = 0;
                for (int a = targetItems.Count-1; a >= 0; a--)
                {
                    if(targetItems[a] != null && targetItems[a].IsValid)
                    {
                        ItemInfo info = ItemInfo.FromId(targetItems[a].Entry);
                        WoWItemQuality quality = targetItems[a].Quality;

                        if(!ProtectedItems.Contains(targetItems[a].Entry) && (quality == WoWItemQuality.Uncommon && (info.Level < 290) || quality == WoWItemQuality.Common || quality == WoWItemQuality.Poor))
                        {
                              WoWMovement.MoveStop();
                              Logging.Write("[Allrounder] Destroying " + targetItems[a].Entry + " \""+ targetItems[a].Name + "\"...");
                                Lua.DoString("UseItemByName(\"" + targetItems[a].Name + "\")");
//                                Lua.DoString("UseContainerItem({0}, {1})", targetItems[a].BagIndex + 1, targetItems[a].BagSlot + 1);
                                Lua.DoString("PickupContainerItem({0}, {1})", targetItems[a].BagIndex + 1, targetItems[a].BagSlot + 1);
                                Thread.Sleep(900);
                                Lua.DoString("DeleteCursorItem();");
                        }
                    }
                }
             }
        }

        static bool IsDone = false;
        static bool SellDone = false;
        public static bool InteractAndSell(uint unitID, SellItemActionType SellItemType, uint OptionalItemEntry, uint Count) 
                                                                            // Count - for "SellItemActionType.Specific"
        {
//                  while (!Me.Combat && )
//                  Thread.Sleep(100);
            if (!IsDone)
            {
                if (MerchantFrame.Instance == null || !MerchantFrame.Instance.IsVisible)
                {
                    WoWPoint movetoPoint = Me.Location;
                    WoWUnit unit = null;
                    unit = ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.Entry == unitID).
                        OrderBy(o => o.Distance).FirstOrDefault();
                    if (unit != null)
                        movetoPoint = WoWMathHelper.CalculatePointFrom(Me.Location, unit.Location, 3);
                    else if (movetoPoint == WoWPoint.Zero)
                        movetoPoint = GetLocationFromDB(MoveToType.NpcByID, unitID);
                    if (movetoPoint != WoWPoint.Zero && ObjectManager.Me.Location.Distance(movetoPoint) > 4.5)
                    {
                        Navigator.MoveTo(movetoPoint);
                        // Util.MoveTo(movetoPoint);
                    }
                    else if (unit != null)
                    {
                        // open selling interface:
                        unit.Target();
                        unit.Interact();
                    }
                    if (GossipFrame.Instance != null && GossipFrame.Instance.IsVisible)
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
                    if (SellItemType == SellItemActionType.Specific)
                    {
                        List<WoWItem> itemList = ObjectManager.Me.BagItems.Where(u => u.Entry == OptionalItemEntry).Take((int)Count).ToList();
                        if (itemList != null)
                        {
                            using (new FrameLock())
                            {
                                foreach (WoWItem item in itemList)
                                    item.UseContainerItem();
                            }
                        }
                    }
                    else
                    {
                        List<WoWItem> itemList = null;
                        IEnumerable<WoWItem> itemQuery = from item in Me.BagItems
                                                         where !ProtectedItems.Contains(item.Entry)
                                                         select item;
                        switch (SellItemType)
                        {
                            case SellItemActionType.Greys:
                                itemList = itemQuery.Where(i => i!=null && i.IsValid && (i.Quality == WoWItemQuality.Poor)).ToList();
                                break;
                            case SellItemActionType.Whites:
                                itemList = itemQuery.Where(i => i!=null && i.IsValid && (i.Quality == WoWItemQuality.Common || i.Quality == WoWItemQuality.Poor)).ToList();
                                break;
                            case SellItemActionType.Greens:
                                itemList = itemQuery.Where(i => i!=null && i.IsValid && ( i.Quality == WoWItemQuality.Uncommon || i.Quality == WoWItemQuality.Common || i.Quality == WoWItemQuality.Poor)).ToList();
                                break;
                        }
                        if (itemList != null)
                        {
                            using (new FrameLock())
                            {
                                foreach (WoWItem item in itemList)
                                {
                                    item.UseContainerItem();
                                }
                            }
                        }
                    }
                    Logging.Write("SellItemAction Completed ");
                    IsDone = true;
                    return true;
                }
                if (IsDone)
                    Lua.DoString("CloseMerchant()");
                else 
                    return false;
            }
            return false;

        }
         static public WoWPoint GetLocationFromDB(MoveToType type, uint entry)
        {
            NpcResult npcResults = null;
            switch (type)
            {
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
        public enum SellItemActionType
        {
            Specific,
            Greys,
            Whites,
            Greens,
        }
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

        private static LocalPlayer Me { get { return ObjectManager.Me; } }

       }
}

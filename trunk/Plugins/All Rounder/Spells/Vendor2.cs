using System;
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
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins.PluginClass;
using Styx.Logic.Pathing;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.MailBox;
using System.Text.RegularExpressions;
using Styx.Logic.Profiles;

namespace Allrounder
{
    class Vendors
    {
        public static List<uint> _Vendorlist = new List<uint>
        {
            // Enter Ids for Vendoring Here
            52191,//Ocean Sapphire
            52190,//Inferno Ruby
            52195,//Amberjewel
            52194,//Demonseye
            52192,//Dream Emerald
            52193,//Ember topaz
        };

        public static void Vendors2()
        {
            List<WoWUnit> unitList = ObjectManager.GetObjectsOfType<WoWUnit>();
            List<WoWUnit> merchantList = new List<WoWUnit>();
            foreach (WoWUnit u in unitList)
            {
                //u.IsAmmoVendor || u.IsFoodVendor || u.IsInnkeeper || u.IsPoisonVendor || u.IsReagentVendor || u.IsRepairMerchant || 
                if (u.IsVendor)
                {
                    merchantList.Add(u);
                }
            }
            merchantList.Sort((p1, p2) => p1.Location.Distance(ObjectManager.Me.Location).CompareTo(p2.Location.Distance(ObjectManager.Me.Location)));
            if (merchantList.Count > 0)
            {
                while (!StyxWoW.Me.Combat && merchantList[0].Distance > 5)
                {
                    Navigator.MoveTo(merchantList[0].Location);
                    Thread.Sleep(100);
                }
                merchantList[0].Interact();
                Thread.Sleep(2000);
                try
                {
                    GossipFrame gFrame = GossipFrame.Instance;
                    foreach (var option in gFrame.GossipOptionEntries)
                    {
                        if (option.Type == GossipEntry.GossipEntryType.Vendor)
                        {
                            gFrame.SelectGossipOption(option.Index);
                            break;
                        }
                    }
                }
                catch { }
                Thread.Sleep(500);
                foreach (WoWItem item in ObjectManager.Me.BagItems)
                {
                    if (_Vendorlist.Contains(item.Entry) && item.BagSlot != -1)
                    {
                        Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                        Thread.Sleep(100);
                    }
                }
            }
            else
            {
                Logging.Write(System.Drawing.Color.Red, "No Vendor Found in Area");
            }
        }
    }
}

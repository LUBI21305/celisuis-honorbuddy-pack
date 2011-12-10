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
    class Cloth
    {
        public static List<uint> _itemlist7 = new List<uint>
        {
            53010, //Embersilk Cloth
            33470, //Frostweave Cloth
        };
        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }
        public static void Clothspell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (_itemlist7.Contains(item.Entry) && item.StackCount >= 5)
                {
                    WoWMovement.MoveStop();
                    Thread.Sleep(100);
                    while (NumOfItemsInBag(53010) >= 5)
                    {
                        SpellManager.Cast(74964);
                        Thread.Sleep(250);
                        Logging.Write(Color.Red, "[Cloth]:Using " + item.Name + "");
                        Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                        Thread.Sleep(1000);
                        if (ObjectManager.Me.FreeBagSlots <= 3)
                        {
                            Logging.Write(Color.Red, "Stopping Because Free Bagspace");
                            break;
                        }
                    }
                    while (NumOfItemsInBag(33470) >= 5)
                    {
                        SpellManager.Cast(55899);
                        Thread.Sleep(250);
                        Logging.Write(Color.Red, "[Cloth]:Using " + item.Name + "");
                        Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                        Thread.Sleep(1000);
                        if (ObjectManager.Me.FreeBagSlots <= 3)
                        {
                            Logging.Write(Color.Red, "Stopping Because Free Bagspace");
                            break;
                        }
                    }
                }

            }
        }
    }
}

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
    class Prospecting
    {
        public static List<uint> OreList = new List<uint>()
         {
            2770,  //Copper
            2771,  //Tin
            2772,  //Iron
            10620, //Thorium
            3858,  //Mithril
            23424, //Fel Iron
            23425, //Adamantite
            36909, //Cobalt
            36912, //Saronite
            36910, //Titanium
            52185, //Elementium
            53038, //Obsidium
            52183, //Pyrite
         };
        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }
        public static void Prospectingspell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (OreList.Contains(item.Entry))
                {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (item.StackCount >= 5 && item.BagSlot != -1) 
                        {
                            SpellManager.Cast(31252);
                            Logging.Write(Color.Cyan, "[Prospecting]:Using " + item.Name + " i have {0}", NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(4500);                           
                        }
                        if (ObjectManager.Me.FreeNormalBagSlots <= 2)
                        {
                            Logging.Write(Color.Red, "Stopping Because Free Bagspace");
                            break;
                        }
                    }
                }
            }
        }
    }

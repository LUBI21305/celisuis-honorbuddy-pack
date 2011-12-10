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
    class Leather
    {
        public static List<uint> _Leatherlist = new List<uint>()
      {
         // Leather
            33568, // Borean Leather
            52977, //Savage Leather Scraps
            67495,//Strange Bloated Stomach
      };
        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }
        public static void leather()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (_Leatherlist.Contains(item.Entry))
                {
                    if (item.StackCount >= 6)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (item.StackCount >= 6) 
                        {
                            WoWMovement.MoveStop();
                            Logging.Write("[Leather]: Using " + item.Name + "");
                            Lua.DoString("UseItemByName(\"" + item.Name + "\")");
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
}

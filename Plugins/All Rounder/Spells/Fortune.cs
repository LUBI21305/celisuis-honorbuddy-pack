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
    class Fortunespell
    {
        public static List<uint> Fortunelist = new List<uint>
        {
            39502, // Paper
            61978, // Ink
        };

        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }

        public static void useFortunespell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (Fortunelist.Contains(item.Entry))
                {
                    if (item.StackCount > 1)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(100);
                        while (NumOfItemsInBag(39502) >= 1 && NumOfItemsInBag(61978) >= 1)
                        {
                            Logging.Write(Color.Purple, "[Fortune]:Making Fortune Cards");
                            Styx.Logic.Combat.WoWSpell fortune = Styx.Logic.Combat.WoWSpell.FromId(86609);
                            Styx.Logic.Combat.SpellManager.Cast(fortune);
                            Thread.Sleep(2500);
                            
                        }
                        if (NumOfItemsInBag(39502) < 1)
                        {
                            Logging.Write(Color.Chocolate, "[Fortune]:Need to buy more Paper");
                            break;
                        }
                        else if (NumOfItemsInBag(61978) < 1)
                        {
                            Logging.Write(Color.Chocolate, "[Fortune]:Need more Ink");
                            break;
                        }
                    }                   
                }
            }
        }
    }
}

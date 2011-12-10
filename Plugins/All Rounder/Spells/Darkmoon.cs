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
    class Darkmoon
    {
        public static List<uint> Darkmoonlist = new List<uint>
        {
            39502, // Paper
            61981, // Ink
            52329, //Volitile Life
        };

        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }

        public static void Darkmoonspell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (Darkmoonlist.Contains(item.Entry))
                {
                    if (NumOfItemsInBag(39502) >= 1 && NumOfItemsInBag(61981) >= 10 && NumOfItemsInBag(52329) >= 30)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(100);
                        while (NumOfItemsInBag(39502) >= 1 && NumOfItemsInBag(61981) >= 10 && NumOfItemsInBag(52329) >= 30)
                        {
                            Logging.Write(Color.Crimson, "[Darkmoon]:Making Darkmoon Cards Of Destruction");
                            Styx.Logic.Combat.WoWSpell dark = Styx.Logic.Combat.WoWSpell.FromId(86615);
                            Styx.Logic.Combat.SpellManager.Cast(dark);
                            Thread.Sleep(3500);
                        }
                        if (NumOfItemsInBag(39502) < 1 || NumOfItemsInBag(61981) < 10 || NumOfItemsInBag(52329) < 30)
                        {
                            Logging.Write(Color.Chocolate, "[Darkmoon]: Cant make any more, have no Mats");
                            break;
                        }
                    }
                }
            }
        }
    }
}


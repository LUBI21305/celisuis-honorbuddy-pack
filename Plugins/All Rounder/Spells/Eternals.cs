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
    class Eternals
    {
        public static List<uint> _Eternallist = new List<uint>
        {
         37700, // Crystallized Air
         37701, // Crystallized Earth
         37702, // Crystallized Fire
         37703, // Crystallized Shadow
         37704, // Crystallized Life
         37705, // Crystallized Water
         22572, // Mote of Air
         22573, // Mote of Earth 
         22574, // Mote of Fire
         22575, // Mote of Life
         22576, // Mote of Mana
         22577, // Mote of Shadow
         22578,	// Mote of Water
        };
        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }

        public static void Eternalsspell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (_Eternallist.Contains(item.Entry))
                {
                    if (item.StackCount >= 10)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        if (NumOfItemsInBag(item.Entry) >= 10) // 
                        {
                            SpellManager.Cast(51005);
                            Thread.Sleep(250);
                            Logging.Write(Color.SpringGreen, "[Eternal]:Using " + item.Name + "");
                            Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                            Thread.Sleep(4500);
                        }
                    }
                }
            }
                
        }
    }
}

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
    class Gems
    {
        public static List<uint> _gemlist = new List<uint>
        {
            52182, //Jasper
            52177, //Carnelian 
            52181, //Hessonite
            52178, //Zephyrite
            52179, //Alicite
            52180,  //Nightstone
        };

        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }

        public static void Gemspell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (_gemlist.Contains(item.Entry) && item.StackCount >= 1)
                {
                    WoWMovement.MoveStop();
                    Thread.Sleep(100);
                    Logging.Write("Cutting Gems");
                    while (NumOfItemsInBag(52182) >= 1)
                    {
                        Styx.Logic.Combat.WoWSpell jasper = Styx.Logic.Combat.WoWSpell.FromId(73274);
                        Logging.Write(" Cutting " + item.Name + "");
                        Styx.Logic.Combat.SpellManager.Cast(jasper);
                        Thread.Sleep(2000);
                    }
                    if (NumOfItemsInBag(52177) >= 1)
                    {
                        Styx.Logic.Combat.WoWSpell carnelian = Styx.Logic.Combat.WoWSpell.FromId(73222);
                        Logging.Write(" Cutting " + item.Name + "");
                        Styx.Logic.Combat.SpellManager.Cast(carnelian);
                        Thread.Sleep(2000);
                    }
                    if (NumOfItemsInBag(52181) >= 1)
                    {
                        Styx.Logic.Combat.WoWSpell Hessonite = Styx.Logic.Combat.WoWSpell.FromId(73268);
                        Logging.Write(" Cutting " + item.Name + "");
                        Styx.Logic.Combat.SpellManager.Cast(Hessonite);
                        Thread.Sleep(2000);
                    }
                    if (NumOfItemsInBag(52179) >= 1)
                    {
                        Styx.Logic.Combat.WoWSpell Alicite = Styx.Logic.Combat.WoWSpell.FromId(73239);
                        Logging.Write(" Cutting " + item.Name + "");
                        Styx.Logic.Combat.SpellManager.Cast(Alicite);
                        Thread.Sleep(2000);
                    }
                    if (NumOfItemsInBag(52180) >= 1)
                    {
                        Styx.Logic.Combat.WoWSpell Nightstone = Styx.Logic.Combat.WoWSpell.FromId(73250);
                        Logging.Write(" Cutting " + item.Name + "");
                        Styx.Logic.Combat.SpellManager.Cast(Nightstone);
                        Thread.Sleep(2000);
                    }
                    if (NumOfItemsInBag(52178) >= 1)
                    {
                        Styx.Logic.Combat.WoWSpell Zephyrite = Styx.Logic.Combat.WoWSpell.FromId(73230);
                        Logging.Write(" Cutting " + item.Name + "");
                        Styx.Logic.Combat.SpellManager.Cast(Zephyrite);
                        Thread.Sleep(2000);
                    }
                }
                if (StyxWoW.Me.FreeBagSlots <= 3)
                {
                    Logging.Write(Color.Red, "Stopping Because Free Bagspace");
                    break;
                }
            }
        }
    }
}

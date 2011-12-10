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
    class Lockpicking
    {
        public static List<uint> Lockboxes = new List<uint>
        {
            3714,
            103815,
            123330,
            181665 ,
            178244,
            105176 ,
            184793,
            74447,
            121264,
            105570,
            179486,
            123214,
            129127,
            179487,
            3239,
            179488,
            179490,
            179491,
            20691,
            179492,
            131978,
            179498,
            184931,
            184740,
            184936,
            184741,
            184940,
            191543,
            186648,
            16882,
            63349,
            16885,
            43575,
            29569,
            16884,
            16883,
            45986,
            43624,
            68729,
            5760,
            43622,
            4633,
            4634,
            31952,
            5758,
            19425,
            4632,
            4638,
            4637,
            4636,
            5759,
            30454,
            30646,
            30429,
            12192,
            12191,
            7870
        };

        public static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }

        public static void Lockpickingspell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (Lockboxes.Contains(item.Entry))
                {
                    WoWMovement.MoveStop();
                    Thread.Sleep(100);
                    while (NumOfItemsInBag(item.Entry) >= 1)
                    {
                        Styx.Logic.Combat.WoWSpell lockpick = Styx.Logic.Combat.WoWSpell.FromId(1804);
                        Logging.Write(Color.DimGray, "[Lockpicking]: Opening " + item.Name + "");
                        Styx.Logic.Combat.SpellManager.Cast(lockpick);
                        Thread.Sleep(100);
                        Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                        Thread.Sleep(5500);
                    }
                }
            }
        }
    }
}

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
    class Disenchant
    {
        public static void Disenchantspell()
        {
            if (!SpellManager.HasSpell(13262) || Battlegrounds.IsInsideBattleground || ObjectManager.Me.Mounted || ObjectManager.Me.Combat) 
            { 
            return; 
            }
            DisenchantItem();
        }
            
        public static List<WoWItem> SkipDisenchant = new List<WoWItem>();

        public static List<uint> ignoreItems = new List<uint>() {
            // mats
            10940, 10938, 10939, 10998, 10978, 11082, 11083, 11084, 11134, 11139, 11174, 11175,
            11176, 11177, 11178, 14343, 14344, 16202, 16203, 16204, 20725, 22445, 22446, 22447,
            22448, 22449, 22450, 34052, 34053, 34054, 34055, 34056, 34057, 46849, 49649, 52718,
            52719, 
            2592,52976,52555,52328,52326,52722,52325,53010,53643,52327,21877,4235,52329,42253,           // ANG
            11137,4306,33470,7392,56516,3864,14341,7910,2319,12808,8169,38425,

            // rods
            44452,

            // lockbox
            43622,68729,

            // healthstones
            5509, 5510, 5511, 5512, 9421, 19004, 19005, 19006,  
            19007, 19008, 19009, 19010, 19011, 19012, 19013, 
            22103, 22104, 22105, 36889, 36890, 36891, 36892,
            36893, 36894,

            // soulstones
            5232, 16892, 16893, 16895, 16896, 22116, 36895,

            // firestones
            40773, 41169, 41170, 41171, 41172, 41173, 41174,

            // spellstones
            41191, 41192, 41193, 41194, 41195, 41196,

            // HEALTH:
            57191, 34721,

            // food:
            58261,62668,
            // usable:
            21536,3823,33447,1357,
            // frequent:
            63128,
        };

        private static void DisenchantItem()
        {
            List<WoWItem> targetItems = ObjectManager.Me.BagItems;
            
            for (int a = targetItems.Count-1; a >= 0; a--)
            {
                    if (ignoreItems.Contains(targetItems[a].Entry) || SkipDisenchant.Contains(targetItems[a]))
                    {
                        targetItems.RemoveAt(a);
                    }
                    else if (targetItems[a].IsSoulbound || targetItems[a].IsAccountBound || ignoreItems.Contains(targetItems[a].Entry) || targetItems[a].Quality != WoWItemQuality.Uncommon) // targetItems[a].Quality != WoWItemQuality.Common && 
                    //  && item.ItemInfo.Level , required level
                    {
                        SkipDisenchant.Add(targetItems[a]);
                        targetItems.RemoveAt(a);
                    }
            }

                if (Equals(null, targetItems)) { return; }

                foreach (WoWItem deItem in targetItems)
                {
                    if(deItem.BagSlot != -1)
                    {
                        Styx.Helpers.Logging.Write(Color.FromName("DarkRed"), "[LiquidDisenchant]: {0} (Entry:{1}).", deItem.Name, deItem.Entry);
                        WoWSpell spell = WoWSpell.FromId(13262);    // ANG
                        using (new Styx.FrameLock())
                        {
                            spell.Cast();
                            Lua.DoString("UseContainerItem({0}, {1})", deItem.BagIndex + 1, deItem.BagSlot + 1);
                        }
//                      Lua.DoString("CastSpellByName(\"Disenchant\")");
//                      Thread.Sleep(500);
//                      Lua.DoString("UseItemByName(\"" + deItem.Name + "\")");
                        Thread.Sleep(300);

                    while (ObjectManager.Me.IsCasting)
                        {
                            Thread.Sleep(3100);
                        }

                    Thread.Sleep(2500);

                    // wait for lootframe to close
                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    while (Styx.Logic.LootTargeting.LootFrameIsOpen)
                        {
                            Thread.Sleep(250);

                            if (timer.ElapsedMilliseconds >= 6000)
                            {
                                break;
                            }
                        }
                    
                        if (!ObjectManager.Me.Combat)
                        {
                            Thread.Sleep(1500);
                            SkipDisenchant.Add(deItem);
                        }
                    }
                }
            return;
        }
        
    }
}
    


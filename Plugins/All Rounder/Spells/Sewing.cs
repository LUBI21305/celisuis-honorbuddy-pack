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
    class Sewing
    {

        static uint LastUpdate = 0;
       static WoWSkill _Tailoring = ObjectManager.Me.GetSkill(Styx.SkillLine.Tailoring);
       public static void SewingDo()
       {
            if(LastUpdate + 2000 < (uint)Environment.TickCount)
            {
                LastUpdate = (uint)Environment.TickCount;
            } else
                return;

            if (450 <= _Tailoring.CurrentValue && !Me.IsCasting && !Me.Combat && !Me.IsFlying && !Battlegrounds.IsInsideBattleground)
            {
                Styx.Logic.Combat.WoWSpell CurrRecipeProduction = Styx.Logic.Combat.WoWSpell.FromId(74964); 
//                if(SpellManager.HasSpell(CurrRecipeProduction)) // does not work
                {
//                    Logging.Write("[Allrounder] Have spell.");
                    List<WoWItem> targetItems = ObjectManager.Me.BagItems;
                    
                    uint TotalCount = 0;
                    for (int a = targetItems.Count-1; a >= 0; a--)
                    {
                            if ((targetItems[a].Entry == 53010))            // Embersilk
                                TotalCount += targetItems[a].StackCount;
                            if(targetItems[a].Entry == 67495)   // container
                            {
                                Lua.DoString("UseContainerItem({0}, {1})", targetItems[a].BagIndex + 1, targetItems[a].BagSlot + 1);
                                Thread.Sleep(900);
                            }


                        if(TotalCount >= 5)
                        {
                                Logging.Write("[Allrounder] Making Bolt of Embersilk...");
                                WoWMovement.MoveStop();
                                Thread.Sleep(200);
                                SpellManager.Cast(CurrRecipeProduction);         // Bolt of Embersilk
                                Thread.Sleep(2000);
                                break;
                        }
                    }
                }
            }
       }

       private static LocalPlayer Me { get { return ObjectManager.Me; } }

    }
}
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
    class Skinning
    {

        static uint LastUpdate = 0;
       static WoWSkill _Leatherworking = ObjectManager.Me.GetSkill(Styx.SkillLine.Leatherworking);
       public static void SkinningDo()
       {
            if(LastUpdate + 2000 < (uint)Environment.TickCount)
            {
                LastUpdate = (uint)Environment.TickCount;
            } else
                return;

            if (450 <= _Leatherworking.CurrentValue && !Me.IsCasting && !Me.IsFlying && !Me.Combat && !Battlegrounds.IsInsideBattleground)
            {
                Styx.Logic.Combat.WoWSpell CurrRecipeProduction = Styx.Logic.Combat.WoWSpell.FromId(78436); 
//                if(SpellManager.HasSpell(CurrRecipeProduction)) // does not work
                {
//                    Logging.Write("[Allrounder] Have spell.");
                    List<WoWItem> targetItems = ObjectManager.Me.BagItems;
                    
                    uint TotalCount = 0;
                    for (int a = targetItems.Count-1; a >= 0; a--)
                    {
                            if ((targetItems[a].Entry == 52976))            // Savage Leather
                                TotalCount += targetItems[a].StackCount;
                            if(targetItems[a].Entry == 67495)
                            {
                                Lua.DoString("UseContainerItem({0}, {1})", targetItems[a].BagIndex + 1, targetItems[a].BagSlot + 1);
                                Thread.Sleep(900);
                            }


                        if(TotalCount >= 5)
                        {
                                Logging.Write("[Allrounder] Making Savage Leather...");
                                WoWMovement.MoveStop();
                                Thread.Sleep(200);
                                SpellManager.Cast(CurrRecipeProduction);         // Heavy Savage Leather
                                Thread.Sleep(3500);
                                break;
                        }
                    }
                }
            }
       }

       private static LocalPlayer Me { get { return ObjectManager.Me; } }

    }
}
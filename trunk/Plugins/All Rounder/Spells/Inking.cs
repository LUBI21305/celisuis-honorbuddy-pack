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
    class Inking
    {
        public static List<uint> _Inklist = new List<uint>
        {
            // INKS
            39151, //Alabaster Pigment
            39334, //Dusky Pigment
            43103, //Verdant Pigment
            39338, //Golden Pigment
            43104, //Burnt Pigment
            39339, //Emerald Pigment
            43105, //Indigo Pigment
            39340, //Violet Pigment
            43106, //Ruby Pigment
            39341, //Silvery Pigment
            43107, //Sapphire Pigment
            39342, //Nether Pigment
            43108, //Ebon Pigment
            39343, //Azure Pigment
            43109, //Icy Pigment
            61979, //Ashen Pigment
            61980, //Burning Embers
        };
        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }
        public static void Inkingspell()
        {
        foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (_Inklist.Contains(item.Entry))
                {
                    if (item.Entry == 39343 && item.StackCount >= 2)
                    {

                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell Pig = Styx.Logic.Combat.WoWSpell.FromId(57715);
                            Styx.Logic.Combat.SpellManager.Cast(Pig);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 43109 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell Pig1 = Styx.Logic.Combat.WoWSpell.FromId(57716);
                            Styx.Logic.Combat.SpellManager.Cast(Pig1);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                            
                        }
                    }
                    else if (item.Entry == 61980 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)  
                        {
                            Styx.Logic.Combat.WoWSpell Pig2 = Styx.Logic.Combat.WoWSpell.FromId(86005);
                            Styx.Logic.Combat.SpellManager.Cast(Pig2);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                            
                        }
                    }

                    else if (item.Entry == 61979 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell Pig3 = Styx.Logic.Combat.WoWSpell.FromId(86004);
                            Styx.Logic.Combat.SpellManager.Cast(Pig3);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                            
                        }
                    }

                    else if (item.Entry == 39151 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell Pig4 = Styx.Logic.Combat.WoWSpell.FromId(52738);
                            Styx.Logic.Combat.SpellManager.Cast(Pig4);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 39334 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell Pig5 = Styx.Logic.Combat.WoWSpell.FromId(53462);
                            Styx.Logic.Combat.SpellManager.Cast(Pig5);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 43103 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell Pig6 = Styx.Logic.Combat.WoWSpell.FromId(57703);
                            Styx.Logic.Combat.SpellManager.Cast(Pig6);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 39338 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig7 = Styx.Logic.Combat.WoWSpell.FromId(57704);
                            Styx.Logic.Combat.SpellManager.Cast(pig7);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 43104 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig8 = Styx.Logic.Combat.WoWSpell.FromId(57706);
                            Styx.Logic.Combat.SpellManager.Cast(pig8);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 39339 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig9 = Styx.Logic.Combat.WoWSpell.FromId(57707);
                            Styx.Logic.Combat.SpellManager.Cast(pig9);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 43105 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig10 = Styx.Logic.Combat.WoWSpell.FromId(57708);
                            Styx.Logic.Combat.SpellManager.Cast(pig10);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 39340 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig11 = Styx.Logic.Combat.WoWSpell.FromId(57709);
                            Styx.Logic.Combat.SpellManager.Cast(pig11);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 43106 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig12 = Styx.Logic.Combat.WoWSpell.FromId(57710);
                            Styx.Logic.Combat.SpellManager.Cast(pig12);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 39341 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig13 = Styx.Logic.Combat.WoWSpell.FromId(57711);
                            Styx.Logic.Combat.SpellManager.Cast(pig13);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 43107 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig14 = Styx.Logic.Combat.WoWSpell.FromId(43107);
                            Styx.Logic.Combat.SpellManager.Cast(pig14);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 39342 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig15 = Styx.Logic.Combat.WoWSpell.FromId(57713);
                            Styx.Logic.Combat.SpellManager.Cast(pig15);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }

                    else if (item.Entry == 43108 && item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(250);
                        while (NumOfItemsInBag(item.Entry) >= 2)
                        {
                            Styx.Logic.Combat.WoWSpell pig16 = Styx.Logic.Combat.WoWSpell.FromId(57714);
                            Styx.Logic.Combat.SpellManager.Cast(pig16);
                            Logging.Write(Color.Purple, "[Inking]:Inking {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                            Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                            Thread.Sleep(2500);
                        }
                    }
                }
            }
        }
    }
}

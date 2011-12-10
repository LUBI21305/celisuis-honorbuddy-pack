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
    class Autosmelt
    {
        public static List<uint> _Orelist = new List<uint>()
      {

         // ORE
            2770,  //Copper
            2771,  //Tin
            2772,  //Iron
            2776,  //Gold
            2775,  //Silver
            7911,  //Truesilver
            10620, //Thorium
            3858,  //Mithril
            23424, //Fel Iron
            23425, //Adamantite
            36909, //Cobalt
            36912, //Saronite
            36910, //Titanium
            52185, //Elementium
            53038, //Obsidium
            52183, //Pyrite
      };

        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }

        public static void UseNearestAnvil()
        {
            List<WoWGameObject> ObjList = ObjectManager.GetObjectsOfType<WoWGameObject>();
            List<WoWGameObject> forgeList = new List<WoWGameObject>();

            foreach (WoWGameObject o in ObjList)
            {
                if (o.SubType == WoWGameObjectType.SpellFocus)
                {
                    forgeList.Add(o);
                }
            }
            if (forgeList.Count > 0)
            {
                forgeList.Sort((p1, p2) => p1.Location.Distance(ObjectManager.Me.Location).CompareTo(p2.Location.Distance(ObjectManager.Me.Location)));
                while (!ObjectManager.Me.Combat && forgeList[0].Distance > 5)
                {
                    Navigator.MoveTo(forgeList[0].Location);
                    Thread.Sleep(100);
                }
            }
            else
            {
                Logging.Write(Color.Red, "No Anvil in area");
                return;
            }
        }

        public static void Autosmeltspell()
        {
            foreach (WoWItem item in ObjectManager.Me.BagItems)
            {
                if (_Orelist.Contains(item.Entry))
                {
                    if (item.StackCount >= 2)
                    {
                        WoWMovement.MoveStop();
                        Thread.Sleep(100);
                        if (item.Entry == 2770)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltcopper = Styx.Logic.Combat.WoWSpell.FromId(2657);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltcopper);
                            Thread.Sleep(2000);
                        }

                        else if (item.Entry == 2771)
                        {
                            Styx.Logic.Combat.WoWSpell Smelttin = Styx.Logic.Combat.WoWSpell.FromId(3304);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smelttin);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 2775)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltsilver = Styx.Logic.Combat.WoWSpell.FromId(2658);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltsilver);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 2772)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltiron = Styx.Logic.Combat.WoWSpell.FromId(3307);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltiron);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 2776)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltgold = Styx.Logic.Combat.WoWSpell.FromId(3308);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltgold);
                            Thread.Sleep(2000);
                        }

                        else if (item.Entry == 3858)
                        {

                            Styx.Logic.Combat.WoWSpell Smeltmithril = Styx.Logic.Combat.WoWSpell.FromId(10097);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltmithril);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 7911)
                        {
                            Styx.Logic.Combat.WoWSpell Smelttruesilver = Styx.Logic.Combat.WoWSpell.FromId(10098);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smelttruesilver);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 10620)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltthorium = Styx.Logic.Combat.WoWSpell.FromId(16153);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltthorium);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 23424)
                        {

                            Styx.Logic.Combat.WoWSpell Smeltfeliron = Styx.Logic.Combat.WoWSpell.FromId(29356);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltfeliron);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 23425)
                        {

                            Styx.Logic.Combat.WoWSpell Smeltadamantite = Styx.Logic.Combat.WoWSpell.FromId(29358);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltadamantite);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 23427)
                        {

                            Styx.Logic.Combat.WoWSpell Smelteternium = Styx.Logic.Combat.WoWSpell.FromId(29359);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smelteternium);
                            Thread.Sleep(2000);
                        }



                        else if (item.Entry == 36909)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltcobalt = Styx.Logic.Combat.WoWSpell.FromId(49252);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltcobalt);
                            Thread.Sleep(2000);
                        }



                        else if (item.Entry == 36912)
                        {

                            Styx.Logic.Combat.WoWSpell Smeltsaronite = Styx.Logic.Combat.WoWSpell.FromId(49258);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltsaronite);
                            Thread.Sleep(2000);
                        }



                        else if (item.Entry == 36910)
                        {
                            Styx.Logic.Combat.WoWSpell Smelttitanium = Styx.Logic.Combat.WoWSpell.FromId(55211);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smelttitanium);
                            Thread.Sleep(2000);
                        }

                        else if (item.Entry == 52185)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltelementium = Styx.Logic.Combat.WoWSpell.FromId(74530);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltelementium);
                            Thread.Sleep(2000);
                        }


                        else if (item.Entry == 52183)
                        {
                            Styx.Logic.Combat.WoWSpell Smeltpyrite = Styx.Logic.Combat.WoWSpell.FromId(74529);
                            Styx.Helpers.Logging.Write("Smelting " + item.Name + "");
                            Styx.Logic.Combat.SpellManager.Cast(Smeltpyrite);
                            Thread.Sleep(2000);
                        }
                    }
                }
            }
        }
    }
}      
                
                                          
                                                  
                                                
                                            
                                        
                                    
                                
                            
                        
                    
                
            
        
    

                    
       
                
            
        
    


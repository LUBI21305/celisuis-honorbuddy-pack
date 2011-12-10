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
    class Milling
    {
       static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }
        public static LocalPlayer Me = ObjectManager.Me;
        
        public static void milling()
        {
            foreach (WoWItem item in Me.BagItems)
            {
                List<uint> HerbList = new List<uint>
                {
                    //Classic Herbs
                    785,  //MageRoyal
                    2449, //Earthroot
                    2447, // Peacebloom
                    765, //Silverleaf
                    2450, //Briarthorn
                    2453, //Bruiseweed
                    3820, //Stranglekelp
                    2452, //Swiftthistle
                    3369, //Grave Moss
                    3356, //Kingsblood
                    3357, //Liferoot
                    3355, //Wild Steelbloom
                    3819, //Dragonsteeth
                    3818, //Fadeleaf
                    3821, //Goldthorn
                    3358, //Khadgar's Whisker
                    8836, //Arthas Tear
                    8839, //Blindweed
                    4625, // Firebloom
                    8846, //Gromsblood
                    8831, //Purple lotus
                    8838, //Sungrass
                    13463, //Dreamfoil

                    //BC Herbs
                    13464, //Golden Sansam
                    22786, //Dreaming Glory
                    22785, //Felweed
                    22793, // Mana thistle
                    22791, //Netherbloom
                    22792, //Nightmare vine
                    22787, //Rageveil
                    22789, //Terocone

                    //WOTLK Herbs
                    36907, //Talandra's Rose
                    36903, //Adder's Tongue
                    36906, //Icethorn
                    36904, //Tiger Lily
                    36905, //Lichbloom
                    36901, //Goldclover
                    39970,//Fire Leaf
                    37921,//Deadnettle

                    //Cata Herbs
                    52983,//Cinderbloom
                    52987,//Twilight Jasmine
                    52984,//Stormvine
                    52986,//Heartblossom
                    52985,//Azshara's Veil
                    52988,//Whiptail
                    

                };

                while (HerbList.Contains(item.Entry) && item.StackCount >= 5 && item.BagSlot != -1)
                {
                    WoWMovement.MoveStop();
                    Thread.Sleep(250);
                    Logging.Write(Color.RoyalBlue, "[Milling]:Milling {0} I have {1}", item.Name, NumOfItemsInBag(item.Entry));
                    SpellManager.Cast(51005);
                    Lua.DoString(string.Format("UseContainerItem({0}, {1})", item.BagIndex + 1, item.BagSlot + 1));
                    Thread.Sleep(2500);
                }
                if (StyxWoW.Me.FreeNormalBagSlots <= 2)
                {
                    Logging.Write(Color.Red, "Stopping Because Free Bagspace");
                    break;
                }
            }
        }
    }
}
    


                
              
            
        
    


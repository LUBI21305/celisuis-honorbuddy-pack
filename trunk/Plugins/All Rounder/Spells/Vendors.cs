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
using Styx.Logic.Inventory.Frames.Merchant;

namespace Allrounder
{
    class DarkVendors
    {
        static int NumOfItemsInBag(uint entry)
        {
            return Lua.GetReturnValues("return GetItemCount (\"" + entry.ToString() + "\")", "fish.lua")[0].ToInt32();
        }

        public static WoWPoint Movetovendink = new WoWPoint(-8854.4, 862.9097, 99.61047);
        public static WoWPoint Movetovendpap = new WoWPoint(-8863.951, 861.2503, 99.60957);
        public static WoWPoint Inscrip = new WoWPoint(-8856.686, 859.3149, 99.60765);
        static LocalPlayer Me = ObjectManager.Me;

        public static void PaperVendors()
        {
            List<WoWUnit> Merchantlist = ObjectManager.GetObjectsOfType<WoWUnit>();
            while (Me.Location != Movetovendpap && NumOfItemsInBag(39502) < 20)
            {
                Logging.Write("Moving to Paper Vendor");
                Navigator.MoveTo(Movetovendpap);
                if (Me.Location.Distance(Movetovendpap) < 5)
                {
                    Logging.Write("breaking loop");
                    break;
                }
            }

            foreach (WoWUnit u in Merchantlist)
            {
                if (!Me.Combat && NumOfItemsInBag(39502) < 60)
                {
                    while (u.Name == "Stanly McCormick" && u.Distance <= 5)
                    {
                        u.Target();
                        WoWMovement.MoveStop();
                        u.Interact(u.Name == "Stanley McCormick");
                        int p = 6;
                        Logging.Write("Buying Paper");
                        Lua.DoString("BuyMerchantItem(\"" + p + "\",1)");
                        Thread.Sleep(100);
                        if (NumOfItemsInBag(39502) >= 60)
                        {
                            break;
                        }
                    }
                }
            }
        }


        public static void Inkvendor()
        {
            List<WoWUnit> Merchantlist = ObjectManager.GetObjectsOfType<WoWUnit>();
            while (Me.Location != Movetovendink && NumOfItemsInBag(61978) > 10)
            {
                Logging.Write("Moving to Ink Vendor");
                Navigator.MoveTo(Movetovendink);
                if (Me.Location.Distance(Movetovendink) < 5)
                {
                    Logging.Write("breaking loop");
                    break;
                }
            }

            foreach (WoWUnit u in Merchantlist)
            {
                if (!Me.Combat && NumOfItemsInBag(61978) >= 10)
                {
                    while (u.Name == "Sarana Damir" && u.Distance <= 5)
                    {
                        u.Target();
                        WoWMovement.MoveStop();
                        u.Interact(u.Name == "Sarana Damir");
                        Thread.Sleep(250);
                        int i = 11;
                        Logging.Write("Trading Inks");
                        Lua.DoString("BuyMerchantItem(\"" + i + "\",1)");
                        Thread.Sleep(100);
                        if (NumOfItemsInBag(61978) < 10)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}

  
    




   

    


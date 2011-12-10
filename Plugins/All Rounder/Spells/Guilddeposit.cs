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
    class Guilddeposit
    {     
        public static int GetNumGuildBankTabs
        {
            get { return Lua.GetReturnVal<int>("return GetNumGuildBankTabs()", 0); }
        }

        static bool CanViewGuildTab(int tab)
        {
            return Lua.GetReturnValues("return GetGuildBankTabInfo(" + tab.ToString() + ")", "trade.lua")[2] == "1";
        }

        static int NumOfItemsInGbank(uint entry)
        {
            int count = 0;
            int tabNum = GetNumGuildBankTabs;
            for (int tabIndex = 1; tabIndex <= 6; tabIndex++)
            {
                if (CanViewGuildTab(tabIndex))
                {
                    Lua.DoString("GuildBankTab" + tabIndex + "Button:Click(\"\")");
                    Thread.Sleep(2000);
                }
            }
            return count;
        }

        public static Form3 b = new Form3();
        public static deposititems dl = new deposititems();

        public static void Guilddepositspell()
        {

            List<WoWGameObject> gbankList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                                                         .Where(x => x.SubType == WoWGameObjectType.GuildBank)
                                                         .ToList();
            if (gbankList.Count > 0)
            {
                gbankList.OrderBy(x => x.Location.Distance(ObjectManager.Me.Location));

                while (!ObjectManager.Me.Combat && gbankList[0].Distance > 5)
                {
                    Logging.Write("[GuildDeposit]:Moving to GuildBank");
                    Navigator.MoveTo(gbankList[0].Location);
                    Thread.Sleep(100);
                }

                gbankList[0].Interact();
                Thread.Sleep(200);

                int tabNum = GetNumGuildBankTabs;

                for (int tab = 1; tab <= tabNum; tab++)
                {
                    Logging.Write("[GuildDeposit]:Checking if we can view guild bank tab {0}", tab);
                    Thread.Sleep(250);
                    if (CanViewGuildTab(tab))
                    {
                        Logging.Write("[GuildDeposit]:Can view, loading guild bank tab {0}", tab);

                        Lua.DoString("SetCurrentGuildBankTab(" + tab.ToString() + ")");
                        for (int slot = 1; slot <= 98; slot++)
                        {
                                Logging.Write("Checking slot {0}", slot);

                            Logging.Write("Tab: {0}, Slot: {1}.", tab, slot);
                            Thread.Sleep(100);
                            String ret = Lua.GetReturnVal<String>("return GetGuildBankItemLink(" + tab.ToString() + "," + slot.ToString() + ")", 0);
                            Logging.Write("Return value: {0}", ret);

                            if (String.IsNullOrEmpty(ret))
                            {
                                if (!b.PulseLoadsettings3())
                                {
                                    b.PulseLoadsettings3();
                                    Logging.Write(Color.SeaShell, "Guild Deposit Settings Loaded");
                                }

                                foreach (WoWItem item in ObjectManager.Me.BagItems)
                                {
                                    if (b.Inkde)
                                    {
                                        if (dl.Inklist.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (b.Gemsde)
                                    {
                                        if (dl.GemsList.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (b.Darkde)
                                    {
                                        if (dl.Darklist.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (b.Leatherde)
                                    {
                                        if (dl.LeatherList.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (b.Eternalde)
                                    {
                                        if (dl.EternalList.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (b.Disenchantde)
                                    {
                                        if (dl.Disenchantlist.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (b.Clothde)
                                    {
                                        if (dl.Clothlist.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (b.Fortunede)
                                    {
                                        if (dl.Fortune.Contains(item.Entry))
                                        {
                                            item.UseContainerItem();
                                            Logging.Write("Depositing {0} into {1},{2}", item.Name, tab, slot);
                                            Lua.DoString("PickupGuildBankItem(" + tab.ToString(), slot.ToString() + ")");
                                            Thread.Sleep(1000);
                                        }
                                    }

                                }
                            }
                            if (tab == 6 && slot == 98)
                            {
                                Lua.DoString("CloseGuildBankFrame()");
                                Logging.Write(Color.Red, "Guild Deposit Run Complete");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}


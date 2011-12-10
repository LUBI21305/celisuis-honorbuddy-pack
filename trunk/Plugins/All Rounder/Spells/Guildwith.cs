using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using Allrounder;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Allrounder
{
    class Guildwith
    {
        public static int GetNumGuildBankTabs
        {
            get { return Lua.GetReturnVal<int>("return GetNumGuildBankTabs()", 0); }
        }

        static bool CanViewGuildTab(int tab)
        {
            return Lua.GetReturnValues("return GetGuildBankTabInfo(" + tab.ToString() + ")", "trade.lua")[2] == "1";
        }

        
        public static Guildwithdraw a = new Guildwithdraw();
        public static withdrawitems gl = new withdrawitems();
        

        public static void Guildwithspell()
        {

            List<WoWGameObject> gbankList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                                                         .Where(x => x.SubType == WoWGameObjectType.GuildBank)
                                                         .ToList();
            if (gbankList.Count > 0)
            {
                gbankList.OrderBy(x => x.Location.Distance(ObjectManager.Me.Location));

                while (!ObjectManager.Me.Combat && gbankList[0].Distance > 5)
                {
                    Logging.Write("[GuildWithdraw]:Moving to GuildBank");
                    Navigator.MoveTo(gbankList[0].Location);
                    Thread.Sleep(100);
                }

                gbankList[0].Interact();
                Thread.Sleep(200);

                int tabNum = GetNumGuildBankTabs;
               
                for (int tabIndex = 1; tabIndex <= 6; tabIndex++)
                {
               
                    Logging.Write("[GuildWithdraw]:Checking if we can view guild bank tab {0}", tabIndex);
                    Thread.Sleep(250);
                    if (CanViewGuildTab(tabIndex))
                    {

                        Lua.DoString("GuildBankTab" + tabIndex + "Button:Click(\"\")");
                        Logging.Write("[GuildWithdraw]:Can view, loading guild bank tab {0}", tabIndex);

                        for (int slot = 1; slot <= 98; slot++)
                        {

                            if (ObjectManager.Me.FreeNormalBagSlots <= 10)
                            {
				                Lua.DoString("CloseGuildBankFrame()");
                                Logging.Write("Clear bags for space, Moving to profession");
                                return;
                            }

                            Logging.Write("Checking slot {0}", slot);
                            Logging.Write("Tab: {0}, Slot: {1}.", tabIndex, slot);
                            String ret = String.Empty;
                            try
                            {
                                ret =
                                    Lua.GetReturnVal<String>("return GetGuildBankItemLink(" + tabIndex.ToString() + "," + slot.ToString() + ")", 0);
                                Logging.Write("Return value: {0}", ret);
                                Thread.Sleep(50);
                            }
                            catch
                            {
                                Logging.Write("{0} returned null", ret);
                                continue;
                                
                            }

                            if (!String.IsNullOrEmpty(ret))
                            {
                                String pattern = @"Hitem\:(?<ItemID>[0-9]+?)\:";
                                Regex rgx = new Regex(pattern);
                                Match match = rgx.Match(ret);

                                if (!a.PulsesettingsLoad2())
                                {
                                    a.PulsesettingsLoad2();
                                    Logging.Write("Guild Settings Loaded");
                                }

                                if (a.Herbs)
                                {
                                    if (gl.Herbs.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Ores)
                                {
                                    if (gl.Ores.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Gems)
                                {
                                    if (gl.Gems.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Inks)
                                {
                                    if (gl.Pigments.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Leathers)
                                {
                                    if (gl.Leather.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Lockboxes)
                                {
                                    if (gl.Lockboxes.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Eternals)
                                {
                                    if (gl.Eternals.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Fortunemats)
                                {
                                    if (gl.Fortunemats.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                                if (a.Darkmoonmats)
                                {
                                    if (gl.Darkmoonmats.Contains(uint.Parse(match.Groups["ItemID"].Value)))
                                    {
                                        Lua.DoString("AutoStoreGuildBankItem(" + tabIndex.ToString() + "," + slot.ToString() + ")");
                                        Thread.Sleep(250);
                                    }
                                }
                            }
                            if (tabIndex == 6 && slot == 98)
                            {
                                Logging.Write(Color.Red, "Guild Withdraw Run Complete");
                                Lua.DoString("CloseGuildBankFrame()");
                                return;
                            }
                        
                        }
                    }
                }
            }
        }
    }
}


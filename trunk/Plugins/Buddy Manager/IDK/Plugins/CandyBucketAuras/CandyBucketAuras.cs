using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;
using Styx.Plugins.PluginClass;
using Styx.Combat.CombatRoutine;
using Styx.Logic;
using System.Windows.Forms;




namespace CandyBucketAuras
{
    class CandyBucketAuras : HBPlugin
    {
        /*Change the following only!*/
        private string userlanguage = "English";
        /* Available languages:
         * "Deutsch"
         * "Espanol"
         * "Francias"
         * "Russian"
         * 
         * If your WoW client needs a different Macro, change the following so it works*/
        private string cancelaura = "/cancelaura";

        /* DO NOT CHANGE ANYTHING BELOW THIS! */




        private string[] costumes = {"Bat Costume",
                                    "Wisp Costume",
                                    "Ghost Costume",
                                    "Leper Gnome Costume",
                                    "Ninja Costume",
                                    "Pirate Costume",
                                    "Skeleton Costume" };
        private string[] costumes2 = {"Fledermauskostüm",
                                    "Irrwischkostüm",
                                    "Geistkostüm",
                                    "Lepragnomkostüm",
                                    "Ninjakostüm",
                                    "Piratenkostüm",
                                    "Skelettkostüm" };
        private string[] costumes3 = {"Traje de murciélago",
                                    "Traje de fuego fatuo",
                                    "Traje de fantasma",
                                    "Traje de gnomo paria",
                                    "Traje de ninja",
                                    "Traje de pirata",
                                    "Traje de esqueleto" };
        private string[] costumes4 = {"Costume de chauve-souris",
                                    "Costume de feu follet",
                                    "Costume de fantôme",
                                    "Costume de gnome lépreux",
                                    "Costume de ninja",
                                    "Costume de pirate",
                                    "Costume de squelette" };
        private string[] costumes5 = {"Костюм летучей мыши",
                                    "Костюм огонька",
                                    "Костюм призрака",
                                    "Костюм лепрогнома",
                                    "Костюм ниндзя",
                                    "Костюм пирата",
                                    "Костюм скелета" };
        
        public override string Name { get { return "CandyBucketAuras"; } }
        public override string Author { get { return "No1KnowsY"; } }
        public override Version Version { get { return new Version(1, 0); } }
        public LocalPlayer Me { get { return ObjectManager.Me; } }


        public override bool WantButton { get { return true; } }

        public override void OnButtonPress()
        {
        }

        public void Log(string argument, string argument2)
        {
            if (argument2 != null)
            {
                Logging.Write(System.Drawing.Color.Orange, "[{0}]: {1} {2}", Name, argument, argument2);
            }
            else { Logging.Write(System.Drawing.Color.Orange, "[{0}]: {1}", Name, argument); }
        }

        public override void Initialize()
        {
            Log("Active: Will Cancel Trick/Treat Auras!", null);
        }

        Random rnd = new Random();
        public int ranNum(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public override void Pulse()
        {
            if(userlanguage == "English")
            {
                for (int i = 0; i < 7; i++)
                {
                    if(Me.Auras.ContainsKey(costumes[i]))
                    {
                    string macrotxt = cancelaura + " " + costumes[i];
                    Lua.DoString(string.Format("RunMacroText(\"{0}\")", macrotxt));
                    Log("Cancelled Aura: ", costumes[i]);
                    }
                }
            }

            if (userlanguage == "Deutsch")
            {
                for (int i = 0; i < 7; i++)
                {
                    if (Me.Auras.ContainsKey(costumes2[i]))
                    {
                        string macrotxt = cancelaura + " " + costumes2[i];
                        Lua.DoString(string.Format("RunMacroText(\"{0}\")", macrotxt));
                        Log("Cancelled Aura: ", costumes[i]);
                    }
                }
            }

            if (userlanguage == "Espanol")
            {
                for (int i = 0; i < 7; i++)
                {
                    if (Me.Auras.ContainsKey(costumes3[i]))
                    {
                        string macrotxt = cancelaura + " " + costumes3[i];
                        Lua.DoString(string.Format("RunMacroText(\"{0}\")", macrotxt));
                        Log("Cancelled Aura: ", costumes[i]);
                    }
                }
            }

            if (userlanguage == "Francias")
            {
                for (int i = 0; i < 7; i++)
                {
                    if (Me.Auras.ContainsKey(costumes4[i]))
                    {
                        string macrotxt = cancelaura + " " + costumes4[i];
                        Lua.DoString(string.Format("RunMacroText(\"{0}\")", macrotxt));
                        Log("Cancelled Aura: ", costumes[i]);
                    }
                }
            }

            if (userlanguage == "Russian")
            {
                for (int i = 0; i < 7; i++)
                {
                    if (Me.Auras.ContainsKey(costumes5[i]))
                    {
                        string macrotxt = cancelaura + " " + costumes5[i];
                        Lua.DoString(string.Format("RunMacroText(\"{0}\")", macrotxt));
                        Log("Cancelled Aura: ", costumes[i]);
                    }
                }
            }
			Thread.Sleep(ranNum(25,50));

        }
    }
}



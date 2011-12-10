

using System.Drawing;

namespace AzeniusHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Styx;
    using Styx.Combat;
    using Styx.Combat.CombatRoutine;
    using Styx.Database;
    using Styx.Helpers;
    using Styx.Loaders;
    using Styx.Logic;
    using Styx.Logic.AreaManagement;
    using Styx.Logic.AreaManagement.Triangulation;
    using Styx.Logic.BehaviorTree;
    using Styx.Logic.Combat;
    using Styx.Logic.Common;
    using Styx.Logic.Inventory;
    using Styx.Logic.Inventory.Frames;
    using Styx.Logic.Inventory.Frames.Gossip;
    using Styx.Logic.Inventory.Frames.LootFrame;
    using Styx.Logic.Inventory.Frames.MailBox;
    using Styx.Logic.Inventory.Frames.Merchant;
    using Styx.Logic.Inventory.Frames.Quest;
    using Styx.Logic.Inventory.Frames.Taxi;
    using Styx.Logic.Inventory.Frames.Trainer;
    using Styx.Logic.Pathing;
    using Styx.Logic.Pathing.OnDemandDownloading;
    using Styx.Logic.POI;
    using Styx.Logic.Profiles;
    using Styx.Logic.Profiles.Quest;
    using Styx.Logic.Questing;
    using Styx.Patchables;
    using Styx.Plugins;
    using Styx.Plugins.PluginClass;
    using Styx.WoWInternals;
    using Styx.WoWInternals.Misc;
    using Styx.WoWInternals.Misc.DBC;
    using Styx.WoWInternals.World;
    using Styx.WoWInternals.WoWCache;
    using Styx.WoWInternals.WoWObjects;
    using System.Diagnostics;

    public class AzeniusHelper : HBPlugin
    {

        #region Boring Plugin Info

        public override string Name { get { return "Azenius Helper"; } }
        public override string Author { get { return "Deadly, Shak & Megser"; } }
        public override Version Version { get { return new Version(1,0,1); } }
        public override bool WantButton { get { return true; } }
        public override string ButtonText { get { return "No Config"; } }

        #endregion

        #region Logging function (Adapted from example by CodenameG)

        public static void hlog(string format, params object[] args)
        {
            Logging.Write(System.Drawing.Color.Teal, "[AzeniusHelper]: " + format, args);
        }

        #endregion


        private static readonly LocalPlayer Me = ObjectManager.Me;

        private static Stopwatch GlobalTimer = new Stopwatch();

        #region List of WoWUnits and WoWGameObjects needed

        public List<WoWUnit> Yenniku
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 2530 && u.CurrentTargetGuid == Me.Guid && !u.Dead).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> AdrineTowhide
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => u.Entry == 44456)
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> RessGoblin
        {
            // NPC - Doc Zapnozzle
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(ret => (ret.Entry == 36608)).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWGameObject> q24817controller
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(ret => (ret.Entry == 202108 && !Me.Dead)).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWUnit> q24817_hammer
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(ret => (ret.Entry == 36682 && !Me.Dead)).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWUnit> q24817_vehicle
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(ret => (ret.Entry == 38318 && !Me.Dead)).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWUnit> q24958_Giant_Turtle
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(ret => (ret.Entry == 38855 && !Me.Dead)).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWUnit> StickboneBerserker
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 44329 && u.Distance < 5).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWGameObject> ScourgeBoneAnimus
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(u => u.Entry == 204966 && !Me.Dead && u.Distance < 10).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> StonevaultRuffian
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 46711 && !u.Dead && u.Distance < 200).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> StonevaultGoon
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 46712 && !u.Dead && u.Distance < 200).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> WardensPawn
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(ret => (ret.Entry == 46344 && !Me.Dead)).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWUnit> Kalaran
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 46859 && !u.Dead && u.Distance < 10).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> Moldarr
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 46938 && !u.Dead && u.Distance < 8).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> Jirakka
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 46860 && !u.Dead && u.Distance < 8).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> Nyxondra
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 46861 && !u.Dead && u.Distance < 25).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWUnit> BloodSailCorsair
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 43726 && !u.Dead && u.Distance < 25).OrderBy(u => u.Distance).ToList();
            }
        }
        public List<WoWGameObject> GatewayShaadraz
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(u => u.Entry == 183351 && !Me.Dead).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWGameObject> GatewayMurketh
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(u => u.Entry == 183350 && !Me.Dead).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWUnit> MoargOverseer
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 19397 && !Me.Dead).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWUnit> GanArgPeon
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 19398 && !Me.Dead).OrderBy(ret => ret.Distance).ToList();
            }
        }
        public List<WoWGameObject> FelCannon
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(u => u.Entry == 19399 && !Me.Dead).OrderBy(ret => ret.Distance).ToList();
            }
        }
        #endregion

        #region Some Quest Helper Functions
        
        public bool IsOnQuest(uint questId)
        {
            return Me.QuestLog.GetQuestById(questId) != null && !Me.QuestLog.GetQuestById(questId).IsCompleted;
        }

        public bool QuestComplete(uint questId)
        {
            return Me.QuestLog.GetQuestById(questId).IsCompleted;
        }
        

        public bool QuestFailed(uint questId)
        {
            return Me.QuestLog.GetQuestById(questId).IsFailed;
        }

        public bool ItemOnCooldown(uint ItemId)
        {
            return Lua.GetReturnVal<bool>("GetItemCooldown(" + ItemId + ")", 0);

        }

        public void UseQuestItem(uint ItemId)
        {
            Lua.DoString("UseItemByName(" + ItemId + ")");
        }

        public void UseIfNotOnCooldown(uint ItemId)
        {
            if (!ItemOnCooldown(ItemId))
            {
                UseQuestItem(ItemId);
            }
        }

        public bool QuestObjectiveComplete(uint questId, int objectiveNum) {
            return (Lua.GetReturnVal<int>("a,b,c=GetQuestLogLeaderBoard(" + objectiveNum + ",GetQuestLogIndexByID(" + questId + "));if c==1 then return 1 else return 0 end", 0) == 1);
        }

        public bool TargetingNpc(uint npcId) {
            return Me.CurrentTarget.Entry == npcId;
        }

        public bool HasQuest(uint questId) {
            return Me.QuestLog.GetQuestById(questId) != null;
        }

        #endregion

        public AzeniusHelper()
		{
            hlog("Loaded and ready to work!");
		}

        public override void Pulse()
        {

            #region Disable if Alliance
            
            /*if (Me.IsAlliance) { 
                hlog("This plugin is not meant for Alliance (yet!) Disabling.");
                PluginContainer plugin = PluginManager.Plugins.FirstOrDefault(p => p.Name.ToLower().Contains("azenius".ToLower()));

			    if(plugin != null) {
                    plugin.Enabled =! plugin.Enabled;
                    hlog("Successfuly disabled. Goodbye!");
                } else {
                    hlog("Unable to disable plugin. Weird!");
                }
		    }
             */

            #endregion

            #region Quest 14239 - Don't Go Into the Light!
            
            if (Me.Race == WoWRace.Goblin && Me.HasAura("Near Death!") && Me.ZoneId == 4720 && RessGoblin.Count > 0)
            {
                // RessGoblin = Doc Zapnozzle
                RessGoblin[0].Interact();
                Thread.Sleep(1000);
                Lua.DoString("RunMacroText('/click QuestFrameCompleteQuestButton')");
            }

            #endregion

            #region Quest 6544 - Torek's Assault

            if (IsOnQuest(6544) && QuestFailed(6544))
            {
                Me.QuestLog.AbandonQuestById(6544);
            }

            #endregion

            #region Quest 6641 - Vorsha the Lasher
            
            // Bot has issues pulling the target. Force it to pull everything
            
            if (IsOnQuest(6641))
            {
                if (Me.CurrentTarget == null)
                    return;
                if (Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid != Me.Guid)
                {
                    RoutineManager.Current.Pull();
                }
            }

            #endregion

            #region Quest 13980 - They're Out There!
            
            // Keep Jinx's Goggles (quest item) on during the entirety of the quest
            
            if (IsOnQuest(13980) && (Me.MinimapZoneText == "The Skunkworks" || Me.MinimapZoneText == "Talondeep Vale") && !Me.HasAura("Jinx's Elf Detection Ray"))
            {
                Lua.DoString("UseItemByName(46776)"); // Jinx's Goggles
                Thread.Sleep(500);
            }

            #endregion

            #region Quest 14236 - Weed Whacker *

            // Activate the Weed Whacker and cycle through points until the quest is complete
            // @TODO - Do we need to disable this aura once the quest is complete?

            if (!Me.Dead && IsOnQuest(14236))
            {
                WoWPoint wpWeed1 = new WoWPoint(638.7761, 2780.211, 88.81393);
                WoWPoint wpWeed2 = new WoWPoint(634.825, 2824.758, 87.50606);
                WoWPoint wpWeed3 = new WoWPoint(684.2277, 2821.671, 86.48402);
                WoWPoint wpWeed4 = new WoWPoint(646.324, 2859.586, 87.25509);
                WoWPoint wpWeed5 = new WoWPoint(700.0909, 2848.549, 84.93351);
                WoWPoint wpWeed6 = new WoWPoint(610.5126, 2908.886, 91.3634);
                WoWPoint wpWeed7 = new WoWPoint(574.0838, 2886.616, 90.26514);
                WoWPoint wpWeed8 = new WoWPoint(582.1985, 2797.607, 88.356);
                WoWPoint wpWeed9 = new WoWPoint(602.7809, 2784.686, 88.45428);

                while (!Me.HasAura("Weed Whacker"))
                {
                    Lua.DoString("UseItemByName(49108)"); // Weed Whacker
                    Thread.Sleep(1000);
                }

                while (!Me.QuestLog.GetQuestById(14236).IsCompleted)
                {
                    WoWMovement.ClickToMove(wpWeed1);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed2);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed3);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed4);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed5);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed6);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed7);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed8);
                    Thread.Sleep(20000);
                    WoWMovement.ClickToMove(wpWeed9);
                    Thread.Sleep(20000);
                }
            }

            #endregion

            #region Quest 24958 - Volcanoth!

            if (IsOnQuest(24958) && !Me.Dead)
            {
                
                WoWPoint wpTurtle = new WoWPoint(1305.009, 1183.095, 121.1527);
                
                while (Me.Location.Distance(wpTurtle) > 5)
                {
                    Navigator.MoveTo(wpTurtle);
                    Thread.Sleep(500);
                }

                if (q24958_Giant_Turtle.Count != 0)
                {
                    q24958_Giant_Turtle[0].Target();
                    q24958_Giant_Turtle[0].Face();
                }

                while (IsOnQuest(24958) && Me.CurrentTarget != null && Me.CurrentTarget.IsAlive)
                {
                    WoWMovement.MoveStop();
                    Thread.Sleep(100);
                    Lua.DoString("UseItemByName(52043)"); // Bootzooka
                    Thread.Sleep(100);
                }

            }

            #endregion

            #region Quest 13961 - Drag it Out of Them

            if (IsOnQuest(13961))
            {
                if (Me.CurrentTargetGuid != 0 && Me.CurrentTarget.Name == "Razormane Pillager" && !Me.HasAura("Dragging a Razormane"))
                {
                    // Attempt to throw the net to pacify it
                    while (!Me.CurrentTarget.IsFriendly)
                    {
                        Lua.DoString("UseItemByName(46722)"); // Grol'dom Net
                        Thread.Sleep(500);
                    }

                    // Pillager has been caught.. Move towards it
                    // This conditional is theoretically unnecessary...
                    if (Me.CurrentTarget.IsFriendly)
                    {
                        while (Me.CurrentTarget.Distance > 5)
                        {
                            Navigator.MoveTo(Me.CurrentTarget.Location);
                            Thread.Sleep(100);
                        }
                        Me.CurrentTarget.Interact();
                        Thread.Sleep(500);
                        Lua.DoString("SelectGossipOption(1)");
                        Thread.Sleep(500);
                    }

                }
            }
            #endregion

            #region Quest 25165 - Never Trust a Big Barb and a Smile

            if (IsOnQuest(25165) && !Me.HasAura("Poison Extraction Totem"))
            {
                bool IsOnCD = Lua.GetReturnVal<bool>("GetItemCooldown(52505)", 0); // Object has a 15 second cooldown.. Rather long.
                if (!IsOnCD)
                {
                    Lua.DoString("UseItemByName(52505)"); // Poison Extraction Totem
                }
            }

            #endregion

            #region Lashtail Hatchling Quests - 26321, 26325
            
            if (IsOnQuest(26321) || IsOnQuest(23625))
            {
                bool IsOnCD = Lua.GetReturnVal<bool>("GetItemCooldown(58165)", 0);
                if (!IsOnCD && !Me.HasAura("A Lashtail Hatchling: Hatchling Guardian Aura"))
                {
                    Lua.DoString("UseItemByName(58165)");
                }
            }

            #endregion

            #region Quest 26953 - Zen'Kiki, the Druid

            if (IsOnQuest(26953) && !Me.HasAura("Zen'Kiki Guardian Aura"))
            {
                if (AdrineTowhide.Count > 0 && AdrineTowhide[0].Distance < 5)
                {
                    AdrineTowhide[0].Interact();
                    Thread.Sleep(1000);
                    Lua.DoString("SelectGossipOption(1)");
                }
                else if (AdrineTowhide.Count > 0 && AdrineTowhide[0].Distance > 5)
                {
                    while (AdrineTowhide[0].Distance > 5)
                    {
                        Navigator.MoveTo(AdrineTowhide[0].Location);
                        Thread.Sleep(100);
                    }
                }
                else if (AdrineTowhide.Count < 1)
                {
                    WoWPoint TowHide = new WoWPoint(1796.26, -1684.78, 60.1698);
                    while (Me.Location.Distance(TowHide) > 5)
                    {
                        Navigator.MoveTo(TowHide);
                        Thread.Sleep(100);
                    }
                }
            }

            #endregion

            #region Quest 26925 - Araj the Summoner

            if (IsOnQuest(26925) && (StickboneBerserker.Count >= 1))
            {
                if (!ItemOnCooldown(60678)) // Jearl's Hand Grenades
                {
                    UseQuestItem(60678);
                    LegacySpellManager.ClickRemoteLocation(StickboneBerserker[0].Location);
                }
            }
            #endregion

            #region Quest 26648 - Our Mortal Enemies

            if (IsOnQuest(26648))
            {
                if (!ItemOnCooldown(59226) && !Me.HasAura("Dead Eye's Intuition"))
                {
                    UseQuestItem(59226); // Dead-Eye's Flare Gun
                }
            }

            #endregion

            #region Quest 14238 - Infrared = Infradead

            if (IsOnQuest(14238) && !Me.HasAura("Infrared Heat Focals"))
            {
                UseIfNotOnCooldown(49611); // Infrared Heat Focals
            }

            #endregion

            #region Quest 27789 - Troggish Troubles *

            // @TODO - Document the Pet Actions here

            if (IsOnQuest(27789))
            {
                while (IsOnQuest(27789))
                {
                    StonevaultRuffian[0].Target();
                    WoWMovement.ConstantFace(Me.CurrentTarget.Guid);
                    Lua.DoString("CastPetAction(1)");
                    StonevaultGoon[0].Target();
                    WoWMovement.ConstantFace(Me.CurrentTarget.Guid);
                    Lua.DoString("CastPetAction(1)");
                    Lua.DoString("CastPetAction(2)");
                }
            }

            #endregion

            #region Quest 27771 - Third Sample: Implanted Eggs
            
            // Need to loot eggs which take 10 seconds

            if (IsOnQuest(27771))
            {
                while (IsOnQuest(27771))
                {
                    Lua.DoString("RunMacroText('/cleartarget')");
                    GlobalTimer.Start();
                    if (GlobalTimer.Elapsed.Seconds > 12)
                        return;
                }
            }

            #endregion

            #region Quest 27885 - The Warden's Game

            if (IsOnQuest(27885))
            {
                WoWPoint wpPawn1 = new WoWPoint(-6970.479, -3439.854, 200.8959);
                WoWPoint wpPawn2 = new WoWPoint(-6968.06, -3440.255, 200.8969);
                WoWPoint wpPawn3 = new WoWPoint(-6964.444, -3440.112, 200.8969);
                WoWPoint wpPawn4 = new WoWPoint(-6961.984, -3439.921, 200.8963);
                WoWPoint wpPawn5 = new WoWPoint(-6959.851, -3445.163, 201.2538);
                WoWPoint wpPawn6 = new WoWPoint(-6959.738, -3447.433, 201.6079);
                WoWPoint wpPawn7 = new WoWPoint(-6964.568, -3450.362, 200.8965);
                WoWPoint wpPawn8 = new WoWPoint(-6966.959, -3450.602, 200.8965);
                WoWPoint wpPawn9 = new WoWPoint(-6969.584, -3445.054, 200.8965);
                WoWPoint wpPawn10 = new WoWPoint(-6969.789, -3442.688, 200.8965);
                WoWPoint wpPawn11 = new WoWPoint(-6964.177, -3440.75, 200.8958);
                WoWPoint wpPawn12 = new WoWPoint(-6961.631, -3440.965, 200.8958);
                WoWPoint wpPawn13 = new WoWPoint(-6960.158, -3445.682, 200.8958);
                WoWPoint wpPawn14 = new WoWPoint(-6960.385, -3447.647, 200.8958);
                WoWPoint wpPawn15 = new WoWPoint(-6964.737, -3449.662, 200.8958);
                WoWPoint wpPawn16 = new WoWPoint(-6967, -3449.534, 200.8955);
                WoWPoint wpPawn17 = new WoWPoint(-6968.355, -3445.081, 200.8955);
                WoWPoint wpPawn18 = new WoWPoint(-6968.267, -3442.952, 200.8955);
                WoWPoint wpPawn19 = new WoWPoint(-6964.177, -3440.75, 200.8958);
                WoWPoint wpPawn20 = new WoWPoint(-6961.631, -3440.965, 200.8958);
                WoWPoint wpPawn21 = new WoWPoint(-6960.158, -3445.682, 200.8958);
                WoWPoint wpPawn22 = new WoWPoint(-6960.385, -3447.647, 200.8958);
                WoWPoint wpPawn23 = new WoWPoint(-6964.737, -3449.662, 200.8958);
                WoWPoint wpPawn24 = new WoWPoint(-6967, -3449.534, 200.8955);
                WoWPoint wpPawn25 = new WoWPoint(-6968.355, -3445.081, 200.8955);
                WoWPoint wpPawn26 = new WoWPoint(-6968.267, -3442.952, 200.8955);
                WoWPoint wpPawn27 = new WoWPoint(-6964.177, -3440.75, 200.8958);
                WoWPoint wpPawn28 = new WoWPoint(-6961.631, -3440.965, 200.8958);
                WoWPoint wpPawn29 = new WoWPoint(-6960.158, -3445.682, 200.8958);
                WoWPoint wpPawn30 = new WoWPoint(-6961.002, -3447.482, 200.8966);
                WoWPoint wpPawn31 = new WoWPoint(-6964.568, -3445.147, 200.8966);

                while (!Me.QuestLog.GetQuestById(27885).IsCompleted)
                {
                    WoWMovement.ClickToMove(wpPawn1);
                    Thread.Sleep(3000);
                    WoWMovement.ClickToMove(wpPawn2);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn3);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn4);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn5);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn6);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn7);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn8);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn9);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn10);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn11);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn12);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn13);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn14);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn15);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn16);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn17);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn18);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn19);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn20);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn21);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn22);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn23);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn24);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn25);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn26);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn27);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn28);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn29);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn30);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                    WoWMovement.ClickToMove(wpPawn31);
                    Thread.Sleep(1000);
                    WardensPawn[0].Target();
                    WardensPawn[0].Interact();
                    Thread.Sleep(1000);
                }
            }

            #endregion

            #region Quest 27893 - Gargal, the Behemot

            if (IsOnQuest(27893))
            {
                WoWPoint wpDarkflight1 = new WoWPoint(-6730.564, -2448.625, 272.7784);
                WoWPoint wpDarkflight2 = new WoWPoint(-6805.746, -2435.204, 272.7776);

                while (!QuestComplete(27893))
                {
                    WoWMovement.ClickToMove(wpDarkflight1);
                    Thread.Sleep(10000);
                    WoWMovement.ClickToMove(wpDarkflight2);
                    Thread.Sleep(13000);
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(1)");
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(4)");
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(8)");
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(2)");
                    Thread.Sleep(5000);
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(5)");
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(7)");
                    Thread.Sleep(5000);
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(1)");
                    Thread.Sleep(5000);
                    Lua.DoString("RunMacroText('/target Darkflight Flameblade')");
                    Lua.DoString("CastPetAction(1)");
                }
            }

            #endregion

            #region Quest 27894 - The Wrath of a Dragonflight

            if (IsOnQuest(27894))
            {
                while (!QuestComplete(27894) && (Kalaran.Count >= 1))
                {
                    Lua.DoString("CastPetAction(1)");
                    Lua.DoString("CastPetAction(4)");
                    Lua.DoString("CastPetAction(8)");
                    Lua.DoString("CastPetAction(2)");
                    Lua.DoString("CastPetAction(5)");
                    Lua.DoString("CastPetAction(7)");
                    GlobalTimer.Start();
                    if (GlobalTimer.Elapsed.Seconds > 12)
                        return;
                }
            }
            #endregion

            #region Quest 27895 - Their Hunt Continues

            if (IsOnQuest(27895))
            {
                while (!QuestComplete(27895) && (Moldarr.Count >= 1))
                {
                    Lua.DoString("CastPetAction(1)");
                    Lua.DoString("CastPetAction(4)");
                    Lua.DoString("CastPetAction(8)");
                    Lua.DoString("CastPetAction(2)");
                    Lua.DoString("CastPetAction(5)");
                    Lua.DoString("CastPetAction(7)");
                    Lua.DoString("AttackTarget()");
                    GlobalTimer.Start();
                    if (GlobalTimer.Elapsed.Seconds > 12)
                        return;
                }
            }

            #endregion

            #region Quest 27896 - The Sorrow and the Fury

            if (IsOnQuest(27896))
            {
                while (!QuestComplete(27896) && (Nyxondra.Count >= 1))
                {
                    Lua.DoString("RunMacroText('/target Nyxondra')");
                    Lua.DoString("CastPetAction(1)");
                    Lua.DoString("CastPetAction(4)");
                    Lua.DoString("CastPetAction(8)");
                    Lua.DoString("CastPetAction(2)");
                    Lua.DoString("CastPetAction(5)");
                    Lua.DoString("CastPetAction(7)");
                    GlobalTimer.Start();
                    if (GlobalTimer.Elapsed.Seconds > 12)
                        return;
                }
            }

            #endregion

            #region Quest 28226 - Scrapped Golems

            if (IsOnQuest(28226))
            {

                if (GossipFrame.Instance.IsVisible)
                {
                    // If we have two options it means that the Stone Power Core dropped (rare) so select it!
                    if (!QuestObjectiveComplete(28226,4) && GossipFrame.Instance.GossipOptionEntries.Count > 1)
                    {
                        Lua.DoString("SelectGossipOption(2)");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Lua.DoString("SelectGossipOption(1)");
                        Thread.Sleep(1000);
                    }
                }
            }

            #endregion

            #region Quest 26922 -The Endless Flow *

            // @TODO - Find out why this doesn't check for quest completion

            if (Me.QuestLog.GetQuestById(26922) != null)
            {
                if (StickboneBerserker.Count > 0) // Near StickboneBerserker
                {
                    StickboneBerserker[0].Target();
                    Thread.Sleep(500);
                    UseQuestItem(60678); // Jearl's Hand Grenades
                    Thread.Sleep(500);
                    LegacySpellManager.ClickRemoteLocation(StickboneBerserker[0].Location);
                    Thread.Sleep(1000);
                }

                if (ScourgeBoneAnimus.Count > 0)
                {
                    UseQuestItem(60678); // Jearl's Hand Grenades
                    Thread.Sleep(500);
                    LegacySpellManager.ClickRemoteLocation(ScourgeBoneAnimus[0].Location);
                    Thread.Sleep(1000);
                }
            }

            #endregion

            #region Quest 10129 - Mission: Gateways Murketh and Shaadraz

            // Use Grenade on Murkey and Shaadraz

            if (IsOnQuest(10129))
            {
                if (!ItemOnCooldown(28038))
                {
                    UseQuestItem(28038); // Seaforium PU-36 Explosive Nether Modulator
                    LegacySpellManager.ClickRemoteLocation(GatewayShaadraz[0].Location);
                    UseQuestItem(28038);
                    LegacySpellManager.ClickRemoteLocation(GatewayMurketh[0].Location);
                }
            }

            #endregion

            #region Quest 10162 - Mission: The Abyssal Shelf

            if (IsOnQuest(10162))
            {
                if (!ItemOnCooldown(28132)) // Area 52 Special
                {
                    if (!QuestObjectiveComplete(10162,1))
                    {
                        UseQuestItem(28132);
                        LegacySpellManager.ClickRemoteLocation(MoargOverseer[0].Location);
                    }
                    if (!QuestObjectiveComplete(10162,2))
                    {
                        UseQuestItem(28132);
                        LegacySpellManager.ClickRemoteLocation(GanArgPeon[0].Location);
                    }
                    if (!QuestObjectiveComplete(10162,3))
                    {
                        UseQuestItem(28132);
                        LegacySpellManager.ClickRemoteLocation(FelCannon[0].Location);
                    }
                }
            }

            #endregion

            #region Quest 26305 - Saving Yenniku

            if (IsOnQuest(26305))
            {
                if (TargetingNpc(2530)) // Yenniku
                {
                    UseQuestItem(3912); // Soul Gem
                }
            }

            #endregion

            #region Disguise Quests in Burning Steppes 28439, 28440, 28432, 28433, 28434, 28435
            // 28439 = I Am the Law and I Am the Lash
            // 28440 = Abuse of Power
            // 28432 = Into the Black Tooth Hovel
            // 28433 = Grunt Work
            // 28434 = Strategic Cuts
            // 28435 = The Kodocaller's Horn

            if (HasQuest(28439) || HasQuest(28440) || HasQuest(28432) || HasQuest(28433) || HasQuest(28434) || HasQuest(28435))
            {
                bool disguiseFound = false;
                foreach (WoWAura s in ObjectManager.Me.ActiveAuras.Values)
                {
                    if (s.Name.Contains("Disguise"))
                    {
                        disguiseFound = true;
                    }
                }

                if (!disguiseFound)
                {
                    UseQuestItem(63357);
                    Thread.Sleep(6000);
                    }
            }

            #endregion

        }
    }
}


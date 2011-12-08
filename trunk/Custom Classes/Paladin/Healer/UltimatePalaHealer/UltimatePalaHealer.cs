 // Custom Class for Paladins Healing in a PVP and PVE environment, from BGs to Raids
 // A collaboration of work with Sm0k3d and Glideroy
 // And a special thanks to all others who helped!

using System;
using System.Diagnostics;
using System.Drawing;
using Styx.Combat.CombatRoutine;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;


namespace UltiumatePalaHealer
{
    partial class UltimatePalaHealer
    {

        private WoWUnit lastCast=null;
        private WoWPlayer fallbacktank=null;
        private WoWPlayer tank;
        private WoWUnit Enemy;
        private Random rng;
        private WoWPlayer x;
        private WoWPlayer tar;
        private WoWPlayer mtank;
        private WoWUnit Epet;
        //From here for the gui
            private int Judgment_range = 0; //for me..
            private bool PVE_want_urgent_cleanse = true;    //Do you want to cleanse critical stuff like Static Cling or Polymorph? (used)
            private bool PVP_want_urgent_cleanse = true;    //Do you want to cleanse critical stuff like Static Cling or Polymorph? (used)
            private bool PVE_want_cleanse = true;           //Do you want to cleanse debuff?
            private bool PVP_want_cleanse = true;           //Do you want to cleanse debuff?
            private int PVE_max_healing_distance = 40;      //Ignore people more far away
            private int PVE_ohshitbutton_activator = 40;    //Percentage to click 1 OhShitButton!
            private int PVP_ohshitbutton_activator = 40;    //Percentage to click 1 OhShitButton!
            private bool PVE_wanna_LoH = true;              //wanna LoH?
            private int PVE_min_LoH_hp=15;                  //LoH will be cast on target below this
            private bool PVE_wanna_HoP = true;              //wanna HoP?
            private int PVE_min_HoP_hp = 25;                //HoP will be cast on target below this that are not the tank
            private bool PVE_wanna_HoS = true;              //wanna HoS?
            private int PVE_min_HoS_hp = 65;                //HoS will be cast on the tank if he drop below this
            private bool PVE_want_HR = true;                //wanna HR at all?
            private int PVE_min_player_inside_HR = 3;       //HR will be cast when we have that many player that need heals inside it
            private bool PVE_Inf_of_light_wanna_DL = true;  //when we have infusion of light do we wanna DL? if false will HL instead
            private int PVE_Inf_of_light_min_DL_hp = 70;    //max HP to cast divine light with infusion of light buff
            private int PVE_min_FoL_hp = 35;                //will cast FoL un unit below that value
            private int PVE_min_DL_hp = 70;                 //same, with DL
            private int PVE_min_HL_hp = 85;                 //same, with HL
            private bool PVE_wanna_DF = true;               //do we want to use Divine favor?
            private bool PVE_wanna_AW = true;               //do we want to use Awenging Wrath?
            private bool PVE_wanna_GotAK = true;            //do we want to use Guardian of ancient king?
            private int PVE_do_not_heal_above = 95;         //at how much health we ignore people
            private int rest_if_mana_below = 60;            //if mana is this low and out of combat then drink
            private int use_mana_rec_trinket_every = 60;    //use the trinket to rec mana every 60 sec (for now only support Tyrande's Favorite Doll couse i have it :P)
            private int use_mana_rec_trinket_on_mana_below = 40;  //will use the trinket only if mana is below that
            private bool use_mana_potion = true;            //will use a mana potion? (not yet implemented)
            private int use_mana_potion_below = 20;         //% of mana where to use the potion
            private bool PVE_wanna_DP = true;               //do we wanna use Divine Protection?
            private int PVE_DP_min_hp = 85;                 //max hp to use divine protection at (will use on lower hp)
            private bool PVE_wanna_DS= true;                      //wanna use Divine Shield?
            private int PVE_DS_min_hp = 35;                 //at witch hp wanna use Divine Shield?
            private bool PVE_wanna_everymanforhimself = true; //wanna use Every Man For Himself?
            private bool PVP_wanna_DP = true;
            private int PVP_DP_min_hp = 85;
            private bool PVP_wanna_DS = true;
            private int PVP_DS_min_hp = 50;
            private bool PVP_wanna_everymanforhimself = true;
            private bool PVE_wanna_Judge = true;
            private bool PVP_wanna_Judge = true;
            private int PVE_min_Divine_Plea_mana = 70;
            private int PVP_min_Divine_Plea_mana = 70;
            private bool PVE_wanna_HoW = true;
            private bool PVP_wanna_HoW = true;
            private bool PVE_wanna_Denunce = true;
            private bool PVP_wanna_Denunce = true;
            private bool PVE_wanna_CS = true;
            private bool PVP_wanna_CS = true;
            private bool PVE_wanna_buff = true;
            private bool PVP_wanna_buff = true;
            private bool PVE_wanna_mount = true;
            private bool PVP_wanna_mount = true;
            private bool PVE_wanna_HoJ = false;
            private bool PVE_wanna_rebuke = false;
            private bool PVE_wanna_move_to_HoJ = false;
            private bool PVP_wanna_HoJ = true;
            private bool PVP_wanna_rebuke = true;
            private bool PVP_wanna_move_to_HoJ = false;
            private bool ARENA_wanna_move_to_HoJ = true;
            private bool PVP_want_HR = true;
            private bool PVP_wanna_move_to_heal = false;
            private bool ARENA_wanna_move_to_heal = true;
            private bool PVE_wanna_move_to_heal = false;
            private bool PVP_wanna_LoH = true;              //wanna LoH?
            private int PVP_min_LoH_hp = 15;                  //LoH will be cast on target below this
            private bool PVP_wanna_HoP = true;              //wanna HoP?
            private int PVP_min_HoP_hp = 25;                //HoP will be cast on target below this that are not the tank
            private bool PVP_wanna_HoS = true;              //wanna HoS?
            private int PVP_min_HoS_hp = 65;                //HoS will be cast on the tank if he drop below this
            private int PVP_min_player_inside_HR = 2;       //HR will be cast when we have that many player that need heals inside it
            private bool PVP_Inf_of_light_wanna_DL = true;
            private int PVP_min_FoL_hp = 70;
            private int PVP_min_FoL_on_tank_hp = 80;
            private int PVP_min_HL_hp = 85;
            private bool PVP_wanna_DF = true;
            private bool PVP_wanna_AW = true;
            private bool PVP_wanna_GotAK = true;
            private int PVP_do_not_heal_above = 95;
            private bool PVP_wanna_HoF = true;
            private bool PVE_wanna_target = true;           //do you want the cc to target something if you do not have any target?
            private bool PVP_wanna_target = true;           //do you want the cc to target something if you do not have any target?
            private bool wanna_face = true;                 //do you want to face enemy when needed?
            private float PVP_min_run_to_HoF = 5.05f;         //if target speed drop below this, HoF
            private float PVE_min_run_to_HoF = 5.05f;         //if target speed drop below this, HoF
            private bool PVE_wanna_HoF = false;             //wanna HoF in PVE
            private int PVE_HR_how_far = 12;
            private int PVP_HR_how_far = 20;
            private int PVE_HR_how_much_health = 70;
            private int PVP_HR_how_much_health = 85;
            private int PVP_mana_judge = 50;                //will start judging on cooldwn at this mana
            private int PVE_mana_judge = 70;
            private bool tank_healer = false;
            private bool debug = false;
            private bool PVP_wanna_crusader = true;         //wanna switch to crusader aura in pvp when mounted?
            private bool PVE_wanna_crusader = false;        //wanna switch to crusader aura in pve when mounted?
            private int last_word = 1;                      //0 1 or 2 point in the talent Last Word
            private bool Solo_wanna_move = true;
            private int Solo_mana_judge = 100;
            private bool Solo_wanna_rebuke = true;
            private bool Solo_wanna_HoJ = true;
            private bool Solo_wanna_move_to_HoJ = true;
            private bool Solo_wanna_crusader = true;
            private bool Solo_wanna_buff = true;
            private bool Solo_wanna_face = true;
            private bool PVE_use_stoneform = true;
            private bool PVP_use_stoneform = true;
            private int stoneform_perc = 80;
            private bool PVP_use_escapeartist = true;
            private bool PVE_use_escapeartist = true;
            private bool PVE_use_gift = true;
            private bool PVP_use_gift = true;
            private int PVE_min_gift_hp = 40;
            private int PVP_min_gift_hp = 40;
            private bool PVE_use_bloodfury = true;
            private bool PVP_use_bloodfury = true;
            private int PVP_bloodfury_min_hp = 40;
            private int PVE_bloodfury_min_hp = 40;
            private bool PVP_use_warstomp = true;
            private bool PVE_use_warstomp = false;
            private int PVP_min_warstomp_hp = 50;
            private int PVE_min_warstomp_hp = 50;
            private bool PVE_use_bersekering = true;
            private bool PVP_use_bersekering = true;
            private int PVE_min_bersekering_hp = 40;
            private int PVP_min_bersekering_hp = 40;
            private bool PVP_use_will_forsaken = true;
            private bool PVE_use_will_forsaken = true;
            private bool PVE_use_torrent = false;
            private bool PVP_use_torrent = true;
            private int PVE_min_torrent_hp = 50;
            private int PVP_min_torrent_hp = 50;
            private bool ARENA_wanna_taunt = true;
            private int PVP_min_DL_hp = 0;
            private int ARENA_min_FoL_hp = 95;
            private int ARENA_min_DL_hp = 0;
            private int ARENA_min_HL_hp = 95;
            private bool chimaeron = false;
            private bool chimaeron_p1 = false;
            private int aura_type = 0; //0 for concentration 1 for resistance


        
        ////////////////////////////////////////////////////////////////////////////////////////////


        
        private bool gottank;
        private bool isinterrumpable = false;
        private string lastBehaviour = null;
        private string actualBehaviour = null;
        private string usedBehaviour = null;
        private double maxAOEhealth = 85;
        private double dontHealAbove = 95;
        private bool castedDL = false;
        private string lastbless =null;
        

        private static Stopwatch sw = new Stopwatch();
        private static Stopwatch combatfrom = new Stopwatch();
        private static Stopwatch noncombatfrom = new Stopwatch();

        private string version = "2.2";
        private string revision = "86";

        public override sealed string Name { get { return "UltimatePalaHealer v " + version + " revision " + revision; } }

        public override WoWClass Class { get { return WoWClass.Paladin; } }


        private static LocalPlayer Me { get { return ObjectManager.Me; } }

        public override void Initialize() 
        {
            slog(Color.Orange, "Hello Executor!\n I\'m UPaHCC and i\'m here to assist you keeping your friend alive\n You are using UPaHCC version {0} revision {1}", version,revision);
            Judgment_range = (int)SpellManager.Spells["Judgement"].MaxRange;
            slog(Color.Orange, "Your Judgment range is {0} yard! will use this value", Judgment_range);
        }       //if you need to run something just once do it here (will put up talent check in here)

        #region Combat

        public override void Combat()
        {
            if (!sw.IsRunning) sw.Start();
            /*
            if (Me == null) { return; }
            if (Me.Dead) { return; }
            if (!Me.IsValid) { return; }
            */
            if (unitcheck(Me) < 0)
            {
                slog(Color.DarkRed, "We are on a loading schreen, or dead, or a ghost, CC is in PAUSE");
                return;
            }

            x = PartyCombat();
            tar = GetHealTarget();

            Behaviour();

            //if (Me != null && Me.IsValid && Me.IsAlive)
            if(unitcheck(Me)==1)
            {
                ObjectManager.Update();
                if (usedBehaviour == "Arena") 
                { 
                    ArenaCombat(); 
                }
                else if (usedBehaviour == "Dungeon") 
                { 
                    DungeonCombat(); 
                }
                else if (usedBehaviour == "Raid") 
                { 
                    RaidCombat(); 
                }
                else if (usedBehaviour == "Party or Raid") 
                { 
                    RafCombat(); 
                }
                else if (usedBehaviour == "Solo") 
                { 
                    SoloCombat(); 
                }
                else if (usedBehaviour == "World PVP") 
                { 
                    WorldPVPCombat(); 
                }
                else if (usedBehaviour == "Battleground")
                { 
                    BattlegroundCombat(); 
                }

            }
            return;
        }
        #endregion

        #region CC_Begin

        #endregion

        #region Pull

        public override void Pull() { }

        #endregion

        #region Pull Buffs

        public override bool NeedPullBuffs { get { Combat(); return false; } }

        public override void PullBuff() { }

        #endregion

        #region Pre Combat Buffs

        public override bool NeedPreCombatBuffs { get { Combat(); return false; } }

        public override void PreCombatBuff() { return; }

        #endregion

        #region Rest

        public override bool NeedRest { get { Combat(); return false; } }

        #endregion

        #region Combat Buffs

        public override bool NeedCombatBuffs { get { Combat(); return false; } }

        public override void CombatBuff() { }

        #endregion

        #region Heal

        public override bool NeedHeal { get { return false; } }

        public override void Heal() { }

        #endregion

        #region Falling

        public void HandleFalling() { }

        #endregion     

    }
}

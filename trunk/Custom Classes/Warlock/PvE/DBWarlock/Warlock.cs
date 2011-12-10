/**********************************************************************************************
 * '$Author:$
 * '$Id:$
 * '$Rev:$
 *  
 * DarkBen's DemonWarlock 3
 * Created for the HonorBody Warlock Comunity with Help of Warlock Comunity!
 * This custom class has been Re-Writen from scratch, with some coding from the Base Class.
 * 
 * DO NOT USE FOR PVP
 * 
 * Features:
 * New Demon Warlock for Wow 4.x Cataclysm Changes
 * 
 * Change Log
 * 
 * 0.7.1 Sep-30-2011
 * Added autoUpdater code (borrowed from Ensemble) :)
 * 
 * 0.7 Sep-24-2011
 * Changed Demonology Rotation - Expect DPS Increase
 * Changed Metamorphosis Behavior
 *
 *  0.6.1 Aug-16-2011
 * Added Fell Flame to Rotation
 * Added Hand of Gul'dan to Rotation
 * 
 * 0.6 Aug-12-2011
 * Added pet casting
 * 
 * 0.5.2 Aug-08-2011
 * Fixed a bug with the save file not saving correctly.
 * 
 * 0.5.1 Jul-23-2011
 * Changed setting directory from within the fixed CC name to the HB settings Directory.
 * 
 * 0.5 Jul-17-2011
 * Added Felstorm to be casted automatically when there is to or more attackers.
 * 
 * 0.4 May-18-2011
 * Changed casting behavior to dont cast some spells when npc death is eminent.
 * 
 * 0.3 Mar-24-2011
 * Added Check for Drain Soul
 * Added Check Unending Breath
 * 
 * 0.2 Nov-1x-2010
 * Added Check for pet control
 * 
 * 0.1 Nov-15-2010
 * Autodetect Warlock Talent Tree.
 * Tested from level 1 to 27 Demonology
 * Tested from level 1 to 16 Affliction
 * Tested from level 1 to 14 Destruction
 * Implemented Anti Afk!
 * Limited Usage of Soul Burn (For summoning and use of healthstone)
 * 
 * In Development:
 * Rebuild the Gui Form to edit Configuration.
 * PVP Support
 *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using DBWarlock.Gui;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace DBWarlock
{
    public partial class Warlock : CombatRoutine
    {

        public static WarlockSettings _settings = new WarlockSettings();

        public override void Initialize()
        {
            
            try
            {
                if (Me.Class != WoWClass.Warlock)
                    return;
                if (!BotManager.Current.Initialized)
                    return;

                slog("-------------------------------------------------------------------------------", false);
                slog("-------------------------------------------------------------------------------", false);
                slog("Welcome to " + Name + "!", false);
                slog("This is a Warlock CC", false);
                slog("-------------------------------------------------------------------------------", false);
                isRunning = true;
                DetectSpec(true);
                //_settings.Load();
                slog("Checking for CC Updates!", false);
                CheckUpdate();

            }
            catch (Exception e)
            {
                slog(e.Message);
            }
        }

        private void DetectSpec(bool isInit)
        {
            slog("Determining Warlock Tree", false);
            if (Me.Level < 10)
            {
                WarlockTree = (int)WarlockType.Low;
                slog(Color.Brown, "{0} is not able to choose a tree yet!", false, Me.Name);
            }
            else if (SpellManager.Spells.ContainsKey("Unstable Affliction"))
            {
                WarlockTree = (int)WarlockType.Affliction;
                slog(Color.Brown, "{0} is an Affliction Warlock!", false, Me.Name);
            }
            else if (SpellManager.Spells.ContainsKey("Summon Felguard"))
            {
                WarlockTree = (int)WarlockType.Demonology;
                slog(Color.Brown, "{0} is a Demonology Warlock!", false, Me.Name);
            }
            else if (SpellManager.Spells.ContainsKey("Conflagrate"))
            {
                WarlockTree = (int)WarlockType.Destruction;
                slog(Color.Brown, "{0} is a Destruction Warlock!", false, Me.Name);
            }
            else
            {
                WarlockTree = (int)WarlockType.Low;
                slog(Color.Red, "{0} Has not choosen a talent tree yet!", false, Me.Name);
            }

        }

        public int WarlockTree = (int)WarlockType.Low;

        public enum WarlockType
        {
            Low,
            Affliction,
            Demonology,
            Destruction
        }

        public Warlock()
        {
            Initialize();
        }
        
        ~Warlock()
        {
            if (Me.Class == WoWClass.Warlock && isRunning)
            {
                slog("-------------------------------------------------------------------------------", false);
                slog("Stopping " + Name +"!", false);
                slog("Good Bye ", false);
                slog("-------------------------------------------------------------------------------", false);
                isRunning = false;
            }
        }

        public static bool isRunning = false;

            

        //public string ButtonText = "DBWarlock Config";
        /*{
            get
            {
                return "DbWarlock Config";
            }
        }*/

        public override bool WantButton
        {
            get
            {
                return true;
            }
        }

        public override void OnButtonPress()
        {
            ConfigForm.ShowDialog();
        }
        

        /// <summary>
        /// Configuration Form
        /// </summary>
        private Form _myForm;
        public Form ConfigForm 
        { 
            get 
            {
                if (_myForm == null)
                    _myForm = new DBWConfig();

                return _myForm; 
            } 
        }


        #region Global variables
        
        public override WoWClass Class { get { return WoWClass.Warlock; } }
        public Version Version { get { return new Version(0,7,1); } }
        public override sealed string Name { get { return "DarkBen's Demon Warlock 3 (" + Version + ")"; } }
        private string ShortName { get { return "DBWarlock 3 (" + Version + ")"; } }


        private readonly double shadowRange = 20; //SpellManager.Spells["Shadow Bolt"].MaxRange - 6;


        private static LocalPlayer Me { get { return ObjectManager.Me; } }

        private Stopwatch pullTimer = new Stopwatch();

        private Stopwatch fightTimer = new Stopwatch();

        private Stopwatch dismountTimer = new Stopwatch();

        private Stopwatch followTimer = new Stopwatch();

        //private static Stopwatch lastCast = new Stopwatch();

        //private static String lastSpell = "Dont Be Evil";

        private ulong lastGuid;

        private static Stopwatch _immolateCoolDown = new Stopwatch();

        private delegate bool CastRequirements(WoWUnit unit);


        #endregion


        #region Combat Buffs

        public override bool NeedPreCombatBuffs { get { return false; } }
        public override void PreCombatBuff() { }



        private readonly Dictionary<SpellPriority, CastRequirements> _combatBuff = new Dictionary<SpellPriority, CastRequirements>
        {
            {new SpellPriority("Metamorphosis", 100), unit => _settings.useMetamorphosis && CanCast("Metamorphosis") && !GotBuff("Metamorphosis")}
        };
        // Detects if you use the glyph of corruption Shadow trance state
        public override bool NeedCombatBuffs 
        {
            get 
            {
                
                _rotation.Clear();
                Enqueue(_combatBuff, 1);
                return (_rotation.Count > 0 && _settings.metamorphosisMinimumAggros - 1 < getAdds().Count);
            } 
        }
        public override void CombatBuff() 
        {
            while (Me.IsCasting)
            {
                Thread.Sleep(_settings.myLag);
            }

            MetamorphosisMania();

        }
        public override bool NeedPullBuffs 
        { 
            get 
            {
                bool pb = false;
                if (SpellManager.Spells.ContainsKey("Soul Link") &&
                    !Me.Auras.ContainsKey("Soul Link"))
                {
                    slog("Need Soul Link Pull Buff!");
                    pb = true;
                }

                if (!pb)
                    pb = needPet;

                if (pb)
                    slog("Need Pull Buff!");

                return pb;
            } 
        }
        public override void PullBuff() 
        {
            //slog("Pull Buff");
            CheckSummon();

        }

        #endregion

 
        #region pull

        private Stopwatch _pulLoop = new Stopwatch();
        private bool isPulling = false;
        public override void Pull()
        {
            isPulling = true;
            while (Me.IsMoving)
                WoWMovement.MoveStop();
                
            _pulLoop.Reset();
            _pulLoop.Start();
            if (_settings.showDebug)
            {
                slog("Starting Pull Loop!");
            }
            dismountTimer.Start();

            while ((dismountTimer.ElapsedMilliseconds < 5000 && dismountTimer.IsRunning) && !Me.GotAlivePet)
            {
                if (_settings.showDebug)
                {
                    slog("Wait for pet or 5s - now {0} ms! {1}", dismountTimer.ElapsedMilliseconds, dismountTimer.IsRunning);
                    slog("PetAlive {0}!", Me.GotAlivePet);
                }
                Thread.Sleep(_settings.myLag);
            }
            if (_settings.showDebug)
            {
                slog("Wait ends!");
            }

            if (combatChecks)
            {
                if (Battlegrounds.IsInsideBattleground && Me.CurrentTarget.Mounted)
                {
                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromSeconds(30));
                    Me.ClearTarget();
                    return;
                }
                if (Me.CurrentTarget.Guid != lastGuid)
                {
                    fightTimer.Reset();
                    lastGuid = Me.CurrentTarget.Guid;
                    slog("Killing " + Me.CurrentTarget.Name + " at distance " +
                        System.Math.Round(targetDistance).ToString() + ".");
                    pullTimer.Reset();
                    pullTimer.Start();

                }
                else
                {
                    if (pullTimer.ElapsedMilliseconds > 30 * 1000)
                    {
                        slog("Cannot pull " + Me.CurrentTarget.Name + " now.  Blacklist for 30 minutes.");
                        Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromMinutes(30));
                    }
                }


                if (Me.GotAlivePet && Me.GotTarget)
                {
                    if (!Me.Pet.IsAutoAttacking && Me.Level > 10)
                    {
                       // Me.Pet.
                        Lua.DoString("PetAttack()");
                        slog("Let " + Me.Pet.Name + " gain aggro.");
                        Thread.Sleep(_settings.petAttackDelay);
                    }
                }

//                if (!Me.IsAutoAttacking)
//                {
//                    Lua.DoString("StartAttack()");
//                    Thread.Sleep(_settings.myLag);
//               }


                while (targetDistance > shadowRange+2)
                {
                    slog("{0} yard from {1}.", Math.Round(targetDistance).ToString(), Me.CurrentTarget.Name);
                    Navigator.MoveTo(attackPoint);
                    //WoWMovement.ClickToMove(Me.CurrentTarget.Location, (float)shadowRange);
                    Thread.Sleep(_settings.myLag);
                    if (pullTimer.ElapsedMilliseconds > 10000)
                    {
                        Me.ClearTarget();
                        Lua.DoString("PetFollow()");
                        return;
                    }
                }

                while (!Me.CurrentTarget.InLineOfSightOCD && targetDistance > 2)
                {
                    slog("Not in LOS, closing the target");
                    Navigator.MoveTo(Me.CurrentTarget.Location);
                    //WoWMovement.ClickToMove(Me.CurrentTarget.Location, (float)(targetDistance - 1) );
                    Thread.Sleep(_settings.myLag * 2);
                    if (pullTimer.ElapsedMilliseconds > 12000)
                    {
                        Me.ClearTarget();
                        Lua.DoString("PetFollow()");
                        return;
                    }
                }

                while (Me.IsMoving)
                    WoWMovement.MoveStop();

                CombatDecision();

            
                _pulLoop.Stop();

                if (_settings.showDebug)
                {
                    slog("Exiting Pull Loop! Duration {0}ms", _pulLoop.ElapsedMilliseconds);
                }
            }
        }

        #endregion


        #region combat

        private Stopwatch _combatLoop = new Stopwatch();

        public override void Combat()
        {
            isPulling = false;
            _combatLoop.Reset();
            _combatLoop.Start();

            if (_settings.showDebug)
            {
                slog("Starting Combat Loop!");
            }


            try
            {

                if (Battlegrounds.IsInsideBattleground)
                {
                    if (Me.GotTarget &&
                        Me.CurrentTarget.IsPet)
                    {
                        Blacklist.Add(Me.CurrentTarget, TimeSpan.FromDays(1));
                        Me.ClearTarget();
                        return;
                    }
                }

                #region bugged mobs
                if (Me.GotTarget && (!fightTimer.IsRunning || Me.CurrentTarget.Guid != lastGuid))
                {
                    fightTimer.Reset();
                    fightTimer.Start();
                    lastGuid = Me.CurrentTarget.Guid;
                    slog("Killing " + Me.CurrentTarget.Name + " at distance " +
                        System.Math.Round(targetDistance).ToString() + ".");

                }

                if (Me.GotTarget && !Me.CurrentTarget.IsPlayer &&
                    fightTimer.ElapsedMilliseconds > 25 * 1000 &&
                    Me.CurrentTarget.HealthPercent > 95)
                {
                    slog(" This " + Me.CurrentTarget.Name + " is a bugged mob.  Blacklisting for 1 hour.");

                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromHours(1.00));
                    Me.ClearTarget();

                    Styx.Helpers.KeyboardManager.PressKey('S');
                    Thread.Sleep(5 * 1000);
                    Styx.Helpers.KeyboardManager.ReleaseKey('S');
                    Me.ClearTarget();
                    lastGuid = 0;
                    return;
                }

                #endregion

                DotEmUp(); // Thanks to CodeNameG

                SmartTarget(); // TM :P

                if (Me.GotTarget)
                {
                    if (isTotem)
                    {
                        slog("Killing totem {0}", Me.CurrentTarget.Name);
                        KillTotem();
                        //                        Thread.Sleep(_settings.myLag);
                    }
                }
                else
                {
                    _combatLoop.Stop();
                    if (_settings.showDebug)
                    {
                        slog("Exiting Combat Loop (NoTarget)! Duration {0}ms", _combatLoop.ElapsedMilliseconds);
                    }
                    return;
                }


                //Thread.Sleep(_settings.myLag);

                if (Me.GotAlivePet)
                {
                    if (Me.GotTarget && Me.CurrentTarget.CurrentTargetGuid == Me.Guid)
                    {
                        Lua.DoString("PetAttack()");
                        slog("Target is attacking me, calling {0} to get aggro", Me.Pet.Name);
                    }
                    Lua.DoString("PetAttack()");
                }
                followTimer.Reset();
                followTimer.Start();


                while (targetDistance > shadowRange + 2)
                {
                    slog("{0} yard from {1}.", System.Math.Round(targetDistance).ToString(), Me.CurrentTarget.Name);
                    Navigator.MoveTo(attackPoint);
                    //WoWMovement.ClickToMove(Me.CurrentTarget.Location, (float)shadowRange);
                    Thread.Sleep(_settings.myLag);
                    if (followTimer.ElapsedMilliseconds > 20000 && Me.CurrentTarget.IsPlayer)
                    {
                        followTimer.Stop();
                        slog("Followed for more than 20 secs!");
                        Me.ClearTarget();
                        Lua.DoString("PetFollow()");
                        return;
                    }

                }
                followTimer.Stop();


                CheckSummon();

                while (Me.IsCasting)
                {
                    Thread.Sleep(_settings.myLag);
                }
                if (Me.GotTarget)
                {
                   CombatDecision();
                }

                if (targetDistance <= 4 && !Me.IsCasting)
                {
                    WoWMovement.Face();
                    Thread.Sleep(_settings.myLag);
                    if (!Me.IsAutoAttacking)
                        Lua.DoString("StartAttack()");
                }

            }
            catch (Exception ex)
            {
                if (_settings.showDebug)
                {
                    slog(ex.Source);
                    slog(ex.Message);
                    slog(ex.StackTrace);
                }
                
            }

            _combatLoop.Stop();

            if (_settings.showDebug)
            {
                slog("Exiting Combat Loop! Duration {0}ms", _combatLoop.ElapsedMilliseconds);
            }


        }

        #endregion

        #region falling
        public void HandleFalling()
        {
        }

        #endregion


        #region rest
        public override bool NeedHeal { get { return false; } }
        public override void Heal() { }


        public override bool NeedRest
        {
            get
            {
                if (Me.Combat)
                    return false;
                if (Me.CurrentSoulShards < Me.MaxSoulShards && CanCast("Soul Harvest"))
                {
                    slog("Need Soulshard! Current have: " + Me.CurrentSoulShards.ToString() + " Can Have: " + Me.MaxSoulShards.ToString());
                    return true;
                }

                if ((Me.Auras.ContainsKey("Drink") && Me.ManaPercent < 100) || (Me.Auras.ContainsKey("Food") && Me.HealthPercent < 100))
                    return true;

                if (!Me.Auras.ContainsKey("Recently Bandaged") && Me.HealthPercent < 60 && HaveItemCheck(bandageEID) != null)
                {
                    slog("Will use Bandages...");
                    return true;
                }


                if (Me.Mounted)
                {
                    dismountTimer.Stop();
                    dismountTimer.Reset();
                    return false;
                }
                else
                {
                    if (!dismountTimer.IsRunning)
                    {
                        dismountTimer.Start();
                    }
                }
                if (!Me.GotAlivePet && canSummon)
                    return true;

                if (NeedBuffs)
                {
                    slog("Need Buffs!");
                    return true;
                }

                bool nresting = false;


                if (Me.ManaPercent < _settings.restManaPercent)
                {
                    slog("Need Rest : Mana below {0}", _settings.restManaPercent);
                    nresting = true;
                }
                if (Me.HealthPercent < _settings.restHealthPercent)
                {
                    slog("Need Rest : Health below {0}", _settings.restHealthPercent);
                    nresting = true;
                }

                return (nresting );
            }
        }

        public override void Rest() 
        {
            WoWMovement.MoveStop();
            Thread.Sleep(_settings.myLag);
            if (Me.Combat)
                return;

            while (Me.Auras.ContainsKey("Drink") && Me.ManaPercent < 100)
            {
                Thread.Sleep(_settings.myLag);
            }

            while (Me.Auras.ContainsKey("Food") && Me.HealthPercent < 100)
            {
                Thread.Sleep(_settings.myLag);
            }

            //slog("Resting!");

            Thread.Sleep(_settings.myLag);



            if (NeedLifeTap)
            {
                LifeTap();
                Thread.Sleep(_settings.myLag);
            }
            CheckSummon();

            if (NeedBuffs)
            {
                BuffMe();
            }


//            if (soulShardCount > 0 && Equals(null, myFirestone) && SpellManager.CanCast("Create Firestone"))
//           {
//                CreateFirestone();
//                Thread.Sleep(_settings.myLag);
//            }


            if (!Me.Auras.ContainsKey("Recently Bandaged") && Me.HealthPercent < 60 && HaveItemCheck(bandageEID) != null)
            {
                UseBandage();
            }


            if (Me.CurrentSoulShards < Me.MaxSoulShards && CanCast("Soul Harvest"))
            {
                SafeCast("Soul Harvest", false);
                Thread.Sleep(2000);

                while (GotBuff("Soul Harvest") && (Me.CurrentSoulShards < Me.MaxSoulShards))
                {
                    slog("Harvesting Souls!");
                    Thread.Sleep(_settings.myLag);
                }
                return;

            }

            Styx.Logic.Common.Rest.Feed();

        }

        #endregion
  

    }
}

/**************************/
/* Cimmerian 3.0 Beta 3 by Mord /*
/**************************/

/*If you need help please check the thread where you got this download*/
/*I do not rip code from others. Code I use from others is given to me and I add credit*/
/*Please return the favor.*/



using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using Cimmerian.Gui;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Cimmerian
{
    public class Deathknight : CombatRoutine
    {
        #region System Stuff

        private Form _cachedForm; //Best damn form around.

        public override string Name
        {
            get { return "Cimmerian 3.0 Final by Mord"; }
        }

        public override WoWClass Class
        {
            get { return WoWClass.DeathKnight; }
        }

        public override bool NeedCombatBuffs
        {
            get { return false; }
        }

        public override bool NeedPullBuffs
        {
            get { return false; }
        }

        public override bool NeedPreCombatBuffs
        {
            get { return false; }
        }

        public override bool NeedHeal
        {
            get { return false; }
        }


        public override bool WantButton
        {
            get { return true; }
        }

        public Form ConfigForm
        {
            get { return new CimmerianForm(); }
        }

        public override void CombatBuff()
        {
        }

        public override void PullBuff()
        {
        }

        public override void PreCombatBuff()
        {
        }

        public override void Heal()
        {
        }

        public override void OnButtonPress() //Do on Config Button Press
        {
            if (_cachedForm != null)
                _cachedForm.ShowDialog();

            _cachedForm = new CimmerianForm();
            _cachedForm.ShowDialog();
        }

        private void Slog(string msg)
        {
            if (msg == _logspam)
            {
                return;
            }

            Logging.Write(msg);
            _logspam = msg;
        }

        #region Global Variables DONE

        private string _logspam;

        private static LocalPlayer Me
        {
            get { return ObjectManager.Me; }
        }

        public override void Initialize()
        {
            CimmerianSettings.Instance.Load();
        }

        #endregion

        #region Class Specific Variables

        public static int StuckCount;
        public static int LoSCount;
        private bool _addSpam;
        private bool _drwSpam;
        private bool _drwToggle;
        private bool _gotAdds;
        private bool _hcToggle;
        private bool _openerComplete;
        private bool _moveWithMelee;
        private bool _pullSpam;
        private bool _pullTypeSpam;
        private bool _rimeProc;
        private bool _csProc;
        private bool _sgSpam;
        private bool _sgToggle;
        private bool _pulseSlog;
        private int _pullCount;
        private int _pestilenceCount;

        #region Advanced Settings

        //*********Opener********//

        private static bool OpenWithDeathGrip
        {
            get { return CimmerianSettings.Instance.OpenWithDeathGrip; }
        }

        private static bool OpenWithIcyTouch
        {
            get { return CimmerianSettings.Instance.OpenWithIcyTouch; }
        }

        private static bool OpenWithDarkCommand
        {
            get { return CimmerianSettings.Instance.OpenWithDarkCommand; }
        }

        private static int IcyTouchRange
        {
            get { return CimmerianSettings.Instance.IcyTouchRange; }
        }

        private static bool OpenWithIcyTouchBackup
        {
            get { return CimmerianSettings.Instance.OpenWithIcyTouchBackup; }
        }

        //*********Player Detector********//

        private static int PdRange
        {
            get { return CimmerianSettings.Instance.PlayerDetectorRange; }
        }

        private static bool PlayerAlert
        {
            get { return CimmerianSettings.Instance.AlertPlayers; }
        }

        private static bool PlayerAlertLog
        {
            get { return CimmerianSettings.Instance.AlertPlayersLog; }
        }

        //*********Blood********//

        private static bool UseVampiricBlood
        {
            get { return CimmerianSettings.Instance.UseVampiricBlood; }
        }

        private static bool VampiricBloodAdds
        {
            get { return CimmerianSettings.Instance.VampiricBloodAdds; }
        }

        private static int VampiricBloodHealth
        {
            get { return CimmerianSettings.Instance.VampiricBloodHealth; }
        }

        private static bool UseDrw
        {
            get { return CimmerianSettings.Instance.UseDrw; }
        }

        private static bool DrwAdds
        {
            get { return CimmerianSettings.Instance.DrwAdds; }
        }

        private static bool UseBoneShield
        {
            get { return CimmerianSettings.Instance.UseBoneShield; }
        }

        private static bool UseRuneTap
        {
            get { return CimmerianSettings.Instance.UseRuneTap; }
        }

        private static int RuneTapHealth
        {
            get { return CimmerianSettings.Instance.RuneTapHealth; }
        }

        private static bool UseBloodTap
        {
            get { return CimmerianSettings.Instance.UseBloodTap; }
        }

        //*********Frost********//

        private static bool UseIceboundFortitude
        {
            get { return CimmerianSettings.Instance.UseIceboundFortitude; }
        }

        private static int IceboundFortitudeHealth
        {
            get { return CimmerianSettings.Instance.IceboundFortitudeHealth; }
        }

        private static bool IceboundFortitudeAdds
        {
            get { return CimmerianSettings.Instance.IceboundFortitudeAdds; }
        }

        private static bool UsePillar
        {
            get { return CimmerianSettings.Instance.UsePillar; }
        }

        private static int PillarHealth
        {
            get { return CimmerianSettings.Instance.PillarHealth; }
        }

        private static bool PillarAdds
        {
            get { return CimmerianSettings.Instance.PillarAdds; }
        }

        private static bool UseErw
        {
            get { return CimmerianSettings.Instance.UseErw; }
        }

        private static bool UseErwRunes
        {
            get { return CimmerianSettings.Instance.UseErwRunes; }
        }

        private static bool UseErwAdds
        {
            get { return CimmerianSettings.Instance.UseErwAdds; }
        }

        private static bool RimeIcyTouch
        {
            get { return CimmerianSettings.Instance.RimeIcyTouch; }
        }

        private static bool RimeHb
        {
            get { return CimmerianSettings.Instance.RimeHb; }
        }

        private static bool UseRuneStrike
        {
            get { return CimmerianSettings.Instance.UseRuneStrike; }
        }

        private static bool UseHc
        {
            get { return CimmerianSettings.Instance.UseHc; }
        }

        private static bool UseHowlingBlast
        {
            get { return CimmerianSettings.Instance.UseHowlingBlast; }
        }

        private static bool UseLichborne
        {
            get { return CimmerianSettings.Instance.UseLichborne; }
        }

        private static int LichborneHealth
        {
            get { return CimmerianSettings.Instance.LichbornHealth; }
        }

        //*********Unholy********//

        private static bool UseHorn
        {
            get { return CimmerianSettings.Instance.UseHorn; }
        }

        private static bool UseOutbreak
        {
            get { return CimmerianSettings.Instance.UseOutbreak; }
        }

        private static bool UseSummonGargoyle
        {
            get { return CimmerianSettings.Instance.UseSummonGargoyle; }
        }

        private static bool UseDarkTransformation
        {
            get { return CimmerianSettings.Instance.UseDarkTransformation; }
        }

        private static bool UseUnholyFrenzy
        {
            get { return CimmerianSettings.Instance.UseUnholyFrenzy; }
        }

        //*********Pet********//

        private static bool UseRaiseDead
        {
            get { return CimmerianSettings.Instance.UseRaiseDead; }
        }
        private static int DeathPactHealth
        {
            get { return CimmerianSettings.Instance.DeathPactHealth; }
        }

        private static bool UseDeathPact
        {
            get { return CimmerianSettings.Instance.UseDeathPact; }
        }

        //*********Racials********//

        private static bool UseAt
        {
            get { return CimmerianSettings.Instance.UseAt; }
        }

        private static bool UseNaaru
        {
            get { return CimmerianSettings.Instance.UseNaaru; }
        }

        private static int NaaruHealth
        {
            get { return CimmerianSettings.Instance.NaaruHealth; }
        }

        private static bool UseStoneForm
        {
            get { return CimmerianSettings.Instance.UseStoneForm; }
        }

        private static int StoneFormHealth
        {
            get { return CimmerianSettings.Instance.StoneFormHealth; }
        }

        private static bool UseEm
        {
            get { return CimmerianSettings.Instance.UseEm; }
        }

        private static bool NaaruAdds
        {
            get { return CimmerianSettings.Instance.NaaruAdds; }
        }

        private static bool SfAdds
        {
            get { return CimmerianSettings.Instance.SfAdds; }
        }

        private static bool UseWarStomp
        {
            get { return CimmerianSettings.Instance.UseWarStomp; }
        }

        private static int WarStompHealth
        {
            get { return CimmerianSettings.Instance.WarStompHealth; }
        }

        private static bool WarStompAdds
        {
            get { return CimmerianSettings.Instance.WarStompAdds; }
        }

        private static bool WarStompCasters
        {
            get { return CimmerianSettings.Instance.WarStompCasters; }
        }

        private static bool UseLifeBlood
        {
            get { return CimmerianSettings.Instance.UseLifeBlood; }
        }

        private static bool LifeBloodAdds
        {
            get { return CimmerianSettings.Instance.LifeBloodAdds; }
        }

        private static int LifeBloodHealth
        {
            get { return CimmerianSettings.Instance.LifebloodHealth; }
        }

        private static bool UseBloodFury
        {
            get { return CimmerianSettings.Instance.UseBloodFury; }
        }

        private static bool BloodFuryAdds
        {
            get { return CimmerianSettings.Instance.BloodFuryAdds; }
        }

        private static int BloodFuryHealth
        {
            get { return CimmerianSettings.Instance.BloodFuryHealth; }
        }

        //*********Misc********//
        
        private static int RestHealth
        {
            get { return CimmerianSettings.Instance.RestHealth; }
        }

        private static bool UsePoF
        {
            get { return CimmerianSettings.Instance.UsePoF; }
        }
        
        private static bool UseStrangulate
        {
            get { return CimmerianSettings.Instance.UseStrangulate; }
        }

        private static bool UseStrangulateMelee
        {
            get { return CimmerianSettings.Instance.UseStrangulateMelee; }
        }

        private static bool UseMindFreeze
        {
            get { return CimmerianSettings.Instance.UseMindFreeze; }
        }

        private static bool UseDeathGripInterupt
        {
            get { return CimmerianSettings.Instance.UseDeathGripInterupt; }
        }

        private static bool IgnoreAdds
        {
            get { return CimmerianSettings.Instance.IgnoreAdds; }
        }

        private static bool UseAntiMagicShell
        {
            get { return CimmerianSettings.Instance.UseAntiMagicShell; }
        }

        private static bool UseChainsOfIce
        {
            get { return CimmerianSettings.Instance.UseChainsOfIce; }
        }

        private static bool UseDeathGripRunners
        {
            get { return CimmerianSettings.Instance.UseDeathGripRunners; }
        }

        private static bool UseDarkCommandRunners
        {
            get { return CimmerianSettings.Instance.UseDarkCommandRunners; }
        }

        private static int AddsCount
        {
            get { return CimmerianSettings.Instance.AddsCount; }
        }

        private static int CooldownHealth
        {
            get { return CimmerianSettings.Instance.CooldownHealth; }
        }

        private static bool UseBloodPresence
        {
            get { return CimmerianSettings.Instance.UseBloodPresence; }
        }

        private static bool UseFrostPresence
        {
            get { return CimmerianSettings.Instance.UseFrostPresence; }
        }

        private static bool UseUnholyPresence
        {
            get { return CimmerianSettings.Instance.UseUnholyPresence; }
        }

        #endregion

        #endregion

        #endregion

        #region Pulse Override

        /////////////////////////////////
        //Override HB Movement (CodenameG Inspired)
        /////////////////////////////////

        public override void Pulse()
        {
            if ((UseBloodPresence) && (!Me.Auras.ContainsKey("Blood Presence")))

            {
                if (SpellManager.CanCast("Blood Presence"))
                {
                    BloodPresence();
                    return;
                }
            }

            if ((UseFrostPresence) && (!Me.Auras.ContainsKey("Frost Presence")))
            {
                if (SpellManager.CanCast("Frost Presence"))
                {
                    FrostPresence();
                    return;
                }
            }

            if ((UseUnholyPresence) && (!Me.Auras.ContainsKey("Unholy Presence")))
            {
                if (SpellManager.CanCast("Unholy Presence"))
                {
                    UnholyPresence();
                    return;
                }
            }

            //Keep Bone Shield up at all times.
            
            if ((!Me.Mounted) && (!Me.Combat))
            {
                if (UseBoneShield) //Cast Boneshield now!
                {
                    if (!Me.Auras.ContainsKey("Bone Shield"))
                    {
                        if (SpellManager.CanCast("Bone Shield"))
                        {
                            BoneShield();
                            return;
                        }
                    }
                }

                if (UseHorn) //Cast Boneshield now!
                {
                    if (!Me.Auras.ContainsKey("Horn of Winter"))
                    {
                        if (SpellManager.CanCast("Horn of Winter"))
                        {
                            HornOfWinter();
                            return;
                        }
                    }
                }
            }

            if (Me.IsSwimming)
            {

                if ((UsePoF) && (!Me.Auras.ContainsKey("Path of Frost")))
                {
                    if (SpellManager.CanCast("Path of Frost"))
                        PathofFrost();
                }

            }

            /*if (CaLevelTwo) // Level 2 Assist
            {
                

                while (Styx.Logic.BehaviorTree.TreeRoot.IsRunning && !Me.Combat)
                {
                    if (!_pulseSlog)
                    {

                        Slog("#Out of Combat. Waiting for your next Pull#");
                        
                        _pulseSlog = true;
                    }
                    
                    Thread.Sleep(25);
                }
            }

            //if (Me.GotTarget && !Me.Combat)
            //{

            //if(Me.CurrentTarget.IsAlive && Me.CurrentTarget.Attackable && !Me.CurrentTarget.IsFriendly)
            //{
            //Slog("Not Sleep");
            //RoutineManager.Current.Pull();
            //return;  
            //}

            //}*/
        }

        #endregion

        #region Pull DONE

        /////////////////////////////////
        //A global state run before combat when in range of target
        /////////////////////////////////

        private readonly Stopwatch _pullTimer = new Stopwatch(); //A Pull Timer        

        public override void Pull()
        {


            ///////////////////////////////////////////////////////////////////////
            //Not in BG
            ///////////////////////////////////////////////////////////////////////



            if (Me.CurrentTarget.TaggedByOther) //HB is too fuckin slow to see the player tagged.
            {
                Blacklist.Add(Me.CurrentTarget, TimeSpan.FromSeconds(5.0));
                Me.ClearTarget();
                WoWMovement.MoveStop();
                _pullSpam = false;
                _pullTypeSpam = false;
                Slog("#Target is Tagged, Duh#");
            }

            if (!_pullTimer.IsRunning) //reset timer if it is still running from previous pull
                _pullTimer.Start();

            if (Me.CurrentTarget.Distance > 50) //Also make sure we are sorta close
            {
                Blacklist.Add(Me.CurrentTarget, TimeSpan.FromSeconds(2.00));
                Me.ClearTarget();
                _pullSpam = false;
                _pullTypeSpam = false;
                return;
            }

            if (_pullTimer.Elapsed.Seconds > 12)
            {
                Slog("Error pulling! Took too long! Clear Target.");
                Blacklist.Add(Me.CurrentTarget, TimeSpan.FromSeconds(60.00));
                Me.ClearTarget();
                _pullTimer.Reset();
                _pullSpam = false;
                return;
            }

            if (_pullCount > 9)
            {
                Slog("Error pulling! Too many attempts! Clear Target.");
                Blacklist.Add(Me.CurrentTarget, TimeSpan.FromSeconds(60.00));
                Me.ClearTarget();
                _pullTimer.Reset();
                _pullSpam = false;
                _pullCount = 0;
                return;
            }

            if (!_pullSpam) //Avoid that annoying spam
                Slog("#Killing " + Me.CurrentTarget.Name + " Distance : " +
                     Math.Floor(Me.CurrentTarget.Distance) + " yards.#");

            //
            //////////////////////
            //Opener is Deathgrip 
            //////////////////////
            //

            if (OpenWithDeathGrip)
            {
                if (Me.CurrentTarget.Distance < 3) //Too Close for Deathgrip, just start attacking
                {
                    if (AutoAttack()) //Start Auto Attack
                        return;
                }

                if (!SpellManager.CanCast("Death Grip")) //Death grip is on CD
                {
                    if (OpenWithIcyTouchBackup) //Use Icy Touch as a backup
                    {
                        if (!SpellManager.CanCast("Icy Touch")) //Can not cast Icy Touch
                        {
                            if (!_pullTypeSpam)
                                Slog(
                                    "#Opener(s) is on Cooldown. Moving to within 3 yards of current target for melee attack#");

                            Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                              ObjectManager.Me.CurrentTarget.
                                                                                  Location, 2.5f));
                            _pullSpam = true;
                            _pullTypeSpam = true;
                            _moveWithMelee = true;
                        }

                        else
                        {
                            if (Me.CurrentTarget.Distance >= IcyTouchRange) //Not in range of Icy Touch 
                            {
                                if (!_pullTypeSpam)
                                {
                                    Slog("#Death Grip is on Cooldown, using Icy Touch#");
                                    Slog("#Moving to within " + IcyTouchRange +
                                         " yards of current target for Icy Touch#");
                                }

                                Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                                  ObjectManager.Me.CurrentTarget
                                                                                      .Location, 2.5f));
                                _pullSpam = true;
                                _pullTypeSpam = true;
                                _moveWithMelee = false;
                                return;
                            }

                            if (!_pullTypeSpam)
                                Slog("#Within range of target for Icy Touch#");

                            if (!Me.CurrentTarget.InLineOfSight) //Dont track if the target is not LoS
                            {
                                Slog("#But not in Line of Sight. Move to LoS#");
                                Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(
                                    ObjectManager.Me.Location, ObjectManager.Me.CurrentTarget.Location, 2.5f));
                                _pullSpam = true;
                                _pullTypeSpam = true;
                                _moveWithMelee = false;
                                return;
                            }

                            _pullTypeSpam = true;
                            _pullSpam = true;
                            _moveWithMelee = false;
                        }
                    }

                    else //Can not Cast Death Grip. No Backup
                    {
                        if (!_pullTypeSpam)
                            Slog(
                                "#Opener(s) is on Cooldown. Moving to within 3 yards of current target for melee attack#");

                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = true;
                    }
                }

                else //Death Grip is ready
                {
                    if (Me.CurrentTarget.Distance >= 29)
                    {
                        if (!_pullTypeSpam)
                            Slog("#Moving to within 29 yards of current target for Death Grip#");

                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = false;
                        return;
                    }

                    if (!_pullTypeSpam)
                        Slog("#Within range of target for Death Grip#");

                    if (!Me.CurrentTarget.InLineOfSight) //Dont track if the target is not LoS
                    {
                        Slog("#But not in Line of Sight. Move to LoS#");
                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = false;
                        return;
                    }

                    _pullTypeSpam = true;
                    _pullSpam = true;
                    _moveWithMelee = false;
                }
            }

                //
            //////////////////////
            //Opener is Dark Command 
            //////////////////////
            //

            else if (OpenWithDarkCommand)
            {
                if (Me.CurrentTarget.Distance < 3) //Too Close for Dark Command, just start attacking
                {
                    if (AutoAttack()) //Start Auto Attack
                        return;
                }

                if (!SpellManager.CanCast("Dark Command")) //Dark Command is on CD
                {
                    if (OpenWithIcyTouchBackup) //Use Icy Touch as a backup
                    {
                        if (!SpellManager.CanCast("Icy Touch")) //Can not cast Icy Touch
                        {
                            if (!_pullTypeSpam)
                                Slog(
                                    "#Opener(s) is on Cooldown. Moving to within 3 yards of current target for melee attack#");

                            Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                              ObjectManager.Me.CurrentTarget.
                                                                                  Location, 2.5f));
                            _pullSpam = true;
                            _pullTypeSpam = true;
                            _moveWithMelee = true;
                        }

                        else
                        {
                            if (Me.CurrentTarget.Distance >= IcyTouchRange) //Not in range of Icy Touch 
                            {
                                if (!_pullTypeSpam)
                                {
                                    Slog("#Dark Command is on Cooldown, using Icy Touch#");
                                    Slog("#Moving to within " + IcyTouchRange +
                                         " yards of current target for Icy Touch#");
                                }

                                Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                                  ObjectManager.Me.CurrentTarget
                                                                                      .Location, 2.5f));
                                _pullSpam = true;
                                _pullTypeSpam = true;
                                _moveWithMelee = false;
                                return;
                            }

                            if (!_pullTypeSpam)
                                Slog("#Within range of target for Icy Touch#");

                            if (!Me.CurrentTarget.InLineOfSight) //Dont track if the target is not LoS
                            {
                                Slog("#But not in Line of Sight. Move to LoS#");
                                Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(
                                    ObjectManager.Me.Location, ObjectManager.Me.CurrentTarget.Location, 2.5f));
                                _pullSpam = true;
                                _pullTypeSpam = true;
                                _moveWithMelee = false;
                                return;
                            }

                            _pullTypeSpam = true;
                            _pullSpam = true;
                            _moveWithMelee = false;
                        }
                    }

                    else //Can not Cast Death Grip. No Backup
                    {
                        if (!_pullTypeSpam)
                            Slog(
                                "#Opener(s) is on Cooldown. Moving to within 3 yards of current target for melee attack#");

                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = true;
                        return;
                    }
                }

                else //Dark Command is ready
                {
                    if (Me.CurrentTarget.Distance >= 29)
                    {
                        if (!_pullTypeSpam)
                            Slog("#Moving to within 29 yards of current target for Dark Command#");

                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = false;
                        return;
                    }

                    if (!_pullTypeSpam)
                        Slog("#Within range of target for Dark Command#");

                    if (!Me.CurrentTarget.InLineOfSight) //Dont track if the target is not LoS
                    {
                        Slog("#But not in Line of Sight. Move to LoS#");
                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = false;
                        return;
                    }

                    _pullTypeSpam = true;
                    _pullSpam = true;
                    _moveWithMelee = false;
                }
            }

                //
            /////////////////////
            //Opener is Icy Touch
            /////////////////////
            //

            else if (OpenWithIcyTouch)
            {
                if (Me.CurrentTarget.Distance < 3) //Too Close for Icy Touch, just start attacking
                {
                    if (AutoAttack()) //Start Auto Attack
                        return;
                }

                if (SpellManager.CanCast("Icy Touch"))
                {
                    if (Me.CurrentTarget.Distance >= IcyTouchRange)
                    {
                        if (!_pullTypeSpam)
                            Slog("#Moving to within " + IcyTouchRange +
                                 " yards of current target for Icy Touch#");

                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = false;
                        return;
                    }

                    if (!_pullTypeSpam)
                        Slog("#Within range of target for Icy Touch#");

                    if (!Me.CurrentTarget.InLineOfSight) //Dont track if the target is not LoS
                    {
                        Slog("#But not in Line of Sight. Move to LoS#");
                        Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                          ObjectManager.Me.CurrentTarget.
                                                                              Location, 2.5f));
                        _pullSpam = true;
                        _pullTypeSpam = true;
                        _moveWithMelee = false;
                        return;
                    }

                    _pullTypeSpam = true;
                    _pullSpam = true;
                    _moveWithMelee = false;
                }

                else
                {
                    if (!_pullTypeSpam)
                        Slog(
                            "#Opener(s) is on Cooldown. Moving to within 3 yards of current target for melee attack#");

                    Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                      ObjectManager.Me.CurrentTarget.Location,
                                                                      2.5f));
                    _pullSpam = true;
                    _pullTypeSpam = true;
                    _moveWithMelee = true;
                }
            }

            else //No opener selected. Move to melee
            {
                if (!_pullTypeSpam)
                    Slog("#Moving to within 3 yards of current target for melee attack#");

                _moveWithMelee = true;

                Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                  ObjectManager.Me.CurrentTarget.Location, 2.5f));
                _pullSpam = true;
                _pullTypeSpam = true;
                _moveWithMelee = true;
            }

            ///////////Pull////////////
            if (!_moveWithMelee)
            {
                if (Opener()) //Do Opener
                {
                    Slog("#Opener was successfull!#");
                    _pullSpam = false;
                    _pullTypeSpam = false;
                    _pullCount++; // In case everything else fails
                }

                else
                {
                    Slog("#Opener failed! Try again#");
                    _moveWithMelee = false;
                    WoWMovement.MoveStop();
                    Thread.Sleep(100);
                    WoWMovement.Face();
                    return;
                }
            }

            else //Move to Melee
            {
                if (Me.CurrentTarget.Distance < 10) //Dont start auto Attack till we are closer
                {
                    if (AutoAttack()) //Start Auto Attack
                        return;
                }
            }

            if (Me.CurrentTarget.Distance < 1)
            {
                Slog("#Overshot Target... Stop & Face.#");
                WoWMovement.MoveStop();
                Thread.Sleep(125);
                WoWMovement.Face();
                Thread.Sleep(125);
            }
        }

        #endregion

        #region Combat DONE

        public override void Combat()
        {
            _pullSpam = false;

            if (_pulseSlog)
                _pulseSlog = false;

            if (_pullCount > 1)
                _pullCount = 0;

            if (_pullTimer.IsRunning && Me.Combat)
                _pullTimer.Reset(); //If timer is still running, stop it.

            //if (Me.Minions.Add())
                //Slog("Have Minions");

            if (TargetErrorCheck())
            {

                if (AutoAttack()) //Start Auto Attack
                    return;

                if (RunCheck())
                    return; //Prevent stupid movment

                if (Rotation()) //Decide a spell to use
                    return;

            }

        }

        #endregion

        #region PvE Logic

        #region PvE Rotation

        /////////////////////////////////
        //PvE In Combat rotation planner
        /////////////////////////////////

        public bool Rotation()
        {
            if (SpellManager.Spells.ContainsKey("Heart Strike")) //Blood Rotation
            {
                if (MordBlood())
                    return true;
            }

            if (SpellManager.Spells.ContainsKey("Frost Strike")) //Frost Rotation
            {

                if (MordFrost())
                    return true;
            }

            if (SpellManager.Spells.ContainsKey("Scourge Strike")) //Unholy Rotation
            {

                if (MordUnholy())
                    return true;
            }

            if ((!SpellManager.Spells.ContainsKey("Heart Strike")) && (!SpellManager.Spells.ContainsKey("Frost Strike")) && (!SpellManager.Spells.ContainsKey("Scourge Strike"))) //Basic Rotation
            {
                if (StartingRotation())
                    return true;
            }

            return false;
        }

        #endregion

        #region PVE Move to Melee DONE

        /////////////////////////////////
        //Checks range to target and moves accordingly
        /////////////////////////////////

        public bool MoveToMelee()
        {
            if (((NullCheck()) && (!Me.CurrentTarget.Fleeing))) //We must stay closer if target is Fleeing
            {



                if ((NullCheck()) && (Me.CurrentTarget.Distance > 4.75) && (!Me.CurrentTarget.Auras.ContainsKey("Death Grip"))) //We need to get in meele range first
                {
                    //Ranged Instant Spells here



                    if ((ApproachDetector()))
                        return true;

                    Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                      ObjectManager.Me.CurrentTarget.Location, 2.5f));


                    return true;

                }

                if ((NullCheck()) && (Me.CurrentTarget.Distance < 2.00))
                {

                    WoWMovement.MoveStop(); //Dont Move!
                    WoWMovement.Face(); //Just in case we arent facing the right direction                
                    return false;
                }

                return false;
            }

            if ((NullCheck() && (Me.CurrentTarget.Fleeing)) && (Me.CurrentTarget.Distance > 1.75)) //We need to get in meele range first
            {


                //Ranged Instant Spells here

                if ((ApproachDetector()))
                    return true;



                Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location,
                                                                  ObjectManager.Me.CurrentTarget.Location, 1.00f));


                return true;
            }

            return false;
        }

        #endregion

        #region PvE Rotation : Level 55 DONE

        /////////////////////////////////
        //Starting Rotation.
        //Assumes no talents
        //Only Checks are Death Coil, Range and facing
        /////////////////////////////////

        public bool StartingRotation()
        {
            //Apply Dieses

            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Frost Fever")) )
            {
                if (SpellManager.CanCast("Icy Touch"))
                {
                    if (SrChecks())
                        return true;

                    Slog("#Apply Frost Fever#");

                    IcyTouch();

                    return true;

                }
            }

            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Blood Plague")) )
            {
                if (SpellManager.CanCast("Plague Strike"))
                {
                    if (SrChecks())
                        return true;

                    Slog("#Apply Blood Plague#");

                    PlagueStrike();

                    return true;

                }
            }

            //Check for Death Strike first

            if (SpellManager.Spells.ContainsKey("Death Strike"))
            {

                if ((SpellManager.CanCast("Death Strike")))
                {
                    if (SrChecks())
                        return true;

                    DeathStrike();

                    return true;
                }

                if ((SpellManager.CanCast("Blood Strike")))
                {
                    if (SrChecks())
                        return true;

                    BloodStrike();

                    return true;
                }

            }

            //if (Me.BloodRuneCount > 0)
            //{
            //}

            //Otherwise use Blood Strike

            else
            {

                if ((SpellManager.CanCast("Blood Strike")))
                {
                    if (SrChecks())
                        return true;

                    BloodStrike();

                    return true;
                }

                if (SpellManager.CanCast("Icy Touch"))
                {
                    if (SrChecks())
                        return true;

                    IcyTouch();

                    return true;

                }

                if (SpellManager.CanCast("Plague Strike"))
                {
                    if (SrChecks())
                        return true;

                    PlagueStrike();

                    return true;

                }
            }

            return false;
        }

        public bool SrChecks()
        {
            if (Me.CurrentRunicPower > 40)
            {
                if (SpellManager.CanCast("Death Coil")) //Need 40 RP to cast
                {
                    DeathCoil();
                    return true;
                }
            }

            if (HealReactor()) //Check a heal first
                return true;

            if (MoveToMelee()) //Range Checks, and faces
                return true;

            if (CastReactor())
                return true; //Deal with Casters            

            return false;
        }

        #endregion

        #region PvE Rotation : Mord Blood

        public bool MordBlood()
        {
            GetAdds(); //Check for adds while in combat

            if (!_gotAdds)
                _pestilenceCount = 0;

            //Apply Dieses

            if (UseOutbreak)
            {
                if ((NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Frost Fever"))) ||
                    (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Blood Plague"))))
                {
                    if (SpellManager.CanCast("Outbreak"))
                    {

                        Outbreak();

                        return true;

                    }
                }
            }


            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Frost Fever")))
            {
                if (SpellManager.CanCast("Icy Touch"))
                {
                    if (SrChecks())
                        return true;

                    Slog("#Apply Frost Fever#");

                    IcyTouch();

                    return true;

                }
            }

            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Blood Plague")))
            {
                if (SpellManager.CanCast("Plague Strike"))
                {
                    if (SrChecks())
                        return true;

                    Slog("#Apply Blood Plague#");

                    PlagueStrike();

                    return true;

                }
            }

            //Check for Free Blood Boil

            CsCheck();

            if (_csProc)
            {

                if ((SpellManager.CanCast("Blood Boil")))
                {

                    BloodBoil();
                    return true;
                }

            }


            if (_gotAdds)
            {
                if (_pestilenceCount >= 10)
                    _pestilenceCount = 0;

                if (_pestilenceCount == 0)
                {

                    if ((SpellManager.CanCast("Pestilence")))
                    {
                        if (CombatChecks())
                            return true;

                        Pestilence();
                        _pestilenceCount++;

                        return true;
                    }
                }


                if ((SpellManager.CanCast("Blood Boil")))
                {
                    if (CombatChecks())
                        return true;

                    BloodBoil();
                    _pestilenceCount++;
                    return true;
                }


            }

            if (SpellManager.CanCast("Heart Strike"))
            {


                if (CombatChecks())
                    return true;

                HeartStrike();
                _pestilenceCount++;
                return true;
            }

            if (SpellManager.CanCast("Death Strike"))
            {


                if (CombatChecks())
                    return true;

                DeathStrike();
                return true;
            }

            return true;


        }

        #endregion

        #region PvE Rotation : Mord Frost

        /////////////////////////////////
        //General Frost Spec based Rotation.
        /////////////////////////////////

        public bool MordFrost()
        {
            GetAdds(); //Check for adds while in combat

            if (!_gotAdds)
                _pestilenceCount = 0;

            //Apply Dieses

            if (UseOutbreak)
            {
                if (((NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Frost Fever")))) ||
                    (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Blood Plague"))))
                {
                    if (SpellManager.CanCast("Outbreak"))
                    {

                        Outbreak();

                        return true;

                    }
                }
            }


            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Frost Fever")))
            {
                if (SpellManager.CanCast("Howling Blast"))
                {
                    if (CombatChecks())
                        return true;

                    Slog("#Apply Frost Fever#");

                    HowlingBlast();

                    return true;

                }

                if (SpellManager.CanCast("Icy Touch"))
                {
                    if (CombatChecks())
                        return true;

                    Slog("#Apply Frost Fever#");

                    IcyTouch();

                    return true;

                }
            }

            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Blood Plague")))
            {
                if (SpellManager.CanCast("Plague Strike"))
                {
                    if (CombatChecks())
                        return true;

                    Slog("#Apply Blood Plague#");

                    PlagueStrike();

                    return true;

                }
            }

            //Check for Free Howling Blast

            RimeCheck();

            if (_rimeProc)
            {

                if (RimeHb)
                {

                    if ((SpellManager.CanCast("Howling Blast")))
                    {

                        HowlingBlast();

                        return true;
                    }

                }

                if (RimeIcyTouch)
                {

                    if ((SpellManager.CanCast("Icy Touch")))
                    {

                        IcyTouch();

                        return true;
                    }

                }

            }

            if (_gotAdds)
            {
                if (_pestilenceCount >= 10)
                    _pestilenceCount = 0;

                if (_pestilenceCount == 0)
                {

                    if ((SpellManager.CanCast("Pestilence")))
                    {
                        if (CombatChecks())
                            return true;

                        Pestilence();
                        _pestilenceCount++;

                        return true;
                    }
                }

                if ((UsePillar) && (PillarAdds))
                {
                    if (SpellManager.CanCast("Pillar of Frost"))
                    {

                        if (CombatChecks())
                            return true;

                        PillarofFrost();
                        _pestilenceCount++;
                        return true;
                    }
                }

            }

            if (SpellManager.CanCast("Obliterate"))
            {


                if (CombatChecks())
                    return true;

                Obliterate();
                _pestilenceCount++;
                return true;
            }

            return true;
        }

        #endregion

        #region PvE Rotation : Mord Unholy

        /////////////////////////////////
        //General Unholy Spec based Rotation. Requires level 58
        /////////////////////////////////

        public bool MordUnholy()
        {
            GetAdds(); //Check for adds while in combat

            if (!_gotAdds)
                _pestilenceCount = 0;

            //Apply Dieses

            if (UseOutbreak)
            {
                if ((NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Frost Fever")) ) ||
                    (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Blood Plague"))))
                {
                    if (SpellManager.CanCast("Outbreak"))
                    {

                        Outbreak();

                        return true;

                    }
                }
            }


            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Frost Fever")))
            {

                if (SpellManager.CanCast("Icy Touch"))
                {
                    if (CombatChecks())
                        return true;

                    Slog("#Apply Frost Fever#");

                    IcyTouch();

                    return true;

                }
            }

            if (NullCheck() && (!Me.CurrentTarget.Auras.ContainsKey("Blood Plague")))
            {
                if (SpellManager.CanCast("Plague Strike"))
                {
                    if (CombatChecks())
                        return true;

                    Slog("#Apply Blood Plague#");

                    PlagueStrike();

                    return true;

                }
            }

            if (_gotAdds)
            {
                if (_pestilenceCount >= 10)
                    _pestilenceCount = 0;

                if (_pestilenceCount == 0)
                {

                    if ((SpellManager.CanCast("Pestilence")))
                    {
                        if (CombatChecks())
                            return true;

                        Pestilence();
                        _pestilenceCount++;

                        return true;
                    }
                }

            }

            if (SpellManager.CanCast("Scourge Strike"))
            {
                if (CombatChecks())
                    return true;

                ScourgeStrike();

                return true;

            }

            if (SpellManager.CanCast("Festering Strike"))
            {
                if (CombatChecks())
                    return true;

                FesteringStrike();

                return true;

            }
            

            return true;
        }

        #endregion

        #region PvE Cast Reactor DONE

        /////////////////////////////////
        //This will detect casters and deal with them
        /////////////////////////////////

        public bool CastReactor()
        {
            if ((NullCheck()) && (Me.CurrentTarget.IsCasting)) //If target starts to cast, move in.
            {
                Slog("#Target is casting#");

                if ((NullCheck()) && (Me.CurrentTarget.Distance <= 4))
                {
                    if ((UseMindFreeze) && (SpellManager.CanCast("Mind Freeze")))
                    //Interupt the cast if MindFreeze is available
                    {
                        MindFreeze();
                        return true;
                    }
                }

                if ((NullCheck()) && (UseStrangulate) && (UseStrangulateMelee) && (SpellManager.CanCast("Strangulate")) && (Me.CurrentTarget.Distance <= 29)) //Strangulate to Interupt Caster
                {
                    Slog("#Strangulate#");
                    Strangulate();
                    return true;
                }

                if ((NullCheck()) && (UseStrangulate) && (SpellManager.CanCast("Strangulate")) && (Me.CurrentTarget.Distance <= 29) &&
                    (Me.CurrentTarget.Distance > 4)) //Strangulate to Interupt Caster
                {
                    Slog("#Too far away! Interupt using Strangulate#");
                    Strangulate();
                    return true;
                }

                if ((NullCheck()) && (UseStrangulate) && (UseDeathGripInterupt) && (!SpellManager.CanCast("Strangulate")) &&
                    (SpellManager.CanCast("Death Grip")) && (Me.CurrentTarget.Distance <= 29) &&
                    (Me.CurrentTarget.Distance > 4)) //If St on CD cast DeathGrip
                {
                    Slog("Too far away! Strangulate not ready! Use Deathgrip.");
                    DeathGrip();
                    return true;
                }

                if ((NullCheck()) && (UseWarStomp) && (WarStompCasters) && (SpellManager.CanCast("War Stomp")) &&
                    (Me.CurrentTarget.Distance <= 8))
                {
                    WarStomp();
                    return true;
                }

                if ((NullCheck()) && (UseAt) && (SpellManager.CanCast("Arcane Torrent")) && (Me.CurrentTarget.Distance <= 8))
                {
                    At();
                    return true;
                }

                if ((UseAntiMagicShell) && (SpellManager.CanCast("Anti-Magic Shell")))
                {
                    Slog("#Interupt is on cooldown, Cast Shell#");
                    AntiMagicShell();
                    return true;
                }

                return false;
            }

            return false;
        }

        #endregion

        #region PvE Flee Reactor DONE

        /////////////////////////////////
        //This will detect casters and deal with them
        /////////////////////////////////

        public bool FleeReactor()
        {
            if ((NullCheck()) && (Me.CurrentTarget.Fleeing)) //If target starts to cast, move in.
            {
                Slog("#Target is running#");

                if ((NullCheck()) && (Me.CurrentTarget.Distance <= 29)) //If we are not in range, dont bother
                {
                    if (UseChainsOfIce)
                    {
                        if ((NullCheck()) && (!Me.CurrentTarget.Auras.ContainsKey("Chains of Ice")))
                        {
                            if (SpellManager.CanCast("Chains of Ice"))
                            {
                                Slog("#Freeze!#");
                                Chains();
                                return true;
                            }
                        }
                    }

                    if (UseDeathGripRunners)
                    {
                        if (SpellManager.CanCast("Death Grip"))
                        {
                            Slog("#Get over here!#");
                            DeathGrip();
                            return true;
                        }
                    }

                    if (UseDarkCommandRunners)
                    {
                        if (SpellManager.CanCast("Dark Command")) //Try Dark Command
                        {
                            Slog("#I, Vigo, the Scourge of Carpathia, the Sorrow of Moldavia, Command You!#");
                            DarkCommand();
                            return true;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        #endregion

        #region PvE Heal Reactor DONE

        /////////////////////////////////
        //This will decide if we need a heal
        /////////////////////////////////

        public bool HealReactor()
        {
            if (_gotAdds)
            {
                if (!Me.Auras.ContainsKey("Lichborne"))
                {
                    if (SpellManager.CanCast("Death Coil"))
                    {
                        Me.Target();
                        Thread.Sleep(125);
                        DeathCoil();
                        Me.TargetLastTarget();
                        return true;
                    }
                }

                if (Me.Stunned)
                {
                    if (UseEm)
                    {
                        if (SpellManager.CanCast("Every Man for Himself")) //Get Unstunned
                        {
                            Em();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= RuneTapHealth) //Rune Tap
                {
                    if (UseRuneTap)
                    {
                        if (SpellManager.CanCast("Rune Tap"))
                        {
                            RuneTap();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= LichborneHealth) //Lichborne
                {
                    if (UseLichborne)
                    {
                        if ((SpellManager.CanCast("Lichborne")) && (SpellManager.CanCast("Death Coil")))
                        {
                            
                            Lichborne();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= DeathPactHealth) //Death Pact
                {
                    if (UseDeathPact)
                    {
                        if (Me.GotAlivePet)
                        {
                            if (SpellManager.CanCast("Death Pact"))
                            {
                                DeathPact();
                                return true;
                            }

                        }
                    }
                }

                if (VampiricBloodAdds) //Cast VB when we get adds
                {
                    if (UseVampiricBlood)
                    {
                        if (SpellManager.CanCast("Vampiric Blood"))
                        {
                            VampiricBlood();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= VampiricBloodHealth) //Vampiric Blood 
                {
                    if (UseVampiricBlood)
                    {
                        if ((NullCheck()) && (Me.CurrentTarget.CurrentHealth > CooldownHealth))
                        //Dont waste it if target is near death
                        {
                            if (SpellManager.CanCast("Vampiric Blood"))
                            {
                                VampiricBlood();
                                return true;
                            }
                        }
                    }
                }

                if (IceboundFortitudeAdds) //Cast VB when we get adds
                {
                    if (UseIceboundFortitude)
                    {
                        if (SpellManager.CanCast("Icebound Fortitude"))
                        {
                            IceboundFortitude();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= IceboundFortitudeHealth) //Icebound Fortitude
                {
                    if (UseIceboundFortitude)
                    {
                        if ((NullCheck()) && (Me.CurrentTarget.CurrentHealth > CooldownHealth))
                        //Dont waste it if target is near death
                        {
                            if (SpellManager.CanCast("Icebound Fortitude"))
                            {
                                IceboundFortitude();
                                return true;
                            }
                        }
                    }
                }

                if (LifeBloodAdds)
                {
                    if (UseLifeBlood)
                    {
                        if ((SpellManager.CanCast("Lifeblood")))
                        {
                            LifeBlood();
                            return true;
                        }
                    }
                }

                else
                {
                    if (Me.HealthPercent <= LifeBloodHealth)
                    {
                        if (UseLifeBlood)
                        {
                            if (SpellManager.CanCast("Lifeblood")) //Use Lifeblood when low on health
                            {
                                LifeBlood();
                                return true;
                            }
                        }
                    }
                }

                if (BloodFuryAdds)
                {
                    if (UseBloodFury)
                    {
                        if ((SpellManager.CanCast("Blood Fury")))
                        {
                            BloodFury();
                            return true;
                        }
                    }
                }

                else
                {
                    if (Me.HealthPercent <= BloodFuryHealth)
                    {
                        if (UseBloodFury)
                        {
                            if (SpellManager.CanCast("Blood Fury")) //Use Blood Fury when low on health
                            {
                                BloodFury();
                                return true;
                            }
                        }
                    }
                }

                if (NaaruAdds)
                {
                    if (UseNaaru)
                    {
                        if ((SpellManager.CanCast("Gift of the Naaru")))
                        {
                            Naaru();
                            return true;
                        }
                    }
                }

                else
                {
                    if (Me.HealthPercent <= NaaruHealth)
                    {
                        if (UseNaaru)
                        {
                            if (SpellManager.CanCast("Gift of the Naaru")) //Use Naaru when low on health
                            {
                                Naaru();
                                return true;
                            }
                        }
                    }
                }

                if (WarStompAdds)
                {
                    if (UseWarStomp)
                    {
                        if ((SpellManager.CanCast("Warstomp")))
                        {
                            WarStomp();
                            return true;
                        }
                    }
                }

                else
                {
                    if (Me.HealthPercent <= WarStompHealth)
                    {
                        if (UseWarStomp)
                        {
                            if (SpellManager.CanCast("War Stomp"))
                            {
                                if ((NullCheck()) && (Me.CurrentTarget.Distance <= 8)) //Use WarStomp when low on health
                                {
                                    WarStomp();
                                    return true;
                                }
                            }
                        }
                    }
                }

                if (SfAdds)
                {
                    if (UseStoneForm)
                    {
                        if ((SpellManager.CanCast("Stoneform")))
                        {
                            Sf();
                            return true;
                        }
                    }
                }

                else
                {
                    if (Me.HealthPercent <= StoneFormHealth)
                    {
                        if (UseStoneForm)
                        {
                            if (SpellManager.CanCast("Stoneform")) //Use StoneForm when low on health
                            {
                                Sf();
                                return true;
                            }
                        }
                    }
                }

                if (UseErw)
                {
                    if (UseErwAdds)
                    {

                        if (SpellManager.CanCast("Empower Rune Weapon"))
                        {

                            ERW();
                            _pestilenceCount++;
                            return true;

                        }

                    }

                }

                if (UseUnholyFrenzy)
                {

                    if (SpellManager.CanCast("Unholy Frenzy"))
                    {

                        UnholyFrenzy();
                        _pestilenceCount++;
                        return true;

                    }

                }
            }

            else //No adds
            {
                if (Me.Stunned)
                {
                    if (UseEm)
                    {
                        if (SpellManager.CanCast("Every Man for Himself")) //Get Unstunned
                        {
                            Em();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= RuneTapHealth) //Rune Tap
                {
                    if (UseRuneTap)
                    {
                        if (SpellManager.CanCast("Rune Tap"))
                        {
                            RuneTap();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= DeathPactHealth) //Death Pact
                {
                    if (UseDeathPact)
                    {
                        if (Me.GotAlivePet)
                        {
                            if (SpellManager.CanCast("Death Pact"))
                            {
                                DeathPact();
                                return true;
                            }

                        }
                    }
                }

                if (Me.HealthPercent <= VampiricBloodHealth) //Vampiric Blood 
                {
                    if (UseVampiricBlood)
                    {
                        if ((NullCheck()) && (Me.CurrentTarget.CurrentHealth > CooldownHealth))
                        //Dont waste it if target is near death
                        {
                            if (SpellManager.CanCast("Vampiric Blood"))
                            {
                                VampiricBlood();
                                return true;
                            }
                        }
                    }
                }

                if (Me.HealthPercent <= IceboundFortitudeHealth) //Icebound Fortitude
                {
                    if (UseIceboundFortitude)
                    {
                        if ((NullCheck()) && (Me.CurrentTarget.CurrentHealth > CooldownHealth))
                        //Dont waste it if target is near death
                        {
                            if (SpellManager.CanCast("Icebound Fortitude"))
                            {
                                IceboundFortitude();
                                return true;
                            }
                        }
                    }
                }


                if (Me.HealthPercent <= BloodFuryHealth)
                {
                    if (UseBloodFury)
                    {
                        if (SpellManager.CanCast("Blood Fury")) //Use Blood Fury when low on health
                        {
                            BloodFury();
                            return true;
                        }
                    }
                }


                if (Me.HealthPercent <= LifeBloodHealth)
                {
                    if (UseLifeBlood)
                    {
                        if (SpellManager.CanCast("Lifeblood")) //Use Lifeblood when low on health
                        {
                            LifeBlood();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= NaaruHealth)
                {
                    if (UseNaaru)
                    {
                        if (SpellManager.CanCast("Gift of the Naaru")) //Use Naaru when low on health
                        {
                            Naaru();
                            return true;
                        }
                    }
                }

                if (Me.HealthPercent <= WarStompHealth)
                {
                    if (UseWarStomp)
                    {
                        if (SpellManager.CanCast("War Stomp"))
                        {
                            if ((NullCheck()) && (Me.CurrentTarget.Distance <= 8)) //Use WarStomp when low on health
                            {
                                WarStomp();
                                return true;
                            }
                        }
                    }
                }


                if (Me.HealthPercent <= StoneFormHealth)
                {
                    if (UseStoneForm)
                    {
                        if (SpellManager.CanCast("Stoneform")) //Use StoneForm when low on health
                        {
                            Sf();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region PvE ApproachDetector DONE

        /////////////////////////////////
        //This will detect if a mob is approaching
        /////////////////////////////////

        public bool ApproachDetector()
        {
            if (NullCheck())
            {
                double aDone = Me.CurrentTarget.Distance;
                Thread.Sleep(125);
                double aDtwo = Me.CurrentTarget.Distance;

                if (aDone > aDtwo) //Difference between the two
                {
                    return true;
                }

                return false;
            }
            return false;
        }

        #endregion

        #region PvE Adds Reactor DONE

        //Credit to Hawker for getAdds()

        private void GetAdds()
        {
            var longList = ObjectManager.ObjectList;

            var mobList = (from thing in longList where (int)thing.Type == 3 select thing.ToUnit()).ToList();

            var enemyMobList = mobList.Where(thing => (thing.Guid != Me.Guid) && (thing.IsTargetingMeOrPet) && (!thing.Name.ToLower().Contains("rotting w"))).ToList();
            if (enemyMobList.Count >= AddsCount)
            {
                if (!_addSpam)
                    Slog("#Warning: We have " + enemyMobList.Count + " attackers.#");

                _gotAdds = true;

                _addSpam = true;
            }

            else
            {
                _gotAdds = false;

                _addSpam = false;
            }

            return;
        }
        #endregion

        #region PvE PlayerDetector DONE

        //Credit to Hawker for ScanForPlayers()

        private void ScanForPlayers()
        {

            foreach (var newPlayer in from player in ObjectManager.ObjectList
                                      where player.Type == WoWObjectType.Player && player.Guid != Me.Guid
                                      select player.ToPlayer())
            {
                if (newPlayer.Distance >= PdRange) continue;
                var d = Math.Round(newPlayer.Distance, 1);
                Slog("#Player Named: " + newPlayer.Name + " Is " + d + " yards away.#");
                SystemSounds.Exclamation.Play();
            }
            return;
        }

        #endregion

        #region PvE Opener DONE

        /////////////////////////////////
        //Decide an opener
        /////////////////////////////////

        public bool Opener()
        {
            if ((Me.GotTarget) && (OpenWithDeathGrip) && (SpellManager.CanCast("Death Grip")) && (Me.CurrentTarget.Distance <= 29))
            {
                Slog("#Casting Opener : Death Grip#");
                WoWMovement.MoveStop();
                Thread.Sleep(125);
                WoWMovement.Face();
                Thread.Sleep(125);

                DeathGrip(); //Do Opener

                if (PlayerAlert)
                    ScanForPlayers(); //Find players near

                _openerComplete = true;

                return true;
            }

            if ((Me.GotTarget) && (OpenWithDarkCommand) && (SpellManager.CanCast("Dark Command")) &&
                (Me.CurrentTarget.Distance <= 29))
            {
                Slog("#Casting Opener : Dark Command#");
                WoWMovement.MoveStop();
                Thread.Sleep(125);
                WoWMovement.Face();
                Thread.Sleep(125);

                DarkCommand(); //Do Opener 

                if (PlayerAlert)
                    ScanForPlayers(); //Find players near

                _openerComplete = true;

                return true;
            }


            if ((Me.GotTarget) && (OpenWithIcyTouch) && (SpellManager.CanCast("Icy Touch")) &&
                (Me.CurrentTarget.Distance <= IcyTouchRange))
            {
                Slog("#Casting Opener : Icy Touch#");
                WoWMovement.MoveStop();
                Thread.Sleep(125);
                WoWMovement.Face();
                Thread.Sleep(125);

                IcyTouch(); //Do Opener

                if (PlayerAlert)
                    ScanForPlayers(); //Find players near

                _openerComplete = true;

                return true;
            }


            if (((Me.GotTarget) && (OpenWithDeathGrip) && (!SpellManager.CanCast("Death Grip"))) ||
                ((Me.GotTarget) && (OpenWithDarkCommand) && (!SpellManager.CanCast("Death Grip"))) && (OpenWithIcyTouchBackup) &&
                (SpellManager.CanCast("IcyTouch")) && (Me.CurrentTarget.Distance <= IcyTouchRange))
            {
                Slog("#Casting Opener : Icy Touch#");
                WoWMovement.MoveStop();
                Thread.Sleep(125);
                WoWMovement.Face();
                Thread.Sleep(125);

                IcyTouch(); //Do Opener

                if (PlayerAlert)
                    ScanForPlayers(); //Find players near

                _openerComplete = true;

                return true;
            }

            return false;
        }

        #endregion

        #region PVE Run Check DONE

        /////////////////////////////////
        //Try to fix HB Movement and targeting Bugs
        /////////////////////////////////

        public bool RunCheck()
        {


            WoWMovement.Face(); //We want to face our target by any means.            

            if (Me.IsMoving) //Make sure we arent running all over the place
            {
                if ((NullCheck()) && (!Me.CurrentTarget.Fleeing))
                {
                    if ((NullCheck()) && (!Me.CurrentTarget.IsCasting))
                    {
                        if ((NullCheck()) && (Me.CurrentTarget.Distance < 2.0))
                        {
                            WoWMovement.MoveStop();

                            return true;
                        }
                    }
                }
            }

            return false;

        }

        #endregion

        #region PvE Runic Power Reactor DONE

        /////////////////////////////////
        //This will keep track of runic power and use it
        /////////////////////////////////

        public bool RunicPowerReactor()
        {
            if ((!SpellManager.Spells.ContainsKey("Heart Strike")) && (!SpellManager.Spells.ContainsKey("Frost Strike")) && (!SpellManager.Spells.ContainsKey("Scourge Strike"))) //Basic Rotation
            {
                if (UseRaiseDead) //Cast Raise Dead now!
                {
                    if (SpellManager.CanCast("Raise Dead"))
                    {
                        RaiseDead();
                        return true;
                    }
                }

                if (SpellManager.CanCast("Death Coil"))
                {
                    if (!_drwToggle)
                    {
                        DeathCoil();
                        _pestilenceCount++;
                        return true;
                    }
                }
            }
            //Blood
            if (SpellManager.Spells.ContainsKey("Heart Strike")) //Blood Rotation
            {

                if (UseRaiseDead) //Cast Raise Dead now!
                {
                    if (SpellManager.CanCast("Raise Dead"))
                    {
                        RaiseDead();
                        return true;
                    }
                }

                if (UseDrw)
                {
                    if (SpellManager.Spells.ContainsKey("Dancing Rune Weapon"))
                    {
                        if ((NullCheck()) && (Me.CurrentTarget.CurrentHealth > CooldownHealth))
                        {
                            WoWSpell spell = SpellManager.Spells["Dancing Rune Weapon"];

                            if (!spell.Cooldown) //We may not have enoph RP to cast. So check if its on CD
                            {
                                if (((DrwAdds) && (_gotAdds)) || (!DrwAdds))
                                {
                                    if (!_drwSpam)
                                        Slog("#Dancing Rune Weapon Ready.... Disable RP Dump#");

                                    _drwToggle = true;

                                    _drwSpam = true;
                                }
                            }
                        }
                    }
                }

                if ((NullCheck()) && (_drwToggle) && (Me.CurrentRunicPower > 60) && (Me.CurrentTarget.CurrentHealth > CooldownHealth))
                //Need 60 RP to cast
                {
                    Drw();
                    _pestilenceCount++;
                    return true;
                }

                if ((NullCheck()) && (_drwToggle) && (Me.CurrentRunicPower > 60) && (Me.CurrentTarget.CurrentHealth < CooldownHealth))
                //Need 60 RP to cast
                {

                    _drwToggle = false;

                    return false;
                }

                if (UseRuneStrike)
                {

                    if (SpellManager.CanCast("Rune Strike"))
                    {


                        if ((UseRuneStrike) && (!_drwToggle))
                        {
                            RuneStrike();
                            _pestilenceCount++;
                            return true;
                        }

                    }
                }

                if (SpellManager.CanCast("Death Coil"))
                {
                    if (!_drwToggle)
                    {
                        DeathCoil();
                        _pestilenceCount++;
                        return true;
                    }
                }

            }

            //Frost 

            if (SpellManager.Spells.ContainsKey("Frost Strike")) 
            {

                if (UseRaiseDead) //Cast Raise Dead now!
                {
                    if (SpellManager.CanCast("Raise Dead"))
                    {
                        RaiseDead();
                        return true;
                    }
                }

                if ((UseHc) && (_gotAdds))
                {

                    if ((SpellManager.CanCast("Hungering Cold")))
                    {

                        HungeringCold();
                        _pestilenceCount++;
                        return true;
                    }
                }

                if (UseRuneStrike)
                {

                    if (SpellManager.CanCast("Rune Strike"))
                    {


                        if ((UseRuneStrike) && (!_hcToggle))
                        {
                            RuneStrike();
                            _pestilenceCount++;
                            return true;
                        }

                    }
                }

                if (SpellManager.CanCast("Frost Strike"))
                {

                    FrostStrike();
                    _pestilenceCount++;
                    return true;

                }

                if (SpellManager.CanCast("Death Coil"))
                {

                    DeathCoil();
                    _pestilenceCount++;
                    return true;

                }

            }

            //Unholy
            if (SpellManager.Spells.ContainsKey("Scourge Strike")) //Unholy Rotation
            {
                if ((UseRaiseDead) && (!Me.GotAlivePet))//Cast Raise Dead now!
                {
                    if (SpellManager.CanCast("Raise Dead"))
                    {
                        RaiseDead();
                        return true;
                    }
                }

                if (UseSummonGargoyle)
                {
                    if (SpellManager.Spells.ContainsKey("Summon Gargoyle"))
                    {
                        WoWSpell spell = SpellManager.Spells["Summon Gargoyle"];

                        if (!spell.Cooldown) //We may not have enoph RP to cast. So check if its on CD
                        {
                            if (!_sgSpam)
                                Slog("#Summon Gargoyle Ready.... Disable RP Dump#");

                            _sgToggle = true;

                            _sgSpam = true;
                        }
                    }
                }

                if (UseDarkTransformation)
                {
                    if (SpellManager.CanCast("Dark Transformation"))
                    
                    {
                        DarkTransformation();
                        return true;
                    }

                }

                if ((_sgToggle) && (Me.CurrentRunicPower > 60) && (SpellManager.CanCast("Summon Gargoyle")))
                //Need 60 RP to cast
                {
                    SummonGargoyle();
                    _pestilenceCount++;
                    _sgToggle = false;
                    return true;
                }

                if (UseRuneStrike)
                {

                    if (SpellManager.CanCast("Rune Strike"))
                    {


                        if ((UseRuneStrike) && (!_sgToggle))
                        {
                            RuneStrike();
                            _pestilenceCount++;
                            return true;
                        }

                    }

                }

                if ((SpellManager.CanCast("Death Coil")) && (!_sgToggle))
                {

                    DeathCoil();
                    _pestilenceCount++;
                    return true;

                }

            }

            return false;
        }

        #endregion

        #region PvE Cooldown Reactor DONE

        /////////////////////////////////
        //This will detect cooldown abilities and use them
        /////////////////////////////////

        public bool CdReactor() //Change to buff reactor when casting works
        {

            if ((UsePillar) && (Me.HealthPercent <= PillarHealth))
            {
                if (SpellManager.CanCast("Pillar of Frost"))
                {
                    PillarofFrost();
                    _pestilenceCount++;
                    return true;
                }
            }


            if (UseBloodTap)
            {
                if (Me.BloodRuneCount == 0)
                {

                    if (SpellManager.CanCast("Blood Tap"))
                    {

                        BloodTap();
                        _pestilenceCount++;
                        return true;

                    }

                }

            }

            if (UseErw)
            {
                if (UseErwRunes)
                {
                    if ((Me.BloodRuneCount == 0) && (Me.FrostRuneCount == 0) && (Me.UnholyRuneCount == 0) &&
                        (Me.DeathRuneCount == 0))
                    {

                        if (SpellManager.CanCast("Empower Rune Weapon"))
                        {

                            ERW();
                            _pestilenceCount++;
                            return true;

                        }

                    }

                }

            }

            return false;
        }

        #endregion

        #region PvE EvadeCheck

        /////////////////////////////////////
        //Check for evades and blacklist those evades. Credit?
        /////////////////////////////////////

        #endregion

        #region PvE Target Error Check DONE

        public bool TargetErrorCheck()
        {
            if (((NullCheck())) && (!Me.CurrentTarget.IsPlayer)) //Got a target, now lets check its properties. Skip this if its a player.
            {
                //If we have a target, but our target is not attacking us or pet. Clear, stop, retarget.
                if ((NullCheck()) && (!Me.CurrentTarget.Dead) && (Me.CurrentTarget.CurrentTargetGuid != Me.Guid) && (Me.CurrentTarget.CurrentTargetGuid != Me.CurrentTargetGuid))
                {
                    if (Me.GotAlivePet) //Have a pet?
                    {
                        if ((NullCheck()) && (Me.CurrentTarget.CurrentTargetGuid != Me.Pet.Guid)) //Target isnt targeting our pet
                        {

                            if ((NullCheck()) && (!Me.CurrentTarget.Fleeing))//The target isnt running either, so lets blacklist
                            {
                                WoWMovement.MoveStop();
                                Thread.Sleep(125);
                                Blacklist.Add(Me.CurrentTarget, TimeSpan.FromSeconds(5.00));
                                Me.ClearTarget();
                                return false;
                            }
                        }
                    }
                    else //Dont have a pet
                    {
                        if ((NullCheck()) && (!Me.CurrentTarget.Fleeing) && !Me.CurrentTarget.IsTargetingAnyMinion)//The target isnt running either, so lets blacklist
                        {
                            WoWMovement.MoveStop();
                            Thread.Sleep(125);
                            Blacklist.Add(Me.CurrentTarget, TimeSpan.FromSeconds(5.00));
                            Me.ClearTarget();
                            return false;
                        }
                    }
                }

            }

            if ((Me.GotTarget) && (Me.CurrentTarget.Dead))
            {
                Me.ClearTarget();
                return false;
            }

            return true;

        }

        #endregion

        #region PvE Combat Checks

        public bool CombatChecks()
        {


            if (FleeReactor()) //Is target running?
                return true;

            if (HealReactor()) //Check a heal first
                return true;

            if (MoveToMelee()) //Range Checks, and faces
                return true;

            if (CastReactor())
                return true; //Deal with Casters

            if (RunicPowerReactor()) //Dump RP if we have it
                return true;

            if (CdReactor()) //Check instant abilities and use if needed
                return true;

            //EvadeCheck();

            return false;
        }

        #endregion

        #endregion

        #region Global Logic

        #region Global Need Rest DONE

        /////////////////////////////////
        //This pulses 8 times per second while out of combat
        /////////////////////////////////

        public override bool NeedRest
        {
            get
            {
                if (_pullTimer.IsRunning)
                    _pullTimer.Reset(); //If timer is still running, stop it.

                return Me.GetPowerPercent(WoWPowerType.Health) <= RestHealth;
            }
        }

        #endregion

        #region Global Rest DONE

        public override void Rest()
        {

            if ((Me.HealthPercent <= RestHealth))
            {
                Slog("Health is at " + Me.HealthPercent + "%, Eat.");

                Styx.Logic.Common.Rest.Feed();
            }
        }

        #endregion

        #region Global Null Check DONE

        public bool NullCheck()
        {
            if (!Me.GotTarget) //Ok we dont have a target so let check a few things
            {

                if (Me.GotAlivePet)
                {
                    if (Me.Pet.GotTarget) //Got pet, and pet has target
                    {
                        Me.Pet.CurrentTarget.Target(); //Target pets target all is good.
                        return true;
                    }

                }

                foreach (WoWUnit hostile in Targeting.Instance.TargetList.Where(hostile => hostile.IsTargetingAnyMinion))
                {
                    WoWMovement.MoveStop();
                    hostile.Target();
                    Thread.Sleep(500);
                    return true;
                }

                 //No pet, lets assume somthing is wrong and have HB correct itself
                 return false;

            }

            return true;

        }

        #endregion

        #region Global Party Check DONE

        public bool InParty()
        {
            if (Me.PartyMembers.Count >= 1)
                return true;

            return false;
        }

        #endregion

        #region Global Leader Check DONE

        public bool IsLeader()
        {
            if (Me.IsGroupLeader)
                return true;

            return false;
        }

        #endregion

        #region Global BGCheck DONE

        /////////////////////////////////
        //Check to see if we are in a BG (Credit to Bobby for BG Check)
        /////////////////////////////////

        public bool InBg()
        {
            return Battlegrounds.IsInsideBattleground;
        }

        #endregion

        #region Global Auto Attack DONE

        /////////////////////////////////
        //Simply makes sure we are attacking
        /////////////////////////////////

        public bool AutoAttack()
        {
            if (NullCheck())
            {
                if (!Me.IsAutoAttacking)
                {
                    if (!Me.CurrentTarget.Dead)
                    {
                        Slog("#Combat Stance Enabled#");
                        Lua.DoString("StartAttack()");
                        return true;
                    }

                    return false;
                }

                return false;

            }

            return false;
        }

        #endregion

        #region Global Rime Check

        /////////////////////////////////
        //Detect Rime (Credit to Bobby for code)
        /////////////////////////////////

        public void RimeCheck()
        {
            const string cRime = "Freezing Fog";
            Lua.DoString("buffName,_,_,stackCount,_,_,_,_,_=UnitBuff(\"player\",\"" + cRime + "\")");
            string buffName = Lua.GetLocalizedText("buffName", Me.BaseAddress);


            if (buffName == cRime)
            {
                _rimeProc = true;
                Slog("#Freezing Fog Detected#");
            }

            else
                _rimeProc = false;
        }

        #endregion

        #region Global Crimson Scourge

        /////////////////////////////////
        //Detect Rime (Credit to Bobby for code)
        /////////////////////////////////

        public void CsCheck()
        {
            const string cCrimsonScourge = "Crimson Scourge";
            Lua.DoString("buffName,_,_,stackCount,_,_,_,_,_=UnitBuff(\"player\",\"" + cCrimsonScourge + "\")");
            string buffName = Lua.GetLocalizedText("buffName", Me.BaseAddress);


            if (buffName == cCrimsonScourge)
            {
                _csProc = true;
                Slog("#Crimson Scourge Detected#");
            }

            else
                _csProc = false;
        }

        #endregion

        #region Global Spells

        #region Blood Spells

        /////////////////////////////////////
        //Blood Strike (Req Level 55) Blood
        /////////////////////////////////////

        private void BloodStrike()
        {
            SpellManager.Cast("Blood Strike"); //Do It!                       


            Slog("**Blood Strike**");
        }

        /////////////////////////////////////
        //Rune Tap (Req 10 Point Blood Talent)
        /////////////////////////////////////

        private void RuneTap()
        {
            SpellManager.Cast("Rune Tap"); //Do It!                       


            Slog("**Rune Tap**");
        }

        /////////////////////////////////////
        //Blood Tap
        /////////////////////////////////////

        private void BloodTap()
        {
            SpellManager.Cast("Blood Tap"); //Do It!                       


            Slog("**Blood Tap**");
        }

        /////////////////////////////////////
        //Pestilence (Req Level 56) Blood
        /////////////////////////////////////

        private void Pestilence()
        {
            SpellManager.Cast("Pestilence"); //Do It!                       


            Slog("**Pestilence**");
        }

        /////////////////////////////////////
        //Heart Strike (Req 40 Blood Talen) Blood
        /////////////////////////////////////

        private void HeartStrike()
        {
            SpellManager.Cast("Heart Strike"); //Do It!                       


            Slog("**Heart Strike**");
        }

        /////////////////////////////////////
        //Blood Boil (Req Level 58) Blood
        /////////////////////////////////////

        private void BloodBoil()
        {
            SpellManager.Cast("Blood Boil"); //Do It!                       


            Slog("**Blood Boil**");
        }

        /////////////////////////////////////
        //Strangulate (Req Level 59) Blood
        /////////////////////////////////////

        private void Strangulate()
        {
            SpellManager.Cast("Strangulate"); //Do It!


            Slog("**Strangulate**");
        }

        /////////////////////////////////////
        //Vampiric Blood (Req 35 Blood Talent) Blood
        /////////////////////////////////////

        private void VampiricBlood()
        {
            SpellManager.Cast("Vampiric Blood"); //Do It!


            Slog("#Heath is low... **Vampiric Blood** #");
        }

        #endregion

        #region Frost Spells

        /////////////////////////////////
        //Icy Touch (Req Level 55) Frost
        /////////////////////////////////

        private void IcyTouch()
        {
            SpellManager.Cast("Icy Touch"); //Do It!                                                


            Slog("**Icy Touch**");
        }

        /////////////////////////////////
        //Chains of Ice (Req Level 58) Frost
        /////////////////////////////////

        private void Chains()
        {
            SpellManager.Cast("Chains of Ice"); //Do It!                                                


            Slog("**Chains of Ice**");
        }

        /////////////////////////////////////
        //Icebound Fortitude (Req Level 62) Frost
        /////////////////////////////////////

        private void IceboundFortitude()
        {
            SpellManager.Cast("Icebound Fortitude"); //Do It!


            Slog("Icebound Fortitude");
        }

        /////////////////////////////////////
        //Lichborne (Req 10 Frost Talent) Frost
        /////////////////////////////////////

        private void Lichborne()
        {
            SpellManager.Cast("Lichborne"); //Do It!


            Slog("**Lichborne**");
        }

        /////////////////////////////////////
        //Hungering Cold (Req 30 Frost Talent) Frost
        /////////////////////////////////////

        private void HungeringCold()
        {
            SpellManager.Cast("Hungering Cold"); //Do It!


            Slog("**Hungering Cold**");
        }

        /////////////////////////////////////
        //Howling Blast (Req 50 Frost Talent) Frost
        /////////////////////////////////////

        private void HowlingBlast()
        {
            SpellManager.Cast("Howling Blast"); //Do It!                         


            Slog("**Howling Blast**");
        }

        /////////////////////////////////////
        //Frost Strike (Req 40 Frost Talent) Frost
        /////////////////////////////////////

        private void FrostStrike()
        {
            SpellManager.Cast("Frost Strike"); //Do It!                       


            Slog("**Frost Strike**");
        }

        /////////////////////////////////////
        //Festering Strike  Frost
        /////////////////////////////////////

        private void FesteringStrike()
        {
            SpellManager.Cast("Festering Strike"); //Do It!                       


            Slog("**Festering Strike**");
        }

        /////////////////////////////////////
        //Path of Frost (Req Level 68) Frost
        /////////////////////////////////////

        private void PathofFrost()
        {
            SpellManager.Cast("Path of Frost"); //Do It!


            Slog("**Path of Frost**");
        }

        /////////////////////////////////////
        //Pillar of Frost
        /////////////////////////////////////

        private void PillarofFrost()
        {
            SpellManager.Cast("Pillar of Frost"); //Do It!


            Slog("**Pillar of Frost**");
        }

        /////////////////////////////////////
        //Empower Rune Weapon
        /////////////////////////////////////

        private void ERW()
        {
            SpellManager.Cast("Empower Rune Weapon"); //Do It!


            Slog("**Empower Rune Weapon**");
        }

        #endregion

        #region Unholy Spells

        /////////////////////////////////////
        //Plague Strike (Req Level 55) Unholy
        /////////////////////////////////////

        private void PlagueStrike()
        {
            SpellManager.Cast("Plague Strike"); //Do It!                        


            Slog("**Plague Strike**");
        }

        /////////////////////////////////////
        //Bone Shield (Requires 35 Unholy Talent)
        /////////////////////////////////////

        public void BoneShield()
        {
            WoWMovement.MoveStop();
            Thread.Sleep(125);
            SpellManager.Cast("Bone Shield"); //Do It!                        


            Slog("**Bone Shield**");

            Thread.Sleep(2500); //Try to save refresh time
        }

        /////////////////////////////////////
        //Anti-Magic Shell (Req Level 68) Unholy
        /////////////////////////////////////

        private void AntiMagicShell()
        {
            SpellManager.Cast("Anti-Magic Shell"); //Do It!                        


            Slog("**Anti-Magic Shell**");
        }

        /////////////////////////////////////
        //Outbreak (Req Level 81) Unholy
        /////////////////////////////////////

        private void Outbreak()
        {
            SpellManager.Cast("Outbreak"); //Do It!                        


            Slog("**Outbreak**");
        }

        /////////////////////////////////////
        //Dark Transformation Unholy
        /////////////////////////////////////

        private void DarkTransformation()
        {
            SpellManager.Cast("Dark Transformation"); //Do It!                        


            Slog("**Dark Transformation**");
        }

        /////////////////////////////////////
        //Unholy Frenzy
        /////////////////////////////////////

        private void UnholyFrenzy()
        {
            SpellManager.Cast("Unholy Frenzy"); //Do It!                        


            Slog("**Unholy Frenzy**");
        }

        #endregion

        #region Runic Power Spells

        /////////////////////////////////////
        //Death Coil (Req Level 55) Runic Power
        /////////////////////////////////////

        private void DeathCoil()
        {
            SpellManager.Cast("Death Coil"); //Do It!                       


            Slog("**Death Coil**");
        }

        /////////////////////////////////////
        //Mind Freeze (Req Level 57) Runic Power
        /////////////////////////////////////

        private void MindFreeze()
        {
            SpellManager.Cast("Mind Freeze"); //Do It!


            Slog("**Mind Freeze**");
        }

        /////////////////////////////////////
        //Dancing Rune Weapon (Req 51 Blood Talent)
        /////////////////////////////////////

        private void Drw()
        {
            SpellManager.Cast("Dancing Rune Weapon"); //Do It!


            Slog("**Dancing Rune Weapon**");


            _drwToggle = false; //Go back to RP Dump

            _drwSpam = false; //Reset Spam
        }

        /////////////////////////////////////
        //Summon Gargoyle (Req 51 Unholy Talent) Unholy
        /////////////////////////////////////

        private void SummonGargoyle()
        {
            SpellManager.Cast("Summon Gargoyle"); //Do It!


            Slog("**Summon Gargoyle**");

            _sgToggle = false; //Go back to RP Dump

            _sgSpam = false; //Reset Spam
        }

        #endregion

        #region Hybrid Spells

        /////////////////////////////////////
        //Obliterate (Req Level 61) Frost/Unholy
        /////////////////////////////////////

        private void Obliterate()
        {
            SpellManager.Cast("Obliterate"); //Do It!                       


            Slog("**Obliterate**");
        }

        /////////////////////////////////////
        //Scourge Strike (Req 40 Unholy Talents) Unholy/Frost
        /////////////////////////////////////

        private void ScourgeStrike()
        {
            SpellManager.Cast("Scourge Strike"); //Do It!                       


            Slog("**Scourge Strike**");
        }

        /////////////////////////////////////
        //Death Strike Level 56 Unholy/Frost
        /////////////////////////////////////

        private void DeathStrike()
        {
            SpellManager.Cast("Death Strike"); //Do It!                       


            Slog("**Death Strike**");
        }

        /////////////////////////////////////
        //Rune Strike Level 56 Frost
        /////////////////////////////////////

        private void RuneStrike()
        {
            SpellManager.Cast("Rune Strike"); //Do It!                       


            Slog("**Rune Strike**");
        }
        #endregion

        #region Other Spells

        /////////////////////////////////////
        //Death Grip (Req Level 55)
        /////////////////////////////////////

        private void DeathGrip()
        {
            SpellManager.Cast("Death Grip"); //Do It!                


            Slog("**Death Grip**");

            Thread.Sleep(500);
        }

        /////////////////////////////////////
        //Horn of Winter (Level 65)
        /////////////////////////////////////

        public void HornOfWinter()
        {
            WoWMovement.MoveStop();
            Thread.Sleep(125);
            SpellManager.Cast("Horn of Winter"); //Do It!        


            Slog("**Horn of Winter**");
        }

        /////////////////////////////////////
        //Dark Command (Req Level 65)
        /////////////////////////////////////

        private void DarkCommand()
        {
            SpellManager.Cast("Dark Command"); //Do It!                   


            Slog("**Dark Command**");
        }

        /////////////////////////////////////
        //Raise Dead (Req Level 56)
        /////////////////////////////////////

        private void RaiseDead()
        {
            SpellManager.Cast("Raise Dead"); //Do It!

            Slog("**Raise Dead**");
        }

        /////////////////////////////////////
        //Death Pact (Req Level ??)
        /////////////////////////////////////

        private void DeathPact()
        {
            SpellManager.Cast("Death Pact"); //Do It!

            Slog("**Death Pact**");

        }

        /////////////////////////////////////
        //Blood Presence (Req Level ??)
        /////////////////////////////////////

        private void BloodPresence()
        {
            SpellManager.Cast("Blood Presence"); //Do It!

            Slog("**Blood Presence**");

        }

        /////////////////////////////////////
        //Frost Presence (Req Level ??)
        /////////////////////////////////////

        private void FrostPresence()
        {
            SpellManager.Cast("Frost Presence"); //Do It!

            Slog("**Frost Presence**");

        }

        /////////////////////////////////////
        //Unholy Presence (Req Level ??)
        /////////////////////////////////////

        private void UnholyPresence()
        {
            SpellManager.Cast("Unholy Presence"); //Do It!

            Slog("**Unholy Presence**");

        }

        #endregion

        #region Misc Spells

        /////////////////////////////////
        //Every Man for Himself (Req Human)
        /////////////////////////////////

        private void Em()
        {
            SpellManager.Cast("Every Man for Himself"); //Do It!                


            Slog("**Every Man for Himself**");
        }

        /////////////////////////////////
        //Stoneform (Req Dwarf)
        /////////////////////////////////

        private void Sf()
        {
            SpellManager.Cast("Stoneform"); //Do It!                


            Slog("**Stoneform**");
        }

        /////////////////////////////////
        //Gift of the Naaru (Req Draenie)
        /////////////////////////////////

        private void Naaru()
        {
            SpellManager.Cast("Gift of the Naaru"); //Do It!                


            Slog("**Gift of the Naaru**");
        }

        /////////////////////////////////
        //Arcane Torrent (Req Blood Elf)
        /////////////////////////////////

        private void At()
        {
            SpellManager.Cast("Arcane Torrent"); //Do It!                


            Slog("**Arcane Torrent**");
        }

        /////////////////////////////////
        //WarStomp (Req Tauren)
        /////////////////////////////////

        private void WarStomp()
        {
            SpellManager.Cast("War Stomp"); //Do It!                


            Slog("**War Stomp**");
        }

        /////////////////////////////////
        //Blood Fury (Req Orc)
        /////////////////////////////////

        private void BloodFury()
        {
            SpellManager.Cast("Blood Fury"); //Do It!                


            Slog("**Blood Fury**");
        }

        /////////////////////////////////
        //LifeBlood (Req Hearbalist)
        /////////////////////////////////

        private void LifeBlood()
        {
            SpellManager.Cast("Lifeblood"); //Do It!                


            Slog("**Lifeblood**");
        }

        #endregion

        #endregion

        #endregion


    }

}   
    

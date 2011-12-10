using System;
using System.Threading;

using Styx;
using Styx.Logic;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.Logic.Pathing;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        private WoWUnit FindPlayers()
        {
            var playerList = ObjectManager.GetObjectsOfType<WoWUnit>();
            foreach (WoWUnit players in playerList)
            {
                if (players.IsPlayer)
                {
                    slog("Got Players! Run for the hills!");
                    return players;
                }
            }
            return null;
        }
        bool IsFacingAway
        {
            get
            {
                bool face = false;
                double bearing = Me.CurrentTarget.Rotation;
                //double Angle = bearing * 180 / Math.PI; 
                if (bearing > Math.PI / 2 || bearing < -Math.PI / 2)
                    face = true;
                return face;
            }
        }
        //degrees = radians * 180 / Math.PI
        public override void Pull()
        {
            try
            {
                //if (initialized == false)
                //{
                //    Initialize();
                //}
                bgTargetCheck();
                getAdds();
                slog("Starting Pull");
                if (!Me.Combat && Me.GotTarget)
                {
                    slog("Safe location is saved");
                    prevPrevSafePoint = prevSafePoint;
                    prevSafePoint = safePoint;
                    safePoint = Me.Location;
                }
                slog("PVP Checked");
                if (Blacklist.Contains(Me.CurrentTarget.Guid))
                {
                    slog("Target is blacklisted");
                    Styx.Logic.Blacklist.Add(Me.CurrentTarget.Guid, System.TimeSpan.FromSeconds(30));
                    Me.ClearTarget();
                    //pullGuid = 0;
                }
                if (Me.CurrentTarget.Guid != lastGuid)
                {
                    slog("Pull starting. Target is new");
                    pullTimer.Reset();
                    pullTimer.Start();
                    lastGuid = Me.CurrentTarget.Guid;
                    if (Me.CurrentTarget.IsPlayer)
                    {
                        slog("Pull: Killing Player at distance " + Math.Round(Me.CurrentTarget.Distance).ToString() + "");
                    }
                    slog("Pull: Killing " + Me.CurrentTarget.Name + " at distance " + Math.Round(Me.CurrentTarget.Distance).ToString() + "");
                    pullTimer.Reset();
                    pullTimer.Start();
                }
                if (!Me.CurrentTarget.IsPlayer && Me.CurrentTarget.CurrentHealth > 95 && 30 * 1000 < pullTimer.ElapsedMilliseconds)
                {
                    slog(" This " + Me.CurrentTarget.Name + " is a bugged mob.  Blacklisting for 1 hour.");
                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromHours(1.00));
                    Me.ClearTarget();
                    //pullGuid = 0;
                    if (Me.Location.Distance(safePoint) >= 30)
                    {
                        slog("Try to move to safePoint");
                        SafeMoveToPoint(safePoint, 10000);
                    }
                    else if (Me.Location.Distance(prevSafePoint) >= 30)
                    {
                        slog("Try to move to prevSafePoint");
                        SafeMoveToPoint(prevSafePoint, 10000);
                    }
                    else if (Me.Location.Distance(prevPrevSafePoint) >= 30)
                    {
                        slog("Try to move to prevPrevSafePoint");
                        SafeMoveToPoint(prevPrevSafePoint, 10000);
                    }
                    else
                    {
                        slog("Can't move to locations");
                    }
                }
                if (SSSettings.Instance.UseDistract && SpellManager.CanCast("Distract"))
                {
                    if (Me.IsAlive && Me.GotTarget && !Me.Combat)
                    {
                        if (Me.CurrentTarget.Distance > 4 && Me.CurrentTarget.Distance < 30)
                        {
                            Distract();
                        }
                    }
                }
                if (!Me.Combat && targetDistance > 4 && targetDistance < Styx.Logic.Targeting.PullDistance + 10)
                {
                    slog("Move to target");
                    int a = 0;
                    float pullRange = 3.5f + StyxWoW.Me.CurrentTarget.BoundingRadius;
                    while (a < 50 && ObjectManager.Me.IsAlive && ObjectManager.Me.GotTarget && ObjectManager.Me.CurrentTarget.Distance > pullRange)
                    {
                        if (ObjectManager.Me.Combat)
                        {
                            slog("Combat has started.  Abandon pull.");
                            break;
                        }
                        WoWMovement.Face();
                        Navigator.MoveTo(WoWMovement.CalculatePointFrom(ObjectManager.Me.CurrentTarget.Location, 3f /* + Me.CurrentTarget.BoundingRadius */));
                        StyxWoW.SleepForLagDuration();
                        ++a;
                    }
                }
                else
                {
                    WoWMovement.MoveStop();
                    WoWMovement.Face();
                }
                if (Me.GotTarget &&
                    targetDistance <= 5 &&
                    !Me.IsAutoAttacking)
                {
                    slog("Final state of pulling");
                    if (attackPoint != WoWPoint.Empty)
                    {
                        Navigator.MoveTo(attackPoint);
                    }
                    if (!PocketPicked)
                    {
                        if (SSSettings.Instance.UsePickPocket && SpellManager.CanCast("Pick Pocket") && Me.GotTarget && targetDistance <= 5 && !Me.CurrentTarget.IsPlayer)
                        {
                            if (Me.CurrentTarget.CreatureType == WoWCreatureType.Humanoid || Me.CurrentTarget.CreatureType == WoWCreatureType.Undead)
                            {
                                slog("Try to pickpocket");
                                PickPocket();
                                Thread.Sleep(1000);
                                PocketPicked = true;
                            }
                        }
                    }
                    if (SSSettings.Instance.PullType.Equals(1))
                    {
                        if (Me.GotTarget && targetDistance <= 5)
                        {
                            if (SpellManager.CanCast("Cheap Shot"))
                            {
                                CheapShot();
                            }
                            else
                            {
                                Lua.DoString("StartAttack()");
                            }
                        }
                    }
                    if (SSSettings.Instance.PullType.Equals(2))
                    {
                        if (SpellManager.CanCast("Ambush") && Me.GotTarget && targetDistance <= 5)
                        {
                            Ambush();
                        }
                        else if (SpellManager.CanCast("Backstab") && Me.GotTarget && targetDistance <= 5)
                        {
                            Backstab();
                        }
                        else
                        {
                            Lua.DoString("StartAttack()");
                        }
                    }
                    if (SSSettings.Instance.PullType.Equals(3))
                    {
                        if (Me.GotTarget && targetDistance <= 5)
                        {
                            if (SpellManager.CanCast("Garrote"))
                            {
                                Garrote();
                            }
                            else
                            {
                                Lua.DoString("StartAttack()");
                            }
                        }
                    }
                    if (SSSettings.Instance.PullType.Equals(4))
                    {
                        Lua.DoString("StartAttack()");
                    }
                    if (SSSettings.Instance.PullType.Equals(5))
                    {
                        Throw();
                        Lua.DoString("StartAttack()");
                    }
                }
            }
            finally
            {
                slog("Pull done.");
                PocketPicked = false;
                fightTimer.Reset();
                fightTimer.Start();
            }
        }
    }
}

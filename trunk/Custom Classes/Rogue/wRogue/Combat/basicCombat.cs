using System;
using System.Threading;
using System.Collections.Generic;

using Styx;
using Styx.Logic;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

using wRogue.Talents;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public override void Combat()
        {
            // Fix for POI System Loop while not having target and still in combat
            if (Me.CurrentTargetGuid == 0 || Me.CurrentTarget == null) return;
            // Clear target if CurrentTarget is dead
            if (Me.GotTarget && !Me.CurrentTarget.IsAlive)
            {
                Me.ClearTarget();
                StyxWoW.SleepForLagDuration();
            }
            if (Me.GotTarget && (Me.CurrentTarget.Guid != lastGuid || InfoPanel.Deaths > deathCount))
            {
                if (InfoPanel.Deaths > deathCount)
                {
                    deathCount = InfoPanel.Deaths;
                }
                if (Me.CurrentTarget.IsPlayer)
                {
                    slog("Killing level " + Me.CurrentTarget.Level.ToString() + " " + Me.CurrentTarget.Race.ToString() + " " + Me.CurrentTarget.Class.ToString() + " at range of " + System.Math.Round(Me.CurrentTarget.Distance).ToString() + " yards.");
                }
                else
                {
                    slog("Killing " + Me.CurrentTarget.Name + " at range of " + System.Math.Round(Me.CurrentTarget.Distance).ToString() + " yards.");
                }
                fightTimer.Reset();
                fightTimer.Start();
                lastGuid = Me.CurrentTarget.Guid;
            }
            // If in combat and have no target, find one!
            if (!Me.GotTarget)
            {
                // Find mobs attacking us
                addsList = getAdds();
                // If we find an attacking mob
                if (addsList.Count > 0)
                {
                    // Log
                    slog("Combat: Finding target.");
                    // Find valid mob
                    int mobIndex = 0;
                    do
                    {
                        addsList[mobIndex++].Target();
                    } while (!Me.GotTarget && (Me.GotTarget && !Me.CurrentTarget.IsAlive) && addsList.Count < mobIndex);
                    // Move to attack range
                    if (attackPoint != WoWPoint.Empty)
                    {
                        Navigator.MoveTo(attackPoint);
                    }
                }
                else
                // No attacking mobs
                {
                    // Log
                    slog("Combat: No target found, ending combat.");
                    // Return from Combat()
                    return;
                }
            }
            if (combatChecks)
            {
                addsList = getAdds();
                if (addsList.Count > 1)
                {
                    if (addsList[1].Distance < 7)
                    {
                        if (SSSettings.Instance.UseBladeFlurry)
                        {
                            if (SpellManager.CanCast("Blade Flurry") && Me.GotTarget && targetDistance <= 5)
                            {
                                BladeFlurry();
                            }
                        }
                        if (SSSettings.Instance.UseKillingSpree)
                        {
                            if (SpellManager.CanCast("Killing Spree") && Me.GotTarget && targetDistance <= 5)
                            {
                                KillingSpree();
                            }
                        }
                        if (SSSettings.Instance.UseAdrenalineRush)
                        {
                            if (SpellManager.CanCast("Adrenaline Rush") && Me.GotTarget && targetDistance <= 5)
                            {
                                AdrenalineRush();
                            }
                        }
                        if (SSSettings.Instance.UseAddManagement)
                        {
                            if (!Me.HasAura("Blade Flurry") && !SpellManager.CanCast("Fan of Knives") && addsList.Count <= 2)
                            {
                                BlindAndGouge();
                            }
                        }
                        if (SSSettings.Instance.UseFanOfKnives)
                        {
                            if (!Me.HasAura("Blade Flurry") && SpellManager.CanCast("Fan of Knives") && Me.GotTarget && targetDistance <= 5)
                            {
                                FanofKnives();
                            }
                        }
                    }
                }
                if (Me.Stunned || Me.Dazed && Me.Combat)
                {
                    WillOfTheForsaken();
                    EveryManForHimself();
                }
                if (StyxWoW.Me.ComboPoints > 5)
                {
                    slog(StyxWoW.Me.ComboPoints.ToString() + " combo points.  Need to restart WoW.");
                }
            }
            else
            {
                return;
            }
            if (combatChecks)
            {
                if (SpellManager.CanCast("Berserking"))
                {
                    Berserking();
                }
                if (SpellManager.CanCast("Bloodrage"))
                {
                    Bloodrage();
                }
                if (Me.GotTarget && Me.CurrentTarget.IsCasting)
                {
                    if (SpellManager.CanCast("Kick") && Me.GotTarget && targetDistance <= 5)
                    {
                        Kick();
                    }
                    if (SpellManager.CanCast("Gouge") && !SpellManager.CanCast("Kick"))
                    {
                        Gouge();
                    }
                    if (!SpellManager.CanCast("Kick") && !SpellManager.CanCast("Gouge") && SpellManager.CanCast("Arcane Torrent"))
                    {
                        ArcaneTorrent();
                    }
                }
            }
            if (Me.GotTarget && Me.HealthPercent < 50 && Me.CurrentTarget.HealthPercent > 20)
            {
                if (!combatChecks)
                {
                    return;
                }
                if (SpellManager.CanCast("Evasion") && Me.GotTarget && targetDistance <= 5)
                {
                    Evasion();
                }
                if (SpellManager.CanCast("Stoneform"))
                {
                    Stoneform();
                }
            }
            if (!SpellManager.CanCast("Vanish") && SpellManager.CanCast("Cloak of Shadows") && 25 > Me.HealthPercent && 10 < Me.CurrentTarget.HealthPercent && Me.GotTarget)
            {
                CloakofShadows();
                Me.ClearTarget();
                if (Me.Location.Distance(safePoint) >= 30)
                {
                    slog("Try to move to safePoint");
                    SafeMoveToPoint(safePoint, 7000);
                }
                else if (Me.Location.Distance(prevSafePoint) >= 30)
                {
                    slog("Try to move to prevSafePoint");
                    SafeMoveToPoint(prevSafePoint, 7000);
                }
                else if (Me.Location.Distance(prevPrevSafePoint) >= 30)
                {
                    slog("Try to move to prevPrevSafePoint");
                    SafeMoveToPoint(prevPrevSafePoint, 7000);
                }
                else
                {
                    slog("Can't move to locations");
                }
            }
            if (Me.GotTarget && Me.HealthPercent < 25 && Me.CurrentTarget.HealthPercent > 10 && SpellManager.CanCast("Vanish"))
            {
                Vanish();
                //pullGuid = 0;
                if (Me.Location.Distance(safePoint) >= 30)
                {
                    slog("Try to move to safePoint");
                    SafeMoveToPoint(safePoint, 7000);
                }
                else if (Me.Location.Distance(prevSafePoint) >= 30)
                {
                    slog("Try to move to prevSafePoint");
                    SafeMoveToPoint(prevSafePoint, 7000);
                }
                else if (Me.Location.Distance(prevPrevSafePoint) >= 30)
                {
                    slog("Try to move to prevPrevSafePoint");
                    SafeMoveToPoint(prevPrevSafePoint, 7000);
                }
                else
                {
                    slog("Can't move to locations");
                }
            }
            if (SpellManager.CanCast("Deadly Throw") && Me.GotTarget && 10 < Me.CurrentTarget.Distance && 30 >= targetDistance && StyxWoW.Me.ComboPoints > 0)
            {
                DeadlyThrow();
            }
            if (!SpellManager.CanCast("Deadly Throw") && SpellManager.CanCast("Sprint") && Me.GotTarget && 10 < Me.CurrentTarget.Distance && 30 >= targetDistance)
            {
                Sprint();
            }
            if (30 * 1000 < fightTimer.ElapsedMilliseconds && Me.CurrentTarget.HealthPercent > 95)
            {
                slog(" This " + Me.CurrentTarget.Name + " is a bugged mob.  Combat blacklisting for 1 hour.");
                Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromHours(1.00));
                Me.ClearTarget();
                lastGuid = 0;
                if (Me.Location.Distance(safePoint) >= 30)
                {
                    slog("Try to move to safePoint");
                    SafeMoveToPoint(safePoint, 7000);
                }
                else if (Me.Location.Distance(prevSafePoint) >= 30)
                {
                    slog("Try to move to prevSafePoint");
                    SafeMoveToPoint(prevSafePoint, 7000);
                }
                else if (Me.Location.Distance(prevPrevSafePoint) >= 30)
                {
                    slog("Try to move to prevPrevSafePoint");
                    SafeMoveToPoint(prevPrevSafePoint, 7000);
                }
                else
                {
                    slog("Can't move to locations");
                }
            }

            //if (Me.IsInInstance)
            //{
            //    if (!Me.BehindTarget)
            //    {
            //        Navigator.MoveTo(InstanceBehindTarget);
            //    }
            // }

            if (Me.Combat && Me.GotTarget)
            {
                AdvancedCombat();
            }
        }

        private bool combatChecks
        {
            get
            {
                if (!ObjectManager.Me.GotTarget)
                {
                    slog("No target.");
                    return false;
                }
                if (ObjectManager.Me.Dead)
                {
                    slog("I died.");
                    return false;
                }
                if (ObjectManager.Me.GotTarget && !ObjectManager.Me.IsAutoAttacking)
                {
                    WoWMovement.Face();
                    Lua.DoString("StartAttack()");
                }
                if (Me.GotTarget && ObjectManager.Me.CurrentTarget.Distance > 50)
                {
                    if (ObjectManager.Me.CurrentTarget.IsPlayer)
                    {
                        slog("Out of range: Level " + ObjectManager.Me.CurrentTarget.Level.ToString() + " " + ObjectManager.Me.CurrentTarget.Race.ToString() + " " + System.Math.Round(ObjectManager.Me.CurrentTarget.Distance).ToString() + " yards away.");
                    }
                    else
                    {
                        slog("Out of range: Level " + ObjectManager.Me.CurrentTarget.Name + " is " + System.Math.Round(ObjectManager.Me.CurrentTarget.Distance).ToString() + " yards away.");
                    }
                    Me.ClearTarget();
                    return false;
                }
                float meleeRange = 3.5f + Me.CurrentTarget.BoundingRadius;
                if (Me.GotTarget && Me.CurrentTarget.Distance > meleeRange)
                {
                    int a = 0;
                    while (a < 50 && ObjectManager.Me.IsAlive && ObjectManager.Me.GotTarget && ObjectManager.Me.CurrentTarget.Distance > meleeRange)
                    {
                        WoWMovement.Face();
                        Navigator.MoveTo(WoWMovement.CalculatePointFrom(ObjectManager.Me.CurrentTarget.Location, 3.5f + Me.CurrentTarget.BoundingRadius));
                        StyxWoW.SleepForLagDuration();
                        ++a;
                    }
                }

                //if (Me.IsInInstance)
                //{
                //   double meleeRange = Me.CurrentTarget.BoundingRadius + 4;
                //  if (ObjectManager.Me.CurrentTarget.Distance > meleeRange)
                // {
                //   int a = 0;
                // while (a < 50 && ObjectManager.Me.IsAlive && ObjectManager.Me.GotTarget && ObjectManager.Me.CurrentTarget.Distance > meleeRange)
                // {
                //   WoWMovement.Face();
                //  Navigator.MoveTo(InstanceBehindTarget);
                // StyxWoW.SleepForLagDuration();
                //WoWMovement.Face();
                // ++a;
                //}
                //}
                //}

                if (Me.GotTarget)
                {
                    WoWMovement.Face();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private List<WoWUnit> getAdds()
        {
            // Variables
            List<WoWUnit> enemyMobList = new List<WoWUnit>();
            // Get Objects of WoWUnit
            List<WoWUnit> mobList = ObjectManager.GetObjectsOfType<WoWUnit>(false);
            // Iterate WoWUnits found
            foreach (WoWUnit thing in mobList)
            {
                // Mob validation
                if (
                    thing.Distance > 40 ||
                    !thing.IsAlive
                    )
                {
                    continue;
                }
                // Mob must be attacking me
                if (thing.IsTargetingMeOrPet)
                {
                    // Push into List
                    enemyMobList.Add(thing);
                }
            }
            // Warning about amount of Mobs - if debugging, then display as well
            if ((enemyMobList.Count > 1 && _logspamMobCount != enemyMobList.Count))
            {
                slog("Warning - there are " + enemyMobList.Count.ToString() + " attackers");
            }
            // Spam check variable update
            _logspamMobCount = enemyMobList.Count;
            enemyMobList.Sort(SortByDistance);
            // Return list
            return enemyMobList;
        }
        private void BlindAndGouge()
        {
            WoWUnit myAdd = addsList[1];
            if (myAdd != oldAdd)
            {
                countBlind = 0;
                countGouge = 0;
            }
            if (countBlind < 3 && SpellManager.CanCast("Blind"))
            {
                addsList[1].Target();
                WoWMovement.Face();
                Thread.Sleep(750);
                SpellManager.Cast("Blind");
                addsList[0].Target();
                WoWMovement.Face();
                Thread.Sleep(750);
                ++countBlind;
                Lua.DoString("StartAttack()");
                oldAdd = myAdd;
            }
            else if (SpellManager.CanCast("Gouge") && 3 > countGouge)
            {
                addsList[1].Target();
                WoWMovement.Face();
                Thread.Sleep(750);
                SpellManager.Cast("Gouge");
                addsList[0].Target();
                Thread.Sleep(750);
                WoWMovement.Face();
                Lua.DoString("StartAttack()");
                ++countGouge;
                oldAdd = myAdd;
            }
            else
                return;
        }
    }
}

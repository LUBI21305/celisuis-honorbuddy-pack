// Custom Class for Paladins Healing in a PVP environment, like BG or Arena
// Inspired by the wonderfull Sm0k3d's EndGame Pally HealBot CC
// Many thanks to him and to all the other developers!
// Created by Gilderoy

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace UltiumatePalaHealer
{
    partial class UltimatePalaHealer : CombatRoutine
    {
        #region Combat
        private void ArenaCombat()
        {
            tank = PVPGetTank();
            //checkthetank();
            if (unitcheck(tank) < 0)
            {
                slog(Color.DarkRed, "Ok, I'm lost, I thought we found a tank, instead I did not, CC is in Pause, please report this Error with full LOG!");
                return;
            }
            //if (tank == null) { tank = Me; }
            //if (!tank.IsValid) { return; }
            //mtank = PVPgetmtank();
            Enemy = PVPGiveEnemy(29);
            Epet = GiveEnemyPet(38);
            if (PVP_wanna_target && unitcheck(Enemy)>0 && (Me.CurrentTarget == null || !Me.CurrentTarget.IsValid || Me.CurrentTarget.Dead))
            {
                if (unitcheck(Enemy)>0)
                {
                    Enemy.Target();
                }
                else if (unitcheck(tank.CurrentTarget)>0)
                {
                    tank.CurrentTarget.Target();
                }
            }
            /*
            if (Enemy != null)
            {
                Enemy.Target();
            }
            else if (tank!=null && tank.CurrentTarget != null)
            {
                tank.CurrentTarget.Target();
                Enemy = tank.CurrentTarget;
            }*/
            /*if (PVPInterrupt() || StyxWoW.GlobalCooldown) { return; }
            else */
            if (NeedtoRest()) { slog("resting"); return; }
            else if (Self(PVP_wanna_DP, PVP_wanna_DS, PVP_DP_min_hp, PVP_DS_min_hp)) { return; }
            else if (Racials(PVP_use_stoneform, PVP_wanna_everymanforhimself, PVP_use_escapeartist, PVP_min_run_to_HoF, PVP_use_gift, PVP_min_gift_hp, PVP_use_bloodfury, PVP_bloodfury_min_hp, PVP_use_warstomp, PVP_min_warstomp_hp, PVP_use_bersekering, PVP_min_bersekering_hp, PVP_use_will_forsaken, PVP_use_torrent, PVP_min_torrent_hp)) { return; }
            else if (ARENA_wanna_taunt && Taunt()) { return; }
            else if (Enemy != null && Me.Combat && PVP_wanna_Judge && Judge(PVP_mana_judge)) { return; }
            else if (PVP_want_urgent_cleanse && PVPCleanseNow()) { return; }
            else if (Enemy != null && Me.Combat && EnemyInterrupt(PVP_wanna_rebuke, PVP_wanna_HoJ, ARENA_wanna_move_to_HoJ)) { return; }
            else if (PVPHealing(ARENA_wanna_move_to_heal)) { return; }
            else if (ManaRec(PVP_min_Divine_Plea_mana)) { return; }
            else if (PVP_want_cleanse && (Me.Combat || !Me.Mounted) && Cleansing()) { return; }
            else if (Enemy != null && Me.Combat && Dps(PVP_wanna_HoW, PVP_wanna_Denunce)) { return; }
            else if (TopOff(ARENA_wanna_move_to_heal, ARENA_min_FoL_hp, ARENA_min_DL_hp, ARENA_min_HL_hp)) { return; }
            else if (Enemy != null && Me.Combat && ConsumeTime(PVP_wanna_CS)) { return; }
            else if (PVP_wanna_buff && Buff(ARENA_wanna_move_to_heal)) { return; }
            else if (PVP_wanna_mount && MountUp()) { return; }
            else
            {

            }
            return;
        }
        private void WorldPVPCombat()
        {
            BattlegroundCombat();
            return;
        }
        private void BattlegroundCombat()
        {
            tank = GetTank();
            //checkthetank();
            if (unitcheck(tank) < 0)
            {
                slog(Color.DarkRed, "Ok, I'm lost, I thought we found a tank, instead I did not, CC is in Pause, please report this Error with full LOG!");
                return;
            }
            //if (tank == null) { tank = Me; }
            //if (!tank.IsValid) { return; }
            Enemy = PVPGiveEnemy(29);
            if (PVP_wanna_target && unitcheck(Enemy) > 0 && (unitcheck(Me.CurrentTarget)<0))
            {
                if (unitcheck(Enemy) > 0)
                {
                    Enemy.Target();
                }
                else if (unitcheck(tank.CurrentTarget) > 0)
                {
                    tank.CurrentTarget.Target();
                }
            }
            /*
            if (Enemy != null)
            {
                Enemy.Target();
            }
            else if (tank.CurrentTarget != null)
            {
                tank.CurrentTarget.Target();
                Enemy = tank.CurrentTarget;
            }*/
           /* if (PVPInterrupt() || StyxWoW.GlobalCooldown) { return; }
            else */
            if (NeedtoRest()) { slog("resting"); return; }
            else if (Self(PVP_wanna_DP, PVP_wanna_DS && !Me.HasAura("Alliance Flag") && !Me.HasAura("Horde Flag"), PVP_DP_min_hp, PVP_DS_min_hp)) { return; }
            else if (Racials(PVP_use_stoneform, PVP_wanna_everymanforhimself, PVP_use_escapeartist, PVP_min_run_to_HoF, PVP_use_gift, PVP_min_gift_hp, PVP_use_bloodfury, PVP_bloodfury_min_hp, PVP_use_warstomp, PVP_min_warstomp_hp, PVP_use_bersekering, PVP_min_bersekering_hp, PVP_use_will_forsaken, PVP_use_torrent, PVP_min_torrent_hp)) { return; }
            else if (Enemy != null && Me.Combat && PVP_wanna_Judge && Judge(PVP_mana_judge)) { return; }
            else if (PVP_want_urgent_cleanse && PVPCleanseNow()) { return; }
            else if (Enemy != null && Me.Combat && EnemyInterrupt(PVP_wanna_rebuke, PVP_wanna_HoJ, PVP_wanna_move_to_HoJ)) { return; }
            else if (PVPHealing(PVP_wanna_move_to_heal)) { return; }
            else if (ManaRec(PVP_min_Divine_Plea_mana)) { return; }
            else if (PVP_want_cleanse && (Me.Combat || !Me.Mounted) && Cleansing()) { return; }
            else if (Enemy != null && Me.Combat && Dps(PVP_wanna_HoW, PVP_wanna_Denunce)) { return; }
            else if (TopOff(PVP_wanna_move_to_heal, PVP_min_FoL_hp, PVP_min_DL_hp, PVP_min_HL_hp)) { return; }
            else if (Enemy != null && Me.Combat && ConsumeTime(PVP_wanna_CS)) { return; }
            else if (PVP_wanna_buff && Buff(PVP_wanna_move_to_heal)) { return; }
            else if (PVP_wanna_mount && MountUp()) { return; }
            else
            {

            }
            return;
        }

        #endregion

        #region Action

        private bool PVPCleanseNow()
        {
            WoWPlayer NeedFreedom;
            NeedFreedom = NeedHoFNoW();
            /*
            if ((NeedFreedom == null || !NeedFreedom.IsValid) && Me.MovementInfo.RunSpeed < PVP_min_run_to_HoF)
            {
                NeedFreedom = Me;
            }
             */
            if (PVP_wanna_HoF && NeedFreedom != null && NeedFreedom.IsValid && !NeedFreedom.Dead)
            {
                if (NeedFreedom.Combat && CanCast("Hand of Freedom", NeedFreedom))//&& NeedFreedom.MovementInfo.RunSpeed < PVE_min_run_to_HoF)
                {
                    Cast("Hand of Freedom", NeedFreedom, 30, "OhShit", "Target is slowed or snared");
                    slog(Color.Violet, "Hand of Freedom on {0} is movement speed is {1}", NeedFreedom.Name, NeedFreedom.MovementInfo.RunSpeed);
                }
                else if (NeedFreedom.Combat && !CanCast("Hand of Freedom", NeedFreedom))
                {
                    Cast("Cleanse", NeedFreedom, 30, "OhShit", "Target is slowed or snared");
                    slog(Color.Violet, "Cleanse on {0} couse HoF is on cooldown", NeedFreedom.Name);
                }
            }
            /*
            if (PVP_wanna_HoF)
            {
                foreach (WoWPlayer d in Me.PartyMembers)
                {
                    if (d.MovementInfo.RunSpeed < PVP_min_run_to_HoF && CanCast("Hand of Freedom", d) && d.Combat && d.Distance<30)
                    {
                        Cast("Hand of Freedom", d, 30, "OhShit", "Target is slowed or snared, speed " + d.MovementInfo.RunSpeed.ToString());
                        //slog(Color.Violet, "Hand of Freedom on {0}", d.Name);
                    }
                }
            }
             */
            WoWPlayer p = PVPGetUrgentCleanseTarget();
            //slog(Color.Violet, "Urgent Cleansing {0}", p.Name);
            return Cast("Cleanse", p, 40, "Cleanse", "Need Urgent Dispell");
        }



        private WoWPlayer PVPGetTank()
        {
            WoWPlayer localtank = GetTank();
            if (unitcheck(localtank) < 0)
            {
                slog(Color.DarkRed, "We are in arena, but no tank can be found, CC is in PAUSE");
                return null;
            }
            else if (usedBehaviour == "Arena")
            {
                if (combatfrom.Elapsed.TotalSeconds > 2)
                {
                    //if (tank != null && Me != null && tank.IsValid && Me.IsValid && !tank.Dead && !Me.Dead)
                    if (unitcheck(Me) == 1 && unitcheck(localtank) == 1)
                    {
                        if (Me.HealthPercent <
                            (localtank.HealthPercent - 20) && CanCast("Hand of Salvation"))
                        {
                            slog(Color.Black, "Changing Tank to Me couse they are beating on me!");
                            return Me;
                        }
                        else
                        {
                            return localtank;
                        }
                    }
                    else if (unitcheck(Me) == 1)
                    {
                        return Me;
                    }
                    combatfrom.Reset();
                    combatfrom.Start();
                    slog(Color.DarkRed, "We are in arena, but i'm not valid, CC is in PAUSE");
                    return null;
                }
                else
                {
                    return localtank;
                }
            }
            else
            {
                return localtank;
            }
        }
            
            
            /*
            if (tank == null)
            {
                if (RaFHelper.Leader != null)
                {
                    slog("rapid tank=leader");
                    tank = RaFHelper.Leader;
                }else if (Me != null)
                {
                    slog("rapid tank=Me");
                    tank = Me;
                }
            }*/
            /*
            if (combatfrom.Elapsed.TotalSeconds > 2)
            {
                if (tank != null && Me != null && tank.IsValid && Me.IsValid && !tank.Dead && !Me.Dead)
                {
                    if (Me.HealthPercent <
                        (tank.HealthPercent - 20) && CanCast("Hand of Salvation"))
                    {
                        slog(Color.Black, "Changing Tank to Me couse they are beating on me!");
                        return Me;
                    }
                    else if (RaFHelper.Leader != null)
                    {
                        if (tank != null && tank == Me && RaFHelper.Leader != null && RaFHelper.Leader != Me) { slog(Color.Black, "Changing Tank back to real tank!"); } else if (RaFHelper.Leader != null && tank != null && RaFHelper.Leader != tank) { slog(Color.Black, "Import the tank from LazyRaider!"); }
                        return RaFHelper.Leader;
                    }
                }
                combatfrom.Reset();
                combatfrom.Start();
            }
            else if (RaFHelper.Leader != null)
            {
                if (tank != null && tank == Me && RaFHelper.Leader != null && RaFHelper.Leader != Me) { slog(Color.Black, "Changing Tank back to real tank!"); } else if (RaFHelper.Leader != null && tank != null && RaFHelper.Leader != tank) { slog(Color.Black, "Import the tank from LazyRaider!"); }
                return RaFHelper.Leader;
            }
            else if (InRaid())
            {
                foreach (WoWPlayer p in Me.RaidMembers)
                {
                    if (IsTank(p))
                    {
                        return p;
                    }
                }
            }
            else
            {
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    if (IsTank(p))
                    {
                        return p;
                    }
                }
            }
            return null;
        }
        */
        /*
        private bool HoFNow()
        {
            WoWPlayer p = HoFNowTarget();
            if (CanCast("Hand of Freedom", p))
            {
                //slog(Color.Violet, "Hand of Freedom {0}", p.Name);
                return Cast("Hand of Freedom", p, 30, "OhShit", "Target is slowed or snared");
            }
            else
            {
                return false;
            }
        }
        */
        private bool PVPHealing(bool should_move)
        {
            if (tar != null && tank != null && tar.IsValid && tank.IsValid && !tar.Dead)
            {
                if (should_move && (tar.Distance > 30 || !tar.InLineOfSight)) { slog("Healing target is too far away or not in LOS, moving to them!"); MoveTo(tar); return true; }
                if (Me.Combat && !combatfrom.IsRunning) { combatfrom.Start(); noncombatfrom.Reset(); } else if (!Me.Combat) { combatfrom.Reset(); noncombatfrom.Start(); }
                if (!Me.Combat) { noncombatfrom.Start(); }
                String s = null;
                double hp = tar.HealthPercent;
                if (Me.Mounted && !Me.Combat && !tank.Combat) { return false; }
                if (!Me.Combat && !tar.Combat && tar.Distance > 40) { return false; }

                if (Me.Combat && hp < PVP_ohshitbutton_activator)
                {
                    if (GotBuff("Divine Plea", Me)) { Lua.DoString("CancelUnitBuff(\"Player\",\"Divine Plea\")"); slog(Color.DarkOrange, "Cancelling Divine Plea due to a Oh Shit! moment"); }
                    PVPOhShitButton();
                }

                if (PVP_wanna_LoH && hp < PVP_min_LoH_hp && !tar.ActiveAuras.ContainsKey("Forbearance") && CanCast("Lay on Hands", tar) && (Me.Combat || tar.Combat)) { s = "Lay on Hands"; return Cast(s, tar, 40, "Heal", "Saving someone life"); }
                else if (PVP_wanna_HoP && hp < PVP_min_HoP_hp && tar.Guid != tank.Guid && !tar.ActiveAuras.ContainsKey("Forbearance") && CanCast("Hand of Protection", tar) && (Me.Combat || tar.Combat) && !tar.HasAura("Alliance Flag") && !tar.HasAura("Horde Flag")) { s = "Hand of Protection"; return Cast(s, tar, 30, "Heal", "Saving someone life"); }
                else if (PVP_wanna_HoS && hp < PVP_min_HoS_hp && tar.Guid == tank.Guid && CanCast("Hand of Sacrifice", tar) && Me.HealthPercent > 90 && (Me.Combat || tar.Combat)) { s = "Hand of Sacrifice"; return Cast(s, tar, 30, "Heal", "I'm fine, can Sacrifice for you"); }
                else if (tar.Guid == tank.Guid && BeaconNeedsRefresh(tank) && CanCast("Beacon of Light", tar) && tank.Distance < 30 && !Me.Mounted) { s = "Beacon of Light"; return Cast(s, tar, 40, "Heal", "Beacon of Light dropping off"); }
                /*else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar) && tar.Guid==tank.Guid && tar.HealthPercent<=65) { s = "Word of Glory"; }
                else if (Me.CurrentHolyPower == 3 && CanCast("Light of Dawn") && ShouldLightofDawn(3)) { s = "Light of Dawn"; }
                else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar)) { s = "Word of Glory"; }
              */
                //else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar) && tar.HealthPercent <= 35) { s = "Word of Glory"; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (Me.CurrentHolyPower == 3 && (HolyPowerDump())) { s = "Light of Dawn"; return Cast(s, "Heal", "Healing"); }
                else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar)) { s = "Word of Glory"; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (CanCast("Holy Shock", tar)) { s = "Holy Shock"; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (PVP_want_HR && ShouldHolyRadiance(PVP_min_player_inside_HR, PVP_HR_how_far, PVP_HR_how_much_health)) { s = "Holy Radiance"; return Cast(s, "Heal", "Healing"); }
                else if (PVP_Inf_of_light_wanna_DL && GotBuff("Infusion of Light") && !(Me.IsMoving)) { s = "Divine Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (!PVP_Inf_of_light_wanna_DL && GotBuff("Infusion of Light") && !(Me.IsMoving)) { s = "Holy Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (hp < PVP_min_FoL_hp && CanCast("Flash of Light", tar) && !(Me.IsMoving)) { s = "Flash of Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (hp < PVP_min_FoL_on_tank_hp && tar.Guid == tank.Guid && CanCast("Flash of Light", tar) && !(Me.IsMoving)) { s = "Flash of Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (CanCast("Holy Light", tar) && !(Me.IsMoving) && hp < Math.Min(PVP_min_HL_hp, 85)) { s = "Holy Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }



             //   slog(Color.Green, "Casting " + s + " on {0} at {1} %", tar.Name, Round(tar.HealthPercent));
                //if (s == "Hand of Protection") { slog(Color.DarkGreen, "I'm using HoP.. on {0}, tank is {1}, tank should be {2}", tar.Name, tank.Name, RaFHelper.Leader.Name); }
                return false;

            }
            else
            {
                return false;
            }
        }



        #endregion

        #region Helpers
        
        /*
        private WoWUnit PVPGiveEnemy(int distance)
        {
            WoWUnit enemy = (from unit in ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                             orderby unit.HealthPercent ascending
                             //where unit.IsHostile                                                         PVE
                             //where (unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember)      //  PVP e si potrebbe anche togliere
                             where !unit.Dead
                             where tank.CurrentTargetGuid == unit.Guid
                             //where tank.CurrentTargetGuid == unit.Guid
                             where unit.Distance < distance
                             where unit.InLineOfSight
                             
                             select unit
                             
                            ).FirstOrDefault();
            if (enemy != null) { slog(Color.Black, "Enemy {0}", enemy.Name); }
            return enemy;
        }*/
        private WoWPlayer PVPGiveEnemy(int distance)
        {
            
            WoWPlayer enemy = (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true,false)
                             //where unit.IsHostile                                                         
                             orderby unit.HealthPercent ascending
                             //where (unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember)      //  PVP e si potrebbe anche togliere
                             where !unit.IsInMyPartyOrRaid
                             where !unit.Dead
                             //where tank.CurrentTargetGuid == unit.Guid
                             where unit.Distance < distance
                             where unit.InLineOfSight
                             where !unit.IsPet
                             select unit
                            ).FirstOrDefault();
          //  if (enemy != null) { slog(Color.Black, "Enemy {0}", enemy.Name); }
            return enemy;
        }


        private bool PVPInterrupt()
        {
            if (Me.IsCasting && (lastCast != null && !lastCast.Dead && lastCast.HealthPercent >= PVP_do_not_heal_above) && isinterrumpable)
            {
                SpellManager.StopCasting();
                if (lastCast != null) { slog(Color.Brown, "Interrupting Healing, target at {0} %", Round(lastCast.HealthPercent)); }
                isinterrumpable = false;
                lastCast = null;
                return true;
            }
            else if (Me.IsCasting)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool PVPOhShitButton()
        {
            if (((!GotBuff("Divine Favor") && CanCast("Divine Favor")) ||
                (!GotBuff("Avenging Wrath") && CanCast("Avenging Wrath")) ||
                (!GotBuff("Guardian of Ancient Kings") && CanCast("Guardian of Ancient Kings"))) ||
                (!GotBuff("Aura Mastery") && CanCast("Aura Mastery"))
              )
            {
                slog("Pressing OH SHIT buttons!");
                ChainSpells("Divine Favor", "Guardian of Ancient Kings", "Avenging Wrath");
                return true;
            }
            return false;
        }

        private void ChainSpells(params string[] spells)
        {
            string macro = "";
            foreach (string s in spells)
            {
                macro += "CastSpellByName(\"" + s + "\", true);";
            }
            Lua.DoString(macro);
        }
        /*
        private WoWPlayer HoFNowTarget()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, false)
                    orderby unit.HealthPercent ascending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.Distance < 80
                    where (unit.IsInMyPartyOrRaid || unit.IsMe)
                    where NeedHoFNow(unit)
                    select unit).FirstOrDefault();

        }
        */
        private WoWPlayer PVPGetUrgentCleanseTarget()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    orderby unit.HealthPercent ascending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.Distance < 80
                    where (unit.IsInMyPartyOrRaid || unit.IsMe)
                    where PVPNeedsUrgentCleanse(unit)
                    select unit).FirstOrDefault();
        }
        /*
        private bool NeedHoFNow(WoWPlayer p)
        {
            foreach (WoWAura a in p.ActiveAuras.Values)
            {
                if ((a.IsHarmful) &&
                    (!p.ActiveAuras.ContainsKey("Unstable Affliction")) &&
                    ((p.ActiveAuras.ContainsKey("Entangling Roots")) ||
                    (p.ActiveAuras.ContainsKey("Frost Nova")) ||
                    (p.ActiveAuras.ContainsKey("Chains of Ice"))
                ))
                {
                    WoWDispelType t = a.Spell.DispelType;
                    if (t == WoWDispelType.Disease || t == WoWDispelType.Magic || t == WoWDispelType.Poison)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        */
        /*
        private bool PVPNeedsUrgentCleanse(WoWPlayer p)
        {
            foreach (WoWAura a in p.ActiveAuras.Values)
            {
                if (
                    (!p.ActiveAuras.ContainsKey("Unstable Affliction")) &&
                    ((p.ActiveAuras.ContainsKey("Fear")) ||
                    (p.ActiveAuras.ContainsKey("Polymorph")) ||
                    (p.ActiveAuras.ContainsKey("Freezing Trap")) ||
                    (p.ActiveAuras.ContainsKey("Wyvern Sting")) ||
                    (p.ActiveAuras.ContainsKey("Seduction")) ||
                    (p.ActiveAuras.ContainsKey("Mind Control")) ||
                    //(p.ActiveAuras.ContainsKey("Entangling Roots")) ||
                    //(p.ActiveAuras.ContainsKey("Frost Nova")) ||
                    //(p.ActiveAuras.ContainsKey("Chains of Ice")) ||
                    (p.ActiveAuras.ContainsKey("Repentance")) ||
                    (p.ActiveAuras.ContainsKey("Hex")) ||
                    (p.ActiveAuras.ContainsKey("Psychic Scream")) ||
                    (p.ActiveAuras.ContainsKey("Hammer of Justice")) ||
                    (p.ActiveAuras.ContainsKey("Intimidating Shout")) ||
                    (p.ActiveAuras.ContainsKey("Howl of Terror"))
                )){
                    WoWDispelType t = a.Spell.DispelType;
                    slog(Color.Orange, "There is a urgent buff to dispell!");
                    if (t == WoWDispelType.Disease || t == WoWDispelType.Magic || t == WoWDispelType.Poison)
                    {
        //                slog(Color.Orange, "And is dispellable");
                        return true;
                    }
                    else
                    {
          //              slog(Color.Orange, "But is not dispellable");
                    }
                }
            }
            return false;
        }*/
        private bool PVPNeedsUrgentCleanse(WoWPlayer p)
        {
            if (p.ActiveAuras.ContainsKey("Unstable Affliction"))
            {
                return false;
            }
            foreach (WoWAura a in p.Debuffs.Values)
            {
                if (a.Name == "Fear" || a.Name == "Polymorph" || a.Name == "Freezing Trap" || a.Name == "Wyvern Sting" || a.Name == "Seduction" || a.Name == "Mind Control" || a.Name == "Repetance" || a.Name == "Psychic Scream"
                    || a.Name == "Hammer of Justice" || a.Name == "Intimidating Shout" || a.Name == "Howl of Terror")
                {
                    WoWDispelType t = a.Spell.DispelType;
                    //slog(Color.Orange, "There is a urgent buff to dispell!");
                    if (t == WoWDispelType.Disease || t == WoWDispelType.Magic || t == WoWDispelType.Poison)
                    {
                        //                slog(Color.Orange, "And is dispellable");
                        return true;
                    }
                    //return true;
                }
            }
            return false;
        }
        #endregion
    }   
}

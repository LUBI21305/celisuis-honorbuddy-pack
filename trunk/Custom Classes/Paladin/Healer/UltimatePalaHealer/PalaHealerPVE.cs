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

        private void DungeonCombat()
        {
            tank = GetTank();
            //checkthetank();
            if (unitcheck(tank) < 0)
            {
                slog(Color.DarkRed, "Ok, I'm lost, I thought we found a tank, instead I did not, CC is in Pause, please report this Error with full LOG!");
                return;
            }
            /*
            if (tank == null) 
            { 
                tank = Me; 
            }
            if (!tank.IsValid) { return; }
                              */

            Enemy = PVEGiveEnemy(29);
            if (PVE_wanna_target && (Me.CurrentTarget == null || !Me.CurrentTarget.IsValid || Me.CurrentTarget.Dead))
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
            if (Interrupt()) 
            {
                return; 
            }
            else if (NeedtoRest()) 
            { 
                slog("resting"); 
                return; 
            }
            else if (PVE_wanna_mount && MountUp()) { return; }
            else if (Self(PVE_wanna_DP, PVE_wanna_DS, PVE_DP_min_hp, PVE_DS_min_hp)) 
            //else if (Self(true, true, true, 80, 101)) 
            //else if (Self(PVP_wanna_DP, PVP_wanna_DS, PVP_wanna_everymanforhimself, PVP_DP_min_hp, PVP_DS_min_hp))
            { 
                return; 
            }
            else if (Racials(PVE_use_stoneform, PVE_wanna_everymanforhimself, PVE_use_escapeartist, PVE_min_run_to_HoF, PVE_use_gift, PVE_min_gift_hp, PVE_use_bloodfury, PVE_bloodfury_min_hp, PVE_use_warstomp, PVE_min_warstomp_hp, PVE_use_bersekering, PVE_min_bersekering_hp, PVE_use_will_forsaken, PVE_use_torrent, PVE_min_torrent_hp)) { return; }
            else if (Me.Combat && PVE_wanna_Judge && Judge(PVE_mana_judge)) 
            { 
                return; 
            }
            else if (PVE_want_urgent_cleanse && PVECleanseNow()) 
            { 
                return; 
            }
            else if (Me.Combat && EnemyInterrupt(PVE_wanna_rebuke, PVE_wanna_HoJ, PVE_wanna_move_to_HoJ)) 
            { 
                return; 
            }
            else if (PVEHealing(PVE_wanna_move_to_heal)) 
            { 
                return; 
            }
            else if (ManaRec(PVE_min_Divine_Plea_mana)) 
            { 
                return; 
            }
            else if (PVE_want_cleanse && (Me.Combat || !Me.Mounted) && Cleansing()) 
            { 
                return; 
            }
            else if (TopOff(PVE_wanna_move_to_heal,PVE_min_FoL_hp,PVE_min_DL_hp, PVE_min_HL_hp)) 
            { 
                return; 
            }
            else if (Me.Combat && Dps(PVE_wanna_HoW, PVE_wanna_Denunce))
            {
                return;
            }
            else if (Me.Combat && ConsumeTime(PVE_wanna_CS)) 
            { 
                return; 
            }
            else if (PVE_wanna_buff && Buff(PVE_wanna_move_to_heal)) { return; }
            
            else
            {

            }
            return;
        }
        private bool RaidCombat()
        {
            if (unitcheck(tar) == 1 && (tar.Auras.ContainsKey("Finkle\'s Mixture") || tar.Auras.ContainsKey("Finkle's Mixture")))
            {
                chimaeron = true;
                chimaeron_p1 = true;
            }
            else if (!Me.Combat)
            {
                chimaeron = false;
                chimaeron_p1 = false;
            }
            else
            {
                chimaeron_p1 = false;
            }
            if (unitcheck(tar)==1 && ( GotBuff("Mortality") || GotBuff("Mortality", tar)))
            {
                SoloCombat();
                return false;
            }
            DungeonCombat();
            return false;
        }
        private void SoloCombat()
        {
            tank = Me;
            if (unitcheck(tank) < 0)
            {
                slog(Color.DarkRed, "Ok, I'm lost, I thought we found a tank, instead I did not, CC is in Pause, please report this Error with full LOG!");
                return;
            }

            if (Me.Combat)
            {
                if (chimaeron)
                {
                    Enemy = PVEGiveEnemy(29);
                }
                else
                {
                    Enemy = SoloGiveEnemy(29);
                }
            }
            else
            {
                Enemy = null;
            }
            if (unitcheck(Enemy) > 0 && (Me.CurrentTarget == null || !Me.CurrentTarget.IsValid || Me.CurrentTarget.Dead))
            {
                    Enemy.Target();
            }

           // if (Interrupt() || StyxWoW.GlobalCooldown)
           // {
           //     return;
           // }
            else if (NeedtoRest())
            {
                slog("resting");
                return;
            }
            else if (Solo_wanna_buff && Buff(PVE_wanna_move_to_heal)) { return; }
            else if (PVE_wanna_mount && MountUp()) { return; }
            else if (Self(PVE_wanna_DP, PVE_wanna_DS, PVE_DP_min_hp, PVE_DS_min_hp))
            //else if (Self(true, true, true, 80, 101)) 
            //else if (Self(PVP_wanna_DP, PVP_wanna_DS, PVP_wanna_everymanforhimself, PVP_DP_min_hp, PVP_DS_min_hp))
            {
                return;
            }
            else if (Racials(PVE_use_stoneform, PVE_wanna_everymanforhimself, PVE_use_escapeartist, PVE_min_run_to_HoF, PVE_use_gift, PVE_min_gift_hp, PVE_use_bloodfury, PVE_bloodfury_min_hp, PVE_use_warstomp, PVE_min_warstomp_hp, PVE_use_bersekering, PVE_min_bersekering_hp, PVE_use_will_forsaken, PVE_use_torrent, PVE_min_torrent_hp)) { return; }
            else if (Me.Combat && PVE_wanna_Judge && Judge(Solo_mana_judge))
            {
                return;
            }
            else if (PVE_want_urgent_cleanse && PVECleanseNow())
            {
                return;
            }
            else if (Solo_wanna_move && Solomove())
            {
                return;
            }
            else if (Me.Combat && EnemyInterrupt(Solo_wanna_rebuke, Solo_wanna_HoJ, Solo_wanna_move_to_HoJ))
            {
                return;
            }
            else if (Me.Combat && Dps(PVE_wanna_HoW, PVE_wanna_Denunce))
            {
                return;
            }
            else if (!GotBuff("Mortality") && PVEHealing(PVE_wanna_move_to_heal))
            {
                return;
            }
            else if (ManaRec(PVE_min_Divine_Plea_mana))
            {
                return;
            }
            else if (PVE_want_cleanse && (Me.Combat || !Me.Mounted) && Cleansing())
            {
                return;
            }
            else if (Me.Combat && Solo_Combat())
            {
                return;
            }
            else if (Me.Combat && ConsumeTime(PVE_wanna_CS))
            {
                return;
            }
            else if (!GotBuff("Mortality") && TopOff(PVE_wanna_move_to_heal,PVE_min_FoL_hp,PVE_min_DL_hp, PVE_min_HL_hp))
            {
                return;
            }
            
            return;
        }

        private bool RafCombat()
        {
            DungeonCombat();
            return false;
        }
        #endregion

        #region Action
        /*
        private WoWPlayer NeedHoFNoW()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, false)
                    orderby unit.MovementInfo.RunSpeed ascending
                    where !Blacklist.Contains(unit.Guid, true)  //do not heal a blacklisted target
                    where unit.IsValid
                    where !unit.Dead
                    where !unit.IsGhost
                    where (unit.IsInMyPartyOrRaid || unit.IsMe)
                    where unit.Distance < 30
                    where !unit.IsPet
                    where unit.MovementInfo.RunSpeed < PVE_min_run_to_HoF
                    select unit).FirstOrDefault();    
        }
        */
        private bool PVECleanseNow()
        {
            WoWPlayer NeedFreedom;
            NeedFreedom = NeedHoFNoW();
            /*
            if ((NeedFreedom == null || !NeedFreedom.IsValid) && Me.MovementInfo.RunSpeed < PVE_min_run_to_HoF)
            {
                NeedFreedom = Me;
            }
             */
            if (PVE_wanna_HoF && NeedFreedom != null && NeedFreedom.IsValid && !NeedFreedom.Dead)
            {
                if (NeedFreedom.Combat && CanCast("Hand of Freedom", NeedFreedom) )//&& NeedFreedom.MovementInfo.RunSpeed < PVE_min_run_to_HoF)
                {
                    Cast("Hand of Freedom", NeedFreedom, 30, "OhShit", "Target is slowed or snared");
                    slog(Color.Violet, "Hand of Freedom on {0} is movement speed is {1}", NeedFreedom.Name, NeedFreedom.MovementInfo.RunSpeed);
                }
                else if(NeedFreedom.Combat && !CanCast("Hand of Freedom", NeedFreedom) )
                {
                    Cast("Cleanse", NeedFreedom, 30, "OhShit", "Target is slowed or snared");
                    slog(Color.Violet, "Cleanse on {0} couse HoF is on cooldown", NeedFreedom.Name);
                }
            }
                /*
                foreach (WoWPlayer d in Me.PartyMembers)
                {
                    if (d.MovementInfo.RunSpeed < PVE_min_run_to_HoF && CanCast("Hand of Freedom", d) && d.Combat && d.Distance < 30)
                    {
                        Cast("Hand of Freedom", d, 30, "OhShit", "Target is slowed or snared");
                        //slog(Color.Violet, "Hand of Freedom on {0}", d.Name);
                    }
                }*/

            WoWPlayer p = PVEGetUrgentCleanseTarget();
            //slog(Color.Violet, "Urgent Cleansing {0}", p.Name);
            return Cast("Cleanse", p, 40, "Cleanse", "Need Urgent Dispell");
        }

        private bool PVEHealing(bool should_move)
        {
            if (tar != null && tank != null && tar.IsValid && tank.IsValid && !tar.Dead)
            {
                castedDL = false;
                String s = null;
                double hp = tar.HealthPercent;
                if (tank != null && Me.Mounted && !Me.Combat && !tank.Combat) { return false; } else if (tank == null) { return false; }
                if (tank != null && !Me.Combat && !tar.Combat && tar.Distance > PVE_max_healing_distance) { return false; } else if (tank == null) { return false; }

                if (should_move && (tar.Distance > 30 || !tar.InLineOfSight)) { slog("Healing target is too far away or not in LOS, moving to them!"); MoveTo(tar); return true; }

                if (Me.Combat && hp < PVE_ohshitbutton_activator && !chimaeron_p1)
                {
                    if (GotBuff("Divine Plea", Me)) { Lua.DoString("CancelUnitBuff(\"Player\",\"Divine Plea\")"); slog(Color.DarkOrange, "Cancelling Divine Plea due to a Oh Shit! moment"); }
                    PVEOhShitButton();
                }

                if (!chimaeron && PVE_wanna_LoH && hp < PVE_min_LoH_hp && !tar.ActiveAuras.ContainsKey("Forbearance") && CanCast("Lay on Hands", tar) && (Me.Combat || tar.Combat)) { s = "Lay on Hands"; return Cast(s, tar, 40, "Heal", "Saving someone life"); }
                else if (!chimaeron && PVE_wanna_HoP && hp < PVE_min_HoP_hp && tar.Guid != tank.Guid && !IsTank(tar) && !tar.ActiveAuras.ContainsKey("Forbearance") && CanCast("Hand of Protection", tar) && (Me.Combat || tar.Combat)) { s = "Hand of Protection"; return Cast(s, tar, 30, "Heal", "Saving someone life"); }
                else if (PVE_wanna_HoS && hp < PVE_min_HoS_hp && tar.Guid != tank.Guid && CanCast("Hand of Sacrifice", tar) && Me.HealthPercent > 90 && (Me.Combat || tar.Combat)) { s = "Hand of Sacrifice"; return Cast(s, tar, 30, "Heal", "I'm fine can Sacrifice"); }
                else if (tar.Guid == tank.Guid && BeaconNeedsRefresh(tank) && CanCast("Beacon of Light", tar) && tank.Distance < 30 && !Me.Mounted) { s = "Beacon of Light"; return Cast(s, tar, 40, "Heal", "Beacon is dropping off"); }
                /*else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar) && tar.Guid == tank.Guid && tar.HealthPercent <= 65) { s = "Word of Glory"; }
                else if (Me.CurrentHolyPower == 3 && CanCast("Light of Dawn") && ShouldLightofDawn(3)) { s = "Light of Dawn"; }
                else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar)) { s = "Word of Glory"; }
                */
                else if (SealNeedRefresh() && !Me.Mounted) { s = "Seal of Insight"; return Cast(s, "Buff", "Missing Seal"); }
                //else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar) && tar.HealthPercent <= 35) { s = "Word of Glory"; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (Me.CurrentHolyPower == 3 && HolyPowerDump()) { s = "Light of Dawn"; return Cast(s, "Heal", "Healing"); }
                else if (Me.CurrentHolyPower == 3 && CanCast("Word of Glory", tar)) { s = "Word of Glory"; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (CanCast("Holy Shock", tar)) { s = "Holy Shock"; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (PVE_want_HR && ShouldHolyRadiance(PVE_min_player_inside_HR, PVE_HR_how_far, PVE_HR_how_much_health)) { s = "Holy Radiance"; return Cast(s, "Heal", "Healing"); }
                else if (PVE_Inf_of_light_wanna_DL && GotBuff("Infusion of Light") && hp < PVE_Inf_of_light_min_DL_hp && !(Me.IsMoving)) { s = "Divine Light"; isinterrumpable = true; castedDL = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (!PVE_Inf_of_light_wanna_DL && GotBuff("Infusion of Light") && !(Me.IsMoving)) { s = "Holy Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (hp < PVE_min_FoL_hp && CanCast("Flash of Light", tar) && !(Me.IsMoving)) { s = "Flash of Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (hp < PVE_min_DL_hp && CanCast("Divine Light", tar) && !(Me.IsMoving)) { s = "Divine Light"; isinterrumpable = true; castedDL = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                else if (hp < Math.Min(PVE_min_HL_hp, 85) && CanCast("Holy Light", tar) && !(Me.IsMoving)) { s = "Holy Light"; isinterrumpable = true; return Cast(s, tar, 40, "Heal", "Healing"); }
                


                //slog(Color.Green, "Casting " + s + " on {0} at {1} %", tar.Name, Round(tar.HealthPercent));
                //if (tar == null) { slog("BoP but tar NULL WTF??"); }
                //if (tank == null) { slog("BoP but tank NULL WTF??"); }
                //if (RaFHelper.Leader == null) { slog("Bop but rafhelper.leader NULL WTF??"); }
                //if (s == "Hand of Protection" && tar != null && tank != null && RaFHelper.Leader != null) { slog(Color.DarkGreen, "I'm using HoP.. on {0}, tank is {1}, tank should be {2}", tar.Name, tank.Name, RaFHelper.Leader.Name); }
                //return Cast(s, tar, 40, "Heal", "Healing");
                return false;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Helpers

        private WoWUnit SoloGiveEnemy(int distance)
        {
            WoWUnit enemy = (from unit in ObjectManager.GetObjectsOfType<WoWUnit>(true, false)
                             where unit.IsHostile
                             where unit.CurrentTarget==Me
                             where !unit.Dead
                             where unit.Distance < distance
                             select unit
                            ).FirstOrDefault();
            return enemy;
        }

        private WoWUnit PVEGiveEnemy(int distance)
        {
            WoWUnit enemy = (from unit in ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                             where unit.IsHostile
                             where !unit.Dead
                             where (tank!=null && tank.CurrentTargetGuid == unit.Guid)
                             where unit.Distance < distance
                             where unit.InLineOfSight
                             select unit
                            ).FirstOrDefault();
            return enemy;
        }

        private bool PVEOhShitButton()
        {
            if (!GotBuff("Divine Favor") && !GotBuff("Avenging Wrath") && !GotBuff("Guardian of Ancient Kings"))
            {
                if (PVE_wanna_AW && CanCast("Avenging Wrath")) 
                {     
                //    slog(Color.Black, "Oh Shit! Casting Avenging Wrath"); 
                    return Cast("Avenging Wrath", "OhShit", "Need more power");
                }
                else if (PVE_wanna_DF && CanCast("Divine Favor")) 
                {
                    //slog(Color.Black, "Oh Shit! Casting Divine Favor"); 
                    return Cast("Divine Favor", "OhShit", "Need more power"); 
                }
                else if (PVE_wanna_GotAK && CanCast("Guardian of Ancient Kings")) 
                {
                    //slog(Color.Black, "Oh Shit! Casting Guardian of Ancient Kings"); 
                    return Cast("Guardian of Ancient Kings", "OhShit", "Need more power"); 
                }
            }
            return false;
        }

        private WoWPlayer PVEGetUrgentCleanseTarget()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    orderby unit.HealthPercent ascending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.Distance < 80
                    where (unit.IsInMyPartyOrRaid || unit.IsMe)
                    where PVENeedsUrgentCleanse(unit)
                    select unit).FirstOrDefault();
        }

        private bool PVENeedsUrgentCleanse(WoWPlayer p)
        {

            foreach (WoWAura a in p.Debuffs.Values)//p.ActiveAuras.Values)
            {

                /*if (
                    (p.ActiveAuras.ContainsKey("Static Cling")) ||
                    (p.ActiveAuras.ContainsKey("Flame Shock")) ||
                    (p.ActiveAuras.ContainsKey("Static Discharge")) ||
                    (p.ActiveAuras.ContainsKey("Consuming Darkness")) ||
                    (p.ActiveAuras.ContainsKey("Lash of Anguish")) ||
                    (p.ActiveAuras.ContainsKey("Static Disruption")) ||
                    (p.ActiveAuras.ContainsKey("Accelerated Corruption")) ||
                    (p.ActiveAuras.ContainsKey("Mangle")) ||
                    (p.ActiveAuras.ContainsKey("Corruption")) ||
                    (p.ActiveAuras.ContainsKey("Fear")
                    )
                   )*/
                if(a.Name=="Fear"||a.Name=="Static Cling"||a.Name=="Flame Shock"||a.Name=="Static Discharge"||a.Name=="Consuming Darkness"||a.Name=="Lash of Anguish"||a.Name=="Static Disruption"
                    ||a.Name=="Accelerated Corruption")
                {
                    
                    //slog(Color.Orange, "There is a urgent buff to dispell!");
                    WoWDispelType t = a.Spell.DispelType;
                    if (t == WoWDispelType.Disease || t == WoWDispelType.Magic || t == WoWDispelType.Poison)
                    {
                    //    slog(Color.Orange, "And is dispellable {0} harmfull {1} name {2} applyauratype {3}, flags {4} category {5} dispelltype {6}", t.ToString(), a.IsHarmful.ToString(), a.Name, a.ApplyAuraType.ToString(), a.Flags.ToString(), a.Spell.Category.ToString(), a.Spell.DispelType.ToString());
                        return true;
                    }
                    else
                    {
                    //    slog(Color.Orange, "But is not dispellable {0} harmfull {1} name {2} applyauratype {3}, flags {4} category {5} dispelltype {6}", t.ToString(), a.IsHarmful.ToString(), a.Name, a.ApplyAuraType.ToString(), a.Flags.ToString(), a.Spell.Category.ToString(), a.Spell.DispelType.ToString());
                    }
                    //return true;
                }
            }
            return false;
        }

        #endregion
    }
}
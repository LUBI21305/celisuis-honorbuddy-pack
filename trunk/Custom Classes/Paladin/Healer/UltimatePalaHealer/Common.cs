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
    partial class UltimatePalaHealer
    {
        #region Action

        private bool Interrupt()
        {
            if (Me.IsCasting && (lastCast != null && !lastCast.Dead && lastCast.HealthPercent >= PVE_do_not_heal_above) && isinterrumpable)
            {
                SpellManager.StopCasting();
                if (lastCast != null)
                {
                    slog(Color.Brown, "Interrupting Healing, target at {0} %", Round(lastCast.HealthPercent));
                }
                lastCast = null;
                isinterrumpable = false;
                return true;
            }
            else if (castedDL && Me.IsCasting && lastCast != null && !lastCast.Dead && lastCast.HealthPercent >= PVE_Inf_of_light_min_DL_hp + 10 && isinterrumpable)
            {
                castedDL = false;
                lastCast = null;
                SpellManager.StopCasting();
                slog(Color.Brown, "Interrupting Divine Light, target at {0} %", Round(lastCast.HealthPercent));
                isinterrumpable = false;
                return true;
            }
                /*
            else if (Me.IsCasting)
            {

                return true;
            }
                 */
            else
            {
                return false;
            }
        }

        public bool NeedtoRest()
        {
            if (Me.ManaPercent > 20 && usedBehaviour != "Battleground" && !Me.Combat && !tank.Combat && Resurrecting()) { return true; }

            else if ((Me.ManaPercent <= rest_if_mana_below) && (!Me.Combat) && (!Me.HasAura("Drink")) && (!Me.IsMoving) && (!Me.Mounted))
            {
                slog(Color.Blue, "#Out of Combat - Mana is at {0} %. Time to drink.#", Round(Me.ManaPercent));
                Styx.Logic.Common.Rest.Feed();
                return true;
            }

            // This is to ensure we STAY SEATED while eating/drinking. No reason for us to get up before we have to.
            if ((!Me.Combat) && (Me.HasAura("Drink") && ((Me.ManaPercent < 95)|| (!GotBuff("Well Fed") && Me.ActiveAuras["Drink"].TimeLeft.TotalSeconds >19))))
            {
                return true;
            }

            return false;
        }


        private bool EnemyInterrupt(bool rebuke, bool HoJ, bool also_move)
        {
            bool casted = false, thiscasted = false;
            if (Enemy!=null && Enemy.IsValid && !Enemy.Dead && Enemy.IsCasting)
            {
                if (rebuke)
                {
                    //slog(Color.DarkMagenta, "Casting Rebuke on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                    thiscasted= Cast("Rebuke", Enemy, 5, "EnemyInterrupt", "Interrupting");
                    if (thiscasted) { casted = true; }
                }
                if (HoJ && Enemy.HealthPercent < 50)
                {
                    slog(Color.DarkMagenta, "Wanna Hammer of Justice on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                    if (also_move && Enemy.Distance > 10 && Enemy.Distance < 40)
                    {
                        slog(Color.DarkMagenta, "Moving in to Hammer of Justice");
                        MoveTo(Enemy);
                    }
                    else
                    {
                        //slog(Color.DarkMagenta, "Casting Hammer of Justice on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                        thiscasted= Cast("Hammer of Justice", Enemy, 10, "EnemyInterupt", "Interrupting"); ;
                        if (thiscasted) { casted = true; }
                    }
                }
            }
            return casted;
        }

        private bool Racials(bool stoneform, bool EMFH, bool escapeartist, float minspeed, bool giftnaaru, int minhpgift, 
            bool bloodfury, int minbllodf, bool warstomp, int minwarstomphp, bool berserking, int minbers, bool willforsaken,
            bool arcanetorrent, int minarcanetorrent)
        {
            bool casted = false, thiscasted = false;
            if (stoneform && CanCast("Stoneform") && Me.HealthPercent < stoneform_perc && !GotBuff("Divine Protection"))
            {
                thiscasted = Cast("Stoneform", "Self", "Low HP");
                if (thiscasted) { casted = true; }
            }
            if (EMFH && Me.Stunned && !GotBuff("Charge Stun") && !GotBuff("Sap"))
            {
                //slog(Color.Red, "Stunned, casting Every Man for Himself");
                thiscasted = Cast("Every Man for Himself", "Self", "Stunned");
                if (thiscasted) { casted = true; }
            }
            if (escapeartist && !Me.Stunned && Me.MovementInfo.RunSpeed < minspeed)
            {
                thiscasted = Cast("Escape Artist", "Self", "Slowed or Snared");
                if (thiscasted) { casted = true; }
            }
            if (giftnaaru && unitcheck(tar) == 1 && tar.HealthPercent < minhpgift)
            {
                thiscasted = Cast("Gift of the Naaru", tar,40,"Self", "Free heal");
                if (thiscasted) { casted = true; }
            }
            if (bloodfury && unitcheck(tar) == 1 && tar.HealthPercent < minbllodf)
            {
                thiscasted = Cast("Blood Fury", "Self", "Need more power");
                if (thiscasted) { casted = true; }
            }
            if (warstomp && Me.HealthPercent < minwarstomphp)
            {
                thiscasted = Cast("War Stomp", "Self", "2 secon of peace");
                if (thiscasted) { casted = true; }
            }
            if (berserking && unitcheck(tar) == 1 && tar.HealthPercent < minbers)
            {
                thiscasted = Cast("Berserking", "Self", "Need more power");
                if (thiscasted) { casted = true; }
            }
            if (willforsaken && Me.Fleeing)
            {
                thiscasted = Cast("Will of the Forsaken", "Self", "Feared");
                if (thiscasted) { casted = true; }
            }
            if (arcanetorrent && unitcheck(Enemy) == 1 && Enemy.IsCasting && Enemy.HealthPercent < minarcanetorrent && Enemy.Distance<8)
            {
                thiscasted = Cast("Arcane Torrent", "Self", "Silencing enemy");
                if (thiscasted) { casted = true; }
            }
            return casted;
        }

        private bool Self(bool DP, bool DS, int DP_perc, int DS_perc)
        {  
            bool casted=false, thiscasted=false;
            if (sw.Elapsed.TotalSeconds > use_mana_rec_trinket_every && Me.Combat && Me.ManaPercent <= use_mana_rec_trinket_on_mana_below)
            {
                Lua.DoString("UseItemByName(\"" + "Tyrande\'s Favorite Doll" + "\")");
                slog(Color.Blue, "Mana at {0} % using Tyrande\'s Favorite Doll", Round(Me.ManaPercent));
                sw.Reset();
                sw.Start();
            }
            if (use_mana_potion && Me.ManaPercent <= use_mana_potion_below)
            {
                // use potion?
            }
            if (DP && Me.HealthPercent < DP_perc && Me.Combat && CanCast("Divine Protection"))
            {
                //slog(Color.Red, "Health at {0} % casting Divine Protection", Round(Me.HealthPercent));
                thiscasted = Cast("Divine Protection", "Self", "Low HP");
                if (thiscasted) { casted = true; }
            }

            if (DS && Me.HealthPercent < DS_perc && !GotBuff("Forbearance") && Me.Combat)
            {
                //slog(Color.Red, "Health at {0} % casting Divine Shield", Round(Me.HealthPercent));
                thiscasted=Cast("Divine Shield", "Self", "Low HP");
                if (thiscasted) { casted = true; }
            }
            else
            {
                return false;
            }
            return casted;
        }

        private bool Judge(int manajudge)
        {
            if ((!GotBuff("Judgements of the Pure") || Me.ManaPercent < manajudge) && unitcheck(Enemy)==1)
            {
                //slog(Color.DarkMagenta, "Casting Judgment on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                return Cast("Judgement", Enemy, Judgment_range, "Buff", "Missing Judgment of the Pure");
            }
            return false;
        }

        private bool Judgeformana()
        {
            //slog(Color.DarkMagenta, "Casting Judgment on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
            return Cast("Judgement", Enemy, Judgment_range, "Mana", "Judging to rec mana");
        }

        private bool ManaRec(int DP_min)
        {
            if (Me.Combat && Enemy != null && Enemy.IsValid && !Enemy.Dead)
            {
                Judgeformana();
            }
            if (Me.ManaPercent <= DP_min && Me.Combat)
            {
                //slog(Color.Blue, "Mana at {0} % using Divine Plea", Round(Me.ManaPercent));
                return Cast("Divine Plea", "Mana", "Noone to heal, need mana");
            }
            return false;
        }

        private bool Cleansing()
        {
            WoWPlayer p = GetCleanseTarget();
            //slog(Color.Violet, "Cleansing {0}", p.Name);
            return Cast("Cleanse", p, 40, "Cleanse", "Noone to heal, dispelling");
        }
        private bool Dps(bool HoW, bool Denunce)
        {
            if (HoW && CanCast("Hammer of Wrath") && Me.Combat && Enemy != null && Enemy.IsValid && !Enemy.Dead)
            {
                if (wanna_face) { WoWMovement.Face(); }
                //slog(Color.DarkMagenta, "Casting Hammer of Wrath on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                return Cast("Hammer of Wrath", Enemy, 30, "DPS", "Enemy low on health");
            }
            if (Denunce && GotBuff("Denounce") && Me.Combat && Enemy != null && Enemy.IsValid && !Enemy.Dead)
            {
                if (wanna_face) { WoWMovement.Face(); }
                //slog(Color.DarkMagenta, "Casting Exorcism on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                return Cast("Exorcism", Enemy, 30, "DPS", "Got Denunce buff");
            }
            return false;
        }

        private bool TopOff(bool should_move, int FL, int DL, int HL)
        {
            if (tar != null)
            {
                if (should_move && (tar.Distance > 38 || !tar.InLineOfSight)) { slog("Healing target is too far away or not in LOS, moving to them!"); MoveTo(tar); return true; }

                if (tank != null && (Me.Mounted && !Me.Combat && !tank.Combat) || ((!Me.Combat && !tar.Combat && tar.Distance > 40)))
                {
                    return false;
                }
                else if (tank == null)
                {
                    return false;
                }
                if (!(Me.IsMoving))
                {
                    if (tar.HealthPercent < FL)
                    {
                        return Cast("Flash of Light", tar, 40, "Heal", "Topping people off");
                    }
                    else if (tar.HealthPercent < DL)
                    {
                        return Cast("Divine Light", tar, 40, "Heal", "Topping people off");
                    }
                    else
                    //if (tar.HealthPercent > HL)
                    {
                        //WoWMovement.Face();
                        return Cast("Holy Light", tar, 40, "Heal", "Topping people off");
                        //slog(Color.Green, "Topping off {0} at {1} %", tar.Name, Round(tar.HealthPercent));
                    }
                }
            }
            return false;
        }

        private bool ConsumeTime(bool CS)
        {
            /*
            if (CanCast("Judgement", Enemy))
            {
                Cast("Judgement", Enemy);
                slog(Color.DarkMagenta, "Casting Judgment on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                return true;
            }
            */
            if (CS && Me.Combat && Enemy != null && Enemy.IsValid && !Enemy.Dead)
            {
                if (wanna_face) { WoWMovement.Face(); }
                //slog(Color.DarkMagenta, "Casting Crusader Strike on {0} at {1}", Enemy.Name, Round(Enemy.Distance));
                return Cast("Crusader Strike", Enemy, 5, "DPS", "Melee range, free HP");
            }
            return false;
        }

        private bool Buff(bool should_move)
        {
            bool casted = false, thiscasted = false;
            bool should_king;
            if (!GotBuff("Seal of Insight") && (!Me.Mounted))
            {
                //slog(Color.Violet, "Casting Seal of Insight");
                thiscasted= Cast("Seal of Insight", "Buff", "Missing seal");
                if (thiscasted) { casted = true; }
            }
            if (usedBehaviour == "Battleground")
            {
                if (PVP_wanna_crusader && Me.Mounted)
                {
                    if (!IsPaladinAura("Crusader Aura"))
                    {
                        //slog(Color.Violet, "Casting Concentration Aura");
                        thiscasted = Cast("Crusader Aura", "Buff", "We are mounted, Crudsader aura");
                        if (thiscasted) { casted = true; }
                    }
                }
                else
                {
                    if (aura_type == 0)
                    {
                        if (!IsPaladinAura("Concentration Aura"))
                        {
                            //slog(Color.Violet, "Casting Concentration Aura");
                            thiscasted = Cast("Concentration Aura", "Buff", "Missing aura");
                            if (thiscasted) { casted = true; }
                        }
                    }
                    else if (aura_type == 1)
                    {
                        if (!IsPaladinAura("Resistance Aura"))
                        {
                            thiscasted = Cast("Resistance Aura", "Buff", "Missing aura");
                            if (thiscasted) { casted = true; }
                        }
                    }
                }
            }
            else if (usedBehaviour == "Solo")
            {
                if (Solo_wanna_crusader && Me.Mounted)
                {
                    if (!IsPaladinAura("Crusader Aura"))
                    {
                        //slog(Color.Violet, "Casting Concentration Aura");
                        thiscasted = Cast("Crusader Aura", "Buff", "We are mounted, Crudsader aura");
                        if (thiscasted) { casted = true; }
                    }
                }
                else
                {
                    if (aura_type == 0)
                    {
                        if (!IsPaladinAura("Concentration Aura"))
                        {
                            //slog(Color.Violet, "Casting Concentration Aura");
                            thiscasted = Cast("Concentration Aura", "Buff", "Missing aura");
                            if (thiscasted) { casted = true; }
                        }
                    }
                    else if (aura_type == 1)
                    {
                        if (!IsPaladinAura("Resistance Aura"))
                        {
                            thiscasted = Cast("Resistance Aura", "Buff", "Missing aura");
                            if (thiscasted) { casted = true; }
                        }
                    }
                }
            }
            else
            {
                if (PVE_wanna_crusader && Me.Mounted)
                {
                    if (!IsPaladinAura("Crusader Aura"))
                    {
                        //slog(Color.Violet, "Casting Concentration Aura");
                        thiscasted = Cast("Crusader Aura", "Buff", "We are mounted, Crudsader aura");
                        if (thiscasted) { casted = true; }
                    }
                }
                else
                {
                    if (aura_type == 0)
                    {
                        if (!IsPaladinAura("Concentration Aura"))
                        {
                            //slog(Color.Violet, "Casting Concentration Aura");
                            thiscasted = Cast("Concentration Aura", "Buff", "Missing aura");
                            if (thiscasted) { casted = true; }
                        }
                    }
                    else if (aura_type == 1)
                    {
                        if (!IsPaladinAura("Resistance Aura"))
                        {
                            thiscasted = Cast("Resistance Aura", "Buff", "Missing aura");
                            if (thiscasted) { casted = true; }
                        }
                    }
                }
            }
            if (should_move && (tank.Distance > 38 || !tank.InLineOfSight)) { slog("Healing target is too far away or not in LOS, moving to them!"); MoveTo(tank); return true; }
            if (tank != null && BeaconNeedsRefresh(tank) && !Me.Mounted)
            {
                //slog(Color.Violet, "Casting Beacon of Light on {0}", tank.Name);
                thiscasted= Cast("Beacon of Light", tank, 40, "Buff", "Missing Beacon of light");
                if (thiscasted) { casted = true; }
            }

            should_king = ShouldKing();
            if (should_king && lastbless!="King")
            {
                slog(Color.Violet, "We should King if needed");
                lastbless = "King";
            }
            else if (!should_king && lastbless!="Might")
            {
                slog(Color.Violet, "We should Might if needed");
                lastbless = "Might";
            }
            if (InParty())
            {
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    if (p.Distance < 0 || p.Distance > 40 || p.Dead || p.IsGhost || !p.InLineOfSight)
                        continue;
                    if ((GotBuff("Mark of the Wild", p) || GotBuff("Blessing of Kings", p)) && !GotBuff("Blessing of Might", p) && !should_king && (!Me.Mounted))
                    {
                        //slog(Color.Violet, "Casting Blessing of Might, couse of {0}", p.Name);
                        thiscasted = Cast("Blessing of Might", p, 40, "Buff", "Missing blessing");
                        if (thiscasted) { casted = true; }
                    }
                    else if (!GotBuff("Blessing of Kings", p) && !GotBuff("Mark of the Wild", p) && should_king && (!Me.Mounted))
                    {
                        //slog(Color.Violet, "Casting Blessing of King, couse of {0}", p.Name);
                        thiscasted = Cast("Blessing of Kings", p, 40, "Buff", "Missing blessing");
                        if (thiscasted) { casted = true; }
                    }

                }
            }
            else if (InRaid())
            {
                foreach (WoWPlayer p in Me.RaidMembers)
                {
                    if (p.Distance < 0 || p.Distance > 40 || p.Dead || p.IsGhost || !p.InLineOfSight)
                        continue;
                    if ((GotBuff("Mark of the Wild", p) || GotBuff("Blessing of Kings", p)) && !GotBuff("Blessing of Might", p) && !should_king && (!Me.Mounted))
                    {
                        //slog(Color.Violet, "Casting Blessing of Might, couse of {0}", p.Name);
                        thiscasted = Cast("Blessing of Might", p, 40, "Buff", "Missing blessing");
                        if (thiscasted) { casted = true; }
                    }
                    else if (!GotBuff("Blessing of Kings", p) && !GotBuff("Mark of the Wild", p) && should_king && (!Me.Mounted))
                    {
                        //slog(Color.Violet, "Casting Blessing of King, couse of {0}", p.Name);
                        thiscasted = Cast("Blessing of Kings", p, 40, "Buff", "Missing blessing");
                        if (thiscasted) { casted = true; }
                    }

                }
            }
            else
            {
                if (unitcheck(Me) == 1)
                {

                    if ((GotBuff("Mark of the Wild", Me) || GotBuff("Blessing of Kings", Me)) && !GotBuff("Blessing of Might", Me) && !should_king && (!Me.Mounted))
                    {
                        //slog(Color.Violet, "Casting Blessing of Might, couse of {0}", p.Name);
                        thiscasted = Cast("Blessing of Might", Me, 40, "Buff", "Missing blessing");
                        if (thiscasted) { casted = true; }
                    }
                    else if (!GotBuff("Blessing of Kings", Me) && !GotBuff("Mark of the Wild", Me) && should_king && (!Me.Mounted))
                    {
                        //slog(Color.Violet, "Casting Blessing of King, couse of {0}", p.Name);
                        thiscasted = Cast("Blessing of Kings", Me, 40, "Buff", "Missing blessing");
                        if (thiscasted) { casted = true; }
                    }
                }
            }
            return casted;
        }

        private bool Taunt()
        {
            
            if (unitcheck(Epet)==1 && unitcheck(Epet.CurrentTarget)==1 && Epet.CurrentTarget!=Me && unitcheck(tank) == 1 && (!Me.Mounted || ! tank.Mounted) && (Me.Combat || tank.Combat) && !Me.IsCasting)
            {
                Cast("Righteous Defense", Epet.CurrentTarget, 40, "Utility", "Taunting pet if any");
            }
            return false;
        }

        private bool MountUp()
        {
          //  if (noncombatfrom.Elapsed.TotalSeconds > 2)
          //  {
          //  WoWPlayer localtank;
          //  localtank = GetTank();
          //  if (localtank==null){return false;}else
            if (!tank.IsValid) { return false; }
            if (!Me.IsValid) { return false; }
                if (tank != null && Me != null && !tank.Dead && !Me.Dead && tank.Mounted && !Me.Mounted && !Me.Combat && !tank.Combat && Me != tank && Mount.CanMount())
                {
                    WoWMovement.MoveStop();
                    Mount.MountUp();
                    slog(Color.DarkOrange, "Mounting UP to follow the tank!");
                    lastCast = null;
                    return true;
                }
                else if (tank != null && Me != null && Me.Mounted && !Me.Combat && !tank.Combat)
                {
                    slog(Color.DarkOrange, "We are mounted... chill");
                    lastCast = null;
                    return true;
                }
                noncombatfrom.Reset();
                noncombatfrom.Start();
                return false;
        //    }
        //    return false;

        }
        private bool Solomove()
        {
            if (unitcheck(Enemy)==1 && Enemy.Distance > 5)
            {
                slog(Color.Black, "Solo: Moving to enemy");
                Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(ObjectManager.Me.Location, ObjectManager.Me.CurrentTarget.Location, 2.5f));
                return true;
            }
            return false;
        }

        private bool Solo_Combat()
        {
            bool casted = false, thiscasted = false;
            //slog(Color.Black, "We are dpsissing");
            if (unitcheck(Enemy) > 0)
            {
                if (Me.CurrentHolyPower == 3 && !GotBuff("Inquisition"))
                {
                    thiscasted = Cast("Inquisition", "Buff", "Got plenty of holy power, time to pew pew");
                    if (thiscasted) { casted = true; }
                }
                else if (CanCast("Holy Shock", Enemy) && Enemy.Distance < 20)
                {
                    if (Solo_wanna_face) { WoWMovement.Face(); }
                    thiscasted = Cast("Holy Shock", Enemy, 20, "DPS", "Solo, we need to kill");
                    if (thiscasted) { casted = true; }
                }
                else
                {
                    if (Solo_wanna_face) { WoWMovement.Face(); }
                    thiscasted = Cast("Exorcism", Enemy, 30, "DPS", "Solo, we need to kill");
                    if (thiscasted) { casted = true; }
                }
            }
            return casted;
        }

    }

        #endregion
 }
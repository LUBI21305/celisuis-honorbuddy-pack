using System;
using System.Collections;
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
        #region Spells

        private bool ShouldHolyRadiance(int how_many, int how_far, int how_much_health)
        {
            int counter;
            counter = 0;
            if (!CanCast("Holy Radiance"))
            {
                return false;
            }
            if (InRaid())
            {
                foreach (WoWPlayer p in Me.RaidMembers)
                {
                    if (unitcheck(p) == 1 && p.Distance < how_far && p.HealthPercent < how_much_health && (!p.Auras.ContainsKey("Finkle\'s Mixture") || (p.Auras.ContainsKey("Finkle\'s Mixture") && p.CurrentHealth < 10000)))
                    {
                        counter++;
                    }
                }
            }
            else
            {
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    if (unitcheck(p)==1 && p.Distance < how_far && p.HealthPercent < how_much_health)
                    {
                        counter++;
                    }
                }
                if (Me.HealthPercent < how_much_health) { counter++; }
            }
            //slog(Color.DarkRed,"there are {0} injuried unit in  yard", counter);
            if (counter >= how_many)
            {
                slog(Color.DarkRed, "Holy Radiacen: there are {0} injuried unit in {1} yard", counter,how_far);
                /*
                slog(Color.DarkRed, "Player {0} discance {1} life {2} %", Me.Name, Round(Me.Distance), Round(Me.HealthPercent));
                slog(Color.DarkRed, "Player {0} discance {1} life {2} %", Me.PartyMember1.Name, Round(Me.PartyMember1.Distance), Round(Me.PartyMember1.HealthPercent));
                slog(Color.DarkRed, "Player {0} discance {1} life {2} %", Me.PartyMember2.Name, Round(Me.PartyMember2.Distance), Round(Me.PartyMember2.HealthPercent));
                slog(Color.DarkRed, "Player {0} discance {1} life {2} %", Me.PartyMember3.Name, Round(Me.PartyMember3.Distance), Round(Me.PartyMember3.HealthPercent));
                slog(Color.DarkRed, "Player {0} discance {1} life {2} %", Me.PartyMember4.Name, Round(Me.PartyMember4.Distance), Round(Me.PartyMember4.HealthPercent));
                */
                return true;
            }
            return false;
        }

        private bool ShouldLightofDawn(int how_many)
        {
            int counter;
            counter = 0;

            if (InRaid())
            {
                foreach (WoWPlayer p in Me.RaidMembers)
                {
                    if (unitcheck(p) == 1 && (
                        (p.Distance <= 5 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 150)) ||
                        (p.Distance <= 10 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 90)) ||
                        (p.Distance <= 15 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 53)) ||
                        (p.Distance <= 20 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 36)) ||
                        (p.Distance <= 25 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 28)) ||
                        (p.Distance <= 30 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 22))
                    ))
                    {
                        counter++;
                    }
                }
            }
            else
            {
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    if (unitcheck(p) == 1 && (
                        (p.Distance <= 5 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 150)) ||
                        (p.Distance <= 10 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 90)) ||
                        (p.Distance <= 15 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 53)) ||
                        (p.Distance <= 20 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 36)) ||
                        (p.Distance <= 25 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 28)) ||
                        (p.Distance <= 30 && p.HealthPercent <= maxAOEhealth && Me.IsSafelyFacing(p, 22))
                    ))
                    {
                        counter++;
                    }
                }
                if (Me.HealthPercent <= maxAOEhealth) { counter++; }
            }
            slog(Color.DarkRed, "there are {0} injuried units in Light of Dawn area", counter);
            if (counter >= how_many)
            {
                return true;
            }
            return false;
        }

        private double LoD()
        {
            double LoDcounter;
            int LoDmax,i,LoDnot,maxAOEh;
            double[] LoDeffect = new double[41];
            LoDcounter = 0;
            LoDmax = 0;
            LoDnot = 0;
            if (tar.HealthPercent >= 60)
            {
                maxAOEh = 95;
            }
            else
            {
                maxAOEh = (int)maxAOEhealth;
            }
            foreach (int cont in LoDeffect)
            {
                LoDeffect[cont] = 0;
            }
            i=0;
            if (InRaid())
            {
                foreach (WoWPlayer p in Me.RaidMembers)
                {
                    i++;
                    if (unitcheck(p) == 1 && (
                        (p.Distance <=  5 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p, 150)) ||
                        (p.Distance <= 10 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  90)) ||
                        (p.Distance <= 15 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  53)) ||
                        (p.Distance <= 20 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  36)) ||
                        (p.Distance <= 25 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  28)) ||
                        (p.Distance <= 30 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  22))
                    ))
                    {
                        if (p == tank && p != Me)
                        {
                            LoDcounter++;
                            LoDeffect[i] = 1;
                        }
                        else if (p != Me)
                        {
                            if (tank.HealthPercent > dontHealAbove)
                            {
                                LoDcounter++;
                                LoDeffect[i] = 1;
                            }
                            else
                            {
                                LoDcounter += 1.5;
                                LoDeffect[i] = 1.5;
                            }
                        }
                        LoDmax++;
                        slog("{0} is in range and have {1} hp", privacyname(p), p.HealthPercent);
                        //    if (LoDmax == 5) { slog(Color.DarkRed, "Light of Dawn weight is {0} there are {1} injuried players in range", LoDcounter, LoDmax); return LoDcounter; }
                    }
                }
                if (Me.HealthPercent < dontHealAbove)
                {
                    if (tank.HealthPercent < dontHealAbove)
                    {
                        if (Me != tank)
                        {
                            LoDmax++;
                            LoDeffect[0] = 1.5;
                        }
                        else
                        {
                            LoDmax++;
                            LoDeffect[0] = 1;
                        }
                    }
                    else
                    {
                        LoDeffect[0] = 1;
                    }
                    slog("{0} is in range and have {1} hp", privacyname(Me), Me.HealthPercent);
                    LoDmax++;
                }
                else
                {
                    if (tank.HealthPercent < dontHealAbove)
                    {
                        LoDmax++;
                        LoDeffect[0] = 0.5;
                    }
                    slog("{0} is in range and have {1} hp (overhealing)", privacyname(Me), Me.HealthPercent);
                    LoDnot++;
                }
                i = 0;
                foreach (WoWPlayer p in Me.RaidMembers)
                {
                    i++;
                    if (unitcheck(p) == 1 && (
                        (p.Distance <=  5 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p, 150)) ||
                        (p.Distance <= 10 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  90)) ||
                        (p.Distance <= 15 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  53)) ||
                        (p.Distance <= 20 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  36)) ||
                        (p.Distance <= 25 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  28)) ||
                        (p.Distance <= 30 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  22))
                    ))
                    {
                        if (p == tank && p!=Me)
                        {
                            if (tank.HealthPercent > dontHealAbove)
                            { }
                            else
                            {
                                LoDcounter++;
                                LoDeffect[i] = 1;
                            }
                        }
                        else if (p != Me)
                        {
                            if (tank.HealthPercent > dontHealAbove)
                            { }
                            else
                            {
                                LoDcounter += 0.5;
                                LoDeffect[i] = 0.5;
                            }
                        }
                        slog("{0} is in range and have {1} hp (overhealing)", privacyname(p), p.HealthPercent);
                        LoDnot++;
                        //if (LoDmax == 5) { slog(Color.DarkRed, "Light of Dawn weight is {0} there are {1} injuried players in range", LoDcounter, LoDmax); return LoDcounter; }
                    }
                }
                //if (Me.HealthPercent > maxAOEhealth && tank.HealthPercent < dontHealAbove) { LoDcounter += 0.5; LoDmax++; }
            }
            else if(InParty())
            {
                i = 0;
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    i++;
                    if (unitcheck(p) == 1 && (
                        (p.Distance <=  5 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p, 150)) ||
                        (p.Distance <= 10 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  90)) ||
                        (p.Distance <= 15 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  53)) ||
                        (p.Distance <= 20 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  36)) ||
                        (p.Distance <= 25 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  28)) ||
                        (p.Distance <= 30 && p.HealthPercent <= maxAOEh && Me.IsSafelyFacing(p,  22))
                    ))
                    {
                        if (p == tank && p!=Me)
                        {
                            LoDcounter++;
                            LoDeffect[i] = 1;
                        }
                        else if (p != Me)
                        {
                            if (tank.HealthPercent > dontHealAbove)
                            {
                                LoDcounter += 1;
                                LoDeffect[i] = 1;
                            }
                            else
                            {
                                LoDcounter += 1.5;
                                LoDeffect[i] = 1.5;
                            }
                        }
                        slog("{0} is in range and have {1} hp", privacyname(p), p.HealthPercent);
                        LoDmax++;
                    }
                }
                //if (Me.HealthPercent <= maxAOEhealth && tank.HealthPercent < dontHealAbove) { LoDcounter += 1.5; LoDmax++; } else if (Me.HealthPercent <= maxAOEhealth) { LoDcounter++; LoDmax++; }

                if (Me.HealthPercent < dontHealAbove)
                {
                    if (tank.HealthPercent < dontHealAbove)
                    {
                        if (tank != Me)
                        {
                            LoDmax++;
                            LoDeffect[0] = 1.5;
                        }
                        else
                        {
                            LoDmax++;
                            LoDeffect[0] = 1;
                        }
                    }
                    else
                    {
                        LoDeffect[0] = 1;
                    }
                    LoDmax++;
                    slog("{0} is in range and have {1} hp", privacyname(Me), Me.HealthPercent);
                }
                else
                {
                    if (tank.HealthPercent < dontHealAbove)
                    {
                        LoDmax++;
                        LoDeffect[0] = 0.5;
                    }
                    slog("{0} is in range and have {1} hp", privacyname(Me), Me.HealthPercent);
                    LoDnot++;
                }
                i = 0;
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    i++;
                    if (unitcheck(p)==1 && (
                        (p.Distance <=  5 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p, 150)) ||
                        (p.Distance <= 10 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  90)) ||
                        (p.Distance <= 15 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  53)) ||
                        (p.Distance <= 20 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  36)) ||
                        (p.Distance <= 25 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  28)) ||
                        (p.Distance <= 30 && p.HealthPercent > maxAOEh && Me.IsSafelyFacing(p,  22))
                    ))
                    {
                        if (p == tank && p!=Me)
                        {
                            if (tank.HealthPercent > dontHealAbove)
                            { }
                            else
                            {
                                LoDcounter++;
                                LoDeffect[i] = 1;
                            }
                        }
                        else if (p != Me)
                        {
                            if (tank.HealthPercent > dontHealAbove)
                            { }
                            else
                            {
                                LoDcounter += 0.5;
                                LoDeffect[i] = 0.5;
                            }
                        }
                        slog("{0} is in range and have {1} hp (overhealing)", privacyname(p), p.HealthPercent);
                        LoDnot++;
                    }
                }
              //  if (Me.HealthPercent > maxAOEhealth && tank.HealthPercent > dontHealAbove) { } else { LoDcounter += 0.5; LoDmax++; }
            }
           // slog(Color.DarkRed, "Light of Dawn weight is {0} there are {1} injuried players in range", LoDcounter, LoDmax);
        //    return LoDcounter;
            i = 0;
            Array.Sort(LoDeffect);
            Array.Reverse(LoDeffect);
            double sum;
            sum = 0;
            for(int con=0;con<5;con++)
            {
                sum += LoDeffect[con];
            }
            if (debug)
            {
                for (int cos = 0; cos < LoDeffect.Length; cos++)
                {
                    slog("{0}    {1}", cos, LoDeffect[cos]);
                }
            }
            slog(Color.DarkRed, "Light of Dawn weight is {0} there are {1} injuried players and {2} not injurie player in range", sum, LoDmax,LoDnot);
            return sum;
        }

        private double WoG()
        {
            double WoGcounter;
            WoGcounter = 0;
            if (tar.Guid == tank.Guid)
            {
                WoGcounter = 1;
            }
            else if (tar.Guid != Me.Guid)
            {
                WoGcounter = 1.5;
            }
            else
            {
                WoGcounter = 1.17;
            }
            slog(Color.DarkOliveGreen, "Wog weight is {0} modified {1}", WoGcounter, WoGcounter * 2.37 * 1.3);
            return WoGcounter;
        }

        private bool HolyPowerDump()
        {
            double last_word_modifier;
            if (Me.CurrentHolyPower < 3)
            {
                return false;
            }
            if (tar.HealthPercent <= 35)
            {
                last_word_modifier = 1*(1-last_word*0.3)+1.5*(last_word*0.3);
            }
            else
            {
                last_word_modifier = 1;
            }

            if (WoG() * 2.37 * 1.3 * last_word_modifier> LoD())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void AutoAttack()
        {
            if (!Me.IsAutoAttacking)
            {
                Lua.DoString("StartAttack()");
            }

        }
        #endregion
    }
}

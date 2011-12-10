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
        #region Helpers

        private void slog(string format, params object[] args)
        {
            Logging.Write(format, args);
        }

        private void slog(System.Drawing.Color color, string format, params object[] args)
        {
            Logging.Write(color, format, args);
        }

        private void slog(System.Drawing.Color color, string format)
        {
            Logging.Write(color, format);
        }
        private void formatslog(string type, string reason, string spell, WoWUnit target)
        {
            System.Drawing.Color textcolor;
            switch (type)
            {
                case "Heal":
                    textcolor = Color.Green;
                    break;
                case "Cleanse":
                    textcolor = Color.Magenta;
                    break;
                case "Buff":
                    textcolor = Color.Brown;
                    break;
                case "OhShit":
                    textcolor = Color.Red;
                    break;
                case "Mana":
                    textcolor = Color.Blue;
                    break;
                case "DPS":
                    textcolor = Color.Violet;
                    break;
                case "Utility":
                    textcolor = Color.Orange;
                    break;
                default:
                    textcolor = Color.Black;
                    break;
            }
            slog(textcolor, reason + ": casting {0} on {1} at distance {2} with type {3} at hp {4}", spell, privacyname(target), Round(target.Distance), type, Round(target.HealthPercent));
        }

        private string privacyname(WoWUnit unit)
        {
            string name;
            if (unit == Me)
            {
                return "Myself";
            }
            else if (unit == tank)
            {
                return "Tank " + tank.Name[0] + tank.Name[1] +  "****";
            }
            else if (unit is WoWPlayer)
            {
                name = unit.Class.ToString() + " " + unit.Name[0] +unit.Name[1]+ "****";
                return name;
            }
            else return unit.Name;
        }
        private void formatslog(string type, string reason, string spell)
        {
            formatslog(type, reason, spell, Me);
        }

        private bool Behaviour()
        {
            actualBehaviour = Me.PvPState.ToString();
            if ((actualBehaviour == "None") && lastBehaviour == null)
            {
                usedBehaviour = "Starting";
            }
            else if (!Me.IsInInstance && !Me.IsInParty && !Me.IsInRaid)
            {
                usedBehaviour = "Solo";
            }
            else if ((actualBehaviour == "None") && !Me.IsInInstance)
            {
                usedBehaviour = "Party or Raid";
            }
            else if ((actualBehaviour == "FFAPVP") && Me.IsInInstance)
            {
                usedBehaviour = "Arena";
            }
            else if (!Me.IsInInstance && actualBehaviour == "PVP")
            {
                usedBehaviour = "World PVP";
            }
            else if (Me.IsInInstance && actualBehaviour == "PVP")
            {
                usedBehaviour = "Battleground";
            }
            else if ((Me.IsInInstance) && (Me.IsInParty) && actualBehaviour != "FFAPVP" && actualBehaviour != "PVP")
            {
                usedBehaviour = "Dungeon";
            }
            else if ((Me.IsInInstance) && (Me.IsInRaid) && actualBehaviour != "FFAPVP" && actualBehaviour != "PVP")
            {
                usedBehaviour = "Raid";
            }
            else
            {
                usedBehaviour = "WTF are you doing?";
            }
            if (lastBehaviour == usedBehaviour && usedBehaviour != null)
            {
                return false;
            }
            slog(Color.HotPink, "{0}", usedBehaviour);
            lastBehaviour = usedBehaviour;
            return true;
        }

        public bool InRaid() {
            return Me.RaidMembers.Count > 0; 
        }

        public bool InParty() { 
            return Me.PartyMember1 != null; 
        }

        private bool IsTank(WoWPlayer p)
        {
            if (p != null)
            {
                return Lua.GetReturnValues("return UnitGroupRolesAssigned('" + DeUnicodify(p.Name) + "')").First() == "TANK";
            }
            else return false;
        }

        private WoWPlayer tankfromlua()
        {
            if (InRaid())
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
        
        private WoWPlayer GetTank()
        {
            WoWPlayer luatank = tankfromlua();
            if (unitcheck(RaFHelper.Leader) == 1)
            {
                if (fallbacktank != RaFHelper.Leader)
                {
                    slog(Color.DarkRed, "Selecting the tank {0} from LazyRaider!",privacyname(RaFHelper.Leader));
                }
                fallbacktank = RaFHelper.Leader;
                return RaFHelper.Leader;
            }
            else if (unitcheck(luatank) == 1)
                {
                    slog(Color.DarkRed, "You did not selected anyone as tank, also {0} as the Role of tank, selecting him as tank",privacyname(luatank));
                    RaFHelper.SetLeader(luatank);
                    fallbacktank = luatank;
                    return luatank;
                }
            else if (unitcheck(fallbacktank) == 1)
            {
                slog(Color.DarkRed, "mm.. we are using the fallbacktank {0} not that good.. please report this", privacyname(fallbacktank));
                return fallbacktank;
            }
            else
            {
                if (unitcheck(Me) == 1)
                {
                    if (usedBehaviour == "Raid" || usedBehaviour == "Dungeon" || usedBehaviour=="Party or Raid")
                    {
                        slog(Color.DarkRed, "We are in dungeon or raid but no valid tank is found, i'm tanking and that's not good at all! Is tank dead? if not select a tank from lazyraider or perform a RoleCheck!");
                    }
                    return Me;
                }
                else
                {
                    slog(Color.DarkRed, "Noone is a valid tank, even Myself, CC is in PAUSE");
                    return null;
                }
            }
        }
            
            /*
            if (tank == null)
            {
                if (RaFHelper.Leader != null)
                {
                    tank = RaFHelper.Leader;
                }
                else if (Me != null)
                {
                    tank = Me;
                }
            }*/

            /*
            if (unitcheck(tank) < 0)
            {
                if (unitcheck(RaFHelper.Leader) < 0)
                {
                    if (unitcheck(fallbacktank) < 0)
                    {
                        if (unitcheck(Me) < 0)
                        {
                            slog(Color.DarkRed, "Sorry we tryed everything, no valid tank, no help from LazyRaider, no valid fallback tank, even I'm not a valid tank.. i'm giving up, CC is in Pause");
                            return null;
                        }
                        else
                        {
                            if ((Me.IsInRaid || Me.IsInParty) && usedBehaviour != "Battleground")
                            {
                                slog(Color.DarkRed, "Noone else is a valid tank, selecting me as tank, PLS select a tank from LazyRaider configuration!");
                            }
                            tank = Me;
                        }
                    }
                    else
                    {
                        slog(Color.DarkRed, "Actual tank is not valid! LazyRaider is not helping! using the Fallbacktank {0} PLS select a tank from LazyRaider configuration!", privacyname(fallbacktank));
                        tank = fallbacktank;
                    }
                }
                else
                {
                    slog(Color.DarkRed, "Acual tankk is not valid! using LazyRaider tank as tank from now on! tank is now {0}", privacyname(RaFHelper.Leader));
                    tank = RaFHelper.Leader;
                }
            }
            else if (tank == RaFHelper.Leader || tank == tankfromlua())
            {
                fallbacktank = tank;
                return tank;
            }
            else if (usedBehaviour == "Battleground" && (tank == Me || tank==RaFHelper.Leader || tank==tankfromlua())
            {
                fallbacktank = tank;
                return tank;
            }
            
            if (RaFHelper.Leader != null)
            {
                if (tank == null || RaFHelper.Leader != tank) { slog(Color.Black, "Import the tank from LazyRaider!"); }
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
             * */
        



        private bool CanCast(string spell, WoWUnit target)
        {
            return SpellManager.CanCast(spell, target);
        }

        private bool CanCast(string spell)
        {
            return SpellManager.CanCast(spell);
        }

        private bool Cast(string spell, WoWUnit target, string reason)
        {
            return internal_Cast(spell, target, 80, "Default",reason);
        }
        private bool Cast(string spell, string type,string reason)
        {
            return internal_Cast(spell, Me, 5, type,reason);
        }
        private bool Cast(string spell, WoWUnit target, string type,string reason)
        {
            return internal_Cast(spell, target, 80, type,reason);
        }
        private bool Cast(string spell,string reason)
        {
            return internal_Cast(spell, Me, 5, "Default",reason);
        }

        private bool Cast(string spell, WoWUnit target, int max_distance,string reason)
        {
            return internal_Cast(spell, target, max_distance, "Default",reason);
        }

        private bool Cast(string spell, WoWUnit target, int max_distance, string type,string reason)
        {
            return internal_Cast(spell, target, max_distance, type,reason);
        }

        private bool castaress(string spell, WoWUnit target, int max_distance, string type, string reason)
        {
            if (target != null && target.IsValid && target.Dead && target.InLineOfSight && target.Distance < (double)max_distance && CanCast(spell, target))
            {
                lastCast = target;
                formatslog(type, reason, spell, target);
                return SpellManager.Cast(spell, target);
            }
            else if (target != null && target.IsValid && target.Dead && target.InLineOfSight && target.Distance > (double)max_distance)
            {
                slog(Color.DarkRed, reason + ": but Target {0} at {1} max spell distance {2} Out of range! Move to him?", privacyname(target), Round(target.Distance), max_distance);
                //Blacklist.Add(target, new TimeSpan(0, 0, 5));
                return false;
            }
            else if (target != null && target.IsValid && target.Dead && !target.InLineOfSight)
            {
                slog(Color.DarkRed, reason + ": but Target {0} at {1} max spell distance {2} Out of LoS! Move to him?", privacyname(target), Round(target.Distance), max_distance);
                //Blacklist.Add(target, new TimeSpan(0, 0, 5));
                return false;

            }
            return false;
        }

        private bool internal_Cast(string spell, WoWUnit target, int max_distance, string type, string reason)
        {
            if (target!=null && target.IsValid && !target.Dead && target.InLineOfSight && target.Distance<(double)max_distance && CanCast(spell, target))
            {
                if (target == Me) { 
                    lastCast = null;
                    formatslog(type,reason,spell);
                    return SpellManager.Cast(spell, Me); 
                } 
                else 
                {
                    lastCast = target;
                    formatslog(type, reason, spell,target);
                    return SpellManager.Cast(spell, target); 
                }
            }
            else if (target != null && target.IsValid && !target.Dead && target.InLineOfSight && target.Distance > (double)max_distance)
            {
                slog(Color.DarkRed, reason + ": but Target {0} at {1} max spell distance {2} Out of range! Move to him?", privacyname(target), Round(target.Distance), max_distance);
                //Blacklist.Add(target, new TimeSpan(0, 0, 5));
                return false;
            }
            else if (target != null && target.IsValid && !target.Dead && !target.InLineOfSight)
            {
                slog(Color.DarkRed, reason + ": but Target {0} at {1} max spell distance {2} Out of LoS! Move to him?", privacyname(target), Round(target.Distance), max_distance);
                //Blacklist.Add(target, new TimeSpan(0, 0, 5));
                return false;
            
            }
                return false;
        }

        private int unitcheck(WoWUnit unit)
        {
            if (unit == null)
            {
                return -1;
            }
            else if (!unit.IsValid)
            {
                return -2;
            }
            else if (unit.Dead)
            {
                return -3;
            }
            return 1;
        }

        private int unitcheck(WoWPlayer unit)
        {
            if (unit == null)
            {
                return -1;
            }
            else if (!unit.IsValid)
            {
                return -2;
            }
            else if (unit.Dead)
            {
                return -3;
            }
            else if (unit.IsGhost)
            {
                return -4;
            }
            else if (!unit.IsInMyPartyOrRaid && !unit.IsMe)
            {
                return -5;
            }
            return 1;

        }

        private void checkthetank()
        {
            if (unitcheck(tank) < 0)
            {
                if (unitcheck(RaFHelper.Leader) < 0)
                {
                    if (unitcheck(fallbacktank) < 0)
                    {
                        if (unitcheck(Me) < 0)
                        {
                            slog(Color.DarkRed, "Sorry we tryed everything, no valid tank, no help from LazyRaider, no valid fallback tank, even I'm not a valid tank.. i'm giving up, CC is in Pause");
                            return;
                        }
                        else
                        {
                            if ((Me.IsInRaid || Me.IsInParty) && usedBehaviour != "Battleground")
                            {
                                slog(Color.DarkRed, "Noone else is a valid tank, selecting me as tank, PLS select a tank from LazyRaider configuration!");
                            }
                            tank = Me;
                        }
                    }
                    else
                    {
                        slog(Color.DarkRed, "Actual tank is not valid! LazyRaider is not helping! using the Fallbacktank {0} PLS select a tank from LazyRaider configuration!", privacyname(fallbacktank));
                        tank = fallbacktank;
                    }
                }
                else
                {
                    slog(Color.DarkRed, "Acual tankk is not valid! using LazyRaider tank as tank from now on! tank is now {0}", privacyname(RaFHelper.Leader));
                    tank = RaFHelper.Leader;
                }
            }
            fallbacktank = tank;
        }

        private string DeUnicodify(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            foreach (byte b in bytes)
            {
                if (b != 0)
                    sb.Append("\\" + b);
            }
            return sb.ToString();
        }


        private bool MoveTo(WoWUnit u)
        {
            if (rng == null)
            {
                rng = new Random();
            }
            if (CanCast("Holy Radiance")) {Cast("Holy Radiance","Buff","Moving faaaaaaaaaaster");}
            if (!Me.IsMoving && u != null)//&& u.Distance > 35)
            {
                Navigator.MoveTo(WoWMathHelper.CalculatePointAtSide(u.Location, u.Rotation, rng.Next(10), rng.Next(2) == 1));
                return true;
            }
            else
            {
                return false;
            }
        }

        private double roleweighthealth(WoWUnit p)
        {
            if (p.Guid == tank.Guid)
            {
                return tank.HealthPercent - ((100 - tank.HealthPercent) / 2d);
            }
            else if (p.Guid == Me.Guid)
            {
                return Me.HealthPercent - ((100 - Me.HealthPercent) / 4d);
            }
            else
            {
                return p.HealthPercent;
            }
        }

        private bool BeaconNeedsRefresh(WoWUnit u)
        {
            if (GotBuff("Beacon of Light", u) && u.Distance < 40)
            {
                return u.ActiveAuras["Beacon of Light"].TimeLeft.TotalSeconds <= 5;
            }
            else
            {
                return true;
            }
        }

        private bool SealNeedRefresh()
        {
            if (GotBuff("Seal of Insight", Me))
            {
                return Me.ActiveAuras["Seal of Insight"].TimeLeft.TotalSeconds <= 5;
            }
            else
            {
                return true;
            }
        }


        private bool IsPaladinAura(string aura)
        {
            return Me.HasAura(aura);
            //string s = Lua.GetReturnVal<string>("return UnitAura(\"player\", \"" + aura + "\")", 0);
            //return s != null;
        }

        private bool Resurrecting()
        {
            foreach (WoWPlayer p in GetResurrectTargets())
            {
                slog(Color.DarkRed, "I need to ress {0}", privacyname(p));
                if (Blacklist.Contains(p.Guid, true))
                {
                    continue;
                }
                else
                {
                    if (p.Distance > 40 || !p.InLineOfSight)
                    {
                        //MoveTo(p);
                        return true;
                    }
                    else if (CanCast("Redemption", p) && castaress("Redemption", p, 30, "Buff", "Ressing"))
                    {
                        Blacklist.Add(p, new TimeSpan(0, 0, 15));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private bool ShouldKing()
        {
            if (InParty())
            {
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    if (p.Class == WoWClass.Druid) { return false; }
                    else if ((p.Class == WoWClass.Paladin) && (p.Guid != Me.Guid))
                    {
                        if (GotBuff("Blessing of Kings", p) && !GotBuff("Blessing of Might", p) && p.ActiveAuras["Blessing of Kings"].CreatorGuid != Me.Guid)
                        {
                            return false;
                        }
                    }
                }
                if (GotBuff("Blessing of Kings", Me) && !GotBuff("Blessing of Might", Me) && Me.ActiveAuras["Blessing of Kings"].CreatorGuid != Me.Guid)
                {
                    return false;
                }
                return true;
            }
            else if (InRaid())
            {
                foreach (WoWPlayer p in Me.RaidMembers)
                {
                    if (p.Class == WoWClass.Druid && usedBehaviour!="Battleground") { return false; }
                    else if ((p.Class == WoWClass.Paladin) && (p.Guid != Me.Guid) && usedBehaviour != "Battleground")
                    {
                        if (GotBuff("Blessing of Kings", p) && !GotBuff("Blessing of Might", p) && p.ActiveAuras["Blessing of Kings"].CreatorGuid != Me.Guid)
                        {
                            return false;
                        }
                    }
                }
                if (GotBuff("Blessing of Kings", Me) && !GotBuff("Blessing of Might", Me) && Me.ActiveAuras["Blessing of Kings"].CreatorGuid != Me.Guid)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        private WoWPlayer GetHealTarget()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    //orderby (tank_healer)?(roleweighthealth(unit)):(unit.HealthPercent) ascending
                    orderby unit.HealthPercent ascending
                    where !Blacklist.Contains(unit.Guid, true)  //do not heal a blacklisted target
                    where !unit.Dead
                    where !unit.IsGhost
                    where (unit.IsInMyPartyOrRaid || unit.IsMe)
                    where unit.Distance < 40
                    where unit.HealthPercent < 95
                    where !unit.Auras.ContainsKey("Bloodletting")
                    where !unit.Auras.ContainsKey("Finkle\'s Mixture") || IsTank(unit) || (unit.Auras.ContainsKey("Finkle\'s Mixture") && unit.CurrentHealth < 10000)
                    //FIXME the escape code is needed or not? mah!
                    where !unit.Auras.ContainsKey("Finkle's Mixture") || IsTank(unit) || (unit.Auras.ContainsKey("Finkle's Mixture") && unit.CurrentHealth < 10000)
                    select unit).FirstOrDefault();

        }

        private WoWPlayer PartyCombat()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    orderby unit.Distance descending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.IsInMyPartyOrRaid
                    where unit.Distance < 100
                    where unit.Combat
                    select unit).FirstOrDefault();

        }

        private IEnumerable<WoWPlayer> GetResurrectTargets()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, false)
                    orderby unit.Distance ascending
                    where unit.Dead
                    where unit.IsInMyPartyOrRaid
                    where !unit.IsGhost
                    where unit.Distance < 100
                    select unit);
        }

        private WoWPlayer GetHealTarget(int distance, int maxhealt)
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    //orderby (tank_healer) ? (roleweighthealth(unit)) : (unit.HealthPercent) ascending
                    orderby unit.HealthPercent ascending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.Distance < distance
                    where (unit.IsInMyPartyOrRaid || unit.IsMe)
                    where unit.HealthPercent < maxhealt
                    where !unit.Auras.ContainsKey("Bloodletting")
                    where !unit.Auras.ContainsKey("Finkle\'s Mixture") || IsTank(unit) || (unit.Auras.ContainsKey("Finkle\'s Mixture") && unit.CurrentHealth<10000)
                    //FIXME the escape code is needed or not? mah!
                    where !unit.Auras.ContainsKey("Finkle's Mixture") || IsTank(unit) || (unit.Auras.ContainsKey("Finkle's Mixture") && unit.CurrentHealth < 10000)
                    select unit).FirstOrDefault();

        }


        private WoWPlayer GetCleanseTarget()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    orderby unit.HealthPercent ascending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.Distance < 80
                    where (unit.IsInMyPartyOrRaid || unit.IsMe)
                    where NeedsCleanse(unit)
                    select unit).FirstOrDefault();
        }

        private WoWUnit GiveEnemyPet(int distance)
        {
            WoWUnit enemy = (from unit in ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                             orderby unit.Distance descending
                             where unitcheck(unit)==1
                             where unit.Distance < distance
                             where unit.InLineOfSight
                             where unit.IsPet
                             where unit.IsTargetingMyPartyMember
                             select unit
                            ).FirstOrDefault();
            return enemy;
        }
        /*
        private bool NeedsCleanse(WoWPlayer p)
        {
            int dispelme;
            bool touchme;
            dispelme = 0;
            touchme = true;
            foreach (WoWAura a in p.ActiveAuras.Values)
            {
                if ((a.IsHarmful) && (!p.ActiveAuras.ContainsKey("Blackout")) && (!p.ActiveAuras.ContainsKey("Toxic Torment")) && (!p.ActiveAuras.ContainsKey("Frostburn Formula")) && (!p.ActiveAuras.ContainsKey("Burning Blood")) && (!p.ActiveAuras.ContainsKey("Unstable Affliction")))
                {
                    WoWDispelType t = a.Spell.DispelType;
                    if (t == WoWDispelType.Disease || t == WoWDispelType.Magic || t == WoWDispelType.Poison)
                    {
                        dispelme += 1;
                        // return true;
                    }
                }
                else if ((p.ActiveAuras.ContainsKey("Blackout")) || (p.ActiveAuras.ContainsKey("Toxic Torment")) || (p.ActiveAuras.ContainsKey("Frostburn Formula")) | (p.ActiveAuras.ContainsKey("Burning Blood")) || (p.ActiveAuras.ContainsKey("Unstable Affliction")))
                {
                    touchme = false;
                }
            }
            if ((dispelme > 0) && (touchme)) { return true; } else { return false; }
        }
        */
        private bool NeedsCleanse(WoWPlayer p)
        {
            //int dispelme;
            //dispelme = 0;
            if ((p.ActiveAuras.ContainsKey("Blackout")) || (p.ActiveAuras.ContainsKey("Toxic Torment")) || (p.ActiveAuras.ContainsKey("Frostburn Formula")) || (p.ActiveAuras.ContainsKey("Burning Blood")) || (p.ActiveAuras.ContainsKey("Unstable Affliction")))
            {
                return false;
            }
            foreach (WoWAura a in p.Debuffs.Values)
            {
                    WoWDispelType t = a.Spell.DispelType;
                    if (t == WoWDispelType.Disease || t == WoWDispelType.Magic || t == WoWDispelType.Poison)
                    {
                        //dispelme += 1;
                        return true;
                    }
            }
            //if (dispelme > 0) { return true; } else { return false; }
            return false;
        }
        
        private WoWPlayer NeedHoFNoW()
        {
            return(from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, false)
                  orderby unit.HealthPercent ascending
                  where !unit.Dead
                  where !unit.IsGhost
                  where unit.Distance < 30
                  where (unit.IsInMyPartyOrRaid || unit.IsMe)
                  where NeedsUrgentHoF(unit)
                  select unit).FirstOrDefault();
        }

        private bool NeedsUrgentHoF(WoWPlayer p)
        {
            foreach (WoWAura a in p.Debuffs.Values)
            {
                if (a.Name == "Entangling Roots"||a.Name=="Frost Nova"||a.Name=="Chains of Ice")
                {
                    return true;
                }
            }
            return false;
        }

        private bool GotBuff(string name)
        {
            return GotBuff(name, Me);
        }

        private bool GotBuff(string name, WoWUnit u)
        {
            return u.ActiveAuras.ContainsKey(name);
        }

        private double Round(double d)
        {
            return Math.Round(d, 2);
        }
        #endregion


    }
}

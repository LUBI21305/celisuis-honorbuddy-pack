using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx;
using Styx.Logic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Drawing;

namespace HazzDruid
{
    class class2 : CombatRoutine
    {
        private WoWUnit lastCast;
        private WoWUnit tank;
        private Random rng;

        public override void Pulse()
        {
            if (Me != null && Me.IsValid && Me.IsAlive && Me.IsInInstance)
            {
                tank = GetTank();
                if (tank == null)
                {
                    tank = Me;
                }
                Combat();
            }
        }
		
		public override void Initialize()
        {
        }

        public override bool WantButton
        {
            get
            {
                return true;
            }
        }

        public override void OnButtonPress()
        {
            HazzDruid.HazzDruidConfig f1 = new HazzDruid.HazzDruidConfig();
            f1.ShowDialog();
        }

        public override void Combat()
        {
            if (StyxWoW.GlobalCooldown)
                return;
			else if (Mounted())
                return;
            else if (CancelHeal())
                return;
            else if (Cleansing())
                return;	
            else if (Self())
                return;
			else if (Tranquility())
                return;
			else if (Clearcast())
                return;
            else if (Healing())
                return;
			else if (Lifebloom())
                return;
			else if (Buff())
                return;
			else if (Revive())
                return;
			else if (Rebirth())
                return;

		}
		
		private bool Mounted()
        {
            if(Me.Mounted)
			{
			return true;
			}
			else
            {
            return false;
            }
        }

        private bool CancelHeal()
        {
            if (Me.IsCasting && (lastCast != null && !lastCast.Dead && lastCast.HealthPercent >= 90))
            {
                lastCast = null;
                SpellManager.StopCasting();
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

        private WoWPlayer GetTank()
        {
            foreach (WoWPlayer p in Me.PartyMembers)
            {
                if (IsTank(p))
                {
                    return p;
                }
            }
            return null;
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

        private bool IsTank(WoWPlayer p)
        {
            return Lua.GetReturnValues("return UnitGroupRolesAssigned('" + DeUnicodify(p.Name) + "')").First() == "TANK";
        }

        private bool Self()
        {
			if (Me.ManaPercent <= 50 && CC("Innervate"))
            {
				Logging.Write("Innervate");
                C("Innervate");
                return true;
            }
			else if (Me.HealthPercent <= 40 && CC("Tree of Life") && HazzDruidSettings.Instance.UseTree)
            {
				Logging.Write("Tree of Life");
                C("Tree of Life");
                return true;
            }
            else
            {
                return false;
            }
        }
		
		private bool Tranquility()
        {
            WoWPlayer tar = GetHealTarget();
            if (tar != null)
            {
                if (tar.Distance > 40 || !tar.InLineOfSight)
                {
                    return true;
                }
                else
                {
                    String s = null;
                    bool needCast = false;
                    double hp = tar.HealthPercent;
					
					if (hp < HazzDruidSettings.Instance.TranquilityPercent && !isAuraActive("Tranquility", tar))
                    {
						Logging.Write("Tranquility");
                        s = "Tranquility";
						needCast = true;
                    }
					if (s != null && CC(s, tar))
                    {
                        if (!C(s, tar))
                        {
                        }
                        if (!needCast)
                        {
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        private bool Healing()
        {
            WoWPlayer tar = GetHealTarget();
            if (tar != null)
            {
                if (tar.Distance > 40 || !tar.InLineOfSight)
                {
                    return true;
                }
                else
                {
                    String s = null;
                    bool needCast = false;
                    double hp = tar.HealthPercent;
					
					if (hp < HazzDruidSettings.Instance.NaturesPercent && !isAuraActive("Nature's Swiftness", tar))
                    {
						Logging.Write("Nature's Swiftness");
                        s = "Nature's Swiftness";
						Blacklist.Add(tar, new TimeSpan(0, 3, 5));
                    }
					else if (hp < HazzDruidSettings.Instance.RegrowthPercent && !isAuraActive("Regrowth", tar))
                    {
						Logging.Write("Regrowth");
                        s = "Regrowth";
						needCast = true;
						Blacklist.Add(tar, new TimeSpan(0, 0, 5));
                    }
					else if (hp < HazzDruidSettings.Instance.RejuvenationPercent && !isAuraActive("Rejuvenation", tar))
                    {
						Logging.Write("Rejuvenation");
                        s = "Rejuvenation";
						needCast = true;
						Blacklist.Add(tar, new TimeSpan(0, 0, 8));
                    }
					else if (hp < HazzDruidSettings.Instance.HealingTouchPercent && Me.ActiveAuras.ContainsKey("Harmony"))
                    {
						Logging.Write("Healing Touch");
                        s = "Healing Touch";
						Blacklist.Add(tar, new TimeSpan(0, 0, 6));
                    }
					else if (hp < HazzDruidSettings.Instance.HealingTouchPercent && !isAuraActive("Nourish", tar))
                    {
						Logging.Write("Nourish");
                        s = "Nourish";
						Blacklist.Add(tar, new TimeSpan(0, 1, 0));
                    }
					else if (hp < 85 && !isAuraActive("Wild Growth", tar))
					{
						Logging.Write("Wild Growth");
                        s = "Wild Growth";
						Blacklist.Add(tar, new TimeSpan(0, 0, 6));
                    }
					else if (hp < HazzDruidSettings.Instance.SwiftmendPercent && CC("Swiftmend") && isAuraActive("Rejuvenation", tar))
                    {
						Logging.Write("Swiftmend");
                        s = "Swiftmend";
                    }
					if (s != null && CC(s, tar))
                    {
                        if (!C(s, tar))
                        {
                        }
                        if (!needCast)
                        {
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
		
        private bool Lifebloom()
        {
            WoWPlayer tar = GetHealTarget();
            if (tar != null)
            {
                if (tar.Distance > 40 || !tar.InLineOfSight)
                {
                    return true;
                }
                else
                {
                    String s = null;
                    bool needCast = false;
                    double hp = tar.HealthPercent;
					
					if (tar.Guid == tank.Guid && CC("Lifebloom") && !isAuraActive("Lifebloom", tar)) 
					{
						Logging.Write("Lifebloom");
						s = "Lifebloom";
					}
					else if (tar.Guid == tank.Guid && CC("Lifebloom") && isAuraActive("Lifebloom", tar) && tar.ActiveAuras["Lifebloom"].StackCount < HazzDruidSettings.Instance.LifebloomPercent) 
					{
						Logging.Write("Lifebloom");
						s = "Lifebloom";
					}
					else if (tank.Auras["Lifebloom"].TimeLeft.TotalSeconds < 4) 
					{
						Logging.Write("Lifebloom");
						s = "Lifebloom";	
					}
					if (s != null && CC(s, tar))
                    {
                        if (!C(s, tar))
                        {
                        }
                        if (!needCast)
                        {
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
		
		private bool Clearcast()
        {
            WoWPlayer tar = GetHealTarget();
            if (tar != null)
            {
                if (tar.Distance > 40 || !tar.InLineOfSight)
                {
                    return true;
                }
                else
                {
                    String s = null;
                    bool needCast = false;
                    double hp = tar.HealthPercent;
					
					if (Me.ActiveAuras.ContainsKey("Clearcasting") && hp > 70 && CC("Swiftmend"))
                    {
						Logging.Write("Clearcasting Swiftmend");
                        s = "Swiftmend";
                    }
                    else if (Me.ActiveAuras.ContainsKey("Clearcasting") && hp > 50 && CC("Regrowth"))
                    {
						Logging.Write("Clearcasting Regrowth");
                        s = "Regrowth";
                    }
                    else if (Me.ActiveAuras.ContainsKey("Clearcasting") && hp < 50 && CC("Healing Touch"))
                    {
						Logging.Write("Clearcasting Healing Touch");
                        s = "Healing Touch";
                    }
                    else
                    {
                        if (Me.ActiveAuras.ContainsKey("Clearcasting") && !CC("Swiftmend"))
                        {
                            s = "regrowth";
                        }
                    }
                    if (s != null && CC(s, tar))
                    {
                        if (!C(s, tar))
                        {
                        }
                        if (!needCast)
                        {
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        private bool CC(string spell, WoWUnit target)
        {
            return SpellManager.CanCast(spell, target);
        }

        private bool CC(string spell)
        {
            return SpellManager.CanCast(spell);
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

        private bool C(string spell, WoWUnit target)
        {
            if (SpellManager.Cast(spell, target))
            {
                lastCast = target;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool C(string spell)
        {
            lastCast = null;
            return SpellManager.Cast(spell);
        }


        private bool Cleansing()
        {
			if (HazzDruidSettings.Instance.UseRemoveCurse)
            {
            WoWPlayer p = GetCleanseTarget();
            if (p != null)
            {
                if (p.Distance > 40 || !p.InLineOfSight)
                {
                    return true;
                }
                else if (CC("Remove Corruption", p))
                {
                    C("Remove Corruption", p);
                    return true;
                }
                else
                {
                    Logging.Write("Remove Corruption");
                    return false;
                    }
                }
                else
                {
                    return false;
                }
            } return false;
        }

        private WoWPlayer GetCleanseTarget()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    orderby unit.HealthPercent ascending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.Distance < 80
                    where NeedsCleanse(unit)
                    select unit).FirstOrDefault();
        }

        private bool NeedsCleanse(WoWPlayer p)
        {
            foreach (WoWAura a in p.ActiveAuras.Values)
            {
                if (a.IsHarmful && Me.ManaPercent > 50)
                {
                    WoWDispelType t = a.Spell.DispelType;
                    if (t == WoWDispelType.Curse || t == WoWDispelType.Magic || t == WoWDispelType.Poison)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private WoWPlayer GetHealTarget()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(true, true)
                    orderby unit.HealthPercent ascending
                    where !unit.Dead
                    where !unit.IsGhost
                    where unit.Distance < 80
                    where unit.HealthPercent < 99
                    select unit).FirstOrDefault();

        }

        private IEnumerable<WoWPlayer> GetResurrectTargets()
        {
            return (from unit in ObjectManager.GetObjectsOfType<WoWPlayer>(false, false)
                    orderby unit.Distance ascending
                    where unit.Dead
                    where unit.IsInMyPartyOrRaid
                    where !unit.IsGhost
                    where unit.Distance < 100
                    select unit);
        }
		
		private bool Rebirth()
        {
            foreach (WoWPlayer p in GetResurrectTargets())
            {
                if (Blacklist.Contains(p.Guid, true))
                {
                    continue;
                }
                else
                {
                    if (p.Distance > 40 || !p.InLineOfSight)
                    {
                        return true;
                    }
					else if (CC("Rebirth", p) && HazzDruidSettings.Instance.UseRebirth)
                    {
						C("Rebirth", p);
                        Logging.Write("Rebirth" + p);
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

        private bool Revive()
        {
            foreach (WoWPlayer p in GetResurrectTargets())
            {
                if (Blacklist.Contains(p.Guid, true))
                {
                    continue;
                }
                else
                {
                    if (p.Distance > 40 || !p.InLineOfSight)
                    {
                        return true;
                    }
					else if (CC("Revive", p) && HazzDruidSettings.Instance.UseRevive)
                    {
						C("Revive", p);
                        Logging.Write("Revive " + p);
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

        private bool Buff()
        {
            if (!isAuraActive("Mark of the Wild"))
            {
				Logging.Write("Mark of the Wild");
                C("Mark of the Wild");
                return true;
            }
            foreach (WoWPlayer p in Me.PartyMembers)
            {
                if (p.Distance > 40 || p.Dead || p.IsGhost)
                    continue;
                else if (!isAuraActive("Blessing of Kings", p) && !isAuraActive("Mark of the Wild", p))
                {
					Logging.Write("Mark of the Wild");
                    C("Mark of the Wild", p);
                    return true;
                }
            }
            return false;
        }

        private bool isAuraActive(string name)
        {
            return isAuraActive(name, Me);
        }

        private bool isAuraActive(string name, WoWUnit u)
        {
            return u.ActiveAuras.ContainsKey(name);
        }

        public override sealed string Name { get { return "HazzDruid EliT3 2.3"; } }

        public override WoWClass Class { get { return WoWClass.Druid; } }

        private static LocalPlayer Me { get { return ObjectManager.Me; } }

        public override bool NeedRest
        {
            get
            {
                if (Me.ManaPercent < HazzDruidSettings.Instance.ManaPercent &&
                    !Me.Auras.ContainsKey("Drink"))
                {
                    Logging.Write("Drinking");
                    return true;
                }
                if (Me.HealthPercent < HazzDruidSettings.Instance.HealthPercent)
                {
                    Logging.Write("Eating");
                    return true;
                }

                return false;
            }
        }
        public override void Rest()
        {
            if (Me.ManaPercent < 99)
            {
                Styx.Logic.Common.Rest.Feed();
            }
            if (Me.HealthPercent < 99)
            {
                Styx.Logic.Common.Rest.Feed();
            }

        }
		
        public override bool NeedPullBuffs { get { Pulse(); return false; } }

        public override bool NeedCombatBuffs { get { Pulse(); return false; } }

        public override bool NeedPreCombatBuffs { get { Pulse(); return false; } }

    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.POI;
using TreeSharp;
namespace DaBloodDK
{
    class DeathKnight : CombatRoutine
    {


        public override sealed string Name { get { return "DaBloodDK - By Lbniese" + ver; } }
        public override WoWClass Class { get { return WoWClass.DeathKnight; } }
        private static LocalPlayer Me { get { return ObjectManager.Me; } }


        private Version ver = new Version(0, 0, 2);

        public override bool NeedRest
        {
            get
            {
                return false;
            }
        }



        public override void Rest()
        {

        }

        public override bool NeedPullBuffs { get { return false; } }
        public override bool NeedCombatBuffs { get { return false; } }

        public struct Act
        {
            public string Spell;
            public float Trigger;
            public Act(String s, float t)
            {
                Spell = s;
                Trigger = t;
            }
        }
        public struct Trinket
        {
            public string TrinketName;
            public string StackName;
            public int StackNumber;
            public float Trigger;
            public Trinket(String s, String sn, int snn, float t)
            {
                TrinketName = s;
                StackName = sn;
                StackNumber = snn;
                Trigger = t;
            }
        }

        WoWUnit Target;

        List<String> Rotation = new List<String> { "Death Strike", "Heart Strike", "Rune Strike"};
        List<String> RangedRotation = new List<String> { "Death Grip", "Death Coil" };
        List<String> Interrupts = new List<String> { "Mind Freeze", "Strangulate", "Arcane Torrent" };
        List<String> CD = new List<String> { "Outbreak","Dancing Rune Weapon"};
        List<Act> HealRotation = new List<Act> { new Act("Anti-Magic Shell", 60.0f), new Act("Rune Tap", 35.0f), new Act("Vampiric Blood", 60.0f) };
        List<Trinket> Trinkets = new List<Trinket> { new Trinket("Vicious Gladiator's Badge of Victory", null, 0, 100.0f), new Trinket("Essence of the Eternal Flame", null, 0, 100.0f), new Trinket("Apparatus of Khaz'goroth", "Titanic Power", 5, 60.0f), new Trinket("Fury of Angerforge", "Raw Fury", 5, 60.0f) };
        float LichbornHealPercent = 50.0f;

        public override void Combat()
        {
            Target = Me.CurrentTarget;
            if (Target == null || !Target.Attackable)
            {
                return;
            }

            //Face pet-owner
            else if (Target != null && Target.IsPet)
            {
                WoWUnit Owner = Target.CreatedByUnit;
                if (Owner != null && Owner.IsValid)
                {
                    Blacklist.Add(Target, new TimeSpan(0, 0, 5));
                    Logging.Write("Changing targets to pet owner");
                    Target = Owner;
                    TargetUnit(Target);
                }
            }
            //Face the target
            Face(Target);
			
			
			
            //Always try and move ontop of the enemy target
            if (!Me.IsCasting)
            {
                Move(Target.Location);
            }


            if (Target.Distance < 2 && Styx.BotManager.Current.Name != "LazyRaider")
                Navigator.PlayerMover.MoveStop();


            if (Me.IsCasting)
            {
            }
            else if ((Target.Distance > 30 || !Target.IsAlive) && Me.Combat && Styx.BotManager.Current.Name != "LazyRaider")
            {
                Logging.Write(Target.Name + " is currently " + Target.Distance.ToString() + " dropping target");
                Me.ClearTarget();
                SeekTarget();
            }
            else if (isAuraActive("Freezing Fog") && CanCast("Howling Blast"))
            {
                Cast("Howling Blast");
            }
            else if ((Target.Distance >= 5d && Target.Distance < 20d))
            {
                if (Target.MovementInfo.CurrentSpeed > 7.0f && CanCast("Chains of Ice"))
                {
                    Cast("Chains of Ice");
                }
                else
                {
                    foreach (String Ability in RangedRotation)
                    {
                        cctc(Target, Ability);
                    }
                }
                Move(Target.Location);
            }

            else if (Target.Distance <= 6d || MeleeLatency())
            {

                foreach (String Ability in CD)
                {
                    cctc(Ability);
                }

                if (Target.IsCasting)
                {
                    foreach (String Ability in Interrupts)
                    {
                        if (cctc(Ability))
                        {
                            Thread.Sleep(50);
                            break;
                        }
                    }
                }
                else if ((Me.HealthPercent < 30.0f || isAuraActive("Dark Succor")) && CanCast("Death Strike") && Me.HealthPercent < 80.0f)
                {
                    Cast("Death Strike");
                }
                else
                {
                    if (isAuraActive("Crimson Scourge") && (CanCast("Blood Boil") || CanCast("Rune Tap")))
                    {
                  
                        if (CanCast("Rune Tap"))
                        {
                            Cast("Rune Tap");
                        }
                        else
                        {
                            Cast("Blood Boil");
                        }
                    }

                    foreach (String Ability in Rotation)
                    {
                        if (cctc(Ability))
                        {
                            break;
                        }
                    }
                }
                Usetrinkets();
            }


        }

        public void Usetrinkets()
        {
            foreach (Trinket t in Trinkets)
            {

                if (StyxWoW.Me.Inventory.Equipped.Trinket1 != null && StyxWoW.Me.Inventory.Equipped.Trinket1.Name.Contains(t.TrinketName) && StyxWoW.Me.Inventory.Equipped.Trinket1.Cooldown <= 0)
                {
                    if ((t.StackName != null) && (HasAuraStacks(t.StackName, t.StackNumber, Me)) && Target.HealthPercent < t.Trigger)
                    {
                        Logging.Write("Trinket one");
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                    }
                    else if (t.StackName == null)
                    {
                        Logging.Write("Trinket one");
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                    }
                }
                else if (StyxWoW.Me.Inventory.Equipped.Trinket2 != null && StyxWoW.Me.Inventory.Equipped.Trinket2.Name == t.TrinketName && StyxWoW.Me.Inventory.Equipped.Trinket2.Cooldown <= 0)
                {
                    if ((t.StackName != null) && (HasAuraStacks(t.StackName, t.StackNumber, Me)) && Target.HealthPercent < t.Trigger)
                    {
                        Logging.Write("Trinket two");
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                    }
                    else if (t.StackName == null)
                    {
                        Logging.Write("Trinket two");
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                    }
                }

            }
        }
        System.Timers.Timer Heartbeat;
        /// <summary>
        /// Gonna try something interesting, using a timer rather then pulse to check for target usage, as it seems sometimes pulse stops getting called
        /// </summary>
        public override void Initialize()
        {
            Logging.Write("Init gets called");
            Heartbeat = new System.Timers.Timer(200);
            Heartbeat.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            Heartbeat.Enabled = true;
            //base.Initialize();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Me.Mounted && !Me.GotTarget && !Me.Dead)
            {
                SeekTarget();
            }
        }


        public bool HasAuraStacks(String aura, int stacks, WoWUnit unit)
        {
            if (unit.ActiveAuras.ContainsKey(aura))
            {
                return unit.ActiveAuras[aura].StackCount >= stacks;
            }
            return false;
        }

        void Cast(string Name)
        {
            Logging.Write(Name);
            SpellManager.Cast(Name);
        }

        bool CanCast(string Name)
        {
            return SpellManager.CanCast(Name);
        }
        bool CanCast(WoWUnit Target, string Name)
        {
            return SpellManager.CanCast(Name, Target);
        }

        bool cctc(string Name)
        {
            if (SpellManager.CanCast(Name))
            {
                Logging.Write(Name);
                SpellManager.Cast(Name);
                return true;
            }
            else
            {
                return false;
            }
        }

        bool cctc(WoWUnit Who, String Name)
        {
            if (SpellManager.CanCast(Name))
            {
                Logging.Write(Name + "@" + Who);
                SpellManager.Cast(Name, Who);
                return true;
            }
            else
            {
                return false;
            }
        }

        bool MeleeLatency()
        {
            return false;
        }

        protected void TargetUnit(WoWUnit unit)
        {
            Logging.Write("Targeting " + unit.Name);
            BotPoi.Current = new BotPoi(unit, PoiType.Kill);
            Tar(unit);
        }

        public override void Pull()
        {


            if (!Me.GotTarget || !Me.CurrentTarget.IsAlive)
            {
                SeekTarget();
            }
            else
            {
                foreach (String Ability in RangedRotation)
                {
                    cctc(Target, Ability);
                }
                foreach (String Ability in Rotation)
                {
                    if (cctc(Ability))
                    {
                        break;
                    }
                }

                Me.ToggleAttack();
                Move(Me.CurrentTarget.Location);
            }

        }

        private void SeekTarget()
        {
            WoWPlayer unit = ObjectManager.GetObjectsOfType<WoWPlayer>(false, false).
                Where(p => p.IsHostile && !p.IsTotem && !p.IsPet && !p.Dead && p.DistanceSqr <= (10 * 10)).
                OrderBy(u => u.HealthPercent).FirstOrDefault();

            if (unit == null)
            {
                unit = ObjectManager.GetObjectsOfType<WoWPlayer>(false, false).Where(
                                       p => p.IsHostile && !p.IsTotem && !p.IsPet && !p.Dead && p.DistanceSqr <= (35 * 35)).OrderBy(
                                           u => u.DistanceSqr).FirstOrDefault();
            }
            if (unit != null)
            {
                TargetUnit(unit);
                Move(unit.Location);
            }


        }

        private void Move(WoWPoint loc)
        {
            if (Styx.BotManager.Current.Name != "LazyRaider")
            {
                Navigator.MoveTo(loc);
            }
        }

        private void Tar(WoWUnit tar)
        {
            if (Styx.BotManager.Current.Name != "LazyRaider")
            {
                tar.Target();
                WoWMovement.ConstantFace(tar.Guid);
            }
        }

        private void Face(WoWUnit tar)
        {
            if (Styx.BotManager.Current.Name != "LazyRaider")
            {
                tar.Face();
            }
        }



        public override void PreCombatBuff()
        {
            cctc(Me, Buffcheck());
        }


        public override bool NeedPreCombatBuffs
        {
            get
            {
                return (Buffcheck() != null);
            }
        }

        public override bool NeedHeal
        {
            get
            {
                return HealCheck();
            }
        }

        public override void Heal()
        {

            foreach (Act action in HealRotation)
            {
                if ((Me.HealthPercent <= action.Trigger) && CanCast(action.Spell))
                {
                    Cast(action.Spell);
                }
            }
        }



        /// <summary>
        /// Return the name of the buff we need to use
        /// </summary>
        /// <returns></returns>
        private string Buffcheck()
        {
            if (!isAuraActive("Battle Shout") && !isAuraActive("Horn of Winter") && !isAuraActive("Strength Of Earth Totem") && !isAuraActive("Roar of Courage"))
            {
                return "Horn of Winter";
            }


            return null;
        }



        /// <summary>
        /// Runs through all the checks in the healrotation
        /// </summary>
        /// <returns></returns>
        private bool HealCheck()
        {
            foreach (Act action in HealRotation)
            {
                if ((Me.HealthPercent <= action.Trigger) && CanCast(action.Spell))
                {
                    return true;
                }
            }
            return false;
        }
        private bool isAuraActive(string name)
        {
            //Me.Auras.con
            return Me.ActiveAuras.ContainsKey(name);
        }



    }
}
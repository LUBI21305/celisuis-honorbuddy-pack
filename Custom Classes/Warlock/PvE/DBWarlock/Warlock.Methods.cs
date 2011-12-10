using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
namespace DBWarlock
{
    public partial class Warlock
    {
        //        private delegate bool Condition();

        #region Settings
        private string _logspam;

        #endregion


        #region utilities


        private readonly Queue<WoWSpell> _rotation = new Queue<WoWSpell>();


        private void Enqueue(WoWUnit unit, Dictionary<SpellPriority, CastRequirements> spellrotation, int maxQueueSize)
        {
            if (_rotation != null)
            {
                _rotation.Clear();
                var items = from k in spellrotation.Keys
                            orderby k.Priority descending
                            select k;

                foreach (var s in items)
                {
                    if (unit != null)
                    {
                        if (spellrotation[s].Invoke(unit))
                        {
                            if (SpellManager.Spells.ContainsKey(s.Name))
                            {
                                _rotation.Enqueue(SpellManager.Spells[s.Name]);
                            }

                            if (_rotation.Count == maxQueueSize)
                                break;
                        }
                    }
                    else
                    {
                        if (spellrotation[s].Invoke(null))
                        {
                            if (SpellManager.Spells.ContainsKey(s.Name))
                            {
                                _rotation.Enqueue(SpellManager.Spells[s.Name]);
                            }

                            if (_rotation.Count == maxQueueSize)
                                break;
                        }
                    }
                }
            }
        }


        private void Enqueue(Dictionary<SpellPriority, CastRequirements> spellrotation, int maxQueueSize)
        {
            Enqueue(Me.CurrentTarget, spellrotation, maxQueueSize);
            /*            if (_rotation != null)
                        {
                            _rotation.Clear();
                            var items = from k in spellrotation.Keys
                                        orderby k.Priority descending
                                        select k;

                            foreach (var s in items)
                            {
                                if (Me.CurrentTarget != null)
                                {
                                    if (spellrotation[s].Invoke(Me.CurrentTarget))
                                    {
                                        if (SpellManager.Spells.ContainsKey(s.Name))
                                        {
                                            _rotation.Enqueue(SpellManager.Spells[s.Name]);
                                        }

                                        if (_rotation.Count == maxQueueSize)
                                            break;
                                    }
                                }
                                else
                                {
                                    if (spellrotation[s].Invoke(null))
                                    {
                                        if (SpellManager.Spells.ContainsKey(s.Name))
                                        {
                                            _rotation.Enqueue(SpellManager.Spells[s.Name]);
                                        }

                                        if (_rotation.Count == maxQueueSize)
                                            break;
                                    }
                                }
                            }
                        }*/
        }

        private void SelfCast()
        {
            if (_rotation != null)
            {
                while (_rotation.Count > 0)
                {
                    if (!string.IsNullOrEmpty(_rotation.Peek().Name))
                    {
                        //slog("Casting#{0}", _rotation.Peek().Name);
                        SafeCast(_rotation.Dequeue().Name, false);
                    }
                    while (Me.IsCasting)
                        Thread.Sleep(_settings.myLag);
                }
            }
        }

        private void Cast()
        {
            if (Me.CurrentTarget != null)
            {
                if (_rotation != null)
                {
                    while (_rotation.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(_rotation.Peek().Name))
                        {
                            while (StyxWoW.GlobalCooldown)
                                Thread.Sleep(50);
                            //slog("Casting#{0}", _rotation.Peek().Name);
                            if (_rotation.Peek().Name == "Immolation Aura")
                                SafeCast(_rotation.Dequeue().Name, false);
                            else
                                SafeCast(_rotation.Dequeue().Name);
                            while (Me.IsCasting)
                                Thread.Sleep(_settings.myLag);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Returns true if the unit has a debuff
        /// </summary>
        /// <param name="name">string - name of the debuff</param>
        /// <param name="unit">WoWUnit - name of the debuff</param>
        /// <returns>boolean indicating if debugg is present or not</returns>
        private static bool GotDebuff(string name, WoWUnit unit)
        {
            if (unit == null)
            { return false; }

            return (unit.Auras.ContainsKey(name));
        }
        /// <summary>
        /// Returns true if my current target has a debuff
        /// </summary>
        /// <param name="name">string - name of the debuff</param>
        /// <returns>boolean indicating if debugg is present or not</returns>
        private static bool GotDebuff(string name)
        {
            if (Me.CurrentTarget == null)
            { return false; }
            return GotDebuff(name, Me.CurrentTarget);
        }

        /// <summary>
        /// Returns true if i got this buff
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool GotBuff(string name)
        {
            return Me != null ? Me.Auras.ContainsKey(name) : false;
        }


        private bool isItemInCooldown(WoWItem item)
        {
            if (Equals(null, item))
                return true;

            return (item.Cooldown > 0);
        }

        /*  private static bool checkLastCast(string spellName)
          {
              if (!lastCast.IsRunning)
              {
                  lastCast.Start();
                  lastSpell = "None";
                  return true;
              }
              else
                  if (lastCast.ElapsedMilliseconds < 2000)
                      return !lastSpell.Equals(spellName);
                  else
                      return true;
          }*/

        private bool SafeCast(string spellName)
        {
            return SafeCast(spellName, true);
        }

        private bool SafeCast(string spellName, bool needTartget)
        {
            while (StyxWoW.GlobalCooldown)
                Thread.Sleep(50);



            try
            {
                if (needTartget && !Me.GotTarget)
                {
                    slog("Can't cast {0} right now!", spellName);
                    return false;
                }
                if (CanCast(spellName))
                {
                    if (needTartget)
                    {
                        if (!Me.IsFacing(Me.CurrentTarget))
                        {
                            Me.Face();
                        }
                        if ((Me.CurrentTarget.Distance <= SpellManager.Spells[spellName].MaxRange))
                        {
                            // gambiarra
                            if (spellName == "Immolate" || spellName == "Unstable Affliction")
                            {
                                _immolateCoolDown.Stop();
                                _immolateCoolDown.Reset();
                                _immolateCoolDown.Start();
                            }
                            if (spellName == "Soul Fire" && Me.CurrentSoulShards > 1 && CanCast("Soulburn"))
                            {
                                slog(Color.RoyalBlue, "Boosting spelcast with Soulburn!", true);
                                SpellManager.Cast("Soulburn");
                                Thread.Sleep(_settings.myLag);
                            }


                            SpellManager.Cast(spellName);
                            slog(spellName);

                            return true;
                        }
                        else
                        {
                            slog("{0} is out of range for casting {1}", Me.CurrentTarget.Name, spellName);
                        }
                    }
                    else
                    {
                        SpellManager.Cast(spellName);
                        //slog(spellName);
                        return true;
                    }
                }



                //slog("{0} Could not be casted", spellName);

            }
            catch (Exception ex)
            {
                if (_settings.showDebug)
                {
                    slog("************************");
                    slog("Something nasty happened!");
                    slog("Inform DarkBen with the execution Log");
                    slog(ex.Source);
                    slog(ex.Message);
                    slog(ex.StackTrace);
                    slog(ex.TargetSite.Name);
                    slog(spellName);
                    slog(needTartget.ToString());
                    slog("************************");
                }
            }


            return false;
        }


        private WoWItem HaveItemCheck(List<uint> listId)
        {
            foreach (WoWItem item in ObjectManager.GetObjectsOfType<WoWItem>(false))
            {
                if (listId.Contains(item.Entry))
                {
                    return item;
                }
            }
            return null;
        }

        private List<WoWUnit> getAdds()
        {
            List<WoWUnit> enemyMobList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(
                unit => (unit.Guid != Me.Guid &&
                    unit.IsTargetingMeOrPet &&
                    !unit.IsFriendly && unit.Distance <= 30 &&
                    unit.InLineOfSight &&
                    !Styx.Logic.Blacklist.Contains(unit.Guid)));


            if (enemyMobList.Count > 1)
            {
                slog("Warning: there are " + enemyMobList.Count.ToString() + " attackers.");
            }
            return enemyMobList;
        }



        private void DotEmUp()
        {
            if (!_settings.dotAdds && !GotBuff("Metamorphosis"))
                return;
            List<WoWUnit> targets = getAdds();
            if (targets.Count < 2)
                return;
            List<WoWUnit> DotList = targets.FindAll(unit => ((_settings.useImmolate && !GotDebuff("Immolate", unit) && CanCast("Immolate")) || (_settings.useCorruption && !GotDebuff("Corruption", unit) && CanCast("Corruption"))) && unit.HealthPercent > 30);
            _rotation.Clear();
            if (DotList.Count > 1)
            {
                slog("Dotting Adds");
                foreach (WoWUnit E in DotList)
                {
                    E.Target();
                    Thread.Sleep(_settings.myLag);
                    if (_settings.useImmolate && !GotDebuff("Immolate", E) && CanCast("Immolate"))
                        SafeCast("Immolate");
                    if (_settings.useCorruption && !GotDebuff("Corruption", E) && CanCast("Corruption"))
                        SafeCast("Corruption");
                }
            }
        }


        private void SmartTarget()
        {
            try
            {
                List<WoWUnit> targets = getAdds();
                List<WoWUnit> totems = getTotems();

                //            List<WoWUnit> targetsOnMe = getAddsTargetingMe();
                if (targets.Count < 1)
                    return;
                List<WoWUnit> targetsOnMe = targets.FindAll(unit => unit != null && unit.CurrentTarget.IsMe);
                if (totems.Count > 0)
                {
                    slog("We found {0} totem(s)", totems.Count);
                    totems[0].Target();
                    return;
                }


                if (targetsOnMe.Count > 0)
                {

                    if (targetsOnMe.Count > 1)
                    {
                        // slog("Warning: there are {0} attackers targeting me.", targetsOnMe.Count);
                        WoWUnit bestTarget = targetsOnMe[0];

                        foreach (WoWUnit tgt in targetsOnMe)
                        {
                            try
                            {
                                if (tgt.CurrentHealth < bestTarget.CurrentHealth && tgt.Guid != bestTarget.Guid)
                                {
                                    bestTarget = tgt;
                                }

                            }
                            catch (Exception ex)
                            {
                                slog("Targeting error.");
                                slog(ex.Message);
                            }
                        }
                        bestTarget.Target();
                        WoWMovement.Face();
                        return;
                    }
                    //slog("Warning: {0} is targeting me.", targetsOnMe[0].Name);
                    targetsOnMe[0].Target();
                    WoWMovement.Face();
                    return;

                }

                if (targets.Count == 1)
                {
                    targets[0].Target();
                    WoWMovement.Face();
                    return;
                }

                if (targets.Count > 1)
                {
                    WoWUnit bestTarget = targets[0];

                    foreach (WoWUnit tgt in targets)
                    {
                        try
                        {
                            if (tgt.CurrentHealth < bestTarget.CurrentHealth && tgt.Guid != bestTarget.Guid)
                            {
                                bestTarget = tgt;
                            }

                        }
                        catch (Exception ex)
                        {
                            slog("Targeting error.");
                            slog(ex.Message);
                        }
                    }
                    bestTarget.Target();
                    WoWMovement.Face();
                    return;
                }
            }
            catch (Exception e)
            {
                if (_settings.showDebug)
                {
                    slog(e.Message);
                    slog(e.Source);
                    slog(e.StackTrace);
                    slog(e.TargetSite.ToString());
                }

            }
        }

        private double targetDistance
        {
            get
            {
                return Me.GotTarget ? Me.CurrentTarget.Distance : uint.MaxValue - 1;
            }
        }

        private bool isTotem
        {
            get
            {
                if (Me.GotTarget)
                    return Me.CurrentTarget.CreatureType == WoWCreatureType.Totem;
                return false;
            }
        }

        private List<WoWUnit> getTotems()
        {
            return ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit => unit.CreatureType == WoWCreatureType.Totem && unit.IsHostile && unit.Distance < Styx.Logic.Targeting.PullDistance);
        }



        #endregion



        #region movement logic
        WoWPoint attackPointBuffer;
        WoWPoint attackPoint
        {
            get
            {
                if (Me.GotTarget)
                {
                    if (!movementTimer.IsRunning ||
                        movementTimer.ElapsedMilliseconds > 2500)
                    {
                        movementTimer.Reset();
                        movementTimer.Start();
                        attackPointBuffer = WoWMovement.CalculatePointFrom(Me.CurrentTarget.Location, (float)shadowRange - 2);
                        return attackPointBuffer;
                    }
                    else
                    {
                        return attackPointBuffer;
                    }
                }
                else
                {
                    WoWPoint noSpot = new WoWPoint();
                    return noSpot;
                }
            }
        }


        private Stopwatch movementTimer = new Stopwatch();


        #endregion


        #region combat checks and utils

        private static bool PetCanCast(string spellName)
        {
            WoWPetSpell petAction = Me.PetSpells.FirstOrDefault(p => p.ToString() == spellName);
            if (petAction == null || petAction.Spell == null)
            {
                return false;
            }

            return !petAction.Spell.Cooldown;
        }

        public static void PetCast(string spellName)
        {
            var spell = Me.PetSpells.FirstOrDefault(p => p.ToString() == spellName);
            if (spell == null)
                return;

            //slog(string.Format("[Pet] Casting {0}", spellName));

            Lua.DoString("CastPetAction({0})", spell.ActionBarIndex + 1);
        }

        private static bool CanCast(string spellName)
        {
            if (SpellManager.Spells.ContainsKey(spellName))
            {
                // Gambiarra
                if (spellName == "Immolate" || spellName == "Unstable Affliction")
                {
                    if (!_immolateCoolDown.IsRunning)
                        _immolateCoolDown.Start();
                    else if (_immolateCoolDown.ElapsedMilliseconds < 5000)
                        return false;
                }

                return (SpellManager.CanCast(spellName) && SpellManager.Spells[spellName].PowerCost < Me.CurrentMana);
            }

            return false;
        }


        private void curseMyTarget()
        {
            if (!Me.GotTarget || !_settings.useCurse)
                return;
            switch (_settings.curseName)
            {
                case "Auto":
                    if (!Me.CurrentTarget.Auras.ContainsKey("Bane of Agony"))
                        Curse("Bane of Agony");
                    break;
                default:
                    if (!Me.CurrentTarget.Auras.ContainsKey(_settings.curseName))
                        Curse(_settings.curseName);
                    break;
            }
        }



        private bool combatChecks
        {
            get
            {
                if (_settings.showDebug)
                {
                    slog("Combat Check!");
                }

                if (Me.GotTarget && !Me.IsCasting)
                {
                    if (!Me.CurrentTarget.IsAlive)
                    {
                        Me.ClearTarget();
                        return false;
                    }
                    WoWMovement.MoveStop();
                    Me.CurrentTarget.Face();
                    if (_settings.showDebug)
                    {
                        slog("Combat Check Ok!");
                    }
                    return true;
                }
                if (_settings.showDebug)
                {
                    slog("Combat Check False!");
                }
                return false;

            }
        }

        private bool NeedDeathCoil
        {
            get
            {
                return (Me.GotTarget &&
                Me.HealthPercent < _settings.dcMaxHealth &&
                _settings.useDeathCoil &&
                CanCast("Death Coil"));
            }
        }

        private bool NeedHealthStone
        {
            get
            {
                return (Me.HealthPercent < _settings.healthStoneHealthPercent &&
                _settings.useHealthStone && !isItemInCooldown(myHealthstone));
            }
        }

        private bool NeedEMHealth
        {
            get
            {
                return (Me.HealthPercent < _settings.restHealthPercent);
            }

        }

        private bool NeedEMMana
        {
            get
            {
                return (Me.ManaPercent < _settings.restManaPercent);
            }

        }

        private bool NeedDrainSoul
        {
            get
            {
                return (Me.GotTarget && _settings.useDrainSoul && Me.CurrentSoulShards < Me.MaxSoulShards && Me.CurrentTarget.HealthPercent < 30 && CanCast("Drain Soul"));
            }
        }

        private bool NeedLifeTap
        {
            get
            {
                return (Me.ManaPercent < _settings.lfTapMaxMana && Me.HealthPercent > _settings.lfTapMinHealth && _settings.useLifeTap && CanCast("Life Tap"));
            }
        }

        private bool NeedDrainLife
        {
            get
            {
                return (_settings.useDrainLife && Me.GotTarget && Me.HealthPercent < _settings.drainLifeMaxHealth && CanCast("Drain Life"));
            }
        }
        private bool NeedDrainMana
        {
            get
            {
                return (_settings.useDrainMana && Me.ManaPercent < 50 && Me.GotTarget && CanCast("Drain Mana"));
            }
        }


        private void CombatHealthManager()
        {

            if (NeedEMHealth)
            {
                slog("Combat Emergency");
                if (!UseHealthStone)
                {
                    Healing.UseHealthPotion();
                }
                else
                {
                    return;
                }
            }

            if (NeedEMMana)
            {
                Healing.UseManaPotion();
            }

            while (Me.IsCasting)
            {
                Thread.Sleep(50);
            }


            if (NeedDeathCoil)
            {
                SafeCast("Death Coil");
                return;
            }


            if (NeedHealthStone)
            {
                if (UseHealthStone)
                {
                    slog("Health stone used!");
                    return;
                }
            }




            if (NeedDrainSoul)
            {
                SafeCast("Drain Soul");
                Thread.Sleep(2000);

                while (Me.GotTarget && GotDebuff("Drain Soul"))
                {
                    slog("{0}, Your Soul is Mine!", Me.CurrentTarget.Name);
                    Thread.Sleep(50);
                }
                return;
            }

            if (NeedLifeTap)
            {
                SafeCast("Life Tap", false);
                Thread.Sleep(_settings.myLag);
                while (Me.IsCasting)
                {
                    Thread.Sleep(50);
                }
                //return;
            }


            if (NeedDrainLife)
            {
                SafeCast("Drain Life");
                Thread.Sleep(2000);

                while (Me.GotTarget && GotDebuff("Drain Life") && Me.HealthPercent < _settings.drainLifeStopHealth)
                {
                    //slog("ChaneledCasting {0} - isCasting {1} - TargetBuff {2}", Me.ChanneledCasting, Me.IsCasting, Me.CurrentTarget.Auras.ContainsKey("Drain Life"));
                    slog("{0}, Your Health is Mine!", Me.CurrentTarget.Name);
                    Thread.Sleep(50);
                }
                return;
            }
            if (NeedDrainMana)
            {
                SafeCast("Drain Mana");
                Thread.Sleep(2000);
                while (Me.GotTarget && GotDebuff("Drain Mana") && Me.ManaPercent < 95)
                {
                    slog("{0}, Your Mana is Mine!", Me.CurrentTarget.Name);
                    Thread.Sleep(0);
                }
                return;
            }


        }


        private void KillTotem()
        {
            while (Me.GotTarget && isTotem)
            {
                if (!Me.IsAutoAttacking)
                    Lua.DoString("StartAttack()");

                Shoot();

                if (Me.GotAlivePet)
                    Lua.DoString("PetAttack()");

                Thread.Sleep(_settings.myLag);

                while (targetDistance > 5)
                {
                    Navigator.MoveTo(Me.CurrentTarget.Location);
                    //WoWMovement.ClickToMove(Me.CurrentTarget.Location, 1);
                    WoWMovement.Face();
                    Thread.Sleep(_settings.myLag);
                }



            }

        }



        private readonly Dictionary<SpellPriority, CastRequirements>[] _spellRotation = 
        {new Dictionary<SpellPriority, CastRequirements> // Warlock <10
            {
                {new SpellPriority("Immolate", 200), unit => _settings.useImmolate && !GotDebuff("Immolate",unit) && CanCast("Immolate") && (unit.HealthPercent > 30)},
                {new SpellPriority("Corruption", 190), unit => _settings.useCorruption && !GotDebuff("Corruption",unit) && CanCast("Corruption")},
                {new SpellPriority("Bane of Agony", 170), unit => _settings.useCurse && (_settings.curseName == "Auto") && !GotDebuff("Bane of Agony",unit) && CanCast("Bane of Agony")},
                {new SpellPriority("Shadow Bolt", 90), unit => _settings.useShadowBolt && CanCast("Shadow Bolt")&& (unit.HealthPercent > 30)}
            },
        new Dictionary<SpellPriority, CastRequirements> // Affliction
            {
                {new SpellPriority("Fel Flame", 220), unit => _settings.useCurse &&  GotDebuff("Unstable Affliction",unit) && CanCast("Fel Flame") && (new Random()).Next(100)>50},
                {new SpellPriority("Curse of the Elements", 210), unit => !GotDebuff("Curse of the Elements",unit) && CanCast("Curse of the Elements")},
                {new SpellPriority("Unstable Affliction", 205), unit => _settings.useImmolate && !GotDebuff("Unstable Affliction",unit) && CanCast("Unstable Affliction")},
                {new SpellPriority("Corruption", 190), unit => _settings.useCorruption && !GotDebuff("Corruption",unit) && CanCast("Corruption")},
                {new SpellPriority("Bane of Doom", 180), unit => _settings.useCurse && (_settings.curseName == "Auto") && !GotDebuff("Bane of Doom",unit) &&  CanCast("Bane of Doom")},
                {new SpellPriority("Bane of Agony", 170), unit => _settings.useCurse && (_settings.curseName == "Auto") && !GotDebuff("Bane of Agony",unit)&& !GotDebuff("Bane of Doom",unit) && CanCast("Bane of Agony")},
                {new SpellPriority("Curse of Weakness", 160), unit => !GotDebuff("Curse of Weakness",unit) && !GotDebuff("Curse of the Elements",unit) && CanCast("Curse of Weakness")},
                {new SpellPriority(_settings.curseName, 150), unit => _settings.useCurse && _settings.curseName != "Auto" && !GotDebuff(_settings.curseName,unit) && CanCast(_settings.curseName)},
                {new SpellPriority("Hand of Gul'dan", 101), unit => CanCast("Hand of Gul'dan") && (new Random()).Next(1,100)>70},
                {new SpellPriority("Incinerate", 100), unit => _settings.useIncinerate && CanCast("Incinerate") && GotDebuff("Immolate")},
                {new SpellPriority("Shadow Bolt", 90), unit => _settings.useShadowBolt && CanCast("Shadow Bolt")&& (unit.HealthPercent > 30)},
                {new SpellPriority("Searing Pain", 80), unit => _settings.useSearingOfPain && CanCast("Searing Pain")}
            },
        new Dictionary<SpellPriority, CastRequirements> // Demonology
            {
                //{new SpellPriority("Metamorphosis", 250), unit => CanCast("Metamorphosis")},
                {new SpellPriority("Immolation Aura", 300), unit => GotBuff("Metamorphosis") && !GotBuff("Immolation Aura") },
                {new SpellPriority("Shadow Bolt", 300), unit => GotBuff("Shadow Trance") },
                {new SpellPriority("Immolate", 240), unit => !GotDebuff("Immolate",unit) && CanCast("Immolate")&& (unit.HealthPercent > 30)},
                {new SpellPriority("Bane of Doom", 235), unit =>  !GotDebuff("Bane of Doom",unit) &&  CanCast("Bane of Doom")},
                {new SpellPriority("Hand of Gul'dan", 231), unit => CanCast("Hand of Gul'dan")  && !GotDebuff("Curse of Gul'dan",unit)},
                {new SpellPriority("Corruption", 230), unit => _settings.useCorruption && !GotDebuff("Corruption",unit) && CanCast("Corruption")},
                {new SpellPriority("Curse of the Elements", 220), unit => !GotDebuff("Curse of the Elements",unit)  && CanCast("Curse of the Elements")},
                {new SpellPriority("Bane of Agony", 170), unit => _settings.useCurse && (_settings.curseName == "Auto") && !GotDebuff("Bane of Agony",unit)&& !GotDebuff("Bane of Doom",unit) && CanCast("Bane of Agony")},
                {new SpellPriority("Curse of Weakness", 160), unit =>  !GotDebuff("Curse of Gul'dan",unit) && !GotDebuff("Curse of Weakness",unit) && !GotDebuff("Curse of the Elements",unit) && CanCast("Curse of Weakness")},
                {new SpellPriority("Incinerate", 150), unit => _settings.useIncinerate && CanCast("Incinerate") && GotDebuff("Molten Core",Me)},
                {new SpellPriority("Soul Fire", 150), unit => _settings.useSoulFire && CanCast("Soul Fire")&& (GotBuff("Decimation")) },
                {new SpellPriority("Shadow Bolt", 140), unit => _settings.useShadowBolt && CanCast("Shadow Bolt")&& ((unit.HealthPercent > 50) || (unit.CurrentHealth > 600))},
                {new SpellPriority("Searing Pain", 80), unit => _settings.useSearingOfPain && CanCast("Searing Pain")}
            },
        new Dictionary<SpellPriority, CastRequirements> // Destruction
            {
                {new SpellPriority("Fel Flame", 220), unit => _settings.useCurse &&  GotDebuff("Immolate",unit) && CanCast("Fel Flame") && (new Random()).Next(100)>50},
                {new SpellPriority("Curse of the Elements", 210), unit => _settings.useCurse &&  (_settings.curseName == "Auto") && !GotDebuff("Curse of the Elements",unit) && CanCast("Curse of the Elements")},
                {new SpellPriority("Immolate", 200), unit => _settings.useImmolate && !GotDebuff("Immolate",unit)&& CanCast("Immolate")&& (unit.HealthPercent > 30)},
                {new SpellPriority("Conflagrate", 195), unit => _settings.useImmolate && GotDebuff("Immolate",unit)&& !GotDebuff("Conflagrate",unit) && CanCast("Conflagrate")},
                {new SpellPriority("Corruption", 190), unit => _settings.useCorruption && !GotDebuff("Corruption",unit) && CanCast("Corruption")},
                {new SpellPriority("Bane of Doom", 180), unit => _settings.useCurse && (_settings.curseName == "Auto") && !GotDebuff("Bane of Doom",unit) &&  CanCast("Bane of Doom")},
                {new SpellPriority("Bane of Agony", 170), unit => _settings.useCurse && (_settings.curseName == "Auto") && !GotDebuff("Bane of Agony",unit)&& !GotDebuff("Bane of Doom",unit) && CanCast("Bane of Agony")},
                {new SpellPriority("Curse of Weakness", 160), unit => _settings.useCurse && (_settings.curseName == "Auto") && !GotDebuff("Curse of Weakness",unit) && !GotDebuff("Curse of the Elements",unit) && CanCast("Curse of Weakness")},
                {new SpellPriority(_settings.curseName, 150), unit => _settings.useCurse && _settings.curseName != "Auto" && !GotDebuff(_settings.curseName,unit) && CanCast(_settings.curseName)},
                {new SpellPriority("Hand of Gul'dan", 101), unit => CanCast("Hand of Gul'dan") && (new Random()).Next(1,100)>70},
                {new SpellPriority("Incinerate", 100), unit => _settings.useIncinerate && CanCast("Incinerate") && GotDebuff("Immolate")},
                {new SpellPriority("Shadow Bolt", 90), unit => _settings.useShadowBolt && CanCast("Shadow Bolt")&& (unit.HealthPercent > 30)},
                {new SpellPriority("Searing Pain", 80), unit => _settings.useSearingOfPain && CanCast("Searing Pain")}
            }                                                                                        };

        private Stopwatch _cDecision = new Stopwatch();

        private void CombatDecision()
        {
            _cDecision.Reset();
            _cDecision.Start();
            if (_settings.showDebug)
            {
                slog("Starting Combat Decision Routine!");
            }

            if (!Me.GotTarget)
            {
                _cDecision.Stop();
                if (_settings.showDebug)
                {
                    slog("Exiting Combat Decision Routine (NoTarget)! Duration {0}ms", _cDecision.ElapsedMilliseconds);
                }
                return;
            }

            if (!isPulling && (NeedDeathCoil || NeedDrainLife || NeedDrainMana || NeedDrainSoul || NeedEMHealth ||
                  NeedEMMana || NeedHealthStone || NeedLifeTap))
            {
                if (_settings.showDebug)
                {
                    slog("Decision to Health Manager");
                }
                CombatHealthManager();
                while (Me.IsCasting)
                    Thread.Sleep(50);
            }
            else
            {
                if (_settings.showDebug)
                {
                    slog("Decision to Enqueue SpellRotation");
                }
                _rotation.Clear();

                if (combatChecks)
                {
                    _rotation.Clear();
                    Enqueue(_spellRotation[WarlockTree], 1);
                    Cast();
                    MetamorphosisMania(true);
                }
            }
            while (Me.IsCasting)
                Thread.Sleep(50);

            if (_settings.useWand && CanCast("Shoot") )
                Shoot();

            _cDecision.Stop();
            if (_settings.showDebug)
            {
                slog("Exiting Combat Decision Routine! Duration {0}ms", _cDecision.ElapsedMilliseconds);
            }


        }


        #endregion

        #region metamorphisis
        private void MetamorphosisMania()
        {
            MetamorphosisMania(false);
        }

        private void MetamorphosisMania(bool check)
        {

            if (!GotBuff("Metamorphosis") && !check)
            {
                Metamorphosis();
                Thread.Sleep(_settings.myLag);
            }
            
            if (GotBuff("Metamorphosis"))
            {
                if (Me.CurrentTarget != null)
                {
                    if (Me.CurrentTarget.Distance <= 25 && Me.CurrentTarget.Distance >= 8)
                        SafeCast("Demon Leap",false);

                    while (Me.CurrentTarget.Distance > 5 && Me.CurrentTarget.InLineOfSight && !Me.Fleeing)
                    {
                        WoWMovement.ClickToMove(Me.CurrentTarget.Location, 2);
                        Thread.Sleep(_settings.myLag);
                    }

                    if (!Me.IsAutoAttacking)
                        Lua.DoString("StartAttack()");


                }
            }

        }

        #endregion


        #region buffs
        private readonly Dictionary<SpellPriority, CastRequirements> _buffs = new Dictionary<SpellPriority, CastRequirements>
        {
            {new SpellPriority("Unending Breath", 110), unit =>  Me.IsSwimming && CanCast("Unending Breath") && !GotBuff("Unending Breath") },
            {new SpellPriority("Fel Armor", 100), unit =>  _settings.useArmor && CanCast("Fel Armor") && !GotBuff("Fel Armor") },
            {new SpellPriority("Demon Armor", 90), unit => _settings.useArmor && CanCast("Demon Armor") && !GotBuff("Demon Armor") && !GotBuff("Fel Armor")},
            {new SpellPriority("Demon Skin", 80), unit => _settings.useArmor && CanCast("Demon Skin") && !GotBuff("Demon Skin")&& !GotBuff("Demon Armor") && !GotBuff("Fel Armor") }
        };

        private bool NeedBuffs
        {
            get
            {
                if (Me.IsAFKFlagged)
                {
                    Styx.Helpers.KeyboardManager.AntiAfk();
                    Styx.Helpers.KeyboardManager.PressKey('Z');
                    Thread.Sleep(1000);
                    Styx.Helpers.KeyboardManager.ReleaseKey('Z');
                }

                _rotation.Clear();
                Enqueue(_buffs, 1);

                if (_rotation.Count > 0)
                {
                    slog("Need {0}", _rotation.Peek().Name);
                    return true;
                }

                if (SpellManager.Spells.ContainsKey("Create Healthstone") &&
                    (myHealthstone == null) &&
                    _settings.useHealthStone)
                {
                    slog("Need a Healthstone");
                    return true;
                }

                if (mySoulstone != null &&
                    !GotBuff("Soulstone Resurrection") &&
                    (!isItemInCooldown(mySoulstone)) &&
                    _settings.useSoulstone && !GotBuff("Metamorphosis"))
                {
                    slog("Need Soulstone Buff");
                    return true;
                }

                if (needPet)
                    return true;

                return false;
            }
        }
        private void BuffMe()
        {
            //if (!NeedBuffs)
            //    return;

            if (_rotation.Count > 0)
            {
                //slog("Need {0}", _rotation.Peek().Name);
                SelfCast();
            }

            if (SpellManager.Spells.ContainsKey("Create Healthstone") &&
                (myHealthstone == null) &&
                _settings.useHealthStone)
            {
                slog("Create a Healthstone");
                CreateHealthstone();
                //return true;
            }

            if (_settings.useSoulstone &&
                mySoulstone != null &&
                !Me.Auras.ContainsKey("Soulstone Resurrection") &&
                !isItemInCooldown(mySoulstone))
            {
                if (UseSoulStone)
                {
                    return;
                }
            }


        }


        #endregion



        #region dots

        /*        private bool NeedDot(WoWUnit unit)
        {
            if (unit.HealthPercent <= 20 || Me.ManaPercent <= 40 || Me.HealthPercent <= 40 || !Me.Combat)
                return false;

            _rotation.Clear();

            Enqueue(unit,_dotRotation, 3);

            return _rotation.Count > 0;
        }
        private bool NeedDot()
        {

            return NeedDot(Me.CurrentTarget);
        }
        */
        /*        private void DotMyTarget()
                {
                    _rotation.Clear();

                    while (Me.IsCasting)
                    {
                        Thread.Sleep(_settings.myLag);
                    }

                    if (combatChecks)
                    {
                        if (isTotem)
                        {
                            slog(Me.CurrentTarget.Type.ToString());
                            slog("Dont DoT a Totem! Duh!");
                            if (_settings.useWand)
                                Shoot();
                            if (Me.GotAlivePet)
                                Lua.DoString("PetAttack()");
                            Thread.Sleep(_settings.myLag);
                        }
                        else
                        {
                            Enqueue(_dotRotation, 3);
                            Cast();

                        }

                        if (Me.CurrentTarget.HealthPercent > 20)
                        {
                            if (SpellManager.Spells.ContainsKey("Haunt"))
                            {
                                Haunt();
                            }
                        }

                        if (Me.CurrentTarget.HealthPercent > 40)
                        {
                            UnstableAffliction();
                        }

                    }
                }*/
        #endregion



        #region summons

        private bool canSummon
        {
            get
            {

                return ((SpellManager.Spells.ContainsKey("Summon Imp") ||
                    SpellManager.Spells.ContainsKey("Summon Voidwalker") ||
                    SpellManager.Spells.ContainsKey("Summon Felguard")) &&
                    _settings.useSummon);
            }

        }

        private void CallSummon()
        {

            if (canSummon)
            {
                slog("Calling my Summon!");
                if (CanCast("Soulburn"))
                {
                    SafeCast("Soulburn", false);
                    Thread.Sleep(_settings.myLag * 2);
                }

                switch (_settings.summonEntry)
                {
                    case 416:
                        if (SpellManager.Spells.ContainsKey("Summon Imp"))
                            SummonImp();
                        break;
                    case 1860:
                        if (SpellManager.Spells.ContainsKey("Summon Voidwalker"))
                        {
                            SummonVoidwalker();
                        }
                        else
                        {
                            slog("Cannot Summon Voidwalker, summoning imp instead!");
                            SummonImp();
                        }
                        break;
                    case 1863:
                        if (SpellManager.Spells.ContainsKey("Summon Succubus"))
                        {
                            SummonSuccubus();
                        }
                        else
                        {
                            slog("Cannot Summon Succubus, summoning imp instead!");
                            SummonImp();
                        }
                        break;
                    case 17252:
                        if (SpellManager.Spells.ContainsKey("Summon Felguard"))
                        {
                            SummonFelguard();
                        }
                        else
                        {
                            slog("Cannot Summon Felguard, summoning imp instead!");
                            SummonImp();
                        }
                        break;
                    default:
                        slog("Method = AutoSumon");
                        if (SpellManager.Spells.ContainsKey("Summon Felguard"))
                            SummonFelguard();
                        else if (SpellManager.Spells.ContainsKey("Summon Succubus") && Battlegrounds.IsInsideBattleground)
                            SummonSuccubus();
                        else if (SpellManager.Spells.ContainsKey("Summon Voidwalker"))
                            SummonVoidwalker();
                        else if (SpellManager.Spells.ContainsKey("Summon Imp"))
                            SummonImp();
                        break;
                }
                Thread.Sleep(_settings.myLag * 2);
            }
            else
                slog("I Cant Summon Anything right now");

        }

        private bool needPet
        {
            get
            {
                if (!canSummon)
                    return false;

                if (Me.GotAlivePet)
                {
                    if (_settings.summonEntry == 0)
                        return false;

                    if (_settings.summonEntry != Me.Pet.Entry)
                    {
                        slog("Need to change summon");
                        return true;
                    }
                }

                if (!Me.GotAlivePet && _settings.useSummon)
                    return true;

                return false;
            }

        }


        private void CheckSummon()
        {
            if (!dismountTimer.IsRunning)
                dismountTimer.Start();

            if (!canSummon || Me.Mounted || dismountTimer.ElapsedMilliseconds < 4000)
                return;
            if (needPet && (!Me.Combat || (Me.Combat && Me.HealthPercent > 40 && Me.ManaPercent > 40)))
            {
                CallSummon();
                return;
            }

            if (Me.GotAlivePet)
            {
                if (Me.Combat || !PetCanCast("Consume Shadows"))
                {
                    if (_settings.useHealthFunnel &&
                        CanCast("Health Funnel") &&
                        Me.Pet.HealthPercent < _settings.hfPetMinHealth &&
                        Me.HealthPercent > _settings.hfMinPlayerHealth)
                    {
                        SafeCast("Health Funnel", false);
                        Thread.Sleep(_settings.myLag);
                        while (Me.HealthPercent > _settings.hfMinPlayerHealth &&
                            Me.Pet.HealthPercent < _settings.hfPetMaxHealth &&
                            GotBuff("Health Funnel") && Me.Combat)
                        {
                            slog("Funneling!");
                            Thread.Sleep(50);
                        }
                    }
                }
                else
                {
                    if (!Me.Combat && PetCanCast("Consume Shadows"))
                        if (Me.Pet.HealthPercent < 70)
                        {
                            PetCast("Consume Shadows");
                            Thread.Sleep(_settings.myLag);
                        }
                }
                if (Me.Combat && PetCanCast("Felstorm"))
                {
                    PetCast("Felstorm");
                    Thread.Sleep(_settings.myLag);
                }
                if (Me.Combat && CanCast("Demonic Empowerment") &&
                    !Me.Pet.Auras.ContainsKey("Demonic Empowerment"))
                {
                    DemonicEmpowerment();
                    Thread.Sleep(_settings.myLag);
                }
                if (!GotBuff("Soul Link") && CanCast("Soul Link"))
                {
                    SoulLink();
                    Thread.Sleep(_settings.myLag);
                }

            }



        }

        #endregion



        #region warlock spells

        /// <summary>
        /// Strikes fear in the enemy, causing it to run in fear for up to 10 sec.
        /// Damage caused my interrupt the effect.
        /// Only 1 target can be feared at a time.
        /// </summary>
        private bool Fear()
        {
            if (_settings.useFear)
            {
                if (SafeCast("Fear"))
                {
                    slog("Run Away! Chicken!");
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Protects the caster, increasing armor by 90, and increasing the amount of 
        /// health generated through spells and effects by 20%. 
        /// Only one type of Armor spell can be active on the Warlock at any time.  Lasts 30 min.
        /// </summary>        
        private bool DemonSkin()
        {
            if (SafeCast("Demon Skin", false))
            {
                slog("Demon Skin.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Burns the enemy for 8 Fire damage and then an additional Fire damage over 15 sec.
        /// </summary>        
        private bool Immolate()
        {
            if (_settings.useImmolate)
            {
                if (SafeCast("Immolate"))
                {
                    slog("Immolate that HO! :D.");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sends a shadowy bolt at the enemy, causing Shadow damage.
        /// </summary>        
        private bool ShadowBolt()
        {
            if (_settings.useShadowBolt)
            {
                if (SafeCast("Shadow Bolt"))
                {
                    slog("Shadow Bolt.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Summons an Imp under the command of the Warlock.
        /// </summary>

        private bool SummonImp()
        {
            if (_settings.useSummon)
            {
                if (SafeCast("Summon Imp", false))
                {
                    slog("Summon Imp.");
                    Thread.Sleep(1000);
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(_settings.myLag);
                        if (Me.GotAlivePet && Me.Pet.Entry == 416)
                        {
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;


        }
        /// <summary>
        /// Enslave Target Demon up to CurrrentLevel + 2 for 5 minutes 
        /// </summary>        
        private bool EnslaveDemon()
        {
            if (_settings.useEnslaveDemon)
            {
                if (SafeCast("Enslave Demon"))
                {
                    slog("Enslaving {0}", Me.CurrentTarget.Name);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Corrupts the target, causing 40 Shadow damage over 12 sec.
        /// </summary>        
        private bool Corruption()
        {
            if (_settings.useCorruption)
            {
                if (SafeCast("Corruption"))
                {
                    slog("Corruption this moff! :>");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts health into [0 + SPI * 1] mana.  Spirit increases quantity of health converted.
        /// </summary>        
        private bool LifeTap()
        {

            if (_settings.useLifeTap)
            {
                if (SafeCast("Life Tap", false))
                {
                    slog("Life Tap.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Curses the target with agony, causing Shadow damage over 24 sec.  This damage is dealt 
        /// slowly at first, and builds up as the Curse reaches its full duration. 
        /// </summary>        
        //        private bool CurseOfAgony()
        //        {
        //            if (!_settings.useCurse)
        //            {
        //                return false;
        //            }

        //            if (SafeCast("Bane of Agony"))
        //            {
        //                slog("Bane of Agony, this sob");
        //                return true;
        //            }
        //            else
        //            {
        // slog("Can't use Bane of Agony now.");
        //                return false;
        //            }
        //        }
        /// <summary>
        /// </summary>        
        private bool Curse(string curseName)
        {
            if (_settings.useCurse)
            {
                if (SafeCast(curseName))
                {
                    slog("{0}, feel my {1}", Me.CurrentTarget.Name, curseName);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a Healthstone that can be used to instantly restore health.
        /// </summary>        
        private bool CreateHealthstone()
        {
            if (_settings.useHealthStone)
            {

                if (SafeCast("Create Healthstone", false))
                {
                    slog("Create Healthstone.");
                    Thread.Sleep((int)SpellManager.Spells["Create Healthstone"].CastTime);
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(_settings.myLag);
                    }

                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// Drains the soul of the target, causing 55 Shadow damage over 15 sec.  If the target is at or 
        /// below 25% health, Drain Soul causes four times the normal damage
        /// If the target dies while being drained, and yields experience or honor, 
        /// the caster gains a Soul Shard.  Each time the Drain Soul damages the target, 
        /// it also has a chance to generate a Soul Shard. 
        /// </summary>        
        private bool DrainSoul()
        {
            if (_settings.useDrainSoul)
            {

                if (SafeCast("Drain Soul"))
                {
                    slog("Drain Soul.");
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// Summons a Voidwalker under the command of the Warlock.
        /// </summary>        
        private bool SummonVoidwalker()
        {
            if (SafeCast("Summon Voidwalker", false))
            {
                //slog("Summon Voidwalker.");
                Thread.Sleep((int)SpellManager.Spells["Summon Voidwalker"].CastTime);
                while (Me.IsCasting)
                {
                    Thread.Sleep(_settings.myLag);
                    if (Me.GotAlivePet && Me.Pet.Entry == 1860)
                    {
                        break;
                    }
                }
                return true;
            }
            else
            {
                // slog("Can't use Summon Voidwalker now.");
                return false;
            }
        }

        /// <summary>
        /// Summons a Voidwalker under the command of the Warlock.
        /// </summary>        
        private bool SummonSuccubus()
        {
            if (SafeCast("Summon Succubus", false))
            {
                slog("Summon Succubus.");
                Thread.Sleep((int)SpellManager.Spells["Summon Succubus"].CastTime);
                while (Me.IsCasting)
                {
                    Thread.Sleep(_settings.myLag);
                    if (Me.GotAlivePet) //&& Me.Pet.Entry == 1860)
                    {
                        break;
                    }
                }
                return true;
            }
            else
            {
                // slog("Can't use Summon Voidwalker now.");
                return false;
            }
        }

        /// <summary>
        /// Summons a Felguard under the command of the Warlock.
        /// </summary>        
        private bool SummonFelguard()
        {
            //            SpellManager.CastSpell("Fel Domination");

            //            Thread.Sleep(1000);

            if (SafeCast("Summon Felguard", false))
            {
                Thread.Sleep((int)SpellManager.Spells["Summon Felguard"].CastTime);
                while (Me.IsCasting)
                {
                    Thread.Sleep(_settings.myLag);
                    if (Me.GotAlivePet && Me.Pet.Entry == 17252)
                    {
                        break;
                    }
                }
                return true;
            }
            else
            {
                // slog("Can't use Summon Felguard now.");
                return false;
            }
        }

        /// <summary>
        /// Gives health to the caster's pet every second for 10 sec as long as the caster channels.
        /// </summary>        
        private bool HealthFunnel()
        {
            if (_settings.useHealthFunnel)
            {
                if (SafeCast("Health Funnel", false))
                {
                    slog("Health Funnel.");
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Attack with equiped wand.
        /// </summary>        
        private bool Shoot()
        {
            if (_settings.useWand)
            {
                if (SafeCast("Shoot"))
                {
                    //slog("Using wand!");
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Transfers health every 1 sec from the target to the caster.  Lasts 5 sec.
        /// </summary>        
        private bool DrainLife()
        {
            if (_settings.useDrainLife)
            {
                if (SafeCast("Drain Life"))
                {
                    slog("Drain Life.");
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Transfers health every 1 sec from the target to the caster.  Lasts 5 sec.
        /// </summary>        
        private bool DrainMana()
        {
            if (_settings.useDrainMana)
            {

                if (SafeCast("Drain Mana"))
                {
                    slog("Drain Mana.");
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// The Soulstone can be used to store one target's soul.  If the target dies while his soul is stored, 
        /// he will be able to resurrect.
        /// </summary>        
        private bool CreateSoulstone()
        {
            if (_settings.useSoulstone)
            {
                if (SafeCast("Create Soulstone", false))
                {
                    slog("Create Soulstone.");
                    Thread.Sleep((int)SpellManager.Spells["Create Soulstone"].CastTime);
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(_settings.myLag);
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Protects the caster, increasing armor, and increasing the amount of health 
        /// generated through spells and effects by 20%
        /// </summary>        
        private bool DemonArmor()
        {
            if (SafeCast("Demon Armor", false))
            {
                slog("Demon Armor.");
                return true;
            }
            return false;

        }

        /// <summary>
        /// While applied to target weapon it increases damage dealt by direct spells by 1% 
        /// and spell critical strike rating. 
        /// </summary>        
        private bool CreateFirestone()
        {
            if (_settings.useFirestone)
            {
                if (SafeCast("Create Firestone", false))
                {
                    slog("Create Firestone.");
                    Thread.Sleep((int)SpellManager.Spells["Create Firestone"].CastTime);
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(_settings.myLag);
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// While applied to target weapon it increases damage dealt by periodic spells by 1% and spell haste rating.
        /// </summary>        
        private bool CreateSpellstone()
        {
            if (_settings.useSpellstone)
            {
                if (SafeCast("Create Spellstone", false))
                {
                    slog("Create Spellstone.");
                    Thread.Sleep((int)SpellManager.Spells["Create Spellstone"].CastTime);
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(_settings.myLag);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Causes the enemy target to run in horror for 3 sec and causes Shadow damage.  
        /// The caster gains 300% of the damage caused in health.
        /// </summary>        
        private bool DeathCoil()
        {
            if (_settings.useDeathCoil)
            {
                if (SafeCast("Death Coil"))
                {
                    slog("Death Coil.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Voidwalker - Increases the Voidwalker's health by 20%, and its threat 
        /// generated from spells and attacks by 20% for 20 sec.
        /// 
        /// Increases the Felguard's attack speed by 20% and breaks all stun, 
        /// snare and movement impairing effects and makes your Felguard immune to them for 15 sec.
        /// </summary>
        /// <returns></returns>
        private bool DemonicEmpowerment()
        {
            if (_settings.useDemonicEmpowerment)
            {
                if (SafeCast("Demonic Empowerment"))
                {
                    //slog("Demonic Empowerment.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Drains your summoned demon's Mana, returning 100% to you.
        /// </summary>        
        private bool DarkPact()
        {
            if (_settings.useDarkPact)
            {
                if (SafeCast("Dark Pact", false))
                {
                    slog("Dark Pact.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Shadow energy slowly destroys the target, causing damage over 15 sec.  
        /// In addition, if the Unstable Affliction is dispelled it will cause serious damage to the dispeller 
        /// and silence them for 5 sec.
        /// </summary>        
        private bool UnstableAffliction()
        {
            if (_settings.useUnstableAffliction)
            {
                if (SafeCast("Unstable Affliction"))
                {
                    slog("Unstable Affliction.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Surrounds the caster with fel energy, increasing spell power plus additional spell 
        /// power equal to 30% of your Spirit. 
        /// In addition, you regain 2% of your maximum health every 5 sec.
        /// </summary>        
        private bool FelArmor()
        {
            if (SafeCast("Fel Armor", false))
            {
                slog("Fel Armor.");
                return true;
            }

            return false;

        }
        /// <summary>
        /// Your next Imp, Voidwalker, Succubus, Felhunter or Felguard Summon spell has its casting 
        /// time reduced by 5.5 sec and its Mana cost reduced by 50%.
        /// </summary>        

        private bool FelDomination()
        {
            if (_settings.useFelDomination)
            {
                if (SafeCast("Fel Domination", false))
                {
                    slog("Fel Domination.");
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// When active, 20% of all damage taken by the caster is taken by your Imp, Voidwalker, 
        /// Succubus, Felhunter, Felguard, or enslaved demon instead.  
        /// That damage cannot be prevented. Lasts as long as the demon is active and controlled.
        /// </summary>        
        private bool SoulLink()
        {
            if (_settings.useSoulLink)
            {
                if (SafeCast("Soul Link", false))
                {
                    slog("Soul Link.");
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// You send a ghostly soul into the target, dealing Shadow damage and increasing all damage done 
        /// by your Shadow damage-over-time effects on the target by 20% for 12 sec. 
        /// When the Haunt spell ends or is dispelled, the soul returns to you, healing you for 100% of the 
        /// damage it did to the target.
        /// </summary>        
        private bool Haunt()
        {
            if (_settings.useHaunt)
            {
                if (SafeCast("Haunt"))
                {
                    slog("Haunt.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// You transform into a Demon for 30 sec.  This form increases your armor by 600%, damage by 20%, 
        /// reduces the chance you'll be critically hit by melee attacks by 6% and reduces the duration of 
        /// stun and snare effects by 50%.
        ///  You gain some unique demon abilities in addition to your normal abilities.
        /// </summary>        
        private bool Metamorphosis()
        {
            if (_settings.useMetamorphosis)
            {
                if (SafeCast("Metamorphosis", false))
                {
                    slog("*Metamorphosis*");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Inflicts 110 Shadow damage to an enemy target and nearby allies, affecting up to 3 targets.
        /// </summary>        
        private bool ShadowCleave()
        {
            if (_settings.useShadowCleave)
            {
                if (SafeCast("Shadow Cleave", false))
                {
                    slog("Shadow Cleave.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Ignites the area surrounds you, causing 251 Fire damage to all nearby enemies every 1 sec.  
        /// Lasts 15 sec.
        /// </summary>
        private bool ImmolationAura()
        {
            if (_settings.useImmolationAura)
            {
                if (SafeCast("Immolation Aura", false))
                {
                    slog("Immolation Aura.");
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Targets in a cone in front of the 
        /// caster take Shadow damage and an additional Fire damage over 8 sec.
        /// </summary>        
        private bool Shadowflame()
        {
            if (_settings.useShadowFlame)
            {

                if (SafeCast("Shadowflame"))
                {
                    slog("Shadowflame.");
                    return true;
                }
            }
            return false;
        }

        #endregion



        #region drinks
        private string detectDrink()//Created by erenion
        {
            List<string> drinks = new List<string>();
            string drink;
            if (Me.Level >= 80)
            {
                drinks.Add("Conjured Mana Strudel");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }

            if (Me.Level >= 75)
            {
                drinks.Add("Black Jelly");
                drinks.Add("Crusader's Waterskin");
                drinks.Add("Honeymint Tea");
                drinks.Add("Kungaloosh");
                drinks.Add("Star's Sorrow");
                drinks.Add("Yeti Milk");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }

            if (Me.Level >= 74)
            {
                drinks.Add("Conjured Mana Pie");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }

            if (Me.Level >= 70)
            {
                drinks.Add("Bitter Plasma");
                drinks.Add("Fresh Apple Juice");
                drinks.Add("Fresh-Squeezed Limeade");
                drinks.Add("Grilled Bonescale");
                drinks.Add("Pungent Seal Whey");
                drinks.Add("Smoked Rockfin");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 65)
            {
                drinks.Add("Black Coffee");
                drinks.Add("Blackrock Fortified Water");
                drinks.Add("Conjured Glacier Water");
                drinks.Add("Conjured Mana Biscuit");
                drinks.Add("Dos Ogris");
                drinks.Add("Enriched Terocone Juice");
                drinks.Add("Ethermead");
                drinks.Add("Frostberry Juice");
                drinks.Add("Gilneas Sparkling Water");
                drinks.Add("Grizzleberry Juice");
                drinks.Add("Hot buttered Trout");
                drinks.Add("Mountain Water");
                drinks.Add("Naaru Ration");
                drinks.Add("Purified Draenic Water");
                drinks.Add("Sparkling Southshore Cider");
                drinks.Add("Star's Tears");
                drinks.Add("Sweetened Goat's Milk");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 60)
            {
                drinks.Add("Blackrock Mineral Water");
                drinks.Add("Conjured Mountain Spring Water");
                drinks.Add("Filtered Draenic Water");
                drinks.Add("Silverwine");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 55)
            {
                drinks.Add("Conjured Crystal Water");
                drinks.Add("Hyjal Nectar");
                drinks.Add("Star's Lament");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 51)
            {
                drinks.Add("Alterac Manna Biscuit");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 45)
            {
                drinks.Add("Blackrock Spring Water");
                drinks.Add("Conjured Sparkling Water");
                drinks.Add("Morning Glory Dew");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 35)
            {
                drinks.Add("Bottled Winterspring Water");
                drinks.Add("Conjured Mineral Water");
                drinks.Add("Moonberry Juice");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 25)
            {
                drinks.Add("Conjured Spring Water");
                drinks.Add("Enchanted Water");
                drinks.Add("Goldthorn Tea");
                drinks.Add("Sweet Nectar");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 15)
            {
                drinks.Add("Melon Juice");
                drinks.Add("Bubbling Water");
                drinks.Add("Conjured Purified Water");
                drinks.Add("Fizzy Faire Drink");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 5)
            {
                drinks.Add("Blended Bean Brew");
                drinks.Add("Conjured Fresh Water");
                drinks.Add("Ice Cold Milk");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 4)
            {
                drinks.Add("Green Tea Leaf");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            if (Me.Level >= 0)
            {
                drinks.Add("Conjured Water");
                drinks.Add("Refreshing Spring Water");
                drinks.Add("Snggin Root");
                if ((drink = findItem(drinks)) != "")
                {
                    return drink;
                }
                drinks.Clear();
            }
            return "";
        }
        private string findItem(List<string> drinks)//Created by erenion
        {
            foreach (WoWItem item in ObjectManager.GetObjectsOfType<WoWItem>())
            {
                foreach (string drink in drinks)
                {
                    if (drink == item.Name)
                    {
                        return drink;
                    }
                }
            }
            return "";
        }

        #endregion


        #region bandagelogic
        private readonly List<uint> bandageEID = new List<uint>()
        {
            34722,34721,21991,21990,14530,14529,8545,8544,
            6451,6450,3531,3530,2581,1251,
        };

        private void UseBandage()
        {
            Thread.Sleep(_settings.myLag);
            if (Me.Auras.ContainsKey("Recently Bandaged"))
                return;
            WoWItem bandage = HaveItemCheck(bandageEID);
            if (bandage != null)
            {
                slog("** BANDAGE NOW **");
                Lua.DoString("UseItemByName(\"" + bandage.Name + "\")");

                Thread.Sleep(1500);

                //                Stopwatch castTimeout = new Stopwatch();
                //castTimeout.Reset();
                //                castTimeout.Start();

                //                while (castTimeout.ElapsedMilliseconds < 6000 || Me.ChanneledCasting != 0)
                while (Me.Auras.ContainsKey("First Aid"))
                {
                    slog("... bandaging...");
                    Thread.Sleep(50);
                }
            }
            //            else
            //                slog("** NO BANDAGES **");
        }

        #endregion


        #region shardlogic





        #endregion


        #region healthStone
        /// <summary>
        /// THANKS NINKO
        /// </summary>
        private readonly List<uint> _healthstoneEntryId = new List<uint>
        {
            5509, 5510, 5511, 5512, 9421, 19004, 19005, 19006,  
            19007, 19008, 19009, 19010, 19011, 19012, 19013, 
            22103, 22104, 22105, 36889, 36890, 36891, 36892,
            36893, 36894,
        };


        private WoWItem myHealthstone
        {
            get
            {
                return HaveItemCheck(_healthstoneEntryId);
            }
        }


        private bool UseHealthStone
        {
            get
            {
                if (Equals(null, myHealthstone))
                    return false;

                if (isItemInCooldown(myHealthstone) ||
                    !_settings.useHealthFunnel)
                {
                    return false;
                }


                slog("Take a health stone.");
                if (CanCast("Soulburn"))
                {
                    SafeCast("Soulburn", false);
                    Thread.Sleep(_settings.myLag * 2);
                }

                Lua.DoString("UseItemByName(\"" + myHealthstone.Name + "\")");
                return true;
            }

        }
        #endregion


        #region soulStone
        private readonly List<uint> _soulstoneEntryId = new List<uint>
        {
            5232, 16892, 16893, 16895, 16896, 22116,  36895,
        };



        private WoWItem mySoulstone
        {
            get
            {
                return HaveItemCheck(_soulstoneEntryId);
            }
        }

        private bool UseSoulStone
        {
            get
            {
                if (Equals(null, mySoulstone))
                    return false;

                if (isItemInCooldown(mySoulstone) ||
                    !_settings.useSoulstone)
                {
                    return false;
                }

                slog("Take a soul stone.");
                Lua.DoString("UseItemByName(\"" + mySoulstone.Name + "\")");
                Thread.Sleep(3000 + _settings.myLag);
                return true;

            }

        }

        private bool NeedSoulStone
        {
            get
            {
                return false;
            }

        }

        #endregion


        #region firestone
        private readonly List<uint> _firestoneEnchantEntryId = new List<uint>
        {
            3597, 3609, 3610, 3611, 3612, 3613, 3614,
        };

        private readonly List<uint> _firestoneEntryId = new List<uint>
        {
            40773, 41169, 41170, 41171, 41172, 41173, 41174,
        };

        private WoWItem myFirestone
        {
            get
            {
                return HaveItemCheck(_firestoneEntryId);
            }
        }


        #endregion

        #region spellstone
        private readonly List<uint> _spellstoneEnchantEntryId = new List<uint>
        {
            3615, 3616, 3617, 3618, 3619, 3620,
        };

        private readonly List<uint> _spellstoneEntryId = new List<uint>
        {
            41191, 41192, 41193, 41194, 41195, 41196,
        };

        private WoWItem mySpellstone
        {
            get
            {
                return HaveItemCheck(_spellstoneEntryId);
            }
        }


        #endregion


        #region log

        public void slog(Color cor, string msg, bool showName, params object[] args)
        {
            if (msg != _logspam)
            {
                if (showName)
                {
                    Logging.Write(cor, "[" + ShortName + "] " + msg, args);
                }
                else
                {
                    Logging.Write(cor, msg, args);
                }
                _logspam = msg;
            }
        }

        public void slog(string msg, params object[] args)
        {
            slog(Color.ForestGreen, msg, true, args);
        }


        public void slog(string msg)
        {

            slog(Color.ForestGreen, msg, true, "");
        }

        public void slog(string msg, bool showName)
        {

            slog(Color.ForestGreen, msg, showName, "");
        }

        #endregion


    }

}

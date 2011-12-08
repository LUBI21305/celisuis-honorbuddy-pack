using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CommonBehaviors.Actions;
using Styx;
using Amplify.Talents;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Sequence = TreeSharp.Sequence;
using System.Linq;
using Action = TreeSharp.Action;

namespace Amplify
{
    public partial class Amplify
    {
        //I would like to thank no1knowsy for helping me tune and come up with some of the checks 
        //for the core fire spec rotation without his help i couldn't have finished. 
        //Thank you. -Comment Added: 3/9/11

        #region Fire Bools
        private bool _Scorch;
        private bool _LivingBomb;
        private bool _Combustion;
        private bool _Fireball;
        private bool _FireBlast;
        private bool _Pyroblast;
        private bool _FireSpamSpell;
        private bool _Frostbolt;
        #endregion

        private Composite Fire_ChecksAndRotation()
        {
            return new Sequence(
                new Decorator(GetAllChecks("Fire", true)),

                new Decorator(
                    new PrioritySelector(Fire_rotation()))

                    );
        }

        private Composite Fire_rotation()
        {
            return new PrioritySelector(
                new Decorator(ret => _FindClosestPlayer,
                    new Action(ret => FindClostestPlayer(50))),


             new Decorator(ret => _ClearTarget,
                    new Action(ret => Me.ClearTarget())),

                //ManaGem Logic
               new Decorator(ret => _UseManaGem,
                   new Action(ctx => UseManaGem())),

               new Decorator(ret => _SheepLogic,
                   new Action(ctx => SheepLogic())),


               new Decorator(ret => _FrostNova,
                   new Sequence(
                       new Action(ret => SpellManager.Cast("Frost Nova")),
                       new Action(ret => FrostNova())
                           )),

             new Decorator(ret => _SpellSteal,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Counterspell"))),

                new Decorator(ret => _SpellSteal,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Spellsteal"))),

            new Decorator(ret => _Evocation,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Evocation"))),

             new Decorator(ret => _ManaShield,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mana Shield"))),
             new Decorator(ret => _Scorch,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Scorch"))),
                       
             new Decorator(ret => _LivingBomb,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Living Bomb"))),

             new Decorator(ret => _Combustion,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Combustion"))),


                       //in case somehow it gets into fire before level 10
               new Decorator(ret => _Fireball,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Fireball"))),


               new Decorator(ret => _FireBlast,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fire Blast")
                        )),
                        
               new Decorator(ret => _Pyroblast,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Pyroblast")
                        )),

               new Decorator(ret => _FireSpamSpell,
                    new PrioritySelector(
                        CreateSpellCheckAndCast(FireSpamSpell)
                        )),
               new Decorator(ret => _Frostbolt,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Frostbolt")
                        )),

               new Decorator(ret => _Shoot,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Shoot")
                        ))
                        );
        }


        #region Fire Rotation Old
        private Composite Fire_rotationOLD()
        {
            return new PrioritySelector(
                new Decorator(ret => IsBattleGround() && Me.CurrentTarget == null,
                    new Action(ret => FindClostestPlayer(50))),



                new Decorator(ret => IsBattleGround() && Me.CurrentTarget.Dead && Me.Combat,
                    new Action(ret => FindClostestPlayer(50))),

             new Decorator(ret => IsBattleGround() && Me.CurrentTarget.Dead,
                    new Action(ret => Me.ClearTarget())),

                //ManaGem Logic
               new Decorator(ret => AmplifySettings.Instance.Use_ManaGems && HaveManaGem() && Me.ManaPercent <= AmplifySettings.Instance.ManaGems_MP_Percent,
                   new Action(ctx => UseManaGem())),

               new Decorator(ret => AmplifySettings.Instance.Use_Polymorph && NeedToSheep(),
                   new Action(ctx => SheepLogic())),


               new Decorator(ret => Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= AmplifySettings.Instance.FrostNova_Mob_Hp_Above && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Frost Nova") && AmplifySettings.Instance.FrostNova != "Off" && AmplifySettings.Instance.Use_FrostNova,
                   new Sequence(
                       new Action(ret => SpellManager.Cast("Frost Nova")),
                       new Action(ret => FrostNova())
                           )),

             new Decorator(ret => AmplifySettings.Instance.Use_CounterSpell && SpellManager.HasSpell("Counterspell") && SpellManager.CanCast("Counterspell") && Me.CurrentTarget.IsCasting && (Me.CurrentTarget.CastingSpell.SpellEffect1.EffectType == WoWSpellEffectType.Heal || Me.CurrentTarget.CastingSpell.SpellEffect1.EffectType == WoWSpellEffectType.HealMaxHealth || Me.CurrentTarget.CastingSpell.SpellEffect1.EffectType == WoWSpellEffectType.HealPct || Me.CurrentTarget.CastingSpell.SpellEffect1.EffectType == WoWSpellEffectType.HealthLeech || Me.CurrentTarget.CastingSpell.SpellEffect1.EffectType == WoWSpellEffectType.Energize || Me.CurrentTarget.CastingSpell.SpellEffect1.EffectType == WoWSpellEffectType.EnergizePct),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Counterspell"))),

                new Decorator(ret => AmplifySettings.Instance.Spellsteal == "On" && SpellManager.HasSpell("Spellsteal") && SpellManager.CanCast("Spellsteal") && SpellToSteal(),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Spellsteal"))),

            new Decorator(ret => SpellManager.HasSpell("Evocation") && AmplifySettings.Instance.Use_Evocation && SpellManager.CanCast("Evocation") && (Me.ManaPercent < AmplifySettings.Instance.Evocation_MP_Percent || Me.HealthPercent < AmplifySettings.Instance.Evocation_HP_Percent) && (!SpellManager.HasSpell("Mana Shield") || Me.Auras.ContainsKey("Mana Shield") || Me.Auras.ContainsKey("Ice Barrier")),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Evocation"))),

             new Decorator(ret => AmplifySettings.Instance.ManaShieldArcane == "Always" && !Me.Auras.ContainsKey("Mana Shield") && Me.HealthPercent <= AmplifySettings.Instance.ManaShield_Hp_Percent && SpellManager.HasSpell("Mana Shield") && SpellManager.CanCast("Mana Shield"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mana Shield"))),
             new Decorator(ret => AmplifySettings.Instance.CriticalMass && !Me.CurrentTarget.ActiveAuras.ContainsKey("Critical Mass") && SpellManager.CanCast("Scorch"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Scorch"))),

             new Decorator(ret => AmplifySettings.Instance.LivingBomb && !Me.CurrentTarget.ActiveAuras.ContainsKey("Living Bomb") && SpellManager.CanCast("Living Bomb"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Living Bomb"))),

             new Decorator(ret => Me.CurrentTarget.ActiveAuras.ContainsKey("Living Bomb") && Me.CurrentTarget.ActiveAuras.ContainsKey("Ignite") && Me.CurrentTarget.ActiveAuras.ContainsKey("Pyroblast!") && SpellManager.CanCast("Combustion"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Combustion"))),


                       //in case somehow it gets into fire before level 10
               new Decorator(ret => !SpellManager.HasSpell("Frostbolt"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Fireball"))),


               new Decorator(ret => SpellManager.CanCast("Fire Blast") && Me.CurrentTarget.Distance < SpellManager.Spells["Fire Blast"].MaxRange - 2 && Me.ActiveAuras.ContainsKey("Impact"),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fire Blast")
                        )),

               new Decorator(ret => SpellManager.CanCast("Pyroblast") && Me.ActiveAuras.ContainsKey("Hot Streak"),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Pyroblast")
                        )),

               new Decorator(ret => SpellManager.CanCast(FireSpamSpell),
                    new PrioritySelector(
                        CreateSpellCheckAndCast(FireSpamSpell)
                        )),
               new Decorator(ret => SpellManager.CanCast("Frostbolt") && !SpellManager.CanCast("Fireball"),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Frostbolt")
                        )),

               new Decorator(ret => SpellManager.CanCast("Shoot") && IsNotWanding && AmplifySettings.Instance.Use_Wand,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Shoot")
                        ))
                        );
        }
        #endregion


    }
}

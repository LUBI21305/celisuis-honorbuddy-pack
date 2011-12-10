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

        #region ArcaneBools
        private bool _FindClosestPlayer;
        private bool _ClearTarget;
        private bool _UseManaGem;
        private bool _SheepLogic;
        private bool _FrostNova;
        private bool _CounterSpell;
        private bool _SpellSteal;
        private bool _ManaShield;
        private bool _PresenceOfMind;
        private bool _Evocation;
        private bool _FireBall;
        private bool _ArcaneMissiles;
        private bool _ArcaneM_Level20;
        private bool _ArcaneBarrage;
        private bool _ArcaneBlast;
        private bool _FrostBolt;
        private bool _Shoot;
        #endregion

        /*This will run a void with a framelock embedded that will grab ALL necessary checks
         *Being that it is a Sequence it will continue to the next node if it returns sucess, if failure it will stop and return failure up the tree*/
        private Composite Arcane_ChecksAndRotation()
        {
            return new Sequence(
                new Decorator(GetAllChecks("Arcane", true)),

                new Decorator(
                    new PrioritySelector(Arcane_rotation()))

                    );
        }

        /* Potential composites/voids due to Stress WoWClient/HB during single Framelock()  (Mainly for older shitty machines)
    new Decorator(GetChecks2()),
             
    new Decorator(Arcane_rotation2())

    new Decorator(GetChecks3()),

    new Decorator(Arcane_rotation3())
         */





        
        

        private Composite Arcane_rotation()
        {
            return new PrioritySelector(
                new Decorator(ret => _FindClosestPlayer,
                    new Action(ret => FindClostestPlayer(200))),

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

             new Decorator(ret => _CounterSpell,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Counterspell"))),

                new Decorator(ret => _SpellSteal,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Spellsteal"))),

             new Decorator(ret => _ManaShield,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mana Shield"))),

             new Decorator(ret => _PresenceOfMind,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Presence of Mind"))),

            new Decorator(ret => _Evocation,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Evocation"))),



                //Arcane Missles if Proc
             new Decorator(ret => _ArcaneMissiles,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                        )),

                //Arcane Missles < level 20
             new Decorator(ret => _ArcaneM_Level20,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                        )),
             new Decorator(ret => _ArcaneBarrage,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Barrage")
                        )),


               new Decorator(ret => _ArcaneBlast,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Blast")
                        )),
               new Decorator(ret => _FrostBolt,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Frostbolt")
                        )),

               new Decorator(ret => _Shoot,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Shoot")
                        ))
                        );
        }




        #region Old Arcane Rotation
        private Composite Arcane_rotationOLD()
        {
            return new PrioritySelector(
                new Decorator(ret => IsBattleGround() && Me.CurrentTarget == null,
                    new Action(ret => FindClostestPlayer(200))),

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

             new Decorator(ret => AmplifySettings.Instance.ManaShieldArcane == "Always" && !Me.Auras.ContainsKey("Mana Shield") && Me.HealthPercent <= AmplifySettings.Instance.ManaShield_Hp_Percent && SpellManager.HasSpell("Mana Shield") && SpellManager.CanCast("Mana Shield"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mana Shield"))),

             new Decorator(ret => AmplifySettings.Instance.PresenceofMind == "Always" && SpellManager.CanCast("Presence of Mind"),
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Presence of Mind"))),

            new Decorator(ret => SpellManager.HasSpell("Evocation") && AmplifySettings.Instance.Use_Evocation && SpellManager.CanCast("Evocation") && (Me.ManaPercent < AmplifySettings.Instance.Evocation_MP_Percent || Me.HealthPercent < AmplifySettings.Instance.Evocation_HP_Percent) && (!SpellManager.HasSpell("Mana Shield") || Me.Auras.ContainsKey("Mana Shield") || Me.Auras.ContainsKey("Ice Barrier")),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Evocation"))),

                       //in case somehow it gets into arcane before level 10
               new Decorator(ret => !SpellManager.HasSpell("Frostbolt"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Fireball"))),
					   


                //Arcane Missles if Proc
             new Decorator(ret => SpellManager.CanCast("Arcane Missiles") && SpellManager.HasSpell("Arcane Blast") && Me.HasAura("Arcane Blast") && AmplifySettings.Instance.Use_ProcArcaneMissles && Me.ActiveAuras["Arcane Blast"].StackCount >= AmplifySettings.Instance.ArcaneBlastStacks, 
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                        )),

                //Arcane Missles < level 20
             new Decorator(ret => SpellManager.CanCast("Arcane Missiles") && !SpellManager.HasSpell("Arcane Blast") && AmplifySettings.Instance.Use_ProcArcaneMissles,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                        )),
             new Decorator(ret => SpellManager.CanCast("Arcane Barrage") && Me.HasAura("Arcane Blast") && Me.ActiveAuras["Arcane Blast"].StackCount >= AmplifySettings.Instance.ArcaneBlastStacks,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Barrage")
                        )),			
						

               new Decorator(ret => SpellManager.CanCast("Arcane Blast"),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Blast")
                        )),
               new Decorator(ret => SpellManager.CanCast("Frostbolt") && !SpellManager.CanCast("Arcane Blast"),
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
        private Composite ArcaneRaid()
        {
            return new PrioritySelector(
                //some switch here. 
                );
        }
        private Composite ArcaneRaid_Build()
        {
            return new PrioritySelector(
                   new Decorator(ret => SpellManager.CanCast("Shoot") && IsNotWanding && AmplifySettings.Instance.Use_Wand,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Shoot")
                        ))
                );
        }
        private Composite ArcaneRaid_Burn()
        {
            return new PrioritySelector(
                   new Decorator(ret => SpellManager.CanCast("Shoot") && IsNotWanding && AmplifySettings.Instance.Use_Wand,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Shoot")
                        ))
                );
        }
    }
}

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

        #region FrostBools

        private bool _FrostFireFixTry;
        private bool _FixPolyAddDies;
        private bool _IceBlock;
        private bool _IceLance;
        private bool _SummonWaterElemental;
        private bool _ColdSnap;
        private bool _FrostNovaNavigated;
        private bool _Counterspell;
        private bool _IcyVeinAdds;
        private bool _IcyVeinAlways;
        private bool _MirrorImageAdds;
        private bool _MirrorImageAlways;
        private bool _TimeWarp;
        private bool _FlameOrbAlways;
        private bool _FlameOrbAdds;
        private bool _IceBarrier;
        private bool _DeepFreeze_FoF;
        private bool _IceLance_FoF;
        private bool _FireBall_BF;
        private bool _FrostFire_BF;
        private bool _ArcaneMissiles_Proc;
        private bool _FrostFinisher;
        private bool _CurrentSpamSpell;
        
        #endregion


        private Composite Frost_ChecksAndRotation()
        {
            return new Sequence(
                new Decorator(GetAllChecks("Frost", true)),

                new Decorator(
                    new PrioritySelector(Frost_rotation()))

                    );
        }


        public Stopwatch FreezeTimer = new Stopwatch();
        public static Stopwatch SheepTimer = new Stopwatch();
        private Composite Frost_rotation()
        {
            return new PrioritySelector(
                // a Try to fix the Frostfire bug. 
                new Decorator(ret => _FrostFireFixTry,
                    new NavigationAction(ret => Me.CurrentTarget.Location)),
                
                //to fix Applying Poly when the other add dies off. (usaly from frost elemental killing of main target as poly gets applyed)
                new Decorator(ret => _FixPolyAddDies,
                    new Action(ret => SpellManager.StopCasting())),


                new Decorator(ret => _FindClosestPlayer,
                    new Action(ret => FindClostestPlayer(200))),

                new Decorator(ret => _ClearTarget,
                    new Action(ret => Me.ClearTarget())),

               new Decorator(ret => _IceBlock,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Ice Block"))),

                //Fingers of Frost Proc - IceLance
               new Decorator(ret => _IceLance,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Ice Lance")
                        )),

              //Summon Water Elemental if not out, not on cooldown, and if spell is learned. 
             new Decorator(ret => _SummonWaterElemental,
                   new PrioritySelector(CreateSpellCheckAndCast("Summon Water Elemental"))),

                //Step Through Pet Logic.
               Freeze(),

                //Step Through AoE Logic - Blizzard and Flamestrike.
               AOE(),
				   
                //Cast ColdSnap if 2 or more Frost Abilitys are on cooldown.
                new Decorator(ret => _ColdSnap,
                  new Action(ret => SpellManager.Cast("Cold Snap"))),

                //ManaGem Logic
               new Decorator(ret => _UseManaGem,
                   new Action(ctx => UseManaGem())),

               new Decorator(ret => _SheepLogic,
                   new Action(ctx => SheepLogic())),
/*
               new Decorator(ret => Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= AmplifySettings.Instance.FrostNova_Mob_Hp_Above && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Ring of Frost") && getAdds().Count >= 2,
                   new Sequence(
                       new Action(ret => SpellManager.Cast("Ring of Frost")),
                       new Action(ret => Thread.Sleep(600)),
                       new Action(ret => LegacySpellManager.ClickRemoteLocation(Me.Location)),
                       new Action(ctx => BlinkBack())
                           )),
*/

               new Decorator(ret => _FrostNovaNavigated,
                   new Sequence(
                       new Action(ret => SpellManager.Cast("Frost Nova")),
                        new ActionMoveToPoint(NavMe)
                           )),

               new Decorator(ret => _FrostNova,
               
                       //new NavigationAction(ret => FindSuperSafeSpot()),
                       new Action(ret => FrostNova())
                           ),

             new Decorator(ret => _Counterspell,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Counterspell"))),

                new Decorator(ret => _SpellSteal,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Spellsteal"))),

               new Decorator(ret => _IcyVeinAdds,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Icy Veins"))),

               new Decorator(ret => _IcyVeinAlways,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Icy Veins"))),

               new Decorator(ret => _MirrorImageAdds,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mirror Image"))),

               new Decorator(ret => _MirrorImageAlways,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mirror Image"))),

               new Decorator(ret => _TimeWarp,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Time Warp"))),

               new Decorator(ret => _FlameOrbAlways,
                   new Sequence(
                       new Action(ctx => Me.CurrentTarget.Face()),
                       new Action(ctx => Thread.Sleep(650)),
                       new PrioritySelector(CreateSpellCheckAndCast("Flame Orb"))
                           )),
                      
               new Decorator(ret => _FlameOrbAdds,
                   new Sequence(
                       new Action(ctx => Me.CurrentTarget.Face()),
                       new PrioritySelector(CreateSpellCheckAndCast("Flame Orb"))
                           )),

                //IceBarrier Logic use when below 50% Health && ManaShield is not Currently on Me.
             new Decorator(ret => _IceBarrier,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Ice Barrier"))),

             new Decorator(ret => _ManaShield,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mana Shield"))),

            new Decorator(ret => _Evocation,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Evocation"))),

                //Fingers of Frost Proc - DeepFreeze
             new Decorator(ret => _DeepFreeze_FoF,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Deep Freeze")
                        )),
                //Fingers of Frost Proc - IceLance
             new Decorator(ret => _IceLance_FoF,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Ice Lance")
                        )),

                //Brain Freeze Proc - FireBall
             new Decorator(ret => _FireBall_BF,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fireball")
                        )),
                //Brain Freeze Proc - FrostFirebolt 
             new Decorator(ret => _FrostFire_BF,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Frostfire Bolt")
                        )),
                //Arcane Missles if Proc
             new Decorator(ret => _ArcaneMissiles,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                        )),

               new Decorator(ret => _FrostFinisher,
                    new PrioritySelector(
                        CreateSpellCheckAndCast(AmplifySettings.Instance.FrostFinisher)
                        )),

               new Decorator(ret => _CurrentSpamSpell,
                    new PrioritySelector(
                        CreateSpellCheckAndCast(CurrentSpamSpell)
                        )),
               new Decorator(ret => _FireBall,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fireball")
                        )),

               new Decorator(ret => _Shoot,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Shoot")
                        ))
                        );
        }


        #region Old Frost Rotation
        private Composite Frost_rotationOLD()
        {
            return new PrioritySelector(
                // a Try to fix the Frostfire bug. 
                new Decorator(ret => !AmplifySettings.Instance.MoveDisable && Me.GotTarget && (Me.CurrentTarget.Distance > SpellManager.Spells[AmplifySettings.Instance.FrostSpamSpell].MaxRange - 1 || !Me.CurrentTarget.InLineOfSight),
                    new NavigationAction(ret => Me.CurrentTarget.Location)),

                //to fix Applying Poly when the other add dies off. (usaly from frost elemental killing of main target as poly gets applyed)
                new Decorator(ret => Me.IsCasting && Me.CastingSpell == SpellManager.Spells["Polymorph"] && getAdds2().Count == 0,
                    new Action(ret => SpellManager.StopCasting())),


                new Decorator(ret => IsBattleGround() && Me.CurrentTarget == null,
                    new Action(ret => FindClostestPlayer(200))),

                new Decorator(ret => IsBattleGround() && Me.CurrentTarget.Dead,
                    new Action(ret => Me.ClearTarget())),

               new Decorator(ret => SpellManager.HasSpell("Ice Block") && Me.HealthPercent < AmplifySettings.Instance.IceBlock_hP_Percent && AmplifySettings.Instance.Use_IceBlock,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Ice Block"))),

                //Fingers of Frost Proc - IceLance
               new Decorator(ret => Me.CurrentTarget != null && Me.CurrentTarget.CreatureType == WoWCreatureType.Totem,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Ice Lance")
                        )),

              //Summon Water Elemental if not out, not on cooldown, and if spell is learned. 
             new Decorator(ret => !Me.Mounted && !Me.GotAlivePet && SpellManager.HasSpell("Summon Water Elemental") && SpellManager.CanCast("Summon Water Elemental"),
                   new PrioritySelector(CreateSpellCheckAndCast("Summon Water Elemental"))),

                //Step Through Pet Logic.
               Freeze(),

                //Step Through AoE Logic - Blizzard and Flamestrike.
               AOE(),

                //Cast ColdSnap if 2 or more Frost Abilitys are on cooldown.
                new Decorator(ret => AmplifySettings.Instance.Use_ColdSnap && ColdSnapCheck() && SpellManager.HasSpell("Cold Snap") && SpellManager.CanCast("Cold Snap"),
                  new Action(ret => SpellManager.Cast("Cold Snap"))),

                //ManaGem Logic
               new Decorator(ret => AmplifySettings.Instance.Use_ManaGems && HaveManaGem() && Me.ManaPercent <= AmplifySettings.Instance.ManaGems_MP_Percent,
                   new Action(ctx => UseManaGem())),

               new Decorator(ret => AmplifySettings.Instance.Use_Polymorph && NeedToSheep() && (!SheepTimer.IsRunning || SheepTimer.Elapsed.Seconds > 5),
                   new Action(ctx => SheepLogic())),
                /*
                               new Decorator(ret => Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= AmplifySettings.Instance.FrostNova_Mob_Hp_Above && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Ring of Frost") && getAdds().Count >= 2,
                                   new Sequence(
                                       new Action(ret => SpellManager.Cast("Ring of Frost")),
                                       new Action(ret => Thread.Sleep(600)),
                                       new Action(ret => LegacySpellManager.ClickRemoteLocation(Me.Location)),
                                       new Action(ctx => BlinkBack())
                                           )),
                */

               new Decorator(ret => !AmplifySettings.Instance.MoveDisable && Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= 30 && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Frost Nova") && AmplifySettings.Instance.FrostNova != "Off" && AmplifySettings.Instance.FrostNova == "Mr.Navigator" && AmplifySettings.Instance.Use_FrostNova,
                   new Sequence(
                       new Action(ret => SpellManager.Cast("Frost Nova")),
                        new ActionMoveToPoint(NavMe)
                           )),

               new Decorator(ret => !AmplifySettings.Instance.MoveDisable && Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= 30 && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Frost Nova") && AmplifySettings.Instance.FrostNova != "Off" && AmplifySettings.Instance.Use_FrostNova,

                       //new NavigationAction(ret => FindSuperSafeSpot()),
                       new Action(ret => FrostNova())
                           ),

             new Decorator(ret => Me.CurrentTarget != null && AmplifySettings.Instance.Use_CounterSpell && SpellManager.HasSpell("Counterspell") && SpellManager.CanCast("Counterspell", Me.CurrentTarget, true, true) && Me.CurrentTarget.IsCasting,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Counterspell"))),

                new Decorator(ret => AmplifySettings.Instance.Spellsteal == "On" && SpellManager.HasSpell("Spellsteal") && SpellManager.CanCast("Spellsteal") && SpellToSteal(),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Spellsteal"))),

               new Decorator(ret => SpellManager.CanCast("Icy Veins") && (getAdds().Count > AmplifySettings.Instance.IcyVeinswhenXAdds || Me.IsInInstance && IsInPartyOrRaid()) && AmplifySettings.Instance.IcyVeinSettings == "OnlyOnAdds" && Me.CurrentTarget.Distance < 30,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Icy Veins"))),

               new Decorator(ret => SpellManager.CanCast("Icy Veins") && AmplifySettings.Instance.IcyVeinSettings == "Always" && Me.CurrentTarget.HealthPercent > 20,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Icy Veins"))),

               new Decorator(ret => SpellManager.CanCast("Mirror Image") && (getAdds().Count > AmplifySettings.Instance.MirrorImagewhenXAdds || Me.IsInInstance && IsInPartyOrRaid()) && AmplifySettings.Instance.MirrorImage == "OnlyOnAdds" && Me.CurrentTarget.Distance < 30,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mirror Image"))),

               new Decorator(ret => SpellManager.CanCast("Mirror Image") && AmplifySettings.Instance.MirrorImage == "Always" && Me.CurrentTarget.HealthPercent > 40,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mirror Image"))),

               new Decorator(ret => SpellManager.CanCast("Time Warp") && AmplifySettings.Instance.TimeWarp == "Always" && !Me.ActiveAuras.ContainsKey("Temporal Displacement") && !Me.ActiveAuras.ContainsKey("Bloodlust") && !Me.ActiveAuras.ContainsKey("Heroism") && Me.CurrentTarget.HealthPercent > 20,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Time Warp"))),

               new Decorator(ret => SpellManager.CanCast("Flame Orb") && AmplifySettings.Instance.FlameOrbSelection == "Always" && Me.CurrentTarget.HealthPercent > 50,
                   new Sequence(
                       new Action(ctx => Me.CurrentTarget.Face()),
                       new Action(ctx => Thread.Sleep(650)),
                       new PrioritySelector(CreateSpellCheckAndCast("Flame Orb"))
                           )),

               new Decorator(ret => SpellManager.CanCast("Flame Orb") && AmplifySettings.Instance.FlameOrbSelection == "OnlyOnAdds" && getAdds().Count > 2 && Me.CurrentTarget.HealthPercent > 20,
                   new Sequence(
                       new Action(ctx => Me.CurrentTarget.Face()),
                       new PrioritySelector(CreateSpellCheckAndCast("Flame Orb"))
                           )),

               new Decorator(ret => SpellManager.CanCast("Mirror Image") && AmplifySettings.Instance.Use_MirrorImage && getAdds().Count > AmplifySettings.Instance.MirrorImagewhenXAdds && Me.CurrentTarget.Distance < 30,
                   new PrioritySelector(
                       CreateBuffCheckAndCast("Mirror Image"))),

                //IceBarrier Logic use when below 50% Health && ManaShield is not Currently on Me.
             new Decorator(ret => !Me.Auras.ContainsKey("Ice Barrier") && AmplifySettings.Instance.Use_IceBarrier && Me.HealthPercent <= AmplifySettings.Instance.IceBarrierWhenBelow_Hp_Percent && SpellManager.HasSpell("Ice Barrier") && SpellManager.CanCast("Ice Barrier") && !Me.Auras.ContainsKey("Mana Shield"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Ice Barrier"))),

             new Decorator(ret => AmplifySettings.Instance.Use_ManaShield && !Me.Auras.ContainsKey("Mana Shield") && Me.HealthPercent <= AmplifySettings.Instance.ManaShield_Hp_Percent && SpellManager.HasSpell("Mana Shield") && SpellManager.CanCast("Mana Shield"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Mana Shield"))),

            new Decorator(ret => SpellManager.HasSpell("Evocation") && AmplifySettings.Instance.Use_Evocation && SpellManager.CanCast("Evocation") && (Me.ManaPercent < AmplifySettings.Instance.Evocation_MP_Percent || Me.HealthPercent < AmplifySettings.Instance.Evocation_HP_Percent) && (!SpellManager.HasSpell("Mana Shield") || Me.Auras.ContainsKey("Mana Shield") || Me.Auras.ContainsKey("Ice Barrier")),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Evocation"))),

                //Fingers of Frost Proc - DeepFreeze
             new Decorator(ret => SpellManager.CanCast("Deep Freeze") && (Me.ActiveAuras.ContainsKey("Fingers of Frost") || Me.CurrentTarget.HasAura("Frost Nova") || Me.CurrentTarget.HasAura("Freeze")),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Deep Freeze")
                        )),
                //Fingers of Frost Proc - IceLance
             new Decorator(ret => (Me.ActiveAuras.ContainsKey("Fingers of Frost") || Me.CurrentTarget.ActiveAuras.ContainsKey("Frost Nova") || Me.CurrentTarget.ActiveAuras.ContainsKey("Freeze")),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Ice Lance")
                        )),

                //Brain Freeze Proc - FireBall
             new Decorator(ret => SpellManager.CanCast("Fireball") && AmplifySettings.Instance.Use_FireballOnBF && Me.ActiveAuras.ContainsKey("Brain Freeze"),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fireball")
                        )),
                //Brain Freeze Proc - FrostFirebolt 
             new Decorator(ret => SpellManager.CanCast("Frostfire Bolt") && AmplifySettings.Instance.Use_FrostFireboltOnBF && Me.ActiveAuras.ContainsKey("Brain Freeze"),
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Frostfire Bolt")
                        )),
                //Arcane Missles if Proc
             new Decorator(ret => SpellManager.CanCast("Arcane Missiles") && Me.Auras.ContainsKey("Arcane Missiles!") && AmplifySettings.Instance.Use_ProcArcaneMissles,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                        )),

               new Decorator(ret => SpellManager.CanCast(AmplifySettings.Instance.FrostFinisher) && Me.CurrentTarget.HealthPercent <= AmplifySettings.Instance.FireBlast_Hp_Percent && Me.CurrentTarget.Distance < SpellManager.Spells[AmplifySettings.Instance.FrostFinisher].MaxRange - 2,
                    new PrioritySelector(
                        CreateSpellCheckAndCast(AmplifySettings.Instance.FrostFinisher)
                        )),

               new Decorator(ret => SpellManager.CanCast(CurrentSpamSpell),
                    new PrioritySelector(
                        CreateSpellCheckAndCast(CurrentSpamSpell)
                        )),
               new Decorator(ret => SpellManager.CanCast("Fireball") && AmplifySettings.Instance.Use_Fireball_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fireball")
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

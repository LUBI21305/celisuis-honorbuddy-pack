using System;
using System.Collections.Generic;
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
        private Composite _combatBehavior;
        public override Composite CombatBehavior
        {
            get
            {
                if (_combatBehavior == null)
                {
                    Log("Creating 'Combat' behavior");
                    _combatBehavior = CreateCombatBehavior();
                }

                return _combatBehavior;
            }
        }

        /// <summary>s
        /// Creates the behavior used for combat. Castsequences, add management, crowd control etc.
        /// </summary>
        /// <returns></returns>
        private Composite CreateCombatBehavior()
        {
            return new PrioritySelector(

                    CreateCombat()
                );
        }


        private Composite CreateCombat()
        {

            return new PrioritySelector(
                //Retarget Polymorphed add if we dont have a current target.
                new Decorator(ret => !AmplifySettings.Instance.MoveDisable && !IsInPartyOrRaid() && GotSheep && (Me.CurrentTarget == null || Me.CurrentTarget.Dead),
                              new Action(ctx => retargetSheep())),

                //Retarget Totem if no other unit is in play.
                new Decorator(ret => !AmplifySettings.Instance.MoveDisable && !IsInPartyOrRaid() && !GotSheep && (Me.CurrentTarget == null || Me.CurrentTarget.Dead),
                              new Action(ctx => retargetTotem())),

                //Added to make sure we have a target when movement is disabled before spamming my pull sqeuence
                new Decorator(ret => AmplifySettings.Instance.MoveDisable && !Me.GotTarget && !Me.CurrentTarget.IsFriendly && !Me.CurrentTarget.Dead && Me.CurrentTarget.Attackable,
                    new Action(ctx => RunStatus.Success)),

                //If we have an active pet, make it attack the same target we are.
               new Decorator(ret => Me.CurrentTarget != null && !Me.CurrentTarget.Dead && Me.GotTarget && Me.GotAlivePet && (!Me.Pet.GotTarget || Me.Pet.CurrentTarget != Me.CurrentTarget),
                    new Action(ret => Lua.DoString("PetAttack()"))),


                // Face thehe tart if we aren't
                new Decorator(ret => !AmplifySettings.Instance.MoveDisable && Me.GotTarget && !Me.IsFacing(Me.CurrentTarget),
                              new Action(ret => Me.CurrentTarget.Face())),

                new Decorator(ret => Me.IsCasting || Me.Silenced || Me.ActiveAuras.ContainsKey("Hypothermia"),
                    new Action(ctx => RunStatus.Success)),

                new Decorator(ret => Me.ActiveAuras.ContainsKey("Ice Block"),
                   new Action(ctx => RunStatus.Success)),
                // if Health Low, Use gift of the Narru
                new Decorator(ret => Me.HealthPercent <= 40 && SpellManager.CanCast("Gift of the Naaru"),
                              new PrioritySelector(CreateSpellCheckAndCast("Gift of the Naaru"))),
                // Use Potions
                new Decorator(ret => Me.HealthPercent <= AmplifySettings.Instance.HealthPotPercent && HaveHealthPotion() && HealthPotionReady(),
                              new Action(ret => UseHealthPotion())),
                              
                new Decorator(ret => Me.ManaPercent <= AmplifySettings.Instance.ManaPotPercent && HaveManaPotion() && ManaPotionReady(),
                              new Action(ret => UseManaPotion())),

                new Decorator(ret => Me.ManaPercent <= 20 && HaveManaGem(),
                              new Action(ret => UseManaGem())),



                // Move closer to the target if we are too far away or in !Los
                new Decorator(ret => !AmplifySettings.Instance.MoveDisable && Me.GotTarget && (Me.CurrentTarget.Distance > PullDistance + 3 || !Me.CurrentTarget.InLineOfSight),
                    new NavigationAction(ret => Me.CurrentTarget.Location)),


                // At this point we shouldn't be moving. Atleast not with this 'simple' kind of logic
                new Decorator(ret => !AmplifySettings.Instance.MoveDisable && Me.IsMoving,
                              new Action(ret => WoWMovement.MoveStop())),

                //new Decorator(ret => Me.GotAlivePet && Me.Pet.GotTarget && Me.Pet.CurrentTarget != Me.CurrentTarget,
                             // new Action(ret => WoWMovement.MoveStop())),




            new Switch<MageTalentSpec>(r => _talentManager.Spec, 
                                   new SwitchArgument<MageTalentSpec>(Low_ChecksAndRotation(),MageTalentSpec.Lowbie),
                                   new SwitchArgument<MageTalentSpec>(Frost_ChecksAndRotation(), MageTalentSpec.Frost),
                                   new SwitchArgument<MageTalentSpec>(Fire_ChecksAndRotation(), MageTalentSpec.Fire),
                                   new SwitchArgument<MageTalentSpec>(Arcane_ChecksAndRotation(), MageTalentSpec.Arcane))
                                    );
  

        }




        #region GetAllChecks
        /* The following composite was written by No1KnowsY (Panda)
         * The changes to the tree to use this were also implemented
         * Frost and Arcane were tested directly after coding on TestDummies (Frost did better DPS due to gearing, and lack of development in Arcane)
         * Arcane will be worked on soon
         * Fire may follow afterwards
         * 
         * Added in version 1.8.3 on 11/11/2011 (lots of 11's)
         * Enjoy :)
         */
        private Composite GetAllChecks(string specTree, bool universal)
        {
            return new Decorator(
                new Action(delegate
                {

                    using (new FrameLock())
                    {
                        ObjectManager.Update();
                        if (universal)
                        {
                            _FindClosestPlayer = false;
                            if (IsBattleGround() && Me.CurrentTarget == null) { _FindClosestPlayer = true; }
                            _ClearTarget = false;
                            if (IsBattleGround() && Me.CurrentTarget.Dead) { _ClearTarget = true; }
                            _UseManaGem = false;
                            if (AmplifySettings.Instance.Use_ManaGems && HaveManaGem() && Me.ManaPercent <= AmplifySettings.Instance.ManaGems_MP_Percent) { _UseManaGem = true; }
                            _SheepLogic = false;
                            if (AmplifySettings.Instance.Use_Polymorph && NeedToSheep() && (!SheepTimer.IsRunning || SheepTimer.Elapsed.Seconds > 5)) { _SheepLogic = true; }
                            _FrostNova = false;
                            if (!AmplifySettings.Instance.MoveDisable && Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= 30 && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Frost Nova") && AmplifySettings.Instance.FrostNova != "Off" && AmplifySettings.Instance.Use_FrostNova) { _FrostNova = true; }
                            _Counterspell = false;
                            if (Me.CurrentTarget != null && AmplifySettings.Instance.Use_CounterSpell && SpellManager.HasSpell("Counterspell") && SpellManager.CanCast("Counterspell", Me.CurrentTarget, true, true) && Me.CurrentTarget.IsCasting) { _Counterspell = true; }
                            _SpellSteal = false;
                            if (AmplifySettings.Instance.Spellsteal == "On" && SpellManager.HasSpell("Spellsteal") && SpellManager.CanCast("Spellsteal") && SpellToSteal()) { _SpellSteal = true; }
                            _ManaShield = false;
                            if (AmplifySettings.Instance.ManaShieldArcane == "Always" && !Me.Auras.ContainsKey("Mana Shield") && Me.HealthPercent <= AmplifySettings.Instance.ManaShield_Hp_Percent && SpellManager.HasSpell("Mana Shield") && SpellManager.CanCast("Mana Shield")) { _ManaShield = true; }
                            _Evocation = false;
                            if (SpellManager.HasSpell("Evocation") && AmplifySettings.Instance.Use_Evocation && SpellManager.CanCast("Evocation") && (Me.ManaPercent < AmplifySettings.Instance.Evocation_MP_Percent || Me.HealthPercent < AmplifySettings.Instance.Evocation_HP_Percent) && (!SpellManager.HasSpell("Mana Shield") || Me.Auras.ContainsKey("Mana Shield") || Me.Auras.ContainsKey("Ice Barrier"))) { _Evocation = true; }
                            _Shoot = false;
                            if (SpellManager.CanCast("Shoot") && IsNotWanding && AmplifySettings.Instance.Use_Wand) { _Shoot = true; }
                            //Log("Made it through Universal GetAllChecks");

                        }
                        switch (specTree)
                        {
                            case "Arcane":
                                {
                                    _PresenceOfMind = false;
                                    if (AmplifySettings.Instance.PresenceofMind == "Always" && SpellManager.CanCast("Presence of Mind")) { _PresenceOfMind = true; }
                                    _FireBall = false;
                                    if (!SpellManager.HasSpell("Frostbolt")) { _FireBall = true; }
                                    _ArcaneMissiles = false;
                                    if (SpellManager.CanCast("Arcane Missiles") && SpellManager.HasSpell("Arcane Blast") && Me.HasAura("Arcane Blast") && AmplifySettings.Instance.Use_ProcArcaneMissles && Me.ActiveAuras["Arcane Blast"].StackCount >= AmplifySettings.Instance.ArcaneBlastStacks) { _ArcaneMissiles = true; }
                                    _ArcaneM_Level20 = false;
                                    if (SpellManager.CanCast("Arcane Missiles") && !SpellManager.HasSpell("Arcane Blast") && AmplifySettings.Instance.Use_ProcArcaneMissles) { _ArcaneM_Level20 = true; }
                                    _ArcaneBarrage = false;
                                    if (SpellManager.CanCast("Arcane Barrage") && Me.HasAura("Arcane Blast") && Me.ActiveAuras["Arcane Blast"].StackCount >= AmplifySettings.Instance.ArcaneBlastStacks) { _ArcaneBarrage = true; }
                                    _ArcaneBlast = false;
                                    if (SpellManager.CanCast("Arcane Blast")) { _ArcaneBlast = true; }
                                    _FrostBolt = false;
                                    if (SpellManager.CanCast("Frostbolt") && !SpellManager.CanCast("Arcane Blast")) { _FrostBolt = true; }
                                    //Log("Made it through Arcane GetAllChecks");
                                }
                                break;


                            case "Frost":
                                {
                                    _FrostFireFixTry = false;
                                    if (!AmplifySettings.Instance.MoveDisable && Me.GotTarget && (Me.CurrentTarget.Distance > SpellManager.Spells[AmplifySettings.Instance.FrostSpamSpell].MaxRange - 1 || !Me.CurrentTarget.InLineOfSight)) { _FrostFireFixTry = true;}
                                    _FixPolyAddDies = false;
                                    if (Me.IsCasting && Me.CastingSpell == SpellManager.Spells["Polymorph"] && getAdds2().Count == 0) { _FixPolyAddDies = true; }
                                    _IceBlock = false;
                                    if (SpellManager.HasSpell("Ice Block") && Me.HealthPercent < AmplifySettings.Instance.IceBlock_hP_Percent && AmplifySettings.Instance.Use_IceBlock) { _IceBlock = true; }
                                    _IceLance = false;
                                    if (Me.CurrentTarget != null && Me.CurrentTarget.CreatureType == WoWCreatureType.Totem) { _IceLance = true; }
                                    _SummonWaterElemental = false;
                                    if (!Me.Mounted && !Me.GotAlivePet && SpellManager.HasSpell("Summon Water Elemental") && SpellManager.CanCast("Summon Water Elemental")) { _SummonWaterElemental = true; }
                                    _ColdSnap = false;
                                    if (AmplifySettings.Instance.Use_ColdSnap && ColdSnapCheck() && SpellManager.HasSpell("Cold Snap") && SpellManager.CanCast("Cold Snap")) { _ColdSnap = true; }
                                    _FrostNovaNavigated = false;
                                    if (!AmplifySettings.Instance.MoveDisable && Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= 30 && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Frost Nova") && AmplifySettings.Instance.FrostNova != "Off" && AmplifySettings.Instance.FrostNova == "Mr.Navigator" && AmplifySettings.Instance.Use_FrostNova) { _FrostNovaNavigated = true; }
                                    _IcyVeinAdds = false;
                                    if (SpellManager.CanCast("Icy Veins") && (getAdds().Count > AmplifySettings.Instance.IcyVeinswhenXAdds || Me.IsInInstance && IsInPartyOrRaid()) && AmplifySettings.Instance.IcyVeinSettings == "OnlyOnAdds" && Me.CurrentTarget.Distance < 30) { _IcyVeinAdds = true; }
                                    _IcyVeinAlways = false;
                                    if (SpellManager.CanCast("Icy Veins") && AmplifySettings.Instance.IcyVeinSettings == "Always" && Me.CurrentTarget.HealthPercent > 20) { _IcyVeinAlways = true; }
                                    _MirrorImageAdds = false;
                                    if (SpellManager.CanCast("Mirror Image") && (getAdds().Count > AmplifySettings.Instance.MirrorImagewhenXAdds || Me.IsInInstance && IsInPartyOrRaid()) && AmplifySettings.Instance.MirrorImage == "OnlyOnAdds" && Me.CurrentTarget.Distance < 30) { _MirrorImageAdds = true; }
                                    _MirrorImageAlways = false;
                                    if (SpellManager.CanCast("Mirror Image") && AmplifySettings.Instance.MirrorImage == "Always" && Me.CurrentTarget.HealthPercent > 40) { _MirrorImageAlways = true; }
                                    _TimeWarp = false;
                                    if (SpellManager.CanCast("Time Warp") && AmplifySettings.Instance.TimeWarp == "Always" && !Me.ActiveAuras.ContainsKey("Temporal Displacement") && !Me.ActiveAuras.ContainsKey("Bloodlust") && !Me.ActiveAuras.ContainsKey("Heroism") && Me.CurrentTarget.HealthPercent > 20) { _TimeWarp = true; }
                                    _FlameOrbAlways = false;
                                    if (SpellManager.CanCast("Flame Orb") && AmplifySettings.Instance.FlameOrbSelection == "Always" && Me.CurrentTarget.HealthPercent > 50) { _FlameOrbAlways = true; }
                                    _FlameOrbAdds = false;
                                    if (SpellManager.CanCast("Flame Orb") && AmplifySettings.Instance.FlameOrbSelection == "OnlyOnAdds" && getAdds().Count > 2 && Me.CurrentTarget.HealthPercent > 20) { _FlameOrbAdds = true; }
                                    _IceBarrier = false;
                                    if (!Me.Auras.ContainsKey("Ice Barrier") && AmplifySettings.Instance.Use_IceBarrier && Me.HealthPercent <= AmplifySettings.Instance.IceBarrierWhenBelow_Hp_Percent && SpellManager.HasSpell("Ice Barrier") && SpellManager.CanCast("Ice Barrier") && !Me.Auras.ContainsKey("Mana Shield")) { _IceBarrier = true; }
                                    _DeepFreeze_FoF = false;
                                    if (SpellManager.CanCast("Deep Freeze") && (Me.ActiveAuras.ContainsKey("Fingers of Frost") || Me.CurrentTarget.HasAura("Frost Nova") || Me.CurrentTarget.HasAura("Freeze"))) { _DeepFreeze_FoF = true; }
                                    _IceLance_FoF = false;
                                    if ((Me.ActiveAuras.ContainsKey("Fingers of Frost") || Me.CurrentTarget.ActiveAuras.ContainsKey("Frost Nova") || Me.CurrentTarget.ActiveAuras.ContainsKey("Freeze"))) { _IceLance_FoF = true; }
                                    _FireBall_BF = false;
                                    if (SpellManager.CanCast("Fireball") && AmplifySettings.Instance.Use_FireballOnBF && Me.ActiveAuras.ContainsKey("Brain Freeze")) { _FireBall_BF = true; }
                                    _FrostFire_BF = false;
                                    if (SpellManager.CanCast("Frostfire Bolt") && AmplifySettings.Instance.Use_FrostFireboltOnBF && Me.ActiveAuras.ContainsKey("Brain Freeze")) { _FrostFire_BF = true; }
                                    _ArcaneMissiles_Proc = false;
                                    if (SpellManager.CanCast("Arcane Missiles") && Me.Auras.ContainsKey("Arcane Missiles!") && AmplifySettings.Instance.Use_ProcArcaneMissles) { _ArcaneMissiles = true; }
                                    _FrostFinisher = false;
                                    if (SpellManager.CanCast(AmplifySettings.Instance.FrostFinisher) && Me.CurrentTarget.HealthPercent <= AmplifySettings.Instance.FireBlast_Hp_Percent && Me.CurrentTarget.Distance < SpellManager.Spells[AmplifySettings.Instance.FrostFinisher].MaxRange - 2) { _FrostFinisher = true; }
                                    _CurrentSpamSpell = false;
                                    if (SpellManager.CanCast(CurrentSpamSpell)) { _CurrentSpamSpell = true; }
                                    _FireBall = false;
                                    if (SpellManager.CanCast("Fireball") && AmplifySettings.Instance.Use_Fireball_Low) { _FireBall = true; }
                                    _Shoot = false;
                                    if (SpellManager.CanCast("Shoot") && IsNotWanding && AmplifySettings.Instance.Use_Wand) { _Shoot = true; }
                                    //Log("Made it through Frost GetAllChecks");
                                }
                                break;


                            case "Fire":
                                {
                                    _Scorch = false;
                                    if (AmplifySettings.Instance.CriticalMass && !Me.CurrentTarget.ActiveAuras.ContainsKey("Critical Mass") && SpellManager.CanCast("Scorch")) { _Scorch = true; }
                                    _LivingBomb = false;
                                    if (AmplifySettings.Instance.LivingBomb && !Me.CurrentTarget.ActiveAuras.ContainsKey("Living Bomb") && SpellManager.CanCast("Living Bomb")) { _LivingBomb = true; }
                                    _Combustion = false;
                                    if (Me.CurrentTarget.ActiveAuras.ContainsKey("Living Bomb") && Me.CurrentTarget.ActiveAuras.ContainsKey("Ignite") && Me.CurrentTarget.ActiveAuras.ContainsKey("Pyroblast!") && SpellManager.CanCast("Combustion")) { _Combustion = true; }
                                    _Fireball = false;
                                    if (!SpellManager.HasSpell("Frostbolt")) { _Fireball = true; }
                                    _FireBlast = false;
                                    if (SpellManager.CanCast("Fire Blast") && Me.CurrentTarget.Distance < SpellManager.Spells["Fire Blast"].MaxRange - 2 && Me.ActiveAuras.ContainsKey("Impact")) { _FireBlast = true; }
                                    _Pyroblast = false;
                                    if (SpellManager.CanCast("Pyroblast") && Me.ActiveAuras.ContainsKey("Hot Streak")) { _Pyroblast = true; }
                                    _FireSpamSpell = false;
                                    if (SpellManager.CanCast(FireSpamSpell)) { _FireSpamSpell = true; }
                                    _Frostbolt = false;
                                    if (SpellManager.CanCast("Frostbolt") && !SpellManager.CanCast("Fireball")) { _Frostbolt = true; }
                                    //Log("Made it through Fire GetAllChecks");
                                }
                                break;


                            case "LowLevel":
                                {
                                    _ArcaneMissiles_Low = false;
                                    if (SpellManager.CanCast("Arcane Missiles") && Me.Auras.ContainsKey("Arcane Missiles!") && AmplifySettings.Instance.Use_ProcArcaneMissles_Low) { _ArcaneMissiles_Low = true; }
                                    _Fireball_Low_Safety = false;
                                    if (!SpellManager.HasSpell("Frostbolt")) { _Fireball_Low_Safety = true; }
                                    _FireBlast_Low = false;
                                    if (SpellManager.CanCast("Fire Blast") && Me.CurrentTarget.HealthPercent <= AmplifySettings.Instance.FireBlast_Hp_Percent && AmplifySettings.Instance.Use_FireBlast) { _FireBlast_Low = true; }
                                    _FrostBolt_Low = false;
                                    if (SpellManager.CanCast("Frostbolt") && AmplifySettings.Instance.Use_Frostbolt_Low) { _FrostBolt_Low = true; }
                                    _FireBall_Low = false;
                                    if (SpellManager.CanCast("Fireball") && AmplifySettings.Instance.Use_Fireball_Low) { _FireBall_Low = true; }
                                    //Log("Made it through LowLevel GetAllChecks");
                                }
                                break;
                        }

                    } return (RunStatus.Success);
                }));

        }
        /*It is pretty obvious what this is doing
         * 
         * You could potentially break this into 2 or more composites,
         * and also add new nodes into the Sequence above
         * 
         * string tells the Switch which Case to use
         * Bool tells it whether to use the universal checks or not.  This could be useful in re-using this Framelock()
         * Could use this composite for basically anything you want to be within a framelock 
         * ...since it has the switch : case
         * 
         * Set the bool to true when calling if you are doing a spell rotation, as (frost/arcane/fire).cs all use those
         */

        #endregion


    }
}

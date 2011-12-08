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
        #region LowLevel Bools
        private bool _ArcaneMissiles_Low;
        private bool _Fireball_Low_Safety;
        private bool _FireBlast_Low;
        private bool _FrostBolt_Low;
        private bool _FireBall_Low;
        #endregion

        private Composite Low_ChecksAndRotation()
        {
            return new Sequence(
                new Decorator(GetAllChecks("LowLevel", false)),
                //Universal checks are not needed in this tree.  None of the additional spells are used, <false>

                new Decorator(
                    new PrioritySelector(Low_rotation()))

                    );
        }

        private Composite Low_rotation()
        {
            return new PrioritySelector(

               new Decorator(ret => _FrostNova,
                   new Sequence(
                       new Action(ret => SpellManager.Cast("Frost Nova")),
                       new Action(ret => FrostNova())
                           )),

      //Arcane Missles if Proc
             new Decorator(ret => _ArcaneMissiles_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                     )),

               new Decorator(ret => _Fireball_Low_Safety,
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Fireball"))),

               new Decorator(ret => _FireBlast_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fire Blast")
                        )),

               new Decorator(ret => _FrostBolt_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Frostbolt")
                        )),

               new Decorator(ret => _FireBall_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fireball")
                        ))
           
                        );
        }


        #region Low Level Old
        private Composite Low_rotationOLD()
        {
            return new PrioritySelector(

               new Decorator(ret => Me.CurrentTarget != null && !HasSheeped() && Me.CurrentTarget.HealthPercent >= AmplifySettings.Instance.FrostNova_Mob_Hp_Above && Me.CurrentTarget.Distance < 7 && SpellManager.CanCast("Frost Nova") && AmplifySettings.Instance.FrostNova != "Off" && AmplifySettings.Instance.Use_FrostNova,
                   new Sequence(
                       new Action(ret => SpellManager.Cast("Frost Nova")),
                       new Action(ret => FrostNova())
                           )),
                //Arcane Missles if Proc
             new Decorator(ret => SpellManager.CanCast("Arcane Missiles") && Me.Auras.ContainsKey("Arcane Missiles!") && AmplifySettings.Instance.Use_ProcArcaneMissles_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Arcane Missiles")
                     )),

               new Decorator(ret => !SpellManager.HasSpell("Frostbolt"),
                   new PrioritySelector(
                       CreateSpellCheckAndCast("Fireball"))),

               new Decorator(ret => SpellManager.CanCast("Fire Blast") && Me.CurrentTarget.HealthPercent <= AmplifySettings.Instance.FireBlast_Hp_Percent && AmplifySettings.Instance.Use_FireBlast,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fire Blast")
                        )),
               new Decorator(ret => SpellManager.CanCast("Frostbolt") && AmplifySettings.Instance.Use_Frostbolt_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Frostbolt")
                        )),
               new Decorator(ret => SpellManager.CanCast("Fireball") && AmplifySettings.Instance.Use_Fireball_Low,
                    new PrioritySelector(
                        CreateSpellCheckAndCast("Fireball")
                        ))

                        );
        }
        #endregion

    }
}

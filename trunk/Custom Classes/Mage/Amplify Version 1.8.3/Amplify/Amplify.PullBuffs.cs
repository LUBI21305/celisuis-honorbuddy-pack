using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = System.Action;
using Color = System.Drawing.Color;
using Sequence = Styx.Logic;
using Styx.WoWInternals.World;

namespace Amplify
{
    public partial class Amplify
    {
        private Composite _pullBuffBehavior;
        public override Composite PullBuffBehavior
        {
            get
            {
                if (_pullBuffBehavior == null)
                {
                    Log("Creating 'PullBuff' behavior");
                    _pullBuffBehavior = CreatePullBuffBehavior();
                }

                return _pullBuffBehavior;
            }
        }

        /// <summary>
        /// Creates the behavior used for pulling mobs
        /// </summary>
        /// <returns></returns>
        private Composite CreatePullBuffBehavior()
        {
            return new PrioritySelector(

            new Decorator(ret => !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport && SpellManager.HasSpell("Summon Water Elemental") && !Me.GotAlivePet,
                new PrioritySelector(
                    CreateBuffCheckAndCast("Summon Water Elemental")
                                  )),
            new Decorator(ret => !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport && !Me.Auras.ContainsKey("Ice Barrier") && AmplifySettings.Instance.IceBarrierBeforePull &&
                SpellManager.HasSpell("Ice Barrier") && SpellManager.CanCast("Ice Barrier"),
                new PrioritySelector(
                    CreateSpellCheckAndCast("Ice Barrier")))
                    );

        }
    }
}

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
        public Composite Freeze()
        {
            return new Decorator(ret => Me.CurrentTarget != null && Me.GotAlivePet && AmplifySettings.Instance.Freeze && Me.CurrentTarget.Distance > 10 && Me.CurrentTarget.HealthPercent >= 20 && PetActionReady && !Me.CurrentTarget.HasAura("Frost Nova") && !WillChain(0, Me.CurrentTarget.Location),
                                  new Sequence(
                                  new Action(ret => Log("Casting Freeze")),
                                  new Action(ret => Lua.DoString("CastPetAction(4)")),
                                  new Action(ret => FreezeTimer.Reset()),
                                  new Action(ret => FreezeTimer.Start()),
                                  new Action(ret => Thread.Sleep(300)),
                                  new Action(ret => LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location)))
                                );
                                    
              
        }
        public Composite AOE()
        {
            return new Decorator(ret => Me.CurrentTarget != null && (SpellManager.CanCast("Blizzard") || SpellManager.CanCast("Flamestrike")) && AmplifySettings.Instance.AoE != "No AoE" && getAdds2().Count >= AmplifySettings.Instance.BlizzardwhenXAdds && Me.CurrentTarget.Distance < 29,
                     new Switch<string>(r => AmplifySettings.Instance.AoE,
                                   new SwitchArgument<string>(Blizzard(), "Blizzard"),
                                   new SwitchArgument<string>(FlameStrike(), "Flamestrike"))

                );
        }
        public Composite Blizzard()
        {

        return new Decorator(ret => SpellManager.CanCast("Blizzard"),
             new Sequence(
                        new Action(ret => Log("Casting Blizzard")),
                        new Action(ret => SpellManager.Cast("Blizzard")),
                        new Action(ret => Thread.Sleep(300)),
                        new Action(ret => LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location)))
                        
                        //new WaitContinue(5, ret => Me.CurrentPendingCursorSpell.Name == "Blizzard",
                                //new Action(ret => LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location)))
);
        }
        public Composite FlameStrike()
        {

            return new Decorator(ret => SpellManager.CanCast("Flamestrike"),
                 new Sequence(
                            new Action(ret => Log("Casting Flamestrike")),
                            new Action(ret => SpellManager.Cast("Flamestrike")),
                            new Action(ret => Thread.Sleep(300)),
                            new Action(ret => LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location)))
                       
                            //new WaitContinue(5, ret => Me.CurrentPendingCursorSpell.Name == "Flamestrike",
                                    //new Action(ret => LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location)))
                                    
                                    );
        }
    }
}

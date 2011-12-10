using Styx.Logic.Combat;
using TreeSharp;

namespace Amplify
{
    public partial class Amplify
    {
        private Composite _preCombatBuffBehavior;
        public override Composite PreCombatBuffBehavior
        {
            get
            {
                if (_preCombatBuffBehavior == null)
                {
                    Log("Creating 'PreCombatBuff' behavior");
                    _preCombatBuffBehavior = CreatePreCombatBuffsBehavior();
                }

                return _preCombatBuffBehavior;
            }
        }


        private Composite CreatePreCombatBuffsBehavior()
        {
            return new PrioritySelector(

                new Decorator(ret => !Me.IsFlying && !Me.IsOnTransport && !Me.Mounted && SpellManager.HasSpell("Arcane Brilliance") && !Me.Auras.ContainsKey("Fel Intelligence") && !Me.Auras.ContainsKey("Arcane Brilliance") && !Me.Auras.ContainsKey("Dalaran Brilliance") && !Me.Auras.ContainsKey("Wisdom of Agamaggan"),
                              new PrioritySelector(
                                  CreateBuffCheckAndCast("Arcane Brilliance")
                                  )),

                new Decorator(ret => !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport && Me.IsInParty && PartyNeedInt() && AmplifySettings.Instance.BuffParty,
                              new PrioritySelector(
                                  Cast("Arcane Brilliance")
                                  )),
               new Decorator(ret => !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport  && !Me.Combat && PartyHaveCurse() && AmplifySettings.Instance.DeCurseParty,
                   new Action(ctx => PartyDeCurse())),


                new Decorator(ret => !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport && SpellManager.HasSpell("Molten Armor") && !Me.Mounted && !SpellManager.HasSpell("Mage Armor") && !SpellManager.HasSpell("Frost Armor") && AmplifySettings.Instance.ArmorSelect == "Auto" || AmplifySettings.Instance.ArmorSelect == "Molten" && !Me.Mounted && SpellManager.HasSpell("Molten Armor"),
                              new PrioritySelector(
                                  CreateBuffCheckAndCast("Molten Armor", true)
                                  )),

                new Decorator(ret => !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport && SpellManager.HasSpell("Molten Armor") && !Me.Mounted && !SpellManager.HasSpell("Mage Armor") && SpellManager.HasSpell("Frost Armor") && AmplifySettings.Instance.ArmorSelect == "Auto" || AmplifySettings.Instance.ArmorSelect == "Frost" && !Me.Mounted && SpellManager.HasSpell("Frost Armor"),
                              new PrioritySelector(
                                  CreateBuffCheckAndCast("Frost Armor", true)
                                  )),

                new Decorator(ret => !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport && SpellManager.HasSpell("Molten Armor") && !Me.Mounted && SpellManager.HasSpell("Mage Armor") && SpellManager.HasSpell("Frost Armor") && AmplifySettings.Instance.ArmorSelect == "Auto" || AmplifySettings.Instance.ArmorSelect == "Mage" && !Me.Mounted && SpellManager.HasSpell("Mage Armor"),
                              new PrioritySelector(
                                  CreateBuffCheckAndCast("Mage Armor", true)
                                  ))
                );
        }
    }
}

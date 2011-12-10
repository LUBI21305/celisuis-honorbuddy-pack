using Styx.Logic.Combat;
using Styx.Combat.CombatRoutine;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public override bool NeedCombatBuffs
        {
            get
            {
                if (SSSettings.Instance.UsePreparation)
                {
                    if (SpellManager.HasSpell("Preparation"))
                    {
                        if (SpellManager.HasSpell("Evasion") && !SpellManager.CanCast("Evasion"))
                        {
                            if (SpellManager.CanCast("Preparation"))
                            {
                                return true;
                            }
                        }
                        if (SpellManager.HasSpell("Vanish") && !SpellManager.CanCast("Vanish"))
                        {
                            if (SpellManager.CanCast("Preparation"))
                            {
                                return true;
                            }
                        }
                        if (SpellManager.HasSpell("Shadowstep") && !SpellManager.CanCast("Shadowstep"))
                        {
                            if (SpellManager.CanCast("Preparation"))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
        public override void CombatBuff()
        {
            if (SSSettings.Instance.UsePreparation)
            {
                if (SpellManager.HasSpell("Evasion"))
                {
                    if (SpellManager.HasSpell("Evasion") && !SpellManager.CanCast("Evasion"))
                    {
                        if (SpellManager.CanCast("Preparation"))
                        {
                            Preparation();
                        }
                    }
                    if (SpellManager.HasSpell("Vanish") && !SpellManager.CanCast("Vanish"))
                    {
                        if (SpellManager.CanCast("Preparation"))
                        {
                            Preparation();
                        }
                    }
                    if (SpellManager.HasSpell("Shadowstep") && !SpellManager.CanCast("Shadowstep"))
                    {
                        if (SpellManager.CanCast("Preparation"))
                        {
                            Preparation();
                        }
                    }
                }
            }
        }
    }
}

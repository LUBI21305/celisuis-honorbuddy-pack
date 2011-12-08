using Styx.Logic;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.Combat.CombatRoutine;


namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public override void Pulse()
        {
            if (SSSettings.Instance.UseStealthAlways)
            {
                if (!Me.Mounted)
                {
                    if (!Me.Combat && !NeedHeal)
                    {
                        if (!Me.HasAura("Food") && SpellManager.CanCast("Stealth"))
                        {
                            Stealth();
                        }
                    }
                }
            }
//            if (SSSettings.Instance.UseRecuperate && !Me.HasAura("Recuperate"))
//            {
//                if (SpellManager.HasSpell("Recuperate") && SpellManager.CanCast("Recuperate"))
//                {
//                    if (ObjectManager.Wow.ReadRelative<uint>(0x990759) > 0 && !Me.Combat)
//                    {
//                        slog("We have left over combo points. Attempting Recuperate.");
//                        Recuperate();
//                    }
//                }
//            }
        }
    }
}



using System.Threading;

using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.Combat.CombatRoutine;


namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public override bool NeedPullBuffs
        {
            get
            {
                if (SSSettings.Instance.UseStealthToPull && SpellManager.HasSpell("Stealth") && SpellManager.HasSpell("Ambush"))
                {
                    if (!Me.HasAura("Stealth") && SpellManager.CanCast("Stealth"))
                    {
                        return true;
                    }
                }
                if (SSSettings.Instance.UseSprintPull && SpellManager.HasSpell("Sprint"))
                {
                    if (!Me.HasAura("Sprint") && SpellManager.CanCast("Sprint"))
                    {
                        return true;
                    }
                }
                if (SSSettings.Instance.UsePremeditation && SpellManager.HasSpell("Preditation"))
                {
                    if (!Me.HasAura("Premeditation") && SpellManager.CanCast("Premeditation"))
                    {
                        return true;
                    }
                }
                if (SSSettings.Instance.UseRedirect && SpellManager.HasSpell("Redirect"))
                {
                    if (ObjectManager.Wow.ReadRelative<uint>(0x990759) > 0 && SpellManager.CanCast("Redirect"))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public override void PullBuff()
        {
            if (SSSettings.Instance.UseRedirect && SpellManager.HasSpell("Redirect") && Me.HasAura("Recuperate"))
            {
                if (ObjectManager.Wow.ReadRelative<uint>(0x990759) > 0 && SpellManager.CanCast("Redirect"))
                {
                    if (!Me.Combat && Me.GotTarget && Me.CurrentTarget.IsAlive)
                    {
                        slog("We are recuperating and have combo points. Attempting Redirect.");
                        Redirect();
                    }
                }
            }
            if (Me.Combat)
            {
                slog("We've entered combat. Cancelling Pull Buffs");
                return;
            }
            if (!Me.Combat)
            {
                if (SSSettings.Instance.UseStealthToPull && !Me.HasAura("Stealth") && SpellManager.CanCast("Stealth") && SpellManager.HasSpell("Ambush"))
                {
                    Stealth();
                }
                if (SSSettings.Instance.UseSprintPull && !Me.HasAura("Sprint") && SpellManager.CanCast("Sprint") && targetDistance > 7)
                {
                    Sprint();
                }
                if (SSSettings.Instance.UsePremeditation && !Me.HasAura("Premeditation") && SpellManager.CanCast("Premeditation"))
                {
                    Premeditation();
                }
            }


        }
    }
}

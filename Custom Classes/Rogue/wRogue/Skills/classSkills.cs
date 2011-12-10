using System.Threading;

using Styx;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.Logic.Pathing;
using Styx.Combat.CombatRoutine;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        // Distract
        public void Distract()
        {
            WoWPoint distractPoint = WoWMovement.CalculatePointFrom(Me.CurrentTarget.Location, -4.0f);
            SpellManager.Cast("Distract");
            LegacySpellManager.ClickRemoteLocation(distractPoint);
            slog("Skill: Distract");
        }
        void Recuperate()
        {
            SpellManager.Cast("Recuperate");
            slog("Skill: Recuperate");
        }
        void Vendetta()
        {
            SpellManager.Cast("Vendetta");
            slog("Skill: Vendetta");
        }
        void RevealingStrike()
        {
            SpellManager.Cast("Revealing Strike");
            slog("Skill: Revealing Strike");
        }
        void FanofKnives()
        {
            SpellManager.Cast("Fan of Knives");
            slog("Skill: Fan of Knives");
        }
        void Riposte()
        {
            SpellManager.Cast("Riposte");
            slog("Skill: Riposte");
        }
        void DeadlyThrow()
        {
            SpellManager.Cast("Deadly Throw");
            slog("Skill: Deadly Throw");
        }
        void CloakofShadows()
        {
            SpellManager.Cast("Cloak of Shadows");
            slog("Skill: Cloak of Shadows");
        }
        void KidneyShot()
        {
            SpellManager.Cast("Kidney Shot");
            slog("Skill: Kidney Shot");
        }
        void PickPocket()
        {
            SpellManager.Cast("Pick Pocket");
            slog("Skill: Pick Pocket");
        }
        void Envenom()
        {
            SpellManager.Cast("Envenom");
            slog("Skill: Envenom");
        }
        void Lifeblood()
        {
            SpellManager.Cast("Lifeblood");
            slog("Skill: Lifeblood");
        }
        void Rupture()
        {
            SpellManager.Cast("Rupture");
            slog("Skill: Rupture");
        }
        void Mutilate()
        {
            SpellManager.Cast("Mutilate");
            slog("Skill: Mutilate");
        }
        void Hemorrhage()
        {
            SpellManager.Cast("Hemorrhage");
            slog("Skill: Hemorrhage");
        }
        void Premeditation()
        {
            SpellManager.Cast("Premeditation");
            slog("Skill: Premeditation");
        }
        void Shadowstep()
        {
            SpellManager.Cast("Shadowstep");
            slog("Skill: Shadowstep");
        }
        void ShadowDance()
        {
            SpellManager.Cast("Shadow Dance");
            slog("Skill: Shadow Dance");
        }
        void GhostlyStrike()
        {
            SpellManager.Cast("Ghostly Strike");
            slog("Skill: Ghostly Strike");
        }
        void SinisterStrike()
        {
            SpellManager.Cast("Sinister Strike");
            slog("Skill: Sinister Strike");
        }
        void Eviscerate()
        {
            SpellManager.Cast("Eviscerate");
            slog("Skill: Eviscerate");
        }
        void Stealth()
        {
            SpellManager.Cast("Stealth");
            slog("Skill: Stealth");
        }
        void Evasion()
        {
            SpellManager.Cast("Evasion");
            slog("Skill: Evasion");
        }
        void Sprint()
        {
            SpellManager.Cast("Sprint");
            slog("Skill: Sprint");
        }
        void Backstab()
        {
            SpellManager.Cast("Backstab");
            slog("Skill: Backstab");
        }
        void Gouge()
        {
            SpellManager.Cast("Gouge");
            slog("Skill: Gouge");
        }
        void CheapShot()
        {
            SpellManager.Cast("Cheap Shot");
            slog("Skill: Cheap Shot");
        }
        void Kick()
        {
            SpellManager.Cast("Kick");
            slog("Skill: Kick");
        }
        void Garrote()
        {
            SpellManager.Cast("Garrote");
            slog("Skill: Garrote");
        }
        void Ambush()
        {
            SpellManager.Cast("Ambush");
            slog("Skill: Ambush");
        }
        void SliceAndDice()
        {
            SpellManager.Cast("Slice and Dice");
            slog("Skill: Slice and Dice");
        }
        void Vanish()
        {
            SpellManager.Cast("Vanish");
            slog("Skill: Vanish");
        }
        void HungerForBlood()
        {
            SpellManager.Cast("Hunger For Blood");
            slog("Skill: Hunger For Blood");
        }
        void BladeFlurry()
        {
            SpellManager.Cast("Blade Flurry");
            slog("Skill: Blade Flurry");
        }
        void ColdBlood()
        {
            SpellManager.Cast("Cold Blood");
            slog("Skill: Cold Blood");
        }
        void AdrenalineRush()
        {
            SpellManager.Cast("Adrenaline Rush");
            slog("Skill: Adrenaline Rush");
        }
        void KillingSpree()
        {
            SpellManager.Cast("Killing Spree");
            slog("Skill: Killing Spree");
        }
        void Preparation()
        {
            SpellManager.Cast("Preparation");
            slog("Skill: Preparation");
        }
        void Redirect()
        {
            SpellManager.Cast("Redirect");
            slog("Skill: Redirect");
        }
        bool Throw()
        {
            if (Me.GotTarget && !Me.Combat)
            {
                if (Me.CurrentTarget.Distance >= 6 && Me.CurrentTarget.Distance <= 28)
                {
                    if (SpellManager.Cast("Throw")) //Found Throw
                    {
                        slog("Skill: Throw");
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

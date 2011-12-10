using System.Threading;

using Styx;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.Combat.CombatRoutine;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        // Combat for people under level ten.
        void FinishLow()
        {
            if (!Me.Combat)
            {
                return;
            }
            if (StyxWoW.Me.ComboPoints > 1 && Me.CurrentTarget.HealthPercent < 20)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints > 3 && Me.CurrentTarget.HealthPercent < 40)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints > 4 && Me.CurrentTarget.HealthPercent < 100)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints < 5)
            {
                if (SpellManager.HasSpell("Sinister Strike"))
                {
                    if (SpellManager.CanCast("Sinister Strike") && Me.GotTarget && targetDistance <= 5)
                    {
                        SinisterStrike();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        // Combat for people in Assassination spec.
        void FinishAss()
        {
            if (!Me.Combat)
            {
                return;
            }
            if (StyxWoW.Me.CurrentTarget.HasAura("Deadly Poison"))
            {
                if (SpellManager.CanCast("Envenom"))
                {
                    if (targetDistance <= 5 && Me.Combat)
                    {
                        if (StyxWoW.Me.ComboPoints == 5 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Envenom();
                            }
                            else
                            {
                                Envenom();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 4 && Me.CurrentTarget.HealthPercent < 55 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Envenom();
                            }
                            else
                            {
                                Envenom();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 3 && Me.CurrentTarget.HealthPercent < 40 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Envenom();
                            }
                            else
                            {
                                Envenom();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 2 && Me.CurrentTarget.HealthPercent < 25 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Envenom();
                            }
                            else
                            {
                                Envenom();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 1 && Me.CurrentTarget.HealthPercent < 10 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Envenom();
                            }
                            else
                            {
                                Envenom();
                            }
                        }
                    }
                }
            }

            if (!SpellManager.HasSpell("Envenom"))
            {
                if (SpellManager.CanCast("Eviscerate"))
                {
                    if (targetDistance <= 5 && Me.Combat)
                    {
                        if (StyxWoW.Me.ComboPoints == 5 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Eviscerate();
                            }
                            else
                            {
                                Eviscerate();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 4 && Me.CurrentTarget.HealthPercent < 55 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Eviscerate();
                            }
                            else
                            {
                                Eviscerate();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 3 && Me.CurrentTarget.HealthPercent < 40 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Eviscerate();
                            }
                            else
                            {
                                Eviscerate();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 2 && Me.CurrentTarget.HealthPercent < 25 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Eviscerate();
                            }
                            else
                            {
                                Eviscerate();
                            }
                        }
                        if (StyxWoW.Me.ComboPoints >= 1 && Me.CurrentTarget.HealthPercent < 10 && Me.GotTarget)
                        {
                            if (SSSettings.Instance.UseColdBlood && SpellManager.CanCast("Cold Blood"))
                            {
                                ColdBlood();
                                Eviscerate();
                            }
                            else
                            {
                                Eviscerate();
                            }
                        }
                    }
                }
            }
            if (!StyxWoW.Me.CurrentTarget.HasAura("Rupture"))
            {
                if (targetDistance <= 5 && Me.Combat)
                {
                    if (SpellManager.CanCast("Rupture"))
                    {
                        if (StyxWoW.Me.ComboPoints >= 2 && Me.CurrentTarget.HealthPercent <= 80 && Me.GotTarget)
                        {
                            Rupture();
                        }
                    }
                }
            }
            if (StyxWoW.Me.ComboPoints < 5)
            {
                if (!SpellManager.HasSpell("Mutilate"))
                {
                    SinisterStrike();
                }
                if (SpellManager.CanCast("Mutilate") && Me.GotTarget && targetDistance <= 5)
                {
                    Mutilate();
                }
                else
                {
                    return;
                }
            }
        }

        // Combat for people in Combat spec.
        void FinishCom()
        {
            if (!Me.Combat)
            {
                return;
            }
            if (StyxWoW.Me.ComboPoints > 1 && Me.CurrentTarget.HealthPercent < 20)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints > 3 && Me.CurrentTarget.HealthPercent < 40)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints > 4 && Me.CurrentTarget.HealthPercent < 100)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints < 5)
            {
                if (SpellManager.CanCast("Revealing Strike") && Me.GotTarget && targetDistance <= 5)
                {
                    if (!Me.CurrentTarget.HasAura("Revealing Strike"))
                    {
                        RevealingStrike();
                    }
                    else
                    {
                        SinisterStrike();
                    }
                }
                else if (!SpellManager.HasSpell("Revealing Strike"))
                {
                    if (SpellManager.CanCast("Sinister Strike") && Me.GotTarget && targetDistance <= 5)
                    {
                        SinisterStrike();
                    }
                }
                else
                {
                    return;
                }
            }
        }


        // Combat for people in Subtlety spec.
        void FinishSub()
        {
            if (!Me.Combat)
            {
                return;
            }
            if (SSSettings.Instance.UseShadowDance)
            {
                if (SpellManager.CanCast("Shadowstep") && SpellManager.CanCast("Shadow Dance") && SpellManager.CanCast("Gouge"))
                {
                    Gouge();
                    StyxWoW.SleepForLagDuration();
                    ShadowDance();
                    StyxWoW.SleepForLagDuration();
                    Shadowstep();
                    StyxWoW.SleepForLagDuration();
                    WoWMovement.Face();
                    StyxWoW.SleepForLagDuration();
                    Ambush();
                }
            }
            if (StyxWoW.Me.ComboPoints > 1 && Me.CurrentTarget.HealthPercent < 20)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints > 3 && Me.CurrentTarget.HealthPercent < 40)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints > 4 && Me.CurrentTarget.HealthPercent < 100)
            {
                if (SpellManager.CanCast("Eviscerate") && Me.GotTarget && targetDistance <= 5)
                {
                    Eviscerate();
                }
            }
            if (StyxWoW.Me.ComboPoints < 5)
            {
                if (!SpellManager.HasSpell("Hemorrhage"))
                {
                    SinisterStrike();
                }
                if (SpellManager.CanCast("Hemorrhage") && Me.GotTarget && targetDistance <= 5)
                {
                    Hemorrhage();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
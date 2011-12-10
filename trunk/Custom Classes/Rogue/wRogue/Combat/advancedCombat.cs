using System;
using System.Threading;
using System.Collections.Generic;

using Styx;
using Styx.Logic;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

using wRogue.Talents;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {

        private void AdvancedCombat()
        {
            if (!Me.Combat)
            {
                return;
            }

            if (SSSettings.Instance.UseSliceAndDice)
            {
                if (SpellManager.CanCast("Slice and Dice") && !Me.HasAura("Slice and Dice"))
                {
                    if (StyxWoW.Me.ComboPoints > 1 && Me.GotTarget && targetDistance <= 5)
                    {
                        SliceAndDice();
                    }
                }
            }
            if (SSSettings.Instance.UseRecuperate)
            {
                if (SpellManager.HasSpell("Recuperate"))
                {
                    if (Me.GotTarget && targetDistance >= 25)
                    {
                        if (SpellManager.CanCast("Recuperate") && !Me.HasAura("Recuperate"))
                        {
                            if (StyxWoW.Me.ComboPoints >= 1 && Me.HealthPercent < 98)
                            {
                                Recuperate();
                            }
                        }
                    }
                    if (Me.GotTarget && targetDistance <= 5)
                    {
                        if (SpellManager.CanCast("Recuperate") && !Me.HasAura("Recuperate"))
                        {
                            if (Me.HealthPercent < 98 && Me.ComboPoints >= 2)
                            {
                                Recuperate();
                            }
                        }
                    }
                }
            }
            if (SSSettings.Instance.UseVendetta)
            {
                if (Me.GotTarget && targetDistance <= 5)
                {
                    if (SpellManager.CanCast("Vendetta"))
                    {
                        Vendetta();
                    }
                }
            }
            if (Me.Combat && Me.GotTarget)
            {
                if (TalentSpec == wRogueSpec.Lowbie)
                {
                    FinishLow();
                }
                if (TalentSpec == wRogueSpec.Assassination)
                {
                    FinishAss();
                }
                if (TalentSpec == wRogueSpec.Combat)
                {
                    FinishCom();
                }
                if (TalentSpec == wRogueSpec.Subtlety)
                {
                    FinishSub();
                }
            }
        }
    }
}
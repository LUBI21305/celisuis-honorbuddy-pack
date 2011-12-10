using System.Linq;
using System.Threading;
using System.Linq.Expressions;
using System.Collections.Generic;

using Styx;
using Styx.Logic;
using Styx.WoWInternals;
using Styx.Logic.Combat;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public override bool NeedRest
        {
            get
            {
                if (!SSSettings.Instance.UseRest)
                {
                    return false;
                }
                if (Me.HasAura("Food"))
                {
                    return false;
                }
                if (SSSettings.Instance.UseRest)
                {
                    if (Me.HealthPercent < SSSettings.Instance.restHealth)
                    {
                        if (!Me.IsSwimming && !Me.Combat)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public override void Rest()
        {
            if (SSSettings.Instance.UseRest)
            {
                if (!Styx.Logic.Common.Rest.NoFood)
                {
                    if (Me.HealthPercent < SSSettings.Instance.restHealth)
                    {
                        if (!Me.Combat && !Me.IsSwimming)
                        {
                            if (Me.Mounted)
                            {
                                Mount.Dismount();
                            }
                            slog("Rest: Eating Food");
                            Lua.DoString("UseItemByName(\"" + Styx.Helpers.LevelbotSettings.Instance.FoodName + "\")");
                            Thread.Sleep(500);
                            SpellManager.Cast("Stealth");
                            while ((Me.HealthPercent < 98) && Me.HasAura("Food"))
                            {
                                if (!ObjectManager.IsInGame || Me == null)
                                    break;

                                WoWPulsator.Pulse(BotManager.Current.PulseFlags);
                                ObjectManager.Update();
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
            }
            fightTimer.Reset();
            pullTimer.Reset();
        }
    }
}




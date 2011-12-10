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
        public override bool NeedHeal
        {
            get
            {
                WoWItem bandage = HaveItemCheck(BandageIds);
                WoWItem healPotion = HaveItemCheck(HealPotionIds);

                if (!SSSettings.Instance.UseHealthPotions && !SSSettings.Instance.UseBandages)
                {
                    return false;
                }
                if (SSSettings.Instance.UseHealthPotions)
                {
                    if (!Equals(null, healPotion))
                    {
                        if (Me.HealthPercent < SSSettings.Instance.UseHealthPotionsAt && Me.CurrentTarget.HealthPercent > SSSettings.Instance.DontWastePotion)
                        {
                            if (Me.Combat)
                            {
                                return true;
                            }
                        }
                    }
                }
                if (SSSettings.Instance.UseBandages)
                {
                    if (!Equals(null, bandage))
                    {
                        if (Me.HealthPercent < SSSettings.Instance.UseBandagesAt && Me.CurrentTarget.HealthPercent > SSSettings.Instance.DontWasteBandage)
                        {
                            if (Me.Combat)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
        public override void Heal()
        {
            if (SSSettings.Instance.UseHealthPotions && Me.Combat)
            {
                if (Me.HealthPercent < SSSettings.Instance.UseHealthPotionsAt && Me.CurrentTarget.HealthPercent > SSSettings.Instance.DontWastePotion)
                {
                    HealPotionDrink();
                }
            }
            if (SSSettings.Instance.UseBandages && Me.Combat)
            {
                if (Me.HealthPercent < SSSettings.Instance.UseBandagesAt && Me.CurrentTarget.HealthPercent > SSSettings.Instance.DontWasteBandage)
                {
                    WoWItem bandage = HaveItemCheck(BandageIds);
                    if (!Me.CurrentTarget.HasAura("Blind") && SpellManager.CanCast("Blind") && !Me.HasAura("Recently Bandaged") && !Equals(null, bandage))
                    {
                        SpellManager.Cast("Blind");
                        Bandaging();
                    }
                    else if (SpellManager.CanCast("Gouge") && !Me.HasAura("Recently Bandaged") && !Equals(null, bandage))
                    {
                        SpellManager.Cast("Gouge");
                        Bandaging();
                    }
                }
            }
        }
        private void HealPotionDrink()
        {
            WoWItem healPotion = HaveItemCheck(HealPotionIds);
            if (!Equals(null, healPotion))
            {
                slog("Drinking Heal Potion.");
                Lua.DoString("UseItemByName(\"" + healPotion.Entry + "\")");
                StyxWoW.SleepForLagDuration();
            }
            else
                slog("No potions in backpack.");
        }
        private void Bandaging()
        {
            WoWItem bandage = HaveItemCheck(BandageIds);
            if (!Equals(null, bandage) && addsList.Count < 2)
            {
                slog("Bandaging.");
                Lua.DoString("UseItemByName(\"" + bandage.Entry + "\")");
                StyxWoW.SleepForLagDuration();
                Lua.DoString("TargetUnit(\"" + player + "\")");
                uint a = 0;
                while (a < 81)
                {
                    if (addsList[0].HasAura("Gouge") || addsList[0].HasAura("Blind"))
                    {
                        StyxWoW.SleepForLagDuration();
                        ++a;
                    }
                    else
                    {
                        addsList[0].Target();
                        WoWMovement.Face();
                        Lua.DoString("StartAttack()");
                        a = 81;
                    }
                }
            }
            else
                slog("No bandages in backpack.");
        }
    }
}




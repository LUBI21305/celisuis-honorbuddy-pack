using System;
using System.Threading;
using System.Collections.Generic;

using Styx;
using Styx.Logic;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public override bool NeedPreCombatBuffs
        {
            get
            {
                if (SSSettings.Instance.UsePoisons)
                {
                    if (NeedPoisons())
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public override void PreCombatBuff()
        {
            if (NeedPoisons())
            {
                ApplyPoisons();
                Thread.Sleep(3000);
            }
        }
        private bool IsWeaponEnhanceNeeded(out bool needMainhand, out bool needOffhand)
        {
            needMainhand = false;
            needOffhand = false;
            List<string> weaponEnchants = Lua.GetReturnValues("return GetWeaponEnchantInfo()", "wired420.lua");
            if (Equals(null, weaponEnchants))
                return false;
            needMainhand = weaponEnchants[0] == "" || weaponEnchants[0] == "nil";
            if (Me.Inventory.Equipped.OffHand.Entry > 0)
                needOffhand = weaponEnchants[3] == "" || weaponEnchants[3] == "nil";
            if (needMainhand)
                slog("Mainhand weapon " + Me.Inventory.Equipped.MainHand.Name + " needs enchancement");
            if (needOffhand)
                slog("Offhand weapon " + Me.Inventory.Equipped.OffHand == null ? "(none)" : Me.Inventory.Equipped.OffHand.Name + "needs enhancement");
            return needMainhand || needOffhand;
        }
        private bool NeedPoisons()
        {
            var mainHandEnchant = Me.Inventory.Equipped.MainHand.TemporaryEnchantment.Id;
            //slog("Set mainHandEnchant to " + Me.Inventory.Equipped.MainHand.TemporaryEnchantment.Id);
            var offHandEnchant = Me.Inventory.Equipped.OffHand.TemporaryEnchantment.Id;
            //slog("Set offHandEnchant to " + Me.Inventory.Equipped.OffHand.TemporaryEnchantment.Id);

            // over level 10
            if (Me.Level < 10)
            {
                return false;
            }

            if (mainHandEnchant != 0 && offHandEnchant != 0)
            {
                return false;
            }

            // look at all items in inventory and check we have poisons to apply
            WoWItem instantPoison = HaveItemCheck(PoisonInstantEntryIds);
            WoWItem deadlyPoison = HaveItemCheck(PoisonDeadlyEntryIds);
            WoWItem woundPoison = HaveItemCheck(PoisonWoundEntryIds);
            WoWItem cripplingPoison = HaveItemCheck(PoisonCripplingEntryIds);
            switch (SSSettings.Instance.poisonToMain)
            {
                case 1:
                    mainHandPoison = instantPoison;
                    break;
                case 2:
                    mainHandPoison = deadlyPoison;
                    break;
                case 3:
                    mainHandPoison = cripplingPoison;
                    break;
                case 4:
                    mainHandPoison = woundPoison;
                    break;
            }
            switch (SSSettings.Instance.poisonToOff)
            {
                case 1:
                    offHandPoison = instantPoison;
                    break;
                case 2:
                    offHandPoison = deadlyPoison;
                    break;
                case 3:
                    offHandPoison = cripplingPoison;
                    break;
                case 4:
                    offHandPoison = woundPoison;
                    break;
            }
            if (Equals(null, mainHandPoison))
            {
                return false;
            }
            if (Equals(null, offHandPoison) && Equals(null, mainHandPoison))
            {
                return false;
            }
            // look at weapons and see if they have poisons applied
            if (Me.Inventory.Equipped.MainHand.Entry == Me.Inventory.Equipped.OffHand.Entry)
            {
                bool needPoisonMainhand, needPoisonOffhand;
                return IsWeaponEnhanceNeeded(out needPoisonMainhand, out needPoisonOffhand);
            }
            if (mainHandEnchant == 0)
            {
                return true;
            }
            if (offHandEnchant == 0)
            {
                return true;
            }
            return false;
        }
        private void ApplyPoisons()
        {
            WoWItem instantPoison = HaveItemCheck(PoisonInstantEntryIds);
            WoWItem deadlyPoison = HaveItemCheck(PoisonDeadlyEntryIds);
            WoWItem woundPoison = HaveItemCheck(PoisonWoundEntryIds);
            WoWItem cripplingPoison = HaveItemCheck(PoisonCripplingEntryIds);
            if (Me.Mounted)
            {
                Mount.Dismount();
            }
            WoWMovement.MoveStop();
            bool needPoisonMainhand, needPoisonOffhand;
            if (IsWeaponEnhanceNeeded(out needPoisonMainhand, out needPoisonOffhand))
            {
                if (needPoisonOffhand && !Equals(null, offHandPoison))
                {
                    slog("Applying poison to offhand weapon: " + Me.Inventory.Equipped.OffHand.Name);
                    Lua.DoString("UseItemByName(\"" + offHandPoison.Entry + "\")");
                    StyxWoW.SleepForLagDuration();
                    Lua.DoString("UseInventoryItem(17)");
                    Thread.Sleep(3100);
                }
                if (needPoisonMainhand && !Equals(null, mainHandPoison))
                {
                    slog("Applying poison to main hand weapon: " + Me.Inventory.Equipped.MainHand.Name);
                    Lua.DoString("UseItemByName(\"" + mainHandPoison.Entry + "\")");
                    StyxWoW.SleepForLagDuration();
                    Lua.DoString("UseInventoryItem(16)");
                    Thread.Sleep(3100);
                }
            }
        }
    }
}

//
//#undef DEBUG
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace AutoEquip
{
    public class AutoEquip : HBPlugin
    {
        private readonly Dictionary<InventorySlot, WoWItem> _equippedItems = new Dictionary<InventorySlot, WoWItem>();
        private readonly HashSet<ulong> _ignoreItems = new HashSet<ulong>();
        private readonly Version _ver = new Version(2, 0, 2);
        private bool _hasInitialized;

        public AutoEquip()
        {
            AutoEquipSettings.InitializeSettings();
            // If this is because of a recompile, we will be able to find the default set right now. Else we will do it again in the first pulse. :)
            AutoEquipSettings.LoadDefaultSet();
        }

        public override string Name { get { return "AutoEquip"; } }

        public override string Author { get { return "MaiN"; } }

        public override Version Version { get { return _ver; } }

        public override string ButtonText { get { return "Select Weight Set"; } }

        public override bool WantButton { get { return true; } }

        public override void OnButtonPress()
        {
            var form = new FormWeightSelector();
            form.ShowDialog();
        }
        
        private void HandleLootClosed(object sender, LuaEventArgs e)
        {
            try
            {

                if (!TreeRoot.IsRunning)
                    return;

                if (ObjectManager.Me.Combat || ObjectManager.Me.Dead || ObjectManager.Me.IsGhost)
                    return;

                if (Battlegrounds.IsInsideBattleground)
                    return;

                DoCheck();
            }
            catch (Exception ex)
            {
                Styx.Helpers.Logging.WriteException(ex);
            }
        }

        public override void Initialize()
        {
            if (AutoEquipSettings.ChosenWeightSet == null)
                AutoEquipSettings.LoadDefaultSet();

            BotEvents.OnBotStart += BotEvents_OnBotStart;
            BotEvents.Player.OnLevelUp += Player_OnLevelUp;
            Lua.Events.AttachEvent("UNIT_INVENTORY_CHANGED", HandleLootClosed);
            Lua.Events.AttachEvent("LOOT_CLOSED", HandleLootClosed);
            Lua.Events.AttachEvent("AUTOEQUIP_BIND_CONFIRM", HandleBindConfirm);
            Lua.Events.AttachEvent("EQUIP_BIND_CONFIRM", HandleBindConfirm);
            Lua.Events.AttachEvent("LOOT_BIND_CONFIRM", HandleBindConfirm);
            _initialized = true;
        }

        public override void Dispose()
        {
            BotEvents.OnBotStart -= BotEvents_OnBotStart;
            BotEvents.Player.OnLevelUp -= Player_OnLevelUp;
            Lua.Events.DetachEvent("UNIT_INVENTORY_CHANGED", HandleLootClosed);
            Lua.Events.DetachEvent("LOOT_CLOSED", HandleLootClosed);
            Lua.Events.DetachEvent("AUTOEQUIP_BIND_CONFIRM", HandleBindConfirm);
            Lua.Events.DetachEvent("EQUIP_BIND_CONFIRM", HandleBindConfirm);
            Lua.Events.DetachEvent("LOOT_BIND_CONFIRM", HandleBindConfirm);
        }

        private bool _initialized;
        public override void Pulse()
        {
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }
        }

        private void HandleBindConfirm(object sender, LuaEventArgs args)
        {
            Lua.DoString("EquipPendingItem(0) ConfirmBindOnUse() StaticPopup1Button1:Click()");
        }

        private WaitTimer _doCheckWait = WaitTimer.FiveSeconds;

        private void DoCheck()
        {
            if (AutoEquipSettings.ChosenWeightSet == null)
            {
                Log(true, "You have not yet selected a weight set!{0}Please open the form and select a weight set to use AutoEquip.", Environment.NewLine);
                return;
            }

            if (!_doCheckWait.IsFinished)
                return;

            _doCheckWait.Reset();

            RefreshEquippedItems();


            int tickCount = Environment.TickCount;

            HashSet<WoWItemQuality> equipQualities = AutoEquipSettings.EquipQualities;
            var items = new List<WoWItem>();
            items.AddRange(ObjectManager.Me.Inventory.Backpack.PhysicalItems);
            for (uint i = 0; i < 4; i++)
            {
                WoWContainer bag = ObjectManager.Me.GetBagAtIndex(i);
                if (bag != null)
                    items.AddRange(bag.PhysicalItems);
            }

            var equippableNewItems = new List<WoWItem>();

            foreach (WoWItem item in items)
            {
                ulong itemGuid = item.Guid;
                if (_ignoreItems.Contains(itemGuid))
                    continue;

                if (_equippedItems.ContainsValue(item)
                    || AutoEquipSettings.IsItemIgnored(item)
                    || AutoEquipSettings.IgnoreTypes.Contains(item.ItemInfo.InventoryType)
                    || !equipQualities.Contains(item.ItemInfo.Quality)
                    || !ObjectManager.Me.CanEquipItem(item))
                {
                    _ignoreItems.Add(itemGuid);
                    continue;
                }

                equippableNewItems.Add(item);
            }

            int took = Environment.TickCount - tickCount;

            if (equippableNewItems.Count == 0)
                return;

            if (AutoEquipSettings.AutoEquipItems)
            {
                tickCount = Environment.TickCount;


                CheckForItems(equippableNewItems);


                took = Environment.TickCount - tickCount;
            }
            if (AutoEquipSettings.AutoEquipBags)
            {
                tickCount = Environment.TickCount;


                CheckForBag(equippableNewItems);


                took = Environment.TickCount - tickCount;
            }
        }

        private void Player_OnLevelUp(BotEvents.Player.LevelUpEventArgs args)
        {
            ClearItemCache();
        }

        private void BotEvents_OnBotStart(EventArgs args)
        {
            ClearItemCache();
        }

        private void ClearItemCache()
        {
            _ignoreItems.Clear();
            _equippedItems.Clear();
        }

        private static void EquipItemIntoSlot(WoWItem item, InventorySlot slot)
        {
            Lua.DoString(string.Format("EquipItemByName({0}, {1})", item.Entry, (int) slot), "_main.lua");

            Thread.Sleep(1000);

            //Lua.DoString("EquipPendingItem(0) ConfirmBindOnUse() StaticPopup_Hide(\"AUTOEQUIP_BIND\") StaticPopup_Hide(\"EQUIP_BIND\") StaticPopup_Hide(\"USE_BIND\")");
            //Thread.Sleep(1000);
        }

        private static void EquipItem(WoWItem item)
        {

            item.UseContainerItem();
            // Wait 1 second... kthx
            Thread.Sleep(1000);

            //item.Interact();
            //Lua.DoString(string.Format("EquipItemByName(\"{0}\")", Lua.Escape(item.Name)), "_main.lua");
            //Thread.Sleep(1000);

            //Lua.DoString("EquipPendingItem(0) ConfirmBindOnUse() StaticPopup_Hide(\"AUTOEQUIP_BIND\") StaticPopup_Hide(\"EQUIP_BIND\") StaticPopup_Hide(\"USE_BIND\")");
            //Thread.Sleep(1000);
        }

        private void CheckForItems(IEnumerable<WoWItem> equippableItems)
        {
            ILookup<InventoryType, WoWItem> categorizedItems = equippableItems.ToLookup(item => item.ItemInfo.InventoryType);
            foreach (var grouping in categorizedItems)
            {
                if (grouping.Key == InventoryType.Bag)
                    continue;

                float bestEquipItemScore;
                WoWItem bestEquipItem = FindBestEquipItem(grouping, out bestEquipItemScore);
                if (bestEquipItem == null)
                    continue;

                List<InventorySlot> equipSlots = DecideEquipmentSlots(bestEquipItem);

                float lowestItemScore;
                InventorySlot bestSlot = FindBestEquipmentSlot(equipSlots, out lowestItemScore);
                if (bestSlot == InventorySlot.None)
                {
                    Log(false, "I'm not equipping item {0} of inventory type {1} as there are no slots to equip it into", bestEquipItem.Name, bestEquipItem.ItemInfo.InventoryType);

                    continue;
                }

                if (bestEquipItemScore > lowestItemScore)
                {
                    if (lowestItemScore == float.MinValue)
                        Log(true, "Equipping {2} \"{0}\" into empty slot {1}", bestEquipItem.Name, bestSlot, bestEquipItem.ItemInfo.InventoryType);
                    else
                        Log(true, "Equipping {4} \"{0}\" instead of \"{1}\" - it scored {2} while the old scored {3}", bestEquipItem.Name, _equippedItems[bestSlot].Name, bestEquipItemScore, lowestItemScore, bestEquipItem.ItemInfo.InventoryType);

                    EquipItemIntoSlot(bestEquipItem, bestSlot);
                }
            }
        }

        private InventorySlot FindBestEquipmentSlot(IEnumerable<InventorySlot> equipSlots, out float lowestItemScore)
        {
            InventorySlot bestSlot = InventorySlot.None;
            float lowestEquippedItemScore = float.MaxValue;
            foreach (InventorySlot inventorySlot in equipSlots)
            {
                if (AutoEquipSettings.ProtectedSlots.Contains(inventorySlot))
                {
                    Log(false, "I'm not equipping into equipment slot {0} as it is protected", inventorySlot);

                    continue;
                }

                if (!_equippedItems.ContainsKey(inventorySlot))
                {
                    Log(false, "InventorySlot {0} is unknown! Please report this to MaiN.", inventorySlot);
                    continue;
                }

                WoWItem equippedItem = _equippedItems[inventorySlot];
                if (equippedItem == null)
                {
                    lowestItemScore = float.MinValue;
                    return inventorySlot;
                }

                if (!AutoEquipSettings.ReplaceHeirlooms && equippedItem.Quality == WoWItemQuality.Heirloom)
                {
                    Log(false, "I'm not equipping anything into {0} as I can't replace heirloom items!", inventorySlot);
                    continue;
                }

                float itemScore = AutoEquipSettings.ChosenWeightSet.EvaluateItem(equippedItem);
                if (itemScore < lowestEquippedItemScore)
                {
                    bestSlot = inventorySlot;
                    lowestEquippedItemScore = itemScore;
                }
            }

            lowestItemScore = lowestEquippedItemScore;
            return bestSlot;
        }

        private List<InventorySlot> DecideEquipmentSlots(WoWItem item)
        {
            List<InventorySlot> slots = InventoryManager.GetInventorySlotsByEquipSlot(item.ItemInfo.InventoryType);
            if (slots.Contains(InventorySlot.SecondaryHandSlot))
            {
                WoWItem mainHand = _equippedItems[InventorySlot.MainHandSlot];
                if (mainHand != null)
                {
                    InventoryType type = mainHand.ItemInfo.InventoryType;
                    if (type == InventoryType.TwoHandWeapon)
                    {
                        Log(false, "I have two handed weapon equipped therefore I can't equip {0} into secondary hand slot!", item.Name);

                        // This item takes up two slots - we have to ensure that the specific item will be checked against the main-hand.
                        slots.Clear();
                        slots.Add(InventorySlot.MainHandSlot);
                    }
                }
            }

            return slots;
        }

        private WoWItem FindBestEquipItem(IEnumerable<WoWItem> items, out float bestScore)
        {
            WoWItem bestEquipItem = null;
            float bestEquipItemScore = float.MinValue;
            foreach (WoWItem item in items)
            {
                float itemScore = AutoEquipSettings.ChosenWeightSet.EvaluateItem(item);
                if (itemScore <= bestEquipItemScore)
                    continue;

                bestEquipItemScore = itemScore;
                bestEquipItem = item;
            }

            bestScore = bestEquipItemScore == float.MinValue ? 0f : bestEquipItemScore;
            return bestEquipItem;
        }

        private void CheckForBag(IList<WoWItem> items)
        {
            WoWItem bestBag = FindBestBag(items);
            if (bestBag == null)
                return;

            for (uint b = 0; b < 4; b++)
            {
                if (ObjectManager.Me.GetBagAtIndex(b) != null)
                    continue;

                Log(true, "Equipping bag {0}", bestBag.Name);
                EquipItem(bestBag);
                break;
            }
        }

        public WoWItem FindBestBag(IList<WoWItem> items)
        {
            WoWItem bestBag = null;
            int mostSlots = int.MinValue;
            for (int i = 0; i < items.Count; i++)
            {
                WoWItem item = items[i];

                ItemInfo itemInfo = item.ItemInfo;

                if (itemInfo == null || itemInfo.InternalInfo.BagSlots <= 0 || itemInfo.InternalInfo.BagFamilyId != 0 || itemInfo.InventoryType != InventoryType.Bag)
                {
                    continue;
                }

                if (itemInfo.InternalInfo.BagSlots <= mostSlots)
                    continue;

                bestBag = item;
                mostSlots = itemInfo.InternalInfo.BagSlots;
            }

            return bestBag;
        }

        private void RefreshEquippedItems()
        {
            WoWItem[] items = ObjectManager.Me.Inventory.Equipped.Items;

            _equippedItems.Clear();
            for (int i = 0; i < 23; i++)
                _equippedItems.Add((InventorySlot)(i + 1), items[i]);
        }

        private static void Log(bool main, string format, params object[] args)
        {
            AutoEquipSettings.Log(main, format, args);
        }

        private Styx.WoWItemArmorClass GetWantedArmorClass()
        {
            switch (StyxWoW.Me.Class)
            {
                case WoWClass.DeathKnight: return WoWItemArmorClass.Plate;
                case WoWClass.Paladin:
                case WoWClass.Warrior:
                    if (StyxWoW.Me.Level < 40)
                        return WoWItemArmorClass.Mail;
                    return WoWItemArmorClass.Plate;
                case WoWClass.Hunter:
                case WoWClass.Shaman:
                    if (StyxWoW.Me.Level < 40)
                        return WoWItemArmorClass.Leather;
                    return WoWItemArmorClass.Mail;
                case WoWClass.Druid:
                case WoWClass.Rogue:
                    return WoWItemArmorClass.Leather;
                case WoWClass.Priest:
                case WoWClass.Mage:
                case WoWClass.Warlock:
                    return WoWItemArmorClass.Cloth;
            }

            return WoWItemArmorClass.None;
        }
    }
}
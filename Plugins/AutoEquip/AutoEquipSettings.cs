using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Styx;
using Styx.Helpers;
using Styx.Logic.Inventory;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace AutoEquip
{
    public static class AutoEquipSettings
    {
        private static readonly string DefaultSettingsContents = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<AutoEquipSettings>\n  <AutoEquipItems>true</AutoEquipItems>\n  <AutoEquipBags>true</AutoEquipBags>\n  <ReplaceHeirlooms>false</ReplaceHeirlooms>\n  <EquipPurples>false</EquipPurples>\n  <EquipBlues>true</EquipBlues>\n  <EquipGreens>true</EquipGreens>\n  <EquipWhites>true</EquipWhites>\n  <EquipGrays>true</EquipGrays>\n  \n  <IgnoreItems>\n    <!--Common Miscellaneous -->\n    <Item Entry=\"7005\" Name=\"Skinning Knife\" />\n    <Item Entry=\"46069\" Name=\"Alliance Lance\" />\n    <Item Entry=\"6219\" Name=\"Archlight Spanner\" />\n    <Item Entry=\"46106\" Name=\"Argent Lance\" /> \n    <Item Entry=\"5956\" Name=\"Blacksmith hammer\" />\n    <Item Entry=\"45861\" Name=\"Diamond-tipped Cane\" />\n    <Item Entry=\"45750\" Name=\"Elekk Lance [PH]\" />\n    <Item Entry=\"46070\" Name=\"Horde Lance\" />\n    <Item Entry=\"2901\" Name=\"Mining Pick\" />\n    <Item Entry=\"4616\" Name=\"Ryedol's Lucky Pick\" />\n    <Item Entry=\"45073\" Name=\"Spring Flowers\" />\n    <Item Entry=\"37708\" Name=\"Stick\" />\n    <!-- Fishing Rods -->\n    <Item Entry=\"44050\" Name=\"Mastercraft Kalu'ak Fishing Pole\" />\n    <Item Entry=\"19970\" Name=\"Arcanite Fishing Pole\" />\n    <Item Entry=\"45991\" Name=\"Bone Fishing Pole\" />\n    <Item Entry=\"45992\" Name=\"Jeweled Fishing Pole\" />\n    <Item Entry=\"45858\" Name=\"Nat's Lucky Fishing Pole\" />\n    <Item Entry=\"19022\" Name=\"Nat Pagle's Extreme Angler FC-5000\" />\n    <Item Entry=\"25978\" Name=\"Sath's Graphite Fishing Pole\" />  \n    <Item Entry=\"6367\" Name=\"Big Iron Fishing Pole\" />\n    <Item Entry=\"12225\" Name=\"Blump Family Fishing Pole\"/>\n    <Item Entry=\"6366\" Name=\"Darkwood Fishin Pole\" />\n    <Item Entry=\"6256\" Name=\"Fishing Pole\" />\n    <!-- Darkmoon Faire Common Weapons -->\n    <Item Entry=\"19292\" Name=\"Last Month's Mutton\" />\n    <Item Entry=\"19293\" Name=\"Last Year's Mutton\" />\n  </IgnoreItems>\n  \n  <IgnoreTypes>\n    <Type>Trinket</Type>\n    <Type>Ammo</Type>\n    <!-- Removing this will not remove it from the plug-in.\n    It's hardcoded - this is just an example. -->\n    <!-- Valid types are:\n    Head, Neck, Shoulder, Body, Chest, Waist, Legs, Feet, Wrist, Hand, Finger, Trinket, Weapon, Shield, Ranged, Cloak,\n    TwoHandWeapon, Bag, Tabard, Robe, WeaponMainHand, WeaponOffHand, Holdable, Ammo, Thrown,  RangedRight, Quiver, Relic\n    -->\n  </IgnoreTypes>\n  \n  <ProtectedSlots>\n    <Slot>Trinket0Slot</Slot>\n    <Slot>Trinket1Slot</Slot>\n    <Slot>AmmoSlot</Slot>\n    <!-- Removing this will not remove it from the plug-in.\n    It's hardcoded - this is just an example. -->\n    <!-- Valid slots are:\n    AmmoSlot, HeadSlot, NeckSlot, ShoulderSlot, ShirtSlot, ChestSlot, WaistSlot, LegsSlot, FeetSlot, WristSlot, HandsSlot, Finger0Slot, Finger1Slot\n    Trinket0Slot, Trinket1Slot, BackSlot, MainHandSlot, SecondaryHandSlot, RangedSlot, TabardSlot, Bag0Slot, Bag1Slot, Bag2Slot, Bag3Slot\n    -->\n  </ProtectedSlots>\n</AutoEquipSettings>";

        public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "AutoEquip"));

        static AutoEquipSettings()
        {
            AutoEquipItems = true;
            AutoEquipBags = true;
            IgnoreTypes = new HashSet<InventoryType> {InventoryType.Trinket, InventoryType.Ammo};
            ProtectedSlots = new HashSet<InventorySlot>();
            IgnoreItems = new List<KeyValuePair<uint, string>>();
        }

        #region Settings

        public static bool AutoEquipItems { get; private set; }

        public static bool AutoEquipBags { get; private set; }

        public static bool ReplaceHeirlooms { get; private set; }

        public static HashSet<InventoryType> IgnoreTypes { get; private set; }

        public static bool EquipPurples { get; private set; }

        public static bool EquipBlues { get; private set; }

        public static bool EquipGreens { get; private set; }

        public static bool EquipWhites { get; private set; }

        public static bool EquipGrays { get; private set; }

        public static HashSet<WoWItemQuality> EquipQualities
        {
            get
            {
                var temp = new HashSet<WoWItemQuality>();
                if (EquipPurples)
                    temp.Add(WoWItemQuality.Epic);

                if (EquipBlues)
                    temp.Add(WoWItemQuality.Rare);

                if (EquipGreens)
                    temp.Add(WoWItemQuality.Uncommon);

                if (EquipWhites)
                    temp.Add(WoWItemQuality.Common);

                if (EquipGrays)
                    temp.Add(WoWItemQuality.Poor);

                return temp;
            }
        }

        public static HashSet<InventorySlot> ProtectedSlots { get; private set; }

        public static List<KeyValuePair<uint, string>> IgnoreItems { get; private set; }

        #endregion

        public static WeightSet ChosenWeightSet { get; set; }

        public static bool IsItemIgnored(WoWItem item)
        {
            string itemNameLower = item.Name.ToLower();
            uint itemId = item.Entry;

            foreach (var kvp in IgnoreItems)
            {
                if (kvp.Key == itemId || kvp.Value.ToLower() == itemNameLower)
                    return true;
            }

            return false;
        }

        public static void InitializeSettings()
        {
            string settingsPath = Path.Combine(PluginFolderPath, "Settings.xml");
            if (!File.Exists(settingsPath))
            {
                StreamWriter writer = new StreamWriter(settingsPath);
                string[] lines = DefaultSettingsContents.Split('\n');
                foreach (string line in lines)
                    writer.WriteLine(line);
                writer.Flush();
                writer.Dispose();
            }

            Load(XElement.Load(settingsPath));
        }

        public static void LoadDefaultSet()
        {
            string defaultSetsPath = Path.Combine(PluginFolderPath, "AutoEquipSetDefaults.xml");
            if (!File.Exists(defaultSetsPath))
                return;

            if (ObjectManager.Me == null)
                return;
            var myGUID = ObjectManager.Me.Guid;

            XElement elm = XElement.Load(defaultSetsPath);
            XElement[] chars = elm.Elements("Char").ToArray();
            foreach (XElement cha in chars)
            {
                var guid = cha.Attributes("Guid").ToList();
                if (guid.Count <= 0)
                    continue;

                ulong guidVal;
                if (!ulong.TryParse(guid[0].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out guidVal))
                    continue;


                if (guidVal != myGUID)
                    continue;

                var set = cha.Attributes("Set").ToList();
                if (set.Count <= 0)
                    continue;

                string setName = set[0].Value;
                string pathToWeightSet = Path.Combine(PluginFolderPath, "Weight Sets\\" + setName + ".xml");
                if (!File.Exists(pathToWeightSet))
                    continue;

                try
                {
                    ChosenWeightSet = LoadWeightSetFromXML(setName, XElement.Load(pathToWeightSet));
                }
                catch (XmlException ex)
                {
                    Logging.Write("[AutoEquip]: Could not load weight set {0} - {1}", setName, ex.Message);
                }

                break;
            }
        }

        public static void SaveDefaultAutoEquipSet()
        {
            if (ChosenWeightSet == null)
                return;

            var guid = ObjectManager.Me.Guid;
            string defaultSetsPath = Path.Combine(PluginFolderPath, "AutoEquipSetDefaults.xml");
            XElement saveElm = File.Exists(defaultSetsPath) ? XElement.Load(defaultSetsPath) : new XElement("AutoEquipSetDefaults");

            XElement[] existingChars = saveElm.Elements("Char").ToArray();
            bool foundAndSaved = false;
            foreach (XElement cha in existingChars)
            {
                var guidAttr = cha.Attributes("Guid").ToArray();
                if (guidAttr.Length == 0)
                    continue;

                ulong guidVal;
                if (!ulong.TryParse(guidAttr[0].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out guidVal))
                    continue;

                if (guidVal != guid)
                    continue;

                cha.ReplaceWith(new XElement("Char", new XAttribute("Guid", guid.ToString("X", CultureInfo.InvariantCulture)), new XAttribute("Set", ChosenWeightSet.Name)));
                foundAndSaved = true;
                break;
            }

            if (!foundAndSaved)
                saveElm.Add(new XElement("Char", new XAttribute("Guid", guid.ToString("X", CultureInfo.InvariantCulture)), new XAttribute("Set", ChosenWeightSet.Name)));

            saveElm.Save(defaultSetsPath);
        }

        public static void Load(XElement xml)
        {
            XElement[] elements = xml.Elements().ToArray();
            foreach (XElement element in elements)
            {
                string name = element.Name.ToString();
                string value = element.Value;

                switch (name.ToLower())
                {
                    case "autoequipitems":
                    case "equipitems":
                        bool autoEquipItems;
                        if (!bool.TryParse(value, out autoEquipItems))
                        {
                            Log(false, "Setting {0} has wrong value - boolean value expected, True/False. Value was: {1}", name, value);
                            autoEquipItems = true;
                        }

                        AutoEquipItems = autoEquipItems;
                        break;
                    case "autoequipbags":
                    case "equipbags":
                        bool autoEquipBags;
                        if (!bool.TryParse(value, out autoEquipBags))
                        {
                            Log(false, "Setting {0} has wrong value - boolean value expected, True/False. Value was: {1}", name, value);
                            autoEquipBags = true;
                        }

                        AutoEquipBags = autoEquipBags;
                        break;
                    case "equipepics":
                    case "equipepic":
                    case "equippurples":
                    case "equippurple":
                        bool equipEpics;
                        if (!bool.TryParse(value, out equipEpics))
                        {
                            Log(false, "Setting {0} has wrong value - boolean value expected, True/False. Value was: {1}", name, value);
                            equipEpics = false;
                        }

                        EquipPurples = equipEpics;
                        break;
                    case "equiprares":
                    case "equiprare":
                    case "equipblues":
                    case "equipblue":
                        bool equipRares;
                        if (!bool.TryParse(value, out equipRares))
                        {
                            Log(false, "Setting {0} has wrong value - boolean value expected, True/False. Value was: {1}", name, value);
                            equipRares = false;
                        }

                        EquipBlues = equipRares;
                        break;
                    case "equipuncommons":
                    case "equipuncommon":
                    case "equipgreens":
                    case "equipgreen":
                        bool equipGreens;
                        if (!bool.TryParse(value, out equipGreens))
                        {
                            Log(false, "Setting {0} has wrong value - boolean value expected, True/False. Value was: {1}", name, value);
                            equipGreens = false;
                        }

                        EquipGreens = equipGreens;
                        break;
                    case "equipcommons":
                    case "equipcommon":
                    case "equipwhites":
                    case "equipwhite":
                        bool equipWhites;
                        if (!bool.TryParse(value, out equipWhites))
                        {
                            Log(false, "Setting {0} has wrong value - boolean value expected, True/False. Value was: {1}", name, value);
                            equipWhites = false;
                        }

                        EquipWhites = equipWhites;
                        break;
                    case "equippoors":
                    case "equippoor":
                    case "equipgray":
                    case "equipgrays":
                    case "equipgrey":
                    case "equipgreys":
                        bool equipGrays;
                        if (!bool.TryParse(value, out equipGrays))
                        {
                            Log(false, "Setting {0} has wrong value - boolean value expected, True/False. Value was: {1}", name, value);
                            equipGrays = false;
                        }

                        EquipGrays = equipGrays;
                        break;
                    case "replaceboa":
                    case "replaceboas":
                    case "replaceheirloom":
                    case "replaceheirlooms":
                        bool replaceHeirlooms;
                        if (!bool.TryParse(value, out replaceHeirlooms))
                        {
                            Log(false, "Setting ReplaceHeirlooms has wrong value - boolean value expected, True/False. Value was: {0}", value);
                            replaceHeirlooms = false;
                        }

                        ReplaceHeirlooms = replaceHeirlooms;
                        break;
                    case "ignoretypes":
                    case "dontequiptypes":
                        XElement[] ignoreTypes = element.Elements("Type").ToArray();
                        foreach (XElement ignoreType in ignoreTypes)
                        {
                            string ignoreTypeValue = ignoreType.Value;
                            if (string.IsNullOrEmpty(ignoreTypeValue))
                                continue;

                            InventoryType type;
                            try
                            {
                                type = (InventoryType)Enum.Parse(typeof(InventoryType), ignoreTypeValue, true);
                            }
                            catch (ArgumentException)
                            {
                                Log(false, "Ignore type \"{0}\" is unknown!", ignoreTypeValue);
                                continue;
                            }

                            if (!IgnoreTypes.Contains(type))
                                IgnoreTypes.Add(type);
                        }
                        break;
                    case "protectedslots":
                    case "protectslots":
                        XElement[] protectedSlots = element.Elements("Slot").ToArray();
                        foreach (XElement protectedSlot in protectedSlots)
                        {
                            string protectedSlotValue = protectedSlot.Value;
                            if (string.IsNullOrEmpty(protectedSlotValue))
                                continue;

                            InventorySlot slot;
                            try
                            {
                                slot = (InventorySlot)Enum.Parse(typeof(InventorySlot), protectedSlotValue, true);
                            }
                            catch (ArgumentException)
                            {
                                Log(false, "Protected slot \"{0}\" is unknown!", protectedSlotValue);
                                continue;
                            }

                            if (!ProtectedSlots.Contains(slot))
                                ProtectedSlots.Add(slot);
                        }
                        break;
                    case "ignoreitems":
                    case "protecteditems":
                        XElement[] ignoreItems = element.Elements("Item").ToArray();
                        foreach (XElement ignoreItem in ignoreItems)
                        {
                            uint ignoreItemId = 0;
                            string ignoreItemName = "";

                            XAttribute[] ignoreItemAttribs = ignoreItem.Attributes().ToArray();
                            foreach (XAttribute ignoreItemAttrib in ignoreItemAttribs)
                            {
                                string attribName = ignoreItemAttrib.Name.ToString();
                                string attribValue = ignoreItemAttrib.Value;

                                switch (attribName.ToLower())
                                {
                                    case "id":
                                    case "entry":
                                        if (!uint.TryParse(attribValue, out ignoreItemId))
                                            Log(false, "Setting {0} has wrong value - positive number expected, 0 or > 0. Value was: {1}", name, value);

                                        break;
                                    case "name":
                                        ignoreItemName = attribValue;
                                        break;
                                }
                            }

                            IgnoreItems.Add(new KeyValuePair<uint, string>(ignoreItemId, ignoreItemName));
                        }
                        break;
                }
            }
        }

        public static WeightSet LoadWeightSetFromXML(string weightSetName, XContainer weightElm)
        {
            if (string.IsNullOrEmpty(weightSetName))
                throw new ArgumentNullException("weightSetName", "The weight set name can not be null or empty!");

            var weightDict = new Dictionary<Stat, float>();
            XElement[] elements = weightElm.Elements().ToArray();

            foreach (XElement element in elements)
            {
                string name = element.Name.ToString();
                Stat stat;
                try
                {
                    stat = (Stat)Enum.Parse(typeof(Stat), name, true);
                }
                catch (ArgumentException)
                {
                    Log(false, "Unknown stat name {0} while parsing weight set {1}. Skipping stat.", name, weightSetName);
                    continue;
                }

                float weight;
                if (!float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out weight))
                {
                    Log(false, "Invalid stat value {0} for stat {1} while parsing weight set {2}. Floating value expected. Skipping stat.", element.Value, name, weightSetName);
                    continue;
                }

                if (weightDict.ContainsKey(stat))
                {
                    Log(false, "Weight set {0} contains duplicate stat {1}", weightSetName, stat);
                    continue;
                }

                weightDict.Add(stat, weight);
            }

            return new WeightSet(weightSetName, weightDict);
        }

        internal static void Log(bool main, string format, params object[] args)
        {
			if (main)
				Logging.Write("[AutoEquip]: {0}", string.Format(format, args));
			else
				Logging.WriteDebug("[AutoEquip]: {0}", string.Format(format, args));
        }
    }
}
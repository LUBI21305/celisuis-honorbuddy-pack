//!CompilerOption:Optimize:On

using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames;
using Styx.Patchables;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWCache;
using Styx.WoWInternals.WoWObjects;
using System.Collections.ObjectModel;
using Styx.Logic.Combat;
using Styx.Helpers;
using System.Diagnostics;
using System.IO;

namespace HighVoltz
{
    #region TradeSkillFrame
    public class TradeSkillFrame : Frame
    {
        // needs to be moved to offsets enum

        //private static uint TradeskillOffset = 0xA92550; // wow 4.1
        //static uint TradeskillOffset = 0xB0D5E8; // wow 4.2
        internal struct SkillOffset
        {
            public const uint Guid = 0; // guid of player who's tradeskill is shown, could be someone besides localplayer.
            public const uint SpellID = 0x08; // 0 if linked 
            public const uint ShownSkill = 0x0C; // skill Id, doesn't presist after tradeskill frame has been closed

            public const uint SelectedRecipeID = 0x10;// 0 if no recipes are shown
            public const uint IsLoading = 0x14; // non-zero if waiting on server to send info
            public const uint TotalRecipeCount = 0x18;
            public const uint SubClassFilterNum = 0x1C;

            public const uint ShownRecipeNum = 0x20;   // number of recipes shown
            public const uint SearchStringPtr = 0x24;
            public const uint MinItemLevelFilter = 0x28;
            public const uint MaxItemLevelFilter = 0x2C;

            public const uint HaveMaterialsFilter = 0x30;   // 1 on, 0 off
            public const uint HaveSkillupFilter = 0x34;   // 1 on, 0 off
            public const uint Unknown1 = 0x38;   // filter related.
            public const uint QueuedRecipeID1 = 0x3C;

            public const uint QueuedRecipeRepeat1 = 0x40;
            public const uint QueuedRecipeID2 = 0x44;
            public const uint QueuedRecipeRepeat2 = 0x48;
            public const uint Unknown2 = 0x4C;   // related to linked tradeskill...

            public const uint LinkedSkillLevel = 0x50;
            public const uint LinkedSkillMaxLevel = 0x54;
            public const uint IsLinked = 0x58; // 0x0001 = any linked profession, 0x0010 = guild profession, though this is bugged on live and not working
            public const uint LoadedSkill = 0x5C; // skill id, presists after tradeskill frame is closed

            //public const uint NewUnknown1 = 0x60;      // (Added WoW 4.2) hmm what could this be? cba to check 
            //public const uint NewUnknown2 = 0x64;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown3 = 0x68;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown4 = 0x6C;      // (Added WoW 4.2) hmm what could this be? cba to check

            //public const uint NewUnknown5 = 0x70;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown6 = 0x74;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown7 = 0x78;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown8 = 0x7C;      // (Added WoW 4.2) hmm what could this be? cba to check

            //public const uint NewUnknown9 = 0x80;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown10 = 0x84;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown11 = 0x88;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown12 = 0x8C;      // (Added WoW 4.2) hmm what could this be? cba to check

            //public const uint NewUnknown13 = 0x90;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown14 = 0x94;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown15 = 0x98;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown16 = 0x9C;      // (Added WoW 4.2) hmm what could this be? cba to check

            //public const uint NewUnknown17 = 0xA0;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown18 = 0xA4;      // (Added WoW 4.2) hmm what could this be? cba to check
            //public const uint NewUnknown19 = 0xA8;      // (Added WoW 4.2) hmm what could this be? cba to check
            public const uint Unknown3 = 0xAC; // related to recipeArraySize, often same number

            public const uint RecipeArraySize = 0xB0; // use TotalRecipeCount instead since there can be linkering recipes from a previously loaded tradeskill
            public const uint RecipeArray = 0xB4; // array of pointers
            public const uint Unknown4 = 0xB8;     // nothing important, cba to reverse it
            public const uint Unknown5 = 0xBC;       // probably has a similar purpose as Unknown3

            public const uint FilterArraySize = 0xC0;// use SubClassFilterNum instead since there can be linkering filters not yet disposed
            public const uint FilterArrayPtr = 0xC4; // filters... cba to reverse
            public const uint Unknown6 = 0xC8;       // probably has a similar purpose as Unknown4
            public const uint Unknown7 = 0xCC;       // hmm what could this be? cba to check.. looks like its 64bit(long)

            public const uint Unknown9 = 0xD4;       // hmm what could this be? cba to check
            public const uint Unknown10 = 0xD8;      // hmm what could this be? cba to check
            public const uint Unknown11 = 0xDC;      // hmm what could this be? cba to check

            public const uint Unknown12 = 0xE0;      // hmm what could this be? cba to check
            public const uint Unknown13 = 0xE4;      // hmm what could this be? cba to check
            public const uint Unknown14 = 0xE8;      // hmm what could this be? cba to check
        }

        public readonly static List<SkillLine> SupportedSkills = new List<SkillLine>
        {
            SkillLine.Alchemy,
            SkillLine.Blacksmithing,
            SkillLine.Cooking,
            SkillLine.Enchanting,
            SkillLine.Engineering,
            SkillLine.FirstAid,
            SkillLine.Inscription,
            SkillLine.Jewelcrafting,
            SkillLine.Leatherworking,
            SkillLine.Mining,// smelting
            SkillLine.Tailoring,
            SkillLine.Archaeology,
        };

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static readonly TradeSkillFrame Instance = new TradeSkillFrame();
        private static readonly uint _baseAddress;

        static TradeSkillFrame()
        {
            ProcessModule mod = ObjectManager.WoWProcess.MainModule;
            uint baseAddress = (uint)mod.BaseAddress;
            if (ProfessionBuddySettings.Instance.WowVersion != mod.FileVersionInfo.FileVersion ||
                ProfessionBuddySettings.Instance.TradeskillFrameOffset == 0)
            {
                Professionbuddy.Log("A new wow version has been detected\nScanning for new TradeskillFrame offset");
                try
                {
                    uint pointer = Util.FindPattern("89 0D 00 00 00 00 8B 50 04 8B 45 0C 53 33 DB 89 15 00 00 00 00 89 1D 00 00 00 00 8B 08 89 0D 00 00 00 00 8B 50 04 89 15",
                        "xx????xxxxxxxxxxx????xx????xxxx????xxxxx") + 2;
                    ProfessionBuddySettings.Instance.TradeskillFrameOffset = ObjectManager.Wow.Read<uint>(baseAddress + pointer) - baseAddress;
                    ProfessionBuddySettings.Instance.WowVersion = mod.FileVersionInfo.FileVersion;
                    Professionbuddy.Log("Found TradeskillFrame offset for WoW Version {0} at offset 0x{1:X}",
                        mod.FileVersionInfo.FileVersion, ProfessionBuddySettings.Instance.TradeskillFrameOffset);
                    ProfessionBuddySettings.Instance.Save();
                }
                catch (InvalidDataException)
                {
                    Professionbuddy.Log("Unable to find TradeskillFrame offset for WoW Version {0}\nPlease notify the developer of this issue",
                        mod.FileVersionInfo.FileVersion);
                }
            }
            _baseAddress = (uint)ObjectManager.WoWProcess.MainModule.BaseAddress + ProfessionBuddySettings.Instance.TradeskillFrameOffset;
        }

        public TradeSkillFrame()
            : base("TradeSkillFrame")
        {
        }

        public static string GetItemCacheName(uint id)
        {
            var cache = Styx.StyxWoW.Cache[CacheDb.Item].GetInfoBlockById(id);
            if (cache != null)
                return ObjectManager.Wow.Read<string>(cache.ItemSparse.Name);
            else
                return null;
        }

        /// <summary>
        /// Returns a list of currently loaded Recipes
        /// </summary>
        public TradeSkill GetTradeSkill()
        {
            return GetTradeSkill(Skill, false);
        }

        public TradeSkill GetTradeSkill(SkillLine skillLine)
        {
            return GetTradeSkill(skillLine, false);
        }

        /// <summary>
        /// Returns a list of recipes from selected skill
        /// </summary>
        /// <param name="skillLine"></param>
        /// <param name="blockFrame">prevents tradeskill frame from showing</param>
        /// <returns></returns>
        public TradeSkill GetTradeSkill(SkillLine skillLine, bool blockFrame)
        {
            if (!ObjectManager.IsInGame)
                throw new InvalidOperationException("Must Be in game to call GetTradeSkill()");
            if (skillLine == 0 || !SupportedSkills.Contains(skillLine))
                throw new InvalidOperationException(string.Format("The tradekill {0} can not be loaded", skillLine));
            // if HB is not running then we need to pulse objectmanger for item counts
            if (!TreeRoot.IsRunning)
                ObjectManager.Update();
            WoWSkill wowSkill = ObjectManager.Me.GetSkill(skillLine);

            TradeSkill tradeSkill = new TradeSkill(wowSkill);

            //lets copy over to a local variable for performance
            bool _isVisible = IsVisible;
            bool loadSkill = Skill != skillLine || !_isVisible;
            string lua = string.Format("{0}{1}{2}{3}",
                blockFrame && loadSkill ? "if not TradeSkillFrame then LoadAddOn('Blizzard_TradeSkillUI') end local fs = {} local f = EnumerateFrames() while f do if f:IsEventRegistered('TRADE_SKILL_SHOW') == 1 and f:GetName() ~= 'UIParent' then f:UnregisterEvent('TRADE_SKILL_SHOW') table.insert (fs,f) end f = EnumerateFrames(f) end " : "",
                // have to hard code in mining since I need to cast 'Smelting' not 'Mining' XD
                // also using lua over SpellManager.Cast since its grabing name from DBC. one word, localization.
                loadSkill ? (skillLine == SkillLine.Mining ? "CastSpellByID(2656) " : string.Format("CastSpellByName('{0}') ", wowSkill.Name)) : "",
                // force cache load
                "for i=1, GetNumTradeSkills() do GetTradeSkillItemLink(i) for n=1,GetTradeSkillNumReagents(i) do GetTradeSkillReagentInfo(i,n) end end ",
                blockFrame && loadSkill ? "for k,v in pairs(fs) do v:RegisterEvent('TRADE_SKILL_SHOW') end CloseTradeSkill() " : ""
            );
            using (new FrameLock())
            {
                if (!string.IsNullOrEmpty(lua))
                    Lua.DoString(lua);

                int _recipeCount = RecipeCount;
                if (Skill != skillLine || _recipeCount <= 0)
                {// we failed to load tradeskill
                    throw new Exception(string.Format("Unable to load {0}", skillLine));
                }
                // array of pointers that point to each recipe structure.
                uint[] recipePtrArray = ObjectManager.Wow.ReadStructArray<uint>(RecipeOffset, RecipeCount);
                for (int index = 0; index < _recipeCount; index++)
                {
                    uint[] recipeData = ObjectManager.Wow.ReadStructArray<uint>(recipePtrArray[index], 9);
                    uint id = recipeData[(int)Recipe.RecipeIndex.RecipeID];
                    // check if its a header
                    if (id == uint.MaxValue)
                    {
                        continue; // no further info to get for header
                    }
                    tradeSkill.Recipes.Add(id, new Recipe(recipeData, tradeSkill, skillLine));
                }
                tradeSkill.InitIngredientList();
                tradeSkill.InitToolList();
            }
            return tradeSkill;
        }

        /// <summary>
        /// Returns Guid of the player who's tradeskill is shown
        /// </summary>
        ulong Guid
        {
            get { return ObjectManager.Wow.Read<ulong>(_baseAddress + SkillOffset.Guid); }
        }
        /// <summary>
        /// Returns true if the currently loaded tradeskill is from a tradeskill link
        /// </summary>
        public bool IsLinked
        {
            get { return ObjectManager.Wow.Read<bool>(_baseAddress + SkillOffset.IsLinked); }
        }

        new public bool IsVisible
        {
            get { return ObjectManager.Wow.Read<bool>(_baseAddress + SkillOffset.ShownSkill); }
        }
        public bool IsLoading
        {
            get { return ObjectManager.Wow.Read<bool>(_baseAddress + SkillOffset.IsLoading); }
        }
        /// <summary>
        /// Name of currently loaded tradeskill
        /// </summary>
        public string Name
        {
            get
            {
                return ObjectManager.Me.GetSkill(Skill).Name;
            }
        }
        /// <summary>
        /// ID of the recipe that's currently being crafted
        /// </summary>
        public int QueuedRecipeID
        {
            get
            {
                int recipe1 = ObjectManager.Wow.Read<int>(_baseAddress + SkillOffset.QueuedRecipeID1);
                int recipe2 = ObjectManager.Wow.Read<int>(_baseAddress + SkillOffset.QueuedRecipeID2);
                return recipe1 != 0 ? recipe1 : recipe2;
            }
        }
        /// <summary>
        /// Number of recipes, this includes headers
        /// </summary>
        public int RecipeCount
        {
            get { return ObjectManager.Wow.Read<int>(_baseAddress + SkillOffset.TotalRecipeCount); }
        }
        // offset to the recipe array of pointers.
        uint RecipeOffset
        {
            get { return ObjectManager.Wow.Read<uint>(_baseAddress + SkillOffset.RecipeArray); }
        }
        /// <summary>
        /// Number of times a recipe is set to repeat itself
        /// </summary>
        public int RepeatQueueCount
        {
            get
            {
                int repeat1 = ObjectManager.Wow.Read<int>(_baseAddress + SkillOffset.QueuedRecipeRepeat1);
                int repeat2 = ObjectManager.Wow.Read<int>(_baseAddress + SkillOffset.QueuedRecipeRepeat2);
                return repeat1 != 0 ? repeat1 : repeat2;
            }
        }
        /// <summary>
        /// Number of recipes currently shown (filtered), this includes headers
        /// </summary>
        int ShownRecipeCount
        {
            get { return ObjectManager.Wow.Read<int>(_baseAddress + SkillOffset.ShownRecipeNum); }
        }
        /// <summary>
        /// Opens TradeSkill Frame
        /// </summary>
        override public void Show()
        {
            Show(Skill);
        }
        /// Opens TradeSkill Frame for the specific skill
        /// </summary>
        /// <param name="skillLine"></param>
        public void Show(SkillLine skillLine)
        {
            if (skillLine != 0 && ObjectManager.IsInGame && !IsVisible)
            {
                WoWSkill wowSkill = ObjectManager.Me.GetSkill(skillLine);
                bool loadSkill = Skill != skillLine || !IsVisible;
                // have to hard code in mining since I need to cast 'Smelting' not 'Mining' XD
                // also using lua over SpellManager.Cast since its grabing name from DBC. one word, localization.
                string lua = loadSkill ? (skillLine == SkillLine.Mining ? "CastSpellByID(2656);" : string.Format("CastSpellByName('{0}');", wowSkill.Name)) : null;
                if (lua != null)
                    Lua.DoString(lua);
            }
        }
        /// <summary>
        /// Returns the skill of currently loaded tradeskill
        /// </summary>
        public SkillLine Skill
        {
            get { return (SkillLine)ObjectManager.Wow.Read<int>(_baseAddress + SkillOffset.LoadedSkill); }
        }
        /// <summary>
        /// Updates the skill level, recipe difficulty and adds new recipes.
        /// </summary>
        /// <param name="tradeSkill"></param>
        public void UpdateTradeSkill(TradeSkill tradeSkill, bool blockFrame)
        {
            if (!ObjectManager.IsInGame || tradeSkill == null)
            {
                return;
            }
            // if HB is not running then we should maybe pulse objectmanger for item counts
            if (!TreeRoot.IsRunning)
                ObjectManager.Update();

            tradeSkill.WoWSkill = ObjectManager.Me.GetSkill(tradeSkill.SkillLine);
            //lets copy over to a local variable for performance
            bool _isVisible = IsVisible;
            bool loadSkill = Skill != tradeSkill.SkillLine || !_isVisible;
            string lua = string.Format("{0}{1}{2}{3}",
                blockFrame && loadSkill ? "if not TradeSkillFrame then LoadAddOn('Blizzard_TradeSkillUI') end local fs = {} local f = EnumerateFrames() while f do if f:IsEventRegistered('TRADE_SKILL_SHOW') == 1 then f:UnregisterEvent('TRADE_SKILL_SHOW') table.insert (fs,f) end f = EnumerateFrames(f) end " : "",
                // have to hard code in mining since I need to cast 'Smelting' not 'Mining' XD
                // also using lua over SpellManager.Cast since its grabing name from DBC. one word, localization.
                 loadSkill ? (tradeSkill.SkillLine == SkillLine.Mining ? "CastSpellByID(2656) " : string.Format("CastSpellByName('{0}') ", tradeSkill.Name)) : "",
                "for i=1, GetNumTradeSkills() do GetTradeSkillItemLink(i) for n=1,GetTradeSkillNumReagents(i) do GetTradeSkillReagentInfo(i,n) end end ",
                blockFrame && loadSkill ? "for k,v in pairs(fs) do v:RegisterEvent('TRADE_SKILL_SHOW') end CloseTradeSkill() " : ""
                );
            // using framelock here to lock memory while I'm reading from it.. since it's changing constantly..
            using (new FrameLock())
            {
                if (!string.IsNullOrEmpty(lua))
                    Lua.DoString(lua);

                if (Skill != tradeSkill.SkillLine)
                {// we failed to load tradeskill
                    throw new Exception("Fail to update Tradeskill " + tradeSkill.SkillLine.ToString());
                }
                // array of pointers that point to each recipe structure.
                uint[] recipePtrArray = ObjectManager.Wow.ReadStructArray<uint>(RecipeOffset, RecipeCount);
                int _recipeCount = RecipeCount;
                for (int index = 0; index < _recipeCount; index++)
                {
                    uint[] recipeData = ObjectManager.Wow.ReadStructArray<uint>(recipePtrArray[index], 9);
                    uint id = recipeData[(int)Recipe.RecipeIndex.RecipeID];
                    // check if its a header
                    if (id == uint.MaxValue)
                    {
                        continue; // no further info to get for header
                    }
                    if (tradeSkill.Recipes.ContainsKey(id))
                    {
                        tradeSkill.Recipes[id].Update(recipeData);
                    }
                    else
                    {
                        tradeSkill.Recipes.Add(id, new Recipe(recipeData, tradeSkill, tradeSkill.SkillLine));
                    }
                }
            }
        }
    }
    #endregion

    #region TradeSkill
    public class TradeSkill
    {
        public TradeSkill(WoWSkill skill)
        {
            this.WoWSkill = skill;
            Recipes = new Dictionary<uint, Recipe>();
        }
        public WoWSkill WoWSkill { get; internal set; }
        public void AddRecipe(Recipe recipe)
        {
            if (Recipes.ContainsKey(recipe.ID))
                return;
            Recipes.Add(recipe.ID, recipe);
            recipe.InitIngredients();
            recipe.InitTools();
        }
        /// <summary>
        /// Amount of Bonus the player has to the Tradeskill 
        /// </summary>
        public uint Bonus { get { return WoWSkill.Bonus; } }
        /// <summary>
        /// Tradeskill level
        /// </summary>
        public int Level { get { return WoWSkill.CurrentValue; } }
        /// <summary>
        /// Maximum level of the Tradeskill 
        /// </summary>
        public int MaxLevel { get { return WoWSkill.MaxValue; } }
        /// <summary>
        /// Name of the Tradeskill
        /// </summary>
        public string Name { get { return WoWSkill.Name; } }
        public SkillLine SkillLine { get { return (SkillLine)WoWSkill.Id; } }
        /// <summary>
        /// List of ingredients 
        /// </summary>
        public Dictionary<uint, IngredientSubClass> Ingredients
        {
            get
            {
                if (ingredients == null)
                {
                    InitIngredientList();
                }
                return ingredients;
            }
        }

        internal void InitIngredientList()
        {
            ingredients = new Dictionary<uint, IngredientSubClass>();
            foreach (var recipePair in Recipes)
            {
                recipePair.Value.InitIngredients();
            }

        }

        Dictionary<uint, IngredientSubClass> ingredients;
        /// <summary>
        /// List of Tools
        /// </summary>
        public List<Tool> Tools
        {
            get
            {
                if (tools == null)
                {
                    InitToolList();
                }
                return tools;
            }
        }
        List<Tool> tools;
        internal void InitToolList()
        {
            tools = new List<Tool>();
            foreach (var recipePair in Recipes)
            {
                recipePair.Value.InitTools();
            }
        }

        /// <summary>
        /// List of Recipes
        /// </summary>
        public Dictionary<uint, Recipe> Recipes { get; internal set; }
        /// <summary>
        /// Syncs Ingredient and Tool list with Bags
        /// </summary>
        public void PulseBags()
        {
            if (!TreeRoot.IsRunning)
                ObjectManager.Update();
            foreach (var ingredPair in Ingredients)
            {
                ingredPair.Value.UpdateInBagsCount();
            }
            foreach (var tool in Tools)
            {
                tool.UpdateToolStatus();
            }
        }
        // syncs the TradeSkill, updating Skill level,recipe dificulty and adding new recipes that arent in list
        public void PulseSkill()
        {
            TradeSkillFrame.Instance.UpdateTradeSkill(this, true);
        }
        /// <summary>
        /// number of Recipes
        /// </summary>
        public int RecipeCount
        {
            get { return Recipes.Count; }
        }

    }
    #endregion

    #region Recipe
    public class Recipe
    {
        internal TradeSkill parent;
        internal struct RecipeIndex
        {
            public const uint RecipeID = 0;
            public const uint RecipeDifficulty = 1;
            public const uint SubClass1 = 2;
            public const uint SubClass2 = 3;
            public const uint CanRepeatNum = 6;
            public const uint TurnsGrayLevel = 7;
            public const uint Skillups = 8;
        }
        enum SpellDB
        {
            NamePtr = 21,
            SpellCastingReqIndex = 34,
            SpellReagentsIndex = 43,
            SpellTotemsIndex = 46,
        };
        public enum RecipeDifficulty
        {
            optimal, medium, easy, trivial
        }
        uint[] recipeData;
        internal Recipe(uint[] data, TradeSkill parent, SkillLine skill)
        {
            this.recipeData = data;
            this.Skill = skill;
            this.parent = parent;
        }
        /// <summary>
        /// Returns the color that represents the recipes difficulty
        /// </summary>
        public System.Drawing.Color Color
        {
            get
            {
                switch (Difficulty)
                {
                    case RecipeDifficulty.optimal:
                        return System.Drawing.Color.DarkOrange;
                    case RecipeDifficulty.medium:
                        return System.Drawing.Color.Yellow;
                    case RecipeDifficulty.easy:
                        return System.Drawing.Color.Lime;
                    default:
                        return System.Drawing.Color.Gray;
                }
            }
        }
        /// <summary>
        /// The Number of times recipe can be crafted with current mats in bags using internal Ingredient list.
        /// </summary>
        public uint CanRepeatNum
        {
            get
            {
                uint repeat = uint.MaxValue;
                foreach (Ingredient ingred in Ingredients)
                {
                    uint cnt = (uint)System.Math.Floor((double)(ingred.InBagItemCount / ingred.Required));
                    if (repeat > cnt)
                    {
                        repeat = cnt;
                    }
                }
                return repeat;
            }
        }
        /// <summary>
        /// The Number of times recipe can be crafted with current mats in bags. Checks directly with ObjectManager
        /// </summary>
        public uint CanRepeatNum2
        {
            get
            {
                uint repeat = uint.MaxValue;
                foreach (Ingredient ingred in Ingredients)
                {
                    uint cnt = (uint)System.Math.Floor((double)
                        (Ingredient.GetInBagItemCount(ingred.ID) / ingred.Required));
                    if (repeat > cnt)
                    {
                        repeat = cnt;
                    }
                }
                return repeat;
            }
        }
        /// <summary>
        /// Returns ID of the item the recipe makes
        /// </summary>
        public uint CraftedItemID { get { return (uint)(craftedItemID ?? (craftedItemID = GetCraftedItemID())); } }
        uint? craftedItemID;

        uint? GetCraftedItemID()
        {
            if (Spell != null)
            {
                return Spell.SpellEffect1.ItemType;
            }
            else
                return null;
        }

        /// <summary>
        /// Returns the difficulty of the Recipe
        /// </summary>
        public RecipeDifficulty Difficulty
        {
            get { return (RecipeDifficulty)recipeData[(int)RecipeIndex.RecipeDifficulty]; }
        }
        internal void InitIngredients()
        {  // instantizing ingredients in here and doing a null check to prevent recursion from Trade.Ingredients() 
            if (ingredients != null)
                return;
            uint recipeID = ID;
            ingredients = new List<Ingredient>();
            WoWDb.DbTable SpelldbTable;
            WoWDb.Row SpelldbRow;
            SpelldbTable = StyxWoW.Db[ClientDb.Spell];
            if (SpelldbTable != null && recipeID <= SpelldbTable.MaxIndex && recipeID >= SpelldbTable.MinIndex)
            {
                SpelldbRow = SpelldbTable.GetRow((uint)recipeID);
                if (SpelldbRow != null)
                {
                    uint reagentIndex = SpelldbRow.GetField<uint>((uint)SpellDB.SpellReagentsIndex);// Changed to 43 in WoW 4.2
                    WoWDb.DbTable reagentDbTable = StyxWoW.Db[ClientDb.SpellReagents];
                    if (reagentDbTable != null && reagentIndex <= reagentDbTable.MaxIndex &&
                        reagentIndex >= reagentDbTable.MinIndex)
                    {
                        WoWDb.Row reagentDbRow = reagentDbTable.GetRow(reagentIndex);
                        for (uint index = 1; index <= 8; index++)
                        {
                            uint id = reagentDbRow.GetField<uint>(index);
                            if (id != 0)
                            {
                                ingredients.Add(new Ingredient(id, reagentDbRow.GetField<uint>(index + 8), parent.Ingredients));
                            }
                        }
                    }
                }
            }
        }

        string GetHeader()
        {
            WoWDb.DbTable dbTable;
            WoWDb.Row dbRow;
            string header = "";
            // if the subclasses are both uint.MaxValue we grab the header name from ClientDb.SkillLine(6)
            if (recipeData[RecipeIndex.SubClass1] == uint.MaxValue && recipeData[(int)RecipeIndex.SubClass2] == uint.MaxValue)
            {
                dbTable = Styx.StyxWoW.Db[ClientDb.SkillLine];
                dbRow = dbTable.GetRow((uint)Skill);
                header = ObjectManager.Wow.Read<string>(dbRow.GetField<uint>(6));
            }
            else
            {
                // we need to iterate through ClientDb.ItemSubClass till we find matching
                dbTable = Styx.StyxWoW.Db[ClientDb.ItemSubClass];
                for (int i = dbTable.MinIndex; i <= dbTable.MaxIndex; i++)
                {
                    dbRow = dbTable.GetRow((uint)i);
                    int iSubClass1 = dbRow.GetField<int>(1);
                    int iSubClass2 = dbRow.GetField<int>(2);
                    if (iSubClass1 == recipeData[(int)RecipeIndex.SubClass1] &&
                        iSubClass2 == recipeData[(int)RecipeIndex.SubClass2])
                    {
                        uint stringPtr = dbRow.GetField<uint>(12);
                        // if pointer in field (12) is 0 or it points to null string then use field (11)
                        if (stringPtr == 0 ||
                            string.IsNullOrEmpty(header = ObjectManager.Wow.Read<string>(stringPtr)))
                        {
                            stringPtr = dbRow.GetField<uint>(11);
                            if (stringPtr != 0)
                                header = ObjectManager.Wow.Read<string>(stringPtr);
                        }
                        break;
                    }
                }
            }
            return header;
        }
        // grab name from dbc
        string GetName()
        {
            string name = null;
            WoWDb.DbTable t = StyxWoW.Db[Styx.Patchables.ClientDb.Spell];
            WoWDb.Row r = t.GetRow((uint)ID);
            uint stringPtr = r.GetField<uint>((uint)SpellDB.NamePtr);
            if (stringPtr != 0)
            {
                name = ObjectManager.Wow.Read<string>(stringPtr);
            }
            return name;
        }
        // grab name from dbc
        internal void InitTools()
        { // instantizing tools in here and doing a null check to prevent recursion from Trade.Tools() 
            if (tools != null)
                return;
            tools = new List<Tool>();
            WoWDb.DbTable t = StyxWoW.Db[Styx.Patchables.ClientDb.Spell];
            WoWDb.Row spellDbRow = t.GetRow((uint)ID);
            uint spellReqIndex = spellDbRow.GetField<uint>((uint)SpellDB.SpellCastingReqIndex); // changed from 33 to 34 in WOW 4.2
            if (spellReqIndex != 0)
            {
                t = StyxWoW.Db[Styx.Patchables.ClientDb.SpellCastingRequirements];
                WoWDb.Row spellReqDbRow = t.GetRow(spellReqIndex);
                uint spellFocusIndex = spellReqDbRow.GetField<uint>(6);
                // anvils, forge etc
                if (spellFocusIndex != 0)
                {
                    tools.Add(GetTool(spellFocusIndex, Tool.ToolType.SpellFocus));
                }
                uint areaGroupIndex = spellReqDbRow.GetField<uint>(4);
                if (areaGroupIndex != 0)
                {
                    t = StyxWoW.Db[Styx.Patchables.ClientDb.AreaGroup];
                    WoWDb.Row areaGroupDbRow = t.GetRow(areaGroupIndex);
                    uint areaTableIndex = areaGroupDbRow.GetField<uint>(1);
                    // not sure which kind of tools this covers
                    if (areaTableIndex != 0)
                    {
                        tools.Add(GetTool(areaTableIndex, Tool.ToolType.AreaTable));
                    }
                }
            }
            uint spellTotemsIndex = spellDbRow.GetField<uint>((uint)SpellDB.SpellTotemsIndex); // changed from 45 to 46 in WOW 4.2
            if (spellTotemsIndex != 0)
            {
                t = StyxWoW.Db[Styx.Patchables.ClientDb.SpellTotems];
                WoWDb.Row spellTotemsDbRow = t.GetRow(spellTotemsIndex);
                uint cacheIndex = 0, totemCategoryIndex = 0;
                for (uint i = 1; i <= 4 && cacheIndex == 0; i++)
                {
                    if (cacheIndex == 0 && i >= 3)
                        cacheIndex = spellTotemsDbRow.GetField<uint>(i);
                    if (totemCategoryIndex == 0 && i <= 2)
                        totemCategoryIndex = spellTotemsDbRow.GetField<uint>(i);
                }
                // not sure which kind of tools this covers
                if (cacheIndex != 0)
                {
                    tools.Add(GetTool(cacheIndex, Tool.ToolType.Item));
                }
                // Blacksmith hammer, mining pick 
                if (totemCategoryIndex != 0)
                {
                    t = StyxWoW.Db[Styx.Patchables.ClientDb.TotemCategory];
                    spellTotemsDbRow = t.GetRow(totemCategoryIndex);
                    tools.Add(GetTool(totemCategoryIndex, Tool.ToolType.Totem));
                }
            }
        }
        // this basically checks if the master tool list already contains this tool and 
        // returns that tool if it does, otherwise it adds the tool to the master Tool list
        // and returns it.
        Tool GetTool(uint index, Tool.ToolType toolType)
        {
            Tool newTool = new Tool(index, toolType);
            Tool tool = parent.Tools.Find(a => a.Equals(newTool));
            if (tool == null)
            {
                tool = newTool;
                parent.Tools.Add(tool);
            }
            return tool;
        }
        /// <summary>
        /// Name of header this recipe belongs to
        /// </summary>
        public string Header
        {
            get { return header ?? (header = GetHeader()); }
        }
        string header;
        /// <summary>
        /// List of ingredients required for the recipe
        /// </summary>
        public ReadOnlyCollection<Ingredient> Ingredients
        {
            get
            {
                if (ingredients == null)
                    InitIngredients();
                return ingredients.AsReadOnly();
            }
        }
        List<Ingredient> ingredients;
        /// <summary>
        /// Name of the Recipe
        /// </summary>
        public string Name
        {
            get { return name ?? (name = GetName()); }
        }
        string name;

        public uint ID { get { return recipeData[RecipeIndex.RecipeID]; } }
        /// <summary>
        /// The Skill this recipe is from
        /// </summary>
        public SkillLine Skill { get; private set; }
        /// <summary>
        /// Number of Skillup this recipe will grant when crafted
        /// </summary>
        public uint Skillups { get { return recipeData[RecipeIndex.Skillups]; } }
        /// <summary>
        /// returns the spell that's attached to the recipe
        /// </summary>
        public WoWSpell Spell
        {
            get
            {
                return WoWSpell.FromId((int)ID);
            }
        }
        public ReadOnlyCollection<Tool> Tools
        {
            get
            {
                if (tools == null)
                    InitTools();
                return tools.AsReadOnly();
            }
        }
        List<Tool> tools;

        internal void Update(uint[] data)
        {
            this.recipeData = data;
        }
    }
    #endregion

    #region IngredientSubClass
    // This class is used in a dictionary in the TradeSkill class and the purpose is to 
    // keep track of the number of ingredients in players bag,updated via Pulse().
    // this saves cpu cycles because it doesnt need to iterate through all the recipes.
    // Also this saves memory usage because each ingredient name gets loaded only once per ingredient.

    // someone give this a good name, kthz
    public class IngredientSubClass
    {
        internal Ingredient parent;
        internal IngredientSubClass(Ingredient parent, uint inBagCount)
        {
            this.parent = parent;
            this.InBagsCount = inBagCount;
        }
        static object _nameLockObject = new object();
        public string Name
        {
            get { lock (_nameLockObject) { return name ?? (name = GetName()); } }
        }

        string GetName()
        {
            string _name = TradeSkillFrame.GetItemCacheName(parent.ID);
            if (_name == null)
            {
                // ok so it couldn't find the item in cache. since we're going to need to force a load
                // might as well do a framelock and try load all the items. 
                IEnumerable<uint> ids = parent.masterList.Keys.Where(id => id != parent.ID);

                using (new FrameLock())
                {
                    foreach (var id in ids)
                    {
                        name = TradeSkillFrame.GetItemCacheName(id);
                    }
                }
            }
            return _name;
        }
        string name;
        /// <summary>
        /// number of ingredients in players bags
        /// </summary>
        public uint InBagsCount { get; internal set; }
        /// <summary>
        /// updates the InBagsCount
        /// </summary>
        internal void UpdateInBagsCount()
        {
            InBagsCount = Ingredient.GetInBagItemCount(parent.ID);
        }
    }
    #endregion

    #region Ingredient
    public class Ingredient
    {
        // someone give this a good name, kthz
        IngredientSubClass subclass;
        // list where every ingredient is stored, used to save memory usage,
        // this points to the one initilized in a TradeSkill instance
        internal Dictionary<uint, IngredientSubClass> masterList;
        internal Ingredient(uint id, uint requiredNum, Dictionary<uint, IngredientSubClass> masterList)
        {
            this.ID = id;
            this.Required = requiredNum;
            this.masterList = masterList;
            if (masterList.ContainsKey(id))
            {
                subclass = masterList[id];
            }
            else
            {
                subclass = new IngredientSubClass(this, GetInBagItemCount(id));
                masterList.Add(id, subclass);
            }
        }
        /// <summary>
        /// Name of the Reagent
        /// </summary>
        public string Name
        {
            get { return subclass.Name; }
        }
        /// <summary>
        /// The required number of this reagent needed
        /// </summary>
        public uint Required { get; private set; }
        public uint ID { get; private set; }
        /// <summary>
        /// Number of this Reagent in players possession
        /// </summary>
        public uint InBagItemCount { get { return subclass.InBagsCount; } }

        public static uint GetInBagItemCount(uint id)
        {
            try
            {
                return (uint)ObjectManager.Me.BagItems.Sum(i => i != null && i.IsValid && i.Entry == id ? i.StackCount : 0);
            }
            catch (Exception ex) { Logging.WriteException(ex); return 0; }
        }
    }
    #endregion

    #region Tool
    public class Tool
    {
        internal enum ToolType { SpellFocus, AreaTable, Item, Totem }

        uint index; // index to some DBC, depends on type
        ToolType toolType;
        internal Tool(uint index, ToolType toolType)
        {
            this.index = index;
            this.toolType = toolType;
            UpdateToolStatus();
            if (toolType == ToolType.Item)
                this.ID = index;
            else
                this.ID = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Tool)
            {
                Tool t = obj as Tool;
                return this.index == t.index && this.toolType == t.toolType;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (int)index + (int)toolType * 100000;
        }

        string GetName()
        {
            string _name = null;
            uint stringPtr = 0;
            switch (toolType)
            {
                case ToolType.SpellFocus:
                    WoWDb.DbTable t = StyxWoW.Db[Styx.Patchables.ClientDb.SpellFocusObject];
                    WoWDb.Row r = t.GetRow(index);
                    stringPtr = r.GetField<uint>(1);
                    break;
                case ToolType.AreaTable:
                    t = StyxWoW.Db[Styx.Patchables.ClientDb.AreaTable];
                    r = t.GetRow(index);
                    stringPtr = r.GetField<uint>(11);
                    break;
                case ToolType.Item:
                    _name = TradeSkillFrame.GetItemCacheName(index);
                    break;
                case ToolType.Totem:
                    t = StyxWoW.Db[Styx.Patchables.ClientDb.TotemCategory];
                    r = t.GetRow(index);
                    stringPtr = r.GetField<uint>(1);
                    break;
            }
            if (stringPtr != 0)
            {
                _name = ObjectManager.Wow.Read<string>(stringPtr);
            }
            return _name;
        }

        internal void UpdateToolStatus()
        {
            switch (toolType)
            {
                case ToolType.SpellFocus:
                case ToolType.AreaTable:
                    HasTool = true;
                    break;
                case ToolType.Item:
                case ToolType.Totem:
                    if (ID != 0)
                    {
                        HasTool = ObjectManager.GetObjectsOfType<WoWItem>()
                            .Where(i => i.Entry == ID).Count() > 0;
                    }
                    else
                    {
                        WoWItem item = ObjectManager.GetObjectsOfType<WoWItem>()
                            .Where(i => i.Name == Name).FirstOrDefault();
                        if (item != null)
                        {
                            ID = item.Entry;
                            HasTool = true;
                        }
                        else
                            HasTool = false;
                    }
                    break;
            }
        }

        #region Public Members
        public uint ID { get; private set; }
        /// <summary>
        /// returns true if tool is in players bags
        /// </summary>
        public bool HasTool { get; internal set; }
        /// <summary>
        /// Name of the tool
        /// </summary>
        public string Name
        {
            get { return name ?? (name = GetName()); }
        }
        string name;
        #endregion
    }
    #endregion
}
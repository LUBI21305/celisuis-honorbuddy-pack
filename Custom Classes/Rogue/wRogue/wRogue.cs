using System;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using Styx;
using Styx.Logic;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

using wRogue.Talents;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public override WoWClass Class { get { return WoWClass.Rogue; } }
        #region Global
        public override string Name { get { return "wRogue"; } }
        List<WoWUnit> addsList = new List<WoWUnit>();
        public readonly LocalPlayer Me = ObjectManager.Me;
        private string _logspam;
        private uint deathCount = 0;
        public WoWPoint prevPrevSafePoint = new WoWPoint();
        public WoWPoint prevSafePoint = new WoWPoint();
        public WoWPoint safePoint = new WoWPoint();
        public uint countBlind = 0;
        public uint countGouge = 0;
        public WoWUnit oldAdd;
        WoWPoint noSpot = new WoWPoint();
        private static ulong lastGuid;
        private static Stopwatch fightTimer = new Stopwatch();
        private static Stopwatch pullTimer = new Stopwatch();
        private static Stopwatch moveTimer = new Stopwatch();
        public uint player = ObjectManager.Me.DisplayId;
        public WoWItem mainHandPoison;
        public WoWItem offHandPoison;
        public List<uint> mainHandEPoison;
        public List<uint> offHandEPoison;
        private int _logspamMobCount;
        public SSSettings Settings = new SSSettings();
        private bool PocketPicked;
        public static Rogue My { get; private set; }

        // Deadly Poison Entry Ids
        private List<uint> PoisonDeadlyEntryIds = new List<uint>()
        {
            2892, 2893, 8984, 8985, 20844, 22053, 22054, 43232, 43233
        };
        // Deadly Poison Enchant Ids
        private List<uint> PoisonDeadlyEnchantIds = new List<uint>()
        {
            2823, 2824, 11355, 11356, 57973, 25351, 26967, 27186, 57972
        };
        // Instant Poison Entry Ids
        private List<uint> PoisonInstantEntryIds = new List<uint>()
        {
            6947, 6949, 6950, 8926, 8927, 8928, 21927, 43230, 43231
        };
        // Instant Poison Enchant Ids
        private List<uint> PoisonInstantEnchantIds = new List<uint>()
        {
            323, 324, 325, 623, 624, 625, 2641, 3768, 3769
        };
        // Crippling Poison Entry Ids
        private List<uint> PoisonCripplingEntryIds = new List<uint>()
        {
            3775
        };
        // Crippling Poison Enchant Ids
        private List<uint> PoisonCripplingEnchantIds = new List<uint>()
        {
            3408
        };
        // Wound Poison Entry Ids
        private List<uint> PoisonWoundEntryIds = new List<uint>()
        {
            10918, 10920, 10921, 10922, 22055, 43234, 43235
        };
        // Wound Poison Enchant Ids
        private List<uint> PoisonWoundEnchantIds = new List<uint>()
        {
            13219, 13225, 13226, 13227, 27188, 57977, 57978
        };
        private List<uint> BandageIds = new List<uint>()
        {
            1251, 2581, 3530, 3531, 6450, 6451, 8544, 8545, 14529, 14530, 21990, 21991, 34721, 34722
        };
        private List<uint> HealPotionIds = new List<uint>()
        {
            118, 858, 4596, 929, 1710, 3928, 13446, 17348, 28100, 33934, 22829, 17349, 32947, 33092, 18839, 31839, 31852, 31853, 31838, 39671, 33447, 41166, 43531
        };
        private WoWItem HaveItemCheck(List<uint> listId)
        {
            foreach (WoWItem item in ObjectManager.GetObjectsOfType<WoWItem>(false))
            {
                if (listId.Contains(item.Entry))
                {
                    return item;
                }
            }
            return null;
        }
        private void DisplayWoWUnit(List<WoWUnit> list)
        {
            foreach (WoWUnit s in list)
            {
                if (s.CurrentTargetGuid == Me.Guid) slog("Name: " + s.Name + ", Distance: " + Math.Round(s.Distance, 2) + ", DistanceFromPet: " + Math.Round(Me.Pet.Location.Distance(s.Location), 2));
            }
        }
        #endregion
        #region Private Members
        private void slog(string msg)
        {
            if (msg == _logspam)
            {
                return;
            }
            Logging.Write("0=={wRogue==> " + msg);
            _logspam = msg;
        }
        public override bool WantButton { get { return true; } }

        private Form _configForm;
        public override void OnButtonPress()
        {
            if (_configForm == null || _configForm.IsDisposed || _configForm.Disposing)
                _configForm = new wRogueConfig();

            _configForm.ShowDialog();
        }
        #endregion

        #region Startup
        internal Version wRogueVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        internal wRogueSpec TalentSpec { get; private set; }
        internal TalentManager Talents { get; private set; }
        private WaitTimer _specRefresh = new WaitTimer(new TimeSpan(0, 5, 0));
        public int CurSpec;

        public override void Initialize()
        {
            if (!Styx.StyxWoW.IsInGame)
                return;
            if (Me.Class != WoWClass.Rogue)
                return;

            // Temp Looting Fix
            BotEvents.Player.OnMobKilled += e =>
            {
                WoWMovement.MoveStop();
                StyxWoW.SleepForLagDuration();
            };

            // Temp Skinning Fix
            BotEvents.Player.OnMobLooted += e =>
            {
                WoWMovement.MoveStop();
                StyxWoW.SleepForLagDuration();
            };

            slog("Indexing Talent Trees");
            Talents = new TalentManager();
            slog("Calculating Talent Trees");
            TalentSpec = Talents.GetSpec();
            slog("Determined " + TalentSpec + " as your spec.");
            _specRefresh.Reset();
            slog("v" + wRogueVersion + " By wired420 Loaded");
            slog("Checking for Updates");
            Update();
        }
        #endregion
    }
}
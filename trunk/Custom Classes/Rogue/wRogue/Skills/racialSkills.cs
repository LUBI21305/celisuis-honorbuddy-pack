using Styx.Logic.Combat;
using Styx.Combat.CombatRoutine;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        #region horde racials
        // Berserking
        void Berserking()
        {
            SpellManager.Cast("Berserking");
            slog("Racial: Berserking");
        }
        // Bloodrage
        void Bloodrage()
        {
            SpellManager.Cast("Bloodrage");
            slog("Racial: Bloodrage");
        }
        // War Stomp
        void WarStomp()
        {
            if (SpellManager.CanCast("War Stomp"))
            {
                SpellManager.Cast("War Stomp");
                slog("Racial: War Stomp");
            }
        }
        // Arcane Torrent
        void ArcaneTorrent()
        {
            if (SpellManager.CanCast("Arcane Torrent"))
            {
                SpellManager.Cast("Arcane Torrent");
                slog("Racial: Arcane Torrent");
            }
        }
        // Will of the Forsaken 
        void WillOfTheForsaken()
        {
            if (SpellManager.CanCast("Will of the Forsaken"))
            {
                SpellManager.Cast("Will of the Forsaken");
                slog("Racial: Will of the Forsaken");
            }
        }
        #endregion
        #region alliance racials
        // Stoneform
        void Stoneform()
        {
            SpellManager.Cast("Stoneform");
            slog("Racial: Stoneform");
        }
        // Gift of the Naaru
        void GiftOfTheNaaru()
        {
            if (SpellManager.CanCast("Gift of the Naaru"))
            {
                SpellManager.Cast("Gift of the Naaru");
                slog("Racial: Gift of the Naaru");
            }
        }
        // Every Man for Himself
        void EveryManForHimself()
        {
            if (SpellManager.CanCast("Every Man for Himself"))
            {
                SpellManager.Cast("Every Man for Himself");
                slog("Racial: Every Man for Himself");
            }
        }
        // Shadowmeld
        void Shadowmeld()
        {
            if (SpellManager.CanCast("Shadowmeld"))
            {
                SpellManager.Cast("Shadowmeld");
                slog("Racial: Shadowmeld");
            }
        }
        // Escape Artist
        void EscapeArtist()
        {
            if (SpellManager.CanCast("Escape Artist"))
            {
                SpellManager.Cast("Escape Artist");
                slog("Racial: Escape Artist");
            }
        }
        #endregion
    }
}

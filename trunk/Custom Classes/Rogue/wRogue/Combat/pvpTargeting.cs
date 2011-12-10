using System;

using Styx;
using Styx.Logic;
using Styx.Combat.CombatRoutine;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        public void bgTargetCheck()
        {
            if (Battlegrounds.IsInsideBattleground)
            {
                if (Me.GotTarget && Me.CurrentTarget.IsPet)
                {
                    slog("Battleground: Blacklist Pet " + Me.CurrentTarget.Name);
                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromDays(1));
                    Me.ClearTarget();
                    lastGuid = 0;
                }
                if (Me.GotTarget && !Me.CurrentTarget.InLineOfSight)
                {
                    slog("Battleground: Target out of Line of Sight, blacklisting for 3 seconds");
                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromSeconds(3));
                    Me.ClearTarget();
                    lastGuid = 0;
                }
                if (Me.GotTarget && Me.CurrentTarget.Distance > 29)
                {
                    slog("Battleground: Target out of Range (" + Me.CurrentTarget.Distance + " yards), blacklisting for 3 seconds");
                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromSeconds(3));
                    Me.ClearTarget();
                    lastGuid = 0;
                }
                if (Me.GotTarget && Me.CurrentTarget.HasAura("Divine Shield"))
                {
                    slog("Battleground: Target has Divine Shield, blacklisting for 10 seconds");
                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromSeconds(10));
                    Me.ClearTarget();
                    lastGuid = 0;
                }
                if (Me.GotTarget && Me.CurrentTarget.HasAura("Ice Block"))
                {
                    slog("Battleground: Target has Ice Block, blacklisting for 10 seconds");
                    Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromSeconds(10));
                    Me.ClearTarget();
                    lastGuid = 0;
                }
            }
        }
    }
}

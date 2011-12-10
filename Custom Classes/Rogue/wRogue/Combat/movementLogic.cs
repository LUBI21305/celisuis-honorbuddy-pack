using System.Threading;
using System.Diagnostics;

using Styx.WoWInternals;
using Styx.Logic.Pathing;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        private void SafeMoveToPoint(WoWPoint point, int duration)
        {
            moveTimer.Reset();
            moveTimer.Start();
            while (Me.HealthPercent > 1)
            {
                Thread.Sleep(500);
                Navigator.MoveTo(point);
                if (!Me.HasAura("Stealth") || moveTimer.ElapsedMilliseconds >= duration)
                {
                    break;
                }
                WoWMovement.MoveStop();
            }
            return;
        }
        WoWPoint attackPointBuffer;
        WoWPoint attackPoint
        {
            get
            {
                if (Me.GotTarget)
                {
                    attackPointBuffer = WoWMovement.CalculatePointFrom(Me.CurrentTarget.Location, 3f /* + Me.CurrentTarget.BoundingRadius */ );
                    return attackPointBuffer;
                }
                else
                {
                    WoWPoint noSpot = new WoWPoint();
                    return noSpot;
                }
            }
        }
        //private static bool UseNavigator;
        //private WoWUnit _lastTarget;
        private WoWPoint _dest = WoWPoint.Zero;
        private WoWPoint GetTraceLinePos()
        {
            return new WoWPoint(Me.X, Me.Y, Me.Z + 2.132f);
        }
        private static Stopwatch movementTimer = new Stopwatch();
        private int SortByDistance(WoWUnit x, WoWUnit y)
        {
            int retval = Me.Location.Distance(x.Location).CompareTo(Me.Location.Distance(y.Location));
            return retval;
        }
    }
}

using Styx;
using Styx.Logic;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Combat.CombatRoutine;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        private double targetDistance
        {
            get
            {
                return Me.GotTarget ? Me.CurrentTarget.Distance : uint.MaxValue - 1;
            }
        }
        private WoWPoint InstanceBehindTarget
        {
            get
            {
                if (Me.GotTarget)
                {
                    return WoWMathHelper.CalculatePointBehind(Me.CurrentTarget.Location, Me.CurrentTarget.Rotation, Me.CurrentTarget.BoundingRadius + 4);
                }
                else
                {
                    return noSpot;
                }
            }
        }
        private WoWPoint behindTarget
        {
            get
            {
                if (Me.GotTarget)
                {
                    return WoWMathHelper.CalculatePointBehind(Me.CurrentTarget.Location, Me.CurrentTarget.Rotation, 4);
                }
                else
                {
                    return noSpot;
                }
            }
        }
        private WoWPoint targetLocation
        {
            get
            {
                if (Me.GotTarget)
                {
                    return Me.CurrentTarget.Location;
                }
                else
                {
                    return noSpot;
                }
            }
        }
        private ulong targettingGuid
        {
            get
            {
                if (!Equals(null, Targeting.Instance.TargetList[0]) &&
                    Targeting.Instance.TargetList.Count > 0)
                {
                    return Targeting.Instance.TargetList[0].Guid;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
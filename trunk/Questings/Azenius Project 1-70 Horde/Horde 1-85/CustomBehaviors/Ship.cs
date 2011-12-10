using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx.Database;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles.Quest.Order;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Styx.Logic.BehaviorTree;
using Action = TreeSharp.Action;

namespace Styx.Bot.Quest_Behaviors
{
    public class Ship : CustomForcedBehavior
    {
        #region Overrides of CustomForcedBehavior

        Dictionary<string, object> recognizedAttributes = new Dictionary<string, object>()
        {

            {"ObjectId",null},
            {"QuestId",null},
            {"ShipGetOnX",null},
            {"ShipGetOnY",null},
            {"ShipGetOnZ",null},
            {"ShipGetOffX",null},
            {"ShipGetOffY",null},
            {"ShipGetOffZ",null},
            {"GetOnX",null},
            {"GetOnY",null},
            {"GetOnZ",null},
            {"GetOffX",null},
            {"GetOffY",null},
            {"GetOffZ",null},
            {"DockX",null},
            {"DockY",null},
            {"DockZ",null},

        };

        bool success = true;

        public Ship(Dictionary<string, string> args)
            : base(args)
        {

            CheckForUnrecognizedAttributes(recognizedAttributes);

            WoWPoint getOnlocation = new WoWPoint(0, 0, 0);
            WoWPoint getOfflocation = new WoWPoint(0, 0, 0);
            WoWPoint reachedlocation = new WoWPoint(0, 0, 0);
            WoWPoint endlocation = new WoWPoint(0, 0, 0);
            WoWPoint docklocation = new WoWPoint(0, 0, 0);
            WoWPoint shipstand = new WoWPoint(0, 0, 0);
            int questId = 0;
            int objectId = 0;

            success = success && GetAttributeAsInteger("ObjectId", true, "1", 0, int.MaxValue, out objectId);
            success = success && GetAttributeAsInteger("QuestId", false, "0", 0, int.MaxValue, out questId);
            success = success && GetXYZAttributeAsWoWPoint("ShipGetOnX", "ShipGetOnY", "ShipGetOnZ", true, new WoWPoint(0, 0, 0), out reachedlocation);
            success = success && GetXYZAttributeAsWoWPoint("ShipGetOffX", "ShipGetOffY", "ShipGetOffZ", true, new WoWPoint(0, 0, 0), out endlocation);
            success = success && GetXYZAttributeAsWoWPoint("GetOnX", "GetOnY", "GetOnZ", true, new WoWPoint(0, 0, 0), out getOnlocation);
            success = success && GetXYZAttributeAsWoWPoint("GetOffX", "GetOffY", "GetOffZ", true, new WoWPoint(0, 0, 0), out getOfflocation);
            success = success && GetXYZAttributeAsWoWPoint("DockX", "DockY", "DockZ", false, new WoWPoint(0, 0, 0), out docklocation);
            success = success && GetXYZAttributeAsWoWPoint("ShipStandX", "ShipStandDockY", "ShipStandDockZ", false, new WoWPoint(0, 0, 0), out shipstand);

            QuestId = (uint)questId;
            ObjectID = objectId;

            GetOnLocation = getOnlocation;
            GetOffLocation = getOfflocation;
            ReachedLocation = reachedlocation;
            EndLocation = endlocation;
            DockLocation = docklocation;
            ShipStandLocation = shipstand;

            MovedOnShip = false;
            MovedToTarget = false;
            Counter = 0;
        }

        public WoWPoint GetOnLocation { get; private set; }
        public WoWPoint GetOffLocation { get; private set; }
        public WoWPoint ReachedLocation { get; private set; }
        public WoWPoint EndLocation { get; private set; }
        public WoWPoint DockLocation { get; private set; }
        public WoWPoint ShipStandLocation { get; private set; }
        public int Counter { get; set; }
        public bool MovedOnShip { get; set; }
        public bool MovedToTarget { get; set; }
        public int ObjectID { get; set; }
        public uint QuestId { get; set; }

        public static LocalPlayer me = ObjectManager.Me;

        public List<WoWGameObject> objectList;

        private Composite _root;
        protected override Composite CreateBehavior()
        {
            return _root ?? (_root =
                new PrioritySelector(

                    new Decorator(ret => Counter >= 1,
                        new Action(ret => _isDone = true)),

                        new PrioritySelector(

                            new Decorator(ret => !MovedToTarget,
                                new Action(delegate
                                {
                                    // using Styx.Logic.BehaviorTree;
                                    TreeRoot.GoalText = "Ship Behavior: Running";
                                    TreeRoot.StatusText = "Moving To Dock";

                                    if (DockLocation.Distance(me.Location) > 3)
                                    {
                                        Navigator.MoveTo(DockLocation);
                                    }
                                    else
                                    {
                                        MovedToTarget = true;
                                        return RunStatus.Success;
                                    }

                                    return RunStatus.Running;

                                })
                                ),

                            new Decorator(ret => MovedToTarget,
                                new Action(delegate
                                {
                                    objectList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                                        .Where(u => u.Entry == ObjectID)
                                        .OrderBy(u => u.Distance).ToList();


                                    TreeRoot.StatusText = "Waiting For Ship";

                                    foreach (WoWGameObject value in objectList)
                                    {
                                        Tripper.Tools.Math.Matrix m = value.GetWorldMatrix();

                                        WoWPoint matrixLocation = new WoWPoint(m.M41, m.M42, m.M43);

                                        //Logging.Write("" + matrixLocation.X + " " + matrixLocation.Y + " " + matrixLocation.Z);

                                        if (GetOffLocation.Distance(me.Location) < 3)
                                        {
                                                _isDone = true;
                                                return RunStatus.Success;
                                        }

                                        if (matrixLocation.Distance(me.Location) < 20 && !MovedOnShip)
                                        {
                                            TreeRoot.StatusText = "Moving Onto Ship";
                                            Thread.Sleep(3000);
                                            WoWMovement.ClickToMove(GetOnLocation);
                                            MovedOnShip = true;
                                        }
                                        else if (MovedOnShip)
                                        {
                                            TreeRoot.StatusText = "On Ship, Waiting";
                                            if (ShipStandLocation.X > 0)
                                            {
                                                WoWMovement.ClickToMove(ShipStandLocation);
                                            }
                                            if (matrixLocation.Distance(EndLocation) < 20)
                                            {
                                                TreeRoot.StatusText = "Moving Off Ship";
                                                Thread.Sleep(3000);
                                                WoWMovement.ClickToMove(GetOffLocation);
                                                MovedOnShip = true;
                                            }
                                        }

                                        return RunStatus.Success;
                                    }

                                    return RunStatus.Running;
                                })
                                ),

                            new Action(ret => Logging.Write(""))
                        )
                    ));
        }

        private bool _isDone;
        public override bool IsDone
        {
            get { return _isDone; }
        }

        #endregion
    }
}


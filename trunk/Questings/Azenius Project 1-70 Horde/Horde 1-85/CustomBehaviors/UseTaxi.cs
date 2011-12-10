using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;
using Styx.Logic.BehaviorTree;

namespace Styx.Bot.Quest_Behaviors
{
    public class UseTaxi : CustomForcedBehavior
    {
        #region Overrides of CustomForcedBehavior.

        public UseTaxi(Dictionary<string, string> args)
            : base(args)
        {
			/// 		UseTaxi by Vlad
			/// Accepted arguments:
			///
			/// MobId: (Required) - Id of the Flight Master to use
			/// QuestId: (Optional) - associates a quest with this behavior. 
			///						If the quest is complete or not in the quest log, this will not be executed.
			/// DestName: (Required if ViewNodesOnly is not specified) - specifies the destination NAME of the node on the TaxiMap. 
			///						This should be a name string in the list of your TaxiMap node names. The argument is CASE SENSITIVE!
			/// ViewNodesOnly: (Optional) true/false - Use this option only to print the list of destinations of your TaxiMap, 
			///						if you don't know exact names of locations. The list will be printed ingame. Default is false
			/// X, Y, Z: (Required) - Location of Flight Master Npc
			///

            MobId = GetAttributeAsNullable<int>("MobId", true, ConstrainAs.MobId, new[] { "NpcId", "NpcID" }) ?? 0;
            QuestId = (uint?)GetAttributeAsNullable<int>("QuestId", false, ConstrainAs.QuestId(this), null) ?? 0;
            DestName = GetAttributeAs<string>("DestName", false, null, new[] { "NodeName" }) ?? "";
            ViewNodesOnly = GetAttributeAsNullable<bool>("ViewNodesOnly", false, null, null) ?? false;
            Location = GetAttributeAsNullable<WoWPoint>("", true, ConstrainAs.WoWPointNonEmpty, null) ?? WoWPoint.Empty;
            Counter = 0;
        }

        public WoWPoint MovePoint { get; private set; }
		public WoWPoint Location { get; private set; }
        public int Counter { get; set; }
        public int MobId { get; set; }
        public uint QuestId { get; set; }
		public string DestName { get; set; }
		public bool ViewNodesOnly { get; set; }
		
        public static LocalPlayer Me = ObjectManager.Me;

        public List<WoWUnit> npcList;

        private Composite _root;
        protected override Composite CreateBehavior()
        {
            return _root ?? (_root =
                new PrioritySelector(

                new Decorator(ret => ((QuestId != 0 && Me.QuestLog.GetQuestById(QuestId) == null)
                         || (QuestId != 0 && Me.QuestLog.GetQuestById(QuestId) != null && Me.QuestLog.GetQuestById(QuestId).IsCompleted)),
                        new Action(ret => _isDone = true)),

                    new Decorator(ret => Counter >= 1,
                        new Action(ret => _isDone = true)),

                        new PrioritySelector(

                            new Decorator(ret => Counter == 0,
                                new Action(delegate
                                {
									npcList = ObjectManager.GetObjectsOfType<WoWUnit>()
                                            .Where(u => u.Entry == MobId && !u.Dead)
                                            .OrderBy(u => u.Distance).ToList();
                                    TreeRoot.GoalText = string.Format("UseTaxi Running");
                                    if (npcList.Count == 0)
									{
										TreeRoot.StatusText = "Moving to Flight Master location";
										if (!Me.IsIndoors && !Me.Mounted && Me.Level > 19)
										{
										Styx.Logic.Mount.MountUp();
										}
										Navigator.MoveTo(Location);
									}
									else
                                    {
									
                                        if (npcList[0].Location.Distance(Me.Location) >= 3)
                                        {
											TreeRoot.StatusText = string.Format("Moving To Flight Master");
                                            Navigator.MoveTo(npcList[0].Location);
                                            Thread.Sleep(300);
                                            return RunStatus.Running;
                                        }
										else 
										{
											if (!ViewNodesOnly)
											{
												TreeRoot.StatusText = string.Format("Taking a ride to desired location");
												WoWMovement.MoveStop();
												Styx.Logic.Inventory.Frames.Taxi.TaxiFrame TaxiMap = new Styx.Logic.Inventory.Frames.Taxi.TaxiFrame();
												while (!TaxiMap.IsVisible)
												{
													npcList[0].Interact();
													Thread.Sleep(3000);
												}
												while (!Me.OnTaxi)
												{
													WoWInternals.Lua.DoString("for i=1,NumTaxiNodes() do a=TaxiNodeName(i); if strmatch(a,'"+DestName+"')then b=i; TakeTaxiNode(b); end end");
													Thread.Sleep(2000);
												}
												Counter++;
												return RunStatus.Success;
											}
											else
											{	
												TreeRoot.StatusText = string.Format("Listing known TaxiNodes");
												WoWMovement.MoveStop();
												npcList[0].Interact();
												Thread.Sleep(1000);
												WoWInternals.Lua.DoString("for i=1,NumTaxiNodes() do a=TaxiNodeName(i); print(i,a);end;");
												Counter++;
												return RunStatus.Success;
											}
										}
                                        
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx.Database;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles.Quest;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

namespace Styx.Bot.Quest_Behaviors
{
    public class TalkToActivate : CustomForcedBehavior
    {
        #region Overrides of CustomForcedBehavior

        Dictionary<string, object> recognizedAttributes = new Dictionary<string, object>()
        {

            {"TalkNow",null},
            {"QuestId",null},

        };

        bool success = true;

        public TalkToActivate(Dictionary<string, string> args)
            : base(args)
        {

            CheckForUnrecognizedAttributes(recognizedAttributes);

            int talkNow = 0;
            int questId = 0;

            success = success && GetAttributeAsInteger("TalkNow", false, "10000", 0, int.MaxValue, out talkNow);
            success = success && GetAttributeAsInteger("QuestId", false, "0", 0, int.MaxValue, out questId);

            Counter = 0;
            QuestId = (uint)questId;
            TalkNow = talkNow;

        }

        public WoWPoint MovePoint { get; private set; }
        public int Counter { get; set; }
        public int TalkNow { get; set; }
        public uint QuestId { get; set; }

        public static LocalPlayer me = ObjectManager.Me;

        private Composite _root;
        protected override Composite CreateBehavior()
        {
            return _root ?? (_root =
                new PrioritySelector(

                    new Decorator(ret => Counter >= 1,
                        new Action(ret => _isDone = true)),

                        new PrioritySelector(

                            new Decorator(ret => Counter == 0,
                                new Action(delegate
                                {
                                    Lua.DoString("GossipTitleButton" + TalkNow + ":Click()");
                                    Thread.Sleep(500);

                                    Counter++;
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


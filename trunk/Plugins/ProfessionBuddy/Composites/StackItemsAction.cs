using Styx;
using TreeSharp;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.Logic;
using Styx.Logic.Combat;
using System.Diagnostics;
using Styx.Patchables;
using System.Linq;
using Styx.Plugins;
using Styx.Plugins.PluginClass;
using Styx.Logic.Pathing;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals.WoWObjects;
using CommonBehaviors.Actions;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using Styx.Combat.CombatRoutine;
using Styx.Logic.POI;
using HighVoltz.Composites;


namespace HighVoltz.Composites
{
    #region StackItemsAction
    class StackItemsAction : PBAction
    {
        public StackItemsAction()
        {
        }
        string lua =
            "local items={} " +
            "local done = 1 " +
            "for bag = 0,4 do " +
               "for slot=1,GetContainerNumSlots(bag) do " +
                  "local id = GetContainerItemID(bag,slot) " +
                  "local _,c,l = GetContainerItemInfo(bag, slot) " +
                  "if id ~= nil then " +
                     "local n,_,_,_,_,_,_, maxStack = GetItemInfo(id) " +
                     "if c < maxStack then " +
                        "if items[id] == nil then " +
                           "items[id] = {left=maxStack-c,bag=bag,slot=slot,locked = l or 0} " +
                        "else " +
                           "if items[id].locked == 0 then " +
                              "PickupContainerItem(bag, slot) " +
                              "PickupContainerItem(items[id].bag, items[id].slot) " +
                              "items[id] = nil " +
                           "else " +
                              "items[id] = {left=maxStack-c,bag=bag,slot=slot,locked = l or 0} " +
                           "end " +
                           "done = 0 " +
                        "end " +
                    "end " +
                  "end " +
               "end " +
            "end " +
            "return done ";
        Stopwatch throttleSW = new Stopwatch();
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                if (!throttleSW.IsRunning || throttleSW.ElapsedMilliseconds > 500)
                {
                    throttleSW.Reset();
                    throttleSW.Start();
                    IsDone = Lua.GetReturnVal<int>(lua, 0) == 1;
                }
                if (!IsDone)
                    return RunStatus.Running;
            }
            return RunStatus.Failure;
        }
        public override string Name { get { return "Stack Items"; } }
        public override string Title
        {
            get
            {
                return string.Format("{0}", Name);
            }
        }
        public override string Help
        {
            get
            {
                return "This action will stack up items in player's bags. Use this before Disenchanting/Milling/Prospecting";
            }
        }
        public override object Clone()
        {
            return new StackItemsAction();
        }
    }
    #endregion
}

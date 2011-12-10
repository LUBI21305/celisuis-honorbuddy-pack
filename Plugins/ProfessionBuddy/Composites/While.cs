using System.Collections.Generic;
using System.Text;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Styx.Helpers;
using TreeSharp;
using System.Diagnostics;
using PrioritySelector = TreeSharp.PrioritySelector;
namespace HighVoltz.Composites
{
    class While : If
    {
        protected override IEnumerable<RunStatus> Execute(object context)
        {
            //lock (_lockObject)
            //{
                if (IsDone)
                {
                    yield return RunStatus.Failure;
                    yield break;
                }
                foreach (Composite node in Children)
                {
                    node.Start(context);
                    // Keep stepping through the enumeration while it's returning RunStatus.Running
                    // or until CanRun() returns false if IgnoreCanRun is false..
                    while ((IgnoreCanRun || (CanRun(context) && !IgnoreCanRun)) &&
                        node.Tick(context) == RunStatus.Running)
                    {
                        Selection = node;
                        yield return RunStatus.Running;
                    }
                    Selection = null;
                    //node.Stop(context); tick will call stop, so why do it twice?
                    if (node.LastStatus == RunStatus.Success)
                    {
                        yield return RunStatus.Success;
                        yield break;
                    }
                }
                Reset();
                if (!CanRun(context))
                {
                    yield return RunStatus.Failure;
                    yield break;
                }
                else
                {
                    yield return RunStatus.Success;
                    //yield break;
                }
           // }
        }

        override public string Name { get { return "While Condition"; } }
        override public string Title
        {
            get
            {
                return string.Format("While {0}",
                    string.IsNullOrEmpty(Condition) ? "Condition" : "(" + Condition + ")");
            }
        }
        override public string Help { get { return "'While Condition' will execute the actions it contains if the specified condition is true. 'Ignore Condition until done' basically will ignore the Condition if any of the actions it contains is running. The difference between this and the 'If Condition' is that this will auto reset all actions within it and all nested 'If/While' Conditions"; } }
        public override object Clone()
        {
            While w = new While()
            {
                CanRunDelegate = this.CanRunDelegate,
                Condition = this.Condition,
                IgnoreCanRun = this.IgnoreCanRun
            };
            return w;
        }
    }
    //class While : If
    //{
    //    private static bool doingLoop = false;
    //    public override RunStatus Tick(object context)
    //    {
    //        if ((LastStatus == RunStatus.Running && IgnoreCanRun) || CanRun(null))
    //        {
    //            if (!DecoratedChild.IsRunning)
    //                DecoratedChild.Start(null);
    //            doingLoop = false;
    //            LastStatus = DecoratedChild.Tick(null);
    //            if (doingLoop)
    //            {
    //                LastStatus = RunStatus.Running;
    //                return RunStatus.Success;
    //            }
    //            if (LoopComplete && CanRun(null))
    //            {
    //                LastStatus = RunStatus.Running;
    //                Reset();
    //                doingLoop = true;
    //                return RunStatus.Success;
    //            }
    //            if (IsDone)
    //            {
    //                Reset();
    //                //((PrioritySelector)DecoratedChild).Stop(context);
    //                LastStatus = RunStatus.Failure;
    //            }
    //            else
    //            {
    //                LastStatus = RunStatus.Running;
    //            }
    //        }
    //        else
    //            LastStatus = RunStatus.Failure;
    //        return (RunStatus)LastStatus;
    //    }

    //    public bool LoopComplete
    //    {
    //        get
    //        {
    //            PrioritySelector ps = (PrioritySelector)DecoratedChild;
    //            return ps.Children.Count(c => ((IPBComposite)c).IsDone) == ps.Children.Count;
    //        }
    //    }

    //    override public bool IsDone
    //    {
    //        get
    //        {
    //            return (!CanRun(null) && (!IgnoreCanRun || (IgnoreCanRun && LastStatus != RunStatus.Running)));
    //            //PrioritySelector ps = (PrioritySelector)DecoratedChild;
    //            //bool childrenDone = ps.Children.Count(c => ((IPBComposite)c).IsDone) == ps.Children.Count;
    //            //return (childrenDone && IgnoreCanRun) ||
    //            //    (!IgnoreCanRun && !CanRun(null));
    //        }
    //    }


    //    //public override RunStatus Tick(object context)
    //    //{
    //    //    bool canRun = CanRun(null);
    //    //    if ((LastStatus == RunStatus.Running && IgnoreCanRun) || canRun)
    //    //    {
    //    //        if (!DecoratedChild.IsRunning)
    //    //            DecoratedChild.Start(null);
    //    //        LastStatus = DecoratedChild.Tick(null);
    //    //        if (IsDone)
    //    //        {
    //    //            Reset();
    //    //        }
    //    //        else
    //    //            return RunStatus.Running;
    //    //    }
    //    //    return RunStatus.Failure;
    //    //}

    //    override public string Name { get { return "While Condition"; } }
    //    override public string Title
    //    {
    //        get
    //        {
    //            return string.Format("While {0}",
    //                string.IsNullOrEmpty(Condition) ? "Condition" : "(" + Condition + ")");
    //        }
    //    }
    //    override public string Help { get { return "'While Condition' will execute the actions it contains if the specified condition is true. 'Ignore Condition until done' basically will ignore the Condition if any of the actions it contains is running. The difference between this and the 'If Condition' is that this will auto reset all actions within it and all nested 'If/While' Conditions"; } }
    //    public override object Clone()
    //    {
    //        While w = new While()
    //        {
    //            CanRunDelegate = this.CanRunDelegate,
    //            Condition = this.Condition,
    //            IgnoreCanRun = this.IgnoreCanRun
    //        };
    //        return w;
    //    }
    //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TreeSharp;
using System.Threading;
using Styx.Logic.BehaviorTree;
using Styx;
using System.Xml;
using Styx.Helpers;
using System.Windows;
using System.Diagnostics;

namespace HighVoltz.Composites
{
    public class ChangeBotAction : PBAction
    {
        public string BotName
        {
            get { return (string)Properties["BotName"].Value; }
            set { Properties["BotName"].Value = value; }
        }

        public ChangeBotAction()
        {
            Properties["BotName"] = new MetaProp("BotName", typeof(string), new DisplayNameAttribute("Bot Name"));
            BotName = "";
        }

        protected override RunStatus Run(object context)
        {
            if (!IsDone && !_botIsChanging)
            {
                try
                {
                    ChangeBot();
                }
                finally
                {
                    IsDone = true;
                }
            }
            return RunStatus.Failure;
        }

        public void ChangeBot()
        {
            ChangeBot(BotName);
        }
        
        static Timer _timer;
        //static Stopwatch _throttleSW = new Stopwatch();
        static bool _botIsChanging = false;
        static public void ChangeBot(string name)
        {
            if (_botIsChanging)
            {
                Professionbuddy.Log("Must wait for previous ChangeBot to finish before calling ChangeBot again.");
                return;
            }
            BotBase bot = BotManager.Instance.Bots.FirstOrDefault(b => b.Key.Contains(name)).Value;
            if (BotManager.Current == bot)
                return;
            if (bot != null)
            {
                // execute from GUI thread since this thread will get aborted when switching bot
                _botIsChanging = true;
                Application.Current.Dispatcher.BeginInvoke(
                    new System.Action(() => {
                        bool isRunning = TreeRoot.IsRunning;
                        BotManager.Instance.SetCurrent(bot);
                        if (isRunning)
                        {
                            Professionbuddy.Log("Restarting HB in 3 seconds");
                            _timer = new Timer(new TimerCallback((o) => {
                                TreeRoot.Start();
                                Professionbuddy.Log("Restarting HB");
                                _botIsChanging = false;
                            }),null,3000,Timeout.Infinite);
                            
                        }
                    }
                ));
                Professionbuddy.Log("Changing bot to {0}", name);
            }
            else
            {
                Professionbuddy.Err("Bot {0} does not exist", name);
            }
        }

        public override string Name { get { return "Change Bot"; } }
        public override string Title
        {
            get
            {
                return string.Format("{0} to :{1}", Name, BotName);
            }
        }
        public override string Help
        {
            get
            {
                return "This action will change to the bot specified with 'Bot Name' Property. 'Bot Name' can be a partial match";
            }
        }
        public override object Clone()
        {
            return new ChangeBotAction() { BotName = this.BotName };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            BotName = reader["BotName"];
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("BotName", BotName.ToString());
        }
        #endregion
    }
}

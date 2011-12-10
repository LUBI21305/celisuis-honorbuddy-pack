using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.POI;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals;
using Styx;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System;

namespace mastahg
{
    public class FlagReturner : HBPlugin, IDisposable
    {


        public override string Name { get { return "FlagReturner"; } }
        public override string Author { get { return "mastahg"; } }
        public override Version Version { get { return new Version(1, 0, 0); } }
        public override bool WantButton { get { return true; } }
        public override string ButtonText { get { return Version.ToString(); } }
        private LocalPlayer Me { get { return ObjectManager.Me; } }
   

        public override void OnButtonPress()
        {
            FlagFound(true);
        }
        WoWGameObject Flag;
       bool FlagFound(bool showlog)
        {
			Thread.Sleep(250);
            ObjectManager.Update();
			
            List<WoWGameObject> Objs = ObjectManager.GetObjectsOfType<WoWGameObject>(false, false);
			foreach(WoWGameObject obj in Objs)
            {
                String Name = obj.Name;
                if (showlog)
                {
                    Log(Name);
                }
                if (Name.Equals(FactionFlag))
                {
                    Flag = obj;
                    return true;
                }
				
            }
           return false;
        }
        String MyFaction;
        String FactionFlag;
        static String Reg = "The (Horde|Alliance) Flag was (picked up|dropped) by (.+)!";
        Regex TheReg = new Regex(Reg, RegexOptions.IgnoreCase);

        void WoWChat_Battleground(WoWChat.ChatSimpleMessageEventArgs e)
        {
            //Log("Message Inc");
            //Split the incoming message for the bits we care about.
            String Message = e.Message;
            Log(Message);
            String[] Parts = TheReg.Split(Message);
            String Faction = Parts[1];
            String Action = Parts[2];
            String Player = Parts[3];
            Log(Faction + "-" + Action + "-" + Player);
            //Check if the faction matches, we only care about our flag.
            if (Faction == MyFaction && Action == "dropped" && Me.IsAlive)
            {
                bool FlagCheck = FlagFound(false);
                if (FlagCheck)
                {
                    Log("Flag Found");
                    Flag.Interact();
                }
                else
                {
                    Log("Flag not Found");
                }
            }
        }

        public override void Initialize()
        {
            if (StyxWoW.Me.IsHorde)
            {
                MyFaction = "Horde";
            }
            else
            {
                MyFaction = "Alliance";
            }
            FactionFlag = MyFaction + " Flag";
            Log("Searching for " + FactionFlag);

            WoWChat.AllianceBattleground += WoWChat_Battleground;
            WoWChat.HordeBattleground += WoWChat_Battleground;
            WoWChat.NeutralBattleground  += WoWChat_Battleground;



        }
		public override void Pulse()
		{
		}

        public override void Dispose()
        {
            Log("Dispose");
            WoWChat.AllianceBattleground -= WoWChat_Battleground;
            WoWChat.HordeBattleground -= WoWChat_Battleground;
            WoWChat.NeutralBattleground -= WoWChat_Battleground;

        }

        private void Log(string format, params object[] args)
        {
            Logging.Write(Color.CadetBlue, "[FlagReturner] " + format, args);
        }
        

    }

	
	
}



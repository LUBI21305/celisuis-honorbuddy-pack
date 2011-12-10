using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.IO;

using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;


namespace GuildInviter2
{
    public class guildInviter2 : HBPlugin
    {
        #region guildInviter Code
        public int minLevel = GISettings.Instance.MinLevel;

        public override string Name { get { return "Guild Inviter v2.0"; } }
        public override string Author { get { return "v2: Wownerds | v1: ETMA"; } }
        public override Version Version { get { return _version; } }
        private readonly Version _version = new Version(2, 0, 0, 0);
        public override string ButtonText { get { return "Config"; } }
        public override bool WantButton
        {
            get
            {
                return true;
            }
        }

        public override void OnButtonPress()
        {
            GuildInviter2.cfg cfg = new GuildInviter2.cfg();
            cfg.ShowDialog();
        }
        private Stopwatch _peopleListSW = new Stopwatch();
        private Stopwatch _peopleInviteSW = new Stopwatch();
        private bool _firstPulse = true;
        List<string> whoList;
        Dictionary<string, Persons> allPersons = new Dictionary<string, Persons>();
        Dictionary<string, Persons> invitedPersons = new Dictionary<string, Persons>();
        public int _curLevelWho = GISettings.Instance.MinLevel;
        public string _curClassWho = "";
        public int _curClassWhoInt = 0;
        public string _curRaceWho = "";
        public int _curRaceWhoInt = 0;
        public string _curZoneWho = "";
        public string _curNameWho = "";
        public int _curImprovement = 0;
        public int totalWhos = 0;
        string myFaction = ObjectManager.Me.Faction.Name.Split(' ')[1].ToLower();
        public List<string> alliances = new List<string>(new string[] { "human", "gnome", "dwarf", "worgen", "night elf", "draenei" });
        public List<string> hordes = new List<string>(new string[] { "undead", "tauren", "orc", "troll", "goblin", "blood elf" });
        public List<string> classes = new List<string>(new string[] { "Warrior", "Warlock", "Hunter", "Druid", "Rogue", "Priest", "Shaman", "Paladin", "Death Knight", "Mage" });
        public override void Pulse()
        {
            if (_curLevelWho > GISettings.Instance.MaxLevel)
            {
                _curLevelWho = GISettings.Instance.MinLevel;
            }
            if (_firstPulse)
            {
                buildInvitedPersons();
                Lua.Events.AttachEvent("WHO_LIST_UPDATE", recievedWhoList);
                _firstPulse = false;
                _peopleInviteSW.Start();
                _curLevelWho = GISettings.Instance.MinLevel;
            }
            if (!_peopleListSW.IsRunning || (_peopleListSW.IsRunning && _peopleListSW.ElapsedMilliseconds == 0))
            {
                _peopleListSW.Start();
                doWhoSearch();
            }
            if (_peopleListSW.ElapsedMilliseconds > 15 * 1000)
            {
                _peopleListSW.Reset();
                _peopleListSW.Stop();
            }
            if (_peopleInviteSW.ElapsedMilliseconds >= 5000)
            {
                _peopleInviteSW.Reset();
                _peopleInviteSW.Start();
                ginviteNextPerson();
            }
        }

        public void buildInvitedPersons()
        {
            if (File.Exists("./invitedPersons.txt"))
            {
                StreamReader sr = new StreamReader("./invitedPersons.txt");
                while (sr.Peek() > 0)
                {
                    string curLine = sr.ReadLine().Trim();
                    if (!invitedPersons.ContainsKey(curLine))
                    {
                        Persons curPerson = new Persons();
                        curPerson.name = curLine;
                        invitedPersons.Add(curLine, curPerson);
                    }
                }
                sr.Close();
            }
        }

        public void ginviteNextPerson()
        {
            foreach (KeyValuePair<string, Persons> curPerson in allPersons)
            {
                if (!invitedPersons.ContainsKey(curPerson.Key))
                {
                    Logging.Write("Whispering :" + curPerson.Key);
                    Lua.DoString("SendChatMessage(\"" + GISettings.Instance.whispertext + "\" ,\"WHISPER\" ,nil ,\"" + curPerson.Key + "\")");
                    Logging.Write("GuildInviting :" + curPerson.Key);
                    Lua.DoString("GuildInvite(\"" + curPerson.Key + "\")");
                    writeInvitedPerson(curPerson.Value);
                    break;
                }
            }
        }

        public void writeInvitedPerson(Persons curPerson)
        {
            invitedPersons.Add(curPerson.name, curPerson);
            StreamWriter sw = new StreamWriter("./invitedPersons.txt", true, Encoding.UTF8);
            sw.WriteLine(curPerson.name);
            sw.Close();

        }
        public void buildAllPersons()
        {
            if (File.Exists("./allPersons.txt"))
            {
                StreamReader sr = new StreamReader("./allPersons.txt");
                while (sr.Peek() > 0)
                {
                    string curLine = sr.ReadLine().Trim();
                    try
                    {

                        Persons curPerson = new Persons();
                        curPerson.name = curLine;
                        if (!allPersons.ContainsKey(curLine))
                        {
                            allPersons.Add(curLine, curPerson);
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.Write("ERROR: " + e);
                    }
                }
                sr.Close();
            }
        }
        public void writePerson(Persons curPerson)
        {
            StreamWriter sw = new StreamWriter("./allPersons.txt", true, Encoding.UTF8);
            sw.WriteLine(curPerson.name);
            sw.Close();
        }
        public void recievedWhoList(object sender, LuaEventArgs args)
        {
            List<string> whoNumAmounts = Lua.GetReturnValues("return GetNumWhoResults()");
            int shownWhos = Int32.Parse(whoNumAmounts[0]);
            totalWhos = Int32.Parse(whoNumAmounts[1]);
            for (int i = 1; i <= totalWhos; i++)
            {
                List<string> whoInfo = Lua.GetReturnValues("return GetWhoInfo(" + i + ")");
                if (whoInfo[1].Length <= 0)
                {
                    Persons curPerson = new Persons();
                    curPerson.name = whoInfo[0];
                    curPerson.level = Int32.Parse(whoInfo[2]);
                    if (!allPersons.ContainsKey(whoInfo[0]))
                    {
                        allPersons.Add(whoInfo[0], curPerson);
                        writePerson(curPerson);
                    }
                }
            }
            Lua.DoString("ToggleFriendsFrame(2)");
            improveSearch();
        }

        public void improveSearch()
        {
            if (totalWhos >= 49)
            {
                _curImprovement++;
            }
            improveNextSearch();
        }

        public void improveNextSearch()
        {
            if (_curImprovement == 0)
            {
                _curLevelWho++;
            }
            else if (_curImprovement == 1)
            {
                changeWhoClass();
            }
            else if (_curImprovement == 2)
            {
                changeWhoRace();
            }
            else
            {
                Logging.Write(GISettings.Instance.MaxLevel + "with specific class and race. Continueing due to slack from me.");
                _curImprovement = 1;
                changeWhoClass();
            }
        }

        public void changeWhoRace()
        {
            switch (_curRaceWhoInt)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    if (alliances.Contains(myFaction))
                        _curRaceWho = alliances[_curRaceWhoInt];
                    if (hordes.Contains(myFaction))
                        _curRaceWho = hordes[_curRaceWhoInt];
                    _curRaceWhoInt++;
                    break;
                case 6:
                    _curRaceWhoInt = 0;
                    _curRaceWho = "";
                    _curImprovement--;
                    break;
            }
        }

        public void changeWhoClass()
        {
            switch (_curClassWhoInt)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    _curClassWho = classes[_curClassWhoInt];
                    _curClassWhoInt++;
                    break;
                case 10:
                    _curClassWhoInt = 0;
                    _curClassWho = "";
                    _curImprovement--;
                    if (_curLevelWho >= GISettings.Instance.MaxLevel)
                    {
                        _curLevelWho = GISettings.Instance.MinLevel;
                        _curImprovement = 0;
                    }
                    improveSearch();
                    break;
            }
        }

        public void doWhoSearch()
        {
            string whoLine = "";
            if (_curLevelWho > 0)
                whoLine += "" + _curLevelWho + "-" + _curLevelWho + " ";
            if (_curClassWho.Length > 0)
                whoLine += "c-\\\"" + _curClassWho + "\\\" ";
            if (_curRaceWho.Length > 0)
                whoLine += "r-\\\"" + _curRaceWho + "\\\" ";
            if (_curZoneWho.Length > 0)
                whoLine += "z-\\\"" + _curZoneWho + "\\\" ";

            string whoLineDone = "SendWho(\"" + whoLine + "\")";
            Logging.Write(whoLineDone);
            Lua.DoString("SetWhoToUI(1);");
            Lua.DoString(whoLineDone);
        }
        #endregion
    }
    public class Persons
    {
        public string name;
        public string guild;
        public int level;
        public string race;
        public string wowClass;
        public string zone;
    }
}

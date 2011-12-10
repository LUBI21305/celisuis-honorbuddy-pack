// Credits to Apoc and Main 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Styx;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Patchables;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;


namespace HighVoltz
{
    struct worldMapCoord : IEquatable<object>
    {
        public float X;
        public float Y;
        public uint MapID;

        public override bool Equals(object obj)
        {
            if (!(obj is worldMapCoord))
                return false;
            worldMapCoord wmc = (worldMapCoord)obj;
            return wmc.X == this.X && wmc.Y == this.Y && wmc.MapID == this.MapID;
        }

        public override int GetHashCode()
        {
            return (int)((X * 100F) + (Y * 100f) + (MapID * 100f));
        }

        public static bool operator ==(worldMapCoord a, worldMapCoord b)
        {
            return a.X == b.X && a.Y == b.Y && a.MapID == b.MapID;
        }
        public static bool operator !=(worldMapCoord a, worldMapCoord b)
        {
            return a.X != b.X || a.Y != b.Y || a.MapID != b.MapID;
        }
    }

    class ClickToGo : HBPlugin
    {
        static public string name = "ClickToGo";
        public override string Name { get { return name + " " + Version.ToString(); } }
        public override string Author { get { return "HighVoltz"; } }
        private readonly Version _version = new Version(1, 8);
        public override Version Version { get { return _version; } }
        public override string ButtonText { get { return "ClickToGo"; } }
        public override bool WantButton { get { return false; } }
        private static worldMapCoord oldMapCoord = new worldMapCoord();
        private static worldMapCoord mapCoord = new worldMapCoord();
        static LocalPlayer Me = ObjectManager.Me;
        static ClickToGoSettings MySettings;
        static Random Rand = new Random();
        static NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        static CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        WoWPoint GotoLocation;
        string profilePath = Logging.ApplicationPath + @"\Default Profiles\ClickToGo.xml";

        public ClickToGo()
        {
        }


        public override void Initialize()
        {
            MySettings = new ClickToGoSettings(Application.StartupPath + "\\Plugins\\ClickToGo.xml");
            if (MySettings.worldMapCoordName == "")
            {
                MySettings.worldMapCoordName = CreateRandomString(System.Environment.TickCount + 100);
                MySettings.getWorldMapCoordName = CreateRandomString(System.Environment.TickCount + 200);
                MySettings.worldMapButton_OnClickHook = CreateRandomString(System.Environment.TickCount + 300);
                MySettings.WorldMapButton_OnUpdateHook = CreateRandomString(System.Environment.TickCount + 400);
                MySettings.worldMapButton_OldOnClickHook = CreateRandomString(System.Environment.TickCount + 500);
                MySettings.Save();
            }
            InitWorldMapClickOverride();
            CharacterSettings.Instance.UseFlightPaths = true;
        }

        public override void Pulse()
        {
            try
            {
                mapCoord = getWorldMapCoord();
                if (mapCoord != oldMapCoord)
                {
                    oldMapCoord = mapCoord;
                    GotoLocation = mapToWorldCoords(mapCoord);
                    if (GotoLocation == WoWPoint.Empty)
                    {
                        Logging.Write("Unable to Genorate path to location. Try a different spot nearby");
                        return;
                    }
                    Logging.Write("{0}",ToHotSpot(GotoLocation));
                    //WriteProfile(GotoLocation);
                    //ProfileManager.LoadNew(profilePath);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                Log("Exception in Pulse:{0}", e);
            }
        }

        static WoWPoint mapToWorldCoords(worldMapCoord wmc)
        {
            WoWPoint worldPoint = new WoWPoint();
            WoWDb.DbTable worldMapArea = StyxWoW.Db[ClientDb.WorldMapArea];
            WoWDb.Row worldMapAreaFields = worldMapArea.GetRow(wmc.MapID);
            float ay = worldMapAreaFields.GetField<float>(4);
            float by = worldMapAreaFields.GetField<float>(5);
            float ax = worldMapAreaFields.GetField<float>(6);
            float bx = worldMapAreaFields.GetField<float>(7);
            //Logging.Write("You are at {0}, {1} relative", (myLoc.Y - a.Y) / (b.Y - a.Y), (myLoc.X - a.X) / (b.X - a.X));
            worldPoint.X = ax + (wmc.Y * (bx - ax));
            worldPoint.Y = ay + (wmc.X * (by - ay));
            try
            {
                worldPoint.Z = Navigator.FindHeights(worldPoint.X, worldPoint.Y).Max();
            }
            catch { return WoWPoint.Empty; }
            return worldPoint;
        }

        private static worldMapCoord getWorldMapCoord()
        {
            worldMapCoord wmc = new worldMapCoord();
            List<string> ret = Lua.GetReturnValues(string.Format("return {0}()", MySettings.getWorldMapCoordName), "clicky.lua");
            if (ret != null && ret.Count == 3)
            {
                float.TryParse(ret[0], style, culture, out wmc.X);
                float.TryParse(ret[1], style, culture, out wmc.Y);
                uint.TryParse(ret[2], style, culture, out wmc.MapID);
            }
            return wmc;
        }

        static public bool hasCarbonite { get { return Lua.GetReturnValues("if Nx then return 1 else return 0 end")[0] == "1"; } }

        static public bool CombatChecks
        {
            get
            {
                return Me.Combat || !Me.IsAlive;
            }
        }

        static public void Log(string msg, params object[] args) { Logging.Write(msg, args); }

        static public void Log(System.Drawing.Color c, string msg, params object[] args) { Logging.Write(c, msg, args); }
        
        static string ToHotSpot(WoWPoint point)
        {
            return string.Format(culture, "<Hotspot X=\"{0}\" Y=\"{1}\" Z=\"{2}\" />", point.X, point.Y, point.Z);
        }

        public void InitWorldMapClickOverride()
        {
            string getWorldMapCoordLua =
                "function " + MySettings.getWorldMapCoordName + "() " +
                    "if " + MySettings.worldMapCoordName + " then " +
                        "return unpack(" + MySettings.worldMapCoordName + ") " +
                    "else " +
                        "return nil " +
                    "end " +
                "end ";

            string CarboniteGotoLua =
            "function Nx.Map:STAC() " +
                "Nx.Que.Wat:CAT() " +
                "local wx,wy=self:FPTWP(self.CFX,self.CFY) " +
                "local zx,zy=self:GZP(self.MaI,wx,wy) " +
                "local str=format('Goto %.0f, %.0f',zx,zy) " +
                "self:SeT3('Goto',wx,wy,wx,wy,nil,nil,str,IsShiftKeyDown()) " +
                MySettings.worldMapCoordName + " = {zx/100,zy/100, GetCurrentMapAreaID()} " +
            "end ";

            string worldMapOnClickLua =
            "function " + MySettings.worldMapButton_OnClickHook + "(button, mouseButton) " +
                "CloseDropDownMenus(); " +
                "if ( mouseButton == 'LeftButton' ) then " +
                    "local x, y = GetCursorPosition() " +
                    "x = x / button:GetEffectiveScale() " +
                    "y = y / button:GetEffectiveScale() " +
                    "local centerX, centerY = button:GetCenter() " +
                    "local width = button:GetWidth() " +
                    "local height = button:GetHeight() " +
                    "local adjustedY = (centerY + (height/2) - y) / height " +
                    "local adjustedX = (x - (centerX - (width/2))) / width " +
                    "if GetCurrentMapZone()~= 0 then " +
                        MySettings.worldMapCoordName + " = {adjustedX,adjustedY, GetCurrentMapAreaID()} " +
                        "WorldMapPing:Show() " +
                        "WorldMapPing:SetPoint('TOPLEFT', 'WorldMapDetailFrame', 'CENTER',(x - centerX ) - 24,(y - centerY)+ 24) " +
                    "end " +
                "end " +
                MySettings.worldMapButton_OldOnClickHook + "(button, mouseButton) " +
            "end ";

            string worldMapOnUpdateLua =
            "function " + MySettings.WorldMapButton_OnUpdateHook + "(self, elapsed) " +
                "if GetCurrentMapAreaID() == " + MySettings.worldMapCoordName + "[3] then " +
                    "WorldMapPing:Show() " +
                    "WorldMapPing:SetPoint('CENTER', 'WorldMapDetailFrame', 'TOPLEFT', " + MySettings.worldMapCoordName + "[1] * WorldMapDetailFrame:GetWidth()," + MySettings.worldMapCoordName + "[2] * WorldMapDetailFrame:GetHeight()) " +
                "else " +
                    "WorldMapPing:Hide() " +
                "end " +
            "end ";

            string myLua = MySettings.worldMapCoordName + "= {} " +
                getWorldMapCoordLua +
                worldMapOnClickLua +
                "if not " + MySettings.worldMapButton_OldOnClickHook + " then " +
                     MySettings.worldMapButton_OldOnClickHook + "=WorldMapButton:GetScript('OnMouseUp') " +
                "end " +
                "WorldMapButton:SetScript('OnMouseUp'," + MySettings.worldMapButton_OnClickHook + ") " +
                worldMapOnUpdateLua +
                "WorldMapButton:HookScript('OnUpdate'," + MySettings.WorldMapButton_OnUpdateHook + ") ";
            if (hasCarbonite)
                myLua += CarboniteGotoLua;
            Lua.DoString(myLua);
        }

        // credit to Apoc
        public static string CreateRandomString(int seed)
        {
            char[] characters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            // Create a random length string
            int length = Rand.Next(6, 13);
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(Rand.NextDouble() > .5
                              ? char.ToUpper(characters[Rand.Next(0, characters.Length)])
                              : characters[Rand.Next(0, characters.Length)]);
            }
            return sb.ToString();
        }

        void WriteProfile(WoWPoint point)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            FileStream fs = File.Open(profilePath, FileMode.Create);
            string buf = string.Format(culture, "<HBProfile>\n<Hotspots>\n{0}\n</Hotspots>\n</HBProfile>\n", ToHotSpot(point));
            fs.Write(encoding.GetBytes(buf), 0, buf.Length);
            fs.Close();
        }

        public class ClickToGoSettings : Settings
        {
            public ClickToGoSettings(string settingsPath)
                : base(settingsPath)
            {
                Load();
            }
            [Setting, DefaultValue("")]
            public string worldMapCoordName { get; set; }

            [Setting, DefaultValue("")]
            public string getWorldMapCoordName { get; set; }

            [Setting, DefaultValue("")]
            public string worldMapButton_OnClickHook { get; set; }

            [Setting, DefaultValue("")]
            public string worldMapButton_OldOnClickHook { get; set; }

            [Setting, DefaultValue("")]
            public string WorldMapButton_OnUpdateHook { get; set; }

        }
    }
}


//!CompilerOption:Optimize:On
// the above line is a flag that turns on optimization.. leave it as it is

// ********************* Highvoltz's Premium Honorbuddy Plugin Template **********************

// if the project files are not in HB's folder you can have the Post Build events automatically copy the files over
// to the Honorbuddy folder. To set it up go to Project -> Project Properties -> Build Events -> Edit Post Build
// and remove the 'rem' at the front of the line and change the 2nd string to the path you want to copy the files to.
// can be relative or absolute path. If you have a dropbox account you can also have it copy directly to your Dropbox folder
// xcopy "$(ProjectDir)*.cs" "$(ProjectDir)..\..\Honorbuddy\Plugins\TestPLGIN" /I /S /Y

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.AreaManagement;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Plugins;
using System.Text;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.WoWInternals.WoWObjects;
using Styx.Patchables;
using System.Collections.Specialized;
using Tripper.Tools.Math;

namespace RoutesBuddy
{
	class Route
	{
		public Route(uint continentID, string name, List<WoWPoint> points)
		{
			this.ContinentID = continentID;
			this.Name = name;
			this.Points = points;
		}
		public uint ContinentID { get; private set; }
		public string Name { get; private set; }
		public List<WoWPoint> Points { get; private set; }
		public override string ToString()
		{
			return Name;
		}
	}
	class RoutesBuddy : HBPlugin, IDisposable
	{
		public override string Name { get { return "RoutesBuddy"; } }
		public override string Author { get { return "Highvoltz"; } }
		public override Version Version { get { return new Version(1, 0, 0, 3); } }
		public override string ButtonText { get { return Name; } }
		public override bool WantButton { get { return true; } }

		public static RoutesBuddy Instance { get; private set; }
		public static System.Random Rng = new Random();
		public event EventHandler OnImportDone;

		public RoutesBuddySettings MySettings;
		LocalPlayer Me = ObjectManager.Me;
		MainForm Gui;
		public List<Route> Routes;
		public List<List<string>> RawImport;

		public RoutesBuddy() { Instance = this; }
		// put your initialization code in here..
		public override void Initialize()
		{
			MySettings = new RoutesBuddySettings
				(Path.Combine(Logging.ApplicationPath, string.Format(@"Settings\{0}\{0}-{1}.xml", Name, Me.Name)));
			Gui = new MainForm();
		}
		public override void OnButtonPress()
		{
			Gui.ShowDialog();
		}
		// Abort any created threads or unhook any events here 
		public override void Dispose()
		{
		}
		// this gets called on every plugin pulse.. like 13 times per sec, put your main code here
		public override void Pulse()
		{
		}
		public void ImportRoutes()
		{
			Routes = new List<Route>();
			string table = RandomString;
			using (new FrameLock())
			{
				string lua1 = string.Format("if RoutesDB then {0} = {{}} for k,v in pairs(RoutesDB.global.routes) do for k1,v1 in pairs (v) do if #v1.route > 0 then table.insert({0},{{k,k1,unpack(v1.route)}}) end end end return #{0} end return 0 ", table);
				int routeNum = Lua.GetReturnVal<int>(lua1, 0);
				if (routeNum == 0)
				{
					Logging.Write(System.Drawing.Color.Red, "No Routes found");
					return;
				}
				RawImport = new List<List<string>>();
				for (int i = 1; i <= routeNum; i++)
				{
					List<string> retVal = Lua.GetReturnValues(string.Format("return unpack({0}[{1}])", table, i));
					if (retVal != null && retVal.Count >= 3)
					{
						RawImport.Add(retVal);
					}
				}
				Lua.DoString(table + " = {}");
			}

			try
			{
				uint myContinent = ObjectManager.Me.MapId;
				for (int i=0;i< RawImport.Count;i++)
				{
					uint mapID = FindMapId(RawImport[i][0]);
					uint continentID = GetContinentId(mapID);
					if (myContinent != continentID)
					{
						Logging.Write("Skipping {0} because its on a different continent", RawImport[i][1]);
					}
					else
					{
						if (mapID > 0)
						{
							List<WoWPoint> points = new List<WoWPoint>();
							for (int n = 2; n < RawImport[i].Count; n++)
							{
								float x, y;
								uint coord;
								uint.TryParse(RawImport[i][n], out coord);
								//local ex, ey = floor(point / 10000) / 10000, (point % 10000) / 10000
								x = (float)Math.Floor((float)coord / 10000f) / 10000f;
								y = ((float)coord % 10000f) / 10000f;
								points.Add(mapToWorldCoords(x, y, mapID));
								int retCnt = RawImport.Count;
								int process1 = (i * 100) / retCnt;
								int process2 = ((i + 1) * 100) / retCnt;
								int process3 = (n * (process2 - process1) / RawImport[i].Count) + process1;
								Gui.UpdateProgressBar(process3);
							}
							if (points.Count > 0)
								Routes.Add(new Route(mapID, RawImport[i][1], ProcessPoints(points)));
						}
					}
				}
			}
			catch (Exception ex) { Logging.Write(System.Drawing.Color.Red, ex.ToString()); }
			if (OnImportDone != null)
				OnImportDone(Instance, null);
		}

		List<WoWPoint> ProcessPoints(List<WoWPoint>points)
		{
			List<WoWPoint> newPoints = new List<WoWPoint>();
			newPoints.Add(points[0]);
			for (int i = 1; i < points.Count;i++ )
			{
				List<WoWPoint> tempPoints = CreatePathSegment(points[i - 1], points[i]);
				if (Gui.smoothCheck.Checked)
					tempPoints = SmoothOut3dSegment(tempPoints);
				newPoints.AddRange(tempPoints);
			}
			return newPoints;
		}

		List<WoWPoint> CreatePathSegment(WoWPoint from, WoWPoint to)
		{
			List<WoWPoint> segment = new List<WoWPoint>();
			WoWPoint point = from;
			float step = 50;
			float noMeshStep = 5;
			for (float i = from.Distance(to) - step; i > 0;)
			{
				point = WoWMathHelper.CalculatePointFrom(from, to, i);
				try
				{
					float z = Navigator.FindHeights(point.X, point.Y).Max() ;
					i -= step;
					if (Gui.smoothCheck.Checked && z > point.Z)
						point.Z = z;
					segment.Add(point);
				}
				catch { i -= noMeshStep; }
			}
			segment.Add(to);
			return segment;
		}

		//List<WoWPoint> SmoothOut3dSegment(List<WoWPoint> path)
		//{
		//    List<WoWPoint> newPath = new List<WoWPoint>();
		//    WoWPoint end = path[path.Count - 1];
		//    int startI = 0;
		//    int maxIndex = startI;
		//    newPath.Add(path[0]);
		//    while (startI < path.Count - 1)
		//    {
		//        float maxAngDif = 0;
		//        for (int i = startI; i < path.Count; i++)
		//        {
		//            WoWPoint endV = path[startI].GetDirectionTo(end);
		//            WoWPoint pointV = path[startI].GetDirectionTo(path[i]);
		//            end.X = end.Z;
		//            pointV.X = pointV.Z;
		//            float angDiff = GetVectorAngleDiff2(endV, pointV);
		//            if (angDiff >= maxAngDif)
		//            {
		//                maxAngDif = angDiff;
		//                maxIndex = i;
		//            }
		//        }
		//        startI = maxIndex;
		//        newPath.Add(path[startI]);

		//    }
		//    return newPath;
		//}

		List<WoWPoint> SmoothOut3dSegment(List<WoWPoint> path)
		{
			WoWPoint end = path[path.Count - 1];
			int startI = 0;
			int maxIndex = startI;
			while (startI < path.Count - 1)
			{
				float maxAngDif = 0;
				for (int i = startI; i < path.Count; i++)
				{
					WoWPoint endV = path[startI].GetDirectionTo(end);
					WoWPoint pointV = path[startI].GetDirectionTo(path[i]);
					end.X = end.Z;
					pointV.X = pointV.Z;
					float angDiff = GetVectorAngleDiff2(endV, pointV);
					if (angDiff >= maxAngDif)
					{
						maxAngDif = angDiff;
						maxIndex = i;
					}
				}
				float multiplier = (path[maxIndex].Z - path[startI].Z) / (path[startI].Distance2D(path[maxIndex]));
				for (int n = startI+1; n < maxIndex; n++)
				{
					WoWPoint point = path[n];
					point.Z = (path[startI].Distance2D(path[n]) * multiplier) + path[startI].Z;
					path[n]= point;
				}
			   startI = maxIndex;
			}
			return path;
		}

		WoWPoint mapToWorldCoords(float x, float y, uint mapId)
		{
			WoWPoint worldPoint = new WoWPoint();
			WoWDb.DbTable worldMapArea = StyxWoW.Db[ClientDb.WorldMapArea];
			WoWDb.Row worldMapAreaFields = worldMapArea.GetRow(mapId);
			float ay = worldMapAreaFields.GetField<float>(4);
			float by = worldMapAreaFields.GetField<float>(5);
			float ax = worldMapAreaFields.GetField<float>(6);
			float bx = worldMapAreaFields.GetField<float>(7);
			worldPoint.X = ax + (y * (bx - ax));
			worldPoint.Y = ay + (x * (by - ay));
			try
			{
				worldPoint.Z = Navigator.FindHeights(worldPoint.X, worldPoint.Y).Max();
			}
			catch { return TryGetHieght(worldPoint); }
			return worldPoint;
		}

		WoWPoint TryGetHieght(WoWPoint point)
		{
			float PIx2 = (float)Math.PI * 2f;
			int step = 20;
			for (int d = 5; d <= 50; d += 5)
			{
				for (int i = 0; i < 20; i++)
				{
					WoWPoint newPoint = point.RayCast((i * PIx2) / step, d);
					try
					{
						newPoint.Z = Navigator.FindHeights(newPoint.X, newPoint.Y).Max();
						return newPoint;
					}
					catch { }
				}
			}
			return point;
		}

		uint FindMapId(string localMapName)
		{
			WoWDb.DbTable t = StyxWoW.Db[ClientDb.WorldMapArea];
			int max = t.MaxIndex;
			for (int i = t.MinIndex; i <= max; )
			{
				WoWDb.Row r = t.GetRow((uint)i);
				string mapName = ObjectManager.Wow.Read<string>(r.GetField<uint>(3));
				if (mapName == localMapName)
					return (uint)i;
				if (i < max)
					i = r.GetField<int>(12);
				else
					break;
			}
			return 0;
		}

		uint GetContinentId(uint mapID)
		{
			WoWDb.DbTable t = StyxWoW.Db[ClientDb.WorldMapArea];
			WoWDb.Row r = t.GetRow(mapID);
			return r.GetField<uint>(1);
		}

		public static string RandomString
		{
			get
			{
				int size = Rng.Next(6, 15);
				StringBuilder sb = new StringBuilder(size);
				for (int i = 0; i < size; i++)
				{
					// random upper/lowercase character using ascii code
					sb.Append((char)(Rng.Next(2) == 1 ? Rng.Next(65, 91) + 32 : Rng.Next(65, 91)));
				}
				return sb.ToString();
			}
		}
		// get heading of a vector
		static Func<Vector3, Vector3, float> GetHeading =
			(a, b) => WoWMathHelper.NormalizeRadian((float)Math.Atan2(b.Y - a.Y, b.X - a.X));
		// angle dif 0 - PI*2
		static Func<Vector3, Vector3, float> GetVectorAngleDiff =
			(a, b) => WoWMathHelper.NormalizeRadian(GetHeading(Vector3.Zero, b) - GetHeading(Vector3.Zero, a));
		// angle dif 0 - PI
		static Func<Vector3, Vector3, float> GetVectorAngleDiff2
			= (a, b) => { float r = GetVectorAngleDiff(a, b); return r > Math.PI ? ((float)Math.PI * 2) - r : r; };

	}
}

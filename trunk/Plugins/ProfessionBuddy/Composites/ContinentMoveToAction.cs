using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Styx;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Styx.Logic;
using Styx.Combat.CombatRoutine;
using Styx.Logic.POI;
using Styx.Patchables;
using Styx.Logic.AreaManagement;
using Tripper.Tools.Math;
using System.Collections.Specialized;

//using ObjectManager = Styx.WoWInternals.ObjectManager;

///* HBConsole portal code

//List<WoWObject> objs = ObjectManager.ObjectList.Where(o => o.Name.Contains("Portal")).ToList();
//foreach(var obj in objs)
//  Logging.Write("{0} Id: {1} {2} mapId: {3}",obj.Name,obj.Entry,obj.Location,ObjectManager.Me.MapId);
//*/
namespace HighVoltz.Composites
{
    public enum Continent { EasternKingdom, Kalimdor, Outlands = 530, Northrend = 571, Maelstrom = 751 }
    public enum PortalFaction { Alliance, Horde, Nuetral }
    public enum TransportDirection { OneWay, TwoWay }
    // Ride = boat,zeplin,trans. 
    // Interact = objects that require a 'right-click' to use them, for example portal to Blasted Lands
    // Walkthrough = portals that you walk through to zone. example the portal to Outlands
    // Spell = mage port or similar spells
    public enum TransportInteractionType { Ride, Interact, Walkthrough, Spell }

    public class Transport
    {
        // setup some default values
        public Transport()
        {
            Direction = TransportDirection.OneWay;
            Faction = PortalFaction.Nuetral;
            Class = WoWClass.None;
            StartLandPoint = EndLandPoint = StartPoint = EndPoint = StartBoardPoint = EndBoardPoint = ExitPoint = WaitPoint = WoWPoint.Zero;
            ItemId = LevelRequirement = Id = 0;
        }
        public TransportInteractionType InteractionType { get; internal set; }
        public PortalFaction Faction { get; internal set; }
        public TransportDirection Direction { get; internal set; }
        public WoWClass Class { get; internal set; }
        /// <summary>
        /// The Continent on which the transport is locatated
        /// </summary>
        public Continent StartContinent { get; internal set; }
        /// <summary>
        /// The Continent which the tranport ends at
        /// </summary>
        public Continent EndContinent { get; internal set; }
        /// <summary>
        /// Used for a landing spot when using Flightor.It then dismounts and travels the rest of the way on foot/landmount
        /// Should be outside with nothing overhead
        /// </summary>
        public WoWPoint StartLandPoint { get; internal set; }
        /// <summary>
        /// Used for a landing spot when using Flightor, used on the 'EndPoint' side of the transport if it's a two way transport
        /// It then dismounts and travels the rest of the way on foot/landmount
        /// Should be outside with nothing overhead
        /// </summary>
        public WoWPoint EndLandPoint { get; internal set; }
        /// <summary>
        /// End location of the transport
        /// For TwoWay Walkthrough tansport this is the point where to walk to on the oposite end to get teleported.
        /// </summary>
        public WoWPoint StartPoint { get; internal set; }
        /// <summary>
        /// End location of the transport
        /// For TwoWay Walkthrough tansport this is the point where to walk to on the oposite end to get teleported.
        /// </summary>
        public WoWPoint EndPoint { get; internal set; }
        /// <summary>
        /// Location at which to wait for the transport apear. also used for a landing location for flying mounts
        /// For WalkThrough portals its the point you get teleported to when walking through the portal from the 'EndPoint' side
        /// and should be close to the StartPoint.
        /// </summary>
        public WoWPoint WaitPoint { get; internal set; }
        /// <summary>
        /// the location to stand at on the Transport at StartPoint location
        /// </summary>
        public WoWPoint StartBoardPoint { get; internal set; }
        /// <summary>
        /// the location to stand at on the Transport at EndPoint side
        /// </summary>
        public WoWPoint EndBoardPoint { get; internal set; }
        /// <summary>
        /// The Location to move to when getting off the transport
        /// For WalkThrough portals its the point you get teleported to when walking through the portal from the 'StartPoint' side
        /// and should be close to the EndPoint.
        /// </summary>
        public WoWPoint ExitPoint { get; internal set; }
        /// <summary>
        /// Id of the transport object, or the spellId if transport is a spell
        /// </summary>
        public uint Id { get; internal set; }
        public uint LevelRequirement { get; internal set; }
        /// <summary>
        /// Id of item that summons the transport if theres one.
        /// </summary>
        public uint ItemId { get; internal set; }
    }

    public class ContinentArea : PolygonArea
    {
        public ContinentArea(Continent cont,List<Transport> trans,List<uint> areaIds,Vector2[] areaDefinition):base(areaDefinition)
        {
            Continent = cont;
            Transports = trans;
            AreaIdList = areaIds;
        }
        public override AreaType Type { get { return AreaType.Polygon; } }
        public Continent Continent { get; private set; }
        public List<Transport> Transports { get; private set; }
        public List<uint> AreaIdList { get; private set; }
    }

//    public class ContinentMoveToAction : PBAction
//    {
//        #region Transport List
//        public List<Transport> MasterTransportList = new List<Transport>()
//        {
//        new Transport() // Blasted Lands portal to Outlands
//            {
//                InteractionType = TransportInteractionType.Walkthrough,
//                Direction = TransportDirection.TwoWay,
//                StartContinent = Continent.EasternKingdom, 
//                EndContinent = Continent.Outlands,
//                StartPoint = new WoWPoint(-11907.08, -3208.696, -14.85771),
//                EndPoint = new WoWPoint(-246.2763, 895.8818, 84.38078),
//                WaitPoint = new WoWPoint(-11896.8, -3206.77, -14.6724),
//                ExitPoint = new WoWPoint(-248.113, 922.9, 84.3497),
//                LevelRequirement = 57,
//            },
//            new Transport() // Stormwind portal to Blasted Lands
//            {
//                InteractionType = TransportInteractionType.Interact,
//                Direction = TransportDirection.OneWay,
//                StartContinent = Continent.EasternKingdom, 
//                EndContinent = Continent.EasternKingdom,
//                StartPoint = new WoWPoint(-9007.58, 871.8698, 129.6922),
//                EndPoint = new WoWPoint(-11708.4, -3168, -5.07),
//                StartLandPoint = new WoWPoint(-8981.373, 870.5462, 122.2039),
//                LevelRequirement = 57,
//                Faction = PortalFaction.Alliance, 
//                Id =195141, 
//            },
//            new Transport() // Ironforge portal to Blasted Lands
//            {
//                InteractionType = TransportInteractionType.Interact,
//                Direction = TransportDirection.OneWay,
//                StartContinent = Continent.EasternKingdom, 
//                EndContinent = Continent.EasternKingdom,
//                StartPoint = new WoWPoint(-4606.441, -928.9965, 501.07),
//                EndPoint = new WoWPoint(-11708.4, -3168, -5.07),
//                StartLandPoint = new WoWPoint(-5043.865,-807.1965,495.1276),
//                LevelRequirement = 57,
//                Faction = PortalFaction.Alliance, 
//                Id =195141, 
//            },
//            new Transport() // Stormwind ship to Worlk.
//            {
//                InteractionType = TransportInteractionType.Ride,
//                Direction = TransportDirection.TwoWay,
//                StartContinent = Continent.EasternKingdom, 
//                EndContinent = Continent.Northrend,
//                StartPoint = new WoWPoint(-8288.816, 1424.703, 0.04611586),
//                EndPoint = new WoWPoint(2218.391, 5119.589, -0.0279433),
//                WaitPoint = new WoWPoint(-8295.013, 1406.331, 4.400626),
//                ExitPoint = new WoWPoint(2231.906, 5133.915, 5.343191),
//                StartBoardPoint = new WoWPoint(-8294.499, 1423.412, 9.458605),
//                EndBoardPoint = new WoWPoint(2224.042, 5118.323, 9.570542),
//                Faction = PortalFaction.Alliance, 
//                Id = 190536, 
//            },
//            new Transport() // Stormwind ship to Ret'theran Village Kilimdor.
//            {
//                InteractionType = TransportInteractionType.Ride,
//                Direction = TransportDirection.TwoWay,
//                StartContinent = Continent.EasternKingdom, 
//                EndContinent = Continent.Kalimdor,
//                StartPoint = new WoWPoint(-8650.719, 1346.051, -0.04878661),
//                EndPoint = new WoWPoint(8162.587, 1005.365, -0.006209105),
//                WaitPoint = new WoWPoint(-8640.669, 1327.066, 5.233096),
//                ExitPoint = new WoWPoint(8180.222, 1002.822, 6.924821),
//                StartBoardPoint = new WoWPoint(-8646.097, 1343.046, 6.056096),
//                EndBoardPoint = new WoWPoint(8168.344, 1005.508, 6.20304),
//                Faction = PortalFaction.Alliance, 
//                Id = 176310, 
//            },
//            new Transport() //  Ret'theran Village ship to Azuremyst Isle
//            {
//                InteractionType = TransportInteractionType.Ride,
//                Direction = TransportDirection.TwoWay,
//                StartContinent = Continent.Kalimdor, 
//                EndContinent = Continent.Outlands,
//                StartPoint = new WoWPoint(8346.647, 1177.085, -0.03221168),
//                EndPoint = new WoWPoint(-4264.997, -11317.2, -0.02397193),
//                WaitPoint = new WoWPoint(8345.854, 1161.569, 4.633209),
//                ExitPoint = new WoWPoint(-4260.84, -11332.68, 5.637951),
//                StartBoardPoint = new WoWPoint(8345.248, 1172.261, 5.037251),
//                EndBoardPoint = new WoWPoint(-4261.748, -11322.04, 5.022839),
//                Faction = PortalFaction.Alliance, 
//                Id = 181646, 
//            },
//            //new Transport() //  Ret'theran Village ship to Azuremyst Isle
//            //{
//            //    InteractionType = TransportInteractionType.Ride,
//            //    Direction = TransportDirection.TwoWay,
//            //    StartContinent = Continent.Kalimdor, 
//            //    EndContinent = Continent.Kalimdor,
//            //    StartPoint = new WoWPoint(8346.647, 1177.085, -0.03221168),
//            //    EndPoint = new WoWPoint(-4264.997, -11317.2, -0.02397193),
//            //    WaitPoint = new WoWPoint(8344.349, 1157.741, 4.839566),
//            //    ExitPoint = new WoWPoint(-4260.84, -11332.68, 5.637951),
//            //    StartBoardPoint = new WoWPoint(8344.603, 1173.043, 5.01434),
//            //    EndBoardPoint = new WoWPoint(-4261.748, -11322.04, 5.022839),
//            //    Faction = PortalFaction.Alliance, 
//            //    Id = 181646, 
//            //},
//        };
////        #endregion

//        public static List<ContinentArea> ContinentAreas { get; private set; }
//        public ContinentMoveToAction()
//        {
//            if (ContinentAreas == null)
//                ContinentAreas = BuildContinentAreas();
//        }
//        struct MapBounds
//        {
//            public Vector2 TopLeft;
//            public Vector2 BottomRight;
//            public Continent Continent;
//            public uint Area;
//            public override string ToString()
//            {
//                var table = StyxWoW.Db[ClientDb.AreaTable];
//                var row = table.GetRow(Area);
//                string name = ObjectManager.Wow.Read<string>(row.GetField<uint>(11));
//                //int flags = row.GetField<int>(4);
//                //BitVector32 ba = new BitVector32(flags);
//                return string.Format("Continent: {0} <{1},{2}> <{3},{4}> {5} {6}",Continent,TopLeft.X,TopLeft.Y,BottomRight.X,BottomRight.Y,name);
//            }
//        }

//        List<ContinentArea> BuildContinentAreas()
//        {
//            List<MapBounds> MapBoundsList = BuildMapBounds();
//            List<ContinentArea> contList = BuildContinentBounds(MapBoundsList);
//            return contList;
//        }

//        List <ContinentArea> BuildContinentBounds(List<MapBounds> mBounds)
//        {
//            List<ContinentArea> caList = new List<ContinentArea>();
//            foreach (MapBounds mb in mBounds)
//                Logging.Write(mb.ToString());
//            return caList;
//        }
//        List <MapBounds> BuildMapBounds()
//        {
//            List<MapBounds> mList = new List<MapBounds>();
//            var table = StyxWoW.Db[ClientDb.WorldMapArea];
//            for (int i = table.MinIndex; i < table.MaxIndex; )
//            {
//                var row = table.GetRow((uint)i);
//                uint area = row.GetField<uint>(2);
//                if (area > 0 && !IsInstance(area))
//                {
//                    MapBounds mb = new MapBounds();
//                    int cont = row.GetField<int>(1);
//                    int vCont = row.GetField<int>(8);
//                    float ay = row.GetField<float>(4);
//                    float by = row.GetField<float>(5);
//                    float ax = row.GetField<float>(6);
//                    float bx = row.GetField<float>(7);
                    
//                    mb.Continent = (Continent)(vCont != -1 ? vCont : cont);
//                    mb.TopLeft = new Vector2(ax, ay);
//                    mb.BottomRight = new Vector2(bx, by);
//                    mb.Area = area;
//                    mList.Add(mb);
//                }
//                if (i < table.MaxIndex)
//                    i = row.GetField<int>(12);
//                else
//                    break;
//            }
//            return mList;
//        }

//        bool IsInstance (uint area)
//        {
//            var table = StyxWoW.Db[ClientDb.AreaTable];
//            var row = table.GetRow(area);
//            uint mapRow = row.GetField<uint>(1);
//            table  = StyxWoW.Db[ClientDb.Map];
//            row = table.GetRow(mapRow);
//            int isDungeon = row.GetField<int>(2);
//            return isDungeon > 0 && isDungeon < 5;
//        }
//    }
}

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.WoWInternals;

namespace fnav
{
    public partial class UIForm1 : Form
    {
        readonly string _profilePath = string.Format(@"{0}\Plugins\FNav\FNavGoTo.xml", Logging.ApplicationPath);
        static readonly string FnavDataFile = string.Format(@"{0}\Plugins\FNav\fnav.xml", Logging.ApplicationPath);
        private bool _loading = true;
        private bool _firstrun = true;
        private WoWPoint _currentGoToLocation;


        public UIForm1() { InitializeComponent(); }
        private void UIForm1_Load(object sender, EventArgs e)
        {
            _loading = true;
            PopulateLocations();
            useFlightPath.Checked = CharacterSettings.Instance.UseFlightPaths;
            _loading = false;
        }

        public void PopulateLocations()
        {
            bool needToUpdateZones = false;
            _loading = true;
            LocationList.Items.Clear();
            
            // Populate category and zone filters
            if (_firstrun)
            {
                ZoneFilter.Items.Clear(); CategoryFilter.Items.Clear();
                if (ZoneFilter.Items.Count == 0) ZoneFilter.Items.Add("[All]");
                if (CategoryFilter.Items.Count == 0) CategoryFilter.Items.Add("[All]");

                using (Locations locations = new Locations())
                {
                    foreach (Location loc in locations)
                    {
                        if (loc.Category != "" && !CategoryFilter.Items.Contains(loc.Category)) CategoryFilter.Items.Add(loc.Category);
                        if (loc.Category != "" && !CategoryNew.Items.Contains(loc.Category)) CategoryNew.Items.Add(loc.Category);
                        if (loc.Zone != "" && !ZoneFilter.Items.Contains(loc.Zone)) ZoneFilter.Items.Add(loc.Zone);
                    }
                }
                
                CategoryFilter.SelectedIndex = 0;
                ZoneFilter.SelectedIndex = 0;
            }

            needToUpdateZones = ZoneFilter.SelectedItem.ToString().Contains("[All]");
            if (needToUpdateZones)
            {
                ZoneFilter.Items.Clear();
                ZoneFilter.Items.Add("[All]");
                ZoneFilter.SelectedIndex = 0;
            }

            //Logging.Write(ObjectManager.Me.MapName);
            // Populate 'Locations' dropdown);
            using (Locations locations = new Locations()) 
            { 
                foreach (Location loc in locations)
                {
                    if (!ObjectManager.Me.IsAlliance && loc.Faction.Contains("Alliance")) continue;
                    if (!ObjectManager.Me.IsHorde && loc.Faction.Contains("Horde")) continue;
                    if (ObjectManager.Me.MapName != loc.Continent) continue;
                    if (!CategoryFilter.SelectedItem.ToString().Contains("[All]") && loc.Category != CategoryFilter.SelectedItem.ToString()) continue;
                    if (!ZoneFilter.SelectedItem.ToString().Contains("[All]") && loc.Zone != ZoneFilter.SelectedItem.ToString()) continue;

                    if (!CategoryFilter.SelectedItem.ToString().Contains("[All]")) loc.FilteringByCategory = true;
                    if (!ZoneFilter.SelectedItem.ToString().Contains("[All]")) loc.FilteringByZone = true;
                    LocationList.Items.Add(loc);

                    if (needToUpdateZones && !ZoneFilter.Items.Contains(loc.Zone)) ZoneFilter.Items.Add(loc.Zone);
                    
                } 
            }

            if (LocationList.Items.Count >0) LocationList.SelectedIndex = 0;

            _firstrun = false;
            _loading = false;
        }


        private void CategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            _loading = true;
            PopulateLocations();
        }

        private void ZoneFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            _loading = true;
            PopulateLocations();
        }
        

        void WriteProfile(string x, string y, string z,string profileName)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            using (FileStream fs = File.Open(_profilePath, FileMode.Create))
            {
                string s = string.Format("<HBProfile>\n<Name>{3}</Name>\n<Hotspots>\n<Hotspot X=\"{0}\" Y=\"{1}\" Z=\"{2}\" />\n</Hotspots>\n</HBProfile>\n", x, y, z, profileName);
                fs.Write(encoding.GetBytes(s), 0, s.Length);
                fs.Close();
            }
        }

        private void AddCurrentLocation_Click(object sender, EventArgs e)
        {
            if (ObjectManager.Me.Location.X.ToString() == "0" && ObjectManager.Me.Location.Y.ToString() == "0" && ObjectManager.Me.Location.Z.ToString() == "0")
            {
                MessageBox.Show("Ahhhhhhh SHIT! HB is bugging out, our current location is <0,0,0> which is obviously wrong. Location has not been added","Ahhhhhh Shit!",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }

            errorProvider1.SetIconAlignment(CategoryNew,ErrorIconAlignment.MiddleLeft);
            errorProvider1.SetIconAlignment(NewLocationName, ErrorIconAlignment.TopLeft);

            if (CategoryNew.Text == "") errorProvider1.SetError(CategoryNew, "Category name can not be blank");
            if (NewLocationName.Text == "") errorProvider1.SetError(NewLocationName, "Location name can not be blank");

            if (NewLocationName.Text == "")
            {
                MessageBox.Show("The location name can not be blank, add a location name in the above field.","Location name is blank", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
                return;
            }
            if (CategoryNew.Text == "")
            {
                MessageBox.Show("The category name can not be blank. Select a category name from the drop down or add a new one.", "Category name is blank", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            errorProvider1.SetError(NewLocationName, "");
            errorProvider1.SetError(CategoryNew, "");

            Logging.Write(string.Format("-FNav adding new location {0} ({1}) in {2}", NewLocationName.Text, CategoryNew.Text, ObjectManager.Me.ZoneText));


            string faction = "";
            if (ObjectManager.Me.IsHorde) faction = "Horde";
            if (ObjectManager.Me.IsAlliance) faction = "Alliance";

            string subzone = "no subzone";
            if (ObjectManager.Me.SubZoneText != "") subzone = ObjectManager.Me.SubZoneText;

            string categorytext = CategoryNew.Text;


            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(FnavDataFile);

            XmlNode node = xdoc.CreateNode(XmlNodeType.Element, "Location", null);
            
            XmlNode locationNameNode = xdoc.CreateElement("Name");
            locationNameNode.InnerText = NewLocationName.Text;

            XmlNode factionNode = xdoc.CreateElement("Faction");
            factionNode.InnerText = faction;

            XmlNode zonenameNode = xdoc.CreateElement("Zone");
            zonenameNode.InnerText = ObjectManager.Me.RealZoneText;

            XmlNode subzonenameNode = xdoc.CreateElement("SubZone");
            subzonenameNode.InnerText = subzone;

            XmlNode continentnameNode = xdoc.CreateElement("Continent");
            continentnameNode.InnerText = ObjectManager.Me.MapName;

            XmlNode categoryNode = xdoc.CreateElement("Category");
            categoryNode.InnerText = categorytext;

            XmlNode xNode = xdoc.CreateElement("X");
            xNode.InnerText = ObjectManager.Me.Location.X.ToString();

            XmlNode yNode = xdoc.CreateElement("Y");
            yNode.InnerText = ObjectManager.Me.Location.Y.ToString();

            XmlNode zNode = xdoc.CreateElement("Z");
            zNode.InnerText = ObjectManager.Me.Location.Z.ToString();

            node.AppendChild(locationNameNode); 
            node.AppendChild(factionNode);
            node.AppendChild(zonenameNode);
            node.AppendChild(subzonenameNode);
            node.AppendChild(continentnameNode);
            node.AppendChild(categoryNode);
            node.AppendChild(xNode);
            node.AppendChild(yNode);
            node.AppendChild(zNode);

            XmlNodeList l = xdoc.GetElementsByTagName("fnav");
            l[0].AppendChild(node);
            xdoc.Save(FnavDataFile);


            PopulateLocations();
        }

        private void GoToLocation_Click(object sender, EventArgs e)
        {
            Location loc = (Location) LocationList.SelectedItem;

            WriteProfile(loc.X,loc.Y,loc.Z,"FNav Go To");
            Logging.Write(string.Format("-FNav moving to {0}, {1} in {2}", CategoryNew.Text, NewLocationName.Text, ObjectManager.Me.ZoneText));
            _currentGoToLocation = new WoWPoint(float.Parse(loc.X),float.Parse(loc.Y),float.Parse(loc.Z));
            ProfileManager.LoadNew(_profilePath);

            /*
            try
            {
                int distanceToDestination = (int)_currentGoToLocation.Distance2D(ObjectManager.Me.Location);
                distanceBar.Maximum = distanceToDestination;
            }
            finally { }
             */

            if (!TreeRoot.IsRunning) TreeRoot.Start();

        }

        private void RemoveLocation_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Are you sure you want to remove this item?","Remove Item",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (answer == DialogResult.No)return;

            XmlDocument xdoc = new XmlDocument(); xdoc.Load(FnavDataFile);
            XmlNodeList nodes = xdoc.DocumentElement.ChildNodes;
            Location loc = (Location)LocationList.SelectedItem;

            foreach (XmlNode node in nodes)
            {
                if (node.InnerText.Contains(loc.Name) && node.InnerText.Contains(loc.X) && node.InnerText.Contains(loc.Y) && node.InnerText.Contains(loc.Z))
                {
                    xdoc.DocumentElement.RemoveChild(node);
                    break;
                }
            }

            xdoc.Save(FnavDataFile);
            PopulateLocations();
        }

        private void Locations_SelectedIndexChanged(object sender, EventArgs e)
        {
            Location loc = (Location) LocationList.SelectedItem;
            WoWPoint destination = new WoWPoint(float.Parse(loc.X), float.Parse(loc.Y), float.Parse(loc.Z));
            int distanceToDestination = (int)destination.Distance2D(ObjectManager.Me.Location);
            //string distanceToDest = "";


            statusLabel.Text = string.Format("{0} ({1}) in {2} is {3} yards", loc.Name, loc.Category, loc.Zone, distanceToDestination);

            //statuslabel.Text = loc.Name + subzone + zone;
        }

        public class Locations : IEnumerable, IDisposable
        {
            // When the foreach loop begins, this method is invoked 
            // so that the loop gets an enumerator to query.
            public IEnumerator GetEnumerator()
            {
                return new LocationEnumerator();
            }

            public void Dispose()
            {
                //throw new NotImplementedException();
            }
        }

        public class LocationEnumerator : IEnumerator
        {
            private XmlTextReader _reader;

            // IEnumerator Members
            // Called when the enumerator needs to be reinitialized.
            public void Reset()
            {
                if (_reader != null) _reader.Close();
                System.Diagnostics.Debug.Assert(File.Exists(FnavDataFile), "Location file does not exist!");
                StreamReader stream = new StreamReader(FnavDataFile);
                _reader = new XmlTextReader(stream);
            }

            // Called just prior to Current being invoked.  
            // If true is returned then the foreach loop will
            // try to get another value from Current.  
            // If false is returned then the foreach loop terminates.
            public bool MoveNext()
            {
                // Call Reset the first time MoveNext is called 
                // instead of in the constructor 
                // so that we keep the stream open only as long as needed.
                if (_reader == null) Reset();

                if (FindNextTextElement()) return true;

                // If there are no more text elements in the XML file then
                // we have read in all of the data 
                // and the foreach loop should end.
                _reader.Close(); return false;
            }

            // Invoked every time MoveNext() returns true. 
            // This extracts the values for the "current" location from 
            // the XML file and returns that data packaged up as a Location object.
            public object Current
            {
                get
                {
                    string val;

                    // No need to call FindNextTextElement here
                    // because it was called for us by MoveNext().
                    string name = _reader.Value;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string faction = val;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string zone = val;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string subzone = val;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string continent = val;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string category = val;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string x = val;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string y = val;

                    val = "";
                    if (FindNextTextElement()) val = _reader.Value;
                    string z = val;

                    return new Location(name,faction, zone,subzone,continent,category, x, y, z);
                }
            }


            // Advances the XmlTextReader to the next Text
            // element in the XML stream.
            // Returns true if there is more data to be read
            // from the stream, else false.
            private bool FindNextTextElement()
            {
                bool readOn = _reader.Read();
                bool prevTagWasElement = false;
                while (readOn && this._reader.NodeType != XmlNodeType.Text)
                {
                    // If the current element is empty, stop reading and return false.
                    if (prevTagWasElement && _reader.NodeType == XmlNodeType.EndElement) readOn = false;
                    prevTagWasElement = _reader.NodeType == XmlNodeType.Element;
                    readOn = readOn && _reader.Read();
                }
                return readOn;
            }
        }

        public class Location
        {
            public Location(string name,string faction,string zone,string subzone,string continent, string category, string x, string y, string z)
            {
                Name = name; Faction = faction; Zone = zone; SubZone = subzone;Continent = continent;Category = category; X = x; Y = y; Z = z;
            }

            public string Name { get; private set; }
            public string Faction{ get; private set; }
            public string Zone { get; private set; }
            public string SubZone { get; private set; }
            public string Continent { get; private set; }
            public string Category { get; private set; }
            public string X { get; private set; }
            public string Y { get; private set; }
            public string Z { get; private set; }

            public bool FilteringByCategory { get; set; }
            public bool FilteringByZone { get; set; }

            
            
            //public override string ToString() { return Name; }
            public override string ToString()
            {
                string displayAs = "";
                //string subzonetext = " (" + SubZone + ", " + Zone + ")";
                //if (subzonetext.Contains("no subzone")) subzonetext = "";

                //return Category + " - " + Name + subzonetext;
                if (!FilteringByCategory) displayAs = Category + " - ";
                displayAs = displayAs + Name;
                if (!SubZone.Contains("no subzone")) displayAs = displayAs + " (" + SubZone; if (SubZone.Contains("no subzone") && !FilteringByZone) displayAs = displayAs + " (";
                if (!SubZone.Contains("no subzone") && !FilteringByZone) displayAs = displayAs + ", ";
                if (!FilteringByZone) displayAs = displayAs + Zone + ")"; if (FilteringByZone && !SubZone.Contains("no subzone")) displayAs = displayAs + ")";
                
                return displayAs;
            }
        }

        private void Subzone_Click(object sender, EventArgs e)
        {
            NewLocationName.Text = ObjectManager.Me.SubZoneText;
        }

        private void useFlightPath_CheckedChanged(object sender, EventArgs e)
        {
            CharacterSettings.Instance.UseFlightPaths = useFlightPath.Checked;
        }

        private void UIForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Hide();
        }

        private void GoToLocation_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Location locationItem = null;
                int closestLocation = 999999999;

                //Logging.Write("inside mouse click ok");

                foreach (Location loc in LocationList.Items)
                {
                    WoWPoint destination = new WoWPoint(float.Parse(loc.X), float.Parse(loc.Y), float.Parse(loc.Z));
                    int distanceToLocation = (int)destination.Distance2D(ObjectManager.Me.Location);
                    if (distanceToLocation < closestLocation) 
                    {
                        locationItem = loc;
                        closestLocation = distanceToLocation;
                    }
                }

                if (!Equals(null, locationItem)) LocationList.SelectedItem = locationItem;


            }

        }

      

    }
}

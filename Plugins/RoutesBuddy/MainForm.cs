using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Styx.Helpers;
using System.Globalization;
using System.IO;
using Styx.WoWInternals;

namespace RoutesBuddy
{
    public partial class MainForm : Form
    {
        static CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        public MainForm()
        {
            InitializeComponent();
            ProfileTypeCombo.SelectedIndex = 0;
            folderBrowserDialog.SelectedPath = Environment.CurrentDirectory;
        }

        Thread importThread;
        private void ImportBut_Click(object sender, EventArgs e)
        {
            updateProgressBar(0);
            DisabelControls();
            importThread = new Thread(RoutesBuddy.Instance.ImportRoutes);
            importThread.IsBackground = true;
            RoutesBuddy.Instance.OnImportDone += new EventHandler(Instance_OnImportDone);
            importThread.Start();
        }
        void EnableControls()
        {
            progressBar.Visible = false;
            ImportBut.Enabled = true;
            ExportBut.Enabled = true;
            ImportBut.Visible = true;
            ExportBut.Visible = true;
            ProfileTypeCombo.Enabled = true;
            HeightNumeric.Enabled = true;
            RouteList.Enabled = true;
            DebugButton.Enabled = true;
        }

        void DisabelControls()
        {
            progressBar.Visible = true;
            ImportBut.Visible = false;
            ExportBut.Visible = false;
            ImportBut.Enabled = false;
            ExportBut.Enabled = false;
            ProfileTypeCombo.Enabled = false;
            HeightNumeric.Enabled = false;
            RouteList.Enabled = false;
            DebugButton.Enabled = false;
        }

        delegate void invokeDelegate();
        void Instance_OnImportDone(object sender, EventArgs e)
        {
            if (RouteList.InvokeRequired)
                RouteList.BeginInvoke(new invokeDelegate(showRoutes));
            else
                showRoutes();
        }

        delegate void pBarDelegate(int value);
        public void UpdateProgressBar(int value)
        {
            if (progressBar.InvokeRequired)
                progressBar.BeginInvoke(new pBarDelegate(updateProgressBar),value);
            else
                updateProgressBar(value);
        }

        void updateProgressBar(int value)
        {
            if (value > 100)
                value = 100;
            progressBar.Value = value;
        }

        void showRoutes()
        {
            EnableControls();
            RouteList.Rows.Clear();
            foreach (Route r in RoutesBuddy.Instance.Routes)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = r.Name;
                cell.Tag = r;
                row.Cells.Add(cell);
                RouteList.Rows.Add(row);
            }
        }

        private void ExportBut_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && RouteList.SelectedRows != null)
            {
                foreach (DataGridViewRow row in RouteList.SelectedRows)
                {
                    Route route = (Route)row.Cells[0].Tag;
                    string filename = Path.Combine(folderBrowserDialog.SelectedPath,
                        string.Format("{0}{1}.xml", (string)row.Cells[0].Value, (ProfileTypeCombo.SelectedIndex == 0) ? "[GB]" : "[HB]"));
                    using (FileStream fs = File.Open(filename, FileMode.Create))
                    {
                        if (ProfileTypeCombo.SelectedIndex == 0)
                            fs.Write(encoding.GetBytes(GBprefix), 0, GBprefix.Length);
                        else
                            fs.Write(encoding.GetBytes(HBprefix), 0, HBprefix.Length);
                        foreach (var point in route.Points)
                        {
                            int height = (int)HeightNumeric.Value;
                            var newPoint = point;
                            if (height >= 0)
                            {
                                newPoint.Z += height;
                            }
                            else
                            {
                                try
                                {
                                    newPoint.Z = Styx.Logic.Pathing.Navigator.FindHeights(newPoint.X, newPoint.Y).Min() + height;
                                }
                                catch { }
                            }
                            string buf;
                            if (ProfileTypeCombo.SelectedIndex == 0)
                                buf = string.Format(culture, "    <Waypoint>{0} {1} {2}</Waypoint>{3}",
                                    newPoint.X, newPoint.Y, newPoint.Z, Environment.NewLine);
                            else
                                buf = string.Format(culture, "    <Hotspot X=\"{0}\" Y=\"{1}\" Z=\"{2}\" />{3}",
                                    newPoint.X, newPoint.Y, newPoint.Z, Environment.NewLine);
                            fs.Write(encoding.GetBytes(buf), 0, buf.Length);
                        }
                        if (ProfileTypeCombo.SelectedIndex == 0)
                            fs.Write(encoding.GetBytes(GBpostfix), 0, GBpostfix.Length);
                        else
                            fs.Write(encoding.GetBytes(HBpostfix), 0, HBpostfix.Length);
                    }
                    Logging.Write("Exported {0}", filename);
                }
            }
        }

        private void DebugButton_Click(object sender, EventArgs e)
        {
            if (RoutesBuddy.Instance.RawImport != null && RoutesBuddy.Instance.RawImport.Count > 0)
            {
                using (new Styx.FrameLock())
                {
                    foreach (var list in RoutesBuddy.Instance.RawImport)
                    {
                        Logging.Write("*** {0} *****", list[1]);
                        string l = string.Format("return #(RoutesDB.global.routes[\"{0}\"][\"{1}\"].route) ",
                            list[0], list[1]);
                        int internalSize = Lua.GetReturnVal<int>(l, 0);
                        Logging.Write("Table.Count {0}, Exported Table.Count {1}",internalSize,list.Count-2);
                        for (int i = 2; i < list.Count; i++)
                        {
                            string lua = string.Format("return RoutesDB.global.routes[\"{0}\"][\"{1}\"].route[{2}] ",
                                list[0], list[1], i - 1);
                            string internalCoord = Lua.GetReturnVal<string>(lua,0);
                            Logging.Write("[{0}] = {1} == {2} ? {3}",i-1,internalCoord,list[i],internalCoord == list[i]);
                        }
                    }
                }
            }
        }

        string HBprefix =
@"<HBProfile>
  <Name></Name>
  <MinDurability>0.4</MinDurability>
  <MinFreeBagSlots>1</MinFreeBagSlots>
  
  <MinLevel>1</MinLevel>
  <MaxLevel>86</MaxLevel>
  <Factions>99999</Factions>
  
  <MailGrey>False</MailGrey>
  <MailWhite>True</MailWhite>
  <MailGreen>True</MailGreen>
  <MailBlue>True</MailBlue>
  <MailPurple>True</MailPurple>
 
  <SellGrey>True</SellGrey>
  <SellWhite>True</SellWhite>
  <SellGreen>False</SellGreen>
  <SellBlue>False</SellBlue>
  <SellPurple>False</SellPurple>

	<Vendors>

	</Vendors>
	
	<Mailboxes>

	</Mailboxes>

  <Hotspots>
";
        string HBpostfix =
@"  </Hotspots>
</HBProfile>
";
        string GBprefix =
@"
<?xml version=""1.0""?>
<GlideProfile>
  <MinLevel>1</MinLevel>
  <MaxLevel>80</MaxLevel>
  <NaturalRun>True</NaturalRun>
  <LureMinutes>0</LureMinutes>
  <SkipWaypoints>True</SkipWaypoints>
  <Factions></Factions>
";
        string GBpostfix =
        @"</GlideProfile>
";


    }
}

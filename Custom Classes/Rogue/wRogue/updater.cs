// This code was given to me by Raphus. If you want to use it
// you must obtain his permission. Not mine.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Threading;

using Styx.Combat.CombatRoutine;
using Styx.Helpers;

namespace wRogue
{
    public partial class Rogue : CombatRoutine
    {
        private void Update()
        {
            try
            {
                string wRoguePath = Logging.ApplicationPath + "\\CustomClasses\\wRogue";
                string wRogueUrl = "http://www.rootswitch.com/honorbuddy/wRogue";

                WebClient client = new WebClient();


                XDocument manifestLatest = XDocument.Load(wRogueUrl + "/manifest.xml");
                XDocument manifestCurrent = XDocument.Load(wRoguePath + "\\manifest.xml");
                DateTime latestTime = DateTime.Parse(manifestLatest.Element("Manifest").Element("UpdateTime").Value);
                DateTime currentTime = DateTime.Parse(manifestCurrent.Element("Manifest").Element("UpdateTime").Value);

                if (latestTime <= currentTime)
                    return;

                client.DownloadFile(wRogueUrl + "/manifest.xml", wRoguePath + "\\manifest.xml");

                Dictionary<string, string> fileList = new Dictionary<string, string>();
                foreach (XElement fileNode in manifestLatest.Element("Manifest").Descendants("File"))
                {
                    string path = fileNode.Element("Path").Value;
                    string hash = fileNode.Element("Hash").Value;
                    fileList.Add(path, hash);
                }
                int count = 0;
                foreach (KeyValuePair<string, string> file in fileList)
                {
                    string HashCode;
                    if (File.Exists(wRoguePath + @file.Key))
                    {
                        FileStream stmcheck = File.OpenRead(wRoguePath + @file.Key);
                        byte[] hash = new MD5CryptoServiceProvider().ComputeHash(stmcheck);
                        HashCode = BitConverter.ToString(hash).Replace("-", "");
                        stmcheck.Close();
                    }
                    else
                        HashCode = "";

                    if (HashCode != file.Value)
                    {
                        count++;
                        if (@file.Key.Contains('\\') && @file.Key.LastIndexOf('\\') != 0)
                        {
                            string directoryName = @file.Key.Remove(@file.Key.LastIndexOf('\\'));
                            Directory.CreateDirectory(wRoguePath + directoryName);
                        }

                        string url = wRogueUrl + @file.Key;
                        client.DownloadFile(url.Replace('\\', '/'), wRoguePath + @file.Key);
                        if (HashCode == "")
                            Logging.Write(Color.DarkRed, "Added {0}", file.Key);
                        else
                            Logging.Write(Color.DarkRed, "Updated {0}", file.Key);
                    }
                }
                if (count > 0)
                {
                    Logging.Write(Color.DarkRed, "Downloaded {0} new file{1}", count, count == 1 ? "" : "s");
                    Logging.Write(Color.DarkRed, "Close and restart HB to let changes take effect");
                }
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception e)
            {
                slog(e.ToString());
            }
        }
    }
}

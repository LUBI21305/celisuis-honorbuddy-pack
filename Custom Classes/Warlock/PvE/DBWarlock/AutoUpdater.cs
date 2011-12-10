using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;

namespace DBWarlock
{
    public partial class Warlock : CombatRoutine
    {

        public void CheckUpdate()
        {
            try
            {
                slog(Color.Green, "Checking for new update!", true, "");
                string CCPath = Logging.ApplicationPath + "\\CustomClasses\\DBWarlock3";
                string CCUrl = "http://darkben-hb-repository.googlecode.com/svn/trunk/CustomClasses/DBWarlock 3 Custom Class/trunk/DBWarlock/";
                WebClient client = new WebClient();


                slog(Color.Yellow, "Downloading manifest file from repository: {0}/manifest.xml", true, CCUrl);

                XDocument manifestLatest = XDocument.Load(CCUrl + "/manifest.xml");
                slog(Color.Yellow, "Checking manifest for updates to current installed files...", true, "");

                XDocument manifestCurrent = XDocument.Load(CCPath + "\\manifest.xml");


                client.DownloadFile(CCUrl + "/manifest.xml", CCPath + "\\manifest.xml");

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
                    if (File.Exists(CCPath + @file.Key))
                    {
                        FileStream stmcheck = File.OpenRead(CCPath + @file.Key);
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
                            Directory.CreateDirectory(CCPath + directoryName);
                        }

                        string url = CCUrl + @file.Key;
                        client.DownloadFile(url.Replace('\\', '/'), CCPath + @file.Key);
                        if (HashCode == "")
                        {
                            slog(Color.Red, "Added {0}", true, file.Key);
                        }
                        else
                        {
                            slog(Color.Blue, "Updated {0}", true, file.Key);
                        }
                    }
                }

                if (count > 0)
                {
                    slog(Color.DarkRed, "Downloaded {0} new file{1}", true, count, count == 1 ? "" : "s");
                    slog(Color.DarkRed, "Close and restart HB to let changes take effect", true, "");
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

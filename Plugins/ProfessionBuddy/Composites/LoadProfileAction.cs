using System;
using System.ComponentModel;
using TreeSharp;
using Styx.Helpers;
using Styx.Logic.Profiles;
using System.Xml;
using System.IO;
using System.Drawing.Design;
using Styx.Logic.Combat;

namespace HighVoltz.Composites
{
    #region LoadProfileAction
    public class LoadProfileAction : PBAction
    {
        public enum LoadProfileType
        {
            Honorbuddy,
            Professionbuddy,
        }
        public string Path
        {
            get { return (string)Properties["Path"].Value; }
            set { Properties["Path"].Value = value; }
        }
        public LoadProfileType ProfileType
        {
            get { return (LoadProfileType)Properties["ProfileType"].Value; }
            set { Properties["ProfileType"].Value = value; }
        }
        string folder;
        public LoadProfileAction()
        {
            Properties["Path"] = new MetaProp("Path", typeof(string), new EditorAttribute(typeof(PropertyBag.FileLocationEditor), typeof(UITypeEditor)));
            Properties["ProfileType"] = new MetaProp("ProfileType", typeof(LoadProfileType));

            Path = "";
            ProfileType = LoadProfileType.Honorbuddy;

            folder = Logging.ApplicationPath;
        }  
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                Load();
                IsDone = true;
            }
            return RunStatus.Failure;
        }

        public void Load()
        {
            string absPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Pb.CurrentProfile.XmlPath), Path);
            if (ProfileManager.XmlLocation != absPath)
            {
                try
                {
                    if (ProfileType == LoadProfileType.Honorbuddy)
                    {
                        Professionbuddy.Log("Loading Profile :{0}, previous profile was {1}", absPath,ProfileManager.XmlLocation);
                        if (string.IsNullOrEmpty(Path))
                        {
                            ProfileManager.LoadEmpty();
                        }
                        else if (System.IO.File.Exists(absPath))
                        {
                            ProfileManager.LoadNew(absPath);
                        }
                        else
                        {
                            Logging.Write(System.Drawing.Color.Red, "Unable to load profile {0}", Path);
                        }
                    }
                    else
                    {
                        Professionbuddy.LoadProfile(absPath);
                    }
                }
                catch (Exception ex) { Professionbuddy.Err(ex.ToString()); }
            }
        }

        public override string Name { get { return "Load profile"; } }
        public override string Title
        {
            get
            {
                return string.Format("{0}: {1} ProfileType:{2}", Name, Path, ProfileType);
            }
        }
        public override string Help
        {
            get
            {
                return "This action will load a profile, which can be either an Honorbuddy or Professionbuddy profile. Path needs to be relative to the currently loaded Professionbuddy profile and either in same folder or in a subfolder. If Path is empty and the profile type is Honorbuddy then an empty profile will be loaded.";
            }
        }
        public override object Clone()
        {
            return new LoadProfileAction() { Path = this.Path, ProfileType = this.ProfileType };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            Path = reader["Path"];
            ProfileType = (LoadProfileType)Enum.Parse(typeof(LoadProfileType), reader["ProfileType"]);
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Path", Path);
            writer.WriteAttributeString("ProfileType", ProfileType.ToString());
        }
        #endregion
    }
    #endregion
}

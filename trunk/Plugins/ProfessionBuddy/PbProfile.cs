//!CompilerOption:Optimize:On
//!CompilerOption:AddRef:WindowsBase.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Packaging;

using Styx;
using TreeSharp;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.Logic;
using Styx.Logic.Combat;
using System.Diagnostics;
using Styx.Patchables;
using Styx.Plugins;
using Styx.Plugins.PluginClass;
using Styx.Logic.Pathing;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals.WoWObjects;
using CommonBehaviors.Actions;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using Styx.Combat.CombatRoutine;
using Styx.Logic.POI;
using HighVoltz.Composites;

using Action = TreeSharp.Action;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace HighVoltz
{
    public class PbProfile
    {
        Professionbuddy _pb = Professionbuddy.Instance;
        public PbProfile()
        {
            ProfilePath = XmlPath = "";
            Branch = new PrioritySelector();
        }
        public PbProfile(string path)
        {
            ProfilePath = path;
            LoadFromFile(ProfilePath);
        }
        /// <summary>
        /// Path to a .xml or .package PB profile
        /// </summary>
        public string ProfilePath { get; protected set; }
        /// <summary>
        /// Path to a .xml PB profile
        /// </summary>
        public string XmlPath { get; protected set; }
        /// <summary>
        /// Profile behavior.
        /// </summary>
        public PrioritySelector Branch { get; protected set; }

        public PBIdentityComposite LoadFromFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    Professionbuddy.Log("Loading profile {0}", path);
                    ProfilePath = path;

                    if (Path.GetExtension(path).Equals(".package", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (Package zipFile = Package.Open(path, FileMode.Open, FileAccess.Read))
                        {
                            var packageRelation = zipFile.GetRelationships().FirstOrDefault();
                            if (packageRelation == null)
                            {
                                Professionbuddy.Err("{0} contains no usable profiles", path);
                                return null;
                            }
                            PackagePart pbProfilePart = zipFile.GetPart(packageRelation.TargetUri);
                            path = ExtractPart(pbProfilePart, _pb.TempFolder);
                            var pbProfileRelations = pbProfilePart.GetRelationships();
                            foreach (var rel in pbProfileRelations)
                            {
                                var hbProfilePart = zipFile.GetPart(rel.TargetUri);
                                ExtractPart(hbProfilePart, _pb.TempFolder);
                            }
                        }
                    }
                    Branch.Children.Clear();
                    PBIdentityComposite idComp;
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;
                    settings.IgnoreProcessingInstructions = true;
                    settings.IgnoreComments = false;

                    using (XmlReader reader = XmlReader.Create(path, settings))
                    {
                        idComp = new PBIdentityComposite(Branch);
                        idComp.ReadXml(reader);
                    }
                    XmlPath = path;
                    return idComp;
                }
                else
                {
                    Professionbuddy.Err("Profile: {0} does not exist", path);
                    return null;
                }
            }
            catch (Exception ex) { Professionbuddy.Err(ex.ToString()); return null; }
        }


        public void SaveXml(string file)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(file, settings))
            {
                ((PBIdentityComposite)_pb.Root).WriteXml(writer);
            }
            XmlPath = file;
        }

        private const string PackageRelationshipType = @"http://schemas.microsoft.com/opc/2006/sample/document";
        private const string ResourceRelationshipType = @"http://schemas.microsoft.com/opc/2006/sample/required-resource";
        public void CreatePackage(string path, string profilePath)
        {
            try
            {
                Uri partUriProfile = PackUriHelper.CreatePartUri(
                    new Uri(Path.GetFileName(profilePath), UriKind.Relative));
                Dictionary<string, Uri> HbProfileUrls = new Dictionary<string, Uri>();
                GetHbprofiles(profilePath, _pb.CurrentProfile.Branch, HbProfileUrls);
                using (Package package = Package.Open(path, FileMode.Create))
                {
                    // Add the PB profile
                    PackagePart packagePartDocument =
                        package.CreatePart(partUriProfile, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);
                    using (FileStream fileStream = new FileStream(
                           profilePath, FileMode.Open, FileAccess.Read))
                    {
                        CopyStream(fileStream, packagePartDocument.GetStream());
                    }
                    package.CreateRelationship(packagePartDocument.Uri, TargetMode.Internal, PackageRelationshipType);

                    foreach (var kv in HbProfileUrls)
                    {
                        PackagePart packagePartHbProfile =
                            package.CreatePart(kv.Value, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);

                        using (FileStream fileStream = new FileStream(kv.Key, FileMode.Open, FileAccess.Read))
                        {
                            CopyStream(fileStream, packagePartHbProfile.GetStream());
                        }
                        packagePartDocument.CreateRelationship(kv.Value, TargetMode.Internal, ResourceRelationshipType);
                    }
                }
            }
            catch (Exception ex)
            { Professionbuddy.Err(ex.ToString()); }
        }

        static internal void GetHbprofiles(string pbProfilePath, Composite comp, Dictionary<string, Uri> dict)
        {
            if (comp is LoadProfileAction && !string.IsNullOrEmpty(((LoadProfileAction)comp).Path) &&
                ((LoadProfileAction)comp).ProfileType == LoadProfileAction.LoadProfileType.Honorbuddy)
            {
                Uri profileUri = PackUriHelper.CreatePartUri(new Uri(((LoadProfileAction)comp).Path, UriKind.Relative));
                string pbProfileDirectory = Path.GetDirectoryName(pbProfilePath);
                string profilePath = Path.Combine(pbProfileDirectory, ((LoadProfileAction)comp).Path);
                if (!dict.ContainsKey(profilePath))
                    dict.Add(profilePath, profileUri);
            }
            if (comp is GroupComposite)
            {
                foreach (Composite c in ((GroupComposite)comp).Children)
                {
                    GetHbprofiles(pbProfilePath, c, dict);
                }
            }
        }

        void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }

        private string ExtractPart(PackagePart packagePart, string targetDirectory)
        {
            string packageRelative = Uri.UnescapeDataString(packagePart.Uri.ToString().TrimStart('/'));
            string fullPath = Path.Combine(targetDirectory, packageRelative);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                CopyStream(packagePart.GetStream(), fileStream);
            }
            return fullPath;
        }
    }
}

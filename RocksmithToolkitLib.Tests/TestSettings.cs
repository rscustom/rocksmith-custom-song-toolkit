using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.DLCPackage;
using System.IO;
using RocksmithToolkitLib.Extensions;
using System.Diagnostics;

namespace RocksmithToolkitLib.Tests
{
    /// <summary>
    /// Initializes an instance of TestSettings global variables for Unit Testing
    /// </summary>
    public class TestSettings : NotifyPropChangedBase
    {
        public string ResourcesDir { get; set; }
        public List<string> SrcPaths { get; set; }
        public string DestDir { get; set; }
        public List<string> UnpackedDirs { get; set; }

        public void Load()
        {
            ResourcesDir = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Resources");
            string[] fileExt = new string[] { "_p.psarc", "_m.psarc", "_ps3.psarc.edat", "_xbox", "_p.psarc", "_m.psarc", "_ps3.psarc.edat", "_xbox" };
            SrcPaths = Directory.EnumerateFiles(ResourcesDir, "*", SearchOption.TopDirectoryOnly).Where(fi => fileExt.Any(fi.ToLower().EndsWith)).ToList();
            if (!SrcPaths.Any())
                throw new Exception("The 'Resources' directory contains no valid archive files ...");

            DestDir = Path.Combine(Path.GetTempPath(), "UnitTest");
        }

        public void Unpack()
        {
            var unpackedDirs = new List<string>();
            
            foreach (var srcPath in SrcPaths)
            {
                Platform platform = srcPath.GetPlatform();
                if (platform == null || platform.platform == GamePlatform.None)
                    throw new Exception("GetPlatform Failed: " + Path.GetFileName(srcPath));

                // unpack artifacts
                var unpackedDir = Packer.Unpack(srcPath, DestDir, true, true, platform);
                unpackedDirs.Add(unpackedDir);
            }

            UnpackedDirs = unpackedDirs;


            if (SrcPaths.Count != UnpackedDirs.Count)
                throw new Exception("SrcPaths.Count does not equal UnpackedDirs.Count");
        }

        private static TestSettings _instance;
        public static TestSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TestSettings();
                return _instance;
            }
        }

    }
}

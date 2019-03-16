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
    public class TestSettings
    {
        public string ResourcesDir { get; set; }
        public List<string> ResourcePaths { get; set; }
        public List<string> ArchivePaths { get; set; }
        public string TmpDestDir { get; set; }
        public List<string> UnpackedDirs { get; set; }

        public void Load()
        {
            ResourcesDir = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Resources");
            string[] fileExt = new string[] { "_p.psarc", "_m.psarc", "_ps3.psarc.edat", "_xbox", ".dat" };
            ResourcePaths = Directory.EnumerateFiles(ResourcesDir, "*", SearchOption.TopDirectoryOnly).Where(fi => fileExt.Any(fi.ToLower().EndsWith)).ToList();
            if (!ResourcePaths.Any())
                throw new Exception("The 'Resources' directory contains no valid archive files ...");

            TmpDestDir = Path.Combine(Path.GetTempPath(), "UnitTest");

            // start with clean 'Local Settings/Temp/UnitTest' directory
            IOExtension.DeleteDirectory(TmpDestDir);
            Directory.CreateDirectory(TmpDestDir);
        }

        public void CopyResources()
        {
            var archivePaths = new List<string>();

            // copy original Resources (song archive files) to DestDir and populate ArchivePaths
            foreach (var resourcePath in ResourcePaths)
            {
                var archivePath = Path.Combine(TmpDestDir, Path.GetFileName(resourcePath));
                if (File.Exists(archivePath))
                    File.Delete(archivePath);

                File.Copy(resourcePath, archivePath);
                archivePaths.Add(archivePath);
            }

            ArchivePaths = archivePaths;
            if (ArchivePaths.Count != ResourcePaths.Count)
                throw new Exception("<ERROR> ArchivePaths.Count does not equal ResourcesPaths.Count ...");
        }

        public void UnpackResources()
        {
            var unpackedDirs = new List<string>();

            // unpack original Resources
            foreach (var srcPath in ResourcePaths)
            {
                Platform srcPlatform = srcPath.GetPlatform();
                if (srcPlatform == null || srcPlatform.platform == GamePlatform.None)
                    throw new Exception("GetPlatform Failed: " + Path.GetFileName(srcPath));

                // unpack artifacts
                var unpackedDir = Packer.Unpack(srcPath, TmpDestDir, srcPlatform, true, true);
                unpackedDirs.Add(unpackedDir);
            }

            UnpackedDirs = unpackedDirs;
            if (UnpackedDirs.Count != ResourcePaths.Count)
                throw new Exception("UnpackedDirs.Count does not equal ResourcePaths.Count ...");
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

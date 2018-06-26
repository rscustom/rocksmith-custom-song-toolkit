using System.Collections.Generic;
using System.ComponentModel;
using RocksmithToolkitGUI.DLCPackerUnpacker;
using System;
using NUnit.Framework;
using System.IO;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib.PsarcLoader;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Tests;
using System.Linq;
using System.Diagnostics;

namespace RocksmithToolkitGUI.Tests
{
    /// <summary>
    /// Unit test class for DLCPackerUnpacker
    ///</summary>
    [TestFixture]
    public class DLCPackerUnpackerTest
    {
        private DLCPackerUnpacker.DLCPackerUnpacker packerUnpacker;
        private List<string> unpackedDirs;
        private List<string> destPaths;

        [TestFixtureSetUp]
        public void Init()
        {
            TestSettings.Instance.Load();
            if (!TestSettings.Instance.SrcPaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            Globals.IsUnitTest = true;
            packerUnpacker = new DLCPackerUnpacker.DLCPackerUnpacker();
            // stress test user selectable options
            packerUnpacker.OverwriteSongXml = true;
            packerUnpacker.DecodeAudio = true;
            packerUnpacker.UpdateManifest = true;
            packerUnpacker.UpdateSng = true;

            // empty the 'Local Settings/Temp' directory before starting
            DirectoryExtension.SafeDelete(Path.GetTempPath());
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            packerUnpacker.Dispose();
            Globals.IsUnitTest = false;
        }

        /// <summary>
        /// Quick unit test of all DLCPackerUnpacker methods 
        /// Preserves correct testing order for efficiency/speed
        /// </summary>
        [Test]
        [Ignore("Intentionally Ignored TestAll ... OK")]
        public void TestAll()
        {
            // order is important to reduce redundant unit testing
            TestUnpackSongs();
            TestPackSong();
            // valid ODLC Bob Marley, Three Little Birds - no messagebox 
            TestRepackAppId("492988");
        }

        /// <summary>
        /// Unit test for UnpackSongs()
        ///</summary>
        [Test]
        public void TestUnpackSongs()
        {
            unpackedDirs = new List<string>();
            unpackedDirs = packerUnpacker.UnpackSongs(TestSettings.Instance.SrcPaths, TestSettings.Instance.DestDir);
            Assert.AreEqual(TestSettings.Instance.SrcPaths.Count, unpackedDirs.Count);
        }

        /// <summary>
        /// Unit test for PackSong()
        ///</summary>
        [Test]
        public void TestPackSong()
        {
            // confirm UnpackSongs has been tested/loaded
            if (unpackedDirs == null)
                TestUnpackSongs();

            destPaths = new List<string>();
            foreach (var unpackedDir in unpackedDirs)
            {
                var destPath = Path.Combine(TestSettings.Instance.DestDir, packerUnpacker.RecycleFolderName(unpackedDir));
                if (String.IsNullOrEmpty(destPath))
                    Assert.Fail("RecycleFolderName Method Failed ...");

                packerUnpacker.PackSong(unpackedDir, destPath);

                if (!File.Exists(destPath))
                    Assert.Fail("PackSong Method Failed: " + Path.GetFileName(unpackedDir));

                destPaths.Add(destPath);
            }

            Assert.AreEqual(destPaths.Count, unpackedDirs.Count);
        }

        /// <summary>
        /// Unit test for RepackAppId
        /// TestCases commented out to reduce messagebox verbosity
        ///</summary>        
        // [TestCase("69")] // not valid 6 digits - show messagebox
        // [TestCase("248769")] // not valid ODLC - show messagebox
        [TestCase("492988")] // valid ODLC Bob Marley, Three Little Birds - no messagebox 


        public void TestRepackAppId(string appId)
        {
            // confirm PackSong has been tested and saved
            if (destPaths == null)
                TestPackSong();

            packerUnpacker.AppId = appId;

            foreach (var destPath in destPaths)
            {
                // test AppId validation method
                packerUnpacker.SelectComboAppId(packerUnpacker.AppId);

                // console package does not have an AppId
                var platform = destPath.GetPlatform();
                if (platform.IsConsole)
                {
                    // NOTE: when unit test is finished, double click the test result to see this message
                    Debug.WriteLine("---------------------------------");
                    Debug.WriteLine("TestRepackAppId skipped PS3 file: " + Path.GetFileName(destPath));
                    Debug.WriteLine("---------------------------------");
                    continue;
                }

                // call background worker method from unit test to avoid threading issues
                packerUnpacker.UpdateAppId(null, new DoWorkEventArgs(new string[] { destPath }));

                if (!File.Exists(destPath))
                    Assert.Fail("RepackAppId Method Failed: " + Path.GetFileName(destPath));

                // check if RepackAppId wrote the new AppId
                var psarcLoader = new PsarcLoader(destPath, true);
                var entryAppId = psarcLoader.ExtractAppId();

                Assert.AreEqual(packerUnpacker.AppId, entryAppId);
            }
        }


    }
}

/*
            var xboxHeaderPaths = Directory.EnumerateFiles(srcPath, "*.txt", SearchOption.TopDirectoryOnly).ToList();
            if (!xboxHeaderPaths.Any())
                throw new FileLoadException("<ERROR> UnpackXbox360Package Failed.  Could not find psarc archive. ");
            if (xboxHeaderPaths.Count > 1)
                throw new FileLoadException("<ERROR> UnpackXbox360Package Failed.  Found more than one psarc archive. ");
*/
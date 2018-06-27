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
        private List<string> archivePaths;

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

            archivePaths = new List<string>();
            foreach (var unpackedDir in unpackedDirs)
            {
                var expArchivePath = Path.Combine(TestSettings.Instance.DestDir, Packer.RecycleArtifatsFolder(unpackedDir));
                if (String.IsNullOrEmpty(expArchivePath))
                    Assert.Fail("RecycleArtifactsFolder Method Failed ...");

                var actArchivePath = packerUnpacker.PackSong(unpackedDir, expArchivePath);

                if (!File.Exists(actArchivePath))
                    Assert.Fail("Pack Method Failed: " + Path.GetFileName(unpackedDir));

                Assert.AreEqual(expArchivePath, actArchivePath);
                archivePaths.Add(actArchivePath);
            }

            Assert.AreEqual(archivePaths.Count, unpackedDirs.Count);
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
            if (archivePaths == null)
                TestPackSong();

            packerUnpacker.AppId = appId;

            foreach (var archivePath in archivePaths)
            {
                // test AppId validation method
                packerUnpacker.SelectComboAppId(packerUnpacker.AppId);

                // console package does not have an AppId
                var platform = archivePath.GetPlatform();
                if (platform.IsConsole)
                {
                    // NOTE: when unit test is finished, double click the test result to see this message
                    Debug.WriteLine("---------------------------------");
                    Debug.WriteLine("TestRepackAppId skipped PS3 file: " + Path.GetFileName(archivePath));
                    Debug.WriteLine("---------------------------------");
                    continue;
                }

                // call background worker method from unit test to avoid threading issues
                packerUnpacker.UpdateAppId(null, new DoWorkEventArgs(new string[] { archivePath }));

                if (!File.Exists(archivePath))
                    Assert.Fail("RepackAppId Method Failed: " + Path.GetFileName(archivePath));

                // check if RepackAppId wrote the new AppId
                var psarcLoader = new PsarcLoader(archivePath, true);
                var entryAppId = psarcLoader.ExtractAppId();

                Assert.AreEqual(packerUnpacker.AppId, entryAppId);
            }
        }


    }
}


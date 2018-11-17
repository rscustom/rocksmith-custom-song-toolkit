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
            if (!TestSettings.Instance.ResourcePaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            ConfigGlobals.IsUnitTest = true;
            packerUnpacker = new DLCPackerUnpacker.DLCPackerUnpacker();
            // stress test user selectable options
            packerUnpacker.OverwriteSongXml = true;
            packerUnpacker.DecodeAudio = true;
            packerUnpacker.UpdateManifest = true;
            packerUnpacker.UpdateSng = true;
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            packerUnpacker.Dispose();
            ConfigGlobals.IsUnitTest = false;
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
            TestRepackAppId("492988", GameVersion.RS2014);
            TestRepackAppId("206102", GameVersion.RS2012);
        }

        /// <summary>
        /// Unit test for UnpackSongs()
        ///</summary>
        [Test]
        public void TestUnpackSongs()
        {
            // work with original Resources
            unpackedDirs = new List<string>();
            unpackedDirs = packerUnpacker.UnpackSongs(TestSettings.Instance.ResourcePaths, TestSettings.Instance.TmpDestDir);
            Assert.AreEqual(TestSettings.Instance.ResourcePaths.Count, unpackedDirs.Count);
        }

        /// <summary>
        /// Unit test for PackSong()
        ///</summary>
        [Test]
        public void TestPackSong()
        {
            if (unpackedDirs == null)
            {
                TestSettings.Instance.UnpackResources();
                if (!TestSettings.Instance.UnpackedDirs.Any())
                    Assert.Fail("TestSettings UnpackResources Failed ...");

                unpackedDirs = TestSettings.Instance.UnpackedDirs;
            }

            archivePaths = new List<string>();
            foreach (var unpackedDir in unpackedDirs)
            {
                Platform platform = unpackedDir.GetPlatform();
                if (platform == null || platform.platform == GamePlatform.None || platform.version == GameVersion.None)
                    Assert.Fail("GetPlatform Failed: " + Path.GetFileName(unpackedDir));

                var expArchiveName = Packer.RecycleUnpackedDir(unpackedDir);
                if (String.IsNullOrEmpty(expArchiveName))
                    Assert.Fail("RecycleArtifactsFolder Method Failed ...");

                var actArchivePath = packerUnpacker.PackSong(unpackedDir, expArchiveName);

                if (!File.Exists(actArchivePath))
                    Assert.Fail("Pack Method Failed: " + Path.GetFileName(unpackedDir));

                Assert.AreEqual(expArchiveName, actArchivePath);

                archivePaths.Add(actArchivePath);
            }

            Assert.AreEqual(archivePaths.Count, unpackedDirs.Count);
        }

        /// <summary>
        /// Unit test for RepackAppId
        /// TestCases commented out to reduce messagebox verbosity
        ///</summary>        
        // [TestCase("69", GameVersion.RS2014)] // not valid 6 digits - show messagebox
        // [TestCase("248769", GameVersion.RS2014)] // not valid ODLC - show messagebox
        [TestCase("492988", GameVersion.RS2014)] // free ODLC Bob Marley, Three Little Birds - no messagebox 
        [TestCase("206102", GameVersion.RS2012)] // free ODLC Holiday Song Pack - no messagebox
        public void TestRepackAppId(string appId, GameVersion gameVersion)
        {
            // start fresh
            TestSettings.Instance.CopyResources();
            if (!TestSettings.Instance.ArchivePaths.Any())
                Assert.Fail("TestSettings CopyResources Failed ...");

            packerUnpacker.Version = gameVersion;
            packerUnpacker.NewAppId = appId;

            foreach (var archivePath in TestSettings.Instance.ArchivePaths)
            {
                var platform = archivePath.GetPlatform();
                if (gameVersion != platform.version)
                    continue;

                // console package does not have an AppId
                if (platform.IsConsole)
                {
                    // NOTE: when unit test is finished, double click the test result to see this message
                    Debug.WriteLine("---------------------------------");
                    Debug.WriteLine("TestRepackAppId skipped console files: " + Path.GetFileName(archivePath));
                    Debug.WriteLine("---------------------------------");
                    continue;
                }

                // test AppId validation method
                packerUnpacker.SelectComboAppId(packerUnpacker.NewAppId);

                // call background worker method from unit test to avoid threading issues
                packerUnpacker.UpdateAppId(null, new DoWorkEventArgs(new string[] { archivePath }));

                if (!File.Exists(archivePath))
                    Assert.Fail("RepackAppId Method Failed: " + Path.GetFileName(archivePath));

                if (gameVersion == GameVersion.RS2012)
                    continue;

                // check if RepackAppId wrote the new AppId (RS2014 ONLY)
                using (var psarcLoader = new PsarcLoader(archivePath, true))
                {
                    var entryAppId = psarcLoader.ExtractAppId();
                    Assert.AreEqual(packerUnpacker.NewAppId, entryAppId);
                }
            }
        }


    }
}


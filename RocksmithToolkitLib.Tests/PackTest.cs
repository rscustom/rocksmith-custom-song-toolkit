using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Extensions;
//
// TODO: create addition unit tests in this class
//
namespace RocksmithToolkitLib.Tests
{
    [TestFixture]
    public class PackTest
    {
        private List<string> unpackedDirs;

        [TestFixtureSetUp]
        public void Init()
        {
            TestSettings.Instance.Load();
            if (!TestSettings.Instance.SrcPaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            TestSettings.Instance.Unpack();
            if (!TestSettings.Instance.UnpackedDirs.Any())
                Assert.Fail("TestSettings Unpack Failed ...");
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            // any cleanup goes here
        }

        /// <summary>
        /// Unit test for Pack() method
        ///</summary>
        [Test]
        public void TestPack()
        {
            var archivePaths = new List<string>();

            foreach (var unpackedDir in TestSettings.Instance.UnpackedDirs)
            {
                Platform platform = unpackedDir.GetPlatform();
                if (platform == null)
                    Assert.Fail("GetPlatform Failed: " + Path.GetFileName(unpackedDir));

                var expArchivePath = Path.Combine(TestSettings.Instance.DestDir, Packer.RecycleUnpackedDir(unpackedDir));
                if (String.IsNullOrEmpty(expArchivePath))
                    Assert.Fail("RecycleArtifactsFolder Method Failed ...");

                var actArchivePath = Packer.Pack(unpackedDir, expArchivePath, platform, true, true);

                if (!File.Exists(actArchivePath))
                    Assert.Fail("Pack Method Failed: " + Path.GetFileName(unpackedDir));

                Assert.AreEqual(expArchivePath, actArchivePath);
                archivePaths.Add(actArchivePath);
            }

            Assert.AreEqual(TestSettings.Instance.SrcPaths.Count, archivePaths.Count);
        }


    }
}

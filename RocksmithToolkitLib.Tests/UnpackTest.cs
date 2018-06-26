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
    public class UnpackTest
    {
        private List<string> unpackedDirs;

        [TestFixtureSetUp]
        public void Init()
        {
            TestSettings.Instance.Load();
            if (!TestSettings.Instance.SrcPaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            unpackedDirs = new List<string>();

            // empty the 'Local Settings/Temp' directory before starting
            DirectoryExtension.SafeDelete(Path.GetTempPath());
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            // any cleanup goes here
        }

        /// <summary>
        /// Unit test for Unpack() method
        ///</summary>
        [Test]
        public void TestUnpack()
        {
            foreach (var srcPath in TestSettings.Instance.SrcPaths)
            {
                Platform platform = srcPath.GetPlatform();
                if (platform == null)
                    Assert.Fail("GetPlatform Failed: " + Path.GetFileName(srcPath));

                var unpackedDir = Packer.Unpack(srcPath, TestSettings.Instance.DestDir, true, true, platform);
                unpackedDirs.Add(unpackedDir);
            }

            Assert.AreEqual(TestSettings.Instance.SrcPaths.Count, unpackedDirs.Count);
        }


    }
}

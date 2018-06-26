using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib.Sng2014HSL;
using System.Linq;
//
// TODO: create addition unit tests in this class
//
namespace RocksmithToolkitLib.Tests
{
    [TestFixture]
    public class Sng2014FileTest
    {
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
        /// Unit Test for LoadFromFile() method
        /// </summary>
        [Test]
        public void TestLoadFromFile()
        {
            foreach (var unpackedDir in TestSettings.Instance.UnpackedDirs)
            {
                var platform = unpackedDir.GetPlatform();
                var sngPaths = Directory.EnumerateFiles(unpackedDir, "*.sng", SearchOption.AllDirectories).ToList();
                foreach (var sngPath in sngPaths)
                {
                    var song = Sng2014File.LoadFromFile(sngPath, platform);
                    if (song == null)
                        Assert.Fail("LoadFromFile Failed: " + Path.GetFileName(unpackedDir));

                    if (!song.Arrangements.Arrangements.Any())
                        Assert.Fail("LoadFromFile Arrangements Failed: " + Path.GetFileName(unpackedDir));
                }
            }
        }


    }
}

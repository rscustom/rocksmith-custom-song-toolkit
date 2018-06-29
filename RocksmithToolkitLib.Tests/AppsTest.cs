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
    public class AppsTest
    {
        private List<string> unpackedDirs;

        [TestFixtureSetUp]
        public void Init()
        {
            TestSettings.Instance.Load();
            if (!TestSettings.Instance.SrcPaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            // empty the 'Local Settings/Temp' directory before starting
            DirectoryExtension.SafeDelete(Path.GetTempPath());
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            // any cleanup goes here
        }

        /// <summary>
        /// Unit test for VerifyExternalApps() method
        ///</summary>
        [Test]
        public void TestVerifyExternalApps()
        {
            var results = ExternalApps.VerifyExternalApps();
            Assert.IsTrue(results);
        }


    }
}

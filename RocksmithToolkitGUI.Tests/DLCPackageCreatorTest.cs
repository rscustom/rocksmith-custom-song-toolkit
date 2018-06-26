using System.ComponentModel;
using RocksmithToolkitGUI.DLCPackageCreator;
using RocksmithToolkitGUI.Config;
using System;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;
using System.IO;
using RocksmithToolkitLib;
using NUnit.Framework;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using RocksmithToolkitLib.PsarcLoader;
using RocksmithToolkitLib.Tests;
using System.Collections.Generic;
using System.Linq;

namespace RocksmithToolkitGUI.Tests
{
    /// <summary>
    /// Unit test class for DLCPackageCreator
    ///</summary>
    [TestFixture]
    public class DLCPackageCreatorTest
    {
        private DLCPackageCreator.DLCPackageCreator packageCreator;
        private List<string> unpackedDirs;
        private List<string> templatePaths;

        /// <summary>
        /// Quick unit test of all DLCPackageCreator methods 
        /// Preserves correct testing order for efficiency/speed
        /// </summary>
        [Test]
        [Ignore("Intentionally Ignored TestAll ... OK")]
        public void TestAll()
        {
            // order is important to reduce redundant unit testing
            TestPackageImport();
            TestPackageGenerate();
            TestSaveTemplateFile();
            TestLoadTemplateFile();
        }

        [TestFixtureSetUp]
        public void Init()
        {
            TestSettings.Instance.Load();
            if (!TestSettings.Instance.SrcPaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            Globals.IsUnitTest = true;
            packageCreator = new DLCPackageCreator.DLCPackageCreator();

            // empty the 'Local Settings/Temp' directory before starting
            DirectoryExtension.SafeDelete(Path.GetTempPath());
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            packageCreator.Dispose();
            Globals.IsUnitTest = false;
        }

        /// <summary>
        /// Unit test for PackageImport()
        /// </summary>
        [Test]
        public void TestPackageImport()
        {
            unpackedDirs = new List<string>();
            foreach (var srcPath in TestSettings.Instance.SrcPaths)
            {
                DLCPackageData info = new DLCPackageData();
                info = packageCreator.PackageImport(srcPath, TestSettings.Instance.DestDir);

                // validate critical packageCreator/info data
                Assert.IsNotNullOrEmpty(packageCreator.DLCKey);
                Assert.IsNotNull(info);
                // consoles do not have AppId, test AppId all other platforms
                if (!info.PS3 && !info.XBox360)
                    Assert.IsNotNullOrEmpty(info.AppId);
                // test that UnpackedDir was created
                if (String.IsNullOrEmpty(packageCreator.UnpackedDir) || !Directory.Exists(packageCreator.UnpackedDir))
                    Assert.Fail("PackageImport failed to create unpackDir for: " + Path.GetFileName(srcPath));

                unpackedDirs.Add(packageCreator.UnpackedDir);
            }

            // confirm PackageImport creeated unpackedDirs 
            if (unpackedDirs.Count == 0)
                Assert.Fail("PackageImport did not create any unpackDirs ...");
        }

        /// <summary>
        /// Unit test for PackageGenerate() 
        /// All platform generations are tested (PC, Mac, Xbox, and PS3)
        /// </summary>
        [Test]
        public void TestPackageGenerate()
        {
            foreach (var srcPath in TestSettings.Instance.SrcPaths)
            {
                // load info from srcPath
                DLCPackageData info = new DLCPackageData();
                info = packageCreator.PackageImport(srcPath, TestSettings.Instance.DestDir);

                // set package version, old srcPackage may not have it
                packageCreator.PackageVersion = "8"; // make easy to identify
                // consoles do not have AppId, test AppId all other platforms
                if (!info.PS3 && !info.XBox360)
                    packageCreator.AppId = "492988"; // use a free RS2014 RM song for test
                // stress test package generation for all platforms
                packageCreator.PlatformPC = true;
                packageCreator.PlatformMAC = true;
                packageCreator.PlatformXBox360 = true;
                packageCreator.PlatformPS3 = true;

                // set destPath and test getting the package data
                var destPath = Path.Combine(TestSettings.Instance.DestDir, Path.GetFileName(srcPath));
                packageCreator.DestPath = destPath;
                var packageData = packageCreator.PackageGenerate();
                Assert.NotNull(packageData);

                // call background worker method from unit test to avoid threading issues
                packageCreator.GeneratePackage(null, new DoWorkEventArgs(packageData));
                var shortPath = destPath.Replace("_p.psarc", "").Replace("_m.psarc", "").Replace("_ps3.psarc.edat", "").Replace("_xbox", "");

                if (!File.Exists(shortPath + "_p.psarc"))
                    Assert.Fail("PC Package Generation Failed ...");

                if (!File.Exists(shortPath + "_m.psarc"))
                    Assert.Fail("Mac Package Generation Failed ...");

                if (!File.Exists(shortPath + "_xbox"))
                    Assert.Fail("Xbox Package Generation Failed ...");

                if (!File.Exists(shortPath + "_ps3.psarc.edat"))
                    Assert.Fail("PS3 Package Generation Failed ...");

                // console files do not contain an AppId
                if (!info.PS3 && !info.XBox360)
                {
                    // check if GeneratePackage wrote the new AppId
                    var psarcLoader = new PsarcLoader(destPath, true);
                    var entryAppId = psarcLoader.ExtractAppId();

                    Assert.AreEqual(packageCreator.AppId, entryAppId);
                }
            }
        }

        /// <summary>
        /// Unit test for SaveTemplateFile() 
        /// </summary>
        [Test]
        public void TestSaveTemplateFile()
        {
            templatePaths = new List<string>();

            foreach (var srcPath in TestSettings.Instance.SrcPaths)
            {
                DLCPackageData info = packageCreator.PackageImport(srcPath, TestSettings.Instance.DestDir);
                // set package version because srcPackage may not have it
                packageCreator.PackageVersion = "8"; // make easy to identify
                var templatePath = packageCreator.SaveTemplateFile(packageCreator.UnpackedDir, true);

                if (String.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    Assert.Fail("Save Template File Failed ...");

                templatePaths.Add(templatePath);
            }

            // confirm SaveTemplateFile created templatePaths 
            if (templatePaths.Count == 0)
                Assert.Fail("SaveTemplateFile did not create any templatePaths ...");
        }

        /// <summary>
        /// Unit test for LoadTemplateFile() 
        /// </summary>
        [Test]
        public void TestLoadTemplateFile()
        {
            // confirm SaveTemplateFile has been tested/created
            if (templatePaths == null || templatePaths.Count == 0)
                TestSaveTemplateFile();

            foreach (var templatePath in templatePaths)
            {
                packageCreator.DLCKey = String.Empty;
                packageCreator.LoadTemplateFile(templatePath);

                if (String.IsNullOrEmpty(packageCreator.DLCKey))
                    Assert.Fail("Load Template File Failed: " + Path.GetFileName(templatePath));
            }
        }


    }
}

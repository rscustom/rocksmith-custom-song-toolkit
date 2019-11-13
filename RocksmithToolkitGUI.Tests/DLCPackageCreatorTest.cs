using System.ComponentModel;
using RocksmithToolkitGUI.DLCPackageCreator;
using RocksmithToolkitGUI.Config;
using System;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.PSARC;
using RocksmithToolkitLib.XmlRepository;
using System.IO;
using RocksmithToolkitLib;
using NUnit.Framework;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
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
            if (!TestSettings.Instance.ResourcePaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            GlobalsLib.IsUnitTest = true;
            packageCreator = new DLCPackageCreator.DLCPackageCreator();

            // empty the 'Local Settings/Temp/UnitTest' directory before starting
            IOExtension.DeleteDirectory(TestSettings.Instance.TmpDestDir);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            packageCreator.Dispose();
            GlobalsLib.IsUnitTest = false;
        }

        /// <summary>
        /// Unit test for PackageImport()
        /// </summary>
        [Test]
        public void TestPackageImport()
        {
            unpackedDirs = new List<string>();
            foreach (var srcPath in TestSettings.Instance.ResourcePaths)
            {
                DLCPackageData info = new DLCPackageData();
                var platform = srcPath.GetPlatform();
                if (platform.version == GameVersion.RS2012)
                    packageCreator.CurrentGameVersion = GameVersion.RS2012;
                else
                    packageCreator.CurrentGameVersion = GameVersion.RS2014;

                info = packageCreator.PackageImport(srcPath, TestSettings.Instance.TmpDestDir);

                // validate critical packageCreator/info data
                Assert.IsNotNull(info);
                Assert.IsNotNullOrEmpty(packageCreator.DLCKey);
                // consoles do not have AppId, test AppId all other platforms
                if (!info.PS3 && !info.XBox360)
                    Assert.IsNotNullOrEmpty(info.AppId);
                // test that UnpackedDir was created
                if (String.IsNullOrEmpty(packageCreator.UnpackedDir) || !Directory.Exists(packageCreator.UnpackedDir))
                    Assert.Fail("PackageImport failed to create unpackDir for: " + Path.GetFileName(srcPath));

                unpackedDirs.Add(packageCreator.UnpackedDir);
            }

            Assert.AreEqual(TestSettings.Instance.ResourcePaths.Count, unpackedDirs.Count);
        }

        /// <summary>
        /// Unit test for PackageGenerate() 
        /// All platform generations are tested (PC, Mac, Xbox, and PS3)
        /// </summary>
        [Test]
        public void TestPackageGenerate()
        {
            var archivePaths = new List<string>();

            foreach (var srcPath in TestSettings.Instance.ResourcePaths)
            {
                Debug.WriteLine("Processing: " + srcPath);
                // load info from srcPath
                DLCPackageData info = new DLCPackageData();

                var platform = srcPath.GetPlatform();
                if (platform.version == GameVersion.RS2012)
                    packageCreator.CurrentGameVersion = GameVersion.RS2012;
                else
                    packageCreator.CurrentGameVersion = GameVersion.RS2014;

                info = packageCreator.PackageImport(srcPath, TestSettings.Instance.TmpDestDir);

                // set package version, old srcPackage may not have it
                packageCreator.PackageVersion = "8"; // make easy to identify
                packageCreator.AppId = "492988"; // free Bob Marley - Three Little Birds
                // stress test package generation for all platforms
                packageCreator.PlatformPC = true;
                packageCreator.PlatformXBox360 = true;
                packageCreator.PlatformPS3 = true;
                packageCreator.PlatformMAC = true;
                // skip testing ddc generation
                // ConfigRepository.Instance()["ddc_autogen"] = "false";

                if (packageCreator.CurrentGameVersion == GameVersion.RS2012)
                {
                    packageCreator.AppId = "206102"; // free Holiday Song Pack
                    packageCreator.PlatformPS3 = false;
                    packageCreator.PlatformMAC = false;
                }

                // set destPath and test getting the package data
                var archivePath = Path.Combine(TestSettings.Instance.TmpDestDir, Path.GetFileName(srcPath));
                packageCreator.DestPath = archivePath;
                var packageData = packageCreator.PackageGenerate();
                Assert.NotNull(packageData);

                // call background worker method from unit test to avoid threading issues
                packageCreator.GeneratePackage(null, new DoWorkEventArgs(packageData));
                var shortPath = archivePath.Replace("_p.psarc", "").Replace("_m.psarc", "").Replace("_ps3.psarc.edat", "").Replace("_xbox", "").Replace(".dat", "");

                if (packageCreator.CurrentGameVersion == GameVersion.RS2012)
                {
                    if (!File.Exists(shortPath + ".dat"))
                        Assert.Fail("RS1 PC Package Generation Failed ...");

                    if (!File.Exists(shortPath + "_xbox"))
                        Assert.Fail("RS1 Xbox Package Generation Failed ...");
                }
                else
                {
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
                        using (var psarcLoader = new PsarcLoader(archivePath, true))
                        {
                            var entryAppId = psarcLoader.ExtractAppId();
                            Assert.AreEqual(packageCreator.AppId, entryAppId);
                        }
                    }
                }

                archivePaths.Add(archivePath);
            }

            Assert.AreEqual(TestSettings.Instance.ResourcePaths.Count, archivePaths.Count);
        }

        /// <summary>
        /// Unit test for SaveTemplateFile() 
        /// </summary>
        [Test]
        public void TestSaveTemplateFile()
        {
            templatePaths = new List<string>();

            foreach (var srcPath in TestSettings.Instance.ResourcePaths)
            {
                var platform = srcPath.GetPlatform();
                if (platform.version == GameVersion.RS2012)
                    packageCreator.CurrentGameVersion = GameVersion.RS2012;
                else
                    packageCreator.CurrentGameVersion = GameVersion.RS2014;

                DLCPackageData info = packageCreator.PackageImport(srcPath, TestSettings.Instance.TmpDestDir);
                // set package version because srcPackage may not have it
                packageCreator.PackageVersion = "8"; // make easy to identify
                var templatePath = packageCreator.SaveTemplateFile(packageCreator.UnpackedDir, true);

                if (String.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    Assert.Fail("Save Template File Failed ...");

                templatePaths.Add(templatePath);
            }

            Assert.AreEqual(TestSettings.Instance.ResourcePaths.Count, templatePaths.Count);
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

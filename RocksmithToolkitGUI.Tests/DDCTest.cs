using System.Collections.Generic;
using System.ComponentModel;
using System;
using NUnit.Framework;
using System.IO;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Tests;
using System.Linq;
using System.Diagnostics;
using System.Text;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI.Tests
{
    /// <summary>
    /// Unit test class for DDC
    ///</summary>
    [TestFixture]
    public class DDCTest
    {
        private DDC.DDC ddc;

        [TestFixtureSetUp]
        public void Init()
        {
            TestSettings.Instance.Load();
            if (!TestSettings.Instance.ResourcePaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            GlobalsLib.IsUnitTest = true;
            ddc = new DDC.DDC();
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            ddc.Dispose();
            GlobalsLib.IsUnitTest = false;
        }

        /// <summary>
        /// Quick unit test of all DDC methods 
        /// Preserves correct testing order for efficiency/speed
        /// </summary>
        [Test]
        [Ignore("Intentionally Ignored TestAll ... OK")]
        public void TestAll()
        {
            // order is important to reduce redundant unit testing
            // add additional test here
            TestApplyPackageDD();
        }

        /// <summary>
        /// Unit test for ApplyPackageDD()
        ///</summary>
        [Test]
        public void TestApplyPackageDD()
        {
            TestSettings.Instance.CopyResources();
            if (!TestSettings.Instance.ArchivePaths.Any())
                Assert.Fail("TestApplyPackageDD CopyResources Failed ...");

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var ddcDir = Path.Combine(baseDir, "ddc");
            var results = new List<int>();
            var errorsFound = new StringBuilder();
            var errorCount = 0;
            var phraseLen = 8;
            var removeSus = false;
            var rampPath = Path.Combine(ddcDir, "ddc_default.xml");
            var cfgPath = Path.Combine(ddcDir, "ddc_default.cfg");
            var overWrite = false;
            var keepLog = false;

            foreach (var srcPath in TestSettings.Instance.ArchivePaths)
            {
                var consoleOutput = String.Empty;
                var result = ddc.ApplyPackageDD(srcPath, phraseLen, removeSus, rampPath, cfgPath, out consoleOutput, overWrite, keepLog);
                results.Add(result);

                if (!String.IsNullOrEmpty(consoleOutput))
                {
                    errorsFound.AppendLine(consoleOutput + Environment.NewLine + srcPath);
                    errorCount++;
                }
            }

            Assert.AreEqual(TestSettings.Instance.ArchivePaths.Count, results.Count);

            if (errorCount > 0)
                Assert.Fail("TestApplyPackageDD Failed ..." + Environment.NewLine + errorsFound.ToString());
        }




    }
}


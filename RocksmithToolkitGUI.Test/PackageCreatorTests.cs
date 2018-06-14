using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

// TODO: impliment unit testing

namespace RocksmithToolkitGUI.Test
{
    [TestClass]
    public class PackageCreatorTests
    {
        public readonly string TestFilePath1 = Path.Combine(Environment.CurrentDirectory, @"..\..\Resources\PeppaPig_p.psarc");

        [TestMethod]
        public void TestImportPackage()
        {
            var expecting = "";
            var result = "";

            Assert.AreEqual(expecting, result);
        }
    }
}

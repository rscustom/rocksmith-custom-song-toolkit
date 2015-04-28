using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using RocksmithToolkitGUI.DLCPackageCreator;
using RocksmithToolkitLib;
using NUnit.Framework;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLibTest
{
    [TestFixture]
    public class TuningDefinitionTests
    {
        [Test]
        public void NameFromStringsTest()
        {
            var t = new TuningDefinition();
            var tName = t.NameFromStrings(new TuningStrings(new Int16[]{ 0, 0, 0, 0, 0, 0 }), false, false);
            Assert.AreEqual("E Standard", tName);
        }

        [Test]
        public void EqualsTest()
        {
            var expected = new TuningStrings(new Int16[] { 0, 0, 0, 0, 0, 0 });
            var actual = new TuningStrings(new Int16[] { 0, 0, 0, 0, 0, 0 });
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TuningFormTest()
        {
            bool addNew;
            TuningDefinition formTuning;
            using (var form = new TuningForm())
            {
                form.Tuning = tuning;
                form.IsBass = selectedType == ArrangementType.Bass;

                if (DialogResult.OK != form.ShowDialog())
                    return;

                // prevent any further SET calls to form.Tuning
                formTuning = form.Tuning;
                addNew = form.AddNew;
            }
        }
    }
}

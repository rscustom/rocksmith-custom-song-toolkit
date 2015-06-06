using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
//using RocksmithToolkitGUI.DLCPackageCreator;
using RocksmithToolkitLib;
using NUnit.Framework;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Showlight;

namespace RocksmithToolkitLibTest
{
    [TestFixture]
    public class TuningDefinitionTests
    {
        [Test]
        public void NameFromStringsTest()
        {
            string tName = string.Empty; 
            var t = new TuningDefinition();
            var Estd = new TuningStrings(new Int16[] { 0, 0, 0, 0, 0, 0 });
            tName = t.NameFromStrings(Estd, false);
            Assert.AreEqual("E Standard", tName);

            var Ebstd = new TuningStrings(new Int16[] { -1, -1, -1, -1, -1, -1 });
            tName = t.NameFromStrings(Ebstd, true);
            Assert.AreEqual("Eb Standard", tName);

            var EbstdB = new TuningStrings(new Int16[] { -1, -1, -1, -1 }); //TODO: support bass
            tName = t.NameFromStrings(EbstdB, true);
            //Assert.AreEqual("Eb Standard", tName);

            var Ccstd = new TuningStrings(new Int16[] { -3, -3, -3, -3, -3, -3 });
            tName = t.NameFromStrings(Ccstd, false);
            Assert.AreEqual("C# Standard", tName);

            var DropEb = new TuningStrings(new Int16[] { -3, -1, -1, -1, -1, -1 });
            tName = t.NameFromStrings(DropEb, true);
            Assert.AreEqual("Eb Drop Db", tName);

            var DropA = new TuningStrings(new Int16[] { 5, 7, 7, 7, 7, 7 });
            tName = t.NameFromStrings(DropA, false);
            Assert.AreEqual("B Drop A", tName);
        }

        [Test]
        public void TuningStringsEqualsTest()
        {
            //Object\regular equality
            var expected = new TuningStrings(new Int16[] { -2, 0, 0, 0, 0, 0 });
            var actual = new TuningStrings(new Int16[] { 0, 0, 0, 0, 0, 0 });
            Assert.AreNotEqual(expected, actual);

            //LINQ equality
            var list = new List<TuningStrings> { expected, expected, actual };
            var distinct = list.Union(new List<TuningStrings> { actual, actual, expected }).ToList();
            if (list.Count == distinct.Count)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ShowlightEqualsTest()
        {
            var one = new Showlight
            {
                Note = 24,
                Time = 0.0F
            };
            var two = new Showlight
            {
                Note = 24,
                Time = 1.4F
            };
            var three= new Showlight
            {
                Note = 24,
                Time = 3.0F
            };
            var two1 = new Showlight
            {
                Note = 24,
                Time = 11.4F
            };
            var three1 = new Showlight
            {
                Note = 24,
                Time = 13.0F
            };
            var list = new List<Showlight> { one, two, two1, three, three1 };
            var distinct = list.OrderBy(x => x.Time).Union(new List<Showlight> { three, three1, two, two1, one }).ToList();
            if (list.Count == distinct.Count)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TuningFormTest()
        {/*
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
            }*/
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib.Sng2014HSL;
using System.Diagnostics;
using RocksmithToolkitLib.Sng;
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
            if (!TestSettings.Instance.ResourcePaths.Any())
                Assert.Fail("TestSettings Load Failed ...");

            TestSettings.Instance.UnpackResources();
            if (!TestSettings.Instance.UnpackedDirs.Any())
                Assert.Fail("TestSettings UnpackResources Failed ...");
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
                if (platform.version == GameVersion.RS2012)
                {
                    // NOTE: when unit test is finished, double click the test result to see this message
                    Debug.WriteLine("---------------------------------");
                    Debug.WriteLine("TestLoadFromFile skipped RS1 file: " + unpackedDir);
                    Debug.WriteLine("---------------------------------");
                    continue;
                }

                var sngPaths = Directory.EnumerateFiles(unpackedDir, "*.sng", SearchOption.AllDirectories).ToList();
                foreach (var sngPath in sngPaths)
                {
                    var song = Sng2014File.LoadFromFile(sngPath, platform);
                    if (song == null)
                        Assert.Fail("LoadFromFile Failed: " + Path.GetFileName(unpackedDir));

                    if (sngPath.ToLower().EndsWith("vocals.sng"))
                        continue;

                    if (!song.Arrangements.Arrangements.Any())
                        Assert.Fail("LoadFromFile Arrangements Failed: " + Path.GetFileName(unpackedDir));
                }
            }
        }

    }
}

//
// CODE GRAVEYARD - RS1 depricated testing
//
//
//[Test, TestCaseSource("AllSongs")]
//public void BeatsInOrder(SngFile song)
//{
//    Ebeat lastBeat = null;
//    foreach (var beat in song.Beats)
//    {
//        if (lastBeat != null)
//        {
//            Assert.IsTrue(lastBeat.Time < beat.Time);
//            if (lastBeat.Measure == beat.Measure)
//            {
//                Assert.AreEqual(lastBeat.Beat + 1, beat.Beat);
//            }
//            else
//            {
//                Assert.AreEqual(beat.Beat, 0);
//                Assert.LessOrEqual(lastBeat.Measure + 1, beat.Measure);
//            }
//        }
//        lastBeat = beat;
//    }
//}

//[Test, TestCaseSource("AllSongs")]
//public void PhraseIterationsInOrder(SngFile song)
//{
//    foreach (var pi in song.PhraseIterations)
//    {
//        Assert.LessOrEqual(pi.StartTime, pi.EndTime);
//        Assert.Less(pi.StartTime, 2000);
//        Assert.GreaterOrEqual(pi.StartTime, 0);
//        Assert.Less(pi.EndTime, 2000);
//        Assert.GreaterOrEqual(pi.EndTime, 0);
//    }
//}

//[Test, TestCaseSource("AllSongs")]
//public void VocalsInOrder(SngFile song)
//{
//    Vocal lastVocal = null;
//    foreach (var vocal in song._vocals)
//    {
//        if (lastVocal != null)
//        {
//            Assert.Less(lastVocal.Time, vocal.Time);
//        }
//        lastVocal = vocal;
//    }
//}

//[Test, TestCaseSource("AllSongs")]
//public void SongEventsValid(SngFile song)
//{
//    SongEvent lastSongEvent = null;
//    foreach (var songEvent in song.SongEvents)
//    {
//        if (lastSongEvent != null)
//        {
//            Assert.LessOrEqual(lastSongEvent.Time, songEvent.Time);
//        }

//        //Just make sure the time is sane
//        Assert.Less(songEvent.Time, 2000);
//        Assert.GreaterOrEqual(songEvent.Time, 0);

//        ValidString(songEvent.Code);

//        lastSongEvent = songEvent;
//    }
//}

//[Test, TestCaseSource("AllSongs")]
//public void SongSectionsInOrder(SngFile song)
//{
//    foreach (var songSection in song.SongSections)
//    {
//        Assert.LessOrEqual(songSection.StartTime, songSection.EndTime);
//    }
//}

//[Test, TestCaseSource("AllSongs")]
//public void ControlsValid(SngFile song)
//{
//    foreach (var control in song.Controls)
//    {
//        // These times can jump around (ex: TCAnchorZoneIntro_Lead)
//        /* 
//        if (lastControl != null)
//        {
//            Assert.LessOrEqual(lastControl.Time, control.Time);
//        }
//         * */

//        ValidString(control.Code);
//        Assert.Less(control.Time, 2000);
//        Assert.GreaterOrEqual(control.Time, 0);
//    }
//}

//[Test, TestCaseSource("AllSongs")]
//public void NotesInOrder(SngFile song)
//{
//    foreach (var level in song.SongLevels)
//    {
//        Note lastNote = null;
//        foreach (var note in level.Notes)
//        {
//            if (lastNote != null)
//            {
//                Assert.LessOrEqual(lastNote.Time, note.Time);
//            }
//            lastNote = note;
//        }
//    }
//}

//private void TestSngGeneration(SngFile rsSng, Action<SngFile, StringBuilder> func)
//{
//    var songKey = rsSng.Metadata.SongTitle + rsSng.Metadata.Arrangement + rsSng.Metadata.SongPart;
//    if (songKey == "0")
//    {
//        return;
//    }
//    string xmlName = null, xmlLocation = null;
//    if (rsXmlMappings.ContainsKey(songKey))
//    {
//        xmlName = rsXmlMappings[songKey];
//        if (xmlName == null)
//        {
//            return;
//        }
//        xmlLocation = Path.Combine(@"C:\Projects\RS\RS XML Clean", Path.ChangeExtension(xmlName, "xml"));
//    }
//    if (!File.Exists(xmlLocation))
//    {
//        xmlName = rsSng.Metadata.SongTitle + " - " + rsSng.Metadata.Arrangement;
//        xmlLocation = Path.Combine(@"C:\Projects\RS\RS XML Clean", Path.ChangeExtension(xmlName, "xml"));
//    }
//    if (!File.Exists(xmlLocation))
//    {
//        Assert.Fail("Couldn't find XML file for SNG: {0}", songKey);
//    }
//    var tmpSngLocation = Path.GetTempFileName();
//    var arrangement = rsSng.Metadata.Arrangement == "Bass" ? ArrangementType.Bass : ArrangementType.Guitar;
//    SngFileWriter.Write(xmlLocation, tmpSngLocation, arrangement, new Platform(GamePlatform.Pc, GameVersion.None));
//    SngFile toolkitSng = new SngFile(tmpSngLocation);
//    StringBuilder sb = new StringBuilder();
//    func(toolkitSng, sb);
//    try
//    {
//        File.Delete(tmpSngLocation);
//    }
//    catch { }
//    if (sb.Length > 0)
//    {
//        Assert.Fail(sb.ToString());
//    }
//}

//[Test, TestCaseSource("AllSongs"), Explicit("All facets are tested individually below - this saves time and localizes problems")]
//public void SngGenerationFilesMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("SngFile", toolkitSng, rsSng, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationEbeatsMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("Ebeats", toolkitSng.Beats, rsSng.Beats, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationPhraseIterationsMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("PhraseIterations", toolkitSng.PhraseIterations, rsSng.PhraseIterations, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationPhrasesMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("Phrases", toolkitSng.Phrases, rsSng.Phrases, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationControlsMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("Controls", toolkitSng.Controls, rsSng.Controls, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationMetadataMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("Metadata", toolkitSng.Metadata, rsSng.Metadata, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationPhrasePropertiesMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("PhraseProperties", toolkitSng.PhraseProperties, rsSng.PhraseProperties, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationSongEventsMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("SongEvents", toolkitSng.SongEvents, rsSng.SongEvents, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationSongLevelsMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("SongLevels", toolkitSng.SongLevels, rsSng.SongLevels, sb)
//    );
//}

//[Test, TestCaseSource("AllSongs")]
//public void SngGenerationSongSectionsMatch(SngFile rsSng)
//{
//    TestSngGeneration(rsSng, (toolkitSng, sb) =>
//        AssertEx.PropertyValuesAreEqual("SongSections", toolkitSng.SongSections, rsSng.SongSections, sb)
//    );
//}

//private Dictionary<string, string> rsXmlMappings = new Dictionary<string, string> {
//    //These have different XML names due to special characters, extra spaces, etc.
//    { "More Than Feeling BassBass1", "More Than  Feeling Bass - Bass.xml" },
//    { "What's My Age Again? - BassBass1", "What's My Age Again  - Bass - Bass.xml" },

//    //These are problem files - the XML just doesn't match what's in the SNG
//    { "First_Encounter_LeadLead1", null },
//    { "Technique Challenge - Bends ExampleLead1", null },
//    { "TEST - TunerLead1", null },
//};

//private IList<SngFile> _allSongs;

//public IEnumerable<SngFile> AllSongs
//{
//    get
//    {
//        return _allSongs ??
//            (_allSongs = Directory.EnumerateFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Rocksmith\Songs\GRExports\Generic\", "*.sng")
//                .Union(Directory.EnumerateFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Rocksmith\BassPack\GRExports\Generic\", "*.sng"))
//                .Select(x => new SngFile(x))
//                .ToList());
//    }
//}

//private static void ValidString(string s)
//{
//    var bytes = Encoding.ASCII.GetBytes(s);
//    Assert.False(bytes.Any(x => x > 127));
//    Assert.False(bytes.Any(x => x < 9));
//    Assert.False(bytes.Any(x => x > 9 && x < 32));
//}

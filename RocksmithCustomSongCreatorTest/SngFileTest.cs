using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RocksmithSngCreator;
using RocksmithSngCreator.IO;

namespace RocksmithCustomSongCreatorTest
{
    [TestFixture]
    public class SngFileTest
    {
        [Test]
        public void CanLoad()
        {
            var song = new SngFile(@"C:\Program Files (x86)\Steam\steamapps\common\Rocksmith\Songs\GRExports\Generic\NumberThirteen_Lead.sng");
        }

        [Test, TestCaseSource("AllSongs")]
        public void KnownVersion(SngFile song)
        {
            Assert.AreEqual(song._version, 49);

        }

        [Test, TestCaseSource("AllSongs")]
        public void BeatsInOrder(SngFile song)
        {
            Ebeat lastBeat = null;
            foreach (var beat in song._beats)
            {
                if (lastBeat != null)
                {
                    Assert.IsTrue(lastBeat.Time < beat.Time);
                    if (lastBeat.Measure == beat.Measure)
                    {
                        Assert.AreEqual(lastBeat.Beat + 1, beat.Beat);
                    }
                    else
                    {
                        Assert.AreEqual(beat.Beat, 0);
                        Assert.LessOrEqual(lastBeat.Measure + 1, beat.Measure);
                    }
                }
                lastBeat = beat;
            }
        }

        [Test, TestCaseSource("AllSongs")]
        public void PhraseIterationsInOrder(SngFile song)
        {
            PhraseIteration lastPi = null;
            foreach (var pi in song._phraseIterations)
            {
                if (lastPi != null)
                {
                    //Assert.Less(lastPi.Value._id, pi._id);
                }
                Assert.LessOrEqual(pi.StartTime, pi.EndTime);
                lastPi = pi;

                Assert.Less(pi.StartTime, 2000);
                Assert.GreaterOrEqual(pi.StartTime, 0);
                Assert.Less(pi.EndTime, 2000);
                Assert.GreaterOrEqual(pi.EndTime, 0);
            }
        }

        [Test, TestCaseSource("AllSongs")]
        public void VocalsInOrder(SngFile song)
        {
            Vocal lastVocal = null;
            foreach (var vocal in song._vocals)
            {
                if (lastVocal != null)
                {
                    Assert.Less(lastVocal.Time, vocal.Time);
                }
                lastVocal = vocal;
            }
        }

        [Test, TestCaseSource("AllSongs")]
        public void SongEventsValid(SngFile song)
        {
            SongEvent lastSongEvent = null;
            foreach (var songEvent in song.SongEvents)
            {
                if (lastSongEvent != null)
                {
                    Assert.LessOrEqual(lastSongEvent.Time, songEvent.Time);
                }
                 
                //Just make sure the time is sane
                Assert.Less(songEvent.Time, 2000);
                Assert.GreaterOrEqual(songEvent.Time, 0);

                ValidString(songEvent.Code);

                lastSongEvent = songEvent;
            }
        }

        [Test, TestCaseSource("AllSongs")]
        public void SongSectionsInOrder(SngFile song)
        {
            SongSection lastSongSection = null;
            foreach (var songSection in song.SongSections)
            {
                Assert.LessOrEqual(songSection.StartTime, songSection.EndTime);
                lastSongSection = songSection;
            }
        }

        [Test, TestCaseSource("AllSongs")]
        public void ControlsValid(SngFile song)
        {
            foreach (var control in song.Controls)
            {
                // These times can jump around (ex: TCAnchorZoneIntro_Lead)
                /* 
                if (lastControl != null)
                {
                    Assert.LessOrEqual(lastControl.Time, control.Time);
                }
                 * */

                ValidString(control.Code);
                Assert.Less(control.Time, 2000);
                Assert.GreaterOrEqual(control.Time, 0);
            }
        }

        [Test, TestCaseSource("AllSongs")]
        public void NotesInOrder(SngFile song)
        {
            foreach (var level in song.SongLevels)
            {
                Note lastNote = null;
                foreach (var note in level.Notes)
                {
                    if (lastNote != null)
                    {
                        Assert.LessOrEqual(lastNote.Time, note.Time);
                    }
                    lastNote = note;
                }
            }
        }

        private IList<SngFile> _allSongs = null;
        public IEnumerable<SngFile> AllSongs
        {
            get
            {
                if ( _allSongs == null )
                {
                    _allSongs = Directory.EnumerateFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Rocksmith\Songs\GRExports\Generic\", "*.sng")
                        .Select(x => new SngFile(x))
                        .ToList();
                }
                return _allSongs;
            }
            
        }

        private void ValidString(string s)
        {
            var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(s);
            Assert.False(bytes.Any(x => x > 127));
            Assert.False(bytes.Any(x => x < 9));
            Assert.False(bytes.Any(x => x > 9 && x < 32));
        }
    }
}

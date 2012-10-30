using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RocksmithSngCreator;

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

        public void KnownVersion(string filePath = null)
        {
            var song = new SngFile(filePath);

            Assert.AreEqual(song._version, 49);

        }

        [Test]
        public void BeatsInOrder()
        {
            BeatsInOrder(@"C:\Program Files (x86)\Steam\steamapps\common\Rocksmith\Songs\GRExports\Generic\NumberThirteen_Lead.sng");
        }

        public void BeatsInOrder(string filePath = null)
        {
            var song = new SngFile(filePath);

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

        public void ValidString(string s)
        {
            var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(s);
            Assert.False(bytes.Any(x => x > 127));
            Assert.False(bytes.Any(x => x < 9));
            Assert.False(bytes.Any(x => x > 9 && x < 32));
        }

        [Test]
        public void PhraseIterationsInOrder()
        {
            PhraseIterationsInOrder(@"C:\Program Files (x86)\Steam\steamapps\common\Rocksmith\Songs\GRExports\Generic\NumberThirteen_Lead.sng");
        }

        public void PhraseIterationsInOrder(string filePath = null)
        {
            var song = new SngFile(filePath);

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

        public void VocalsInOrder(string filePath)
        {
            var song = new SngFile(filePath);

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

        public void SongEventsValid(string filePath)
        {
            var song = new SngFile(filePath);

            SongEvent lastSongEvent = null;
            foreach (var songEvent in song.SongEvents)
            {
                // These times can jump around (ex: TCAnchorZoneIntro_Lead)
                /* 
                if (lastSongEvent != null)
                {
                    Assert.LessOrEqual(lastSongEvent.Time, songEvent.Time);
                }
                 * */

                //Just make sure the time is sane
                Assert.Less(songEvent.Time, 2000);
                Assert.GreaterOrEqual(songEvent.Time, 0);

                ValidString(songEvent.Code);

                lastSongEvent = songEvent;
            }
        }

        public void SongSectionsInOrder(string filePath)
        {
            var song = new SngFile(filePath);

            SongSection lastSongSection = null;
            foreach (var songSection in song.SongSections)
            {
                Assert.LessOrEqual(songSection.StartTime, songSection.EndTime);
                lastSongSection = songSection;
            }
        }

        public void ControlsValid(string filePath)
        {
            var song = new SngFile(filePath);

            SongSection lastSongSection = null;
            foreach (var control in song.Controls)
            {
                ValidString(control.Code);
                Assert.Less(control.Time, 2000);
                Assert.GreaterOrEqual(control.Time, 0);
            }
        }

        [Test]
        public void TestAllSongs()
        {
            foreach (var file in Directory.EnumerateFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Rocksmith\Songs\GRExports\Generic\", "*.sng"))
            {
                KnownVersion(file);
                BeatsInOrder(file);
                PhraseIterationsInOrder(file);
                VocalsInOrder(file);
                ControlsValid(file);
                SongEventsValid(file);
                SongSectionsInOrder(file);
            }
        }
    }
}

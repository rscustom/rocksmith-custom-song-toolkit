using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng2014HSL;

/*
  * * * * * * * * * Color scheme * * * * * * * * * * * * * * * * * * 
  * Fog midi notes: 24-35
  * 24(C)  = Green;                     25(C#) = Dark Red(G like)
  * 26(D)  = Medium Turquoise(C# like); 27(D#) = Brown(A like)
  * 28(E)  = Blue(D# like);             29(F)  = LtGreen(B like)
  * 30(F#) = Purple(E like);            31(G)  = Dark LtGreen(C# like)
  * 32(G#) = Dark Orange;               33(A)  = Yellow(A# like)
  * 34(A#) = LtBlue(D like);            35(B)  = Dark Violet(F like)
  * 
  * Spotlights midi notes: 48-59, 42 is off
  * 48 = Green                  54 = Dark Red
  * 49 = Medium Turquoise       55 = Brown
  * 50 = Blue                   56 = LtGreen
  * 51 = Purple                 57 = Dark LtGreen
  * 52 = Dark Orange            58 = Yellow
  * 53 = LtBlue                 59 = Dark Violet

  * Laser lights: 66 is off (maybe), 67 is on
  *
  * Unknown: 36-41
  * notes 43-47 causes game hangs
  * notes 60-62 causes game hangs
  * Unknown: 63-65
  * Unknown: 68-69
  *
  * CAUTION - showlights can be major cause of in-game crashes
  * Initialize Fog and Beam in first two elements to avoid in-game crashes
  * A minimum of two elements are required to produce default showlighting
  * Sometimes 2nd to last note is laser off followed by BeamNote and have same time
*/

namespace RocksmithToolkitLib.XML
{
    [XmlType("showlight")]
    public class Showlight : System.IEquatable<Showlight>
    {
        [XmlAttribute("time")]
        public float Time { get; set; }

        [XmlAttribute("note")]
        public int Note { get; set; }

        #region IEquatable implementation

        public static bool operator ==(Showlight left, Showlight right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Showlight left, Showlight right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                //Force Equals on each same note element, because we need near equal case for the time.
                return 0; //Note.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as Showlight;
            return other != null && Equals(other);
        }
        public bool Equals(Showlight other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Note == Note && Time + 2.0F > other.Time; //will work only if collection is ordered by time.
        }

        #endregion

    }

    [XmlRoot("showlights", Namespace = "", IsNullable = false)]
    public class Showlights
    {
        [XmlElement("showlight")]
        public List<Showlight> ShowlightList { get; set; }

        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        public static Showlights LoadFromFile(string showlightsRS2014File)
        {
            using (var reader = new StreamReader(showlightsRS2014File))
            {
                return new Extensions.XmlStreamingDeserializer<Showlights>(reader).Deserialize();
            }
        }

        public void Serialize(Stream stream, bool omitXmlDeclaration = false)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            Count = ShowlightList.Count;
            using (var writer = System.Xml.XmlWriter.Create(stream, new System.Xml.XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = omitXmlDeclaration,
                Encoding = new UTF8Encoding(false)
            }))
            {
                new XmlSerializer(typeof(Showlights)).Serialize(writer, this, ns);
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        public void CreateShowlights(DLCPackageData info)
        {
            int maxNoteChordCount = 0;
            int maxArrNdx = 0;
            int maxLevelNdx = 0;

            // find arrangment with most notes and chords
            for (int i = 0; i < info.Arrangements.Count; i++)
            {
                if (info.Arrangements[i].ArrangementType == ArrangementType.Vocal)
                    continue;
                if (info.Arrangements[i].ArrangementType == ArrangementType.ShowLight)
                    continue;
                if (info.Arrangements[i].SongXml.File == null)
                    continue;

                var song = Song2014.LoadFromFile(info.Arrangements[i].SongXml.File);

                // find level with most notes and chords
                for (int j = 0; j < song.Levels.Count(); j++)
                {
                    int noteCount = song.Levels[j].Notes.Count();
                    int chordCount = song.Levels[j].Chords.Count();
                    int noteChordCount = noteCount + chordCount;

                    if (noteChordCount > maxNoteChordCount)
                    {
                        maxNoteChordCount = noteChordCount;
                        maxArrNdx = i;
                        maxLevelNdx = j;
                    }
                }
            }

            Generate(info.Arrangements[maxArrNdx].SongXml.File, maxLevelNdx);
            AdjustShowlights(ShowlightList);
            Count = ShowlightList.Count;
        }

        /// <summary>
        /// Showlights Generator Rev3
        /// using arrangement and level with most notes and chords
        /// </summary>
        /// <param name="xmlFile">Xml file.</param>
        private void Generate(string xmlFile, int maxLevelNdx)
        {
            var midiNotes = new List<Showlight>();
            var chordNotes = new List<Showlight>();
            var song = Song2014.LoadFromFile(xmlFile);

            // error checking
            if (song.Phrases == null || song.Levels == null || song.Tuning == null)
                throw new Exception("Arrangement: " + xmlFile + Environment.NewLine +
                                    "Contains no phrases, levels and/or tuning.");

            // tuning used to get proper midi notes
            var tuning = song.Tuning.ToArray();

            foreach (var note in song.Levels[maxLevelNdx].Notes)
            {
                // make showlights changes occure on the beat/measure
                var measOffset = song.Ebeats.Where(eb => eb.Measure != -1 && eb.Time <= note.Time).Last();
                // forcing midi notes for guitar gives consistent results even with bass arrangements
                // var mNote = Sng2014FileWriter.GetMidiNote(tuning, (Byte)note.String, (Byte)note.Fret, false, song.Capo);
                // varying midi notes gives more color changes
                var mNote = Sng2014FileWriter.GetMidiNote(tuning, (Byte)note.String, (Byte)note.Fret, song.Arrangement == "Bass", song.Capo);
                midiNotes.Add(new Showlight { Time = measOffset.Time, Note = mNote });
            }

            foreach (var chord in song.Levels[maxLevelNdx].Chords)
            {
                if (chord.HighDensity == 1)
                    continue; //speedhack

                // make showlights occure on the beat/measure
                var measOffset = song.Ebeats.Where(eb => eb.Measure != -1 && eb.Time <= chord.Time).Last();
                // forcing midi notes for guitar may give consistent results even with bass arrangements
                // var mNote = Sng2014FileWriter.getChordNote(tuning, chord, song.ChordTemplates, false, song.Capo);
                // varying midi notes gives more color changes
                var mNote = Sng2014FileWriter.getChordNote(tuning, chord, song.ChordTemplates, song.Arrangement == "Bass", song.Capo);
                chordNotes.Add(new Showlight { Time = measOffset.Time, Note = mNote });
            }

            ShowlightList = new List<Showlight>();
            AddShowlights(midiNotes);
            AddShowlights(chordNotes);
        }

        private void AddShowlights(List<Showlight> list)
        {
            ShowlightList = ShowlightList.Concat(list).ToList();
            ShowlightList = ShowlightList.OrderBy(s => s.Time).ToList();
            // remove duplicate times
            ShowlightList = ShowlightList.GroupBy(s => s.Time).Select(g => g.Last()).ToList();
        }

        /// <summary>
        /// Adjust midi notes for tolerable showlight display
        /// </summary>
        /// <param name="showlightList"></param>
        private void AdjustShowlights(List<Showlight> showlightList)
        {
            if (showlightList.Count == 0)
                return;

            // remove notes that appear too early, showlights start at 10 seconds
            showlightList = showlightList.Where(s => s.Time >= 10.0F).ToList();

            // initialize showlights Fog and Beam
            // nothing else seems to matter if initialization is done properly
            if (showlightList[0].Time > 10.0F)
                showlightList.Insert(0, new Showlight { Note = GetFogNote(showlightList[0].Note), Time = 10.0F });

            if (showlightList[0].Note < 24 || showlightList[0].Note > 35)
                showlightList[0].Note = GetFogNote(showlightList[0].Note);

            if (showlightList[1].Note < 48 || showlightList[1].Note > 59)
                showlightList[1].Note = GetBeamNote(showlightList[1].Note);

            int[] badNotes = new int[] { 36, 37, 38, 39, 40, 41, 43, 44, 45, 46, 47 }; // , 60, 61, 62, 63, 64, 65, 68, 69 };



            // using bottoms up approach leaving first two initializing elements unchanged
            for (var i = showlightList.Count - 1; i > 1; i--)
            {
                // remove any bad/unknown notes from list
                var j = i;
                if (badNotes.AsParallel().Any(n => showlightList[j].Note.Equals(n)))
                {
                    showlightList.Remove(showlightList[j]);
                    continue;
                }

                // Change FogNote to BeamNote when half octive changes occure (quasi solo swell effect) not really
                if ((showlightList[i].Note > 23 && showlightList[i].Note < 36) &&
                    (showlightList[i - 1].Note > 23 && showlightList[i - 1].Note < 36))
                {
                    if (Math.Abs(showlightList[i].Note - showlightList[i - 1].Note) >= 6)
                    {
                        showlightList[i].Note = GetBeamNote(showlightList[i].Note);
                    }
                }

                // testing randomize after 12 effects
                //if (i % 12 == 0)  
                //{
                //    // for random effect
                //    var rnd = new Random(Guid.NewGuid().GetHashCode());
                //    showlightList[i].Note = rnd.Next(24, 36);
                //    showlightList[i - 1].Note = rnd.Next(48, 60);
                //    // showlightList[i - 1].Note = 42; // turn off spot
                //}

                // testing turn on laser / turn off spot
                if (showlightList[i - 1].Note > 59)
                {
                    showlightList[i - 1].Note = 42; // turn off spot
                    // showlightList[i - 1].Note = 67;
                }

                // adjust any back to back duplicate notes
                if (showlightList[i].Note == showlightList[i - 1].Note)
                {
                    // showlightList.Remove(showlightList[i]);
                    // TODO: testing randomized color effect instead of removal
                    var rnd = new Random(Guid.NewGuid().GetHashCode());
                    showlightList[i].Note = rnd.Next(24, 36);
                    showlightList[i - 1].Note = rnd.Next(48, 60);
                }
            }

            // add extra BeamNote at the end
            showlightList.Add(new Showlight
            {
                Note = GetBeamNote(showlightList[showlightList.Count - 1].Note),
                Time = showlightList[showlightList.Count - 1].Time
            });

            // now turn off the laser effect on 2nd to last note
            if (showlightList[showlightList.Count - 2].Note != 66)
                showlightList[showlightList.Count - 2].Note = 66;

            ShowlightList = new List<Showlight>();
            ShowlightList = showlightList;
        }

        private int GetFogNote(int midiNote) // 24-35
        {
            // Console.WriteLine("GetFogNote: " + (midiNote % 12) + (12 * 2));
            var rnd = new Random(midiNote);
            var fogNote = rnd.Next(24, 36);  // upper limit is exclusive
            return fogNote;
        }

        private int GetBeamNote(int midiNote) // 48-59
        {
            // Console.WriteLine("GetBeamNote: " + (midiNote % 12) + (12 * 4));
            var rnd = new Random(midiNote);
            var beamNote = rnd.Next(48, 60);  // upper limit is exclusive
            return beamNote;
        }

    }
}



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

namespace RocksmithToolkitLib.XML
{
    /*
     * * * * * * * * * Color scheme * * * * * * * * * * * * * * * * * * 
     * Fog midi notes: 24-35 (color scheme isn't natural)
     * 24(C)  = Green;                     25(C#) = Dark Red(G like)
     * 26(D)  = Medium Turquoise(C# like); 27(D#) = Brown(A like)
     * 28(E)  = Blue(D# like);             29(F)  = LtGreen(B like)
     * 30(F#) = Purple(E like);            31(G)  = Dark LtGreen(C# like)
     * 32(G#) = Dark Orange;               33(A)  = Yellow(A# like)
     * 34(A#) = LtBlue(D like);            35(B)  = Dark Violet(F like)
     * 
     * Unknown: 36-41
     * (?) Spotlights/colors/effects: 42-59
     * TODO: (?) Game hangs caused by 60-62
     * Unknown: 63, 64-65
     * (?) Laser lights: 66-67
     * 
     * Need to define Fog Color + stage lights before Venue shows up (Time = 0-10)
     */

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
            ShowlightList = ShowlightList.GroupBy(s => s.Time).Select(g => g.Last()).ToList();
        }

        /// <summary>
        /// Adjust midi notes to usable showlight notes
        /// </summary>
        /// <param name="showlightList"></param>
        private void AdjustShowlights(List<Showlight> showlightList)
        {
            if (showlightList.Count == 0)
                return;

            //Setup Stage Fog+Lights
            if (showlightList[0].Time > 10.0F)
            {
                showlightList.Insert(0, new Showlight { Note = GetFogNote(showlightList[0].Note), Time = 10.0F });
                showlightList.Insert(1, new Showlight { Note = GetBeamNote(showlightList[0].Note), Time = 10.0F });
            }
            else if (showlightList[0].Note < 24 || showlightList[0].Note > 35)
            {
                showlightList[0].Note = GetFogNote(showlightList[0].Note);
            }

            if (!showlightList.Select(s => s.Time < 10.0F && (s.Note > 41 && s.Note < 60)).Any())
            {
                showlightList.Insert(1, new Showlight { Note = GetBeamNote(showlightList[0].Note), Time = 10.0F });
            }

            // using bottoms up approach
            for (var i = showlightList.Count - 1; i > 0; i--)
            {
                // Remove any back to back duplicate notes
                if (showlightList[i].Note == showlightList[i - 1].Note)
                {
                    // showlightList.Remove(showlightList[i]);
                    // TODO: testing randomized color effect instead of removal
                    var rnd = new Random(DateTime.Now.Millisecond);
                    showlightList[i - 1].Note = rnd.Next(24, 35);
                }

                // Change FogNote to BeamNote when half octive changes occure
                if ((showlightList[i].Note > 23 && showlightList[i].Note < 36) &&
                    (showlightList[i - 1].Note > 23 && showlightList[i - 1].Note < 36))
                {
                    if (Math.Abs(showlightList[i].Note - showlightList[i - 1].Note) >= 6)
                        showlightList[i - 1].Note = GetBeamNote(showlightList[i - 1].Note);
                }
                else if (showlightList[i].Note > 41 && showlightList[i].Note < 60)
                    continue;
                else // Turn all other notes into FogNote
                    showlightList[i].Note = GetFogNote(showlightList[i].Note);
            }

            // add a duplicate note at the end
            showlightList.Add(new Showlight
            {
                Note = showlightList[showlightList.Count - 1].Note,
                Time = showlightList[showlightList.Count - 1].Time + 1.0F
            });

            // Forced laser effect for last note
            if (showlightList.Last().Note != 66)
                showlightList.Add(new Showlight { Note = 66, Time = showlightList[showlightList.Count - 1].Time });

            ShowlightList = new List<Showlight>();
            AddShowlights(showlightList);
        }

        private int GetFogNote(int midiNote) // 24-35
        {
            // Console.WriteLine("GetFogNote: " + (midiNote % 12) + (12 * 2));
            var rnd = new Random(midiNote % 12);
            var fogNote = rnd.Next(24, 35);
            return fogNote;
        }

        private int GetBeamNote(int midiNote) // 42-59
        {
            // Console.WriteLine("GetBeamNote: " + (midiNote % 12) + (12 * 4));
            var rnd = new Random(midiNote % 12);
            var beamNote = rnd.Next(42, 59);
            return beamNote;
        }




    }
}



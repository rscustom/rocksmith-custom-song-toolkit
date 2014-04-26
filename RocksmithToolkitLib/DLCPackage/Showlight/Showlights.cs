using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng2014HSL;

namespace RocksmithToolkitLib.DLCPackage.Showlight
{
    /*
     * * * * * * * * * Color sheme * * * * * * * * * * * * * * * * * * 
     * 24(C) = Green; 25(C#)= Dark Red(G like)
     * 26(D) = Medium Turquoise(C# like); 27(D#) = Brown(A like)
     * 28(E) = Blue(D# like); 29(F) = LtGreen(B like)
     * 30(F#) = Purple(E like); 31(G) = Dark LtGreen(C# like)
     * 32(G#) = Dark Orange; 33(A) = Yellow(A# like)
     * 34(A#) = LtBlue(D like); 35(B) = Dark Violet(F like)
     * 
     * Fog midi notes : 24-35 (color cheme not natural)
     * Unknown: 36-41
     * (?)Spotlights/colors/effects: 42-59
     * (?)Laser lights: 66-67
     * 
     * Need define Fog Color + stage lights before Venue shows (Time = 0-10)
     * 
     */

    [XmlRoot("showlights", Namespace = "", IsNullable = false)]
    public class Showlights
    {
        [XmlElement("showlight")]
        public List<Showlight> ShowlightList { get; set; }

        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        public Showlights() { ShowlightList = new List<Showlight>(); }

        public Showlights(DLCPackageData info) {
            ShowlightList = new List<Showlight>();

            foreach (var arrangement in info.Arrangements) {
                if (arrangement.ArrangementType == Sng.ArrangementType.Vocal)
                    continue;

                var showlightFile = Path.Combine(Path.GetDirectoryName(arrangement.SongXml.File), 
                    Path.GetFileNameWithoutExtension(arrangement.SongXml.File) + "_showlights.xml");

                if (!File.Exists(showlightFile))
                {
                    showlightFile = Path.Combine(Path.GetDirectoryName(arrangement.SongXml.File), info.Name + "_showlights.xml");

                    if (PopShList(Genegate(arrangement.SongXml.File).ShowlightList))
                        continue;
                }

                if (PopShList(LoadFromFile(showlightFile).ShowlightList))
                    continue;
            }

            ShowlightList = FixShowlights(ShowlightList);
            Count = ShowlightList.Count;
        }

        class EqShowlight : IEqualityComparer<Showlight>
        {
            public bool Equals(Showlight x, Showlight y)
            {
                if  (x == null || y == null)
                    return false;
                
                return (x.Note == y.Note && x.Time == y.Time) || 
                       (x.Note == y.Note && x.Time + .500D > y.Time);
            }

            public int GetHashCode(Showlight obj)
            {
                if (Object.ReferenceEquals(obj, null))
                    return 0;

                return obj.Time.GetHashCode() ^ obj.Time.GetHashCode() + obj.Note.GetHashCode();
            }
        }

        public void Serialize(Stream stream)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var writer = System.Xml.XmlWriter.Create(stream, new System.Xml.XmlWriterSettings {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = new UTF8Encoding(false) }))
            {
                new XmlSerializer(typeof(Showlights)).Serialize(writer, this, ns);
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);            
        }

        private int getFogNote(int midiNote)
        {
            return (midiNote % 12) + (12 * 2);
        }

        private int getBeamNote(int midiNote)
        {
            return (midiNote % 12) + (12 * 4);
        }

        /* Add to list logic
         * if (i+1 == List.Count) List.Add(objectToAdd);
         * else List.Insert(i+1, objectToAdd);
         */
        public List<Showlight> FixShowlights(List<Showlight> ShowlightList)
        {
            if (ShowlightList.Count == 0) return ShowlightList;

            if (ShowlightList[0].Time > 10.0F) {
                ShowlightList.Insert(0, new Showlight() { Note = getFogNote(ShowlightList[0].Note), Time = 10.0F });
            }

            if (ShowlightList[0].Note < 24 || ShowlightList[0].Note > 35) {
                ShowlightList[0].Note = getFogNote(ShowlightList[0].Note);
            }

            //Additional fix for stage lights
            for (var i = 1; i+1 <= ShowlightList.Count; i++)
            {
                try
                {
                    //if current is last, add new one n=66 t=i-1.t
                    if (i + 1 == ShowlightList.Count)
                    {
                        var objectToAdd = new Showlight()
                        {
                            Note = ShowlightList[i].Note,
                            Time = ShowlightList[i].Time + 1
                        };
                        
                        ShowlightList.Add(objectToAdd);
                    }

                    if (ShowlightList[i].Note == ShowlightList[i + 1].Note) // if next note is current
                        ShowlightList.Remove(ShowlightList[i + 1]);
                }
                catch {}
                //Fog Color for, every: note < 24 replace to its eq from 1 oct

                if (ShowlightList[i].Note < 24)
                {
                    ShowlightList[i].Note = getFogNote(ShowlightList[i].Note);
                    continue;
                }

                //For all notes > 67 || note in range [36..41] translate it to Beam\spotlight, range [42..59]
                if (ShowlightList[i].Note > 35 && ShowlightList[i].Note < 42 || ShowlightList[i].Note > 67)
                {
                    ShowlightList[i].Note = getBeamNote(ShowlightList[i].Note);
                    continue;
                }
            }

            return ShowlightList;
        }

        public Showlights Genegate(string xmlFile)
        {
            var midiNotes = new List<Showlight>();
            var chordNotes = new List<Showlight>();
            var ShowL = new Showlights();
            var song = Song2014.LoadFromFile(xmlFile);
            // If vocals
            if (song.Phrases == null || song.Tuning == null) return null;
            //Generate ShowlightList
            var tuning = song.Tuning.ToShortArray();

            if (song.Levels != null)
            {
                foreach (var lvl in song.Levels)
                {
                    for (int i = 0; i + 1 <= lvl.Notes.Count(); i++)
                    {
                        var mNote = Sng2014FileWriter.GetMidiNote(tuning,
                            (Byte)lvl.Notes[i].String,
                            (Byte)lvl.Notes[i].Fret,
                            song.Arrangement == "Bass",
                            song.Capo);

                        midiNotes.Add(new Showlight() { Time = lvl.Notes[i].Time, Note = mNote });
                    }
                    for (int i = 0; i + 1 <= lvl.Chords.Count(); i++)
                    {
                        if (lvl.Chords[i].HighDensity == 1)
                            continue; //speedhack

                        int mNote = Sng2014FileWriter.getChordNote(tuning,
                            lvl.Chords[i], song.ChordTemplates,
                            song.Arrangement == "Bass",
                            song.Capo);

                        chordNotes.Add(new Showlight() { Time = lvl.Chords[i].Time, Note = mNote });
                    }
                }
            }

            ShowL.PopShList(midiNotes);
            ShowL.PopShList(chordNotes);
            ShowL.Count = ShowL.ShowlightList.Count;

            return ShowL;
        }

        internal bool PopShList(List<Showlight> list)
        {
            if (ShowlightList.Count == 0)
                ShowlightList.AddRange(list);
            else
            {
                try {
                    var comp = new EqShowlight();
                    ShowlightList = list.Union(ShowlightList, comp).OrderBy(x => x.Time).ToList<Showlight>();
                    ShowlightList.TrimExcess();
                }
                catch {
                    return false;
                }
            }

            return true;
        }

        public Showlights LoadFromFile(string showlightsRS2014File)
        {
            using (var reader = new StreamReader(showlightsRS2014File))
            {
                return new Extensions.XmlStreamingDeserializer<Showlights>(reader).Deserialize();
            }
        }
    }
}

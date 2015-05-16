using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng2014HSL;

namespace RocksmithToolkitLib.DLCPackage.Showlight
{
    /*
     * * * * * * * * * Color sheme * * * * * * * * * * * * * * * * * * 
     * Fog midi notes: 24-35 (color cheme isn't natural)
     * 24(C)  = Green;                     25(C#) = Dark Red(G like)
     * 26(D)  = Medium Turquoise(C# like); 27(D#) = Brown(A like)
     * 28(E)  = Blue(D# like);             29(F)  = LtGreen(B like)
     * 30(F#) = Purple(E like);            31(G)  = Dark LtGreen(C# like)
     * 32(G#) = Dark Orange;               33(A)  = Yellow(A# like)
     * 34(A#) = LtBlue(D like);            35(B)  = Dark Violet(F like)
     * 
     * Unknown: 36-41
     * (?)Spotlights/colors/effects: 42-59
     * (?)Laser lights: 66-67
     * 
     * Need to define Fog Color + stage lights before Venue shows up (Time = 0-10)
     */

    [XmlRoot("showlights", Namespace = "", IsNullable = false)]
    public class Showlights
    {
        [XmlElement("showlight")]
        public List<Showlight> ShowlightList { get; set; }

        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        public Showlights()
        {
            ShowlightList = new List<Showlight>();
        }

        public Showlights(string showlightsfile)
            : this()
        {
            GetShowlights(showlightsfile);
        }

        public Showlights(DLCPackageData info)
            : this()
        {
            if (info.Arrangements.Any(a => a.ArrangementType == ArrangementType.Bass))
            {
                DoTheThing(info, info.Arrangements.First(ar => ar.ArrangementType == ArrangementType.Bass));
                return;
            }

            foreach (var arrangement in info.Arrangements) {
                if (arrangement.ArrangementType == ArrangementType.Vocal)
                    continue;
                if (arrangement.ArrangementType == ArrangementType.ShowLight)
                    continue;
                if (arrangement.SongXml.File == null)
                    continue;

                DoTheThing(info, arrangement);
            }
        }

        private void DoTheThing(DLCPackageData info, Arrangement arrangement)
        {
            var shlFile = Path.Combine(Path.GetDirectoryName(arrangement.SongXml.File),
                arrangement.SongXml.Name + "_showlights.xml");
            var shlCommon = Path.Combine(Path.GetDirectoryName(shlFile), info.Name + "_showlights.xml");
            if (!File.Exists(shlCommon))
            {
                //Generate
                GetShowlights(arrangement.SongXml.File);
                return;
            }
            GetShowlights(shlCommon);
        }

        public void Serialize(Stream stream)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            FixShowlights(ShowlightList);
            Count = ShowlightList.Count;
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

        private void GetShowlights(string showlightsfile)
        {
            PopShList(showlightsfile.ToLower().Contains("_showlights")
                ? LoadFromFile(showlightsfile)
                : Generate(showlightsfile));
        }

        private int GetFogNote(int midiNote)
        {
            Console.WriteLine((midiNote % 12) + (12 * 2));
            return (midiNote % 12) + (12 * 2);
        }

        private int GetBeamNote(int midiNote)
        {
            Console.WriteLine((midiNote % 12) + (12 * 4));
            return (midiNote % 12) + (12 * 4);
        }

        public bool FixShowlights(Showlights shl)
        {
            Console.WriteLine(shl.ShowlightList.ToString());
            return FixShowlights(shl.ShowlightList);
        }

        /* Add to list logic
         * if (i+1 == List.Count) List.Add(objectToAdd);
         * else List.Insert(i+1, objectToAdd);
         */
        public bool FixShowlights(List<Showlight> showlightList)
        {
            if (showlightList.Count == 0) return true;

            //Setup Stage Fog Color
            if (showlightList[0].Time > 10.0F) {
                showlightList.Insert(0, new Showlight { Note = GetFogNote(showlightList[0].Note), Time = 10.0F });
            }
            else if (showlightList[0].Note < 24 || showlightList[0].Note > 35) {
                showlightList[0].Note = GetFogNote(showlightList[0].Note);
            }
            //Setup Stage lights
            //Additional fix for stage lights
            for (var i = 1; i + 1 <= showlightList.Count; i++)
            {

                //if current is last, add new one n=n t=t+1
                if (i + 1 == showlightList.Count)
                {
                    var objectToAdd = new Showlight
                    {
                        Note = showlightList[i].Note,
                        Time = showlightList[i].Time + 1
                    };

                    showlightList.Add(objectToAdd);
                }

                if (showlightList[i].Note == showlightList[i + 1].Note) // if next note is current
                    showlightList.Remove(showlightList[i + 1]);

                //Fog Color for, every: Solo, every 30% of the song. NO EFFECT.
                if (showlightList[i].Note > 23 && showlightList[i].Note < 36)
                {
                    showlightList[i].Note = GetBeamNote(showlightList[i].Note);
                    continue;
                }

                //For all notes > 67 || note in range [36..41] translate it to Beam\spotlight, range [42..59]
                if (showlightList[i].Note < 24 || showlightList[i].Note > 35 && showlightList[i].Note < 42 || showlightList[i].Note > 67)
                {
                    showlightList[i].Note = GetBeamNote(showlightList[i].Note);
                    continue;//useless for now
                }
            }
            //Forced laser effect for last note (we probablty couldn't see it)
            showlightList[showlightList.Count - 1].Note = 66;

            return PopShList(showlightList);
        }

        /// <summary>
        /// Poorly written sowlights generator, lots of things missing.
        /// </summary>
        /// <param name="xmlFile">Xml file.</param>
        public Showlights Generate(string xmlFile)
        {
            var midiNotes = new List<Showlight>();
            var chordNotes = new List<Showlight>();
            var song = Song2014.LoadFromFile(xmlFile);
            // If vocals
            if (song.Phrases == null || song.Tuning == null) return null;
            //Generate ShowlightList
            var tuning = song.Tuning.ToArray();

            if (song.Levels != null)
            {
                foreach (var lvl in song.Levels)
                {
                    for (int i = 0; i + 1 <= lvl.Notes.Length; i++)
                    {
                        var mNote = Sng2014FileWriter.GetMidiNote(tuning,
                            (Byte)lvl.Notes[i].String,
                            (Byte)lvl.Notes[i].Fret,
                            song.Arrangement == "Bass",
                            song.Capo);

                        midiNotes.Add(new Showlight { Time = lvl.Notes[i].Time, Note = mNote });
                    }
                    for (int i = 0; i + 1 <= lvl.Chords.Length; i++)
                    {
                        if (lvl.Chords[i].HighDensity == 1)
                            continue; //speedhack

                        int mNote = Sng2014FileWriter.getChordNote(tuning,
                            lvl.Chords[i], song.ChordTemplates,
                            song.Arrangement == "Bass",
                            song.Capo);

                        chordNotes.Add(new Showlight { Time = lvl.Chords[i].Time, Note = mNote });
                    }
                }
            }

            PopShList(midiNotes);
            PopShList(chordNotes);

            return this;
        }

        public bool PopShList(Showlights shl)
        {
            return PopShList(shl.ShowlightList);
        }
        /// <summary>
        /// Populates current showlights list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>true if success.</returns>
        public bool PopShList(List<Showlight> list)
        {
            if (this.ShowlightList.Count == 0)
                this.ShowlightList.AddRange(list);
            else
            {
                try
                {
                    this.ShowlightList = this.ShowlightList.OrderBy(x => x.Time).Union(list.OrderBy(x => x.Time)).ToList();
                    this.ShowlightList.TrimExcess();
                }
                catch (Exception)
                {
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

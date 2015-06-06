using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng2014HSL;

namespace RocksmithToolkitLib
{
    public class TuningDefinition
    {
        [XmlAttribute("Version")]
        public GameVersion GameVersion { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string UIName { get; set; }

        [XmlAttribute]
        public bool Custom { get; set; }

        [XmlElement]
        public TuningStrings Tuning { get; set; }

        static string NoteName(TuningStrings tuning, byte s, bool flats = false)
        {
            String[] notesNamesHi = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            String[] notesNamesLo = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

            var id = Sng2014FileWriter.GetMidiNote(tuning.ToArray(), s, 0, false, 0)%12;
            return flats ? notesNamesLo[id] : notesNamesHi[id];
        }
        public string NameFromStrings(TuningStrings tuning, bool flats = true)
        {
            var t = tuning.ToArray();
            var noteNames = String.Empty;
            switch (GetTuningFamily(t))
            {
                case TuningFamily.Standard:
                    noteNames = string.Format("{0} Standard", NoteName(tuning, 0, flats));
                    break;
                case TuningFamily.Drop:
                    noteNames = string.Format("{0} Drop {1}", NoteName(tuning, 5, true), NoteName(tuning, 0, flats));
                    break;
                case TuningFamily.Open:
                    break;
                default:
                    for (Byte s = 0; s < 6; s++)
                        noteNames += NoteName(tuning, s, flats);
                    break;
            }
            return noteNames;
        }

        enum TuningFamily { None, Standard, Drop, Open }
        static TuningFamily GetTuningFamily(Int16[] t)
        {
            if (t[1] != t[2] || t[2] != t[3] || t[3] != t[4] || t[4] != t[5]) return TuningFamily.None;
            if (t[0] == t[1])
                return TuningFamily.Standard;
            if (t[0] + 2 == t[1])
                return TuningFamily.Drop;
            //if (false)
            //{
            //    return TuningFamily.Open;
            //}
            return TuningFamily.None;
        }
        public override string ToString()
        {
            return UIName;
        }

        // old fashioned but reliable way to load xml file to list object
        public static List<TuningDefinition> LoadFile(string fileName, GameVersion gameVersion)
        {
            List<TuningDefinition> tuningDefinitionsFiltered = new List<TuningDefinition>();

            using (FileStream stream = File.OpenRead(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<TuningDefinition>));
                List<TuningDefinition> tuningDefinitions = (List<TuningDefinition>)serializer.Deserialize(stream);

                foreach (var tuningDefinition in tuningDefinitions)
                {
                    if (tuningDefinition.GameVersion == gameVersion)
                        tuningDefinitionsFiltered.Add(tuningDefinition);
                }
            }

            return tuningDefinitionsFiltered;
        }

        public static void WriteFile(string fileName, List<TuningDefinition> tuningDefinitions)
        {
            using (FileStream stream = File.OpenWrite(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (List<TuningDefinition>));
                serializer.Serialize(stream, tuningDefinitions);
            }
        }

        public static TuningStrings Convert2Bass(TuningStrings guitarTuning)
        {
            var bassTuning = new TuningStrings
            {
                String0 = guitarTuning.String0,
                String1 = guitarTuning.String1,
                String2 = guitarTuning.String2,
                String3 = guitarTuning.String3,
                String4 = 0,
                String5 = 0
            };

            return bassTuning;
        }

        public static TuningDefinition Convert2Bass(TuningDefinition tuningDefinition)
        {
            TuningDefinition bassTuning = tuningDefinition;
            bassTuning.Tuning = Convert2Bass(tuningDefinition.Tuning);

            return bassTuning;
        }

    }
}

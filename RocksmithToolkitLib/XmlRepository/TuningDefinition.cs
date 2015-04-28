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

        public string NameFromStrings(TuningStrings tuning, bool isBass, bool flats = true)
        {
            String noteNames = String.Empty;
            String[] notesNamesHi = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            String[] notesNamesLo = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

            for (Byte s = 0; s < 6; s++)
            {
                var mNote = Sng2014FileWriter.GetMidiNote(tuning.ToArray(), s, 0, isBass, 0);
                if (flats)
                    noteNames += notesNamesLo[mNote % 12]; //oct = mNote / 12 - 1
                else
                    noteNames += notesNamesHi[mNote % 12]; //oct = mNote / 12 - 1
            }
            return noteNames;
        }

        public override string ToString()
        {
            //return (Custom) ? String.Format("{0} (custom)", UIName) : UIName;
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

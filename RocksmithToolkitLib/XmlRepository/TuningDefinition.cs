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

        public string NameFromStrings(TuningStrings tuning, bool isBass, bool inBem = true)
        {
            List<Int32> Notes = new List<Int32>();
            List<String> NoteNames = new List<String>();
            String[] notesNames = new String[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            String[] notesNamesHi = new String[] { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };
            for (Byte s = 0; s < 6; s++)
                Notes.Add(Sng2014FileWriter.GetMidiNote(tuning.ToArray(), s, 0, isBass, 0));
            foreach (var mNote in Notes)
                if (inBem) NoteNames.Add(notesNamesHi[mNote % 12]); //oct = mNote / 12 - 1
                else NoteNames.Add(notesNames[mNote % 12]); //oct = mNote / 12 - 1


            return String.Format("{0}{1}{2}{3}{4}{5}", NoteNames[0], NoteNames[1], NoteNames[2], NoteNames[3], NoteNames[4], NoteNames[5]);
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

        // .SequenceEqual, .Equals, "==", .GetHashCode do not produce accurate results
        // for objects ... the following works as expected for objects
        public static bool TuningEquals(TuningStrings x, TuningStrings y)
        {
            if (x == null || y == null)
                return false;

            return (x.String0 == y.String0 && x.String1 == y.String1 &&
                    x.String2 == y.String2 && x.String3 == y.String3 &&
                    x.String4 == y.String4 && x.String5 == y.String5);
        }

        public static TuningStrings Convert2Bass(TuningStrings guitarTuning)
        {
            TuningStrings bassTuning = new TuningStrings();
            bassTuning.String0 = guitarTuning.String0;
            bassTuning.String1 = guitarTuning.String1;
            bassTuning.String2 = guitarTuning.String2;
            bassTuning.String3 = guitarTuning.String3;
            bassTuning.String4 = 0;
            bassTuning.String5 = 0;

            return bassTuning;
        }

        public static TuningDefinition Convert2Bass(TuningDefinition tuningDefinition)
        {
            TuningDefinition bassTuning = new TuningDefinition();
            bassTuning = tuningDefinition;
            bassTuning.Tuning.String0 = tuningDefinition.Tuning.String0;
            bassTuning.Tuning.String1 = tuningDefinition.Tuning.String1;
            bassTuning.Tuning.String2 = tuningDefinition.Tuning.String2;
            bassTuning.Tuning.String3 = tuningDefinition.Tuning.String3;
            bassTuning.Tuning.String4 = 0;
            bassTuning.Tuning.String5 = 0;

            return bassTuning;
        }

    }
}

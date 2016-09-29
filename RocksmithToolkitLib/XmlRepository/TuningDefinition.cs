using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng2014HSL;

namespace RocksmithToolkitLib.XmlRepository
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

        public TuningDefinition()
        {
        }

        public TuningDefinition(TuningStrings tStrings, GameVersion rsVersion, string name = "", bool custom = true)
        {
            Custom = custom;
            Tuning = tStrings;
            GameVersion = rsVersion;

            UIName = Name = !string.IsNullOrEmpty(name) ? name : NameFromStrings(tStrings);
        }
        static string NoteName(TuningStrings tuning, byte s, bool flats = false)
        {
            String[] notesNamesHi = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }; //TODO: use maj\min or intervals classification...
            String[] notesNamesLo = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

            var id = Sng2014FileWriter.GetMidiNote(tuning.ToArray(), s, 0, false, 0) % 12;
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
            if (t[1] != t[2] || t[2] != t[3] || t[3] != t[4] || t[4] != t[5])
                return TuningFamily.None;
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

        // old fashioned but reliable way to load xml file to list object (avoid LINQ pls)
        public static List<TuningDefinition> LoadFile(string fileName, GameVersion gameVersion)
        {
            var tuningDefinitionsFiltered = new List<TuningDefinition>();

            using (var stream = File.OpenRead(fileName))
            {
                var tuningDefinitions = (List<TuningDefinition>) new XmlSerializer(typeof(List<TuningDefinition>)).Deserialize(stream);

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
            using (var stream = File.OpenWrite(fileName))
            {
                new XmlSerializer(typeof(List<TuningDefinition>)).Serialize(stream, tuningDefinitions);
            }
        }

        [Obsolete("Deprecated, please use regular guitar tuning methods.", true)]
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

        [Obsolete("Deprecated, please use regular guitar tuning methods.", true)]
        public static TuningDefinition Convert2Bass(TuningDefinition tuningDefinition)
        {
            TuningDefinition bassTuning = tuningDefinition;
            bassTuning.Tuning = Convert2Bass(tuningDefinition.Tuning);

            return bassTuning;
        }

        public static string TuningToName(TuningStrings songTuning, GameVersion gameVersion = GameVersion.RS2014, List<TuningDefinition> tuningXml = null)
        {
            // speed hack ... use preloaded TuningDefinitionRepository if available
            if (tuningXml == null)
                tuningXml = TuningDefinitionRepository.Instance.LoadTuningDefinitions(gameVersion);

            foreach (var tuning in tuningXml)
                if (tuning.Tuning.String0 == songTuning.String0 &&
                    tuning.Tuning.String1 == songTuning.String1 &&
                    tuning.Tuning.String2 == songTuning.String2 &&
                    tuning.Tuning.String3 == songTuning.String3 &&
                    tuning.Tuning.String4 == songTuning.String4 &&
                    tuning.Tuning.String5 == songTuning.String5)
                    return tuning.UIName;

            return "Other";
        }

        public static string TuningToName(string jsonTuning, GameVersion gameVersion = GameVersion.RS2014, List<TuningDefinition> tuningXml = null)
        {
            // speed hack ... use preloaded TuningDefinitionRepository if available
            if (tuningXml == null)
                tuningXml = TuningDefinitionRepository.Instance.LoadTuningDefinitions(gameVersion);

            var jObj = JObject.Parse(jsonTuning);
            TuningStrings songTuning = jObj.ToObject<TuningStrings>();

            foreach (var tuning in tuningXml)
                if (tuning.Tuning.String0 == songTuning.String0 &&
                    tuning.Tuning.String1 == songTuning.String1 &&
                    tuning.Tuning.String2 == songTuning.String2 &&
                    tuning.Tuning.String3 == songTuning.String3 &&
                    tuning.Tuning.String4 == songTuning.String4 &&
                    tuning.Tuning.String5 == songTuning.String5)
                    return tuning.UIName;

            return "Other";
        }

        public static string TuningStringToName(string strings, GameVersion gameVersion = GameVersion.RS2014, List<TuningDefinition> tuningXml = null)
        {
            // speed hack ... use preloaded TuningDefinitionRepository if available
            if (tuningXml == null)
                tuningXml = TuningDefinitionRepository.Instance.LoadTuningDefinitions(gameVersion);

            foreach (var tuning in tuningXml)
                if ("" + (tuning.Tuning.String0) + (tuning.Tuning.String1) + (tuning.Tuning.String2) + (tuning.Tuning.String3) + (tuning.Tuning.String4) + (tuning.Tuning.String5) == strings)
                    return tuning.UIName;

            return "Other";
        }
    }
}

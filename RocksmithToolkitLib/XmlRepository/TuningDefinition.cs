using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng2014HSL;

namespace RocksmithToolkitLib {
    public class TuningDefinition {
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
                Notes.Add(Sng2014FileWriter.GetMidiNote(tuning.ToShortArray(), s, 0, isBass, 0));
            foreach (var mNote in Notes)
                if(inBem) NoteNames.Add(notesNamesHi[mNote % 12]); //oct = mNote / 12 - 1
                else NoteNames.Add(notesNames[mNote % 12]); //oct = mNote / 12 - 1


            return String.Format("{0}{1}{2}{3}{4}{5}", NoteNames[0], NoteNames[1], NoteNames[2], 
                                                       NoteNames[3], NoteNames[4], NoteNames[5]);
        }

        public override string ToString() {
            //return (Custom) ? String.Format("{0} (custom)", UIName) : UIName;
            return UIName;
        }
    }
}

using System;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.SngToTab
{
    public class TabChord : TabNote
    {
        public readonly int[] Frets;
        public readonly string ChordName;

        public TabChord(TabFile tabFile, Note note, SngFile sngFile)
            : base(tabFile, note)
        {
            ChordTemplate chord = sngFile.ChordTemplates[note.ChordId];
            ChordName = chord.Name;
            Frets = new int[TabFile.StringCount];
            Frets[0] = chord.Fret0;
            Frets[1] = chord.Fret1;
            Frets[2] = chord.Fret2;
            Frets[3] = chord.Fret3;
            Frets[4] = chord.Fret4;
            Frets[5] = chord.Fret5;
        }

        // Returns the tab lines displaying the chord on the fretboard, including its name etc.
        public override string[] GetLines()
        {
            string[] notes = new string[TabFile.LineCount];
            int length = 0;
            for (int i = 0; i < TabFile.StringCount; i++)
            {
                if (Frets[i] == -1)
                    notes[i] = "";
                else
                {
                    notes[i] = Frets[i].ToString();
                    if (notes[i].Length > length)
                        length = notes[i].Length;
                }
            }

            length = Math.Max(ChordName.Length, length);

            string info = "";
            if (Harmonic)
                info = "H";
            if (PalmMute)
                info = ".";
            info = info.PadLeft(length, ' ');

            string chordName = ChordName.PadLeft(length, TabFile.PADDING_INFO);
            string emptyString = "-".PadLeft(length, TabFile.PADDING_STRING);

            string[] lines = new string[TabFile.LineCount];
            lines[TabFile.INFO] = info;
            lines[TabFile.CHORD] = chordName;
            lines[TabFile.BEAT] = "".PadLeft(length, TabFile.PADDING_INFO);
            for (int i = TabFile.StringCount - 1; i >= 0; i--)
            {
                int s = TabFile.StringCount - i - 1;
                if (Frets[s] != -1)
                    lines[TabFile.FIRST_STRING + i] = notes[s].PadLeft(length, TabFile.PADDING_STRING);
                else
                    lines[TabFile.FIRST_STRING + i] = emptyString;
            }

            return lines;
        }

        public override string ToString()
        {
            return "CHORD: " + ChordName;
        }
    }
}

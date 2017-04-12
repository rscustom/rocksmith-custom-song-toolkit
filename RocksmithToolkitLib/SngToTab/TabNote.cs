using System;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.SngToTab
{
    public class TabNote : TabLinesEntity
    {
        public readonly int String;
        public readonly int Fret;
        public readonly int Bend;
        public readonly bool HammerOn;
        public readonly bool Harmonic;
        public readonly bool PalmMute;
        public readonly bool PullOff;
        public readonly int SlideTo;
        public readonly Single SustainTime;
        public readonly bool Tremolo;

        public TabNote(TabFile tabFile, Note note)
            : base(tabFile, note.Time)
        {
            String = note.String;
            Fret = note.Fret;
            Bend = note.Bend;
            HammerOn = note.HammerOn != 0 ? true : false;
            Harmonic = note.Harmonic != 0 ? true : false;
            PalmMute = note.PalmMute != 0 ? true : false;
            PullOff = note.PullOff != 0 ? true : false;
            SlideTo = note.SlideTo;
            SustainTime = note.SustainTime;
            Tremolo = note.Tremolo != 0 ? true : false;
        }

        public override void Apply(TabFile tabFile)
        {
            if (tabFile.CurrentMeasure != null)
                tabFile.CurrentMeasure.AddNote(this);
        }

        // Returns the tab lines displaying the note on the fretboard with additional information
        // like bends, hammer-ons, palm-mutes etc.
        public override string[] GetLines()
        {
            string note = Fret.ToString();
            if (Bend != 0)
                note += "b" + (Fret + Bend).ToString();
            if (HammerOn)
                note = "h" + note;
            if (PullOff)
                note = "p" + note;
            if (SlideTo != -1)
                note += "s" + SlideTo;
            if (Tremolo)
                note += "~";

            int length = note.Length;

            string info = "";
            if (Harmonic)
                info = "H";
            if (PalmMute)
                info = ".";
            info = info.PadLeft(length, TabFile.PADDING_INFO);

            string emptyString = "".PadLeft(length, TabFile.PADDING_STRING);

            string[] lines = new string[TabFile.LineCount];
            lines[TabFile.INFO] = info;
            lines[TabFile.CHORD] = "".PadLeft(length, TabFile.PADDING_INFO);
            lines[TabFile.BEAT] = "".PadLeft(length, TabFile.PADDING_INFO);
            for (int i = TabFile.LineCount - 1; i >= TabFile.FIRST_STRING; i--)
            {
                int s = i - TabFile.FIRST_STRING;
                if (TabFile.StringCount - s - 1 == String)
                    lines[i] = note.PadLeft(length, TabFile.PADDING_STRING);
                else
                    lines[i] = emptyString;
            }

            return lines;
        }

        public override string ToString()
        {
            return "NOTE: String: " + String + ", Fret: " + Fret;
        }
    }
}

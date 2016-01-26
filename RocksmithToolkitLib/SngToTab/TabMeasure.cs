using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocksmithToolkitLib.SngToTab
{
    public class TabMeasure : TabEntity
    {

        public readonly Single Start;
        public readonly Single End;
        public readonly int BeatCount;

        public readonly SortedList<float, TabLinesEntity> Notes;
        public readonly SortedList<float, TabBeat> Beats;

        private string[] _lines;

        public TabMeasure(TabFile tabFile, float start, Single end, int beatCount)
            : base(tabFile, start)
        {
            Start = start;
            End = end;
            BeatCount = beatCount;

            Notes = new SortedList<float, TabLinesEntity>();
            Beats = new SortedList<float, TabBeat>();

            float step = (end - start) / (float)beatCount;
            for (int t = 0; t < beatCount; t++)
            {
                float tt = start + (float)t * step;
                Beats.Add(tt, new TabBeat(TabFile, tt));
            }

            _lines = null;
        }

        public TabMeasure(TabMeasure other)
            : this(other.TabFile, other.Start, other.End, other.BeatCount)
        {
        }

        public override void Apply(TabFile tabFile)
        {
            tabFile.NextMeasure(this);
        }

        public void AddNote(TabNote note)
        {
            Notes[note.Time] = note;
        }

        // Formatting code to display this measure and the notes it contains
        public string[] GetLines()
        {
            if (_lines != null)
                return _lines;

            _lines = new string[TabFile.LineCount];
            for (int i = 0; i < TabFile.LineCount; i++)
                _lines[i] = "";

            // Group the event time of all entities together in order to calculate
            // their position in the measure relative to their event time
            SortedSet<float> events = new SortedSet<float>();
            events.Add(Start);
            events.Add(End);
            events.UnionWith(Notes.Keys);
            events.UnionWith(Beats.Keys);
            // The slots array holds the positions of the individual entities
            // in the measure
            int[] slots = Common.GetSlots(events.ToArray(), 0.05f);
            bool[] slotIsNote = new bool[slots.Length];
            int t = 0;
            foreach (float time in events)
                slotIsNote[t++] = Notes.ContainsKey(time);

            // Now get the notes' and chords' text strings to be added to the measure
            // and calculate the width of one slot in order to create a monospaced measure
            List<string[]> noteStrings = new List<string[]>();
            int maxNoteLength = 0;
            foreach (TabLinesEntity n in Notes.Values)
            {
                string[] l = n.GetLines();
                noteStrings.Add(l);
                if (l[0].Length > maxNoteLength)
                    maxNoteLength = l[0].Length;
            }

            int slotLength = maxNoteLength + 1;

            // The slot length calculation algorithm might have returned high slot values
            // causing overly an long tab measure => "compress" the slot distribution
            while (slots[slots.Length - 1] * slotLength > TabFile.LINE_WRAP)
            {
                int[] nextSlots = new int[slots.Length];
                bool last = false;

                for (int i = 0; i < slots.Length; i++)
                {
                    nextSlots[i] = slots[i] / 2;

                    // Stop if 2 notes/chords would occupy the same position on the next compression step
                    if (i != 0 && slotIsNote[i] && slotIsNote[i - 1] && nextSlots[i] == nextSlots[i - 1])
                    {
                        last = true; // "break out of for loop"
                        break;
                    }
                }

                if (last)
                    break;

                slots = nextSlots;
            }

            // Now create the measure's tab representation
            int s = 0;
            foreach (float time in events)
            {
                // Check if the current event actually is a note and not a beat
                if (!Notes.ContainsKey(time))
                {
                    s++;
                    continue;
                }

                TabLinesEntity tle = Notes[time];

                int slot = slots[s];
                // calculate the position of the current note/chord in the measure
                // +1 because the first column should always be empty for readability's sake
                int index = slot * slotLength + 1;

                // First fill the measure with empty measure data up to the current position
                FillLines(index);

                // Now append the note/chord to the measure
                string[] l = tle.GetLines();
                for (int j = 0; j < TabFile.LineCount; j++)
                {
                    char padding = (j < TabFile.FIRST_STRING ? TabFile.PADDING_INFO : TabFile.PADDING_STRING);
                    _lines[j] += l[j].PadLeft(slotLength, padding);
                }

                s++;
            }

            // Fill the measure till the end
            int lastSlot = slots[events.Count - 1];
            int measureLength = lastSlot * slotLength;
            FillLines(measureLength);

            _lines[0] += "".PadLeft(2, TabFile.PADDING_INFO);
            _lines[1] += "".PadLeft(2, TabFile.PADDING_INFO);
            _lines[2] += "".PadLeft(2, TabFile.PADDING_INFO);
            _lines[TabFile.FIRST_STRING + 0] += "|".PadLeft(2, TabFile.PADDING_STRING);
            _lines[TabFile.FIRST_STRING + 1] += "|".PadLeft(2, TabFile.PADDING_STRING);
            _lines[TabFile.FIRST_STRING + 2] += "|".PadLeft(2, TabFile.PADDING_STRING);
            _lines[TabFile.FIRST_STRING + 3] += "|".PadLeft(2, TabFile.PADDING_STRING);
            _lines[TabFile.FIRST_STRING + 4] += "|".PadLeft(2, TabFile.PADDING_STRING);
            _lines[TabFile.FIRST_STRING + 5] += "|".PadLeft(2, TabFile.PADDING_STRING);

            // Now insert the beats
            s = 0;
            foreach (float time in events)
            {
                // Check if the current event actually is a beat
                if (!Beats.ContainsKey(time))
                {
                    s++;
                    continue;
                }

                TabBeat b = Beats[time];
                string beatString = TabFile.BEAT_STRING.PadLeft(slotLength, TabFile.PADDING_INFO);

                int slot = slots[s];
                int index = slot * slotLength + 1;

                _lines[TabFile.BEAT] = Common.ReplaceAt(_lines[TabFile.BEAT], index, beatString);
                s++;
            }
            return _lines;
        }

        // Fills the measure's text lines with empty data op to a given length
        private void FillLines(int length)
        {
            for (int l = 0; l < TabFile.LineCount; l++)
            {
                char padding = (l < TabFile.FIRST_STRING ? TabFile.PADDING_INFO : TabFile.PADDING_STRING);
                _lines[l] = _lines[l].PadRight(length, padding);
            }
        }

        public override string ToString()
        {
            return "--- MEASURE ---";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.SngToTab
{

    public class TabFile
    {
        public static readonly int INFO = 0;
        public static readonly int CHORD = 1;
        public static readonly int BEAT = 2;
        public static readonly int FIRST_STRING = 3;

        public static readonly int LINE_WRAP = 80;

        // Paddings chars for the tab representation of a measure
        public static readonly char PADDING_INFO = ' ';
        public static readonly char PADDING_STRING = '-';

        public static readonly string BEAT_STRING = "'";

        // Collects all measures in current section, so the output of all measures of the current section can be generated together
        private List<TabMeasure> _measureQueue;
        // Measure where notes are being added to
        private TabMeasure _currentMeasure;
        // Sometimes a copy of the last measure is needed if a measure is split by a section separator,
        // i.e. the first half of the section belongs to the verse and the second half to the chorus
        private TabMeasure _lastMeasure;

        public TabMeasure CurrentMeasure
        {
            get
            {
                if (_currentMeasure == null) // only happens if a section splits the current measure, cf. _currentMeasure definition
                {
                    // TODO: some songs contain notes timed prior to the definition of the first measure
                    //       which are simply dropped at the moment
                    if (_lastMeasure == null)
                        return null;

                    _currentMeasure = new TabMeasure(_lastMeasure);
                }
                return _currentMeasure;
            }
        }

        public int StringCount { get; private set; }

        public int LineCount
        {
            get
            {
                return FIRST_STRING + StringCount;
            }
        }

        public readonly Dictionary<Tuning, TuningInfo> TuningInfos;
        public readonly TuningInfo TuningInfo;

        public readonly int MaxDifficulty;

        // Output Buffer
        public List<string> Lines { get; protected set; }

        public TabFile(SngFile sngFile, int maxDifficulty)
        {
            StringCount = 6;

            _measureQueue = new List<TabMeasure>();
            _currentMeasure = null;
            Lines = new List<string>(); // holds text stream

            // Create the tuning information table
            TuningInfos = new Dictionary<Tuning, TuningInfo>();
            TuningInfos[Tuning.Standard] = new TuningInfo(this, Tuning.Standard, "Standard", new string[] { "e", "B", "G", "D", "A", "E" });
            TuningInfos[Tuning.DropD] = new TuningInfo(this, Tuning.DropD, "Drop D", new string[] { "e", "B", "G", "D", "A", "D" });
            TuningInfos[Tuning.EFlat] = new TuningInfo(this, Tuning.EFlat, "E Flat", new string[] { "eb", "Bb", "Gb", "Db", "Ab", "Eb" });
            TuningInfos[Tuning.OpenG] = new TuningInfo(this, Tuning.OpenG, "Open G", new string[] { "D", "B", "D", "G", "D", "G" });

            TuningInfo = TuningInfos[(Tuning)sngFile.Metadata.Tuning];

            // Start assembling the tab
            TabHeader header = new TabHeader(this, sngFile);

            /* Tab Entities are the parts that together make up a tab, i.e. notes, chords, measures and sections.
             * Since the notes and chords are all contained in a single array without any measure or section data,
             * the measure and section data needs to be merged with notes and chords so that all of that data can be
             * iterated over in the correct order. Entities get sorted by time index (the SortedList key).
             * Multiple entities (of different types) can have the same time index and need to be processed in the
             * correct order, i.e. sections before measures and measures before notes/chords. */
            SortedList<float, LinkedList<TabEntity>> entities = new SortedList<float, LinkedList<TabEntity>>();

            // Add SECTION entities
            foreach (SongSection s in sngFile.SongSections)
                addEntity(new TabSection(this, s), entities);

            // Add MEASURE entities
            float lastTime = sngFile.Beats[0].Time;
            int beatCount = 1;
            int lastMeasure = sngFile.Beats[0].Measure;

            for (int i = 1; i < sngFile.Beats.Length; i++)
            {
                Ebeat b = sngFile.Beats[i];
                // To indicate that the current beat is simply the next beat in the current measure and does not define
                // a new measure, either set b.Measure to -1 or to the same value as the beat before
                if (b.Measure == -1 || b.Measure == lastMeasure)
                    beatCount++;
                else
                {
                    addEntity(new TabMeasure(this, lastTime, b.Time, beatCount), entities);
                    lastTime = b.Time;
                    beatCount = 1;
                    lastMeasure = b.Measure;
                }
            }
            // Don't forget the final measure
            TabMeasure finalMeasure = new TabMeasure(this, lastTime, sngFile.Beats[sngFile.Beats.Length - 1].Time, beatCount);
            addEntity(finalMeasure, entities);

            MaxDifficulty = 0;
            // Add NOTE / CHORD entities
            foreach (PhraseIteration pi in sngFile.PhraseIterations)
            {
                Phrase p = sngFile.Phrases[pi.Id];
                if (MaxDifficulty < p.MaxDifficulty)
                    MaxDifficulty = p.MaxDifficulty;

                int level = Math.Min(maxDifficulty, p.MaxDifficulty);
                List<Note> notes = new List<Note>();

                foreach (Note n in sngFile.SongLevels[level].Notes)
                {
                    if (n.Time < pi.StartTime || n.Time > pi.EndTime)
                        continue;

                    if (n.ChordId != -1)
                        addEntity(new TabChord(this, n, sngFile), entities);
                    else
                        addEntity(new TabNote(this, n), entities);
                }
            }

            // Now that the entity list has been build, iterate over it and Apply every single entity to this TabFile,
            // which will format the entity's data and add it to the TabFile's output buffer.
            header.Apply(this, maxDifficulty);
            foreach (LinkedList<TabEntity> lle in entities.Values)
            {
                foreach (TabEntity e in lle)
                    e.Apply(this); // "this" contains a stream to make the tablature text
            }
            flushMeasureQueue(); // Don't forget, otherwise the last section's measures will be missing
        }

        // Shorthand to add entities to the SortedList->LinkedList structure
        private void addEntity(TabEntity entity, SortedList<float, LinkedList<TabEntity>> entities)
        {
            if (!entities.ContainsKey(entity.Time))
                entities[entity.Time] = new LinkedList<TabEntity>();
            entities[entity.Time].AddLast(entity);
        }

        public override string ToString()
        {
            return string.Join("", Lines.ToArray());
        }

        // Call this every time a new measure or section is being encountered
        // in order to properly finalize processing of the current measure
        private void FinishCurrentMeasure()
        {
            if (_currentMeasure == null)
                return;
            _measureQueue.Add(_currentMeasure);
            _lastMeasure = _currentMeasure;
            _currentMeasure = null;
        }

        // Finalizes the current measure and skips the to next measure given by parameter
        public void NextMeasure(TabMeasure tabMeasure)
        {
            FinishCurrentMeasure();
            _currentMeasure = tabMeasure;
        }

        // If a section is ending (due to new section start or EOF), output all measures#
        // belonging to this section.
        private void flushMeasureQueue()
        {
            FinishCurrentMeasure();
            if (_measureQueue.Count != 0)
            {
                string[] lines = FormatTabMeasures(_measureQueue);
                foreach (string line in lines)
                    Lines.Add(line + Environment.NewLine);
            }
            _measureQueue.Clear();
        }

        public void AppendLine(string line)
        {
            flushMeasureQueue();
            if (!line.EndsWith(Environment.NewLine))
                line += Environment.NewLine;
            Lines.Add(line);
        }

        public static void MergeLines(string[] a, string[] b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] += b[i];
        }

        // Does the actual tab formatting together with TabMeasure.getLines()
        private string[] FormatTabMeasures(List<TabMeasure> measures)
        {
            List<String> allLines = new List<String>();
            string[] lines = TuningInfo.StartLines;

            foreach (TabMeasure m in measures)
            {
                string[] measureLines = m.GetLines();

                // Line wrapping handling
                if (lines[0].Length + measureLines[0].Length > LINE_WRAP)
                {
                    allLines.InsertRange(allLines.Count, lines);
                    allLines.Add(Environment.NewLine);
                    lines = TuningInfo.StartLines;
                }

                MergeLines(lines, measureLines);
            }

            allLines.InsertRange(allLines.Count, lines);

            return allLines.ToArray();
        }
    }


}

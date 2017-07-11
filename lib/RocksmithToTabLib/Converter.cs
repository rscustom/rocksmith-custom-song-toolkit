using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.Sng2014HSL;

namespace RocksmithToTabLib
{
    public class Converter
    {
        /// <summary>
        /// Contains basic settings for tracks, namely the display name and
        /// the track colour. These are assigned to the tracks based on their
        /// identifier.
        /// </summary>
        private class TrackSettings
        {
            public string DisplayName;
            public int[] Color;
        }

        /// <summary>
        /// Default settings for the tracks based on their identifier. The "identifier3" are there
        /// for safety, mostly, haven't seen them yet.
        /// </summary>
        private static Dictionary<string, TrackSettings> DefaultTrackSettings = new Dictionary<string, TrackSettings>()
        {
            { "lead", new TrackSettings() { DisplayName = "Lead", Color = new int[] { 255, 151, 48 } } },
            { "lead1", new TrackSettings() { DisplayName = "Lead 1", Color = new int[] { 255, 151, 48 } } },
            { "lead2", new TrackSettings() { DisplayName = "Lead 2", Color = new int[] { 255, 111, 71 } } },
            { "lead3", new TrackSettings() { DisplayName = "Lead 3", Color = new int[] { 255, 62, 48 } } },
            { "combo", new TrackSettings() { DisplayName = "Combo", Color = new int[] { 102, 137, 255 } } },
            { "combo1", new TrackSettings() { DisplayName = "Combo 1", Color = new int[] { 102, 137, 255 } } },
            { "combo2", new TrackSettings() { DisplayName = "Combo 2", Color = new int[] { 84, 109, 255 } } },
            { "combo3", new TrackSettings() { DisplayName = "Combo 3", Color = new int[] { 59, 56, 255 } } },
            { "rhythm", new TrackSettings() { DisplayName = "Rhythm", Color = new int[] { 0, 163, 25 } } },
            { "rhythm1", new TrackSettings() { DisplayName = "Rhythm 1", Color = new int[] { 0, 163, 25 } } },
            { "rhythm2", new TrackSettings() { DisplayName = "Rhythm 2", Color = new int[] { 0, 124, 25 } } },
            { "rhythm3", new TrackSettings() { DisplayName = "Rhythm 3", Color = new int[] { 0, 93, 25 } } },
            { "bass", new TrackSettings() { DisplayName = "Bass", Color = new int[] { 119, 81, 67 } } },
            { "bass1", new TrackSettings() { DisplayName = "Bass 1", Color = new int[] { 119, 81, 67 } } },
            { "bass2", new TrackSettings() { DisplayName = "Bass 2", Color = new int[] { 99, 65, 52 } } },
            { "bass3", new TrackSettings() { DisplayName = "Bass 3", Color = new int[] { 61, 40, 32 } } },
        };


        /// <summary>
        /// Converts a single Rocksmith arrangement to a track in our intermediate score representation.
        /// The track is constructed from the given difficulty level, which defaults to the max available.
        /// The identifier name is used to set some display settings for the track.
        /// </summary>
        public static Track ConvertArrangement(Song2014 arrangement, string identifier, int difficultyLevel = int.MaxValue)
        {
            var track = new Track();

            track.Identifier = identifier;
            // we use the identifier to get a suitable display name and track color for this track.
            // if we don't have defaults for a particular identifier (shouldn't happen), we'll default
            // to the name given in the arrangement itself and the color red.
            TrackSettings trackSettings;
            if (DefaultTrackSettings.TryGetValue(identifier, out trackSettings))
            {
                track.Name = trackSettings.DisplayName;
                track.Color = trackSettings.Color;
            }
            else
            {
                track.Name = arrangement.Arrangement;
                track.Color = new int[] { 255, 0, 0 };
            }

            track.AverageBeatsPerMinute = arrangement.AverageTempo;
            if (arrangement.Arrangement.ToLower() == "bass")
                track.Instrument = Track.InstrumentType.Bass;
            else
                track.Instrument = Track.InstrumentType.Guitar;
            // todo: vocals

            if (arrangement.ArrangementProperties.PathBass != 0)
                track.Path = Track.PathType.Bass;
            else if (arrangement.ArrangementProperties.PathLead != 0)
                track.Path = Track.PathType.Lead;
            else
                track.Path = Track.PathType.Rhythm;
            track.Bonus = arrangement.ArrangementProperties.BonusArr != 0;

            // get tuning
            track.Tuning = GetTuning(arrangement);
            track.Capo = arrangement.Capo;

            // get chord templates
            track.ChordTemplates = GetChordTemplates(arrangement);

            // gather measures
            track.Bars = GetBars(arrangement);

            // gather notes
            int numStrings;
            track.DifficultyLevel = CollectNotesForDifficulty(arrangement, track.Bars, track.ChordTemplates, difficultyLevel, out numStrings);
            // ensure that the string minimum is sane
            if (track.Instrument == Track.InstrumentType.Guitar)
                numStrings = Math.Max(numStrings, 6);
            else if (track.Instrument == Track.InstrumentType.Bass)
                numStrings = Math.Max(numStrings, 4);
            track.NumStrings = numStrings;

            // add section headings to relevant notes
            AddSectionNames(arrangement, track.Bars);

            // figure out note durations and clean up potentially overlapping notes
            CalculateNoteDurations(track.Bars);
            HandleSustainsAndSilence(track.Bars);

            // take care of some after-processing for certain techniques
            SplitImplicitSlides(track.Bars);
            CalculateAndCleanBendOffsets(track.Bars);
            TransferHopo(track.Bars);

            return track;
        }


        static int[] GetTuning(Song2014 arrangement)
        {
            // Guitar Pro expects the tuning as Midi note values
            // In Rocksmith, the tuning is given as the difference in half steps
            // from standard tuning for each string, so we need to convert that.
            bool isBass = arrangement.Title.ToLower() == "bass";
            int[] tuning = new int[6];
            for (byte s = 0; s < tuning.Length; ++s)
                tuning[s] = Sng2014FileWriter.GetMidiNote(arrangement.Tuning.ToArray(), s, 0, isBass, 0);
            return tuning;
        }


        static Dictionary<int, ChordTemplate> GetChordTemplates(Song2014 arrangement)
        {
            var templates = new Dictionary<int, ChordTemplate>();

            for (int i = 0; i < arrangement.ChordTemplates.Length; ++i)
            {
                var rsTemplate = arrangement.ChordTemplates[i];

                var template = new ChordTemplate()
                {
                    ChordId = i,
                    Name = rsTemplate.ChordName,
                    Frets = new int[] { rsTemplate.Fret0, rsTemplate.Fret1, rsTemplate.Fret2,
                        rsTemplate.Fret3, rsTemplate.Fret4, rsTemplate.Fret5 },
                    Fingers = new int[] { rsTemplate.Finger0, rsTemplate.Finger1, 
                        rsTemplate.Finger2, rsTemplate.Finger3, rsTemplate.Finger4,
                        rsTemplate.Finger5 }
                };
                // correct for capo position. this is necessary since Rocksmith is weird when using 
                // a capo: the open string is indexed as 0, but any pressed fret is given as the
                // absolute fret, not relative to the capo.
                for (int j = 0; j < 6; ++j)
                {
                    if (template.Frets[j] > 0)
                        template.Frets[j] -= arrangement.Capo;
                }

                templates.Add(template.ChordId, template);

            }

            return templates;
        }


        static List<Bar> GetBars(Song2014 arrangement)
        {
            var bars = new List<Bar>();
            // Rocksmith keeps a list of ebeats with sub-beats, which translate to 
            // the actual beats in the measure. We can use this to make a sensible guess of
            // the measure's time.
            Bar currentMeasure = null;
            foreach (var ebeat in arrangement.Ebeats)
            {
                if (ebeat.Measure >= 0)
                {
                    // ebeat has a positive id, meaning it's a new measure
                    if (currentMeasure != null)
                    {
                        currentMeasure.End = ebeat.Time;
                        currentMeasure.BeatTimes.Add(ebeat.Time);
                        // sanity check: ensure there are no duplicates, which would indicate an empty sub-beat
                        currentMeasure.BeatTimes = currentMeasure.BeatTimes.Distinct().ToList();
                        currentMeasure.TimeNominator = currentMeasure.BeatTimes.Count - 1;
                        // figure out time and tempo
                        if (currentMeasure.End <= currentMeasure.Start)
                        {
                            // can happen in the last few silent bars that the bar is actually
                            // empty or even of negative length. in that case, just give it
                            // 2 seconds to ensure our note length calculations aren't thrown off.
                            currentMeasure.End = currentMeasure.Start + 2;
                        }
                        currentMeasure.GuessTimeAndBPM(arrangement.AverageTempo);
                    }

                    currentMeasure = new Bar() { TimeNominator = 1, Start = ebeat.Time };
                    currentMeasure.BeatTimes.Add(ebeat.Time);
                    bars.Add(currentMeasure);
                }
                else
                {
                    // sub-beat. Increase current measure's time nominator
                    if (currentMeasure == null)
                    {
                        Console.WriteLine("  WARNING: Encountered ebeat without id with no active measure?!");
                        // ignore for now
                    }
                    else
                    {
                        currentMeasure.TimeNominator += 1;
                        currentMeasure.BeatTimes.Add(ebeat.Time);
                    }
                }
            }

            if (bars.Count > 0)
            {
                //var lastBar = bars.Last();
                //lastBar.End = arrangement.SongLength;
                //lastBar.GuessTimeAndBPM(arrangement.AverageTempo);                

                // remove last bar, as it seems to have no actual function
                bars.RemoveAt(bars.Count - 1);
            }

            return bars;
        }


        static int CollectNotesForDifficulty(Song2014 arrangement, List<Bar> bars, Dictionary<int, ChordTemplate> chordTemplates, int difficultyLevel, out int numStrings)
        {
            // Rocksmith keeps its notes separated by the difficulty levels. Higher difficulty
            // levels only contain notes for phrases where the notes differ from lower levels.
            // This makes collection a little awkward, as we have to go phrase by phrase, 
            // to extract all the right notes.
            int maxDifficulty = 0;
            IEnumerable<Chord> allNotes = new List<Chord>();
            for (int pit = 0; pit < arrangement.PhraseIterations.Length; ++pit)
            {
                var phraseIteration = arrangement.PhraseIterations[pit];
                var phrase = arrangement.Phrases[phraseIteration.PhraseId];
                int difficulty = Math.Min(difficultyLevel, phrase.MaxDifficulty);
                var level = arrangement.Levels.FirstOrDefault(x => x.Difficulty == difficulty);
                maxDifficulty = Math.Max(difficulty, maxDifficulty);
                float startTime = phraseIteration.Time;
                float endTime = float.MaxValue;
                if (pit < arrangement.PhraseIterations.Length - 1)
                    endTime = arrangement.PhraseIterations[pit + 1].Time;

                // gather single notes and chords inside this phrase iteration
                var notes_temp = from n in level.Notes where n.Time >= startTime && n.Time < endTime
                                 select n; //CreateChord(n, arrangement.Capo);
                var notes = from n in notes_temp select CreateChord(n, arrangement.Capo);
                var chords = from c in level.Chords where c.Time >= startTime && c.Time < endTime
                             select CreateChord(c, chordTemplates, arrangement.Capo);
                allNotes = allNotes.Concat(notes.Concat(chords));
            }

            // figure out how many strings are used in the notes
            numStrings = 0;
            foreach (var chord in allNotes)
            {
                foreach (var note in chord.Notes)
                {
                    numStrings = Math.Max(numStrings, note.Value.String);
                }
            }
            numStrings++;  // strings are 0-indexed, so the count is max index + 1

            // avoid recreating all the notes/chords at every bar, so cache the conversion
            var collectedNotesList = allNotes.ToList();

            // Now put the chords into the bars they belong.
            for (int b = 0; b < bars.Count; ++b)
            {
                var bar = bars[b];
                // gather chords that lie within this bar
                bar.Chords = collectedNotesList.Where(x => x.Start >= bar.Start && x.Start < bar.End)
                    .OrderBy(x => x.Start).ToList();
                // if the bar is empty or the first chord does not start wit the bar,
                // fill the beginning of the bar with silence.
                if (bar.Chords.Count == 0 || bar.Chords.First().Start > bar.Start)
                {
                    // an empty chord indicates silence.
                    bar.Chords.Insert(0, new Chord() { Start = bar.Start });
                }
            }

            return maxDifficulty;
        }


        static Chord CreateChord(SongNote2014 note, int capo)
        {
            var chord = new Chord();
            chord.Start = note.Time;
            var convertedNote = CreateNote(note, capo);
            chord.Notes.Add(note.String, convertedNote);
            chord.Tremolo = convertedNote.Tremolo;
            chord.Slapped = convertedNote.Slapped;
            chord.Popped = convertedNote.Popped;
            return chord;
        }


        static Chord CreateChord(SongChord2014 rsChord, Dictionary<int, ChordTemplate> chordTemplates, int capo)
        {
            var chord = new Chord();
            chord.Start = rsChord.Time;
            chord.ChordId = rsChord.ChordId;
            chord.Tremolo = false;
            if (rsChord.ChordNotes != null)
            {
                foreach (var note in rsChord.ChordNotes)
                {
                    chord.Notes.Add(note.String, CreateNote(note, capo));
                }
            }
            if (chordTemplates.ContainsKey(chord.ChordId))
            {
                // need to determine chords from the chord template
                var template = chordTemplates[chord.ChordId];
                for (int i = 0; i < 6; ++i)
                {
                    if (template.Frets[i] >= 0 && !chord.Notes.ContainsKey(i))
                    {
                        var note = new Note()
                        {
                            Fret = template.Frets[i],
                            String = i,
                            LeftFingering = template.Fingers[i],
                            RightFingering = -1,
                        };
                        chord.Notes.Add(i, note);
                    }
                }
            }
            if (chord.Notes.Count == 0)
            {
                Console.WriteLine("  Warning: Empty chord. Cannot find chord with chordId {0}.", chord.ChordId);
            }

            // some properties set on the chord in Rocksmith need to be passed down to the individual notes
            // and vice versa
            foreach (var kvp in chord.Notes)
            {
                if (rsChord.PalmMute != 0)
                    kvp.Value.PalmMuted = true;
                if (rsChord.FretHandMute != 0)
                    kvp.Value.Muted = true;
                if (rsChord.Accent != 0)
                    kvp.Value.Accent = true;
                if (kvp.Value.Tremolo)
                    chord.Tremolo = true;
                if (kvp.Value.Slapped)
                    chord.Slapped = true;
                if (kvp.Value.Popped)
                    chord.Popped = true;
            }

            // we will show a strum hint for all chords played with an up-stroke,
            // and a down-stroke hint for all chords with more than 3 notes (to exclude power-chords)
            //if (rsChord.Strum.ToLower() == "up")
            //    chord.BrushDirection = Chord.BrushType.Up;
            //else if (chord.Notes.Count > 3 && rsChord.Strum.ToLower() == "down")
            //    chord.BrushDirection = Chord.BrushType.Down;
            // disabled, since apparently the strum hints aren't really useful. I might have
            // misunderstood the parameter.

            return chord;
        }


        static Note CreateNote(SongNote2014 rsNote, int capo)
        {
            var note = new Note()
            {
                Start = rsNote.Time,
                String = rsNote.String,
                Fret = rsNote.Fret,
                PalmMuted = rsNote.PalmMute != 0,
                Muted = rsNote.Mute != 0,
                Hopo = rsNote.HammerOn != 0 || rsNote.PullOff != 0,
                Vibrato = rsNote.Vibrato > 0,
                LinkNext = rsNote.LinkNext != 0,
                Accent = rsNote.Accent != 0,
                Harmonic = rsNote.Harmonic != 0,
                PinchHarmonic = rsNote.HarmonicPinch != 0,
                Tremolo = rsNote.Tremolo != 0,
                Tapped = rsNote.Tap != 0,
                Slapped = rsNote.Slap == 1,
                Popped = rsNote.Pluck == 1,
                LeftFingering = rsNote.LeftHand,
                RightFingering = rsNote.RightHand,
                Sustain = rsNote.Sustain
            };
            if (rsNote.SlideTo != -1)
            {
                note.Slide = Note.SlideType.ToNext;
                note.SlideTarget = rsNote.SlideTo;
            }
            else if (rsNote.SlideUnpitchTo != -1)
            {
                if (rsNote.SlideUnpitchTo > rsNote.Fret)
                    note.Slide = Note.SlideType.UnpitchUp;
                else
                    note.Slide = Note.SlideType.UnpitchDown;
            }
            if (rsNote.BendValues != null)
            {
                foreach (var val in rsNote.BendValues)
                {
                    note.BendValues.Add(new Note.BendValue()
                    {
                        Start = val.Time,
                        Step = val.Step
                    });
                }
                note.BendValues = note.BendValues.OrderBy(x => x.Start).ToList();
            }
            // adjust for capo
            if (note.Fret > 0)
                note.Fret -= capo;

            return note;
        }


        static void AddSectionNames(Song2014 arrangement, List<Bar> bars)
        {
            var sections = arrangement.Sections.OrderBy(x => x.StartTime);
            int c = 0;
            int b = 0;
            foreach (var section in sections)
            {
                while (b < bars.Count && bars[b].Chords[c].Start < section.StartTime)
                {
                    ++c;
                    if (c >= bars[b].Chords.Count)
                    {
                        c = 0;
                        ++b;
                    }
                }

                if (b < bars.Count)
                {
                    var chord = bars[b].Chords[c];
                    chord.Section = section.Name.ToUpper();
                }
            }
        }


        static void HandleSustainsAndSilence(List<Bar> bars)
        {
            // Wo go through all bars and extend sustained notes or chords at the end of 
            // a bar.
            Chord lastChord = null;
            var sustainedNotes = new Dictionary<int, Note>();
            for (int b = 0; b < bars.Count; ++b)
            {
                var bar = bars[b];

                Chord nextChord = (bar.Chords.Last().Notes.Count != 0) ? bar.Chords.Last() : null;

                // if the first chord of the bar is empty (silent), we will extend the last
                // chord of the previous bar (if applicable)
                if (bar.Chords.First().Notes.Count == 0 && sustainedNotes.Count == 0 && lastChord != null)
                {
                    // extend the chord from the previous bar into the silence
                    var newChord = SplitChord(lastChord, bar.Start);
                    newChord.Duration = bar.Chords[0].Duration;
                    newChord.Section = bar.Chords[0].Section;
                    bar.Chords[0] = newChord;
                }
                lastChord = nextChord;

                // next, we handle sustained notes
                foreach (var chord in bar.Chords)
                {
                    // add previous sustained notes to the current chord
                    foreach (var kvp in sustainedNotes)
                    {
                        if (kvp.Value.Start + kvp.Value.Sustain <= chord.Start)
                            continue;  // already past its sustain time

                        if (!chord.Notes.ContainsKey(kvp.Key))
                        {
                            var newNote = SplitNote(kvp.Value, chord.Start);
                            chord.Notes.Add(kvp.Key, newNote);
                        }
                        else
                        {
                            //Console.WriteLine("  Warning: A sustained note was cut off prematurely in bar {0}", b);
                        }
                    }
                    sustainedNotes.Clear();
                    // now see if any notes in the current chord should be sustained
                    foreach (var kvp in chord.Notes)
                    {
                        if (kvp.Value.Sustain > 0)
                            sustainedNotes.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }


        static Note SplitNote(Note note, float startTime)
        {
            Note newNote = new Note()
            {
                Start = startTime,
                String = note.String,
                Fret = note.Fret,
                Hopo = false,
                Accent = note.Accent,
                Harmonic = note.Harmonic,
                PinchHarmonic = note.PinchHarmonic,
                LeftFingering = -1,
                RightFingering = -1,
                Popped = false,
                Slapped = false,
                LinkNext = note.LinkNext,
                Muted = note.Muted,
                PalmMuted = note.PalmMuted,
                Tapped = false,
                Slide = note.Slide,
                SlideTarget = note.SlideTarget,
                Vibrato = note.Vibrato,
                Tremolo = note.Tremolo,
                Sustain = note.Start + note.Sustain - startTime,
                _Extended = true
            };
            note.Slide = Note.SlideType.None;
            note.LinkNext = true;
            note.Sustain = startTime - note.Start;

            // Split bend values
            newNote.BendValues = note.BendValues.Where(x => x.Start >= startTime).ToList();
            note.BendValues = note.BendValues.Where(x => x.Start < startTime).ToList();
            var before = note.BendValues.LastOrDefault();
            var after = newNote.BendValues.FirstOrDefault();
            if (after != null)
            {
                // there may be a linear change in bend between the two notes, calculate the 
                // bend value at the point between both notes and insert into each note
                float beforeStart = (before != null) ? before.Start : 0;
                float beforeStep = (before != null) ? before.Step : 0;
                float distance = after.Start - beforeStart;
                float steps = after.Step - beforeStep;
                float gradient = steps / distance;
                newNote.BendValues.Insert(0, new Note.BendValue()
                {
                    Start = startTime,
                    Step = beforeStep + gradient * (startTime - beforeStart)
                });
                note.BendValues.Add(new Note.BendValue()
                {
                    Start = startTime,
                    Step = beforeStep + gradient * (startTime - beforeStart)
                });
            }
            else if (before != null && before.Step != 0)
            {
                newNote.BendValues.Insert(0, new Note.BendValue()
                    {
                        Start = startTime,
                        Step = before.Step
                    });
            }

            return newNote;
        }


        static Chord SplitChord(Chord chord, float startTime)
        {
            var newChord = new Chord()
            {
                ChordId = chord.ChordId,
                BrushDirection = chord.BrushDirection,
                Start = startTime,
                Popped = false,
                Slapped = false,
                Tremolo = chord.Tremolo
            };

            // copy over notes
            foreach (var kvp in chord.Notes)
            {
                var newNote = SplitNote(kvp.Value, startTime);
                newChord.Notes.Add(kvp.Key, newNote);
            }

            return newChord;
        }


        static void TransferHopo(List<Bar> bars)
        {
            // Rocksmith places hammer-on / pull-off on the second note.
            // However, for our exporter it is much easier to handle the flag being set
            // on the first note, so we'll go through all notes and move the flag one note
            // to the left.
            bool[] hopo = new bool[] { false, false, false, false, false, false };
            for (int b = bars.Count - 1; b >= 0; --b)
            {
                var bar = bars[b];
                for (int c = bar.Chords.Count - 1; c >= 0; --c)
                {
                    var chord = bar.Chords[c];
                    foreach (var kvp in chord.Notes)
                    {
                        bool temp = kvp.Value.Hopo;
                        kvp.Value.Hopo = hopo[kvp.Key];
                        hopo[kvp.Key] = temp;
                    }
                }
            }
        }


        static void CalculateNoteDurations(List<Bar> bars)
        {
            for (int b = 0; b < bars.Count; ++b)
            {
                var bar = bars[b];
                //Console.Write(">>BAR {0}>> ", b+1);
                float total = 0;

                var noteDurations = new List<float>();
                for (int i = 0; i < bar.Chords.Count; ++i)
                {
                    var chord = bar.Chords[i];
                    Single end = (i == bar.Chords.Count - 1) ? bar.End : bar.Chords[i + 1].Start;
                    float duration = bar.GetDuration(chord.Start, end - chord.Start);
                    //Console.WriteLine("Bar {0}, note {1}, duration {2:f2}, start {3:f2}, end {4:f2}", b, i, duration, chord.Start, end);
                    noteDurations.Add(duration);
                    total += duration;
                }
                //Console.WriteLine("   << S {0:f2}", total);

                var cleanRhythm = RhythmDetector.GetRhythm(noteDurations, bar.GetBarDuration(), bar.GetBeatDuration());
                int curIndex = -1;
                int curChord = 0;

                foreach (var rhythm in cleanRhythm)
                {
                    if (rhythm.NoteIndex == curIndex)
                    {
                        // repeat index means the previous chord was split, so need to duplicate
                        var prevChord = bar.Chords[curChord - 1];
                        float splitPoint = prevChord.Start + bar.GetDurationLength(prevChord.Start, prevChord.Duration);
                        var newChord = SplitChord(prevChord, splitPoint);
                        bar.Chords.Insert(curChord, newChord);
                    }
                    curIndex = rhythm.NoteIndex;
                    var chord = bar.Chords[curChord];
                    chord.Duration = rhythm.Duration;

                    if (chord.Duration == 0)
                    {
                        // duration of 0 means we should merge with the next chord.
                        // this can happen e.g. if Rocksmith places several single notes at the same time
                        // instead of using a chord.
                        if (curChord < bar.Chords.Count - 1)
                        {
                            //Console.WriteLine("  Note value too short, merging with next note in bar {0}", b);
                            var next = bar.Chords[curChord + 1];
                            next.Start = chord.Start;
                            if (next.Section == null)
                                next.Section = chord.Section;
                            foreach (var kvp in chord.Notes)
                            {
                                if (!next.Notes.ContainsKey(kvp.Key))
                                    next.Notes.Add(kvp.Key, kvp.Value);
                                else if (!next.Notes[kvp.Key]._Extended)
                                    Console.WriteLine("  Warning: Not possible to merge empty note with neighbour in bar {0}", b);
                            }

                        }
                        else
                        {
                            // very unlikely (?) should merge with next bar
                            if (b != bars.Count - 1)
                            {
                                //Console.WriteLine("  Note value too short, merging with first note of next bar in bar {0}", b);
                                var next = bars[b + 1].Chords.First();
                                if (next.Section == null)
                                    next.Section = chord.Section;
                                foreach (var kvp in chord.Notes)
                                {
                                    if (!next.Notes.ContainsKey(kvp.Key))
                                        next.Notes.Add(kvp.Key, kvp.Value);
                                    else if (!next.Notes[kvp.Key]._Extended)
                                        Console.WriteLine("  Warning: Not possible to merge empty note with next bar in bar {0}", b);
                                }
                            }
                        }
                    }

                    ++curChord;
                }
                

                bar.Chords.RemoveAll(x => x.Duration == 0);
            }
        }



        static void SplitImplicitSlides(List<Bar> bars)
        {
            // Unfortunately, for targeted slides, Rocksmith does not usually follow the sliding note
            // with a target note, so the target note may be implied only. Of course, this does not 
            // work for our export, so if we find such a case, we need to split the sliding note into
            // two and set the second one to the target.
            Chord lastChord = null;
            for (int b = 0; b < bars.Count; ++b)
            {
                var bar = bars[b];
                var nextBar = (b < bars.Count-1) ? bars[b+1] : null;
                for (int i = 0; i < bar.Chords.Count; ++i)
                {
                    var chord = bar.Chords[i];
                    var nextChord = (i < bar.Chords.Count - 1) ? bar.Chords[i + 1] : ((nextBar != null) ? nextBar.Chords.FirstOrDefault() : null);
                    // see if there's an unmatched slide in the current chord.
                    foreach (var kvp in chord.Notes)
                    {
                        var note = kvp.Value;
                        if (note.Slide == Note.SlideType.ToNext)
                        {
                            if (!note.LinkNext || nextChord == null || !nextChord.Notes.ContainsKey(kvp.Key) ||
                                nextChord.Notes[kvp.Key].Fret != note.SlideTarget)
                            {
                                // found an instance where we need to set the slide up manually
                                if (note._Extended && lastChord != null && lastChord.Notes.ContainsKey(kvp.Key))
                                {
                                    // this note was already split previously, so move the
                                    // slide one step back and change this to the target
                                    var prevNote = lastChord.Notes[kvp.Key];
                                    prevNote.Slide = note.Slide;
                                    prevNote.SlideTarget = note.SlideTarget;
                                    note.Fret = note.SlideTarget;
                                    note.Slide = Note.SlideType.None;
                                    note.SlideTarget = -1;
                                }
                                else
                                {
                                    // split the chord, ideally in half, but take care to use durations
                                    // that are valid
                                    int duration = chord.Duration / 2 + 1;
                                    int remaining = chord.Duration - duration;
                                    while (duration >= 2)
                                    {
                                        if (RhythmDetector.PrintableDurations.Contains(duration) &&
                                            RhythmDetector.PrintableDurations.Contains(remaining))
                                            break;
                                        --duration;
                                        ++remaining;
                                    }
                                    if (!RhythmDetector.PrintableDurations.Contains(duration) ||
                                        !RhythmDetector.PrintableDurations.Contains(remaining))
                                    {
                                        Console.WriteLine("  WARNING: Cannot split slide! Leaving incomplete as is...");
                                        continue;
                                    }
                                    var newChord = SplitChord(chord, bar.GetDurationLength(chord.Start, duration));
                                    foreach (var kvp2 in newChord.Notes)
                                    {
                                        if (kvp2.Value.Slide == Note.SlideType.ToNext)
                                        {
                                            kvp2.Value.Fret = kvp2.Value.SlideTarget;
                                            var prevNote = chord.Notes[kvp2.Key];
                                            prevNote.Slide = Note.SlideType.ToNext;
                                            prevNote.SlideTarget = kvp2.Value.Fret;
                                            kvp2.Value.Slide = Note.SlideType.None;
                                            kvp2.Value.SlideTarget = -1;
                                        }
                                    }
                                    newChord.Duration = remaining;
                                    chord.Duration = duration;
                                    bar.Chords.Insert(i + 1, newChord);
                                    nextChord = newChord;
                                }
                            }
                        }
                    }
                    lastChord = chord;
                }
            }
        }

        static void CalculateAndCleanBendOffsets(List<Bar> bars)
        {
            // So far, we only have absolute time positions for bend steps.
            // We'll convert them to a relative position referencing the
            // current note's length. We can easily do this by just comparing
            // the time offset with the note's sustain.
            // Also, we will remove any pointless bend point that may have been
            // inserted in the Rocksmith arrangement or during our conversion
            // process, but serves no real purpose.
            // Finally, where necessary we will insert bend points at the beginning
            // and end of a note, because Guitar Pro 5 expects them to be there.
            foreach (var bar in bars)
            {
                foreach (var chord in bar.Chords)
                {
                    foreach (var kvp in chord.Notes)
                    {
                        var note = kvp.Value;
                        bool activeBend = false;
                        for (int i = 0; i < note.BendValues.Count; )
                        {
                            var bend = note.BendValues[i];
                            float distance = bend.Start - chord.Start;
                            bend.RelativePosition = distance / note.Sustain;
                            // ensure that the position is between 0 and 1
                            if (bend.RelativePosition < 0)
                                bend.RelativePosition = 0;
                            else if (bend.RelativePosition > 1 || note.Sustain == 0)
                                bend.RelativePosition = 1;
                            if (bend.Step < 0)
                                bend.Step = 0;
                            if (bend.Step > 0.05)
                                activeBend = true;

                            // sort out duplicate time values
                            if (i > 0 && Math.Abs(bend.RelativePosition - note.BendValues[i - 1].RelativePosition) < 0.005)
                                note.BendValues.RemoveAt(i - 1);
                            else
                                ++i;
                        }
                        if (activeBend)
                        {
                            // We need to ensure that there is a bend point at the beginning
                            // and end of a note. If there isn't one, insert it with the right 
                            // value.
                            var firstBend = note.BendValues.First();
                            if (firstBend.RelativePosition <= 0.005)
                            {
                                // close enough to the beginning of the note
                                firstBend.RelativePosition = 0;
                            }
                            else
                            {
                                // insert a new bend point at the beginning with step 0.
                                note.BendValues.Insert(0, new Note.BendValue()
                                {
                                    RelativePosition = 0,
                                    Step = 0,
                                });
                            }

                            var lastBend = note.BendValues.Last();
                            if (lastBend.RelativePosition >= 0.995)
                            {
                                // close enough to the end of the note
                                lastBend.RelativePosition = 1;
                            }
                            else
                            {
                                // insert a new bend point at the end, repeating the last step value.
                                note.BendValues.Add(new Note.BendValue()
                                {
                                    RelativePosition = 1,
                                    Step = lastBend.Step
                                });
                            }

                            // find and clean up any unnecessary bend points
                            for (int i = 1; i < note.BendValues.Count - 1; )
                            {
                                if (note.BendValues[i].Step == note.BendValues[i-1].Step &&
                                    note.BendValues[i].Step == note.BendValues[i+1].Step)
                                {
                                    note.BendValues.RemoveAt(i);
                                }
                                else
                                {
                                    ++i;
                                }
                            }

                        }
                        else
                        {
                            // no bend step actually bends the note to any measurable amount,
                            // so just delete all the points.
                            note.BendValues.Clear();
                        }
                    }
                }
            }
        }



    }
}

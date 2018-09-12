using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;
using RocksmithToolkitLib.XML;
using System.ComponentModel;

namespace RocksmithToolkitLib.Sng
{
    public enum ArrangementName { Lead = 0/* Single notes */, Rhythm /* Chords */, Combo /* Combo */, Bass, Vocals, JVocals, ShowLights };
    public enum ArrangementType { Guitar, Bass, Vocal, ShowLight };
    public enum InstrumentTuning { [Description("E Standard")] Standard, [Description("Drop D")] DropD, [Description("Eb")] EFlat, [Description("Open G")] OpenG };
    public enum PluckedType { NotPicked = 0, Picked = 1 };
    public enum Metronome { None, Itself, Generate };

    public static class InstrumentTuningExtensions
    {

        private static readonly Int16[] StandardOffsets = { 0, 0, 0, 0, 0, 0 };
        private static readonly Int16[] DropDOffsets = { -2, 0, 0, 0, 0, 0 };
        private static readonly Int16[] EFlatOffsets = { -1, -1, -1, -1, -1, -1 };
        private static readonly Int16[] OpenGOffsets = { -2, -2, 0, 0, 0, -2 };
        private static readonly Int16[] StandardMidiNotes = { 40, 45, 50, 55, 59, 64 };

        public static IList<Int16> GetOffsets(this InstrumentTuning tuning)
        {
            switch (tuning)
            {
                case InstrumentTuning.Standard:
                    return StandardOffsets;
                case InstrumentTuning.DropD:
                    return DropDOffsets;
                case InstrumentTuning.EFlat:
                    return EFlatOffsets;
                case InstrumentTuning.OpenG:
                    return OpenGOffsets;
                default:
                    throw new InvalidOperationException("Unexpected tuning value");
            }
        }

        public static InstrumentTuning GetTuningByOffsets(Int16[] strings)
        {
            if (strings.SequenceEqual(DropDOffsets))
                return InstrumentTuning.DropD;
            if (strings.SequenceEqual(EFlatOffsets))
                return InstrumentTuning.EFlat;
            if (strings.SequenceEqual(OpenGOffsets))
                return InstrumentTuning.OpenG;
            return InstrumentTuning.Standard;
        }

        public static int GetMidiNote(this InstrumentTuning tuning, ArrangementType arrangementType, int stringNumber, int fret)
        {
            if (fret == -1) return -1;
            var strNote = StandardMidiNotes[stringNumber] + tuning.GetOffsets()[stringNumber];
            strNote -= arrangementType == ArrangementType.Bass ? 12 : 0;

            return strNote + fret;
        }
    };

    public struct TimeLinkedEntity
    {
        public Single Time { get; set; }
        public Object Entity { get; set; }
    }

    public static class SngFileWriter
    {
        public static void Write(string inputFile, string outputFile, ArrangementType arrangementType, Platform platform)
        {
            using (var reader = new StreamReader(inputFile))
            {
                var bitConverter = platform.GetBitConverter;
                if (arrangementType == ArrangementType.Vocal)
                {
                    var serializer = new XmlSerializer(typeof(Vocals));
                    var vocals = (Vocals)serializer.Deserialize(reader);
                    WriteRocksmithVocalsFile(vocals, outputFile, bitConverter);
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(Song));
                    var song = (Song)serializer.Deserialize(reader);

                    // TODO: song.Tuning is null in toolkit generated RS1 Xml files
                    // HACK: this is only a quick fix of the root problem
                    var tuning = new Int16[] { 0, 0, 0, 0, 0, 0 };
                    if (song.Tuning != null) 
                        tuning = song.Tuning.ToArray();

                    WriteRocksmithSngFile(song, InstrumentTuningExtensions.GetTuningByOffsets(tuning), arrangementType, outputFile, bitConverter);

                }
            }
        }

        // COMPLETE
        private static void WriteRocksmithVocalsFile(Vocals vocals, string outputFile, EndianBitConverter bitConverter)
        {
            // WRITE THE .SNG FILE
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            using (EndianBinaryWriter w = new EndianBinaryWriter(bitConverter, fs))
            {
                // file header
                WriteRocksmithSngHeader(w, ArrangementType.Vocal);

                // unused filler
                w.Write(new byte[16]);

                // vocal count
                if (vocals.Count != vocals.Vocal.Length)
                    throw new InvalidDataException("XML vocals header count does not match number of vocal items.");
                w.Write(vocals.Count);

                // vocals
                for (int i = 0; i < vocals.Vocal.Length; i++)
                {
                    // vocal time
                    w.Write(vocals.Vocal[i].Time);

                    // vocal note
                    w.Write(vocals.Vocal[i].Note);

                    // vocal length
                    w.Write(vocals.Vocal[i].Length);

                    // vocal lyric
                    string lyric = vocals.Vocal[i].Lyric;
                    if (lyric.Length > 32)
                        throw new InvalidDataException(string.Format("Vocal lyric '{0}' at position {1} exceeded the maximum width of 32 bytes.", lyric, i));
                    foreach (char c in lyric)
                    {
                        w.Write(Convert.ToByte(c));
                    }
                    // padding after name
                    w.Write(new byte[32 - lyric.Length]);
                }

                // unused
                w.Write(new byte[254]);
            }
        }

        // COMPLETE
        private static void WriteRocksmithSngFile(Song rocksmithSong, InstrumentTuning tuning, ArrangementType arrangementType, string outputFile, EndianBitConverter bitConverter)
        {
            var iterationInfo = CreatePhraseIterationInfo(rocksmithSong);
            // WRITE THE .SNG FILE
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            using (EndianBinaryWriter w = new EndianBinaryWriter(bitConverter, fs))
            {
                // HEADER
                WriteRocksmithSngHeader(w, arrangementType);

                // EBEATS DATA
                WriteRocksmithSngEbeats(w, rocksmithSong.Ebeats);

                // PHRASES
                WriteRocksmithSngPhrases(w, rocksmithSong.Phrases, rocksmithSong.PhraseIterations);

                // CHORD TEMPLATES
                WriteRocksmithSngChordTemplates(w, rocksmithSong.ChordTemplates, tuning, arrangementType);

                // FRET HAND MUTE TEMPLATE
                WriteRocksmithSngFretHandMuteTemplates(w, rocksmithSong.FretHandMuteTemplates);

                // VOCALS TEMPLATE 
                w.Write(new byte[4]); // not used on song file

                // PHRASE ITERATIONS
                WriteRocksmithSngPhraseIterations(w, rocksmithSong.PhraseIterations, rocksmithSong.SongLength);

                // PHRASE PROPERTIES
                WriteRocksmithSngPhraseProperties(w, rocksmithSong.PhraseProperties);

                // LINKED DIFFS
                WriteRocksmithSngLinkedDiffs(w, rocksmithSong.LinkedDiffs);

                // CONTROLS
                WriteRocksmithSngControls(w, rocksmithSong.Controls);

                // EVENTS
                WriteRocksmithSngEvents(w, rocksmithSong.Events);

                // SECTIONS
                WriteRocksmithSngSections(w, rocksmithSong.Sections, rocksmithSong.PhraseIterations, rocksmithSong.SongLength);

                // LEVELS
                WriteRocksmithSngLevels(w, rocksmithSong.Levels, rocksmithSong.SongLength, iterationInfo, arrangementType);

                // SONG META DATA
                WriteRocksmithSngMetaDetails(w, rocksmithSong, tuning, iterationInfo);
            }
        }

        private static List<PhraseIterationInfo> CreatePhraseIterationInfo(Song rocksmithSong)
        {
            List<PhraseIterationInfo> info = new List<PhraseIterationInfo>(rocksmithSong.PhraseIterations.Length);
            for (int i = 0; i < rocksmithSong.PhraseIterations.Length; i++)
            {
                var iteration = rocksmithSong.PhraseIterations[i];
                var phrase = rocksmithSong.Phrases[iteration.PhraseId];
                var endTime = rocksmithSong.SongLength;
                if (i + 1 < rocksmithSong.PhraseIterations.Length)
                {
                    endTime = rocksmithSong.PhraseIterations[i + 1].Time;
                }
                info.Add(new PhraseIterationInfo
                {
                    IterationId = i,
                    PhraseId = iteration.PhraseId,
                    MaxDifficulty = phrase.MaxDifficulty,
                    StartTime = iteration.Time,
                    EndTime = endTime
                });
            }
            return info;
        }

        // COMPLETE
        private static void WriteRocksmithSngHeader(EndianBinaryWriter w, ArrangementType arrangementType)
        {
            w.Write(arrangementType == ArrangementType.Bass ? 51 : 49); // version num?
        }

        // COMPLETE
        private static void WriteRocksmithSngEbeats(EndianBinaryWriter w, SongEbeat[] ebeats)
        {
            // output header
            if (ebeats == null || ebeats.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(ebeats.Length);

            Int16 currentMeasure = -1;
            Int16 currentMeasureBeat = -1;

            // output ebeats
            foreach (SongEbeat ebeat in ebeats)
            {
                // ebeat time
                w.Write(ebeat.Time);

                // measure
                Int16 measure = Convert.ToInt16(ebeat.Measure);
                if (measure >= 0)
                {
                    currentMeasure = measure;
                    currentMeasureBeat = 0;

                    w.Write(currentMeasure);
                    w.Write(currentMeasureBeat);
                    w.Write(true);
                    w.Write(false);
                    w.Write(false);
                    w.Write(false);
                }
                else if (measure == -1)
                {
                    currentMeasureBeat++;

                    w.Write(currentMeasure);
                    w.Write(currentMeasureBeat);
                    w.Write(false);
                    w.Write(false);
                    w.Write(false);
                    w.Write(false);
                }
            }
        }

        // COMPLETE
        private static void WriteRocksmithSngPhrases(EndianBinaryWriter w, SongPhrase[] phrases, SongPhraseIteration[] phraseIterations)
        {
            // Sample: begins at position 7,208 in NumberThirteen_Lead.sng

            // output header
            if (phrases == null || phrases.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phrases.Length);

            // output phrases
            for (int i = 0; i < phrases.Length; i++)
            {
                // solo
                w.Write(phrases[i].Solo == 1);

                // disparity
                w.Write(phrases[i].Disparity == 1);

                // ignore
                w.Write(phrases[i].Ignore == 1);

                // unused padding
                w.Write(new byte());

                // maxDifficulty tag
                w.Write(phrases[i].MaxDifficulty);

                // count of usage in iterations
                int phraseIterationCount = 0;
                for (int i2 = 0; i2 < phraseIterations.Length; i2++)
                {
                    if (phraseIterations[i2].PhraseId == i)
                    {
                        phraseIterationCount++;
                    }
                }
                w.Write(phraseIterationCount);

                // name tag
                string name = phrases[i].Name;
                if (name.Length > 32)
                {
                    name = name.Substring(0, 32);
                }
                foreach (char c in name)
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after name
                w.Write(new byte[32 - name.Length]);
            }
        }

        // COMPLETE
        private static void WriteRocksmithSngChordTemplates(EndianBinaryWriter w, SongChordTemplate[] chordTemplates, InstrumentTuning tuning, ArrangementType arrangementType)
        {
            // output header
            if (chordTemplates == null || chordTemplates.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(chordTemplates.Length);

            // output chord templates
            foreach (SongChordTemplate chordTemplate in chordTemplates)
            {
                // fret numbers
                w.Write(chordTemplate.Fret0);
                w.Write(chordTemplate.Fret1);
                w.Write(chordTemplate.Fret2);
                w.Write(chordTemplate.Fret3);
                w.Write(chordTemplate.Fret4);
                w.Write(chordTemplate.Fret5);

                // finger positions
                w.Write(chordTemplate.Finger0);
                w.Write(chordTemplate.Finger1);
                w.Write(chordTemplate.Finger2);
                w.Write(chordTemplate.Finger3);
                w.Write(chordTemplate.Finger4);
                w.Write(chordTemplate.Finger5);

                // note values
                w.Write(tuning.GetMidiNote(arrangementType, 0, chordTemplate.Fret0));
                w.Write(tuning.GetMidiNote(arrangementType, 1, chordTemplate.Fret1));
                w.Write(tuning.GetMidiNote(arrangementType, 2, chordTemplate.Fret2));
                w.Write(tuning.GetMidiNote(arrangementType, 3, chordTemplate.Fret3));
                w.Write(tuning.GetMidiNote(arrangementType, 4, chordTemplate.Fret4));
                w.Write(tuning.GetMidiNote(arrangementType, 5, chordTemplate.Fret5));

                // chord name
                string name = chordTemplate.ChordName;
                if (name.Length > 32)
                {
                    name = name.Substring(0, 32);
                }
                foreach (char c in name)
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after name
                w.Write(new byte[32 - name.Length]);
            }
        }

        // NO EXAMPLES IN ROCKSMITH?
        private static void WriteRocksmithSngFretHandMuteTemplates(EndianBinaryWriter w, SongFretHandMuteTemplate[] fretHandMuteTemplates)
        {
            w.Write(new byte[4]); // placeholder
        }

        // COMPLETE
        private static void WriteRocksmithSngPhraseIterations(EndianBinaryWriter w, SongPhraseIteration[] phraseIterations, Single songLength)
        {
            // Sample: begins at position 7,664 in NumberThirteen_Lead.sng

            // output header
            if (phraseIterations == null || phraseIterations.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phraseIterations.Length);

            // output phrase iterations
            for (int i = 0; i < phraseIterations.Length; i++)
            {
                // phrase id
                w.Write(phraseIterations[i].PhraseId);
                // start time
                w.Write(phraseIterations[i].Time);
                // end time
                var endTime = i == phraseIterations.Length - 1
                    ? songLength
                    : phraseIterations[i + 1].Time;
                w.Write(endTime);
            }
        }

        // INCOMPLETE - review Angela_Combo.xml for some inconsistencies
        private static void WriteRocksmithSngPhraseProperties(EndianBinaryWriter w, SongPhraseProperty[] phraseProperties)
        {
            // output header
            if (phraseProperties == null || phraseProperties.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phraseProperties.Length);

            // output phrase properties
            foreach (SongPhraseProperty phraseProperty in phraseProperties)
            {
                // phrase id
                w.Write(phraseProperty.PhraseId);

                // difficulty
                w.Write(phraseProperty.Difficulty);

                // empty?
                w.Write(phraseProperty.Empty);

                // These seem to be 1 in many RS SNGs where the XML shows 0.
                // level jump?
                w.Write(phraseProperty.LevelJump);

                // redundant
                w.Write(phraseProperty.Redundant);
            }
        }

        // COMPLETE - except hardcoded field
        private static void WriteRocksmithSngLinkedDiffs(EndianBinaryWriter w, SongLinkedDiff[] linkedDiffs)
        {
            // output header
            if (linkedDiffs == null || linkedDiffs.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(linkedDiffs.Length);

            foreach (SongLinkedDiff linkedDiff in linkedDiffs)
            {
                // parent id
                w.Write(linkedDiff.ParentId);

                // child id
                w.Write(linkedDiff.ChildId);

                // unknown (first byte seems to be a boolean bit with true value in all cases reviewed)
                w.Write(true);
                w.Write(false);
                w.Write(false);
                w.Write(false);
            }
        }

        // COMPLETE
        private static void WriteRocksmithSngControls(EndianBinaryWriter w, SongControl[] controls)
        {
            // output header
            if (controls == null || controls.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(controls.Length);

            // output controls
            foreach (var songControl in controls)
            {
                // control time
                w.Write(songControl.Time);

                // control code
                string code = songControl.Code;
                if (code.Length > 256)
                {
                    code = code.Substring(0, 256);
                }
                foreach (char c in code)
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after control code
                w.Write(new byte[256 - code.Length]);
            }
        }

        // COMPLETE
        private static void WriteRocksmithSngEvents(EndianBinaryWriter w, XML.SongEvent[] events)
        {
            // Sample: begins at position 8,172 in NumberThirteen_Lead.sng

            // output header
            if (events == null || events.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(events.Length);

            // output events
            foreach (var songEvent in events)
            {
                // event time
                w.Write(songEvent.Time);
                // event code
                string eventCode = songEvent.Code;
                if (eventCode.Length > 256)
                {
                    eventCode = eventCode.Substring(0, 256);
                }
                foreach (char c in eventCode)
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after event code
                w.Write(new byte[256 - eventCode.Length]);
            }
        }

        // COMPLETE except hardcoded fields
        // Sample: begins at position 9,216 in NumberThirteen_Lead.sng
        private static void WriteRocksmithSngSections(EndianBinaryWriter w, XML.SongSection[] sections, SongPhraseIteration[] phraseIterations, Single songLength)
        {
            // output header
            if (sections == null || sections.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            // output header count
            w.Write(sections.Length);

            // output sections
            for (int i = 0; i < sections.Length; i++)
            {
                // section name
                string name = sections[i].Name;
                if (name.Length > 32)
                {
                    name = name.Substring(0, 32);
                }
                foreach (char c in name)
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after section name
                w.Write(new byte[32 - name.Length]);

                // number tag
                w.Write(sections[i].Number);

                // start time
                w.Write(sections[i].StartTime);

                // end time
                var endTime = i == sections.Length - 1
                    ? songLength
                    : sections[i + 1].StartTime;
                w.Write(endTime);

                // phrase iteration start index
                bool phraseIterationFound = false;
                for (int p = 0; p < phraseIterations.Length; p++)
                {
                    if (sections[i].StartTime <= phraseIterations[p].Time)
                    {
                        w.Write(p);
                        phraseIterationFound = true;
                        break;
                    }
                }
                if (!phraseIterationFound)
                    throw new Exception(string.Format("No phrase iteration found with matching time for section {0}.", i.ToString()));

                // phrase iteration end index
                if (i == sections.Length - 1) // if last section, default to last phrase iteration
                {
                    w.Write(phraseIterations.Length - 1);
                }
                else
                {
                    //bool endPhraseIterationFound = false;
                    for (int p = 0; p < phraseIterations.Length; p++)
                    {
                        if (sections[i + 1].StartTime <= phraseIterations[p].Time)
                        {
                            w.Write(Convert.ToInt32(p - 1));
                            //endPhraseIterationFound = true;
                            break;
                        }
                    }
                    //if (!endPhraseIterationFound)
                    //    throw new Exception(string.Format("No end phrase iteration found with matching time for section {0}.", i.ToString()));
                }

                // HACK: series of 8 unknown bytes (look like flags)? below logic is wrong, just defaulting for now
                w.Write(true);
                w.Write(true);
                w.Write(true);
                w.Write(true);
                w.Write(true);
                w.Write(false);
                w.Write(false);
                w.Write(false);
            }
        }

        // INCOMPLETE 
        // section begins at @ 9,820 in NumberThirteen_Lead.sng
        private static void WriteRocksmithSngLevels(EndianBinaryWriter w, XML.SongLevel[] levels, Single songLength, List<PhraseIterationInfo> iterationInfo, ArrangementType arrangementType)
        {
            // output header
            if (levels == null || levels.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(levels.Length);

            // output
            foreach (var level in levels)
            {
                // level difficulty tag
                w.Write(level.Difficulty);

                // anchors
                WriteRocksmithSngLevelAnchors(w, level.Anchors, level, iterationInfo, songLength);

                // slide properties
                WriteRockmithSngLevelSlideProperties(w, level.Notes);

                // handshapes
                WriteRocksmithSngLevelHandShapes(w, level.HandShapes, level, songLength);

                // notes and chords
                WriteRocksmithSngLevelNotes(w, iterationInfo, level.Notes, level.Chords, songLength, arrangementType);

                var iterationNotes = new List<int>();
                foreach (var iteration in iterationInfo)
                {
                    int num = 0;
                    if (level.Notes != null)
                    {
                        num += level.Notes.Count(n => n.Time >= iteration.StartTime && n.Time < iteration.EndTime);
                    }
                    if (level.Chords != null)
                    {
                        num += level.Chords.Count(n => n.Time >= iteration.StartTime && n.Time < iteration.EndTime);
                    }
                    iterationNotes.Add(num);
                }

                var phrases = iterationInfo.GroupBy(it => it.PhraseId).OrderBy(grp => grp.Key);
                // count of phrases
                w.Write(phrases.Count());
                foreach (var phrase in phrases)
                {
                    float notes = (float)phrase.Sum(iteration => iterationNotes[iteration.IterationId]);
                    float count = phrase.Count();

                    w.Write(notes / count); // This is the number of notes + chords in all iterations of this phrase for this level, divided by number of iterations of this phrase
                }

                // count of phrase iterations
                w.Write(iterationInfo.Count);
                foreach (var iterationNoteCount in iterationNotes)
                {
                    w.Write(iterationNoteCount);// This appears to be the number of notes + chords in each iteration for this level
                }

                // count of phrase iterations
                w.Write(iterationInfo.Count);
                foreach (var iterationNoteCount in iterationNotes)
                {
                    w.Write(iterationNoteCount);// This appears to be the number of notes + chords in each iteration for this level
                }
            }
        }

        // COMPLETE 
        private static void WriteRocksmithSngLevelAnchors(EndianBinaryWriter w, SongAnchor[] anchors, XML.SongLevel level, List<PhraseIterationInfo> iterationInfo, Single songLength)
        {
            if (anchors == null || anchors.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output anchors header count
            w.Write(anchors.Length);

            // output anchors
            for (int i = 0; i < anchors.Length; i++)
            {
                // anchor start time
                var startTime = anchors[i].Time;
                w.Write(startTime);

                // anchor end time
                var endTime = i == anchors.Length - 1
                    ? songLength
                    : anchors[i + 1].Time;
                w.Write(endTime);

                float? lastNote = null, lastChord = null;
                var notes = (level.Notes == null) ? null : level.Notes.Where(note => note.Time >= startTime && note.Time < endTime);
                if (notes != null && notes.Any())
                {
                    var note = notes.OrderByDescending(n => n.Time).First();
                    lastNote = note.Time + note.Sustain + .1f;
                }
                var chords = (level.Chords == null) ? null : level.Chords.Where(chord => chord.Time >= startTime && chord.Time < endTime);
                if (chords != null && chords.Any())
                {
                    lastChord = chords.Max(chord => chord.Time) + .1f;
                }
                float lastTime = lastNote == null && lastChord == null ? endTime : Math.Max(lastNote ?? startTime, lastChord ?? startTime);
                w.Write(Math.Min(lastTime, endTime));

                // fret
                w.Write(anchors[i].Fret);

                // phrase iteration index
                bool phraseIterationFound = false;
                foreach (var iteration in iterationInfo)
                {
                    if (anchors[i].Time >= iteration.StartTime &&
                        anchors[i].Time < iteration.EndTime)
                    {
                        w.Write(iteration.IterationId);
                        phraseIterationFound = true;
                        break;
                    }
                }
                if (!phraseIterationFound)
                {
                    w.Write(iterationInfo.Count - 1);
                }
            }
        }

        // COMPLETE
        private static void WriteRockmithSngLevelSlideProperties(EndianBinaryWriter w, SongNote[] notes)
        {
            if (notes == null || notes.Length == 0)
            {
                w.Write(new byte[4]);
                return;
            }

            // count of number of slides in level
            w.Write(notes.Count(x => x.SlideTo > -1));

            foreach (SongNote note in notes)
            {
                if (note.SlideTo > -1)
                {
                    // slide end time
                    if (note.Sustain > 0)
                    {
                        w.Write(note.Time + note.Sustain);
                    }
                    else
                    { // default sustain if user forgets it
                        w.Write(note.Time + (float)0.125);
                    }

                    // slide end fret
                    w.Write(note.SlideTo);
                }
            }
        }

        // INCOMPLETE
        private static void WriteRocksmithSngLevelHandShapes(EndianBinaryWriter w, SongHandShape[] handShapes, XML.SongLevel level, float songLength)
        {
            // sample section begins @ 328,356 in NumberThirteen_Combo.sng
            // sample section begins @ 4,300 in TCPowerChords_Lead.sng   

            if (handShapes == null || handShapes.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            // output notes header count
            w.Write(handShapes.Length);

            // output handshapes
            for (int i = 0; i < handShapes.Length; i++)
            {
                SongHandShape handShape = handShapes[i];
                // hand shape start time
                w.Write(handShape.StartTime);

                // hand shape end time
                w.Write(handShape.EndTime);

                // unknown
                w.Write(Convert.ToSingle(-1));

                // unknown
                w.Write(Convert.ToSingle(-1));

                // chord id
                w.Write(handShape.ChordId);

                // chord start time again ???
                // Probably the first chord in the shape.
                w.Write(handShape.StartTime);

                var endTime = handShape.EndTime;
                //var endTime = i == handShapes.Length - 1
                //     ? songLength
                //     : handShapes[i + 1].StartTime;

                // This should actually be the time of the last chord before the end time.
                float? lastChord = null;
                var chords = level.Chords == null ? null : level.Chords.Where(chord => chord.Time >= handShape.StartTime && chord.Time < endTime);
                var note = level.Notes == null ? null : level.Notes.FirstOrDefault(n => n.Time == endTime);
                if (note != null)
                {
                    lastChord = endTime;
                }
                else if (chords != null && chords.Any())
                {
                    lastChord = chords.Max(chord => chord.Time);
                }

                w.Write(lastChord ?? endTime);
            }
        }

        // COMPLETE except hardcoded fields
        private static void WriteRocksmithSngLevelNotes(EndianBinaryWriter w, List<PhraseIterationInfo> iterationInfo, SongNote[] notes, SongChord[] chords, Single songLength, ArrangementType arrangementType)
        {
            List<TimeLinkedEntity> notesChords = new List<TimeLinkedEntity>();

            // add notes to combined note/chord array
            if (notes != null && notes.Length != 0)
            {
                notesChords.AddRange(notes.Select(note =>
                    new TimeLinkedEntity
                    {
                        Time = note.Time,
                        Entity = note
                    }));
            }

            // add chords to combined note/chord array
            if (chords != null && chords.Length != 0)
            {
                notesChords.AddRange(chords.Select(chord =>
                    new TimeLinkedEntity
                    {
                        Time = chord.Time,
                        Entity = chord
                    }));
            }

            // sort the notes and chords by time
            notesChords.Sort((s1, s2) => s1.Time.CompareTo(s2.Time));

            // write empty header if no notes or chords
            if (notesChords.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output notes and chords header count
            w.Write(notesChords.Count);

            // ouput notes and chords
            for (int i = 0; i < (notesChords.Count); i++)
            {
                // note time tag
                w.Write(notesChords[i].Time);

                // string tag
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).String : -1);

                // fret tag
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Fret : (sbyte)-1);

                // chord id
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? -1 : ((SongChord)notesChords[i].Entity).ChordId);

                // unknown
                w.Write(Convert.ToInt32(-1));

                // sustain time
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Sustain : 0);

                // bend
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Bend : (byte)0);

                // slideTo
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).SlideTo : (sbyte)-1);

                // tremolo
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Tremolo : new byte());

                // harmonic
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Harmonic : new byte());

                // palm mute
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).PalmMute : new byte());

                if (arrangementType == ArrangementType.Bass)
                {
                    w.Write(new byte());//unknownB

                    //Bass only - Slap
                    w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Slap : (sbyte)-1);

                    //Bass only - Pluck
                    w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Pluck : (sbyte)-1);
                }

                // hopo
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Hopo : new byte());

                // hammerOn
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).HammerOn : new byte());

                // pullOff
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).PullOff : new byte());

                // ignore
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Ignore : ((SongChord)notesChords[i].Entity).Ignore);

                // high density chord
                if (arrangementType == ArrangementType.Bass)
                {
                    w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? new byte() : (byte)((SongChord)notesChords[i].Entity).HighDensity);
                    w.Write(new byte());
                    w.Write((byte)140);
                    w.Write(new byte());
                }
                else
                {
                    w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? new byte() : ((SongChord)notesChords[i].Entity).HighDensity);
                    w.Write(new byte[4]);
                }

                //w.Write(Convert.ToInt16(246));
                //w.Write(Convert.ToInt16(7472));

                // phrase iteration start index and id ????
                bool phraseStartIterationFound = false;
                foreach (var iteration in iterationInfo)
                {
                    if (notesChords[i].Time >= iteration.StartTime && notesChords[i].Time < iteration.EndTime)
                    {
                        w.Write(iteration.IterationId); // phrase iteration
                        w.Write(iteration.PhraseId);
                        phraseStartIterationFound = true;
                        break;
                    }
                }
                if (!phraseStartIterationFound)
                {
                    throw new Exception(string.Format("No phrase start iteration found with matching time for note {0}.", i.ToString()));
                }
            }
        }

        // INCOMPLETE
        private static void WriteRocksmithSngMetaDetails(EndianBinaryWriter w, Song s, InstrumentTuning tuning, List<PhraseIterationInfo> iterationInfo)
        {
            // Max score when fully leveled (minus bonuses - always 100000).  Or possible Master mode unlock score
            w.Write((double)100000);

            //This needs to be calculated to take real charts into account.
            double totalNotes = 0;
            foreach (var iteration in iterationInfo)
            {
                if (s.Levels.Length <= iteration.MaxDifficulty)
                {
                    throw new Exception("There is a phrase defined with maxDifficulty=" + iteration.MaxDifficulty + ", but the highest difficulty level is " + (s.Levels.Length - 1));
                }
                var level = s.Levels[iteration.MaxDifficulty];
                if (level.Notes != null)
                {
                    totalNotes += level.Notes.Count(note => note.Time >= iteration.StartTime && note.Time < iteration.EndTime);
                }
                if (level.Chords != null)
                {
                    totalNotes += level.Chords.Count(chord => chord.Time >= iteration.StartTime && chord.Time < iteration.EndTime);
                }
            }
            // Total notes when fully leveled
            w.Write(totalNotes);

            // points per note
            w.Write((double)((float)(100000f / (float)totalNotes)));

            // song beat timing
            if (s.Ebeats.Length < 2)
                throw new InvalidDataException("Song must contain at least 2 beats");

            // this is not 100% accurate unless all beats are evenly spaced in a song;
            // still trying to determine exactly how Rocksmith is deriving this time value
            w.Write(s.Ebeats[1].Time - s.Ebeats[0].Time);

            // first beat time(?); confirmed as not first phraseIteration time and not first section time
            w.Write(s.Ebeats[0].Time);

            // song conversion date
            var lastConvertDate = s.LastConversionDateTime;
            if (lastConvertDate.Length > 32)
            {
                lastConvertDate = lastConvertDate.Substring(0, 32);
            }
            foreach (char c in lastConvertDate)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - lastConvertDate.Length]); //pad to 32 bytes

            // song title
            var title = s.Title;
            if (title.Length > 64)
            {
                title = title.Substring(0, 64);
            }
            foreach (char c in title)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[64 - title.Length]); // pad to 64 bytes

            // arrangement
            var arrangement = s.Arrangement;
            if (arrangement.Length > 32)
            {
                arrangement = arrangement.Substring(0, 32);
            }
            foreach (char c in arrangement)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - arrangement.Length]); //pad to 32 bytes

            // artist
            string artistValue = string.IsNullOrEmpty(s.ArtistName) ? "DUMMY" : s.ArtistName;
            if (artistValue.Length > 32)
            {
                artistValue = artistValue.Substring(0, 32);
            }
            foreach (char c in artistValue)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - artistValue.Length]); //pad to 32 bytes

            // song part
            w.Write(s.Part);

            // song length
            w.Write(s.SongLength);

            // tuning
            w.Write((Int32)tuning);

            //Song difficulty
            float difficulty = (float)iterationInfo.Average(it => it.MaxDifficulty);
            w.Write(difficulty); // float with 10.2927 in NumberThirteen_Lead.sng

            // unknown
            // from 6AMSalvation_Combo.xml value = 11.525
            // which does not match any phrase iteration start time
            // and does not match first ebeat time
            // and does not match any section start time
            // does match first event time of E3 (although this is not a match in other files)
            // does not match any note times on 1st difficulty level
            // nor an anchor time on 1st difficulty level

            //It appears to be the time of the first note.
            float firstNote = s.Levels.Min(level => level.Notes == null || level.Notes.Length == 0 ? float.MaxValue : level.Notes.Min(note => note.Time));
            float firstChord = s.Levels.Min(level => level.Chords == null || level.Chords.Length == 0 ? float.MaxValue : level.Chords.Min(chord => chord.Time));
            float first = Math.Min(firstChord, firstNote);
            w.Write(first);

            w.Write(first);

            // max difficulty
            int maxDifficulty = s.Levels.Length - 1;
            w.Write(maxDifficulty);

            // unknown section
            w.Write(new byte[4]); // header with repeating array; song works in game if array is defaulted to 0 count so will leave this alone for now

            // unknown section
            // There seems to be 1 entry per letter in the chord templates, although there are songs with chord templates that don't have this section.
            w.Write(new byte[4]); // header with repeating array - only populated in limited songs
        }
    }
}


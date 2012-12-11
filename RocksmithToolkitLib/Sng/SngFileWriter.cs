using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.Sng
{
    public enum GamePlatform { Pc, Console };
    public enum ArrangementType { Instrument, Vocal }
    public enum InstrumentTuning { Standard, DropD, EFlat, OpenG }
    public static class InstrumentTuningExtensions {
        private static readonly int[] StandardOffsets = { 0, 0, 0, 0, 0, 0 };
        private static readonly int[] DropDOffsets = { -2, 0, 0, 0, 0, 0 };
        private static readonly int[] EFlatOffsets = { -1, -1, -1, -1, -1, -1 };
        private static readonly int[] OpenGOffsets = { -2, -2, 0, 0, 0, -2 };
        private static readonly int[] StandardMidiNotes = { 40, 45, 50, 55, 59, 64 };
        public static IList<int> GetOffsets(this InstrumentTuning tuning)
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
        public static int GetMidiNote(this InstrumentTuning tuning, int stringNumber, int fret)
        {
            if (fret == -1) return -1;
            return StandardMidiNotes[stringNumber] + tuning.GetOffsets()[stringNumber] + fret;
        }
    };

    public struct TimeLinkedEntity
    {
        public Single Time { get; set; }
        public Object Entity { get; set; }
    }
    
    public static class SngFileWriter
    {
        public static void Write(string inputFile, string outputFile, ArrangementType arrangementType, GamePlatform platform, InstrumentTuning tuning)
        {
            using (var reader = new StreamReader(inputFile))
            {
                var bitConverter = platform == GamePlatform.Pc
                    ? (EndianBitConverter)EndianBitConverter.Little
                    : (EndianBitConverter)EndianBitConverter.Big;

                if (arrangementType == ArrangementType.Instrument)
                {
                    var serializer = new XmlSerializer(typeof(Song));
                    var song = (Song)serializer.Deserialize(reader);
                    WriteRocksmithSngFile(song, tuning, outputFile, bitConverter);
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(Vocals));
                    var vocals = (Vocals)serializer.Deserialize(reader);
                    WriteRocksmithVocalsFile(vocals, outputFile, bitConverter);
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
                WriteRocksmithSngHeader(w);

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
        private static void WriteRocksmithSngFile(Song rocksmithSong, InstrumentTuning tuning, string outputFile, EndianBitConverter bitConverter)
        {
            var iterationInfo = CreatePhraseIterationInfo(rocksmithSong);
            // WRITE THE .SNG FILE
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            using (EndianBinaryWriter w = new EndianBinaryWriter(bitConverter, fs))
            {
                // HEADER
                WriteRocksmithSngHeader(w);

                // EBEATS DATA
                WriteRocksmithSngEbeats(w, rocksmithSong.Ebeats);

                // PHRASES
                WriteRocksmithSngPhrases(w, rocksmithSong.Phrases, rocksmithSong.PhraseIterations);

                // CHORD TEMPLATES
                WriteRocksmithSngChordTemplates(w, rocksmithSong.ChordTemplates, tuning);
                    
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
                WriteRocksmithSngLevels(w, rocksmithSong.Levels, rocksmithSong.SongLength, iterationInfo);                   

                // SONG META DATA
                WriteRocksmithSngMetaDetails(w, rocksmithSong, tuning, iterationInfo);
            }
        }

        private static List<PhraseIterationInfo> CreatePhraseIterationInfo(Song rocksmithSong)
        {
            List<PhraseIterationInfo> info = new List<PhraseIterationInfo>(rocksmithSong.PhraseIterations.PhraseIteration.Length);
            for (int i = 0; i < rocksmithSong.PhraseIterations.PhraseIteration.Length; i++)
            {
                var iteration = rocksmithSong.PhraseIterations.PhraseIteration[i];
                var phrase = rocksmithSong.Phrases.Phrase[iteration.PhraseId];
                var endTime = rocksmithSong.SongLength;
                if (i + 1 < rocksmithSong.PhraseIterations.PhraseIteration.Length)
                {
                    endTime = rocksmithSong.PhraseIterations.PhraseIteration[i + 1].Time;
                }
                info.Add(new PhraseIterationInfo {
                    IterationId = i, 
                    PhraseId = iteration.PhraseId, 
                    MaxDifficulty = phrase.MaxDifficulty,
                    StartTime = iteration.Time,
                    EndTime = endTime });
            }
            return info;
        }

        // COMPLETE
        private static void WriteRocksmithSngHeader(EndianBinaryWriter w)
        {
            w.Write(49); // version num?
        }

        // COMPLETE
        private static void WriteRocksmithSngEbeats(EndianBinaryWriter w, SongEbeats ebeats)
        {
            // output header
            if (ebeats.Ebeat == null || ebeats.Ebeat.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(ebeats.Ebeat.Length);

            Int16 currentMeasure = -1;
            Int16 currentMeasureBeat = -1;

            // output ebeats
            foreach (SongEbeat ebeat in ebeats.Ebeat)
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
        private static void WriteRocksmithSngPhrases(EndianBinaryWriter w, SongPhrases phrases, SongPhraseIterations phraseIteration)
        {
            // Sample: begins at position 7,208 in NumberThirteen_Lead.sng

            // output header
            if (phrases == null || phrases.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phrases.Count);

            // output phrases
            for (int i = 0; i < phrases.Phrase.Length; i++)
            {
                // solo
                w.Write(phrases.Phrase[i].Solo == 1 ? true : false);

                // disparity
                w.Write(phrases.Phrase[i].Disparity == 1 ? true : false);

                // ignore
                w.Write(phrases.Phrase[i].Ignore == 1 ? true : false);

                // unused padding
                w.Write(new byte());

                // maxDifficulty tag
                w.Write(phrases.Phrase[i].MaxDifficulty);

                // count of usage in iterations
                int phraseIterationCount = 0;
                for (int i2 = 0; i2 < phraseIteration.PhraseIteration.Length; i2++)
                {
                    if (phraseIteration.PhraseIteration[i2].PhraseId == i)
                    {
                        phraseIterationCount++;
                    }
                }
                w.Write(phraseIterationCount);

                // name tag
                string name = phrases.Phrase[i].Name;
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
        private static void WriteRocksmithSngChordTemplates(EndianBinaryWriter w, SongChordTemplates chordTemplates, InstrumentTuning tuning)
        {
            // output header
            if (chordTemplates == null || chordTemplates.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(chordTemplates.Count);

            // output chord templates
            foreach (SongChordTemplate chordTemplate in chordTemplates.ChordTemplate)
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
                w.Write(tuning.GetMidiNote(0, chordTemplate.Fret0));
                w.Write(tuning.GetMidiNote(1, chordTemplate.Fret1));
                w.Write(tuning.GetMidiNote(2, chordTemplate.Fret2));
                w.Write(tuning.GetMidiNote(3, chordTemplate.Fret3));
                w.Write(tuning.GetMidiNote(4, chordTemplate.Fret4));
                w.Write(tuning.GetMidiNote(5, chordTemplate.Fret5));

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
        private static void WriteRocksmithSngFretHandMuteTemplates(EndianBinaryWriter w, SongFretHandMuteTemplates fretHandMuteTemplates)
        {
            w.Write(new byte[4]); // placeholder
        }

        // COMPLETE
        private static void WriteRocksmithSngPhraseIterations(EndianBinaryWriter w, SongPhraseIterations phraseIterations, Single songLength)
        {
            // Sample: begins at position 7,664 in NumberThirteen_Lead.sng

            // output header
            if (phraseIterations == null || phraseIterations.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phraseIterations.Count);

            // output phrase iterations
            for (int i = 0; i < phraseIterations.PhraseIteration.Length; i++)
            {
                // phrase id
                w.Write(phraseIterations.PhraseIteration[i].PhraseId);
                // start time
                w.Write(phraseIterations.PhraseIteration[i].Time);
                // end time
                var endTime = i == phraseIterations.PhraseIteration.Length - 1
                    ? songLength
                    : phraseIterations.PhraseIteration[i + 1].Time;
                w.Write(endTime);
            }
        }

        // INCOMPLETE - review Angela_Combo.xml for some inconsistencies
        private static void WriteRocksmithSngPhraseProperties(EndianBinaryWriter w, SongPhraseProperties phraseProperties)
        {
            // output header
            if (phraseProperties == null || phraseProperties.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phraseProperties.Count);

            // output phrase properties
            foreach (SongPhraseProperty phraseProperty in phraseProperties.PhraseProperty)
            {
                // phrase id
                w.Write(phraseProperty.PhraseId);
                
                // difficulty
                w.Write(phraseProperty.Difficulty);

                // empty?
                w.Write(phraseProperty.Empty);

                // level jump?
                w.Write(phraseProperty.LevelJump);

                // redundant
                w.Write(phraseProperty.Redundant);
            }
        }

        // COMPLETE - except hardcoded field
        private static void WriteRocksmithSngLinkedDiffs(EndianBinaryWriter w, SongLinkedDiffs linkedDiffs)
        {
            // output header
            if (linkedDiffs == null || linkedDiffs.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(linkedDiffs.Count);

            foreach (SongLinkedDiff linkedDiff in linkedDiffs.LinkedDiff)
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
        private static void WriteRocksmithSngControls(EndianBinaryWriter w, SongControls controls)
        {
            // output header
            if (controls == null || controls.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(controls.Count);

            // output controls
            foreach (var songControl in controls.Control)
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
        private static void WriteRocksmithSngEvents(EndianBinaryWriter w, SongEvents events)
        {
            // Sample: begins at position 8,172 in NumberThirteen_Lead.sng

            // output header
            if (events == null || events.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(events.Count);

            // output events
            foreach (var songEvent in events.Event)
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
        private static void WriteRocksmithSngSections(EndianBinaryWriter w, SongSections sections, SongPhraseIterations phraseIterations, Single songLength)
        {
            // output header
            if (sections.Section == null || sections.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            // output header count
            w.Write(sections.Count);

            // output sections
            for (int i = 0; i < sections.Section.Length; i++)
            {
                // section name
                string name = sections.Section[i].Name;
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
                w.Write(sections.Section[i].Number);

                // start time
                w.Write(sections.Section[i].StartTime);

                // end time
                var endTime = i == sections.Section.Length - 1
                    ? songLength
                    : sections.Section[i + 1].StartTime;
                w.Write(endTime);

                // phrase iteration start index
                bool phraseIterationFound = false;
                for (int p = 0; p < phraseIterations.PhraseIteration.Length; p++)
                {
                    if (sections.Section[i].StartTime <= phraseIterations.PhraseIteration[p].Time)
                    {
                        w.Write(p);
                        phraseIterationFound = true;
                        break;
                    }
                }
                if (!phraseIterationFound)
                    throw new Exception(string.Format("No phrase iteration found with matching time for section {0}.", i.ToString()));

                // phrase iteration end index           
                if (i == sections.Section.Length - 1) // if last section, default to last phrase iteration
                {
                    w.Write(phraseIterations.PhraseIteration.Length - 1);
                }
                else
                {
                    bool endPhraseIterationFound = false;
                    for (int p = 0; p < phraseIterations.PhraseIteration.Length; p++)
                    {
                        if (sections.Section[i + 1].StartTime <= phraseIterations.PhraseIteration[p].Time)
                        {
                            w.Write(Convert.ToInt32(p - 1));
                            endPhraseIterationFound = true;
                            break;
                        }
                    }
                    if (!endPhraseIterationFound)
                        throw new Exception(string.Format("No end phrase iteration found with matching time for section {0}.", i.ToString()));
                }

                // series of 8 unknown bytes (look like flags)? below logic is wrong, just defaulting for now
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
        private static void WriteRocksmithSngLevels(EndianBinaryWriter w, SongLevels levels, Single songLength, List<PhraseIterationInfo> iterationInfo)
        {
            // output header
            if (levels.Level == null || levels.Level.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(levels.Level.Length);

            // output
            foreach (var level in levels.Level)
            {
                // level difficulty tag
                w.Write(level.Difficulty);

                // anchors
                WriteRocksmithSngLevelAnchors(w, level.Anchors, level, iterationInfo, songLength);

                // unknown section - appears in NumberThirteen_Lead.sng @ 171,032; this section first appears in level 9 of difficulty
                // another example @ 328,272 in NumberThirteen_Combo.sng
                w.Write(new byte[4]);
                // has format of float (matching some anchor times + integer)
                //6 count header
                //12.609 + 5
                //14.4709 + 5
                //16.319 + 5
                //20.0289 + 5
                //21.91899 + 5
                //23.78599 + 5
                //WriteRocksmithSngLevelChords(w, levels.Level[i].Chords);

                // handshapes
                WriteRocksmithSngLevelHandShapes(w, level.HandShapes, level, songLength);

                // notes and chords
                WriteRocksmithSngLevelNotes(w, iterationInfo, level.Notes, level.Chords, songLength);

                var iterationNotes = new List<int>();
                foreach (var iteration in iterationInfo)
                {
                    int num = 0;
                    if (level.Notes != null && level.Notes.Note!=null)
                    {
                        num += level.Notes.Note.Where(n => n.Time >= iteration.StartTime && n.Time < iteration.EndTime).Count();
                    }
                    if (level.Chords != null && level.Chords.Chord!=null)
                    {
                        num += level.Chords.Chord.Where(n => n.Time >= iteration.StartTime && n.Time < iteration.EndTime).Count();
                    }
                    iterationNotes.Add(num);
                }

                var phrases = iterationInfo.GroupBy(it => it.PhraseId).OrderBy(grp => grp.Key); ;
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
        private static void WriteRocksmithSngLevelAnchors(EndianBinaryWriter w, SongAnchors anchors, Xml.SongLevel level, List<PhraseIterationInfo> iterationInfo, Single songLength)
        {
            if (anchors == null || anchors.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output anchors header count
            w.Write(anchors.Count);

            // output anchors
            for (int i = 0; i < anchors.Anchor.Length; i++)
            {
                // anchor start time
                var startTime = anchors.Anchor[i].Time;
                w.Write(startTime);

                // anchor end time
                var endTime = i == anchors.Anchor.Length - 1
                    ? songLength
                    : anchors.Anchor[i + 1].Time;
                w.Write(endTime);

                float? lastNote = null, lastChord = null;
                var notes = (level.Notes == null || level.Notes.Note == null) ? null : level.Notes.Note.Where(note => note.Time >= startTime && note.Time < endTime);
                if (notes != null && notes.Count() > 0)
                {
                    lastNote = notes.Max(note => note.Time);
                }
                var chords = (level.Chords == null || level.Chords.Chord == null) ? null : level.Chords.Chord.Where(chord => chord.Time >= startTime && chord.Time < endTime);
                if (chords != null && chords.Count() > 0)
                {
                    lastChord = chords.Max(chord => chord.Time);
                }
                float lastTime = Math.Min(lastNote ?? endTime, lastChord ?? endTime);
                w.Write(lastTime);

                // fret
                w.Write(anchors.Anchor[i].Fret);

                // phrase iteration index
                bool phraseIterationFound = false;
                foreach (var iteration in iterationInfo)
                {
                    if (anchors.Anchor[i].Time >= iteration.StartTime &&
                        anchors.Anchor[i].Time < iteration.EndTime)
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

        // INCOMPLETE
        private static void WriteRocksmithSngLevelHandShapes(EndianBinaryWriter w, SongHandShapes handShapes, Xml.SongLevel level, float songLength)
        {
            // sample section begins @ 328,356 in NumberThirteen_Combo.sng
            //  sample section begins @ 4,300 in TCPowerChords_Lead.sng   

            if (handShapes == null || handShapes.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            // output notes header count
            w.Write(handShapes.Count);

            // ouput handshapes
            for (int i = 0; i < handShapes.HandShape.Length; i++)
            {
                SongHandShape handShape = handShapes.HandShape[i];
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

                var endTime = i == handShapes.HandShape.Length - 1
                    ? songLength
                    : handShapes.HandShape[i + 1].StartTime;

                // This should actually be the time of the last chord before the end time.
                float? lastChord = null;
                var chords = (level.Chords == null || level.Chords.Chord == null) ? null : level.Chords.Chord.Where(chord => chord.Time >= handShape.StartTime && chord.Time < endTime);
                if (chords != null && chords.Count() > 0)
                {
                    lastChord = chords.Max(chord => chord.Time);
                }

                w.Write(lastChord ?? endTime);
            }
        }

        // COMPLETE except hardcoded fields
        private static void WriteRocksmithSngLevelNotes(EndianBinaryWriter w, List<PhraseIterationInfo> iterationInfo, SongNotes notes, SongChords chords, Single songLength)
        {
            List<TimeLinkedEntity> notesChords = new List<TimeLinkedEntity>();

            // add notes to combined note/chord array
            if (notes != null && notes.Count != 0)
            {
                notesChords.AddRange(notes.Note.Select(note =>
                    new TimeLinkedEntity
                    {
                        Time = note.Time,
                        Entity = note
                    }));
            }

            // add chords to combined note/chord array
            if (chords != null && chords.Count != 0)
            {
                notesChords.AddRange(chords.Chord.Select(chord =>
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
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Fret : -1);

                // chord id
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? -1 : ((SongChord)notesChords[i].Entity).ChordId);

                // unknown
                w.Write(Convert.ToInt32(-1));

                // sustain time
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Sustain : 0);

                // bend
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Bend : 0);

                // slideTo
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).SlideTo : -1);

                // tremolo
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Tremolo : new byte());

                // harmonic
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Harmonic : new byte());

                // palm mute
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).PalmMute : new byte());

                // hopo
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Hopo : new byte());

                // hammerOn
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).HammerOn : new byte());

                // pullOff
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).PullOff : new byte());

                // ignore
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Ignore : ((SongChord)notesChords[i].Entity).Ignore);

                // high density chord
                w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? new byte() : ((SongChord)notesChords[i].Entity).HighDensity);

                // this position is sometimes empty bytes, often the values below, or other values as well 
                w.Write(new byte[4]);
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
                var level = s.Levels.Level[iteration.MaxDifficulty];
                if (level.Notes != null && level.Notes.Note != null)
                {
                    totalNotes += level.Notes.Note.Where(note => note.Time >= iteration.StartTime && note.Time < iteration.EndTime).Count();
                }
                if (level.Chords != null && level.Chords.Chord != null)
                {
                    totalNotes += level.Chords.Chord.Where(chord => chord.Time >= iteration.StartTime && chord.Time < iteration.EndTime).Count();
                }
            }
            // Total notes when fully leveled
            w.Write(totalNotes);

            // points per note
            w.Write(100000d / totalNotes);
      
            // song beat timing
            if (s.Ebeats.Ebeat.Length < 2)
                throw new InvalidDataException("Song must contain at least 2 beats");

            // this is not 100% accurate unless all beats are evenly spaced in a song;
            // still trying to determine exactly how Rocksmith is deriving this time value
            w.Write(s.Ebeats.Ebeat[1].Time - s.Ebeats.Ebeat[0].Time); 

            // first beat time(?); confirmed as not first phraseIteration time and not first section time
            w.Write(s.Ebeats.Ebeat[0].Time);

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
            w.Write(new byte[64 -title.Length]); // pad to 64 bytes

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
            string artistValue = string.IsNullOrEmpty(s.Artist) ? "DUMMY" : s.Artist;
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
            // which does not match any pharse iteration start time
            // and does not match first ebeat time
            // and does not match any section start time
            // does match first event time of E3 (although this is not a match in other files)
            // does not match any note times on 1st difficulty level
            // nor an anchor time on 1st difficulty level

            //It appears to be the time of the first note.
            w.Write(s.Ebeats.Ebeat[0].Time); // wrong
            
            // unknown
            w.Write(s.Ebeats.Ebeat[0].Time); // wrong
            
            // max difficulty
            int maxDifficulty = s.Phrases.Phrase.Max(p => p.MaxDifficulty);
            w.Write(maxDifficulty);

            // unknown section
            w.Write(new byte[4]); // header with repeating array; song works in game if array is defaulted to 0 count so will leave this alone for now

            // unknown section
            w.Write(new byte[4]); // header with repeating array - only populated in limited songs
        }
    }
}


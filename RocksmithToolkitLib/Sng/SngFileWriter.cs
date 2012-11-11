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

    public struct TimeLinkedEntity
    {
        public Single Time { get; set; }
        public Object Entity { get; set; }
    }
    
    public static class SngFileWriter
    {
        public static void Write(string inputFile, string outputFile, ArrangementType arrangementType, GamePlatform platform)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Song));
            using (var reader = new StreamReader(inputFile))
            {
                var bitConverter = platform == GamePlatform.Pc
                    ? (EndianBitConverter)EndianBitConverter.Little
                    : (EndianBitConverter)EndianBitConverter.Big;

                if (arrangementType == ArrangementType.Instrument)
                {
                    var song = (Song)serializer.Deserialize(reader);
                    WriteRocksmithSngFile(song, outputFile, bitConverter);
                }
                else
                {
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
        private static void WriteRocksmithSngFile(Song rocksmithSong, string outputFile, EndianBitConverter bitConverter)
        {
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
                WriteRocksmithSngChordTemplates(w, rocksmithSong.ChordTemplates);
                    
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
                WriteRocksmithSngLevels(w, rocksmithSong.Levels, rocksmithSong.SongLength, rocksmithSong.Phrases, rocksmithSong.PhraseIterations);                   

                // SONG META DATA
                WriteRocksmithSngMetaDetails(w, rocksmithSong);
            }
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
                if (measure > 0)
                {
                    currentMeasure = measure;
                    currentMeasureBeat = 0;

                    w.Write(currentMeasure);
                    w.Write(currentMeasureBeat);
                    w.Write(true);
                    w.Write(false); // TODO: confirm why populated with 00 on some examples and FF on others.
                    w.Write(false); // TODO: confirm why populated with 00 on some examples and FF on others.
                    w.Write(false); // TODO: confirm why populated with 00 on some examples and FF on others.
                }
                else if (measure == -1)
                {
                    currentMeasureBeat++;

                    w.Write(currentMeasure);
                    w.Write(currentMeasureBeat);
                    w.Write(false);
                    w.Write(false); // TODO: confirm why populated with 00 on some examples and FF on others.
                    w.Write(false); // TODO: confirm why populated with 00 on some examples and FF on others.
                    w.Write(false); // TODO: confirm why populated with 00 on some examples and FF on others.
                }
            }
        }

        // COMPLETE (need solo example to confirm)
        // Sample: begins at position 7,208 in NumberThirteen_Lead.sng
        private static void WriteRocksmithSngPhrases(EndianBinaryWriter w, SongPhrases phrases, SongPhraseIterations phraseIteration)
        {
            // output header
            if (phrases.Phrase == null || phrases.Phrase.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phrases.Phrase.Length);

            // output phrases
            for (int i = 0; i < phrases.Phrase.Length; i++)
            {
                // unusued padding (?)
                w.Write(new byte());

                // disparity
                w.Write(phrases.Phrase[i].Disparity == 1 ? true : false);

                // unused padding (?)
                w.Write(new byte[2]);

                // maxDifficulty tag
                w.Write(phrases.Phrase[i].MaxDifficulty);

                // count of usage in iterations
                int phraseIterationCount = phraseIteration.PhraseIteration.Count(t => t.PhraseId == i);
                w.Write(phraseIterationCount);

                // name tag
                string name = phrases.Phrase[i].Name;
                foreach (char c in name)
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after name
                w.Write(new byte[32 - name.Length]);

                // solo

                // ignore
            }
        }

        // COMPLETE
        private static void WriteRocksmithSngChordTemplates(EndianBinaryWriter w, SongChordTemplates chordTemplates)
        {
            // output header
            if (chordTemplates.ChordTemplate == null || chordTemplates.ChordTemplate.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(chordTemplates.ChordTemplate.Length);

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
                w.Write(chordTemplate.Fret0 == -1 ? -1 : (40 + chordTemplate.Fret0));
                w.Write(chordTemplate.Fret1 == -1 ? -1 : (45 + chordTemplate.Fret1));
                w.Write(chordTemplate.Fret2 == -1 ? -1 : (50 + chordTemplate.Fret2));
                w.Write(chordTemplate.Fret3 == -1 ? -1 : (55 + chordTemplate.Fret3));
                w.Write(chordTemplate.Fret4 == -1 ? -1 : (59 + chordTemplate.Fret4));
                w.Write(chordTemplate.Fret5 == -1 ? -1 : (64 + chordTemplate.Fret5));

                // chord name
                string name = chordTemplate.ChordName;
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
            if (phraseIterations.PhraseIteration == null || phraseIterations.PhraseIteration.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phraseIterations.PhraseIteration.Length);

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

        // COMPLETE - might have level jump and empty reversed
        private static void WriteRocksmithSngPhraseProperties(EndianBinaryWriter w, SongPhraseProperties phraseProperties)
        {
            // output header
            if (phraseProperties.PhraseProperty == null || phraseProperties.PhraseProperty.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(phraseProperties.PhraseProperty.Length);

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
            if (linkedDiffs == null || linkedDiffs.LinkedDiff == null || linkedDiffs.LinkedDiff.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(linkedDiffs.LinkedDiff.Length);

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
            if (controls == null || controls.Control == null || controls.Control.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(controls.Control.Length);

            // output controls
            foreach (var songControl in controls.Control)
            {
                // control time
                w.Write(songControl.Time);

                // control code
                string code = songControl.Code;
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
            if (events.Event == null || events.Event.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output header count
            w.Write(events.Event.Length);

            // output events
            foreach (var songEvent in events.Event)
            {
                // event time
                w.Write(songEvent.Time);
                // event code
                string eventCode = songEvent.Code;
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
            if (sections.Section == null || sections.Section.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            // output header count
            w.Write(sections.Section.Length);

            // output sections
            for (int i = 0; i < sections.Section.Length; i++)
            {
                // section name
                string name = sections.Section[i].Name;
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
                var phraseIterationStartIndex = phraseIterations.PhraseIteration.IndexOf(pi => pi.Time >= sections.Section[i].StartTime);
                if (!phraseIterationStartIndex.HasValue)
                    throw new InvalidDataException(string.Format("No phrase iteration found with matching time for section {0}.", i));
                w.Write(phraseIterationStartIndex.Value);

                // phrase iteration end index
                if (i == sections.Section.Length - 1) // if last section, default to last phrase iteration
                {
                    w.Write(phraseIterations.PhraseIteration.Length - 1);
                }
                else
                {
                    var phraseIterationEndIndex = phraseIterations.PhraseIteration.IndexOf(pi => pi.Time >= sections.Section[i + 1].StartTime);
                    if (!phraseIterationEndIndex.HasValue)
                        throw new InvalidDataException(string.Format("No end phrase iteration found with matching time for section {0}.", i));
                    w.Write(phraseIterationEndIndex.Value);
                }

                // series of 8 unknown bits? below logic is wrong, just defaulting for now
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
        private static void WriteRocksmithSngLevels(EndianBinaryWriter w, SongLevels levels, Single songLength, SongPhrases phrases, SongPhraseIterations phraseIterations)
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
                WriteRocksmithSngLevelAnchors(w, level.Anchors, phraseIterations, songLength);

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
                WriteRocksmithSngLevelHandShapes(w, level.HandShapes);

                // notes and chords
                WriteRocksmithSngLevelNotes(w, phraseIterations, level.Notes, level.Chords, songLength);

                // count of phrases
                w.Write(phrases.Phrase.Length);
                foreach (var phrase in phrases.Phrase)
                {
                    w.Write(Convert.ToSingle(1)); // unknown float here; hard coded to 1 for now
                }

                // count of phrase iterations
                w.Write(phraseIterations.PhraseIteration.Length);
                foreach (var phraseIteration in phraseIterations.PhraseIteration)
                {
                    w.Write(1); // unknown int or flags here?; hard coded to 1 for now
                }

                // count of phrase iterations
                w.Write(phraseIterations.PhraseIteration.Length);
                foreach (var phraseIteration in phraseIterations.PhraseIteration)
                {
                    w.Write(1); // unknown int or flags here?; hard coded to 1 for now
                }
            }
        }

        // COMPLETE except unknown time field which currently has rough estimation logic as placeholder
        private static void WriteRocksmithSngLevelAnchors(EndianBinaryWriter w, SongAnchors anchors, SongPhraseIterations phraseIterations, Single songLength)
        {
            if (anchors == null || anchors.Anchor == null || anchors.Anchor.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }

            // output anchors header count
            w.Write(anchors.Anchor.Length);

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

                // unknown time @ 12,440; seems to be 2-5%  less than end value; fix this ballpark later:
                w.Write(endTime * (float).97);

                // fret
                w.Write(anchors.Anchor[i].Fret);

                // phrase iteration index
                var phraseIterationIndex = phraseIterations.PhraseIteration.IndexOf(pi => startTime < pi.Time)
                    ?? phraseIterations.PhraseIteration.Length;
                w.Write(phraseIterationIndex - 1);
            }
        }

        // INCOMPLETE
        private static void WriteRocksmithSngLevelHandShapes(EndianBinaryWriter w, SongHandShapes handShapes)
        {
            // sample section begins @ 328,356 in NumberThirteen_Combo.sng
            //  sample section begins @ 4,300 in TCPowerChords_Lead.sng   

            if (handShapes == null || handShapes.HandShape == null || handShapes.HandShape.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            // output notes header count
            w.Write(handShapes.HandShape.Length);

            // ouput handshapes
            foreach (SongHandShape handShape in handShapes.HandShape)
            {
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
                w.Write(handShape.StartTime); 

                // chord start time again ???
                w.Write(handShape.StartTime);
            }
        }

        // COMPLETE except hardcoded fields
        private static void WriteRocksmithSngLevelNotes(EndianBinaryWriter w, SongPhraseIterations phraseIterations, SongNotes notes, SongChords chords, Single songLength)
        {
            List<TimeLinkedEntity> notesChords = new List<TimeLinkedEntity>();

            // add notes to combined note/chord array
            if (notes != null && notes.Note != null && notes.Note.Length != 0)
            {
                notesChords.AddRange(notes.Note.Select(note =>
                    new TimeLinkedEntity
                    {
                        Time = note.Time,
                        Entity = note
                    }));
            }

            // add chords to combined note/chord array
            if (chords != null && chords.Chord != null && chords.Chord.Length != 0)
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

                // unknown
                w.Write(Convert.ToInt16(246));  // wrong
                w.Write(Convert.ToInt16(7472)); // wrong

                // phrase iteration start index and id ????
                var phraseIterationStartIndex = phraseIterations.PhraseIteration.IndexOf((item, next) =>
                        notesChords[i].Time >= item.Time &&
                        notesChords[i].Time < (next != null ? next.Time : songLength));
                if (!phraseIterationStartIndex.HasValue)
                    throw new InvalidDataException(string.Format("No phrase start iteration found with matching time for note {0}.", i));

                w.Write(phraseIterationStartIndex.Value); // phrase iteration
                w.Write(phraseIterations.PhraseIteration[phraseIterationStartIndex.Value].PhraseId);
            }
        }

        // INCOMPLETE
        private static void WriteRocksmithSngMetaDetails(EndianBinaryWriter w, Song s)
        {
            // unknown
            w.Write(0); // populated on training charts only (?)    

            // unknown
            w.Write((Single)7.7629395); // always this value?  related to scoring

            // unknown
            w.Write(0); // populated on training charts only (?)   

            // unknown
            w.Write(0); // float value typically around ~4.5

            // unknown
            w.Write(1086698382); // seems to be related to scoring

            // unknown
            w.Write(954437177); // seems to be related to scoring; float value typically around ~3.5
      
            // song beat timing
            if (s.Ebeats.Ebeat.Length < 2)
                throw new InvalidDataException("Song must contain at least 2 beats");

            // this is not 100% accurate unless all beats are evenly spaced in a song;
            // still trying to determine exactly how Rocksmith is deriving this time value
            w.Write(s.Ebeats.Ebeat[1].Time - s.Ebeats.Ebeat[0].Time); 

            // first beat time(?); confirmed as not first phraseIteration time and not first section time
            w.Write(s.Ebeats.Ebeat[0].Time); 
            
            // song conversion date
            foreach (char c in s.LastConversionDateTime)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - s.LastConversionDateTime.Length]); //pad to 32 bytes

            // song title
            foreach (char c in s.Title)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[64 - s.Title.Length]); // pad to 64 bytes

            // arrangement
            foreach (char c in s.Arrangement)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - s.Arrangement.Length]); //pad to 32 bytes

            // artist
            string artistValue = string.IsNullOrEmpty(s.Artist) ? "DUMMY" : s.Artist;
            foreach (char c in artistValue)
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - artistValue.Length]); //pad to 32 bytes

            // song part
            w.Write(s.Part);

            // song length
            w.Write(s.SongLength);

            // unknown
            w.Write(new byte[4]); // blank 4 bytes in NumberThirteen_Lead.sng

            // unknown
            w.Write(new byte[4]); // float with 10.2927 in NumberThirteen_Lead.sng

            // unknown
            w.Write(s.Ebeats.Ebeat[0].Time); // wrong; float with time of first beat; time of phraseid=1 (not 0) in somes examples;  float with time of first note where ignore <> 1
            
            // unknown
            w.Write(s.Ebeats.Ebeat[0].Time); // wrong; float with time of first beat; time of phraseid=1 (not 0) in somes examples;  float with time of first note where ignore <> 1
            
            // unknown
            w.Write(new byte[4]); // float with 1.9618 in NumberThirteen_Lead.sng....  number 4?

            // unknown section
            w.Write(new byte[4]); // header with repeating array; song works in game if array is defaulted to 0 count so will leave this alone for now

            // unknown section
            w.Write(new byte[4]); // header with repeating array - seems to be populated only in training sng files
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;
using RocksmithSngCreator.Serialization;

namespace RocksmithSngCreator
{
    public enum GamePlatform { PC, XBOX, PS3 };

    public struct TimeLinkedEntity
    {
        public Single Time { get; set; }
        public Object Entity { get; set; }
    }
    
    public class SngFileWriter
    {
        GamePlatform _platform;

        public SngFileWriter(GamePlatform platform)
        {
            _platform = platform;
        }

        public void WriteRocksmithSongChart(string inputFile, string outputFile)
        {
            Song rocksmithSong;
            XmlSerializer serializer = new XmlSerializer(typeof(Song));
            StreamReader reader = new StreamReader(inputFile);
            rocksmithSong = (Song)serializer.Deserialize(reader);
            reader.Close();

            if (_platform == GamePlatform.PC)
                writeRocksmithSngFile(rocksmithSong, outputFile, EndianBitConverter.Little);
            else
                writeRocksmithSngFile(rocksmithSong, outputFile, EndianBitConverter.Big);
        }

        public void WriteRocksmithVocalChart(string inputFile, string outputFile)
        {
            Vocals vocals;
            XmlSerializer serializer = new XmlSerializer(typeof(Vocals));
            StreamReader reader = new StreamReader(inputFile);
            vocals = (Vocals)serializer.Deserialize(reader);
            reader.Close();

            if (_platform == GamePlatform.PC)
                writeRocksmithVocalsFile(vocals, outputFile, EndianBitConverter.Little);
            else
                writeRocksmithVocalsFile(vocals, outputFile, EndianBitConverter.Big);
        }
        
        // COMPLETE
        private void writeRocksmithVocalsFile(Vocals vocals, string outputFile, EndianBitConverter bitConverter)
        {
            // WRITE THE .SNG FILE
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                using (EndianBinaryWriter w = new EndianBinaryWriter(bitConverter, fs))
                {
                    // file header
                    this.writeRocksmithSngHeader(w);

                    // unused filler
                    w.Write(new byte[16]);

                    // vocal count
                    if (vocals.Count != vocals.Vocal.Length)
                        throw new Exception("XML vocals header count does not match number of vocal items.");
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
                            throw new Exception(string.Format("Vocal lyric '{0}' at position {1} exceeded the maximum width of 32 bytes.", lyric, i.ToString()));
                        foreach (char c in lyric.ToCharArray())
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
        }

        // COMPLETE
        private void writeRocksmithSngFile(Song rocksmithSong, string outputFile, EndianBitConverter bitConverter)
        {
            // WRITE THE .SNG FILE
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                using (EndianBinaryWriter w = new EndianBinaryWriter(bitConverter, fs))
                {
                    // HEADER
                    this.writeRocksmithSngHeader(w);

                    // EBEATS DATA                    
                    this.writeRocksmithSngEbeats(w, rocksmithSong.Ebeats);

                    // PHRASES                    
                    this.writeRocksmithSngPhrases(w, rocksmithSong.Phrases, rocksmithSong.PhraseIterations);

                    // CHORD TEMPLATES
                    this.writeRocksmithSngChordTemplates(w, rocksmithSong.ChordTemplates);                    
                    
                    // FRET HAND MUTE TEMPLATE
                    this.writeRocksmithSngFretHandMuteTemplates(w, rocksmithSong.FretHandMuteTemplates);

                    // VOCALS TEMPLATE 
                    w.Write(new byte[4]); // not used on song file

                    // PHRASE ITERATIONS                    
                    this.writeRocksmithSngPhraseIterations(w, rocksmithSong.PhraseIterations, rocksmithSong.SongLength);

                    // PHRASE PROPERTIES
                    this.writeRocksmithSngPhraseProperties(w, rocksmithSong.PhraseProperties);

                    // LINKED DIFFS
                    this.writeRocksmithSngLinkedDiffs(w, rocksmithSong.LinkedDiffs);

                    // CONTROLS
                    this.writeRocksmithSngControls(w, rocksmithSong.Controls);

                    // EVENTS
                    this.writeRocksmithSngEvents(w, rocksmithSong.Events);

                    // SECTIONS
                    this.writeRocksmithSngSections(w, rocksmithSong.Sections, rocksmithSong.PhraseIterations, rocksmithSong.SongLength);

                    // LEVELS
                    this.writeRocksmithSngLevels(w, rocksmithSong.Levels, rocksmithSong.SongLength, rocksmithSong.Phrases, rocksmithSong.PhraseIterations);                   

                    // SONG META DATA
                    writeRocksmithSngMetaDetails(w, rocksmithSong);
                }
            }
        }

        // COMPLETE
        private void writeRocksmithSngHeader(EndianBinaryWriter w)
        {
            w.Write(49); // version num?
        }

        // COMPLETE
        private void writeRocksmithSngEbeats(EndianBinaryWriter w, SongEbeats ebeats)
        {
            // output header
            if (ebeats.Ebeat == null || ebeats.Ebeat.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(ebeats.Ebeat.Length);
            }

            Int16 currentMeasure = -1;
            Int16 currentMeasureBeat = -1;

            // output ebeats
            for (int i = 0; i < ebeats.Ebeat.Length; i++)
            {
                // ebeat time
                w.Write(ebeats.Ebeat[i].Time);

                // measure
                Int16 measure = Convert.ToInt16(ebeats.Ebeat[i].Measure);
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
        private void writeRocksmithSngPhrases(EndianBinaryWriter w, SongPhrases phrases, SongPhraseIterations phraseIteration)
        {
            // output header
            if (phrases.Phrase == null || phrases.Phrase.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(phrases.Phrase.Length);
            }

            // output phrases
            for (int i = 0; i < phrases.Phrase.Length; i++)
            {
                // disparity
                if (phrases.Phrase[i].Disparity == 1)
                    w.Write(Convert.ToInt32(256));
                else
                    w.Write(Convert.ToInt32(0));

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
                foreach (char c in name.ToCharArray())
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
        private void writeRocksmithSngChordTemplates(EndianBinaryWriter w, SongChordTemplates chordTemplates)
        {
            // output header
            if (chordTemplates.ChordTemplate == null || chordTemplates.ChordTemplate.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(chordTemplates.ChordTemplate.Length);
            }

            // output chord templates
            for (int i = 0; i < chordTemplates.ChordTemplate.Length; i++)
            {
                // fret numbers
                w.Write(chordTemplates.ChordTemplate[i].Fret0);
                w.Write(chordTemplates.ChordTemplate[i].Fret1);
                w.Write(chordTemplates.ChordTemplate[i].Fret2);
                w.Write(chordTemplates.ChordTemplate[i].Fret3);
                w.Write(chordTemplates.ChordTemplate[i].Fret4);
                w.Write(chordTemplates.ChordTemplate[i].Fret5);

                // finger positions
                w.Write(chordTemplates.ChordTemplate[i].Finger0);
                w.Write(chordTemplates.ChordTemplate[i].Finger1);
                w.Write(chordTemplates.ChordTemplate[i].Finger2);
                w.Write(chordTemplates.ChordTemplate[i].Finger3);
                w.Write(chordTemplates.ChordTemplate[i].Finger4);
                w.Write(chordTemplates.ChordTemplate[i].Finger5);

                // note values
                w.Write(chordTemplates.ChordTemplate[i].Fret0 == -1 ? -1 : (40 + chordTemplates.ChordTemplate[i].Fret0));
                w.Write(chordTemplates.ChordTemplate[i].Fret1 == -1 ? -1 : (45 + chordTemplates.ChordTemplate[i].Fret1));
                w.Write(chordTemplates.ChordTemplate[i].Fret2 == -1 ? -1 : (50 + chordTemplates.ChordTemplate[i].Fret2));
                w.Write(chordTemplates.ChordTemplate[i].Fret3 == -1 ? -1 : (55 + chordTemplates.ChordTemplate[i].Fret3));
                w.Write(chordTemplates.ChordTemplate[i].Fret4 == -1 ? -1 : (59 + chordTemplates.ChordTemplate[i].Fret4));
                w.Write(chordTemplates.ChordTemplate[i].Fret5 == -1 ? -1 : (64 + chordTemplates.ChordTemplate[i].Fret5));

                // chord name
                string name = chordTemplates.ChordTemplate[i].ChordName;
                foreach (char c in name.ToCharArray())
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after name
                w.Write(new byte[32 - name.Length]);
            }
        }

        // NO EXAMPLES IN ROCKSMITH?
        private void writeRocksmithSngFretHandMuteTemplates(EndianBinaryWriter w, SongFretHandMuteTemplates fretHandMuteTemplates)
        {
            w.Write(new byte[4]); // placeholder
        }

        // COMPLETE     
        private void writeRocksmithSngPhraseIterations(EndianBinaryWriter w, SongPhraseIterations phraseIterations, Single songLength)
        {
            // Sample: begins at position 7,664 in NumberThirteen_Lead.sng

            // output header
            if (phraseIterations.PhraseIteration == null || phraseIterations.PhraseIteration.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(phraseIterations.PhraseIteration.Length);
            }

            // output phrase iterations
            for (int i = 0; i < phraseIterations.PhraseIteration.Length; i++)
            {
                // phrase id
                w.Write(phraseIterations.PhraseIteration[i].PhraseId);
                // start time
                w.Write(phraseIterations.PhraseIteration[i].Time);
                // end time
                if (i == phraseIterations.PhraseIteration.Length - 1)
                    w.Write(songLength);
                else
                    w.Write(phraseIterations.PhraseIteration[i + 1].Time);
            }
        }

        // COMPLETE - might have level jump and empty reversed
        private void writeRocksmithSngPhraseProperties(EndianBinaryWriter w, SongPhraseProperties phraseProperties)
        {
            // output header
            if (phraseProperties.PhraseProperty == null || phraseProperties.PhraseProperty.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(phraseProperties.PhraseProperty.Length);
            }

            // output phrase properties
            for (int i = 0; i < phraseProperties.PhraseProperty.Length; i++)
            {
                // phrase id
                w.Write(phraseProperties.PhraseProperty[i].PhraseId);
                
                // difficulty
                w.Write(phraseProperties.PhraseProperty[i].Difficulty);

                // empty?
                w.Write(phraseProperties.PhraseProperty[i].Empty);

                // level jump?
                w.Write(phraseProperties.PhraseProperty[i].LevelJump);

                // redundant
                w.Write(phraseProperties.PhraseProperty[i].Redundant);
            }
        }

        // COMPLETE - except hardcoded field
        private void writeRocksmithSngLinkedDiffs(EndianBinaryWriter w, SongLinkedDiffs linkedDiffs)
        {
            // output header
            if (linkedDiffs == null || linkedDiffs.LinkedDiff == null || linkedDiffs.LinkedDiff.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(linkedDiffs.LinkedDiff.Length);

                for (int i = 0; i < linkedDiffs.LinkedDiff.Length; i++)
                {
                    // parent id
                    w.Write(linkedDiffs.LinkedDiff[i].ParentId);

                    // child id
                    w.Write(linkedDiffs.LinkedDiff[i].ChildId);

                    // unknown (first byte seems to be a boolean bit with true value in all cases reviewed)
                    w.Write(true);
                    w.Write(false);
                    w.Write(false);
                    w.Write(false);
                }
            }
        }

        // COMPLETE
        private void writeRocksmithSngControls(EndianBinaryWriter w, SongControls controls)
        {
            // output header
            if (controls == null || controls.Control == null || controls.Control.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(controls.Control.Length);
            }

            // output controls
            for (int i = 0; i < controls.Control.Length; i++)
            {
                // control time
                w.Write(controls.Control[i].Time);

                // control code
                string code = controls.Control[i].Code;
                foreach (char c in code.ToCharArray())
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after control code
                w.Write(new byte[256 - code.Length]);
            }
        }

        // COMPLETE
        private void writeRocksmithSngEvents(EndianBinaryWriter w, SongEvents events)
        {
            // Sample: begins at position 8,172 in NumberThirteen_Lead.sng

            // output header
            if (events.Event == null || events.Event.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(events.Event.Length);
            }

            // output events
            for (int i = 0; i < events.Event.Length; i++)
            {
                // event time
                w.Write(events.Event[i].Time);
                // event code
                string eventCode = events.Event[i].Code;
                foreach (char c in eventCode.ToCharArray())
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after event code
                w.Write(new byte[256 - eventCode.Length]);
            }
        }

        // COMPLETE except hardcoded fields
        // Sample: begins at position 9,216 in NumberThirteen_Lead.sng
        private void writeRocksmithSngSections(EndianBinaryWriter w, SongSections sections, SongPhraseIterations phraseIterations, Single songLength)
        {
            // output header
            if (sections.Section == null || sections.Section.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(sections.Section.Length);
            }

            // output sections
            for (int i = 0; i < sections.Section.Length; i++)
            {
                // section name
                string name = sections.Section[i].Name;
                foreach (char c in name.ToCharArray())
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
                if (i == sections.Section.Length - 1)
                    w.Write(songLength);
                else
                    w.Write(sections.Section[i + 1].StartTime);

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
        private void writeRocksmithSngLevels(EndianBinaryWriter w, SongLevels levels, Single songLength, SongPhrases phrases, SongPhraseIterations phraseIterations)
        {
            // output header
            if (levels.Level == null || levels.Level.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output header count
                w.Write(levels.Level.Length);
            }

            // output
            for (int i = 0; i < levels.Level.Length; i++)
            {
                // level difficulty tag
                w.Write(levels.Level[i].Difficulty);

                // anchors
                writeRocksmithSngLevelAnchors(w, levels.Level[i].Anchors, phraseIterations, songLength);

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
                //writeRocksmithSngLevelChords(w, levels.Level[i].Chords);
                                 
                // handshapes
                writeRocksmithSngLevelHandShapes(w, levels.Level[i].HandShapes);
                
                // notes and chords
                writeRocksmithSngLevelNotes(w, phraseIterations, levels.Level[i].Notes, levels.Level[i].Chords, songLength);

                // count of phrases
                w.Write(phrases.Phrase.Length);
                for (int p = 0; p < phrases.Phrase.Length; p++)
                {
                    w.Write(Convert.ToSingle(1)); // unknown float here; hard coded to 1 for now
                }
                
                // count of phrase iterations
                w.Write(phraseIterations.PhraseIteration.Length);
                for (int p = 0; p < phraseIterations.PhraseIteration.Length; p++)
                {
                    w.Write(1); // unknown int or flags here?; hard coded to 1 for now
                }

                // count of phrase iterations
                w.Write(phraseIterations.PhraseIteration.Length);
                for (int p = 0; p < phraseIterations.PhraseIteration.Length; p++)
                {
                    w.Write(1); // unknown int or flags here?; hard coded to 1 for now
                }
            }
        }

        // COMPLETE except unknown time field which currently has rough estimation logic as placeholder
        private void writeRocksmithSngLevelAnchors(EndianBinaryWriter w, SongAnchors anchors, SongPhraseIterations phraseIterations, Single songLength)
        {
            if (anchors == null || anchors.Anchor == null || anchors.Anchor.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output anchors header count
                w.Write(anchors.Anchor.Length);

                // ouput anchors
                for (int i = 0; i < anchors.Anchor.Length; i++)
                {
                    // anchor start time
                    w.Write(anchors.Anchor[i].Time);

                    // anchor end time
                    if (i == anchors.Anchor.Length - 1)
                        w.Write(songLength);
                    else
                        w.Write(anchors.Anchor[i + 1].Time);

                    // unknown time @ 12,440; seems to be 2-5%  less than end value; fix this ballpark later:
                    if (i == anchors.Anchor.Length - 1)
                        w.Write(songLength * (float).97);
                    else
                        w.Write(anchors.Anchor[i + 1].Time * (float).97);

                    // fret
                    w.Write(anchors.Anchor[i].Fret);

                    // phrase iteration index
                    bool phraseIterationFound = false;
                    for (int p = 0; p < phraseIterations.PhraseIteration.Length; p++)
                    {
                        if (anchors.Anchor[i].Time < phraseIterations.PhraseIteration[p].Time)
                        {
                            w.Write(p-1);
                            phraseIterationFound = true;
                            break;
                        }
                    }
                    if (!phraseIterationFound)
                        w.Write(phraseIterations.PhraseIteration.Length - 1);
                }
            }
        }

        // INCOMPLETE
        private void writeRocksmithSngLevelHandShapes(EndianBinaryWriter w, SongHandShapes handShapes)
        {
            // sample section begins @ 328,356 in NumberThirteen_Combo.sng
            //  sample section begins @ 4,300 in TCPowerChords_Lead.sng   

            if (handShapes == null || handShapes.HandShape == null || handShapes.HandShape.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output notes header count
                w.Write(handShapes.HandShape.Length);

                // ouput handshapes
                for (int i = 0; i < handShapes.HandShape.Length; i++)
                {
                    // hand shape start time
                    w.Write(handShapes.HandShape[i].StartTime); 

                    // hand shape end time
                    w.Write(handShapes.HandShape[i].EndTime); 

                    // unknown
                    w.Write(Convert.ToSingle(-1));

                    // unknown
                    w.Write(Convert.ToSingle(-1));

                    // chord id
                    w.Write(handShapes.HandShape[i].ChordId);

                    // chord start time again ???
                    w.Write(handShapes.HandShape[i].StartTime); 

                    // chord start time again ???
                    w.Write(handShapes.HandShape[i].StartTime); 
                }
            }
        }

        // COMPLETE except hardcoded fields
        private void writeRocksmithSngLevelNotes(EndianBinaryWriter w, SongPhraseIterations phraseIterations, SongNotes notes, SongChords chords, Single songLength)
        {
            //int notesCount = 0;
            //int chordsCount = 0;
            List<TimeLinkedEntity> notesChords = new List<TimeLinkedEntity>();

            // add notes to combined note/chord array
            if (notes != null && notes.Note != null && notes.Note.Length != 0)
            {
                foreach (SongNote note in notes.Note)
                {
                    notesChords.Add(new TimeLinkedEntity { Time = note.Time, Entity = note });
                }
            }

            // add chords to combined note/chord array
            if (chords != null && chords.Chord != null && chords.Chord.Length != 0)
            {
                foreach (SongChord chord in chords.Chord)
                {
                    notesChords.Add(new TimeLinkedEntity { Time = chord.Time, Entity = chord });
                }
            }

            // sort the notes and chords by time
            notesChords.Sort((s1, s2) => s1.Time.CompareTo(s2.Time));

            // write empty header if no notes or chords
            if (notesChords.Count == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {                
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
                    w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Sustain : (Single)0);

                    // bend
                    w.Write(notesChords[i].Entity.GetType() == typeof(SongNote) ? ((SongNote)notesChords[i].Entity).Bend : (Int32)0);

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
                    //w.Write(new byte[4]); //unknown int
                    w.Write(Convert.ToInt16(246));  // wrong
                    w.Write(Convert.ToInt16(7472)); // wrong

                    // phrase iteration start index and id ????
                    bool phraseStartIterationFound = false;
                    for (int p = 0; p < phraseIterations.PhraseIteration.Length; p++)
                    {
                        Single phraseIterationStart = phraseIterations.PhraseIteration[p].Time;
                        Single phraseIterationEnd = (p == phraseIterations.PhraseIteration.Length) ? songLength : phraseIterations.PhraseIteration[p + 1].Time;

                        if (notesChords[i].Time >= phraseIterationStart && notesChords[i].Time < phraseIterationEnd)
                        {
                            w.Write(p); // phrase iteration
                            w.Write(phraseIterations.PhraseIteration[p].PhraseId);
                            phraseStartIterationFound = true;
                            break;
                        }
                    }
                    if (!phraseStartIterationFound)
                        throw new Exception(string.Format("No phrase start iteration found with matching time for note {0}.", i.ToString()));
                }
            }
        }

        // INCOMPLETE
        private void writeRocksmithSngMetaDetails(EndianBinaryWriter w, Song s)
        {
            // data below is garbage... just borrowed from a random song to get the charts working.
            w.Write(1); // unknown           
            w.Write(new byte[4]); // unknown empty in one example
            w.Write(1075970048); // unknown 16418 or 2.53125 in one example
            w.Write(new byte[4]); // unknown empty in one example
            w.Write(1086698382); // unknown float 6.1782 in one example           
            w.Write(954437177); // unknown float 0.0001 in one example            
            w.Write(1056964611); // unknown float 0.5 in one example
            
            // not first phraseIteration time
            // not first section time
            // is first beat time?
            w.Write(s.Ebeats.Ebeat[0].Time); 
            
            // song conversion date
            foreach (char c in s.LastConversionDateTime.ToCharArray())
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - s.LastConversionDateTime.Length]); //pad to 32 bytes

            // song title
            foreach (char c in s.Title.ToCharArray())
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[64 - s.Title.Length]); // pad to 64 bytes

            // arrangement
            foreach (char c in s.Arrangement.ToCharArray())
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - s.Arrangement.Length]); //pad to 32 bytes

            // artist
            string artistValue = string.IsNullOrEmpty(s.Artist) ? "DUMMY" : s.Artist;
            foreach (char c in artistValue.ToCharArray())
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - artistValue.Length]); //pad to 32 bytes                       

            // song part
            w.Write(s.Part);

            // song length
            w.Write(s.SongLength);

            // unknown meta data
            w.Write(new byte[4]); // blank 4 bytes in NumberThirteen_Lead.sng
            w.Write(new byte[4]); // float with 10.2927 in NumberThirteen_Lead.sng
            w.Write(s.Ebeats.Ebeat[0].Time); // wrong; float with time of first beat; time of phraseid=1 (not 0) in somes examples;  float with time of first note where ignore <> 1
            w.Write(s.Ebeats.Ebeat[0].Time); // wrong; float with time of first beat; time of phraseid=1 (not 0) in somes examples;  float with time of first note where ignore <> 1
            w.Write(new byte[4]); // float with 1.9618 in NumberThirteen_Lead.sng....  number 4?

            // unknown section
            w.Write(new byte[4]); // header with repeating array; song works in game if array is defaulted to 0 count so will leave this alone for now

            w.Write(new byte[4]); // unknown trailing 4 bytes - section used in TCPowerChords_Lead.sng, need to look into this
        }
    }
}

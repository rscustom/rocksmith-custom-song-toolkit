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

                    //// UNKNOWN
                    ////w.Write(new byte[28]);

                    // UNKNOWN SECTION
                    // int32 = 6 = anchor count?
                    // unknown float
                    // 4 empty bytes
                    // unknown float
                    // 4 empty bytes
                    // unknown float
                    // unknown float

                    // float with 1st beat time minus 2nd beat time? (or offset)

                    // SONG META DATA
                    writeRocksmithSngMetaDetails(w, rocksmithSong);

                    // UNKNOWN SECTION
                    w.Write(new byte[4]); // header count for unknown section with repeating float/empty 4 bytes/int pattern

                    // UNKNOWN SECTION
                    w.Write(new byte[4]); // blank 4 bytes at end of all files reviewed so far
                }
            }
        }

        // COMPLETE
        private void writeRocksmithSngHeader(EndianBinaryWriter w)
        {
            w.Write(49); // version num?
        }

        // COMPLETE
        // Sample: begins at position 4 in NumberThirteen_Lead.sng
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

        // COMPLETE except hardcoded fields
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

                // unknown
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));

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
        // Sample: begins at position 7,664 in NumberThirteen_Lead.sng
        private void writeRocksmithSngPhraseIterations(EndianBinaryWriter w, SongPhraseIterations phraseIterations, Single songLength)
        {
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

        // INCOMPLETE
        private void writeRocksmithSngLinkedDiffs(EndianBinaryWriter w, SongLinkedDiffs phraseProperties)
        {
            w.Write(new byte[4]); // placeholder
        }

        // SKIP FOR NOW - SPECIAL IN GAME SONGS ONLY
        private void writeRocksmithSngControls(EndianBinaryWriter w, SongControls phraseProperties)
        {
            w.Write(new byte[4]); // placeholder
        }

        // COMPLETE
        // Sample: begins at position 8,172 in NumberThirteen_Lead.sng
        private void writeRocksmithSngEvents(EndianBinaryWriter w, SongEvents events)
        {
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
                w.Write(false);
                w.Write(false);
                w.Write(false);
                w.Write(false);
                w.Write(false);
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
                writeRocksmithSngLevelAnchors(w, levels.Level[i].Anchors, songLength);

                // chords placeholder
                w.Write(new byte[4]);

                // handshape placeholder
                w.Write(new byte[4]);

                // notes
                writeRocksmithSngLevelNotes(w, levels.Level[i].Notes);

                // header count for unknown section; total seems to match phrases (alignment stops working around 169,548 though)
                w.Write(new byte[4]); // send blank header until this gets sorted out
                //w.Write(phrases.Count);

                //// unknown child section @ 12,048
                ////32 bit single "1" x 9
                //for (int p = 0; p < 9; p++)
                //{
                //    w.Write(Convert.ToSingle(1));
                //}

                // fret hand mutes placeholder?
                w.Write(new byte[4]);

                // header count for unknown section; total seems to match phrase iterations
                w.Write(new byte[4]); // send blank header until this gets sorted out
                //w.Write(Convert.ToInt32(phraseIterations.Count));

                //// unknown child section
                //for (int p = 0; p < phraseIterations.Count; p++)
                //{
                //    if (p == (phraseIterations.Count - 1))
                //        w.Write(Convert.ToInt32(0));
                //    else
                //        w.Write(Convert.ToInt32(1));
                //}

                // header count for unknown section; total seems to match phrase iterations
                w.Write(new byte[4]); // send blank header until this gets sorted out
                //w.Write(phraseIterations.Count);

                //// unknown child section
                //for (int p = 0; p < phraseIterations.Count; p++)
                //{
                //    if (p == (phraseIterations.Count - 1))
                //        w.Write(Convert.ToInt32(0));
                //    else
                //        w.Write(Convert.ToInt32(1));
                //}

            }
        }

        // COMPLETE except hardcoded fields
        private void writeRocksmithSngLevelAnchors(EndianBinaryWriter w, SongAnchors anchors, Single songLength)
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

                    // unknown mid time @ 12,440; fix this:
                    if (i == anchors.Anchor.Length - 1)
                        w.Write(songLength - 3);
                    else
                        w.Write(anchors.Anchor[i + 1].Time - 3);

                    // fret
                    w.Write(anchors.Anchor[i].Fret);

                    // unknown mapped field
                    w.Write(new byte[4]); // hardcoded to empty for now
                }
            }
        }

        // COMPLETE except hardcoded fields
        private void writeRocksmithSngLevelNotes(EndianBinaryWriter w, SongNotes notes)
        {
            if (notes == null || notes.Note == null || notes.Note.Length == 0)
            {
                w.Write(new byte[4]); // empty header
                return;
            }
            else
            {
                // output notes header count
                w.Write(notes.Note.Length);

                // ouput notes
                for (int i = 0; i < notes.Note.Length; i++)
                {
                    // note time tag
                    w.Write(notes.Note[i].Time);

                    // string tag
                    w.Write(notes.Note[i].String);

                    // fret tag
                    w.Write(notes.Note[i].Fret);

                    // TBD
                    w.Write(Convert.ToInt32(-1));

                    // TBD
                    w.Write(Convert.ToInt32(-1));

                    // sustain tag
                    w.Write(notes.Note[i].Sustain);

                    // bend tag
                    w.Write(notes.Note[i].Bend);

                    // TBD
                    w.Write(Convert.ToInt32(-1));

                    // palm mute
                    w.Write(Convert.ToInt16(0)); // padding
                    w.Write(Convert.ToInt16(notes.Note[i].PalmMute));

                    // TBD
                    w.Write(Convert.ToInt32(0));

                    // TBD
                    w.Write(Convert.ToInt32(0));

                    // note index
                    w.Write(i);

                    // TBD - needs mapping
                    w.Write(Convert.ToInt32(0));

                }
            }
        }

        // INCOMPLETE
        private void writeRocksmithSngMetaDetails(EndianBinaryWriter w, Song s)
        {
            // float with time of first phraseId = 1 (found example where 1st phrase matched offset and was skipped over)?
            w.Write(new byte[4]); 
            
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
            w.Write(new byte[4]); // wrong; float with time of first beat; time of phraseid=1 (not 0) in somes examples
            w.Write(new byte[4]); // wrong; float with time of first beat; time of phraseid=1 (not 0) in somes examples
            w.Write(new byte[4]); // float with 1.9618 in NumberThirteen_Lead.sng....  number 4?
        }
    }
}

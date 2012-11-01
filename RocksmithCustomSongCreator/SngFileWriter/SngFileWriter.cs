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
                    // Example: begins at position 4 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngEbeats(w, rocksmithSong.Ebeats);

                    // PHRASES
                    // Phrases example: begins at position 7,208 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngPhrases(w, rocksmithSong.Phrases, rocksmithSong.PhraseIterations);

                    // CHORD TEMPLATES
                    this.writeRocksmithSngChordTemplates(w, rocksmithSong.ChordTemplates);

                    // FRET HAND MUTE TEMPLATE
                    this.writeRocksmithSngFretHandMuteTemplates(w, rocksmithSong.FretHandMuteTemplates);

                    // VOCALS TEMPLATE 
                    w.Write(new byte[4]); // placeholder

                    // PHRASE ITERATIONS
                    // Iteration example: begins at position 7,664 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngPhraseItersations(w, rocksmithSong.PhraseIterations, rocksmithSong.SongLength);

                    // PHRASE PROPERTY
                    w.Write(new byte[4]); // placeholder

                    // LINKED DIFFS
                    w.Write(new byte[4]); // placeholder

                    // CONTROLS
                    w.Write(new byte[4]); // placeholder

                    // EVENTS DATA
                    // Events example: begins at position 8,172 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngEvents(w, rocksmithSong.Events);

                    // SECTIONS DATA
                    // Sections example: begins at position 9,216 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngSections(w, rocksmithSong.Sections, rocksmithSong.Levels.Level[0].Notes, rocksmithSong.SongLength);

                    // LEVELS DATA [sample start @ pos 9,820 in NumberThirteen_Lead.sng]
                    this.writeRocksmithSngLevels(w, rocksmithSong.Levels, rocksmithSong.SongLength, rocksmithSong.Phrases, rocksmithSong.PhraseIterations);

                    // UNKNOWN
                    w.Write(new byte[28]);

                    w.Write(rocksmithSong.Ebeats.Ebeat[0].Time); // float with time of first beat; todo: fix this

                    // SONG META DATA
                    writeRocksmithSngMetaDetails(w, rocksmithSong);

                    // UNKNOWN
                    w.Write(new byte[4]); // blank 4 bytes in NumberThirteen_Lead.sng
                    w.Write(new byte[4]); // float with 10.2927 in NumberThirteen_Lead.sng
                    w.Write(rocksmithSong.Ebeats.Ebeat[0].Time); // float with time of first beat; todo: fix this
                    w.Write(rocksmithSong.Ebeats.Ebeat[0].Time); // float with time of first beat; todo: fix this
                    w.Write(new byte[4]); // float with 1.9618 in NumberThirteen_Lead.sng
                    w.Write(new byte[4]); // header count for unknown section with repeating float/empty 4 bytes/int pattern
                    w.Write(new byte[4]); // blank 4 bytes at EOF in NumberThirteen_Lead.sng

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
        private void writeRocksmithSngPhraseItersations(EndianBinaryWriter w, SongPhraseIterations phraseIterations, Single songLength)
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

        // COMPLETE
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
        private void writeRocksmithSngSections(EndianBinaryWriter w, SongSections sections, SongNotes zeroLevelNotes, Single songLength)
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

                // start note index (level-0) in this section (?)
                bool startNoteFound = false;
                for (int n = 0; n < zeroLevelNotes.Note.Length; n++)
                {
                    if (sections.Section[i].StartTime <= zeroLevelNotes.Note[n].Time)
                    {
                        w.Write(n);
                        startNoteFound = true;
                        break;
                    }
                }
                if (!startNoteFound)
                    throw new Exception(string.Format("No start note found with matching section time for section {0}.", i.ToString()));

                // end note index (level-0) in this section (?)                
                if (i == sections.Section.Length - 1) // if last section, default to last note
                {
                    w.Write(zeroLevelNotes.Note.Length);
                }
                else
                {
                    bool endNoteFound = false;
                    for (int n = 0; n < zeroLevelNotes.Note.Length; n++)
                    {
                        if (sections.Section[i + 1].StartTime <= zeroLevelNotes.Note[n].Time)
                        {
                            w.Write(Convert.ToInt32(n - 1));
                            endNoteFound = true;
                            break;
                        }
                    }
                    if (!endNoteFound)
                        throw new Exception(string.Format("No end note found with matching section time for section {0}.", i.ToString()));
                }

                // unknown attribute
                w.Write(new Byte[] { 1, 1, 1, 1 }); // this is wrong, not all records have this value                

                // unknown attributes @ 9,276
                w.Write(true); // this is wrong
                w.Write(false);
                w.Write(false);
                w.Write(false);

                // sample date for last flag
                //<section name="intro" startTime="11.969" number="1"/>		0, 3	true
                //<section name="verse" startTime="41.916" number="1"/>		4, 7	true
                //<section name="chorus" startTime="72.296" number="1"/>		8, 12	true
                //<section name="verse" startTime="100.615" number="2"/>		13, 16	false
                //<section name="chorus" startTime="131.157" number="2"/> 	17, 20	false
                //<section name="transition" startTime="157.094" number="1"/> 	21, 24	false
                //<section name="bridge" startTime="183.064" number="1"/> 	25, 28	false
                //<section name="transition" startTime="212.640" number="2"/> 	29, 31	true
                //<section name="verse" startTime="233.099" number="3"/> 		32, 35	true 
                //<section name="chorus" startTime="263.861" number="3"/> 	36, 40	false
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

                    // unknown time @ 12,440
                    w.Write(Convert.ToSingle(0)); // fix this

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
            // first beat time (?)
            
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
        }
    }
}

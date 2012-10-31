using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace RocksmithSngCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
            fd.FilterIndex = 1;
            fd.ShowDialog();
            inputXmlTextBox.Text = fd.FileName;
            outputFileTextBox.Text = inputXmlTextBox.Text.Substring(0, inputXmlTextBox.Text.Length - 4) + ".sng";
        }

        private void convertBtn_Click(object sender, EventArgs e)
        {
            if (vocalsRadioButton.Checked)
            {
                SongVocals vocals;
                XmlSerializer serializer = new XmlSerializer(typeof(SongVocals));
                StreamReader reader = new StreamReader(inputXmlTextBox.Text);
                vocals = (SongVocals)serializer.Deserialize(reader);
                reader.Close();

                if (littleEndianRadioBtn.Checked)
                    writeRocksmithVocalsFile(vocals, outputFileTextBox.Text, EndianBitConverter.Little);
                else
                    writeRocksmithVocalsFile(vocals, outputFileTextBox.Text, EndianBitConverter.Big);
            }
            else
            {
                song rocksmithSong;
                XmlSerializer serializer = new XmlSerializer(typeof(song));
                StreamReader reader = new StreamReader(inputXmlTextBox.Text);
                rocksmithSong = (song)serializer.Deserialize(reader);
                reader.Close();

                if (littleEndianRadioBtn.Checked)
                    writeRocksmithSngFile(rocksmithSong, outputFileTextBox.Text, EndianBitConverter.Little);
                else
                    writeRocksmithSngFile(rocksmithSong, outputFileTextBox.Text, EndianBitConverter.Big);
            }
            
            MessageBox.Show("Process Complete","File Creation Process");
        }

        // COMPLETE
        private void writeRocksmithVocalsFile(SongVocals vocals, string outputFile, EndianBitConverter bitConverter)
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

        private void writeRocksmithSngFile(song rocksmithSong, string outputFile, EndianBitConverter bitConverter)
        {
            Single songLength = Convert.ToSingle(rocksmithSong.songLength);

            // apply some basic validations on the imported xml song
            this.validateRocksmithSongXmlImport(rocksmithSong);

            // WRITE THE .SNG FILE
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                using (EndianBinaryWriter w = new EndianBinaryWriter(bitConverter, fs))                
                {
                    // HEADER
                    // Example: begins at position 0 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngHeader(w);

                    // EBEATS DATA
                    // Example: begins at position 4 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngEbeats(w, rocksmithSong.ebeats[0]);

                    // PHRASES
                    // Phrases example: begins at position 7,208 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngPhrases(w, rocksmithSong.phrases[0], rocksmithSong.phraseIterations[0]);

                    // CHORD TEMPLATES
                    this.writeRocksmithSngChordTemplates(w, rocksmithSong.chordTemplates[0]);

                    // FRET HAND MUTE TEMPLATE
                    this.writeRocksmithSngFretHandMuteTemplates(w, rocksmithSong.fretHandMuteTemplates[0]);

                    // VOCALS TEMPLATE
                    w.Write(new byte[4]); // placeholder
                    //this.writeRocksmithSngVocals(w, rocksmithSong.vocals[0]);

                    // PHRASE ITERATIONS
                    // Iteration example: begins at position 7,664 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngPhraseItersations(w, rocksmithSong.phraseIterations[0], songLength);

                    // PLACEHOLDER (?)
                    w.Write(new byte[12]);

                    // EVENTS DATA
                    // Events example: begins at position 8,172 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngEvents(w, rocksmithSong.events[0]);

                    // SECTIONS DATA
                    // Sections example: begins at position 9,216 in NumberThirteen_Lead.sng
                    this.writeRocksmithSngSections(w, rocksmithSong.sections[0], rocksmithSong.levels[0].level[0].notes[0], songLength);

                    // PLACEHOLDER (?)
                    //w.Write(new byte[12]);

                    // PLACEHOLDER (?)
                    //w.Write(new byte[12]);

                    // LEVELS DATA [sample start @ pos 9,820 in NumberThirteen_Lead.sng]
                    this.writeRocksmithSngLevels(w, rocksmithSong.levels[0], songLength, rocksmithSong.phrases[0], rocksmithSong.phraseIterations[0]);

                    // SONG META DATA
                    writeRocksmithSngMetaDetails(w, rocksmithSong);
                }
            }
        }
        
        private void validateRocksmithSongXmlImport(song rocksmithSong)
        {
            if (rocksmithSong.ebeats == null || rocksmithSong.ebeats.Length != 1)
                throw new Exception("Missing or unexpected ebeats data format encountered.");

            if (rocksmithSong.phrases == null || rocksmithSong.phrases.Length != 1)
                throw new Exception("Missing or unexpected phrases data format encountered.");

            if (rocksmithSong.chordTemplates == null || rocksmithSong.chordTemplates.Length != 1)
                throw new Exception("Missing or unexpected chord templates data format encountered.");

            if (rocksmithSong.fretHandMuteTemplates == null || rocksmithSong.fretHandMuteTemplates.Length != 1)
                throw new Exception("Missing or unexpected fret hand mute templates data format encountered.");

            //if (rocksmithSong.vocals == null || rocksmithSong.vocals.Length != 1)
            //    throw new Exception("Missing or unexpected vocals data format encountered.");

            if (rocksmithSong.phraseIterations == null || rocksmithSong.phraseIterations.Length != 1)
                throw new Exception("Missing or unexpected phrase iterations data format encountered.");

            if (rocksmithSong.events == null || rocksmithSong.events.Length != 1)
                throw new Exception("Missing or unexpected song events data format encountered.");

            if (rocksmithSong.sections == null || rocksmithSong.sections.Length != 1)
                throw new Exception("Missing or unexpected song sections data format encountered.");

            if (rocksmithSong.levels == null || rocksmithSong.levels.Length != 1)
                throw new Exception("Missing or unexpected song levels data encountered.");
        }

        // COMPLETE
        private void writeRocksmithSngHeader(EndianBinaryWriter w)
        {
            w.Write(49); // version num?
        }

        // COMPLETE
        private void writeRocksmithSngEbeats(EndianBinaryWriter w, songEbeats ebeats)
        {
            // output header count
            w.Write(Convert.ToInt32(ebeats.count));

            Int16 currentMeasure = -1;
            Int16 currentMeasureBeat = -1;

            // output ebeats
            for (int i = 0; i < ebeats.ebeat.Length; i++)
            {
                // ebeat time
                w.Write(Convert.ToSingle(ebeats.ebeat[i].time));

                // measure
                Int16 measure = Convert.ToInt16(ebeats.ebeat[i].measure);
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

        // COMPLETE (although solo and ignore tags are not supported yet)
        private void writeRocksmithSngPhrases(EndianBinaryWriter w, songPhrases phrases, songPhraseIterations phraseIteration)
        {
            // output header count
            w.Write(Convert.ToInt32(phrases.count));

            // output phrases
            for (int i = 0; i < phrases.phrase.Length; i++)
            {
                // disparity
                if (phrases.phrase[i].disparity == "1")
                    w.Write(Convert.ToInt32(256));
                else
                    w.Write(Convert.ToInt32(0));

                // maxDifficulty tag
                w.Write(Convert.ToInt32(phrases.phrase[i].maxDifficulty));

                // count of usage in iterations
                int phraseIterationCount = 0;
                for (int i2 = 0; i2 < phraseIteration.phraseIteration.Length; i2++)
                {
                    if (phraseIteration.phraseIteration[i2].phraseId == i.ToString())
                    {
                        phraseIterationCount++;
                    }
                }
                w.Write(Convert.ToInt32(phraseIterationCount));

                // name tag
                string name = phrases.phrase[i].name;
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

        // INCOMPLETE (missing mapping for 6 attributes between finger positions and chord name)
        private void writeRocksmithSngChordTemplates(EndianBinaryWriter w, songChordTemplates chordTemplates)
        {
            // output header count
            w.Write(Convert.ToInt32(chordTemplates.count));

            // output chord templates
            for (int i = 0; i < chordTemplates.chordTemplate.Length; i++)
            {
                // fret numbers
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].fret0));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].fret1));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].fret2));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].fret3));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].fret4));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].fret5));

                // finger positions
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].finger0));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].finger1));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].finger2));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].finger3));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].finger4));
                w.Write(Convert.ToInt32(chordTemplates.chordTemplate[i].finger5));
                
                // unknown
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));
                w.Write(Convert.ToInt32(-1));

                // chord name
                string name = chordTemplates.chordTemplate[i].chordName;
                foreach (char c in name.ToCharArray())
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after name
                w.Write(new byte[32 - name.Length]);                
            }                             
        }

        // WIP
        private void writeRocksmithSngFretHandMuteTemplates(EndianBinaryWriter w, songFretHandMuteTemplates fretHandMuteTemplates)
        {
            w.Write(new byte[4]); // placeholder
        }

        // WIP
        //private void writeRocksmithSngVocals(EndianBinaryWriter w, songVocals vocals)
        //{
        //    w.Write(new byte[4]); // placeholder
        //}

        // COMPLETE
        private void writeRocksmithSngPhraseItersations(EndianBinaryWriter w, songPhraseIterations phraseIteration, Single songLength)
        {            
            // output header count
            w.Write(Convert.ToInt32(phraseIteration.count));

            // output phrase iterations
            for (int i = 0; i < phraseIteration.phraseIteration.Length; i++)
            {
                // phrase id
                w.Write(Convert.ToInt32(phraseIteration.phraseIteration[i].phraseId));
                // start time
                w.Write(Convert.ToSingle(phraseIteration.phraseIteration[i].time));
                // end time
                if (i == phraseIteration.phraseIteration.Length - 1)
                    w.Write(songLength);
                else
                    w.Write(Convert.ToSingle(phraseIteration.phraseIteration[i + 1].time));
            }
        }

        // COMPLETE
        private void writeRocksmithSngEvents(EndianBinaryWriter w, songEvents events)
        {
            // output header count
            w.Write(Convert.ToInt32(events.count));

            // output events
            for (int i = 0; i < events.@event.Length; i++)
            {
                // event time
                w.Write(Convert.ToSingle(events.@event[i].time));
                // event code
                string eventCode = events.@event[i].code;
                foreach (char c in eventCode.ToCharArray())
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after event code
                w.Write(new byte[256 - eventCode.Length]);
            }
        }

        // INCOMPLETE - output fields appear correct, but hard coded unknown fields need mapping rule
        private void writeRocksmithSngSections(EndianBinaryWriter w, songSections sections, songLevelsLevelNotes zeroLevelNotes, Single songLength)
        {
            // output header count
            w.Write(Convert.ToInt32(sections.count));

            // output sections
            for (int i = 0; i < sections.section.Length; i++)
            {
                // section name
                string name = sections.section[i].name;
                foreach (char c in name.ToCharArray())
                {
                    w.Write(Convert.ToByte(c));
                }
                // padding after section name
                w.Write(new byte[32 - name.Length]);

                // number tag
                w.Write(Convert.ToInt32(sections.section[i].number));

                // start time
                w.Write(Convert.ToSingle(sections.section[i].startTime));

                // end time
                if (i == sections.section.Length - 1)
                    w.Write(Convert.ToSingle(songLength));
                else
                    w.Write(Convert.ToSingle(sections.section[i + 1].startTime));

                // first note index (level-0) in this section (?)
                bool startNoteFound = false;
                for (int n = 0; n < zeroLevelNotes.note.Length; n++)
                {
                    if (Convert.ToSingle(sections.section[i].startTime) <= Convert.ToSingle(zeroLevelNotes.note[n].time))
                    {
                        w.Write(Convert.ToInt32(n));
                        startNoteFound = true;
                        break;
                    }
                }
                if (!startNoteFound)
                    throw new Exception(string.Format("No start note found with matching section time for section {0}.", i.ToString()));
                
                // last note index (level-0) in this section (?)                
                if (i == sections.section.Length - 1) // if last section, default to last note
                {
                    w.Write(Convert.ToInt32(zeroLevelNotes.note.Length));
                }
                else
                {
                    bool endNoteFound = false;
                    for (int n = 0; n < zeroLevelNotes.note.Length; n++)
                    {
                        if (Convert.ToSingle(sections.section[i + 1].startTime) <= Convert.ToSingle(zeroLevelNotes.note[n].time))
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
        private void writeRocksmithSngLevels(EndianBinaryWriter w, songLevels levels, Single songLength, songPhrases phrases, songPhraseIterations phraseIterations)
        {            
            // output header count @ 9,820
            w.Write(Convert.ToInt32(levels.count));

            // output levels ~@ 9,960
            for (int i = 0; i < levels.level.Length; i++)
            {
                // level difficulty tag
                w.Write(Convert.ToInt32(levels.level[i].difficulty));

                // output anchors
                if (levels.level[i].anchors != null && levels.level[i].anchors.Length == 1 && levels.level[i].anchors[0].count != "0")
                {
                    // output anchors header count
                    w.Write(Convert.ToInt32(levels.level[i].anchors[0].count));

                    // ouput anchors
                    for (int i2 = 0; i2 < levels.level[i].anchors[0].anchor.Length; i2++)
                    {
                        // anchor start time
                        w.Write(Convert.ToSingle(levels.level[i].anchors[0].anchor[i2].time));

                        // anchor end time
                        if (i2 == levels.level[i].anchors[0].anchor.Length - 1)
                            w.Write(Convert.ToSingle(songLength));
                        else
                            w.Write(Convert.ToSingle(levels.level[i].anchors[0].anchor[i2 + 1].time));

                        // anchor mid time??? @ 12,440
                        w.Write(Convert.ToSingle(0)); // fix this

                        // fret
                        w.Write(Convert.ToInt32(levels.level[i].anchors[0].anchor[i2].fret));

                        // unknown mapped field
                        w.Write(Convert.ToInt32(0)); // hardcoded to 0 for now
                    }
                }

                // placeholder (probably for chords section)
                w.Write(Convert.ToInt32(0));

                // placeholder (probably for fretHandMutes section)
                w.Write(Convert.ToInt32(0));

                // output notes
                if (levels.level[i].notes != null && levels.level[i].notes.Length == 1 && levels.level[i].notes[0].count != "0")
                {
                    // output notes header count
                    w.Write(Convert.ToInt32(levels.level[i].notes[0].count));

                    // ouput notes
                    for (int i2 = 0; i2 < levels.level[i].notes[0].note.Length; i2++)
                    {
                        // note time tag
                        w.Write(Convert.ToSingle(levels.level[i].notes[0].note[i2].time));

                        // string tag
                        w.Write(Convert.ToInt32(levels.level[i].notes[0].note[i2].@string));

                        // fret tag
                        w.Write(Convert.ToInt32(levels.level[i].notes[0].note[i2].fret));

                        // TBD
                        w.Write(Convert.ToInt32(-1));

                        // TBD
                        w.Write(Convert.ToInt32(-1));

                        // sustain tag
                        w.Write(Convert.ToSingle(levels.level[i].notes[0].note[i2].sustain));

                        // bend tag
                        w.Write(Convert.ToInt32(levels.level[i].notes[0].note[i2].bend));

                        // TBD
                        w.Write(Convert.ToInt32(-1));

                        // palm mute
                        w.Write(Convert.ToInt16(0)); // padding
                        w.Write(Convert.ToInt16(levels.level[i].notes[0].note[i2].palmMute));

                        // TBD
                        w.Write(Convert.ToInt32(0));

                        // TBD
                        w.Write(Convert.ToInt32(0));

                        // note index
                        w.Write(i2);

                        // TBD - needs mapping
                        w.Write(Convert.ToInt32(0));

                    }

                }

                // total phrases
                w.Write(Convert.ToInt32(phrases.count));

                // unknown @ 12,048
                //32 bit single "1" x 9
                for (int p = 0; p < 9; p++)
                {
                    w.Write(Convert.ToSingle(1));
                }

                // unknown
                w.Write(Convert.ToInt32(0));

                // total phrase iterations
                w.Write(Convert.ToInt32(phraseIterations.count));

                // unknown attribute series
                for (int p = 0; p < Convert.ToInt32(phraseIterations.count); p++)
                {
                    if(p == Convert.ToInt32(phraseIterations.count) - 1)
                        w.Write(Convert.ToInt32(0));
                    else
                        w.Write(Convert.ToInt32(1));
                }

                // total phrase iterations
                w.Write(Convert.ToInt32(phraseIterations.count));

                // unknown attribute series
                for (int p = 0; p < Convert.ToInt32(phraseIterations.count); p++)
                {
                    if (p == Convert.ToInt32(phraseIterations.count) - 1)
                        w.Write(Convert.ToInt32(0));
                    else
                        w.Write(Convert.ToInt32(1));
                }

            }
        }

        // INCOMPLETE
        private void writeRocksmithSngMetaDetails(EndianBinaryWriter w, song s)
        {
            // song conversion date
            w.Write(s.lastConversionDateTime);
            w.Write(new byte[32 - s.lastConversionDateTime.Length]); //pad to 32 bytes

            // song title
            foreach (char c in s.title.ToCharArray())
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[64 - s.title.Length]); // pad to 64 bytes

            // arrangement
            foreach (char c in s.arrangement.ToCharArray())
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - s.arrangement.Length]); //pad to 32 bytes

            // artist
            string artistValue = string.IsNullOrEmpty(s.artist) ? "DUMMY" : s.artist;
            foreach (char c in artistValue.ToCharArray())
            {
                w.Write(Convert.ToByte(c));
            }
            w.Write(new byte[32 - artistValue.Length]); //pad to 32 bytes                       
            
            // song part
            w.Write(Convert.ToInt32(s.part));

            // song length
            w.Write(Convert.ToSingle(s.songLength));
        }
    }
}

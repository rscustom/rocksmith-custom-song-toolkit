using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using System.Xml.Serialization;
using System.Text;

namespace RocksmithToolkitLib.Sng
{
    public class Sng2014FileWriter
    {
        private static readonly int[] StandardMidiNotes = { 40, 45, 50, 55, 59, 64 };
        private static List<ChordNotes> cns = new List<ChordNotes>();

        public void Write(string inputFile, string outputFile)
        {
            using (FileStream fs = new FileStream(outputFile, FileMode.Create)) {
                BinaryWriter w = new BinaryWriter(fs);
                // SNG reader
                //var sng = new SngFile("SNG");
                // XML parser
                var sng = new Sng2014File();
                // TODO tuning needed to compute MIDI notes
                Int16[] tuning = { 0,0,0,0,0,0 };
                bool bass = false;
                readXml(inputFile, sng, tuning, bass);
                // writer
                sng.write(w);
            }
        }

        public void readXml(string inputFile, Sng2014File sng, Int16[] tuning, bool bass) {
            using (var reader = new StreamReader(inputFile)) {
                var serializer = new XmlSerializer(typeof(Song2014));
                var song = (Song2014)serializer.Deserialize(reader);
                parseEbeats(song, sng);
                parsePhrases(song, sng);
                parse_chords(song, sng, tuning, bass);
                // vocals will need different parse function
                sng.Vocals = new VocalSection();
                sng.Vocals.Vocals = new SngVocal2014[0];
                parsePhraseIterations(song, sng);
                parsePhraseExtraInfo(song, sng);
                parseNld(song, sng);
                parseActions(song, sng);
                parseEvents(song, sng);
                parseTones(song, sng);
                parseDNAs(song, sng);
                parseSections(song, sng);
                parseArrangements(song, sng);
                parseMetadata(song, sng, tuning);
                // this needs to be initialized after arrangements
                parseChordNotes(song, sng);
            }
        }

        public Int32 getMidiNote(Int16[] tuning, Byte str, Byte fret, bool bass)
        {
            if (fret == 255)
                return 255;
            Int32 note = StandardMidiNotes[str] + tuning[str] + fret - (bass? 12 : 0);
            return note;
        }
        
        public Int32 getMaxDifficulty(Song2014 xml)
        {
            var max = 0;
            foreach (var phrase in xml.Phrases)
                if (max < phrase.MaxDifficulty)
                    max = phrase.MaxDifficulty; 
            return max;
        }
        
        public void parseMetadata(Song2014 xml, Sng2014File sng, Int16[] tuning)
        {
            sng.Metadata = new Metadata2014();
            sng.Metadata.MaxScore = 100000;
            
            sng.Metadata.MaxDifficulty = getMaxDifficulty(xml);
            // max used level
            SongLevel2014 full = xml.Levels[sng.Metadata.MaxDifficulty];
            sng.Metadata.MaxNotesAndChords = full.Chords.Length + full.Notes.Length;
            sng.Metadata.Unk3_MaxNotesAndChords = sng.Metadata.MaxNotesAndChords;
            sng.Metadata.PointsPerNote = sng.Metadata.MaxScore / sng.Metadata.MaxNotesAndChords;
            sng.Metadata.FirstBeatLength = xml.Ebeats[1].Time - xml.Ebeats[0].Time;
            sng.Metadata.StartTime = xml.Offset * -1;
            sng.Metadata.CapoFretId = (xml.Capo == 0)? (Byte) 255 : xml.Capo;
            readString(xml.LastConversionDateTime, sng.Metadata.LastConversionDateTime);
            sng.Metadata.Part = xml.Part;
            sng.Metadata.SongLength = xml.SongLength;
            sng.Metadata.StringCount = 6;
            sng.Metadata.Tuning = new Int16[sng.Metadata.StringCount];
            sng.Metadata.Tuning = tuning;
            var start = xml.Sections[0].StartTime;
            sng.Metadata.Unk11_FirstSectionStartTime = start;
            sng.Metadata.Unk12_FirstSectionStartTime = start;
        }
        
        public void parseEbeats(Song2014 xml, Sng2014File sng)
        {
            sng.BPMs = new Sng.BpmSection();
            sng.BPMs.Count = xml.Ebeats.Length;
            sng.BPMs.BPMs = new SngBpm[sng.BPMs.Count];
            Int16 measure = 0;
            Int16 beat = 0;
            for (int i=0; i<sng.BPMs.Count; i++) {
                var ebeat = xml.Ebeats[i];
                var bpm = new SngBpm();
                bpm.Time = ebeat.Time;
                
                if (ebeat.Measure >= 0) {
                    measure = ebeat.Measure;
                    beat = 0;
                }
                else
                {
                    beat++;
                }
                
                bpm.Measure = measure;
                bpm.Beat = beat;
                
                for (int iter_id=0; iter_id<xml.PhraseIterations.Length; iter_id++) {
                    var iter = xml.PhraseIterations[iter_id];
                    if (iter.Time > bpm.Time) {
                        // we're one past current iteration
                        bpm.PhraseIteration = iter_id - 1;
                        break;
                    }
                }
                
                if (beat == 0) {
                    bpm.Mask |= 1;
                    if (measure % 2 == 0)
                        bpm.Mask |= 2;
                }
                sng.BPMs.BPMs[i] = bpm;
            }
        }
        
        public void readString(string From, Byte[] To) {
            var bytes = Encoding.ASCII.GetBytes(From);
            System.Buffer.BlockCopy(bytes, 0, To, 0, bytes.Length);
        }
        
        public void parse_chords(Song2014 xml, Sng2014File sng, Int16[] tuning, bool bass)
        {
            sng.Chords = new ChordSection();
            sng.Chords.Count = xml.ChordTemplates.Length;
            sng.Chords.Chords = new SngChord[sng.Chords.Count];
            
            for (int i=0; i<sng.Chords.Count; i++) {
                var chord = xml.ChordTemplates[i];
                var c = new SngChord();
                
                // TODO
                //"Mask",
                c.Frets[0] = (Byte) chord.Fret0;
                c.Frets[1] = (Byte) chord.Fret1;
                c.Frets[2] = (Byte) chord.Fret2;
                c.Frets[3] = (Byte) chord.Fret3;
                c.Frets[4] = (Byte) chord.Fret4;
                c.Frets[5] = (Byte) chord.Fret5;
                c.Fingers[0] = (Byte) chord.Finger0;
                c.Fingers[1] = (Byte) chord.Finger1;
                c.Fingers[2] = (Byte) chord.Finger2;
                c.Fingers[3] = (Byte) chord.Finger3;
                c.Fingers[4] = (Byte) chord.Finger4;
                c.Fingers[5] = (Byte) chord.Finger5;

                for (Byte s=0; s<6; s++)
                    c.Notes[s] = getMidiNote(tuning, s, c.Frets[s], bass);
                readString(chord.ChordName, c.Name);
                sng.Chords.Chords[i] = c;
            }
        }
        
        public void parseChordNotes(Song2014 xml, Sng2014File sng)
        {
            sng.ChordNotes = new Sng.ChordNotesSection();
            sng.ChordNotes.ChordNotes = cns.ToArray();
            sng.ChordNotes.Count = sng.ChordNotes.ChordNotes.Length;
        }
        
        public Int32 addChordNotes(SongChord2014 chord)
        {
            // TODO processing all chordnotes in all levels separately, but
            //      there is a lot of reuse going on in original files
            //      (probably if all attributes match)
            var c = new ChordNotes();
            
            for (int i=0; i<6; i++) {
                SongNote2014 n = null;
                foreach (var cn in chord.chordNotes) {
                    if (cn.String == i)
                    {
                        n = cn;
                        break;
                    }
                }
                
                // TODO helper function to translate XML element to note mask
                //"NoteMask",
                // TODO
                c.BendData[i] = new SngBendData();
                for (int j=0; j<32; j++)
                    c.BendData[i].BendData32[j] = new SngBendData32();
                c.StartFretId[i] = 255;
                c.EndFretId[i] = 255;
                // TODO just guessing
                if (n != null && n.SlideTo != -1) {
                    c.StartFretId[i] = n.Fret;
                    c.EndFretId[i] = (Byte) n.SlideTo;
                }
                // this appears to be always zero
                //"Unk_0"
            }

            Int32 id = cns.Count;
            cns.Add(c);
            return id;
        }
        
        public void parsePhrases(Song2014 xml, Sng2014File sng)
        {
            sng.Phrases = new PhraseSection();
            sng.Phrases.Count = xml.Phrases.Length;
            sng.Phrases.Phrases = new SngPhrase2014[sng.Phrases.Count];
            
            for (int i=0; i<sng.Phrases.Count; i++) {
                var phrase = xml.Phrases[i];
                var p = new SngPhrase2014();
                p.Solo = phrase.Solo;
                p.Disparity = phrase.Disparity;
                p.Ignore = phrase.Ignore;
                p.MaxDifficulty = phrase.MaxDifficulty;
                Int32 links = 0;
                
                foreach (var iter in xml.PhraseIterations)
                    if (iter.PhraseId == i)
                        links++;

                p.PhraseIterationLinks = links;
                readString(phrase.Name, p.Name);
                sng.Phrases.Phrases[i] = p;
            }
        }
        
        public void parsePhraseIterations(Song2014 xml, Sng2014File sng)
        {
            sng.PhraseIterations = new PhraseIterationSection();
            sng.PhraseIterations.Count = xml.PhraseIterations.Length;
            sng.PhraseIterations.PhraseIterations = new SngPhraseIteration2014[sng.PhraseIterations.Count];
            
            for (int i=0; i<sng.PhraseIterations.Count; i++) {
                var piter = xml.PhraseIterations[i];
                var p = new SngPhraseIteration2014();
                p.PhraseId = piter.PhraseId;
                p.StartTime = piter.Time;
                if (i+1 < sng.PhraseIterations.Count)
                    p.NextPhraseTime = xml.PhraseIterations[i+1].Time;
                else
                    p.NextPhraseTime = xml.SongLength;
                
                // TODO unknown meaning (rename in HSL and regenerate when discovered)
                //"Unk3",
                //"Unk4",
                //"Unk5"
                sng.PhraseIterations.PhraseIterations[i] = p;
            }
        }
        
        public void parsePhraseExtraInfo(Song2014 xml, Sng2014File sng)
        {
            sng.PhraseExtraInfo = new PhraseExtraInfoByLevelSection();
            sng.PhraseExtraInfo.Count = 0;
            sng.PhraseExtraInfo.PhraseExtraInfoByLevel = new SngPhraseExtraInfoByLevel[sng.PhraseExtraInfo.Count];

            for (int i=0; i<sng.PhraseExtraInfo.Count; i++) {
                // TODO
                //var extra = xml.?[i];
                var e = new SngPhraseExtraInfoByLevel();
                //"PhraseId",
                //"Difficulty",
                //"Empty",
                //"LevelJump",
                //"Redundant",
                //"Padding"
                sng.PhraseExtraInfo.PhraseExtraInfoByLevel[i] = e;
            }
        }
        
        public void parseNld(Song2014 xml, Sng2014File sng)
        {
            // TODO it is unclear whether LinkedDiffs affect RS2 SNG
            sng.NLD = new Sng.NLinkedDifficultySection();
            sng.NLD.Count = xml.NewLinkedDiff.Length;
            sng.NLD.NLinkedDifficulties = new SngNLinkedDifficulty[sng.NLD.Count];
            
            for (int i=0; i<sng.NLD.Count; i++) {
                var nld = xml.NewLinkedDiff[i];
                var n = new SngNLinkedDifficulty();
                // TODO Ratio attribute unused?
                n.LevelBreak = nld.LevelBreak;
                n.PhraseCount = nld.PhraseCount;
                n.NLD_Phrase = new Int32[n.PhraseCount];
                
                for (int j=0; j<n.PhraseCount; j++)
                {
                    Console.WriteLine("{0}", j);
                    n.NLD_Phrase[j] = nld.Nld_phrase[j].Id;
                }
                sng.NLD.NLinkedDifficulties[i] = n;
            }
        }
        
        public void parseActions(Song2014 xml, Sng2014File sng)
        {
            // there is no XML example, EOF does not support it either
            sng.Actions = new Sng.ActionSection();
            sng.Actions.Count = 0;
            sng.Actions.Actions = new SngAction[sng.Actions.Count];
            
            for (int i=0; i<sng.Actions.Count; i++) {
                //var action = xml.?[i];
                var a = new SngAction();
                //a.Time = action.Time;
                //read_string(action.ActionName, a.ActionName);
                sng.Actions.Actions[i] = a;
            }
        }
        
        public void parseEvents(Song2014 xml, Sng2014File sng)
        {
            sng.Events = new Sng.EventSection();
            sng.Events.Count = xml.Events.Length;
            sng.Events.Events = new SngEvent[sng.Events.Count];
            
            for (int i=0; i<sng.Events.Count; i++) {
                var evnt = xml.Events[i];
                var e = new SngEvent();
                e.Time = evnt.Time;
                readString(evnt.Code, e.EventName);
                sng.Events.Events[i] = e;
            }
        }
        
        // TODO empty for one tone songs, need to pass tone changes for more
        public void parseTones(Song2014 xml, Sng2014File sng)
        {
            sng.Tones = new Sng.ToneSection();
            sng.Tones.Count = 0;
            sng.Tones.Tones = new SngTone[sng.Tones.Count];
        }
        
        public void parseDNAs(Song2014 xml, Sng2014File sng)
        {
            sng.DNAs = new Sng.DnaSection();
            List<SngDna> dnas = new List<SngDna>();
            
            // TODO this is unclear
            // there can be less DNAs (ID 3 for start and ID 0 for end)
            // noguitar => 0
            // verse => 2?
            // chorus/hook/solo => 3?
            var id = -1;
            foreach (var section in xml.Sections) {
                var new_id = -1;
                switch (section.Name) {
                    case "noguitar":
                        new_id = 0;
                        break;
                    
                    // TODO disabled for now to match lesson DNAs
                    //case "verse":
                    //  new_id = 2;
                    //  break;
                    default:
                        new_id = 3;
                        break;
                }
                
                if (new_id == id)
                    continue;
                
                id = new_id;
                var dna = new SngDna();
                dna.Time = section.StartTime;
                dna.DnaId = id;
                dnas.Add(dna);
            }
            
            sng.DNAs.Dnas = dnas.ToArray();
            sng.DNAs.Count = sng.DNAs.Dnas.Length;
        }
        
        public void parseSections(Song2014 xml, Sng2014File sng)
        {
            sng.Sections = new SectionSection();
            sng.Sections.Count = xml.Sections.Length;
            sng.Sections.Sections = new SngSection[sng.Sections.Count];
            
            int p_id = 0;
            for (int i=0; i<sng.Sections.Count; i++) {
                var section = xml.Sections[i];
                var s = new SngSection();
                readString(section.Name, s.Name);
                s.Number = section.Number;
                s.StartTime = section.StartTime;

                if (i+1 < sng.Sections.Count)
                    s.EndTime = xml.Sections[i+1].StartTime;
                else
                    s.EndTime = xml.SongLength;

                s.StartPhraseIterationId = p_id;
                // find phrase iteration outside of section time
                for (int end = p_id+1; end<xml.PhraseIterations.Length; end++) {
                    if (xml.PhraseIterations[end].Time >= s.EndTime) {
                        // this p_id marks the start of the next section
                        p_id = end;
                        break;
                    }
                }
                
                s.EndPhraseIterationId = p_id - 1;
                // TODO unknown meaning (rename in HSL and regenerate when discovered)
                //"Unk12",
                //"Unk13",
                //"Unk14",
                //"Unk15",
                // these appear to be always zero
                //"Unk16_0",
                //"Unk17_0",
                //"Unk18_0",
                //"Unk19_0",
                //"Unk20_0"
                sng.Sections.Sections[i] = s;
            }
        }
        
        public void parseNote(Song2014 xml, SongNote2014 note, SngNotes n)
        {
            // TODO helper function to translate XML element to note mask
            //"NoteMask",
            // TODO unknown meaning (rename in HSL and regenerate when discovered)
            //"Unk1",
            n.Time = note.Time;
            n.StringIndex = note.String;
            // TODO this is an array, unclear how to do this
            //n.FretId = note.Fret;
            // this appears to be always 4
            n.Unk3_4 = 4;
            n.ChordId = 255;
            n.ChordNotesId = 255;
            
            // counting on phrase iterations to be sorted by time
            for (int i=0; i<xml.PhraseIterations.Length; i++)
                if (xml.PhraseIterations[i].Time > n.Time) {
                    n.PhraseIterationId = i-1;
                    n.PhraseId = xml.PhraseIterations[n.PhraseIterationId].PhraseId;
                }
            
            // TODO
            //"FingerPrintId",
            // TODO unknown meaning (rename in HSL and regenerate when discovered)
            //"Unk4",
            //"Unk5",
            //"Unk6",
            
            // TODO
            //"FingerId",
            n.PickDirection = (Byte) note.PickDirection;
            n.Slap = (Byte) note.Slap;
            n.Pluck = (Byte) note.Pluck;
            n.Vibrato = note.Vibrato;
            n.Sustain = note.Sustain;
            n.MaxBend = note.Bend;
            
            // TODO
            n.BendData = new Sng.BendDataSection();
            n.BendData.Count = 0;
            n.BendData.BendData = new SngBendData32[n.BendData.Count];
        }
        
        public void parseChord(Song2014 xml, SongChord2014 chord, SngNotes n, Int32 id)
        {
            // TODO helper function to translate XML element to note mask
            //"NoteMask",
            // TODO unknown meaning (rename in HSL and regenerate when discovered)
            //"Unk1",
            n.Time = chord.Time;
            n.StringIndex = 255;
            
            // TODO this is an array, unclear how to do this
            //"FretId",
            // this appears to be always 4
            n.Unk3_4 = 4;
            n.ChordId = chord.ChordId;
            n.ChordNotesId = id;

            // counting on phrase iterations to be sorted by time
            for (int i=0; i<xml.PhraseIterations.Length; i++)
                if (xml.PhraseIterations[i].Time > n.Time) {
                    n.PhraseIterationId = i-1;
                    n.PhraseId = xml.PhraseIterations[n.PhraseIterationId].PhraseId;
                }
            
            // TODO
            //"FingerPrintId",
            // TODO unknown meaning (rename in HSL and regenerate when discovered)
            //"Unk4",
            //"Unk5",
            //"Unk6",
            
            // TODO
            //"FingerId",
            n.PickDirection = 255;
            n.Slap = 255;
            n.Pluck = 255;
            // TODO are these always zero for chords and used only in chordnotes?
            n.Vibrato = 0;
            n.Sustain = 0;
            n.MaxBend = 0;
            n.BendData = new Sng.BendDataSection();
            n.BendData.Count = 0;
            n.BendData.BendData = new SngBendData32[n.BendData.Count];
        }
        
        public void parseArrangements(Song2014 xml, Sng2014File sng)
        {
            sng.Arrangements = new Sng.ArrangementSection();
            sng.Arrangements.Count = getMaxDifficulty(xml) + 1;
            sng.Arrangements.Arrangements = new SngArrangement[sng.Arrangements.Count];
            
            for (int i=0; i<sng.Arrangements.Count; i++) {
                var level = xml.Levels[i];
                var a = new SngArrangement();
                a.Difficulty = level.Difficulty;
                var anchors = new AnchorSection();
                anchors.Count = level.Anchors.Length;
                anchors.Anchors = new SngAnchor2014[anchors.Count];

                for (int j=0; j<anchors.Count; j++) {
                    var anchor = new SngAnchor2014();
                    anchor.StartBeatTime = level.Anchors[j].Time;
                    if (j+1 < anchors.Count)
                        anchor.EndBeatTime = level.Anchors[j+1].Time;
                    else
                        // last section = noguitar
                        anchor.EndBeatTime = xml.Sections[xml.Sections.Length-1].StartTime;
                    anchor.Unk3_StartBeatTime = anchor.StartBeatTime;
                    anchor.Unk4_StartBeatTime = anchor.StartBeatTime;
                    anchor.FretId = level.Anchors[j].Fret;
                    anchor.Width = (Int32) level.Anchors[j].Width;

                    // TODO
                    //"PhraseIterationId"
                    anchors.Anchors[j] = anchor;
                }
                
                a.Anchors = anchors;
                // TODO no idea what this is, there is no XML/SNG using it?
                a.AnchorExtensions = new Sng.AnchorExtensionSection();
                a.AnchorExtensions.Count = 0;
                a.AnchorExtensions.AnchorExtensions = new SngAnchorExtension[0];
                
                // TODO one for fretting hand and one for picking hand?
                //"Fingerprints1",
                a.Fingerprints1 = new Sng.FingerprintSection();
                a.Fingerprints1.Count = 0;
                a.Fingerprints1.Fingerprints = new SngFingerprint[0];
                //"Fingerprints2",
                a.Fingerprints2 = new Sng.FingerprintSection();
                a.Fingerprints2.Count = 0;
                a.Fingerprints2.Fingerprints = new SngFingerprint[0];
                // notes and chords sorted by time
                List<SngNotes> notes = new List<SngNotes>();
                foreach (var note in level.Notes) {
                    var n = new SngNotes();
                    parseNote(xml, note, n);
                    notes.Add(n);
                }
                
                foreach (var chord in level.Chords) {
                    var n = new SngNotes();
                    Int32 id = -1;
                    if (chord.chordNotes != null && chord.chordNotes.Length > 0)
                        id = addChordNotes(chord);
                    parseChord(xml, chord, n, id);
                    notes.Add(n);
                }
                
                a.Notes = new Sng.NotesSection();
                a.Notes.Count = notes.Count;
                notes.Sort((x, y) => x.Time.CompareTo(y.Time));
                a.Notes.Notes = notes.ToArray();
                a.PhraseCount = xml.Phrases.Length;
                // TODO
                a.AverageNotesPerIteration = new float[a.PhraseCount];
                // TODO
                a.PhraseIterationCount1 = 0;
                a.NotesInIteration1 = new Int32[a.PhraseIterationCount1];
                // this is a copy?
                a.PhraseIterationCount2 = a.PhraseIterationCount1;
                a.NotesInIteration2 = a.NotesInIteration1;
                sng.Arrangements.Arrangements[i] = a;
            }
        }
    }
}

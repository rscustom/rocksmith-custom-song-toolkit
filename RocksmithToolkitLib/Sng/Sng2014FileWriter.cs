using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Properties;
using System.Xml.Serialization;
using System.Text;
using System.Linq;

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014FileWriter {
        private static readonly int[] StandardMidiNotes = { 40, 45, 50, 55, 59, 64 };

        public static Sng2014File ReadVocals(string xmlFile)
        {
            var data = new MemoryStream(Resources.VOCALS_RS2);
            var sng = new Sng2014File(data);
            var xml = Vocals.LoadFromFile(xmlFile);
            Sng2014FileWriter.parseVocals(xml, sng);
            return sng;
        }

        public void ReadSong(Song2014 songXml, Sng2014File sngFile)
        {
            Int16[] tuning = {
                (Int16) songXml.Tuning.String0,
                (Int16) songXml.Tuning.String1,
                (Int16) songXml.Tuning.String2,
                (Int16) songXml.Tuning.String3,
                (Int16) songXml.Tuning.String4,
                (Int16) songXml.Tuning.String5,
            };
            parseEbeats(songXml, sngFile);
            parsePhrases(songXml, sngFile);
            parseChords(songXml, sngFile, tuning, songXml.Arrangement == "Bass");
            // vocals use different parse function
            sngFile.Vocals = new VocalSection();
            sngFile.Vocals.Vocals = new Vocal[0];
            parsePhraseIterations(songXml, sngFile);
            parsePhraseExtraInfo(songXml, sngFile);
            parseNLD(songXml, sngFile);
            parseActions(songXml, sngFile);
            parseEvents(songXml, sngFile);
            parseTones(songXml, sngFile);
            parseDNAs(songXml, sngFile);
            parseSections(songXml, sngFile);
            parseArrangements(songXml, sngFile);
            parseMetadata(songXml, sngFile, tuning);

            // this needs to be initialized after arrangements
            parseChordNotes(songXml, sngFile);
        }

        public static Int32 GetMidiNote(Int16[] tuning, Byte str, Byte fret, bool bass, int capo) {
            if (fret == unchecked((Byte) (-1)))
                return -1;
            Int32 note = StandardMidiNotes[str] + tuning[str] + fret - (bass ? 12 : 0);
            // catch unaccessible frets with capo
            if (capo > 0 && fret != 0 && fret < capo) {
                throw new InvalidDataException("Invalid XML data: Frets below capo fret are not playable");
            }
            // catch wrong capo template values
            if (capo > 0 && fret == capo) {
                throw new InvalidDataException("Invalid XML data: Capo frets should be defined as open strings");
            }
            // adjust note value for open strings with capo
            if (capo > 0 && fret == 0) {
               note += capo;
            }
            return note;
        }

        /// <summary>
        /// Showlights only.
        /// </summary>
        /// <param name="tuning"></param>
        /// <param name="crd"></param>
        /// <param name="bass"></param>
        /// <returns></returns>
        public static Int32 getChordNote(Int16[] tuning, SongChord2014 crd, SongChordTemplate2014[] handShape, bool bass, int capo)
        {
            if (handShape[crd.ChordId] != null)
            {
                List<int> cNote = new List<int>();                
                cNote.AddRange(new int[]{
                    GetMidiNote(tuning, (Byte)0, (Byte)handShape[crd.ChordId].Fret0, bass, capo),
                    GetMidiNote(tuning, (Byte)1, (Byte)handShape[crd.ChordId].Fret1, bass, capo),
                    GetMidiNote(tuning, (Byte)2, (Byte)handShape[crd.ChordId].Fret2, bass, capo),
                    GetMidiNote(tuning, (Byte)3, (Byte)handShape[crd.ChordId].Fret3, bass, capo),
                    GetMidiNote(tuning, (Byte)4, (Byte)handShape[crd.ChordId].Fret4, bass, capo),
                    GetMidiNote(tuning, (Byte)5, (Byte)handShape[crd.ChordId].Fret5, bass, capo)
                });
                //Cleanup for -1 notes
                var cOut = new List<int>();
                foreach (var c in cNote)
                    if (c > 0) cOut.Add(c);
                //Return bass note
                if (cOut.Count < 3 && cOut[0] > cOut[1])
                    return cOut[1];
                //Return most used note
                if (cOut.Count > 3)
                {
                    return cOut.Where(n => cOut.Any(t => t > n)).FirstOrDefault();
                }
                //Return bass note [2]
                else return cOut[0];
            } return 35;
        }

        private Int32 getMaxDifficulty(Song2014 xml) {
            var max = 0;
            foreach (var phrase in xml.Phrases)
                if (max < phrase.MaxDifficulty)
                    max = phrase.MaxDifficulty;
            return max;
        }

        // Easy, Medium, Hard = 0, 1, 2
        public int[] NoteCount { get; set; }
        private int getNoteCount(Sng2014File sng, int Level)
        {
            // time => note count
            var notes = new Dictionary<float,int>();
            var level = new Dictionary<float,int>();

            for (int i=sng.Arrangements.Count-1; i>=0; i--) {
                var a = sng.Arrangements.Arrangements[i];
                foreach (var n in a.Notes.Notes) {
                    if (i > sng.PhraseIterations.PhraseIterations[n.PhraseIterationId].Difficulty[Level])
                        // this note is above requested level
                        continue;

                    if (!notes.ContainsKey(n.Time)) {
                        // 1 note at difficulty i
                        notes[n.Time] = 1;
                        level[n.Time] = i;
                    } else if (i == level[n.Time]) {
                        // we can add notes while still in the same difficulty
                        notes[n.Time] += 1;
                    }
                }
            }

            int count = 0;
            foreach (var time_count in notes.Values)
                count += time_count;
            return count;
        }

        private void parseMetadata(Song2014 xml, Sng2014File sng, Int16[] tuning) {
            // Easy, Medium, Hard
            NoteCount = new int[3];
            NoteCount[0] = getNoteCount(sng, 0);
            NoteCount[1] = getNoteCount(sng, 1);
            NoteCount[2] = getNoteCount(sng, 2);

            sng.Metadata = new Metadata();
            sng.Metadata.MaxScore = 100000;

            sng.Metadata.MaxDifficulty = getMaxDifficulty(xml);
            sng.Metadata.MaxNotesAndChords = NoteCount[2];
            sng.Metadata.Unk3_MaxNotesAndChords = sng.Metadata.MaxNotesAndChords;
            sng.Metadata.PointsPerNote = sng.Metadata.MaxScore / sng.Metadata.MaxNotesAndChords;

            sng.Metadata.FirstBeatLength = xml.Ebeats[1].Time - xml.Ebeats[0].Time;
            sng.Metadata.StartTime = xml.Offset * -1;
            sng.Metadata.CapoFretId = (xml.Capo == 0) ? unchecked((Byte) (-1)) : xml.Capo;
            readString(xml.LastConversionDateTime, sng.Metadata.LastConversionDateTime);
            sng.Metadata.Part = xml.Part;
            sng.Metadata.SongLength = xml.SongLength;
            sng.Metadata.StringCount = 6;
            sng.Metadata.Tuning = new Int16[sng.Metadata.StringCount];
            sng.Metadata.Tuning = tuning;
            // calculated when parsing arrangements
            sng.Metadata.Unk11_FirstNoteTime = first_note_time;
            sng.Metadata.Unk12_FirstNoteTime = first_note_time;
        }

        private static Int32 getPhraseIterationId(Song2014 xml, float Time, bool end)
        {
            Int32 id = 0;
            while (id+1 < xml.PhraseIterations.Length) {
                if (!end && xml.PhraseIterations[id+1].Time > Time)
                    break;
                if (end && xml.PhraseIterations[id+1].Time >= Time)
                    break;
                ++id;
            }
            return id;
        }

        private void parseEbeats(Song2014 xml, Sng2014File sng) {
            sng.BPMs = new BpmSection();
            sng.BPMs.Count = xml.Ebeats.Length;
            sng.BPMs.BPMs = new Bpm[sng.BPMs.Count];
            Int16 measure = 0;
            Int16 beat = 0;
            for (int i = 0; i < sng.BPMs.Count; i++) {
                var ebeat = xml.Ebeats[i];
                var bpm = new Bpm();
                bpm.Time = ebeat.Time;
                if (ebeat.Measure >= 0) {
                    measure = ebeat.Measure;
                    beat = 0;
                } else {
                    beat++;
                }
                bpm.Measure = measure;
                bpm.Beat = beat;
                bpm.PhraseIteration = getPhraseIterationId(xml, bpm.Time, true);
                if (beat == 0) {
                    bpm.Mask |= 1;
                    if (measure % 2 == 0)
                        bpm.Mask |= 2;
                }
                sng.BPMs.BPMs[i] = bpm;
            }
        }

        private static void readString(string From, Byte[] To) {
            var bytes = Encoding.UTF8.GetBytes(From);
            System.Buffer.BlockCopy(bytes, 0, To, 0, bytes.Length);
        }

        private void parseChords(Song2014 xml, Sng2014File sng, Int16[] tuning, bool bass) {
            sng.Chords = new ChordSection();
            sng.Chords.Count = xml.ChordTemplates.Length;
            sng.Chords.Chords = new Chord[sng.Chords.Count];

            for (int i = 0; i < sng.Chords.Count; i++) {
                var chord = xml.ChordTemplates[i];
                var c = new Chord();
                if (chord.DisplayName.EndsWith("arp"))
                    c.Mask |= CHORD_MASK_ARPEGGIO;
                else if (chord.DisplayName.EndsWith("nop"))
                    c.Mask |= CHORD_MASK_NOP;
                c.Frets[0] = (Byte)chord.Fret0;
                c.Frets[1] = (Byte)chord.Fret1;
                c.Frets[2] = (Byte)chord.Fret2;
                c.Frets[3] = (Byte)chord.Fret3;
                c.Frets[4] = (Byte)chord.Fret4;
                c.Frets[5] = (Byte)chord.Fret5;
                c.Fingers[0] = (Byte)chord.Finger0;
                c.Fingers[1] = (Byte)chord.Finger1;
                c.Fingers[2] = (Byte)chord.Finger2;
                c.Fingers[3] = (Byte)chord.Finger3;
                c.Fingers[4] = (Byte)chord.Finger4;
                c.Fingers[5] = (Byte)chord.Finger5;
                for (Byte s = 0; s < 6; s++)
                    c.Notes[s] = GetMidiNote(tuning, s, c.Frets[s], bass, xml.Capo);
                readString(chord.ChordName, c.Name);
                sng.Chords.Chords[i] = c;
            }
        }

        private void parseChordNotes(Song2014 xml, Sng2014File sng) {
            sng.ChordNotes = new ChordNotesSection();
            sng.ChordNotes.ChordNotes = cns.ToArray();
            sng.ChordNotes.Count = sng.ChordNotes.ChordNotes.Length;
        }

        private static List<ChordNotes> cns = new List<ChordNotes>();
        private static Dictionary<UInt32,Int32> cnsId = new Dictionary<UInt32,Int32>();
        public Int32 addChordNotes(Sng2014File sng, SongChord2014 chord) {
            var c = new ChordNotes();
            for (int i = 0; i < 6; i++) {
                SongNote2014 n = null;
                foreach (var cn in chord.ChordNotes) {
                    if (cn.String == i) {
                        n = cn;
                        break;
                    }
                }
                // TODO need to figure out which masks are not applied
                c.NoteMask[i] = parseNoteMask(n, false);
                c.BendData[i] = new BendData();
                c.BendData[i].BendData32 = parseBendData(n, false);
                if (n != null && n.BendValues != null)
                    c.BendData[i].UsedCount = n.BendValues.Length;
                if (n != null) {
                    c.SlideTo[i] = (Byte)n.SlideTo;
                    c.SlideUnpitchTo[i] = (Byte)n.SlideUnpitchTo;
                } else {
                    c.SlideTo[i] = unchecked((Byte) (-1));
                    c.SlideUnpitchTo[i] = unchecked((Byte) (-1));
                }
                if (n != null)
                    c.Vibrato[i] = n.Vibrato;
            }

            UInt32 crc = sng.HashStruct(c);
            if (cnsId.ContainsKey(crc))
                return cnsId[crc];

            // don't export chordnotes if there are no techniques
            bool noTechniques = true;
            foreach (var m in c.NoteMask)
                if (m != 0) {
                    noTechniques = false;
                    break;
                }
            if (noTechniques)
                return -1;

            // add new ChordNotes instance
            Int32 id = cns.Count;
            cnsId[crc] = id;
            cns.Add(c);
            return cnsId[crc];
        }

        private void parsePhrases(Song2014 xml, Sng2014File sng) {
            sng.Phrases = new PhraseSection();
            sng.Phrases.Count = xml.Phrases.Length;
            sng.Phrases.Phrases = new Phrase[sng.Phrases.Count];

            for (int i = 0; i < sng.Phrases.Count; i++) {
                var phrase = xml.Phrases[i];
                var p = new Phrase();
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

        private void parsePhraseIterations(Song2014 xml, Sng2014File sng) {
            sng.PhraseIterations = new PhraseIterationSection();
            sng.PhraseIterations.Count = xml.PhraseIterations.Length;
            sng.PhraseIterations.PhraseIterations = new PhraseIteration[sng.PhraseIterations.Count];

            for (int i = 0; i < sng.PhraseIterations.Count; i++) {
                var piter = xml.PhraseIterations[i];
                var p = new PhraseIteration();
                p.PhraseId = piter.PhraseId;
                p.StartTime = piter.Time;
                if (i + 1 < sng.PhraseIterations.Count)
                    p.NextPhraseTime = xml.PhraseIterations[i + 1].Time;
                else
                    p.NextPhraseTime = xml.SongLength;
                // default to (0, 0, max)
                // they use Medium (previous) value if there is hero=3 missing
                p.Difficulty[2] = xml.Phrases[p.PhraseId].MaxDifficulty;
                if (piter.HeroLevels != null)
                    foreach (var h in piter.HeroLevels)
                        p.Difficulty[h.Hero-1] = h.Difficulty;
                sng.PhraseIterations.PhraseIterations[i] = p;
            }
        }

        private void parsePhraseExtraInfo(Song2014 xml, Sng2014File sng) {
            sng.PhraseExtraInfo = new PhraseExtraInfoByLevelSection();
            sng.PhraseExtraInfo.Count = 0;
            sng.PhraseExtraInfo.PhraseExtraInfoByLevel = new PhraseExtraInfoByLevel[sng.PhraseExtraInfo.Count];

            // probably leftover section from RS1 format, not used anymore
            // for (int i = 0; i < sng.PhraseExtraInfo.Count; i++) {
            //     //var extra = xml.?[i];
            //     var e = new PhraseExtraInfoByLevel();
            //     //"PhraseId",
            //     //"Difficulty",
            //     //"Empty",
            //     //"LevelJump",
            //     //"Redundant",
            //     //"Padding"
            //     sng.PhraseExtraInfo.PhraseExtraInfoByLevel[i] = e;
            // }
        }

        private void parseNLD(Song2014 xml, Sng2014File sng) {
            sng.NLD = new NLinkedDifficultySection();
            sng.NLD.Count = xml.NewLinkedDiff.Length;
            sng.NLD.NLinkedDifficulties = new NLinkedDifficulty[sng.NLD.Count];

            for (int i = 0; i < sng.NLD.Count; i++) {
                var nld = xml.NewLinkedDiff[i];
                var n = new NLinkedDifficulty();
                // TODO Ratio attribute unused?
                n.LevelBreak = nld.LevelBreak;
                n.PhraseCount = nld.PhraseCount;
                n.NLD_Phrase = new Int32[n.PhraseCount];
                for (int j = 0; j < n.PhraseCount; j++) {
                    n.NLD_Phrase[j] = nld.Nld_phrase[j].Id;
                }
                sng.NLD.NLinkedDifficulties[i] = n;
            }
        }

        private void parseActions(Song2014 xml, Sng2014File sng) {
            // there is no XML example, EOF does not support it either
            sng.Actions = new ActionSection();
            sng.Actions.Count = 0;
            sng.Actions.Actions = new Action[sng.Actions.Count];

            // no RS2 SNG is using this
            // for (int i = 0; i < sng.Actions.Count; i++) {
            //     //var action = xml.?[i];
            //     var a = new Action();
            //     //a.Time = action.Time;
            //     //read_string(action.ActionName, a.ActionName);
            //     sng.Actions.Actions[i] = a;
            // }
        }

        private void parseEvents(Song2014 xml, Sng2014File sng) {
            sng.Events = new EventSection();
            sng.Events.Count = xml.Events.Length;
            sng.Events.Events = new Event[sng.Events.Count];

            for (int i = 0; i < sng.Events.Count; i++) {
                var evnt = xml.Events[i];
                var e = new Event();
                e.Time = evnt.Time;
                readString(evnt.Code, e.EventName);
                sng.Events.Events[i] = e;
            }
        }

        private void parseTones(Song2014 xml, Sng2014File sng) {
            sng.Tones = new ToneSection();
            if (xml.Tones != null)
                sng.Tones.Count = xml.Tones.Length;
            else
                sng.Tones.Count = 0;

            sng.Tones.Tones = new Tone[sng.Tones.Count];
            for (int i = 0; i < sng.Tones.Count; i++) {
                var tn = xml.Tones[i];
                var t = new Tone();
                t.Time = tn.Time;
                    if (tn.Name == xml.ToneA)
                        t.ToneId = 0;
                    else if (tn.Name == xml.ToneB)
                        t.ToneId = 1;
                    else if (tn.Name == xml.ToneC)
                        t.ToneId = 2;
                    else if (tn.Name == xml.ToneD)
                        t.ToneId = 3;
                    else
                        throw new InvalidDataException("Undefined tone name: " + tn.Name);
                sng.Tones.Tones[i] = t;
            };
        }

        private static void parseVocals(Vocals xml, Sng2014File sng)
        {
            sng.Vocals = new VocalSection();
            sng.Vocals.Count = xml.Vocal.Length;
            sng.Vocals.Vocals = new Vocal[sng.Vocals.Count];

            for (int i = 0; i < sng.Vocals.Count; i++)
            {
                var vcl = xml.Vocal[i];
                var v = new Vocal();
                v.Time = vcl.Time;
                v.Note = vcl.Note;
                v.Length = vcl.Length;
                readString(vcl.Lyric, v.Lyric);
                sng.Vocals.Vocals[i] = v;
            }
        }

        // none, solo, riff, chord
        public int[] DNACount { get; set; }
        private void parseDNAs(Song2014 xml, Sng2014File sng) {
            sng.DNAs = new DnaSection();
            List<Dna> dnas = new List<Dna>();

            DNACount = new int[4];

            // based on events: dna_none=0, dna_solo=1, dna_riff=2, dna_chord=3
            foreach (var e in xml.Events) {
                Int32 id = -1;
                switch (e.Code) {
                case "dna_none":
                    id = 0;
                    break;
                case "dna_solo":
                    id = 1;
                    break;
                case "dna_riff":
                    id = 2;
                    break;
                case "dna_chord":
                    id = 3;
                    break;
                }

                if (id != -1) {
                    var dna = new Dna();
                    dna.Time = e.Time;
                    dna.DnaId = id;
                    DNACount[id] += 1;
                    dnas.Add(dna);
                }
            }

            sng.DNAs.Dnas = dnas.ToArray();
            sng.DNAs.Count = sng.DNAs.Dnas.Length;
        }

        private void parseSections(Song2014 xml, Sng2014File sng) {
            sng.Sections = new SectionSection();
            sng.Sections.Count = xml.Sections.Length;
            sng.Sections.Sections = new Section[sng.Sections.Count];

            for (int i = 0; i < sng.Sections.Count; i++) {
                var section = xml.Sections[i];
                var s = new Section();
                readString(section.Name, s.Name);
                s.Number = section.Number;
                s.StartTime = section.StartTime;
                if (i + 1 < sng.Sections.Count)
                    s.EndTime = xml.Sections[i + 1].StartTime;
                else
                    s.EndTime = xml.SongLength;
                s.StartPhraseIterationId = getPhraseIterationId(xml, s.StartTime, false);
                s.EndPhraseIterationId = getPhraseIterationId(xml, s.EndTime, true);
                for (int j=getMaxDifficulty(xml); j>=0; j--) {
                    // used string mask for section at all difficulty j
                    Byte mask = 0;
                    foreach (var note in xml.Levels[j].Notes)
                        if (note.Time >= s.StartTime && note.Time < s.EndTime) {
                            mask |= (Byte) (1 << note.String);
                        }
                    foreach (var chord in xml.Levels[j].Chords)
                        if (chord.Time >= s.StartTime && chord.Time < s.EndTime) {
                            var ch = xml.ChordTemplates[chord.ChordId];
                            if (ch.Fret0 != -1)
                                mask |= (Byte) (1 << 0);
                            if (ch.Fret1 != -1)
                                mask |= (Byte) (1 << 1);
                            if (ch.Fret2 != -1)
                                mask |= (Byte) (1 << 2);
                            if (ch.Fret3 != -1)
                                mask |= (Byte) (1 << 3);
                            if (ch.Fret4 != -1)
                                mask |= (Byte) (1 << 4);
                            if (ch.Fret5 != -1)
                                mask |= (Byte) (1 << 5);

                            if (mask == 0x3F)
                                break;
                        }

                    // use mask from next section if there are no notes
                    if (mask == 0 && j<getMaxDifficulty(xml))
                        mask = s.StringMask[j+1];

                    s.StringMask[j] = mask;
                }
                sng.Sections.Sections[i] = s;
            }
        }

        #region CONSTANTS

        // more constants: RocksmithSngHSL/RocksmithSng_constants.txt
        // unknown constant
        public const UInt32 NOTE_TURNING_BPM_TEMPO      = 0x00000004;

        // chord template Mask (displayName ends with "arp" or "nop")
        public const UInt32 CHORD_MASK_ARPEGGIO         = 0x00000001;
        public const UInt32 CHORD_MASK_NOP              = 0x00000002;

        // NoteFlags:
        public const UInt32 NOTE_FLAGS_NUMBERED         = 0x00000001;

        // NoteMask:
        public const UInt32 NOTE_MASK_UNDEFINED         = 0x0;
        // missing - not used in lessons/songs            0x01
        public const UInt32 NOTE_MASK_CHORD             = 0x02;
        public const UInt32 NOTE_MASK_OPEN              = 0x04;
        public const UInt32 NOTE_MASK_FRETHANDMUTE      = 0x08;
        public const UInt32 NOTE_MASK_TREMOLO           = 0x10;
        public const UInt32 NOTE_MASK_HARMONIC          = 0x20;
        public const UInt32 NOTE_MASK_PALMMUTE          = 0x40;
        public const UInt32 NOTE_MASK_SLAP              = 0x80;
        public const UInt32 NOTE_MASK_PLUCK             = 0x0100;
        public const UInt32 NOTE_MASK_POP               = 0x0100;
        public const UInt32 NOTE_MASK_HAMMERON          = 0x0200;
        public const UInt32 NOTE_MASK_PULLOFF           = 0x0400;
        public const UInt32 NOTE_MASK_SLIDE             = 0x0800;
        public const UInt32 NOTE_MASK_BEND              = 0x1000;
        public const UInt32 NOTE_MASK_SUSTAIN           = 0x2000;
        public const UInt32 NOTE_MASK_TAP               = 0x4000;
        public const UInt32 NOTE_MASK_PINCHHARMONIC     = 0x8000;
        public const UInt32 NOTE_MASK_VIBRATO           = 0x010000;
        public const UInt32 NOTE_MASK_MUTE              = 0x020000;
        public const UInt32 NOTE_MASK_IGNORE            = 0x040000;   // ignore=1
        public const UInt32 NOTE_MASK_LEFTHAND          = 0x00080000;
        public const UInt32 NOTE_MASK_RIGHTHAND         = 0x00100000;
        public const UInt32 NOTE_MASK_HIGHDENSITY       = 0x200000;
        public const UInt32 NOTE_MASK_SLIDEUNPITCHEDTO  = 0x400000;
        public const UInt32 NOTE_MASK_SINGLE            = 0x00800000; // single note
        public const UInt32 NOTE_MASK_CHORDNOTES        = 0x01000000; // has chordnotes exported
        public const UInt32 NOTE_MASK_DOUBLESTOP        = 0x02000000;
        public const UInt32 NOTE_MASK_ACCENT            = 0x04000000;
        public const UInt32 NOTE_MASK_PARENT            = 0x08000000; // linkNext=1
        public const UInt32 NOTE_MASK_CHILD             = 0x10000000; // note after linkNext=1
        public const UInt32 NOTE_MASK_ARPEGGIO          = 0x20000000;
        // missing - not used in lessons/songs            0x40000000
        public const UInt32 NOTE_MASK_STRUM             = 0x80000000; // handShape defined at chord time

        public const UInt32 NOTE_MASK_ARTICULATIONS_RH  = 0x0000C1C0;
        public const UInt32 NOTE_MASK_ARTICULATIONS_LH  = 0x00020628;
        public const UInt32 NOTE_MASK_ARTICULATIONS     = 0x0002FFF8;
        public const UInt32 NOTE_MASK_ROTATION_DISABLED = 0x0000C1E0;

        #endregion

        private UInt32 parseNoteMask(SongNote2014 note, bool single) {
            if (note == null)
                return NOTE_MASK_UNDEFINED;

            // single note
            UInt32 mask = 0;

            if (single)
                mask |= NOTE_MASK_SINGLE;

            if (note.Fret == 0)
                mask |= NOTE_MASK_OPEN;

            if (note.LinkNext != 0)
                mask |= NOTE_MASK_PARENT;

            if (note.Accent != 0)
                mask |= NOTE_MASK_ACCENT;
            if (note.Bend != 0)
                mask |= NOTE_MASK_BEND;
            if (note.HammerOn != 0)
                mask |= NOTE_MASK_HAMMERON;
            if (note.Harmonic != 0)
                mask |= NOTE_MASK_HARMONIC;

            // TODO seems to have no effect
            // hopo = 0

            if (single && note.Ignore != 0)
                mask |= NOTE_MASK_IGNORE;
            if (single && note.LeftHand != -1)
                mask |= NOTE_MASK_LEFTHAND;
            if (note.Mute != 0)
                mask |= NOTE_MASK_MUTE;
            if (note.PalmMute != 0)
                mask |= NOTE_MASK_PALMMUTE;
            if (note.Pluck != -1)
                mask |= NOTE_MASK_PLUCK;
            if (note.PullOff != 0)
                mask |= NOTE_MASK_PULLOFF;
            if (note.Slap != -1)
                mask |= NOTE_MASK_SLAP;
            if (note.SlideTo != -1)
                mask |= NOTE_MASK_SLIDE;
            if (note.Sustain != 0)
                mask |= NOTE_MASK_SUSTAIN;
            if (note.Tremolo != 0)
                mask |= NOTE_MASK_TREMOLO;
            if (note.HarmonicPinch != 0)
                mask |= NOTE_MASK_PINCHHARMONIC;

            // TODO seems to have no effect
            // pickDirection = 0

            if (note.RightHand != -1)
                mask |= NOTE_MASK_RIGHTHAND;
            if (note.SlideUnpitchTo != -1)
                mask |= NOTE_MASK_SLIDEUNPITCHEDTO;
            if (note.Tap != 0)
                mask |= NOTE_MASK_TAP;
            if (note.Vibrato != 0)
                mask |= NOTE_MASK_VIBRATO;

            return mask;
        }

        private void parseNote(Song2014 xml, SongNote2014 note, Notes n, Notes prev) {
            n.NoteMask = parseNoteMask(note, true);
            // numbering (NoteFlags) will be set later
            n.Time = note.Time;
            n.StringIndex = note.String;
            // actual fret number
            n.FretId = (Byte) note.Fret;
            // anchor fret will be set later
            n.AnchorFretId = unchecked((Byte) (-1));
            // will be overwritten
            n.AnchorWidth = unchecked((Byte) (-1));
            n.ChordId = -1;
            n.ChordNotesId = -1;
            n.PhraseIterationId = getPhraseIterationId(xml, n.Time, false);
            n.PhraseId = xml.PhraseIterations[n.PhraseIterationId].PhraseId;
            // these will be overwritten
            n.FingerPrintId[0] = -1;
            n.FingerPrintId[1] = -1;
            // these will be overwritten
            n.NextIterNote = -1;
            n.PrevIterNote = -1;
            n.ParentPrevNote = -1;
            n.SlideTo = unchecked((Byte) note.SlideTo);
            n.SlideUnpitchTo = unchecked((Byte) note.SlideUnpitchTo);
            n.LeftHand = unchecked((Byte) note.LeftHand);
            // bvibrato and rchords8 are using 0 value but without TAP mask
            if (note.Tap != 0)
                n.Tap = unchecked((Byte) note.Tap);
            else
                n.Tap = unchecked((Byte) (-1));

            n.PickDirection = (Byte)note.PickDirection;
            n.Slap = (Byte)note.Slap;
            n.Pluck = (Byte)note.Pluck;
            n.Vibrato = note.Vibrato;
            n.Sustain = note.Sustain;
            n.MaxBend = note.Bend;
            n.BendData = new BendDataSection();
            n.BendData.BendData = parseBendData(note, true);
            n.BendData.Count = n.BendData.BendData.Length;
        }

        private void parseChord(Song2014 xml, Sng2014File sng, SongChord2014 chord, Notes n, Int32 chordNotesId) {
            n.NoteMask |= NOTE_MASK_CHORD;
            if (chordNotesId != -1) {
                // there should always be a STRUM too => handshape at chord time
                // probably even for chordNotes which are not exported to SNG
                n.NoteMask |= NOTE_MASK_CHORDNOTES;
            }

            if (chord.LinkNext != 0)
                n.NoteMask |= NOTE_MASK_PARENT;

            if (chord.Accent != 0)
                n.NoteMask |= NOTE_MASK_ACCENT;
            if (chord.FretHandMute != 0)
                n.NoteMask |= NOTE_MASK_FRETHANDMUTE;
            if (chord.HighDensity != 0)
                n.NoteMask |= NOTE_MASK_HIGHDENSITY;
            if (chord.Ignore != 0)
                n.NoteMask |= NOTE_MASK_IGNORE;
            if (chord.PalmMute != 0)
                n.NoteMask |= NOTE_MASK_PALMMUTE;
            // TODO does not seem to have a mask or any effect
            // if (chord.Hopo != 0)
            //     n.NoteMask |= ;

            // numbering will be set later
            //n.NoteFlags = NOTE_FLAGS_NUMBERED;

            n.Time = chord.Time;
            n.StringIndex = unchecked((Byte) (-1));
            // always -1
            n.FretId = unchecked((Byte) (-1));
            // anchor fret will be set later
            n.AnchorFretId = unchecked((Byte) (-1));
            // will be overwritten
            n.AnchorWidth = unchecked((Byte) (-1));
            n.ChordId = chord.ChordId;
            n.ChordNotesId = chordNotesId;
            n.PhraseIterationId = getPhraseIterationId(xml, n.Time, false);
            n.PhraseId = xml.PhraseIterations[n.PhraseIterationId].PhraseId;
            // these will be overwritten
            n.FingerPrintId[0] = -1;
            n.FingerPrintId[1] = -1;
            // these will be overwritten
            n.NextIterNote = -1;
            n.PrevIterNote = -1;
            // seems to be unused for chords
            n.ParentPrevNote = -1;
            n.SlideTo = unchecked((Byte) (-1));
            n.SlideUnpitchTo = unchecked((Byte) (-1));
            n.LeftHand = unchecked((Byte) (-1));
            n.Tap = unchecked((Byte) (-1));
            n.PickDirection = unchecked((Byte) (-1));
            n.Slap = unchecked((Byte) (-1));
            n.Pluck = unchecked((Byte) (-1));
            if (chord.ChordNotes != null) {
                foreach (var cn in chord.ChordNotes)
                    if (cn.Sustain > n.Sustain)
                        n.Sustain = cn.Sustain;
            }

            if (n.Sustain > 0)
                n.NoteMask |= NOTE_MASK_SUSTAIN;

            int cnt = 0;
            for (int str=0; str<6; str++)
                if (sng.Chords.Chords[chord.ChordId].Frets[str] != 255)
                    ++cnt;
            if (cnt == 2)
                n.NoteMask |= NOTE_MASK_DOUBLESTOP;

            // there are only zeros for all chords in lessons
            //n.Vibrato = 0;
            //n.MaxBend = 0;
            n.BendData = new BendDataSection();
            n.BendData.Count = 0;
            n.BendData.BendData = new BendData32[n.BendData.Count];
        }

        private BendData32[] parseBendData(SongNote2014 note, bool single) {
            // single can be any size, otherwise 32x BendData32 are allocated
            Int32 count = 32;

            // count of available values
            Int32 bend_values = 0;
            if (note != null && note.BendValues != null)
                bend_values = note.BendValues.Length;

            if (single) {
                count = bend_values;
            }

            var bd = new BendData32[count];
            for (int i=0; i<count; i++)
                bd[i] = new BendData32();

            // intentionally not using "count"
            for (int i=0; i<bend_values; i++) {
                var b = bd[i];
                b.Time = note.BendValues[i].Time;
                b.Step = note.BendValues[i].Step;
                // these are always zero for lessons and songs
                //"Unk3_0",
                //"Unk4_0",
                // TODO unknown meaning, added attribute to XML for testing
                //"Unk5"
                b.Unk5 = note.BendValues[i].Unk5;
            }

            return bd;
        }

        private float first_note_time = 0;
        private void parseArrangements(Song2014 xml, Sng2014File sng) {
            sng.Arrangements = new ArrangementSection();
            sng.Arrangements.Count = getMaxDifficulty(xml) + 1;
            sng.Arrangements.Arrangements = new Arrangement[sng.Arrangements.Count];

            // not strictly necessary but more helpful than hash value
            var note_id = new Dictionary<UInt32,UInt32>();

            for (int i = 0; i < sng.Arrangements.Count; i++) {
                var level = xml.Levels[i];
                var a = new Arrangement();
                a.Difficulty = level.Difficulty;

                var anchors = new AnchorSection();
                anchors.Count = level.Anchors.Length;
                anchors.Anchors = new Anchor[anchors.Count];
                for (int j = 0; j < anchors.Count; j++) {
                    var anchor = new Anchor();
                    anchor.StartBeatTime = level.Anchors[j].Time;
                    if (j + 1 < anchors.Count)
                        anchor.EndBeatTime = level.Anchors[j + 1].Time;
                    else
                        // last phrase iteration = noguitar/end
                        anchor.EndBeatTime = xml.PhraseIterations[xml.PhraseIterations.Length - 1].Time;
                    // TODO not 100% clear
                    // times will be updated later
                    // these garbage values are only in interactive lessons?
                    //anchor.Unk3_FirstNoteTime = (float) 3.4028234663852886e+38;
                    //anchor.Unk4_LastNoteTime = (float) 1.1754943508222875e-38;
                    anchor.FretId = (byte) level.Anchors[j].Fret;
                    anchor.Width = (Int32)level.Anchors[j].Width;
                    anchor.PhraseIterationId = getPhraseIterationId(xml, anchor.StartBeatTime, false);
                    anchors.Anchors[j] = anchor;
                }

                a.Anchors = anchors;
                // each slideTo will get anchor extension
                a.AnchorExtensions = new AnchorExtensionSection();

                foreach (var note in level.Notes)
                    if (note.SlideTo != -1)
                        ++a.AnchorExtensions.Count;

                a.AnchorExtensions.AnchorExtensions = new AnchorExtension[a.AnchorExtensions.Count];
                // Fingerprints1 is for handshapes without "arp" displayName
                a.Fingerprints1 = new FingerprintSection();
                // Fingerprints2 is for handshapes with "arp" displayName
                a.Fingerprints2 = new FingerprintSection();

                List<Fingerprint> fp1 = new List<Fingerprint>();
                List<Fingerprint> fp2 = new List<Fingerprint>();
                foreach (var h in level.HandShapes) {
                    var fp = new Fingerprint();
                    fp.ChordId = h.ChordId;
                    fp.StartTime = h.StartTime;
                    fp.EndTime = h.EndTime;
                    // TODO not always StartTime
                    //fp.Unk3_FirstNoteTime = fp.StartTime;
                    //fp.Unk4_LastNoteTime = fp.StartTime;
                    if(fp.ChordId < 0) continue;

                    if (xml.ChordTemplates[fp.ChordId].DisplayName.EndsWith("arp"))
                        fp2.Add(fp);
                    else
                        fp1.Add(fp);
                }

                a.Fingerprints1.Count = fp1.Count;
                a.Fingerprints1.Fingerprints = fp1.ToArray();
                a.Fingerprints2.Count = fp2.Count;
                a.Fingerprints2.Fingerprints = fp2.ToArray();

                // calculated as we go through notes, seems to work
                // NotesInIteration1 is count without ignore="1" notes
                a.PhraseIterationCount1 = xml.PhraseIterations.Length;
                a.NotesInIteration1 = new Int32[a.PhraseIterationCount1];
                // NotesInIteration2 seems to be the full count
                a.PhraseIterationCount2 = a.PhraseIterationCount1;
                a.NotesInIteration2 = new Int32[a.PhraseIterationCount2];

                // notes and chords sorted by time
                List<Notes> notes = new List<Notes>();
                int acent = 0;
                foreach (var note in level.Notes) {
                    var n = new Notes();
                    Notes prev = null;
                    if (notes.Count > 0)
                        prev = notes.Last();
                    parseNote(xml, note, n, prev);
                    notes.Add(n);

                    for (int j=0; j<xml.PhraseIterations.Length; j++) {
                        var piter = xml.PhraseIterations[j];
                        if (piter.Time > note.Time) {
                            if (note.Ignore == 0)
                                ++a.NotesInIteration1[j-1];
                            ++a.NotesInIteration2[j-1];
                            break;
                        }
                    }
                    if (note.SlideTo != -1) {
                        var ae = new AnchorExtension();
                        ae.FretId = (Byte) note.SlideTo;
                        ae.BeatTime = note.Time + note.Sustain;
                        a.AnchorExtensions.AnchorExtensions[acent++] = ae;
                    }
                }

                foreach (var chord in level.Chords) {
                    var cn = new Notes();
                    Int32 id = -1;
                    if (chord.ChordNotes != null && chord.ChordNotes.Length > 0)
                        id = addChordNotes(sng, chord);
                    parseChord(xml, sng, chord, cn, id);
                    notes.Add(cn);

                    for (int j=0; j<xml.PhraseIterations.Length; j++) {
                        var piter = xml.PhraseIterations[j];
                        if (chord.Time >= piter.Time && piter.Time >= chord.Time)
                        {
                            if (chord.Ignore == 0)
                                ++a.NotesInIteration1[j];
                            ++a.NotesInIteration2[j]; // j-1 not safe with j=0
                            break;
                        }
                    }
                }

                // need to be sorted before anchor note times are updated
                notes.Sort((x, y) => x.Time.CompareTo(y.Time));
                if (first_note_time == 0 || first_note_time > notes[0].Time)
                    first_note_time = notes[0].Time;

                foreach (var n in notes) {
                    for (Int16 id=0; id<fp1.Count; id++)
                        if (n.Time >= fp1[id].StartTime && n.Time < fp1[id].EndTime) {
                            n.FingerPrintId[0] = id;
                            // add STRUM to chords
                            if (fp1[id].StartTime == n.Time && n.ChordId != -1)
                                n.NoteMask |= NOTE_MASK_STRUM;
                            if (fp1[id].Unk3_FirstNoteTime == 0)
                                fp1[id].Unk3_FirstNoteTime = n.Time;

                            float sustain = 0;
                            if (n.Time + n.Sustain < fp1[id].EndTime)
                                sustain = n.Sustain;
                            fp1[id].Unk4_LastNoteTime = n.Time + sustain;
                            break;
                        }
                    for (Int16 id=0; id<fp2.Count; id++)
                        if (n.Time >= fp2[id].StartTime && n.Time < fp2[id].EndTime) {
                            n.FingerPrintId[1] = id;
                            // add STRUM to chords
                            if (fp2[id].StartTime == n.Time && n.ChordId != -1)
                                n.NoteMask |= NOTE_MASK_STRUM;
                            n.NoteMask |= NOTE_MASK_ARPEGGIO;
                            if (fp2[id].Unk3_FirstNoteTime == 0)
                                fp2[id].Unk3_FirstNoteTime = n.Time;

                            float sustain = 0;
                            if (n.Time + n.Sustain < fp2[id].EndTime)
                                sustain = n.Sustain;
                            fp2[id].Unk4_LastNoteTime = n.Time + sustain;
                            break;
                        }
                    for (int j=0; j<a.Anchors.Count; j++)
                        if (n.Time >= a.Anchors.Anchors[j].StartBeatTime && n.Time < a.Anchors.Anchors[j].EndBeatTime) {
                            n.AnchorWidth = (Byte) a.Anchors.Anchors[j].Width;
                            // anchor fret
                            n.AnchorFretId = (Byte) a.Anchors.Anchors[j].FretId;
                            if (a.Anchors.Anchors[j].Unk3_FirstNoteTime == 0)
                                a.Anchors.Anchors[j].Unk3_FirstNoteTime = n.Time;

                            float sustain = 0;
                            if (n.Time + n.Sustain < a.Anchors.Anchors[j].EndBeatTime - 0.1)
                                sustain = n.Sustain;
                            a.Anchors.Anchors[j].Unk4_LastNoteTime = n.Time + sustain;
                            break;
                        }
                }

                // initialize times for empty anchors, based on lrocknroll
                foreach (var anchor in a.Anchors.Anchors)
                    if (anchor.Unk3_FirstNoteTime == 0) {
                        anchor.Unk3_FirstNoteTime = anchor.StartBeatTime;
                        anchor.Unk4_LastNoteTime = anchor.StartBeatTime + (float)0.1;
                    }

                a.Notes = new NotesSection();
                a.Notes.Count = notes.Count;
                a.Notes.Notes = notes.ToArray();

                foreach (var piter in sng.PhraseIterations.PhraseIterations) {
                    int count = 0;
                    int j = 0;
                    for (; j<a.Notes.Count; j++) {
                        // skip notes outside of a phraseiteration
                        if (a.Notes.Notes[j].Time < piter.StartTime)
                            continue;
                        if (a.Notes.Notes[j].Time >= piter.NextPhraseTime) {
                            break;
                        }
                        // set to next arrangement note
                        a.Notes.Notes[j].NextIterNote = (Int16) (j+1);
                        // set all but first note to previous note
                        if (count > 0)
                            a.Notes.Notes[j].PrevIterNote = (Int16) (j-1);
                        ++count;
                    }
                    // fix last phrase note
                    if (count > 0)
                        a.Notes.Notes[j-1].NextIterNote = -1;
                }

                for (int j=1; j<a.Notes.Notes.Length; j++) {
                    var n = a.Notes.Notes[j]; var p = a.Notes.Notes[j-1]; int prvnote = 1; //set current + prev note + initialize prvnote variable
                    //do not do this searching for a parent, if the previous note timestamp != current time stamp
                    if (n.Time != p.Time) prvnote = 1;
                    else
                    {                    
                        for (int x = 1; x < (a.Notes.Notes.Length); x++) //search up till the begining of iteration 
                        {
                            if (j - x < 1) //dont search past the first note in iteration
                            {
                                prvnote = x; x = a.Notes.Notes.Length + 2; break; // stop searching for a match we reached the begining
                            }
                            var prv = a.Notes.Notes[j - x]; // get the info for the note we are checking against
                            if (prv.Time != n.Time) {//now check the timestamp if its the same timestamp then keep looking
                                if (prv.ChordId != -1) {//check if its a chord
                                    prvnote = x; x = a.Notes.Notes.Length + 2; break; //stop here, its a chord so dont need to check the strings
                                }
                                if (prv.StringIndex == n.StringIndex) {//check to see if we are looking at the same string
                                    prvnote = x; x = a.Notes.Notes.Length + 2; break; //stop here we found the same string, at a differnt timestamp, thats not a chord
                                }
                            }
                        }
                    }

                    var prev = a.Notes.Notes[j - prvnote]; //this will be either the first note of piter, or the last note on the same string at previous timestamp
                    if ((prev.NoteMask & NOTE_MASK_PARENT) != 0 ) {
                        n.ParentPrevNote = (short)(prev.NextIterNote - 1); n.NoteMask |= NOTE_MASK_CHILD; //set the ParentPrevNote# = the matched Note#//add CHILD flag
                        }
                }

                a.PhraseCount = xml.Phrases.Length;
                a.AverageNotesPerIteration = new float[a.PhraseCount];
                var iter_count = new float[a.PhraseCount];
                for (int j=0; j<xml.PhraseIterations.Length; j++) {
                    var piter = xml.PhraseIterations[j];
                    // using NotesInIteration2 to calculate
                    a.AverageNotesPerIteration[piter.PhraseId] += a.NotesInIteration2[j];
                    ++iter_count[piter.PhraseId];
                }

                for (int j=0; j<iter_count.Length; j++) {
                    if (iter_count[j] > 0)
                        a.AverageNotesPerIteration[j] /= iter_count[j];
                }

                // this is some kind of optimization in RS2 where they
                // hash all note data but their position in phrase iteration
                // to mark otherwise unchanged notes
                foreach (var n in a.Notes.Notes) {
                    MemoryStream data = sng.CopyStruct(n);
                    BinaryReader r = new BinaryReader(data);
                    var ncopy = new Notes();
                    ncopy.read(r);
                    ncopy.NextIterNote = 0;
                    ncopy.PrevIterNote = 0;
                    ncopy.ParentPrevNote = 0;
                    UInt32 crc = sng.HashStruct(ncopy);
                    if (!note_id.ContainsKey(crc))
                        note_id[crc] = (UInt32) note_id.Count;
                    n.Hash = note_id[crc];
                }

                numberNotes(sng, a.Notes.Notes);
                sng.Arrangements.Arrangements[i] = a;
            }
        }

        private void numberNotes(Sng2014File sng, Notes[] notes)
        {
            // current phrase iteration
            int p = 0;
            for (int o=0; o < notes.Length; o++) {
                var current = notes[o];

                // skip open strings
                if (current.FretId == 0) {
                    continue;
                }

                // are we past phrase iteration boundary?
                if (current.Time >= sng.PhraseIterations.PhraseIterations[p].NextPhraseTime) {
                    // advance and re-run
                    // will be repeated through empty iterations
                    ++p;
                    o = o-1;
                    continue;
                }

                bool repeat = false;
                int start = o-8;
                if (start < 0)
                    start = 0;
                // search last 8 notes
                for (int i=o-1; i>=start; i--) {
                    // ignore notes which are too far away
                    if (notes[i].Time + 2.0 < current.Time)
                        continue;
                    // ignore notes outside of iteration
                    if (notes[i].Time < sng.PhraseIterations.PhraseIterations[p].StartTime)
                        continue;

                    // count as repeat if this fret/chord was numbered recently
                    if ((current.ChordId == -1 && notes[i].FretId == current.FretId) ||
                        (current.ChordId != -1 && notes[i].ChordId == current.ChordId)) {
                        if ((notes[i].NoteFlags & NOTE_FLAGS_NUMBERED) != 0) {
                            repeat = true;
                            break;
                        }
                    }
                }

                // change
                if (!repeat)
                    current.NoteFlags |= NOTE_FLAGS_NUMBERED;
            }
        }
    }
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng;
using System.Xml.Serialization;
using System.Text;
using System.Linq;

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014FileWriter {
        private static readonly int[] StandardMidiNotes = { 40, 45, 50, 55, 59, 64 };
        private static List<ChordNotes> cns = new List<ChordNotes>();

        public void readXml(Song2014 songXml, Sng2014File sngFile, ArrangementType arrangementType)
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
            parseChords(songXml, sngFile, tuning, arrangementType == ArrangementType.Bass);
            // vocals will need different parse function
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

        private Int32 getMidiNote(Int16[] tuning, Byte str, Byte fret, bool bass) {
            if (fret == unchecked((Byte) (-1)))
                return -1;
            Int32 note = StandardMidiNotes[str] + tuning[str] + fret - (bass ? 12 : 0);
            return note;
        }

        private Int32 getMaxDifficulty(Song2014 xml) {
            var max = 0;
            foreach (var phrase in xml.Phrases)
                if (max < phrase.MaxDifficulty)
                    max = phrase.MaxDifficulty;
            return max;
        }

        private void parseMetadata(Song2014 xml, Sng2014File sng, Int16[] tuning) {
            sng.Metadata = new Metadata();
            sng.Metadata.MaxScore = 100000;

            sng.Metadata.MaxDifficulty = getMaxDifficulty(xml);
            // we need to track note times because of incremental arrangements
            // TODO this is not correct for e21_bwalking1, same timestamp
            sng.Metadata.MaxNotesAndChords = note_times.Count;
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
            var start = sng.Arrangements.Arrangements[sng.Metadata.MaxDifficulty].Notes.Notes[0].Time;
            sng.Metadata.Unk11_FirstNoteTime = start;
            sng.Metadata.Unk12_FirstNoteTime = start;
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

        private void readString(string From, Byte[] To) {
            var bytes = Encoding.ASCII.GetBytes(From);
            System.Buffer.BlockCopy(bytes, 0, To, 0, bytes.Length);
        }

        private Dictionary<Int32,SByte> chordFretId = new Dictionary<Int32,SByte>();
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
                // this value seems to be used in chord's FretId in Notes
                for (int j=0; j<6; j++) {
                    chordFretId[i] = (SByte) 0;
                    SByte FretId = unchecked((SByte) c.Frets[j]);
                    if (FretId > 0 && (SByte) chordFretId[i] > FretId)
                        chordFretId[i] = FretId;
                }
                c.Fingers[0] = (Byte)chord.Finger0;
                c.Fingers[1] = (Byte)chord.Finger1;
                c.Fingers[2] = (Byte)chord.Finger2;
                c.Fingers[3] = (Byte)chord.Finger3;
                c.Fingers[4] = (Byte)chord.Finger4;
                c.Fingers[5] = (Byte)chord.Finger5;
                for (Byte s = 0; s < 6; s++)
                    c.Notes[s] = getMidiNote(tuning, s, c.Frets[s], bass);
                readString(chord.ChordName, c.Name);
                sng.Chords.Chords[i] = c;
            }
        }

        private void parseChordNotes(Song2014 xml, Sng2014File sng) {
            sng.ChordNotes = new ChordNotesSection();
            sng.ChordNotes.ChordNotes = cns.ToArray();
            sng.ChordNotes.Count = sng.ChordNotes.ChordNotes.Length;
        }

        public Int32 addChordNotes(SongChord2014 chord) {
            // TODO processing all chordnotes in all levels separately, but
            //      there is a lot of reuse going on in original files
            //      (probably if all attributes match)
            var c = new ChordNotes();
            for (int i = 0; i < 6; i++) {
                SongNote2014 n = null;
                foreach (var cn in chord.chordNotes) {
                    if (cn.String == i) {
                        n = cn;
                        break;
                    }
                }
                // TODO guessing that NOTE mask is used here
                c.NoteMask[i] = parse_notemask(n, null);
                // TODO no XML example of chordnotes bend (like weezer)?
                c.BendData[i] = new BendData();
                for (int j = 0; j < 32; j++)
                    c.BendData[i].BendData32[j] = new BendData32();
                // TODO just guessing
                if (n != null && n.SlideTo != -1) {
                    c.StartFretId[i] = (Byte)n.Fret;
                    c.EndFretId[i] = (Byte)n.SlideTo;
                } else {
                    c.StartFretId[i] = unchecked((Byte) (-1));
                    c.EndFretId[i] = unchecked((Byte) (-1));
                }
                // this appears to be always zero
                //"Unk_0"
            }
            Int32 id = cns.Count;
            cns.Add(c);
            return id;
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
                // TODO how to choose difficulties?
                p.Easy = 0;
                p.Medium = 0;
                p.Hard = xml.Phrases[p.PhraseId].MaxDifficulty;
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
            // TODO there are no newLinkedDiffs produced by EOF XML
            if (xml.NewLinkedDiff == null) {
                sng.NLD = new NLinkedDifficultySection();
                sng.NLD.Count = 0;
                sng.NLD.NLinkedDifficulties = new NLinkedDifficulty[sng.NLD.Count];
                return;
            }
            // TODO it is unclear whether old LinkedDiffs affect RS2 SNG
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

        // TODO empty for one tone songs, need to pass tone changes for more
        private void parseTones(Song2014 xml, Sng2014File sng) {
            sng.Tones = new ToneSection();
            sng.Tones.Count = 0;
            sng.Tones.Tones = new Tone[sng.Tones.Count];
        }

        private void parseDNAs(Song2014 xml, Sng2014File sng) {
            sng.DNAs = new DnaSection();
            List<Dna> dnas = new List<Dna>();

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

                var dna = new Dna();
                dna.Time = section.StartTime;
                dna.DnaId = id;
                dnas.Add(dna);
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
                        }

                    // use mask from next section if there are no notes
                    if (mask == 0 && j<getMaxDifficulty(xml))
                        mask = s.StringMask[j+1];

                    s.StringMask[j] = mask;
                }
                sng.Sections.Sections[i] = s;
            }
        }


        // more constants: http://pastebin.com/Hn3LsP4X
        // unknown constant -- is this for field Unk3_4?
        const UInt32 NOTE_TURNING_BPM_TEMPO     = 0x00000004;

        // chord template Mask (displayName ends with "arp" or "nop")
        const UInt32 CHORD_MASK_ARPEGGIO        = 0x00000001;
        const UInt32 CHORD_MASK_NOP             = 0x00000002;

        // NoteFlags:
        const UInt32 NOTE_FLAGS_NUMBERED        = 0x00000001;

        // NoteMask:
        const UInt32 NOTE_MASK_UNDEFINED        = 0x0;
        // missing                                0x01  // not used in lessons
        const UInt32 NOTE_MASK_CHORD            = 0x02; // confirmed
        const UInt32 NOTE_MASK_OPEN             = 0x04; // confirmed
        const UInt32 NOTE_MASK_FRETHANDMUTE     = 0x08;
        const UInt32 NOTE_MASK_TREMOLO          = 0x10;
        const UInt32 NOTE_MASK_HARMONIC         = 0x20;
        const UInt32 NOTE_MASK_PALMMUTE         = 0x40;
        const UInt32 NOTE_MASK_SLAP             = 0x80;
        const UInt32 NOTE_MASK_PLUCK            = 0x0100;
        const UInt32 NOTE_MASK_POP              = 0x0100;
        const UInt32 NOTE_MASK_HAMMERON         = 0x0200;
        const UInt32 NOTE_MASK_PULLOFF          = 0x0400;
        const UInt32 NOTE_MASK_SLIDE            = 0x0800; // confirmed
        const UInt32 NOTE_MASK_BEND             = 0x1000;
        const UInt32 NOTE_MASK_SUSTAIN          = 0x2000; // confirmed
        const UInt32 NOTE_MASK_TAP              = 0x4000;
        const UInt32 NOTE_MASK_PINCHHARMONIC    = 0x8000;
        const UInt32 NOTE_MASK_VIBRATO          = 0x010000;
        const UInt32 NOTE_MASK_MUTE             = 0x020000;
        const UInt32 NOTE_MASK_IGNORE           = 0x040000; // confirmed, unknown meaning
        // missing                                0x080000 leftHand?
        // missing                                0x100000 // unknown meaning - used in btapping, bvibrato, rchords8, ... always with single note?
        const UInt32 NOTE_MASK_HIGHDENSITY      = 0x200000;
        const UInt32 NOTE_MASK_SLIDEUNPITCHEDTO = 0x400000;
        // missing                                0x800000 single note?
        // missing                                0x01000000 chord notes?
        const UInt32 NOTE_MASK_DOUBLESTOP       = 0x02000000;
        const UInt32 NOTE_MASK_ACCENT           = 0x04000000;
        const UInt32 NOTE_MASK_PARENT           = 0x08000000; // linkNext=1
        const UInt32 NOTE_MASK_CHILD            = 0x10000000; // note after linkNext=1
        const UInt32 NOTE_MASK_ARPEGGIO         = 0x20000000;
        // missing                                0x40000000 // not used in lessons
        const UInt32 NOTE_MASK_STRUM            = 0x80000000; // barre?

        const UInt32 NOTE_MASK_ARTICULATIONS_RH = 0x0000C1C0;
        const UInt32 NOTE_MASK_ARTICULATIONS_LH = 0x00020628;
        const UInt32 NOTE_MASK_ARTICULATIONS    = 0x0002FFF8;
        const UInt32 NOTE_MASK_ROTATION_DISABLED= 0x0000C1E0;

        // reverse-engineered values
        // single note mask?
        const UInt32 NOTE_MASK_SINGLE           = 0x00800000;
        // CHORD + STRUM + missing mask
        const UInt32 NOTE_MASK_CHORDNOTES       = 0x01000000;
        const UInt32 NOTE_MASK_LEFTHAND         = 0x00080000;

        public UInt32 parse_notemask(SongNote2014 note, Notes prev) {
            if (note == null)
                return NOTE_MASK_UNDEFINED;

            // single note
            UInt32 mask = NOTE_MASK_SINGLE;

            if (note.Fret == 0)
                mask |= NOTE_MASK_OPEN;

            // TODO some masks are not used here (open, arpeggio, chord, ...)
            //      and some are missing (unused attributes below)

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

            // TODO
            // hopo = 0
            //if (note. != 0)
            //  mask |= NOTE_MASK_;

            if (note.Ignore != 0)
                mask |= NOTE_MASK_IGNORE;
            if (note.LeftHand != -1)
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

            // TODO
            // pickDirection="0"
            //if (note. != 0)
            //  mask |= NOTE_MASK_;
            // rightHand="-1"
            //if (note. != 0)
            //  mask |= NOTE_MASK_;

            if (note.SlideUnpitchTo != -1)
                mask |= NOTE_MASK_SLIDEUNPITCHEDTO;
            if (note.Tap != 0)
                mask |= NOTE_MASK_TAP;
            if (note.Vibrato != 0)
                mask |= NOTE_MASK_VIBRATO;

            return mask;
        }

        private Int32 note_id = 1;
        private void parseNote(Song2014 xml, SongNote2014 note, Notes n, Notes prev) {
            // TODO unknown meaning of second mask
            n.NoteMask = parse_notemask(note, prev);
            // TODO when to set numbered note?
            n.NoteFlags = NOTE_FLAGS_NUMBERED;
            // TODO all notes get different id/hash for now
            n.Hash = note_id++;
            n.Time = note.Time;
            n.StringIndex = note.String;
            // actual fret number
            n.FretId[0] = (Byte) note.Fret;
            // TODO unknown, many times same, many times different, few times -1
            n.FretId[1] = (Byte) note.Fret;
            // this appears to be always 4
            n.Unk3_4 = 4;
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
            // TODO
            n.BendData = new BendDataSection();
            n.BendData.Count = 0;
            n.BendData.BendData = new BendData32[n.BendData.Count];
        }

        private void parseChord(Song2014 xml, Sng2014File sng, SongChord2014 chord, Notes n, Int32 id) {
            n.NoteMask |= NOTE_MASK_CHORD;
            if (id != -1)
                // TODO this seems to always add STRUM
                n.NoteMask |= NOTE_MASK_CHORDNOTES | NOTE_MASK_STRUM;

            if (chord.LinkNext != 0)
                n.NoteMask |= NOTE_MASK_PARENT;

            // TODO not checked against examples
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
            // TODO there are some weird values in there, how do they affect SNG?
            //      otherwise does not seem to have a mask
            // if (chord.Hopo != 0)
            //     n.NoteMask |= ;

            // TODO tried STRUM as barre or open chord indicator, but it's something else
            // var ch_tpl = xml.ChordTemplates[chord.ChordId];
            // if (ch_tpl.Fret0 == 0 || ch_tpl.Fret1 == 0 ||
            //     ch_tpl.Fret2 == 0 || ch_tpl.Fret3 == 0 ||
            //     ch_tpl.Fret4 == 0 || ch_tpl.Fret5 == 0) {
            //     n.NoteMask |= NOTE_MASK_STRUM;
            // }

            // TODO when to set numbered note?
            n.NoteFlags = NOTE_FLAGS_NUMBERED;

            // TODO all notes get different id/hash for now
            n.Hash = note_id++;
            n.Time = chord.Time;
            n.StringIndex = unchecked((Byte) (-1));
            // always -1
            n.FretId[0] = unchecked((Byte) (-1));
            // TODO seems to be always lowest non-zero fret
            n.FretId[1] = (Byte) chordFretId[chord.ChordId];
            // this appears to be always 4
            n.Unk3_4 = 4;
            n.ChordId = chord.ChordId;
            n.ChordNotesId = id;
            // counting on phrase iterations to be sorted by time
            for (int i = 0; i < xml.PhraseIterations.Length; i++)
                if (xml.PhraseIterations[i].Time > n.Time) {
                    n.PhraseIterationId = i - 1;
                    n.PhraseId = xml.PhraseIterations[n.PhraseIterationId].PhraseId;
                }
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
            if (chord.chordNotes != null) {
                foreach (var cn in chord.chordNotes)
                    if (cn.Sustain > n.Sustain)
                        n.Sustain = cn.Sustain;
            }
            
            if (n.Sustain > 0)
                n.NoteMask |= NOTE_MASK_SUSTAIN;

            if ((sng.Chords.Chords[chord.ChordId].Mask & CHORD_MASK_ARPEGGIO) != 0)
                n.NoteMask |= NOTE_MASK_ARPEGGIO;

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

        // used for counting total notes+chords (incremental arrangements)
        // TODO this does not include two notes at same time like in bwalking1,
        //      we should add both notes if they are in the same difficulty level
        //      but it would only work if higher diff. didn't override them
        private Hashtable note_times = new Hashtable();

        private void parseArrangements(Song2014 xml, Sng2014File sng) {
            sng.Arrangements = new ArrangementSection();
            sng.Arrangements.Count = getMaxDifficulty(xml) + 1;
            sng.Arrangements.Arrangements = new Arrangement[sng.Arrangements.Count];

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
                    anchor.Unk3_StartBeatTime = anchor.StartBeatTime;
                    anchor.Unk4_StartBeatTime = anchor.StartBeatTime;
                    anchor.FretId = level.Anchors[j].Fret;
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
                // TODO need to double check
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
                    fp.Unk3_StartTime = fp.StartTime;
                    fp.Unk4_StartTime = fp.StartTime;

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
                int aecnt = 0;
                foreach (var note in level.Notes) {
                    var n = new Notes();
                    Notes prev = null;
                    if (notes.Count > 0)
                        prev = notes.Last();
                    parseNote(xml, note, n, prev);
                    notes.Add(n);
                    note_times[note.Time] = note;
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
                        a.AnchorExtensions.AnchorExtensions[aecnt++] = ae;
                    }
                }
                foreach (var chord in level.Chords) {
                    var n = new Notes();
                    Int32 id = -1;
                    if (chord.chordNotes != null && chord.chordNotes.Length > 0)
                        id = addChordNotes(chord);
                    parseChord(xml, sng, chord, n, id);
                    notes.Add(n);
                    note_times[chord.Time] = chord;
                    for (int j=0; j<xml.PhraseIterations.Length; j++) {
                        var piter = xml.PhraseIterations[j];
                        if (piter.Time > chord.Time) {
                            if (chord.Ignore == 0)
                                ++a.NotesInIteration1[j-1];
                            ++a.NotesInIteration2[j-1];
                            break;
                        }
                    }
                }
                foreach (var n in notes) {
                    for (Int16 id=0; id<fp1.Count; id++)
                        if (n.Time >= fp1[id].StartTime) {
                            n.FingerPrintId[0] = id;
                            break;
                        }
                    for (Int16 id=0; id<fp2.Count; id++)
                        if (n.Time >= fp2[id].StartTime) {
                            n.FingerPrintId[1] = id;
                            break;
                        }
                }
                a.Notes = new NotesSection();
                a.Notes.Count = notes.Count;
                notes.Sort((x, y) => x.Time.CompareTo(y.Time));
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
                    var n = a.Notes.Notes[j];
                    var prev = a.Notes.Notes[j-1];

                    // set ParentPrevNote if previous note is linkNext or is at same timestamp
                    if ((prev.NoteMask & NOTE_MASK_PARENT) != 0 || prev.Time == n.Time) {
                        if (prev.ParentPrevNote == -1)
                            prev.ParentPrevNote = prev.PrevIterNote;
                        n.ParentPrevNote = prev.ParentPrevNote;
                    }

                    // add CHILD flag if previous note has linkNext
                    if ((prev.NoteMask & NOTE_MASK_PARENT) != 0)
                        n.NoteMask |= NOTE_MASK_CHILD;
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
                sng.Arrangements.Arrangements[i] = a;
            }
        }
    }
}

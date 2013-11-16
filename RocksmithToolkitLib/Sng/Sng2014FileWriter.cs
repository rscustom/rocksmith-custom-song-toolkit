using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng;
using System.Xml.Serialization;
using System.Text;

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014FileWriter {
        private static readonly int[] StandardMidiNotes = { 40, 45, 50, 55, 59, 64 };
        private static List<ChordNotes> cns = new List<ChordNotes>();
        private bool consoleMode = !Environment.UserInteractive;

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
            if (fret == 255)
                return 255;
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
            // max used level
            SongLevel2014 full = xml.Levels[sng.Metadata.MaxDifficulty];
            sng.Metadata.MaxNotesAndChords = full.Chords.Length + full.Notes.Length;
            sng.Metadata.Unk3_MaxNotesAndChords = sng.Metadata.MaxNotesAndChords;
            sng.Metadata.PointsPerNote = sng.Metadata.MaxScore / sng.Metadata.MaxNotesAndChords;

            sng.Metadata.FirstBeatLength = xml.Ebeats[1].Time - xml.Ebeats[0].Time;
            sng.Metadata.StartTime = xml.Offset * -1;
            sng.Metadata.CapoFretId = (xml.Capo == 0) ? (Byte)255 : xml.Capo;
            readString(xml.LastConversionDateTime, sng.Metadata.LastConversionDateTime);
            sng.Metadata.Part = xml.Part;
            sng.Metadata.SongLength = xml.SongLength;
            sng.Metadata.StringCount = 6;
            sng.Metadata.Tuning = new Int16[sng.Metadata.StringCount];
            sng.Metadata.Tuning = tuning;
            // TODO actually seems to be first chord/note time
            var start = sng.Arrangements.Arrangements[sng.Metadata.MaxDifficulty].Notes.Notes[0].Time;
            sng.Metadata.Unk11_FirstSectionStartTime = start;
            sng.Metadata.Unk12_FirstSectionStartTime = start;
        }

        private static Int32 getPhraseIterationId(Song2014 xml, float Time)
        {
            Int32 id = 0;
            while (id+1 < xml.PhraseIterations.Length &&
                   xml.PhraseIterations[id+1].Time <= Time) {
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
                bpm.PhraseIteration = getPhraseIterationId(xml, bpm.Time);
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

        private void parseChords(Song2014 xml, Sng2014File sng, Int16[] tuning, bool bass) {
            sng.Chords = new ChordSection();
            sng.Chords.Count = xml.ChordTemplates.Length;
            sng.Chords.Chords = new Chord[sng.Chords.Count];

            for (int i = 0; i < sng.Chords.Count; i++) {
                var chord = xml.ChordTemplates[i];
                var c = new Chord();
                // TODO
                //"Mask",
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
                c.NoteMask[i] = parse_notemask(n);
                // TODO no XML example on chordnote bend values?
                c.BendData[i] = new BendData();
                for (int j = 0; j < 32; j++)
                    c.BendData[i].BendData32[j] = new BendData32();
                // TODO just guessing
                if (n != null && n.SlideTo != -1) {
                    c.StartFretId[i] = (Byte)n.Fret;
                    c.EndFretId[i] = (Byte)n.SlideTo;
                } else {
                    c.StartFretId[i] = 255;
                    c.EndFretId[i] = 255;
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
                // TODO unknown meaning (rename in HSL and regenerate when discovered)
                //"Unk3",
                //"Unk4",
                //"Unk5"
                sng.PhraseIterations.PhraseIterations[i] = p;
            }
        }

        private void parsePhraseExtraInfo(Song2014 xml, Sng2014File sng) {
            sng.PhraseExtraInfo = new PhraseExtraInfoByLevelSection();
            sng.PhraseExtraInfo.Count = 0;
            sng.PhraseExtraInfo.PhraseExtraInfoByLevel = new PhraseExtraInfoByLevel[sng.PhraseExtraInfo.Count];

            for (int i = 0; i < sng.PhraseExtraInfo.Count; i++) {
                // TODO
                //var extra = xml.?[i];
                var e = new PhraseExtraInfoByLevel();
                //"PhraseId",
                //"Difficulty",
                //"Empty",
                //"LevelJump",
                //"Redundant",
                //"Padding"
                sng.PhraseExtraInfo.PhraseExtraInfoByLevel[i] = e;
            }
        }

        private void parseNLD(Song2014 xml, Sng2014File sng) {
            // TODO there are no newLinkedDiffs produced by EOF XML
            if (xml.NewLinkedDiff == null) {
                sng.NLD = new NLinkedDifficultySection();
                sng.NLD.Count = 0;
                sng.NLD.NLinkedDifficulties = new NLinkedDifficulty[sng.NLD.Count];
                return;
            }
            // TODO it is unclear whether LinkedDiffs affect RS2 SNG
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
                    if (consoleMode)
                        Console.WriteLine("{0}", j);
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

            for (int i = 0; i < sng.Actions.Count; i++) {
                //var action = xml.?[i];
                var a = new Action();
                //a.Time = action.Time;
                //read_string(action.ActionName, a.ActionName);
                sng.Actions.Actions[i] = a;
            }
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
                s.StartPhraseIterationId = getPhraseIterationId(xml, s.StartTime);
                s.EndPhraseIterationId = getPhraseIterationId(xml, s.EndTime);
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

        // NoteMask:
        const UInt32 NOTE_MASK_UNDEFINED        = 0x0;
        // missing                                0x01
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
        const UInt32 NOTE_MASK_SLIDE            = 0x0800;
        const UInt32 NOTE_MASK_BEND             = 0x1000;
        const UInt32 NOTE_MASK_SUSTAIN          = 0x2000;
        const UInt32 NOTE_MASK_TAP              = 0x4000;
        const UInt32 NOTE_MASK_PINCHHARMONIC    = 0x8000;
        const UInt32 NOTE_MASK_VIBRATO          = 0x010000;
        const UInt32 NOTE_MASK_MUTE             = 0x020000;
        const UInt32 NOTE_MASK_IGNORE           = 0x040000; // confirmed
        // missing                                0x080000
        // missing                                0x100000
        const UInt32 NOTE_MASK_HIGHDENSITY      = 0x200000;
        const UInt32 NOTE_MASK_SLIDEUNPITCHEDTO = 0x400000;
        // missing                                0x800000
        // missing                                0x01000000
        const UInt32 NOTE_MASK_DOUBLESTOP       = 0x02000000;
        const UInt32 NOTE_MASK_ACCENT           = 0x04000000;
        const UInt32 NOTE_MASK_PARENT           = 0x08000000;
        const UInt32 NOTE_MASK_CHILD            = 0x10000000;
        const UInt32 NOTE_MASK_ARPEGGIO         = 0x20000000;
        // missing                                0x40000000
        const UInt32 NOTE_MASK_STRUM            = 0x80000000;

        // reverse-engineered values
        // single note mask?
        const UInt32 NOTE_MASK_SINGLE           = 0x00800000;
        // CHORD + STRUM + missing mask
        const UInt32 NOTE_MASK_CHORDNOTES       = 0x01000000;
        public UInt32 parse_notemask(SongNote2014 note) {
            if (note == null)
                return NOTE_MASK_UNDEFINED;

            // single note
            UInt32 mask = NOTE_MASK_SINGLE;

            if (note.Fret == 0)
                mask |= NOTE_MASK_OPEN;

            // TODO some masks are not used here (open, arpeggio, chord, ...)
            //      and some are missing (unused attributes below)
            // linkNext = 0
            //if (note. != 0)
            //  mask |= NOTE_MASK_;

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

            // TODO
            // leftHand = -1
            //if (note. != 0)
            //  mask |= NOTE_MASK_;

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

        private void parseNote(Song2014 xml, SongNote2014 note, Notes n) {
            // TODO unknown meaning of second mask
            n.NoteMask[0] = parse_notemask(note);
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
            n.PhraseIterationId = getPhraseIterationId(xml, n.Time);
            n.PhraseId = xml.PhraseIterations[n.PhraseIterationId].PhraseId;
            // TODO
            //"FingerPrintId",
            // TODO unknown meaning (rename in HSL and regenerate when discovered)
            n.Unk4 = 255;
            n.Unk5 = 255;
            n.Unk6 = 255;
            // TODO
            //"FingerId",
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

        private void parseChord(Song2014 xml, SongChord2014 chord, Notes n, Int32 id) {
            n.NoteMask[0] |= NOTE_MASK_CHORD | NOTE_MASK_STRUM;
            if (id != -1)
                n.NoteMask[0] |= NOTE_MASK_CHORDNOTES;
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
            for (int i = 0; i < xml.PhraseIterations.Length; i++)
                if (xml.PhraseIterations[i].Time > n.Time) {
                    n.PhraseIterationId = i - 1;
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
            n.BendData = new BendDataSection();
            n.BendData.Count = 0;
            n.BendData.BendData = new BendData32[n.BendData.Count];
        }

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
                    anchor.PhraseIterationId = getPhraseIterationId(xml, anchor.StartBeatTime);
                    anchors.Anchors[j] = anchor;
                }
                a.Anchors = anchors;
                // TODO no idea what this is, there is no XML/SNG using it?
                a.AnchorExtensions = new AnchorExtensionSection();
                a.AnchorExtensions.Count = 0;
                a.AnchorExtensions.AnchorExtensions = new AnchorExtension[0];
                // TODO one for fretting hand and one for picking hand?
                //"Fingerprints1",
                a.Fingerprints1 = new FingerprintSection();
                a.Fingerprints1.Count = 0;
                a.Fingerprints1.Fingerprints = new Fingerprint[0];
                //"Fingerprints2",
                a.Fingerprints2 = new FingerprintSection();
                a.Fingerprints2.Count = 0;
                a.Fingerprints2.Fingerprints = new Fingerprint[0];
                // notes and chords sorted by time
                List<Notes> notes = new List<Notes>();
                foreach (var note in level.Notes) {
                    var n = new Notes();
                    parseNote(xml, note, n);
                    notes.Add(n);
                }
                foreach (var chord in level.Chords) {
                    var n = new Notes();
                    Int32 id = -1;
                    if (chord.chordNotes != null && chord.chordNotes.Length > 0)
                        id = addChordNotes(chord);
                    parseChord(xml, chord, n, id);
                    notes.Add(n);
                }
                a.Notes = new NotesSection();
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

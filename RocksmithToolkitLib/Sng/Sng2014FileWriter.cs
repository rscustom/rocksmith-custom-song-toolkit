using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.XML;
using MiscUtil.IO;
using MiscUtil.Conversion;
using CON = RocksmithToolkitLib.Sng.Constants;

// TODO: new philosophy ... charting tweaks should be done in EOF by the charter

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014FileWriter
    {
        private static readonly int[] StandardMidiNotes = { 40, 45, 50, 55, 59, 64 };

        public static Sng2014File ReadVocals(string xmlFile, string cdata = null)
        {
            Sng2014File sng;
            if (!String.IsNullOrEmpty(cdata))
                sng = new Sng2014File(new FileStream(cdata, FileMode.Open));
            else 
                sng = new Sng2014File(new MemoryStream(Resources.VOCALS_RS2));

            var xml = Vocals.LoadFromFile(xmlFile);
            parseVocals(xml, sng);

            return sng;
        }

        public void ReadSong(Song2014 songXml, Sng2014File sngFile)
        {
            // fix for 'Object reference not set to an instance of an object' error
            Int16[] tuning = { 0, 0, 0, 0, 0, 0 };
            try
            {
                tuning[0] = songXml.Tuning.String0;
                tuning[1] = songXml.Tuning.String1;
                tuning[2] = songXml.Tuning.String2;
                tuning[3] = songXml.Tuning.String3;
                tuning[4] = songXml.Tuning.String4;
                tuning[5] = songXml.Tuning.String5;
            }
            catch
            {
                // just ignore any error and use any tuning that is available from XML file
            }

            parseEbeats(songXml, sngFile);
            parsePhrases(songXml, sngFile);
            parseChords(songXml, sngFile, tuning, songXml.Arrangement == "Bass");
            // vocals use different parse function
            sngFile.Vocals = new VocalSection { Vocals = new Vocal[0] };
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

        public static Int32 GetMidiNote(Int16[] tuning, Byte str, Byte fret, bool bass, int capo)
        {
            return GetMidiNote(tuning, str, fret, bass, capo, false);
        }
        public static Int32 GetMidiNote(Int16[] tuning, Byte str, Byte fret, bool bass, int capo, bool template = false)
        {
            if (fret == unchecked((Byte)(-1)))
                return -1;
            // catch unaccessible frets with capo (sometimes there is unused templates, so let them be)
            if (capo > 0 && fret != 0 && (!template && fret < capo))
            {
                throw new InvalidDataException("Invalid XML data: Frets below capo fret are not playable");
            }
            // catch wrong capo template values
            if (capo > 0 && fret == capo && !template)
            {
                throw new InvalidDataException("Invalid XML data: Capo frets should be defined as open strings");
            }
            // get note value
            Int32 note = StandardMidiNotes[str] + tuning[str] + fret - (bass ? 12 : 0);
            // adjust note value for open strings with capo
            if (capo > 0 && fret == 0)
            {
                note += capo;
            }
            return note;
        }

        /// <summary>
        /// Showlights only.
        /// </summary>
        /// <param name="tuning"></param>
        /// <param name="crd"></param>
        /// <param name = "handShape"></param>
        /// <param name="bass"></param>
        /// <param name = "capo"></param>
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
                // Cleanup for -1 notes
                var cOut = cNote.Where(c => c > 0).ToList();
                if (cOut.Count < 1)
                    return 35;
                // Return bass note for doublestops
                if (cOut.Count < 3 && cOut[0] > cOut[1])
                    return cOut[1];
                // Return most used note
                return cOut.Count > 3 ? cOut.FirstOrDefault(n => cOut.Any(t => t > n)) : cOut[0];
            } 
            return 35;
        }

        private Int32 getMaxDifficulty(Song2014 xml)
        {
            var max = 0;
            foreach (var phrase in xml.Phrases)
                if (max < phrase.MaxDifficulty)
                    max = phrase.MaxDifficulty;
            return max;
        }

        // Easy, Medium, Hard = 0, 1, 2
        public int[] NoteCount { get; set; }

        // Number of ignored notes and chords when at max difficulty
        private int IgnoreCount;

        private int getNoteCount(Sng2014File sng, int Level)
        {
            // time => note count
            var notes = new Dictionary<float, int>();
            var level = new Dictionary<float, int>();

            for (int i = sng.Arrangements.Count - 1; i >= 0; i--)
            {
                var a = sng.Arrangements.Arrangements[i];
                foreach (var n in a.Notes.Notes)
                {
                    if (i != sng.PhraseIterations.PhraseIterations[n.PhraseIterationId].Difficulty[Level])
                        // This note is above or below requested level
                        continue;

                    if (!notes.ContainsKey(n.Time))
                    {
                        if (Level == 2 && (n.NoteMask & CON.NOTE_MASK_IGNORE) != 0)
                            IgnoreCount++;

                        // 1 note at difficulty i
                        notes[n.Time] = 1;
                        level[n.Time] = i;
                    }
                    else if (i == level[n.Time])
                    {
                        if (Level == 2 && (n.NoteMask & CON.NOTE_MASK_IGNORE) != 0)
                            IgnoreCount++;

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

        private void parseMetadata(Song2014 xml, Sng2014File sng, Int16[] tuning)
        {
            // Easy, Medium, Hard
            NoteCount = new int[3];
            NoteCount[0] = getNoteCount(sng, 0);
            NoteCount[1] = getNoteCount(sng, 1);
            IgnoreCount = 0;
            NoteCount[2] = getNoteCount(sng, 2); // Calculates IgnoreCount

            sng.Metadata = new Metadata();
            sng.Metadata.MaxScore = 100000;

            sng.Metadata.MaxDifficulty = getMaxDifficulty(xml);
            sng.Metadata.MaxNotesAndChords = NoteCount[2];
            sng.Metadata.MaxNotesAndChords_Real = sng.Metadata.MaxNotesAndChords - IgnoreCount; //num "unique notes - ignored"
            sng.Metadata.PointsPerNote = sng.Metadata.MaxScore / sng.Metadata.MaxNotesAndChords;

            sng.Metadata.FirstBeatLength = xml.Ebeats[1].Time - xml.Ebeats[0].Time;
            sng.Metadata.StartTime = xml.Offset * -1;
            sng.Metadata.CapoFretId = (xml.Capo == 0) ? unchecked((Byte)(-1)) : xml.Capo;
            readString(xml.LastConversionDateTime, sng.Metadata.LastConversionDateTime);
            sng.Metadata.Part = xml.Part;
            sng.Metadata.SongLength = xml.SongLength;
            sng.Metadata.StringCount = 6;
            sng.Metadata.Tuning = tuning ?? new Int16[sng.Metadata.StringCount];
            // calculated when parsing arrangements
            sng.Metadata.Unk11_FirstNoteTime = first_note_time;
            sng.Metadata.Unk12_FirstNoteTime = first_note_time;
        }

        private static Int32 getPhraseIterationId(Song2014 xml, float time, bool end)
        {
            Int32 id = 0;
            while (id + 1 < xml.PhraseIterations.Length)
            {
                if (!end && xml.PhraseIterations[id + 1].Time > time)
                    break;
                if (end && xml.PhraseIterations[id + 1].Time >= time)
                    break;
                ++id;
            }
            return id;
        }

        private void parseEbeats(Song2014 xml, Sng2014File sng)
        {
            sng.BPMs = new BpmSection();
            sng.BPMs.Count = xml.Ebeats.Length;
            sng.BPMs.BPMs = new Bpm[sng.BPMs.Count];
            Int16 measure = 0;
            Int16 beat = 0;
            for (int i = 0; i < sng.BPMs.Count; i++)
            {
                var ebeat = xml.Ebeats[i];
                var bpm = new Bpm();
                bpm.Time = ebeat.Time;
                if (ebeat.Measure >= 0)
                {
                    measure = ebeat.Measure;
                    beat = 0;
                }
                else
                {
                    beat++;
                }
                bpm.Measure = measure;
                bpm.Beat = beat;
                bpm.PhraseIteration = getPhraseIterationId(xml, bpm.Time, true);
                if (beat == 0)
                {
                    bpm.Mask |= 1;
                    if (measure % 2 == 0)
                        bpm.Mask |= 2;
                }
                sng.BPMs.BPMs[i] = bpm;
            }
        }

        public static void readString(string From, Byte[] To)
        {
            var bytes = Encoding.UTF8.GetBytes(From);
            System.Buffer.BlockCopy(bytes, 0, To, 0, Math.Min(To.Length, bytes.Length));
        }

        private void parseChords(Song2014 xml, Sng2014File sng, Int16[] tuning, bool bass)
        {
            sng.Chords = new ChordSection();
            sng.Chords.Count = xml.ChordTemplates.Length;
            sng.Chords.Chords = new Chord[sng.Chords.Count];

            for (int i = 0; i < sng.Chords.Count; i++)
            {
                var chord = xml.ChordTemplates[i];
                var c = new Chord();

                // fix for 'Object reference not set to an instance of an object' error
                if (chord.DisplayName == null)
                    chord.DisplayName = String.Empty;

                if (chord.DisplayName.EndsWith("arp"))
                    c.Mask |= CON.CHORD_MASK_ARPEGGIO;
                else if (chord.DisplayName.EndsWith("nop"))
                    c.Mask |= CON.CHORD_MASK_NOP;

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
                    c.Notes[s] = GetMidiNote(tuning, s, c.Frets[s], bass, xml.Capo, template: true);
                readString(chord.ChordName, c.Name);
                sng.Chords.Chords[i] = c;
            }
        }

        private void parseChordNotes(Song2014 xml, Sng2014File sng)
        {
            sng.ChordNotes = new ChordNotesSection();
            sng.ChordNotes.ChordNotes = cns.ToArray();
            sng.ChordNotes.Count = sng.ChordNotes.ChordNotes.Length;
        }

        private List<ChordNotes> cns = new List<ChordNotes>();
        private Dictionary<UInt32, Int32> cnsId = new Dictionary<UInt32, Int32>();

        public Int32 addChordNotes(Sng2014File sng, SongChord2014 chord)
        {
            var c = new ChordNotes();
            for (int i = 0; i < 6; i++)
            {
                SongNote2014 n = null;
                foreach (var cn in chord.ChordNotes)
                {
                    if (cn.String == i)
                    {
                        n = cn;
                        break;
                    }
                }
                // TODO: need to figure out which masks are not applied
                c.NoteMask[i] = parseNoteMask(n, false);
                c.BendData[i] = new BendData();
                c.BendData[i].BendData32 = parseBendData(n, false);
                if (n != null && n.BendValues != null)
                    c.BendData[i].UsedCount = n.BendValues.Length;
                if (n != null)
                {
                    c.SlideTo[i] = (Byte)n.SlideTo;
                    c.SlideUnpitchTo[i] = (Byte)n.SlideUnpitchTo;
                }
                else
                {
                    c.SlideTo[i] = unchecked((Byte)(-1));
                    c.SlideUnpitchTo[i] = unchecked((Byte)(-1));
                }
                if (n != null)
                    c.Vibrato[i] = n.Vibrato;
            }

            UInt32 crc = sng.HashStruct(c);
            if (cnsId.ContainsKey(crc))
                return cnsId[crc];

            // don't export chordnotes if there are no techniques
            bool noTechniques = c.NoteMask.All(m => m == 0);
            if (noTechniques)
                return -1;

            // add new ChordNotes instance
            Int32 id = cns.Count;
            cnsId[crc] = id;
            cns.Add(c);
            return cnsId[crc];
        }

        private void parsePhrases(Song2014 xml, Sng2014File sng)
        {
            sng.Phrases = new PhraseSection();
            sng.Phrases.Count = xml.Phrases.Length;
            sng.Phrases.Phrases = new Phrase[sng.Phrases.Count];

            for (int i = 0; i < sng.Phrases.Count; i++)
            {
                var phrase = xml.Phrases[i];
                var p = new Phrase();
                p.Solo = phrase.Solo;
                p.Disparity = phrase.Disparity;
                p.Ignore = phrase.Ignore;
                p.MaxDifficulty = phrase.MaxDifficulty;
                p.PhraseIterationLinks = xml.PhraseIterations.Count(iter => iter.PhraseId == i);
                readString(phrase.Name, p.Name);

                sng.Phrases.Phrases[i] = p;
            }
        }

        private void parsePhraseIterations(Song2014 xml, Sng2014File sng)
        {
            sng.PhraseIterations = new PhraseIterationSection();
            sng.PhraseIterations.Count = xml.PhraseIterations.Length;
            sng.PhraseIterations.PhraseIterations = new PhraseIteration[sng.PhraseIterations.Count];

            for (int i = 0; i < sng.PhraseIterations.Count; i++)
            {
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
                        p.Difficulty[h.Hero - 1] = h.Difficulty;
                sng.PhraseIterations.PhraseIterations[i] = p;
            }
        }

        private void parsePhraseExtraInfo(Song2014 xml, Sng2014File sng)
        {
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

        private void parseNLD(Song2014 xml, Sng2014File sng)
        {
            sng.NLD = new NLinkedDifficultySection();
            // fix for 'Object reference not set to an instance of an object' error
            if (xml.NewLinkedDiff == null)
                sng.NLD.Count = 0; // TODO: throw error here to reauthor in EOF
            else
                sng.NLD.Count = xml.NewLinkedDiff.Length;

            sng.NLD.NLinkedDifficulties = new NLinkedDifficulty[sng.NLD.Count];

            for (int i = 0; i < sng.NLD.Count; i++)
            {
                var nld = xml.NewLinkedDiff[i];
                var n = new NLinkedDifficulty();
                // TODO: Ratio attribute unused?
                n.LevelBreak = nld.LevelBreak;
                n.PhraseCount = nld.PhraseCount;
                n.NLD_Phrase = new Int32[n.PhraseCount];
                for (int j = 0; j < n.PhraseCount; j++)
                {
                    n.NLD_Phrase[j] = nld.Nld_phrase[j].Id;
                }
                sng.NLD.NLinkedDifficulties[i] = n;
            }
        }

        private void parseActions(Song2014 xml, Sng2014File sng)
        {
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

        private void parseEvents(Song2014 xml, Sng2014File sng)
        {
            sng.Events = new EventSection();
            sng.Events.Count = xml.Events.Length;
            sng.Events.Events = new Event[sng.Events.Count];

            for (int i = 0; i < sng.Events.Count; i++)
            {
                var evnt = xml.Events[i];
                var e = new Event();
                e.Time = evnt.Time;
                readString(evnt.Code, e.EventName);
                sng.Events.Events[i] = e;
            }
        }

        private void parseTones(Song2014 xml, Sng2014File sng)
        {
            //even if no tone changes, write section size (of 4 bytes)
            sng.Tones = new ToneSection();
            sng.Tones.Count = xml.Tones == null ? 0 : xml.Tones.Length;
            sng.Tones.Tones = new Tone[sng.Tones.Count];
            for (int i = 0; i < sng.Tones.Count; i++)
            {
                var tn = xml.Tones[i];
                var t = new Tone { Time = tn.Time };

                try
                {
                    if (String.IsNullOrEmpty(xml.ToneBase))
                        throw new InvalidDataException("ToneBase must be defined.");

                    // fix for undefined tone name (tone name should be shorter)
                    if (xml.ToneBase.ToLower() == tn.Name.ToLower())
                        t.ToneId = 0;

                    if (xml.ToneA.ToLower() == tn.Name.ToLower())
                        t.ToneId = 0;
                    else if (xml.ToneB.ToLower() == tn.Name.ToLower())
                        t.ToneId = 1;
                    else if (xml.ToneC.ToLower() == tn.Name.ToLower())
                        t.ToneId = 2;
                    else if (xml.ToneD.ToLower() == tn.Name.ToLower())
                        t.ToneId = 3;

                    sng.Tones.Tones[i] = t;
                }
                catch (Exception)
                {
                    throw new InvalidDataException("There is tone name error in XML Arrangement: " + xml.Arrangement + "  " + tn.Name + " is not properly defined." + Environment.NewLine + 
                        "Use EOF to re-author custom tones or Notepad to attempt manual repair.");
                }
            }

            // TODO: confirm that tonebase is set to one of the multi tones in case user didn't do it in EOF
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
        private void parseDNAs(Song2014 xml, Sng2014File sng)
        {
            sng.DNAs = new DnaSection();
            List<Dna> dnas = new List<Dna>();

            DNACount = new int[4];

            // based on events: dna_none=0, dna_solo=1, dna_riff=2, dna_chord=3
            foreach (var e in xml.Events)
            {
                Int32 id = -1;
                switch (e.Code)
                {
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

                if (id != -1)
                {
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

        private void parseSections(Song2014 xml, Sng2014File sng)
        {
            sng.Sections = new SectionSection();
            sng.Sections.Count = xml.Sections.Length;
            sng.Sections.Sections = new Section[sng.Sections.Count];

            for (int i = 0; i < sng.Sections.Count; i++)
            {
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
                for (int j = getMaxDifficulty(xml); j >= 0; j--)
                {
                    // used string mask for section at all difficulty j
                    Byte mask = 0;
                    foreach (var note in xml.Levels[j].Notes)
                        if (note.Time >= s.StartTime && note.Time < s.EndTime)
                        {
                            mask |= (Byte)(1 << note.String);
                        }
                    foreach (var chord in xml.Levels[j].Chords)
                        if (chord.Time >= s.StartTime && chord.Time < s.EndTime)
                        {
                            var ch = xml.ChordTemplates[chord.ChordId];
                            if (ch.Fret0 != -1)
                                mask |= (Byte)(1 << 0);
                            if (ch.Fret1 != -1)
                                mask |= (Byte)(1 << 1);
                            if (ch.Fret2 != -1)
                                mask |= (Byte)(1 << 2);
                            if (ch.Fret3 != -1)
                                mask |= (Byte)(1 << 3);
                            if (ch.Fret4 != -1)
                                mask |= (Byte)(1 << 4);
                            if (ch.Fret5 != -1)
                                mask |= (Byte)(1 << 5);

                            if (mask == 0x3F)
                                break;
                        }

                    // use mask from next section if there are no notes
                    if (mask == 0 && j < getMaxDifficulty(xml))
                        mask = s.StringMask[j + 1];

                    s.StringMask[j] = mask;
                }
                sng.Sections.Sections[i] = s;
            }
        }

        private UInt32 parseNoteMask(SongNote2014 note, bool single)
        {
            if (note == null)
                return CON.NOTE_MASK_UNDEFINED;

            // single note
            UInt32 mask = 0;

            if (single)
                mask |= CON.NOTE_MASK_SINGLE;
            if (note.Fret == 0)
                mask |= CON.NOTE_MASK_OPEN;
            if (note.LinkNext != 0)
                mask |= CON.NOTE_MASK_PARENT;
            if (note.Accent != 0)
                mask |= CON.NOTE_MASK_ACCENT;
            if (note.Bend != 0)
                mask |= CON.NOTE_MASK_BEND;
            if (note.HammerOn != 0)
                mask |= CON.NOTE_MASK_HAMMERON;
            if (note.Harmonic != 0)
                mask |= CON.NOTE_MASK_HARMONIC;

            // TODO: seems to have no effect
            // hopo = 0

            if (single && note.Ignore != 0)
                mask |= CON.NOTE_MASK_IGNORE;
            if (single && note.LeftHand != -1)
                mask |= CON.NOTE_MASK_LEFTHAND;
            if (note.Mute != 0)
                mask |= CON.NOTE_MASK_MUTE;
            if (note.PalmMute != 0)
                mask |= CON.NOTE_MASK_PALMMUTE;
            if (note.Pluck != -1)
                mask |= CON.NOTE_MASK_PLUCK;
            if (note.PullOff != 0)
                mask |= CON.NOTE_MASK_PULLOFF;
            if (note.Slap != -1)
                mask |= CON.NOTE_MASK_SLAP;
            if (note.SlideTo != -1)
                mask |= CON.NOTE_MASK_SLIDE;
            if (note.Sustain != 0)
                mask |= CON.NOTE_MASK_SUSTAIN;
            if (note.Tremolo != 0)
                mask |= CON.NOTE_MASK_TREMOLO;
            if (note.HarmonicPinch != 0)
                mask |= CON.NOTE_MASK_PINCHHARMONIC;

            // TODO: seems to have no effect
            // pickDirection = 0

            if (note.RightHand != -1)
                mask |= CON.NOTE_MASK_RIGHTHAND;
            if (note.SlideUnpitchTo != -1)
                mask |= CON.NOTE_MASK_SLIDEUNPITCHEDTO;
            if (note.Tap != 0)
                mask |= CON.NOTE_MASK_TAP;
            if (note.Vibrato != 0)
                mask |= CON.NOTE_MASK_VIBRATO;

            return mask;
        }

        private void parseNote(Song2014 xml, SongNote2014 note, Notes n, Notes prev)
        {
            n.NoteMask = parseNoteMask(note, true);
            // numbering (NoteFlags) will be set later
            n.Time = note.Time;
            n.StringIndex = note.String;
            // actual fret number
            n.FretId = (Byte)note.Fret;
            // anchor fret will be set later
            n.AnchorFretId = unchecked((Byte)(-1));
            // will be overwritten
            n.AnchorWidth = unchecked((Byte)(-1));
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
            n.SlideTo = unchecked((Byte)note.SlideTo);
            n.SlideUnpitchTo = unchecked((Byte)note.SlideUnpitchTo);
            n.LeftHand = unchecked((Byte)note.LeftHand);
            // 'bvibrato' and 'rchords8' are using 0 value but without TAP mask
            if (note.Tap != 0)
                n.Tap = unchecked((Byte)note.Tap);
            else
                n.Tap = unchecked((Byte)(-1));

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

        private void parseChord(Song2014 xml, Sng2014File sng, SongChord2014 chord, Notes n, Int32 chordNotesId)
        {
            n.NoteMask |= CON.NOTE_MASK_CHORD;
            if (chordNotesId != -1)
            {
                // there should always be a STRUM too => handshape at chord time
                // probably even for chordNotes which are not exported to SNG
                n.NoteMask |= CON.NOTE_MASK_CHORDNOTES;
            }

            if (chord.LinkNext != 0)
                n.NoteMask |= CON.NOTE_MASK_PARENT;

            if (chord.Accent != 0)
                n.NoteMask |= CON.NOTE_MASK_ACCENT;
            if (chord.FretHandMute != 0)
                n.NoteMask |= CON.NOTE_MASK_FRETHANDMUTE;
            if (chord.HighDensity != 0)
                n.NoteMask |= CON.NOTE_MASK_HIGHDENSITY;
            if (chord.Ignore != 0)
                n.NoteMask |= CON.NOTE_MASK_IGNORE;
            if (chord.PalmMute != 0)
                n.NoteMask |= CON.NOTE_MASK_PALMMUTE;
            // TODO: does not seem to have a mask or any effect
            // if (chord.Hopo != 0)
            //     n.NoteMask |= ;

            // numbering will be set later
            //n.NoteFlags = CON.NOTE_FLAGS_NUMBERED;

            n.Time = chord.Time;
            n.StringIndex = unchecked((Byte)(-1));
            // always -1
            n.FretId = unchecked((Byte)(-1));
            // anchor fret will be set later
            n.AnchorFretId = unchecked((Byte)(-1));
            // will be overwritten
            n.AnchorWidth = unchecked((Byte)(-1));
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
            n.SlideTo = unchecked((Byte)(-1));
            n.SlideUnpitchTo = unchecked((Byte)(-1));
            n.LeftHand = unchecked((Byte)(-1));
            n.Tap = unchecked((Byte)(-1));
            n.PickDirection = unchecked((Byte)(-1));
            n.Slap = unchecked((Byte)(-1));
            n.Pluck = unchecked((Byte)(-1));
            if (chord.ChordNotes != null)
            {
                foreach (var cn in chord.ChordNotes)
                    if (cn.Sustain > n.Sustain)
                        n.Sustain = cn.Sustain;
            }

            if (n.Sustain > 0)
                n.NoteMask |= CON.NOTE_MASK_SUSTAIN;

            int cnt = 0;
            for (int str = 0; str < 6; str++)
                if (sng.Chords.Chords[chord.ChordId].Frets[str] != 255)
                    ++cnt;
            if (cnt == 2)
                n.NoteMask |= CON.NOTE_MASK_DOUBLESTOP;

            // there are only zeros for all chords in lessons
            //n.Vibrato = 0;
            //n.MaxBend = 0;
            n.BendData = new BendDataSection();
            n.BendData.Count = 0;
            n.BendData.BendData = new BendData32[n.BendData.Count];
        }

        private BendData32[] parseBendData(SongNote2014 note, bool single)
        {
            // single can be any size, otherwise 32x BendData32 are allocated
            Int32 count = 32;

            // count of available values
            Int32 bend_values = 0;
            if (note != null && note.BendValues != null)
                bend_values = note.BendValues.Length;

            if (single)
            {
                count = bend_values;
            }

            var bd = new BendData32[count];
            for (int i = 0; i < count; i++)
                bd[i] = new BendData32();

            // intentionally not using "count"
            for (int i = 0; i < bend_values; i++)
            {
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
        private void parseArrangements(Song2014 xml, Sng2014File sng)
        {
            sng.Arrangements = new ArrangementSection();
            sng.Arrangements.Count = getMaxDifficulty(xml) + 1;
            sng.Arrangements.Arrangements = new Arrangement[sng.Arrangements.Count];

            // not strictly necessary but more helpful than hash value
            var note_id = new Dictionary<UInt32, UInt32>();

            for (int i = 0; i < sng.Arrangements.Count; i++)
            {
                var level = xml.Levels[i];
                var a = new Arrangement();
                a.Difficulty = level.Difficulty;

                var anchors = new AnchorSection();
                anchors.Count = level.Anchors.Length;
                anchors.Anchors = new Anchor[anchors.Count];


                for (int j = 0; j < anchors.Count; j++)
                {
                    var anchor = new Anchor();
                    anchor.StartBeatTime = level.Anchors[j].Time;
                    if (j + 1 < anchors.Count)
                        anchor.EndBeatTime = level.Anchors[j + 1].Time;
                    else
                        // last phrase iteration = noguitar/end
                        anchor.EndBeatTime = xml.PhraseIterations[xml.PhraseIterations.Length - 1].Time;

                    // Times will be updated later

                    // Uninitialized values are found on anchors in phrases without notes
                    anchor.Unk3_FirstNoteTime = (float) 3.4028234663852886e+38;
                    anchor.Unk4_LastNoteTime = (float) 1.1754943508222875e-38;
                    anchor.FretId = (byte)level.Anchors[j].Fret;
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

                var fp1 = new List<Fingerprint>();
                var fp2 = new List<Fingerprint>();
                foreach (var h in level.HandShapes)
                {
                    if (h.ChordId < 0) continue;
                    var fp = new Fingerprint
                    {
                        ChordId = h.ChordId,
                        StartTime = h.StartTime,
                        EndTime = h.EndTime,
                        Unk3_FirstNoteTime = -1, // -1 in empty handshapes
                        Unk4_LastNoteTime = -1 // In empty handshapes, sometimes this is h.StartTime, sometimes -1
                    };

                    if (xml.ChordTemplates[fp.ChordId].DisplayName.EndsWith("arp"))
                        fp2.Add(fp);
                    else
                        fp1.Add(fp);
                }

                a.Fingerprints1.Count = fp1.Count;
                a.Fingerprints1.Fingerprints = fp1.ToArray();
                a.Fingerprints2.Count = fp2.Count;
                a.Fingerprints2.Fingerprints = fp2.ToArray();

                // Calculated as we go through notes, seems to work
                // NotesInIteration1 is count of notes without ignore="1" flag
                a.PhraseIterationCount1 = xml.PhraseIterations.Length;
                a.NotesInIteration1 = new Int32[a.PhraseIterationCount1];
                // NotesInIteration2 seems to be the full count
                a.PhraseIterationCount2 = a.PhraseIterationCount1;
                a.NotesInIteration2 = new Int32[a.PhraseIterationCount2];

                // notes and chords sorted by time
                List<Notes> notes = new List<Notes>();
                int acent = 0;
                foreach (var note in level.Notes)
                {
                    var n = new Notes();
                    Notes prev = null;
                    if (notes.Count > 0)
                        prev = notes.Last();
                    parseNote(xml, note, n, prev);
                    notes.Add(n);

                    for (int j = 0; j < xml.PhraseIterations.Length; j++)
                    {
                        var piter = xml.PhraseIterations[j];

                        // fix for 100% bug issue and improve mastery
                        if (piter.Time > note.Time && j > 0)
                        {
                            if (note.Ignore == 0)
                                ++a.NotesInIteration1[j - 1];
                            ++a.NotesInIteration2[j - 1];
                            break;
                        }
                    }
            
                    if (note.SlideTo != -1)
                    {
                        var ae = new AnchorExtension();
                        ae.FretId = (Byte)note.SlideTo;
                        ae.BeatTime = note.Time + note.Sustain;
                        a.AnchorExtensions.AnchorExtensions[acent++] = ae;
                    }
                }

                foreach (var chord in level.Chords)
                {
                    var cn = new Notes();
                    Int32 id = -1;
                    if (chord.ChordNotes != null && chord.ChordNotes.Length > 0)
                        id = addChordNotes(sng, chord);
                    parseChord(xml, sng, chord, cn, id);
                    notes.Add(cn);

                    for (int j = 0; j < xml.PhraseIterations.Length; j++)
                    {
                        var piter = xml.PhraseIterations[j];

                        // fix for 100% bug issue and improve mastery
                        if (piter.Time > chord.Time && j > 0)
                        {
                            if (chord.Ignore == 0)
                                ++a.NotesInIteration1[j - 1];

                                ++a.NotesInIteration2[j - 1];
                            break;
                        }
                    }
                }

                // exception handler for some poorly formed RS1 CDLC
                try
                {
                    // need to be sorted before anchor note times are updated
                    notes.Sort((x, y) => x.Time.CompareTo(y.Time));

                    // check for RS1 CDLC note time errors 
                    // if (notes.Count > 0) // alt method to deal with the exception
                    if ((int)first_note_time == 0 || first_note_time > notes[0].Time)
                        first_note_time = notes[0].Time;
                }
                catch (Exception)
                {
                    // show error in convert2012CLI command window and continue
                    Console.WriteLine(@" -- CDLC contains note time errors and may not play properly"); // + ex.Message);
                }

                // Fingerprint ID => Chord ID
                Dictionary<int, int> chordInHandshape = new Dictionary<int, int>();
                Dictionary<int, int> chordInArpeggio = new Dictionary<int, int>();

                foreach (var n in notes)
                {
                    for (Int16 id = 0; id < fp1.Count; id++) // FingerPrints 1st level (common handshapes)
                        if (n.Time >= fp1[id].StartTime && n.Time < fp1[id].EndTime)
                        {
                            // Handshapes can be inside other handshapes
                            if (n.FingerPrintId[0] == -1)
                                n.FingerPrintId[0] = id;

                            // Add STRUM to first chord in the handshape (The chord will be rendered as a full chord panel)
                            // In later DLC, frethand muted chords that start a handshape may not have STRUM
                            if (n.ChordId != -1)
                            {
                                // There may be single notes before the first chord so can't use fp1[id].StartTime == n.Time
                                if (!chordInHandshape.ContainsKey(id))
                                {
                                    n.NoteMask |= CON.NOTE_MASK_STRUM;
                                    chordInHandshape.Add(id, n.ChordId);
                                }
                                else if (chordInHandshape[id] != n.ChordId)
                                {
                                    // This should not be necessary for official songs
                                    n.NoteMask |= CON.NOTE_MASK_STRUM;
                                    chordInHandshape[id] = n.ChordId;
                                }
                            }
                            if (fp1[id].Unk3_FirstNoteTime == -1)
                                fp1[id].Unk3_FirstNoteTime = n.Time;

                            float noteEnd = n.Time + n.Sustain;
                            if (noteEnd >= fp1[id].EndTime)
                            {
                                // Not entirely accurate, sometimes Unk4 is -1 even though there is a chord in the handshape...
                                if (n.Time == fp1[id].StartTime)
                                {
                                   fp1[id].Unk4_LastNoteTime = fp1[id].EndTime;
                                }
                            }
                            else
                            {
                                fp1[id].Unk4_LastNoteTime = noteEnd;
                            }
                        }

                    for (Int16 id = 0; id < fp2.Count; id++) // FingerPrints 2nd level (used for -arp(eggio) handshapes)
                        if (n.Time >= fp2[id].StartTime && n.Time < fp2[id].EndTime)
                        {
                            n.FingerPrintId[1] = id;

                            // Add STRUM to first chord in the arpeggio handshape
                            if (n.ChordId != -1)
                            {
                                if (!chordInArpeggio.ContainsKey(id))
                                {
                                    n.NoteMask |= CON.NOTE_MASK_STRUM;
                                    chordInArpeggio.Add(id, n.ChordId);
                                }
                                else if (chordInArpeggio[id] != n.ChordId)
                                {
                                    // This should not be necessary for official songs
                                    n.NoteMask |= CON.NOTE_MASK_STRUM;
                                    chordInArpeggio[id] = n.ChordId;
                                }
                            }
                            n.NoteMask |= CON.NOTE_MASK_ARPEGGIO;
                            if (fp2[id].Unk3_FirstNoteTime == -1)
                                fp2[id].Unk3_FirstNoteTime = n.Time;

                            float sustain = 0;
                            if (n.Time + n.Sustain < fp2[id].EndTime)
                                sustain = n.Sustain;
                            fp2[id].Unk4_LastNoteTime = n.Time + sustain;
                            break;
                        }

                    for (int j = 0; j < a.Anchors.Count; j++)
                        if (n.Time >= a.Anchors.Anchors[j].StartBeatTime && n.Time < a.Anchors.Anchors[j].EndBeatTime)
                        {
                            n.AnchorWidth = (Byte)a.Anchors.Anchors[j].Width;
                            // anchor fret
                            n.AnchorFretId = (Byte)a.Anchors.Anchors[j].FretId;
                            if (a.Anchors.Anchors[j].Unk3_FirstNoteTime == (float)3.4028234663852886e+38)
                                a.Anchors.Anchors[j].Unk3_FirstNoteTime = n.Time;

                            float sustain = 0;
                            if (n.Time + n.Sustain < a.Anchors.Anchors[j].EndBeatTime - 0.1)
                                sustain = n.Sustain;
                            a.Anchors.Anchors[j].Unk4_LastNoteTime = n.Time + sustain;
                            break;
                        }
                }

                a.Notes = new NotesSection();
                a.Notes.Count = notes.Count;
                a.Notes.Notes = notes.ToArray();

                foreach (var piter in sng.PhraseIterations.PhraseIterations)
                {
                    int count = 0;
                    int j = 0;
                    for (; j < a.Notes.Count; j++)
                    {
                        // skip notes outside of a phraseiteration
                        if (a.Notes.Notes[j].Time < piter.StartTime)
                            continue;
                        if (a.Notes.Notes[j].Time >= piter.NextPhraseTime)
                        {
                            break;
                        }
                        // set to next arrangement note
                        a.Notes.Notes[j].NextIterNote = (Int16)(j + 1);
                        // set all but first note to previous note
                        if (count > 0)
                            a.Notes.Notes[j].PrevIterNote = (Int16)(j - 1);
                        ++count;
                    }
                    // fix last phrase note
                    if (count > 0)
                        a.Notes.Notes[j - 1].NextIterNote = -1;
                }

                // Connect parent notes with child notes (linkNext)
                for (int j = 0; j < a.Notes.Notes.Length; j++)
                {
                    // Look for notes with PARENT mask (linkNext=1)
                    var n = a.Notes.Notes[j];
                    if ((n.NoteMask & CON.NOTE_MASK_PARENT) != 0)
                    {
                        if (n.ChordId == -1) // Single note
                        {
                            // Find the next note on the same string
                            int x = j + 1;
                            while (x < a.Notes.Notes.Length)
                            {
                                var nextnote = a.Notes.Notes[x];
                                if (nextnote.StringIndex == n.StringIndex)
                                {
                                    nextnote.ParentPrevNote = (short)(n.NextIterNote - 1);
                                    nextnote.NoteMask |= CON.NOTE_MASK_CHILD;

                                    break;
                                }
                                x++;
                            }
                            if (x == a.Notes.Notes.Length)
                            {
                                // Ran out of notes in the difficulty level without finding a child note
                                // Possible to end up here due to poorly placed phrase or due to DDC moving phrases
                                // Doesn't crash the game or anything so do nothing for now
                            }
                        }
                        else // Chord
                        {
                            // Chordnotes should always be present
                            if (n.ChordNotesId != -1)
                            {
                                var chordnotes = cns[n.ChordNotesId];

                                // Check which chordNotes have linknext
                                for (int cnString = 0; cnString < 6; cnString++)
                                {
                                    if ((chordnotes.NoteMask[cnString] & CON.NOTE_MASK_PARENT) != 0)
                                    {
                                        // Find the next note on the same string
                                        int x = j + 1;
                                        while (x < a.Notes.Notes.Length)
                                        {
                                            var nextnote = a.Notes.Notes[x];
                                            if (nextnote.StringIndex == cnString)
                                            {
                                                // HACK for XML files that do not match official usage re linkNext (allow 1ms margin of error)
                                                if (nextnote.Time - (n.Time + n.Sustain) > 0.001)
                                                    break;

                                                nextnote.ParentPrevNote = (short)(n.NextIterNote - 1);
                                                nextnote.NoteMask |= CON.NOTE_MASK_CHILD;

                                                break;
                                            }
                                            x++;
                                        }
                                        if (x == a.Notes.Notes.Length)
                                        {
                                            // Ran out of notes in the difficulty level without finding a child note
                                            // Possible to end up here due to poorly placed phrase or due to DDC moving phrases
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                a.PhraseCount = xml.Phrases.Length;
                a.AverageNotesPerIteration = new float[a.PhraseCount];
                var iter_count = new float[a.PhraseCount];
                for (int j = 0; j < xml.PhraseIterations.Length; j++)
                {
                    var piter = xml.PhraseIterations[j];
                    // using NotesInIteration2 to calculate
                    a.AverageNotesPerIteration[piter.PhraseId] += a.NotesInIteration2[j];
                    ++iter_count[piter.PhraseId];
                }

                for (int j = 0; j < iter_count.Length; j++)
                {
                    if (iter_count[j] > 0)
                        a.AverageNotesPerIteration[j] /= iter_count[j];
                }

                // this is some kind of optimization in RS2 where they
                // hash all note data but their position in phrase iteration
                // to mark otherwise unchanged notes
                foreach (var n in a.Notes.Notes)
                {
                    MemoryStream data = sng.CopyStruct(n);
                    var r = new EndianBinaryReader(EndianBitConverter.Little, data);
                    var ncopy = new Notes();
                    ncopy.read(r);
                    ncopy.NextIterNote = 0;
                    ncopy.PrevIterNote = 0;
                    ncopy.ParentPrevNote = 0;
                    UInt32 crc = sng.HashStruct(ncopy);
                    if (!note_id.ContainsKey(crc))
                        note_id[crc] = (UInt32)note_id.Count;
                    n.Hash = note_id[crc];
                }

                numberNotes(sng, a.Notes.Notes);
                sng.Arrangements.Arrangements[i] = a;
            }
        }



        /// <summary>
        /// Sign notes with fret number. (each 8 notes)
        /// </summary>
        /// <param name="sng"></param>
        /// <param name="notes"></param>
        private void numberNotes(Sng2014File sng, Notes[] notes)
        {
            // current phrase iteration
            int p = 0;
            for (int o = 0; o < notes.Length; o++)
            {
                var current = notes[o];

                // skip open strings
                if (current.FretId == 0)
                {
                    continue;
                }

                // are we past phrase iteration boundary?
                if (current.Time > sng.PhraseIterations.PhraseIterations[p].NextPhraseTime)
                {
                    // advance and re-run
                    // will be repeated through empty iterations
                    ++p;
                    o = o - 1;
                    continue;
                }

                bool repeat = false;
                int start = o - 8;
                if (start < 0)
                    start = 0;
                // search last 8 notes
                for (int i = o - 1; i >= start; i--)
                {
                    // ignore notes which are too far away
                    if (notes[i].Time + 2.0 < current.Time)
                        continue;
                    // ignore notes outside of iteration
                    if (notes[i].Time < sng.PhraseIterations.PhraseIterations[p].StartTime)
                        continue;

                    // count as repeat if this fret/chord was numbered recently
                    if ((current.ChordId == -1 && notes[i].FretId == current.FretId) ||
                        (current.ChordId != -1 && notes[i].ChordId == current.ChordId))
                    {
                        if ((notes[i].NoteFlags & CON.NOTE_FLAGS_NUMBERED) != 0)
                        {
                            repeat = true;
                            break;
                        }
                    }
                }

                // change
                if (!repeat)
                    current.NoteFlags |= CON.NOTE_FLAGS_NUMBERED;
            }
        }
    }
}

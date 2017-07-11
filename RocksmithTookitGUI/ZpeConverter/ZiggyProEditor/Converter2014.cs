using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.IO;
using RocksmithToolkitLib.XML;

// compatible with ZiggyProEditor Version 70 (2.3.7.13)
namespace RocksmithToolkitGUI.ZpeConverter.ZiggyProEditor
{
    public class Converter2014
    {
        public void Convert(string zigSrcPath, string destPath)
        {
            var deser = new XmlSerializer(typeof(ZpeSong));
            ZpeSong zigSong;
            using (FileStream stream = new FileStream(zigSrcPath, FileMode.Open))
            {
                zigSong = (ZpeSong)deser.Deserialize(stream);
            }

            if (zigSong.PueVersion != 46)
                throw new Exception("Incompatable version of Ziggy Pro Editor XML file");

            bool foundTrack = false;
            StringBuilder sb = new StringBuilder();
            // cross matching arrangment arrays
            string[] rsArray = new string[] { "Lead", "Rhythm", "Bass" };
            string[] zigArray = new string[] { "PART REAL_GUITAR_22", "PART REAL_GUITAR", "PART REAL_BASS_22" };
            int arrIndex = -1;

            foreach (var arrangement in zigArray)
            {
                arrIndex++;
                var guitarTrack = zigSong.Tracks.SingleOrDefault(tr => arrangement.Equals(tr.Name));

                if (guitarTrack == null)
                {
                    continue;
                    //throw new Exception("Couldn't find a guitar track");
                }

                foundTrack = true;
                var rsSong = new Song2014();
                AddSongMetadata(rsSong, zigSong, rsArray[arrIndex]);
                AddEbeats(rsSong, zigSong, arrangement);
                AddNotes(rsSong, zigSong, arrangement);
                AddToneProps(rsSong, rsArray[arrIndex]);

                var destDir = Path.GetDirectoryName(destPath);
                var destName = Path.GetFileNameWithoutExtension(destPath);
                var xmlDestPath = String.Format("{0}_{1}.xml", Path.Combine(destDir, destName), rsArray[arrIndex]);

                using (FileStream stream = new FileStream(xmlDestPath, FileMode.Create))
                {
                    rsSong.Serialize(stream, true);
                }
            }

            if (!foundTrack)
                throw new NullReferenceException("Did not find any Rocksmith 2014 compatible Ziggy Pro tracks in " + Path.GetFileName(zigSrcPath) + Environment.NewLine);
        }

        private static void AddSongMetadata(Song2014 rsSong, ZpeSong zigSong, string arrangment)
        {
            // standard meta header data
            rsSong.Version = "7";
            rsSong.Arrangement = arrangment;
            rsSong.Part = 1;
            rsSong.Offset = 0;
            rsSong.CentOffset = "0";
            rsSong.StartBeat = 0;
            rsSong.Capo = 0;
            rsSong.AlbumName = "Unknown Album";
            rsSong.AlbumYear = DateTime.Now.ToString("yyyy");
            rsSong.CrowdSpeed = "1";
            rsSong.LastConversionDateTime = DateTime.Now.ToString();
            rsSong.SongLength = zigSong.Length;

            Regex regex = new Regex(" - ");
            string[] artistTitle = regex.Split(zigSong.Name);
            rsSong.ArtistName = artistTitle[0] == null ? zigSong.Name : artistTitle[0];
            rsSong.Title = artistTitle[1] == null ? zigSong.Name : artistTitle[1];

            ZpeTempo tempo = zigSong.Tracks[0].Tempos[0];
            float BPM = (float)Math.Round((float)60000000 / tempo.RawTempo, 3);
            rsSong.AverageTempo = BPM;

            ZpeTuning tuning = null;
            for (int i = 0; i < zigSong.Tunings.Tuning.Count; i++)
            {
                if (arrangment == "Lead" || arrangment == "Rhythm")
                    if (zigSong.Tunings.Tuning[i].IsGuitarTuning)
                    {
                        tuning = zigSong.Tunings.Tuning[i];
                        break;
                    }

                if (arrangment == "Bass")
                    if (zigSong.Tunings.Tuning[i].IsBassTuning)
                    {
                        tuning = zigSong.Tunings.Tuning[i];
                        break;
                    }
            }

            if (tuning == null)
                throw new Exception("ZPE XML does not contain tuning");

            rsSong.Tuning = new TuningStrings { String0 = tuning.E, String1 = tuning.A, String2 = tuning.D, String3 = tuning.G, String4 = tuning.B, String5 = tuning.HighE };
        }

        private static void AddEbeats(Song2014 rsSong, ZpeSong zigSong, string arrangement)
        {
            var ebeats = new List<SongEbeat>();
            var phrases = new List<SongPhraseIteration2014>();

            //  var tempoTrack = zigSong.Tracks.Single(tr => "Tempo".Equals(tr.Name));
            ZpeTrack tempoTrack = zigSong.Tracks[0];

            var tempoIndex = 0;
            var tsIndex = 0;
            var tempo = tempoTrack.Tempos[0];
            var signature = tempoTrack.TimeSignatures[0];
            float time = 0;
            int beat = 1;
            short measure = 1;
            const int MU = 1000000;

            // float secondsPerQuarter = tempo.SecondsPerBar / signature.Numerator;
            float secondsPerQuarter = (float)tempo.RawTempo / MU;

            // var end = tempoTrack.MetaEvents.Single(ev => "EndOfTrack".Equals(ev.MetaType)).StartTime;
            Single end = zigSong.Length;

            while (time < end)
            {
                ebeats.Add(new SongEbeat { Measure = (beat == 1) ? measure : (short)-1, Time = (float)Math.Round(time, 3) });
                // denominator already converted by ZPE
                var delta = secondsPerQuarter * ((float)4 / signature.Denominator);
                time += delta;
                var changed = false;
                if (tsIndex + 1 != tempoTrack.TimeSignatures.Count && time + delta / 2 >= tempoTrack.TimeSignatures[tsIndex + 1].StartTime)
                {
                    signature = tempoTrack.TimeSignatures[++tsIndex];
                    time = signature.StartTime;
                    changed = true;
                }
                if (tempoIndex + 1 != tempoTrack.Tempos.Count && time + delta / 2 >= tempoTrack.Tempos[tempoIndex + 1].StartTime)
                {
                    changed = true;
                    tempo = tempoTrack.Tempos[++tempoIndex];
                    time = tempo.StartTime;

                    // secondsPerQuarter = tempo.SecondsPerBar / signature.Numerator;
                    secondsPerQuarter = (float)tempo.RawTempo / MU;
                }
                if (changed || ++beat > signature.Numerator)
                {
                    beat = 1;
                    ++measure;
                }
                // Console.WriteLine("Getting Ebeats for bar: " + measure);
            }
            rsSong.Ebeats = ebeats.ToArray();

            var guitarTrack = zigSong.Tracks.SingleOrDefault(tr => arrangement.Equals(tr.Name));
            var difficultyCount = guitarTrack.Chords.GroupBy(chord => chord.Difficulty).Count();

            var firstNote = (float)Math.Round(guitarTrack.Chords.Min(c => c.StartTime), 3);
            var lastNote = (float)Math.Round(guitarTrack.Chords.Max(c => c.EndTime), 3);
            int measOffset = ebeats.Where(eb => eb.Measure != -1 && eb.Time <= firstNote).Last().Measure, phraseId = 0;
            measure = (short)measOffset;
            SongEbeat ebeat = null;
            //count in
            phrases.Add(new SongPhraseIteration2014 { PhraseId = phraseId++, Time = 0 });

            while (null != (ebeat = ebeats.FirstOrDefault(eb => eb.Measure == measure)))
            {
                if (ebeat.Time >= lastNote)
                {
                    break;
                }
                phrases.Add(new SongPhraseIteration2014 { PhraseId = phraseId++, Time = ebeat.Time });
                measure += 12;
            }
            //end
            phrases.Add(new SongPhraseIteration2014 { PhraseId = phraseId++, Time = lastNote + .001f });
            // phrases.Add(new SongPhraseIteration2014 { PhraseId = phraseId++, Time = end + .001f });
            rsSong.PhraseIterations = phrases.ToArray();
            rsSong.Phrases = phrases.Select(it => new SongPhrase { MaxDifficulty = it.PhraseId == 0 || it.PhraseId == phrases.Count - 1 ? 0 : difficultyCount - 1, Name = it.PhraseId == 0 ? "COUNT" : it.PhraseId == phrases.Count - 1 ? "END" : it.PhraseId.ToString() }).ToArray();
            phrases.RemoveAt(0);
            phrases.RemoveAt(phrases.Count - 1);
            rsSong.Sections = phrases.Select(it => new SongSection { Name = "verse", Number = 1, StartTime = it.Time }).ToArray();
        }

        private static void AddNotes(Song2014 rsSong, ZpeSong zigSong, string arrangement)
        {
            int spread = 3;
            int width = spread;
            var notes = new Dictionary<string, List<SongNote2014>>();
            var chords = new Dictionary<string, List<SongChord2014>>();
            var anchors = new Dictionary<string, List<SongAnchor2014>>();
            var handShapes = new Dictionary<string, List<SongHandShape>>();
            var chordTemps = new Dictionary<string, Tuple<int, SongChordTemplate2014>>();

            var guitarTrack = zigSong.Tracks.SingleOrDefault(tr => arrangement.Equals(tr.Name));
            var difficultyCount = guitarTrack.Chords.GroupBy(chord => chord.Difficulty).Count();

            foreach (var group in guitarTrack.Chords.GroupBy(chord => chord.Difficulty))
            {
                var gNotes = notes[group.Key] = new List<SongNote2014>();
                var gChords = chords[group.Key] = new List<SongChord2014>();
                var gAnchors = anchors[group.Key] = new List<SongAnchor2014>();
                var gHandShapes = handShapes[group.Key] = new List<SongHandShape>();
                var zChords = group.OrderBy(chord => chord.StartTime).ToList();
                var lastMeasure = 0;
                int highFret = -1;
                //bool lastWasChord = false;
                SongAnchor2014 curAnchor = null;
                // dont see any tempo, time sig note or chord time adjustment here because 
                // start end times have already been tempo timsig map adjusted
                for (int i = 0; i < zChords.Count; i++)
                {
                    var zChord = zChords[i];
                    if (zChord.Notes.Count > 1)  // cord
                    {
                        Tuple<int, SongChordTemplate2014> val = GetChordTemplate(zChord, chordTemps);

                        int minCFret = Math.Min(DeZero(val.Item2.Fret0), Math.Min(DeZero(val.Item2.Fret1), Math.Min(DeZero(val.Item2.Fret2), Math.Min(DeZero(val.Item2.Fret3), Math.Min(DeZero(val.Item2.Fret4), DeZero(val.Item2.Fret5))))));

                        if (minCFret != int.MaxValue)
                        {
                            if (curAnchor == null)
                            {
                                if (gAnchors.Count == 0 || gAnchors[gAnchors.Count - 1].Fret != minCFret)
                                {
                                    gAnchors.Add(new SongAnchor2014 { Fret = Math.Min(18, minCFret), Time = (float)Math.Round(zChord.StartTime, 3), Width = spread });
                                }
                            }
                            else
                            {
                                if (minCFret + spread <= highFret)
                                {
                                    curAnchor.Fret = minCFret;
                                }
                                gAnchors.Add(curAnchor);
                                if (curAnchor.Fret != minCFret)
                                {
                                    gAnchors.Add(new SongAnchor2014 { Fret = Math.Min(18, minCFret), Time = (float)Math.Round(zChord.StartTime, 3), Width = spread });
                                }
                                curAnchor = null;
                            }
                        }

                        SongHandShape handShape = new SongHandShape { ChordId = val.Item1, StartTime = (float)Math.Round(zChord.StartTime, 3) };

                        do
                        {
                            SongChord2014 chord = new SongChord2014();
                            chord.Time = (float)Math.Round(zChord.StartTime, 3);
                            chord.ChordId = val.Item1;
                            chord.Strum = "down"; // required by CST

                            List<SongNote2014> noteList = new List<SongNote2014>();
                            for (int zNoteIndex = 0; zNoteIndex < zChord.Notes.Count; zNoteIndex++)
                            {
                                var note = GetNote(zChord, null, zNoteIndex);
                                noteList.Add(note);
                            }
                            chord.ChordNotes = noteList.ToArray();

                            var measure = rsSong.Ebeats.FirstOrDefault(ebeat => ebeat.Time >= chord.Time);
                            if (measure == null || measure.Measure > lastMeasure)
                            {
                                lastMeasure = measure == null ? lastMeasure : measure.Measure;
                                chord.HighDensity = 0;
                            }
                            else if (gChords.Count == 0 || gChords[gChords.Count - 1].ChordId != chord.ChordId)
                            {
                                chord.HighDensity = 0;
                            }
                            else
                            {
                                chord.HighDensity = 1;
                            }

                            gChords.Add(chord);

                            if (i + 1 < zChords.Count)
                            {
                                zChord = zChords[i + 1];
                            }
                        } while (i + 1 < zChords.Count && zChord.Notes.Count > 1 && val.Item1 == GetChordTemplate(zChord, chordTemps).Item1 && ++i != -1);


                        handShape.EndTime = zChord.StartTime;

                        if (handShape.EndTime > handShape.StartTime)
                        {
                            handShape.EndTime -= (handShape.EndTime - zChords[i].EndTime) / 2;
                        }
                        gHandShapes.Add(handShape);
                    }
                    else // single notes
                    {
                        var note = GetNote(zChord, i == zChords.Count - 1 ? null : zChords[i + 1]);
                        if (note.Fret > 0)
                        {
                            if (curAnchor == null)
                            {
                                curAnchor = new SongAnchor2014 { Fret = Math.Min(18, (int)note.Fret), Time = note.Time, Width = spread };
                                highFret = note.Fret;
                            }
                            else if (note.Fret < curAnchor.Fret)
                            {
                                if (note.Fret + spread >= highFret)
                                {
                                    curAnchor.Fret = note.Fret;
                                }
                                else
                                {
                                    gAnchors.Add(curAnchor);
                                    curAnchor = new SongAnchor2014 { Fret = Math.Min(18, (int)note.Fret), Time = note.Time, Width = spread };
                                    highFret = note.Fret;
                                }
                            }
                            else if (note.Fret > highFret)
                            {
                                if (note.Fret - spread <= curAnchor.Fret)
                                {
                                    highFret = note.Fret;
                                }
                                else
                                {
                                    gAnchors.Add(curAnchor);
                                    curAnchor = new SongAnchor2014 { Fret = Math.Min(18, (int)note.Fret), Time = note.Time, Width = spread };
                                    highFret = note.Fret;
                                }
                            }
                        }
                        gNotes.Add(note);

                        //if (note.Fret > width)
                        //    width = note.Fret;
                    }
                }

                //if (width > 8)
                //{
                //    string msgText = zigSong.Name + "  " + guitarTrack.Name + ": has a note spread of " + width + " frets   " +
                //        "\r\nThis exceed the useful intended capicity of this program\r\n" +
                //        "and the Rocksmith note highway will be very wide for this song.\r\n\r\n " +
                //        "I understand it could look like crap, but I want to continue anyhow.";

                //    if (MessageBox.Show(new Form { TopMost = true },
                //        msgText, @"Midi File: Fret Spread Warning",
                //        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                //    {
                //        Application.Exit();
                //        // Application.Restart();
                //        Environment.Exit(-1);
                //    }
                //}

                if (curAnchor != null)
                {
                    gAnchors.Add(curAnchor);
                }
            }

            if (difficultyCount == 1)
            {
                rsSong.Levels = new SongLevel2014[]
                    {
                        new SongLevel2014 {Difficulty = 0, Notes = notes["Easy"].ToArray(), Chords = chords["Easy"].ToArray(), Anchors = anchors["Easy"].ToArray(), HandShapes = handShapes["Easy"].ToArray()}
                    };
            }
            else
            {
                rsSong.Levels = new SongLevel2014[]
            {
                    new SongLevel2014 { Difficulty=0,
                        Notes = notes["Easy"].ToArray() ,
                        Chords =  chords["Easy"].ToArray() ,
                        Anchors = anchors["Easy"].ToArray() ,
                        HandShapes = handShapes["Easy"].ToArray()  },
                    new SongLevel2014 { Difficulty=1,
                        Notes = notes["Medium"].ToArray() ,
                        Chords = chords["Medium"].ToArray() ,
                        Anchors = anchors["Medium"].ToArray() ,
                        HandShapes = handShapes["Medium"].ToArray()  },
                    new SongLevel2014 { Difficulty=2,
                        Notes = notes["Hard"].ToArray(),
                        Chords = chords["Hard"].ToArray(),
                        Anchors = anchors["Hard"].ToArray() ,
                        HandShapes = handShapes["Hard"].ToArray() },
                    new SongLevel2014 { Difficulty=3,
                        Notes = notes["Expert"].ToArray() ,
                        Chords = chords["Expert"].ToArray() ,
                        Anchors = anchors["Expert"].ToArray() ,
                        HandShapes = handShapes["Expert"].ToArray()  }
            };
            }
            rsSong.ChordTemplates = chordTemps.Values.OrderBy(v => v.Item1).Select(v => v.Item2).ToArray();
        }

        private static void AddToneProps(Song2014 rsSong, string arrangment)
        {
            int pathLead = 0;
            int pathRythum = 0;
            int pathBass = 0;

            if (arrangment == "Lead") pathLead = 1;
            if (arrangment == "Rhythm") pathRythum = 1;
            if (arrangment == "Bass") pathBass = 1;

            // some defaults needed by CST to produce CDLC
            rsSong.ToneBase = "AcousticDefault";
            rsSong.ToneA = "AcousticDefault";
            rsSong.ToneB = "";
            rsSong.ToneC = "";
            rsSong.ToneD = "";
            rsSong.Tones = new SongTone2014[0];
            rsSong.Events = new SongEvent[0];
            rsSong.FretHandMuteTemplates = new SongFretHandMuteTemplate[0];
            rsSong.PhraseProperties = new SongPhraseProperty[0];
            rsSong.LinkedDiffs = new SongLinkedDiff[0];
            rsSong.NewLinkedDiff = new SongNewLinkedDiff[0];

            rsSong.ArrangementProperties = new SongArrangementProperties2014
            {
                Represent = 1,
                StandardTuning = 1,
                NonStandardChords = 0,
                BarreChords = 0,
                PowerChords = 0,
                DropDPower = 0,
                OpenChords = 0,
                FingerPicking = 0,
                PickDirection = 0,
                DoubleStops = 0,
                PalmMutes = 0,
                Harmonics = 0,
                PinchHarmonics = 0,
                Hopo = 0,
                Tremolo = 0,
                Slides = 0,
                UnpitchedSlides = 0,
                Bends = 0,
                Tapping = 0,
                Vibrato = 0,
                FretHandMutes = 0,
                SlapPop = 0,
                TwoFingerPicking = 0,
                FifthsAndOctaves = 0,
                Syncopation = 0,
                BassPick = 0,
                Sustain = 0,
                BonusArr = 0,
                PathLead = pathLead,
                PathRhythm = pathRythum,
                PathBass = pathBass,
                RouteMask = 1
            };
        }


        private static int DeZero(int val)
        {
            return val > 0 ? val : int.MaxValue;
        }

        private static Tuple<int, SongChordTemplate2014> GetChordTemplate(ZpeChord zChord, Dictionary<string, Tuple<int, SongChordTemplate2014>> chordTemps)
        {
            Tuple<int, SongChordTemplate2014> val;
            if (!chordTemps.TryGetValue(HashChord(zChord), out val))
            {
                SongChordTemplate2014 templ = new SongChordTemplate2014() { ChordName = "", DisplayName = "" };
                val = new Tuple<int, SongChordTemplate2014>(chordTemps.Count == 0 ? 0 : chordTemps.Values.Select(v => v.Item1).Max() + 1, templ);
                templ.Finger0 = templ.Finger1 = templ.Finger2 = templ.Finger3 = templ.Finger4 = templ.Finger5 = -1;
                templ.Fret0 = templ.Fret1 = templ.Fret2 = templ.Fret3 = templ.Fret4 = templ.Fret5 = -1;
                zChord.Notes.Where(n => n.StringNo == 0).ToList().ForEach(note => templ.Fret0 = (sbyte)note.Fret);
                zChord.Notes.Where(n => n.StringNo == 1).ToList().ForEach(note => templ.Fret1 = (sbyte)note.Fret);
                zChord.Notes.Where(n => n.StringNo == 2).ToList().ForEach(note => templ.Fret2 = (sbyte)note.Fret);
                zChord.Notes.Where(n => n.StringNo == 3).ToList().ForEach(note => templ.Fret3 = (sbyte)note.Fret);
                zChord.Notes.Where(n => n.StringNo == 4).ToList().ForEach(note => templ.Fret4 = (sbyte)note.Fret);
                zChord.Notes.Where(n => n.StringNo == 5).ToList().ForEach(note => templ.Fret5 = (sbyte)note.Fret);
                chordTemps[HashChord(zChord)] = val;
            }
            return val;
        }

        private static SongNote2014 GetNote(ZpeChord chord, ZpeChord nextChord, int zNoteIndex = 0)
        {
            SongNote2014 note = new SongNote2014();
            ZpeNote zNote = chord.Notes[zNoteIndex];
            note.Fret = (sbyte)zNote.Fret;
            note.String = (byte)zNote.StringNo;
            note.Time = (float)chord.StartTime;
            note.Accent = 0;
            note.Bend = 0;
            note.HammerOn = (zNote.IsTapNote || chord.IsHammerOn) ? (byte)1 : (byte)0;
            note.Harmonic = 0;
            note.HarmonicPinch = 0;
            note.Hopo = note.HammerOn;
            note.Ignore = 0;
            note.LinkNext = 0;
            note.LeftHand = -1;
            note.Mute = 0;
            note.PalmMute = (zNote.IsXNote || chord.IsMute) ? (byte)1 : (byte)0;
            note.Pluck = -1;
            note.PullOff = 0;
            note.RightHand = -1;
            note.Slap = -1;
            note.SlideUnpitchTo = -1;
            note.Sustain = (chord.EndTime - chord.StartTime > .5) ? chord.EndTime - chord.StartTime : 0;
            note.Tap = 0;
            note.Tremolo = 0;
            note.Vibrato = 0;

            if (chord.IsSlide && nextChord != null)
            {
                note.SlideTo = (sbyte)Math.Max(nextChord.Notes[0].Fret, 1);
                note.Sustain = chord.EndTime - chord.StartTime;
                note.HammerOn = note.Hopo = note.PalmMute = 0;
            }
            else
                note.SlideTo = -1;

            //// no advanced techniques for now
            //note.SlideTo = -1;
            //note.Sustain = 0;
            //note.PalmMute = 0;
            //note.HammerOn = 0;
            //note.Hopo = 0;

            return note;
        }

        private static string HashChord(ZpeChord chord)
        {
            string hash = "";
            chord.Notes.OrderBy(n => n.StringNo).ToList().ForEach(ch => hash += ch.StringNo + "." + ch.Fret + ".");
            return hash;
        }

    }
}

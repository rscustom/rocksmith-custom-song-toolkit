using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using RsSong = RocksmithToolkitLib.Xml.Song;
using Song = RocksmithToolkitLib.ZiggyProEditor.Song;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    public class Converter
    {
        public void Convert(string inputFileName, string outputFileName)
        {
            var deser = new XmlSerializer(typeof(Song));
            Song zigSong;
            using (FileStream stream = new FileStream(inputFileName, FileMode.Open))
            {
                zigSong = (Song)deser.Deserialize(stream);
            }

            var guitarTrack = GetTrack(zigSong);
            if (guitarTrack == null)
            {
                throw new Exception("Couldn't find a guitar track");
            }

            var rsSong = new RsSong();
            AddSongMetadata(rsSong, zigSong);
            AddEbeats(rsSong, zigSong);
            AddNotes(rsSong, zigSong);

            deser = new XmlSerializer(typeof(RsSong));
            using (FileStream stream = new FileStream(outputFileName, FileMode.Create))
            {
                deser.Serialize(stream, rsSong);
            }
        }

        private void AddSongMetadata(RsSong rsSong, Song zigSong)
        {
            rsSong.Arrangement = "Combo";
            rsSong.Artist = "Unknown Artist";
            rsSong.Title = zigSong.Name;
            rsSong.Offset = 0;
            rsSong.Part = 1;
            rsSong.LastConversionDateTime = DateTime.Now.ToString();
            rsSong.SongLength = zigSong.Length;
        }

        private void AddEbeats(RsSong rsSong, Song zigSong)
        {
            var ebeats = new List<SongEbeat>();
            var phrases = new List<SongPhraseIteration>();
            var tempoTrack = zigSong.Tracks.Single(tr => "Tempo".Equals(tr.Name));
            var tempoIndex = 0;
            var tsIndex = 0;
            var tempo = tempoTrack.Tempos[0];
            var signature = tempoTrack.TimeSignatures[0];
            float time = 0;
            int beat = 1;
            short measure = 1;
            float secondsPerQuarter = tempo.SecondsPerBar / signature.Numerator;
            var end = tempoTrack.MetaEvents.Single(ev => "EndOfTrack".Equals(ev.MetaType)).StartTime;

            while (time < end)
            {
                ebeats.Add(new SongEbeat { Measure = (beat == 1) ? measure : (short)-1, Time = time });
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
                    secondsPerQuarter = tempo.SecondsPerBar / signature.Numerator;
                }
                if (changed || ++beat > signature.Numerator)
                {
                    beat = 1;
                    ++measure;
                }
            }
            rsSong.Ebeats = ebeats.ToArray();


            var guitarTrack = GetTrack(zigSong);
            var firstNote = guitarTrack.Chords.Min(c => c.StartTime);
            var lastNote = guitarTrack.Chords.Max(c => c.EndTime);
            int measOffset = ebeats.Where(eb => eb.Measure != -1 && eb.Time <= firstNote).Last().Measure,
                phraseId = 0;
            measure = (short)measOffset;
            SongEbeat ebeat = null;
            //count in
            phrases.Add(new SongPhraseIteration { PhraseId = phraseId++, Time = 0 });
            while (null != (ebeat = ebeats.FirstOrDefault(eb => eb.Measure == measure)))
            {
                if (ebeat.Time >= lastNote)
                {
                    break;
                }
                phrases.Add(new SongPhraseIteration { PhraseId = phraseId++, Time = ebeat.Time });
                measure += 12;
            }
            //end
            phrases.Add(new SongPhraseIteration { PhraseId = phraseId++, Time = lastNote + .001f });
            rsSong.PhraseIterations = phrases.ToArray();
            rsSong.Phrases = phrases.Select(it =>
                    new SongPhrase { MaxDifficulty = it.PhraseId == 0 || it.PhraseId == phrases.Count - 1 ? 0:3, Name = it.PhraseId == 0 ? "COUNT" : it.PhraseId == phrases.Count - 1 ? "END" : it.PhraseId.ToString() }).ToArray();
            phrases.RemoveAt(0);
            phrases.RemoveAt(phrases.Count - 1);
            rsSong.Sections = phrases.Select(it =>
                    new SongSection { Name = "verse", Number = 1, StartTime = it.Time }).ToArray();
        }

        private void AddNotes(RsSong rsSong, Song zigSong)
        {
            int spread = 3;
            var notes = new Dictionary<string, List<SongNote>>();
            var chords = new Dictionary<string, List<SongChord>>();
            var anchors = new Dictionary<string, List<SongAnchor>>();
            var handShapes = new Dictionary<string, List<SongHandShape>>();

            var chordTemps = new Dictionary<string, Tuple<int, SongChordTemplate>>();
            var guitarTrack = GetTrack(zigSong);
            foreach (var group in guitarTrack.Chords.GroupBy(chord => chord.Difficulty))
            {
                var gNotes = notes[group.Key] = new List<SongNote>();
                var gChords = chords[group.Key] = new List<SongChord>();
                var gAnchors = anchors[group.Key] = new List<SongAnchor>();
                var gHandShapes = handShapes[group.Key] = new List<SongHandShape>();
                var zChords = group.OrderBy(chord => chord.StartTime).ToList();
                var lastMeasure = 0;
                int highFret = -1;
                //bool lastWasChord = false;
                SongAnchor curAnchor = null;
                for (int i = 0; i < zChords.Count; i++)
                {
                    var zChord = zChords[i];
                    if (zChord.Notes.Count > 1)
                    {
                        Tuple<int, SongChordTemplate> val = GetChordTemplate(zChord, chordTemps);

                        int minCFret = Math.Min(DeZero(val.Item2.Fret0),
                            Math.Min(DeZero(val.Item2.Fret1),
                            Math.Min(DeZero(val.Item2.Fret2),
                            Math.Min(DeZero(val.Item2.Fret3),
                            Math.Min(DeZero(val.Item2.Fret4), DeZero(val.Item2.Fret5))))));

                        if (minCFret != int.MaxValue)
                        {
                            if (curAnchor == null)
                            {
                                if (gAnchors.Count == 0 || gAnchors[gAnchors.Count - 1].Fret != minCFret)
                                {
                                    gAnchors.Add(new SongAnchor { Fret = Math.Min(19, minCFret), Time = zChord.StartTime });
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
                                    gAnchors.Add(new SongAnchor { Fret = Math.Min(19, minCFret), Time = zChord.StartTime });
                                }
                                curAnchor = null;
                            }
                        }
                        SongHandShape handShape = new SongHandShape { ChordId = val.Item1, StartTime = zChord.StartTime };
                        do
                        {
                            SongChord chord = new SongChord();
                            chord.Time = zChord.StartTime;
                            chord.ChordId = val.Item1;

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
                    else
                    {
                        var note = GetNote(zChord, i == zChords.Count - 1 ? null : zChords[i + 1]);
                        if (note.Fret > 0)
                        {
                            if (curAnchor == null)
                            {
                                curAnchor = new SongAnchor { Fret = Math.Min(19, (int)note.Fret), Time = note.Time };
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
                                    curAnchor = new SongAnchor { Fret = Math.Min(19, (int)note.Fret), Time = note.Time };
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
                                    curAnchor = new SongAnchor { Fret = Math.Min(19, (int)note.Fret), Time = note.Time };
                                    highFret = note.Fret;
                                }
                            }
                        }
                        gNotes.Add(note);
                    }
                }
                if (curAnchor != null)
                {
                    gAnchors.Add(curAnchor);
                }
            }

            rsSong.Levels = new SongLevel[]
            {
                    new SongLevel { Difficulty=0,
                        Notes = notes["Easy"].ToArray() ,
                        Chords =  chords["Easy"].ToArray() ,
                        Anchors = anchors["Easy"].ToArray() ,
                        HandShapes = handShapes["Easy"].ToArray()  },
                    new SongLevel { Difficulty=1,
                        Notes = notes["Medium"].ToArray() ,
                        Chords = chords["Medium"].ToArray() ,
                        Anchors = anchors["Medium"].ToArray() ,
                        HandShapes = handShapes["Medium"].ToArray()  },
                    new SongLevel { Difficulty=2,
                        Notes = notes["Hard"].ToArray(),
                        Chords = chords["Hard"].ToArray(),
                        Anchors = anchors["Hard"].ToArray() ,
                        HandShapes = handShapes["Hard"].ToArray() },
                    new SongLevel { Difficulty=3,
                        Notes = notes["Expert"].ToArray() ,
                        Chords = chords["Expert"].ToArray() ,
                        Anchors = anchors["Expert"].ToArray() ,
                        HandShapes = handShapes["Expert"].ToArray()  }
            };
            rsSong.ChordTemplates = chordTemps.Values.OrderBy(v => v.Item1).Select(v => v.Item2).ToArray();
        }

        private int DeZero(int val)
        {
            return val > 0 ? val : int.MaxValue;
        }

        private Tuple<int, SongChordTemplate> GetChordTemplate(Chord zChord, Dictionary<string, Tuple<int, SongChordTemplate>> chordTemps)
        {
            Tuple<int, SongChordTemplate> val;
            if (!chordTemps.TryGetValue(HashChord(zChord), out val))
            {
                SongChordTemplate templ = new SongChordTemplate() { ChordName = "" };
                val = new Tuple<int, SongChordTemplate>(chordTemps.Count == 0 ? 0 : chordTemps.Values.Select(v => v.Item1).Max() + 1, templ);
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

        private SongNote GetNote(Chord chord, Chord nextChord)
        {
            SongNote note = new SongNote();
            Note zNote = chord.Notes[0];
            note.Fret = (sbyte)zNote.Fret;
            note.String = (byte)zNote.StringNo;
            note.Bend = 0;
            note.HammerOn = (zNote.IsTapNote || chord.IsHammerOn) ? (byte)1 : (byte)0;
            note.Harmonic = 0;
            note.Hopo = note.HammerOn;
            note.Ignore = 0;
            note.PalmMute = (zNote.IsXNote || chord.IsMute) ? (byte)1 : (byte)0;
            note.Sustain = (chord.EndTime - chord.StartTime > .5) ? chord.EndTime - chord.StartTime : 0;
            note.Time = (float)chord.StartTime;
            note.Tremolo = 0;
            if (chord.IsSlide && nextChord != null)
            {
                note.SlideTo = (sbyte)Math.Max(nextChord.Notes[0].Fret, 1);
                note.Sustain = chord.EndTime - chord.StartTime;
                note.HammerOn = note.Hopo = note.PalmMute = 0;
            }
            else
            {
                note.SlideTo = -1;
            }
            return note;
        }

        string HashChord(Chord chord)
        {
            string hash = "";
            chord.Notes.OrderBy(n => n.StringNo).ToList().ForEach(ch => hash += ch.StringNo + "." + ch.Fret + ".");
            return hash;
        }

        Track GetTrack(Song song)
        {
            var guitarTrack = song.Tracks.SingleOrDefault(tr => "PART REAL_GUITAR_22".Equals(tr.Name));
            if (guitarTrack == null)
            {
                guitarTrack = song.Tracks.SingleOrDefault(tr => "PART REAL_GUITAR".Equals(tr.Name));
            }
            return guitarTrack;
        }
    }
}

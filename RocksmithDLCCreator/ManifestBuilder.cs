using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithDLCCreator
{
    public class ManifestBuilder
    {
        public AggregateGraph AggregateGraph { get; set; }
        public string AlbumName { get; set; }
        public Manifest.Manifest Manifest { get; private set; }
        public ManifestBuilder()
        {
            Manifest = new Manifest.Manifest();
        }
        public string GenerateManifest(string dlcName, List<Arrangement> arrangements)
        {
            var manifest = Manifest;
            manifest.Entries = new Dictionary<string, Dictionary<string, Manifest.Attributes>>();
            bool firstarrangset = false;
            int songPartitioncnt = 1;
            string vocalName = arrangements[0].Name;
            Guid vocalGuid = arrangements[0].Id;
            foreach (var x in arrangements)
            {
                vocalName = x.SongFile.Name;
                vocalGuid = x.Id;
            }
            foreach (var x in arrangements)
            {
                var attribute = new Manifest.Attributes();
                Guid id = x.Id;
                attribute.AlbumArt = String.Format("urn:llid:{0}", AggregateGraph.AlbumArt.LLID);
                attribute.AlbumNameSort = attribute.AlbumName = AlbumName;
                attribute.ArrangementName = x.Name;
                attribute.ArtistName = attribute.ArtistNameSort = x.Artist;
                attribute.AssociatedTechniques = new List<string>();//
                attribute.BarChords = x.BarChords;
                attribute.Bends = false;//
                attribute.BinaryVersion = 49;
                attribute.BlockAsset = String.Format("urn:emergent-world:{0}", AggregateGraph.XBlock.Name);
                attribute.ChordTemplates = null;//
                attribute.DISC_DLC_OTHER = "Disc";
                attribute.DisplayName = x.SongDisplayName;
                attribute.DLCPreview = false;
                attribute.DoubleStops = x.DoubleStops;
                attribute.DropDPowerChords = x.DropDPowerChords;
                if (x.IsVocal)
                    attribute.DynamicVisualDensity = new List<float>(){4.5f,
                                                                      4.3000001907348633f,
                                                                      4.0999999046325684f,
                                                                      3.9000000953674316f,
                                                                      3.7000000476837158f,
                                                                      3.5f,
                                                                      3.2999999523162842f,
                                                                      3.0999999046325684f,
                                                                      2.9000000953674316f,
                                                                      2.7000000476837158f,
                                                                      2.5f,
                                                                      2.2999999523162842f,
                                                                      2.0999999046325684f,
                                                                      2.0f,
                                                                      2.0f,
                                                                      2.0f,
                                                                      2.0f,
                                                                      2.0f,
                                                                      2.0f,
                                                                      2.0f};
                else
                    attribute.DynamicVisualDensity = new List<float>()
                    {
                        4.5f,
                        4.0f,
                        3.5999999046325684f,
                        3.2000000476837158f,
                        2.9000000953674316f,
                        2.5999999046325684f,
                        2.2999999523162842f,
                        2.0999999046325684f,
                        1.8999999761581421f,
                        1.7000000476837158f,
                        1.5f,
                        1.3999999761581421f,
                        1.2999999523162842f,
                        1.2000000476837158f,
                        1.1000000238418579f,
                        1.0f,
                        1.0f,
                        1.0f,
                        1.0f,
                        1.0f
                    };
                attribute.EffectChainMultiplayerName = string.Empty;
                attribute.EffectChainName = dlcName;
                attribute.EventFirstTimeSortOrder = 9999;
                attribute.ExclusiveBuild = new List<object>();
                attribute.FifthsAndOctaves = x.FifthsAndOctaves;
                attribute.FirstArrangementInSong = false;
                if (x.IsVocal && !firstarrangset)
                {
                    firstarrangset = true;
                    attribute.FirstArrangementInSong = true;
                }
                attribute.ForceUseXML = true;
                attribute.FretHandMutes = x.FretHandMutes;
                attribute.Genre = "PLACEHOLDER Genre";
                attribute.Harmonics = false;//
                attribute.HOPOs = false;//
                attribute.InputEvent = x.IsVocal ? "Play_Tone_Standard_Mic" : "Play_Tone_";
                attribute.IsDemoSong = false;
                attribute.IsDLC = true;
                attribute.LastConversionDateTime = "";
                attribute.MasterID_PS3 = 0;
                attribute.MasterID_Xbox360 = 504;
                attribute.MaxPhraseDifficulty = 0;//
                attribute.OpenChords = x.OpenChords;
                attribute.PalmMutes = false;//
                attribute.PersistentID = id.ToString().Replace("-", "").ToUpper();
                attribute.PhraseIterations = new List<Manifest.PhraseIteration>();//
                attribute.Phrases = new List<Manifest.Phrase>();//
                attribute.PluckedType = "Picked";
                attribute.PowerChords = x.PowerChords;
                attribute.Prebends = x.PreBends;
                attribute.RelativeDifficulty = x.RelativeDifficulty;
                attribute.RepresentativeArrangement = false;
                attribute.Score_MaxNotes = 0;
                attribute.Score_PNV = 0;
                attribute.Sections = new List<Manifest.Section>();
                attribute.Shipping = true;
                attribute.SlapAndPop = x.SlapAndPop;
                attribute.Slides = false;//
                attribute.SongAsset = String.Format("urn:llid:{0}", x.SongFile.LLID);
                attribute.SongEvent = String.Format("Play_{0}", "DammitClean");
                attribute.SongKey = dlcName;
                attribute.SongLength = 0;
                attribute.SongNameSort = attribute.SongName = dlcName;
                attribute.SongPartition = 0;
                attribute.SongXml = String.Format("urn:llid:{0}", x.SongXml.LLID);
                attribute.SongYear = x.SongYear;
                attribute.Sustain = false;//
                attribute.Syncopation = false;
                attribute.TargetScore = 0;//
                attribute.ToneUnlockScore = 0;//
                attribute.Tremolo = false;//
                attribute.Tuning = x.Tuning;
                attribute.TwoFingerPlucking = false;
                attribute.TwoHandTapping = false;
                attribute.UnlockKey = "";
                attribute.Vibrato = x.Vibrato;
                attribute.VocalsAssetId = x.IsVocal ? "" : String.Format("{1}|GRSong_{0}", vocalName, vocalGuid.ToString());
                attribute.ChordTemplates = new List<Manifest.ChordTemplate>();
                if (!x.IsVocal)
                {
                    var serializer = new XmlSerializer(typeof(RocksmithSngCreator.Serialization.Song));
                    var song = (RocksmithSngCreator.Serialization.Song)serializer.Deserialize(File.OpenRead(x.SongXml.File));
                    attribute.AverageTempo = x.AverageTempo;
                    attribute.RepresentativeArrangement = true;
                    attribute.SongPartition = songPartitioncnt++;
                    attribute.SongLength = song.SongLength;
                    attribute.LastConversionDateTime = song.LastConversionDateTime;
                    attribute.Bends = song.HasBends();
                    attribute.Harmonics = song.HasHarmonics();
                    attribute.HOPOs = song.HasHOPOs();
                    attribute.PalmMutes = song.HasPalmMutes();
                    attribute.Slides = song.HasSlides();
                    attribute.Sustain = song.HasSustain();
                    attribute.Tremolo = song.HasTremolo();
                    attribute.TargetScore = 100000;
                    attribute.ToneUnlockScore = 70000;
                    attribute.SongDifficulty = x.SongDifficulty;
                    GenerateChordTemplateData(attribute, song);
                    GeneratePhraseData(attribute, song);
                    GenerateSectionData(attribute, song);
                    GeneratePhraseIterationsData(attribute, song);
                }
                var attrDict = new Dictionary<string, Manifest.Attributes>();
                attrDict.Add("Attributes", attribute);
                manifest.Entries.Add(attribute.PersistentID, attrDict);
            }
            manifest.ModelName = "GRSong_Asset";
            manifest.IterationVersion = 2;
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }

        private void GeneratePhraseIterationsData(Manifest.Attributes attribute, RocksmithSngCreator.Serialization.Song song)
        {
            for (int i = 0; i < song.PhraseIterations.PhraseIteration.Length; i++)
            {

                var phraseIteration = song.PhraseIterations.PhraseIteration[i];
                var phrase = song.Phrases.Phrase[phraseIteration.PhraseId];
                var endTime = i >= song.PhraseIterations.Count - 1 ? song.SongLength : song.PhraseIterations.PhraseIteration[i + 1].Time;
                var phraseIt = new Manifest.PhraseIteration()
                {
                    StartTime = phraseIteration.Time,
                    EndTime = endTime,
                    PhraseIndex = phraseIteration.PhraseId,
                    Name = phrase.Name,
                    MaxDifficulty = phrase.MaxDifficulty
                };
                phraseIt.MaxScorePerDifficulty = new List<float>();
                attribute.PhraseIterations.Add(phraseIt);
            }
            var noteCnt = 0;
            foreach (var y in attribute.PhraseIterations)
            {
                noteCnt += GetNoteCount(y.StartTime, y.EndTime, song.Levels.Level[y.MaxDifficulty].Notes.Note);
                if (song.Levels.Level[y.MaxDifficulty].Chords.Count > 0)
                    noteCnt += GetChordCount(y.StartTime, y.EndTime, song.Levels.Level[y.MaxDifficulty].Chords.Chord);
            }
            attribute.Score_MaxNotes = noteCnt;
            attribute.Score_PNV = ((float)attribute.TargetScore) / noteCnt;

            foreach (var y in attribute.PhraseIterations)
            {
                var phrase = song.Phrases.Phrase[y.PhraseIndex];
                for (int o = 0; o <= phrase.MaxDifficulty; o++)
                {
                    var multiplier = ((float)(o + 1)) / (phrase.MaxDifficulty + 1);
                    var pnv = attribute.Score_PNV;
                    var noteCount = GetNoteCount(y.StartTime, y.EndTime, song.Levels.Level[o].Notes.Note);
                    if (song.Levels.Level[o].Chords.Count > 0)
                        noteCount += GetChordCount(y.StartTime, y.EndTime, song.Levels.Level[o].Chords.Chord);
                    var score = pnv * noteCount * multiplier;
                    y.MaxScorePerDifficulty.Add(score);
                }
            }
        }

        private static void GenerateSectionData(Manifest.Attributes attribute, RocksmithSngCreator.Serialization.Song song)
        {
            for (int i = 0; i < song.Sections.Section.Length; i++)
            {
                var section = song.Sections.Section[i];
                var sect = new Manifest.Section()
                {
                    Name = section.Name,
                    Number = section.Number,
                    StartTime = section.StartTime,
                    EndTime = (i >= song.Sections.Section.Length - 1) ? song.SongLength : song.Sections.Section[i + 1].StartTime,
                    UIName = String.Format("$[6007] {0} [1]", section.Name)

                };
                var phraseIterStart = -1;
                var phraseIterEnd = 0;
                var isSolo = false;
                for (int o = 0; o < song.PhraseIterations.Count; o++)
                {
                    var phraseIter = song.PhraseIterations.PhraseIteration[o];
                    if (phraseIterStart == -1 && phraseIter.Time >= sect.StartTime)
                        phraseIterStart = o;
                    if (phraseIter.Time >= sect.EndTime)
                        break;
                    phraseIterEnd = o;
                    if (song.Phrases.Phrase[phraseIter.PhraseId].Solo > 0)
                        isSolo = true;
                }
                sect.StartPhraseIterationIndex = phraseIterStart;
                sect.EndPhraseIterationIndex = phraseIterEnd;
                sect.IsSolo = isSolo;
                attribute.Sections.Add(sect);
            }
        }

        private static void GeneratePhraseData(Manifest.Attributes attribute, RocksmithSngCreator.Serialization.Song song)
        {
            var ind = 0;
            foreach (var y in song.Phrases.Phrase)
            {
                int itcount = 0;
                foreach (var z in song.PhraseIterations.PhraseIteration)
                    if (z.PhraseId == ind)
                        itcount++;
                attribute.Phrases.Add(new Manifest.Phrase()
                {
                    IterationCount = itcount,
                    MaxDifficulty = y.MaxDifficulty,
                    Name = y.Name
                });
                ind++;
            }
        }

        private static void GenerateChordTemplateData(Manifest.Attributes attribute, RocksmithSngCreator.Serialization.Song song)
        {
            var ind = 0;
            foreach (var y in song.ChordTemplates.ChordTemplate)
                attribute.ChordTemplates.Add(new Manifest.ChordTemplate()
                {
                    ChordId = ind++,
                    ChordName = y.ChordName,
                    Fingers = new List<int>() { y.Finger0, y.Finger1, y.Finger2, y.Finger3, y.Finger4, y.Finger5 },
                    Frets = new List<int>() { y.Fret0, y.Fret1, y.Fret2, y.Fret3, y.Fret4, y.Fret5 }
                });
        }
        private int GetNoteCount(float startTime, float endTime, IEnumerable<RocksmithSngCreator.Serialization.SongNote> notes)
        {
            int count = 0;
            for (int i = 0; i < notes.Count(); i++)
            {
                if(notes.ElementAt(i).Time < endTime)
                if (notes.ElementAt(i).Time >= startTime)
                    count++;
            }
            return count;
        }
        private int GetChordCount(float startTime, float endTime, IEnumerable<RocksmithSngCreator.Serialization.SongChord> chords)
        {
            int count = 0;
            for (int i = 0; i < chords.Count(); i++)
            {
                if (chords.ElementAt(i).Time < endTime)
                    if (chords.ElementAt(i).Time >= startTime)
                        count++;
            }
            return count;
        }
    }
}

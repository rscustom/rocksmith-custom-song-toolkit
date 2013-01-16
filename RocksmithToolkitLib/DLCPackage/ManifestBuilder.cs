using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage
{
    public class ManifestBuilder
    {
        public AggregateGraph.AggregateGraph AggregateGraph { get; set; }
        public Manifest.Manifest Manifest { get; private set; }
        public ManifestBuilder()
        {
            Manifest = new Manifest.Manifest();
            SectionUINames = new Dictionary<string, string>();
            SectionUINames.Add("intro", "$[6005] Intro [1]");
            SectionUINames.Add("outro", "$[6006] Outro [1]");
            SectionUINames.Add("verse", "$[6007] Verse [1]");
            SectionUINames.Add("chorus", "$[6008] Chorus [1]");
            SectionUINames.Add("bridge", "$[6009] Bridge [1]");
            SectionUINames.Add("solo", "$[6010] Solo [1]");
            SectionUINames.Add("ambient", "$[6011] Ambient [1]");
            SectionUINames.Add("breakdown", "$[6012] Breakdown [1]");
            SectionUINames.Add("interlude", "$[6013] Interlude [1]");
            SectionUINames.Add("prechorus", "$[6014] Pre Chorus [1]");
            SectionUINames.Add("transition", "$[6015] Transition [1]");
            SectionUINames.Add("postchorus", "$[6016] Post Chorus [1]");
            SectionUINames.Add("hook", "$[6017] Hook [1]");
            SectionUINames.Add("riff", "$[6018] Riff [1]");
            SectionUINames.Add("fadein", "$[6077] Fade In [1]");
            SectionUINames.Add("fadeout", "$[6078] Fade Out [1]");
            SectionUINames.Add("buildup", "$[6079] Buildup [1]");
            SectionUINames.Add("preverse", "$[6080] Pre Verse [1]");
            SectionUINames.Add("modverse", "$[6081] Modulated Verse [1]");
            SectionUINames.Add("postvs", "$[6082] Post Verse [1]");
            SectionUINames.Add("variation", "$[6083] Variation [1]");
            SectionUINames.Add("modchorus", "$[6084] Modulated Chorus [1]");
            SectionUINames.Add("head", "$[6085] Head [1]");
            SectionUINames.Add("modbridge", "$[6086] Modulated Bridge [1]");
            SectionUINames.Add("melody", "$[6087] Melody [1]");
            SectionUINames.Add("postbrdg", "$[6088] Post Bridge [1]");
            SectionUINames.Add("prebrdg", "$[6089] Pre Bridge [1]");
            SectionUINames.Add("vamp", "$[6090] Vamp [1]");
            SectionUINames.Add("noguitar", "$[6091] No Guitar [1]");
            SectionUINames.Add("silence", "$[6092] Silence [1]");
        }
        public string GenerateManifest(string dlcName, IList<Arrangement> arrangements, SongInfo songInfo)
        {
            var manifest = Manifest;
            manifest.Entries = new Dictionary<string, Dictionary<string, Attributes>>();
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
                var attribute = new Attributes();
                Guid id = x.Id;
                attribute.AlbumArt = String.Format("urn:llid:{0}", AggregateGraph.AlbumArt.LLID);
                attribute.AlbumNameSort = attribute.AlbumName = songInfo.Album;
                attribute.ArrangementName = x.Name;
                attribute.ArtistName = attribute.ArtistNameSort = songInfo.Artist;
                attribute.AssociatedTechniques = new List<string>();//
                attribute.BarChords = x.BarChords;
                attribute.Bends = false;//
                attribute.BinaryVersion = 49;
                attribute.BlockAsset = String.Format("urn:emergent-world:{0}", AggregateGraph.XBlock.Name);
                attribute.ChordTemplates = null;//
                attribute.DISC_DLC_OTHER = "Disc";
                attribute.DisplayName = songInfo.SongDisplayName;
                attribute.DLCPreview = false;
                attribute.DoubleStops = x.DoubleStops;
                attribute.DropDPowerChords = x.DropDPowerChords;
                if (x.IsVocal)
                    attribute.DynamicVisualDensity = new List<float>{
                        4.5f, 4.3000001907348633f, 4.0999999046325684f, 3.9000000953674316f, 3.7000000476837158f,
                        3.5f, 3.2999999523162842f, 3.0999999046325684f, 2.9000000953674316f, 2.7000000476837158f,
                        2.5f, 2.2999999523162842f, 2.0999999046325684f,
                        2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f};
                else
                    attribute.DynamicVisualDensity = new List<float>
                    {
                        4.5f, 4.0f,
                        3.5999999046325684f, 3.2000000476837158f, 2.9000000953674316f, 2.5999999046325684f,
                        2.2999999523162842f, 2.0999999046325684f, 1.8999999761581421f, 1.7000000476837158f,
                        1.5f,
                        1.3999999761581421f, 1.2999999523162842f, 1.2000000476837158f, 1.1000000238418579f,
                        1.0f, 1.0f, 1.0f, 1.0f, 1.0f
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
                attribute.Harmonics = false;
                attribute.HOPOs = false;
                attribute.InputEvent = x.IsVocal ? "Play_Tone_Standard_Mic" : "Play_Tone_";
                attribute.IsDemoSong = false;
                attribute.IsDLC = true;
                attribute.LastConversionDateTime = "";
                attribute.MasterID_PS3 = 0;
                attribute.MasterID_Xbox360 = 504;
                attribute.MaxPhraseDifficulty = 0;
                attribute.OpenChords = x.OpenChords;
                attribute.PalmMutes = false;
                attribute.PersistentID = id.ToString().Replace("-", "").ToUpper();
                attribute.PhraseIterations = new List<PhraseIteration>();
                attribute.Phrases = new List<Phrase>();
                attribute.PluckedType = "Picked";
                attribute.PowerChords = x.PowerChords;
                attribute.Prebends = x.PreBends;
                attribute.RelativeDifficulty = x.RelativeDifficulty;
                attribute.RepresentativeArrangement = false;
                attribute.Score_MaxNotes = 0;
                attribute.Score_PNV = 0;
                attribute.Sections = new List<Section>();
                attribute.Shipping = true;
                attribute.SlapAndPop = x.SlapAndPop;
                attribute.Slides = false;
                attribute.SongAsset = String.Format("urn:llid:{0}", x.SongFile.LLID);
                attribute.SongEvent = String.Format("Play_{0}", "DammitClean");
                attribute.SongKey = dlcName;
                attribute.SongLength = 0;
                attribute.SongNameSort = attribute.SongName = songInfo.SongDisplayName;
                attribute.SongPartition = 0;
                attribute.SongXml = String.Format("urn:llid:{0}", x.SongXml.LLID);
                attribute.SongYear = songInfo.SongYear;
                attribute.Sustain = false;
                attribute.Syncopation = false;
                attribute.TargetScore = 0;
                attribute.ToneUnlockScore = 0;
                attribute.Tremolo = false;
                attribute.Tuning = x.Tuning;
                attribute.TwoFingerPlucking = false;
                attribute.TwoHandTapping = false;
                attribute.UnlockKey = "";
                attribute.Vibrato = x.Vibrato;
                attribute.VocalsAssetId = x.IsVocal ? "" : String.Format("{1}|GRSong_{0}", vocalName, vocalGuid.ToString());
                attribute.ChordTemplates = new List<ChordTemplate>();
                if (!x.IsVocal)
                {
                    var serializer = new XmlSerializer(typeof(Song));
                    Song song;
                    using (var fileStream = File.OpenRead(x.SongXml.File))
                    {
                        song = (Song)serializer.Deserialize(fileStream);
                    }
                    attribute.AverageTempo = songInfo.AverageTempo;
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
                    attribute.SongDifficulty = (float)song.PhraseIterations.Average(it => song.Phrases[it.PhraseId].MaxDifficulty);
                    GenerateChordTemplateData(attribute, song);
                    GeneratePhraseData(attribute, song);
                    GenerateSectionData(attribute, song);
                    GeneratePhraseIterationsData(attribute, song);
                }
                var attrDict = new Dictionary<string, Attributes> {{"Attributes", attribute}};
                manifest.Entries.Add(attribute.PersistentID, attrDict);
            }
            manifest.ModelName = "GRSong_Asset";
            manifest.IterationVersion = 2;
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }

        private void GeneratePhraseIterationsData(Attributes attribute, Song song)
        {
            if (song.PhraseIterations == null)
            {
                return;
            }
            for (int i = 0; i < song.PhraseIterations.Length; i++)
            {

                var phraseIteration = song.PhraseIterations[i];
                var phrase = song.Phrases[phraseIteration.PhraseId];
                var endTime = i >= song.PhraseIterations.Length - 1 ? song.SongLength : song.PhraseIterations[i + 1].Time;
                var phraseIt = new PhraseIteration
                {
                    StartTime = phraseIteration.Time,
                    EndTime = endTime,
                    PhraseIndex = phraseIteration.PhraseId,
                    Name = phrase.Name,
                    MaxDifficulty = phrase.MaxDifficulty,
                    MaxScorePerDifficulty = new List<float>()
                };
                attribute.PhraseIterations.Add(phraseIt);
            }
            var noteCnt = 0;
            foreach (var y in attribute.PhraseIterations)
            {
                if (song.Levels[y.MaxDifficulty].Notes != null)
                {
                    noteCnt += GetNoteCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                }
                if (song.Levels[y.MaxDifficulty].Chords != null )
                {
                    noteCnt += GetChordCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Chords);
                }
            }
            attribute.Score_MaxNotes = noteCnt;
            attribute.Score_PNV = ((float)attribute.TargetScore) / noteCnt;

            foreach (var y in attribute.PhraseIterations)
            {
                var phrase = song.Phrases[y.PhraseIndex];
                for (int o = 0; o <= phrase.MaxDifficulty; o++)
                {
                    var multiplier = ((float)(o + 1)) / (phrase.MaxDifficulty + 1);
                    var pnv = attribute.Score_PNV;
                    var noteCount = 0;
                    if (song.Levels[o].Chords != null)
                    {
                        noteCount += GetNoteCount(y.StartTime, y.EndTime, song.Levels[o].Notes);
                    }
                    if (song.Levels[o].Chords != null)
                    {
                        noteCount += GetChordCount(y.StartTime, y.EndTime, song.Levels[o].Chords);
                    }
                    var score = pnv * noteCount * multiplier;
                    y.MaxScorePerDifficulty.Add(score);
                }
            }
        }
        private Dictionary<string, string> SectionUINames { get; set; }
        private void GenerateSectionData(Attributes attribute, Song song)
        {
            if (song.Sections == null)
            {
                return;
            }
            for (int i = 0; i < song.Sections.Length; i++)
            {
                var section = song.Sections[i];
                var sect = new Section
                {
                    Name = section.Name,
                    Number = section.Number,
                    StartTime = section.StartTime,
                    EndTime = (i >= song.Sections.Length - 1) ? song.SongLength : song.Sections[i + 1].StartTime,
                    UIName = String.Format("$[6007] {0} [1]", section.Name)

                };
                var sep = sect.Name.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (sep.Length == 1)
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                        sect.UIName = uiName;
                }
                else
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                    {
                        try
                        {
                            if (Convert.ToInt32(sep[1]) != 0 || Convert.ToInt32(sep[1]) != 1)
                                uiName += String.Format("|{0}", sep[1]);
                        }
                        catch { }
                        sect.UIName = uiName;
                    }
                }
                var phraseIterStart = -1;
                var phraseIterEnd = 0;
                var isSolo = false;
                if (song.PhraseIterations != null)
                {
                    for (int o = 0; o < song.PhraseIterations.Length; o++)
                    {
                        var phraseIter = song.PhraseIterations[o];
                        if (phraseIterStart == -1 && phraseIter.Time >= sect.StartTime)
                            phraseIterStart = o;
                        if (phraseIter.Time >= sect.EndTime)
                            break;
                        phraseIterEnd = o;
                        if (song.Phrases[phraseIter.PhraseId].Solo > 0)
                            isSolo = true;
                    }
                }
                sect.StartPhraseIterationIndex = phraseIterStart;
                sect.EndPhraseIterationIndex = phraseIterEnd;
                sect.IsSolo = isSolo;
                attribute.Sections.Add(sect);
            }
        }

        private static void GeneratePhraseData(Attributes attribute, Song song)
        {
            if (song.Phrases == null)
            {
                return;
            }
            var ind = 0;
            foreach (var y in song.Phrases)
            {
                int itcount = song.PhraseIterations.Count(z => z.PhraseId == ind);
                attribute.Phrases.Add(new Phrase
                {
                    IterationCount = itcount,
                    MaxDifficulty = y.MaxDifficulty,
                    Name = y.Name
                });
                ind++;
            }
        }

        private static void GenerateChordTemplateData(Attributes attribute, Song song)
        {
            var ind = 0;
            if (song.ChordTemplates == null)
            {
                return;
            }
            foreach (var y in song.ChordTemplates)
                attribute.ChordTemplates.Add(new ChordTemplate
                {
                    ChordId = ind++,
                    ChordName = y.ChordName,
                    Fingers = new List<int> { y.Finger0, y.Finger1, y.Finger2, y.Finger3, y.Finger4, y.Finger5 },
                    Frets = new List<int> { y.Fret0, y.Fret1, y.Fret2, y.Fret3, y.Fret4, y.Fret5 }
                });
        }
        private static int GetNoteCount(float startTime, float endTime, ICollection<SongNote> notes)
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
        private static int GetChordCount(float startTime, float endTime, ICollection<SongChord> chords)
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

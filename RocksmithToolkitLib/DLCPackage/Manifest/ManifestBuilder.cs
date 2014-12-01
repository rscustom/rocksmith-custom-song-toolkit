using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using System.ComponentModel;
using System.Reflection;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class ManifestBuilder
    {
        public AggregateGraph.AggregateGraph AggregateGraph { get; set; }
        public Manifest Manifest { get; private set; }

        public ManifestBuilder()
        {
            Manifest = new Manifest();
        }

        public string GenerateManifest(string dlcName, IList<Arrangement> arrangements, SongInfo songInfo, Platform platform)
        {
            var manifest = Manifest;
            manifest.Entries = new Dictionary<string, Dictionary<string, Attributes>>();
            bool firstarrangset = false;

            Arrangement vocal = null;
            if (arrangements.Any<Arrangement>(a => a.ArrangementType == Sng.ArrangementType.Vocal))
                vocal = arrangements.Single<Arrangement>(a => a.ArrangementType == Sng.ArrangementType.Vocal);

            var manifestFunctions = new ManifestFunctions(platform.version);
            var songPartition = new SongPartition();

            foreach (var x in arrangements)
            {
                var isVocal = x.ArrangementType == Sng.ArrangementType.Vocal;
                var song = (isVocal) ? null : Song.LoadFromFile(x.SongXml.File);

                var attribute = new Attributes();
                attribute.AlbumArt = String.Format("urn:llid:{0}", AggregateGraph.AlbumArt.LLID);
                attribute.AlbumNameSort = attribute.AlbumName = songInfo.Album;
                attribute.ArrangementName = x.Name.ToString();
                attribute.ArtistName = songInfo.Artist;
                attribute.ArtistNameSort = songInfo.ArtistSort;
                attribute.AssociatedTechniques = new List<string>();
                //Should be 51 for bass, 49 for vocal and guitar
                attribute.BinaryVersion = x.ArrangementType == Sng.ArrangementType.Bass ? 51 : 49;
                attribute.BlockAsset = String.Format("urn:emergent-world:{0}", AggregateGraph.XBlock.Name);
                attribute.ChordTemplates = null;
                attribute.DISC_DLC_OTHER = "Disc";
                attribute.DisplayName = songInfo.SongDisplayName;
                attribute.DLCPreview = false;
                attribute.EffectChainMultiplayerName = string.Empty;
                attribute.EffectChainName = isVocal ? "" : (dlcName + "_" + x.ToneBase == null ? "Default" : x.ToneBase.Replace(' ', '_'));
                attribute.EventFirstTimeSortOrder = 9999;
                attribute.ExclusiveBuild = new List<object>();
                attribute.FirstArrangementInSong = false;
                if (isVocal && !firstarrangset)
                {
                    firstarrangset = true;
                    attribute.FirstArrangementInSong = true;
                }
                attribute.ForceUseXML = true;
                attribute.Genre = "PLACEHOLDER Genre";
                attribute.InputEvent = isVocal ? "Play_Tone_Standard_Mic" : "Play_Tone_";
                attribute.IsDemoSong = false;
                attribute.IsDLC = true;
                attribute.LastConversionDateTime = "";
                int masterId = isVocal ? 1 : x.MasterId;
                attribute.MasterID_PS3 = masterId;
                attribute.MasterID_Xbox360 = masterId;
                attribute.MaxPhraseDifficulty = 0;
                attribute.PersistentID = x.Id.ToString().Replace("-", "").ToUpper();
                attribute.PhraseIterations = new List<PhraseIteration>();
                attribute.Phrases = new List<Phrase>();
                attribute.PluckedType = x.PluckedType == Sng.PluckedType.Picked ? "Picked" : "Not Picked";
                attribute.RelativeDifficulty = isVocal ? 0 : song.Levels.Length;
                attribute.RepresentativeArrangement = false;
                attribute.Score_MaxNotes = 0;
                attribute.Score_PNV = 0;
                attribute.Sections = new List<Section>();
                attribute.Shipping = true;
                attribute.SongAsset = String.Format("urn:llid:{0}", x.SongFile.LLID);
                attribute.SongEvent = String.Format("Play_{0}", dlcName);
                attribute.SongKey = dlcName;
                attribute.SongLength = 0;
                attribute.SongName = songInfo.SongDisplayName;
                attribute.SongNameSort = songInfo.SongDisplayNameSort;
                attribute.SongXml = String.Format("urn:llid:{0}", x.SongXml.LLID);
                attribute.SongYear = songInfo.SongYear;
                attribute.TargetScore = 0;
                attribute.ToneUnlockScore = 0;
                attribute.TwoHandTapping = false;
                attribute.UnlockKey = "";
                attribute.Tuning = x.Tuning;
                attribute.VocalsAssetId = x.ArrangementType == Sng.ArrangementType.Vocal ? "" : (vocal != null) ? String.Format("{0}|GRSong_{1}", vocal.Id, vocal.Name) : "";
                attribute.ChordTemplates = new List<ChordTemplate>();
                manifestFunctions.GenerateDynamicVisualDensity(attribute, song, x, GameVersion.RS2012);

                if (!isVocal)
                {
                    #region "Associated Techniques"

                    attribute.PowerChords = song.HasPowerChords();
                    if (song.HasPowerChords()) AssociateTechniques(x, attribute, "PowerChords");
                    attribute.BarChords = song.HasBarChords();
                    if (song.HasBarChords()) AssociateTechniques(x, attribute, "BarChords");
                    attribute.OpenChords = song.HasOpenChords();
                    if (song.HasOpenChords()) AssociateTechniques(x, attribute, "ChordIntro");
                    attribute.DoubleStops = song.HasDoubleStops();
                    if (song.HasDoubleStops()) AssociateTechniques(x, attribute, "DoubleStops");
                    attribute.Sustain = song.HasSustain();
                    if (song.HasSustain()) AssociateTechniques(x, attribute, "Sustain");
                    attribute.Bends = song.HasBends();
                    if (song.HasBends()) AssociateTechniques(x, attribute, "Bends");
                    attribute.Slides = song.HasSlides();
                    if (song.HasSlides()) AssociateTechniques(x, attribute, "Slides");
                    attribute.Tremolo = song.HasTremolo();
                    if (song.HasTremolo()) AssociateTechniques(x, attribute, "Tremolo");
                    attribute.SlapAndPop = song.HasSlapAndPop();
                    if (song.HasSlapAndPop()) AssociateTechniques(x, attribute, "Slap");
                    attribute.Harmonics = song.HasHarmonics();
                    if (song.HasHarmonics()) AssociateTechniques(x, attribute, "Harmonics");
                    attribute.PalmMutes = song.HasPalmMutes();
                    if (song.HasPalmMutes()) AssociateTechniques(x, attribute, "PalmMutes");
                    attribute.HOPOs = song.HasHOPOs();
                    if (song.HasHOPOs()) AssociateTechniques(x, attribute, "HOPOs");
                    attribute.FretHandMutes = song.HasFretHandMutes();
                    if (song.HasFretHandMutes()) AssociateTechniques(x, attribute, "FretHandMutes");
                    attribute.DropDPowerChords = song.HasDropDPowerChords();
                    if (song.HasDropDPowerChords()) AssociateTechniques(x, attribute, "DropDPowerChords");
                    attribute.Prebends = song.HasPrebends();
                    if (song.HasPrebends()) AssociateTechniques(x, attribute, "Prebends");
                    attribute.Vibrato = song.HasVibrato();
                    if (song.HasVibrato()) AssociateTechniques(x, attribute, "Vibrato");

                    //Bass exclusive
                    attribute.TwoFingerPlucking = song.HasTwoFingerPlucking();
                    if (song.HasTwoFingerPlucking()) AssociateTechniques(x, attribute, "Plucking");
                    attribute.FifthsAndOctaves = song.HasFifthsAndOctaves();
                    if (song.HasFifthsAndOctaves()) AssociateTechniques(x, attribute, "Octave");
                    attribute.Syncopation = song.HasSyncopation();
                    if (song.HasSyncopation()) AssociateTechniques(x, attribute, "Syncopation");

                    #endregion

                    attribute.AverageTempo = songInfo.AverageTempo;
                    attribute.RepresentativeArrangement = true;
                    attribute.SongPartition = songPartition.GetSongPartition(x.Name, x.ArrangementType);
                    attribute.SongLength = (float)Math.Round((decimal)song.SongLength, 3, MidpointRounding.AwayFromZero); //rounded
                    attribute.LastConversionDateTime = song.LastConversionDateTime;
                    attribute.TargetScore = 100000;
                    attribute.ToneUnlockScore = 70000;
                    attribute.SongDifficulty = (float)song.PhraseIterations.Average(it => song.Phrases[it.PhraseId].MaxDifficulty);
                    manifestFunctions.GenerateChordTemplateData(attribute, song);
                    manifestFunctions.GeneratePhraseData(attribute, song);
                    manifestFunctions.GenerateSectionData(attribute, song);
                    manifestFunctions.GeneratePhraseIterationsData(attribute, song, platform.version);
                }
                var attrDict = new Dictionary<string, Attributes> { { "Attributes", attribute } };
                manifest.Entries.Add(attribute.PersistentID, attrDict);
            }
            manifest.ModelName = "GRSong_Asset";
            manifest.IterationVersion = 2;
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }

        private void AssociateTechniques(Arrangement x, Attributes att, string technique)
        {
            att.AssociatedTechniques.Add(String.Format("{0}{1}", x.ArrangementType == Sng.ArrangementType.Bass ? "Bass" : "", technique));
        }
    }
}

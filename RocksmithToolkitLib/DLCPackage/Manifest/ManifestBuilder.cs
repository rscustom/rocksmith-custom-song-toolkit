using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.Manifest.Functions;
using RocksmithToolkitLib.XML;
using System.ComponentModel;
using System.Reflection;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    // RS1 ONLY
    // causes RS1 in-game hang after tuning ... THESE DON'T BELONG HERE ;<(
    // attribute.JapaneseArtist = songInfo.JapaneseArtistName;
    // attribute.JapaneseSongName = songInfo.JapaneseSongName;

    /// <summary>
    /// Manifest Builder for RS1 ONLY
    /// </summary>
    public class ManifestBuilder
    {
        public AggregateGraph.AggregateGraph AggregateGraph { get; set; }
        public Manifest Manifest { get; private set; }

        public ManifestBuilder()
        {
            Manifest = new Manifest();
        }

        public string GenerateManifest(string dlcKey, IList<Arrangement> arrangements, SongInfo songInfo, Platform platform)
        {
            var manifest = Manifest;
            manifest.Entries = new Dictionary<string, Dictionary<string, Attributes>>();
            bool firstarrangset = false;

            Arrangement vocal = null;
            if (arrangements.Any<Arrangement>(a => a.ArrangementType == Sng.ArrangementType.Vocal))
                vocal = arrangements.Single<Arrangement>(a => a.ArrangementType == Sng.ArrangementType.Vocal);

            var manifestFunctions = new ManifestFunctions(platform.version);
            var songPartition = new SongPartition();

            foreach (var arr in arrangements)
            {
                var isVocal = arr.ArrangementType == Sng.ArrangementType.Vocal;
                var song = (isVocal) ? null : Song.LoadFromFile(arr.SongXml.File);
                var attribute = new Attributes();

                attribute.AlbumArt = String.Format("urn:llid:{0}", AggregateGraph.AlbumArt.LLID);
                attribute.AlbumName = songInfo.Album;
                attribute.AlbumNameSort = songInfo.AlbumSort;
                attribute.ArrangementName = arr.ArrangementName.ToString();
                attribute.ArtistName = songInfo.Artist;
                attribute.ArtistNameSort = songInfo.ArtistSort;
                attribute.AssociatedTechniques = new List<string>();
                //Should be 51 for bass, 49 for vocal and guitar
                attribute.BinaryVersion = arr.ArrangementType == Sng.ArrangementType.Bass ? 51 : 49;
                attribute.BlockAsset = String.Format("urn:emergent-world:{0}", AggregateGraph.XBlock.Name);

                if (!isVocal)
                    attribute.ChordTemplates = new List<ChordTemplate>();

                attribute.DISC_DLC_OTHER = "Disc";
                attribute.DisplayName = songInfo.SongDisplayName;
                attribute.DLCPreview = false;
                attribute.EffectChainMultiplayerName = String.Empty;
                // NOTE: attribute.EffectChainName = tone.Key
               // attribute.EffectChainName = isVocal ? "" : (arr.ToneBase == null ? throw new DataException("<ERROR> " + arr.ArrangementName + " ToneBase/ToneKey is null ...") : arr.ToneBase);

                if (isVocal)
                    attribute.EffectChainName = "";
                else
                {
                    if (arr.ToneBase == null)
                        throw new DataException("<ERROR> " + arr.ArrangementName + " ToneBase/ToneKey is null ...");
                    else // ToneBase w/ space <=> tone.Name w/ space <=> tone.Key w/o space
                        attribute.EffectChainName = arr.ToneBase.GetValidKey(isTone: true);
                }
                attribute.EventFirstTimeSortOrder = 9999;
                attribute.ExclusiveBuild = new List<object>();
                attribute.FirstArrangementInSong = false;
                attribute.ForceUseXML = true;
                attribute.Genre = "PLACEHOLDER Genre";
                attribute.InputEvent = isVocal ? "Play_Tone_Standard_Mic" : "Play_Tone_";
                attribute.IsDemoSong = false;
                attribute.IsDLC = true;

                if (!isVocal)
                    attribute.LastConversionDateTime = song.LastConversionDateTime;

                // TODO: improve this
                int masterId = isVocal ? 1 : arr.MasterId;
                attribute.MasterID_PS3 = masterId;
                attribute.MasterID_Xbox360 = masterId;

                if (!isVocal)
                    attribute.MaxPhraseDifficulty = manifestFunctions.GetMaxDifficulty(song);

                attribute.PersistentID = arr.Id.ToString().Replace("-", "").ToUpper();
                attribute.PhraseIterations = new List<PhraseIteration>();
                attribute.Phrases = new List<Phrase>();
                attribute.PluckedType = arr.PluckedType == Sng.PluckedType.Picked ? "Picked" : "Not Picked"; // TODO: FIXME

                if (isVocal)
                {
                    attribute.RelativeDifficulty = 0;
                    attribute.RepresentativeArrangement = false;
                }
                else
                {
                    attribute.RelativeDifficulty = song.Levels.Length;  //TODO: FIXME
                    attribute.RepresentativeArrangement = true;
                }

                attribute.Score_MaxNotes = 0;
                attribute.Score_PNV = 0;
                attribute.Sections = new List<Section>();
                attribute.Shipping = true;
                attribute.SongAsset = String.Format("urn:llid:{0}", arr.SongFile.LLID);

                if (!isVocal)
                    attribute.SongDifficulty = (float)song.PhraseIterations.Average(it => song.Phrases[it.PhraseId].MaxDifficulty);

                attribute.SongEvent = String.Format("Play_{0}", dlcKey);
                attribute.SongKey = dlcKey;
                attribute.SongLength = 0;
                attribute.SongName = songInfo.SongDisplayName;
                attribute.SongNameSort = songInfo.SongDisplayNameSort;

                if (!isVocal)
                {
                    attribute.SongPartition = songPartition.GetSongPartition(arr.ArrangementName, arr.ArrangementType);
                    attribute.SongLength = (float)Math.Round((decimal)song.SongLength, 3, MidpointRounding.AwayFromZero); //rounded
                }

                attribute.SongXml = String.Format("urn:llid:{0}", arr.SongXml.LLID);
                attribute.SongYear = songInfo.SongYear;

                if (isVocal)
                {
                    attribute.TargetScore = 0;
                    attribute.ToneUnlockScore = 0;
                }
                else
                {
                    attribute.TargetScore = 100000;
                    attribute.ToneUnlockScore = 70000;
                }

                attribute.TwoHandTapping = false;
                attribute.UnlockKey = "";
                attribute.Tuning = arr.Tuning;
                attribute.VocalsAssetId = arr.ArrangementType == Sng.ArrangementType.Vocal ? "" : (vocal != null) ? String.Format("{0}|GRSong_{1}", vocal.Id, vocal.ArrangementName) : "";

                if (!firstarrangset && arr == arrangements.First())
                {
                    firstarrangset = true;
                    attribute.FirstArrangementInSong = true;
                }

                manifestFunctions.GenerateDynamicVisualDensity(attribute, song, arr, GameVersion.RS2012);

                if (!isVocal)
                {
                    #region Include Associated Techniques

                    attribute.PowerChords = song.HasPowerChords();
                    if (attribute.PowerChords) AssociateTechniques(arr, attribute, "PowerChords");
                    attribute.BarChords = song.HasBarChords();
                    if (attribute.BarChords) AssociateTechniques(arr, attribute, "BarChords");
                    attribute.OpenChords = song.HasOpenChords();
                    if (attribute.OpenChords) AssociateTechniques(arr, attribute, "ChordIntro");
                    attribute.DoubleStops = song.HasDoubleStops();
                    if (attribute.DoubleStops) AssociateTechniques(arr, attribute, "DoubleStops");
                    attribute.Sustain = song.HasSustain();
                    if (attribute.Sustain) AssociateTechniques(arr, attribute, "Sustain");
                    attribute.Bends = song.HasBends();
                    if (attribute.Bends) AssociateTechniques(arr, attribute, "Bends");
                    attribute.Slides = song.HasSlides();
                    if (attribute.Slides) AssociateTechniques(arr, attribute, "Slides");
                    attribute.Tremolo = song.HasTremolo();
                    if (attribute.Tremolo) AssociateTechniques(arr, attribute, "Tremolo");
                    attribute.SlapAndPop = song.HasSlapAndPop();
                    if (attribute.SlapAndPop) AssociateTechniques(arr, attribute, "Slap");
                    attribute.Harmonics = song.HasHarmonics();
                    if (song.HasHarmonics()) AssociateTechniques(arr, attribute, "Harmonics");
                    attribute.PalmMutes = song.HasPalmMutes();
                    if (attribute.Harmonics) AssociateTechniques(arr, attribute, "PalmMutes");
                    attribute.HOPOs = song.HasHOPOs();
                    if (attribute.HOPOs) AssociateTechniques(arr, attribute, "HOPOs");
                    attribute.FretHandMutes = song.HasFretHandMutes();
                    if (attribute.FretHandMutes) AssociateTechniques(arr, attribute, "FretHandMutes");
                    attribute.DropDPowerChords = song.HasDropDPowerChords();
                    if (attribute.DropDPowerChords) AssociateTechniques(arr, attribute, "DropDPowerChords");
                    attribute.Prebends = song.HasPrebends();
                    if (attribute.Prebends) AssociateTechniques(arr, attribute, "Prebends");
                    attribute.Vibrato = song.HasVibrato();
                    if (attribute.Vibrato) AssociateTechniques(arr, attribute, "Vibrato");

                    //Bass exclusive
                    attribute.TwoFingerPlucking = song.HasTwoFingerPlucking();
                    if (attribute.TwoFingerPlucking) AssociateTechniques(arr, attribute, "Plucking");
                    attribute.FifthsAndOctaves = song.HasFifthsAndOctaves();
                    if (attribute.FifthsAndOctaves) AssociateTechniques(arr, attribute, "Octave");
                    attribute.Syncopation = song.HasSyncopation();
                    if (attribute.Syncopation) AssociateTechniques(arr, attribute, "Syncopation");

                    #endregion

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

            var jsonManifest = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            return jsonManifest;
        }

        private void AssociateTechniques(Arrangement x, Attributes att, string technique)
        {
            var result = String.Format("{0}{1}", x.ArrangementType == Sng.ArrangementType.Bass ? "Bass" : "", technique);
            att.AssociatedTechniques.Add(result);
        }
    }
}

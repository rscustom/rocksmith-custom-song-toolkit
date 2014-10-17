﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class Attributes2014 : AttributesHeader2014, IAttributes {
        public SongArrangementProperties2014 ArrangementProperties { get; set; }
        public int ArrangementSort { get; set; }
        public int ArrangementType { get; set; }
        public string BlockAsset { get; set; }
        public Dictionary<string, Dictionary<string, object>> Chords { get; set; } //Problem in 3rd sublevel that can be a list or not
        public List<ChordTemplate> ChordTemplates { get; set; }
        public List<float> DynamicVisualDensity { get; set; }
        public string FullName { get; set; }
        public string LastConversionDateTime { get; set; }
        public int MasterID_PS3 { get; set; }
        public int MasterID_Xbox360 { get; set; }
        public int MaxPhraseDifficulty { get; set; }
        public List<PhraseIteration> PhraseIterations { get; set; }
        public List<Phrase> Phrases { get; set; }
        public string PreviewBankPath { get; set; }
        public int RelativeDifficulty { get; set; }
        public float Score_MaxNotes { get; set; }
        public float Score_PNV { get; set; }
        public List<Section> Sections { get; set; }
        public string ShowlightsXML { get; set; }
        public string SongAsset { get; set; }
        public float SongAverageTempo { get; set; }
        public string SongBank { get; set; }
        public string SongEvent { get; set; }
        public float SongOffset { get; set; }
        public int SongPartition { get; set; }
        public string SongXml { get; set; }
        public int TargetScore { get; set; }
        public Dictionary<string, Dictionary<string, object>> Techniques { get; set; } //Problem in 3rd sublevel that can be a list or not
        public string Tone_A { get; set; }
        public string Tone_B { get; set; }
        public string Tone_Base { get; set; }
        public string Tone_C { get; set; }
        public string Tone_D { get; set; }
        public string Tone_Multiplayer { get; set; }
        public List<Tone2014> Tones { get; set; }
        public string InputEvent { get; set; } //Vocals only
        public float SongVolume { get; set; } //Customs only (to easy platform conversion) its float!
        public float? PreviewVolume { get; set; } //Customs only like above

        public Attributes2014() {}

        public Attributes2014(string arrangementFileName, Arrangement arrangement, DLCPackageData info, Platform platform)
            : base(arrangementFileName, arrangement, info, platform)
        {

            #region VARIABLES

            var dlcName = info.Name.ToLower();
            
            var xblockUrn = String.Format(URN_TEMPLATE_SHORT, TagValue.EmergentWorld.GetDescription(), dlcName);
            var showlightUrn = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.XML.GetDescription(), String.Format("{0}_showlights", dlcName));
            var songXmlUrn = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.XML.GetDescription(), String.Format(AggregateGraph2014.NAME_ARRANGEMENT, dlcName, arrangementFileName));
            var songSngUrn = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.MusicgameSong.GetDescription(), String.Format(AggregateGraph2014.NAME_ARRANGEMENT, dlcName, arrangementFileName));

            var manifestFunctions = new ManifestFunctions(platform.version);
            
            #endregion

            #region FILL ATTRIBUTES

            ArrangementSort = arrangement.ArrangementSort;
            BlockAsset = xblockUrn;
            manifestFunctions.GenerateDynamicVisualDensity(this, SongContent, arrangement);
            FullName = String.Format(AggregateGraph2014.NAME_ARRANGEMENT, info.Name, arrangement.Name);
            MasterID_PS3 = (IsVocal) ? -1 : arrangement.MasterId;
            MasterID_Xbox360 = (IsVocal) ? -1 : arrangement.MasterId;
            PreviewBankPath = String.Format("song_{0}_preview.bnk", info.Name.ToLower());
            RelativeDifficulty = 0; //Always 0 in RS2014
            ShowlightsXML = showlightUrn;
            SongAsset = songSngUrn;
            SongBank = String.Format("song_{0}.bnk", info.Name.ToLower());
            SongEvent = String.Format("Play_{0}", info.Name);
            SongXml = songXmlUrn;
            SongVolume = info.Volume;
            PreviewVolume = info.PreviewVolume ?? SongVolume;
                
            // Only for Vocal
            if (IsVocal)
                InputEvent = "Play_Tone_Standard_Mic";

            // Only for instruments
            if (!IsVocal)
            {
                ArrangementProperties = SongContent.ArrangementProperties;
                ArrangementProperties.BassPick = (int)arrangement.PluckedType;
                ArrangementProperties.PathLead = Convert.ToInt32(arrangement.RouteMask == DLCPackage.RouteMask.Lead);
                ArrangementProperties.PathRhythm = Convert.ToInt32(arrangement.RouteMask == DLCPackage.RouteMask.Rhythm);
                ArrangementProperties.PathBass = Convert.ToInt32(arrangement.RouteMask == DLCPackage.RouteMask.Bass);
                ArrangementProperties.RouteMask = (int)arrangement.RouteMask;

                // BONUS ARRANGEMENT
                ArrangementProperties.BonusArr = Convert.ToInt32(arrangement.BonusArr);

                if (arrangement.Name == Sng.ArrangementName.Combo)
                { //Exclusive condition
                    if (arrangement.RouteMask == DLCPackage.RouteMask.Lead)
                        ArrangementType = (int)Sng.ArrangementName.Lead;
                    else if (arrangement.RouteMask == DLCPackage.RouteMask.Rhythm)
                        ArrangementType = (int)Sng.ArrangementName.Rhythm;
                    else
                        ArrangementType = (int)arrangement.Name;
                }
                else
                    ArrangementType = (int)arrangement.Name;

                //Chords        -- //TODO: MISSING GENERATE

                ChordTemplates = new List<ChordTemplate>();
                manifestFunctions.GenerateChordTemplateData(this, SongContent);

                LastConversionDateTime = SongContent.LastConversionDateTime;
                MaxPhraseDifficulty = manifestFunctions.GetMaxDifficulty(SongContent);

                TargetScore = 100000;
                PhraseIterations = new List<PhraseIteration>();
                manifestFunctions.GeneratePhraseIterationsData(this, SongContent, platform.version);
                //Score_MaxNotes -- Generated on function above
                //Score_PNV      -- Generated on function above

                Phrases = new List<Phrase>();
                manifestFunctions.GeneratePhraseData(this, SongContent);

                Sections = new List<Section>();
                manifestFunctions.GenerateSectionData(this, SongContent);

                SongAverageTempo = SongContent.AverageTempo;
                SongOffset = arrangement.Sng2014.Metadata.StartTime * -1;

                //SongPartition  -- Generated in DLCPackageCreator after this constructor

                //Techniques     -- //TODO: MISSING GENERATE
                /*
                 Techniques: {
                       DiffLevelID: {
                           // 4 : 0,[6,9],[13],[12,15] // unpitchedSlides, tremolo, hopo
                           PhraseIterationIndex: [TechID, TechID...]
                       }
                 }
                 */

                //Fix for Dead tones
                var it = info.TonesRS2014;
                Tones = new List<Tone2014>();

                Tone_A = GetToneName(arrangement.ToneA, it);
                Tone_B = GetToneName(arrangement.ToneB, it);
                Tone_Base = GetToneName(arrangement.ToneBase, it);
                Tone_C = GetToneName(arrangement.ToneC, it);
                Tone_D = GetToneName(arrangement.ToneD, it);
                Tone_Multiplayer = GetToneName(arrangement.ToneMultiplayer, it);
            }

            #endregion
        }
        /// <summary>
        /// Gets name of tone and add it to Tones list at once.
        /// </summary>
        /// <param name="arrTone"></param>
        /// <param name="it"></param>
        /// <returns></returns>
        private string GetToneName(string arrTone, List<Tone2014> it)
        {
            string Default = "Default";            
            string ToneName = "";

            if(!String.IsNullOrEmpty(arrTone))
            {
                var matchedTone = it.SingleOrDefault(t => t.Name == arrTone);
                if (ReferenceEquals(matchedTone, null))
                    return ToneName;

                if (matchedTone.GearList.IsNull())
                    ToneName = Default;
                else
                {
                    if (!Tones.Contains(matchedTone))
                    {
                        Tones.Add(matchedTone);
                    }
                    ToneName = arrTone;
                }
            }
            return ToneName;
        }
    }
}

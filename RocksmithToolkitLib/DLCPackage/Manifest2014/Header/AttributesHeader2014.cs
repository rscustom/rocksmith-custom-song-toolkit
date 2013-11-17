using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Extensions;
using System.Reflection;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Header
{
    public class AttributesHeader2014
    {
        public static readonly string URN_TEMPLATE = "urn:{0}:{1}:{2}";
        public static readonly string URN_TEMPLATE_SHORT = "urn:{0}:{1}";


        [JsonIgnore]
        internal bool IsVocal = false;
        [JsonIgnore]
        internal Song2014 SongContent = null;

        public string AlbumArt { get; set; }
        public string AlbumName { get; set; }
        public string AlbumNameSort { get; set; }
        public string ArrangementName { get; set; }
        public string ArtistName { get; set; }
        public string ArtistNameSort { get; set; }
        public double? CentOffset { get; set; }
        public bool DLC { get; set; }
        public string DLCKey { get; set; }
        public double? DNA_Chords { get; set; }
        public double? DNA_Riffs { get; set; }
        public double? DNA_Solo { get; set; }
        public double? EasyMastery { get; set; }
        public int LeaderboardChallengeRating { get; set; }
        public string ManifestUrn { get; set; }
        public int MasterID_RDV { get; set; }
        public double? MediumMastery { get; set; }
        public double? NotesEasy { get; set; }
        public double? NotesHard { get; set; }
        public double? NotesMedium { get; set; }
        public string PersistentID { get; set; }
        public int? Representative { get; set; } // Header only
        public int? RouteMask { get; set; } // Header only
        public bool Shipping { get; set; }
        public string SKU { get; set; }
        public double? SongDiffEasy { get; set; }
        public double? SongDiffHard { get; set; }
        public double? SongDiffMed { get; set; }
        public double? SongDifficulty { get; set; }
        public string SongKey { get; set; }
        public double? SongLength { get; set; }
        public string SongName { get; set; }
        public string SongNameSort { get; set; }
        public int? SongYear { get; set; }
        public TuningStrings Tuning { get; set; }

        public AttributesHeader2014() { }

        public AttributesHeader2014(Attributes2014 attributes) {
            foreach (PropertyInfo prop in attributes.GetType().GetProperties())
                if (GetType().GetProperty(prop.Name) != null)
                    GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(attributes, null), null);
        }

        public AttributesHeader2014(Arrangement arrangement, DLCPackageData info, AggregateGraph2014 aggregateGraph, Platform platform) {
            IsVocal = arrangement.ArrangementType == Sng.ArrangementType.Vocal;
            SongContent = (IsVocal) ? null : Song2014.LoadFromFile(arrangement.SongXml.File);
            
            var albumAggName = aggregateGraph.AlbumArt[0].Name;
            var albumUrn = String.Format(URN_TEMPLATE, TagValue.Image.GetDescription(), TagValue.DDS.GetDescription(), albumAggName.Substring(0, albumAggName.LastIndexOf("_")));
            
            var jsonAggName = String.Format(AggregateGraph2014.NAME_DEFAULT, info.Name.ToLower(), arrangement.Name.ToString().ToLower());
            var jsonAggItem = aggregateGraph.JsonDB.SingleOrDefault(t => t.Name == jsonAggName);
            var jsonUrn = String.Format(URN_TEMPLATE, TagValue.Database.GetDescription(), TagValue.JsonDB.GetDescription(), jsonAggItem.Name);

            //FILL ATTRIBUTES
            this.AlbumArt = albumUrn;
            ArrangementName = arrangement.Name.ToString();
            DLC = true;
            DLCKey = info.Name;
            LeaderboardChallengeRating = 0;
            ManifestUrn = jsonUrn;
            MasterID_RDV = arrangement.MasterId;
            PersistentID = IdGenerator.IdString().ToUpper();
            Shipping = true;
            SKU = "RS2";
            SongKey = info.Name;

            if (!IsVocal)
            {
                AlbumName = AlbumNameSort = info.SongInfo.Album;
                ArtistName = info.SongInfo.Artist;
                CentOffset = 0;
                ArtistNameSort = info.SongInfo.ArtistSort;
                DNA_Chords = 0; //TODO: ???
                DNA_Riffs = 0; //TODO: ???
                DNA_Solo = 0; //TODO: ???
                EasyMastery = 0; //TODO: ???
                MediumMastery = 0; //TODO: ???
                NotesEasy = 0; //TODO: ???
                NotesHard = 0; //TODO: ???
                NotesMedium = 0; //TODO: ???
                Representative = 1;
                RouteMask = (int)arrangement.RouteMask;
                SongDiffEasy = 0; //TODO: ???
                SongDiffMed = 0; //TODO: ???
                SongDiffHard = (float)SongContent.PhraseIterations.Average(it => SongContent.Phrases[it.PhraseId].MaxDifficulty);
                SongDifficulty = SongDiffHard;
                SongLength = SongContent.SongLength;
                SongName = info.SongInfo.SongDisplayName;
                SongNameSort = info.SongInfo.SongDisplayNameSort;
                SongYear = info.SongInfo.SongYear;

                var tunDef = TuningDefinitionRepository.Instance().Select(arrangement.Tuning, platform.version);
                Tuning = tunDef.Tuning;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class Attributes : IAttributes
    {
        public string AlbumArt { get; set; }
        public string AlbumName { get; set; }
        public string AlbumNameSort { get; set; }
        public string ArrangementName { get; set; }
        public string ArtistName { get; set; }
        public string ArtistNameSort { get; set; }
        public List<string> AssociatedTechniques { get; set; }
        public bool BarChords { get; set; }
        public bool Bends { get; set; }
        public int BinaryVersion { get; set; }
        public string BlockAsset { get; set; }
        public List<ChordTemplate> ChordTemplates { get; set; }
        public string DISC_DLC_OTHER { get; set; }
        public string DisplayName { get; set; }
        public bool DLCPreview { get; set; }
        public bool DoubleStops { get; set; }
        public bool DropDPowerChords { get; set; }
        public List<float> DynamicVisualDensity { get; set; }
        public string EffectChainMultiplayerName { get; set; }
        public string EffectChainName { get; set; }
        public int EventFirstTimeSortOrder { get; set; }
        public List<object> ExclusiveBuild { get; set; }
        public bool FifthsAndOctaves { get; set; }
        public bool ForceUseXML { get; set; }
        public bool FretHandMutes { get; set; }
        public string Genre { get; set; }
        public bool Harmonics { get; set; }
        public bool HOPOs { get; set; }
        public string InputEvent { get; set; }
        public bool IsDemoSong { get; set; }
        public bool IsDLC { get; set; }
        public string LastConversionDateTime { get; set; }
        public int MasterID_PS3 { get; set; }
        public int MasterID_Xbox360 { get; set; }
        public int MaxPhraseDifficulty { get; set; }
        public bool OpenChords { get; set; }
        public bool PalmMutes { get; set; }
        public string PersistentID { get; set; }
        public List<PhraseIteration> PhraseIterations { get; set; }
        public List<Phrase> Phrases { get; set; }
        public string PluckedType { get; set; }
        public bool PowerChords { get; set; }
        public bool Prebends { get; set; }
        public int RelativeDifficulty { get; set; }
        public bool RepresentativeArrangement { get; set; }
        public float Score_MaxNotes { get; set; }
        public float Score_PNV { get; set; }
        public List<Section> Sections { get; set; }
        public bool Shipping { get; set; }
        public bool SlapAndPop { get; set; }
        public bool Slides { get; set; }
        public string SongAsset { get; set; }
        public string SongEvent { get; set; }
        public string SongKey { get; set; } // where is DLCKey?
        public float SongLength { get; set; }
        public string SongName { get; set; }
        public string SongNameSort { get; set; }
        public int SongPartition { get; set; }
        public string SongXml { get; set; }
        public int SongYear { get; set; }
        public bool Sustain { get; set; }
        public bool Syncopation { get; set; }
        public int TargetScore { get; set; }
        public int ToneUnlockScore { get; set; }
        public bool Tremolo { get; set; }
        public string Tuning { get; set; }
        public bool TwoFingerPlucking { get; set; }
        public bool TwoHandTapping { get; set; }
        public string UnlockKey { get; set; }
        public bool Vibrato { get; set; }
        public string VocalsAssetId { get; set; }
        public bool FirstArrangementInSong { get; set; }
        public float SongDifficulty { get; set; }
        [JsonIgnore]
        public int AverageTempo { get; set; }
        [JsonIgnore]
        public string CrowdTempo { get; set; }
    }
}

using System;
using System.Reflection;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.AggregateGraph2014;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitLib.DLCPackage.Manifest2014.Header
{
    [Serializable]
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
        public decimal CapoFret { get; set; }
        public double? CentOffset { get; set; }
        public bool DLC { get; set; }
        public string DLCKey { get; set; } // <=> SongKey
        public double? DNA_Chords { get; set; }
        public double? DNA_Riffs { get; set; }
        public double? DNA_Solo { get; set; }
        public double? EasyMastery { get; set; }
        public bool? JapaneseVocal { get; set; }
        public int LeaderboardChallengeRating { get; set; }
        public string ManifestUrn { get; set; }
        public int MasterID_RDV { get; set; }
        public int? Metronome { get; set; }
        public double? MediumMastery { get; set; }
        public double? NotesEasy { get; set; }
        public double? NotesHard { get; set; }
        public double? NotesMedium { get; set; }
        public int? Representative { get; set; } // Header only
        public int? RouteMask { get; set; } // Header only
        public bool Shipping { get; set; }
        public string SKU { get; set; }
        public double? SongDiffEasy { get; set; }
        public double? SongDiffHard { get; set; }
        public double? SongDiffMed { get; set; }
        public double? SongDifficulty { get; set; }
        public string SongKey { get; set; } // <=> DLCKey
        public double? SongLength { get; set; }
        public string SongName { get; set; }
        public string SongNameSort { get; set; }
        public int? SongYear { get; set; }
        public TuningStrings Tuning { get; set; }
        public string PersistentID { get; set; }

        public AttributesHeader2014() { }

        public AttributesHeader2014(Attributes2014 attributes)
        {
            foreach (PropertyInfo prop in attributes.GetType().GetProperties())
                if (GetType().GetProperty(prop.Name) != null)
                    GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(attributes, null), null);
        }

        public AttributesHeader2014(string arrangementFileName, Arrangement arrangement, DLCPackageData info, Platform platform)
        {
            if (arrangement.ArrangementType == ArrangementType.ShowLight)
                return;

            IsVocal = arrangement.ArrangementType == ArrangementType.Vocal;
            SongContent = (IsVocal) ? null : Song2014.LoadFromFile(arrangement.SongXml.File);
            var dlcName = info.Name.ToLower();

            var albumUrn = String.Format(URN_TEMPLATE, TagValue.Image.GetDescription(), TagValue.DDS.GetDescription(), String.Format("album_{0}", dlcName));
            var jsonUrn = String.Format(URN_TEMPLATE, TagValue.Database.GetDescription(), TagValue.JsonDB.GetDescription(), String.Format("{0}_{1}", dlcName, arrangementFileName));

            //FILL ATTRIBUTES
            this.AlbumArt = albumUrn;
            JapaneseVocal |= arrangement.Name == Sng.ArrangementName.JVocals;
            ArrangementName = IsVocal ? Sng.ArrangementName.Vocals.ToString() : arrangement.Name.ToString(); //HACK: weird vocals stuff
            DLC = true;
            DLCKey = info.Name;
            LeaderboardChallengeRating = 0;
            ManifestUrn = jsonUrn;
            MasterID_RDV = arrangement.MasterId;
            PersistentID = arrangement.Id.ToString().Replace("-", "").ToUpper();
            Shipping = true;
            SKU = "RS2";
            SongKey = info.Name; // proof same same

            if (IsVocal) return;
            // added better AlbumNameSort feature
            AlbumName = info.SongInfo.Album;
            AlbumNameSort = info.SongInfo.AlbumSort;
            ArtistName = info.SongInfo.Artist;
            CentOffset = (!arrangement.TuningPitch.Equals(0)) ? TuningFrequency.Frequency2Cents(arrangement.TuningPitch) : 0.0;
            ArtistNameSort = info.SongInfo.ArtistSort;
            CapoFret = (arrangement.Sng2014.Metadata.CapoFretId == 0xFF) ? CapoFret = 0 : Convert.ToDecimal(arrangement.Sng2014.Metadata.CapoFretId);
            DNA_Chords = arrangement.Sng2014.DNACount[(int)DNAId.Chord];
            DNA_Riffs = arrangement.Sng2014.DNACount[(int)DNAId.Riff];
            DNA_Solo = arrangement.Sng2014.DNACount[(int)DNAId.Solo];
            NotesEasy = arrangement.Sng2014.NoteCount[0];
            NotesMedium = arrangement.Sng2014.NoteCount[1];
            NotesHard = arrangement.Sng2014.NoteCount[2];
            EasyMastery = NotesEasy / NotesHard;
            MediumMastery = NotesMedium / NotesHard;
            Metronome = (int?)arrangement.Metronome;
            Representative = Convert.ToInt32(!arrangement.BonusArr);
            RouteMask = (int)arrangement.RouteMask;

            // TODO: use ManifestFunctions.GetSongDifficulty() method (fix generation algorithm)
            // TODO: round to 9 decimal places and improve calculation
            // TODO: fix SongDiff calculations
            SongDiffEasy = SongContent.SongLength / NotesEasy;
            SongDiffMed = SongContent.SongLength / NotesMedium;
            SongDiffHard = SongContent.SongLength / NotesHard;
            SongDifficulty = SongDiffHard;

            SongLength = Math.Round(SongContent.SongLength, 3, MidpointRounding.AwayFromZero);
            SongName = info.SongInfo.SongDisplayName;
            SongNameSort = info.SongInfo.SongDisplayNameSort;
            SongYear = info.SongInfo.SongYear;

            //Detect tuning
            var tuning = TuningDefinitionRepository.Instance.Detect(SongContent.Tuning, platform.version, arrangement.ArrangementType == ArrangementType.Bass);
            Tuning = tuning.Tuning; //can we just use SongContent.Tuning
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Header
{
    public class AttributesHeaderRS2014
    {
        public string AlbumArt { get; set; }
        public string AlbumName { get; set; }
        public string AlbumNameSort { get; set; }
        public string ArrangementName { get; set; }
        public string ArtistName { get; set; }
        public string ArtistNameSort { get; set; }
        public float CentOffset { get; set; }
        public bool DLC { get; set; }
        public string DLCKey { get; set; }
        public float DNA_Chords { get; set; }
        public float DNA_Riffs { get; set; }
        public float DNA_Solo { get; set; }
        public float EasyMastery { get; set; }
        public int LeaderboardChallengeRating { get; set; }
        public string ManifestUrn { get; set; }
        public int MasterID_RDV { get; set; }
        public float MediumMastery { get; set; }
        public float NotesEasy { get; set; }
        public float NotesHard { get; set; }
        public float NotesMedium { get; set; }
        public string PersistentID { get; set; }
        public int Representative { get; set; } // Header only
        public int RouteMask { get; set; } // Header only
        public bool Shipping { get; set; }
        public string SKU { get; set; }
        public float SongDiffEasy { get; set; }
        public float SongDiffHard { get; set; }
        public float SongDifficulty { get; set; }
        public float SongDiffMed { get; set; }
        public string SongKey { get; set; }
        public float SongLength { get; set; }
        public string SongName { get; set; }
        public string SongNameSort { get; set; }
        public int SongYear { get; set; }
        public TuningStrings Tuning { get; set; }
    }
}

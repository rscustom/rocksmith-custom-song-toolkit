using System;

namespace RocksmithToolkitLib.DLCPackage
{
    public class SongInfo
    {
        public string Album { get; set; }
        public string AlbumSort { get; set; } // new
        public string Artist { get; set; }
        public string ArtistSort { get; set; }
        public int AverageTempo { get; set; }
        public string SongDisplayName { get; set; }
        public string SongDisplayNameSort { get; set; }
        public int SongYear { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public class DLCPackageData
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public SongInfo SongInfo { get; set; }
        public string AlbumArtPath { get; set; }
        public string OggPath { get; set; }
        public string OggXBox360Path { get; set; }
        public IList<Arrangement> Arrangements { get; set; }
        public IList<Tone.Tone> Tones { get; set; }
        public decimal Volume { get; set; }
    }
}

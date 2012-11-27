using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public class SongInfo
    {
        public string Album { get; set; }
        public string Artist { get; set; }
        public int AverageTempo { get; set; }
        public int SongDifficulty { get; set; }
        public string SongDisplayName { get; set; }
        public int SongYear { get; set; }
    }
}

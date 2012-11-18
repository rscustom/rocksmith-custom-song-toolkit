using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public class DLCPackageData
    {
        public string Name { get; set; }
        public string Album { get; set; }
        public string AlbumArtPath { get; set; }
        public string OggPath { get; set; }
        public IList<Arrangement> Arrangements { get; set; }
        public Tone.Tone Tone { get; set; }
    }
}

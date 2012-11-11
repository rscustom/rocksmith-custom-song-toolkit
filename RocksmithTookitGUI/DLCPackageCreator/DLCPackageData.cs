using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithDLCCreator;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public class DLCPackageData
    {
        public string Name { get; set; }
        public string Album { get; set; }
        public string AlbumArtPath { get; set; }
        public string OggPath { get; set; }
        public IList<Arrangement> Arrangements { get; set; }
    }
}

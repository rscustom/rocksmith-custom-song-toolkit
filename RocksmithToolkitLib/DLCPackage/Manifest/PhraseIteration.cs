using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class PhraseIteration
    {
        public int PhraseIndex { get; set; }
        public int MaxDifficulty { get; set; }
        public string Name { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public List<float> MaxScorePerDifficulty { get; set; }
    }
}

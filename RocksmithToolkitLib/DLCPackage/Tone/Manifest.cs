using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Tone
{
    public class Manifest
    {
        public List<Tone> Entries { get; set; }
        public string ModelName { get; set; }
        public int IterationVersion { get; set; }
        public Manifest()
        {
            Entries = new List<Tone>();
            ModelName = "GRTonePreset";
            IterationVersion = 2;
        }
    }
}

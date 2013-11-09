using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class PedalRS2014
    {
        public string Type { get; set; }
        public Dictionary<string, decimal> KnobValues { get; set; }
        public string Key { get; set; }
        public string Category { get; set; }

        public PedalRS2014()
        {
            KnobValues = new Dictionary<string, decimal>();
        }
    }
}

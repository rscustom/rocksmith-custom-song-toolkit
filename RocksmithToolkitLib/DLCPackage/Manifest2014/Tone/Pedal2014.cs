using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class Pedal2014
    {
        public string Type { get; set; }
        public Dictionary<string, decimal> KnobValues { get; set; }
        [JsonProperty("Key")]
        public string PedalKey { get; set; }
        public string Category { get; set; }

        public Pedal2014()
        {
            KnobValues = new Dictionary<string, decimal>();
        }

        public bool Any() {
            return KnobValues.Any();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest2014.Tone
{
    public class Pedal2014
    {
        public string Type { get; set; }
        public Dictionary<string, float> KnobValues { get; set; }
        [JsonProperty("Key")]
        public string PedalKey { get; set; }
        public string Category { get; set; }
        public string Skin { get; set; }
        public float? SkinIndex { get; set; }

        public Pedal2014()
        {
            KnobValues = new Dictionary<string, float>();
        }

        public bool Any() {
            return KnobValues.Any();
        }
    }
}

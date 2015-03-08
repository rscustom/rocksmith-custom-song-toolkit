using System.Collections.Generic;
using System.Linq;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class Pedal
    {
        public string PedalKey { get; set; }
        public Dictionary<string, decimal> KnobValues { get; set; }

        public Pedal()
        {
            KnobValues = new Dictionary<string, decimal>();
        }

        public bool Any() {
            return KnobValues.Any();
        }
    }
}

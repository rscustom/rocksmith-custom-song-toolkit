using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.ManifestRS2014.Tone
{
    public class GearRS2014
    {
        public PedalRS2014 Rack1 { get; set; }
        public PedalRS2014 Rack2 { get; set; }
        public PedalRS2014 Rack3 { get; set; }
        public PedalRS2014 Rack4 { get; set; }

        public PedalRS2014 Amp { get; set; }
        public PedalRS2014 Cabinet { get; set; }

        public PedalRS2014 PrePedal1 { get; set; }
        public PedalRS2014 PrePedal2 { get; set; }
        public PedalRS2014 PrePedal3 { get; set; }
        public PedalRS2014 PrePedal4 { get; set; }

        public PedalRS2014 PostPedal1 { get; set; }
        public PedalRS2014 PostPedal2 { get; set; }
        public PedalRS2014 PostPedal3 { get; set; }
        public PedalRS2014 PostPedal4 { get; set; }
    }
}

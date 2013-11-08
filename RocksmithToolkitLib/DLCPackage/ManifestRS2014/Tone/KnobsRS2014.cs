using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class KnobRS2014
    {
        public string RTPC { get; set; }
        public string Base { get; set; }
        public int Type { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public string Name { get; set; }
        public string UnitType { get; set; }
        public float DefaultValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float ValueStep { get; set; }
        public string Ring { get; set; }
        public int Index { get; set; }
    }
}

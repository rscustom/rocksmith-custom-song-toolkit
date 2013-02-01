using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.Tone
{
    public class Knob
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string UnitType { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal ValueStep { get; set; }
        public decimal DefaultValue { get; set; }
        public IList<Tuple<string, string>> EnumValues { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.ToolkitTone
{
    public class ToolkitKnob
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string UnitType { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float ValueStep { get; set; }
        public float DefaultValue { get; set; }
        public int Index { get; set; }
        public IList<Tuple<string, string>> EnumValues { get; set; }

        public ToolkitKnob() {
            EnumValues = new List<Tuple<string, string>>();
        }
    }
}

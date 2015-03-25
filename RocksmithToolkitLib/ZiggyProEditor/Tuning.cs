using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
  //<tunings>
  //  <tuning isGuitarTuning="true" E="0" A="0" D="0" G="0" B="0" HighE="0" />
  //  <tuning isBassTuning="true" E="0" A="0" D="0" G="0" B="0" HighE="0" />
  //</tunings>

    [XmlType("tunings")]
    public class ZpeTunings
    {
        [XmlElement("tunings", IsNullable = false)]
        public List<ZpeTuning> Tuning { get; set; }
    }

    [XmlType("tuning")]
    public class ZpeTuning
    {
        [XmlAttribute("isGuitarTuning")]
        [DefaultValue(false)] // ignores if false
        public bool IsGuitarTuning { get; set; }

        [XmlAttribute("isBassTuning")]
        [DefaultValue(false)] // ignores if false
        public bool IsBassTuning { get; set; }

        [XmlAttribute("E")]
        public Int16 E { get; set; }

        [XmlAttribute("A")]
        public Int16 A { get; set; }

        [XmlAttribute("D")]
        public Int16 D { get; set; }

        [XmlAttribute("G")]
        public Int16 G { get; set; }

        [XmlAttribute("B")]
        public Int16 B { get; set; }

        [XmlAttribute("HighE")]
        public Int16 HighE { get; set; }
    }

}

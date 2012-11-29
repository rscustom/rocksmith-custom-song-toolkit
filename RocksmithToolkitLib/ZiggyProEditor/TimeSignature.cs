using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    [XmlRoot("timeSignature")]
    public class TimeSignature
    {
        [XmlAttribute("startTime")]
        public float StartTime {get;set;}

        [XmlAttribute("startTick")]
        public int StartTick {get;set;}

        [XmlAttribute("clocksPerMetronomeClick")]
        public float ClocksPerMetronomeClick { get; set; }

        [XmlAttribute("numerator")]
        public int Numerator { get; set; }

        [XmlAttribute("denominator")]
        public int Denominator { get; set; }

        [XmlAttribute("thirtySecondNotesPerQuarterNote")]
        public int ThirtySecondNotesPerQuarterNote { get; set; }

    }
}

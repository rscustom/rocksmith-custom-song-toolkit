using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    [XmlRoot("tempo")]
    public class Tempo
    {
        [XmlAttribute("startTime")]
        public float StartTime {get;set;}

        [XmlAttribute("startTick")]
        public int StartTick {get;set;}

        [XmlAttribute("rawTempo")]
        public int RawTempo { get; set; }

        [XmlAttribute("secondsPerQuarterNote")]
        public float SecondsPerQuarterNote {get;set;}

        [XmlAttribute("secondsPerTick")]
        public float SecondsPerTick {get;set;}

        [XmlAttribute("secondsPerBar")]
        public float SecondsPerBar {get;set;}
    }
}

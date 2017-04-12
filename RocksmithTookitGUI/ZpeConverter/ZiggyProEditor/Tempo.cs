using System.Xml.Serialization;

namespace RocksmithToolkitGUI.ZpeConverter.ZiggyProEditor
{
    //<tempo startTime="0" startTick="0" rawTempo="591133" secondsPerWholeNote="5.91133" secondsPerTick="0.00123152708333333" />
    
    [XmlRoot("tempo")]
    public class ZpeTempo
    {
        [XmlAttribute("startTime")]
        public float StartTime { get; set; }

        [XmlAttribute("startTick")]
        public long StartTick { get; set; }

        // rawTempo is in units of micro Seconds Per Quater Note
        [XmlAttribute("rawTempo")]
        public long RawTempo { get; set; }

        // rawTempo / 100000
        [XmlAttribute("secondsPerWholeNote")]
        public float SecondsPerWholeNote { get; set; }

        [XmlAttribute("secondsPerTick")]
        public float SecondsPerTick { get; set; }

        // depricated
        //[XmlAttribute("secondsPerBar")]
        //public float SecondsPerBar { get; set; }

        //[XmlAttribute("secondsPerQuarterNote")]
        //public float SecondsPerQuarterNote { get; set; }


    }
}

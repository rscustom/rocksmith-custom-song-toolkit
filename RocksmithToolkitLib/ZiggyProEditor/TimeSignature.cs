using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    // <timeSignature startTime="0" startTick="0" numerator="4" denominator="4" />
    
    [XmlRoot("timeSignature")]
    public class ZpeTimeSignature
    {
        [XmlAttribute("startTime")]
        public float StartTime {get;set;}

        [XmlAttribute("startTick")]
        public long StartTick {get;set;}
 
        [XmlAttribute("numerator")]
        public int Numerator { get; set; }

        [XmlAttribute("denominator")]
        public int Denominator { get; set; }

        // depricated
        //[XmlAttribute("clocksPerMetronomeClick")]
        //public float ClocksPerMetronomeClick { get; set; }

        //[XmlAttribute("thirtySecondNotesPerQuarterNote")]
        //public int ThirtySecondNotesPerQuarterNote { get; set; }

    }
}

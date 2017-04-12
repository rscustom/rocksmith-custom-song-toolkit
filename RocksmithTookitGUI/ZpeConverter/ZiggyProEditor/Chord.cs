using System.Collections.Generic;
using System.Xml.Serialization;

namespace RocksmithToolkitGUI.ZpeConverter.ZiggyProEditor
{
    // <chord difficulty="Easy" startTime="143.054186" endTime="143.128077625" startTick="116160" endTick="116220">

    [XmlRoot("chord")]
    public class ZpeChord
    {
        [XmlAttribute("difficulty")]
        public string Difficulty { get; set; }

        [XmlAttribute("startTime")]
        public float StartTime { get; set; }

        [XmlAttribute("endTime")]
        public float EndTime { get; set; }

        [XmlAttribute("startTick")]
        public long StartTick { get; set; }

        [XmlAttribute("endTick")]
        public long EndTick { get; set; }

        [XmlArray("notes")]
        [XmlArrayItem("note")]
        public List<ZpeNote> Notes { get; set; }

        // depricated in Ziggy Pro but still used by converter
        [XmlAttribute("isMute")]
        public bool IsMute { get; set; }

        [XmlAttribute("isTap")]
        public bool IsTap { get; set; }

        [XmlAttribute("isSlide")]
        public bool IsSlide { get; set; }

        [XmlAttribute("isSlideReversed")]
        public bool IsSlideReversed { get; set; }

        [XmlAttribute("isHammeron")]
        public bool IsHammerOn { get; set; }

        [XmlAttribute("strumHigh")]
        public bool StrumHigh { get; set; }

        [XmlAttribute("strumMid")]
        public bool StrumMid { get; set; }

        [XmlAttribute("strumLow")]
        public bool StrumLow { get; set; }

        //[XmlAttribute("frets")]
        //public string Frets { get; set; }

        //[XmlAttribute("channels")]
        //public string Channels { get; set; }
    }
}

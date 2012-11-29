using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    [XmlRoot("chord")]
    public class Chord
    {
        [XmlArray("notes")]
        [XmlArrayItem("note")]
        public List<Note> Notes { get; set; }

        [XmlAttribute("difficulty")]
        public string Difficulty { get; set; }
     
        [XmlAttribute("startTime")]
        public float StartTime {get;set;}

        [XmlAttribute("startTick")]
        public int StartTick {get;set;}

        [XmlAttribute("endTime")]
        public float EndTime { get; set; }

        [XmlAttribute("endTick")]
        public int EndTick { get; set; }

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

        [XmlAttribute("frets")]
        public string Frets { get; set; }

        [XmlAttribute("channels")]
        public string Channels { get; set; }
    }
}

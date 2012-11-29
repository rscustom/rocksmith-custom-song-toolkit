using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    [XmlRoot("chord")]
    public class Note
    {
        [XmlAttribute("fret")]
        public int Fret {get;set;}
     
        [XmlAttribute("string")]
        public int StringNo { get; set; }

        [XmlAttribute("isXNote")]
        public bool IsXNote { get; set; }

        [XmlAttribute("isTapNote")]
        public bool IsTapNote { get; set; }

        [XmlAttribute("isArpeggioNote")]
        public bool IsArpeggioNote { get; set; }

        [XmlAttribute("channel")]
        public int Channel { get; set; }
    }
}

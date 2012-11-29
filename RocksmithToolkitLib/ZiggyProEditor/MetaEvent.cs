using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    [XmlRoot("metaEvent")]
    public class MetaEvent
    {
        [XmlAttribute("startTime")]
        public float StartTime {get;set;}

        [XmlAttribute("startTick")]
        public int StartTick {get;set;}

        [XmlAttribute("metaType")]
        public string MetaType { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }
    }
}

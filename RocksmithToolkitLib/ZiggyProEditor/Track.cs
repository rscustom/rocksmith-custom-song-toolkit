using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    [XmlRoot("track")]
    public class Track
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("tempos")]
        [XmlArrayItem("tempo")]
        public List<Tempo> Tempos { get; set; }

        [XmlArray("timeSignatures")]
        [XmlArrayItem("timeSignature")]
        public List<TimeSignature> TimeSignatures { get; set; }

        [XmlArray("metaEvents")]
        [XmlArrayItem("metaEvent")]
        public List<MetaEvent> MetaEvents { get; set; }

        [XmlArray("chords")]
        [XmlArrayItem("chord")]
        public List<Chord> Chords { get; set; }
    }
}

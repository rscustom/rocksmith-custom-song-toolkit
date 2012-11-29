using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    [XmlRoot("song")]
    public class Song
    {
        [XmlArray("tracks")]
        [XmlArrayItem("track")]
        public List<Track> Tracks { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("length")]
        public float Length { get; set; }
    }
}

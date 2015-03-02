using System.Collections.Generic;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    // <song songid="" pue_version="46" name="Akira Yamaoka - Silent Hill 2 Theme of Laura_RB3_v4" description="Akira Yamaoka - Silent Hill 2 Theme of Laura_RB3_v4" shortname="" length="203.940885">

    [XmlRoot("song", Namespace = "", IsNullable = false)]
    public class ZpeSong
    {
        [XmlAttribute("songid")]
        public string Songid { get; set; }

        [XmlAttribute("pue_version")]
        public int PueVersion { get; set; } // 46 = CST compatible version

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("shortname")]
        public string Shortname { get; set; }

        [XmlAttribute("length")]
        public float Length { get; set; }

        [XmlElement("tunings")]
        public ZpeTunings Tunings { get; set; }

        [XmlArray("tracks")]
        [XmlArrayItem("track")]
        public List<ZpeTrack> Tracks { get; set; }








    }
}

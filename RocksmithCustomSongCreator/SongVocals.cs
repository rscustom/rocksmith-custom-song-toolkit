using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithSngCreator
{
    [XmlRoot("vocals", Namespace = "", IsNullable = false)]
    public class SongVocals
    {
        [XmlAttribute("count")]
        public Int32 Count;

        [XmlElement("vocal")]
        public SongVocal[] Vocal { get; set; }
    }

    [XmlType("vocal")]
    public class SongVocal
    {
        [XmlAttribute("time")]
        public float Time;

        [XmlAttribute("note")]
        public Int32 Note;

        [XmlAttribute("length")]
        public float Length;

        [XmlAttribute("lyric")]
        public string Lyric; // len 32
    }
}

using System.Xml.Serialization;

namespace RocksmithToolkitGUI.ZpeConverter.ZiggyProEditor
{
    // <note fret="0" string="2" />

    [XmlRoot("chord")]
    public class ZpeNote
    {
        [XmlAttribute("fret")]
        public int Fret { get; set; }

        [XmlAttribute("string")]
        public int StringNo { get; set; }

        // may be depricated
        [XmlAttribute("channel")]
        public int Channel { get; set; }

        // depricated in Ziggy Pro but still used in conversion
        [XmlAttribute("isXNote")]
        public bool IsXNote { get; set; }

        [XmlAttribute("isTapNote")]
        public bool IsTapNote { get; set; }

        [XmlAttribute("isArpeggioNote")]
        public bool IsArpeggioNote { get; set; }

    }
}

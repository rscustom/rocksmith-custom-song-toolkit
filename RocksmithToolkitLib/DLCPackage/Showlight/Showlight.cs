using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.Showlight
{
    [XmlType("showlight")]
    public class Showlight
    {
        [XmlAttribute("time")]
        public float Time { get; set; }

        [XmlAttribute("note")]
        public int Note { get; set; }
    }
}
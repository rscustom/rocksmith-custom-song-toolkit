using System.Xml.Serialization;

namespace RocksmithToolkitLib.XmlRepository
{
    public class Config
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }
}

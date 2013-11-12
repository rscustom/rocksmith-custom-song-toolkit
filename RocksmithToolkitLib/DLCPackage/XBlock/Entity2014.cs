using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    [XmlType("entity")]
    public class Entity2014 : IEntity
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("modelName")]
        public string ModelName { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("iterations")]
        public int Iterations { get; set; }
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<Property2014> Properties { get; set; }

        public void Serialize(XElement element) {
            throw new NotImplementedException();
        }
    }
}
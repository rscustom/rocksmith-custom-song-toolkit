using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlockRS2014
{
    [XmlType("entity")]
    public class EntityRS2014
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
        public List<PropertyRS2014> Properties { get; set; }
    }
}
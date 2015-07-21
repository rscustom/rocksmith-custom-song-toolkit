using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    [XmlType("entity")]
    public class Entity : IEntity
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("modelName")]
        public string ModelName { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("iterations")]
        public int Iterations { get; set; }
        [XmlElement("property")]
        public List<Property> Properties { get; set; }

        public void Serialize(XElement element)
        {
            var el = new XElement("entity", new XAttribute("id", Id),
                new XAttribute("modelName", ModelName),
                new XAttribute("name", Name),
                new XAttribute("iterations", Iterations));
            foreach (var x in Properties)
                x.Serialize(el);
            element.Add(el);
        }

    }
}

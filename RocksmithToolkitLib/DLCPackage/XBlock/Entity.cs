using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace RocksmithDLCCreator.XBlock
{
    public class Entity
    {
        public string Id { get; set; }
        public string ModelName { get; set; }
        public string Name { get; set; }
        public int Iterations { get; set; }
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

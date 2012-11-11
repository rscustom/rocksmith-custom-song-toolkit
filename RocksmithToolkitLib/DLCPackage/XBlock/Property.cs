using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace RocksmithDLCCreator.XBlock
{
    public class Property
    {
        public string Name { get; set; }
        public ISet Set { get; set; }
        public void Serialize(XElement element)
        {
            var el = new XElement("property", new XAttribute("name", Name));
            Set.Serialize(el);
            element.Add(el);
        }
    }
}

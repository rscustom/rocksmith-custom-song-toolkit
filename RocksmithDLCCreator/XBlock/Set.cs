using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace RocksmithDLCCreator.XBlock
{
    public class Set : ISet
    {
        public string Value { get; set; }
        public void Serialize(XElement element)
        {
            element.Add(new XElement("set", new XAttribute("value", Value)));
        }
    }
}

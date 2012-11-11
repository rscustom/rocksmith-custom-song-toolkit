using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace RocksmithDLCCreator.XBlock
{
    public class MultiItemSet : ISet
    {
        public List<string> Values { get; set; }

        public void Serialize(System.Xml.Linq.XElement element)
        {
            for (int i = 0; i < Values.Count; i++)
                element.Add(new XElement("set", new XAttribute("index", i), new XAttribute("value", Values[i])));
        }
    }
}

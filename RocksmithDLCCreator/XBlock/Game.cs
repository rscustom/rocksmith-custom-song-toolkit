using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;

namespace RocksmithDLCCreator.XBlock
{
    public class Game
    {
        public List<Entity> Entities { get; set; }
        public void Serialize(Stream outStream)
        {
            var document = new XDocument();
            var el = new XElement("entitySet");
            foreach (var x in Entities)
                x.Serialize(el);
            document.Add(new XElement("game", el));
            document.Save(outStream);
        }
    }
}

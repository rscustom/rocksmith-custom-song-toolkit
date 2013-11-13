using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.Showlight
{
    [XmlRoot("showlights", Namespace = "", IsNullable = false)]
    public class Showlights
    {
        [XmlElement("showlight")]
        public Showlight[] ShowlightList { get; set; }

        public void Serialize(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Showlights));
            serializer.Serialize(stream, this);
        }

        public static Showlights LoadFromFile(string showlightsRS2014File)
        {
            Showlights xmlShowlightsRS2014 = null;

            using (var reader = new StreamReader(showlightsRS2014File))
            {
                var serializer = new XmlSerializer(typeof(Showlights));
                xmlShowlightsRS2014 = (Showlights)serializer.Deserialize(reader);
            }

            return xmlShowlightsRS2014;
        }
    }
}

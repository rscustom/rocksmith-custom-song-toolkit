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
    public class Showlights2014
    {
        [XmlElement("showlight")]
        public Showlight2014[] Showlights { get; set; }

        public static Showlights2014 LoadFromFile(string showlightsRS2014File)
        {
            Showlights2014 xmlShowlightsRS2014 = null;

            using (var reader = new StreamReader(showlightsRS2014File))
            {
                var serializer = new XmlSerializer(typeof(Showlights2014));
                xmlShowlightsRS2014 = (Showlights2014)serializer.Deserialize(reader);
            }

            return xmlShowlightsRS2014;
        }
    }
}

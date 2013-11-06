using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.ShowlightRS2014
{
    [XmlRoot("showlights", Namespace = "", IsNullable = false)]
    public class ShowlightsRS2014
    {
        [XmlElement("showlight")]
        public ShowlightRS2014[] Showlights { get; set; }

        public static ShowlightsRS2014 LoadFromFile(string showlightsRS2014File)
        {
            ShowlightsRS2014 xmlShowlightsRS2014 = null;

            using (var reader = new StreamReader(showlightsRS2014File))
            {
                var serializer = new XmlSerializer(typeof(ShowlightsRS2014));
                xmlShowlightsRS2014 = (ShowlightsRS2014)serializer.Deserialize(reader);
            }

            return xmlShowlightsRS2014;
        }
    }
}

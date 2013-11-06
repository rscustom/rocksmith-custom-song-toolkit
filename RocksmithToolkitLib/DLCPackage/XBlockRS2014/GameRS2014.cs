using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlockRS2014
{
    [XmlRoot("game", Namespace = "", IsNullable = false)]
    public class GameRS2014
    {
        [XmlArray("entitySet")]
        [XmlArrayItem("entity")]
        public List<EntityRS2014> EntitySet { get; set; }

        public static GameRS2014 LoadFromFile(string xblockRS2014File) {
            GameRS2014 xmlXblockRS2014 = null;

            using (var reader = new StreamReader(xblockRS2014File)) {
                var serializer = new XmlSerializer(typeof(GameRS2014));
                xmlXblockRS2014 = (GameRS2014)serializer.Deserialize(reader);
            }

            return xmlXblockRS2014;
        }
    }
}

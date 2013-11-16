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
        public List<Showlight> ShowlightList { get; set; }

        public Showlights() { }

        public Showlights(List<Arrangement> arrangements) {
            ShowlightList = new List<Showlight>();
            List<Showlight> listOne = new List<Showlight>();
            foreach (var arrangement in arrangements) {
                if (arrangement.ArrangementType == Sng.ArrangementType.Vocal)
                    continue;

                var showlightFile = Path.Combine(Path.GetDirectoryName(arrangement.SongXml.File), Path.GetFileNameWithoutExtension(arrangement.SongXml.File) + "_showlights.xml");
                if (!File.Exists(showlightFile))
                    continue;

                listOne = Showlights.LoadFromFile(showlightFile).ShowlightList;

                if (ShowlightList.Count == 0)
                    ShowlightList = listOne;
                else
                {
                    var slIntersected = from first in listOne
                                        join second in ShowlightList
                                        on first.Note equals second.Note
                                        where (first.Time - 100) < second.Time
                                        && (first.Time + 100) > second.Time
                                        select first;
                    ShowlightList = slIntersected.ToList();
                }
            }
        }

        public void Serialize(Stream stream)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(typeof(Showlights));
            serializer.Serialize(stream, this, ns);
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

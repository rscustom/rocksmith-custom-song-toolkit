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

        public Showlights(DLCPackageData info) {
            ShowlightList = new List<Showlight>();
            foreach (var arrangement in info.Arrangements) {
                var showlightFile = Path.Combine(Path.GetDirectoryName(arrangement.SongXml.File), 
                    Path.GetFileNameWithoutExtension(arrangement.SongXml.File) + "_showlights.xml");
                if (!File.Exists(showlightFile))
                    showlightFile = Path.Combine(Path.GetDirectoryName(arrangement.SongXml.File),
                	    info.Name  + "_showlights.xml");
                if (!File.Exists(showlightFile))
                    continue;

                var listOne = new List<Showlight>();
                listOne = Showlights.LoadFromFile(showlightFile).ShowlightList;

                if (ShowlightList.Count == 0)
                    ShowlightList.AddRange(listOne);
                else
                {
                    try
                    {
                        var comp = new EqShowlight();
                        ShowlightList.Except(listOne, comp);
                    }
                    catch { continue; }
                }
            }
        }
        class EqShowlight : IEqualityComparer<Showlight>
        {
            public bool Equals(Showlight x, Showlight y)
            { return x.Note == y.Note && (x.Time - 100 < y.Time) && (x.Time + 100 > y.Time); }

            public int GetHashCode(Showlight obj)
            {   if (Object.ReferenceEquals(obj, null)) return 0;
                return obj.Time.GetHashCode() ^ obj.Time.GetHashCode(); }
        }
        public void Serialize(Stream stream)
        {

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var buf = new BufferedStream(stream, 131072);
            var serializer = new XmlSerializer(typeof(Showlights));
            serializer.Serialize(buf, this, ns);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);            
        }

        public static Showlights LoadFromFile(string showlightsRS2014File)
        {
            Showlights xmlShowlightsRS2014 = null;
            using (var reader = new StreamReader(showlightsRS2014File))
            {
                var serializer = new Extensions.XmlStreamingDeserializer<Showlights>(reader);
                xmlShowlightsRS2014 = (Showlights)serializer.Deserialize();            
            }
            return xmlShowlightsRS2014;
        }
    }
}

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

        [XmlAttribute("count")]
        public Int32 Count { get; set; }

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
            ShowlightList = FixShowlights(ShowlightList);
            Count = ShowlightList.Count;
        }
        class EqShowlight : IEqualityComparer<Showlight>
        {
            public bool Equals(Showlight x, Showlight y)
            {   if     (x==null || y==null) return false;
                return (x.Note == y.Note && x.Time == y.Time) || 
                       (x.Note == y.Note && x.Time + .500D > y.Time); }

            public int GetHashCode(Showlight obj)
            {   if (Object.ReferenceEquals(obj, null)) return 0;
                return obj.Time.GetHashCode() ^ obj.Time.GetHashCode() + obj.Note.GetHashCode(); }
        }
        public void Serialize(Stream stream)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var writer = System.Xml.XmlWriter.Create(stream, new System.Xml.XmlWriterSettings
            { Indent = true, OmitXmlDeclaration = false, Encoding = Encoding.UTF8 }))
            {
                new XmlSerializer(typeof(Showlights)).Serialize(writer, this, ns);
            }
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);            
        }
        public static List<Showlight> FixShowlights(List<Showlight> ShowlightList)
        {
            for (var i = 0; ShowlightList.Count - 1 < i; i++)
            {
                if (ShowlightList[i].Note < 35)
                {
                    var objectToAdd = ShowlightList[i]; objectToAdd.Note += 12;
                    if (i + 1 == ShowlightList.Count) ShowlightList.Add(objectToAdd);
                    else ShowlightList.Insert(i + 1, objectToAdd);
                }
            }
            return ShowlightList;
        }
        public static Showlights LoadFromFile(string showlightsRS2014File)
        {
            using (var reader = new StreamReader(showlightsRS2014File))
            {
                return new Extensions.XmlStreamingDeserializer<Showlights>(reader).Deserialize();
            }
        }
    }
}

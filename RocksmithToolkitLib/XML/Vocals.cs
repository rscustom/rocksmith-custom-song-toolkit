using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.XML
{
    [XmlRoot("vocals", Namespace = "", IsNullable = false)]
    public class Vocals
    {
        public Vocals() { }

        public Vocals(Sng2014File sngData, bool validateLyrics = false)
        {
            Vocal = new Vocal[sngData.Vocals.Count];
            for (var i = 0; i < sngData.Vocals.Count; i++)
            {
                var v = new Vocal();
                v.Time = sngData.Vocals.Vocals[i].Time;
                v.Note = sngData.Vocals.Vocals[i].Note;
                v.Length = sngData.Vocals.Vocals[i].Length;
                v.Lyric = sngData.Vocals.Vocals[i].Lyric.ToNullTerminatedUTF8();

                if (validateLyrics)
                    v.Lyric = v.Lyric.GetValidLyric();

                Vocal[i] = v;
            }
            Count = Vocal.Length;
        }

        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("vocal")]
        public Vocal[] Vocal { get; set; }

        public static Vocals LoadFromFile(string xmlVocalsPath)
        {
            var xmlVocals = File.ReadAllText(xmlVocalsPath);
            using (var validXml = xmlVocals.StripIllegalXMLChars())
            using (var reader = new StreamReader(validXml))
            {
                return new XmlStreamingDeserializer<Vocals>(reader).Deserialize();
            }
        }

        public void Serialize(Stream stream, bool omitXmlDeclaration = false)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = omitXmlDeclaration,
                Encoding = new UTF8Encoding(false)
            }))
            {
                new XmlSerializer(typeof(Vocals)).Serialize(writer, this, ns);
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

    }

    [XmlType("vocal")]
    public class Vocal
    {
        [XmlAttribute("time")]
        public float Time { get; set; }

        [XmlAttribute("note")]
        public Int32 Note { get; set; }

        [XmlAttribute("length")]
        public float Length { get; set; }

        [XmlAttribute("lyric")] // len 32 (RS1) | len 48 (RS2014)
        public string Lyric { get; set; }
    }
}

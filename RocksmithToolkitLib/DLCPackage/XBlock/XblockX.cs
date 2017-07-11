using System.IO;
using System.Xml;
using System.Xml.Serialization;
using RocksmithToolkitLib.XML;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    // these methods have been validated for input and output
    // remember output may not look the same as input but the content has been confirmed the same

    [XmlRootAttribute("game", Namespace = "", IsNullable = false)]
    public class XblockX
    {
        [XmlArrayItemAttribute("entity", IsNullable = false)]
        public gameEntity[] entitySet { get; set; }

        public static XblockX LoadFromFile(string xblockFile)
        {
            XblockX xmlXblock = null;

            using (var reader = new StreamReader(xblockFile))
            {
                var serializer = new XmlSerializer(typeof(XblockX));
                xmlXblock = (XblockX)serializer.Deserialize(reader);
            }

            return xmlXblock;
        }

        public void Serialize(Stream stream, bool omitXmlDeclaration = false)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = omitXmlDeclaration }))
            {
                new XmlSerializer(typeof(XblockX)).Serialize(writer, this, ns);
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    public partial class gameEntity
    {
        [XmlElementAttribute("property")]
        public gameEntityProperty[] property { get; set; }

        [XmlAttributeAttribute()]
        public string id { get; set; }

        [XmlAttributeAttribute()]
        public string modelName { get; set; }

        [XmlAttributeAttribute()]
        public string name { get; set; }

        [XmlAttributeAttribute()]
        public sbyte iterations { get; set; }

        [XmlIgnoreAttribute()]
        public bool iterationsSpecified { get; set; }
    }

    public partial class gameEntityProperty
    {
        public gameEntityPropertySet set { get; set; }

        [XmlAttributeAttribute()]
        public string name { get; set; }
    }

    public partial class gameEntityPropertySet
    {
        [XmlAttributeAttribute()]
        public string value { get; set; }

        [XmlAttributeAttribute()]
        public sbyte index { get; set; }

        [XmlIgnoreAttribute()]
        public bool indexSpecified { get; set; }

        [XmlTextAttribute()]
        public string Value { get; set; }
    }

}
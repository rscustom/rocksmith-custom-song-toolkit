using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng2014HSL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.XML
{
    [XmlRoot(Namespace = "")]
    public sealed class GlyphDefinitions
    {
        [XmlAttribute]
        public int TextureWidth { get; set; }

        [XmlAttribute]
        public int TextureHeight { get; set; }

        [XmlElement("GlyphDefinition")]
        public List<GlyphDefinition> Glyphs { get; set; }

        public void Serialize(string filename)
        {
            var nameSpace = new XmlSerializerNamespaces();
            nameSpace.Add("", "");

            var serializer = new XmlSerializer(typeof(GlyphDefinitions));
            var settings = new XmlWriterSettings { Indent = true };

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                serializer.Serialize(writer, this, nameSpace);
            }
        }

        public static GlyphDefinitions LoadFromSng(Sng2014File sng)
        {
            var glyphs = new List<GlyphDefinition>(sng.SymbolsDefinition.Count);
            foreach(var glyph in sng.SymbolsDefinition.SymbolDefinitions)
            {
                glyphs.Add(new GlyphDefinition()
                {
                    Symbol = glyph.Text.ToNullTerminatedUTF8(),

                    InnerXMin = glyph.Rect_Inner.xMin,
                    InnerXMax = glyph.Rect_Inner.xMax,
                    InnerYMin = glyph.Rect_Inner.yMin,
                    InnerYMax = glyph.Rect_Inner.yMax,

                    OuterXMin = glyph.Rect_Outter.xMin,
                    OuterXMax = glyph.Rect_Outter.xMax,
                    OuterYMin = glyph.Rect_Outter.yMin,
                    OuterYMax = glyph.Rect_Outter.yMax
                });
            }

            return new GlyphDefinitions
            {
                TextureHeight = sng.SymbolsTexture.SymbolsTextures[0].Height,
                TextureWidth = sng.SymbolsTexture.SymbolsTextures[0].Width,
                Glyphs = glyphs
            };
        }

        public static void WriteToSng(Sng2014File sng, string glyphDefinitionsPath)
        {
            GlyphDefinitions glyphDefs;
            using (var reader = new StreamReader(glyphDefinitionsPath))
            {
                glyphDefs = new XmlStreamingDeserializer<GlyphDefinitions>(reader).Deserialize();
            }

            sng.SymbolsTexture.SymbolsTextures[0].Width = glyphDefs.TextureWidth;
            sng.SymbolsTexture.SymbolsTextures[0].Height = glyphDefs.TextureHeight;

            var symbolDefs = new List<SymbolDefinition>(glyphDefs.Glyphs.Count);

            foreach (var glyph in glyphDefs.Glyphs)
            {
                if (Encoding.UTF8.GetByteCount(glyph.Symbol) > 12)
                    throw new Exception(String.Format("The following symbol does not fit into 12 bytes when encoded in UTF-8: {0}", glyph.Symbol));

                var sDef = new SymbolDefinition
                {
                    Rect_Inner = new Rect
                    {
                        xMax = glyph.InnerXMax,
                        xMin = glyph.InnerXMin,
                        yMax = glyph.InnerYMax,
                        yMin = glyph.InnerYMin
                    },
                    Rect_Outter = new Rect
                    {
                        xMax = glyph.OuterXMax,
                        xMin = glyph.OuterXMin,
                        yMax = glyph.OuterYMax,
                        yMin = glyph.OuterYMin
                    }
                };
                Encoding.UTF8.GetBytes(glyph.Symbol).CopyTo(sDef.Text, 0);

                symbolDefs.Add(sDef);
            }

            sng.SymbolsDefinition = new SymbolDefinitionSection
            {
                Count = symbolDefs.Count,
                SymbolDefinitions = symbolDefs.ToArray()
            };
        }
    }

    [XmlRoot(Namespace = "")]
    public sealed class GlyphDefinition
    {
        [XmlAttribute]
        public string Symbol { get; set; }

        [XmlAttribute]
        public float InnerYMin { get; set; }

        [XmlAttribute]
        public float InnerYMax { get; set; }

        [XmlAttribute]
        public float InnerXMin { get; set; }

        [XmlAttribute]
        public float InnerXMax { get; set; }

        [XmlAttribute]
        public float OuterYMin { get; set; }

        [XmlAttribute]
        public float OuterYMax { get; set; }

        [XmlAttribute]
        public float OuterXMin { get; set; }

        [XmlAttribute]
        public float OuterXMax { get; set; }
    }
}

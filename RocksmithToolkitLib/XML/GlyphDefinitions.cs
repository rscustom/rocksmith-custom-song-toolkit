using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Sng2014HSL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using System.Linq;

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

        public void Serialize(string destPath)
        {
            var nameSpace = new XmlSerializerNamespaces();
            nameSpace.Add("", "");

            var serializer = new XmlSerializer(typeof(GlyphDefinitions));
            var settings = new XmlWriterSettings { Indent = true };

            using (XmlWriter writer = XmlWriter.Create(destPath, settings))
            {
                serializer.Serialize(writer, this, nameSpace);
            }
        }

        public static GlyphDefinitions LoadFromSng(Sng2014File sng)
        {
            var glyphs = new List<GlyphDefinition>(sng.SymbolsDefinition.Count);
            foreach (var glyph in sng.SymbolsDefinition.SymbolDefinitions)
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

        /// <summary>
        /// Updates CustomFont status by setting LyricsArtPath, GlyphsXmlPath, and HasCustomFont values
        /// <para>Method may not be called from within a foreach loop or used with other types of enumeration</para>
        /// <para>Default vocals (non-jvocals) may also have CustomFonts if '*_jvocals.xml' file does not exist</para>
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static bool UpdateCustomFontStatus(ref DLCPackage.Arrangement arr, string projectDir = "")
        {
            // ========================
            // Respect Processing Order
            // ========================
            // skip non-vocal arrangements
            if (arr.ArrangementType != ArrangementType.Vocal)
                return false;

            // skip arrangements that already have custom font
            if (arr.HasCustomFont)
                return true;

            // determine vocal type
            var vocalType = arr.SongXml.File.IndexOf("jvocals", StringComparison.OrdinalIgnoreCase) >= 0 ? "jvocals" : "vocals";
            if (arr.ArrangementName == ArrangementName.JVocals && vocalType != "jvocals")
                return false;
            if (arr.ArrangementName == ArrangementName.Vocals && vocalType != "vocals")
                return false;

            // determine if the vocal xml arrangement uses a CustomFont
            var useCustomFont = false;
            var vocals = Vocals.LoadFromFile(arr.SongXml.File);
            foreach (var voc in vocals.Vocal)
            {
                if (voc.Lyric.Length != voc.Lyric.GetValidLyric().Length)
                {
                    useCustomFont = true;
                    break;
                }
            }

            // establish the root of the project directory
            if (String.IsNullOrEmpty(projectDir))
                projectDir = Path.GetDirectoryName(arr.SongXml.File);
            // D:\\Temp\RS Root\dlc\Nanase-Aikawa_Yumemiru-Shoujo-ja-Irarenai_v2_RS2014_Pc\EOF\innayumemirushoujojairarenai_jvocals.xml
            // D:\Temp\RS Root\dlc\Nanase-Aikawa_Yumemiru-Shoujo-ja-Irarenai_v2_RS2014_Pc\assets\ui\lyrics\innayumemirushoujojairarenai\lyrics_innayumemirushoujojairarenai.dds
            if (Path.GetFileName(projectDir).Equals("EOF") || Path.GetFileName(projectDir).Equals("Toolkit"))
                projectDir = Path.GetDirectoryName(projectDir);

            // find first custom font dds file
            var lyricsFile = Directory.EnumerateFiles(projectDir, "*.dds", SearchOption.AllDirectories)
                .Where(fn => Path.GetFileName(fn).Equals("lyrics.dds") || Path.GetFileName(fn).StartsWith("lyrics_")).FirstOrDefault();

            // find first glyphs.xml file
            var glyphsFile = Directory.EnumerateFiles(projectDir, "*.glyphs.xml", SearchOption.AllDirectories).FirstOrDefault();

            // glphyType and jvocalsXmlExists can override useCustomFont i.e. non-jvocals may have/use CustomsFont
            if (!String.IsNullOrEmpty(glyphsFile))
            {
                var glyphType = glyphsFile.IndexOf("jvocals", StringComparison.OrdinalIgnoreCase) >= 0 ? "jvocals" : "vocals";
                var jvocalsXmlExists = Directory.EnumerateFiles(projectDir, "*_jvocals.xml", SearchOption.AllDirectories).Any();

                if (arr.ArrangementName == ArrangementName.JVocals && (glyphType == "jvocals" || jvocalsXmlExists))
                    useCustomFont = true;

                if (arr.ArrangementName == ArrangementName.Vocals && glyphType == "vocals" && !jvocalsXmlExists)
                    useCustomFont = true;
            }

            if (!useCustomFont)
                return false;

            if (!String.IsNullOrEmpty(lyricsFile))
                arr.LyricsArtPath = lyricsFile;

            if (!String.IsNullOrEmpty(glyphsFile))
                arr.GlyphsXmlPath = glyphsFile;

            // both lyricsFile and glyphsFile are required to produce CDLC with CustomFont
            if (!String.IsNullOrEmpty(lyricsFile) && !String.IsNullOrEmpty(glyphsFile)) // => HasCustomFont
                return true;

            return false;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class AggregateGraph2014 {
        #region Path/Names Template
        public static readonly string CANONICAL_MANIFEST = "/manifests/songs_dlc_{0}"; //DLC Name
        public static readonly string CANONICAL_GAMESONG = "/songs/bin/{0}"; //Platform Path [0]
        public static readonly string CANONICAL_ALBUMART = "/gfxassets/album_art";
        public static readonly string CANONICAL_XMLSONG = "/songs/arr";
        public static readonly string CANONICAL_XBLOCK = "gamexblocks/nsongs";
        public static readonly string CANONICAL_SOUNDBANK = "/audio/{0}"; //Platform Path [1]
        public static readonly string LOGPATH_SOUNDBANK = "/audio";

        public static readonly string NAME_HSAN = "songs_dlc_{0}"; //DLC Name
        public static readonly string NAME_SHOWLIGHT = "{0}_showlights"; //DLC Name
        public static readonly string NAME_DEFAULT = "{0}_{1}"; //DLC Name - Arrangement Name
        public static readonly string NAME_SOUNDBANK = "song_{0}"; //DLC Name
        public static readonly string NAME_SOUNDBANKPREVIEW = "song_{0}_preview"; //DLC Name
        #endregion

        private Platform currentPlatform { get; set; }

        public List<GraphItem> JsonDB { get; set; }
        public GraphItem HsanDB { get; set; }
        public List<GraphItemLLID> MusicgameSong { get; set; }
        public List<GraphItemLLID> SongXml { get; set; }
        public GraphItemLLID ShowlightXml { get; set; }
        public List<GraphItemLLID> AlbumArt { get; set; }
        public List<GraphItemLLID> Soundbank { get; set; }
        public GraphItem GameXblock { get; set; }

        public AggregateGraph2014() {}

        public AggregateGraph2014(DLCPackageData info, Platform platform) {
            currentPlatform = platform;
            var dlcName = info.Name.ToLower();

            // Xblock
            var xbl = new GraphItem();
            xbl.Name = dlcName;
            xbl.Canonical = String.Format(CANONICAL_XBLOCK, dlcName);
            xbl.RelPathDirectory = xbl.Canonical;
            xbl.Tag = new List<string>();
            xbl.Tag.Add(TagValue.EmergentWorld.GetDescription());
            xbl.Tag.Add(TagValue.XWorld.GetDescription());
            xbl.UUID = IdGenerator.Guid();
            xbl.RelPathFile = String.Format("{0}.xblock", xbl.Name);
            GameXblock = xbl;

            JsonDB = new List<GraphItem>();
            SongXml = new List<GraphItemLLID>();
            MusicgameSong = new List<GraphItemLLID>();
            foreach (var arrangement in info.Arrangements)
            {
                var name = String.Format(NAME_DEFAULT, dlcName, arrangement.Name.ToString().ToLower());
                // JsonDB
                var json = new GraphItem();
                json.Name = name;
                json.Canonical = String.Format(CANONICAL_MANIFEST, dlcName);
                json.RelPathDirectory = json.Canonical;
                json.Tag = new List<string>();
                json.Tag.Add(TagValue.Database.GetDescription());
                json.Tag.Add(TagValue.JsonDB.GetDescription());
                json.UUID = IdGenerator.Guid();
                json.RelPathFile = String.Format("{0}.json", json.Name);
                JsonDB.Add(json);

                // XmlSong
                var xml = new GraphItemLLID();
                xml.Name = name;
                xml.Canonical = String.Format(CANONICAL_XMLSONG, dlcName);
                xml.RelPathDirectory = xml.Canonical;
                xml.LogPathDirectory = xml.Canonical;
                xml.Tag = new List<string>();
                xml.Tag.Add(TagValue.Application.GetDescription());
                xml.Tag.Add(TagValue.XML.GetDescription());
                xml.UUID = arrangement.SongXml.UUID;
                xml.LLID = Guid.Parse(arrangement.SongXml.LLID);
                xml.RelPathFile = String.Format("{0}.xml", xml.Name);
                xml.LogPathFile = xml.RelPathFile;
                SongXml.Add(xml);

                // Musicgame
                var sng = new GraphItemLLID();
                sng.Name = name;
                sng.Canonical = String.Format(CANONICAL_GAMESONG, platform.GetPathName()[0].ToLower());
                sng.RelPathDirectory = sng.Canonical;
                sng.LogPathDirectory = sng.Canonical;
                sng.Tag = new List<string>();
                sng.Tag.Add(TagValue.Application.GetDescription());
                sng.Tag.Add(TagValue.MusicgameSong.GetDescription());
                sng.UUID = arrangement.SongFile.UUID;
                sng.LLID = Guid.Parse(arrangement.SongFile.LLID);
                sng.RelPathFile = String.Format("{0}.sng", sng.Name);
                sng.LogPathFile = sng.RelPathFile;
                MusicgameSong.Add(sng);
            }

            if (currentPlatform.version == GameVersion.RS2014)
            {
                // Hsan
                var hsan = new GraphItem();
                hsan.Name = String.Format(NAME_HSAN, dlcName);
                hsan.Canonical = String.Format(CANONICAL_MANIFEST, dlcName);
                hsan.RelPathDirectory = hsan.Canonical;
                hsan.Tag = new List<string>();
                hsan.Tag.Add(TagValue.Database.GetDescription());
                hsan.Tag.Add(TagValue.HsanDB.GetDescription());
                hsan.UUID = IdGenerator.Guid();
                hsan.RelPathFile = String.Format("{0}.hsan", hsan.Name);
                HsanDB = hsan;

                // Showlight (Xml)
                var xml = new GraphItemLLID();
                xml.Name = String.Format(NAME_SHOWLIGHT, dlcName);
                xml.Canonical = String.Format(CANONICAL_XMLSONG, dlcName);
                xml.RelPathDirectory = xml.Canonical;
                xml.LogPathDirectory = xml.Canonical;
                xml.Tag = new List<string>();
                xml.Tag.Add(TagValue.Application.GetDescription());
                xml.Tag.Add(TagValue.XML.GetDescription());
                xml.UUID = IdGenerator.Guid();
                xml.LLID = Guid.Parse(IdGenerator.LLID());
                xml.RelPathFile = String.Format("{0}.xml", xml.Name);
                xml.LogPathFile = xml.RelPathFile;
                SongXml.Add(xml);
                ShowlightXml = xml;
            }

            // Album Art (DDS)
            var aArtArray = new string[] { info.AlbumArtPath };
            if (currentPlatform.version == GameVersion.RS2014)
               aArtArray = new string[] { 
                    String.Format("album_{0}_256.dds", dlcName), 
                    String.Format("album_{0}_128.dds", dlcName), 
                    String.Format("album_{0}_64.dds", dlcName) };
            AlbumArt = new List<GraphItemLLID>();
            foreach (var album in aArtArray) {
                var dds = new GraphItemLLID();
                dds.Canonical = CANONICAL_ALBUMART;
                dds.RelPathDirectory = dds.Canonical;
                dds.LogPathDirectory = dds.Canonical;
                dds.Tag = new List<string>();
                dds.Tag.Add(TagValue.DDS.GetDescription());
                dds.Tag.Add(TagValue.Image.GetDescription());
                dds.UUID = IdGenerator.Guid();
                dds.LLID = Guid.Parse(IdGenerator.LLID());
                dds.Name = Path.GetFileNameWithoutExtension(album);
                dds.RelPathFile = Path.GetFileName(album);
                dds.LogPathFile = dds.RelPathFile;
                AlbumArt.Add(dds);
            }

            // Soundbank
            Soundbank = new List<GraphItemLLID>();
            var bnk = new GraphItemLLID();
            bnk.Canonical = String.Format(CANONICAL_SOUNDBANK, platform.GetPathName()[1].ToLower());
            bnk.RelPathDirectory = bnk.Canonical;
            bnk.LogPathDirectory = LOGPATH_SOUNDBANK;
            bnk.Tag = new List<string>();
            bnk.Tag.Add(TagValue.Audio.GetDescription());
            bnk.Tag.Add(TagValue.WwiseSoundBank.GetDescription());
            bnk.Tag.Add(GraphItem.GetAudioTagPlatformDescription(currentPlatform.platform));
            bnk.UUID = IdGenerator.Guid();
            bnk.LLID = Guid.Parse(IdGenerator.LLID());
            bnk.Name = String.Format(NAME_SOUNDBANK, dlcName);
            bnk.RelPathFile = String.Format("{0}.bnk", bnk.Name);
            bnk.LogPathFile = bnk.RelPathFile;
            Soundbank.Add(bnk);

            if (currentPlatform.version == GameVersion.RS2014) {
                // Soundbank Preview
                var bnkPreview = new GraphItemLLID();
                bnkPreview.Canonical = String.Format(CANONICAL_SOUNDBANK, platform.GetPathName()[1].ToLower());
                bnkPreview.RelPathDirectory = bnkPreview.Canonical;
                bnkPreview.LogPathDirectory = LOGPATH_SOUNDBANK;
                bnkPreview.Tag = new List<string>();
                bnkPreview.Tag.Add(TagValue.Audio.GetDescription());
                bnkPreview.Tag.Add(TagValue.WwiseSoundBank.GetDescription());
                bnkPreview.Tag.Add(GraphItem.GetAudioTagPlatformDescription(currentPlatform.platform));
                bnkPreview.UUID = IdGenerator.Guid();
                bnkPreview.LLID = Guid.Parse(IdGenerator.LLID());
                bnkPreview.Name = String.Format(NAME_SOUNDBANKPREVIEW, dlcName);
                bnkPreview.RelPathFile = String.Format("{0}.bnk", bnkPreview.Name);
                bnkPreview.LogPathFile = bnkPreview.RelPathFile;
                Soundbank.Add(bnkPreview);
            }
        }

        public void Serialize(Stream stream) {
            StreamWriter writer = new StreamWriter(stream);

            // JSON
            foreach (var json in JsonDB) {
                json.Write(writer);
            }

            if (currentPlatform.version == GameVersion.RS2014) {
                // HSAN
                HsanDB.Write(writer);

                // Showlight
                ShowlightXml.Write(writer);
            }

            // Xblock
            GameXblock.Write(writer);

            // Song Xml
            foreach (var xml in SongXml)
                xml.Write(writer);

            // Song SNG
            foreach (var sng in MusicgameSong)
                sng.Write(writer);

            // Album Art
            foreach (var album in AlbumArt)
                album.Write(writer);

            // Soundbank
            foreach (var bnk in Soundbank)
                bnk.Write(writer);

            writer.Flush();
        }

        public static AggregateGraph2014 LoadFromFile(string agregateGraphFile) {
            AggregateGraph2014 aggregateGraph = new AggregateGraph2014(); ;
            var graphPartList = GraphPart.GetGraphParts(File.ReadAllLines(agregateGraphFile));

            var json = GraphPart.WhereByValue(graphPartList, TagValue.JsonDB.GetDescription());
            if (json.Count() > 0) {
                aggregateGraph.JsonDB = new List<GraphItem>();
                foreach (var j in json) {
                    aggregateGraph.JsonDB.Add(new GraphItem(j.UUID, GraphPart.WhereByUUID(graphPartList, j.UUID)));
                }
            }

            var hsan = GraphPart.SingleByValue(graphPartList, TagValue.HsanDB.GetDescription());
            if (hsan != null)
                aggregateGraph.HsanDB = new GraphItem(hsan.UUID, GraphPart.WhereByUUID(graphPartList, hsan.UUID));

            var sng = GraphPart.WhereByValue(graphPartList, TagValue.MusicgameSong.GetDescription());
            if (sng.Count() > 0) {
                aggregateGraph.MusicgameSong = new List<GraphItemLLID>();
                foreach (var s in sng)
                    aggregateGraph.MusicgameSong.Add(new GraphItemLLID(s.UUID, GraphPart.WhereByUUID(graphPartList, s.UUID)));
            }

            var xml = GraphPart.WhereByValue(graphPartList, TagValue.XML.GetDescription());
            if (xml.Count() > 0) {
                foreach (var x in xml) {
                    aggregateGraph.SongXml = new List<GraphItemLLID>();
                    var graphList = GraphPart.WhereByUUID(graphPartList, x.UUID);
                    if (graphList.Exists(p => p.Value.Contains("showlights")))
                        aggregateGraph.ShowlightXml = new GraphItemLLID(x.UUID, graphList);
                    else
                        aggregateGraph.SongXml.Add(new GraphItemLLID(x.UUID, graphList));
                }
            }

            var dds = GraphPart.WhereByValue(graphPartList, TagValue.Image.GetDescription());
            if (dds.Count() > 0) {
                aggregateGraph.AlbumArt = new List<GraphItemLLID>();
                foreach (var d in dds)
                    aggregateGraph.AlbumArt.Add(new GraphItemLLID(d.UUID, GraphPart.WhereByUUID(graphPartList, d.UUID)));
            }

            var bnk = GraphPart.WhereByValue(graphPartList, TagValue.WwiseSoundBank.GetDescription());
            if (bnk.Count() > 0) {
                aggregateGraph.Soundbank = new List<GraphItemLLID>();
                foreach (var b in bnk)
                    aggregateGraph.Soundbank.Add(new GraphItemLLID(b.UUID, GraphPart.WhereByUUID(graphPartList, b.UUID)));
            }

            var xblock = GraphPart.SingleByValue(graphPartList, TagValue.XWorld.GetDescription());
            aggregateGraph.GameXblock = new GraphItem(xblock.UUID, GraphPart.WhereByUUID(graphPartList, xblock.UUID));


            return aggregateGraph;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph2014
{
    public class AggregateGraph2014 {
        #region Path/Names Template

        // PATH PATTERNS
        public static readonly string CANONICAL_MANIFEST_CONSOLE = "/manifests/songs_dlc";
        public static readonly string CANONICAL_MANIFEST_PC = "/manifests/songs_dlc_{0}"; // DLC Name;
        public static readonly string CANONICAL_GAMESONG = "/songs/bin/{0}"; //Platform Path [1]
        public static readonly string CANONICAL_LYRIC = "/assets/ui/lyrics/{0}"; //DLC Name
        public static readonly string CANONICAL_INLAY = "/assets/gameplay/inlay";
        public static Dictionary<DLCPackageType, string> CANONICAL_IMAGEART {
            get {
                var d = new Dictionary<DLCPackageType, string>();
                d.Add(DLCPackageType.Song, "/gfxassets/album_art");
                d.Add(DLCPackageType.Inlay, "/gfxassets/rewards/guitar_inlays");
                return d;
            }
        }
        public static readonly string CANONICAL_XMLSONG = "/songs/arr";
        public static Dictionary<DLCPackageType, string> CANONICAL_XBLOCK {
            get {
                var d = new Dictionary<DLCPackageType, string>();
                d.Add(DLCPackageType.Song, "/gamexblocks/nsongs");
                d.Add(DLCPackageType.Inlay, "/gamexblocks/nguitars");
                return d;
            }
        }
        public static readonly string CANONICAL_SOUNDBANK = "/audio/{0}"; //Platform Path [0]
        public static readonly string LOGPATH_SOUNDBANK = "/audio";

        // NAME PATTERNS
        public static readonly string NAME_ARRANGEMENT = "{0}_{1}"; //DLC Name - Arrangement Name
        public static readonly string NAME_SHOWLIGHT = "{0}_showlights"; //DLC Name
        #endregion

        private Platform currentPlatform;

        public List<GraphItem> JsonDB { get; set; }
        public List<GraphItem> HsonDB { get; set; }
        public GraphItem HsanDB { get; set; }
        public List<GraphItemLLID> MusicgameSong { get; set; }
        public List<GraphItemLLID> SongXml { get; set; }
        public GraphItemLLID ShowlightXml { get; set; }
        public List<GraphItemLLID> ImageArt { get; set; }
        public List<GraphItemLLID> Soundbank { get; set; }
        public GraphItem GameXblock { get; set; }
        public GraphItemLLID InlayNif { get; set; }

        public AggregateGraph2014() {}

        public AggregateGraph2014(DLCPackageData info, Platform platform, DLCPackageType dlcType = DLCPackageType.Song) {
            currentPlatform = platform;

            switch (dlcType) {
                case DLCPackageType.Song:
                    SongAggregateGraph(info, dlcType);
                    break;
                case DLCPackageType.Lesson:
                    throw new NotImplementedException("Lesson package type not implemented yet :(");
                case DLCPackageType.Inlay:
                    InlayAggregateGraph(info, dlcType);
                    break;
            }
        }

        private void SongAggregateGraph(DLCPackageData info, DLCPackageType dlcType) {
            var dlcName = info.Name.ToLower();
            var songPartition = new SongPartition();

            // Xblock
            var xbl = new GraphItem();
            xbl.Name = dlcName;
            xbl.Canonical = String.Format(CANONICAL_XBLOCK[dlcType], dlcName);
            xbl.RelPathDirectory = xbl.Canonical;
            xbl.Tag = new List<string>();
            xbl.Tag.Add(TagValue.EmergentWorld.GetDescription());
            xbl.Tag.Add(TagValue.XWorld.GetDescription());
            xbl.UUID = IdGenerator.Guid();
            xbl.RelPathFile = String.Format("{0}.xblock", xbl.Name);
            GameXblock = xbl;

            JsonDB = new List<GraphItem>();
            if (currentPlatform.IsConsole)
                HsonDB = new List<GraphItem>();

            SongXml = new List<GraphItemLLID>();
            MusicgameSong = new List<GraphItemLLID>();
            foreach (var arrangement in info.Arrangements) {
                var name = String.Format(NAME_ARRANGEMENT, dlcName, songPartition.GetArrangementFileName(arrangement.Name, arrangement.ArrangementType).ToLower());

                // JsonDB
                var json = new GraphItem();
                json.Name = name;
                json.Canonical = currentPlatform.IsConsole ? CANONICAL_MANIFEST_CONSOLE : String.Format(CANONICAL_MANIFEST_PC, dlcName);
                json.RelPathDirectory = json.Canonical;
                json.Tag = new List<string>();
                json.Tag.Add(TagValue.Database.GetDescription());
                json.Tag.Add(TagValue.JsonDB.GetDescription());
                json.UUID = IdGenerator.Guid();
                json.RelPathFile = String.Format("{0}.json", json.Name);
                JsonDB.Add(json);

                //One file for each arrangement (Xbox360 / PS3 only)
                if (currentPlatform.IsConsole) {
                    // HsonDB
                    var hson = new GraphItem();
                    hson.Name = name;
                    hson.Canonical = CANONICAL_MANIFEST_CONSOLE;
                    hson.RelPathDirectory = hson.Canonical;
                    hson.Tag = new List<string>();
                    hson.Tag.Add(TagValue.Database.GetDescription());
                    hson.Tag.Add(TagValue.HsonDB.GetDescription());
                    hson.UUID = IdGenerator.Guid();
                    hson.RelPathFile = String.Format("{0}.hson", json.Name);
                    HsonDB.Add(hson);
                }

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
                xml.LLID = IdGenerator.LLIDGuid ();
                xml.RelPathFile = String.Format ("{0}.xml", xml.Name);
                xml.LogPathFile = xml.RelPathFile;
                SongXml.Add(xml);

                // Musicgame
                var sng = new GraphItemLLID();
                sng.Name = name;
                sng.Canonical = String.Format(CANONICAL_GAMESONG, currentPlatform.GetPathName()[1].ToLower());
                sng.RelPathDirectory = sng.Canonical;
                sng.LogPathDirectory = sng.Canonical;
                sng.Tag = new List<string>();
                sng.Tag.Add(TagValue.Application.GetDescription());
                sng.Tag.Add(TagValue.MusicgameSong.GetDescription());
                if (currentPlatform.IsConsole)
                    sng.Tag.Add(GraphItem.GetPlatformTagDescription(currentPlatform.platform));
                sng.UUID = arrangement.SongFile.UUID;
                sng.LLID = IdGenerator.LLIDGuid ();
                sng.RelPathFile = String.Format ("{0}.sng", sng.Name);
                sng.LogPathFile = sng.RelPathFile;
                MusicgameSong.Add(sng);
            }

            if (currentPlatform.version == GameVersion.RS2014) {
                //One file for all arrangement (PC / Mac only)
                if (!currentPlatform.IsConsole) {
                    // HsanDB
                    var hsan = new GraphItem();
                    hsan.Name = String.Format("songs_dlc_{0}", dlcName);
                    hsan.Canonical = String.Format(CANONICAL_MANIFEST_PC, dlcName);
                    hsan.RelPathDirectory = hsan.Canonical;
                    hsan.Tag = new List<string>();
                    hsan.Tag.Add(TagValue.Database.GetDescription());
                    hsan.Tag.Add(TagValue.HsanDB.GetDescription());
                    hsan.UUID = IdGenerator.Guid();
                    hsan.RelPathFile = String.Format("{0}.hsan", hsan.Name);
                    HsanDB = hsan;
                }

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
                xml.LLID = IdGenerator.LLIDGuid ();
                xml.RelPathFile = String.Format("{0}.xml", xml.Name);
                xml.LogPathFile = xml.RelPathFile;
                SongXml.Add(xml);
                ShowlightXml = xml;
            }

            // Image Art (DDS)
            var aArtArray = new string[] { info.AlbumArtPath };
            if (currentPlatform.version == GameVersion.RS2014)
                aArtArray = new string[] { 
                    String.Format("album_{0}_256.dds", dlcName), 
                    String.Format("album_{0}_128.dds", dlcName), 
                    String.Format ("album_{0}_64.dds", dlcName)
                };
            ImageArt = new List<GraphItemLLID>();
            foreach (var album in aArtArray) {
                var dds = new GraphItemLLID();
                dds.Canonical = CANONICAL_IMAGEART[dlcType];
                dds.RelPathDirectory = dds.Canonical;
                dds.LogPathDirectory = dds.Canonical;
                dds.Tag = new List<string>();
                dds.Tag.Add(TagValue.DDS.GetDescription());
                dds.Tag.Add(TagValue.Image.GetDescription());
                dds.UUID = IdGenerator.Guid();
                dds.LLID = IdGenerator.LLIDGuid ();
                dds.Name = Path.GetFileNameWithoutExtension(album);
                dds.RelPathFile = Path.GetFileName(album);
                dds.LogPathFile = dds.RelPathFile;
                ImageArt.Add(dds);
            }

            // Lyrics Font Texture
            if (!String.IsNullOrEmpty(info.LyricArtPath)) {
                var dds = new GraphItemLLID();
                dds.Canonical = String.Format(CANONICAL_LYRIC, dlcName);
                dds.RelPathDirectory = dds.Canonical;
                dds.LogPathDirectory = dds.Canonical;
                dds.Tag = new List<string>();
                dds.Tag.Add(TagValue.DDS.GetDescription());
                dds.Tag.Add(TagValue.Image.GetDescription());
                dds.UUID = IdGenerator.Guid();
                dds.LLID = IdGenerator.LLIDGuid ();
                dds.Name = String.Format("lyrics_{0}", dlcName);
                dds.RelPathFile = dds.Name;
                dds.LogPathFile = dds.RelPathFile;
                ImageArt.Add(dds);
            }

            // Soundbank
            Soundbank = new List<GraphItemLLID>();
            var bnk = new GraphItemLLID();
            bnk.Canonical = String.Format(CANONICAL_SOUNDBANK, currentPlatform.GetPathName()[0].ToLower());
            bnk.RelPathDirectory = bnk.Canonical;
            bnk.LogPathDirectory = LOGPATH_SOUNDBANK;
            bnk.Tag = new List<string>();
            bnk.Tag.Add(TagValue.Audio.GetDescription());
            bnk.Tag.Add(TagValue.WwiseSoundBank.GetDescription());
            bnk.Tag.Add(GraphItem.GetPlatformTagDescription(currentPlatform.platform));
            bnk.UUID = IdGenerator.Guid();
            bnk.LLID = IdGenerator.LLIDGuid ();
            bnk.Name = String.Format("song_{0}", dlcName);
            bnk.RelPathFile = String.Format("{0}.bnk", bnk.Name);
            bnk.LogPathFile = bnk.RelPathFile;
            Soundbank.Add(bnk);

            if (currentPlatform.version == GameVersion.RS2014) {
                // Soundbank Preview
                var bnkPreview = new GraphItemLLID();
                bnkPreview.Canonical = String.Format(CANONICAL_SOUNDBANK, currentPlatform.GetPathName()[0].ToLower());
                bnkPreview.RelPathDirectory = bnkPreview.Canonical;
                bnkPreview.LogPathDirectory = LOGPATH_SOUNDBANK;
                bnkPreview.Tag = new List<string>();
                bnkPreview.Tag.Add(TagValue.Audio.GetDescription());
                bnkPreview.Tag.Add(TagValue.WwiseSoundBank.GetDescription());
                bnkPreview.Tag.Add(GraphItem.GetPlatformTagDescription(currentPlatform.platform));
                bnkPreview.UUID = IdGenerator.Guid();
                bnkPreview.LLID = IdGenerator.LLIDGuid ();
                bnkPreview.Name = String.Format("song_{0}_preview", dlcName);
                bnkPreview.RelPathFile = String.Format("{0}.bnk", bnkPreview.Name);
                bnkPreview.LogPathFile = bnkPreview.RelPathFile;
                Soundbank.Add(bnkPreview);
            }
        }

        private void InlayAggregateGraph(DLCPackageData info, DLCPackageType dlcType) {
            var dlcName = info.Inlay.DLCSixName;

            // Xblock
            var xbl = new GraphItem();
            xbl.Name = String.Format("guitar_{0}", dlcName);
            xbl.Canonical = String.Format(CANONICAL_XBLOCK[dlcType], dlcName);
            xbl.RelPathDirectory = xbl.Canonical;
            xbl.Tag = new List<string>();
            xbl.Tag.Add(TagValue.EmergentWorld.GetDescription());
            xbl.Tag.Add(TagValue.XWorld.GetDescription());
            xbl.UUID = IdGenerator.Guid();
            xbl.RelPathFile = String.Format("guitar_{0}.xblock", dlcName);
            GameXblock = xbl;

            // JsonDB
            JsonDB = new List<GraphItem>();
            var json = new GraphItem();
            json.Name = String.Format("dlc_guitar_{0}", dlcName);
            json.Canonical = currentPlatform.IsConsole ? CANONICAL_MANIFEST_CONSOLE : String.Format(CANONICAL_MANIFEST_PC, dlcName);
            json.RelPathDirectory = json.Canonical;
            json.Tag = new List<string>();
            json.Tag.Add(TagValue.Database.GetDescription());
            json.Tag.Add(TagValue.JsonDB.GetDescription());
            json.UUID = IdGenerator.Guid();
            json.RelPathFile = String.Format("dlc_guitar_{0}.json", dlcName);
            JsonDB.Add(json);

            if (currentPlatform.IsConsole) {
                // HsonDB - One file for each manifest (Xbox360 / PS3 only)
                HsonDB = new List<GraphItem>();
                var hson = new GraphItem();
                hson.Name = String.Format("dlc_{0}", dlcName);
                hson.Canonical = CANONICAL_MANIFEST_CONSOLE;
                hson.RelPathDirectory = hson.Canonical;
                hson.Tag = new List<string>();
                hson.Tag.Add(TagValue.Database.GetDescription());
                hson.Tag.Add(TagValue.HsonDB.GetDescription());
                hson.UUID = IdGenerator.Guid();
                hson.RelPathFile = String.Format("dlc_{0}.hson", dlcName);
                HsonDB.Add(hson);
            } else {
                // HsanDB - One file for all manifest (PC / Mac)
                var hsan = new GraphItem();
                hsan.Name = String.Format("dlc_{0}", dlcName); 
                hsan.Canonical = String.Format(CANONICAL_MANIFEST_PC, dlcName);
                hsan.RelPathDirectory = hsan.Canonical;
                hsan.Tag = new List<string>();
                hsan.Tag.Add(TagValue.Database.GetDescription());
                hsan.Tag.Add(TagValue.HsanDB.GetDescription());
                hsan.UUID = IdGenerator.Guid();
                hsan.RelPathFile = String.Format("dlc_{0}.hsan", dlcName);
                HsanDB = hsan;
            }

            ImageArt = new List<GraphItemLLID>();

            // Inlay Icon (DDS)
            var aArtArray = new string[] { String.Format("reward_inlay_{0}_512", dlcName), 
                                           String.Format("reward_inlay_{0}_256", dlcName),
                                           String.Format("reward_inlay_{0}_128", dlcName), 
                String.Format ("reward_inlay_{0}_64", dlcName)
            };

            foreach (var icon in aArtArray) {
                var iconDDS = new GraphItemLLID();
                iconDDS.Canonical = CANONICAL_IMAGEART[dlcType];
                iconDDS.RelPathDirectory = iconDDS.Canonical;
                iconDDS.LogPathDirectory = iconDDS.Canonical;
                iconDDS.Tag = new List<string>();
                iconDDS.Tag.Add(TagValue.DDS.GetDescription());
                iconDDS.Tag.Add(TagValue.Image.GetDescription());
                iconDDS.UUID = IdGenerator.Guid();
                iconDDS.LLID = IdGenerator.LLIDGuid ();
                iconDDS.Name = icon;
                iconDDS.RelPathFile = String.Format("{0}.dds", iconDDS.Name);
                iconDDS.LogPathFile = iconDDS.RelPathFile;
                ImageArt.Add(iconDDS);
            }

            // Inlay Art
            var inlayDDS = new GraphItemLLID();
            inlayDDS.Canonical = CANONICAL_INLAY;
            inlayDDS.RelPathDirectory = inlayDDS.Canonical;
            inlayDDS.LogPathDirectory = inlayDDS.Canonical;
            inlayDDS.Tag = new List<string>();
            inlayDDS.Tag.Add(TagValue.DDS.GetDescription());
            inlayDDS.Tag.Add(TagValue.Image.GetDescription());
            inlayDDS.UUID = IdGenerator.Guid();
            inlayDDS.LLID = IdGenerator.LLIDGuid ();
            inlayDDS.Name = String.Format("inlay_{0}", dlcName);
            inlayDDS.RelPathFile = String.Format("{0}.dds", inlayDDS.Name);
            inlayDDS.LogPathFile = inlayDDS.RelPathFile;
            ImageArt.Add(inlayDDS);

            // Inlay Nif
            var nif = new GraphItemLLID();
            nif.Canonical = CANONICAL_INLAY;
            nif.RelPathDirectory = nif.Canonical;
            nif.LogPathDirectory = nif.Canonical;
            nif.Tag = new List<string>();
            nif.Tag.Add(TagValue.Application.GetDescription());
            nif.Tag.Add(TagValue.GamebryoSceneGraph.GetDescription());
            nif.UUID = IdGenerator.Guid();
            nif.LLID = IdGenerator.LLIDGuid ();
            nif.Name = dlcName;
            nif.RelPathFile = String.Format("{0}.nif", dlcName);
            nif.LogPathFile = nif.RelPathFile;
            InlayNif = nif;
        }

        public void Serialize(Stream stream) {
            StreamWriter writer = new StreamWriter(stream);

            // JSON
            if (JsonDB != null)
                foreach (var json in JsonDB)
                    json.Write(writer);

            // HSON
            if (currentPlatform.IsConsole)
                if (HsonDB != null)
                    foreach (var hson in HsonDB)
                        hson.Write(writer);

            if (currentPlatform.version == GameVersion.RS2014) {
                // HSAN
                if (!currentPlatform.IsConsole)
                    if (HsanDB != null)
                        HsanDB.Write(writer);

                // Showlight
                if (ShowlightXml != null)
                    ShowlightXml.Write(writer);
            }

            // Song Xml
            if (SongXml != null)
                foreach (var xml in SongXml)
                    xml.Write(writer);

            // Song SNG
            if (MusicgameSong != null)
                foreach (var sng in MusicgameSong)
                    sng.Write(writer);

            // Album Art
            if (ImageArt != null)
                foreach (var album in ImageArt)
                    album.Write(writer);

            // Soundbank
            if (Soundbank != null)
                foreach (var bnk in Soundbank)
                    bnk.Write(writer);

            // InlayNif
            if (InlayNif != null)
                InlayNif.Write(writer);

            // Xblock
            GameXblock.Write(writer, true);

            writer.Flush();
        }

        public static AggregateGraph2014 LoadFromFile(string agregateGraphFile) {
            AggregateGraph2014 aggregateGraph = new AggregateGraph2014(); ;
            var graphPartList = GraphPart.GetGraphParts(File.ReadAllLines(agregateGraphFile));

            var json = GraphPart.WhereByValue(graphPartList, TagValue.JsonDB.GetDescription());
            if (json.Any()) {
                aggregateGraph.JsonDB = new List<GraphItem>();
                foreach (var j in json) {
                    aggregateGraph.JsonDB.Add(new GraphItem(j.UUID, GraphPart.WhereByUUID(graphPartList, j.UUID)));
                }
            }

            var hsan = GraphPart.SingleByValue(graphPartList, TagValue.HsanDB.GetDescription());
            if (hsan != null)
                aggregateGraph.HsanDB = new GraphItem(hsan.UUID, GraphPart.WhereByUUID(graphPartList, hsan.UUID));

            var sng = GraphPart.WhereByValue(graphPartList, TagValue.MusicgameSong.GetDescription());
            if (sng.Any()) {
                aggregateGraph.MusicgameSong = new List<GraphItemLLID>();
                foreach (var s in sng)
                    aggregateGraph.MusicgameSong.Add(new GraphItemLLID(s.UUID, GraphPart.WhereByUUID(graphPartList, s.UUID)));
            }

            var xml = GraphPart.WhereByValue(graphPartList, TagValue.XML.GetDescription());
            if (xml.Any()) {
                foreach (var x in xml) {
                    if (aggregateGraph.SongXml == null)
                        aggregateGraph.SongXml = new List<GraphItemLLID>();
                    var graphList = GraphPart.WhereByUUID(graphPartList, x.UUID);
                    if (graphList.Exists(p => p.Value.Contains("showlights")))
                        aggregateGraph.ShowlightXml = new GraphItemLLID(x.UUID, graphList);
                    else
                        aggregateGraph.SongXml.Add(new GraphItemLLID(x.UUID, graphList));
                }
            }

            var dds = GraphPart.WhereByValue(graphPartList, TagValue.Image.GetDescription());
            if (dds.Any()) {
                aggregateGraph.ImageArt = new List<GraphItemLLID>();
                foreach (var d in dds)
                    aggregateGraph.ImageArt.Add(new GraphItemLLID(d.UUID, GraphPart.WhereByUUID(graphPartList, d.UUID)));
            }

            var bnk = GraphPart.WhereByValue(graphPartList, TagValue.WwiseSoundBank.GetDescription());
            if (bnk.Any()) {
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

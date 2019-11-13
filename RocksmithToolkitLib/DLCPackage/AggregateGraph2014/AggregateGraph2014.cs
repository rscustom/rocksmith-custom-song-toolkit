using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.XML;
using System.Diagnostics;
using RocksmithToolkitLib.DLCPackage.Manifest.Functions;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph2014
{
    public class AggregateGraph2014
    {
        #region Paths/Names Templates

        // PATH PATTERNS
        public static readonly string CANONICAL_MANIFEST_CONSOLE = "/manifests/songs_dlc";
        public static readonly string CANONICAL_MANIFEST_PC = "/manifests/songs_dlc_{0}"; // DLC Name;
        public static readonly string CANONICAL_GAMESONG = "/songs/bin/{0}"; //Platform Path [1]
        public static readonly string CANONICAL_LYRIC = "/assets/ui/lyrics/{0}"; //DLC Name
        public static readonly string CANONICAL_INLAY = "/assets/gameplay/inlay";
        public static Dictionary<DLCPackageType, string> CANONICAL_IMAGEART
        {
            get
            {
                var d = new Dictionary<DLCPackageType, string>();
                d.Add(DLCPackageType.Song, "/gfxassets/album_art");
                d.Add(DLCPackageType.Inlay, "/gfxassets/rewards/guitar_inlays");
                return d;
            }
        }
        public static readonly string CANONICAL_XMLSONG = "/songs/arr";
        public static Dictionary<DLCPackageType, string> CANONICAL_XBLOCK
        {
            get
            {
                var d = new Dictionary<DLCPackageType, string>();
                d.Add(DLCPackageType.Song, "/gamexblocks/nsongs");
                d.Add(DLCPackageType.Inlay, "/gamexblocks/nguitars");
                return d;
            }
        }
        public static readonly string CANONICAL_SOUNDBANK = "/audio/{0}"; //Platform Path [0]
        public static readonly string LOGPATH_SOUNDBANK = "/audio";

        // NAME PATTERNS
        public static readonly string NAME_ARRANGEMENT = "{0}_{1}"; //DLC Name_Arrangement Name
        public static readonly string NAME_SHOWLIGHT = "{0}_showlights"; //DLC Name
        #endregion

        // sets a default platform
        private Platform currentPlatform = new Platform(GamePlatform.Pc, GameVersion.RS2014);

        public List<GraphItem> JsonDB { get; set; }
        public List<GraphItem> HsonDB { get; set; }
        public GraphItem HsanDB { get; set; } // combined hsan file of all songs
        public List<GraphItemLLID> MusicgameSong { get; set; }
        public List<GraphItemLLID> SongXml { get; set; }
        public List<GraphItemLLID> ShowlightXml { get; set; }
        public List<GraphItemLLID> ImageArt { get; set; }
        public List<GraphItemLLID> Soundbank { get; set; }
        public List<GraphItem> GameXblock { get; set; }
        public GraphItemLLID InlayNif { get; set; }

        public AggregateGraph2014() { }

        public AggregateGraph2014(DLCPackageData info, Platform platform, DLCPackageType dlcType = DLCPackageType.Song)
        {
            currentPlatform = platform;

            switch (dlcType)
            {
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

        private void SongAggregateGraph(DLCPackageData info, DLCPackageType dlcType)
        {
            var dlcName = info.Name.ToLower();
            var songPartition = new SongPartition();

            // Xblock
            GameXblock = new List<GraphItem>();
            var xbl = new GraphItem();
            xbl.Name = dlcName;
            xbl.Canonical = String.Format(CANONICAL_XBLOCK[dlcType], dlcName);
            xbl.RelPathDirectory = xbl.Canonical;
            xbl.Tag = new List<string>();
            xbl.Tag.Add(TagValue.EmergentWorld.GetDescription());
            xbl.Tag.Add(TagValue.XWorld.GetDescription());
            xbl.UUID = IdGenerator.Guid();
            xbl.RelPathFile = String.Format("{0}.xblock", xbl.Name);
            GameXblock.Add(xbl);

            JsonDB = new List<GraphItem>();
            if (currentPlatform.IsConsole)
                HsonDB = new List<GraphItem>();

            SongXml = new List<GraphItemLLID>();
            MusicgameSong = new List<GraphItemLLID>();
            foreach (var arrangement in info.Arrangements)
            {
                if (arrangement.ArrangementType == Sng.ArrangementType.ShowLight)
                    continue;

                var name = String.Format(NAME_ARRANGEMENT, dlcName, songPartition.GetArrangementFileName(arrangement.ArrangementName, arrangement.ArrangementType).ToLower());

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
                if (currentPlatform.IsConsole)
                {
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
                xml.LLID = IdGenerator.LLIDGuid();
                xml.RelPathFile = String.Format("{0}.xml", xml.Name);
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
                sng.LLID = IdGenerator.LLIDGuid();
                sng.RelPathFile = String.Format("{0}.sng", sng.Name);
                sng.LogPathFile = sng.RelPathFile;
                MusicgameSong.Add(sng);
            }

            if (currentPlatform.version == GameVersion.RS2014)
            {
                //One file for all arrangement (PC / Mac only)
                if (!currentPlatform.IsConsole)
                {
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
                var ShowlightXml = new List<GraphItemLLID>();
                var xml = new GraphItemLLID();
                xml.Name = String.Format(NAME_SHOWLIGHT, dlcName);
                xml.Canonical = String.Format(CANONICAL_XMLSONG, dlcName);
                xml.RelPathDirectory = xml.Canonical;
                xml.LogPathDirectory = xml.Canonical;
                xml.Tag = new List<string>();
                xml.Tag.Add(TagValue.Application.GetDescription());
                xml.Tag.Add(TagValue.XML.GetDescription());
                xml.UUID = IdGenerator.Guid();
                xml.LLID = IdGenerator.LLIDGuid();
                xml.RelPathFile = String.Format("{0}.xml", xml.Name);
                xml.LogPathFile = xml.RelPathFile;
                SongXml.Add(xml); // TODO: check this
                ShowlightXml.Add(xml);
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
            foreach (var album in aArtArray)
            {
                var dds = new GraphItemLLID();
                dds.Canonical = CANONICAL_IMAGEART[dlcType];
                dds.RelPathDirectory = dds.Canonical;
                dds.LogPathDirectory = dds.Canonical;
                dds.Tag = new List<string>();
                dds.Tag.Add(TagValue.DDS.GetDescription());
                dds.Tag.Add(TagValue.Image.GetDescription());
                dds.UUID = IdGenerator.Guid();
                dds.LLID = IdGenerator.LLIDGuid();
                dds.Name = Path.GetFileNameWithoutExtension(album);
                dds.RelPathFile = Path.GetFileName(album);
                dds.LogPathFile = dds.RelPathFile;
                ImageArt.Add(dds);
            }

            // Lyrics Font Texture (DDS)
            var lyricArtPath = String.Empty;
            if (info.Arrangements.Any(arr => arr.HasCustomFont))
                lyricArtPath = info.Arrangements.Find(arr => arr.HasCustomFont).LyricsArtPath;           
            
            // TOC ENTRY /assets/ui/lyrics/[dlcName]/lyrics_[dlcName].dds
            if (!String.IsNullOrEmpty(lyricArtPath))
            {
                var dds = new GraphItemLLID();
                dds.Canonical = String.Format(CANONICAL_LYRIC, dlcName);
                dds.RelPathDirectory = dds.Canonical;
                dds.LogPathDirectory = dds.Canonical;
                dds.Tag = new List<string>();
                dds.Tag.Add(TagValue.DDS.GetDescription());
                dds.Tag.Add(TagValue.Image.GetDescription());
                dds.UUID = IdGenerator.Guid();
                dds.LLID = IdGenerator.LLIDGuid();
                dds.Name = String.Format("lyrics_{0}", dlcName);
                dds.RelPathFile = String.Format("{0}.dds", dds.Name); //keep extension
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
            bnk.LLID = IdGenerator.LLIDGuid();
            bnk.Name = String.Format("song_{0}", dlcName);
            bnk.RelPathFile = String.Format("{0}.bnk", bnk.Name);
            bnk.LogPathFile = bnk.RelPathFile;
            Soundbank.Add(bnk);

            if (currentPlatform.version == GameVersion.RS2014)
            {
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
                bnkPreview.LLID = IdGenerator.LLIDGuid();
                bnkPreview.Name = String.Format("song_{0}_preview", dlcName);
                bnkPreview.RelPathFile = String.Format("{0}.bnk", bnkPreview.Name);
                bnkPreview.LogPathFile = bnkPreview.RelPathFile;
                Soundbank.Add(bnkPreview);
            }
        }

        private void InlayAggregateGraph(DLCPackageData info, DLCPackageType dlcType)
        {
            var dlcName = info.Inlay.DLCSixName;

            // Xblock
            GameXblock = new List<GraphItem>();
            var xbl = new GraphItem();
            xbl.Name = String.Format("guitar_{0}", dlcName);
            xbl.Canonical = String.Format(CANONICAL_XBLOCK[dlcType], dlcName);
            xbl.RelPathDirectory = xbl.Canonical;
            xbl.Tag = new List<string>();
            xbl.Tag.Add(TagValue.EmergentWorld.GetDescription());
            xbl.Tag.Add(TagValue.XWorld.GetDescription());
            xbl.UUID = IdGenerator.Guid();
            xbl.RelPathFile = String.Format("guitar_{0}.xblock", dlcName);
            GameXblock.Add(xbl);

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

            if (currentPlatform.IsConsole)
            {
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
            }
            else
            {
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

            foreach (var icon in aArtArray)
            {
                var iconDDS = new GraphItemLLID();
                iconDDS.Canonical = CANONICAL_IMAGEART[dlcType];
                iconDDS.RelPathDirectory = iconDDS.Canonical;
                iconDDS.LogPathDirectory = iconDDS.Canonical;
                iconDDS.Tag = new List<string>();
                iconDDS.Tag.Add(TagValue.DDS.GetDescription());
                iconDDS.Tag.Add(TagValue.Image.GetDescription());
                iconDDS.UUID = IdGenerator.Guid();
                iconDDS.LLID = IdGenerator.LLIDGuid();
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
            inlayDDS.LLID = IdGenerator.LLIDGuid();
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
            nif.LLID = IdGenerator.LLIDGuid();
            nif.Name = dlcName;
            nif.RelPathFile = String.Format("{0}.nif", dlcName);
            nif.LogPathFile = nif.RelPathFile;
            InlayNif = nif;
        }

        public void Serialize(Stream stream)
        {
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

            if (currentPlatform.version == GameVersion.RS2014)
            {
                // HSAN
                if (!currentPlatform.IsConsole)
                    if (HsanDB != null)
                        HsanDB.Write(writer);

                // Showlight
                if (ShowlightXml != null)
                    foreach (var showlightXml in ShowlightXml)
                        showlightXml.Write(writer);
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
            foreach (var xblock in GameXblock)
                xblock.Write(writer);

            writer.Flush();
        }

        public static AggregateGraph2014 LoadFromFile(string agregateGraphFile)
        {
            var aggregateGraph = new AggregateGraph2014();
            var graphPartList = GraphPart.GetGraphParts(File.ReadAllLines(agregateGraphFile));

            var json = GraphPart.WhereByValue(graphPartList, TagValue.JsonDB.GetDescription());
            if (json.Any())
            {
                aggregateGraph.JsonDB = new List<GraphItem>();
                foreach (var j in json)
                {
                    aggregateGraph.JsonDB.Add(new GraphItem(j.UUID, GraphPart.WhereByUUID(graphPartList, j.UUID)));
                }
            }

            // only one of these
            var hsan = GraphPart.SingleByValue(graphPartList, TagValue.HsanDB.GetDescription());
            if (hsan != null)
                aggregateGraph.HsanDB = new GraphItem(hsan.UUID, GraphPart.WhereByUUID(graphPartList, hsan.UUID));

            var sng = GraphPart.WhereByValue(graphPartList, TagValue.MusicgameSong.GetDescription());
            if (sng.Any())
            {
                aggregateGraph.MusicgameSong = new List<GraphItemLLID>();
                foreach (var s in sng)
                    aggregateGraph.MusicgameSong.Add(new GraphItemLLID(s.UUID, GraphPart.WhereByUUID(graphPartList, s.UUID)));
            }

            var xml = GraphPart.WhereByValue(graphPartList, TagValue.XML.GetDescription());
            if (xml.Any())
            {
                foreach (var x in xml)
                {
                    if (aggregateGraph.SongXml == null)
                        aggregateGraph.SongXml = new List<GraphItemLLID>();
                    if (aggregateGraph.ShowlightXml == null)
                        aggregateGraph.ShowlightXml = new List<GraphItemLLID>();

                    var graphList = GraphPart.WhereByUUID(graphPartList, x.UUID);
                    if (graphList.Exists(p => p.Value.Contains("showlights")))
                        aggregateGraph.ShowlightXml.Add(new GraphItemLLID(x.UUID, graphList));
                    else
                        aggregateGraph.SongXml.Add(new GraphItemLLID(x.UUID, graphList));
                }
            }

            var dds = GraphPart.WhereByValue(graphPartList, TagValue.Image.GetDescription());
            if (dds.Any())
            {
                aggregateGraph.ImageArt = new List<GraphItemLLID>();
                foreach (var d in dds)
                    aggregateGraph.ImageArt.Add(new GraphItemLLID(d.UUID, GraphPart.WhereByUUID(graphPartList, d.UUID)));
            }

            var bnk = GraphPart.WhereByValue(graphPartList, TagValue.WwiseSoundBank.GetDescription());
            if (bnk.Any())
            {
                aggregateGraph.Soundbank = new List<GraphItemLLID>();
                foreach (var b in bnk)
                    aggregateGraph.Soundbank.Add(new GraphItemLLID(b.UUID, GraphPart.WhereByUUID(graphPartList, b.UUID)));
            }

            var xblock = GraphPart.WhereByValue(graphPartList, TagValue.XWorld.GetDescription());
            if (xblock.Any())
            {
                aggregateGraph.GameXblock = new List<GraphItem>();
                foreach (var xb in xblock)
                {
                    aggregateGraph.GameXblock.Add(new GraphItem(xb.UUID, GraphPart.WhereByUUID(graphPartList, xb.UUID)));
                }
            }

            return aggregateGraph;
        }

        public static void MergeHsanFiles(string srcPath, string songPackName, string destPath)
        {
            //Load hsan manifest
            var hsanFiles = Directory.EnumerateFiles(srcPath, "*.hsan", SearchOption.AllDirectories).ToArray();
            if (hsanFiles.Length < 1)
                throw new DataException("No songs_*.hsan file found.");

            // merge multiple hsan files into a single hsan file
            if (hsanFiles.Length > 1)
            {
                var mergeSettings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union };
                JObject hsanObject1 = new JObject();

                foreach (var hsan in hsanFiles)
                {
                    JObject hsanObject2 = JObject.Parse(File.ReadAllText(hsan));
                    hsanObject1.Merge(hsanObject2, mergeSettings);
                }

                var hsanFileName = String.Format("songs_dlc_{0}.hsan", songPackName.ToLower());
                var hsanPath = Path.Combine(destPath, hsanFileName);
                string json = JsonConvert.SerializeObject(hsanObject1, Formatting.Indented);
                File.WriteAllText(hsanPath, json);

                // delete old hsan files
                foreach (var hsan in hsanFiles)
                    File.Delete(hsan);
            }
        }

        public static string DoLikeSongPack(string srcPath, string appId = "248750")
        {
            // create SongPack directory structure
            var dlcName = Path.GetFileName(srcPath).ToLower();
            var songPackDir = Path.Combine(Path.GetTempPath(), String.Format("{0}_songpack_p_Pc", dlcName));
            var audioWindowDir = Path.Combine(songPackDir, "audio", "windows");
            var flatmodelsRsDir = Path.Combine(songPackDir, "flatmodels", "rs");
            var gamexblocksNsongsDir = Path.Combine(songPackDir, "gamexblocks", "nsongs");
            var gfxassetsAlbumArtDir = Path.Combine(songPackDir, "gfxassets", "album_art");
            var manifestSongsDir = Path.Combine(songPackDir, "manifests", String.Format("songs_dlc_{0}", dlcName));
            var songsArrDir = Path.Combine(songPackDir, "songs", "arr");
            var binGenericDir = Path.Combine(songPackDir, "songs", "bin", "generic");

            if (Directory.Exists(songPackDir))
                IOExtension.DeleteDirectory(songPackDir);

            Directory.CreateDirectory(songPackDir);
            Directory.CreateDirectory(audioWindowDir);
            Directory.CreateDirectory(flatmodelsRsDir);
            Directory.CreateDirectory(gamexblocksNsongsDir);
            Directory.CreateDirectory(gfxassetsAlbumArtDir);
            Directory.CreateDirectory(manifestSongsDir);
            Directory.CreateDirectory(songsArrDir);
            Directory.CreateDirectory(binGenericDir);

            // populate SongPack temporary directory
            var audioWemFiles = Directory.EnumerateFiles(srcPath, "*.wem", SearchOption.AllDirectories).ToArray();
            foreach (var wem in audioWemFiles)
                File.Copy(wem, Path.Combine(audioWindowDir, Path.GetFileName(wem)));

            var audioBnkFiles = Directory.EnumerateFiles(srcPath, "*.bnk", SearchOption.AllDirectories).ToArray();
            foreach (var bnk in audioBnkFiles)
                File.Copy(bnk, Path.Combine(audioWindowDir, Path.GetFileName(bnk)));

            var xblockFiles = Directory.EnumerateFiles(srcPath, "*.xblock", SearchOption.AllDirectories).ToArray();
            foreach (var xblock in xblockFiles)
                File.Copy(xblock, Path.Combine(gamexblocksNsongsDir, Path.GetFileName(xblock)));

            var albumArtFiles = Directory.EnumerateFiles(srcPath, "*.dds", SearchOption.AllDirectories).ToArray();
            foreach (var albumArt in albumArtFiles)
                File.Copy(albumArt, Path.Combine(gfxassetsAlbumArtDir, Path.GetFileName(albumArt)));

            var jsonFiles = Directory.EnumerateFiles(srcPath, "*.json", SearchOption.AllDirectories).ToArray();
            foreach (var json in jsonFiles)
                File.Copy(json, Path.Combine(manifestSongsDir, Path.GetFileName(json)));

            var hsanFiles = Directory.EnumerateFiles(srcPath, "*.hsan", SearchOption.AllDirectories).ToArray();
            foreach (var hsan in hsanFiles)
                File.Copy(hsan, Path.Combine(manifestSongsDir, Path.GetFileName(hsan)));

            var sngFiles = Directory.EnumerateFiles(srcPath, "*.sng", SearchOption.AllDirectories).ToArray();
            foreach (var sng in sngFiles)
                File.Copy(sng, Path.Combine(binGenericDir, Path.GetFileName(sng)));

            // declare variables one time for use in DDC generation   
            DDCSettings.Instance.LoadConfigXml();
            var phraseLen = DDCSettings.Instance.PhraseLen;
            // removeSus may be depricated in latest DDC but left here for comptiblity
            var removeSus = DDCSettings.Instance.RemoveSus;
            var rampPath = DDCSettings.Instance.RampPath;
            var cfgPath = DDCSettings.Instance.CfgPath;

            // generate SongPack comment
            var spComment = "(Remastered by SongPack Maker)";
            var addDD = false;
            if (ConfigRepository.Instance().GetBoolean("ddc_autogen"))
            {
                addDD = true;
                spComment += " " + "(DDC by SongPack Maker)";
            }

            var xmlFiles = Directory.EnumerateFiles(srcPath, "*.xml", SearchOption.AllDirectories).ToArray();
            foreach (var xml in xmlFiles)
            {
                // completely skip dlc.xml template files
                if (xml.EndsWith("_RS2014.dlc.xml"))
                    continue;

                var xmlSongPack = Path.Combine(songsArrDir, Path.GetFileName(xml));
                File.Copy(xml, xmlSongPack);

                // skip vocal and showlight xml files
                if (xml.EndsWith("_vocals.xml") || xml.EndsWith("_showlights.xml"))
                    continue;

                // add DDC to xml arrangement
                if (addDD)
                {
                    // check if arrangment has pre existing DD and do not overwrite
                    var songXml = Song2014.LoadFromFile(xml);
                    var mf = new ManifestFunctions(GameVersion.RS2014);
                    if (mf.GetMaxDifficulty(songXml) == 0)
                    {
                        var consoleOutput = String.Empty;
                        // apply DD to xml arrangments... 0 = Ends normally with no error
                        var result = DDCreator.ApplyDD(xmlSongPack, phraseLen, removeSus, rampPath, cfgPath, out consoleOutput, true);
                        if (result == 1)
                            Debug.WriteLine(String.Format("Arrangement file '{0}' => {1}", Path.GetFileNameWithoutExtension(xml), "DDC ended with system error " + consoleOutput));
                        else if (result == 2)
                            Debug.WriteLine(String.Format("Arrangement file '{0}' => {1}", Path.GetFileNameWithoutExtension(xml), "DDC ended with application error " + consoleOutput));
                    }
                }
            }

            // generate new Aggregate Graph
            var aggGraphPack = new AggregateGraph2014();
            aggGraphPack.JsonDB = new List<GraphItem>();
            aggGraphPack.HsonDB = new List<GraphItem>(); // used for consoles ONLY
            aggGraphPack.HsanDB = new GraphItem();
            aggGraphPack.MusicgameSong = new List<GraphItemLLID>();
            aggGraphPack.SongXml = new List<GraphItemLLID>();
            aggGraphPack.ShowlightXml = new List<GraphItemLLID>();
            aggGraphPack.ImageArt = new List<GraphItemLLID>();
            aggGraphPack.Soundbank = new List<GraphItemLLID>();
            aggGraphPack.GameXblock = new List<GraphItem>();

            // fix aggregate graph entries, reusing existing Persistent ID
            var currentPlatform = new Platform(GamePlatform.Pc, GameVersion.RS2014);
            var aggregateGraphFiles = Directory.EnumerateFiles(srcPath, "*.nt", SearchOption.AllDirectories).ToArray();
            foreach (var aggGraph in aggregateGraphFiles)
            {
                var agg = LoadFromFile(aggGraph);

                foreach (var json in agg.JsonDB)
                {
                    json.Canonical = String.Format(CANONICAL_MANIFEST_PC, dlcName);
                    json.RelPathDirectory = json.Canonical;
                    json.Tag = new List<string>();
                    json.Tag.Add(TagValue.Database.GetDescription());
                    json.Tag.Add(TagValue.JsonDB.GetDescription());
                    json.UUID = IdGenerator.Guid();
                    json.RelPathFile = String.Format("{0}.json", json.Name);
                }

                aggGraphPack.JsonDB.AddRange(agg.JsonDB);
                aggGraphPack.MusicgameSong.AddRange(agg.MusicgameSong);
                aggGraphPack.SongXml.AddRange(agg.SongXml);
                aggGraphPack.ShowlightXml.AddRange(agg.ShowlightXml);
                aggGraphPack.ImageArt.AddRange(agg.ImageArt);
                aggGraphPack.Soundbank.AddRange(agg.Soundbank);

                aggGraphPack.GameXblock.AddRange(agg.GameXblock);
            }

            // create a single hsanDB entry
            aggGraphPack.HsanDB.Name = String.Format("songs_dlc_{0}", dlcName);
            aggGraphPack.HsanDB.Canonical = String.Format(CANONICAL_MANIFEST_PC, dlcName);
            aggGraphPack.HsanDB.RelPathDirectory = aggGraphPack.HsanDB.Canonical;
            aggGraphPack.HsanDB.Tag = new List<string>();
            aggGraphPack.HsanDB.Tag.Add(TagValue.Database.GetDescription());
            aggGraphPack.HsanDB.Tag.Add(TagValue.HsanDB.GetDescription());
            aggGraphPack.HsanDB.UUID = IdGenerator.Guid();
            aggGraphPack.HsanDB.RelPathFile = String.Format("{0}.hsan", aggGraphPack.HsanDB.Name);

            var aggregateGraphFileName = Path.Combine(songPackDir, String.Format("{0}_aggregategraph.nt", dlcName));
            using (var fs = new FileStream(aggregateGraphFileName, FileMode.Create))
            using (var ms = new MemoryStream())
            {
                aggGraphPack.Serialize(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
            }

            MergeHsanFiles(songPackDir, dlcName, manifestSongsDir);

            var appIdFile = Path.Combine(songPackDir, "appid.appid");
            File.WriteAllText(appIdFile, appId);

            var toolkitVersionFile = Path.Combine(songPackDir, "toolkit.version");
            using (var fs = new FileStream(toolkitVersionFile, FileMode.Create))
            using (var ms = new MemoryStream())
            {
                DLCPackageCreator.GenerateToolkitVersion(ms, packageVersion: "SongPack Maker v1.2", packageComment: spComment);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
            }

            var rootFlatFile = Path.Combine(flatmodelsRsDir, "rsenumerable_root.flat");
            using (var fs = new FileStream(rootFlatFile, FileMode.Create))
            using (var ms = new MemoryStream(Resources.rsenumerable_root))
            {
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
            }

            var songFlatFile = Path.Combine(flatmodelsRsDir, "rsenumerable_song.flat");
            using (var fs = new FileStream(songFlatFile, FileMode.Create))
            using (var ms = new MemoryStream(Resources.rsenumerable_song))
            {
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
            }

            return songPackDir;
        }


    }
}






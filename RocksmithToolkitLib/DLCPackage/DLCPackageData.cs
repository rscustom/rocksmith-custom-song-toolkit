using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using X360.STFS;

using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.DLCPackage
{
    public class DLCPackageData
    {
        public GameVersion GameVersion;

        public bool Pc { get; set; }
        public bool Mac { get; set; }
        public bool XBox360 { get; set; }
        public bool PS3 { get; set; }

        public string AppId { get; set; }
        public string Name { get; set; }
        public SongInfo SongInfo { get; set; }
        public string AlbumArtPath { get; set; }
        public string OggPath { get; set; }
        public string OggPreviewPath { get; set; }
        public List<Arrangement> Arrangements { get; set; }
        public float Volume { get; set; }
        public PackageMagic SignatureType { get; set; }
        public string PackageVersion { get; set; }

        private List<XBox360License> xbox360Licenses;
        public List<XBox360License> XBox360Licenses
        {
            get
            {
                if (xbox360Licenses == null)
                {
                    xbox360Licenses = new List<XBox360License>();
                    return xbox360Licenses;
                }
                else
                    return xbox360Licenses;
            }
            set { xbox360Licenses = value; }
        }

        #region RS1 only

        public List<Tone.Tone> Tones { get; set; }

        #endregion

        #region RS2014 only

        public List<Tone2014> TonesRS2014 { get; set; }
        public float? PreviewVolume { get; set; }
        public string LyricArtPath { get; set; }

        // Cache art image conversion
        public List<DDSConvertedFile> ArtFiles { get; set; }

        /// <summary>
        /// Loads required DLC info from folder.
        /// </summary>
        /// <returns>The DLCPackageData info.</returns>
        /// <param name="unpackedDir">Unpacked dir.</param>
        /// <param name="targetPlatform">Target platform.</param>
        /// <param name = "sourcePlatform"></param>
        public static DLCPackageData LoadFromFolder(string unpackedDir, Platform targetPlatform, Platform sourcePlatform = null) {
            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            data.SignatureType = PackageMagic.CON;
            if (sourcePlatform == null)
                sourcePlatform = unpackedDir.GetPlatform();

            //Arrangements / Tones
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();

            //Load files
            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            foreach (var json in jsonFiles) {
                var attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.ToArray()[0].Value.ToArray()[0].Value;
                var xmlName = attr.SongXml.Split(':')[3];
                var xmlFile = Directory.GetFiles(unpackedDir, xmlName + ".xml", SearchOption.AllDirectories)[0];

                if (attr.Phrases != null) {
                    if (data.SongInfo == null) {
                        // Fill Package Data
                        data.Name = attr.DLCKey;
                        data.Volume = attr.SongVolume;
                        data.PreviewVolume = (float)(attr.PreviewVolume ?? data.Volume);

                        // Fill SongInfo
                        data.SongInfo = new SongInfo();
                        data.SongInfo.SongDisplayName = attr.SongName;
                        data.SongInfo.SongDisplayNameSort = attr.SongNameSort;
                        data.SongInfo.Album = attr.AlbumName;
                        data.SongInfo.SongYear = attr.SongYear ?? 0;
                        data.SongInfo.Artist = attr.ArtistName;
                        data.SongInfo.ArtistSort = attr.ArtistNameSort;
                        data.SongInfo.AverageTempo = (int)attr.SongAverageTempo;
                    }

                    // Adding Tones
                    foreach (var jsonTone in attr.Tones) {
                        if (jsonTone == null) continue;
                        var key = jsonTone.Key;
                        if (data.TonesRS2014.All(t => t.Key != key))
                            data.TonesRS2014.Add(jsonTone);
                    }

                    // Adding Arrangement
                    data.Arrangements.Add(new Arrangement(attr, xmlFile));
                } else {
                    var voc = new Arrangement();
                    voc.Name = attr.JapaneseVocal == true ? ArrangementName.JVocals : ArrangementName.Vocals;
                    voc.ArrangementType = ArrangementType.Vocal;
                    voc.ScrollSpeed = 20;
                    voc.SongXml = new SongXML { File = xmlFile };
                    voc.SongFile = new SongFile { File = "" };
                    voc.CustomFont = attr.JapaneseVocal == true;
                    // Get symbols stuff, write plain sng to disk.
                    var fontSng = Directory.GetFiles(unpackedDir, xmlName + ".sng", SearchOption.AllDirectories)[0];
                    var vocSng = Sng2014HSL.Sng2014File.LoadFromFile(fontSng, sourcePlatform);
                    if (vocSng.IsCustomFont()) {
                        voc.CustomFont = true;
                        voc.FontSng = fontSng;
                        vocSng.WriteChartData(fontSng, new Platform(GamePlatform.Pc, GameVersion.None));
                    }
                    voc.Sng2014 = Sng2014HSL.Sng2014File.ConvertXML(xmlFile, ArrangementType.Vocal, voc.FontSng);

                    // Adding Arrangement
                    data.Arrangements.Add(voc);
                }
            }

            //Get DDS Files + hacky reuse if exist
            var ddsFiles = Directory.GetFiles(unpackedDir, "album_*.dds", SearchOption.AllDirectories);
            if (ddsFiles.Length > 0) {
                var ddsFilesC = new List<DDSConvertedFile>();
                foreach( var file in ddsFiles )
                switch(Path.GetFileNameWithoutExtension(file).Split('_')[2]){

                case "256":
                    data.AlbumArtPath = file;
                    ddsFilesC.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = file, destinationFile = file.CopyToTempFile(".dds") });
                break;
                case "128":
                    ddsFilesC.Add(new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = file, destinationFile = file.CopyToTempFile(".dds") });
                break;
                case "64":
                    ddsFilesC.Add(new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = file, destinationFile = file.CopyToTempFile(".dds") });
                break;
                } data.ArtFiles = ddsFilesC;
            }
            // Lyric Art
            var LyricArt = Directory.GetFiles(unpackedDir, "lyrics_*.dds", SearchOption.AllDirectories);
            if (LyricArt.Any())
                data.LyricArtPath = LyricArt[0];

            //Get other files
            //Audio files
            var targetAudioFiles = new List<string>();
            var sourceAudioFiles = Directory.GetFiles(unpackedDir, "*.wem", SearchOption.AllDirectories).ToArray();

            foreach (var file in sourceAudioFiles) {
                var newFile = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                if (targetPlatform.IsConsole != (sourcePlatform = file.GetAudioPlatform()).IsConsole)
                {
                    OggFile.ConvertAudioPlatform(file, newFile);
                    targetAudioFiles.Add(newFile);
                }
                else targetAudioFiles.Add(file);
            }

            if (!targetAudioFiles.Any())
                throw new InvalidDataException("Audio files not found.");

            string audioPath = null, audioPreviewPath = null;
            FileInfo a = new FileInfo(targetAudioFiles[0]);
            FileInfo b = null;

            if (targetAudioFiles.Count == 2) {
                b = new FileInfo(targetAudioFiles[1]);

                if (a.Length > b.Length) {
                    audioPath = a.FullName;
                    audioPreviewPath = b.FullName;
                } else {
                    audioPath = b.FullName;
                    audioPreviewPath = a.FullName;
                }
            } else
                audioPath = a.FullName;

            data.OggPath = audioPath;

            //Make Audio preview with expected name when rebuild
            if (!String.IsNullOrEmpty(audioPreviewPath)) {
                var newPreviewFileName = Path.Combine(Path.GetDirectoryName(audioPath), String.Format("{0}_preview{1}", Path.GetFileNameWithoutExtension(audioPath), Path.GetExtension(audioPath)));
                File.Move(audioPreviewPath, newPreviewFileName);
                data.OggPreviewPath = newPreviewFileName;
            }

            //AppID
            var appidFile = Directory.GetFiles(unpackedDir, "*.appid", SearchOption.AllDirectories);
            if (appidFile.Length > 0)
                data.AppId = File.ReadAllText(appidFile[0]);

            //Package version
            var versionFile = Directory.GetFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories);
            if (versionFile.Length > 0)
                data.PackageVersion = GeneralExtensions.ReadPackageVersion(versionFile[0]);
            else data.PackageVersion = "1";

            return data;
        }

        #region RS2014 Inlay only

        [XmlIgnore]
        public InlayData Inlay { get; set; }

        #endregion

        // needs to be called after all packages for platforms are created
        public void CleanCache() {
            if (ArtFiles != null) {
                foreach (var file in ArtFiles) {
                    try {
                        File.Delete(file.destinationFile);
                    } catch { }
                }
                ArtFiles = null;
            }

            if (Arrangements != null)
                foreach (var a in Arrangements)
                    a.CleanCache();
        }

        ~DLCPackageData()
        {
            CleanCache();
        }
        /// <summary>
        /// Transforms unpacked Song into project-like folder structure.
        /// </summary>
        /// <returns>Output folder path.</returns>
        /// <param name="unpackedDir">Unpacked dir.</param>
        public static string DoLikeProject(string unpackedDir)
        {
            const string EOF = "EOF";
            const string KIT = "Toolkit";
            string outdir, eofdir, kitdir;
            string SongName = "SongName";

            // Get name for a new folder
            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            var attr = Manifest2014<Attributes2014>.LoadFromFile(jsonFiles[0]).Entries.ToArray()[0].Value.ToArray()[0].Value;
            SongName = attr.FullName.Split('_')[0];

            //Create dir sruct
            outdir = Path.Combine(Path.GetDirectoryName(unpackedDir), String.Format("{0}_{1}", attr.ArtistName.GetValidSortName(), attr.SongName.GetValidSortName()).Replace(" ","-"));
            eofdir = Path.Combine(outdir, EOF);
            kitdir = Path.Combine(outdir, KIT);
            attr = null; //dispose

            // Don't work in same dir
            if (Directory.Exists(outdir)){
                if(outdir == unpackedDir)
                    return unpackedDir;
                DirectoryExtension.SafeDelete(outdir);
            }

            Directory.CreateDirectory(outdir);
            Directory.CreateDirectory(eofdir);
            Directory.CreateDirectory(kitdir);

            string[] xmlFiles = Directory.GetFiles(unpackedDir, "*.xml", SearchOption.AllDirectories);
            string[] sngFiles = Directory.GetFiles(unpackedDir, "*vocals.sng", SearchOption.AllDirectories);
            foreach (var json in jsonFiles)
            {
                var Name = Path.GetFileNameWithoutExtension(json);
                var xmlFile = xmlFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == Name);
                var sngFile = sngFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == Name);

                //Move all pair JSON\XML
                File.Move(json,    Path.Combine(kitdir, Name + ".json"));
                File.Move(xmlFile, Path.Combine(eofdir, Name + ".xml"));
                if (Name.EndsWith("vocals", StringComparison.Ordinal))
                    File.Move(sngFile, Path.Combine(kitdir, Name + ".sng"));
            }

            //Move all art_size.dds to KIT folder
            var ArtFiles = Directory.GetFiles(unpackedDir, "album_*_*.dds", SearchOption.AllDirectories);
            if (ArtFiles.Any())
                foreach(var art in ArtFiles)
                    File.Move(art, Path.Combine(kitdir, Path.GetFileName(art)));
            var LyricArt = Directory.GetFiles(unpackedDir, "lyrics_*.dds", SearchOption.AllDirectories);
            if (LyricArt.Any())
                foreach(var art in LyricArt)
                    File.Move(art, Path.Combine(kitdir, Path.GetFileName(art)));

            //Move ogg to EOF folder + rename
            var OggFiles = Directory.GetFiles(unpackedDir, "*_fixed.ogg", SearchOption.AllDirectories);
            if(!OggFiles.Any())
                throw new InvalidDataException("Audio files not found.");
            //TODO: read names from bnk and rename.
            var a0 = new FileInfo(OggFiles[0]);
            FileInfo b0 = null;
            if (OggFiles.Length == 2){
                b0 = new FileInfo(OggFiles[1]);

                if (a0.Length > b0.Length) {
                    File.Move(a0.FullName, Path.Combine(eofdir, SongName + ".ogg"));
                    File.Move(b0.FullName, Path.Combine(eofdir, SongName + "_preview.ogg"));
                } else {
                    File.Move(b0.FullName, Path.Combine(eofdir, SongName + ".ogg"));
                    File.Move(a0.FullName, Path.Combine(eofdir, SongName + "_preview.ogg"));
                }
            }
            else File.Move(a0.FullName, Path.Combine(eofdir, SongName + ".ogg"));

            //Move wem to KIT folder + rename
            var WemFiles = Directory.GetFiles(unpackedDir, "*.wem", SearchOption.AllDirectories);
            if(!WemFiles.Any())
                throw new InvalidDataException("Audio files not found.");

            var a1 = new FileInfo(WemFiles[0]);
            FileInfo b1 = null;
            if (WemFiles.Length == 2){
                b1 = new FileInfo(WemFiles[1]);

                if (a1.Length > b1.Length) {
                    File.Move(a1.FullName, Path.Combine(kitdir, SongName + ".wem"));
                    File.Move(b1.FullName, Path.Combine(kitdir, SongName + "_preview.wem"));
                } else {
                    File.Move(b1.FullName, Path.Combine(kitdir, SongName + ".wem"));
                    File.Move(a1.FullName, Path.Combine(kitdir, SongName + "_preview.wem"));
                }
            } 
            else File.Move(a1.FullName, Path.Combine(kitdir, SongName + ".wem"));

            //Move Appid for correct template generation.
            var appidFile = Directory.GetFiles(unpackedDir, "*.appid", SearchOption.AllDirectories);
            if (appidFile.Length > 0)
                File.Move(appidFile[0], Path.Combine(kitdir, Path.GetFileName(appidFile[0])));

            //Move toolkit.version
            var toolkitVersion = Directory.GetFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories);
            if (toolkitVersion.Length > 0)
                File.Move(toolkitVersion[0], Path.Combine(kitdir, Path.GetFileName(toolkitVersion[0])));

            //Remove old folder
            DirectoryExtension.SafeDelete(unpackedDir);

            return outdir;
        }
        #endregion
    }

    public class DDSConvertedFile {
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public string sourceFile { get; set; }
        public string destinationFile { get; set; }
    }

    public class InlayData {
        public string DLCSixName { get; set; }
        public string InlayPath { get; set; }
        public string IconPath { get; set; }
        public Guid Id { get; set; }
        public bool Frets24 { get; set; }
        public bool Colored { get; set; }

        public InlayData() {
            Id = IdGenerator.Guid();
        }
    }
}
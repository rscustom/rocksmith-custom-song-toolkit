using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using X360.STFS;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;
using System.Xml.Serialization;

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
        
        private List<XBox360License> xbox360Licenses = null;
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
        
        // Cache art image conversion
        public List<DDSConvertedFile> ArtFiles { get; set; }

        public string LyricsTex { get; set; }

        public static DLCPackageData LoadFromFolder(string unpackedDir, Platform targetPlatform) {
            //Load files
            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            data.SignatureType = PackageMagic.CON;

            //Get Arrangements / Tones
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();

            foreach (var json in jsonFiles) {
                Attributes2014 attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.ToArray()[0].Value.ToArray()[0].Value;
                var xmlName = attr.SongXml.Split(':')[3];
                var xmlFile = Directory.GetFiles(unpackedDir, xmlName + ".xml", SearchOption.AllDirectories)[0];

                if (attr.Phrases != null) {
                    if (data.SongInfo == null) {
                        // Fill Package Data
                        data.Name = attr.DLCKey;
                        data.Volume = attr.SongVolume;
                        data.PreviewVolume = (attr.PreviewVolume != null) ? (float)attr.PreviewVolume : data.Volume;

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
                        if (!data.TonesRS2014.OfType<Tone2014>().Any(t => t.Key == jsonTone.Key))
                            data.TonesRS2014.Add(jsonTone);
                    }

                    // Adding Arrangement
                    data.Arrangements.Add(new Arrangement(attr, xmlFile));
                } else {
                    var voc = new Arrangement();
                    voc.Name = ArrangementName.Vocals;
                    voc.ArrangementType = ArrangementType.Vocal;
                    voc.SongXml = new SongXML { File = xmlFile };
                    voc.SongFile = new SongFile { File = "" };
                    voc.Sng2014 = Sng2014HSL.Sng2014File.ConvertXML(xmlFile, ArrangementType.Vocal);
                    voc.ScrollSpeed = 20;

                    // Adding Arrangement
                    data.Arrangements.Add(voc);
                }
            }

            //Get Files
            var ddsFiles = Directory.GetFiles(unpackedDir, "*_256.dds", SearchOption.AllDirectories);
            if (ddsFiles.Length > 0)
                data.AlbumArtPath = ddsFiles[0];

            var sourceAudioFiles = Directory.GetFiles(unpackedDir, "*.wem", SearchOption.AllDirectories);

            var targetAudioFiles = new List<string>();
            foreach (var file in sourceAudioFiles) {
                var newFile = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                if (targetPlatform.IsConsole != file.GetAudioPlatform().IsConsole)
                {
                    OggFile.ConvertAudioPlatform(file, newFile);
                    targetAudioFiles.Add(newFile);
                }
                else targetAudioFiles.Add(file);
            }

            if (targetAudioFiles.Count() <= 0)
                throw new InvalidDataException("Audio files not found.");

            string audioPath = null, audioPreviewPath = null;
            FileInfo a = new FileInfo(targetAudioFiles[0]);
            FileInfo b = null;

            if (targetAudioFiles.Count() == 2) {
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

            var appidFile = Directory.GetFiles(unpackedDir, "*.appid", SearchOption.AllDirectories);
            if (appidFile.Length > 0)
                data.AppId = File.ReadAllText(appidFile[0]);

            return data;
        }

        #endregion

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

        public static string DoLikeProject(string unpackedDir)
        {
            //Get name for new folder name
            string outdir = "";
            string EOF = "EOF";
            string KIT = "Toolkit";
            string SongName = "SongName";
            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            var attr = Manifest2014<Attributes2014>.LoadFromFile(jsonFiles[0]).Entries.ToArray()[0].Value.ToArray()[0].Value;

            //Create dir sruct
            SongName = attr.FullName.Split('_')[0];
            outdir = Path.Combine(Path.GetDirectoryName(unpackedDir), String.Format("{0}_{1}", attr.ArtistNameSort.GetValidName(false), attr.SongNameSort.GetValidName(false)));
            if (Directory.Exists(outdir))
                outdir += "_" + DateTime.Now.ToString("yyyy-MM-dd");

            Directory.CreateDirectory(outdir);
            Directory.CreateDirectory(Path.Combine(outdir, EOF));
            Directory.CreateDirectory(Path.Combine(outdir, KIT));

            foreach (var json in jsonFiles)
            {
                var atr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.ToArray()[0].Value.ToArray()[0].Value;
                var Name = atr.SongXml.Split(':')[3];
                var xmlFile = Directory.GetFiles(unpackedDir, Name + ".xml", SearchOption.AllDirectories)[0];

                //Move all pair JSON\XML
                File.Move(json, Path.Combine(outdir, KIT, Name + ".json"));
                File.Move(xmlFile, Path.Combine(outdir, EOF, Name + ".xml"));
            }

            //Move art_256.dds to KIT folder
            var ArtFile = Directory.GetFiles(unpackedDir, "*_256.dds", SearchOption.AllDirectories);
            if (ArtFile.Length > 0)
                File.Move(ArtFile[0], Path.Combine(outdir, KIT, Path.GetFileName(ArtFile[0])));

            //Move ogg to EOF folder + rename
            var OggFiles = Directory.GetFiles(unpackedDir, "*_fixed.ogg", SearchOption.AllDirectories);
            if(OggFiles.Count() <= 0)
                throw new InvalidDataException("Audio files not found.");

            var a0 = new FileInfo(OggFiles[0]);
            FileInfo b0 = null;
            if (OggFiles.Count() == 2){
                b0 = new FileInfo(OggFiles[1]);

                if (a0.Length > b0.Length) {
                    File.Move(a0.FullName, Path.Combine(outdir, EOF, SongName + ".ogg"));
                    File.Move(b0.FullName, Path.Combine(outdir, EOF, SongName + "_preview.ogg"));
                } else {
                    File.Move(b0.FullName, Path.Combine(outdir, EOF, SongName + ".ogg"));
                    File.Move(a0.FullName, Path.Combine(outdir, EOF, SongName + "_preview.ogg"));
                }
            }
            else File.Move(a0.FullName, Path.Combine(outdir, EOF, SongName + ".ogg"));

            //Move wem to KIT folder + rename
            var WemFiles = Directory.GetFiles(unpackedDir, "*.wem", SearchOption.AllDirectories);
            if(WemFiles.Count() <= 0)
                throw new InvalidDataException("Audio files not found.");

            var a1 = new FileInfo(WemFiles[0]);
            FileInfo b1 = null;
            if (WemFiles.Count() == 2){
                b1 = new FileInfo(WemFiles[1]);

                if (a1.Length > b1.Length) {
                    File.Move(a1.FullName, Path.Combine(outdir, KIT, SongName + ".wem"));
                    File.Move(b1.FullName, Path.Combine(outdir, KIT, SongName + "_preview.wem"));
                } else {
                    File.Move(b1.FullName, Path.Combine(outdir, KIT, SongName + ".wem"));
                    File.Move(a1.FullName, Path.Combine(outdir, KIT, SongName + "_preview.wem"));
                }
            } 
            else File.Move(a1.FullName, Path.Combine(outdir, KIT, SongName + ".wem"));

            //Move Appid for correct template generation.
            var appidFile = Directory.GetFiles(unpackedDir, "*.appid", SearchOption.AllDirectories);
            if (appidFile.Length > 0)
                File.Move(appidFile[0], Path.Combine(outdir, KIT, Path.GetFileName(appidFile[0])));

            //Remove old folder
            DirectoryExtension.SafeDelete(unpackedDir);

            return outdir;
        }
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
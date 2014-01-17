using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using X360.STFS;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;

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

        // cache album art conversion
        public Dictionary<int, string> AlbumArt { get; set; }

        #endregion

        public static DLCPackageData LoadFromFile(string unpackedDir) {
            //Load files
            var xmlSongs = Directory.GetFiles(unpackedDir, "*_*.xml", SearchOption.AllDirectories);

            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            var aggregateFile = Directory.GetFiles(unpackedDir, "*.nt", SearchOption.AllDirectories)[0];
            var aggregateData = AggregateGraph2014.LoadFromFile(aggregateFile);

            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            data.SignatureType = PackageMagic.CON;

            //Get Arrangements / Tones
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();

            foreach (var json in jsonFiles) {
                Attributes2014 attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.ToArray()[0].Value.ToArray()[0].Value;
                var xmlName = attr.SongXml.Split(':')[3];
                var aggXml = aggregateData.SongXml.SingleOrDefault(n => n.Name == xmlName);
                var xmlFile = Directory.GetFiles(unpackedDir, xmlName + ".xml", SearchOption.AllDirectories)[0];

                if (attr.Phrases != null) {
                    if (data.SongInfo == null) {
                        // Fill Package Data
                        data.Name = attr.DLCKey;
                        data.Volume = attr.SongVolume;

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
            var ddsFiles = Directory.GetFiles(unpackedDir, "*.dds", SearchOption.AllDirectories);
            if (ddsFiles.Length > 0)
                data.AlbumArtPath = ddsFiles[1];

            var sourceAudioFiles = Directory.GetFiles(unpackedDir, "*.wem", SearchOption.AllDirectories);

            var targetAudioFiles = new List<string>();
            foreach (var file in sourceAudioFiles) {
                var newFile = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                OggFile.ConvertAudioPlatform(file, newFile);
                targetAudioFiles.Add(newFile);
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

        // needs to be called after all packages for platforms are created
        public void CleanCache() {
            if (AlbumArt != null) {
                foreach (var path in AlbumArt.Values) {
                    try {
                        File.Delete(path);
                    } catch { }
                }
                AlbumArt = null;
            }
            foreach (var a in Arrangements)
                a.CleanCache();
        }

        ~DLCPackageData()
        {
            CleanCache();
        }
    }
}

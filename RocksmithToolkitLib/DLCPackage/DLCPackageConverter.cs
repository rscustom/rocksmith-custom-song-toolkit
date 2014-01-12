using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using X360.STFS;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class DLCPackageConverter
    {
        public static string Convert(string sourcePackage, Platform sourcePlatform, Platform targetPlatform, string appId) {

            var needRebuildPackage = sourcePlatform.IsConsole != targetPlatform.IsConsole;
            var tmpDir = Path.GetTempPath();
            var unpackedDir = Path.Combine(tmpDir, String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(sourcePackage), sourcePlatform.platform));

            DirectoryExtension.SafeDelete(unpackedDir);

            Packer.Unpack(sourcePackage, tmpDir);

            // DESTINATION
            var nameTemplate = (!targetPlatform.IsConsole) ? "{0}{1}.psarc" : "{0}{1}";

            var packageName = Path.GetFileNameWithoutExtension(sourcePackage);
            if (packageName.EndsWith(new Platform(GamePlatform.Pc, GameVersion.None).GetPathName()[2]) ||
                    packageName.EndsWith(new Platform(GamePlatform.Mac, GameVersion.None).GetPathName()[2]) ||
                    packageName.EndsWith(new Platform(GamePlatform.XBox360, GameVersion.None).GetPathName()[2]) ||
                    packageName.EndsWith(new Platform(GamePlatform.PS3, GameVersion.None).GetPathName()[2] + ".psarc"))
            {
                packageName = packageName.Substring(0, packageName.LastIndexOf("_"));
            }
            var targetFileName = Path.Combine(Path.GetDirectoryName(sourcePackage), String.Format(nameTemplate, Path.Combine(Path.GetDirectoryName(sourcePackage), packageName), targetPlatform.GetPathName()[2]));

            var search4xml = Directory.GetFiles(unpackedDir, "*_*.xml", SearchOption.AllDirectories).Length;
            var search4showLts = Directory.GetFiles(unpackedDir, "*_showlights.xml", SearchOption.AllDirectories).Length;
            
            var hasNoXmlSong = search4xml <= 1 || search4showLts < 1;

            // CONVERSION                
            if (needRebuildPackage)
            {
                if (hasNoXmlSong)
                {
                    return String.Format("Package {0} is not a custom song, you need a custom song to convert Rocksmith 2014 from non similiar platforms.", sourcePackage);
                }
                ConvertPackageRebuilding(unpackedDir, targetFileName, sourcePlatform, targetPlatform, appId);
            }
            else
                ConvertPackageForSimilarPlatform(unpackedDir, targetFileName, sourcePlatform, targetPlatform, appId);

            DirectoryExtension.SafeDelete(unpackedDir);

            return String.Empty;
        }

        private static void ConvertPackageForSimilarPlatform(string unpackedDir, string targetFileName, Platform sourcePlatform, Platform targetPlatform, string appId)
        {
            // Old and new paths
            var sourceDir0 = sourcePlatform.GetPathName()[0].ToLower();
            var sourceDir1 = sourcePlatform.GetPathName()[1].ToLower();
            var targetDir0 = targetPlatform.GetPathName()[0].ToLower();
            var targetDir1 = targetPlatform.GetPathName()[1].ToLower();

            if (!targetPlatform.IsConsole)
            {
                // Replace AppId
                var appIdFile = Path.Combine(unpackedDir, "appid.appid");
                File.WriteAllText(appIdFile, appId);
            }

            // Replace aggregate graph values
            var aggregateFile = Directory.GetFiles(unpackedDir, "*.nt", SearchOption.TopDirectoryOnly)[0];
            var aggregateGraphText = File.ReadAllText(aggregateFile);
            // Tags
            aggregateGraphText = Regex.Replace(aggregateGraphText, GraphItem.GetPlatformTagDescription(sourcePlatform.platform), GraphItem.GetPlatformTagDescription(targetPlatform.platform), RegexOptions.Multiline);
            // Paths
            aggregateGraphText = Regex.Replace(aggregateGraphText, sourceDir0, targetDir0, RegexOptions.Multiline);
            aggregateGraphText = Regex.Replace(aggregateGraphText, sourceDir1, targetDir1, RegexOptions.Multiline);
            File.WriteAllText(aggregateFile, aggregateGraphText);

            // Rename directories
            foreach (var dir in Directory.GetDirectories(unpackedDir, "*.*", SearchOption.AllDirectories))
            {
                if (dir.EndsWith(sourceDir0))
                {
                    var newDir = dir.Replace(sourceDir0, targetDir0);
                    DirectoryExtension.SafeDelete(newDir);
                    DirectoryExtension.Move(dir, newDir);
                }
                else if (dir.EndsWith(sourceDir1))
                {
                    var newDir = dir.Replace(sourceDir1, targetDir1);
                    DirectoryExtension.SafeDelete(newDir);
                    DirectoryExtension.Move(dir, newDir);
                }
            }

            // Recreates SNG because SNG have different keys in PC and Mac
            bool updateSNG = ((sourcePlatform.platform == GamePlatform.Pc && targetPlatform.platform == GamePlatform.Mac) ||
                (sourcePlatform.platform == GamePlatform.Mac && targetPlatform.platform == GamePlatform.Pc));

            // Packing
            Packer.Pack(unpackedDir, targetFileName, updateSNG);
            DirectoryExtension.SafeDelete(unpackedDir);
        }

        private static void ConvertPackageRebuilding(string unpackedDir, string targetFileName, Platform sourcePlatform, Platform targetPlatform, string appId)
        {
            //Load files
            var xmlSongs = Directory.GetFiles(unpackedDir, "*_*.xml", SearchOption.AllDirectories);

            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            var aggregateFile = Directory.GetFiles(unpackedDir, "*.nt", SearchOption.AllDirectories)[0];
            var aggregateData = AggregateGraph2014.LoadFromFile(aggregateFile);

            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            if (!targetPlatform.IsConsole)
                data.AppId = appId;
            data.SignatureType = PackageMagic.CON;

            //Get Arrangements / Tones
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();

            foreach (var json in jsonFiles)
            {
                Attributes2014 attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.ToArray()[0].Value.ToArray()[0].Value;

                if (attr.Phrases != null)
                {
                    if (data.SongInfo == null)
                    {
                        // Fill Package Data
                        data.Name = attr.DLCKey;
                        data.Volume = attr.SongVolume;

                        // Fill SongInfo
                        data.SongInfo = new SongInfo();
                        data.SongInfo.SongDisplayName = attr.SongName;
                        data.SongInfo.SongDisplayNameSort = attr.SongNameSort;
                        data.SongInfo.Album = attr.AlbumName;
                        //data.SongInfo.SongYear = Convert.ToInt32(attr.SongYear);
                        data.SongInfo.Artist = attr.ArtistName;
                        data.SongInfo.ArtistSort = attr.ArtistNameSort;
                        data.SongInfo.AverageTempo = (int)attr.SongAverageTempo;
                    }

                    // Adding Tones
                    foreach (var jsonTone in attr.Tones)
                    {
                        if (jsonTone == null) continue;
                        if (!data.TonesRS2014.OfType<Tone2014>().Any(t => t.Key == jsonTone.Key))
                            data.TonesRS2014.Add(jsonTone);
                    }
                }

                // Adding Arrangements
                var xmlName = attr.SongXml.Split(':')[3];
                var aggXml = aggregateData.SongXml.SingleOrDefault(n => n.Name == xmlName);
                var xmlFile = Directory.GetFiles(unpackedDir, xmlName+".xml", SearchOption.AllDirectories)[0];
                if (attr.Phrases != null)
                    data.Arrangements.Add(new Arrangement(attr, xmlFile));
                else
                {   // issue for vocal file
                    var voc = new Arrangement();
                    voc.ArrangementType = ArrangementType.Vocal;
                    voc.SongFile = new SongFile { File = "" };
                    voc.SongXml = new SongXML { File = xmlFile };
                    voc.ScrollSpeed = 20;
                    data.Arrangements.Add(voc);
                }
            }

            //Get Files
            var ddsFiles = Directory.GetFiles(unpackedDir, "*.dds", SearchOption.AllDirectories);
            if (ddsFiles.Length > 0)
                data.AlbumArtPath = ddsFiles[1];

            var sourceAudioFiles = Directory.GetFiles(unpackedDir, "*.wem", SearchOption.AllDirectories);

            var targetAudioFiles = new List<string>();
            foreach (var file in sourceAudioFiles)
            {
                var newFile = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                OggFile.ConvertAudioPlatform(file, newFile);
                targetAudioFiles.Add(newFile);
            }

            if (targetAudioFiles.Count() <= 0)
                throw new InvalidDataException("Audio files not found.");

            string audioPath = null, audioPreviewPath = null;
            FileInfo a = new FileInfo(targetAudioFiles[0]);
            FileInfo b = null;

            if (targetAudioFiles.Count() == 2)
            {
                b = new FileInfo(targetAudioFiles[1]);

                if (a.Length > b.Length)
                {
                    audioPath = a.FullName;
                    audioPreviewPath = b.FullName;
                }
                else
                {
                    audioPath = b.FullName;
                    audioPreviewPath = a.FullName;
                }
            } else
                audioPath = a.FullName;

            data.OggPath = audioPath;
            
            //Make Audio preview with expected name when rebuild
            if (!String.IsNullOrEmpty(audioPreviewPath))
            {
                var newPreviewFileName = Path.Combine(Path.GetDirectoryName(audioPath), String.Format("{0}_preview{1}", Path.GetFileNameWithoutExtension(audioPath), Path.GetExtension(audioPath)));
                File.Move(audioPreviewPath, newPreviewFileName);
                data.OggPreviewPath = newPreviewFileName;
            }

            //Build
            RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(targetFileName, data, new Platform(targetPlatform.platform, GameVersion.RS2014));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Properties;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class DLCPackageCreator
    {
        public static void Generate(string packagePath, DLCPackageData info)
        {
            using (var packPsarcStream = new MemoryStream())
            {
                GeneratePackagePsarc(packPsarcStream, info.AppId, info.Name, info.SongInfo, info.AlbumArtPath, info.OggPath, info.Arrangements, info.Tones);
                using (var fl = File.Create(packagePath))
                    RijndaelEncryptor.Encrypt(packPsarcStream, fl, RijndaelEncryptor.DLCKey);
            }
        }

        private static void GeneratePackagePsarc(Stream output, string appId, string dlcName, SongInfo songInfo, string albumArtPath, string oggPath, IList<Arrangement> arrangements, IList<Tone.Tone> tones)
        {
            IList<Stream> toneStreams = new List<Stream>();
            using (var appIdStream = new MemoryStream())
            using (var packageListStream = new MemoryStream())
            using (var songPsarcStream = new MemoryStream())
            {
                try
                {
                    var packPsarc = new PSARC.PSARC();
                    var packageListWriter = new StreamWriter(packageListStream);

                    GenerateAppId(appIdStream, appId);
                    packPsarc.AddEntry("APP_ID", appIdStream);

                    packageListWriter.WriteLine(dlcName);

                    GenerateSongPsarc(songPsarcStream, dlcName, songInfo, albumArtPath, oggPath, arrangements);
                    packPsarc.AddEntry(String.Format("{0}.psarc", dlcName), songPsarcStream);

                    foreach (var tone in tones)
                    {
                        var tonePsarcStream = new MemoryStream();
                        toneStreams.Add(tonePsarcStream);

                        var toneKey = dlcName + "_" + tone.Name == null ? "Default" : tone.Name.Replace(' ', '_');

                        GenerateTonePsarc(tonePsarcStream, toneKey, tone);
                        packPsarc.AddEntry(String.Format("DLC_Tone_{0}.psarc", toneKey), tonePsarcStream);
                        packageListWriter.WriteLine("DLC_Tone_{0}", toneKey);
                    }

                    packageListWriter.Flush();
                    packageListStream.Seek(0, SeekOrigin.Begin);
                    packPsarc.AddEntry("PackageList.txt", packageListStream);

                    packPsarc.Write(output);
                    output.Flush();
                    output.Seek(0, SeekOrigin.Begin);
                }
                finally
                {
                    foreach (var stream in toneStreams)
                    {
                        try
                        {
                            stream.Dispose();
                        }
                        catch { }
                    }
                }
            }
        }

        private static void GenerateAppId(Stream output, string appId)
        {
            var writer = new StreamWriter(output);
            writer.Write(appId??"206113");
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GeneratePackageList(Stream output, string dlcName)
        {
            var writer = new StreamWriter(output);
            writer.WriteLine(dlcName);
            writer.WriteLine("DLC_Tone_{0}", dlcName);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GenerateSongPsarc(Stream output, string dlcName, SongInfo songInfo, string albumArtPath, string oggPath, IList<Arrangement> arrangements)
        {
            var soundBankName = String.Format("Song_{0}", dlcName);
            Stream albumArtStream = null;
            try
            {
                if (File.Exists(albumArtPath))
                {
                    albumArtStream = File.OpenRead(albumArtPath);
                }
                else
                {
                    albumArtStream = new MemoryStream(Resources.albumart);
                }
                using (var aggregateGraphStream = new MemoryStream())
                using (var manifestStream = new MemoryStream())
                using (var xblockStream = new MemoryStream())
                using (var soundbankStream = new MemoryStream())
                using (var packageIdStream = new MemoryStream())
                using (var soundStream = File.OpenRead(oggPath))
                using (var arrangementFiles = new DisposableCollection<Stream>())
                {
                    var manifestBuilder = new ManifestBuilder
                    {
                        AggregateGraph = new AggregateGraph.AggregateGraph
                        {
                            SoundBank = new SoundBank { File = soundBankName + ".bnk" },
                            AlbumArt = new AlbumArt { File = albumArtPath }
                        }
                    };

                    foreach (var x in arrangements)
                    {
                        manifestBuilder.AggregateGraph.SongFiles.Add(x.SongFile);
                        manifestBuilder.AggregateGraph.SongXMLs.Add(x.SongXml);
                    }
                    manifestBuilder.AggregateGraph.XBlock = new XBlockFile { File = dlcName + ".xblock" };
                    manifestBuilder.AggregateGraph.Write(dlcName, aggregateGraphStream);
                    aggregateGraphStream.Flush();
                    aggregateGraphStream.Seek(0, SeekOrigin.Begin);

                    {
                        var manifestData = manifestBuilder.GenerateManifest(dlcName, arrangements, songInfo);
                        var writer = new StreamWriter(manifestStream);
                        writer.Write(manifestData);
                        writer.Flush();
                        manifestStream.Seek(0, SeekOrigin.Begin);
                    }

                    XBlockGenerator.Generate(dlcName, manifestBuilder.Manifest, manifestBuilder.AggregateGraph, xblockStream);
                    xblockStream.Flush();
                    xblockStream.Seek(0, SeekOrigin.Begin);

                    var soundFileName = SoundBankGenerator.GenerateSoundBank(dlcName, soundStream, soundbankStream);
                    soundbankStream.Flush();
                    soundbankStream.Seek(0, SeekOrigin.Begin);

                    GenerateSongPackageId(packageIdStream, dlcName);

                    var songPsarc = new PSARC.PSARC();
                    songPsarc.AddEntry("PACKAGE_ID", packageIdStream);
                    songPsarc.AddEntry("AggregateGraph.nt", aggregateGraphStream);
                    songPsarc.AddEntry("Manifests/songs.manifest.json", manifestStream);
                    songPsarc.AddEntry(String.Format("Exports/Songs/{0}.xblock", dlcName), xblockStream);
                    songPsarc.AddEntry(String.Format("Audio/Windows/{0}.bnk", soundBankName), soundbankStream);
                    songPsarc.AddEntry(String.Format("Audio/Windows/{0}.ogg", soundFileName), soundStream);
                    songPsarc.AddEntry(String.Format("GRAssets/AlbumArt/{0}.dds", manifestBuilder.AggregateGraph.AlbumArt.Name), albumArtStream);

                    foreach (var x in arrangements)
                    {
                        var xmlFile = File.OpenRead(x.SongXml.File);
                        arrangementFiles.Add(xmlFile);
                        var sngFile = File.OpenRead(x.SongFile.File);
                        arrangementFiles.Add(sngFile);
                        songPsarc.AddEntry(String.Format("GR/Behaviors/Songs/{0}.xml", Path.GetFileNameWithoutExtension(x.SongXml.File)), xmlFile);
                        songPsarc.AddEntry(String.Format("GRExports/Generic/{0}.sng", Path.GetFileNameWithoutExtension(x.SongFile.File)), sngFile);
                    }
                    songPsarc.Write(output);
                    output.Flush();
                    output.Seek(0, SeekOrigin.Begin);
                }
            }
            finally
            {
                if (albumArtStream != null)
                {
                    albumArtStream.Dispose();
                }
            }
        }

        private static void GenerateSongPackageId(Stream output, string dlcName)
        {
            var writer = new StreamWriter(output);
            writer.Write(dlcName);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GenerateTonePsarc(Stream output, string toneKey, Tone.Tone tone)
        {
            var tonePsarc = new PSARC.PSARC();

            using (var packageIdStream = new MemoryStream())
            using (var toneManifestStream = new MemoryStream())
            using (var toneXblockStream = new MemoryStream())
            using (var toneAggregateGraphStream = new MemoryStream())
            {
                ToneGenerator.Generate(toneKey, tone, toneManifestStream, toneXblockStream, toneAggregateGraphStream);
                GenerateTonePackageId(packageIdStream, toneKey);

                tonePsarc.AddEntry(String.Format("Exports/Pedals/DLC_Tone_{0}.xblock", toneKey), toneXblockStream);
                tonePsarc.AddEntry("Manifests/tone.manifest.json", toneManifestStream);
                tonePsarc.AddEntry("AggregateGraph.nt", toneAggregateGraphStream);
                tonePsarc.AddEntry("PACKAGE_ID", packageIdStream);
                tonePsarc.Write(output);
            }
        }

        private static void GenerateTonePackageId(Stream output, string toneKey)
        {
            var writer = new StreamWriter(output);
            writer.WriteLine("DLC_Tone_{0}", toneKey);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }
    }
}

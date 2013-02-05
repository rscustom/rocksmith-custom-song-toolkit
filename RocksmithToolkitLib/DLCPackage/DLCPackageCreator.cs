using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.Sng;
using X360.Other;
using X360.STFS;
using X360.IO;
using RocksmithToolkitLib.Ogg;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class DLCPackageCreator
    {
        private static readonly string xboxWorkDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "xboxpackage");

        private static readonly string[] PCPaths = { "Windows", "Generic" };
        private static readonly string[] XBox360Paths = { "XBox360", "XBox360" };
        private static string[] GetPathName(this GamePlatform platform)
        {
            switch (platform)
            {
                case GamePlatform.Pc:
                    return PCPaths;
                case GamePlatform.XBox360:
                    return XBox360Paths;
                case GamePlatform.PS3:
                    throw new InvalidOperationException("PS3 platform is not supported at this time :(");
                default:
                    throw new InvalidOperationException("Unexpected game platform value");
            }
        }

        private static string GetOgg(this GamePlatform platform, DLCPackageData info) {
            switch (platform) {
                case GamePlatform.Pc:
                    return info.OggPath;
                case GamePlatform.XBox360:
                    return info.OggXBox360Path;
                case GamePlatform.PS3:
                    throw new InvalidOperationException("PS3 platform is not supported at this time :(");
                default:
                    throw new InvalidOperationException("Unexpected game platform value");
            }
        }

        private static List<string> XBox360Files = new List<string>();

        public static void Generate(string packagePath, DLCPackageData info, GamePlatform platform)
        {
            if (platform == GamePlatform.XBox360) {
                if (!Directory.Exists(xboxWorkDir))
                    Directory.CreateDirectory(xboxWorkDir);
            }

            using (var packPsarcStream = new MemoryStream())
            {
                GeneratePackagePsarc(packPsarcStream, info.AppId, info.Name, info.SongInfo, info.AlbumArtPath, platform.GetOgg(info), info.Arrangements, info.Tones, platform);
                switch (platform) {
                    case GamePlatform.Pc:
                        using (var fl = File.Create(packagePath))
                            RijndaelEncryptor.Encrypt(packPsarcStream, fl, RijndaelEncryptor.DLCKey);
                        break;
                    case GamePlatform.XBox360:
                        BuildXBox360Package(packagePath, info);
                        break;
                    case GamePlatform.PS3:
                        throw new InvalidOperationException("PS3 platform is not supported at this time :(");
                }                
            }

            try {
                if (!Directory.Exists(xboxWorkDir))
                    Directory.Delete(xboxWorkDir);
            } catch {}
        }

        #region XBox360

        private static void BuildXBox360Package(string packagePath, DLCPackageData info)
        {
            LogRecord x = new LogRecord();
            RSAParams xboxRSA = new RSAParams(new DJsIO(Resources.XBox360_KV, true));
            CreateSTFS xboxSTFS = new CreateSTFS();
            xboxSTFS.HeaderData = info.GetSTFSHeader();
            foreach (string file in XBox360Files)
                xboxSTFS.AddFile(file, Path.GetFileName(file));
            STFSPackage xboxPackage = new STFSPackage(xboxSTFS, xboxRSA, packagePath, x);
            if (!xboxPackage.RebuildPackage(xboxRSA))
                throw new InvalidOperationException("Error on create XBox360 package, details: \n\r" + x.Log);
        }

        private static HeaderData GetSTFSHeader(this DLCPackageData dlcData) {
            HeaderData hd = new HeaderData();
            string displayName = String.Format("{0} by {1}", dlcData.SongInfo.SongDisplayName, dlcData.SongInfo.Artist);
            hd.Title_Package = "Rocksmith";
            hd.Publisher = String.Format("Custom Song Creator Toolkit (v{0}.{1}.{2} beta)",
                                        Assembly.GetExecutingAssembly().GetName().Version.Major,
                                        Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                        Assembly.GetExecutingAssembly().GetName().Version.Build);
            hd.Title_Display = displayName;
            hd.Description = displayName;
            hd.ThisType = PackageType.MarketPlace;
            hd.PackageImageBinary = Resources.XBox360_DLC_image.ImageToBytes(ImageFormat.Png);;
            hd.ContentImageBinary = hd.PackageImageBinary;
            hd.IDTransfer = TransferLock.AllowTransfer;
            return hd;
        }

        public static byte[] ImageToBytes(this Image image, ImageFormat format)
        {
            byte[] xReturn = null;
            using (MemoryStream xMS = new MemoryStream())
            {
                image.Save(xMS, format);
                xReturn = xMS.ToArray();
                xMS.Dispose();
            }
            return xReturn;
        }

        private static void WriteTmpFile(this Stream ms, string fileName)
        {
            string filePath = Path.Combine(xboxWorkDir, fileName);

            FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);
            file.Write(bytes, 0, bytes.Length);
            file.Close();
            
            XBox360Files.Add(filePath);
        }

        #endregion

        private static void GeneratePackagePsarc(Stream output, string appId, string dlcName, SongInfo songInfo, string albumArtPath, string oggPath, IList<Arrangement> arrangements, IList<Tone.Tone> tones, GamePlatform platform)
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

                    if (platform == GamePlatform.Pc)
                    {
                        GenerateAppId(appIdStream, appId);
                        packPsarc.AddEntry("APP_ID", appIdStream);
                    }

                    packageListWriter.WriteLine(dlcName);

                    GenerateSongPsarc(songPsarcStream, dlcName, songInfo, albumArtPath, oggPath, arrangements, platform);
                    string songFileName = String.Format("{0}.psarc", dlcName);
                    packPsarc.AddEntry(songFileName, songPsarcStream);
                    if (platform == GamePlatform.XBox360) songPsarcStream.WriteTmpFile(songFileName);

                    foreach (var tone in tones)
                    {
                        var tonePsarcStream = new MemoryStream();
                        toneStreams.Add(tonePsarcStream);

                        var toneKey = dlcName + "_" + tone.Name == null ? "Default" : tone.Name.Replace(' ', '_');

                        GenerateTonePsarc(tonePsarcStream, toneKey, tone);
                        string toneEntry = String.Format("DLC_Tone_{0}.psarc", toneKey);
                        packPsarc.AddEntry(toneEntry, tonePsarcStream);
                        if (platform == GamePlatform.XBox360) tonePsarcStream.WriteTmpFile(toneEntry);
                        packageListWriter.WriteLine("DLC_Tone_{0}", toneKey);
                    }

                    packageListWriter.Flush();
                    packageListStream.Seek(0, SeekOrigin.Begin);
                    string packageList = "PackageList.txt";
                    packPsarc.AddEntry(packageList, packageListStream);
                    if (platform == GamePlatform.XBox360) packageListStream.WriteTmpFile(packageList);

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

        private static void GenerateSongPsarc(Stream output, string dlcName, SongInfo songInfo, string albumArtPath, string oggPath, IList<Arrangement> arrangements, GamePlatform platform)
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
                using (var soundStream = OggFile.ConvertOgg(oggPath))
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
                        //Generate sng file in execution time
                        string sngFile = Path.Combine(Path.GetDirectoryName(x.SongXml.File), x.SongXml.Name + ".sng");
                        InstrumentTuning tuning = InstrumentTuning.Standard;
                        Enum.TryParse<InstrumentTuning>(x.Tuning, true, out tuning);
                        SngFileWriter.Write(x.SongXml.File, sngFile, x.ArrangementType, platform, tuning);
                        if (x.SongFile == null)
                            x.SongFile = new SongFile();
                        x.SongFile.File = sngFile;
                        //end
                        manifestBuilder.AggregateGraph.SongFiles.Add(x.SongFile);
                        manifestBuilder.AggregateGraph.SongXMLs.Add(x.SongXml);
                    }
                    manifestBuilder.AggregateGraph.XBlock = new XBlockFile { File = dlcName + ".xblock" };
                    manifestBuilder.AggregateGraph.Write(dlcName, platform.GetPathName(), aggregateGraphStream);
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

                    var soundFileName = SoundBankGenerator.GenerateSoundBank(dlcName, soundStream, soundbankStream, platform);
                    soundbankStream.Flush();
                    soundbankStream.Seek(0, SeekOrigin.Begin);

                    GenerateSongPackageId(packageIdStream, dlcName);

                    var songPsarc = new PSARC.PSARC();
                    songPsarc.AddEntry("PACKAGE_ID", packageIdStream);
                    songPsarc.AddEntry("AggregateGraph.nt", aggregateGraphStream);
                    songPsarc.AddEntry("Manifests/songs.manifest.json", manifestStream);
                    songPsarc.AddEntry(String.Format("Exports/Songs/{0}.xblock", dlcName), xblockStream);
                    songPsarc.AddEntry(String.Format("Audio/{0}/{1}.bnk", platform.GetPathName()[0], soundBankName), soundbankStream);
                    songPsarc.AddEntry(String.Format("Audio/{0}/{1}.ogg", platform.GetPathName()[0], soundFileName), soundStream);
                    songPsarc.AddEntry(String.Format("GRAssets/AlbumArt/{0}.dds", manifestBuilder.AggregateGraph.AlbumArt.Name), albumArtStream);

                    foreach (var x in arrangements)
                    {
                        var xmlFile = File.OpenRead(x.SongXml.File);
                        arrangementFiles.Add(xmlFile);
                        var sngFile = File.OpenRead(x.SongFile.File);
                        arrangementFiles.Add(sngFile);
                        songPsarc.AddEntry(String.Format("GR/Behaviors/Songs/{0}.xml", Path.GetFileNameWithoutExtension(x.SongXml.File)), xmlFile);
                        songPsarc.AddEntry(String.Format("GRExports/{0}/{1}.sng", platform.GetPathName()[1], Path.GetFileNameWithoutExtension(x.SongFile.File)), sngFile);
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
                output.Flush();
                output.Seek(0, SeekOrigin.Begin);
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

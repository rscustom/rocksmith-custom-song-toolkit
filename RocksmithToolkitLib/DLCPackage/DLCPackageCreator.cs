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
using System.Diagnostics;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class DLCPackageCreator
    {
        private static readonly string xboxWorkDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "xboxpackage");
        private static readonly string ps3WorkDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "edat");

        private static readonly string[] PCPaths = { "Windows", "Generic" };
        private static readonly string[] XBox360Paths = { "XBox360", "XBox360" };
        private static readonly string[] PS3Paths = { "PS3", "PS3" };

        public static string[] GetPathName(this GamePlatform platform)
        {
            switch (platform)
            {
                case GamePlatform.Pc:
                    return PCPaths;
                case GamePlatform.XBox360:
                    return XBox360Paths;
                case GamePlatform.PS3:
                    return PS3Paths;
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
                    return info.OggPS3Path;
                default:
                    throw new InvalidOperationException("Unexpected game platform value");
            }
        }

        private static List<string> XBox360Files = new List<string>();
        private static List<string> PS3Files = new List<string>();
        private static List<string> SNGTmpFiles = new List<string>();

        public static void Generate(string packagePath, DLCPackageData info, GamePlatform platform, PackageMagic? xboxPackageType)
        {
            switch (platform)
            {
                case GamePlatform.XBox360:
                    if (!Directory.Exists(xboxWorkDir))
                        Directory.CreateDirectory(xboxWorkDir);
                    break;
                case GamePlatform.PS3:
                    if (!Directory.Exists(ps3WorkDir))
                        Directory.CreateDirectory(ps3WorkDir);
                    break;
                }

            using (var packPsarcStream = new MemoryStream())
            {
                GeneratePackagePsarc(packPsarcStream, info, platform);
                switch (platform) {
                    case GamePlatform.Pc:
                        using (var fl = File.Create(packagePath))
                            RijndaelEncryptor.Encrypt(packPsarcStream, fl, RijndaelEncryptor.DLCKey);
                        break;
                    case GamePlatform.XBox360:
                        BuildXBox360Package(packagePath, info, XBox360Files, xboxPackageType);
                        break;
                    case GamePlatform.PS3:
                        EncryptPS3EdatFiles(packagePath);
                        break;
                }                
            }

            try {
                foreach (var sngTmpFile in SNGTmpFiles)
                {
                    if (File.Exists(sngTmpFile))
                        File.Delete(sngTmpFile);
                }
            } catch { /*Have no problem if don't delete*/ }

            XBox360Files.Clear();
            PS3Files.Clear();
            SNGTmpFiles.Clear();
        }

        #region XBox360

        public static void BuildXBox360Package(string packagePath, DLCPackageData info, IEnumerable<string> xboxFiles, PackageMagic? xboxPackageType)
        {
            LogRecord x = new LogRecord();
            RSAParams xboxRSA = xboxPackageType == PackageMagic.CON ? new RSAParams(new DJsIO(Resources.XBox360_KV, true)) : new RSAParams(StrongSigned.LIVE);
            CreateSTFS xboxSTFS = new CreateSTFS();
            xboxSTFS.HeaderData = info.GetSTFSHeader();
            foreach (string file in xboxFiles)
                xboxSTFS.AddFile(file, Path.GetFileName(file));

            STFSPackage xboxPackage = new STFSPackage(xboxSTFS, xboxRSA, packagePath, x);
            var generated = xboxPackage.RebuildPackage(xboxRSA);
            if (!generated)
                throw new InvalidOperationException("Error on create XBox360 package, details: \n\r" + x.Log);

            xboxPackage.FlushPackage(xboxRSA);
            xboxPackage.CloseIO();

            try
            {
                if (Directory.Exists(xboxWorkDir))
                    Directory.Delete(xboxWorkDir, true);
            }
            catch { /*Have no problem if don't delete*/ }
        }

        private static HeaderData GetSTFSHeader(this DLCPackageData dlcData) {
            HeaderData hd = new HeaderData();
            string displayName = String.Format("{0} by {1}", dlcData.SongInfo.SongDisplayName, dlcData.SongInfo.Artist);
            hd.Title_Package = "Rocksmith";
            hd.TitleID = 1431505011; //55530873 in HEXA
            hd.Publisher = String.Format("Custom Song Creator Toolkit (v{0}.{1}.{2}.{3} beta)",
                                        Assembly.GetExecutingAssembly().GetName().Version.Major,
                                        Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                        Assembly.GetExecutingAssembly().GetName().Version.Build,
                                        Assembly.GetExecutingAssembly().GetName().Version.Revision);
            hd.Title_Display = displayName;
            hd.Description = displayName;
            hd.ThisType = PackageType.MarketPlace;
            hd.PackageImageBinary = Resources.XBox360_DLC_image.ImageToBytes(ImageFormat.Png);;
            hd.ContentImageBinary = hd.PackageImageBinary;
            hd.IDTransfer = TransferLock.AllowTransfer;
            if (dlcData.SignatureType == PackageMagic.LIVE)
                foreach (var license in dlcData.XBox360Licenses)
                    hd.AddLicense(license.ID, license.Bit, license.Flag);

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

        private static void WriteTmpFile(this Stream ms, string fileName, GamePlatform platform)
        {
            if (platform == GamePlatform.XBox360 || platform == GamePlatform.PS3)
            {
                string workDir = platform == GamePlatform.XBox360 ? xboxWorkDir : ps3WorkDir;
                string filePath = Path.Combine(workDir, fileName);

                FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                byte[] bytes = new byte[ms.Length];
                ms.Read(bytes, 0, (int)ms.Length);
                file.Write(bytes, 0, bytes.Length);
                file.Close();

                switch (platform)
                {
                    case GamePlatform.XBox360:
                        XBox360Files.Add(filePath);
                        break;
                    case GamePlatform.PS3:
                        PS3Files.Add(filePath);
                        break;
                }
            }
        }

        #endregion

        #region PS3

        public static void EncryptPS3EdatFiles(string packagesPath)
        {
            // Packing using TruAncestor Edat
            string toolkitPath = Path.GetDirectoryName(Application.ExecutablePath);
            string rebuilderApp = Path.Combine(toolkitPath, "rebuilder.cmd");
            
            Process PS3Process = new Process();
            PS3Process.StartInfo.FileName = rebuilderApp;
            PS3Process.StartInfo.WorkingDirectory = toolkitPath;
            PS3Process.StartInfo.UseShellExecute = false;
            PS3Process.StartInfo.CreateNoWindow = true;
            PS3Process.StartInfo.RedirectStandardOutput = true;

            PS3Process.Start();
            PS3Process.WaitForExit();

            string rebuilderResult = PS3Process.StandardOutput.ReadToEnd();

            // Delete .psarc files
            foreach (var ps3File in PS3Files)
            {
                if (File.Exists(ps3File))
                    File.Delete(ps3File);
            }

            // Move directory to user selected path
            if (Directory.Exists(ps3WorkDir))
                DirectoryExtension.Move(ps3WorkDir, packagesPath);

            if (rebuilderResult.IndexOf("Encrypt all EDAT files successfully") < 0)
                throw new InvalidOperationException("Rebuilder error, please check if .edat files are created correctly and see output bellow:" + Environment.NewLine + Environment.NewLine + rebuilderResult);
        }

        #endregion

        private static void GeneratePackagePsarc(Stream output, DLCPackageData info, GamePlatform platform)
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
                        GenerateAppId(appIdStream, info.AppId);
                        packPsarc.AddEntry("APP_ID", appIdStream);
                    }

                    packageListWriter.WriteLine(info.Name);

                    GenerateSongPsarc(songPsarcStream, info, platform);
                    string songFileName = String.Format("{0}.psarc", info.Name);
                    packPsarc.AddEntry(songFileName, songPsarcStream);
                    songPsarcStream.WriteTmpFile(songFileName, platform);

                    for (int i = 0; i < info.Tones.Count; i++)
                    {
                        var tone = info.Tones[i];
                        var tonePsarcStream = new MemoryStream();
                        toneStreams.Add(tonePsarcStream);

                        var toneKey = info.Name + "_" + tone.Name == null ? "Default" : tone.Name.Replace(' ', '_');

                        GenerateTonePsarc(tonePsarcStream, toneKey, tone);
                        string toneEntry = String.Format("DLC_Tone_{0}.psarc", toneKey);
                        packPsarc.AddEntry(toneEntry, tonePsarcStream);
                        tonePsarcStream.WriteTmpFile(toneEntry, platform);
                        if (i + 1 != info.Tones.Count)
                            packageListWriter.WriteLine("DLC_Tone_{0}", toneKey);
                        else
                            packageListWriter.Write("DLC_Tone_{0}", toneKey);
                    }

                    packageListWriter.Flush();
                    packageListStream.Seek(0, SeekOrigin.Begin);
                    if (platform != GamePlatform.PS3)
                    {
                        string packageList = "PackageList.txt";
                        packPsarc.AddEntry(packageList, packageListStream);
                        packageListStream.WriteTmpFile(packageList, platform);
                    }
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

        private static void GenerateSongPsarc(Stream output, DLCPackageData info, GamePlatform platform)
        {
            var soundBankName = String.Format("Song_{0}", info.Name);
            Stream albumArtStream = null;
            try
            {
                if (File.Exists(info.AlbumArtPath))
                {
                    albumArtStream = File.OpenRead(info.AlbumArtPath);
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
                using (var soundStream = OggFile.ConvertOgg(platform.GetOgg(info)))
                using (var arrangementFiles = new DisposableCollection<Stream>())
                {
                    var manifestBuilder = new ManifestBuilder
                    {
                        AggregateGraph = new AggregateGraph.AggregateGraph
                        {
                            SoundBank = new SoundBank { File = soundBankName + ".bnk" },
                            AlbumArt = new AlbumArt { File = info.AlbumArtPath }
                        }
                    };

                    foreach (var x in info.Arrangements)
                    {
                        //Generate sng file in execution time
                        GenerateSNG(x, platform);
                        
                        manifestBuilder.AggregateGraph.SongFiles.Add(x.SongFile);
                        manifestBuilder.AggregateGraph.SongXMLs.Add(x.SongXml);
                    }
                    manifestBuilder.AggregateGraph.XBlock = new XBlockFile { File = info.Name + ".xblock" };
                    manifestBuilder.AggregateGraph.Write(info.Name, platform.GetPathName(), platform, aggregateGraphStream);
                    aggregateGraphStream.Flush();
                    aggregateGraphStream.Seek(0, SeekOrigin.Begin);

                    {
                        var manifestData = manifestBuilder.GenerateManifest(info.Name, info.Arrangements, info.SongInfo, platform);
                        var writer = new StreamWriter(manifestStream);
                        writer.Write(manifestData);
                        writer.Flush();
                        manifestStream.Seek(0, SeekOrigin.Begin);
                    }

                    XBlockGenerator.Generate(info.Name, manifestBuilder.Manifest, manifestBuilder.AggregateGraph, xblockStream);
                    xblockStream.Flush();
                    xblockStream.Seek(0, SeekOrigin.Begin);

                    var soundFileName = SoundBankGenerator.GenerateSoundBank(info.Name, soundStream, soundbankStream, info.Volume, platform);
                    soundbankStream.Flush();
                    soundbankStream.Seek(0, SeekOrigin.Begin);

                    GenerateSongPackageId(packageIdStream, info.Name);

                    var songPsarc = new PSARC.PSARC();
                    songPsarc.AddEntry("PACKAGE_ID", packageIdStream);
                    songPsarc.AddEntry("AggregateGraph.nt", aggregateGraphStream);
                    songPsarc.AddEntry("Manifests/songs.manifest.json", manifestStream);
                    songPsarc.AddEntry(String.Format("Exports/Songs/{0}.xblock", info.Name), xblockStream);
                    songPsarc.AddEntry(String.Format("Audio/{0}/{1}.bnk", platform.GetPathName()[0], soundBankName), soundbankStream);
                    songPsarc.AddEntry(String.Format("Audio/{0}/{1}.ogg", platform.GetPathName()[0], soundFileName), soundStream);
                    songPsarc.AddEntry(String.Format("GRAssets/AlbumArt/{0}.dds", manifestBuilder.AggregateGraph.AlbumArt.Name), albumArtStream);

                    foreach (var x in info.Arrangements)
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
                var x = (from pedal in tone.PedalList
                         where pedal.Value.PedalKey.ToLower().Contains("bass")
                         select pedal).Count();
                tonePsarc.AddEntry(x > 0 ? "Manifests/tone_bass.manifest.json" : "Manifests/tone.manifest.json", toneManifestStream);
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
            writer.Write("DLC_Tone_{0}", toneKey);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        public static void GenerateSNG(Arrangement arrangement, GamePlatform platform) {
            string sngFile = Path.Combine(Path.GetDirectoryName(arrangement.SongXml.File), arrangement.SongXml.Name + ".sng");
            InstrumentTuning tuning = InstrumentTuning.Standard;
            Enum.TryParse<InstrumentTuning>(arrangement.Tuning, true, out tuning);
            SngFileWriter.Write(arrangement.SongXml.File, sngFile, arrangement.ArrangementType, platform, tuning);
            if (arrangement.SongFile == null)
                arrangement.SongFile = new SongFile();
            arrangement.SongFile.File = sngFile;
            SNGTmpFiles.Add(sngFile);
        }
    }
}

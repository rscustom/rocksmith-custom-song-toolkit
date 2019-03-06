using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Header;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.XmlRepository;
using X360.IO;
using X360.Other;
using X360.STFS;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.XBlock;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.Ogg;
using Tone = RocksmithToolkitLib.DLCPackage.Manifest.Tone.Tone;
using RocksmithToolkitLib.PsarcLoader;
using System.Diagnostics;


namespace RocksmithToolkitLib.DLCPackage
{
    public enum DLCPackageType { Song = 0, Lesson = 1, Inlay = 2 }
    public enum ModType { Custom_Guitar_Inlays = 0, Custom_Intro_Screens = 1 }

    public static class DLCPackageCreator
    {
        #region CONSTANT

        // path fixed for unit testing compatiblity
        private static readonly string XBOX_WORKDIR = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "xboxpackage");
        // path fixed for unit testing compatiblity
        private static readonly string PS3_WORKDIR = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "edat");

        private static readonly string[] PATH_PC = { "Windows", "Generic", "_p" };
        private static readonly string[] PATH_MAC = { "Mac", "MacOS", "_m" };
        private static readonly string[] PATH_XBOX = { "XBox360", "XBox360", "_xbox" };
        private static readonly string[] PATH_PS3 = { "PS3", "PS3", "_ps3" };

        private static List<string> FILES_XBOX = new List<string>();
        private static List<string> FILES_PS3 = new List<string>();
        private static List<string> TMPFILES_SNG = new List<string>();
        private static List<string> TMPFILES_ART = new List<string>();
        private static PSARC.PSARC packPsarc;
        private static string dlcName;

        private static void DeleteTmpFiles(List<string> files)
        {
            try
            {
                foreach (var TmpFile in files)
                {
                    if (File.Exists(TmpFile)) File.Delete(TmpFile);
                }
            }
            catch { /*Have no problem if don't delete*/ }
            files.Clear();
        }

        #endregion

        #region FUNCTIONS

        public static string[] GetPathName(this Platform platform)
        {
            return platform.platform.GetPathName();
        }

        public static string[] GetPathName(this GamePlatform gPlatform)
        {
            switch (gPlatform)
            {
                case GamePlatform.Pc:
                    return PATH_PC;
                case GamePlatform.Mac:
                    return PATH_MAC;
                case GamePlatform.XBox360:
                    return PATH_XBOX;
                case GamePlatform.PS3:
                    return PATH_PS3;
                default:
                    throw new InvalidOperationException("Unexpected game platform value");
            }
        }

        #endregion

        #region PACKAGE

        /// <summary>
        /// Generate package archive(s) (Song RS1 and RS2014, Lesson or Inlay) from DLCPackageData
        /// </summary>
        /// <param name="destPath">Archive destination path and file name with/or without extension</param>
        /// <param name="info">DLCPackageData</param>
        /// <param name="platform">Target Platform.</param>
        /// <param name="dlcType">Package Type (Song, Lesson, or Inlay)</param>
        /// <param name="pnum">Packages remaining to generate (used to control art cache)</param>
        public static string Generate(string destPath, DLCPackageData info, Platform platform, DLCPackageType dlcType = DLCPackageType.Song, int pnum = -1)
        {
            switch (platform.platform)
            {
                case GamePlatform.XBox360:
                    if (!Directory.Exists(XBOX_WORKDIR))
                        Directory.CreateDirectory(XBOX_WORKDIR);
                    break;
                case GamePlatform.PS3:
                    if (!Directory.Exists(PS3_WORKDIR))
                        Directory.CreateDirectory(PS3_WORKDIR);
                    break;
            }

            var packageName = Path.GetFileNameWithoutExtension(destPath).StripPlatformEndName();
            var archivePath = Path.Combine(Path.GetDirectoryName(destPath), packageName);

            if (platform.version == GameVersion.RS2014)
                archivePath += platform.GetPathName()[2];

            using (var packPsarcStream = new MemoryStream())
            {
                switch (platform.version)
                {
                    case GameVersion.RS2014:
                        switch (dlcType)
                        {
                            case DLCPackageType.Song:
                                GenerateRS2014SongPsarc(packPsarcStream, info, platform, pnum);
                                break;
                            case DLCPackageType.Lesson:
                                throw new NotImplementedException("Lesson package type not implemented yet :(");
                            case DLCPackageType.Inlay:
                                GenerateRS2014InlayPsarc(packPsarcStream, info, platform);
                                break;
                        }
                        break;
                    case GameVersion.RS2012:
                        GenerateRS1Psarcs(packPsarcStream, info, platform);
                        break;
                    case GameVersion.None:
                        throw new InvalidOperationException("Unexpected game version value");
                }

                // SAVE PACKAGE
                switch (platform.platform)
                {
                    case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        switch (platform.version)
                        {
                            case GameVersion.RS2014:
                                if (!archivePath.EndsWith(".psarc"))
                                    archivePath += ".psarc";

                                using (var fl = File.Create(archivePath))
                                    packPsarcStream.CopyTo(fl);
                                break;
                            case GameVersion.RS2012:
                                if (!archivePath.EndsWith(".dat"))
                                    archivePath += ".dat";

                                using (var fl = File.Create(archivePath))
                                    RijndaelEncryptor.EncryptFile(packPsarcStream, fl, RijndaelEncryptor.DLCKey);

                                break;
                            default:
                                throw new InvalidOperationException("Unexpected game version value");
                        }
                        break;
                    case GamePlatform.XBox360:
                        if (!archivePath.EndsWith("_xbox"))
                            archivePath += "_xbox";

                        archivePath = BuildXBox360Package(archivePath, info, FILES_XBOX, platform.version, dlcType);
                        break;
                    case GamePlatform.PS3:
                        if (!archivePath.EndsWith(".psarc"))
                            archivePath += ".psarc";

                        archivePath = EncryptPS3EdatFiles(archivePath, platform);
                        break;
                }
            }

            if (packPsarc != null)
            {
                packPsarc.Dispose();
                packPsarc = null;
            }

            FILES_XBOX.Clear();
            FILES_PS3.Clear();
            DeleteTmpFiles(TMPFILES_SNG);

            if (pnum <= 1)// doesn't trigger for last one, should be ? == 1 when last package generated.
                DeleteTmpFiles(TMPFILES_ART);

            return archivePath;
        }

        #region XBox360

        public static string BuildXBox360Package(string destPath, DLCPackageData info, IEnumerable<string> xboxFiles, GameVersion gameVersion, DLCPackageType dlcType = DLCPackageType.Song)
        {
            LogRecord x = new LogRecord();
            RSAParams xboxRSA = info.SignatureType == PackageMagic.CON ? new RSAParams(new DJsIO(Resources.XBox360_KV, true)) : new RSAParams(StrongSigned.LIVE);
            CreateSTFS xboxSTFS = new CreateSTFS();
            xboxSTFS.HeaderData = info.GetSTFSHeader(gameVersion, dlcType);
            foreach (string file in xboxFiles)
                xboxSTFS.AddFile(file, Path.GetFileName(file));

            STFSPackage xboxPackage = new STFSPackage(xboxSTFS, xboxRSA, destPath, x);
            var generated = xboxPackage.RebuildPackage(xboxRSA);
            if (!generated)
                throw new InvalidOperationException("Error on create XBox360 package, details: \n" + x.Log);

            xboxPackage.FlushPackage(xboxRSA);
            xboxPackage.CloseIO();

            IOExtension.DeleteDirectory(XBOX_WORKDIR);

            if (File.Exists(destPath))
                return destPath;

            return String.Empty;
        }

        private static HeaderData GetSTFSHeader(this DLCPackageData info, GameVersion gameVersion, DLCPackageType dlcType)
        {
            HeaderData hd = new HeaderData();

            string displayName = "Custom Package";
            switch (dlcType)
            {
                case DLCPackageType.Song:
                    displayName = String.Format("{0} by {1}", info.SongInfo.SongDisplayName, info.SongInfo.Artist);
                    break;
                case DLCPackageType.Lesson:
                    throw new NotImplementedException("Lesson package type not implemented yet :(");
                case DLCPackageType.Inlay:
                    displayName = "Custom Inlay by Song Creator";
                    break;
            }

            switch (gameVersion)
            {
                case GameVersion.RS2012:
                    hd.Title_Package = "Rocksmith";
                    hd.TitleID = 1431505011; //55530873 in HEXA for RS1
                    hd.PackageImageBinary = Resources.XBox360_DLC_image;
                    break;
                case GameVersion.RS2014:
                    hd.Title_Package = "Rocksmith 2014";
                    hd.TitleID = 1431505088; //555308C0 in HEXA for RS2014
                    hd.PackageImageBinary = Resources.XBox360_DLC_image2014;
                    break;
            }

            hd.Publisher = String.Format("Song Creator Toolkit for Rocksmith ({0} beta)", ToolkitVersion.RSTKGuiVersion);
            hd.Title_Display = displayName;
            hd.Description = displayName;
            hd.ThisType = PackageType.MarketPlace;
            hd.ContentImageBinary = hd.PackageImageBinary;
            hd.IDTransfer = TransferLock.AllowTransfer;
            if (info.SignatureType == PackageMagic.LIVE)
                foreach (var license in info.XBox360Licenses)
                    hd.AddLicense(license.ID, license.Bit, license.Flag);

            return hd;
        }

        #endregion

        #region PS3

        // TODO: elimate redundancy
        // this method is almost the same as as Packer.PackPS3 method
        public static string EncryptPS3EdatFiles(string srcPath, Platform platform)
        {
            // source file must be in "/edat" folder in application root directory
            var destPath = String.Empty;

            // Due to PS3 encryption limitation - replace spaces in fname with '_'
            if (Path.GetFileName(srcPath).Contains(" "))
                srcPath = Path.Combine(Path.GetDirectoryName(srcPath), Path.GetFileName(srcPath).Replace(" ", "_"));

            // Cleaning work dir, beware there is .psarc that we need.
            var junkFiles = Directory.EnumerateFiles(PS3_WORKDIR, "*.*").Where(e => !e.EndsWith(".psarc"));
            foreach (var junk in junkFiles)
                File.Delete(junk);

            if (platform.version == GameVersion.RS2014)
            {
                // Have only one file for RS2014 package, so can be rename that the user defined
                if (FILES_PS3.Count == 1)
                    if (File.Exists(FILES_PS3[0]))
                    {
                        var oldName = FILES_PS3[0].Clone().ToString();
                        FILES_PS3[0] = Path.Combine(Path.GetDirectoryName(FILES_PS3[0]), Path.GetFileName(srcPath));

                        if (File.Exists(FILES_PS3[0]))
                            File.Delete(FILES_PS3[0]);
                        File.Move(oldName, FILES_PS3[0]);
                    }
            }

            string encryptResult = RijndaelEncryptor.EncryptPS3Edat();

            // Delete .psarc files
            foreach (var ps3File in FILES_PS3)
                if (File.Exists(ps3File))
                    File.Delete(ps3File);

            // Move directory if RS1 or file in RS2014 to user selected path
            if (platform.version == GameVersion.RS2014)
            {
                var encryptedFile = String.Format("{0}.edat", FILES_PS3[0]);
                destPath = String.Format("{0}.edat", srcPath);

                if (File.Exists(destPath))
                    File.Delete(destPath);

                if (File.Exists(encryptedFile))
                    File.Move(encryptedFile, destPath);
            }
            else
            {
                if (Directory.Exists(PS3_WORKDIR))
                    IOExtension.MoveDirectory(PS3_WORKDIR, String.Format("{0}_PS3", srcPath), true);
            }

            if (encryptResult.IndexOf("No JDK or JRE is installed on your machine") > 0)
                throw new InvalidOperationException("You need install Java SE 7 (x86) or higher on your machine. The Java path should be in PATH Environment Variable:" + Environment.NewLine + Environment.NewLine + encryptResult);

            if (encryptResult.IndexOf(Packer.EDAT_MSG) < 0)
                throw new InvalidOperationException("Rebuilder error, please check if .edat files are created correctly and see output bellow:" + Environment.NewLine + Environment.NewLine + encryptResult);

            return destPath;
        }

        #endregion

        #endregion

        #region Generate PSARC RS2014

        private static void GenerateRS2014SongPsarc(Stream output, DLCPackageData info, Platform platform, int pnum = -1)
        {
            // TODO: Benchmark processes and optimize speed
            dlcName = info.Name.ToLower();
            packPsarc = new PSARC.PSARC();

            // Stream objects
            Stream soundStream = null;
            Stream soundPreviewStream = null;
            Stream rsenumerableRootStream = null;
            Stream rsenumerableSongStream = null;

            try
            {
                // ALBUM ART
                if (info.ArtFiles == null)
                {
                    //Try to get spreserved files
                    string d256, d128, d64;
                    if (File.Exists(info.AlbumArtPath))
                    {
                        d256 = info.AlbumArtPath;
                        d128 = d256.Remove(d256.Length - 7) + "128.dds";
                        d64 = d256.Remove(d256.Length - 7) + "64.dds";
                        if (File.Exists(d64) && File.Exists(d128))
                        {
                            var _found = new List<DDSConvertedFile>
                            {
                                new DDSConvertedFile() { sizeX = 64, destinationFile = d64 },
                                new DDSConvertedFile() { sizeX = 128, destinationFile = d128 },
                                new DDSConvertedFile() { sizeX = 256, destinationFile = d256 }
                            };

                            info.ArtFiles = _found;
                        }
                    }
                }

                if (info.ArtFiles == null)
                {
                    //Generate art files
                    string albumArtPath;
                    var ddsfiles = info.ArtFiles;
                    if (File.Exists(info.AlbumArtPath))
                    {
                        albumArtPath = info.AlbumArtPath;
                    }
                    else
                    {
                        using (var albumArtStream = new MemoryStream(Resources.albumart2014_256))
                        {
                            albumArtPath = GeneralExtension.GetTempFileName(".dds");
                            albumArtStream.WriteFile(albumArtPath);
                            TMPFILES_ART.Add(albumArtPath);
                        }
                    }

                    ddsfiles = new List<DDSConvertedFile>
                    {
                        new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = albumArtPath, destinationFile = GeneralExtension.GetTempFileName(".dds") },
                        new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = albumArtPath, destinationFile = GeneralExtension.GetTempFileName(".dds") },
                        new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = albumArtPath, destinationFile = GeneralExtension.GetTempFileName(".dds") }
                    };

                    // Convert to DDS
                    ToDDS(ddsfiles);

                    // Save for reuse
                    info.ArtFiles = ddsfiles;
                }

                foreach (var dds in info.ArtFiles)
                {
                    packPsarc.AddEntry(String.Format("gfxassets/album_art/album_{0}_{1}.dds", dlcName, dds.sizeX), new FileStream(dds.destinationFile, FileMode.Open, FileAccess.Read, FileShare.Read));
                    if (dds.sizeY != 0)
                    {
                        TMPFILES_ART.Add(dds.destinationFile);
                    }
                }

                // Lyric Art Texture
                if (File.Exists(info.LyricArtPath))
                    packPsarc.AddEntry(String.Format("assets/ui/lyrics/{0}/lyrics_{0}.dds", dlcName), new FileStream(info.LyricArtPath, FileMode.Open, FileAccess.Read, FileShare.Read));

                // AUDIO
                var audioFile = info.OggPath;
                if (File.Exists(audioFile))
                    if (platform.IsConsole != audioFile.GetAudioPlatform().IsConsole)
                        soundStream = OggFile.ConvertAudioPlatform(audioFile);
                    else
                        soundStream = File.OpenRead(audioFile);
                else
                    throw new InvalidOperationException(String.Format("Audio file '{0}' not found.", audioFile));

                // AUDIO PREVIEW
                var previewAudioFile = info.OggPreviewPath;
                if (File.Exists(previewAudioFile))
                    if (platform.IsConsole != previewAudioFile.GetAudioPlatform().IsConsole)
                        soundPreviewStream = OggFile.ConvertAudioPlatform(previewAudioFile);
                    else
                        soundPreviewStream = File.OpenRead(previewAudioFile);
                else
                    soundPreviewStream = soundStream;

                // FLAT MODEL
                rsenumerableRootStream = new MemoryStream(Resources.rsenumerable_root);
                packPsarc.AddEntry("flatmodels/rs/rsenumerable_root.flat", rsenumerableRootStream);
                rsenumerableSongStream = new MemoryStream(Resources.rsenumerable_song);
                packPsarc.AddEntry("flatmodels/rs/rsenumerable_song.flat", rsenumerableSongStream);

                using (var toolkitVersionStream = new MemoryStream())
                using (var appIdStream = new MemoryStream())
                using (var packageListStream = new MemoryStream())
                using (var soundbankStream = new MemoryStream())
                using (var soundbankPreviewStream = new MemoryStream())
                using (var aggregateGraphStream = new MemoryStream())
                using (var manifestHeaderHSANStream = new MemoryStream())
                using (var manifestHeaderHSONStreamList = new DisposableCollection<Stream>())
                using (var manifestStreamList = new DisposableCollection<Stream>())
                using (var arrangementStream = new DisposableCollection<Stream>())
                using (var showlightStream = new MemoryStream())
                using (var xblockStream = new MemoryStream())
                {
                    // TOOLKIT VERSION
                    GenerateToolkitVersion(toolkitVersionStream, info.ToolkitInfo.PackageAuthor, info.ToolkitInfo.PackageVersion, info.ToolkitInfo.PackageComment, info.ToolkitInfo.PackageRating);
                    packPsarc.AddEntry("toolkit.version", toolkitVersionStream);

                    // APP ID
                    if (!platform.IsConsole)
                    {
                        GenerateAppId(appIdStream, info.AppId, platform);
                        packPsarc.AddEntry("appid.appid", appIdStream);
                    }

                    if (platform.platform == GamePlatform.XBox360)
                    {
                        var packageListWriter = new StreamWriter(packageListStream);
                        packageListWriter.Write(dlcName);
                        packageListWriter.Flush();
                        packageListStream.Seek(0, SeekOrigin.Begin);
                        packageListStream.WriteTmpFile("PackageList.txt", platform);
                    }

                    // SOUNDBANK
                    var soundbankFileName = String.Format("song_{0}", dlcName);
                    var audioFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.Name, soundStream, soundbankStream, info.Volume, platform);
                    packPsarc.AddEntry(String.Format("audio/{0}/{1}.bnk", platform.GetPathName()[0].ToLower(), soundbankFileName), soundbankStream);
                    packPsarc.AddEntry(String.Format("audio/{0}/{1}.wem", platform.GetPathName()[0].ToLower(), audioFileNameId), soundStream);

                    // SOUNDBANK PREVIEW
                    var soundbankPreviewFileName = String.Format("song_{0}_preview", dlcName);
                    dynamic audioPreviewFileNameId;
                    var previewVolume = info.PreviewVolume ?? info.Volume;
                    audioPreviewFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.Name + "_Preview", soundPreviewStream, soundbankPreviewStream, previewVolume, platform, true, !(File.Exists(previewAudioFile)));
                    packPsarc.AddEntry(String.Format("audio/{0}/{1}.bnk", platform.GetPathName()[0].ToLower(), soundbankPreviewFileName), soundbankPreviewStream);
                    if (!soundPreviewStream.Equals(soundStream)) packPsarc.AddEntry(String.Format("audio/{0}/{1}.wem", platform.GetPathName()[0].ToLower(), audioPreviewFileNameId), soundPreviewStream);

                    // AGGREGATE GRAPH
                    var aggregateGraphFileName = String.Format("{0}_aggregategraph.nt", dlcName);
                    var aggregateGraph = new AggregateGraph2014.AggregateGraph2014(info, platform);
                    aggregateGraph.Serialize(aggregateGraphStream);
                    aggregateGraphStream.Flush();
                    aggregateGraphStream.Seek(0, SeekOrigin.Begin);
                    packPsarc.AddEntry(aggregateGraphFileName, aggregateGraphStream);

                    var manifestHeader = new ManifestHeader2014<AttributesHeader2014>(platform);
                    var songPartition = new SongPartition();
                    var songPartitionCount = new SongPartition();

                    foreach (var arr in info.Arrangements)
                    {
                        if (arr.ArrangementType == ArrangementType.ShowLight)
                            continue;

                        var arrangementFileName = songPartition.GetArrangementFileName(arr.ArrangementName, arr.ArrangementType).ToLower();

                        // GAME SONG (SNG)
                        UpdateToneDescriptors(info);
                        GenerateSNG(arr, platform); // RS2014
                        var sngSongFile = File.OpenRead(arr.SongFile.File);
                        arrangementStream.Add(sngSongFile);
                        packPsarc.AddEntry(String.Format("songs/bin/{0}/{1}_{2}.sng", platform.GetPathName()[1].ToLower(), dlcName, arrangementFileName), sngSongFile);

                        // XML SONG
                        var xmlSongFile = File.OpenRead(arr.SongXml.File);
                        arrangementStream.Add(xmlSongFile);
                        packPsarc.AddEntry(String.Format("songs/arr/{0}_{1}.xml", dlcName, arrangementFileName), xmlSongFile);

                        // MANIFEST
                        var manifest = new Manifest2014<Attributes2014>();
                        var attribute = new Attributes2014(arrangementFileName, arr, info, platform);
                    
                        // TODO: monitor this change
                        // Commented out - EOF now properly sets the bonus/represent elements
                        //if (arrangement.ArrangementType == ArrangementType.Bass || arrangement.ArrangementType == ArrangementType.Guitar)
                        //{
                        //    // TODO: monitor this new code for bugs
                        //    // represent is set to "1" by default, if there is a bonus then set represent to "0"
                        //    attribute.Representative = arrangement.BonusArr ? 0 : 1;
                        //    attribute.ArrangementProperties.Represent = arrangement.BonusArr ? 0 : 1;

                        //    attribute.SongPartition = songPartitionCount.GetSongPartition(arrangement.Name, arrangement.ArrangementType);
                        //    if (attribute.SongPartition > 1 && !arrangement.BonusArr)
                        //    {
                        //        // for alternate arrangement then both represent and bonus are set to "0"
                        //        attribute.Representative = 0;
                        //        attribute.ArrangementProperties.Represent = 0;
                        //    }
                        //}

                        var attributeDictionary = new Dictionary<string, Attributes2014> { { "Attributes", attribute } };
                        manifest.Entries.Add(attribute.PersistentID, attributeDictionary);
                        var manifestStream = new MemoryStream();
                        manifestStreamList.Add(manifestStream);
                        manifest.Serialize(manifestStream);
                        manifestStream.Seek(0, SeekOrigin.Begin);

                        const string jsonPathPC = "manifests/songs_dlc_{0}/{0}_{1}.json";
                        const string jsonPathConsole = "manifests/songs_dlc/{0}_{1}.json";
                        packPsarc.AddEntry(String.Format((platform.IsConsole ? jsonPathConsole : jsonPathPC), dlcName, arrangementFileName), manifestStream);

                        // MANIFEST HEADER
                        var attributeHeaderDictionary = new Dictionary<string, AttributesHeader2014> { { "Attributes", new AttributesHeader2014(attribute) } };

                        if (platform.IsConsole)
                        {
                            // One for each arrangements (Xbox360/PS3)
                            manifestHeader = new ManifestHeader2014<AttributesHeader2014>(platform);
                            manifestHeader.Entries.Add(attribute.PersistentID, attributeHeaderDictionary);
                            var manifestHeaderStream = new MemoryStream();
                            manifestHeaderHSONStreamList.Add(manifestHeaderStream);
                            manifestHeader.Serialize(manifestHeaderStream);
                            manifestStream.Seek(0, SeekOrigin.Begin);
                            packPsarc.AddEntry(String.Format("manifests/songs_dlc/{0}_{1}.hson", dlcName, arrangementFileName), manifestHeaderStream);
                        }
                        else
                        {
                            // One for all arrangements (PC/Mac)
                            manifestHeader.Entries.Add(attribute.PersistentID, attributeHeaderDictionary);
                        }
                    }

                    if (!platform.IsConsole)
                    {
                        manifestHeader.Serialize(manifestHeaderHSANStream);
                        manifestHeaderHSANStream.Seek(0, SeekOrigin.Begin);
                        packPsarc.AddEntry(String.Format("manifests/songs_dlc_{0}/songs_dlc_{0}.hsan", dlcName), manifestHeaderHSANStream);
                    }

                    // XML SHOWLIGHTS
                    var shlArr = info.Arrangements.FirstOrDefault(ar => ar.ArrangementType == ArrangementType.ShowLight);
                    if (shlArr != null && shlArr.SongXml.File != null)
                    {
                        using (var fs = File.OpenRead(shlArr.SongXml.File))
                            fs.CopyTo(showlightStream);
                    }
                    else
                    {
                        // Generate Showlights 'cst_showlights.xml'
                        var showlight = new Showlights();
                        showlight.CreateShowlights(info);
                        // check for required minimum number of showlight elements
                        if (showlight.ShowlightList.Count > 1)
                        {
                            showlight.Serialize(showlightStream);
                            string shlFilePath = Path.Combine(Path.GetDirectoryName(info.Arrangements[0].SongXml.File), String.Format("{0}_showlights.xml", "cst"));
                            using (FileStream file = new FileStream(shlFilePath, FileMode.Create, FileAccess.Write))
                                showlightStream.WriteTo(file);

                            // write xml comments
                            Song2014.WriteXmlComments(shlFilePath);

                            // reload stream
                            using (var fs = File.OpenRead(shlFilePath))
                                fs.CopyTo(showlightStream);
                        }
                        else
                        {
                            // insufficient showlight changes may crash game
                            throw new InvalidOperationException("<ERROR> Insufficient showlight changes will crash game: " + showlight.ShowlightList.Count);
                        }
                    }

                    if (showlightStream.CanRead && showlightStream.Length > 0)
                        packPsarc.AddEntry(String.Format("songs/arr/{0}_showlights.xml", dlcName), showlightStream);

                    // XBLOCK
                    var game = GameXblock<Entity2014>.Generate2014(info, platform);
                    game.SerializeXml(xblockStream);
                    xblockStream.Flush();
                    xblockStream.Seek(0, SeekOrigin.Begin);
                    packPsarc.AddEntry(String.Format("gamexblocks/nsongs/{0}.xblock", dlcName), xblockStream);

                    // WRITE PACKAGE
                    packPsarc.Write(output, !platform.IsConsole);
                    output.WriteTmpFile(String.Format("{0}.psarc", dlcName), platform);
                }
            }
            finally
            {
                // Dispose all objects
                if (soundStream != null)
                    soundStream.Dispose();
                if (soundPreviewStream != null)
                    soundPreviewStream.Dispose();
                if (rsenumerableRootStream != null)
                    rsenumerableRootStream.Dispose();
                if (rsenumerableSongStream != null)
                    rsenumerableSongStream.Dispose();
                if (pnum <= 1)
                    DeleteTmpFiles(TMPFILES_ART);
                DeleteTmpFiles(TMPFILES_SNG);
            }
        }

        private static void GenerateRS2014InlayPsarc(Stream output, DLCPackageData info, Platform platform)
        {
            dlcName = info.Inlay.DLCSixName;
            packPsarc = new PSARC.PSARC();

            // Stream objects
            Stream rsenumerableRootStream = null;
            Stream rsenumerableGuitarStream = null;

            try
            {
                // ICON/INLAY FILES
                var ddsfiles = info.ArtFiles;

                if (ddsfiles == null)
                {
                    string iconPath;
                    if (File.Exists(info.Inlay.IconPath))
                    {
                        iconPath = info.Inlay.IconPath;
                    }
                    else
                    {
                        using (var iconStream = new MemoryStream(Resources.cgm_default_icon))
                        {
                            iconPath = Path.ChangeExtension(Path.GetTempFileName(), ".png");
                            iconStream.WriteFile(iconPath);
                            TMPFILES_ART.Add(iconPath);
                        }
                    }

                    string inlayPath;
                    if (File.Exists(info.Inlay.InlayPath))
                    {
                        inlayPath = info.Inlay.InlayPath;
                    }
                    else
                    {
                        using (var inlayStream = new MemoryStream(Resources.cgm_default_inlay))
                        {
                            inlayPath = GeneralExtension.GetTempFileName(".png");
                            inlayStream.WriteFile(inlayPath);
                            TMPFILES_ART.Add(inlayPath);
                        }
                    }

                    ddsfiles = new List<DDSConvertedFile>();
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = iconPath, destinationFile = GeneralExtension.GetTempFileName(".dds") });
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = iconPath, destinationFile = GeneralExtension.GetTempFileName(".dds") });
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = iconPath, destinationFile = GeneralExtension.GetTempFileName(".dds") });
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 512, sizeY = 512, sourceFile = iconPath, destinationFile = GeneralExtension.GetTempFileName(".dds") });
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 1024, sizeY = 512, sourceFile = inlayPath, destinationFile = GeneralExtension.GetTempFileName(".dds") });

                    // Convert to DDS
                    ToDDS(ddsfiles, DLCPackageType.Inlay);

                    // Save for reuse
                    info.ArtFiles = ddsfiles;
                }

                foreach (var dds in info.ArtFiles)
                    if (dds.sizeX == 1024)
                        packPsarc.AddEntry(String.Format("assets/gameplay/inlay/inlay_{0}.dds", dlcName), new FileStream(dds.destinationFile, FileMode.Open, FileAccess.Read, FileShare.Read));
                    else
                        packPsarc.AddEntry(String.Format("gfxassets/rewards/guitar_inlays/reward_inlay_{0}_{1}.dds", dlcName, dds.sizeX), new FileStream(dds.destinationFile, FileMode.Open, FileAccess.Read, FileShare.Read));

                // FLAT MODEL
                rsenumerableRootStream = new MemoryStream(Resources.rsenumerable_root);
                packPsarc.AddEntry("flatmodels/rs/rsenumerable_root.flat", rsenumerableRootStream);
                rsenumerableGuitarStream = new MemoryStream(Resources.rsenumerable_guitar);
                packPsarc.AddEntry("flatmodels/rs/rsenumerable_guitars.flat", rsenumerableGuitarStream);

                using (var toolkitVersionStream = new MemoryStream())
                using (var appIdStream = new MemoryStream())
                using (var packageListStream = new MemoryStream())
                using (var aggregateGraphStream = new MemoryStream())
                using (var manifestStreamList = new DisposableCollection<Stream>())
                using (var manifestHeaderStream = new MemoryStream())
                using (var nifStream = new MemoryStream())
                using (var xblockStream = new MemoryStream())
                {
                    // TOOLKIT VERSION
                    GenerateToolkitVersion(toolkitVersionStream);
                    packPsarc.AddEntry("toolkit.version", toolkitVersionStream);

                    // APP ID
                    if (!platform.IsConsole)
                    {
                        GenerateAppId(appIdStream, info.AppId, platform);
                        packPsarc.AddEntry("appid.appid", appIdStream);
                    }

                    if (platform.platform == GamePlatform.XBox360)
                    {
                        var packageListWriter = new StreamWriter(packageListStream);
                        packageListWriter.Write(dlcName);
                        packageListWriter.Flush();
                        packageListStream.Seek(0, SeekOrigin.Begin);
                        const string packageList = "PackageList.txt";
                        packageListStream.WriteTmpFile(packageList, platform);
                    }

                    // AGGREGATE GRAPH
                    var aggregateGraphFileName = String.Format("{0}_aggregategraph.nt", dlcName);
                    var aggregateGraph = new AggregateGraph2014.AggregateGraph2014(info, platform, DLCPackageType.Inlay);
                    aggregateGraph.Serialize(aggregateGraphStream);
                    aggregateGraphStream.Flush();
                    aggregateGraphStream.Seek(0, SeekOrigin.Begin);
                    packPsarc.AddEntry(aggregateGraphFileName, aggregateGraphStream);

                    // MANIFEST
                    var attribute = new InlayAttributes2014(info);
                    var attributeDictionary = new Dictionary<string, InlayAttributes2014> { { "Attributes", attribute } };
                    var manifest = new Manifest2014<InlayAttributes2014>(DLCPackageType.Inlay);
                    manifest.Entries.Add(attribute.PersistentID, attributeDictionary);
                    var manifestStream = new MemoryStream();
                    manifestStreamList.Add(manifestStream);
                    manifest.Serialize(manifestStream);
                    manifestStream.Seek(0, SeekOrigin.Begin);
                    const string jsonPathPC = "manifests/songs_dlc_{0}/dlc_guitar_{0}.json";
                    const string jsonPathConsole = "manifests/songs_dlc/dlc_guitar_{0}.json";
                    packPsarc.AddEntry(String.Format((platform.IsConsole ? jsonPathConsole : jsonPathPC), dlcName), manifestStream);

                    // MANIFEST HEADER
                    var attributeHeaderDictionary = new Dictionary<string, InlayAttributes2014> { { "Attributes", attribute } };
                    var manifestHeader = new ManifestHeader2014<InlayAttributes2014>(platform, DLCPackageType.Inlay);
                    manifestHeader.Entries.Add(attribute.PersistentID, attributeHeaderDictionary);
                    manifestHeader.Serialize(manifestHeaderStream);
                    manifestHeaderStream.Seek(0, SeekOrigin.Begin);
                    const string hsanPathPC = "manifests/songs_dlc_{0}/dlc_{0}.hsan";
                    const string hsonPathConsole = "manifests/songs_dlc/dlc_{0}.hson";
                    packPsarc.AddEntry(String.Format((platform.IsConsole ? hsonPathConsole : hsanPathPC), dlcName), manifestHeaderStream);

                    // XBLOCK
                    GameXblock<Entity2014> game = GameXblock<Entity2014>.Generate2014(info, platform, DLCPackageType.Inlay);
                    game.SerializeXml(xblockStream);
                    xblockStream.Flush();
                    xblockStream.Seek(0, SeekOrigin.Begin);
                    packPsarc.AddEntry(String.Format("gamexblocks/nguitars/guitar_{0}.xblock", dlcName), xblockStream);

                    // INLAY NIF
                    InlayNif nif = new InlayNif(info);
                    nif.Serialize(nifStream);
                    nifStream.Flush();
                    nifStream.Seek(0, SeekOrigin.Begin);
                    packPsarc.AddEntry(String.Format("assets/gameplay/inlay/{0}.nif", dlcName), nifStream);

                    // WRITE PACKAGE
                    packPsarc.Write(output, !platform.IsConsole);
                    output.WriteTmpFile(String.Format("{0}.psarc", dlcName), platform);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Dispose all objects
                if (rsenumerableRootStream != null)
                    rsenumerableRootStream.Dispose();
                if (rsenumerableGuitarStream != null)
                    rsenumerableGuitarStream.Dispose();
                DeleteTmpFiles(TMPFILES_ART);
            }

        }

        #endregion

        #region Generate PSARC RS1

        private static void GenerateRS1Psarcs(Stream output, DLCPackageData info, Platform platform)
        {
            packPsarc = new PSARC.PSARC(); // a shared variable
            IList<Stream> toneStreams = new List<Stream>();

            using (var toolkitVersionStream = new MemoryStream())
            using (var appIdStream = new MemoryStream())
            using (var packageListStream = new MemoryStream())
            using (var songPsarcStream = new MemoryStream())
            {
                var packageListWriter = new StreamWriter(packageListStream);

                try
                {
                    // TOOLKIT VERSION
                    GenerateToolkitVersion(toolkitVersionStream, info.ToolkitInfo.PackageAuthor, info.ToolkitInfo.PackageVersion, info.ToolkitInfo.PackageComment, info.ToolkitInfo.PackageRating);
                    packPsarc.AddEntry("toolkit.version", toolkitVersionStream);

                    // APP ID
                    if (platform.platform == GamePlatform.Pc)
                    {
                        GenerateAppId(appIdStream, info.AppId, platform);
                        packPsarc.AddEntry("APP_ID", appIdStream);
                    }

                    packageListWriter.WriteLine(info.Name);
                    GenerateRS1SongPsarc(songPsarcStream, info, platform);
                    string songFileName = String.Format("{0}.psarc", info.Name);
                    packPsarc.AddEntry(songFileName, songPsarcStream);
                    songPsarcStream.WriteTmpFile(songFileName, platform);

                    for (int i = 0; i < info.Tones.Count; i++)
                    {
                        // TODO: monitor code changes here
                        var tone = info.Tones[i];
                        var toneKey = tone.Key.GetValidKey(isTone: true); // validated JIC

                        if (String.IsNullOrEmpty(toneKey))
                            toneKey = tone.Name == null ? "Default" : tone.Name.GetValidKey(isTone: true);

                        var tonePsarcStream = new MemoryStream();
                        GenerateTonePsarc(tonePsarcStream, toneKey, tone);
                        string toneEntry = String.Format("DLC_Tone_{0}.psarc", toneKey);
                        packPsarc.AddEntry(toneEntry, tonePsarcStream);
                        tonePsarcStream.WriteTmpFile(toneEntry, platform);

                        if (i + 1 != info.Tones.Count)
                            packageListWriter.WriteLine("DLC_Tone_{0}", toneKey);
                        else
                            packageListWriter.Write("DLC_Tone_{0}", toneKey);

                        // TODO: generate single tone.manifest.json file that has multiple tones
                        // currently generating multiple tone.manifest.json files
                        toneStreams.Add(tonePsarcStream); // currently has no purpose
                    }

                    packageListWriter.Flush();
                    packageListStream.Seek(0, SeekOrigin.Begin);

                    if (platform.platform != GamePlatform.PS3)
                    {
                        string packageList = "PackageList.txt";
                        packPsarc.AddEntry(packageList, packageListStream);
                        packageListStream.WriteTmpFile(packageList, platform);
                    }

                    packPsarc.Write(output, false);
                }
                catch (Exception ex)
                {
                    throw new Exception("<ERROR> GenerateRS1Psarcs: " + ex.Message + Environment.NewLine);
                }
                finally
                {
                    foreach (var stream in toneStreams)
                        if (stream != null)
                            stream.Dispose();
                }
            }
        }

        private static void GenerateRS1SongPsarc(Stream output, DLCPackageData info, Platform platform)
        {
            var soundBankName = String.Format("Song_{0}", info.Name);
            Stream albumArtStream = null;
            Stream audioStream = null;
            string albumArtPath;

            try
            {
                if (File.Exists(info.AlbumArtPath))
                {
                    albumArtPath = info.AlbumArtPath;
                }
                else
                {
                    using (var defaultArtStream = new MemoryStream(Resources.albumart))
                    {
                        albumArtPath = GeneralExtension.GetTempFileName(".dds");
                        defaultArtStream.WriteFile(albumArtPath);
                        TMPFILES_ART.Add(albumArtPath);
                    }
                }

                var ddsfiles = info.ArtFiles;
                if (ddsfiles == null)
                {
                    ddsfiles = new List<DDSConvertedFile>();
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 512, sizeY = 512, sourceFile = albumArtPath, destinationFile = GeneralExtension.GetTempFileName(".dds") });
                    ToDDS(ddsfiles);

                    // Save for reuse
                    info.ArtFiles = ddsfiles;
                }

                albumArtStream = new FileStream(info.ArtFiles[0].destinationFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                // AUDIO
                var audioFile = info.OggPath;
                if (File.Exists(audioFile))
                    if (platform.IsConsole != audioFile.GetAudioPlatform().IsConsole)
                        audioStream = OggFile.ConvertAudioPlatform(audioFile);
                    else
                        audioStream = File.OpenRead(audioFile);
                else
                    throw new InvalidOperationException(String.Format("Audio file '{0}' not found.", audioFile));

                using (var aggregateGraphStream = new MemoryStream())
                using (var manifestStream = new MemoryStream())
                using (var xblockStream = new MemoryStream())
                using (var soundbankStream = new MemoryStream())
                using (var packageIdStream = new MemoryStream())
                using (var soundStream = OggFile.ConvertOgg(audioStream))
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

                    foreach (var arr in info.Arrangements)
                    {
                        GenerateSNG(arr, platform); // RS1
                        manifestBuilder.AggregateGraph.SongFiles.Add(arr.SongFile);
                        manifestBuilder.AggregateGraph.SongXMLs.Add(arr.SongXml);
                    }

                    manifestBuilder.AggregateGraph.XBlock = new XBlockFile { File = info.Name + ".xblock" };
                    manifestBuilder.AggregateGraph.Write(info.Name, platform.GetPathName(), platform, aggregateGraphStream);
                    aggregateGraphStream.Flush();
                    aggregateGraphStream.Seek(0, SeekOrigin.Begin);

                    var manifestData = manifestBuilder.GenerateManifest(info.Name, info.Arrangements, info.SongInfo, platform);
                    var writer = new StreamWriter(manifestStream);
                    writer.Write(manifestData);
                    writer.Flush();
                    manifestStream.Seek(0, SeekOrigin.Begin);

                    GameXblock<Entity>.Generate(info.Name, manifestBuilder.Manifest, manifestBuilder.AggregateGraph, xblockStream);
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

                    foreach (var arr in info.Arrangements)
                    {
                        if (!File.Exists(arr.SongFile.File) || !File.Exists(arr.SongXml.File))
                            throw new FileNotFoundException("<ERROR> Can not find required SNG/XML file(s)" + Environment.NewLine);

                        var xmlFile = File.OpenRead(arr.SongXml.File);
                        arrangementFiles.Add(xmlFile);
                        var sngFile = File.OpenRead(arr.SongFile.File);
                        arrangementFiles.Add(sngFile);
                        songPsarc.AddEntry(String.Format("GR/Behaviors/Songs/{0}.xml", Path.GetFileNameWithoutExtension(arr.SongXml.File)), xmlFile);
                        songPsarc.AddEntry(String.Format("GRExports/{0}/{1}.sng", platform.GetPathName()[1], Path.GetFileNameWithoutExtension(arr.SongFile.File)), sngFile);
                    }

                    songPsarc.Write(output, false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("<ERROR> GenerateSongPsarcRS1: " + ex.Message + Environment.NewLine);
            }
            finally
            {
                if (albumArtStream != null)
                    albumArtStream.Dispose();

                if (audioStream != null)
                    audioStream.Dispose();
            }
        }

        private static void GeneratePackageList(Stream output, string dlcName)
        {
            var writer = new StreamWriter(output);
            writer.WriteLine(dlcName);
            writer.WriteLine("DLC_Tone_{0}", dlcName);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GenerateSongPackageId(Stream output, string dlcName)
        {
            var writer = new StreamWriter(output);
            writer.Write(dlcName);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GenerateTonePsarc(Stream output, string toneKey, Tone tone)
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
                var x = (tone.PedalList.Where(pedal => pedal.Value.PedalKey.ToLower().Contains("bass"))).Count();
                tonePsarc.AddEntry(x > 0 ? "Manifests/tone_bass.manifest.json" : "Manifests/tone.manifest.json", toneManifestStream);
                tonePsarc.AddEntry("AggregateGraph.nt", toneAggregateGraphStream);
                tonePsarc.AddEntry("PACKAGE_ID", packageIdStream);
                tonePsarc.Write(output, false);
            }
        }

        private static void GenerateTonePackageId(Stream output, string toneKey)
        {
            var writer = new StreamWriter(output);
            writer.Write("DLC_Tone_{0}", toneKey);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        #endregion

        #region COMMON

        public static void ToDDS(List<DDSConvertedFile> filesToConvert, DLCPackageType dlcType = DLCPackageType.Song)
        {
            string args = null;
            switch (dlcType)
            {
                case DLCPackageType.Song:
                    args = "-file \"{0}\" -output \"{1}\" -prescale {2} {3} -nomipmap -RescaleBox -dxt1a -overwrite -forcewrite";
                    break;
                case DLCPackageType.Lesson:
                    throw new NotImplementedException("Lesson package type not implemented yet :(");
                case DLCPackageType.Inlay:
                    // CRITICAL - DO NOT CHANGE ARGS
                    args = "-file \"{0}\" -output \"{1}\" -prescale {2} {3} -quality_highest -max -dxt5 -nomipmap -alpha -overwrite -forcewrite";
                    break;
            }

            foreach (var item in filesToConvert)
                GeneralExtension.RunExternalExecutable(ExternalApps.APP_NVDXT, true, true, true, String.Format(args, item.sourceFile, item.destinationFile, item.sizeX, item.sizeY));
        }

        /// <summary>
        /// Generates memory stream for toolkit.version file
        /// </summary>
        /// <param name="output"></param>
        /// <param name="packageAuthor"></param>
        /// <param name="packageVersion">0.0 to 9.9 decimal or integer versioning system</param>
        /// <param name="packageComment"></param>
        /// <param name="packageRating">0 to 5 user rating system</param>
        /// <param name="toolkitVersion">If null/empty then the current toolkitversion is used</param>
        public static void GenerateToolkitVersion(Stream output, string packageAuthor = null, string packageVersion = null, string packageComment = null, string packageRating = null, string toolkitVersion = null)
        {
            var writer = new StreamWriter(output);
            if (String.IsNullOrEmpty(packageAuthor))
                packageAuthor = ConfigRepository.Instance()["general_defaultauthor"];
            if (String.IsNullOrEmpty(toolkitVersion))
                toolkitVersion = ToolkitVersion.RSTKGuiVersion;
            if (!String.IsNullOrEmpty(toolkitVersion))
                writer.WriteLine("Toolkit version: {0}", toolkitVersion);
            if (!String.IsNullOrEmpty(packageAuthor))
                writer.WriteLine("Package Author: {0}", packageAuthor);
            if (!String.IsNullOrEmpty(packageVersion))
                writer.WriteLine("Package Version: {0}", packageVersion);
            if (!String.IsNullOrEmpty(packageRating))
                writer.WriteLine("Package Rating: {0}", packageRating);
            if (!String.IsNullOrEmpty(packageComment))
                writer.Write("Package Comment: {0}", packageComment);

            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        public static void GenerateAppId(Stream output, string appId, Platform platform)
        {
            var writer = new StreamWriter(output);
            var defaultAppId = (platform.version == GameVersion.RS2012) ? "206113" : "248750";
            writer.Write(appId ?? defaultAppId);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        public static void UpdateToneDescriptors(DLCPackageData info)
        {
            foreach (var tone in info.TonesRS2014)
            {
                if (tone == null) continue;
                string DescName = tone.Name.Split('_').Last();
                foreach (var td in ToneDescriptor.List())
                {
                    if (td.ShortName != DescName)
                        continue;

                    tone.ToneDescriptors.Clear();
                    tone.ToneDescriptors.Add(td.Descriptor);
                    break;
                }
            }
        }

        public static void GenerateSNG(Arrangement arr, Platform platform)
        {
            string sngFile = Path.ChangeExtension(arr.SongXml.File, ".sng");
            switch (platform.version)
            {
                case GameVersion.RS2012:
                    SngFileWriter.Write(arr, sngFile, platform);
                    break;
                case GameVersion.RS2014:
                    // Sng2014File can be reused when generating multiple platforms from cached results
                    if (arr.Sng2014 == null)
                    {
                        // cache results
                        arr.Sng2014 = Sng2014File.ConvertXML(arr.SongXml.File, arr.ArrangementType, arr.FontSng);
                        if (arr.CustomFont)
                            arr.Sng2014.PopFontPath(dlcName);
                    }

                    using (var fs = new FileStream(sngFile, FileMode.Create))
                        arr.Sng2014.WriteSng(fs, platform);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected game version value");
            }

            if (arr.SongFile == null)
                arr.SongFile = new SongFile { File = "" };

            arr.SongFile.File = Path.GetFullPath(sngFile);
            TMPFILES_SNG.Add(sngFile);
        }

        private static void WriteTmpFile(this Stream memoryStream, string fileName, Platform platform)
        {
            if (platform.IsConsole)
            {
                string workDir = platform.platform == GamePlatform.XBox360 ? XBOX_WORKDIR : PS3_WORKDIR;
                string filePath = Path.Combine(workDir, fileName);

                memoryStream.WriteFile(filePath);
                switch (platform.platform)
                {
                    case GamePlatform.XBox360:
                        FILES_XBOX.Add(filePath);
                        break;
                    case GamePlatform.PS3:
                        FILES_PS3.Add(filePath);
                        break;
                }
            }
        }

        #endregion
    }
}

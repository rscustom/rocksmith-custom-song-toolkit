using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using X360.Other;
using X360.STFS;
using X360.IO;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.XBlock;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.DLCPackage.Showlight;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class DLCPackageCreator {
        #region CONSTANT

        private static readonly string XBOX_WORKDIR = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "xboxpackage");
        private static readonly string PS3_WORKDIR = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "edat");

        private static readonly string[] PATH_PC = { "Windows", "Generic", "_p" };
        private static readonly string[] PATH_MAC = { "Mac", "MacOS", "_m" };
        private static readonly string[] PATH_XBOX = { "XBox360", "XBox360", "_xbox" };
        private static readonly string[] PATH_PS3 = { "PS3", "PS3", "_ps3" };

        private static List<string> FILES_XBOX = new List<string>();
        private static List<string> FILES_PS3 = new List<string>();
        private static List<string> TMPFILES_SNG = new List<string>();
        private static List<string> TMPFILES_ART = new List<string>();

        private static void DeleteTmpFiles(List<string> files)
        {
            try {
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
            switch (platform.platform)
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

        public static void Generate(string packagePath, DLCPackageData info, Platform platform)
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

            using (var packPsarcStream = new MemoryStream())
            {
                switch (platform.version)
                {
                    case GameVersion.RS2014:
                        GeneratePsarcsForRS2014(packPsarcStream, info, platform);
                        break;
                    case GameVersion.RS2012:
                        GeneratePsarcsForRS1(packPsarcStream, info, platform);
                        break;
                    case GameVersion.None:
                        throw new InvalidOperationException("Unexpected game version value");
                }

                var packageName = Path.GetFileNameWithoutExtension(packagePath).StripPlatformEndName();

                var songFileName = String.Format("{0}{1}", Path.Combine(Path.GetDirectoryName(packagePath), packageName), platform.GetPathName()[2]);
                                
                switch (platform.platform)
                {
                    case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        switch (platform.version)
	                    {
                            // SAVE PACKAGE
                            case GameVersion.RS2014:
                                using (FileStream fl = File.Create(songFileName + ".psarc"))
                                {
                                    packPsarcStream.CopyTo(fl);
                                }
                                break;
                            case GameVersion.RS2012:
                                using (var fl = File.Create(songFileName + ".dat"))
                                {
                                    RijndaelEncryptor.EncryptFile(packPsarcStream, fl, RijndaelEncryptor.DLCKey);
                                }
                                break;
                            default:
                                throw new InvalidOperationException("Unexpected game version value");
	                    }
                        break;
                    case GamePlatform.XBox360:
                        BuildXBox360Package(songFileName, info, FILES_XBOX, platform.version);
                        break;
                    case GamePlatform.PS3:
                        EncryptPS3EdatFiles(songFileName + ".psarc", platform);
                        break;
                }                
            }

            FILES_XBOX.Clear();
            FILES_PS3.Clear();
            DeleteTmpFiles(TMPFILES_SNG);
        }

        #region XBox360

        public static void BuildXBox360Package(string songFileName, DLCPackageData info, IEnumerable<string> xboxFiles, GameVersion gameVersion)
        {
            LogRecord x = new LogRecord();
            RSAParams xboxRSA = info.SignatureType == PackageMagic.CON ? new RSAParams(new DJsIO(Resources.XBox360_KV, true)) : new RSAParams(StrongSigned.LIVE);
            CreateSTFS xboxSTFS = new CreateSTFS();
            xboxSTFS.HeaderData = info.GetSTFSHeader(gameVersion);
            foreach (string file in xboxFiles)
                xboxSTFS.AddFile(file, Path.GetFileName(file));

            STFSPackage xboxPackage = new STFSPackage(xboxSTFS, xboxRSA, songFileName, x);
            var generated = xboxPackage.RebuildPackage(xboxRSA);
            if (!generated)
                throw new InvalidOperationException("Error on create XBox360 package, details: \n\r" + x.Log);

            xboxPackage.FlushPackage(xboxRSA);
            xboxPackage.CloseIO();

            try
            {
                if (Directory.Exists(XBOX_WORKDIR))
                    Directory.Delete(XBOX_WORKDIR, true);
            }
            catch { /*Have no problem if don't delete*/ }
        }

        private static HeaderData GetSTFSHeader(this DLCPackageData dlcData, GameVersion gameVersion) {
            HeaderData hd = new HeaderData();
            string displayName = String.Format("{0} by {1}", dlcData.SongInfo.SongDisplayName, dlcData.SongInfo.Artist);
            switch (gameVersion)
            {
                case GameVersion.RS2012:
                    hd.Title_Package = "Rocksmith";
                    hd.TitleID = 1431505011; //55530873 in HEXA for RS1
                    hd.PackageImageBinary = Resources.XBox360_DLC_image.ImageToBytes(ImageFormat.Png);
                    break;
                case GameVersion.RS2014:
                    hd.Title_Package = "Rocksmith 2014";
                    hd.TitleID = 1431505088; //555308C0 in HEXA for RS2014
                    hd.PackageImageBinary = Resources.XBox360_DLC_image2014.ImageToBytes(ImageFormat.Png);
                    break;
            }
            
            hd.Publisher = String.Format("Custom Song Creator Toolkit (v{0}.{1}.{2}.{3} beta)",
                                        Assembly.GetExecutingAssembly().GetName().Version.Major,
                                        Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                        Assembly.GetExecutingAssembly().GetName().Version.Build,
                                        Assembly.GetExecutingAssembly().GetName().Version.Revision);
            hd.Title_Display = displayName;
            hd.Description = displayName;
            hd.ThisType = PackageType.MarketPlace;
            hd.ContentImageBinary = hd.PackageImageBinary;
            hd.IDTransfer = TransferLock.AllowTransfer;
            if (dlcData.SignatureType == PackageMagic.LIVE)
                foreach (var license in dlcData.XBox360Licenses)
                    hd.AddLicense(license.ID, license.Bit, license.Flag);

            return hd;
        }

        public static byte[] ImageToBytes(this System.Drawing.Image image, ImageFormat format)
        {
            byte[] xReturn = null;
            using (MemoryStream xMS = new MemoryStream())
            {
                image.Save(xMS, format);
                xReturn = xMS.ToArray();
            }//disposed automaticly()
            return xReturn;
        }

        private static void WriteTmpFile(this Stream ms, string fileName, Platform platform)
        {
            if (platform.platform == GamePlatform.XBox360 || platform.platform == GamePlatform.PS3)
            {
                string workDir = platform.platform == GamePlatform.XBox360 ? XBOX_WORKDIR : PS3_WORKDIR;
                string filePath = Path.Combine(workDir, fileName);

                using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = new byte[ms.Length];
                    ms.Read(bytes, 0, (int)ms.Length);
                    file.Write(bytes, 0, bytes.Length);
                }
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

        #region PS3

        public static void EncryptPS3EdatFiles(string songFileName, Platform platform)
        {
            if (Path.GetFileName(songFileName).Contains(" "))
                songFileName = Path.Combine(Path.GetDirectoryName(songFileName), Path.GetFileName(songFileName).Replace(" ", "_")); // Due to PS3 encryption limitation

            // Cleaning work dir
            var junkFiles = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".edat") || s.EndsWith(".bak"));
            foreach(var junk in junkFiles)
                File.Delete(junk);

            if (platform.version == GameVersion.RS2014) {
                // Have only one file for RS2014 package, so can be rename that the user defined
                if (FILES_PS3.Count == 1)
                    if (File.Exists(FILES_PS3[0]))
                    {
                        var oldName = FILES_PS3[0].Clone().ToString();
                        FILES_PS3[0] = Path.Combine(Path.GetDirectoryName(FILES_PS3[0]), Path.GetFileName(songFileName));
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
                var userSavePath = String.Format("{0}.edat", songFileName);

                if (File.Exists(userSavePath))
                    File.Delete(userSavePath);

                if (File.Exists(encryptedFile))
                    File.Move(encryptedFile, userSavePath);
            }
            else
            {
                if (Directory.Exists(PS3_WORKDIR))
                    DirectoryExtension.Move(PS3_WORKDIR, String.Format("{0}_PS3", songFileName));
            }

            if (encryptResult.IndexOf("No JDK or JRE is intsalled on your machine") > 0)
                throw new InvalidOperationException("You need install Java SE 7 (x86) or higher on your machine. The Java path should be in PATH Environment Variable:" + Environment.NewLine + Environment.NewLine + encryptResult);

            if (encryptResult.IndexOf("Encrypt all EDAT files successfully") < 0)
                throw new InvalidOperationException("Rebuilder error, please check if .edat files are created correctly and see output bellow:" + Environment.NewLine + Environment.NewLine + encryptResult);
        }

        #endregion

        #endregion

        #region Generate PSARC RS2014

        private static void GeneratePsarcsForRS2014(MemoryStream output, DLCPackageData info, Platform platform)
        {
            var dlcName = info.Name.ToLower();
            
            {
                var packPsarc = new PSARC.PSARC();

                // Stream objects
                Stream soundStream = null,
                       soundPreviewStream = null,
                       rsenumerableRootStream = null,
                       rsenumerableSongStream = null;
                
                try
                {
                    // ALBUM ART
                    Dictionary<int,string> ddsfiles = info.AlbumArt;
                    var sizes = new List<int> { 64, 128, 256 };

                    if (ddsfiles == null) {
                        string albumArtPath;
                        if (File.Exists(info.AlbumArtPath)) {
                            albumArtPath = info.AlbumArtPath;
                        } else {
                            var albumArtStream = new MemoryStream(Resources.albumart2014_256);
                            albumArtPath = System.IO.Path.GetTempFileName();
                            albumArtStream.WriteTmpFile(albumArtPath, platform);
                            TMPFILES_ART.Add(albumArtPath);
                        }

                        ddsfiles = new Dictionary<int,string>();
                        foreach (var size in sizes)
                            ddsfiles[size] = System.IO.Path.GetTempFileName();

                        ToDds(albumArtPath, ddsfiles);
                        // save for reuse
                        info.AlbumArt = ddsfiles;
                    }

                    foreach (var size in sizes)
                        packPsarc.AddEntry(String.Format("gfxassets/album_art/album_{0}_{1}.dds", dlcName, size), new FileStream(ddsfiles[size], FileMode.Open, FileAccess.Read, FileShare.Read));

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
                        GenerateToolkitVersion(toolkitVersionStream);
                        packPsarc.AddEntry("toolkit.version", toolkitVersionStream);

                        // APP ID
                        if (!platform.IsConsole)
                        {
                            GenerateAppId(appIdStream, info.AppId);
                            packPsarc.AddEntry("appid.appid", appIdStream);
                        }

                        if (platform.platform == GamePlatform.XBox360) {
                            var packageListWriter = new StreamWriter(packageListStream);
                            packageListWriter.Write(dlcName);
                            packageListWriter.Flush();
                            packageListStream.Seek(0, SeekOrigin.Begin);
                            string packageList = "PackageList.txt";
                            packageListStream.WriteTmpFile(packageList, platform);
                        }
                            
                        // SOUNDBANK
                        var soundbankFileName = String.Format("song_{0}", dlcName);
                        var audioFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.Name, soundStream, soundbankStream, info.Volume, platform);
                        packPsarc.AddEntry(String.Format("audio/{0}/{1}.bnk", platform.GetPathName()[0].ToLower(), soundbankFileName), soundbankStream);
                        packPsarc.AddEntry(String.Format("audio/{0}/{1}.wem", platform.GetPathName()[0].ToLower(), audioFileNameId), soundStream);

                        // SOUNDBANK PREVIEW
                        var soundbankPreviewFileName = String.Format("song_{0}_preview", dlcName);
                        dynamic audioPreviewFileNameId;
                        if (!soundPreviewStream.Equals(soundStream)) audioPreviewFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.Name + "_Preview", soundPreviewStream, soundbankPreviewStream, info.Volume, platform, true);
                        else audioPreviewFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.Name + "_Preview", soundPreviewStream, soundbankPreviewStream, info.Volume, platform, true, true);
                        packPsarc.AddEntry(String.Format("audio/{0}/{1}.bnk", platform.GetPathName()[0].ToLower(), soundbankPreviewFileName), soundbankPreviewStream);
                        if (!soundPreviewStream.Equals(soundStream)) packPsarc.AddEntry(String.Format("audio/{0}/{1}.wem", platform.GetPathName()[0].ToLower(), audioPreviewFileNameId), soundPreviewStream);

                        // AGGREGATE GRAPH
                        var aggregateGraphFileName = String.Format("{0}_aggregategraph.nt", info.Name.ToLower());
                        var aggregateGraph = new AggregateGraph2014(info, platform);
                        aggregateGraph.Serialize(aggregateGraphStream, platform);
                        aggregateGraphStream.Flush();
                        aggregateGraphStream.Seek(0, SeekOrigin.Begin);
                        packPsarc.AddEntry(aggregateGraphFileName, aggregateGraphStream); 

                        var manifestHeader = new ManifestHeader2014(platform);
                        var songPartition = new SongPartition();
                        var songPartitionCount = new SongPartition();

                        foreach (var arrangement in info.Arrangements)
                        {
                            var arrangementFileName = songPartition.GetArrangementFileName(arrangement.Name, arrangement.ArrangementType).ToLower();

                            // GAME SONG (SNG)
                            UpdateToneDescriptors(info);
                            GenerateSNG(arrangement, platform);
                            var sngSongFile = File.OpenRead(arrangement.SongFile.File);
                            arrangementStream.Add(sngSongFile);
                            packPsarc.AddEntry(String.Format("songs/bin/{0}/{1}_{2}.sng", platform.GetPathName()[1].ToLower(), dlcName, arrangementFileName), sngSongFile);

                            // XML SONG
                            var xmlSongFile = File.OpenRead(arrangement.SongXml.File);
                            arrangementStream.Add(xmlSongFile);
                            packPsarc.AddEntry(String.Format("songs/arr/{0}_{1}.xml", dlcName, arrangementFileName), xmlSongFile);

                            // MANIFEST
                            var manifest = new Manifest2014<Attributes2014>();
                            var attribute = new Attributes2014(arrangementFileName, arrangement, info, platform);
                            if (arrangement.ArrangementType != Sng.ArrangementType.Vocal)
                            {
                                attribute.SongPartition = songPartitionCount.GetSongPartition(arrangement.Name, arrangement.ArrangementType);
                                if (attribute.SongPartition > 1)
                                { // Make the second arrangement with the same arrangement type as ALTERNATE arrangement ingame
                                    attribute.Representative = 0;
                                    attribute.ArrangementProperties.Represent = 0;
                                }
                            }
                            var attributeDictionary = new Dictionary<string, Attributes2014> { { "Attributes", attribute } };
                            manifest.Entries.Add(attribute.PersistentID, attributeDictionary);                                
                            var manifestStream = new MemoryStream();
                            manifestStreamList.Add(manifestStream);
                            manifest.Serialize(manifestStream);
                            manifestStream.Seek(0, SeekOrigin.Begin);

                            var jsonPathPC = "manifests/songs_dlc_{0}/{0}_{1}.json";
                            var jsonPathConsole = "manifests/songs_dlc/{0}_{1}.json";
                            packPsarc.AddEntry(String.Format((platform.IsConsole ? jsonPathConsole : jsonPathPC), dlcName, arrangementFileName), manifestStream);

                            // MANIFEST HEADER
                            var attributeHeaderDictionary = new Dictionary<string, AttributesHeader2014> { { "Attributes", new AttributesHeader2014(attribute) } };

                            if (platform.IsConsole) {
                                // One for each arrangements (Xbox360/PS3)
                                manifestHeader = new ManifestHeader2014(platform);
                                manifestHeader.Entries.Add(attribute.PersistentID, attributeHeaderDictionary);
                                var manifestHeaderStream = new MemoryStream();
                                manifestHeaderHSONStreamList.Add(manifestHeaderStream);
                                manifestHeader.Serialize(manifestHeaderStream);
                                manifestStream.Seek(0, SeekOrigin.Begin);
                                packPsarc.AddEntry(String.Format("manifests/songs_dlc/{0}_{1}.hson", dlcName, arrangementFileName), manifestHeaderStream);
                            } else {
                                // One for all arrangements (PC/Mac)
                                manifestHeader.Entries.Add(attribute.PersistentID, attributeHeaderDictionary);
                            }
                        }

                        if (!platform.IsConsole) {
                            manifestHeader.Serialize(manifestHeaderHSANStream);
                            manifestHeaderHSANStream.Seek(0, SeekOrigin.Begin);
                            packPsarc.AddEntry(String.Format("manifests/songs_dlc_{0}/songs_dlc_{0}.hsan", dlcName), manifestHeaderHSANStream);
                        }

                        // SHOWLIGHT
                        Showlights showlight = new Showlights(info);
                        showlight.Serialize(showlightStream);
                        if(showlightStream.CanRead)
                        packPsarc.AddEntry(String.Format("songs/arr/{0}_showlights.xml", dlcName), showlightStream);

                        // XBLOCK
                        GameXblock<Entity2014> game = GameXblock<Entity2014>.Generate2014(info, platform);
                        game.SerializeXml(xblockStream);
                        xblockStream.Flush();
                        xblockStream.Seek(0, SeekOrigin.Begin);
                        packPsarc.AddEntry(String.Format("gamexblocks/nsongs/{0}.xblock", info.Name.ToLower()), xblockStream);

                        // WRITE PACKAGE
                        packPsarc.Write(output, !platform.IsConsole);
                        output.Flush();
                        output.Seek(0, SeekOrigin.Begin);
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
                    DeleteTmpFiles(TMPFILES_SNG);
                    DeleteTmpFiles(TMPFILES_ART);
                }
            }
        }     

        #endregion

        #region Generate PSARC RS1

        private static void GeneratePsarcsForRS1(Stream output, DLCPackageData info, Platform platform)
        {
            IList<Stream> toneStreams = new List<Stream>();
            using (var toolkitVersionStream = new MemoryStream())
            using (var appIdStream = new MemoryStream())
            using (var packageListStream = new MemoryStream())
            using (var songPsarcStream = new MemoryStream())
            {
                try
                {
                    var packPsarc = new PSARC.PSARC();
                    var packageListWriter = new StreamWriter(packageListStream);

                    // TOOLKIT VERSION
                    GenerateToolkitVersion(toolkitVersionStream);
                    packPsarc.AddEntry("toolkit.version", toolkitVersionStream);

                    // APP ID
                    if (platform.platform == GamePlatform.Pc)
                    {
                        GenerateAppId(appIdStream, info.AppId);
                        packPsarc.AddEntry("APP_ID", appIdStream);
                    }

                    packageListWriter.WriteLine(info.Name);

                    GenerateSongPsarcRS1(songPsarcStream, info, platform);
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
                    if (platform.platform != GamePlatform.PS3)
                    {
                        string packageList = "PackageList.txt";
                        packPsarc.AddEntry(packageList, packageListStream);
                        packageListStream.WriteTmpFile(packageList, platform);
                    }
                    packPsarc.Write(output, false);
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

        private static void GenerateSongPsarcRS1(Stream output, DLCPackageData info, Platform platform) {
            var soundBankName = String.Format("Song_{0}", info.Name);

            try
            {
                Stream albumArtStream = null,
                       audioStream = null;

                string albumArtPath;
                if (File.Exists(info.AlbumArtPath)) {
                    albumArtPath = info.AlbumArtPath;
                } else {
                    var defaultArtStream = new MemoryStream(Resources.albumart);
                    albumArtPath = System.IO.Path.GetTempFileName();
                    defaultArtStream.WriteTmpFile(albumArtPath, platform);
                    defaultArtStream.Dispose();
                    TMPFILES_ART.Add(albumArtPath);
                }

                Dictionary<int,string> ddsfiles = info.AlbumArt;
                if (ddsfiles == null) {
                    ddsfiles = new Dictionary<int,string>();
                    ddsfiles[512] = System.IO.Path.GetTempFileName();
                    ToDds(albumArtPath, ddsfiles);

                    // save for reuse
                    info.AlbumArt = ddsfiles;
                }

                albumArtStream = new FileStream(ddsfiles[512], FileMode.Open, FileAccess.Read, FileShare.Read);

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
                using (var arrangementFiles = new DisposableCollection<Stream>()) {
                    var manifestBuilder = new ManifestBuilder {
                        AggregateGraph = new AggregateGraph.AggregateGraph {
                            SoundBank = new SoundBank { File = soundBankName + ".bnk" },
                            AlbumArt = new AlbumArt { File = info.AlbumArtPath }
                        }
                    };

                    foreach (var x in info.Arrangements) {
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

                    foreach (var x in info.Arrangements) {
                        var xmlFile = File.OpenRead(x.SongXml.File);
                        arrangementFiles.Add(xmlFile);
                        var sngFile = File.OpenRead(x.SongFile.File);
                        arrangementFiles.Add(sngFile);
                        songPsarc.AddEntry(String.Format("GR/Behaviors/Songs/{0}.xml", Path.GetFileNameWithoutExtension(x.SongXml.File)), xmlFile);
                        songPsarc.AddEntry(String.Format("GRExports/{0}/{1}.sng", platform.GetPathName()[1], Path.GetFileNameWithoutExtension(x.SongFile.File)), sngFile);
                    }
                    songPsarc.Write(output, false);
                    output.Flush();
                    output.Seek(0, SeekOrigin.Begin);
                }
            }
            finally
            {
            }
        }

        private static void GeneratePackageList(Stream output, string dlcName) {
            var writer = new StreamWriter(output);
            writer.WriteLine(dlcName);
            writer.WriteLine("DLC_Tone_{0}", dlcName);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GenerateSongPackageId(Stream output, string dlcName) {
            var writer = new StreamWriter(output);
            writer.Write(dlcName);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GenerateTonePsarc(Stream output, string toneKey, Tone.Tone tone) {
            var tonePsarc = new PSARC.PSARC();

            using (var packageIdStream = new MemoryStream())
            using (var toneManifestStream = new MemoryStream())
            using (var toneXblockStream = new MemoryStream())
            using (var toneAggregateGraphStream = new MemoryStream()) {
                ToneGenerator.Generate(toneKey, tone, toneManifestStream, toneXblockStream, toneAggregateGraphStream);
                GenerateTonePackageId(packageIdStream, toneKey);
                tonePsarc.AddEntry(String.Format("Exports/Pedals/DLC_Tone_{0}.xblock", toneKey), toneXblockStream);
                var x = (from pedal in tone.PedalList
                         where pedal.Value.PedalKey.ToLower().Contains("bass")
                         select pedal).Count();
                tonePsarc.AddEntry(x > 0 ? "Manifests/tone_bass.manifest.json" : "Manifests/tone.manifest.json", toneManifestStream);
                tonePsarc.AddEntry("AggregateGraph.nt", toneAggregateGraphStream);
                tonePsarc.AddEntry("PACKAGE_ID", packageIdStream);
                tonePsarc.Write(output, false);
                output.Flush();
                output.Seek(0, SeekOrigin.Begin);
            }
        }

        private static void GenerateTonePackageId(Stream output, string toneKey) {
            var writer = new StreamWriter(output);
            writer.Write("DLC_Tone_{0}", toneKey);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        #endregion

        #region COMMON

        private static void ToDds(string input, Dictionary<int, string> output)
        {
            string format = "dxt1a";
            Dictionary<int, Process> proc = new Dictionary<int, Process>();

            foreach (KeyValuePair<int, string> item in output)
            {
                var MS = new MemoryStream();
                int Size = item.Key;
                string OutPath = item.Value;
                var args = String.Format("-file \"{0}\" -output \"{1}\" -nomipmap -prescale {2} {2} -RescaleBox -{3} -overwrite -forcewrite",
                                         input, OutPath, Size, format);

                var nvdxt = new Process();
                nvdxt.StartInfo = new ProcessStartInfo
                {
                    FileName = "nvdxt.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                nvdxt.Start();
                nvdxt.WaitForExit();
            }
        }

        private static void GenerateToolkitVersion(Stream output)
        {
            var writer = new StreamWriter(output);
            writer.Write(ToolkitVersion.version);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private static void GenerateAppId(Stream output, string appId)
        {
            var writer = new StreamWriter(output);
            writer.Write(appId ?? "206113");
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        public static void UpdateToneDescriptors(DLCPackageData info)
        {
            foreach (var tone in info.TonesRS2014) {
                if (tone == null) continue;
                string DescName = tone.Name.Split('_').Last();
                foreach (var td in ToneDescriptor.List()) {
                    if (td.ShortName != DescName)
                        continue;

                    tone.ToneDescriptors.Clear();
                    tone.ToneDescriptors.Add(td.Descriptor);
                    break;
                }
            }
        }

        public static void GenerateSNG(Arrangement arrangement, Platform platform) {
            string sngFile = Path.ChangeExtension(arrangement.SongXml.File, ".sng");
            switch (platform.version)
            {
                case GameVersion.RS2012:
                    SngFileWriter.Write(arrangement.SongXml.File, sngFile, arrangement.ArrangementType, platform);
                    break;
                case GameVersion.RS2014:
                    if (arrangement.Sng2014 == null) {
                        // TODO this needs to be enabled again
                        //UpdateTones(arrangement);

                        // Sng2014File can be reused when generating for multiple platforms
                        // cache results
                        arrangement.Sng2014 = Sng2014File.ConvertXML(arrangement.SongXml.File, arrangement.ArrangementType);
                    }
                    using (FileStream fs = new FileStream(sngFile, FileMode.Create))
                        arrangement.Sng2014.writeSng(fs, platform);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected game version value");
            }

            if (arrangement.SongFile == null)
                arrangement.SongFile = new SongFile();
            arrangement.SongFile.File = sngFile;

            TMPFILES_SNG.Add(sngFile);
        }

        #endregion
    }
}

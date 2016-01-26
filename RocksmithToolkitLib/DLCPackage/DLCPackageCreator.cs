using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Header;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using X360.IO;
using X360.Other;
using X360.STFS;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Showlight;
using RocksmithToolkitLib.DLCPackage.XBlock;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.Ogg;
using Tone = RocksmithToolkitLib.DLCPackage.Manifest.Tone.Tone;

namespace RocksmithToolkitLib.DLCPackage
{
    public enum DLCPackageType { Song = 0, Lesson = 1, Inlay = 2 }
    public enum ModType { Custom_Guitar_Inlays = 0, Custom_Intro_Screens = 1 }

    public static class DLCPackageCreator
    {
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
        /// Generates CDLC package into packagePath.
        /// </summary>
        /// <param name="packagePath">Package path.</param>
        /// <param name="info">DLCPackageData.</param>
        /// <param name="platform">Target platform.</param>
        /// <param name="dlcType">Package type.</param>
        /// <param name="pnum">Packages left. Used to control art cache.</param>
        public static void Generate(string packagePath, DLCPackageData info, Platform platform, DLCPackageType dlcType = DLCPackageType.Song, int pnum = -1)
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
                        GeneratePsarcsForRS1(packPsarcStream, info, platform);
                        break;
                    case GameVersion.None:
                        throw new InvalidOperationException("Unexpected game version value");
                }

                var packageName = Path.GetFileNameWithoutExtension(packagePath).StripPlatformEndName();
                var songFileName = String.Format("{0}{1}", Path.Combine(Path.GetDirectoryName(packagePath), packageName), platform.GetPathName()[2]);

                // SAVE PACKAGE
                switch (platform.platform)
                {
                    case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        switch (platform.version)
                        {
                            case GameVersion.RS2014:
                                using (var fl = File.Create(songFileName + ".psarc"))
                                    packPsarcStream.CopyTo(fl);
                                break;
                            case GameVersion.RS2012:
                                using (var fl = File.Create(songFileName + ".dat"))
                                    RijndaelEncryptor.EncryptFile(packPsarcStream, fl, RijndaelEncryptor.DLCKey);
                                break;
                            default:
                                throw new InvalidOperationException("Unexpected game version value");
                        }
                        break;
                    case GamePlatform.XBox360:
                        BuildXBox360Package(songFileName, info, FILES_XBOX, platform.version, dlcType);
                        break;
                    case GamePlatform.PS3:
                        EncryptPS3EdatFiles(songFileName + ".psarc", platform);
                        break;
                }
            }

            packPsarc.Dispose();//test this too
            FILES_XBOX.Clear();
            FILES_PS3.Clear();
            DeleteTmpFiles(TMPFILES_SNG);
            if (pnum == 0)
            {
                DeleteTmpFiles(TMPFILES_ART);
            }
        }

        #region XBox360

        public static void BuildXBox360Package(string songFileName, DLCPackageData info, IEnumerable<string> xboxFiles, GameVersion gameVersion, DLCPackageType dlcType = DLCPackageType.Song)
        {
            LogRecord x = new LogRecord();
            RSAParams xboxRSA = info.SignatureType == PackageMagic.CON ? new RSAParams(new DJsIO(Resources.XBox360_KV, true)) : new RSAParams(StrongSigned.LIVE);
            CreateSTFS xboxSTFS = new CreateSTFS();
            xboxSTFS.HeaderData = info.GetSTFSHeader(gameVersion, dlcType);
            foreach (string file in xboxFiles)
                xboxSTFS.AddFile(file, Path.GetFileName(file));

            STFSPackage xboxPackage = new STFSPackage(xboxSTFS, xboxRSA, songFileName, x);
            var generated = xboxPackage.RebuildPackage(xboxRSA);
            if (!generated)
                throw new InvalidOperationException("Error on create XBox360 package, details: \n" + x.Log);

            xboxPackage.FlushPackage(xboxRSA);
            xboxPackage.CloseIO();

            DirectoryExtension.SafeDelete(XBOX_WORKDIR);
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

            hd.Publisher = String.Format("Custom Song Creator Toolkit ({0} beta)", ToolkitVersion.version);
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

        public static void EncryptPS3EdatFiles(string songFileName, Platform platform)
        {
            // Due to PS3 encryption limitation - replace spaces in fname with '_'
            if (Path.GetFileName(songFileName).Contains(" "))
                songFileName = Path.Combine(Path.GetDirectoryName(songFileName), Path.GetFileName(songFileName).Replace(" ", "_"));

            // Cleaning work dir, beware there is .psarcthat we need.
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
                        FILES_PS3[0] = Path.Combine(Path.GetDirectoryName(FILES_PS3[0]), Path.GetFileName(songFileName));

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

            if (encryptResult.IndexOf("No JDK or JRE is installed on your machine") > 0)
                throw new InvalidOperationException("You need install Java SE 7 (x86) or higher on your machine. The Java path should be in PATH Environment Variable:" + Environment.NewLine + Environment.NewLine + encryptResult);

            if (encryptResult.IndexOf("Encrypt all EDAT files successfully") < 0)
                throw new InvalidOperationException("Rebuilder error, please check if .edat files are created correctly and see output bellow:" + Environment.NewLine + Environment.NewLine + encryptResult);
        }

        #endregion

        #endregion

        #region Generate PSARC RS2014

        private static void GenerateRS2014SongPsarc(Stream output, DLCPackageData info, Platform platform, int pnum = -1)
        {
            // TODO: Benchmark processes and optimize speed
            dlcName = info.DLCKey.ToLower();
            packPsarc = new PSARC.PSARC();

            // Stream objects
            Stream soundStream = null,
                   soundPreviewStream = null,
                   rsenumerableRootStream = null,
                   rsenumerableSongStream = null;

            try
            {
                // ALBUM ART
                var ddsfiles = info.ArtFiles;

                if (ddsfiles == null)
                {
                    string albumArtPath;
                    if (File.Exists(info.AlbumArtPath))
                    {
                        albumArtPath = info.AlbumArtPath;
                    }
                    else
                    {
                        using (var albumArtStream = new MemoryStream(Resources.albumart2014_256))
                        {
                            albumArtPath = GeneralExtensions.GetTempFileName(".dds");
                            albumArtStream.WriteFile(albumArtPath);
                            TMPFILES_ART.Add(albumArtPath);
                        }
                    }

                    ddsfiles = new List<DDSConvertedFile>();
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });

                    // Convert to DDS
                    ToDDS(ddsfiles);

                    // Save for reuse
                    info.ArtFiles = ddsfiles;
                }

                foreach (var dds in info.ArtFiles)
                {
                    packPsarc.AddEntry(String.Format("gfxassets/album_art/album_{0}_{1}.dds", dlcName, dds.sizeX), new FileStream(dds.destinationFile, FileMode.Open, FileAccess.Read, FileShare.Read));
                    TMPFILES_ART.Add(dds.destinationFile);
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
                    GenerateToolkitVersion(toolkitVersionStream, packageVersion: info.PackageVersion);
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
                    var audioFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.DLCKey, soundStream, soundbankStream, info.Volume, platform);
                    packPsarc.AddEntry(String.Format("audio/{0}/{1}.bnk", platform.GetPathName()[0].ToLower(), soundbankFileName), soundbankStream);
                    packPsarc.AddEntry(String.Format("audio/{0}/{1}.wem", platform.GetPathName()[0].ToLower(), audioFileNameId), soundStream);

                    // SOUNDBANK PREVIEW
                    var soundbankPreviewFileName = String.Format("song_{0}_preview", dlcName);
                    dynamic audioPreviewFileNameId;
                    var previewVolume = (float)(info.PreviewVolume ?? info.Volume);
                    if (File.Exists(previewAudioFile))
                        audioPreviewFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.DLCKey + "_Preview", soundPreviewStream, soundbankPreviewStream, previewVolume, platform, true);
                    else
                        audioPreviewFileNameId = SoundBankGenerator2014.GenerateSoundBank(info.DLCKey + "_Preview", soundPreviewStream, soundbankPreviewStream, info.Volume, platform, true, true);
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

                    foreach (var arrangement in info.Arrangements)
                    {
                        if (arrangement.ArrangementType == ArrangementType.ShowLight)
                            continue;

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
                        if (arrangement.ArrangementType != ArrangementType.Vocal)
                        {
                            attribute.SongPartition = songPartitionCount.GetSongPartition(arrangement.Name, arrangement.ArrangementType);
                            if (attribute.SongPartition > 1)
                            {
                                // Make the second arrangement with the same arrangement type as ALTERNATE arrangement ingame
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
                        using (var fs = File.OpenRead(shlArr.SongXml.File))
                            fs.CopyTo(showlightStream);
                    else
                    {
                        var showlight = new Showlights(info);
                        showlight.Serialize(showlightStream);
                        string shlFilePath = Path.Combine(Path.GetDirectoryName(info.Arrangements[0].SongXml.File), String.Format("{0}_showlights.xml", "cst"));
                        using (FileStream file = new FileStream(shlFilePath, FileMode.Create, FileAccess.Write))
                            showlightStream.WriteTo(file);
                    }

                    if (showlightStream.CanRead && showlightStream.Length > 0 && info.Showlights)
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
                if (pnum == 0)
                    DeleteTmpFiles(TMPFILES_ART);
                DeleteTmpFiles(TMPFILES_SNG);
            }
        }

        private static void GenerateRS2014InlayPsarc(Stream output, DLCPackageData info, Platform platform)
        {
            dlcName = info.Inlay.DLCSixName;
            // TODO updateProgress remotely from here 
            {
                packPsarc = new PSARC.PSARC();

                // Stream objects
                Stream rsenumerableRootStream = null,
                       rsenumerableGuitarStream = null;

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
                                inlayPath = GeneralExtensions.GetTempFileName(".png");
                                inlayStream.WriteFile(inlayPath);
                                TMPFILES_ART.Add(inlayPath);
                            }
                        }

                        ddsfiles = new List<DDSConvertedFile>();
                        ddsfiles.Add(new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = iconPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                        ddsfiles.Add(new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = iconPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                        ddsfiles.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = iconPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                        ddsfiles.Add(new DDSConvertedFile() { sizeX = 512, sizeY = 512, sourceFile = iconPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                        ddsfiles.Add(new DDSConvertedFile() { sizeX = 1024, sizeY = 512, sourceFile = inlayPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });

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
                        GenerateAppId(appIdStream, info.AppId, platform);
                        packPsarc.AddEntry("APP_ID", appIdStream);
                    }

                    packageListWriter.WriteLine(info.DLCKey);

                    GenerateSongPsarcRS1(songPsarcStream, info, platform);
                    string songFileName = String.Format("{0}.psarc", info.DLCKey);
                    packPsarc.AddEntry(songFileName, songPsarcStream);
                    songPsarcStream.WriteTmpFile(songFileName, platform);

                    for (int i = 0; i < info.Tones.Count; i++)
                    {
                        var tone = info.Tones[i];
                        var tonePsarcStream = new MemoryStream();
                        // TODO: generate single tone.manifest.json file that has multiple tones
                        // currently generating multiple tone.manifest.json files
                        toneStreams.Add(tonePsarcStream);
                        var toneKey = info.DLCKey + "_" + tone.Name == null ? "Default" : tone.Name.Replace(' ', '_');
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

        private static void GenerateSongPsarcRS1(Stream output, DLCPackageData info, Platform platform)
        {
            var soundBankName = String.Format("Song_{0}", info.DLCKey);

            try
            {
                Stream albumArtStream = null,
                       audioStream = null;

                string albumArtPath;
                if (File.Exists(info.AlbumArtPath))
                {
                    albumArtPath = info.AlbumArtPath;
                }
                else
                {
                    using (var defaultArtStream = new MemoryStream(Resources.albumart))
                    {
                        albumArtPath = GeneralExtensions.GetTempFileName(".dds");
                        defaultArtStream.WriteFile(albumArtPath);
                        defaultArtStream.Dispose();
                        TMPFILES_ART.Add(albumArtPath);
                    }
                }

                var ddsfiles = info.ArtFiles;
                if (ddsfiles == null)
                {
                    ddsfiles = new List<DDSConvertedFile>();
                    ddsfiles.Add(new DDSConvertedFile() { sizeX = 512, sizeY = 512, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
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

                    foreach (var x in info.Arrangements)
                    {
                        //Generate sng file in execution time
                        GenerateSNG(x, platform);

                        manifestBuilder.AggregateGraph.SongFiles.Add(x.SongFile);
                        manifestBuilder.AggregateGraph.SongXMLs.Add(x.SongXml);
                    }
                    manifestBuilder.AggregateGraph.XBlock = new XBlockFile { File = info.DLCKey + ".xblock" };
                    manifestBuilder.AggregateGraph.Write(info.DLCKey, platform.GetPathName(), platform, aggregateGraphStream);
                    aggregateGraphStream.Flush();
                    aggregateGraphStream.Seek(0, SeekOrigin.Begin);

                    {
                        var manifestData = manifestBuilder.GenerateManifest(info.DLCKey, info.Arrangements, info.SongInfo, platform);
                        var writer = new StreamWriter(manifestStream);
                        writer.Write(manifestData);
                        writer.Flush();
                        manifestStream.Seek(0, SeekOrigin.Begin);
                    }

                    GameXblock<Entity>.Generate(info.DLCKey, manifestBuilder.Manifest, manifestBuilder.AggregateGraph, xblockStream);
                    xblockStream.Flush();
                    xblockStream.Seek(0, SeekOrigin.Begin);

                    var soundFileName = SoundBankGenerator.GenerateSoundBank(info.DLCKey, soundStream, soundbankStream, info.Volume, platform);
                    soundbankStream.Flush();
                    soundbankStream.Seek(0, SeekOrigin.Begin);

                    GenerateSongPackageId(packageIdStream, info.DLCKey);

                    var songPsarc = new PSARC.PSARC();
                    songPsarc.AddEntry("PACKAGE_ID", packageIdStream);
                    songPsarc.AddEntry("AggregateGraph.nt", aggregateGraphStream);
                    songPsarc.AddEntry("Manifests/songs.manifest.json", manifestStream);
                    songPsarc.AddEntry(String.Format("Exports/Songs/{0}.xblock", info.DLCKey), xblockStream);
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
                    songPsarc.Write(output, false);
                }
            }
            finally
            {
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
            //TODO: there is an option to use NVTT lib: supports Win, Mac, Linux
            foreach (var item in filesToConvert)
                GeneralExtensions.RunExternalExecutable("nvdxt.exe", true, true, true, String.Format(args, item.sourceFile, item.destinationFile, item.sizeX, item.sizeY));
        }

        public static void GenerateToolkitVersion(Stream output, string packageAuthor = null, string packageVersion = null)
        {
            if (String.IsNullOrEmpty(packageAuthor))
                packageAuthor = ConfigRepository.Instance()["general_defaultauthor"];

            var writer = new StreamWriter(output);
            writer.WriteLine("Toolkit version: {0}", ToolkitVersion.version);
            if (!String.IsNullOrEmpty(packageAuthor))
                writer.WriteLine("Package Author: {0}", packageAuthor);
            if (!String.IsNullOrEmpty(packageVersion))
                writer.Write("Package Version: {0}", packageVersion);

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
                    SngFileWriter.Write(arr.SongXml.File, sngFile, arr.ArrangementType, platform);
                    break;
                case GameVersion.RS2014:
                    if (arr.Sng2014 == null)
                    {
                        // Sng2014File can be reused when generating for multiple platforms
                        // cache results
                        //
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

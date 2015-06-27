using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.Manifest.Functions;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using X360.STFS;
using X360.Other;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.DLCPackage.Manifest;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class Packer
    {
        #region FIELDS

        public const string ROOT_XBox360 = "Root";
        private static readonly string PS3_WORKDIR = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "edat");

        #endregion

        #region PACK

        public static void Pack(string sourcePath, string saveFileName, bool updateSng = false, Platform predefinedPlatform = null, bool updateManifest = false)
        {
            DeleteFixedAudio(sourcePath);
            Platform platform = sourcePath.GetPlatform();

            if (predefinedPlatform != null && predefinedPlatform.platform != GamePlatform.None && predefinedPlatform.version != GameVersion.None)
                platform = predefinedPlatform;

            switch (platform.platform)
            {
                case GamePlatform.Pc:
                case GamePlatform.Mac:
                    if (platform.version == GameVersion.RS2012)
                        PackPC(sourcePath, saveFileName, true, updateSng);
                    else if (platform.version == GameVersion.RS2014)
                        Pack2014(sourcePath, saveFileName, platform, updateSng, updateManifest);
                    break;
                case GamePlatform.XBox360:
                    PackXBox360(sourcePath, saveFileName, platform, updateSng, updateManifest);
                    break;
                case GamePlatform.PS3:
                    PackPS3(sourcePath, saveFileName, platform, updateSng, updateManifest);
                    break;
                case GamePlatform.None:
                    throw new InvalidOperationException(String.Format("Invalid directory structure of package. {0}Directory: {1}", Environment.NewLine, sourcePath));
            }
        }

        #endregion

        #region UNPACK

        public static string Unpack(string sourceFileName, string savePath, Platform predefinedPlatform)
        {
            return Unpack(sourceFileName, savePath, false, false, true, predefinedPlatform);
        }

        /// <summary>
        /// Unpack the specified File, returns unpacked dir.
        /// </summary>
        /// <param name="sourceFileName">Source file path.</param>
        /// <param name="savePath">Save path.</param>
        /// <param name="decodeAudio">If set to <c>true</c> decode audio.</param>
        /// <param name="extractSongXml">If set to <c>true</c> extract song xml from sng.</param>
        /// <param name="overwriteSongXml">If set to <c>true</c> overwrite existing song xml with produced.</param>
        /// <param name="predefinedPlatform">Predefined source platform.</param>
        public static string Unpack(string sourceFileName, string savePath, bool decodeAudio = false, bool extractSongXml = false, bool overwriteSongXml = true, Platform predefinedPlatform = null)
        {
            Platform platform = sourceFileName.GetPlatform();
            if (predefinedPlatform != null && predefinedPlatform.platform != GamePlatform.None && predefinedPlatform.version != GameVersion.None)
                platform = predefinedPlatform;
            var fnameWithoutExt = Path.GetFileNameWithoutExtension(sourceFileName);
            if (platform.platform == GamePlatform.PS3)
                fnameWithoutExt = fnameWithoutExt.Substring(0, fnameWithoutExt.LastIndexOf("."));
            var unpackedDir = Path.Combine(savePath, String.Format("{0}_{1}", fnameWithoutExt, platform.platform));
            if (Directory.Exists(unpackedDir))
                DirectoryExtension.SafeDelete(unpackedDir);

            var useCryptography = platform.version == GameVersion.RS2012; // Cryptography way is used only for PC in Rocksmith 1
            switch (platform.platform)
            {
                case GamePlatform.Pc:
                case GamePlatform.Mac:
                    if (platform.version == GameVersion.RS2014)
                        using (var inputStream = File.OpenRead(sourceFileName))
                            ExtractPSARC(sourceFileName, savePath, inputStream, platform);
                    else
                    {
                        using (var inputFileStream = File.OpenRead(sourceFileName))
                        using (var inputStream = new MemoryStream())
                        {
                            if (useCryptography)
                                RijndaelEncryptor.DecryptFile(inputFileStream, inputStream, RijndaelEncryptor.DLCKey);
                            else
                                inputFileStream.CopyTo(inputStream);

                            ExtractPSARC(sourceFileName, savePath, inputStream, platform);
                        }
                    }
                    break;
                case GamePlatform.XBox360:
                    UnpackXBox360Package(sourceFileName, savePath, platform);
                    break;
                case GamePlatform.PS3:
                    UnpackPS3Package(sourceFileName, savePath, platform);
                    break;
                case GamePlatform.None:
                    throw new InvalidOperationException("Platform not found :(");
            }

            // DECODE AUDIO
            if (decodeAudio)
            {
                var audioFiles = Directory.EnumerateFiles(unpackedDir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wem"));
                foreach (var file in audioFiles)
                {
                    var outputAudioFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), ".ogg"));
                    OggFile.Revorb(file, outputAudioFileName, Path.GetDirectoryName(Application.ExecutablePath), Path.GetExtension(file).GetWwiseVersion());
                }
            }

            // EXTRACT XML FROM SNG
            if (extractSongXml && platform.version == GameVersion.RS2014)
            {
                var sngFiles = Directory.EnumerateFiles(unpackedDir, "*.sng", SearchOption.AllDirectories);

                foreach (var sngFile in sngFiles)
                {
                    var xmlOutput = Path.Combine(Path.GetDirectoryName(sngFile), String.Format("{0}.xml", Path.GetFileNameWithoutExtension(sngFile)));
                    xmlOutput = xmlOutput.Replace(String.Format("bin{0}{1}", Path.DirectorySeparatorChar, platform.GetPathName()[1].ToLower()), "arr");

                    if (File.Exists(xmlOutput) && !overwriteSongXml)
                        continue;

                    var arrType = ArrangementType.Guitar;
                    if (Path.GetFileName(xmlOutput).ToLower().Contains("vocal"))
                        arrType = ArrangementType.Vocal;

                    Attributes2014 att = null;
                    if (arrType != ArrangementType.Vocal)
                    {
                        var jsonFiles = Directory.EnumerateFiles(unpackedDir, String.Format("{0}.json", Path.GetFileNameWithoutExtension(sngFile)), SearchOption.AllDirectories).FirstOrDefault();
                        if (jsonFiles.Any() && !String.IsNullOrEmpty(jsonFiles))
                            att = Manifest2014<Attributes2014>.LoadFromFile(jsonFiles).Entries.ToArray()[0].Value.ToArray()[0].Value;
                    }

                    var sngContent = Sng2014File.LoadFromFile(sngFile, platform);
                    using (var outputStream = new FileStream(xmlOutput, FileMode.Create, FileAccess.ReadWrite))
                    {
                        dynamic xmlContent = null;

                        if (arrType == ArrangementType.Vocal)
                            xmlContent = new Vocals(sngContent);
                        else
                            xmlContent = new Song2014(sngContent, att);

                        xmlContent.Serialize(outputStream);
                    }
                }
            }

            return unpackedDir;
        }

        #endregion

        #region PC 2012

        private static void PackPC(string sourcePath, string saveFileName, bool useCryptography, bool updateSng)
        {
            foreach (var namesBlock in Directory.EnumerateFiles(sourcePath, "NamesBlock.bin", SearchOption.AllDirectories))
            {
                if (File.Exists(namesBlock))
                    File.Delete(namesBlock);
            }

            using (var psarcStream = new MemoryStream())
            using (var streamCollection = new DisposableCollection<Stream>())
            {
                var psarc = new PSARC.PSARC();
                foreach (var x in Directory.EnumerateFiles(sourcePath))
                {
                    var fileStream = File.OpenRead(x);
                    streamCollection.Add(fileStream);
                    var entry = new PSARC.Entry
                    {
                        Name = Path.GetFileName(x),
                        Data = fileStream,
                        Length = (ulong)fileStream.Length
                    };
                    psarc.AddEntry(entry);
                }

                foreach (var directory in Directory.EnumerateDirectories(sourcePath))
                {
                    var innerPsarcStream = new MemoryStream();
                    streamCollection.Add(innerPsarcStream);
                    var directoryName = Path.GetFileName(directory);

                    // Recreate SNG
                    if (updateSng)
                        if (directory.ToLower().IndexOf("dlc_tone_") < 0)
                            UpdateSng(directory, new Platform(GamePlatform.Pc, GameVersion.RS2012));

                    PackInnerPC(innerPsarcStream, directory);
                    psarc.AddEntry(directoryName + ".psarc", innerPsarcStream);
                }

                psarc.Write(psarcStream, false);
                psarcStream.Flush();
                psarcStream.Seek(0, SeekOrigin.Begin);

                if (Path.GetExtension(saveFileName) != ".dat")
                    saveFileName += ".dat";

                using (var outputFileStream = File.Create(saveFileName))
                {
                    if (useCryptography)
                        RijndaelEncryptor.EncryptFile(psarcStream, outputFileStream, RijndaelEncryptor.DLCKey);
                    else
                        psarcStream.CopyTo(outputFileStream);
                }
            }
        }

        private static void PackInnerPC(Stream output, string directory)
        {
            using (var streamCollection = new DisposableCollection<Stream>())
            {
                var innerPsarc = new PSARC.PSARC();
                WalkThroughDirectory("", directory, (a, b) =>
                {
                    var fileStream = File.OpenRead(b);
                    streamCollection.Add(fileStream);
                    innerPsarc.AddEntry(a, fileStream);
                });
                innerPsarc.Write(output, false);
            }
        }

        #endregion

        #region PC/MAC 2014

        private static void Pack2014(string sourcePath, string saveFileName, Platform platform, bool updateSng, bool updateManifest)
        {
            using (var psarc = new PSARC.PSARC())
            using (var psarcStream = new MemoryStream())
            {
                if (updateSng)
                    UpdateSng2014(sourcePath, platform);
                if (updateManifest)
                    UpdateManifest2014(sourcePath, platform);

                WalkThroughDirectory("", sourcePath, (a, b) =>
                {
                    var fileStream = File.OpenRead(b);
                    psarc.AddEntry(a, fileStream);
                });

                psarc.Write(psarcStream, !platform.IsConsole);
                psarcStream.Flush();
                psarcStream.Seek(0, SeekOrigin.Begin);

                if (Path.GetExtension(saveFileName) != ".psarc")
                    saveFileName += ".psarc";

                using (var outputFileStream = File.Create(saveFileName))
                    psarcStream.CopyTo(outputFileStream);
            }
        }

        #endregion

        #region XBox 360

        private static void PackXBox360(string sourcePath, string saveFileName, Platform platform, bool updateSng, bool updateManifest)
        {
            if (updateSng && platform.version == GameVersion.RS2014)
            {
                UpdateSng2014(sourcePath, platform);
                UpdateManifest2014(sourcePath, platform);
            }

            var songData = new DLCPackageData();
            var packageRoot = Path.Combine(sourcePath, ROOT_XBox360);

            // If 'Root' directory doesn't exist the packing is a conversion process from another platform
            if (!Directory.Exists(packageRoot))
            {
                var songXmlFiles = Directory.EnumerateFiles(sourcePath, "*_*.xml", SearchOption.AllDirectories);

                var songTitle = String.Empty;
                foreach (var xml in songXmlFiles)
                {
                    if (Path.GetFileNameWithoutExtension(xml).ToLower().Contains("vocal") || Path.GetFileNameWithoutExtension(xml).ToLower().Contains("showlight"))
                        continue;

                    var song = Song2014.LoadFromFile(xml);

                    songData.SongInfo = new SongInfo();
                    songData.SongInfo.SongDisplayName = songTitle = song.Title;
                    songData.SongInfo.Artist = song.ArtistName;

                    songData.SignatureType = PackageMagic.CON;
                    break;
                }

                var directoryList = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
                var fileList = Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories);

                // MAKE THE XBOX360 EXPECTED STRUCTURE TO PACK WORK
                var newPackageName = songTitle.GetValidSongName(songTitle).ToLower();
                var newSongDir = Path.Combine(packageRoot, newPackageName);

                // Creating new directories
                Directory.CreateDirectory(packageRoot);
                Directory.CreateDirectory(newSongDir);

                // Create PackageList file
                var packListFile = Path.Combine(packageRoot, "PackageList.txt");
                File.WriteAllText(packListFile, newPackageName);

                // Move directories to new path
                foreach (string dir in directoryList)
                    Directory.CreateDirectory(dir.Replace(sourcePath, newSongDir));

                // Move files to new path
                foreach (string file in fileList)
                    File.Move(file, file.Replace(sourcePath, newSongDir));

                // Delete old empty directories
                foreach (string emptyDir in directoryList)
                    DirectoryExtension.SafeDelete(emptyDir);
            }

            foreach (var directory in Directory.EnumerateDirectories(packageRoot))
            {
                PackInnerXBox360(packageRoot, directory);
            }

            IEnumerable<string> xboxHeaderFiles = Directory.EnumerateFiles(sourcePath, "*.txt", SearchOption.TopDirectoryOnly);
            if (xboxHeaderFiles.Count() == 1)
            {
                foreach (var file in xboxHeaderFiles)
                {
                    try
                    {
                        string[] xboxHeader = File.ReadAllLines(file);
                        if (xboxHeader != null && xboxHeader.Length > 73)
                        {
                            if (xboxHeader[0].IndexOf("LIVE") > 0)
                            {
                                songData.SignatureType = PackageMagic.LIVE;

                                for (int i = 2; i <= 48; i = i + 3)
                                {
                                    long id = Convert.ToInt64(xboxHeader[i].GetHeaderValue(), 16);
                                    int bit = Convert.ToInt32(xboxHeader[i + 1].GetHeaderValue());
                                    int flag = Convert.ToInt32(xboxHeader[i + 2].GetHeaderValue());

                                    if (id != 0)
                                        songData.XBox360Licenses.Add(new XBox360License() { ID = id, Bit = bit, Flag = flag });
                                }
                            }

                            string songInfo = xboxHeader[74];

                            int index = songInfo.IndexOf(" by ");
                            string songTitle = (index > 0) ? songInfo.Substring(0, index) : songInfo;
                            string songArtist = (index > 4) ? songInfo.Substring(index + 4) : songInfo;

                            if (!String.IsNullOrEmpty(songInfo))
                            {
                                songData.SongInfo = new SongInfo();
                                songData.SongInfo.SongDisplayName = songInfo;
                                songData.SongInfo.Artist = songInfo;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException("XBox360 header file (.txt) not found or is invalid. " + Environment.NewLine +
                                                       "The file is in the same level at 'Root' folder along with the files: 'Content image.png' and 'Package image.png' and no other file .txt can be here.", ex);
                    }
                }
            }

            IEnumerable<string> xboxFiles = Directory.EnumerateFiles(packageRoot);
            DLCPackageCreator.BuildXBox360Package(saveFileName, songData, xboxFiles, platform.version);

            foreach (var file in xboxFiles)
                if (Path.GetExtension(file) == ".psarc" && File.Exists(file))
                    File.Delete(file);
        }

        private static void PackInnerXBox360(string sourcePath, string directory)
        {
            using (var psarcStream = new MemoryStream())
            {
                var innerPsarc = new PSARC.PSARC();
                WalkThroughDirectory("", directory, (a, b) =>
                {
                    var fileStream = File.OpenRead(b);
                    innerPsarc.AddEntry(a, fileStream);
                });

                innerPsarc.Write(psarcStream, false);
                psarcStream.Flush();

                using (var outputFileStream = File.Create(Path.Combine(sourcePath, Path.GetFileName(directory)) + ".psarc"))
                {
                    psarcStream.Seek(0, SeekOrigin.Begin);
                    psarcStream.CopyTo(outputFileStream);
                }
            }
        }

        private static void UnpackXBox360Package(string sourceFileName, string savePath, Platform platform)
        {
            LogRecord x = new LogRecord();
            STFSPackage xboxPackage = new STFSPackage(sourceFileName, x);
            if (!xboxPackage.ParseSuccess)
                throw new InvalidDataException("Invalid Rocksmith XBox 360 package!" + Environment.NewLine + x.Log);

            var rootDir = Path.Combine(savePath, Path.GetFileNameWithoutExtension(sourceFileName)) + String.Format("_{0}", platform.platform.ToString());
            xboxPackage.ExtractPayload(rootDir, true, true);

            foreach (var fileName in Directory.EnumerateFiles(Path.Combine(rootDir, ROOT_XBox360)))
            {
                if (Path.GetExtension(fileName) == ".psarc")
                {
                    using (var outputFileStream = File.OpenRead(fileName))
                    {
                        ExtractPSARC(fileName, Path.GetDirectoryName(fileName), outputFileStream, new Platform(GamePlatform.XBox360, GameVersion.None), false);
                    }
                }

                if (File.Exists(fileName) && Path.GetExtension(fileName) == ".psarc")
                    File.Delete(fileName);
            }

            xboxPackage.CloseIO();
        }

        private static string GetHeaderValue(this string value)
        {
            return value.Substring(value.IndexOf(":") + 2);
        }

        #endregion

        #region PS3

        private static void PackPS3(string sourcePath, string saveFileName, Platform platform, bool updateSng, bool updateManifest)
        {
            Pack2014(sourcePath, saveFileName, platform, updateSng, updateManifest);

            if (!Directory.Exists(PS3_WORKDIR))
                Directory.CreateDirectory(PS3_WORKDIR);

            foreach (var junk in Directory.EnumerateFiles(PS3_WORKDIR, "*.*"))
                File.Delete(junk);

            var sourceCleanPackage = saveFileName + ".psarc";
            var destCleanPackage = Path.Combine(PS3_WORKDIR, Path.GetFileName(saveFileName) + ".psarc");
            var encryptedPackage = destCleanPackage + ".edat";

            if (File.Exists(sourceCleanPackage))
                File.Move(sourceCleanPackage, destCleanPackage);

            var outputMessage = RijndaelEncryptor.EncryptPS3Edat();

            if (outputMessage.IndexOf("Encrypt all EDAT files successfully") > 0)
            {
                if (File.Exists(destCleanPackage))
                    File.Delete(destCleanPackage);

                if (File.Exists(encryptedPackage))
                    File.Move(encryptedPackage, sourceCleanPackage + ".edat");
            }
        }

        private static void UnpackPS3Package(string sourceFileName, string savePath, Platform platform)
        {
            var outputFilename = Path.Combine(PS3_WORKDIR, Path.GetFileName(sourceFileName));

            if (!Directory.Exists(PS3_WORKDIR))
                Directory.CreateDirectory(PS3_WORKDIR);

            foreach (var junk in Directory.EnumerateFiles(PS3_WORKDIR, "*.*"))
                File.Delete(junk);

            if (File.Exists(sourceFileName))
                File.Copy(sourceFileName, outputFilename, true);
            else
                throw new FileNotFoundException(String.Format("File '{0}' not found.", sourceFileName));

            var outputMessage = RijndaelEncryptor.DecryptPS3Edat();

            if (File.Exists(outputFilename))
                File.Delete(outputFilename);

            foreach (var fileName in Directory.EnumerateFiles(PS3_WORKDIR, "*.psarc.dat"))
            {
                using (var outputFileStream = File.OpenRead(fileName))
                {
                    ExtractPSARC(fileName, Path.GetDirectoryName(fileName), outputFileStream, new Platform(GamePlatform.PS3, GameVersion.None));
                }

                if (File.Exists(fileName))
                    File.Delete(fileName);
            }

            var outName = Path.GetFileNameWithoutExtension(sourceFileName);
            var outputDir = Path.Combine(savePath, outName.Substring(0, outName.LastIndexOf(".")) + String.Format("_{0}", platform.platform.ToString()));

            foreach (var unpackedDir in Directory.EnumerateDirectories(PS3_WORKDIR))
                if (Directory.Exists(unpackedDir))
                {
                    if (Directory.Exists(outputDir))
                        DirectoryExtension.SafeDelete(outputDir);

                    DirectoryExtension.Move(unpackedDir, outputDir);
                }

            if (outputMessage.IndexOf("Decrypt all EDAT files successfully") < 0)
                throw new InvalidOperationException("Rebuilder error, please check if .edat files are created correctly and see output below:" + Environment.NewLine + Environment.NewLine + outputMessage);
        }

        #endregion

        #region COMMON FUNCTIONS

        public static void DeleteFixedAudio(string sourcePath)
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(sourcePath, "*_fixed.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wem")))
                    if (File.Exists(file)) File.Delete(file);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Can't delete garbage Audio files!\r\n {0}", ex));
            }
        }

        private static void WalkThroughDirectory(string baseDir, string directory, Action<string, string> action)
        {
            foreach (var fl in Directory.EnumerateFiles(directory))
                action(String.Format("{0}/{1}", baseDir, Path.GetFileName(fl)).TrimStart('/'), fl);

            foreach (var dr in Directory.EnumerateDirectories(Path.Combine(baseDir, directory)))
                WalkThroughDirectory(String.Format("{0}/{1}", baseDir, Path.GetFileName(dr)), dr, action);
        }

        public static Platform GetPlatform(this string fullPath)
        {//TODO: validate Files by MIME type.
            if (File.Exists(fullPath))
            {
                // Get PLATFORM by Extension + Get PLATFORM by pkg EndName
                switch (Path.GetExtension(fullPath))
                {
                    case ".dat":
                        return new Platform(GamePlatform.Pc, GameVersion.RS2012);
                    case "":
                        return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                    case ".edat":
                        return new Platform(GamePlatform.PS3, GameVersion.None);
                    case ".psarc":
                        return TryGetPlatformByEndName(fullPath);
                    default:
                        return new Platform(GamePlatform.None, GameVersion.None);
                }
            }
            else if (Directory.Exists(fullPath))
            {
                var fullPathInfo = new DirectoryInfo(fullPath);
                // GET PLATFORM BY PACKAGE ROOT DIRECTORY
                if (File.Exists(Path.Combine(fullPath, "APP_ID")))
                {
                    // PC 2012
                    return new Platform(GamePlatform.Pc, GameVersion.RS2012);
                }
                else if (File.Exists(Path.Combine(fullPath, "appid.appid")))
                {
                    // PC / MAC 2014
                    var agg = fullPathInfo.EnumerateFiles("*.nt", SearchOption.TopDirectoryOnly).FirstOrDefault().FullName;
                    var aggContent = File.ReadAllText(agg);

                    if (aggContent.Contains("\"dx9\""))
                        return new Platform(GamePlatform.Pc, GameVersion.RS2014);
                    else if (aggContent.Contains("\"macos\""))
                        return new Platform(GamePlatform.Mac, GameVersion.RS2014);
                    else
                        return new Platform(GamePlatform.Pc, GameVersion.RS2014); // Because appid.appid have only in RS2014
                }
                else if (Directory.Exists(Path.Combine(fullPath, ROOT_XBox360)))
                {
                    // XBOX 2012/2014
                    var hTxt = fullPathInfo.EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly).FirstOrDefault().FullName;
                    var hTxtContent = File.ReadAllText(hTxt);

                    if (hTxtContent.Contains("Title ID: 55530873"))
                        return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                    else if (hTxtContent.Contains("Title ID: 555308C0"))
                        return new Platform(GamePlatform.XBox360, GameVersion.RS2014);
                    else
                        return new Platform(GamePlatform.XBox360, GameVersion.None);
                }
                else
                {
                    // PS3 2012/2014
                    var agg = fullPathInfo.EnumerateFiles("*.nt", SearchOption.TopDirectoryOnly).FirstOrDefault().FullName;

                    if (agg.Length > 0)
                    {
                        var aggContent = File.ReadAllText(agg);

                        if (aggContent.Contains("\"PS3\""))
                            return new Platform(GamePlatform.PS3, GameVersion.RS2012);
                        else if (aggContent.Contains("\"ps3\""))
                            return new Platform(GamePlatform.PS3, GameVersion.RS2014);
                        else
                            return TryGetPlatformByEndName(fullPath);
                    }
                    else
                        return TryGetPlatformByEndName(fullPath);
                }
            }
            else
                return new Platform(GamePlatform.None, GameVersion.None);
        }

        /// <summary>
        /// Gets platform from name ending
        /// </summary>
        /// <param name="fileName">Folder of File</param>
        /// <returns>Platform(DetectedPlatform, RS2014 ? None)</returns>
        public static Platform TryGetPlatformByEndName(string fileName)
        {
            GamePlatform p = GamePlatform.None;
            GameVersion v = GameVersion.RS2014;
            var pIndex = Path.GetFileNameWithoutExtension(fileName).LastIndexOf("_");

            if (Directory.Exists(fileName))
            {// Pc, Mac, XBox360, PS3
                string platformString = Path.GetFileNameWithoutExtension(fileName).Substring(pIndex + 1);
                bool isValid = Enum.TryParse(platformString, true, out p);
                if (isValid) return new Platform(p, v);
                else return new Platform(GamePlatform.None, GameVersion.None);
            }
            else
            {//_p, _m, _ps3, _xbox
                string platformString = pIndex > -1 ? Path.GetFileNameWithoutExtension(fileName).Substring(pIndex) : "";
                switch (platformString.ToLower())
                {
                    case "_p":
                        return new Platform(GamePlatform.Pc, v);
                    case "_m":
                        return new Platform(GamePlatform.Mac, v);
                    case "_ps3":
                        return new Platform(GamePlatform.PS3, v);
                    case "_xbox":
                        return new Platform(GamePlatform.XBox360, v);
                    default:
                        return new Platform(GamePlatform.Pc, v);
                }
            }
        }

        private static void ExtractPSARC(string filename, string savePath, Stream inputStream, Platform platform, bool isExternalFile = true)
        {
            string psarcFilename = Path.GetFileNameWithoutExtension(filename);
            if (isExternalFile)
                psarcFilename += String.Format("_{0}", platform.platform);

            var destpath = Path.Combine(savePath, psarcFilename);
            if (Directory.Exists(destpath) && isExternalFile)
                DirectoryExtension.SafeDelete(destpath);

            var psarc = new PSARC.PSARC();
            psarc.Read(inputStream, true);
            foreach (var entry in psarc.TOC)
            {// custom InflateEntries
                var fullfilename = Path.Combine(destpath, entry.Name);

                if (Path.GetExtension(entry.Name).ToLower() == ".psarc")
                {
                    psarc.InflateEntry(entry);
                    ExtractPSARC(fullfilename, destpath, entry.Data, platform, false);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullfilename));
                    psarc.InflateEntry(entry, fullfilename);
                    if (entry.Data != null)
                        entry.Data.Close();
                }

                if (!String.IsNullOrEmpty(psarc.ErrMSG)) throw new InvalidDataException(psarc.ErrMSG);
            }
        }

        private static void UpdateSng(string songDirectory, Platform platform)
        {
            var xmlFiles = Directory.EnumerateFiles(Path.Combine(songDirectory, @"GR\Behaviors\Songs"));

            foreach (var xmlFile in xmlFiles)
            {
                if (File.Exists(xmlFile) && Path.GetExtension(xmlFile) == ".xml")
                {
                    var sngFile = Path.Combine(songDirectory, "GRExports", platform.GetPathName()[1], Path.GetFileNameWithoutExtension(xmlFile) + ".sng");
                    var arrType = ArrangementType.Guitar;

                    if (Path.GetFileName(xmlFile).ToLower().Contains("vocal"))
                    {
                        arrType = ArrangementType.Vocal;
                        SngFileWriter.Write(xmlFile, sngFile, arrType, platform);
                    }
                    else
                    {
                        Song song = Song.LoadFromFile(xmlFile);

                        if (!Enum.TryParse<ArrangementType>(song.Arrangement, out arrType))
                            if (song.Arrangement.ToLower().Contains("bass"))
                                arrType = ArrangementType.Bass;
                    }

                    SngFileWriter.Write(xmlFile, sngFile, arrType, platform);
                }
                else
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid XML file.", xmlFile));
                }
            }
        }

        private static void UpdateSng2014(string songDirectory, Platform targetPlatform)
        {
            var xmlFiles = Directory.EnumerateFiles(Path.Combine(songDirectory, "songs", "arr"), "*_*.xml", SearchOption.AllDirectories).ToList();
            var sngFolder = Path.Combine(songDirectory, "songs", "bin", targetPlatform.GetPathName()[1]); //-3 or more times re-calculation
            foreach (var xmlFile in xmlFiles)
            {
                if (File.Exists(xmlFile))
                {
                    var xmlName = Path.GetFileNameWithoutExtension(xmlFile);
                    bool noShowlights = true;

                    //Update Showlights
                    if (xmlName.ToLower().Contains("_showlights"))
                        UpdateShl(xmlFile);
                    else
                    {
                        var sngFile = Path.Combine(sngFolder, xmlName + ".sng");
                        var arrType = ArrangementType.Guitar;
                        if (Path.GetFileName(xmlFile).ToLower().Contains("vocal"))
                            arrType = ArrangementType.Vocal;

                        // Handle custom fonts
                        string fontSng = null;
                        if (arrType == ArrangementType.Vocal)
                        {
                            var vocSng = Sng2014File.LoadFromFile(sngFile, TryGetPlatformByEndName(songDirectory));
                            if (vocSng.IsCustomFont())
                            {
                                vocSng.WriteChartData((fontSng = Path.GetTempFileName()), new Platform(GamePlatform.Pc, GameVersion.None));
                            }
                        }

                        using (var fs = new FileStream(sngFile, FileMode.Create))
                        {
                            var sng = Sng2014File.ConvertXML(xmlFile, arrType, fontSng);
                            sng.WriteSng(fs, targetPlatform);
                        }

                        noShowlights &= !xmlFiles.Any(x => Path.GetFileName(x).Contains(xmlName.Split('_')[0].ToLower() + "_showlights"));
                        //Create Showlights
                        if (noShowlights && arrType != ArrangementType.Vocal)
                        {
                            var shlName = Path.Combine(Path.GetDirectoryName(xmlFile), xmlName.Split('_')[0] + "_showlights.xml");
                            var shl = new Showlight.Showlights(xmlFile);
                            if (shl.FixShowlights(shl))
                            {
                                using (var fs = new FileStream(shlName, FileMode.Create))
                                    shl.Serialize(fs);
                            }
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Fixes showlights and updates existing xml.
        /// </summary>
        /// <param name="shlPath"></param>
        /// <returns></returns>
        internal static void UpdateShl(string shlPath)
        {
            var shl = new Showlight.Showlights(shlPath);
            if (shl.FixShowlights(shl))
            {
                using (var fs = new FileStream(shlPath, FileMode.Create))
                    shl.Serialize(fs);
            }
        }

        private static void UpdateManifest2014(string songDirectory, Platform platform)
        {
            // UPDATE MANIFEST (RS2014)
            if (platform.version == GameVersion.RS2014)
            {
                var xmlFiles = Directory.EnumerateFiles(songDirectory, "*.xml", SearchOption.AllDirectories);
                var jsonFiles = Directory.EnumerateFiles(songDirectory, "*.json", SearchOption.AllDirectories);
                foreach (var xml in xmlFiles)
                {
                    var xmlName = Path.GetFileNameWithoutExtension(xml);
                    if (xmlName.ToUpperInvariant().Contains("SHOWLIGHT"))
                        continue;
                    if (xmlName.ToUpperInvariant().Contains("VOCAL"))
                        continue;//TODO: Re-generate vocals manifest.

                    string json = jsonFiles.FirstOrDefault(name => Path.GetFileNameWithoutExtension(name) == xmlName);
                    if (!String.IsNullOrEmpty(json))
                    {
                        var xmlContent = Song2014.LoadFromFile(xml);
                        var manifest = new Manifest2014<Attributes2014>();
                        var attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.First().Value.First().Value;

                        var manifestFunctions = new ManifestFunctions(platform.version);

                        attr.PhraseIterations = new List<Manifest.PhraseIteration>();
                        manifestFunctions.GeneratePhraseIterationsData(attr, xmlContent, platform.version);

                        attr.Phrases = new List<Manifest.Phrase>();
                        manifestFunctions.GeneratePhraseData(attr, xmlContent);

                        attr.Sections = new List<Manifest.Section>();
                        manifestFunctions.GenerateSectionData(attr, xmlContent);

                        attr.MaxPhraseDifficulty = manifestFunctions.GetMaxDifficulty(xmlContent);

                        var attributeDictionary = new Dictionary<string, Attributes2014> { { "Attributes", attr } };
                        manifest.Entries.Add(attr.PersistentID, attributeDictionary);
                        manifest.SaveToFile(json);
                    }
                }
            }
        }

        #endregion
    }
}

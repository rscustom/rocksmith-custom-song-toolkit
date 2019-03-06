using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest.Functions;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Header;
using RocksmithToolkitLib.XML;
using X360.STFS;
using X360.Other;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.XmlRepository;


namespace RocksmithToolkitLib.DLCPackage
{
    public static class Packer
    {
        #region CONSTANTS

        public const string ROOT_XBOX360 = "Root";
        public const string EDAT_MSG = "Encrypt all EDAT files successfully";
        // path fixed for unit testing compatiblity
        private static readonly string PS3_EDAT = Path.Combine(ExternalApps.TOOLKIT_ROOT, "edat");

        #endregion

        #region PACK
        /// <summary>
        /// Pack Song Artifacts into a Song Archive file
        /// </summary>
        /// <param name="srcPath">Unpacked Artifacts Directory Path</param>
        /// <param name="destPath">Full Destination Archive Path (including file name and extension)</param>
        /// <param name="predifinedPlatform">Desired Output Platform</param>
        /// <param name="updateSng">If set to <c>true</c> update the SNG files</param>
        /// <param name="updateManifest">If set to <c>true</c> update the manifest files</param>
        /// <returns>Archive Path</returns>
        public static string Pack(string srcPath, string destPath, Platform predefinedPlatform = null, bool updateSng = false, bool updateManifest = false)
        {
            var archivePath = String.Empty;
            ExternalApps.VerifyExternalApps();
            CleanupArtifacts(srcPath);
            Platform srcPlatform = srcPath.GetPlatform();

            if (predefinedPlatform != null && predefinedPlatform.platform != GamePlatform.None && predefinedPlatform.version != GameVersion.None)
                srcPlatform = predefinedPlatform;

            switch (srcPlatform.platform)
            {
                case GamePlatform.Pc:
                case GamePlatform.Mac:
                    if (srcPlatform.version == GameVersion.RS2012)
                        archivePath = PackPC(srcPath, destPath, true, updateSng);
                    else if (srcPlatform.version == GameVersion.RS2014)
                        archivePath = Pack2014(srcPath, destPath, srcPlatform, updateSng, updateManifest);
                    break;
                case GamePlatform.XBox360:
                    archivePath = PackXBox360(srcPath, destPath, srcPlatform, updateSng, updateManifest);
                    break;
                case GamePlatform.PS3:
                    archivePath = PackPS3(srcPath, destPath, srcPlatform, updateSng, updateManifest);
                    break;
                case GamePlatform.None:
                    throw new InvalidOperationException(String.Format("Invalid directory structure of package. {0}Directory: {1}", Environment.NewLine, srcPath));
            }

            return archivePath;
        }

        #endregion

        #region UNPACK

        /// <summary>
        /// Unpack Song Archive file to an Artifacts Directory
        /// </summary>
        /// <param name="srcPath">Full Source Archive Path</param>
        /// <param name="destDirPath">Unpacked Artifacts Directory Path</param>
        /// <param name="predefinedPlatform">Predefined source platform</param>
        /// <param name="decodeAudio">If set to <c>true</c> decode audio</param>
        /// <param name="overwriteSongXml">If set to <c>true</c> overwrite existing song (EOF) xml with SNG data</param>       
        /// <returns>Unpacked Directory Path</returns>
        public static string Unpack(string srcPath, string destDirPath, Platform predefinedPlatform = null, bool decodeAudio = false, bool overwriteSongXml = false)
        {
            ExternalApps.VerifyExternalApps();
            Platform srcPlatform = srcPath.GetPlatform();

            if (predefinedPlatform != null && predefinedPlatform.platform != GamePlatform.None && predefinedPlatform.version != GameVersion.None)
                srcPlatform = predefinedPlatform;

            // Cryptography RS1 ONLY            
            var useCryptography = srcPlatform.version == GameVersion.RS2012;
            var unpackedDir = String.Empty;

            switch (srcPlatform.platform)
            {
                case GamePlatform.Pc:
                case GamePlatform.Mac:
                    if (srcPlatform.version == GameVersion.RS2014)
                        using (var inputStream = File.OpenRead(srcPath))
                        {
                            unpackedDir = ExtractPSARC(srcPath, destDirPath, inputStream, srcPlatform);
                        }
                    else
                    {
                        using (var inputFileStream = File.OpenRead(srcPath))
                        using (var inputStream = new MemoryStream())
                        {
                            if (useCryptography)
                                RijndaelEncryptor.DecryptFile(inputFileStream, inputStream, RijndaelEncryptor.DLCKey);
                            else
                                inputFileStream.CopyTo(inputStream);

                            unpackedDir = ExtractPSARC(srcPath, destDirPath, inputStream, srcPlatform);
                        }
                    }
                    break;
                case GamePlatform.XBox360:
                    unpackedDir = UnpackXBox360Package(srcPath, destDirPath, srcPlatform);
                    break;
                case GamePlatform.PS3:
                    unpackedDir = UnpackPS3Package(srcPath, destDirPath, srcPlatform);
                    break;
                case GamePlatform.None:
                    throw new InvalidOperationException("Platform not found :(");
            }

            // ODLC status
            var isODLC = !Directory.EnumerateFiles(unpackedDir, "toolkit.version", SearchOption.AllDirectories).Any();

            // DECODE AUDIO
            if (decodeAudio)
            {
                GlobalExtension.ShowProgress("Decoding Audio ...", 50);
                var audioFiles = Directory.EnumerateFiles(unpackedDir, "*", SearchOption.AllDirectories).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wem"));
                foreach (var file in audioFiles)
                {
                    var outputAudioFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), ".ogg"));
                    // path fixed for unit testing compatiblity
                    OggFile.Revorb(file, outputAudioFileName, Path.GetExtension(file).GetWwiseVersion());
                }

                // convert album artwork dds to common friendly png
                var ddsFiles = Directory.EnumerateFiles(unpackedDir, "*.dds", SearchOption.AllDirectories).ToList();
                if (ddsFiles.Any())
                {
                    // use the best quality (largest) dds file for the conversion
                    FileInfo[] fileInfos = ddsFiles.Select(fi => new FileInfo(fi)).ToArray();
                    var ddsFile = fileInfos.OrderByDescending(fl => fl.Length).Select(fn => fn.FullName).FirstOrDefault();
                    ExternalApps.Dds2Png(ddsFile);
                }
            }

            // for debugging
            //overwriteSongXml = false;

            // PERFORM QUALITY CONTROL CHECKS
            // Extract XML from SNG and check it against the EOF XML (correct bass tuning from older toolkit/EOF xml files)
            if (srcPlatform.version == GameVersion.RS2014)
            {
                var sngFiles = Directory.EnumerateFiles(unpackedDir, "*.sng", SearchOption.AllDirectories).ToList();
                var step = Math.Round(1.0 / sngFiles.Count * 100, 3);
                double progress = 0;
                GlobalExtension.ShowProgress("Validating XML files ...");

                foreach (var sngFile in sngFiles)
                {
                    var xmlEofFile = Path.Combine(Path.GetDirectoryName(sngFile), String.Format("{0}.xml", Path.GetFileNameWithoutExtension(sngFile)));
                    xmlEofFile = xmlEofFile.Replace(String.Format("bin{0}{1}", Path.DirectorySeparatorChar, srcPlatform.GetPathName()[1].ToLower()), "arr");
                    var xmlSngFile = xmlEofFile.Replace(".xml", ".sng.xml");
                    var arrType = ArrangementType.Guitar;

                    if (Path.GetFileName(xmlSngFile).ToLower().Contains("vocal"))
                        arrType = ArrangementType.Vocal;

                    Attributes2014 att = null;
                    if (arrType != ArrangementType.Vocal)
                    {
                        // Some ODLC json files contain factory errors
                        // Confirmed error in Chords (too many chords are reported in some difficulty levels)
                        var jsonFiles = Directory.EnumerateFiles(unpackedDir, String.Format("{0}.json", Path.GetFileNameWithoutExtension(sngFile)), SearchOption.AllDirectories).FirstOrDefault();
                        if (!String.IsNullOrEmpty(jsonFiles) && jsonFiles.Any())
                            att = Manifest2014<Attributes2014>.LoadFromFile(jsonFiles).Entries.ToArray()[0].Value.ToArray()[0].Value;
                    }

                    // create the xml file from sng file
                    var sngContent = Sng2014File.LoadFromFile(sngFile, srcPlatform);
                    using (var outputStream = new FileStream(xmlSngFile, FileMode.Create, FileAccess.ReadWrite))
                    {
                        dynamic xmlContent = null;

                        if (arrType == ArrangementType.Vocal)
                            xmlContent = new Vocals(sngContent);
                        else
                            xmlContent = new Song2014(sngContent, att);

                        xmlContent.Serialize(outputStream);
                    }

                    // capture/preserve any existing xml comments
                    IEnumerable<XComment> xmlComments = null;
                    if (File.Exists(xmlEofFile))
                        xmlComments = Song2014.ReadXmlComments(xmlEofFile);

                    // correct old toolkit/EOF xml (tuning) issues by syncing with SNG data
                    if (File.Exists(xmlEofFile) && !overwriteSongXml && arrType != ArrangementType.Vocal)
                    {
                        var eofSong = Song2014.LoadFromFile(xmlEofFile);
                        var sngSong = Song2014.LoadFromFile(xmlSngFile);
                        if (eofSong.Tuning != sngSong.Tuning)
                        {
                            eofSong.Tuning = sngSong.Tuning;

                            using (var stream = File.Open(xmlEofFile, FileMode.Create))
                                eofSong.Serialize(stream, true);

                            Song2014.WriteXmlComments(xmlEofFile, xmlComments, customComment: "Synced with SNG file");
                            Console.WriteLine("Fixed Tuning Descrepancies: " + xmlEofFile);
                            GlobalExtension.ShowProgress("Fixed tuning descepancies ...");
                        }
                    }
                    else if (File.Exists(xmlEofFile) && !overwriteSongXml && arrType == ArrangementType.Vocal)
                    {
                        // preserves xml comments from vocals
                    }
                    else // SNG => XML
                    {
                        if (!isODLC)
                            Song2014.WriteXmlComments(xmlSngFile, xmlComments, customComment: "Generated from SNG file");

                        File.Copy(xmlSngFile, xmlEofFile, true);
                    }

                    if (File.Exists(xmlSngFile))
                        File.Delete(xmlSngFile);

                    progress += step;
                    GlobalExtension.UpdateProgress.Value = (int)progress;
                }

                //GlobalExtension.HideProgress();
            }

            return unpackedDir;
        }


        #endregion

        #region PC 2012

        private static string PackPC(string srcPath, string destPath, bool useCryptography, bool updateSng)
        {
            if (Path.GetExtension(destPath) != ".dat")
                destPath += ".dat";

            using (var streamCollection = new DisposableCollection<Stream>())
            using (var outputFileStream = File.Create(destPath))
            {
                var psarc = new PSARC.PSARC();
                foreach (var x in Directory.EnumerateFiles(srcPath))
                {
                    var fileStream = File.OpenRead(x);
                    streamCollection.Add(fileStream);
                    psarc.AddEntry(Path.GetFileName(x), fileStream);
                }

                foreach (var srcInnerPath in Directory.EnumerateDirectories(srcPath))
                {
                    var innerPsarcStream = new MemoryStream();
                    streamCollection.Add(innerPsarcStream);
                    var srcInnerName = Path.GetFileName(srcInnerPath);

                    // Regenerate SNG from XML
                    if (updateSng)
                        if (srcInnerPath.ToLower().IndexOf("dlc_tone_") < 0)
                            UpdateSng(srcPath, srcInnerPath, new Platform(GamePlatform.Pc, GameVersion.RS2012));

                    PackInnerPC(innerPsarcStream, srcInnerPath);
                    psarc.AddEntry(srcInnerName + ".psarc", innerPsarcStream);
                }

                if (useCryptography)
                {
                    using (var psarcStream = new MemoryStream())
                    {
                        psarc.Write(psarcStream, false);
                        RijndaelEncryptor.EncryptFile(psarcStream, outputFileStream, RijndaelEncryptor.DLCKey);
                        return destPath;
                    }
                }

                psarc.Write(outputFileStream, false);
            }

            return destPath;
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

                innerPsarc.Write(output, false, false);
            }
        }

        #endregion

        #region PC/MAC 2014
        // NOTE: See COMMON FUNCTIONS below for PC/MAC UNPACK, aka ExtractPSARC method

        // Pack PC/MAC song artifacts located in the source path and save to the destination path file
        private static string Pack2014(string srcDirPath, string destPath, Platform platform, bool updateSng, bool updateManifest)
        {
            using (var psarc = new PSARC.PSARC())
            using (var psarcStream = new MemoryStreamExtension())
            {
                if (updateSng)
                    UpdateSng2014(srcDirPath, platform);

                if (updateManifest)
                    UpdateManifest2014(srcDirPath, platform);

                WalkThroughDirectory("", srcDirPath, (a, b) =>
               {
                   var fileStream = File.OpenRead(b);
                   psarc.AddEntry(a, fileStream);
               });

                psarc.Write(psarcStream, !platform.IsConsole);

                if (Path.GetExtension(destPath) != ".psarc")
                    destPath += ".psarc";

                using (var outputFileStream = File.Create(destPath))
                    psarcStream.CopyTo(outputFileStream);
            }

            return destPath;
        }

        #endregion

        #region XBox 360

        // Pack Xbox360 song artifacts located in the source path and save to the destination path file
        private static string PackXBox360(string srcDirPath, string destPath, Platform sourcePlatform, bool updateSng, bool updateManifest)
        {
            // apply conditional update of SNG and Manifest files
            if (updateSng && sourcePlatform.version == GameVersion.RS2014)
            {
                UpdateSng2014(srcDirPath, new Platform(GamePlatform.XBox360, GameVersion.RS2014));
                UpdateManifest2014(srcDirPath, new Platform(GamePlatform.XBox360, GameVersion.RS2014));
            }

            var songData = new DLCPackageData();
            var packageRoot = Path.Combine(srcDirPath, ROOT_XBOX360);

            // get songData from XML arrangement file
            var xmlPaths = Directory.EnumerateFiles(srcDirPath, "*.xml", SearchOption.AllDirectories).ToList();
            xmlPaths = xmlPaths.Where(x => !x.Contains("showlight") && !x.Contains("vocal")).ToList();
            if (!xmlPaths.Any())
                throw new FileLoadException("<ERROR> PackXbox360 Failed.  Could not find psarc archive. ");

            var xmlPath = xmlPaths.First();
            var song = Song2014.LoadFromFile(xmlPath);
            songData.SongInfo = new SongInfo();
            songData.SongInfo.SongDisplayName = song.Title;
            songData.SongInfo.Artist = song.ArtistName;
            songData.SignatureType = PackageMagic.CON;

            // TODO: confirm/ensure compatibility RS1 and RS2014
            // If 'Root' directory doesn't exist the packing is a conversion process from another platform
            // create Xbox360 artifacts directory structure
            if (!Directory.Exists(packageRoot))
            {
                // get DLCKey (aka Xbox360 artifact folder name) from an XML arrangment file
                var newSongFolder = song.AlbumArt.Substring(song.AlbumArt.IndexOf("_") + 1);
                var newSongDir = Path.Combine(packageRoot, newSongFolder);

                // Creating new directories
                Directory.CreateDirectory(packageRoot);
                Directory.CreateDirectory(newSongDir);

                // Create PackageList file
                var packListFile = Path.Combine(packageRoot, "PackageList.txt");
                File.WriteAllText(packListFile, newSongFolder);

                var directoryList = Directory.GetDirectories(srcDirPath, "*", SearchOption.AllDirectories);
                var fileList = Directory.EnumerateFiles(srcDirPath, "*.*", SearchOption.AllDirectories);

                // Move directories to new path
                foreach (string dir in directoryList)
                    Directory.CreateDirectory(dir.Replace(srcDirPath, newSongDir));

                // Move files to new path
                foreach (string file in fileList)
                    File.Move(file, file.Replace(srcDirPath, newSongDir));

                // Delete old empty directories
                foreach (string emptyDir in directoryList)
                    IOExtension.DeleteDirectory(emptyDir);
            }

            var packageRootTree = Directory.EnumerateDirectories(packageRoot).ToList();
            // make psarc files out of all packageRoot directories
            foreach (var directory in packageRootTree)
                PackInnerXBox360(packageRoot, directory);

            // tweak the xboxHeaderFile 
            var xboxHeaderFiles = Directory.EnumerateFiles(srcDirPath, "*.txt", SearchOption.TopDirectoryOnly).ToList();
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

                            // update songData from header songInfo
                            if (!String.IsNullOrEmpty(songInfo))
                            {
                                songData.SongInfo = new SongInfo();
                                songData.SongInfo.SongDisplayName = songTitle;
                                songData.SongInfo.Artist = songArtist;
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

            // finally ... put it all together
            var xboxFiles = Directory.EnumerateFiles(packageRoot).ToList();
            var archivePath = DLCPackageCreator.BuildXBox360Package(destPath, songData, xboxFiles, sourcePlatform.version);

            // comment out to leave psarc for debugging if desired 
            var psarcPaths = Directory.EnumerateFiles(srcDirPath, "*.psarc", SearchOption.AllDirectories).ToList();
            foreach (var psarcPath in psarcPaths)
                File.Delete(psarcPath);

            return archivePath;
        }

        private static void PackInnerXBox360(string packageRoot, string directory)
        {
            using (var psarcStream = new MemoryStream())
            {
                var innerPsarc = new PSARC.PSARC();
                WalkThroughDirectory("", directory, (a, b) =>
                {
                    var fileStream = File.OpenRead(b);
                    innerPsarc.AddEntry(a, fileStream);
                });

                innerPsarc.Write(psarcStream, false, false);
                using (var outputFileStream = File.Create(Path.Combine(packageRoot, Path.GetFileName(directory)) + ".psarc"))
                {
                    psarcStream.Seek(0, SeekOrigin.Begin);
                    psarcStream.CopyTo(outputFileStream);
                }
            }
        }

        // Unpack XBox360 package source and return the unpacked directory path
        private static string UnpackXBox360Package(string srcPath, string destDirPath, Platform platform)
        {
            LogRecord x = new LogRecord();
            STFSPackage xboxPackage = new STFSPackage(srcPath, x);
            if (!xboxPackage.ParseSuccess)
                throw new InvalidDataException("Invalid Rocksmith XBox 360 package!" + Environment.NewLine + x.Log);

            // always start fresh
            var unpackedDir = GetUnpackedDir(srcPath, destDirPath, platform);
            IOExtension.DeleteDirectory(unpackedDir);
            Directory.CreateDirectory(unpackedDir);
            xboxPackage.ExtractPayload(unpackedDir, true, true);

            var psarcPaths = Directory.EnumerateFiles(Path.Combine(unpackedDir, ROOT_XBOX360), "*.psarc").ToList();
            if (!psarcPaths.Any())
                throw new FileLoadException("<ERROR> UnpackXbox360Package Failed.  Could not find psarc archive. ");

            var artifactsPath = String.Empty;
            var packageRoot = Path.Combine(unpackedDir, ROOT_XBOX360);
            foreach (var psarcPath in psarcPaths)
            {
                using (var outputFileStream = File.OpenRead(psarcPath))
                    artifactsPath = ExtractPSARC(psarcPath, packageRoot, outputFileStream, platform, false);

                // comment out to leave psarc for debugging 
                File.Delete(psarcPath);
            }

            xboxPackage.CloseIO();
            return unpackedDir;
        }

        private static string GetHeaderValue(this string value)
        {
            return value.Substring(value.IndexOf(":") + 2);
        }

        #endregion

        #region PS3

        // Pack PS3 song artifacts located inside the source path and save to the destination path and/or file
        private static string PackPS3(string srcDirPath, string destPath, Platform srcPlatform, bool updateSng, bool updateManifest)
        {
            // start fresh
            IOExtension.DeleteDirectory(PS3_EDAT);
            Directory.CreateDirectory(PS3_EDAT);

            var psarcDestDir = destPath.Replace(".psarc", "").Replace(".edat", "").Replace("_ps3", "") + "_ps3";
            var cleanPackagePath = Pack2014(srcDirPath, psarcDestDir, srcPlatform, updateSng, updateManifest);
            var rootEdatPath = Path.Combine(PS3_EDAT, Path.GetFileName(cleanPackagePath));
            var encryptedPackagePath = rootEdatPath + ".edat";

            if (!File.Exists(cleanPackagePath))
                throw new FileLoadException("<ERROR> Could not find: " + cleanPackagePath);

            File.Move(cleanPackagePath, rootEdatPath);
            var outputMessage = RijndaelEncryptor.EncryptPS3Edat();

            if (!File.Exists(encryptedPackagePath) || !outputMessage.Equals(EDAT_MSG))
                throw new FileLoadException("<ERROR> PS3 Encryption Failed ...");

            File.Copy(encryptedPackagePath, destPath, true);
            // root edat subfolder contents are still available for debugging

            return destPath;
        }

        // Unpack PS3 package and return the unpacked directory path
        private static string UnpackPS3Package(string srcPath, string destDirPath, Platform platform)
        {
            // start fresh
            IOExtension.DeleteDirectory(PS3_EDAT);
            Directory.CreateDirectory(PS3_EDAT);

            var outputFilename = Path.Combine(PS3_EDAT, Path.GetFileName(srcPath));

            if (File.Exists(srcPath))
                File.Copy(srcPath, outputFilename, true);
            else
                throw new FileNotFoundException(String.Format("File '{0}' not found.", srcPath));

            var outputMessage = RijndaelEncryptor.DecryptPS3Edat();
            if (outputMessage.IndexOf("Decrypt all EDAT files successfully") < 0)
                throw new InvalidOperationException("Rebuilder error, please check if .edat files are created correctly and see output below:" + Environment.NewLine + outputMessage + Environment.NewLine + Environment.NewLine);

            var psarcDatPaths = Directory.EnumerateFiles(PS3_EDAT, "*.psarc.dat").ToList();
            if (!psarcDatPaths.Any())
                throw new FileLoadException("<ERROR> UnpackPS3Package Failed.  Could not find psarc.dat archive. " + Environment.NewLine + "Verify the OS Environmental Variable 'PATH' is configured properly for Java ..." + Environment.NewLine + Environment.NewLine);
            if (psarcDatPaths.Count > 1)
                throw new FileLoadException("<ERROR> UnpackPS3Package Failed.  Found more than one psarc.dat archive. " + Environment.NewLine + Environment.NewLine);

            // FIXME: this will blowup for RS1 PS3 files if/when implimented
            var psarcDatPath = psarcDatPaths.First();
            var artifactsDir = String.Empty;

            using (var outputFileStream = File.OpenRead(psarcDatPath))
                artifactsDir = ExtractPSARC(psarcDatPath, Path.GetDirectoryName(psarcDatPath), outputFileStream, new Platform(GamePlatform.PS3, GameVersion.None));

            // cleanup
            if (File.Exists(outputFilename))
                File.Delete(outputFilename);

            if (File.Exists(psarcDatPath))
                File.Delete(psarcDatPath);

            // start fresh
            var unpackedDir = GetUnpackedDir(srcPath, destDirPath, platform);
            IOExtension.DeleteDirectory(unpackedDir);
            Directory.CreateDirectory(unpackedDir);

            foreach (var edatDir in Directory.EnumerateDirectories(PS3_EDAT))
                IOExtension.MoveDirectory(edatDir, unpackedDir, true);

            return unpackedDir;
        }

        #endregion

        #region COMMON FUNCTIONS

        private static string ExtractPSARC(string srcPath, string destPath, Stream inputStream, Platform platform, bool isInitialCall = true)
        {
            // start fresh on initial call and internalize destPath for recursion
            if (isInitialCall)
                destPath = GetUnpackedDir(srcPath, destPath, platform);

            var psarc = new PSARC.PSARC();
            psarc.Read(inputStream, true);

            var step = Math.Round(1.0 / (psarc.TOC.Count + 2) * 100, 3);
            double progress = 0;
            GlobalExtension.ShowProgress("Inflating Entries ...");

            // InflateEntries - compatible with RS1 and RS2014 files
            foreach (var entry in psarc.TOC)
            {
                var inputPath = Path.Combine(destPath, entry.Name);

                // for dev debugging invalid symbol usage in CDLC
                // inputPath = inputPath.Replace("?", "i");
                // inputPath = Path.Combine(destPath, StringExtensions.GetValidFileName(entry.Name));

                if (Path.GetExtension(entry.Name).ToLower() == ".psarc")
                {
                    psarc.InflateEntry(entry);
                    var outputPath = Path.Combine(destPath, Path.GetFileNameWithoutExtension(entry.Name));
                    ExtractPSARC(inputPath, outputPath, entry.Data, platform, false);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(inputPath));
                    psarc.InflateEntry(entry, inputPath);
                    // Close
                    if (entry.Data != null)
                        entry.Data.Dispose();
                }

                if (!String.IsNullOrEmpty(psarc.ErrMSG)) 
                    throw new InvalidDataException(psarc.ErrMSG);

                progress += step;
                GlobalExtension.UpdateProgress.Value = (int)progress;
            }

            if (psarc != null)
            {
                psarc.Dispose();
                psarc = null;
            }

            return destPath;
            // GlobalExtension.HideProgress();
        }

        public static void CleanupArtifacts(string srcPath)
        {
            try
            {
                // delete template xml remnants that may have been added by SaveTemplateFile method
                var templateFiles = Directory.EnumerateFiles(srcPath, "*", SearchOption.AllDirectories).Where(fn => fn.EndsWith("RS2012.dlc.xml") || fn.EndsWith("RS2014.dlc.xml")).ToList();
                foreach (var templateFile in templateFiles)
                    File.Delete(templateFile);

                // delete friendly named ogg/wem audio files that may have been added by new LoadFromFolder method
                var audioFiles = Directory.EnumerateFiles(srcPath, "song_*.*", SearchOption.AllDirectories).Where(fn => fn.EndsWith(".ogg") || fn.EndsWith(".wem")).ToList();
                foreach (var audioFile in audioFiles)
                    File.Delete(audioFile);

                // delete friendly named _fixed.ogg audio files that may have been added by LoadFromFolder method
                var fixedOggFiles = Directory.EnumerateFiles(srcPath, "*", SearchOption.AllDirectories).Where(fn => fn.EndsWith("_fixed.ogg")).ToList();
                foreach (var fixedOggFile in fixedOggFiles)
                    File.Delete(fixedOggFile);

                // delete png album artwork files that may have been added by Unpack method (DO NOT DELETE any Xbox360 images)
                var pngFiles = Directory.EnumerateFiles(srcPath, "*.png", SearchOption.AllDirectories).Where(fp => !Path.GetFileName(fp).Equals("Package Image.png") && !Path.GetFileName(fp).Equals("Content Image.png")).ToList();
                foreach (var pngFile in pngFiles)
                    File.Delete(pngFile);

                // delete NamesBlock.bin files
                var bins = Directory.EnumerateFiles(srcPath, "NamesBlock.bin", SearchOption.AllDirectories).Where(File.Exists);
                foreach (var namesBlock in bins)
                    File.Delete(namesBlock);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Can't delete extraneous audio files!\r\n {0}", ex));
            }
        }

        private static void WalkThroughDirectory(string baseDir, string directory, Action<string, string> action)
        {
            foreach (var fl in Directory.EnumerateFiles(directory))
                action(String.Format("{0}/{1}", baseDir, Path.GetFileName(fl)).TrimStart('/'), fl);

            foreach (var dr in Directory.EnumerateDirectories(Path.Combine(baseDir, directory)))
                WalkThroughDirectory(String.Format("{0}/{1}", baseDir, Path.GetFileName(dr)), dr, action);
        }

        /// <summary>
        /// Get platform from a source path
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public static Platform GetPlatform(this string srcPath)
        {
            //TODO: Rewrite this method, validate Files by MIME type
            if (File.Exists(srcPath))
            {
                // Get PLATFORM by Extension + Get PLATFORM by pkg EndName
                switch (Path.GetExtension(srcPath))
                {
                    case ".dat":
                        return new Platform(GamePlatform.Pc, GameVersion.RS2012);
                    case "":
                        var fileName = Path.GetFileName(srcPath);
                        if (!fileName.EndsWith("_xbox") && fileName.Length != 42)
                            break; // keep looking
                        // must get game version from ConfigRepository for UnitTest compatibility
                        GameVersion xboxVersion = (GameVersion)Enum.Parse(typeof(GameVersion), ConfigRepository.Instance()["general_defaultgameversion"]);
                        return new Platform(GamePlatform.XBox360, xboxVersion);
                    case ".edat":
                        // must get game version from ConfigRepository for UnitTest compatibility
                        GameVersion ps3Version = (GameVersion)Enum.Parse(typeof(GameVersion), ConfigRepository.Instance()["general_defaultgameversion"]);
                        return new Platform(GamePlatform.PS3, ps3Version);
                    case ".psarc":
                        return TryGetPlatformByEndName(srcPath);
                    default:
                        return new Platform(GamePlatform.None, GameVersion.None);
                }
            }

            if (Directory.Exists(srcPath))
            {
                var fullPathInfo = new DirectoryInfo(srcPath);
                // GET PLATFORM BY PACKAGE ROOT DIRECTORY
                if (File.Exists(Path.Combine(srcPath, "APP_ID")))
                {
                    // PC 2012
                    return new Platform(GamePlatform.Pc, GameVersion.RS2012);
                }

                string agg;

                if (File.Exists(Path.Combine(srcPath, "appid.appid")))
                {
                    // PC / MAC 2014
                    agg = fullPathInfo.EnumerateFiles("*.nt", SearchOption.TopDirectoryOnly).FirstOrDefault().FullName;
                    var aggContent = File.ReadAllText(agg);

                    if (aggContent.Contains("\"dx9\""))
                        return new Platform(GamePlatform.Pc, GameVersion.RS2014);
                    if (aggContent.Contains("\"macos\""))
                        return new Platform(GamePlatform.Mac, GameVersion.RS2014);

                    return new Platform(GamePlatform.Pc, GameVersion.RS2014); // Because appid.appid have only in RS2014
                }

                if (Directory.Exists(Path.Combine(srcPath, ROOT_XBOX360)))
                {
                    // XBOX 2012/2014
                    var hTxt = fullPathInfo.EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly).FirstOrDefault().FullName;
                    var hTxtContent = File.ReadAllText(hTxt);

                    if (hTxtContent.Contains("Title ID: 55530873"))
                        return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                    if (hTxtContent.Contains("Title ID: 555308C0"))
                        return new Platform(GamePlatform.XBox360, GameVersion.RS2014);

                    return new Platform(GamePlatform.XBox360, GameVersion.None);
                }

                if (srcPath.ToLower().EndsWith("_p") || srcPath.ToLower().EndsWith("_pc"))
                {
                    return new Platform(GamePlatform.Pc, GameVersion.RS2014);
                }

                if (srcPath.ToLower().EndsWith("_m") || srcPath.ToLower().EndsWith("_mac"))
                {
                    return new Platform(GamePlatform.Mac, GameVersion.RS2014);
                }

                // some RS1 detection
                if (srcPath.ToLower().EndsWith("_rs1_xbox"))
                {
                    return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                }


                // PS3 2012/2014
                agg = fullPathInfo.EnumerateFiles("*.nt", SearchOption.TopDirectoryOnly).FirstOrDefault().FullName;

                if (agg.Any())
                {
                    var aggContent = File.ReadAllText(agg);

                    if (aggContent.Contains("\"PS3\""))
                        return new Platform(GamePlatform.PS3, GameVersion.RS2012);
                    if (aggContent.Contains("\"ps3\""))
                        return new Platform(GamePlatform.PS3, GameVersion.RS2014);

                    return TryGetPlatformByEndName(srcPath);
                }

                return TryGetPlatformByEndName(srcPath);
            }

            return new Platform(GamePlatform.None, GameVersion.None);
        }

        /// <summary>
        /// Gets platform from source path or file name ending
        /// </summary>
        /// <param name="srcPath">Folder of File</param>
        /// <returns>Platform(DetectedPlatform, RS2014 ? None)</returns>
        public static Platform TryGetPlatformByEndName(string srcPath)
        {
            var p = GamePlatform.None;
            var v = GameVersion.RS2014;
            var name = Path.GetFileName(srcPath);
            var pIndex = name.LastIndexOf("_", StringComparison.Ordinal);

            if (Directory.Exists(srcPath))
            {// Pc, Mac, XBox360, PS3
                var platformString = name.Substring(pIndex + 1);
                var isValid = Enum.TryParse(platformString, true, out p);

                return isValid ? new Platform(p, v) : new Platform(GamePlatform.None, GameVersion.None);
            }
            else
            {//_p, _m, _ps3, _xbox
                var platformString = pIndex > -1 ? name.Substring(pIndex) : "";
                switch (platformString.ToLower())
                {
                    case "_p":
                    case "_p.psarc":
                        return new Platform(GamePlatform.Pc, v);
                    case "_m":
                    case "_m.psarc":
                        return new Platform(GamePlatform.Mac, v);
                    case "_ps3":
                    case "_ps3.edat":
                        return new Platform(GamePlatform.PS3, v);
                    case "_xbox":
                        return new Platform(GamePlatform.XBox360, v);
                    default:
                        return new Platform(GamePlatform.Pc, v);
                }
            }
        }

        private static void UpdateSng(string srcPath, string srcInnerPath, Platform targetPlatform)
        {
            var info = DLCPackageData.RS1LoadFromFolder(srcPath, targetPlatform, false);

            foreach (var arr in info.Arrangements)
            {
                var sngPath = Path.Combine(srcInnerPath, "GRExports", targetPlatform.GetPathName()[1], Path.GetFileNameWithoutExtension(arr.SongXml.File) + ".sng");
                SngFileWriter.Write(arr, sngPath, targetPlatform);
            }
        }

        private static void UpdateSng2014(string artifactsPath, Platform targetPlatform)
        {
            var xmlFiles = Directory.EnumerateFiles(artifactsPath, "*_*.xml", SearchOption.AllDirectories).ToList();
            var sngFolder = Path.GetDirectoryName(Directory.EnumerateFiles(artifactsPath, "*_*.sng", SearchOption.AllDirectories).Where(fp => fp.Contains("songs\\bin")).FirstOrDefault());
            if (String.IsNullOrEmpty(sngFolder))
                throw new DirectoryNotFoundException("<ERROR> Did not find 'songs\bin' folder ...");

            foreach (var xmlFile in xmlFiles)
            {
                var xmlName = Path.GetFileNameWithoutExtension(xmlFile);
                if (xmlName.ToLower().Contains("_showlights"))
                    continue;

                var sngFile = Path.Combine(sngFolder, xmlName + ".sng");
                var arrType = ArrangementType.Guitar;

                if (xmlName.ToLower().Contains("vocal"))
                    arrType = ArrangementType.Vocal;

                // TODO: Handle vocals custom font
                string fontSng = null;
                if (arrType == ArrangementType.Vocal)
                {
                    //var vocSng = Sng2014File.LoadFromFile(sngFile, GetPlatform(songDirectory));
                    //if (vocSng.IsCustomFont())
                    //{
                    //    vocSng.WriteChartData((fontSng = Path.GetTempFileName()), new Platform(GamePlatform.Pc, GameVersion.None));
                    //}
                }

                using (var fs = new FileStream(sngFile, FileMode.Create))
                {
                    var sng = Sng2014File.ConvertXML(xmlFile, arrType, fontSng);
                    sng.WriteSng(fs, targetPlatform);
                }
            }
        }

        // Regenerates the aggregategraph file
        private static void UpdateAggegrateGraph(string songDirectory, Platform targetPlatform, DLCPackageData info)
        {
            var dlcName = info.Name.ToLower();
            var aggregateGraphFileName = Path.Combine(songDirectory, String.Format("{0}_aggregategraph.nt", dlcName));
            var aggregateGraph = new AggregateGraph2014.AggregateGraph2014(info, targetPlatform);

            using (var fs = new FileStream(aggregateGraphFileName, FileMode.Create))
            using (var aggregateGraphStream = new MemoryStream())
            {
                aggregateGraph.Serialize(aggregateGraphStream);
                aggregateGraphStream.Flush();
                aggregateGraphStream.Seek(0, SeekOrigin.Begin);
                aggregateGraphStream.CopyTo(fs);
            }
        }

        // Fixes missing and updates showlights to current standards
        private static void UpdateShowlights(string srcPath, Platform targetPlatform)
        {
            bool hasShowlights = true;
            // TODO: provide some pb feedback for long process
            var info = DLCPackageData.LoadFromFolder(srcPath, targetPlatform);
            var showlightsArr = info.Arrangements.Where(x => x.ArrangementType == ArrangementType.ShowLight).FirstOrDefault();
            var showlightFilePath = showlightsArr.SongXml.File;

            if (String.IsNullOrEmpty(showlightFilePath))
            {
                var xmlFilePath = info.Arrangements[0].SongXml.File;
                var xmlName = Path.GetFileNameWithoutExtension(xmlFilePath);
                showlightFilePath = Path.Combine(Path.GetDirectoryName(xmlFilePath), xmlName.Split('_')[0] + "_showlights.xml");
                hasShowlights = false;
            }

            // Generate Showlights
            var showlight = new Showlights();
            showlight.CreateShowlights(info);
            // need at least two showlight elements to be valid
            if (showlight.ShowlightList.Count > 1)
            {
                var showlightStream = new MemoryStream();
                showlight.Serialize(showlightStream);

                using (FileStream file = new FileStream(showlightFilePath, FileMode.Create, FileAccess.Write))
                    showlightStream.WriteTo(file);

                // write xml comments
                Song2014.WriteXmlComments(showlightFilePath);
            }
            else
            {
                // insufficient showlight changes may crash game
                throw new InvalidOperationException("Detected insufficient showlight changes: " + showlight.ShowlightList.Count);
            }

            if (!hasShowlights) // better not happen!
                UpdateAggegrateGraph(srcPath, targetPlatform, info);
        }

        private static void UpdateManifest2014(string srcPath, Platform targetPlatform)
        {
            if (targetPlatform.version != GameVersion.RS2014)
                return;

            string[] fileExt = new string[] { "hsan", "hson" };
            var hsanFiles = Directory.EnumerateFiles(srcPath, "*", SearchOption.AllDirectories).Where(fi => fileExt.Any(fi.ToLower().EndsWith)).ToList();
            if (!hsanFiles.Any())
                throw new DataException("Error: could not find hsan/hson file");
            if (hsanFiles.Count > 1)
                throw new DataException("Error: there is more than one hsan/hson file");

            var manifestHeader = new ManifestHeader2014<AttributesHeader2014>(targetPlatform);
            var hsanFile = hsanFiles.First();
            var jsonFiles = Directory.EnumerateFiles(srcPath, "*.json", SearchOption.AllDirectories).ToList();
            var xmlFiles = Directory.EnumerateFiles(srcPath, "*.xml", SearchOption.AllDirectories).ToList();
            var toolkitVersionFile = Directory.EnumerateFiles(srcPath, "toolkit.version", SearchOption.AllDirectories).FirstOrDefault();
            //var songFiles = xmlFiles.Where(x => !x.ToLower().Contains("showlight") && !x.ToLower().Contains("vocal")).ToList();
            //var vocalFiles = xmlFiles.Where(x => x.ToLower().Contains("vocal")).ToList();

            foreach (var xmlFile in xmlFiles)
            {
                var xmlName = Path.GetFileNameWithoutExtension(xmlFile);
                if (xmlName.ToLower().Contains("showlight"))
                    continue;

                var json = jsonFiles.FirstOrDefault(name => Path.GetFileNameWithoutExtension(name) == xmlName);
                if (String.IsNullOrEmpty(json))
                    continue;

                var attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.First().Value.First().Value;

                if (!xmlName.ToLower().Contains("vocal"))
                {
                    var manifestFunctions = new ManifestFunctions(targetPlatform.version);
                    var song2014 = Song2014.LoadFromFile(xmlFile);

                    attr.PhraseIterations = new List<Manifest.PhraseIteration>();
                    manifestFunctions.GeneratePhraseIterationsData(attr, song2014, targetPlatform.version);

                    attr.Phrases = new List<Manifest.Phrase>();
                    manifestFunctions.GeneratePhraseData(attr, song2014);

                    attr.Sections = new List<Manifest.Section>();
                    manifestFunctions.GenerateSectionData(attr, song2014);

                    attr.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                    manifestFunctions.GenerateTuningData(attr, song2014);

                    attr.MaxPhraseDifficulty = manifestFunctions.GetMaxDifficulty(song2014);

                    // TODO: monitor this change updates both json and hsan files
                    if (!String.IsNullOrEmpty(toolkitVersionFile))
                    {
                        var tkInfo = GeneralExtension.ReadToolkitInfo(toolkitVersionFile);
                        // hide album artwork marker in-game setlist
                        // while leaving Alternate Arrangements unlocked for new user profile
                        if (tkInfo != null && tkInfo.PackageAuthor != "Ubisoft")
                            attr.SKU = "";
                    }
                }

                // else { // TODO: good place to update vocals }

                // write updated json file
                var attributeDictionary = new Dictionary<string, Attributes2014> { { "Attributes", attr } };
                var manifest = new Manifest2014<Attributes2014>();
                manifest.Entries.Add(attr.PersistentID, attributeDictionary);
                manifest.SaveToFile(json);

                // update manifestHeader (hsan) entry
                var attributeHeaderDictionary = new Dictionary<string, AttributesHeader2014> { { "Attributes", new AttributesHeader2014(attr) } };
                if (targetPlatform.IsConsole)
                {
                    // One for each arrangements (Xbox360/PS3)
                    manifestHeader = new ManifestHeader2014<AttributesHeader2014>(targetPlatform);
                    manifestHeader.Entries.Add(attr.PersistentID, attributeHeaderDictionary);
                }
                else
                    manifestHeader.Entries.Add(attr.PersistentID, attributeHeaderDictionary);
            }

            // write updated hsan file
            manifestHeader.SaveToFile(hsanFile);

            // fix missing or upgrade existing showlights
            UpdateShowlights(srcPath, targetPlatform);
        }

        /// <summary>
        /// Get unpacked directory path name or folder name with platform identifiers 
        /// </summary>
        /// <param name="srcPath">Source Path</param>
        /// <param name="destPath">Destination Path (if 'null' returns an unpacked folder name)</param>
        /// <param name="targetPlatform">Target Platform</param>
        /// <param name="safeDelete">Delete unpacked directory if it already exists</param>
        /// <returns>Unpacked Directory Path or Folder Name</returns>
        public static string GetUnpackedDir(string srcPath, string destPath, Platform targetPlatform, bool safeDelete = true)
        {
            // org Xbox360 packages have random hexidecimal file name with no extension
            var fnameWithoutExt = Path.GetFileName(srcPath);
            string[] extensions = { "_p.psarc", "_m.psarc", "_ps3.psarc.edat", "_xbox", ".dat" };

            foreach (string extension in extensions)
            {
                if (Path.GetFileName(srcPath).EndsWith(extension))
                {
                    fnameWithoutExt = Path.GetFileName(srcPath).Replace(extension, "");
                    break;
                }
            }

            var unpackedDir = String.Format("{0}_{1}", fnameWithoutExt, targetPlatform);

            if (!String.IsNullOrEmpty(destPath))
            {
                unpackedDir = Path.Combine(destPath, unpackedDir);
                if (Directory.Exists(unpackedDir) && safeDelete)
                    IOExtension.DeleteDirectory(unpackedDir);
            }

            return unpackedDir;
        }

        /// <summary>
        /// Recycle an unpacked directory path or folder into archive file path or name
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns>Archive File Path or Name</returns>
        public static string RecycleUnpackedDir(string srcPath)
        {
            var destPath = String.Empty;

            // populate a lookup dictionary with all posible version_platform (keys) and archive file extension (values)
            var versionPlatformExtension = new Dictionary<string, string>();
            var versions = Enum.GetValues(typeof(GameVersion)).Cast<GameVersion>().ToList();
            var platforms = Enum.GetValues(typeof(GamePlatform)).Cast<GamePlatform>().ToList();

            foreach (var platform in platforms)
            {
                foreach (var version in versions)
                {
                    var versionPlatform = String.Format("_{0}_{1}", version, platform);
                    var fileExtension = ".TBD"; // To Be Determined

                    if (version == GameVersion.RS2014)
                    {
                        switch (platform)
                        {
                            case GamePlatform.Pc:
                                fileExtension = "_p.psarc";
                                break;
                            case GamePlatform.Mac:
                                fileExtension = "_m.psarc";
                                break;
                            case GamePlatform.PS3:
                                fileExtension = "_ps3.psarc.edat";
                                break;
                            case GamePlatform.XBox360:
                                fileExtension = "_xbox";
                                break;
                            default:
                                fileExtension = ".TBD"; // To Be Determined
                                break;
                        }
                    }
                    else if (version == GameVersion.RS2012)
                    {
                        switch (platform)
                        {
                            case GamePlatform.Pc:
                                fileExtension = ".dat";
                                break;
                            case GamePlatform.PS3:
                                fileExtension = "_ps3.dat"; // TODO: confirm extension
                                break;
                            case GamePlatform.XBox360:
                                fileExtension = "_xbox";
                                break;
                            default:
                                fileExtension = ".TBD"; // To Be Determined
                                break;
                        }
                    }
                    else if (version == GameVersion.None)
                    {
                        switch (platform)
                        {
                            default:
                                fileExtension = ".TBD"; // To Be Determined
                                break;
                        }
                    }

                    versionPlatformExtension.Add(versionPlatform, fileExtension);
                }
            }

            // now use the lookup dictionary to find the correct archive file extension
            foreach (KeyValuePair<string, string> item in versionPlatformExtension)
            {
                if (srcPath.EndsWith(item.Key))
                {
                    var vpIndex = srcPath.LastIndexOf(item.Key, StringComparison.Ordinal);
                    var pathWoVP = srcPath.Substring(0, vpIndex);
                    destPath = String.Format("{0}{1}", pathWoVP, item.Value);
                    break;
                }
            }

            return destPath;
        }

        public static string StripPlatformEndName(this string filePath)
        {

            if (filePath.EndsWith(GamePlatform.Pc.GetPathName()[2]) ||
               filePath.EndsWith(GamePlatform.Mac.GetPathName()[2]) ||
               filePath.EndsWith(GamePlatform.XBox360.GetPathName()[2]) ||
               filePath.EndsWith(GamePlatform.PS3.GetPathName()[2]) ||
               filePath.EndsWith(GamePlatform.PS3.GetPathName()[2] + ".psarc"))
            {
                return filePath.Substring(0, filePath.LastIndexOf("_"));
            }

            return filePath;
        }

        public static string GetPlatformEndName(Platform srcPlatform)
        {
            var version = srcPlatform.version;
            var platform = srcPlatform.platform;
            var fileExtension = ".TBD"; // To Be Determined

            if (version == GameVersion.RS2014)
            {
                switch (platform)
                {
                    case GamePlatform.Pc:
                        fileExtension = "_p.psarc";
                        break;
                    case GamePlatform.Mac:
                        fileExtension = "_m.psarc";
                        break;
                    case GamePlatform.PS3:
                        fileExtension = "_ps3.psarc.edat";
                        break;
                    case GamePlatform.XBox360:
                        fileExtension = "_xbox";
                        break;
                    default:
                        fileExtension = ".TBD"; // To Be Determined
                        break;
                }
            }
            else if (version == GameVersion.RS2012)
            {
                switch (platform)
                {
                    case GamePlatform.Pc:
                        fileExtension = ".dat";
                        break;
                    case GamePlatform.PS3:
                        fileExtension = "_ps3.dat"; // TODO: confirm extension
                        break;
                    case GamePlatform.XBox360:
                        fileExtension = "_xbox";
                        break;
                    default:
                        fileExtension = ".TBD"; // To Be Determined
                        break;
                }
            }
            else if (version == GameVersion.None)
            {
                switch (platform)
                {
                    default:
                        fileExtension = ".TBD"; // To Be Determined
                        break;
                }
            }

            return fileExtension;
        }

        #endregion

    }
}

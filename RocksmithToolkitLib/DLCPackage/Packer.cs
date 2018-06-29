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
        /// Pack song artifacts located in the source path and saves archive to the destination path file
        /// </summary>
        /// <param name="srcPath">Unpacked Artifacts Path</param>
        /// <param name="destPath">Destination Archive Path</param>
        /// <param name="predifinedPlatform">Desired Output Platform</param>
        /// <param name="updateSng">If set to <c>true</c> update the SNG files</param>
        /// <param name="updateManifest">If set to <c>true</c> update the manifest files</param>
        /// <returns>Archive Path</returns>
        public static string Pack(string srcPath, string destPath, Platform predefinedPlatform = null, bool updateSng = false, bool updateManifest = false)
        {
            var archivePath = String.Empty;
            ExternalApps.VerifyExternalApps();
            DeleteFixedAudio(srcPath);
            Platform srcPlatform = srcPath.GetPlatform();

            if (predefinedPlatform != null && predefinedPlatform.platform != GamePlatform.None && predefinedPlatform.version != GameVersion.None)
                srcPlatform = predefinedPlatform;

            //TODO: check validity of file name here

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
        /// Unpack archive path and return the unpacked artifacts path
        /// </summary>
        /// <param name="srcPath">Source Archive Path</param>
        /// <param name="destPath">Unpacked Artifacts Path</param>
        /// <param name="predefinedPlatform">Predefined source platform</param>
        /// <param name="decodeAudio">If set to <c>true</c> decode audio</param>
        /// <param name="overwriteSongXml">If set to <c>true</c> overwrite existing song (EOF) xml with SNG data</param>       
        /// <returns>Unpacked Directory Path</returns>
        public static string Unpack(string srcPath, string destPath, Platform predefinedPlatform = null, bool decodeAudio = false, bool overwriteSongXml = false)
        {
            ExternalApps.VerifyExternalApps();
            Platform srcPlatform = srcPath.GetPlatform();

            if (predefinedPlatform != null && predefinedPlatform.platform != GamePlatform.None && predefinedPlatform.version != GameVersion.None)
                srcPlatform = predefinedPlatform;

            var unpackedDir = GetUnpackedDir(srcPath, destPath, srcPlatform);
            var useCryptography = srcPlatform.version == GameVersion.RS2012; // Cryptography way is used only for PC in Rocksmith 1

            switch (srcPlatform.platform)
            {
                case GamePlatform.Pc:
                case GamePlatform.Mac:
                    if (srcPlatform.version == GameVersion.RS2014)
                        using (var inputStream = File.OpenRead(srcPath))
                            ExtractPSARC(srcPath, destPath, inputStream, srcPlatform);
                    else
                    {
                        using (var inputFileStream = File.OpenRead(srcPath))
                        using (var inputStream = new MemoryStream())
                        {
                            if (useCryptography)
                                RijndaelEncryptor.DecryptFile(inputFileStream, inputStream, RijndaelEncryptor.DLCKey);
                            else
                                inputFileStream.CopyTo(inputStream);

                            ExtractPSARC(srcPath, destPath, inputStream, srcPlatform);
                        }
                    }
                    break;
                case GamePlatform.XBox360:
                    UnpackXBox360Package(srcPath, destPath, srcPlatform);
                    break;
                case GamePlatform.PS3:
                    UnpackPS3Package(srcPath, destPath, srcPlatform);
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
                var audioFiles = Directory.EnumerateFiles(unpackedDir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wem"));
                foreach (var file in audioFiles)
                {
                    var outputAudioFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), ".ogg"));
                    // path fixed for unit testing compatiblity
                    OggFile.Revorb(file, outputAudioFileName, Path.GetExtension(file).GetWwiseVersion());
                }

                //GlobalExtension.HideProgress();
            }

            // for debugging
            //overwriteSongXml = false;

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
            var bins = Directory.EnumerateFiles(srcPath, "NamesBlock.bin", SearchOption.AllDirectories).Where(File.Exists);
            foreach (var namesBlock in bins)
                File.Delete(namesBlock);

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

                foreach (var directory in Directory.EnumerateDirectories(srcPath))
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
        private static string Pack2014(string srcPath, string destPath, Platform platform, bool updateSng, bool updateManifest)
        {
            using (var psarc = new PSARC.PSARC())
            using (var psarcStream = new MemoryStreamExtension())
            {
                if (updateSng)
                    UpdateSng2014(srcPath, platform);

                if (updateManifest)
                    UpdateManifest2014(srcPath, platform);

                WalkThroughDirectory("", srcPath, (a, b) =>
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
        private static string PackXBox360(string srcPath, string destPath, Platform platform, bool updateSng, bool updateManifest)
        {
            var songData = new DLCPackageData();
            var packageRootPath = Path.Combine(srcPath, ROOT_XBOX360);

            // get songData from XML arrangement file
            var xmlPaths = Directory.EnumerateFiles(srcPath, "*.xml", SearchOption.AllDirectories).ToList();
            xmlPaths = xmlPaths.Where(x => !x.Contains("showlight") && !x.Contains("vocal")).ToList();
            if (!xmlPaths.Any())
                throw new FileLoadException("<ERROR> PackXbox360 Failed.  Could not find psarc archive. ");

            var xmlPath = xmlPaths.First();
            var song = Song2014.LoadFromFile(xmlPath);
            songData.SongInfo = new SongInfo();
            songData.SongInfo.SongDisplayName = song.Title;
            songData.SongInfo.Artist = song.ArtistName;
            songData.SignatureType = PackageMagic.CON;

            // get DLCKey (aka Xbox360 artifact folder name) from an XML arrangment file
            var artifactsFolder = song.AlbumArt.Substring(song.AlbumArt.IndexOf("_") + 1);
            var artifactsPath = Path.Combine(packageRootPath, artifactsFolder);

            // If 'Root' directory doesn't exist the packing is a conversion process from another platform
            // Create Xbox360 artifacts folder structure
            if (!Directory.Exists(packageRootPath))
            {
                // Creating new directories
                Directory.CreateDirectory(packageRootPath);
                Directory.CreateDirectory(artifactsPath);

                // Create PackageList file
                var packListFile = Path.Combine(packageRootPath, "PackageList.txt");
                File.WriteAllText(packListFile, artifactsFolder);

                var directoryList = Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories);
                var fileList = Directory.EnumerateFiles(srcPath, "*.*", SearchOption.AllDirectories);

                // Move directories to new path
                foreach (string dir in directoryList)
                    Directory.CreateDirectory(dir.Replace(srcPath, artifactsPath));

                // Move files to new path
                foreach (string file in fileList)
                    File.Move(file, file.Replace(srcPath, artifactsPath));

                // Delete old empty directories
                foreach (string emptyDir in directoryList)
                    DirectoryExtension.SafeDelete(emptyDir);
            }

            foreach (var directory in Directory.EnumerateDirectories(packageRootPath))
            {
                PackInnerXBox360(packageRootPath, directory);
            }

            // tweak the xboxHeaderFile 
            IEnumerable<string> xboxHeaderFiles = Directory.EnumerateFiles(srcPath, "*.txt", SearchOption.TopDirectoryOnly);
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

            // coditionally update the SNG and Manifest files
            if (updateSng && platform.version == GameVersion.RS2014)
            {
                UpdateSng2014(artifactsPath, platform);
                UpdateManifest2014(artifactsPath, platform);
            }

            // let's pack this sucker
            var xboxFiles = Directory.EnumerateFiles(packageRootPath).ToList();
            DLCPackageCreator.BuildXBox360Package(destPath, songData, xboxFiles, platform.version);

            // comment out to leave psarc for debugging if desired 
            var psarcPaths = Directory.EnumerateFiles(srcPath, "*.psarc", SearchOption.AllDirectories).ToList();
            foreach (var psarcPath in psarcPaths)
                File.Delete(psarcPath);

            return destPath;
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
        private static string UnpackXBox360Package(string srcPath, string destPath, Platform platform)
        {
            LogRecord x = new LogRecord();
            STFSPackage xboxPackage = new STFSPackage(srcPath, x);
            if (!xboxPackage.ParseSuccess)
                throw new InvalidDataException("Invalid Rocksmith XBox 360 package!" + Environment.NewLine + x.Log);

            // always start fresh
            var unpackedDir = Path.Combine(destPath, Path.GetFileNameWithoutExtension(srcPath)) + String.Format("_{0}", platform.platform.ToString());
            DirectoryExtension.SafeDelete(unpackedDir);
            Directory.CreateDirectory(unpackedDir);
            xboxPackage.ExtractPayload(unpackedDir, true, true);

            var psarcPaths = Directory.EnumerateFiles(Path.Combine(unpackedDir, ROOT_XBOX360), "*.psarc").ToList();
            if (!psarcPaths.Any())
                throw new FileLoadException("<ERROR> UnpackXbox360Package Failed.  Could not find psarc archive. ");
            if (psarcPaths.Count > 1)
                throw new FileLoadException("<ERROR> UnpackXbox360Package Failed.  Found more than one psarc archive. ");

            var artifactsPath = String.Empty;
            var psarcPath = psarcPaths.First();
            var packageRoot = Path.Combine(unpackedDir, ROOT_XBOX360);

            using (var outputFileStream = File.OpenRead(psarcPath))
                artifactsPath = ExtractPSARC(psarcPath, packageRoot, outputFileStream, platform, false);

            // comment out to leave psarc for debugging 
            File.Delete(psarcPath);

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
        private static string PackPS3(string srcPath, string destPath, Platform platform, bool updateSng, bool updateManifest)
        {
            // always start with fresh edat subfolder
            DirectoryExtension.SafeDelete(PS3_EDAT);
            Directory.CreateDirectory(PS3_EDAT);

            // =========================
            // TODO: create a generic archive file name generator method for use by all packers
            //
            var archiveDestPath = String.Empty;
            var fileName = Path.GetFileName(destPath);
            // destPath already contains a valid filename            
            if (fileName.EndsWith("_ps3.psarc.edat"))
                archiveDestPath = destPath;
            else // destPath is a directory
            {
                var archiveName = RecycleUnpackedDir(srcPath);
                archiveDestPath = Path.Combine(destPath, archiveName);
            }
            // =========================

            var psarcDestPath = archiveDestPath.Replace(".psarc", "").Replace(".edat", "");
            var cleanPackagePath = Pack2014(srcPath, psarcDestPath, platform, updateSng, updateManifest);
            var rootEdatPath = Path.Combine(PS3_EDAT, Path.GetFileName(cleanPackagePath));
            var encryptedPackagePath = rootEdatPath + ".edat";

            if (File.Exists(cleanPackagePath))
                File.Move(cleanPackagePath, rootEdatPath);
            else
                throw new FileLoadException("<ERROR> Could not find: " + cleanPackagePath);

            var outputMessage = RijndaelEncryptor.EncryptPS3Edat();
            if (!File.Exists(encryptedPackagePath) || !outputMessage.Equals(EDAT_MSG))
                throw new FileLoadException("<ERROR> PS3 Encryption Failed ...");
            else
            {
                if (File.Exists(archiveDestPath))
                    File.Delete(archiveDestPath);

                File.Copy(encryptedPackagePath, archiveDestPath);
                // NOTE: edat subfolder contents are available for debugging
            }

            return archiveDestPath;
        }

        // Unpack PS3 package and return the unpacked directory path
        private static string UnpackPS3Package(string srcPath, string destPath, Platform platform)
        {
            var outputFilename = Path.Combine(PS3_EDAT, Path.GetFileName(srcPath));

            // always start fresh
            DirectoryExtension.SafeDelete(PS3_EDAT);
            Directory.CreateDirectory(PS3_EDAT);

            if (File.Exists(srcPath))
                File.Copy(srcPath, outputFilename, true);
            else
                throw new FileNotFoundException(String.Format("File '{0}' not found.", srcPath));

            var outputMessage = RijndaelEncryptor.DecryptPS3Edat();

            if (File.Exists(outputFilename))
                File.Delete(outputFilename);

            var psarcDatPaths = Directory.EnumerateFiles(PS3_EDAT, "*.psarc.dat").ToList();
            if (!psarcDatPaths.Any())
                throw new FileLoadException("<ERROR> UnpackPS3Package Failed.  Could not find psarc.dat archive. ");
            if (psarcDatPaths.Count > 1)
                throw new FileLoadException("<ERROR> UnpackPS3Package Failed.  Found more than one psarc.dat archive. ");

            var psarcDatPath = psarcDatPaths.First();
            using (var outputFileStream = File.OpenRead(psarcDatPath))
                ExtractPSARC(psarcDatPath, Path.GetDirectoryName(psarcDatPath), outputFileStream, new Platform(GamePlatform.PS3, GameVersion.None));

            if (File.Exists(psarcDatPath))
                File.Delete(psarcDatPath);

            var outName = Path.GetFileNameWithoutExtension(srcPath);
            var outputDir = Path.Combine(destPath, outName.Substring(0, outName.LastIndexOf(".")) + String.Format("_{0}", platform.platform.ToString()));

            foreach (var unpackedDir in Directory.EnumerateDirectories(PS3_EDAT))
                if (Directory.Exists(unpackedDir))
                {
                    if (Directory.Exists(outputDir))
                        DirectoryExtension.SafeDelete(outputDir);

                    DirectoryExtension.Move(unpackedDir, outputDir, true);
                }

            if (outputMessage.IndexOf("Decrypt all EDAT files successfully") < 0)
                throw new InvalidOperationException("Rebuilder error, please check if .edat files are created correctly and see output below:" + Environment.NewLine + Environment.NewLine + outputMessage);

            return outputDir;
        }

        #endregion

        #region COMMON FUNCTIONS

        private static string ExtractPSARC(string srcPath, string destPath, Stream inputStream, Platform platform, bool isExternalFile = true)
        {
            var artifactDirName = Path.GetFileNameWithoutExtension(srcPath);
            if (isExternalFile)
                artifactDirName += String.Format("_{0}", platform.platform);

            var artifactDestPath = Path.Combine(destPath, artifactDirName);
            if (Directory.Exists(artifactDestPath) && isExternalFile)
                DirectoryExtension.SafeDelete(artifactDestPath);

            var psarc = new PSARC.PSARC();
            psarc.Read(inputStream, true);

            var step = Math.Round(1.0 / (psarc.TOC.Count + 2) * 100, 3);
            double progress = 0;
            GlobalExtension.ShowProgress("Inflating Entries ...");

            foreach (var entry in psarc.TOC)
            {
                // custom InflateEntries
                // var debugMe = "Check the TOC";
                var fullfilename = Path.Combine(artifactDestPath, entry.Name);

                if (Path.GetExtension(entry.Name).ToLower() == ".psarc")
                {
                    psarc.InflateEntry(entry);
                    ExtractPSARC(fullfilename, artifactDestPath, entry.Data, platform, false);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullfilename));
                    psarc.InflateEntry(entry, fullfilename);
                    if (entry.Data != null)
                    {
                        entry.Data.Dispose(); //Close();
                    }
                }

                if (!String.IsNullOrEmpty(psarc.ErrMSG)) throw new InvalidDataException(psarc.ErrMSG);

                progress += step;
                GlobalExtension.UpdateProgress.Value = (int)progress;
            }

            return artifactDestPath;
            // GlobalExtension.HideProgress();
        }

        public static void DeleteFixedAudio(string sourcePath)
        {
            try
            {
                // delete template xml remnants that may have been added by SaveTemplateFile method
                foreach (var file in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories).Where(s => s.EndsWith("RS2012.dlc.xml") || s.EndsWith("RS2014.dlc.xml")))
                    File.Delete(file);

                // delete preview audio file remnants that may have been added by LoadFromFolder method
                foreach (var file in Directory.EnumerateFiles(sourcePath, "*_preview.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wem")))
                    File.Delete(file);

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

        /// <summary>
        /// Get platform from a source path
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public static Platform GetPlatform(this string srcPath)
        {
            //TODO: validate Files by MIME type.
            if (File.Exists(srcPath))
            {
                // Get PLATFORM by Extension + Get PLATFORM by pkg EndName
                switch (Path.GetExtension(srcPath))
                {
                    case ".dat":
                        return new Platform(GamePlatform.Pc, GameVersion.RS2012);
                    case "":
                        // old method was failing for RS2014 xbox so revised to read file header
                        // return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                        const int HEADER_SIZE = 4;
                        byte[] bytesFile = new byte[HEADER_SIZE];
                        using (var fs = File.Open(srcPath, FileMode.Open))
                        {
                            fs.Read(bytesFile, 0, HEADER_SIZE);
                            fs.Close();
                        }

                        // along with a little bit of belt and suspenders works nicely
                        if (Encoding.UTF8.GetString(bytesFile) == "CON ")
                            if (TryGetPlatformByEndName(srcPath).Equals(new Platform(GamePlatform.XBox360, GameVersion.RS2014)))
                                return new Platform(GamePlatform.XBox360, GameVersion.RS2014);

                        return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                    case ".edat":
                        // must get game version from ConfigRepository for UnitTest compatibility
                        GameVersion version = (GameVersion)Enum.Parse(typeof(GameVersion), ConfigRepository.Instance()["general_defaultgameversion"]);
                        return new Platform(GamePlatform.PS3, version);
                    case ".psarc":
                        var debugMe = TryGetPlatformByEndName(srcPath);
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

        private static void UpdateSng(string songDirectory, Platform targetPlatform)
        {
            var xmlFiles = Directory.EnumerateFiles(Path.Combine(songDirectory, @"GR\Behaviors\Songs"));

            foreach (var xmlFile in xmlFiles)
            {
                if (File.Exists(xmlFile) && Path.GetExtension(xmlFile) == ".xml")
                {
                    var sngFile = Path.Combine(songDirectory, "GRExports", targetPlatform.GetPathName()[1], Path.GetFileNameWithoutExtension(xmlFile) + ".sng");
                    var arrType = ArrangementType.Guitar;

                    if (Path.GetFileName(xmlFile).ToLower().Contains("vocal"))
                    {
                        arrType = ArrangementType.Vocal;
                        SngFileWriter.Write(xmlFile, sngFile, arrType, targetPlatform);
                    }
                    else
                    {
                        Song song = Song.LoadFromFile(xmlFile);

                        if (!Enum.TryParse<ArrangementType>(song.Arrangement, out arrType))
                            if (song.Arrangement.ToLower().Contains("bass"))
                                arrType = ArrangementType.Bass;
                    }

                    SngFileWriter.Write(xmlFile, sngFile, arrType, targetPlatform);
                }
                else
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid XML file.", xmlFile));
                }
            }
        }

        private static void UpdateSng2014(string srcPath, Platform targetPlatform)
        {
            var xmlFiles = Directory.EnumerateFiles(srcPath, "*_*.xml", SearchOption.AllDirectories).ToList();
            var sngFolder = Path.Combine(srcPath, "songs", "bin", targetPlatform.GetPathName()[1].ToLower());

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
        private static void UpdateShowlights(string songDirectory, Platform targetPlatform)
        {
            bool hasShowlights = true;
            // TODO: provide some pb feedback for long process
            var info = DLCPackageData.LoadFromFolder(songDirectory, targetPlatform);
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

            if (!hasShowlights)
                UpdateAggegrateGraph(songDirectory, targetPlatform, info);
        }

        private static void UpdateManifest2014(string songDirectory, Platform targetPlatform)
        {
            if (targetPlatform.version != GameVersion.RS2014)
                return;

            string[] fileExt = new string[] { "hsan", "hson" };
            var hsanFiles = Directory.EnumerateFiles(songDirectory, "*", SearchOption.AllDirectories).Where(fi => fileExt.Any(fi.ToLower().EndsWith)).ToList();
            if (!hsanFiles.Any())
                throw new DataException("Error: could not find hsan/hson file");
            if (hsanFiles.Count > 1)
                throw new DataException("Error: there is more than one hsan/hson file");

            var manifestHeader = new ManifestHeader2014<AttributesHeader2014>(targetPlatform);
            var hsanFile = hsanFiles.First();
            var jsonFiles = Directory.EnumerateFiles(songDirectory, "*.json", SearchOption.AllDirectories).ToList();
            var xmlFiles = Directory.EnumerateFiles(songDirectory, "*.xml", SearchOption.AllDirectories).ToList();
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
                    var xmlContent = Song2014.LoadFromFile(xmlFile);

                    attr.PhraseIterations = new List<Manifest.PhraseIteration>();
                    manifestFunctions.GeneratePhraseIterationsData(attr, xmlContent, targetPlatform.version);

                    attr.Phrases = new List<Manifest.Phrase>();
                    manifestFunctions.GeneratePhraseData(attr, xmlContent);

                    attr.Sections = new List<Manifest.Section>();
                    manifestFunctions.GenerateSectionData(attr, xmlContent);

                    attr.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                    manifestFunctions.GenerateTuningData(attr, xmlContent);

                    attr.MaxPhraseDifficulty = manifestFunctions.GetMaxDifficulty(xmlContent);
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
            UpdateShowlights(songDirectory, targetPlatform);
        }

        /// <summary>
        /// Get unpacked directory path name with platform identifier 
        /// </summary>
        /// <param name="srcPath">Source Path</param>
        /// <param name="destPath">Destination Path</param>
        /// <param name="targetPlatform">Destination Platform</param>
        /// <returns>Unpacked Directory Path Name</returns>
        public static string GetUnpackedDir(string srcPath, string destPath, Platform targetPlatform)
        {
            var fnameWithoutExt = Path.GetFileNameWithoutExtension(srcPath);
            if (targetPlatform.platform == GamePlatform.PS3)
                fnameWithoutExt = fnameWithoutExt.Substring(0, fnameWithoutExt.LastIndexOf("."));

            var unpackedDir = Path.Combine(destPath, String.Format("{0}_{1}", fnameWithoutExt, targetPlatform.platform));
            if (Directory.Exists(unpackedDir))
                DirectoryExtension.SafeDelete(unpackedDir);

            return unpackedDir;
        }

        /// <summary>
        /// Recycle unpacked directory path into archive file name
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns>Archive File Name</returns>
        public static string RecycleUnpackedDir(string srcPath)
        {
            // looks for long endings first and then short endings
            var endings = new string[] { "_p_Pc", "_m_Mac", "_ps3_PS3", "_xbox_XBox360", "_Pc", "_Mac", "_PS3", "_XBox360" };
            string[] extensions = { "_p.psarc", "_m.psarc", "_ps3.psarc.edat", "_xbox", "_p.psarc", "_m.psarc", "_ps3.psarc.edat", "_xbox" };

            // reuse sanitized folder name as default file name if possible
            var destFileName = Path.GetFileName(srcPath);
            for (int ndx = 0; ndx < endings.Count(); ndx++)
            {
                if (destFileName.EndsWith(endings[ndx]))
                {
                    destFileName = destFileName.Substring(0, destFileName.LastIndexOf(endings[ndx], StringComparison.OrdinalIgnoreCase));
                    destFileName = String.Format("{0}{1}", destFileName, extensions[ndx]);
                    break;
                }
            }

            return destFileName;
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

        #endregion

    }
}

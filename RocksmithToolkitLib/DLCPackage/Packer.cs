using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X360.STFS;
using X360.Other;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class Packer
    {
        private const string ROOT_XBox360 = "Root";
        private const string ROOT_PS3 = "USRDIR";

        public static void Pack(string sourcePath, string saveFileName, bool useCryptography, bool updateSng)
        {
            Platform platform = sourcePath.GetPlatform();

            switch (platform.platform) {
                case GamePlatform.Pc:
                    if (platform.version == GameVersion.RS2012)
                        PackPC(sourcePath, saveFileName, useCryptography, updateSng);
                    else if (platform.version == GameVersion.RS2014)
                        PackPC2014(sourcePath, saveFileName);
                    break;
                case GamePlatform.XBox360:
                    PackXBox360(sourcePath, saveFileName, platform.version);
                    break;
                case GamePlatform.PS3:
                    throw new InvalidOperationException("PS3 platform is not supported at this time :(");
                case GamePlatform.None:
                    throw new InvalidOperationException("Invalid directory structure of package. \n\rDirectory: " + sourcePath);
            }
        }

        private static void PackPC(string sourcePath, string saveFileName, bool useCryptography, bool updateSng)
        {
            string[] namesBlock = Directory.GetFiles(sourcePath, "NamesBlock.bin", SearchOption.AllDirectories);
            foreach (var nb in namesBlock) {
                if (File.Exists(nb))
                    File.Delete(nb);
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

        private static void PackPC2014(string sourcePath, string saveFileName)
        {
            using (var psarcStream = new MemoryStream())
            {
                var psarc = new PSARC.PSARC();

                WalkThroughDirectory("", sourcePath, (a, b) =>
                {
                    var fileStream = File.OpenRead(b);
                    psarc.AddEntry(a, fileStream);
                });

                psarc.Write(psarcStream, true);
                psarcStream.Flush();
                psarcStream.Seek(0, SeekOrigin.Begin);

                if (Path.GetExtension(saveFileName) != ".psarc")
                    saveFileName += ".psarc";

                using (var outputFileStream = File.Create(saveFileName))
                    psarcStream.CopyTo(outputFileStream);
            }
        }

        private static void PackXBox360(string sourcePath, string saveFileName, GameVersion gameVersion) {
            foreach (var directory in Directory.EnumerateDirectories(Path.Combine(sourcePath, ROOT_XBox360))) {
                PackInnerXBox360(Path.Combine(sourcePath, ROOT_XBox360), directory);
            }

            IEnumerable<string> xboxHeaderFiles = Directory.EnumerateFiles(sourcePath, "*.txt");
            DLCPackageData songData = new DLCPackageData();
            foreach (var file in xboxHeaderFiles) {
                if (xboxHeaderFiles.Count() == 1)
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
                        throw new InvalidDataException("XBox360 header file (.txt) not found or is invalid. \n\rThe file is in the same level at 'Root' folder along with the files: 'Content image.png' and 'Package image.png' and no other file .txt can be here.", ex);
                    }
                }
                else
                {
                    throw new InvalidDataException("XBox360 header file (.txt) not found or is invalid. The file is in the same level at 'Root' folder along with the files: 'Content image.png' and 'Package image.png'. No other file .txt can be here.");
                }
            }

            IEnumerable<string> xboxFiles = Directory.EnumerateFiles(Path.Combine(sourcePath, ROOT_XBox360));
            DLCPackageCreator.BuildXBox360Package(saveFileName, songData, xboxFiles, gameVersion);

            foreach (var file in xboxFiles)
                if (Path.GetExtension(file) == ".psarc" && File.Exists(file))
                    File.Delete(file);
        }

        private static string GetHeaderValue(this string value) {
            return value.Substring(value.IndexOf(":") + 2);
        }

        private static void PackInnerXBox360(string sourcePath, string directory) {
            using (var psarcStream = new MemoryStream()) {
                var innerPsarc = new PSARC.PSARC();

                WalkThroughDirectory("", directory, (a, b) =>
                {
                    var fileStream = File.OpenRead(b);
                    innerPsarc.AddEntry(a, fileStream);
                });

                innerPsarc.Write(psarcStream, false);
                psarcStream.Flush();
                psarcStream.Seek(0, SeekOrigin.Begin);

                using (var outputFileStream = File.Create(Path.Combine(sourcePath, Path.GetFileName(directory)) + ".psarc")) {
                    psarcStream.CopyTo(outputFileStream);
                }
            }
        }

        private static void WalkThroughDirectory(string baseDir, string directory, Action<string, string> action) {
            foreach (var fl in Directory.GetFiles(directory))
                action(String.Format("{0}/{1}", baseDir, Path.GetFileName(fl)).TrimStart('/'), fl);
            foreach (var dr in Directory.GetDirectories(Path.Combine(baseDir, directory)))
                WalkThroughDirectory(String.Format("{0}/{1}", baseDir, Path.GetFileName(dr)), dr, action);
        }

        public static void Unpack(string sourceFileName, string savePath, bool useCryptography)
        {
            Platform platform = sourceFileName.GetPlatform();

            switch (platform.platform) {
                case GamePlatform.Pc:
                    if (platform.version == GameVersion.RS2014)
                        using (var inputStream = File.OpenRead(sourceFileName))
                            ExtractPSARC(sourceFileName, savePath, inputStream, platform);
                    else
                    {
                        using (var inputFileStream = File.OpenRead(sourceFileName))
                        using (var inputStream = new MemoryStream())
                        {

                            if (useCryptography)
                            {
                                RijndaelEncryptor.DecryptFile(inputFileStream, inputStream, RijndaelEncryptor.DLCKey);
                            }
                            else
                            {
                                inputFileStream.CopyTo(inputStream);
                            }
                            ExtractPSARC(sourceFileName, savePath, inputStream, platform);
                        }
                    }
                    return;
                case GamePlatform.XBox360:
                    UnpackXBox360Package(sourceFileName, savePath, platform);
                    return;
                case GamePlatform.PS3:
                    throw new InvalidOperationException("PS3 platform is not supported at this time :(");
            }
            
        }

        private static void UnpackXBox360Package(string sourceFileName, string savePath, Platform platform) {
            LogRecord x = new LogRecord();            
            STFSPackage xboxPackage = new STFSPackage(sourceFileName, x);
            if (!xboxPackage.ParseSuccess)
                throw new InvalidDataException("Invalid XBox360 Rocksmith package!\n\r" + x.Log);

            var rootDir = Path.Combine(savePath, Path.GetFileNameWithoutExtension(sourceFileName)) + String.Format("_{0}", platform.platform.ToString());
            xboxPackage.ExtractPayload(rootDir, true, true);

            foreach (var fileName in Directory.EnumerateFiles(Path.Combine(rootDir, "Root"))) {
                if (Path.GetExtension(fileName) == ".psarc") {
                    using (var outputFileStream = File.OpenRead(fileName)) {
                        ExtractPSARC(fileName, Path.GetDirectoryName(fileName), outputFileStream, new Platform(GamePlatform.XBox360, GameVersion.None));
                    }
                }

                if (File.Exists(fileName) && Path.GetExtension(fileName) == ".psarc")
                    File.Delete(fileName);
            }

            xboxPackage.CloseIO();
        }

        public static Platform GetPlatform(this string fileExtension) {
            if (File.Exists(fileExtension)) {
                switch (Path.GetExtension(fileExtension)) {
                    case ".dat":
                        return new Platform(GamePlatform.Pc, GameVersion.RS2012);
                    case "":
                        return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                    case ".pkg":
                    case ".edat":
                        return new Platform(GamePlatform.PS3, GameVersion.RS2012);
                    case ".psarc":
                        return new Platform(GamePlatform.Pc, GameVersion.RS2014);
                    default:
                        return new Platform(GamePlatform.None, GameVersion.None);
                }
            }
            else if (Directory.Exists(fileExtension))
            {
                //TODO: Need to refactor this code in near future, works, but is not the best way.
                if (File.Exists(Path.Combine(fileExtension, "APP_ID")))
                {
                    return new Platform(GamePlatform.Pc, GameVersion.RS2012);
                }
                else if (File.Exists(Path.Combine(fileExtension, "appid.appid")))
                {
                    return new Platform(GamePlatform.Pc, GameVersion.RS2014);
                }
                else if (Directory.Exists(Path.Combine(fileExtension, ROOT_XBox360)))
                {
                    return new Platform(GamePlatform.XBox360, GameVersion.RS2012);
                }
                else if (Directory.Exists(Path.Combine(fileExtension, ROOT_PS3)))
                {
                    return new Platform(GamePlatform.PS3, GameVersion.RS2012);
                }
                else
                    return GetPlatformByFolderEndName(fileExtension);
            }
            return new Platform(GamePlatform.None, GameVersion.None);
        }

        private static Platform GetPlatformByFolderEndName(string fileExtension)
        {
            var pIndex = Path.GetFileNameWithoutExtension(fileExtension).LastIndexOf("_");
            string platformString = Path.GetFileNameWithoutExtension(fileExtension).Substring(pIndex+1);
            GamePlatform p = GamePlatform.None;
            var isValid = Enum.TryParse(platformString, out p);
            if (isValid)
                return new Platform(p, GameVersion.RS2012);
            else
                return new Platform(GamePlatform.None, GameVersion.None);
        }

        private static void ExtractPSARC(string filename, string path, Stream inputStream, Platform platform)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            if (platform.platform == GamePlatform.Pc && platform.version == GameVersion.RS2012 && Path.GetExtension(filename) == ".dat" ||
                platform.platform == GamePlatform.Pc && platform.version == GameVersion.RS2014 && Path.GetExtension(filename) == ".psarc")
            {
                name += String.Format("_{0}", platform.platform.ToString());
            }
            var psarc = new PSARC.PSARC();
            psarc.Read(inputStream);
            foreach (var entry in psarc.Entries)
            {
                var fullfilename = Path.Combine(path, name, entry.Name);
                entry.Data.Seek(0, SeekOrigin.Begin);
                if (Path.GetExtension(entry.Name).ToLower() == ".psarc")
                {
                    ExtractPSARC(fullfilename, Path.Combine(path, name), entry.Data, platform);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullfilename));
                    using (var fileStream = File.Create(fullfilename))
                    {
                        entry.Data.CopyTo(fileStream);
                        entry.Data.Seek(0, SeekOrigin.Begin);
                        entry.Data.Close(); //allow tmp file to be deleted.
                    }
                }
            }
        }

        private static void UpdateSng(string songDirectory, Platform platform) {
            var xmlFiles = Directory.EnumerateFiles(Path.Combine(songDirectory, @"GR\Behaviors\Songs"));

            foreach (var xmlFile in xmlFiles) {
                if (File.Exists(xmlFile) && Path.GetExtension(xmlFile) == ".xml") {
                    var sngFile = Path.Combine(songDirectory, "GRExports", platform.GetPathName()[1], Path.GetFileNameWithoutExtension(xmlFile) + ".sng");
                    var arrType = ArrangementType.Guitar;
                    var tuning = InstrumentTuning.Standard;

                    if (Path.GetFileName(xmlFile).ToLower().IndexOf("vocal") >= 0) {
                        arrType = ArrangementType.Vocal;
                        SngFileWriter.Write(xmlFile, sngFile, arrType, platform, tuning);
                    } else {
                        Song song = Song.LoadFromFile(xmlFile);

                        if (!Enum.TryParse<ArrangementType>(song.Arrangement, out arrType))
                            if (song.Arrangement.ToLower().IndexOf("bass") >= 0)
                                arrType = ArrangementType.Bass;

                        tuning = InstrumentTuningExtensions.GetTuningByOffsets(song.Tuning.ToArray());                        
                    }

                    SngFileWriter.Write(xmlFile, sngFile, arrType, platform, tuning);
                } else {
                    throw new ArgumentException(String.Format("'{0}' is not a valid XML file.", xmlFile));
                }
            }
        }
    }
}

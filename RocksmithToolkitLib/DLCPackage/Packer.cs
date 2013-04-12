using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.Sng;
using X360.STFS;
using X360.Other;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class Packer
    {
        private const string ROOT_XBox360 = "Root";
        private const string ROOT_PS3 = "USRDIR";

        public static void Pack(string sourcePath, string saveFileName, bool useCryptography)
        {
            GamePlatform platform = sourcePath.GetPlatform();

            switch (platform) {
                case GamePlatform.Pc:
                    PackPC(sourcePath, saveFileName, useCryptography);
                    break;
                case GamePlatform.XBox360:
                    PackXBox360(sourcePath, saveFileName);
                    break;
                case GamePlatform.PS3:
                    throw new InvalidOperationException("PS3 platform is not supported at this time :(");
                case GamePlatform.None:
                    throw new InvalidOperationException("Invalid directory structure of package. \n\rDirectory: " + sourcePath);
            }
        }

        private static void PackPC(string sourcePath, string saveFileName, bool useCryptography)
        {
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
                    PackInnerPC(innerPsarcStream, directory);
                    psarc.AddEntry(directoryName + ".psarc", innerPsarcStream);
                }

                psarc.Write(psarcStream);
                psarcStream.Flush();
                psarcStream.Seek(0, SeekOrigin.Begin);

                if (Path.GetExtension(saveFileName) != ".dat")
                    saveFileName += ".dat";

                using (var outputFileStream = File.Create(saveFileName))
                {
                    if (useCryptography)
                        RijndaelEncryptor.Encrypt(psarcStream, outputFileStream, RijndaelEncryptor.DLCKey);
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
                innerPsarc.Write(output);
            }
        }

        private static void PackXBox360(string sourcePath, string saveFileName) {
            foreach (var directory in Directory.EnumerateDirectories(Path.Combine(sourcePath, ROOT_XBox360))) {
                PackInnerXBox360(Path.Combine(sourcePath, ROOT_XBox360), directory);
            }

            IEnumerable<string> xboxHeaderFiles = Directory.EnumerateFiles(sourcePath, "*.txt");
            DLCPackageData songData = new DLCPackageData();
            PackageMagic packageType = PackageMagic.CON;
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
            DLCPackageCreator.BuildXBox360Package(saveFileName, songData, xboxFiles, packageType);

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

                innerPsarc.Write(psarcStream);
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
            GamePlatform platform = sourceFileName.GetPlatform();

            switch (platform) {
                case GamePlatform.Pc:
                    using (var inputFileStream = File.OpenRead(sourceFileName))
                    using (var inputStream = new MemoryStream()) {

                        if (useCryptography) {
                            RijndaelEncryptor.Decrypt(inputFileStream, inputStream, RijndaelEncryptor.DLCKey);
                        } else {
                            inputFileStream.CopyTo(inputStream);
                        }
                        ExtractPSARC(sourceFileName, savePath, inputStream, GamePlatform.Pc);
                    }
                    return;
                case GamePlatform.XBox360:
                    UnpackXBox360Package(sourceFileName, savePath, GamePlatform.XBox360);
                    return;
                case GamePlatform.PS3:
                    throw new InvalidOperationException("PS3 platform is not supported at this time :(");
            }
            
        }

        private static void UnpackXBox360Package(string sourceFileName, string savePath, GamePlatform platform) {
            LogRecord x = new LogRecord();            
            STFSPackage xboxPackage = new STFSPackage(sourceFileName, x);
            if (!xboxPackage.ParseSuccess)
                throw new InvalidDataException("Invalid XBox360 Rocksmith package!\n\r" + x.Log);

            var rootDir = Path.Combine(savePath, Path.GetFileNameWithoutExtension(sourceFileName)) + String.Format("_{0}", platform.ToString());
            xboxPackage.ExtractPayload(rootDir, true, true);

            foreach (var fileName in Directory.EnumerateFiles(Path.Combine(rootDir, "Root"))) {
                if (Path.GetExtension(fileName) == ".psarc") {
                    using (var outputFileStream = File.OpenRead(fileName)) {
                        ExtractPSARC(fileName, Path.GetDirectoryName(fileName), outputFileStream, GamePlatform.XBox360);
                    }
                }

                if (File.Exists(fileName) && Path.GetExtension(fileName) == ".psarc")
                    File.Delete(fileName);
            }

            xboxPackage.CloseIO();
        }

        public static GamePlatform GetPlatform(this string fileExtension) {
            if (File.Exists(fileExtension)) {
                switch (Path.GetExtension(fileExtension)) {
                    case ".dat":
                        return GamePlatform.Pc;
                    case "":
                        return GamePlatform.XBox360;
                    case ".pkg":
                        return GamePlatform.PS3;
                    default:
                        return GamePlatform.None;
                }
            } else if (Directory.Exists(fileExtension)) {
                //TODO: Need to refactor this code in near future, works, but is not the best way.
                if (File.Exists(Path.Combine(fileExtension, "APP_ID"))) {
                    return GamePlatform.Pc;
                } else if (Directory.Exists(Path.Combine(fileExtension, ROOT_XBox360))) {
                    return GamePlatform.XBox360;
                } else if (Directory.Exists(Path.Combine(fileExtension, ROOT_PS3))) {
                    return GamePlatform.PS3;
                }
            }
            return GamePlatform.None;
        }

        private static void ExtractPSARC(string filename, string path, Stream inputStream, GamePlatform platform)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            if (Path.GetExtension(filename) == ".dat")
                name += String.Format("_{0}", platform.ToString());
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
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithToolkitLib.Sng;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System.Diagnostics;

namespace RocksmithToolkitLib.Ogg
{
    public static class OggFile
    {
        public enum WwiseVersion { Wwise2010, Wwise2013, None };

        public static Platform getPlatform(String inputFile)
        {
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var reader = new BinaryReader(inputFileStream))
            {
                string fileID = new string(reader.ReadChars(4));
                if (fileID == "RIFF")
                    return new Platform(GamePlatform.Pc, GameVersion.None);
                else if (fileID == "RIFX")
                    return new Platform(GamePlatform.XBox360, GameVersion.None);
            }
            return new Platform(GamePlatform.None, GameVersion.None);
        }

        public static bool needsConversion(String inputFile)
        {
            var platform = getPlatform(inputFile);
            EndianBitConverter bitConverter = platform.GetBitConverter();

            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
            {
                reader.Seek(16, SeekOrigin.Begin);
                if (reader.ReadUInt32() == 24)
                    return true;
            }
            return false;
        }

        [Obsolete("To be removed, can remove the converter tab on the GUI")]
        public static void ConvertOgg(string inputFile, string outputFileName)
        {
            VerifyHeaders(inputFile);
            var platform = getPlatform(inputFile);
            EndianBitConverter bitConverter = platform.GetBitConverter();

            using (var outputFileStream = File.Open(outputFileName, FileMode.Create))
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var writer = new EndianBinaryWriter(bitConverter, outputFileStream))
            using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
            {
                writer.Write(reader.ReadBytes(4));
                UInt32 fileSize = reader.ReadUInt32();
                fileSize -= 8; // We're removing data, so update the size in the header
                writer.Write(fileSize);
                writer.Write(reader.ReadBytes(8));
                writer.Write(66); reader.ReadUInt32(); // New fmt size is 66
                writer.Write(reader.ReadBytes(16));
                writer.Write((ushort)48); reader.ReadUInt16(); // New cbSize is 48
                writer.Write(reader.ReadBytes(6));
                reader.BaseStream.Seek(8, SeekOrigin.Current); // Skip ahead 8 bytes, we don't want the vorb chunk
                writer.Write(reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));
            }
        }

        public static void VerifyHeaders(string inputFile)
        {
            var platform = getPlatform(inputFile);
            EndianBitConverter bitConverter = platform.GetBitConverter();

            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
            {
                reader.Seek(4, SeekOrigin.Begin);
                if (reader.ReadUInt32() != reader.BaseStream.Length - 8)
                    throw new InvalidDataException("The input OGG file appears to be truncated.");

                if (System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4)) != "WAVE")
                    throw new InvalidDataException("Erorr reading input file - expected WAVE");

                if (System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4)) != "fmt ")
                    throw new InvalidDataException("Error reading input file - expected fmt");

                var fmtLength = reader.ReadUInt32();
                if (fmtLength != 24 && fmtLength != 66)
                    throw new InvalidDataException("Error reading input file - expected fmt length of 24 or 66");

                if (fmtLength == 24)
                {
                    if (reader.ReadUInt16() != 0xFFFF)
                        throw new InvalidDataException("Error reading input file - expected fmt tag of 0xFFFF");

                    reader.BaseStream.Seek(14, SeekOrigin.Current);

                    if (reader.ReadUInt16() != 6)
                        throw new InvalidDataException("Error reading input file - expected cbSize of 6");

                    reader.BaseStream.Seek(6, SeekOrigin.Current);

                    if (System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4)) != "vorb")
                        throw new InvalidDataException("Error reading input file - expected vorb");

                    if (reader.ReadUInt32() != 42)
                        throw new InvalidDataException("Error reading input file - expected vorb length of 42");
                }
            }
        }

        public static Stream ConvertOgg(string inputFile)
        {
            if (needsConversion(inputFile))
            {
                var platform = getPlatform(inputFile);
                EndianBitConverter bitConverter = platform.GetBitConverter();

                using (var outputFileStream = new MemoryStream())
                using (var inputFileStream = File.Open(inputFile, FileMode.Open))
                using (var writer = new EndianBinaryWriter(bitConverter, outputFileStream))
                using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
                {
                    writer.Write(reader.ReadBytes(4));
                    UInt32 fileSize = reader.ReadUInt32();
                    fileSize -= 8; // We're removing data, so update the size in the header
                    writer.Write(fileSize);
                    writer.Write(reader.ReadBytes(8));
                    writer.Write(66); reader.ReadUInt32(); // New fmt size is 66
                    writer.Write(reader.ReadBytes(16));
                    writer.Write((ushort)48); reader.ReadUInt16(); // New cbSize is 48
                    writer.Write(reader.ReadBytes(6));
                    reader.BaseStream.Seek(8, SeekOrigin.Current); // Skip ahead 8 bytes, we don't want the vorb chunk
                    writer.Write(reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));

                    return new MemoryStream(outputFileStream.GetBuffer(), 0, (int)outputFileStream.Length);
                }
            }

            return File.OpenRead(inputFile);
        }

        private static EndianBitConverter GetBitConverter(this Platform platform)
        {
            switch (platform.platform)
            {
                case GamePlatform.Pc:
                    return EndianBitConverter.Little;
                case GamePlatform.XBox360:
                case GamePlatform.PS3:
                    return EndianBitConverter.Big;
                default:
                    throw new InvalidDataException("The input OGG file doesn't appear to be a valid Wwise 2010 OGG file.");
            }
        }

        public static void Revorb(string file, string outputFileName, string appPath, WwiseVersion wwiseVersion)
        {
            string ww2oggPath = Path.Combine(appPath, "ww2ogg.exe");
            string revorbPath = Path.Combine(appPath, "revorb.exe");
            string codebooksPath = Path.Combine(appPath, "packed_codebooks.bin"); // Default
            string codebooks603Path = Path.Combine(appPath, "packed_codebooks_aoTuV_603.bin");

            // Verifying if third part apps is in root application directory
            if (!File.Exists(ww2oggPath))
                throw new FileNotFoundException("ww2ogg executable not found!");

            if (!File.Exists(revorbPath))
                throw new FileNotFoundException("revorb executable not found!");

            if (!File.Exists(codebooksPath))
                throw new FileNotFoundException("packed_codebooks.bin not found!");

            if (!File.Exists(codebooks603Path))
                throw new FileNotFoundException("packed_codebooks_aoTuV_603.bin not found!");

            // Processing with ww2ogg
            Process ww2oggProcess = new Process();
            ww2oggProcess.StartInfo.FileName = ww2oggPath;
            ww2oggProcess.StartInfo.WorkingDirectory = appPath;

            switch (wwiseVersion)
            {
                case WwiseVersion.Wwise2010:
                    ww2oggProcess.StartInfo.Arguments = String.Format("\"{0}\" -o \"{1}\"", file, outputFileName);
                    break;
                case WwiseVersion.Wwise2013:
                    ww2oggProcess.StartInfo.Arguments = String.Format("\"{0}\" -o \"{1}\" --pcb {2}", file, outputFileName, codebooks603Path);
                    break;
                default:
                    throw new InvalidOperationException("Wwise version not supported or invalid input file.");
            }
            
            ww2oggProcess.StartInfo.UseShellExecute = false;
            ww2oggProcess.StartInfo.CreateNoWindow = true;
            ww2oggProcess.StartInfo.RedirectStandardOutput = true;

            ww2oggProcess.Start();
            ww2oggProcess.WaitForExit();
            string ww2oggResult = ww2oggProcess.StandardOutput.ReadToEnd();

            if (ww2oggResult.IndexOf("error") > -1)
                throw new Exception("ww2ogg process error." + Environment.NewLine + ww2oggResult);

            // Processing with revorb
            Process revorbProcess = new Process();
            revorbProcess.StartInfo.FileName = revorbPath;
            revorbProcess.StartInfo.WorkingDirectory = appPath;
            revorbProcess.StartInfo.Arguments = String.Format("\"{0}\"", outputFileName);
            revorbProcess.StartInfo.UseShellExecute = false;
            revorbProcess.StartInfo.CreateNoWindow = true;
            revorbProcess.StartInfo.RedirectStandardOutput = true;

            revorbProcess.Start();
            revorbProcess.WaitForExit();
            string revorbResult = revorbProcess.StandardOutput.ReadToEnd();

            if (ww2oggResult.IndexOf("error") > -1)
            {
                if (File.Exists(outputFileName))
                    File.Delete(outputFileName);

                throw new Exception("revorb process error." + Environment.NewLine + revorbResult);
            }
        }

        public static WwiseVersion GetWwiseVersion(this string extension)
        {
            switch (extension)
            {
                case ".ogg":
                    return WwiseVersion.Wwise2010;
                case ".wem":
                    return WwiseVersion.Wwise2013;
                default:
                    throw new InvalidOperationException("Audio file not supported.");
            }
        }
    }
}

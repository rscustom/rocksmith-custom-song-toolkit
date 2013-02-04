using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithToolkitLib.Sng;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace RocksmithToolkitLib.Ogg
{
    public class OggFile
    {
        public static GamePlatform getPlatform(String inputFile)
        {
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var reader = new BinaryReader(inputFileStream))
            {
                string fileID = new string(reader.ReadChars(4));
                if (fileID == "RIFF")
                    return GamePlatform.Pc;
                else if (fileID == "RIFX")
                    return GamePlatform.XBox360;
            }
            return GamePlatform.None;
        }

        public static bool needsConversion(String inputFile)
        {
            var platform = getPlatform(inputFile);
            EndianBitConverter bitConverter = EndianBitConverter.Big;
            if (platform == GamePlatform.Pc)
                bitConverter = EndianBitConverter.Little;

            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
            {
                reader.Seek(16, SeekOrigin.Begin);
                if (reader.ReadUInt32() == 24)
                    return true;
            }
            return false;
        }

        // to be removed, can remove the converter tab on the GUI
        public static void ConvertOgg(string inputFile, string outputFileName)
        {
            var platform = getPlatform(inputFile);
            EndianBitConverter bitConverter = EndianBitConverter.Big;
            if (platform == GamePlatform.Pc)
                bitConverter = EndianBitConverter.Little;
            else if (platform == GamePlatform.XBox360)
                bitConverter = EndianBitConverter.Big;
            else
                throw new InvalidDataException("The input OGG file doesn't appear to be a valid Wwise 2010 OGG file.");

            using (var outputFileStream = File.Open(outputFileName, FileMode.Create))
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var writer = new EndianBinaryWriter(bitConverter, outputFileStream))
            using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
            {
                reader.Seek(4, SeekOrigin.Begin);
                if (reader.ReadUInt32() != reader.BaseStream.Length - 8)
                    throw new InvalidDataException("The input OGG file appears to be truncated.");

                if (System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4)) != "WAVE")
                    throw new InvalidDataException("Erorr reading input file - expected WAVE");

                if (System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4)) != "fmt ")
                    throw new InvalidDataException("Error reading input file - expected fmt");

                if (reader.ReadUInt32() != 24)
                    throw new InvalidDataException("Error reading input file - expected fmt length of 24");

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

                reader.Seek(0, SeekOrigin.Begin);
                writer.Write(reader.ReadBytes(4));
                UInt32 fileSize = reader.ReadUInt32();
                fileSize -= 8; // We're removing data, so update the size in the header
                writer.Write(fileSize);
                writer.Write(reader.ReadBytes(8));
                writer.Write(66); reader.ReadUInt32(); // New fmt size is 66
                writer.Write(reader.ReadBytes(16));
                writer.Write(48); reader.ReadUInt16(); // New cbSize is 48
                writer.Write(reader.ReadBytes(6));
                reader.BaseStream.Seek(8, SeekOrigin.Current); // Skip ahead 8 bytes, we don't want the vorb chunk
                writer.Write(reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));
            }
        }

        public static Stream ConvertOgg(string inputFile)
        {
            if (needsConversion(inputFile))
            {
                var platform = getPlatform(inputFile);
                EndianBitConverter bitConverter = EndianBitConverter.Big;
                if (platform == GamePlatform.Pc)
                    bitConverter = EndianBitConverter.Little;
                else if (platform == GamePlatform.XBox360)
                    bitConverter = EndianBitConverter.Big;
                else
                    throw new InvalidDataException("The input OGG file doesn't appear to be a valid Wwise 2010 OGG file.");

                using (var outputFileStream = new MemoryStream())
                using (var inputFileStream = File.Open(inputFile, FileMode.Open))
                using (var writer = new EndianBinaryWriter(bitConverter, outputFileStream))
                using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
                {
                    reader.Seek(4, SeekOrigin.Begin);
                    if (reader.ReadUInt32() != reader.BaseStream.Length - 8)
                        throw new InvalidDataException("The input OGG file appears to be truncated.");

                    if (System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4)) != "WAVE")
                        throw new InvalidDataException("Erorr reading input file - expected WAVE");

                    if (System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4)) != "fmt ")
                        throw new InvalidDataException("Error reading input file - expected fmt");

                    if (reader.ReadUInt32() != 24)
                        throw new InvalidDataException("Error reading input file - expected fmt length of 24");

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

                    reader.Seek(0, SeekOrigin.Begin);
                    writer.Write(reader.ReadBytes(4));
                    UInt32 fileSize = reader.ReadUInt32();
                    fileSize -= 8; // We're removing data, so update the size in the header
                    writer.Write(fileSize);
                    writer.Write(reader.ReadBytes(8));
                    writer.Write(66); reader.ReadUInt32(); // New fmt size is 66
                    writer.Write(reader.ReadBytes(16));
                    writer.Write(48); reader.ReadUInt16(); // New cbSize is 48
                    writer.Write(reader.ReadBytes(6));
                    reader.BaseStream.Seek(8, SeekOrigin.Current); // Skip ahead 8 bytes, we don't want the vorb chunk
                    writer.Write(reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));

                    return new MemoryStream(outputFileStream.GetBuffer(), 0, (int)outputFileStream.Length);
                }
            }

            return File.OpenRead(inputFile);
        }
    }
}

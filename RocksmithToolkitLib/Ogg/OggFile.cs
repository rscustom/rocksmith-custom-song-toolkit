using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RocksmithToolkitLib.Ogg
{
    public class OggFile
    {
        private string inputOgg;
        public bool BigEndian { get; set; }

        public void LoadOgg(string inFile)
        {
            inputOgg = inFile;
            VerifyHeaders();
        }

        public void WriteOgg(string outFile)
        {
            VerifyHeaders(); // Just in case the file changed

            using (BinaryWriter writer = new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(inputOgg, FileMode.Open)))
                {
                    writer.Write(reader.ReadBytes(4));
                    UInt32 fileSize = ReadUInt32(reader.ReadBytes(4));
                    fileSize -= 8; // We're removing data, so update the size in the header
                    writer.Write(WriteUInt32(fileSize));
                    writer.Write(reader.ReadBytes(8));
                    writer.Write(WriteUInt32(66)); reader.ReadUInt32(); // New fmt size is 66
                    writer.Write(reader.ReadBytes(16));
                    writer.Write(WriteUInt16(48)); reader.ReadUInt16(); // New cbSize is 48
                    writer.Write(reader.ReadBytes(6));
                    reader.BaseStream.Seek(8, SeekOrigin.Current); // Skip ahead 8 bytes, we don't want the vorb chunk
                    writer.Write(reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));
                }
            }
        }

        private ushort ReadUInt16(byte[] input)
        {
            if (BigEndian)
                Array.Reverse(input);
            return BitConverter.ToUInt16(input, 0);
        }

        private uint ReadUInt32(byte[] input)
        {
            if (BigEndian)
                Array.Reverse(input);
            return BitConverter.ToUInt32(input, 0);
        }

        private byte[] WriteUInt16(ushort input)
        {
            byte[] result = BitConverter.GetBytes(input);
            if (BigEndian)
                Array.Reverse(result);
            return result;
        }

        private byte[] WriteUInt32(uint input)
        {
            byte[] result = BitConverter.GetBytes(input);
            if (BigEndian)
                Array.Reverse(result);
            return result;
        }

        private void VerifyHeaders()
        {
            using (var oggFile = File.Open(inputOgg, FileMode.Open))
            using (var reader = new BinaryReader(oggFile))
            {
                string fileID = new string(reader.ReadChars(4));
                if (fileID == "RIFF")
                    BigEndian = false;
                else if (fileID == "RIFX")
                    BigEndian = true;
                else
                    throw new InvalidDataException("The input OGG file doesn't appear to be a valid Wwise 2010 OGG file.");

                if (ReadUInt32(reader.ReadBytes(4)) != reader.BaseStream.Length - 8)
                    throw new InvalidDataException("The input OGG file appears to be truncated.");

                if (new string(reader.ReadChars(4)) != "WAVE")
                    throw new InvalidDataException("Erorr reading input file - expected WAVE");

                if (new string(reader.ReadChars(4)) != "fmt ")
                    throw new InvalidDataException("Error reading input file - expected fmt");

                if (ReadUInt32(reader.ReadBytes(4)) != 24)
                    throw new InvalidDataException("Error reading input file - expected fmt length of 24");

                if (BitConverter.ToUInt16(reader.ReadBytes(2), 0) != 0xFFFF)
                    throw new InvalidDataException("Error reading input file - expected fmt tag of 0xFFFF");

                reader.BaseStream.Seek(14, SeekOrigin.Current);

                if (ReadUInt16(reader.ReadBytes(2)) != 6)
                    throw new InvalidDataException("Error reading input file - expected cbSize of 6");

                reader.BaseStream.Seek(6, SeekOrigin.Current);

                if (new string(reader.ReadChars(4)) != "vorb")
                    throw new InvalidDataException("Error reading input file - expected vorb");

                if (ReadUInt32(reader.ReadBytes(4)) != 42)
                    throw new InvalidDataException("Error reading input file - expected vorb length of 42");
            }
        }
    }
}

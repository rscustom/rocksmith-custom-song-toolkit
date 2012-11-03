using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace RocksmithEncoder
{
    class OggFile
    {
        string inputOgg;
        string outputOgg;
        public bool bigEndian = false;
        bool isValid = true;

        ushort ReadUInt16(byte[] input)
        {
            if (bigEndian)
            {
                Array.Reverse(input);
                return BitConverter.ToUInt16(input, 0);
            }
            else
                return BitConverter.ToUInt16(input, 0);
        }
        uint ReadUInt32(byte[] input)
        {
            if (bigEndian)
            {
                Array.Reverse(input);
                return BitConverter.ToUInt32(input, 0);
            }
            else
                return BitConverter.ToUInt32(input, 0);
        }

        byte[] WriteUInt16(ushort input)
        {
            if (bigEndian)
            {
                byte[] temp = BitConverter.GetBytes(input);
                Array.Reverse(temp);
                return temp;
            }
            else
                return BitConverter.GetBytes(input);
        }
        byte[] WriteUInt32(uint input)
        {
            if (bigEndian)
            {
                byte[] temp = BitConverter.GetBytes(input);
                Array.Reverse(temp);
                return temp;
            }
            else
                return BitConverter.GetBytes(input);
        }

        bool ShowError(string error)
        {
            isValid = false;
            MessageBox.Show(error);
            return false;
        }

        bool VerifyHeaders()
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inputOgg, FileMode.Open)))
            {
                string fileID = new string(reader.ReadChars(4));
                if (fileID == "RIFF")
                    bigEndian = false;
                else if (fileID == "RIFX")
                    bigEndian = true;
                else
                    return ShowError("The input OGG file doesn't appear to be a valid Wwise 2010 OGG file.");

                if (ReadUInt32(reader.ReadBytes(4)) != reader.BaseStream.Length - 8)
                    return ShowError("The input OGG file appears to be truncated.");

                if (new string(reader.ReadChars(4)) != "WAVE")
                    return ShowError("Erorr reading input file - expected WAVE");

                if (new string(reader.ReadChars(4)) != "fmt ")
                    return ShowError("Error reading input file - expected fmt");

                if (ReadUInt32(reader.ReadBytes(4)) != 24)
                    return ShowError("Error reading input file - expected fmt length of 24");

                if (BitConverter.ToUInt16(reader.ReadBytes(2), 0) != 0xFFFF)
                    return ShowError("Error reading input file - expected fmt tag of 0xFFFF");

                reader.BaseStream.Seek(14, SeekOrigin.Current);

                if (ReadUInt16(reader.ReadBytes(2)) != 6)
                    return ShowError("Error reading input file - expected cbSize of 6");

                reader.BaseStream.Seek(6, SeekOrigin.Current);

                if (new string(reader.ReadChars(4)) != "vorb")
                    return ShowError("Error reading input file - expected vorb");

                if (ReadUInt32(reader.ReadBytes(4)) != 42)
                    return ShowError("Error reading input file - expected vorb length of 42");

                return isValid;
            }
        }

        public bool LoadOgg(string inFile)
        {
            inputOgg = inFile;
            return VerifyHeaders();
        }

        public void WriteOgg(string outFile)
        {
            outputOgg = outFile;
            VerifyHeaders(); // Just in case the file changed

            using (BinaryWriter writer = new BinaryWriter(File.Open(outputOgg, FileMode.Create)))
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
    }
}

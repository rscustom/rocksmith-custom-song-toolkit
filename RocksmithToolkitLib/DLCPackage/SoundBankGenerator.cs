using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithToolkitLib.Properties;

namespace RocksmithToolkitLib.DLCPackage
{
    public class SoundBankGenerator
    {
        private static uint HashString(String str)
        {
            char[] bytes = str.ToLower().ToCharArray();
            uint hash = 2166136261;
            for (var i = 0; i < str.Length; i++)
            {
                hash *= 16777619;
                hash = hash ^ bytes[i];
            }

            return hash;
        }

        public static string GenerateSoundBank(string dlcName, Stream audioStream, Stream outStream)
        {
            string eventName = "Play_" + dlcName;
            string bankName = "Song_" + dlcName;
            var id = RandomGenerator.NextInt();
            using (var bankStream = new MemoryStream(Resources.soundbank))
            using (var bankReader = new BinaryReader(bankStream))
            {
                var audioReader = new BinaryReader(audioStream);
                var bankWriter = new BinaryWriter(outStream);
                bankWriter.Write(bankReader.ReadBytes(0x2c));
                bankReader.ReadInt32();
                bankWriter.Write(id);
                bankWriter.Write(bankReader.ReadInt32());
                int dataSize = bankReader.ReadInt32();
                bankWriter.Write(dataSize);
                bankWriter.Write(bankReader.ReadInt32());
                bankWriter.Write(bankReader.ReadInt32());
                bankWriter.Write(audioReader.ReadBytes(dataSize));
                bankReader.BaseStream.Seek(dataSize, SeekOrigin.Current);
                bankWriter.Write(bankReader.ReadBytes(0x1d));
                bankWriter.Write(id);
                bankWriter.Write(id);
                bankReader.BaseStream.Seek(8, SeekOrigin.Current);
                bankWriter.Write(bankReader.ReadBytes(0x1E1));
                bankReader.ReadInt32();
                bankWriter.Write(HashString(eventName));
                bankWriter.Write(bankReader.ReadBytes(0x14));
                bankWriter.Write(0xc + bankName.Length + 1);
                bankReader.ReadInt32();
                bankWriter.Write(bankReader.ReadBytes(0xc));
                bankWriter.Write((byte)bankName.Length);
                bankWriter.Write(Encoding.ASCII.GetBytes(bankName));
                bankWriter.Flush();
            }
            return id.ToString();
        }
    }
}

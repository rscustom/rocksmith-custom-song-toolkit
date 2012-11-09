using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RocksmithDLCCreator
{
    public class SoundBankGenerator
    {
        public static string GenerateSoundBank(string bankName, Stream audioStream, Stream outStream)
        {
            var id = RandomGenerator.NextInt();
            using (var bankReader = new BinaryReader(System.IO.File.OpenRead("soundbank.bnk")))
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
                bankWriter.Write(bankReader.ReadBytes(0x1F9));
                bankWriter.Write(0xc + bankName.Length + 1);
                bankReader.ReadInt32();
                bankWriter.Write(bankReader.ReadBytes(0xc));
                bankWriter.Write((byte)bankName.Length);
                bankWriter.Write(ASCIIEncoding.ASCII.GetBytes(bankName));
                bankWriter.Flush();
            }
            return id.ToString();
        }
    }
}

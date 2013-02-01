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
        public static string GenerateSoundBank(string bankName, Stream audioStream, Stream outStream)
        {
            var id = RandomGenerator.NextInt();
            using (var bankStream = new MemoryStream(Resources.soundbank))
            using (var bankReader = new BinaryReader(bankStream))
            {
                var bankWriter = new BinaryWriter(outStream);
                bankWriter.Write(bankReader.ReadBytes(0x3d));
                bankWriter.Write(id);
                bankWriter.Write(id);
                bankWriter.Write(bankReader.ReadBytes(0x180));
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

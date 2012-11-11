using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class RijndaelEncryptor
    {
        public static void Encrypt(Stream input, Stream output)
        {
            using (var rij = CreateRijndael())
            {
                Crypto(input, output, rij.CreateEncryptor());
            }
        }

        public static void Decrypt(Stream input, Stream output)
        {
            using (var rij = CreateRijndael())
            {
                Crypto(input, output, rij.CreateDecryptor());
            }
        }

        private static RijndaelManaged CreateRijndael()
        {
            return new RijndaelManaged
            {
                Padding = PaddingMode.None,
                Mode = CipherMode.ECB,
                BlockSize = 128,
                IV = new byte[16],
                Key = new byte[32]
                {
                    0xFA, 0x6F, 0x4F, 0x42, 0x3E, 0x66, 0x9F, 0x9E,
                    0x6A, 0xD2, 0x3A, 0x2F, 0x8F, 0xE5, 0x81, 0x88,
                    0x63, 0xD9, 0xB8, 0xFD, 0xED, 0xDF, 0xFE, 0xBD,
                    0x12, 0xB2, 0x7F, 0x76, 0x80, 0xD1, 0x51, 0x41
                }
            };
        }

        private static void Crypto(Stream input, Stream output, ICryptoTransform transform)
        {
            var buffer = new byte[1024];
            var cs = new CryptoStream(output, transform, CryptoStreamMode.Write);
            for (long i = 0; i < input.Length; i += buffer.Length)
            {
                int sz = (int)Math.Min(input.Length - input.Position, buffer.Length);
                input.Read(buffer, 0, sz);
                cs.Write(buffer, 0, sz);
            }
            int pad = buffer.Length - (int)(input.Length % buffer.Length);
            if (pad > 0)
                cs.Write(new byte[pad], 0, pad);
            cs.Flush();
            output.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }
    }
}

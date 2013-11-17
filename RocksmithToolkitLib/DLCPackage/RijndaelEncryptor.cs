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
        #region RS1

        public static byte[] DLCKey = new byte[32]
        {
            0xFA, 0x6F, 0x4F, 0x42, 0x3E, 0x66, 0x9F, 0x9E,
            0x6A, 0xD2, 0x3A, 0x2F, 0x8F, 0xE5, 0x81, 0x88,
            0x63, 0xD9, 0xB8, 0xFD, 0xED, 0xDF, 0xFE, 0xBD,
            0x12, 0xB2, 0x7F, 0x76, 0x80, 0xD1, 0x51, 0x41
        };

        public static byte[] PCFilesKey = new byte[32]
        {
            0xB8, 0x7A, 0x00, 0xBD, 0xB8, 0x9C, 0x21, 0x03,
            0xA3, 0x94, 0xC0, 0x44, 0x71, 0x51, 0xEE, 0xC4,
            0x3C, 0x3F, 0x72, 0x17, 0xCA, 0x7F, 0x44, 0xC1,
            0xE4, 0x36, 0xFC, 0xFC, 0x84, 0xE6, 0xE7, 0x15
        };

        #endregion

        #region RS2

        public static byte[] PsarcKey = new byte[32]
        {
            0xC5, 0x3D, 0xB2, 0x38, 0x70, 0xA1, 0xA2, 0xF7,
            0x1C, 0xAE, 0x64, 0x06, 0x1F, 0xDD, 0x0E, 0x11,
            0x57, 0x30, 0x9D, 0xC8, 0x52, 0x04, 0xD4, 0xC5,
            0xBF, 0xDF, 0x25, 0x09, 0x0D, 0xF2, 0x57, 0x2C
        };

        public static byte[] SngKeyMac = new byte[32]
        {
            0x98, 0x21, 0x33, 0x0E, 0x34, 0xB9, 0x1F, 0x70,
            0xD0, 0xA4, 0x8C, 0xBD, 0x62, 0x59, 0x93, 0x12,
            0x69, 0x70, 0xCE, 0xA0, 0x91, 0x92, 0xC0, 0xE6,
            0xCD, 0xA6, 0x76, 0xCC, 0x98, 0x38, 0x28, 0x9D
        };

        public static byte[] SngKeyPC = new byte[32]
        {
            0xCB, 0x64, 0x8D, 0xF3, 0xD1, 0x2A, 0x16, 0xBF,
            0x71, 0x70, 0x14, 0x14, 0xE6, 0x96, 0x19, 0xEC,
            0x17, 0x1C, 0xCA, 0x5D, 0x2A, 0x14, 0x2E, 0x3E,
            0x59, 0xDE, 0x7A, 0xDD, 0xA1, 0x8A, 0x3A, 0x30
        };

        #endregion

        public static void EncryptFile(Stream input, Stream output, byte[] key)
        {
            using (var rij = new RijndaelManaged())
            {
                InitRijndael(rij, key, CipherMode.ECB);
                Crypto(input, output, rij.CreateEncryptor(), input.Length);
            }
        }

        public static void DecryptFile(Stream input, Stream output, byte[] key)
        {
            using (var rij = new RijndaelManaged())
            {
                InitRijndael(rij, key, CipherMode.ECB);
                Crypto(input, output, rij.CreateDecryptor(), input.Length);
            }
        }

        public static void EncryptSngData(Stream input, Stream output, byte[] key)
        {
            byte[] iv = new byte[16];
            using (var rij = new RijndaelManaged())
            {
                InitRijndael(rij, key, CipherMode.CFB);
                rij.GenerateIV();
                iv = rij.IV;
                output.Write(iv, 0, iv.Length);

                var buffer = new byte[16];
                long len = input.Length - input.Position;
                for (long i = 0; i < len; i += buffer.Length)
                {
                    using (ICryptoTransform transform = rij.CreateEncryptor())
                    {
                        var cs = new CryptoStream(output, transform, CryptoStreamMode.Write);
                        int bytesread = input.Read(buffer, 0, buffer.Length);
                        cs.Write(buffer, 0, bytesread);

                        int pad = buffer.Length - bytesread;
                        if (pad > 0)
                            cs.Write(new byte[pad], 0, pad);

                        cs.FlushFinalBlock();
                    }

                    int j;
                    bool carry;
                    for (j = (rij.IV.Length) - 1, carry = true; j >= 0 && carry; j--)
                        carry = ((iv[j] = (byte)(rij.IV[j] + 1)) == 0);
                    rij.IV = iv;
                }
            }
        }

        public static void DecryptSngData(Stream input, Stream output, byte[] key)
        {
            var reader = new BinaryReader(input);
            reader.ReadBytes(8); //skip header
            byte[] iv = reader.ReadBytes(16);
            using (var rij = new RijndaelManaged())
            {
                InitRijndael(rij, key, CipherMode.CFB);
                rij.IV = iv;

                var buffer = new byte[16];
                long len = input.Length - input.Position;
                for (long i = 0; i < len; i += buffer.Length)
                {
                    using (ICryptoTransform transform = rij.CreateDecryptor())
                    {
                        var cs = new CryptoStream(output, transform, CryptoStreamMode.Write);
                        int bytesread = input.Read(buffer, 0, buffer.Length);
                        cs.Write(buffer, 0, bytesread);

                        int pad = buffer.Length - bytesread;
                        if (pad > 0)
                            cs.Write(new byte[pad], 0, pad);

                        cs.Flush();
                    }

                    int j;
                    bool carry;
                    for (j = (rij.IV.Length) - 1, carry = true; j >= 0 && carry; j--)
                        carry = ((iv[j] = (byte)(rij.IV[j] + 1)) == 0);
                    rij.IV = iv;
                }
                output.SetLength(input.Length - (iv.Length + 8));
            }
        }

        public static void EncryptPSARC(Stream input, Stream output, long len)
        {
            using (var rij = new RijndaelManaged())
            {
                InitRijndael(rij, PsarcKey, CipherMode.CFB);
                Crypto(input, output, rij.CreateEncryptor(), len);
            }
        }

        public static void DecryptPSARC(Stream input, Stream output, long len)
        {
            using (var rij = new RijndaelManaged())
            {
                InitRijndael(rij, PsarcKey, CipherMode.CFB);
                Crypto(input, output, rij.CreateDecryptor(), len);
            }
        }

        private static void InitRijndael(Rijndael rij, byte[] key, CipherMode cipher)
        {
            rij.Padding = PaddingMode.None;
            rij.Mode = cipher;
            rij.BlockSize = 128;	// byte[16]
            rij.IV = new byte[16];
            rij.Key = key;			// byte[32]
        }

        private static void Crypto(Stream input, Stream output, ICryptoTransform transform, long len)
        {
            var buffer = new byte[512];
            var cs = new CryptoStream(output, transform, CryptoStreamMode.Write);
            for (long i = 0; i < len; i += buffer.Length)
            {
                int sz = (int)Math.Min(len - input.Position, buffer.Length);
                input.Read(buffer, 0, sz);
                cs.Write(buffer, 0, sz);
            }
            //Its need only for RS1, RS2 works fine w\o ?
            int pad = buffer.Length - (int)(len % buffer.Length);
            if (pad > 0)
                cs.Write(new byte[pad], 0, pad);

            cs.Flush();
            output.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }
    }
}

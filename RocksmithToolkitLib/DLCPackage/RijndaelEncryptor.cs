using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using MiscUtil.Conversion;
using MiscUtil.IO;

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
        //metadata
        public static byte[] PCMetaDatKey = new byte[32] 
        {
            0x5F, 0xB0, 0x23, 0xEF, 0x19, 0xD5, 0xDC, 0x37,
            0xAD, 0xDA, 0xC8, 0xF0, 0x17, 0xF8, 0x8F, 0x0E,
            0x98, 0x18, 0xA3, 0xAC, 0x2F, 0x72, 0x46, 0x96,
            0xA5, 0x9D, 0xE2, 0xBF, 0x05, 0x25, 0x12, 0xEB
        };
        //profile and other cdr profile.json stuff common for RS2\RS1
        public static byte[] PCSaveKey = new byte[32] 
        {
            0x72, 0x8B, 0x36, 0x9E, 0x24, 0xED, 0x01, 0x34,
            0x76, 0x85, 0x11, 0x02, 0x18, 0x12, 0xAF, 0xC0,
            0xA3, 0xC2, 0x5D, 0x02, 0x06, 0x5F, 0x16, 0x6B,
            0x4B, 0xCC, 0x58, 0xCD, 0x26, 0x44, 0xF2, 0x9E
        };

        #endregion

        /// <summary>
        /// All profile stuff: crd (u play credentials), LocalProfiles.json and profiles themselves
        /// Good for RS2014 and RS1
        /// </summary>
        /// <param name="str"></param>
        /// <param name="outStream"></param>
        public static void DecryptProfile(Stream str, Stream outStream)
        {
            var source = EndianBitConverter.Little;
            var dec = EndianBitConverter.Big;

            using (var decrypted = new MemoryStream())
            using (EndianBinaryReader br = new EndianBinaryReader(source, str))
            using (EndianBinaryReader brDec = new EndianBinaryReader(dec, decrypted))
            {
                //EVAS + header
                br.ReadBytes(16);
                byte[] key = PCSaveKey;
                uint zLen = br.ReadUInt32(); //size
                // baseStr.pos = 20
                DecryptFile(br.BaseStream, decrypted, key);

                //unZip
                int bSize = 1;
                brDec.BaseStream.Position = 0;
                ushort xU = brDec.ReadUInt16();
                //back to 0
                brDec.BaseStream.Position -= 2;
                if (xU == 30938)//LE 55928 //BE 30938
                {
                    var z = new zlib.ZInputStream(brDec.BaseStream);
                    do
                    {
                        byte[] buf = new byte[bSize];
                        z.read(buf, 0, bSize);
                        outStream.Write(buf, 0, bSize);
                    } while (outStream.Length < (long)zLen);
                    z.Close();
                }
            }
            outStream.Flush();
            outStream.Position = 0;
        }

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
            output.Flush();
            output.Seek(0, SeekOrigin.Begin);
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
            rij.BlockSize = 128;    // byte[16]
            rij.IV = new byte[16];
            rij.Key = key;          // byte[32]
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

        #region PS3 EDAT Encrypt/Decrypt

        /// <summary>
        /// Encrypt using TrueAncestor Edat Rebuilder (files must be in "/edat" folder in application root directory)
        /// </summary>
        /// <returns>Output message from execution</returns>
        public static string EncryptPS3Edat()
        {
            string errors = string.Empty;
            var files = Directory.EnumerateFiles(Path.Combine(toolkitPath, "edat"), "*.psarc");
            foreach (var InFile in files)
            {
                string OutFile = InFile+".edat";
                string command = String.Format("EncryptEDAT \"{0}\" \"{1}\" {2} {3} {4} {5} {6}",
                    InFile, OutFile, kLic, ContentID, Flags, Type, Version);
                errors += EdatCrypto(command);
            }
            return String.IsNullOrEmpty(errors) ? "Encrypt all EDAT files successfully" : errors;
        }

        /// <summary>
        /// Decrypt using TrueAncestor Edat Rebuilder (files must be in "/edat" folder in application root directory)
        /// </summary>
        /// <returns>Output message from execution</returns>
        public static string DecryptPS3Edat()
        {
            string errors = string.Empty;
            var files = Directory.EnumerateFiles(Path.Combine(toolkitPath, "edat"), "*.edat");
            foreach (var InFile in files)
            {
                string OutFile = Path.ChangeExtension(InFile, ".dat");
                string command = String.Format("DecryptFree \"{0}\" \"{1}\" {2}", 
                    InFile, OutFile, kLic);
                errors += EdatCrypto(command);
            }
            return String.IsNullOrEmpty(errors) ? "Decrypt all EDAT files successfully" : errors;
        }
        private const string Flags = "0C", //0x0c
                             Type = "00", 
                             Version = "03";//02 or 03
        private const string kLic = "CB4A06E85378CED307E63EFD1084C19D";
        private const string ContentID = "UP0001-BLUS30670_00-RS001PACK0000003";
        private static readonly string toolkitPath = Path.GetDirectoryName(Application.ExecutablePath);

        private static string EdatCrypto(string command) 
        {// Encrypt/decrypt using TrueAncestor Edat Rebuilder v1.4c
            string core = Path.Combine(toolkitPath, "tool/core.jar");
            string APP = "java";

            Process PS3Process = new Process();
            PS3Process.StartInfo.FileName = APP;
            PS3Process.StartInfo.Arguments = String.Format("-cp \"{0}\" -Xms128m -Xmx1024m {1}", core, command);
            PS3Process.StartInfo.WorkingDirectory = toolkitPath;
            PS3Process.StartInfo.UseShellExecute = false;
            PS3Process.StartInfo.CreateNoWindow = true;
            PS3Process.StartInfo.RedirectStandardError = true;

            PS3Process.Start();
            PS3Process.WaitForExit();

            string stdout = PS3Process.StandardError.ReadToEnd();
            PS3Process.Close();
            //Improove me please
            if (stdout.Contains("is not recognized"))
                return "No JDK or JRE is intsalled on your machine";
            return "";
        }

        #endregion
    }
}

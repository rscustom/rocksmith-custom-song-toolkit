using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.Sng;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class SoundBankGenerator
    {
        private const string PLAY = "Play_";
        private const string PLAY30SEC = "Play_30Sec_";
        private const string SONG = "Song_";
        private static readonly int[] bnkPCOffsets = { 0x2c, 0x1d, 0x17, 0xfa, 0xc8, 0x14, 0xc };
        private static readonly int[] bnkConsoleOffsets = { 0x7ec, 0x1d, 0x17, 0xfa, 0xc8, 0x14, 0xc };
        private static readonly int[] bnkPC2014Offsets = { 0x2c, 0x1d, 0x1a, 0x78, 0x00, 0xc, 0xc };
        private static readonly int[] bnkConsole2014Offsets = { 0x7ec, 0x1d, 0x1a, 0x78, 0x00, 0xc, 0xc };
        
        public static IList<int> GetOffsets(this Platform platform) {
            switch (platform.version)
            {
                case GameVersion.RS2012:
                    switch (platform.platform)
                    {
                        case GamePlatform.Pc:
                            return bnkPCOffsets;
                        case GamePlatform.XBox360:
                        case GamePlatform.PS3:
                            return bnkConsoleOffsets;
                        default:
                            throw new InvalidOperationException("Unexpected game platform value");
                    }
                case GameVersion.RS2014:
                    switch (platform.platform)
                    {
                        case GamePlatform.Pc:
                        case GamePlatform.Mac:
                            return bnkPC2014Offsets;
                        case GamePlatform.XBox360:
                        case GamePlatform.PS3:
                            return bnkConsole2014Offsets;
                        default:
                            throw new InvalidOperationException("Unexpected game platform value");
                    }
                default:
                    throw new InvalidOperationException("Unexpected game version value");
            }
        }

        private static uint HashString(String str)
        {
            char[] bytes = str.ToLower().ToCharArray();
            uint hash = 2166136261;

            for (var i = 0; i < str.Length; i++) {
                hash *= 16777619;
                hash = hash ^ bytes[i];
            }

            return hash;
        }

        public static string GenerateSoundBank(string soundbankName, Stream audioStream, Stream outStream, decimal volume, Platform platform, bool usepreviewbnk = false)
        {
            string eventName = PLAY + soundbankName;
            string previewName = PLAY30SEC + soundbankName;
            string bankName = SONG + soundbankName;
            var id = RandomGenerator.NextInt();

            byte[] soundbank = null;

            switch (platform.version)
            {
                case GameVersion.RS2012:
                    switch (platform.platform)
                    {
                        case GamePlatform.Pc:
                            soundbank = Resources.PC_soundbank;
                            break;
                        case GamePlatform.XBox360:
                        case GamePlatform.PS3:
                            soundbank = Resources.Console_soundbank;
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected game platform value");
                    }
                    break;
                case GameVersion.RS2014:
                    switch (platform.platform)
                    {
                        case GamePlatform.Pc:
                        case GamePlatform.Mac:
                            soundbank = (usepreviewbnk) ? Resources.PC2014_soundbank_preview : Resources.PC2014_soundbank;
                            break;
                        case GamePlatform.XBox360:
                        case GamePlatform.PS3:
                            soundbank = (usepreviewbnk) ? Resources.Console2014_soundbank_preview : Resources.Console2014_soundbank;
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected game platform value");
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unexpected game version value");
            }

            var bitConverter = (platform.platform == GamePlatform.Pc || platform.platform == GamePlatform.Mac)
                    ? (EndianBitConverter)EndianBitConverter.Little
                    : (EndianBitConverter)EndianBitConverter.Big;

            using (var bankStream = new MemoryStream(soundbank))
            using (var bankReader = new EndianBinaryReader(bitConverter, bankStream))
            {
                var audioReader = new EndianBinaryReader(bitConverter, audioStream);
                var bankWriter = new EndianBinaryWriter(bitConverter, outStream);
                bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[0]));
                bankReader.ReadInt32();
                bankWriter.Write(id);
                bankWriter.Write(bankReader.ReadInt32());
                int dataSize = bankReader.ReadInt32();
                bankWriter.Write(dataSize);
                bankWriter.Write(bankReader.ReadInt32());
                bankWriter.Write(bankReader.ReadInt32());
                bankWriter.Write(audioReader.ReadBytes(dataSize));
                bankReader.BaseStream.Seek(dataSize, SeekOrigin.Current);
                bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[1]));
                bankWriter.Write(id);
                bankWriter.Write(id);
                bankReader.BaseStream.Seek(8, SeekOrigin.Current);
                bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[2]));
                bankWriter.Write((float)volume);
                bankReader.ReadInt32();
                bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[3]));
                bankReader.ReadInt32();
                bankWriter.Write(HashString(eventName));
                if (platform.version == GameVersion.RS2012)
                {
                    bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[4]));
                    bankReader.ReadInt32();
                    bankWriter.Write(HashString(previewName));
                }
                bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[5]));
                bankWriter.Write(12 + bankName.Length + 1);
                bankReader.ReadInt32();
                bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[6]));
                bankWriter.Write((byte)bankName.Length);
                bankWriter.Write(Encoding.ASCII.GetBytes(bankName));
                bankWriter.Flush();
            }

            return id.ToString();
        }
    }
}

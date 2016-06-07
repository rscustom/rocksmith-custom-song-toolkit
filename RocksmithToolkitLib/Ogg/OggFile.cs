using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using CFSM.AudioTools;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System.Diagnostics;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.Ogg
{
    public static class OggFile//wwRIFF
    {
        // add support for new versions of Wwise here
        public enum WwiseVersion { None, Wwise2010, Wwise2013, Wwise2014, Wwise2015 };

        #region RS1

        public static Stream ConvertOgg(string inputFile)
        {
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            {
                return ConvertOgg(inputFileStream);
            }
        }

        public static Stream ConvertOgg(Stream inputStream)
        {
            if (inputStream.NeedsConversion())
            {
                var platform = inputStream.GetAudioPlatform();
                var bitConverter = platform.GetBitConverter;

                using (var outputFileStream = new MemoryStream())
                using (var writer = new EndianBinaryWriter(bitConverter, outputFileStream))
                using (var reader = new EndianBinaryReader(bitConverter, inputStream))
                {
                    writer.Write(reader.ReadBytes(4));
                    UInt32 fileSize = reader.ReadUInt32();
                    fileSize -= 8; // We're removing data, so update the size in the header
                    writer.Write(fileSize);
                    writer.Write(reader.ReadBytes(8));
                    writer.Write(66); reader.ReadUInt32(); // New fmt size is 66
                    writer.Write(reader.ReadBytes(16));
                    writer.Write((ushort)48); reader.ReadUInt16(); // New cbSize is 48
                    writer.Write(reader.ReadBytes(6));
                    reader.BaseStream.Seek(8, SeekOrigin.Current); // Skip ahead 8 bytes, we don't want the vorb chunk
                    writer.Write(reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));

                    return new MemoryStream(outputFileStream.GetBuffer(), 0, (int)outputFileStream.Length);
                }
            }
            else
                return inputStream;
        }

        #endregion

        public static void Revorb(string file, string outputFileName, string appPath, WwiseVersion wwiseVersion)
        {
            // testing using dreddfoxx CFSM.AudioTool library.  Thanks to DF.
            var CFSM_AUDIO_TOOLS = File.Exists(Path.Combine(appPath, "CFSM.AudioTools.dll"));

            // TODO: comment out when there is time to debug/test
            // not working for RS1-RS2 ogg conversions ;(
            CFSM_AUDIO_TOOLS = false;

            if (CFSM_AUDIO_TOOLS)
            {
                using (var readStream = File.OpenRead(file))
                using (var outStream = File.Create(outputFileName))
                {
                    WwiseToOgg ww2Ogg = new WwiseToOgg(readStream, outStream);
                    ww2Ogg.ConvertToOgg();
                }
            }
            else
            {
                var ww2oggPath = Path.Combine(appPath, "ww2ogg.exe");
                var revorbPath = Path.Combine(appPath, "revorb.exe");
                var codebooksPath = Path.Combine(appPath, "packed_codebooks.bin"); // Default
                var codebooks603Path = Path.Combine(appPath, "packed_codebooks_aoTuV_603.bin"); // RS2014

                // Verifying if third part apps is in root application directory
                if (!File.Exists(ww2oggPath))
                    throw new FileNotFoundException("ww2ogg executable not found!");

                if (!File.Exists(revorbPath))
                    throw new FileNotFoundException("revorb executable not found!");

                if (!File.Exists(codebooksPath))
                    throw new FileNotFoundException("packed_codebooks.bin not found!");

                if (!File.Exists(codebooks603Path))
                    throw new FileNotFoundException("packed_codebooks_aoTuV_603.bin not found!");

                // Processing with ww2ogg
                Process ww2oggProcess = new Process();
                ww2oggProcess.StartInfo.FileName = ww2oggPath;
                ww2oggProcess.StartInfo.WorkingDirectory = appPath;

                switch (wwiseVersion)
                {
                    case WwiseVersion.Wwise2010:
                        ww2oggProcess.StartInfo.Arguments = String.Format("\"{0}\" -o \"{1}\"", file, outputFileName);
                        break;
                    case WwiseVersion.Wwise2013:
                        ww2oggProcess.StartInfo.Arguments = String.Format("\"{0}\" -o \"{1}\" --pcb \"{2}\"", file, outputFileName, codebooks603Path);
                        break;
                    default:
                        throw new InvalidOperationException("Wwise version not supported or invalid input file.");
                }

                ww2oggProcess.StartInfo.UseShellExecute = false;
                ww2oggProcess.StartInfo.CreateNoWindow = true;
                ww2oggProcess.StartInfo.RedirectStandardOutput = true;

                ww2oggProcess.Start();
                ww2oggProcess.WaitForExit();
                string ww2oggResult = ww2oggProcess.StandardOutput.ReadToEnd();

                if (ww2oggResult.IndexOf("error") > -1)
                    throw new Exception("ww2ogg process error." + Environment.NewLine + ww2oggResult);

                // Processing with revorb
                Process revorbProcess = new Process();
                revorbProcess.StartInfo.FileName = revorbPath;
                revorbProcess.StartInfo.WorkingDirectory = appPath;
                revorbProcess.StartInfo.Arguments = String.Format("\"{0}\"", outputFileName);
                revorbProcess.StartInfo.UseShellExecute = false;
                revorbProcess.StartInfo.CreateNoWindow = true;
                revorbProcess.StartInfo.RedirectStandardOutput = true;

                revorbProcess.Start();
                revorbProcess.WaitForExit();
                string revorbResult = revorbProcess.StandardOutput.ReadToEnd();

                if (ww2oggResult.IndexOf("error") > -1)
                {
                    if (File.Exists(outputFileName))
                        File.Delete(outputFileName);

                    throw new Exception("revorb process error." + Environment.NewLine + revorbResult);
                }
            }
        }

        #region RS2014

        public static void ConvertAudioPlatform(string inputFile, string outputFileName)
        {
            using (var outputFileStream = File.Open(outputFileName, FileMode.Append))
            {
                ConvertAudioPlatform(inputFile).CopyTo(outputFileStream);
            }
        }

        public static Stream ConvertAudioPlatform(string inputFile)
        {
            inputFile.VerifyHeaders();
            var platform = inputFile.GetAudioPlatform();

            EndianBitConverter bitConverter;
            EndianBitConverter targetbitConverter;
            if (!platform.IsConsole)
            {
                bitConverter = EndianBitConverter.Little;
                targetbitConverter = EndianBitConverter.Big;
            }
            else if (platform.IsConsole)
            {
                bitConverter = EndianBitConverter.Big;
                targetbitConverter = EndianBitConverter.Little;
            }
            else
                throw new InvalidDataException("The input file doesn't appear to be a valid Wwise file.");

            using (var outputFileStream = new MemoryStream())
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var writer = new EndianBinaryWriter(targetbitConverter, outputFileStream))
            using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
            {
                // Process Header
                UInt32 header = reader.ReadUInt32();
                if (header == 1179011410)//RIFF header to RIFX
                    //raw
                    writer.Write(1380533848);
                else
                    //raw
                    writer.Write(1179011410); // 1179011410
                writer.Write(reader.ReadUInt32()); // Size of File
                //raw
                writer.Write(reader.ReadBytes(4)); // WAVE (RIFF type)

                //Process Format
                writer.Write(reader.ReadBytes(4)); // fmt magicID                    //raw
                writer.Write(reader.ReadUInt32()); // fmt size
                writer.Write(reader.ReadUInt16()); // fmt tag (-1)
                writer.Write(reader.ReadUInt16()); // channels
                writer.Write(reader.ReadUInt32()); // samplesPerSec
                writer.Write(reader.ReadUInt32()); // avgBytesPerSec                 //SeekTableGranulary?
                writer.Write(reader.ReadUInt16()); // blockAlign
                writer.Write(reader.ReadUInt16()); // bitsPerSample
                writer.Write(reader.ReadUInt16()); //short cbSize 0-22               // WAVEFORMATEXTENSIBLE
                writer.Write(reader.ReadUInt16()); //short wSamplesPerBlock;         // WAVEFORMATEXTENSIBLE
                writer.Write(reader.ReadUInt32()); //long  dwChannelMask;            // WAVEFORMATEXTENSIBLE
                writer.Write(reader.ReadUInt32()); //long  dwTotalPCMFrames;         // Wwise
                UInt32 start = reader.ReadUInt32();
                writer.Write(start);              //long  dwLoopStartPacketOffset;   // Wwise
                UInt32 end = reader.ReadUInt32();
                writer.Write(end);                //long  dwLoopEndPacketOffset;     // Wwise
                writer.Write(reader.ReadUInt16());//short uLoopBeginExtra;           // Wwise
                writer.Write(reader.ReadUInt16());//short uLoopEndExtra;             // Wwise
                UInt32 seektablesize = reader.ReadUInt32();
                writer.Write(seektablesize);      //long dwSeekTableSize;            // Wwise
                writer.Write(reader.ReadUInt32());//long  dwVorbisDataOffset;        // Wwise
                writer.Write(reader.ReadUInt16());//short uMaxPacketSize;            // Wwise
                writer.Write(reader.ReadUInt16());//short uLastGranuleExtra;         // Wwise
                writer.Write(reader.ReadUInt32());//long  dwDecodeAllocSize;         // Wwise
                writer.Write(reader.ReadUInt32());//long  dwDecodeX64AllocSize;      // Wwise
                //raw
                writer.Write(reader.ReadBytes(4));//long  uHashCodebook;             // Wwise vorbis_analysis_headerout
                writer.Write(reader.ReadByte());  //char  uBlockSizes[2];            // Wwise
                writer.Write(reader.ReadByte());  //char  uBlockSizes[2];            // Wwise

                // Process DATA section - contains size, seektable, codebook, stream (biggest part)
                //raw data
                writer.Write(reader.ReadBytes(4)); // the word data
                writer.Write(reader.ReadUInt32()); //data size

                //seektable
                var y = seektablesize / 4;
                for (int i = 0; i < y; i++)
                {
                    writer.Write(reader.ReadUInt16()); //seekgranularity
                    writer.Write(reader.ReadUInt16()); //unk. actual granularity used??
                }

                //codebook
                UInt16 codebooksize = reader.ReadUInt16();
                writer.Write(codebooksize); //codebook size
                for (int i = 0; i < codebooksize; i++)
                {
                    //raw data
                    writer.Write(reader.ReadByte());
                }

                //stream
                var streamsize = (end - start); //calculate the total stream size till End of File
                for (int i = 0; i < streamsize; i++)
                {
                    UInt16 packetsize = reader.ReadUInt16(); // size of packet
                    i++; // increase because two bytes read for size of packet
                    writer.Write(packetsize);
                    for (int z = 0; z < packetsize; z++)
                    {
                        Byte packet = reader.ReadByte();
                        writer.Write(packet); // the packets are the same in both pc/console
                        i++; // add the  bytes read to packetsize counter.
                    }
                }
                return new MemoryStream(outputFileStream.GetBuffer(), 0, (int)outputFileStream.Length);
            }
        }

        /// <summary>
        /// Convert ogg or wave audio files to Wwise 2013 wem audio, including preview wem file.
        /// </summary>
        /// <param name="audioPath"></param>
        /// <param name="audioQuality"></param>
        /// <param name="previewLength"></param>
        /// <param name="chorusTime"></param>
        /// <returns>wemPath</returns>
        public static string Convert2Wem(string audioPath, int audioQuality = 4, long previewLength = 30000, long chorusTime = 4000)
        {
            // ExternalApps.VerifyExternalApps(); // for testing
            var audioPathNoExt = Path.Combine(Path.GetDirectoryName(audioPath), Path.GetFileNameWithoutExtension(audioPath));
            var oggPath = String.Format(audioPathNoExt + ".ogg");
            var wavPath = String.Format(audioPathNoExt + ".wav");
            var wemPath = String.Format(audioPathNoExt + ".wem");
            var oggPreviewPath = String.Format(audioPathNoExt + "_preview.ogg");
            var wavPreviewPath = String.Format(audioPathNoExt + "_preview.wav");
            var wemPreviewPath = String.Format(audioPathNoExt + "_preview.wem");

            if (audioPath.Substring(audioPath.Length - 4).ToLower() == ".ogg") //in RS1 ogg was actually wwise
            {
                ExternalApps.Ogg2Wav(audioPath, wavPath); //detect quality here
                if (!File.Exists(oggPreviewPath))
                {
                    ExternalApps.Ogg2Preview(audioPath, oggPreviewPath, previewLength, chorusTime);
                    ExternalApps.Ogg2Wav(oggPreviewPath, wavPreviewPath);
                }
                audioPath = wavPath;
            }

            if (audioPath.Substring(audioPath.Length - 4).ToLower() == ".wav")
            {
                if (!File.Exists(wavPreviewPath))
                {
                    if (!File.Exists(oggPath))
                    { 
                        //may cause issues if you've got another guitar.ogg in folder, but it's extremely rare.
                        ExternalApps.Wav2Ogg(audioPath, oggPath, audioQuality); // 4
                    }
                    ExternalApps.Ogg2Preview(oggPath, oggPreviewPath, previewLength, chorusTime);
                    ExternalApps.Ogg2Wav(oggPreviewPath, wavPreviewPath);
                }
                Wwise.Convert2Wem(audioPath, wemPath, audioQuality);
                audioPath = wemPath;
            }

            if (audioPath.Substring(audioPath.Length - 4).ToLower() == ".wem" && !File.Exists(wemPreviewPath))
            {
                Revorb(audioPath, oggPath, Path.GetDirectoryName(Application.ExecutablePath), WwiseVersion.Wwise2013);
                ExternalApps.Ogg2Wav(oggPath, wavPath);
                ExternalApps.Ogg2Preview(oggPath, oggPreviewPath, previewLength, chorusTime);
                ExternalApps.Ogg2Wav(oggPreviewPath, wavPreviewPath);
                Wwise.Convert2Wem(wavPath, wemPath, audioQuality);
                audioPath = wemPath;
            }

            return audioPath;
        }

        #endregion

        #region HELPERS

        public static void VerifyHeaders(this string inputFile)
        {
            var platform = inputFile.GetAudioPlatform();
            var bitConverter = platform.GetBitConverter;

            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            using (var reader = new EndianBinaryReader(bitConverter, inputFileStream))
            {
                reader.Seek(4, SeekOrigin.Begin);
                if (reader.ReadUInt32() != reader.BaseStream.Length - 8)
                    throw new InvalidDataException("The input OGG file appears to be truncated.");

                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "WAVE")
                    throw new InvalidDataException("Error reading input file - expected WAVE");

                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "fmt ")
                    throw new InvalidDataException("Error reading input file - expected fmt");

                var fmtLength = reader.ReadUInt32();
                if (fmtLength != 24 && fmtLength != 66)
                    throw new InvalidDataException("Error reading input file - expected fmt length of 24 or 66");

                if (fmtLength == 24)
                {
                    if (reader.ReadUInt16() != 0xFFFF)
                        throw new InvalidDataException("Error reading input file - expected Format Tag of 0xFFFF");

                    reader.BaseStream.Seek(14, SeekOrigin.Current);

                    if (reader.ReadUInt16() != 6)
                        throw new InvalidDataException("Error reading input file - expected cbSize of 6");

                    reader.BaseStream.Seek(6, SeekOrigin.Current);

                    if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "vorb")
                        throw new InvalidDataException("Error reading input file - expected vorb");

                    if (reader.ReadUInt32() != 42)
                        throw new InvalidDataException("Error reading input file - expected vorb length of 42");
                }
            }
        }

        public static Platform GetAudioPlatform(this string inputFile)
        {
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            {
                return inputFileStream.GetAudioPlatform();
            };
        }

        public static Platform GetAudioPlatform(this Stream input)
        {
            using (var MS = new MemoryStream())
            using (var reader = new BinaryReader(MS))
            {
                input.Position = 0; input.CopyTo(MS);
                MS.Position = 0; input.Position = 0;
                var fileID = new string(reader.ReadChars(4));
                if (fileID == "RIFF")//LE
                    return new Platform(GamePlatform.Pc, GameVersion.None);
                if (fileID == "RIFX")//BE
                    return new Platform(GamePlatform.XBox360, GameVersion.None);
            }
            return new Platform(GamePlatform.None, GameVersion.None);
        }

        public static bool NeedsConversion(this string inputFile)
        {
            using (var inputFileStream = File.Open(inputFile, FileMode.Open))
            {
                return inputFileStream.NeedsConversion();
            }
        }

        public static bool NeedsConversion(this Stream input)
        {
            var platform = input.GetAudioPlatform();
            var bitConverter = platform.GetBitConverter;

            using (var MS = new MemoryStream())
            using (var reader = new EndianBinaryReader(bitConverter, MS))
            {
                input.Position = 0; input.CopyTo(MS);
                MS.Position = 0; input.Position = 0;
                reader.Seek(16, SeekOrigin.Begin);
                if (reader.ReadUInt32() == 24)//fmtSize
                    return true;
            }

            return false;
        }

        public static WwiseVersion GetWwiseVersion(this string extension)
        {
            switch (extension)
            {
                case ".ogg":
                    return WwiseVersion.Wwise2010;
                case ".wem":
                    return WwiseVersion.Wwise2013;
                default:
                    throw new InvalidOperationException("Audio file not supported.");
            }
        }

        #endregion
    }
}

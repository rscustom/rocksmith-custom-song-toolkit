using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using RocksmithToolkitLib.Properties;
using RocksmithToolkitLib.Sng;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System.Diagnostics;

namespace RocksmithToolkitLib.DLCPackage
{
    /// <summary>
    /// RS1 SoundBank(.BNK) generator class
    /// </summary>
    public static class SoundBankGenerator
    {
        private const string PLAY = "Play_";
        private const string PLAY30SEC = "Play_30Sec_";
        private const string SONG = "Song_";
        private static readonly int[] bnkPCOffsets = { 0x2c, 0x1d, 0x17, 0xfa, 0xc8, 0x14, 0xc };
        private static readonly int[] bnkConsoleOffsets = { 0x7ec, 0x1d, 0x17, 0xfa, 0xc8, 0x14, 0xc };

        public static IList<int> GetOffsets(this Platform platform)
        {
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
                default:
                    throw new InvalidOperationException("Unexpected game version value");
            }
        }

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

        public static string GenerateSoundBank(string soundbankName, Stream audioStream, Stream outStream, float volume, Platform platform)
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
                default:
                    throw new InvalidOperationException("Unexpected game version value");
            }

            var bitConverter = platform.GetBitConverter;

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
                bankWriter.Write(bankReader.ReadBytes(platform.GetOffsets()[4]));
                bankReader.ReadInt32();
                bankWriter.Write(HashString(previewName));
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

    /// <summary>
    /// RS2014 SoundBank(.BNK) generator class
    /// </summary>
    public static class SoundBankGenerator2014
    {
        private const string PLAY = "Play_";
        private const string SONG = "Song_";
        private static EndianBitConverter _bitConverter;

        private static uint HashString(String str) //FNV hash
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

        private static void WriteChunk(EndianBinaryWriter w, string name, byte[] chunkData)
        {
            w.Write(Encoding.ASCII.GetBytes(name));
            w.Write(chunkData.Length);
            w.Write(chunkData);
        }

        private static byte[] Header(int id, int didxSize, bool isConsole)
        {
            int soundbankVersion = 91;
            int soundbankID = id;
            int languageID = 0;
            int hasFeedback = 0;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write(soundbankVersion);
                chunk.Write(soundbankID);
                chunk.Write(languageID);
                chunk.Write(hasFeedback);

                int alignSize = isConsole ? 2048 : 16;
                int dataSize = (int)chunkStream.Length;
                int junkSize = 24 + didxSize;
                int paddingSize = (dataSize + junkSize) % alignSize;
                if (paddingSize != 0)
                {
                    for (int i = 0; i < (alignSize - paddingSize) / 4; i++)
                        chunk.Write((int)0);
                }

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        private static byte[] DataIndex(int id, int len)
        {
            int fileID = id;
            int fileOffset = 0;
            int fileSize = len;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write(fileID);
                chunk.Write(fileOffset);
                chunk.Write(fileSize);

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        private const byte HIERARCHY_SOUND = 2;

        private static byte[] HierarchySound(int id, int fileid, int mixerid, float volume, bool preview, bool isConsole)
        {
            int soundID = id;
            int pluginID = 262145;
            int streamType = 2; //enum<int>
            int fileID = fileid;
            int sourceID = fileid;
            byte languageSpecific = 0; //soundType = {SFX, Voice}
            byte overrideParent = 0;
            byte numFX = 0;
            int parentBusID = RandomGenerator.NextInt();
            int directParentID = isConsole ? 134217984 : 65536; // TODO: changes on console
            uint unkID1 = (preview) ? 4178100890 : 0;
            int mixerID = mixerid;
            byte priorityOverrideParent = 0;
            byte priorityApplyDist = 0;
            byte overrideMidi = 0;
            byte numParam = 3;
            byte param1Type = 0; // Volume
            byte param2Type = 46; // MidiNoteOnAction
            byte param3Type = 47; // MidiNoteOffAction
            float param1Value = volume;
            int param2Value = 1;
            int param3Value = 3;
            byte numRange = 0;
            byte positionOverride = 0;
            byte overrideGameAux = 0;
            byte useGameAux = 0;
            byte overrideUserAux = 0;
            byte hasAux = 0;
            byte virtualQueueBehavior = (byte)((preview) ? 1 : 0);
            byte killNewest = (byte)((preview) ? 1 : 0);
            byte useVirtualBehavior = 0;
            short maxNumInstance = (byte)((preview) ? 1 : 0);
            byte isGlobalLimit = 0;
            byte belowThresholdBehavior = 0;
            byte isMaxNumInstOverrideParent = (byte)((preview) ? 1 : 0);
            byte isVVoiceOptOverrideParent = 0;
            int stateGroupList = 0;
            short rtpcList = 0;
            int feedbackBus = 0;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write((uint)soundID);
                chunk.Write((uint)pluginID);
                chunk.Write((uint)streamType);
                chunk.Write((uint)fileID);
                chunk.Write((uint)sourceID);
                chunk.Write(languageSpecific);
                chunk.Write(overrideParent);
                chunk.Write(numFX);
                chunk.Write((uint)parentBusID);
                chunk.Write((uint)directParentID);
                chunk.Write(unkID1);
                chunk.Write(mixerID);
                chunk.Write(priorityOverrideParent);
                chunk.Write(priorityApplyDist);
                chunk.Write(overrideMidi);
                chunk.Write(numParam);
                chunk.Write(param1Type);
                chunk.Write(param2Type);
                chunk.Write(param3Type);
                chunk.Write(param1Value);
                chunk.Write(param2Value);
                chunk.Write(param3Value);
                chunk.Write(numRange);
                chunk.Write(positionOverride);
                chunk.Write(overrideGameAux);
                chunk.Write(useGameAux);
                chunk.Write(overrideUserAux);
                chunk.Write(hasAux);
                chunk.Write(virtualQueueBehavior);
                chunk.Write(killNewest);
                chunk.Write(useVirtualBehavior);
                chunk.Write(maxNumInstance);
                chunk.Write(isGlobalLimit);
                chunk.Write(belowThresholdBehavior);
                chunk.Write(isMaxNumInstOverrideParent);
                chunk.Write(isVVoiceOptOverrideParent);
                chunk.Write(stateGroupList);
                chunk.Write(rtpcList);
                chunk.Write(feedbackBus);

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        private const byte HIERARCHY_ACTION = 3;

        private static byte[] HierarchyAction(int id, int objid, int bankid)
        {
            int actionID = id;
            short actionType = 1027; //wrong
            int objectID = objid;
            byte isBus = 0;
            byte numParam = 0;
            byte numRange = 0;
            byte fadeCurve = 4;
            int soundbankID = bankid;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write(actionID);
                chunk.Write(actionType);
                chunk.Write(objectID);
                chunk.Write(isBus);
                chunk.Write(numParam);
                chunk.Write(numRange);
                chunk.Write(fadeCurve);
                chunk.Write(soundbankID);

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        private const byte HIERARCHY_EVENT = 4;

        private static byte[] HierarchyEvent(int id, string name)
        {
            uint eventID = HashString(PLAY + name);
            int numEvents = 1;
            int actionID = id;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write(eventID);
                chunk.Write(numEvents);
                chunk.Write(actionID);

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        private const byte HIERARCHY_ACTORMIXER = 7;

        private static byte[] HierarchyActorMixer(int id, int soundid)
        {
            int mixerID = id;
            byte overrideParent = 0;
            byte numFX = 0;
            uint parentBusID = 2616261673;
            int directParentID = 0;
            int unkID1 = 0;
            int unkID2 = 65792;
            byte priorityOverrideParent = 0;
            byte priorityApplyDist = 0;
            byte numParam = 0;
            byte numRange = 0;
            byte positionOverride = 0;
            byte overrideGameAux = 0;
            byte useGameAux = 0;
            byte overrideUserAux = 0;
            byte hasAux = 0;
            byte virtualQueueBehavior = 0;
            byte killNewest = 0;
            byte useVirtualBehavior = 0;
            short maxNumInstance = 0;
            byte isGlobalLimit = 0;
            byte belowThresholdBehavior = 0;
            byte isMaxNumInstOverrideParent = 0;
            byte isVVoiceOptOverrideParent = 0;
            int stateGroupList = 0;
            short rtpcList = 0;
            int numChild = 1;
            int child1 = soundid;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write(mixerID);
                chunk.Write(overrideParent);
                chunk.Write(numFX);
                chunk.Write(parentBusID);
                chunk.Write(directParentID);
                chunk.Write(unkID1);
                chunk.Write(unkID2);
                chunk.Write(priorityOverrideParent);
                chunk.Write(priorityApplyDist);
                chunk.Write(numParam);
                chunk.Write(numRange);
                chunk.Write(positionOverride);
                chunk.Write(overrideGameAux);
                chunk.Write(useGameAux);
                chunk.Write(overrideUserAux);
                chunk.Write(hasAux);
                chunk.Write(virtualQueueBehavior);
                chunk.Write(killNewest);
                chunk.Write(useVirtualBehavior);
                chunk.Write(maxNumInstance);
                chunk.Write(isGlobalLimit);
                chunk.Write(belowThresholdBehavior);
                chunk.Write(isMaxNumInstOverrideParent);
                chunk.Write(isVVoiceOptOverrideParent);
                chunk.Write(stateGroupList);
                chunk.Write(rtpcList);
                chunk.Write(numChild);
                chunk.Write(child1);

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        private static byte[] Hierarchy(int bankid, int soundid, int fileid, string name, float volume, bool preview, bool isConsole)
        {
            int mixerID = 650605636;
            int actionID = RandomGenerator.NextInt();

            int numObjects = 0;
            byte[] sound = HierarchySound(soundid, fileid, mixerID, volume, preview, isConsole);
            numObjects++;
            byte[] actormixer = HierarchyActorMixer(mixerID, soundid);
            numObjects++;
            byte[] action = HierarchyAction(actionID, soundid, bankid);
            numObjects++;
            byte[] hevent = HierarchyEvent(actionID, name);
            numObjects++;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write(numObjects);

                chunk.Write(HIERARCHY_SOUND);
                chunk.Write(sound.Length);
                chunk.Write(sound);
                chunk.Write(HIERARCHY_ACTORMIXER);
                chunk.Write(actormixer.Length);
                chunk.Write(actormixer);
                chunk.Write(HIERARCHY_ACTION);
                chunk.Write(action.Length);
                chunk.Write(action);
                chunk.Write(HIERARCHY_EVENT);
                chunk.Write(hevent.Length);
                chunk.Write(hevent);

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        private static byte[] StringID(int id, string name)
        {
            int stringType = 1;
            int numNames = 1;
            int soundbankID = id;
            string soundbankName = SONG + name;

            using (var chunkStream = new MemoryStream())
            using (var chunk = new EndianBinaryWriter(_bitConverter, chunkStream))
            {
                chunk.Write(stringType);
                chunk.Write(numNames);
                chunk.Write(soundbankID);
                chunk.Write((byte)soundbankName.Length);
                chunk.Write(Encoding.ASCII.GetBytes(soundbankName));

                chunkStream.Flush();
                return chunkStream.ToArray();
            }
        }

        public static string GenerateSoundBank(string soundbankName, Stream audioStream, Stream outStream, float volume, Platform platform, bool preview = false, bool sameID = false)
        {
            _bitConverter = platform.GetBitConverter;
            int soundbankID = RandomGenerator.NextInt();
            int fileID = sameID ? oldFileID : RandomGenerator.NextInt();
            int soundID = sameID ? oldSoundID : RandomGenerator.NextInt();
            oldSoundID = soundID;
            oldFileID = fileID;

            var audioReader = new EndianBinaryReader(_bitConverter, audioStream);
            byte[] dataChunk = audioReader.ReadBytes(51200); // wwise is based on audio length, we'll just make it up(prefetch lookup is 100ms)
            byte[] dataIndexChunk = DataIndex(fileID, dataChunk.Length);
            byte[] headerChunk = Header(soundbankID, dataIndexChunk.Length, platform.IsConsole);
            byte[] stringIdChunk = StringID(soundbankID, soundbankName);
            byte[] hierarchyChunk = Hierarchy(soundbankID, soundID, fileID, soundbankName, volume, preview, platform.IsConsole);

            var bankWriter = new EndianBinaryWriter(_bitConverter, outStream);
            WriteChunk(bankWriter, "BKHD", headerChunk);
            WriteChunk(bankWriter, "DIDX", dataIndexChunk);
            WriteChunk(bankWriter, "DATA", dataChunk);
            WriteChunk(bankWriter, "HIRC", hierarchyChunk);
            WriteChunk(bankWriter, "STID", stringIdChunk);
            //Flush
            bankWriter.Flush();
            audioStream.Flush();
            outStream.Flush();
            audioStream.Position = 0;
            outStream.Position = 0;

            return fileID.ToString();
        }

        public static int oldSoundID { get; set; }
        public static int oldFileID { get; set; }

        /// <summary>
        /// Given a .bnk file, validate the bnk file and version number
        /// returns error message if not valid
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public static string ValidateBnkFile(string srcPath, Platform platform)
        {
            using (var reader = new EndianBinaryReader(platform.GetBitConverter, File.OpenRead(srcPath)))
            {
                reader.Seek(0, SeekOrigin.Begin);
                var magicBkhd = reader.ReadBytes(4); // BKHD 42 4B 48 44 

                if (Encoding.ASCII.GetString(magicBkhd) != "BKHD")
                    return "<ERROR> Did not find BKHD Header ..." + Environment.NewLine + srcPath;

                var lenBKHD = reader.ReadUInt32(); // Length of BKHD Section (bytes)
                var versionNumber = reader.ReadUInt32(); // BNK Version Number 91                

                if (versionNumber != 91)
                    return "<ERROR> Incorrect BNK Version Number - " + srcPath;

                int offset = (int)lenBKHD + 8; // 36
                reader.Seek(offset, SeekOrigin.Begin); // DIDX Section
                var magicDIDX = reader.ReadBytes(4); // DIDX 44 49 44 58

                if (Encoding.ASCII.GetString(magicDIDX) != "DIDX")
                    return "<ERROR> Did not find DIDX Header ..." + Environment.NewLine + srcPath;
            }

            return String.Empty;
        }

        /// <summary>
        /// Given a .bnk file, get the cross reference to corresponding .wem file
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public static string ReadWemFileId(string srcPath, Platform platform)
        {
            using (var reader = new EndianBinaryReader(platform.GetBitConverter, File.OpenRead(srcPath)))
            {
                reader.ReadBytes(4); // BKHD 42 4B 48 44 
                var lenBKHD = reader.ReadUInt32(); // Length of BKHD Section (bytes)
                int offset = (int)lenBKHD + 8; // 36
                // DIDX Section has already been validated
                reader.Seek(offset, SeekOrigin.Begin); // DIDX Section
                reader.ReadBytes(4); // DIDX 44 49 44 58

                var lenDIDX = reader.ReadUInt32(); // Length of DIDX Section (bytes)
                if (lenDIDX / 12 > 1)
                    throw new Exception("<ERROR> SoundBank Contains More than One WEM File ..." + Environment.NewLine + srcPath);

                // read wem file id
                var wemFileId = reader.ReadUInt32().ToString();
                return wemFileId;
            }
        }

        /// <summary>
        /// Given a .bnk file, read the audio volume factor
        /// </summary>
        /// <param name="srcPath">Wwise bnk file</param>
        /// <param name="platform">Game platform to detect endianness</param>
        /// <returns></returns>
        public static float? ReadVolumeFactor(string srcPath, Platform platform)
        {
            using (var reader = new EndianBinaryReader(platform.GetBitConverter, File.OpenRead(srcPath)))
            {
                // offset to find HIRC
                while (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "HIRC" && reader.BaseStream.Position < reader.BaseStream.Length - 1)
                    reader.ReadBytes((int)reader.ReadUInt32());

                // ok we should be at HIRC now, let's confirm it!
                reader.BaseStream.Position -= 4;
                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "HIRC")
                    throw new Exception("<ERROR> Did not find HIRC Header ..." + Environment.NewLine + srcPath);

                // get HIRC data
                var lenHIRC = reader.ReadUInt32();
                var objCount = reader.ReadUInt32();
                for (var n = 0; n < objCount; n++)
                {
                    var typeId = reader.ReadByte();
                    var lenSection = reader.ReadUInt32();

                    // find correct object type - SFXV
                    if (typeId == 2)
                    {
                        // skip 42 bytes to get params count                        
                        reader.ReadBytes(42);

                        // new bnk has 4, old should have 3
                        var pnum = reader.ReadByte();

                        //we could do a toaster here to see where's volume value is
                        //but I'm solidly hopping they won't change it's order...
                        //skip paramType section
                        reader.ReadBytes(pnum);

                        // fetch volume factor (Param0 value)
                        var volFactor = reader.ReadSingle();
                        return volFactor;
                    }

                    reader.ReadBytes((int)lenSection);
                }
            }

            return null;
        }

    }

    public class BnkWemData
    {
        public string BnkFileName { get; set; }
        public string WemFileId { get; set; }
        public float? VolumeFactor { get; set; }
    }

}

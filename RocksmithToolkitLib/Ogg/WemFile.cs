using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;

// some code taken from https://github.com/Crauzer/WEMSharp
// TODO: use full code for native C# wem to ogg converter

namespace RocksmithToolkitLib.Ogg
{
    public class WemFile
    {
        private Stream _wemFile;

        private uint _fmtChunkOffset = 0xFFFFFFFF;
        private uint _fmtChunkSize = 0xFFFFFFFF;
        private uint _cueChunkOffset = 0xFFFFFFFF;
        private uint _cueChunkSize = 0xFFFFFFFF;
        private uint _listChunkOffset = 0xFFFFFFFF;
        private uint _listChunkSize = 0xFFFFFFFF;
        private uint _smplChunkOffset = 0xFFFFFFFF;
        private uint _smplChunkSize = 0xFFFFFFFF;
        private uint _vorbChunkOffset = 0xFFFFFFFF;
        private uint _vorbChunkSize = 0xFFFFFFFF;
        private uint _dataChunkOffset = 0xFFFFFFFF;
        private uint _dataChunkSize = 0xFFFFFFFF;

        /// <summary>
        /// Channel Count
        /// </summary>
        public ushort Channels { get; private set; }

        /// <summary>
        /// Sample Rate
        /// </summary>
        public uint SampleRate { get; private set; }
        public uint AverageBytesPerSecond { get; private set; }

        private uint _cueCount;
        private uint _loopCount;
        private uint _loopStart;
        private uint _loopEnd;
        private uint _sampleCount;
        private bool _noGranule;
        private bool _modPackets;
        private uint _setupPacketOffset;
        private uint _firstAudioPacketOffset;
        private bool _headerTriadPresent;
        private bool _oldPacketHeaders;
        private uint _uid;
        private byte _blocksize0Pow;
        private byte _blocksize1Pow;

        /// <summary>
        /// Given a .wem stream, get the audio stream info
        /// </summary>
        /// <param name="wemStream">Wwise wem stream</param>
        /// <param name="platform">Game platform to detect endianness</param>
        /// <returns></returns>
        public WemFile(Stream wemStream, Platform platform, WEMForcePacketFormat forcePacketFormat = WEMForcePacketFormat.NoForcePacketFormat)
        {
            this._wemFile = wemStream; // File.OpenRead(fileLocation);
            using (var br = new EndianBinaryReader(platform.GetBitConverter, this._wemFile))
            {
                br.Seek(0, SeekOrigin.Begin);
                string magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                if (magic != "RIFF")
                    throw new Exception("This is either not a WEM file or is of an unsupported type");

                uint riffSize = br.ReadUInt32() + 8;
                string waveMagic = Encoding.ASCII.GetString(br.ReadBytes(4));
                if (waveMagic != "WAVE")
                    throw new Exception("Missing WAVE magic");

                uint chunkOffset = 12;
                while (chunkOffset < riffSize)
                {
                    br.BaseStream.Seek(chunkOffset, SeekOrigin.Begin);

                    string chunkName = Encoding.ASCII.GetString(br.ReadBytes(4));
                    uint chunkSize = br.ReadUInt32();

                    if (chunkName.Substring(0, 3) == "fmt")
                    {
                        this._fmtChunkOffset = chunkOffset + 8;
                        this._fmtChunkSize = chunkSize;
                    }
                    else if (chunkName.Substring(0, 3) == "cue")
                    {
                        this._cueChunkOffset = chunkOffset + 8;
                        this._cueChunkSize = chunkSize;
                    }
                    else if (chunkName == "LIST")
                    {
                        this._listChunkOffset = chunkOffset + 8;
                        this._listChunkSize = chunkSize;
                    }
                    else if (chunkName == "smpl")
                    {
                        this._smplChunkOffset = chunkOffset + 8;
                        this._smplChunkSize = chunkSize;
                    }
                    else if (chunkName == "vorb")
                    {
                        this._vorbChunkOffset = chunkOffset + 8;
                        this._vorbChunkSize = chunkSize;
                    }
                    else if (chunkName == "data")
                    {
                        this._dataChunkOffset = chunkOffset + 8;
                        this._dataChunkSize = chunkSize;
                    }

                    chunkOffset += chunkSize + 8;
                }

                if (chunkOffset > riffSize)
                {
                    throw new Exception("There was an error reading the file");
                }

                if (this._fmtChunkOffset == 0xFFFFFFFF && this._dataChunkOffset == 0xFFFFFFFF)
                {
                    throw new Exception("There was an error reading the file");
                }

                //Read FMT Chunk
                if (this._vorbChunkOffset == 0xFFFFFFFF && this._fmtChunkSize != 0x42)
                {
                    throw new Exception("There was an error reading the file");
                }
                if (this._vorbChunkOffset != 0xFFFFFFFF && this._fmtChunkSize != 0x28 && this._fmtChunkSize != 0x18 && this._fmtChunkSize != 0x12)
                {
                    throw new Exception("There was an error reading the file");
                }
                if (this._vorbChunkOffset == 0xFFFFFFFF && this._fmtChunkSize == 0x42)
                {
                    this._vorbChunkOffset = this._fmtChunkOffset + 0x18;
                }

                br.BaseStream.Seek(this._fmtChunkOffset, SeekOrigin.Begin);

                ushort codecID = br.ReadUInt16();
                if (codecID != 0xFFFF)
                {
                    throw new Exception("FMT Chunk - Wrong Codec ID");
                }

                this.Channels = br.ReadUInt16();
                this.SampleRate = br.ReadUInt32();
                this.AverageBytesPerSecond = br.ReadUInt32();

                ushort blockAlign = br.ReadUInt16();
                if (blockAlign != 0)
                {
                    throw new Exception("FMT Chunk - Wrong Block Align");
                }

                ushort bitsPerSample = br.ReadUInt16();
                if (bitsPerSample != 0)
                {
                    throw new Exception("FMT Chunk - Wrong Bits Per Sample");
                }

                ushort extraFmtLength = br.ReadUInt16();
                if (extraFmtLength != (this._fmtChunkSize - 0x12))
                {
                    throw new Exception("FMT Chunk - Wrong Extra FMT Chunk Size");
                }

                if (this._fmtChunkSize == 0x28)
                {
                    byte[] unknownBufferCheck = new byte[] { 1, 0, 0, 0, 0, 0, 0x10, 0, 0x80, 0, 0, 0xAA, 0, 0x38, 0x9b, 0x71 };
                    byte[] unknownBuffer;

                    unknownBuffer = br.ReadBytes(16);

                    if (unknownBuffer.SequenceEqual(unknownBufferCheck))
                    {
                        throw new Exception("FMT Chunk - Wrong Unknown Buffer Signature");
                    }
                }

                //Read CUE Chunk
                if (this._cueChunkOffset != 0xFFFFFFFF)
                {
                    br.BaseStream.Seek(this._cueChunkOffset, SeekOrigin.Begin);

                    this._cueCount = br.ReadUInt32();
                }

                //Read SMPL Chunk
                if (this._smplChunkOffset != 0xFFFFFFFF)
                {
                    br.BaseStream.Seek(this._smplChunkOffset, SeekOrigin.Begin);

                    this._loopCount = br.ReadUInt32();
                    if (this._loopCount != 1)
                    {
                        throw new Exception("SMPL Chunk - Wrong Loop Count");
                    }

                    br.BaseStream.Seek(this._smplChunkOffset + 0x2C, SeekOrigin.Begin);

                    this._loopStart = br.ReadUInt32();
                    this._loopEnd = br.ReadUInt32();
                }

                //Read VORB Chunk
                switch (this._vorbChunkSize)
                {
                    case 0xFFFFFFFF:
                    case 0x28:
                    case 0x2A:
                    case 0x2C:
                    case 0x32:
                    case 0x34:
                        br.BaseStream.Seek(this._vorbChunkOffset, SeekOrigin.Begin);
                        break;
                    default:
                        throw new Exception("VORB Chunk - Wrong VORB Chunk Size");
                }

                this._sampleCount = br.ReadUInt32();

                switch (this._vorbChunkSize)
                {
                    case 0xFFFFFFFF:
                    case 0x2A:
                        {
                            this._noGranule = true;

                            br.BaseStream.Seek(this._vorbChunkOffset + 4, SeekOrigin.Begin);

                            uint modSignal = br.ReadUInt32();

                            if (modSignal != 0x4A && modSignal != 0x4B && modSignal != 0x69 && modSignal != 0x70)
                            {
                                this._modPackets = true;
                            }

                            br.BaseStream.Seek(this._vorbChunkOffset + 0x10, SeekOrigin.Begin);

                            break;
                        }

                    default:
                        br.BaseStream.Seek(this._vorbChunkOffset + 0x18, SeekOrigin.Begin);
                        break;
                }

                if (forcePacketFormat == WEMForcePacketFormat.ForceNoModPackets)
                {
                    this._modPackets = false;
                }
                else if (forcePacketFormat == WEMForcePacketFormat.ForceModPackets)
                {
                    this._modPackets = true;
                }

                this._setupPacketOffset = br.ReadUInt32();
                this._firstAudioPacketOffset = br.ReadUInt32();

                switch (this._vorbChunkSize)
                {
                    case 0xFFFFFFFF:
                    case 0x2A:
                        br.BaseStream.Seek(this._vorbChunkOffset + 0x24, SeekOrigin.Begin);
                        break;

                    case 0x32:
                    case 0x34:
                        br.BaseStream.Seek(this._vorbChunkOffset + 0x2C, SeekOrigin.Begin);
                        break;
                }

                switch (this._vorbChunkSize)
                {
                    case 0x28:
                    case 0x2C:
                        this._headerTriadPresent = true;
                        this._oldPacketHeaders = true;
                        break;

                    case 0xFFFFFFFF:
                    case 0x2A:
                    case 0x32:
                    case 0x34:
                        this._uid = br.ReadUInt32();
                        this._blocksize0Pow = br.ReadByte();
                        this._blocksize1Pow = br.ReadByte();
                        break;
                }

                if (this._loopCount != 0)
                {
                    if (this._loopEnd == 0)
                    {
                        this._loopEnd = this._sampleCount;
                    }
                    else
                    {
                        this._loopEnd++;
                    }

                    if (this._loopStart >= this._sampleCount || this._loopEnd > this._sampleCount || this._loopStart > this._loopEnd)
                    {
                        throw new Exception("Loops out of range");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Forcing of the OGG Packet Format
    /// </summary>
    public enum WEMForcePacketFormat
    {
        /// <summary>
        /// Uses the original Mod Packet Format from the WEM file
        /// </summary>
        NoForcePacketFormat,
        /// <summary>
        /// Forces to modify the original packets
        /// </summary>
        ForceModPackets,
        /// <summary>
        /// Forces to not modify the original packets
        /// </summary>
        ForceNoModPackets

    }
}

/* EXAMPLE USAGE:
// get main wem audio bitrate (kbps)
var wems = _archive.TOC.Where(entry => entry.Name.StartsWith("audio/") && entry.Name.EndsWith(".wem")).ToList();
if (wems.Count > 1)
{
    wems.Sort((e1, e2) =>
        {
            if (e1.Length < e2.Length)
                return 1;
            if (e1.Length > e2.Length)
                return -1;
            return 0;
        });
}

// assumes main audio is larger wem file (may not always be correct)
if (wems.Count > 0)
{
    var top = wems[0];
    _archive.InflateEntry(top);
    top.Data.Position = 0;
    var wemSize = top.Data.Length;

    // slow actual kbps
    WemFile wemFile = new WemFile(top.Data, platform);
    var kbpsActual = (int)wemFile.AverageBytesPerSecond * 8 / 1000;

    // fast approximate kbps (very close)
    var kbpsApprox = (int)(wemSize * 8 / song.SongLength / 1000);
    song.AudioBitRate = kbpsApprox;
}
*/




 
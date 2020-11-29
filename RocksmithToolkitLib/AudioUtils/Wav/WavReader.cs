using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 *    This file is a part of the Wav encoder/decoder utils.
 *    Copyright (C) 2020  Xuan525
 *
 *    This program is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.
 *
 *    This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *    GNU General Public License for more details.
 *
 *    You should have received a copy of the GNU General Public License
 *    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *    
 *    Email : shanboxuan@me.com
 *    Github : https://github.com/xuan525
*/

namespace RocksmithToolkitLib.AudioUtils.Wav
{
    public class WavReader
    {
        public class FormatNotSupportedException : Exception { }

        /// <summary>
        /// The base chunk of a wave(riff) file
        /// </summary>
        public RiffChunk Riff { get; set; }

        /// <summary>
        /// The class will pares all the data in advance while constructing
        /// </summary>
        /// <param name="stream">The input stream of the wave file</param>
        /// <param name="stringEncoding">Charset of the strings in the file</param>
        public WavReader(Stream stream, Encoding stringEncoding)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                IChunk chunk = NextChunk(binaryReader, stringEncoding);
                if (chunk.Descriptor.ChunkId != "RIFF")
                {
                    throw new FormatNotSupportedException();
                }

                Riff = (RiffChunk)chunk;
            }
        }

        public short GetFormatCode(Guid guid)
        {
            byte[] buffer = guid.ToByteArray();
            if (buffer[1] == 0x00 &&
                buffer[2] == 0x00 &&
                buffer[3] == 0x00 &&
                buffer[4] == 0x00 &&
                buffer[5] == 0x00 &&
                buffer[6] == 0x10 &&
                buffer[7] == 0x00 &&
                buffer[8] == 0x80 &&
                buffer[9] == 0x00 &&
                buffer[10] == 0x00 &&
                buffer[11] == 0xaa &&
                buffer[12] == 0x00 &&
                buffer[13] == 0x38 &&
                buffer[14] == 0x9b &&
                buffer[15] == 0x71)
            {
                if (buffer[0] == 0x01 ||
                    buffer[0] == 0x02 ||
                    buffer[0] == 0x03 ||
                    buffer[0] == 0x06 ||
                    buffer[0] == 0x07 ||
                    buffer[0] == 0x50)
                {
                    return buffer[0];
                }
            }
            throw new FormatNotSupportedException();
        }

        /// <summary>
        /// Get sample data in the wave file
        /// </summary>
        /// <returns>A double array[channel][sample] in range -1.0 to 1.0</returns>
        public double[][] GetSampleData()
        {
            FmtChunk fmtChunk = (FmtChunk)Riff.Chunks["fmt "];
            DataChunk dataChunk = (DataChunk)Riff.Chunks["data"];

            double[][] values = new double[fmtChunk.NumChennals][];
            int channelLength = dataChunk.Data.Length / fmtChunk.BlockAlign;
            for (int i = 0; i < fmtChunk.NumChennals; i++)
            {
                values[i] = new double[channelLength];
            }

            short audioFormat = fmtChunk.AudioFormat;
            if (audioFormat == -2)
            {
                if (fmtChunk.ExtraParams != null)
                {
                    audioFormat = GetFormatCode(fmtChunk.ExtraParams.SubFormatGuid);
                }
                else
                {
                    throw new FormatNotSupportedException();
                }
            }

            switch (audioFormat)
            {
                case 0x0001:
                    switch (fmtChunk.BitsPerSample)
                    {
                        case 16:
                            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(dataChunk.Data)))
                            {
                                for (int pos = 0; pos < channelLength; pos++)
                                {
                                    for (int chan = 0; chan < fmtChunk.NumChennals; chan++)
                                    {
                                        short value = binaryReader.ReadInt16();
                                        values[chan][pos] = (double)value / short.MaxValue;
                                    }
                                }
                            }
                            break;
                        case 24:
                            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(dataChunk.Data)))
                            {
                                for (int pos = 0; pos < channelLength; pos++)
                                {
                                    for (int chan = 0; chan < fmtChunk.NumChennals; chan++)
                                    {
                                        byte[] buffer = new byte[3];
                                        binaryReader.Read(buffer, 0, buffer.Length);
                                        int value = buffer[0] | buffer[1] << 8 | (sbyte)buffer[2] << 16;
                                        values[chan][pos] = (double)value / 8388607;
                                    }
                                }
                            }
                            break;
                        case 32:
                            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(dataChunk.Data)))
                            {
                                for (int pos = 0; pos < channelLength; pos++)
                                {
                                    for (int chan = 0; chan < fmtChunk.NumChennals; chan++)
                                    {
                                        int value = binaryReader.ReadInt32();
                                        values[chan][pos] = (double)value / int.MaxValue;
                                    }
                                }
                            }
                            break;
                        case 64:
                            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(dataChunk.Data)))
                            {
                                for (int pos = 0; pos < channelLength; pos++)
                                {
                                    for (int chan = 0; chan < fmtChunk.NumChennals; chan++)
                                    {
                                        long value = binaryReader.ReadInt64();
                                        values[chan][pos] = (double)value / long.MaxValue;
                                    }
                                }
                            }
                            break;
                        default:
                            throw new FormatNotSupportedException();
                    }
                    break;
                case 0x0003:
                    switch (fmtChunk.BitsPerSample)
                    {
                        case 32:
                            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(dataChunk.Data)))
                            {
                                for (int pos = 0; pos < channelLength; pos++)
                                {
                                    for (int chan = 0; chan < fmtChunk.NumChennals; chan++)
                                    {
                                        float value = binaryReader.ReadSingle();
                                        values[chan][pos] = value;
                                    }
                                }
                            }
                            break;
                        case 64:
                            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(dataChunk.Data)))
                            {
                                for (int pos = 0; pos < channelLength; pos++)
                                {
                                    for (int chan = 0; chan < fmtChunk.NumChennals; chan++)
                                    {
                                        double value = binaryReader.ReadDouble();
                                        values[chan][pos] = value;
                                    }
                                }
                            }
                            break;
                        default:
                            throw new FormatNotSupportedException();
                    }
                    break;
                default:
                    throw new FormatNotSupportedException();
            }
            return values;
        }

        private static IChunk NextChunk(BinaryReader binaryReader, Encoding stringEncoding)
        {
            ChunkDescriptor chunkDescriptor = new ChunkDescriptor(binaryReader);
            if (chunkDescriptor.ChunkId == null)
            {
                return null;
            }

            switch (chunkDescriptor.ChunkId)
            {
                case "RIFF":
                    return new RiffChunk(chunkDescriptor, binaryReader, stringEncoding);
                case "LIST":
                    return new ListChunk(chunkDescriptor, binaryReader, stringEncoding);
                case "fmt ":
                    return new FmtChunk(chunkDescriptor, binaryReader);
                case "data":
                    return new DataChunk(chunkDescriptor, binaryReader);
                default:
                    return new UnknowChunk(chunkDescriptor, binaryReader);
            }
        }

        public class ChunkDescriptor
        {
            public string ChunkId { get; set; }
            public int ChunkSize { get; set; }

            public ChunkDescriptor(BinaryReader binaryReader)
            {
                byte[] buffer = new byte[4];
                int readLength = binaryReader.Read(buffer, 0, buffer.Length);
                if (readLength == 4)
                {
                    ChunkId = Encoding.ASCII.GetString(buffer);
                    ChunkSize = binaryReader.ReadInt32();
                }
            }
        }

        public interface IChunk
        {
            ChunkDescriptor Descriptor { get; }
        }

        public class RiffChunk : IChunk
        {
            public ChunkDescriptor Descriptor { get; set; }
            public string Format { get; set; }
            public Dictionary<string, IChunk> Chunks { get; set; }

            public RiffChunk(ChunkDescriptor chunkDescriptor, BinaryReader binaryReader, Encoding stringEncoding)
            {
                Descriptor = chunkDescriptor;
                Format = Encoding.ASCII.GetString(binaryReader.ReadBytes(4));
                Chunks = new Dictionary<string, IChunk>();
                if (Descriptor.ChunkSize > 0)
                {
                    int position = 4;
                    while (position < Descriptor.ChunkSize)
                    {
                        IChunk chunk = NextChunk(binaryReader, stringEncoding);
                        if (chunk == null)
                        {
                            throw new Exception("Unexpected stream end");
                        }

                        Chunks.Add(chunk.Descriptor.ChunkId, chunk);
                        position += chunk.Descriptor.ChunkSize + 8;
                    }
                }
                else
                {
                    while (true)
                    {
                        IChunk chunk = NextChunk(binaryReader, stringEncoding);
                        if (chunk == null)
                        {
                            break;
                        }

                        Chunks.Add(chunk.Descriptor.ChunkId, chunk);
                    }
                }

            }
        }

        public class FmtChunk : IChunk
        {
            public ChunkDescriptor Descriptor { get; set; }
            public short AudioFormat { get; set; }
            public short NumChennals { get; set; }
            public int SampleRate { get; set; }
            public int ByteRate { get; set; }
            public short BlockAlign { get; set; }
            public short BitsPerSample { get; set; }
            public short ExtraParamSize { get; set; }
            public Extra ExtraParams { get; set; }

            public class Extra
            {
                public short ValidBitsPerSample { get; set; }
                public int ChannelMask { get; set; }
                public Guid SubFormatGuid { get; set; }

                public Extra(BinaryReader binaryReader)
                {
                    ValidBitsPerSample = binaryReader.ReadInt16();
                    ChannelMask = binaryReader.ReadInt32();
                    byte[] guidBuffer = new byte[16];
                    binaryReader.Read(guidBuffer, 0, guidBuffer.Length);
                    SubFormatGuid = new Guid(guidBuffer);
                }
            }

            public FmtChunk(ChunkDescriptor chunkDescriptor, BinaryReader binaryReader)
            {
                Descriptor = chunkDescriptor;

                // General data
                AudioFormat = binaryReader.ReadInt16();
                NumChennals = binaryReader.ReadInt16();
                SampleRate = binaryReader.ReadInt32();
                ByteRate = binaryReader.ReadInt32();
                BlockAlign = binaryReader.ReadInt16();
                BitsPerSample = binaryReader.ReadInt16();

                int position = 16;

                // ExtraParams
                if (Descriptor.ChunkSize > 16)
                {
                    ExtraParamSize = binaryReader.ReadInt16();
                    position += 2;
                    if(ExtraParamSize != 0)
                    {
                        if (ExtraParamSize == 22)
                        {
                            ExtraParams = new Extra(binaryReader);
                            position += ExtraParamSize;
                        }
                        else
                        {
                            throw new FormatNotSupportedException();
                        }
                    }
                    else
                    {
                        ExtraParamSize = 0;
                    }
                }
                else
                {
                    ExtraParamSize = 0;
                }

                // Consume unused data (if exits)
                if (position < Descriptor.ChunkSize)
                {
                    binaryReader.ReadBytes(Descriptor.ChunkSize - position);
                }
            }
        }

        public class ListChunk : IChunk
        {
            public ChunkDescriptor Descriptor { get; set; }
            public string ListType { get; set; }

            public SortedDictionary<string, string> Data { get; set; }

            public ListChunk(ChunkDescriptor chunkDescriptor, BinaryReader binaryReader, Encoding stringEncoding)
            {
                Descriptor = chunkDescriptor;

                ListType = Encoding.ASCII.GetString(binaryReader.ReadBytes(4));
                int position = 4;

                Data = new SortedDictionary<string, string>();
                while (position < Descriptor.ChunkSize)
                {
                    string subChunkId = Encoding.ASCII.GetString(binaryReader.ReadBytes(4));
                    position += 4;
                    int subChunkSize = binaryReader.ReadInt32();
                    position += 4;
                    string data = stringEncoding.GetString(binaryReader.ReadBytes(subChunkSize)).TrimEnd('\0');
                    position += subChunkSize;
                    if (subChunkSize % 2 != 0)
                    {
                        binaryReader.ReadByte();
                        position++;
                    }
                    Data.Add(subChunkId, data);
                }
            }
        }

        public class DataChunk : IChunk
        {
            public ChunkDescriptor Descriptor { get; set; }

            public byte[] Data;

            public DataChunk(ChunkDescriptor chunkDescriptor, BinaryReader binaryReader)
            {
                Descriptor = chunkDescriptor;
                if (Descriptor.ChunkSize > 0)
                {
                    Data = binaryReader.ReadBytes(Descriptor.ChunkSize);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            int length = binaryReader.Read(buffer, 0, buffer.Length);
                            if (length <= 0)
                            {
                                break;
                            }

                            memoryStream.Write(buffer, 0, length);
                        }
                        Data = memoryStream.ToArray();
                    }
                }
            }
        }

        public class UnknowChunk : IChunk
        {
            public ChunkDescriptor Descriptor { get; set; }
            public byte[] Data { get; set; }

            public UnknowChunk(ChunkDescriptor chunkDescriptor, BinaryReader binaryReader)
            {
                Descriptor = chunkDescriptor;
                Data = binaryReader.ReadBytes(Descriptor.ChunkSize);
            }
        }

    }

}

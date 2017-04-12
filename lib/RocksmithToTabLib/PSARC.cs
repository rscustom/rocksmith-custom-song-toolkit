/// Taken from the RocksmithToolkitLib
/// At the moment, I need my own version of this class, because the original
/// decompresses the whole psarc and opens a file handle for each and every
/// file inside the psarc. For larger archives (e.g. songs.psarc), this exceeds
/// the file handle limit on Mac. This modified class only decompresses individual
/// files on demand and thus circumvents the problem, but it only has read support
/// since I couldn't figure out a sensible way to do both without severely breaking
/// the interface.

using zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using RocksmithToolkitLib.PSARC;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToTabLib
{
	public class PSARC
	{
		private class Header
		{
			public uint MagicNumber;
			public uint VersionNumber;
			public uint CompressionMethod;
			public uint TotalTOCSize;
			public uint TOCEntrySize;
			public uint numFiles;
			public uint blockSize;
			public uint archiveFlags;
			public Header()
			{
				this.MagicNumber = 1347633490;
				this.VersionNumber = 65540;
				this.CompressionMethod = 2053925218;
				this.TOCEntrySize = 30;
				this.numFiles = 0;
				this.blockSize = 65536;
				this.archiveFlags = 0;
			}
		}

		private PSARC.Header header = new PSARC.Header();
		public List<Entry> Entries
		{
			get;
			private set;
		}

		public PSARC()
		{
			this.Entries = new List<Entry>();
			this.Entries.Add(new Entry
			{
				id = 0
			});
		}

		public void ReadNames()
		{
			this.Entries[0].Name = "NamesBlock.bin";
			Stream data = this.Entries[0].Data.OpenStream();
			BinaryReader binaryReader = new BinaryReader(data);
			StringBuilder stringBuilder = new StringBuilder(100);
			int index = 1;
			while (data.Position < data.Length)
			{
				char c = binaryReader.ReadChar();
				if (c == '\n')
				{
					this.Entries[index++].Name = stringBuilder.ToString();
					stringBuilder = new StringBuilder();
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			this.Entries[index].Name = stringBuilder.ToString();
			data.Seek(0, SeekOrigin.Begin);
            this.Entries.Remove(this.Entries[0]);
		}

        private void inflateEntries(BigEndianBinaryReader reader, uint[] zLengths, uint blockSize)
        {
            foreach (Entry current in this.Entries)
            {
                current.Data = new Entry.DataPointer(current, reader, zLengths, blockSize);
            }
        }

		public void Read(Stream str)
		{
			this.Entries.Clear();
			BigEndianBinaryReader bigEndianBinaryReader = new BigEndianBinaryReader(str);
			this.header.MagicNumber = bigEndianBinaryReader.ReadUInt32();
			this.header.VersionNumber = bigEndianBinaryReader.ReadUInt32();
			this.header.CompressionMethod = bigEndianBinaryReader.ReadUInt32();
			this.header.TotalTOCSize = bigEndianBinaryReader.ReadUInt32();
			this.header.TOCEntrySize = bigEndianBinaryReader.ReadUInt32();
			this.header.numFiles = bigEndianBinaryReader.ReadUInt32();
			this.header.blockSize = bigEndianBinaryReader.ReadUInt32();
			this.header.archiveFlags = bigEndianBinaryReader.ReadUInt32();

            var tocStream = str;
            BigEndianBinaryReader bigEndianBinaryReaderTOC = bigEndianBinaryReader;
            if (this.header.archiveFlags == 4)
            {
                var decStream = new TempFileStream();
                using (var outputStream = new MemoryStream())
                {
                    RijndaelEncryptor.DecryptPSARC(str, outputStream, this.header.TotalTOCSize);

                    int bytesRead;
                    byte[] buffer = new byte[30000];

                    int decMax = (int)this.header.TotalTOCSize - 32;
                    int decSize = 0;
                    outputStream.Seek(0, SeekOrigin.Begin);
                    while ((bytesRead = outputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        decSize += bytesRead;
                        if (decSize > decMax) bytesRead = decMax - (decSize - bytesRead);
                        decStream.Write(buffer, 0, bytesRead);
                    }
                }

                decStream.Seek(0, SeekOrigin.Begin);
                str.Seek(this.header.TotalTOCSize, SeekOrigin.Begin);
                tocStream = decStream;
                bigEndianBinaryReaderTOC = new BigEndianBinaryReader(tocStream);
            }

			if (this.header.MagicNumber == 1347633490)
			{
				if (this.header.CompressionMethod == 2053925218)
				{
					byte b = 1;
					uint num = 256;
					do
					{
						num *= 256;
						b += 1;
					}
					while (num < this.header.blockSize);
					int num2 = 0;
					while (num2 < this.header.numFiles)
					{
						this.Entries.Add(new Entry
						{
							id = num2,
                            MD5 = bigEndianBinaryReaderTOC.ReadBytes(16),
                            zIndex = bigEndianBinaryReaderTOC.ReadUInt32(),
                            Length = bigEndianBinaryReaderTOC.ReadUInt40(),
                            Offset = bigEndianBinaryReaderTOC.ReadUInt40()
						});
						num2++;
					}

                    long decMax = (this.header.archiveFlags == 4) ? 32 : 0;
                    uint num3 = (this.header.TotalTOCSize - (uint)(tocStream.Position + decMax)) / (uint)b;
					uint[] array = new uint[num3];
					num2 = 0;
					while (num2 < num3)
					{
						switch (b)
						{
						case 2:
                            array[num2] = (uint)bigEndianBinaryReaderTOC.ReadUInt16();
							break;
						case 3:
                            array[num2] = bigEndianBinaryReaderTOC.ReadUInt24();
							break;
						case 4:
                            array[num2] = bigEndianBinaryReaderTOC.ReadUInt32();
							break;
						}
						num2++;
					}
					this.inflateEntries(bigEndianBinaryReader, array.ToArray<uint>(), this.header.blockSize);
					this.ReadNames();
				}
			}
            //str.Flush();
		}

	}


    public class Entry
    {
        private string _name;
        public int id
        {
            get;
            set;
        }
        public ulong Length
        {
            get;
            set;
        }
        public byte[] MD5
        {
            get;
            set;
        }
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
                this.UpdateNameMD5();
            }
        }
        public uint zIndex
        {
            get;
            set;
        }
        public ulong Offset
        {
            get;
            set;
        }
        public DataPointer Data
        {
            get;
            set;
        }
        public Entry()
        {
            this.Name = string.Empty;
        }
        public override string ToString()
        {
            return this.Name;
        }
        public void UpdateNameMD5()
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            this.MD5 = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(this.Name));
        }


        public class DataPointer
        {
            private Entry entry;
            private BigEndianBinaryReader reader;
            private uint[] zLengths;
            private uint blockSize;

            public DataPointer(Entry entry, BigEndianBinaryReader reader, uint[] zLengths, uint blockSize)
            {
                this.entry = entry;
                this.reader = reader;
                this.zLengths = zLengths;
                this.blockSize = blockSize;
            }


            public Stream OpenStream()
            {
                var output = new MemoryStream();
                if (entry.Length != 0)
                {
                    reader.BaseStream.Position = (long)entry.Offset;
                    uint num = entry.zIndex;
                    do
                    {
                        if (zLengths[num] == 0)
                        {
                            byte[] array = reader.ReadBytes((int)blockSize);
                            output.Write(array, 0, (int)blockSize);
                        }
                        else
                        {
                            ushort num2 = reader.ReadUInt16();
                            reader.BaseStream.Position -= 2;
                            byte[] array = reader.ReadBytes((int)zLengths[num]);
                            if (num2 == 30938)
                            {
                                ZOutputStream zOutputStream = new ZOutputStream(output);
                                zOutputStream.Write(array, 0, array.Length);
                                zOutputStream.Flush();
                            }
                            else
                            {
                                output.Write(array, 0, array.Length);
                            }
                        }
                        num += 1;
                    }
                    while (output.Length < (long)entry.Length);
                }
                output.Flush();
                output.Seek(0, SeekOrigin.Begin);

                return output;
            }
        }
    }
}

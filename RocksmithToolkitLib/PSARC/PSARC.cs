using zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.PSARC
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

		public void InflateEntry(Entry entry, BigEndianBinaryReader reader, uint[] zLenghts, uint blockSize, Stream output)
		{
			if (entry.Length != 0)
			{
				reader.BaseStream.Position = (long)entry.Offset;
				uint num = entry.zIndex;
				do
				{
					if (zLenghts[num] == 0)
					{
						byte[] array = reader.ReadBytes((int)blockSize);
						output.Write(array, 0, (int)blockSize);
					}
					else
					{
						ushort num2 = reader.ReadUInt16();
						reader.BaseStream.Position -= 2;
						byte[] array = reader.ReadBytes((int)zLenghts[num]);
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
		}

		public void ReadNames()
		{
			this.Entries[0].Name = "NamesBlock.bin";
			Stream data = this.Entries[0].Data;
			BinaryReader binaryReader = new BinaryReader(data);
			StringBuilder stringBuilder = new StringBuilder(100);
			string empty = string.Empty;
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

        private void inflateEntries(BigEndianBinaryReader reader, uint[] zLenghts, uint blockSize)
        {
            foreach (Entry current in this.Entries)
            {
                current.Data = new TempFileStream();
                this.InflateEntry(current, reader, zLenghts, blockSize, current.Data);
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
            str.Flush();
		}

		private void deflateEntries(out Dictionary<Entry, byte[]> entryDeflatedData, out List<uint> zLengths, uint blockSize)
		{
			entryDeflatedData = new Dictionary<Entry, byte[]>();
			zLengths = new List<uint>();
			foreach (Entry current in this.Entries)
			{
				current.zIndex = (uint)zLengths.Count;
				current.Data.Seek(0, SeekOrigin.Begin);
				Stream data = current.Data;
				List<Tuple<byte[], int>> list = new List<Tuple<byte[], int>>();
				while (data.Position < data.Length)
				{
					byte[] array = new byte[2 * blockSize];
					MemoryStream memoryStream = new MemoryStream(array);
					byte[] array2 = new byte[blockSize];
					int num = data.Read(array2, 0, array2.Length);
					ZOutputStream zOutputStream = new ZOutputStream(memoryStream, 9);
					zOutputStream.Write(array2, 0, num);
					zOutputStream.Flush();
					zOutputStream.finish();
					int num2 = (int)zOutputStream.TotalOut;
					if (num2 > num)
					{
						list.Add(new Tuple<byte[], int>(array2, num));
					}
					else
					{
						if (num2 < (blockSize - 1))
						{
							list.Add(new Tuple<byte[], int>(array, num2));
						}
						else
						{
							list.Add(new Tuple<byte[], int>(array2, num));
						}
					}
				}
				int num3 = 0;
				foreach (Tuple<byte[], int> current2 in list)
				{
					num3 += current2.Item2;
					zLengths.Add((uint)current2.Item2);
				}
				byte[] array3 = new byte[num3];
				MemoryStream memoryStream2 = new MemoryStream(array3);
				foreach (Tuple<byte[], int> current2 in list)
				{
					memoryStream2.Write(current2.Item1, 0, current2.Item2);
				}
				entryDeflatedData.Add(current, array3);
			}
		}

		public void AddEntry(string name, Stream data)
		{
            if (name == "NamesBlock.bin")
                return;

			Entry entry = new Entry
			{
				Name = name,
				Data = data
			};
			entry.Length = (ulong)entry.Data.Length;
			this.AddEntry(entry);
		}

		public void AddEntry(Entry entry)
		{
			this.Entries.Add(entry);
			entry.id = this.Entries.Count - 1;
		}

		private void UpdateManifest()
		{
			if (this.Entries.Count < 1)
			{
				this.Entries.Add(new Entry());
			}
			this.Entries[0].Data = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(this.Entries[0].Data);
			for (int i = 1; i < this.Entries.Count; i++)
			{
				binaryWriter.Write(Encoding.ASCII.GetBytes(this.Entries[i].Name));
				if (i != this.Entries.Count - 1)
					binaryWriter.Write('\n');
			}
			this.Entries[0].Data.Seek(0, SeekOrigin.Begin);
		}

		public void Write(Stream str, bool encrypt)
		{
            if (encrypt)
                this.header.archiveFlags = 4;
			this.header.TOCEntrySize = 30;
			this.UpdateManifest();
			Dictionary<Entry, byte[]> dictionary;
			List<uint> list;
			this.deflateEntries(out dictionary, out list, this.header.blockSize);
			byte b = 1;
			uint num = 256;
			do
			{
				num *= 256;
				b += 1;
			}
			while (num < this.header.blockSize);
			BigEndianBinaryWriter bigEndianBinaryWriter = new BigEndianBinaryWriter(str);
			this.header.TotalTOCSize = (uint)(32 + this.header.TOCEntrySize * (this.Entries.Count) + ((int)b * list.Count));
			this.Entries[0].Offset = (ulong)this.header.TotalTOCSize;
			for (int i = 1; i < this.Entries.Count; i++)
			{
				this.Entries[i].Offset = this.Entries[i - 1].Offset + (ulong)(dictionary[this.Entries[i - 1]].Length);
			}
			bigEndianBinaryWriter.Write(this.header.MagicNumber);
			bigEndianBinaryWriter.Write(this.header.VersionNumber);
			bigEndianBinaryWriter.Write(this.header.CompressionMethod);
			bigEndianBinaryWriter.Write(this.header.TotalTOCSize);
			bigEndianBinaryWriter.Write(this.header.TOCEntrySize);
			bigEndianBinaryWriter.Write(this.Entries.Count);
			bigEndianBinaryWriter.Write(this.header.blockSize);
			bigEndianBinaryWriter.Write(this.header.archiveFlags);
			foreach (Entry current in this.Entries)
			{
				current.UpdateNameMD5();
				bigEndianBinaryWriter.Write((current.id == 0) ? new byte[16] : current.MD5);
				bigEndianBinaryWriter.Write(current.zIndex);
				bigEndianBinaryWriter.WriteUInt40((ulong)current.Data.Length);
				bigEndianBinaryWriter.WriteUInt40(current.Offset);
			}
			foreach (uint current2 in list)
			{
				switch (b)
				{
				    case 2:
					    bigEndianBinaryWriter.Write((ushort)current2);
					    break;
				    case 3:
					    bigEndianBinaryWriter.WriteUInt24(current2);
					    break;
				    case 4:
					    bigEndianBinaryWriter.Write(current2);
					    break;
				}
			}
			foreach (Entry current in this.Entries)
			{
				bigEndianBinaryWriter.Write(dictionary[current]);
			}

            if (encrypt)
            {
                var encStream = new MemoryStreamExtension();
                using (var outputStream = new MemoryStreamExtension())
                {
                    str.Seek(32, SeekOrigin.Begin);
                    RijndaelEncryptor.EncryptPSARC(str, outputStream, this.header.TotalTOCSize);

                    int bytesRead;
                    byte[] buffer = new byte[30000];

                    str.Seek(0, SeekOrigin.Begin);
                    while ((bytesRead = str.Read(buffer, 0, buffer.Length)) > 0)
                        encStream.Write(buffer, 0, bytesRead);
                    int decMax = (int)this.header.TotalTOCSize - 32;
                    int decSize = 0;
                    outputStream.Seek(0, SeekOrigin.Begin);
                    encStream.Seek(32, SeekOrigin.Begin);
                    while ((bytesRead = outputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        decSize += bytesRead;
                        if (decSize > decMax) bytesRead = decMax - (decSize - bytesRead);
                        encStream.Write(buffer, 0, bytesRead);
                    }
                }

                str.Seek(0, SeekOrigin.Begin);
                encStream.Seek(0, SeekOrigin.Begin);
                encStream.CopyTo(str);
            }

			str.Flush();
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.PSARC
{
    public class PSARC : IDisposable
    {
        private class Header
        {
            public uint MagicNumber;
            public uint VersionNumber;
            public uint CompressionMethod;
            public uint TotalTOCSize;
            public uint TOCEntrySize;
            public uint numFiles;
            public uint blockSizeAlloc;
            public uint archiveFlags;
            public Header()
            {
                this.MagicNumber = 1347633490; //PSAR
                this.VersionNumber = 65540;//1.4
                this.CompressionMethod = 2053925218;//zlib (also avalible lzma)
                this.TOCEntrySize = 30;
                this.numFiles = 0;
                this.blockSizeAlloc = 65536;//Decompression buffer size
                this.archiveFlags = 0;
            }
        }

        private PSARC.Header header;
        public List<Entry> TOC
        {
            get;
            private set;
        }
        private uint[] zBlocksSizeList;

        internal int bNum{
            get{ return (int)Math.Log(this.header.blockSizeAlloc, byte.MaxValue + 1); }
        }

        public PSARC()
        {
            this.header = new PSARC.Header();
            this.TOC = new List<Entry>();
            this.TOC.Add(new Entry());
            this.zBlocksSizeList = new uint[]{};
        }

        private long RequiredPsarcSize()
        {
            if(this.TOC.Count > 0)
            {//get last_entry.offset+it's size
                var last_entry = this.TOC[this.TOC.Count - 1];
                var TotalLen = last_entry.Offset;
                var zNum = this.zBlocksSizeList.Length - last_entry.zIndexBegin;
                for( int z = 0; z < zNum; z++ )
                {
                    var num = this.zBlocksSizeList[last_entry.zIndexBegin + z];
                    TotalLen += (num == 0) ? this.header.blockSizeAlloc : num;
                }
                return (long)TotalLen;
            }
            return this.header.TotalTOCSize;//already read
        }

        #region IDisposable implementation
        ~PSARC()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        public void Dispose(bool disposing)
        {
            if(disposing){
                header = null;
                TOC.Clear();
                zBlocksSizeList = null;
                if(_reader != null) _reader.Dispose();
            }
        }
        #endregion
        #region Helpers
        /// <summary>
        /// Inflates selected entry.
        /// </summary>
        /// <param name="entry">Enty to unpack.</param>
        public void InflateEntry(Entry entry)
        {
            var tmpOut = new TempFileStream();
            if(entry.Length > 0)
            {// Decompress Entry
                _reader.BaseStream.Position = (long)entry.Offset;
                uint blockSize = this.header.blockSizeAlloc;
                uint zChunkID = entry.zIndexBegin;
                do
                {
                    if (this.zBlocksSizeList[zChunkID] == 0)
                    {
                        byte[] array = _reader.ReadBytes((int)blockSize);
                        tmpOut.Write(array, 0, (int)blockSize);
                    }
                    else
                    {
                        ushort num2 = _reader.ReadUInt16();
                        _reader.BaseStream.Position -= 2;
                        byte[] array = _reader.ReadBytes((int)this.zBlocksSizeList[zChunkID]);
                        if (num2 == 30938)
                        {
                            try {
                                RijndaelEncryptor.unZip(array, tmpOut, rewind:false);
                            }
                            catch (Exception ex)
                            {// skip this zData chunk, we can't "repair" it, but we can fill it with zeroes.
                                ErrMSG = String.Format(@"{2}CDLC contains a broken datachunk in file '{0}'.{2}Warning: {1}[2]", entry.Name.Split('/').Last(), ex.Message, Environment.NewLine);
                                Console.Write(ErrMSG);
                                tmpOut.Write(new byte[array.Length], 0, array.Length);
                            }
                        }
                        else
                        {
                            tmpOut.Write(array, 0, array.Length);
                        }
                    }
                    zChunkID += 1;
                }
                while (tmpOut.Length < (long)entry.Length);
            }
            tmpOut.Seek(0, SeekOrigin.Begin);
            tmpOut.Flush();
            entry.Data = tmpOut;
        }

        public void InflateEntry(string name)
        {
            foreach(var entry in this.TOC.Where(t => t.Name.EndsWith(name)))
            {
                this.InflateEntry(entry);
            }
        }
        /// <summary>
        /// Inflates the entries.
        /// </summary>
        public void InflateEntries()
        {
            foreach (Entry current in this.TOC)
            {// We really can use Parrallel here.
                this.InflateEntry(current);
            }
        }

        /// <summary>
        /// Packs Entries to zStream
        /// </summary>
        /// <param name="entryDeflatedData">zStreams</param>
        /// <param name="zLengths">zBlocksSizeList</param>
        private void DeflateEntries(out Dictionary<Entry, byte[]> entryDeflatedData, out List<uint> zLengths)
        {
            entryDeflatedData = new Dictionary<Entry, byte[]>();
            uint blockSize = this.header.blockSizeAlloc;
            zLengths = new List<uint>();
            foreach (Entry entry in this.TOC)
            {
                entry.zIndexBegin = (uint)zLengths.Count;
                entry.Data.Seek(0, SeekOrigin.Begin);
                Stream data = entry.Data;
                var list = new List<Tuple<byte[], int>>();
                while (data.Position < data.Length)
                {
                    var array = new byte[blockSize];
                    var array1 = new byte[2 * blockSize];
                    var memoryStream = new MemoryStream(array1);

                    int num = data.Read(array, 0, array.Length);
                    RijndaelEncryptor.Zip(array, memoryStream);

                    int num2 = (int)memoryStream.Length;
                    if (num2 > num)
                    {
                        list.Add(new Tuple<byte[], int>(array, num));
                    }
                    else
                    {
                        if (num2 < (blockSize - 1))
                        {
                            list.Add(new Tuple<byte[], int>(array1, num2));
                        }
                        else
                        {
                            list.Add(new Tuple<byte[], int>(array, num));
                        }
                    }
                }
                int num3 = 0;
                foreach (var current2 in list)
                {
                    num3 += current2.Item2;
                    zLengths.Add((uint)current2.Item2);
                }
                byte[] array3 = new byte[num3];
                MemoryStream memoryStream2 = new MemoryStream(array3);
                foreach (var current2 in list)
                {
                    memoryStream2.Write(current2.Item1, 0, current2.Item2);
                }
                entryDeflatedData.Add(entry, array3);
            }
        }

        public void ReadManifest()
        {
            var toc = this.TOC[0];
            toc.Name = "NamesBlock.bin";
            InflateEntry(toc);
            Stream data = toc.Data;
            var stringName = new StringBuilder();
            int index = 1;
            using( var binaryReader = new BinaryReader(data) )
            {
                while(data.Position < data.Length)
                {
                    char c = binaryReader.ReadChar();
                    if(c == '\n')//0x0A
                    {
                        this.TOC[index++].Name = stringName.ToString();
                        stringName.Clear();
                    }else
                    {
                        stringName.Append(c);
                    }
                }
                this.TOC[index].Name = stringName.ToString();
                data.Seek(0, SeekOrigin.Begin);
                this.TOC.RemoveAt(0);
            }
        }
        private void WriteManifest()
        {
            if (this.TOC.Count < 1)
            {
                this.TOC.Add(new Entry());
            }
            this.TOC[0].Data = new MemoryStream();
            var binaryWriter = new BinaryWriter(this.TOC[0].Data);
            for (int i = 1; i < this.TOC.Count; i++)
            {
                binaryWriter.Write(Encoding.ASCII.GetBytes(this.TOC[i].Name));
                if (i != this.TOC.Count - 1)
                    binaryWriter.Write('\n');
            }
            this.TOC[0].Data.Seek(0, SeekOrigin.Begin);
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
            this.TOC.Add(entry);
            entry.id = this.TOC.Count - 1;
        }
        #endregion
        public string ErrMSG;
        private BigEndianBinaryReader _reader; //Psarc zlib data reader.
        public void Read(Stream psarc, bool lazy = false)
        {
            this.TOC.Clear();
            _reader = new BigEndianBinaryReader(psarc);
            this.header.MagicNumber = _reader.ReadUInt32();
            if(this.header.MagicNumber == 1347633490)//PSAR (BE)
            {
                //Parse Header
                this.header.VersionNumber = _reader.ReadUInt32();
                this.header.CompressionMethod = _reader.ReadUInt32();
                this.header.TotalTOCSize = _reader.ReadUInt32();
                this.header.TOCEntrySize = _reader.ReadUInt32();
                this.header.numFiles = _reader.ReadUInt32();
                this.header.blockSizeAlloc = _reader.ReadUInt32();
                this.header.archiveFlags = _reader.ReadUInt32();
                //Read TOC
                if(this.header.archiveFlags == 4 && (this.header.CompressionMethod == 2053925218))//zlib (BE)
                {// Decrypt TOC
                    const int headerSize = 32;
                    int tocSize = (int)this.header.TotalTOCSize - headerSize;
                    var tocStream = new TempFileStream(FileAccess.ReadWrite, FileShare.Read, (int)this.header.blockSizeAlloc);
                    using( var decStream = new MemoryStream() )
                    {
                        RijndaelEncryptor.DecryptPSARC(psarc, decStream, this.header.TotalTOCSize);

                        int bytesRead;
                        int decSize = 0;
                        byte[] buffer = new byte[this.header.blockSizeAlloc];
                        while ((bytesRead = decStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            decSize += bytesRead;
                            if (decSize > tocSize) bytesRead = tocSize - (decSize - bytesRead);
                            tocStream.Write(buffer, 0, bytesRead);
                        }
                        tocStream.Seek(0, SeekOrigin.Begin);

                    }
                    //Parse TOC Entries
                    _reader = new BigEndianBinaryReader(tocStream);
                    for(int i = 0; i < this.header.numFiles; i++)
                    {
                        this.TOC.Add(new Entry {
                            id = i,
                            MD5 = _reader.ReadBytes(16),
                            zIndexBegin = _reader.ReadUInt32(),
                            Length = _reader.ReadUInt40(),
                            Offset = _reader.ReadUInt40()
                        });
                    }
                    //Parse zBlocksSizeList
                    int zNum = (int)((tocSize - tocStream.Position) / bNum);
                    var zLengths = new uint[zNum];
                    for(int i = 0; i < zNum; i++)
                    {
                        switch(bNum)
                        {
                            case 2:
                                zLengths[i] = _reader.ReadUInt16();
                                break;
                            case 3:
                                zLengths[i] = _reader.ReadUInt24();
                                break;
                            case 4:
                                zLengths[i] = _reader.ReadUInt32();
                                break;
                        }
                    }
                    this.zBlocksSizeList = zLengths.ToArray();
                    tocStream.Close();
                    //Validate psarc size
                    if(psarc.Length < RequiredPsarcSize())
                        throw new InvalidDataException("Turncated psarc.");
                    _reader = new BigEndianBinaryReader(psarc);
                    //Read Filenames
                    ReadManifest();
                    //Read Data
                    psarc.Seek(this.header.TotalTOCSize, SeekOrigin.Begin);
                    if(!lazy) {
                        InflateEntries();
                    }
                }
            }
            psarc.Flush();
        }

        public void Write(Stream psarc, bool encrypt)
        {
            this.header.archiveFlags = encrypt ? 4U : 0U;
            this.header.TOCEntrySize = 30;
            this.WriteManifest();
            //Pack entries
            Dictionary<Entry, byte[]> zStreams; List<uint> zLengths;
            DeflateEntries(out zStreams, out zLengths);
            //Build zLengths
            var bigEndianBinaryWriter = new BigEndianBinaryWriter(psarc);
            this.header.TotalTOCSize = (uint)(32 + this.TOC.Count * this.header.TOCEntrySize + zLengths.Count * bNum);
            this.TOC[0].Offset = (ulong)this.header.TotalTOCSize;
            for (int i = 1; i < this.TOC.Count; i++)
            {
                this.TOC[i].Offset = this.TOC[i - 1].Offset + (ulong)(zStreams[this.TOC[i - 1]].Length);
            }
            //Write Header
            bigEndianBinaryWriter.Write(this.header.MagicNumber);
            bigEndianBinaryWriter.Write(this.header.VersionNumber);
            bigEndianBinaryWriter.Write(this.header.CompressionMethod);
            bigEndianBinaryWriter.Write(this.header.TotalTOCSize);
            bigEndianBinaryWriter.Write(this.header.TOCEntrySize);
            bigEndianBinaryWriter.Write(this.TOC.Count);
            bigEndianBinaryWriter.Write(this.header.blockSizeAlloc);
            bigEndianBinaryWriter.Write(this.header.archiveFlags);
            foreach (Entry current in this.TOC)
            {
                current.UpdateNameMD5();
                bigEndianBinaryWriter.Write((current.id == 0) ? new byte[16] : current.MD5);
                bigEndianBinaryWriter.Write(current.zIndexBegin);
                bigEndianBinaryWriter.WriteUInt40((ulong)current.Data.Length);
                bigEndianBinaryWriter.WriteUInt40(current.Offset);
            }
            foreach (uint zNum in zLengths)
            {
                switch (bNum)
                {
                    case 2:
                        bigEndianBinaryWriter.Write((ushort)zNum);
                        break;
                    case 3:
                        bigEndianBinaryWriter.WriteUInt24(zNum);
                        break;
                    case 4:
                        bigEndianBinaryWriter.Write(zNum);
                        break;
                }
            }
            foreach (Entry current in this.TOC)
            {// Write zData
                bigEndianBinaryWriter.Write(zStreams[current]);
            }
            if (encrypt)
            {// Encrypt TOC
                var encStream = new MemoryStreamExtension();
                using (var outputStream = new MemoryStreamExtension())
                {
                    psarc.Seek(32, SeekOrigin.Begin);
                    RijndaelEncryptor.EncryptPSARC(psarc, outputStream, this.header.TotalTOCSize);

                    int bytesRead;
                    byte[] buffer = new byte[30000];

                    psarc.Seek(0, SeekOrigin.Begin);
                    while ((bytesRead = psarc.Read(buffer, 0, buffer.Length)) > 0)
                        encStream.Write(buffer, 0, bytesRead);

                    int decSize = 0;
                    int tocSize = (int)this.header.TotalTOCSize - 32;
                    outputStream.Seek(0, SeekOrigin.Begin);
                    encStream.Seek(32, SeekOrigin.Begin);
                    while ((bytesRead = outputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        decSize += bytesRead;
                        if (decSize > tocSize) bytesRead = tocSize - (decSize - bytesRead);
                        encStream.Write(buffer, 0, bytesRead);
                    }
                }

                psarc.Seek(0, SeekOrigin.Begin);
                encStream.Seek(0, SeekOrigin.Begin);
                encStream.CopyTo(psarc, (int)this.header.blockSizeAlloc);
            }
            psarc.Flush();
        }
    }
}

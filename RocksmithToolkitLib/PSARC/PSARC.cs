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
                this.VersionNumber = 65540; //1.4
                this.CompressionMethod = 2053925218; //zlib (also avalible lzma)
                this.TOCEntrySize = 30;
                this.numFiles = 0;
                this.blockSizeAlloc = 65536; //Decompression buffer size
                this.archiveFlags = 0; //It's bitfield actually see PSARC.bt
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
        /// <summary>
        /// Checks if psarc is not turncated.
        /// </summary>
        /// <returns>The psarc size.</returns>
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(disposing){
                header = null;
                TOC.Clear();
                if(_reader != null) _reader.Dispose();
                if(_writer != null) _writer.Dispose();
            }
        }
        #endregion
        #region Helpers
        /// <summary>
        /// Inflates selected entry.
        /// </summary>
        /// <param name="entry">Enty to unpack.</param>
        /// <param name = "destfilepath">Destanation file used instead of the temp file.</param>
        public void InflateEntry(Entry entry, string destfilepath = "")
        {
            if(entry.Length > 0)
            {// Decompress Entry
                uint zChunkID = entry.zIndexBegin;
                int blockSize = (int)this.header.blockSizeAlloc;
                bool isZlib = this.header.CompressionMethod == 2053925218;
                const int zHeader = 0x78DA;

                if (destfilepath.Length > 0)
                    entry.Data = new FileStream(destfilepath, FileMode.Create, FileAccess.Write, FileShare.Read);
                else
                    entry.Data = new TempFileStream();
                var data = entry.Data;
                _reader.BaseStream.Position = (long)entry.Offset;
                do
                {
                    if (this.zBlocksSizeList[zChunkID] == 0)
                    {// raw
                        byte[] array = _reader.ReadBytes(blockSize);
                        data.Write(array, 0, blockSize);
                    }
                    else
                    {
                        var num = _reader.ReadUInt16();
                        _reader.BaseStream.Position -= 2;

                        byte[] array = _reader.ReadBytes((int)this.zBlocksSizeList[zChunkID]);
                        if (num == zHeader)
                        {// compressed
                            try {
                                RijndaelEncryptor.Unzip(array, data, false);
                            }
                            catch (Exception ex)
                            {// skip... we can't "repair" it, but we can fill it with zeroes.
                                ErrMSG = String.Format(@"{2}CDLC contains a broken datachunk in file '{0}'.{2}Warning: {1}[2]", entry.Name.Split('/').Last(), ex.Message, Environment.NewLine);
                                Console.Write(ErrMSG);
                                data.Write(new byte[array.Length], 0, array.Length);
                            }
                        }
                        else
                        {// raw. used only after 0?
                            data.Write(array, 0, array.Length);
                        }
                    }
                    zChunkID += 1;
                }
                while (data.Length < (long)entry.Length);
                data.Seek(0, SeekOrigin.Begin);
                data.Flush();
            }
        }
        /// <summary>
        /// Inflates the entry.
        /// </summary>
        /// <param name="name">Name with extension.</param>
        public void InflateEntry(string name)
        {
            foreach(var entry in this.TOC.Where(t => t.Name.EndsWith(name, StringComparison.Ordinal)))
            {
                this.InflateEntry(entry);
            }
        }
        /// <summary>
        /// Inflates all entries in current psarc.
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
                var zList = new List<Tuple<byte[], int>>();
                entry.zIndexBegin = (uint)zLengths.Count;
                entry.Data.Seek(0, SeekOrigin.Begin);
                Stream data = entry.Data;
                while (data.Position < data.Length)
                {
                    var array_i = new byte[blockSize];
                    var array_o = new byte[blockSize * 2];
                    var memoryStream = new MemoryStream(array_o);

                    int plain_len = data.Read(array_i, 0, array_i.Length);
                    int packed_len = (int)RijndaelEncryptor.Zip(array_i, memoryStream, plain_len, false);

                    if (packed_len >= plain_len)
                    {// If packed data "worse" than plain (i.e. already packed) z = 0
                        zList.Add(new Tuple<byte[], int>(array_i, plain_len));
                    }
                    else
                    {// If packed data is good
                        if (packed_len < (blockSize - 1))
                        {// If packed data fits maximum packed block size z = packed_len
                            zList.Add(new Tuple<byte[], int>(array_o, packed_len));
                        }
                        else
                        {// Write plain. z = 0
                            zList.Add(new Tuple<byte[], int>(array_i, plain_len));
                        }
                    }
                }
                int zSisesSum = 0;
                foreach (var zSize in zList)
                {
                    zSisesSum += zSize.Item2;
                    zLengths.Add((uint)zSize.Item2);
                }
                var array3 = new byte[zSisesSum];
                var memoryStream2 = new MemoryStream(array3);
                foreach (var entryblock in zList)
                {
                    memoryStream2.Write(entryblock.Item1, 0, entryblock.Item2);
                }
                entryDeflatedData.Add(entry, array3);
            }
        }

        public void ReadManifest()
        {
            var toc = this.TOC[0];
            InflateEntry(toc);
            int index = 1;
            toc.Name = "NamesBlock.bin";
            using( var bReader = new StreamReader(toc.Data, true) )
            {
                foreach(var name in bReader.ReadToEnd().Split('\n'))//0x0A
                {
                    if(index < this.TOC.Count)
                        this.TOC[index].Name = name;
                    index++;
                }
            }
            this.TOC.RemoveAt(0);
        }
        private void WriteManifest()
        {
            if (this.TOC.Count == 0)
            {
                this.TOC.Add(new Entry());
            }
            this.TOC[0].Data = new MemoryStream();
            var binaryWriter = new BinaryWriter(this.TOC[0].Data);
            for (int i = 1; i < this.TOC.Count; i++)
            {/* '/' - path separator */
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
            entry.Id = this.TOC.Count - 1;
        }

        void ParseTOC()
        {// Parse TOC Entries
            for( int i = 0; i < this.header.numFiles; i++ )
            {
                this.TOC.Add(new Entry {
                    Id = i,
                    MD5 = _reader.ReadBytes(16),
                    zIndexBegin = _reader.ReadUInt32(),
                    Length = _reader.ReadUInt40(),
                    Offset = _reader.ReadUInt40()
                });
            }
        }
        #endregion
        public string ErrMSG;
        private BigEndianBinaryReader _reader;
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
                const int headerSize = 32;
                int tocSize = (int)this.header.TotalTOCSize - headerSize;
                if(this.header.archiveFlags == 4)//TOC_ENCRYPTED
                {// Decrypt TOC
                    var tocStream = new MemoryStream();
                    using( var decStream = new MemoryStream() )
                    {
                        RijndaelEncryptor.DecryptPSARC(psarc, decStream, this.header.TotalTOCSize);

                        int bytesRead;
                        int decSize = 0;
                        var buffer = new byte[this.header.blockSizeAlloc];
                        while((bytesRead = decStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            decSize += bytesRead;
                            if(decSize > tocSize) bytesRead = tocSize - (decSize - bytesRead);
                            tocStream.Write(buffer, 0, bytesRead);
                        }
                    }
                    tocStream.Seek(0, SeekOrigin.Begin);
                    _reader = new BigEndianBinaryReader(tocStream);
                }
                ParseTOC();
                //Parse zBlocksSizeList
                int zNum = (int)((tocSize - this.header.numFiles * this.header.TOCEntrySize) / bNum);
                var zLengths = new uint[zNum];
                for(int i = 0; i < zNum; i++)
                {
                    switch(bNum)
                    {
                        case 2://64KB
                            zLengths[i] = _reader.ReadUInt16();
                            break;
                        case 3://16MB
                            zLengths[i] = _reader.ReadUInt24();
                            break;
                        case 4://4GB
                            zLengths[i] = _reader.ReadUInt32();
                            break;
                    }
                }
                this.zBlocksSizeList = zLengths.ToArray();
                _reader.BaseStream.Flush();
                _reader = new BigEndianBinaryReader(psarc);
                //Validate psarc size
                if(psarc.Length < RequiredPsarcSize())
                    throw new InvalidDataException("Turncated psarc.");
                if(this.header.CompressionMethod == 2053925218)//zlib (BE)
                {   //Read Filenames
                    ReadManifest();
                    psarc.Seek(this.header.TotalTOCSize, SeekOrigin.Begin);
                    if(!lazy)
                    {// Read Data
                        InflateEntries();
                    }
                }
                else if(this.header.CompressionMethod == 1819962721)//lzma (BE)
                {
                    throw new NotImplementedException("LZMA compression not supported.");
                }
            }
            psarc.Flush();
        }
        private BigEndianBinaryWriter _writer;
        public void Write(Stream psarc, bool encrypt)
        {
            this.header.archiveFlags = encrypt ? 4U : 0U;
            this.header.TOCEntrySize = 30;
            this.WriteManifest();
            //Pack entries
            Dictionary<Entry, byte[]> zStreams; List<uint> zLengths;
            DeflateEntries(out zStreams, out zLengths);
            //Build zLengths
            _writer = new BigEndianBinaryWriter(psarc);
            this.header.TotalTOCSize = (uint)(32 + this.TOC.Count * this.header.TOCEntrySize + zLengths.Count * bNum);
            this.TOC[0].Offset = (ulong)this.header.TotalTOCSize;
            for (int i = 1; i < this.TOC.Count; i++)
            {
                this.TOC[i].Offset = this.TOC[i - 1].Offset + (ulong)(zStreams[this.TOC[i - 1]].Length);
            }
            //Write Header
            _writer.Write(this.header.MagicNumber);
            _writer.Write(this.header.VersionNumber);
            _writer.Write(this.header.CompressionMethod);
            _writer.Write(this.header.TotalTOCSize);
            _writer.Write(this.header.TOCEntrySize);
            _writer.Write(this.TOC.Count);
            _writer.Write(this.header.blockSizeAlloc);
            _writer.Write(this.header.archiveFlags);
            //Write Table of contents
            foreach (Entry current in this.TOC)
            {
                current.UpdateNameMD5();
                _writer.Write((current.Id == 0) ? new byte[16] : current.MD5);
                _writer.Write(current.zIndexBegin);
                _writer.WriteUInt40((ulong)current.Data.Length);
                _writer.WriteUInt40(current.Offset);
            }
            foreach (uint zLen in zLengths)
            {
                switch (bNum)
                {
                    case 2:
                        _writer.Write((ushort)zLen);
                        break;
                    case 3:
                        _writer.WriteUInt24(zLen);
                        break;
                    case 4:
                        _writer.Write(zLen);
                        break;
                }
            }
            //Write zData
            foreach (Entry current in this.TOC)
            {
                _writer.Write(zStreams[current]);
                current.Data.Close();
            }
            if (encrypt)
            {// Encrypt TOC
                var encStream = new MemoryStreamExtension();
                using (var outputStream = new MemoryStreamExtension())
                {
                    int bytesRead;
                    int decSize = 0;
                    var buffer = new byte[30000];
                    int tocSize = (int)this.header.TotalTOCSize - 32;

                    psarc.Seek(32, SeekOrigin.Begin);
                    RijndaelEncryptor.EncryptPSARC(psarc, outputStream, this.header.TotalTOCSize);

                    psarc.Seek(0, SeekOrigin.Begin);
                    while ((bytesRead = psarc.Read(buffer, 0, buffer.Length)) > 0)
                        encStream.Write(buffer, 0, bytesRead);

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

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RocksmithToolkitLib.PSARC
{
    public class Entry : System.IDisposable
    {

#region BinaryEntry

        public byte[] MD5 {
            get;
            internal set;
        }

        public uint zIndexBegin {
            get;
            set;
        }
        /// <summary>
        /// Original data length of this entry.
        /// </summary>
        /// <value>The length.</value>
        public ulong Length {
            get;
            set;
        }
        /// <summary>
        /// Starting offset from
        /// </summary>
        /// <value>The offset.</value>
        public ulong Offset {
            get;
            set;
        }

#endregion

        public int Id
        {
            get;
            internal set;
        }
        /*
        /// <summary>
        /// Length of packed bytes to read.
        /// </summary>
        public ulong zDatalen
        {
            get;
            set;
        }
        /// <summary>
        /// Actual entry data from archive.
        /// </summary>
        public Stream zData//TODO: reduce memory usage\streams count.
        {
            get;
            set;
        }*/
        /// <summary>
        /// Unpacked data.
        /// </summary>
        public Stream Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RocksmithToolkitLib.PSARC.Entry"/> is compressed.
        /// </summary>
        /// <value><c>true</c> if compressed; otherwise, <c>false</c>.</value>
        /// <remarks>Kinda rubbish but could be useful someday. Now inactive.</remarks>
        public bool Compressed {
            get;
            set;
        }

        public string Name { get; set; }

        public Entry()
        {
            Id = 0;
            Name = string.Empty;
        }
        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Data != null) Data.Dispose();
            MD5 = null;
        }
        #endregion
        public override string ToString()
        {
            return Name;
        }

        public void UpdateNameMD5()
        {
            MD5 = (Id == 0)? new byte[16] : new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Name));
        }
    }
}
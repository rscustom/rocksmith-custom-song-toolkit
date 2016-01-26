using System;
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
            set;
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

        public Stream Data {
            get;
            set;
        }

        public int Id {
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
        //My best guess is: RS2014 uses method like PSARCBrowser, they pick file from PSARC using md5(faster to use first file from collection, since it's defined be format, haha) and meet 2 of them, so they fail to read filenames from Manifest, rest is impossible. Changed check to Name instead of Id.
        public void UpdateNameMD5()
        {
            MD5 = (Name == String.Empty)? new byte[16] : new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Name));
        }
    }
}
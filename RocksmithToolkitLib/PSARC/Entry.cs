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

        public ulong Length {
            get;
            set;
        }

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
        /// <remarks>Kinda rubbush but could be useful someday. Now inactive.</remarks>
        public bool Compressed {
            get;
            set;
        }

        private string _name;
        public string Name {
            get {
                return this._name;
            }
            set {
                this._name = value;
                this.UpdateNameMD5();
            }
        }

        public Entry()
        {
            this.Id = 0;
            this.Name = string.Empty;
        }
        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(disposing){
                if(Data != null) Data.Dispose();
                MD5 = null;
            }
        }
        #endregion
        public override string ToString()
        {
            return this.Name;
        }

        public void UpdateNameMD5()
        {
            MD5 = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(this.Name));
        }
    }
}
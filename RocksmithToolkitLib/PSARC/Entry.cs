using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RocksmithToolkitLib.PSARC
{
	public class Entry
	{
		#region BinaryEntry
		public byte[] MD5
		{
			get;
			set;
		}
		public uint zIndexBegin
		{
			get;
			set;
		}
		public ulong Length
		{
			get;
			set;
		}
		public ulong Offset
		{
			get;
			set;
		}
		#endregion

		public Stream Data
		{
			get;
			set;
		}
		public int id
		{
			get;
			set;
		}
		private string _name;
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

		public Entry()
		{
			this.id = 0;
			this.Name = string.Empty;
		}
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

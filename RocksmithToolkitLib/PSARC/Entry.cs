using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace RocksmithToolkitLib.PSARC
{
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
		public Stream Data
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
	}
}

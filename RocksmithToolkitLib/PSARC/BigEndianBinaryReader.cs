using System;
using System.IO;
using System.Text;

namespace RocksmithToolkitLib.PSARC
{
	public class BigEndianBinaryReader : IDisposable
	{
		public virtual Stream BaseStream
		{
			get;
			private set;
		}
		public BigEndianBinaryReader(Stream input)
		{
			this.BaseStream = input;
		}
		public BigEndianBinaryReader(Stream input, Encoding encoding)
		{
			throw new NotImplementedException();
		}
		public void Dispose()
		{
			this.BaseStream.Dispose();
		}
		public virtual int PeekChar()
		{
			int result = this.BaseStream.ReadByte();
			this.BaseStream.Seek(-1L, SeekOrigin.Current);
			return result;
		}
		public virtual int Read()
		{
			return this.BaseStream.ReadByte();
		}
		public virtual int Read(byte[] buffer, int index, int count)
		{
			return this.BaseStream.Read(buffer, index, count);
		}
		public virtual int Read(char[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}
		public virtual bool ReadBoolean()
		{
            return this.BaseStream.ReadByte() != 0;
		}
		public virtual byte ReadByte()
		{
			return (byte)this.BaseStream.ReadByte();
		}
		public virtual byte[] ReadBytes(int count)
		{
			byte[] array = new byte[count];
			this.Read(array, 0, count);
			return array;
		}
		public virtual char ReadChar()
		{
			return (char)this.ReadByte();
		}
		public virtual char[] ReadChars(int count)
		{
			throw new NotImplementedException();
		}
		public virtual decimal ReadDecimal()
		{
			throw new NotImplementedException();
		}
		public virtual double ReadDouble()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				array[7 - i] = this.ReadByte();
			}
			return BitConverter.ToDouble(array, 0);
		}
		public virtual short ReadInt16()
		{
			return (short)((int)this.ReadByte() << 8 | (int)this.ReadByte());
		}
		public virtual int ReadInt32()
		{
			return (int)this.ReadByte() << 24 | (int)this.ReadByte() << 16 | (int)this.ReadByte() << 8 | (int)this.ReadByte();
		}
		public virtual long ReadInt64()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				array[7 - i] = this.ReadByte();
			}
			return BitConverter.ToInt64(array, 0);
		}
		public virtual sbyte ReadSByte()
		{
			return (sbyte)this.ReadByte();
		}
		public virtual float ReadSingle()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[3 - i] = this.ReadByte();
			}
			return BitConverter.ToSingle(array, 0);
		}
		public virtual string ReadString()
		{
			throw new NotImplementedException();
		}
		public virtual ushort ReadUInt16()
		{
			byte[] array = new byte[2];
			for (int i = 0; i < 2; i++)
			{
				array[1 - i] = this.ReadByte();
			}
			return BitConverter.ToUInt16(array, 0);
		}
		public virtual uint ReadUInt32()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[3 - i] = this.ReadByte();
			}
			return BitConverter.ToUInt32(array, 0);
		}
		public virtual ulong ReadUInt64()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				array[7 - i] = this.ReadByte();
			}
			return BitConverter.ToUInt64(array, 0);
		}
		public virtual ulong ReadUInt40()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < 5; i++)
			{
				array[4 - i] = this.ReadByte();
			}
			return BitConverter.ToUInt64(array, 0);
		}
		public virtual uint ReadUInt24()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 3; i++)
			{
				array[2 - i] = this.ReadByte();
			}
			return BitConverter.ToUInt32(array, 0);
		}
	}
}

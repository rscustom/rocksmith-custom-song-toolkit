using System;
using System.IO;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.PSARC
{
    public class BigEndianBinaryWriter : IDisposable
    {
        public Stream BaseStream
        {
            get;
            private set;
        }

        public BigEndianBinaryWriter(Stream input)
        {
            this.BaseStream = input;
        }

        public void Dispose()
        {
            this.BaseStream.Dispose();
        }
        public void Write(byte v)
        {
            this.BaseStream.WriteByte(v);
        }
        public void Write(short v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(int v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(long v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(ushort v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(uint v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(ulong v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(float v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(double v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 0; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void WriteUInt24(uint v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 1; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void WriteUInt40(ulong v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            for (int i = 3; i < bytes.Length; i++)
            {
                this.Write(bytes[bytes.Length - i - 1]);
            }
        }
        public void Write(byte[] val)
        {
            for (int i = 0; i < val.Length; i++)
            {
                byte v = val[i];
                this.Write(v);
            }
        }
    }
}

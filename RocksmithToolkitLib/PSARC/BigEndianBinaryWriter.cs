using System;
using System.IO;
using RocksmithToolkitLib.Extensions;
using System.Threading;
using System.Diagnostics;

namespace RocksmithToolkitLib.PSARC
{
    public class BigEndianBinaryWriter : IDisposable
    {
        public Stream BaseStream { get; private set; }

        public BigEndianBinaryWriter(Stream input)
        {
            this.BaseStream = input;
        }

        public void Dispose()
        {
            this.BaseStream.Dispose();
        }

        public void Flush()
        {
            this.BaseStream.Flush();
            GC.Collect();
        }

        public void Write(byte v)
        {
            //try
            //{
            this.BaseStream.WriteByte(v);
            //}
            //catch (Exception ex)
            //{
            //    // little fish
            //    Console.WriteLine("<ERROR> Little Fish: " + ex.Message);
            //    this.Flush();
            //    Thread.Sleep(200);
            //}
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
            //try
            //{
            for (int i = 0; i < val.Length; i++)
            {
                byte v = val[i];
                this.Write(v);
            }
            //}
            //catch (Exception ex)
            //{
            //    // big fish
            //    Console.WriteLine("<ERROR> Big Fish: " + ex.Message);
            //    this.Flush();
            //    Thread.Sleep(200);
            //}
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }


    }
}

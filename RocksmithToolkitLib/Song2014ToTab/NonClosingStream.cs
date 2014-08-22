//Taken from: http://www.yoda.arachsys.com/csharp/miscutil/
//modified by andulv to rewind underlying stream on close instead of "closing" the outer stream


/*
 * "Miscellaneous Utility Library" Software Licence

Version 1.0

Copyright (c) 2004-2008 Jon Skeet and Marc Gravell.
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright
notice, this list of conditions and the following disclaimer in the
documentation and/or other materials provided with the distribution.

3. The end-user documentation included with the redistribution, if
any, must include the following acknowledgment:

"This product includes software developed by Jon Skeet
and Marc Gravell. Contact skeet@pobox.com, or see 
http://www.pobox.com/~skeet/)."

Alternately, this acknowledgment may appear in the software itself,
if and wherever such third-party acknowledgments normally appear.

4. The name "Miscellaneous Utility Library" must not be used to endorse 
or promote products derived from this software without prior written 
permission. For written permission, please contact skeet@pobox.com.

5. Products derived from this software may not be called 
"Miscellaneous Utility Library", nor may "Miscellaneous Utility Library"
appear in their name, without prior written permission of Jon Skeet.

THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESSED OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL JON SKEET BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE. 
*/


using System;
using System.IO;
using System.Runtime.Remoting;

namespace RocksmithToolkitLib.Song2014ToTab
{
    /// <summary>
    /// Wraps a stream for all operations except Close and Dispose, which
    /// merely flush the stream and prevent further operations from being
    /// carried out using this wrapper.
    /// </summary>
    public sealed class NonClosingStreamWrapper : Stream
    {
        #region Members specific to this wrapper class
        /// <summary>
        /// Creates a new instance of the class, wrapping the specified stream.
        /// </summary>
        /// <param name="stream">The stream to wrap. Must not be null.</param>
        /// <exception cref="ArgumentNullException">stream is null</exception>
        public NonClosingStreamWrapper(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this.stream = stream;
        }

        Stream stream;
        /// <summary>
        /// Stream wrapped by this wrapper
        /// </summary>
        public Stream BaseStream
        {
            get { return stream; }
        }
        #endregion

        #region Overrides of Stream methods and properties
        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <param name="buffer">The buffer to read the data into. </param>
        /// <param name="offset">
        /// The byte offset in buffer at which to begin writing data read from the stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read. </param>
        /// <param name="callback">
        /// An optional asynchronous callback, to be called when the read is complete.
        /// </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular 
        /// asynchronous read request from other requests.
        /// </param>
        /// <returns>
        /// An IAsyncResult that represents the asynchronous read, 
        /// which could still be pending.
        /// </returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count,
                                               AsyncCallback callback, object state)
        {
            return stream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous write operation.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The byte offset in buffer from which to begin writing.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <param name="callback">
        /// An optional asynchronous callback, to be called when the write is complete.
        /// </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous 
        /// write request from other requests.
        /// </param>
        /// <returns>
        /// An IAsyncResult that represents the asynchronous write, 
        /// which could still be pending.
        /// </returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count,
                                                AsyncCallback callback, object state)
        {
            return stream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Indicates whether or not the underlying stream can be read from.
        /// </summary>
        public override bool CanRead
        {
            get { return  stream.CanRead; }
        }

        /// <summary>
        /// Indicates whether or not the underlying stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return  stream.CanSeek; }
        }

        /// <summary>
        /// Indicates whether or not the underlying stream can be written to.
        /// </summary>
        public override bool CanWrite
        {
            get { return  stream.CanWrite; }
        }

        /// <summary>
        /// This method is not proxied to the underlying stream; instead, the stream
        /// is "rewinded" (seek to beginning)
        /// </summary>
        public override void Close()
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        

        /// <summary>
        /// Throws a NotSupportedException.
        /// </summary>
        /// <param name="requestedType">The Type of the object that the new ObjRef will reference.</param>
        /// <returns>n/a</returns>
        public override ObjRef CreateObjRef(Type requestedType)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <param name="asyncResult">
        /// The reference to the pending asynchronous request to finish.
        /// </param>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) 
        /// and the number of bytes you requested. Streams only return 
        /// zero (0) at the end of the stream, otherwise, they should 
        /// block until at least one byte is available.
        /// </returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return stream.EndRead(asyncResult);
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            stream.EndWrite(asyncResult);
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            stream.Flush();
        }

        /// <summary>
        /// Throws a NotSupportedException.
        /// </summary>
        /// <returns>n/a</returns>
        public override object InitializeLifetimeService()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return stream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the current position in the underlying stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return stream.Position;
            }
            set
            {
                stream.Position = value;
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the underlying stream and advances the 
        /// position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains 
        /// the specified byte array with the values between offset and 
        /// (offset + count- 1) replaced by the bytes read from the underlying source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data 
        /// read from the underlying stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the 
        /// underlying stream.
        /// </param>
        /// <returns>The total number of bytes read into the buffer. 
        /// This can be less than the number of bytes requested if that many 
        /// bytes are not currently available, or zero (0) if the end of the 
        /// stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the 
        /// stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            return stream.ReadByte();
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">
        /// A value of type SeekOrigin indicating the reference 
        /// point used to obtain the new position.
        /// </param>
        /// <returns>The new position within the underlying stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the underlying stream.
        /// </summary>
        /// <param name="value">The desired length of the underlying stream in bytes.</param>
        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream and advances 
        /// the current position within the stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes 
        /// from buffer to the underlying stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at 
        /// which to begin copying bytes to the underlying stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the underlying stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and
        /// advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream. </param>
        public override void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }
        #endregion
    }
}

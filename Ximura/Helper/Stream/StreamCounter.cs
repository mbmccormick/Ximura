#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Runtime.Serialization;
using System.IO;

using System.Text;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This wrapper stream is used to count the number of bytes being written to the underlying stream.
    /// </summary>
    public class StreamCounter : Stream
    {
        #region Enum --> CounterDirection
        /// <summary>
        /// This enumeration determines which direction the stream is working.
        /// </summary>
        public enum CounterDirection
        {
            /// <summary>
            /// The stream is being written to.
            /// </summary>
            Write,
            /// <summary>
            /// The stream is being read from.
            /// </summary>
            Read
        }
        #endregion // Enum --> CounterDirection

        #region Declarations
        private object syncObj = new object();

        private int mBytesLengthWritten = 0;
        private int mBytesLengthRead = 0;
        private Stream baseStream;
        private CounterDirection mDirection;
        private long mTotalLength;
        private bool mRestrictRead;

        #endregion // Declarations
        #region Constructors
        public StreamCounter(Stream stream)
            : this(stream, CounterDirection.Write, -1)
        {
        }

        public StreamCounter(Stream stream, CounterDirection direction, long totalLength)
            : this(stream, direction, totalLength, false)
        {
        }


        public StreamCounter(Stream stream, CounterDirection direction, long totalLength, bool restrictRead)
        {
            baseStream = stream;
            mDirection = direction;
            mTotalLength = totalLength;
            mRestrictRead = restrictRead;
        }

        #endregion // Constructors

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get
            {
                lock (syncObj)
                {
                    return (mDirection == CounterDirection.Write) ? false :
                        (mRestrictRead ? Position < Length : baseStream.CanRead);
                }
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                lock (syncObj)
                {
                    return baseStream.CanTimeout;
                }
            }
        }

        public override bool CanWrite
        {
            get
            {
                lock (syncObj)
                {
                    return (mDirection == CounterDirection.Read) ? false : baseStream.CanWrite;
                }
            }
        }

#if (!PORTABLE)
        public override void Close()
        {
            lock (syncObj)
            {
                baseStream.Close();
            }
        }
#endif

        //public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        //{
        //    return baseStream.CreateObjRef(requestedType);
        //}



        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            lock (syncObj)
            {
                baseStream.Flush();
            }
        }


        public override long Length
        {
            get
            {
                lock (syncObj)
                {
                    return (mDirection == CounterDirection.Read) ? mTotalLength : baseStream.Length;
                }
            }
        }

        public override long Position
        {
            get
            {
                lock (syncObj)
                {
                    return (mDirection == CounterDirection.Read) ? mBytesLengthRead : mBytesLengthWritten;
                }
            }

            set { ; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (syncObj)
            {
                if (mRestrictRead && (count + Position) > mTotalLength)
                    count = (int)mTotalLength - (int)Position;
                if (count == 0)
                    return 0;
                int read = baseStream.Read(buffer, offset, count);
                mBytesLengthRead += read;
                return read;
            }
        }

        public override int ReadByte()
        {
            lock (syncObj)
            {
                int read = baseStream.ReadByte();
                if (read > -1) mBytesLengthRead++;
                return read;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                lock (syncObj)
                {
                    return baseStream.ReadTimeout;
                }
            }
            set
            {
                lock (syncObj)
                {
                    baseStream.ReadTimeout = value;
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek is not supported");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength is not supported");
        }

        public override string ToString()
        {
            return baseStream.ToString();
        }

        public override int WriteTimeout
        {
            get
            {
                lock (syncObj)
                {
                    return baseStream.WriteTimeout;
                }
            }
            set
            {
                lock (syncObj)
                {
                    baseStream.WriteTimeout = value;
                }
            }
        }

        public override void Write(byte[] array, int offset, int count)
        {
            lock (syncObj)
            {
                baseStream.Write(array, offset, count);
                mBytesLengthWritten += count;
            }
        }

        public override void WriteByte(byte value)
        {
            lock (syncObj)
            {
                baseStream.WriteByte(value);
                mBytesLengthWritten += 1;
            }
        }

        public int BytesLength
        {
            get
            {
                lock (syncObj)
                {
                    return (mDirection == CounterDirection.Read) ? mBytesLengthRead : mBytesLengthWritten;
                }
            }
        }
    }
}

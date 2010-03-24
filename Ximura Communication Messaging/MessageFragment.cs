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
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Text;

using Ximura;

using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// The message fragment class is the base class for message based communication.
    /// </summary>
    public class MessageFragment : MessageFragment<IXimuraMessageTermination>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageFragment()
            : base()
        {
        }
        #endregion

        #region ResetTerminator()
        /// <summary>
        /// The method resets the terminator and returns it to the pool.
        /// </summary>
        protected override void ResetTerminator()
        {
            if (mTerminator != null && mTerminator.ObjectPoolCanReturn)
                mTerminator.ObjectPoolReturn();

            mTerminator = null;
        } 
        #endregion
    }

    /// <summary>
    /// This is the base fragment class that implements fragment functionality.
    /// </summary>
    /// <typeparam name="TERM">The termination class.</typeparam>
    public class MessageFragment<TERM> : MessageStreamBase, IXimuraMessage, IXimuraMessageLoadData<TERM>, IXimuraPoolManagerDirectAccess
        where TERM: class, IXimuraMessageTermination
    {
        #region Declarations
        /// <summary>
        /// This is terminator for the fragment.
        /// </summary>
        protected TERM mTerminator;
        /// <summary>
        /// This is the byte buffer for the fragment.
        /// </summary>
        protected byte[] mBuffer = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageFragment()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This is the reset method to set the content.
        /// </summary>
        public override void Reset()
        {
            ResetTerminator();

            ResetBuffer();
            base.Reset();

            //MaxLength = 0;
            BodyLength = -1;
        }
        #endregion // Reset()

        #region ResetBuffer()
        /// <summary>
        /// This method resets the buffer to its default size.
        /// </summary>
        protected virtual void ResetBuffer()
        {
            if (mBuffer != null && Length >= 0 && Length < BufferSizePrefer)
            {
                //Ok, zero byte the data
                while (Length > 0 && --Length >= 0)
                {
                    mBuffer[Length] = 0;
                }
            }
            else if (mBuffer != null)
            {
                mBuffer = null;
                Length = 0;
            }
        }
        #endregion // ResetBuffer()
        #region InternalBuffer
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual byte[] InternalBuffer
        {
            get
            {
                return mBuffer;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The InternalBuffer cannot be set when the fragment is not initializing.");
                mBuffer = value;
                Position = mBuffer.Length;
                Length = mBuffer.Length;
                MaxLength = mBuffer.Length;
            }
        }
        #endregion // InternalBuffer
        #region EnsureCapacity(int length)
        /// <summary>
        /// This method ensures that the internal buffer has the necessary write capacity.
        /// </summary>
        /// <param name="length">The length of bytes to write.</param>
        protected virtual bool EnsureCapacity(int length)
        {
            try
            {
                lock (this)
                {
                    if (mBuffer == null)
                    {
                        mBuffer = new byte[CalculateCapacity(length)];
                        Position = 0;
                        Length = 0;
                        return true;
                    }

                    if (mBuffer.Length < (Position + length))
                    {
                        byte[] newBuffer = new byte[CalculateCapacity((int)Position + length)];
                        Buffer.BlockCopy(mBuffer, 0, newBuffer, 0, (int)Position);
                        mBuffer = newBuffer;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// This method calculates the new size of the buffer.
        /// </summary>
        /// <param name="length">The full length.</param>
        /// <returns>Returns the length rounded up to the nearest growth factor.</returns>
        protected virtual int CalculateCapacity(int length)
        {
            if (length < BufferSizeInitial)
                return BufferSizeInitial;

            int remainder = length - BufferSizeInitial;
            int newSize = BufferSizeInitial;

            newSize += BufferSizeGrow * (remainder / BufferSizeGrow);
            if ((remainder % BufferSizeGrow) > 0)
                newSize += BufferSizeGrow;

            return newSize;
        }
        #endregion // EnsureCapacity(int length)

        #region ResetTerminator()/ResetTerminator(IXimuraMessageTermination newTerminator)
        /// <summary>
        /// This method resets the terminator and returns it to the object pool. For messages that have a fixed terminator, 
        /// this method should be overrided and should create the default terminator if the terminator is null, or reset the terminator.
        /// </summary>
        protected virtual void ResetTerminator()
        {
            if (mTerminator == null && PoolManager!=null)
                mTerminator = PoolManager.GetPoolManager<TERM>().Get();

            if (mTerminator!=null)
                mTerminator.Reset();
        }

        protected virtual void ResetTerminator(TERM newTerminator)
        {
            if (mTerminator != null && mTerminator.ObjectPoolCanReturn)
                mTerminator.ObjectPoolReturn();

            mTerminator = newTerminator;
        }
        #endregion // ResetTerminator()
        #region Terminator
        /// <summary>
        /// This is the fragment terminator used to close off the fragment.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual TERM Terminator
        {
            get
            {
                if (mTerminator == null)
                    ResetTerminator();

                return mTerminator;
            }
        }
        #endregion // FragmentTerminator

        #region Read
        #region Read(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method reads from the fragment.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The data length.</param>
        /// <returns>The number of byte written in to the buffer.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ByteBufferChecks(buffer, offset, count);

            if (!CanRead || count == 0)
                return 0;

            int available = (int)(Length - Position);
            if (available <= 0)
                return 0;

            if (available > count)
                available = count;

            Buffer.BlockCopy(InternalBuffer, (int)Position, buffer, offset, available);
            Position += available;

            return available;
        }
        #endregion // Read(byte[] buffer, int offset, int count)
        #endregion // Read

        #region Write
        #region Write(byte[] buffer, int offset, int count)
        /// This method reads from the incoming buffer and writes it to the fragment.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The data length.</param>
        /// <returns>Returns the number of bytes actually read from the buffer.</returns>
        public override int Write(byte[] buffer, int offset, int count)
        {
            ByteBufferChecks(buffer, offset, count);

            if (!CanWrite)
                throw new ArgumentOutOfRangeException("Fragment cannot be written to.");

            int written = 0;
            switch (TerminationType)
            {
                case FragmentTerminationType.ByteLength:
                    written = WriteProcessByteLength(buffer, offset, count);
                    break;
                case FragmentTerminationType.Terminator:
                    written = WriteProcessTerminator(buffer, offset, count);
                    break;
                case FragmentTerminationType.Custom:
                    written = WriteProcessCustom(buffer, offset, count);
                    break;
                default:
                    throw new NotImplementedException("Termination Type is not implemented.");
            }

            Position += written;
            Length += written;

            return written;
        }
        #endregion

        #region WriteProcessByteLength (byte[] buffer, int offset, int count)
        protected virtual int WriteProcessByteLength(byte[] buffer, int offset, int count)
        {
            if (!EnsureCapacity(count))
                throw new OutOfMemoryException("There is not enough capacity to write.");
            Buffer.BlockCopy(buffer, offset, mBuffer, (int)Position, count);

            return count;
        }
        #endregion // WriteProcessByteLength (byte[] buffer, int offset, int count)
        #region WriteProcessTerminator(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method searches writes the incoming stream to the buffer, but terminates when it reaches 
        /// the termination characters.
        /// </summary>
        /// <param name="buffer">The incoming buffer.</param>
        /// <param name="offset">The offset</param>
        /// <param name="count">The number of bytes available.</param>
        /// <returns>Returns the number of bytes read from the incoming buffer.</returns>
        protected virtual int WriteProcessTerminator(byte[] buffer, int offset, int count)
        {
            int bytesRead;
            long? bodyLength = null;

            try
            {
                if (Terminator.Match(buffer, offset, count, out bytesRead, out bodyLength))
                {
                    //Ok, we have a full match so write the remaining bytes to the buffer and then quit.
                    CanWrite = false;
                    return bytesRead > 0 ? WriteProcessByteLength(buffer, offset, bytesRead) : 0;
                }
                else
                {
                    //No match just write the bytes to the buffer and continue.
                    return bytesRead > 0 ? WriteProcessByteLength(buffer, offset, bytesRead) : 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bodyLength.HasValue)
                    BodyLength = bodyLength;
            }
        }
        #endregion
        #region IsTerminator
        /// <summary>
        /// This method returns true if the fragment has completed and is exactly equal to the termination string.
        /// </summary>
        public override bool IsTerminator
        {
            get
            {
                if (CanWrite || Terminator == null)
                    return false;

                return Terminator.IsTerminator;
            }
        }
        #endregion
        #region WriteProcessCustom(byte[] buffer, int offset, int count)
        protected virtual int WriteProcessCustom(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("WriteProcessCustom is not implemented.");
        }
        #endregion // WriteProcessCustom(byte[] buffer, int offset, int count)

        #region TerminationType
        /// <summary>
        /// This method specifiies the termination logic for the fragment.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual FragmentTerminationType TerminationType
        {
            get
            {
                if (Terminator == null)
                    return FragmentTerminationType.ByteLength;
                else
                    return FragmentTerminationType.Terminator;
            }
        }
        #endregion // TerminationType
        #endregion // Write

        #region Load
        #region Load(Stream strmData)
        /// <summary>
        /// This method loads the fragment with the initial data from a stream.
        /// </summary>
        /// <param name="strmData">THe stream to read from.</param>
        /// <returns>Returns the number of bytes read from the stream.</returns>
        public override int Load(Stream strmData)
        {
            if (!strmData.CanRead)
                throw new IOException("The incoming stream cannot be read from.");

            Direction = MessageDirection.Write;
            CanWrite = true;
            int length = 0;
            int bytesread = 0;
            byte[] buffer = new byte[4000];

            strmData.Position = 0;
            while (strmData.CanRead)
            {
                bytesread = strmData.Read(buffer, 0, 4000);
                if (bytesread == 0)
                    break;
                length += Write(buffer, 0, bytesread);
            }

            CanWrite = false;
            Position = 0;
            Direction = MessageDirection.Read;
            CanRead = true;

            return length;
        }
        #endregion // Load(Stream strmData)
        #region Load(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method loads the fragment with the initial data.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count of the number of bytes.</param>
        /// <returns>Returns the number of bytes read from the buffer.</returns>
        public override int Load(byte[] buffer, int offset, int count)
        {
            ByteBufferChecks(buffer, offset, count);

            Direction = MessageDirection.Write;
            CanWrite = true;
            int length = Write(buffer, offset, count);
            CanWrite = false;
            Position = 0;
            Direction = MessageDirection.Read;
            CanRead = true;

            return length;
        }
        #endregion // Load(byte[] buffer, int offset, int count)

        #region Load(TERM terminator)
        public void Load(TERM terminator)
        {
            ResetTerminator(terminator);
            Load();
        }
        #endregion // Load(IXimuraMessageTermination terminator)
        #region Load(TERM terminator, long maxLength)
        public void Load(TERM terminator, long maxLength)
        {
            ResetTerminator(terminator);
            Load(maxLength);
        }
        #endregion // Load(IXimuraMessageTermination terminator, long maxLength)

        #region Load(TERM terminator, string data)
        public int Load(TERM terminator, string data)
        {
            ResetTerminator(terminator);
            return Load(data);
        }
        #endregion // Load(IXimuraMessageTermination terminator, string data)
        #region Load(TERM terminator, string data, Encoding encoding)
        public int Load(TERM terminator, string data, Encoding encoding)
        {
            ResetTerminator(terminator);
            return Load(data, encoding);
        }
        #endregion // Load(IXimuraMessageTermination terminator, string data, Encoding encoding)

        #region Load(TERM terminator, Stream data)
        public int Load(TERM terminator, Stream data)
        {
            ResetTerminator(terminator);
            return Load(data);
        }
        #endregion // Load(IXimuraMessageTermination terminator, Stream data)
        #region Load(TERM terminator, byte[] buffer, int offset, int count)
        public int Load(TERM terminator, byte[] buffer, int offset, int count)
        {
            ResetTerminator(terminator);
            return Load(buffer, offset, count);
        }
        #endregion // Load(IXimuraMessageTermination terminator, long maxLength)
        #endregion // Load

        #region DebugString
        /// <summary>
        /// This string provides a ASCII representation of the byte buffer.
        /// </summary>
        public override string DebugString
        {
            get
            {
                if (Length == 0 || InternalBuffer == null)
                    return "";

                return Encoding.UTF8.GetString(InternalBuffer, 0, (int)Length);
            }
        }
        #endregion
        #region ToArray()/ToArray(bool Copy)
        /// <summary>
        /// This method outputs a copy of the current byte array.
        /// </summary>
        /// <returns>Returns a copy of the internal byte array.</returns>
        public virtual byte[] ToArray()
        {
            return ToArray(true);
        }
        /// <summary>
        /// This method outputs the current byte array.
        /// </summary>
        /// <param name="copy">The boolean value should be set to true if you require a copy 
        /// of the internal array, or the actual array.</param>
        /// <returns>Returns a byte array.</returns>
        public virtual byte[] ToArray(bool copy)
        {
            if (!copy)
                return mBuffer;

            if (this.Length == 0)
                return new byte[] { };

            byte[] outBlob = new byte[Length];

            Buffer.BlockCopy(mBuffer, 0, outBlob, 0, (int)Length);

            return outBlob;
        }
        #endregion // ToArray()

        #region Dispose(bool disposing)
        private bool disposed = false;
        /// <summary>
        /// This is the dispose override.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                mTerminator = null;
                mBuffer = null;
                base.Dispose(disposing);
            }
            disposed = true;
        }
        #endregion // Dispose(bool disposing)

    }
}

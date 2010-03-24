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
using System.IO.Compression;
using System.Text;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// Summary description for BinaryReaderWriter.
    /// </summary>
    public class ContentSerializationReaderWriter : IXimuraContentSerializationReaderWriter
    {
        #region Declarations
        private byte[] mBuffer = new byte[0x10];
        private byte[] mLargeByteBuffer = null;
        private Encoding mEncoding = null;
        private Encoder mEncoder = null;
        private Decoder mDecoder = null;
        private Stream mStream;
        private int mMaxChars = 0;
        #endregion // Declarations
        #region Constructor

        public ContentSerializationReaderWriter()
            : this(null, new UTF8Encoding())
        {
        }

        public ContentSerializationReaderWriter(Stream stream)
            : this(stream, new UTF8Encoding())
        {
        }

        public ContentSerializationReaderWriter(Stream stream, Encoding encoding)
        {
            mStream = stream;
            SetEncoding();
        }
        #endregion // Constructor

        #region Dispose
        /// <summary>
        /// This is the dispose constructor.
        /// </summary>
        /// <param name="disposing">A boolean value specifying that the xlass is being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.BaseStream != null)
                    this.BaseStream.Close();
            }
        }
        #endregion // Dispose

        #region SetEncoding()
        /// <summary>
        /// This method sets the default string encoding for the header which is UTF8.
        /// </summary>
        protected virtual void SetEncoding()
        {
            mEncoding = new UTF8Encoding(false, true);
            mEncoder = mEncoding.GetEncoder();
            mDecoder = mEncoding.GetDecoder();
        }
        #endregion // SetEncoding()

        #region BaseStream
        /// <summary>
        /// This is the base stream that will be used when a stream is not specified.
        /// </summary>
        public virtual Stream BaseStream
        {
            get
            {
                return mStream;
            }
            set
            {
                mStream = value;
            }
        }
        #endregion // BaseStream

        #region FillBuffer
        /// <summary>
        /// This method fills the buffer with the required number of bytes. 
        /// The default buffer will be used.
        /// </summary>
        /// <param name="inStream">The stream.</param>
        /// <param name="numBytes">The number of bytes.</param>
        protected void FillBuffer(Stream inStream, int numBytes)
        {
            FillBuffer(inStream, numBytes, mBuffer);
        }
        /// <summary>
        /// This method fills the buffer with the required number of bytes,
        /// </summary>
        /// <param name="inStream">The stream.</param>
        /// <param name="numBytes">The number of bytes.</param>
        /// <param name="buffer">The buffer to use.</param>
        protected void FillBuffer(Stream inStream, int numBytes, byte[] buffer)
        {
            int num1 = 0;
            int num2 = 0;
            if (inStream == null)
            {
                throw new ArgumentNullException("Stream cannot be null.");
            }
            if (numBytes == 1)
            {
                num2 = inStream.ReadByte();
                if (num2 == -1)
                {
                    throw new EndOfStreamException("The end of the stream has been reached.");
                }
                buffer[0] = (byte)num2;
            }
            else
            {
                do
                {
                    num2 = inStream.Read(buffer, num1, numBytes - num1);
                    if (num2 == 0)
                    {
                        throw new EndOfStreamException("The end of the stream has been reached.");
                    }
                    num1 += num2;
                }
                while (num1 < numBytes);
            }
        }
        #endregion // FillBuffer

        #region Close()
        /// <summary>
        /// This method closes the object.
        /// </summary>
        public virtual void Close()
        {
            this.Dispose(true);
        }
        #endregion // Close()
        #region Flush()
        /// <summary>
        /// This method flushes the stream.
        /// </summary>
        public virtual void Flush()
        {
            if (BaseStream != null)
                BaseStream.Flush();
        }
        #endregion // Flush()

        #region WriteCompressedBlob
        public void WriteCompressedBlob(byte[] value)
        {
            WriteCompressedBlob(BaseStream, value);
        }
        public void WriteCompressedBlob(Stream outStream, byte[] value)
        {
            byte[] zipByte = null;

            MemoryStream memStream = new MemoryStream();
            // Use the newly created memory stream for the compressed data.
            GZipStream compressedzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            compressedzipStream.Write(value, 0, value.Length);
            // Close the stream.
            compressedzipStream.Close();
            memStream.Position = 0;

            long length = memStream.Length;
            zipByte = new byte[length];

            Array.Copy(memStream.GetBuffer(), 0, zipByte, 0, length);
            memStream.Close();

            WriteBlob(outStream, zipByte);
            //Write out the original length of the stream, this will help when reading in.
            Write(outStream, value.Length);
        }
        #endregion // WriteCompressedBlob
        #region ReadCompressedBlob
        public byte[] ReadCompressedBlob()
        {
            return ReadCompressedBlob(BaseStream, true, -1);
        }
        public byte[] ReadCompressedBlob(int MaxLength)
        {
            return ReadCompressedBlob(BaseStream, true, MaxLength);
        }
        public byte[] ReadCompressedBlob(bool verifyLength)
        {
            return ReadCompressedBlob(BaseStream, verifyLength, -1);
        }
        public byte[] ReadCompressedBlob(bool verifyLength, int MaxLength)
        {
            return ReadCompressedBlob(BaseStream, verifyLength, MaxLength);
        }
        public byte[] ReadCompressedBlob(Stream inStream)
        {
            return ReadCompressedBlob(inStream, true, -1);
        }
        public byte[] ReadCompressedBlob(Stream inStream, bool verifyLength)
        {
            return ReadCompressedBlob(inStream, verifyLength, -1);
        }
        public byte[] ReadCompressedBlob(Stream inStream, bool verifyLength, int MaxLength)
        {
            byte[] compressedBlob = ReadBlob(inStream, verifyLength, MaxLength);
            int originalLength = this.ReadInt(inStream);
            byte[] buffer = new byte[originalLength];

            using (MemoryStream memStream = new MemoryStream(compressedBlob))
            {
                using (GZipStream zipStream = new GZipStream(memStream, CompressionMode.Decompress, false))
                {
                    int bytesRead = zipStream.Read(buffer, 0, buffer.Length);

                    zipStream.Close();
                }
                memStream.Close();
            }

            return buffer;
        }
        #endregion // ReadCompressedBlob

        #region WriteBlob
        public void WriteBlob(byte[] value)
        {
            WriteBlob(BaseStream, value);
        }
        public void WriteBlob(Stream outStream, byte[] value)
        {
            Write(outStream, value.Length);
            outStream.Write(value, 0, value.Length);
        }
        #endregion // WriteBlob
        #region ReadBlob
        public byte[] ReadBlob()
        {
            return ReadBlob(BaseStream, true, -1);
        }
        public byte[] ReadBlob(int MaxLength)
        {
            return ReadBlob(BaseStream, true, MaxLength);
        }
        public byte[] ReadBlob(bool verifyLength)
        {
            return ReadBlob(BaseStream, verifyLength, -1);
        }
        public byte[] ReadBlob(bool verifyLength, int MaxLength)
        {
            return ReadBlob(BaseStream, verifyLength, MaxLength);
        }
        public byte[] ReadBlob(Stream inStream)
        {
            return ReadBlob(inStream, true, -1);
        }
        public byte[] ReadBlob(Stream inStream, bool verifyLength)
        {
            return ReadBlob(inStream, verifyLength, -1);
        }
        public byte[] ReadBlob(Stream inStream, bool verifyLength, int MaxLength)
        {
            int length = ReadInt(inStream);

            if (MaxLength > 0 && length > MaxLength)
                throw new IOException("The length is longer than the maximum permitted.");
            //Verify that the stream has the necessary data before creating
            //the byte buffer.
            if (verifyLength && inStream.Length < length)
                throw new IOException("Stream does not have enough data to read the stream.");

            byte[] blob = new byte[length];

            FillBuffer(inStream, length, blob);

            return blob;
        }
        #endregion // Blob

        #region Guid
        public void Write(Guid value)
        {
            Write(BaseStream, value);
        }

        public void Write(Stream outStream, Guid value)
        {
            outStream.Write(value.ToByteArray(), 0, 16);
        }

        public Guid ReadGuid()
        {
            return ReadGuid(BaseStream);
        }

        public Guid ReadGuid(Stream inStream)
        {
            FillBuffer(inStream, 16);
            return new Guid(mBuffer);
        }
        #endregion // Guid
        #region Bool
        public void Write(bool value)
        {
            Write(BaseStream, value);
        }

        public void Write(Stream outStream, bool value)
        {
            outStream.WriteByte((byte)(value ? 1 : 0));
        }

        public bool ReadBool()
        {
            return ReadBool(BaseStream);
        }

        public bool ReadBool(Stream inStream)
        {
            FillBuffer(inStream, 1);
            return mBuffer[0] > 0;
        }
        #endregion // Bool

        #region Int8
        public void Write(byte value)
        {
            Write(BaseStream, value);
        }
        public void Write(Stream outStream, byte value)
        {
            outStream.WriteByte(value);
        }

        public byte ReadByte()
        {
            return ReadByte(BaseStream);
        }
        public byte ReadByte(Stream inStream)
        {
            return (byte)inStream.ReadByte();
        }
        #endregion // Short
        #region Int16
        public void Write(short value)
        {
            Write(BaseStream, value);
        }
        public void Write(Stream outStream, short value)
        {
            mBuffer[0] = (byte)value;
            mBuffer[1] = (byte)(value >> 8);
            outStream.Write(mBuffer, 0, 2);
        }

        public short ReadShort()
        {
            return ReadShort(BaseStream);
        }
        public short ReadShort(Stream inStream)
        {
            FillBuffer(inStream, 2);
            return (short)((mBuffer[0]) | (mBuffer[1] << 8));
        }
        #endregion // Short
        #region Int32
        public void Write(int value)
        {
            Write(BaseStream, value);
        }
        public void Write(Stream outStream, int value)
        {
            mBuffer[0] = (byte)value;
            mBuffer[1] = (byte)(value >> 8);
            mBuffer[2] = (byte)(value >> 0x10);
            mBuffer[3] = (byte)(value >> 0x18);
            outStream.Write(mBuffer, 0, 4);
        }

        public int ReadInt()
        {
            return ReadInt(BaseStream);
        }
        public int ReadInt(Stream inStream)
        {
            FillBuffer(inStream, 4);
            return (int)((((mBuffer[0] & 0xff) | (mBuffer[1] << 8)) | (mBuffer[2] << 0x10)) | (mBuffer[3] << 0x18));
        }
        #endregion // int
        #region Int64
        public void Write(long value)
        {
            Write(BaseStream, value);
        }

        public void Write(Stream outStream, long value)
        {
            mBuffer[0] = (byte)value;
            mBuffer[1] = (byte)(value >> 8);
            mBuffer[2] = (byte)(value >> 16);
            mBuffer[3] = (byte)(value >> 24);
            mBuffer[4] = (byte)(value >> 32);
            mBuffer[5] = (byte)(value >> 40);
            mBuffer[6] = (byte)(value >> 48);
            mBuffer[7] = (byte)(value >> 56);
            outStream.Write(mBuffer, 0, 8);
        }

        public long ReadLong()
        {
            return ReadLong(BaseStream);
        }

        public long ReadLong(Stream inStream)
        {
            FillBuffer(inStream, 8);
            uint a = (uint)(((mBuffer[0] | (mBuffer[1] << 8)) | (mBuffer[2] << 16)) | (mBuffer[3] << 24));
            uint b = (uint)(((mBuffer[4] | (mBuffer[5] << 8)) | (mBuffer[6] << 16)) | (mBuffer[7] << 24));
            return (long)(((long)b << 32) | a);
        }
        #endregion // int
        #region uint
        [CLSCompliant(false)]
        public void Write(uint value)
        {
            Write(BaseStream, value);
        }
        [CLSCompliant(false)]
        public void Write(Stream outStream, uint value)
        {
            mBuffer[0] = (byte)value;
            mBuffer[1] = (byte)(value >> 8);
            mBuffer[2] = (byte)(value >> 0x10);
            mBuffer[3] = (byte)(value >> 0x18);
            outStream.Write(mBuffer, 0, 4);
        }

        [CLSCompliant(false)]
        public uint ReadUint()
        {
            return ReadUint(BaseStream);
        }
        [CLSCompliant(false)]
        public uint ReadUint(Stream inStream)
        {
            FillBuffer(inStream, 4);
            return (uint)((((mBuffer[0] & 0xff) | (mBuffer[1] << 8)) | (mBuffer[2] << 0x10)) | (mBuffer[3] << 0x18));
        }
        #endregion // unit
        #region string

        public virtual string ReadString()
        {
            return ReadString(BaseStream, -1);
        }
        public virtual string ReadString(int MaxLength)
        {
            return ReadString(BaseStream, MaxLength);
        }
        public virtual string ReadString(Stream inStream)
        {
            return ReadString(inStream, -1);
        }
        public virtual string ReadString(Stream inStream, int MaxLength)
        {
            int num1 = 0;
            if (inStream == null)
                throw new ArgumentNullException("Stream cannot be null.");

            int num3 = this.ReadInt(inStream);

            if (num3 < 0)
                throw new IOException("Invalid read format for the integer.");

            if (MaxLength > 0 && num3 > MaxLength)
                throw new IOException("The string is longer than the max length.");

            if (num3 == 0)
                return string.Empty;

            byte[] m_charBytes = new byte[0x80];
            char[] m_charBuffer = new char[0x80];

            StringBuilder builder1 = null;
            while (true)
            {
                int num4 = ((num3 - num1) > 0x80) ? 0x80 : (num3 - num1);
                int num2 = inStream.Read(m_charBytes, 0, num4);
                if (num2 == 0)
                {
                    throw new EndOfStreamException("The stream cannot be read.");
                }
                int num5 = mDecoder.GetChars(m_charBytes, 0, num2, m_charBuffer, 0);
                if ((num1 == 0) && (num2 == num3))
                {
                    return new string(m_charBuffer, 0, num5);
                }
                if (builder1 == null)
                {
                    builder1 = new StringBuilder(num3);
                }
                builder1.Append(m_charBuffer, 0, num5);
                num1 += num2;
                if (num1 >= num3)
                {
                    return builder1.ToString();
                }
            }
        }

        public void Write(string value)
        {
            Write(BaseStream, value);
        }
        public virtual void Write(Stream outStream, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("The string value cannot be null.");
            }
            if (outStream == null)
            {
                throw new ArgumentNullException("The outStream value cannot be null.");
            }

            int strByteLength = mEncoding.GetByteCount(value);
            this.Write(outStream, strByteLength);
            if (mLargeByteBuffer == null)
            {
                mLargeByteBuffer = new byte[0x100];
                mMaxChars = 0x100 / mEncoding.GetMaxByteCount(1);
            }
            if (strByteLength <= 0x100)
            {
                mEncoding.GetBytes(value, 0, value.Length, mLargeByteBuffer, 0);
                outStream.Write(mLargeByteBuffer, 0, strByteLength);
            }
            else
            {
                int num4;
                char[] chArray1 = value.ToCharArray();
                int num2 = 0;
                for (int num3 = value.Length; num3 > 0; num3 -= num4)
                {
                    num4 = (num3 > mMaxChars) ? mMaxChars : num3;
                    int num5 = mEncoder.GetBytes(chArray1, num2, num4, mLargeByteBuffer, 0, num4 == num3);
                    outStream.Write(mLargeByteBuffer, 0, num5);
                    num2 += num4;
                }
            }
        }
        #endregion // string
        #region Date
        public void Write(DateTime value)
        {
            Write(BaseStream, value);
        }

        public void Write(Stream outStream, DateTime value)
        {
            long data = value.Ticks;
            Write(outStream, data);
        }

        public DateTime ReadDateTime()
        {
            return ReadDateTime(BaseStream);
        }

        public DateTime ReadDateTime(Stream inStream)
        {
            long item = ReadLong(inStream);
            return new DateTime(item);
        }
        #endregion
    }
}
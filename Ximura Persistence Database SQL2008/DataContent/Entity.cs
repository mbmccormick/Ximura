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
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
#endregion // using
namespace Ximura.Data.SQL
{
    public struct Entity
    {
        #region Declarations
        private readonly static byte[] sPurchaseSchemaBuffer;

        private readonly static Entity mEmpty;

        public bool isEmpty;
        public uint bitmap;
        public int exCap;
        public string name;
        public Guid tid;
        public Guid cid;
        public Guid vid;
        public DateTime mUTCWriteTime;
        public string mcType;
        public string mCBaseType;
        public bool contentDirty;
        public int bodyCount;

        #endregion // Declarations

        #region Static Constructor
        public static Entity Empty
        {
            get
            {
                return mEmpty;
            }
        }

        static Entity()
        {
            mEmpty = new Entity();
            mEmpty.isEmpty = true;

            using (Stream strmConfig = typeof(Entity).Assembly.GetManifestResourceStream(
                "Carrefour.Loyalty.SQL.DataSet.PurchaseTransaction_DataSet.xsd"))
            {

                if (strmConfig == null)
                {
                    sPurchaseSchemaBuffer = null;
                    return;
                }

                sPurchaseSchemaBuffer = new byte[strmConfig.Length];

                strmConfig.Read(sPurchaseSchemaBuffer, 0, sPurchaseSchemaBuffer.Length);
            }

        }
        #endregion // Static Constructor
        #region Constructors
        public Entity(byte[] blob)
            : this(blob, false)
        {
        }

        public Entity(Stream ms)
            : this(ms, false)
        {
        }

        public Entity(byte[] blob, bool returnBody)
        {
            isEmpty = false;
            using (MemoryStream ms = new MemoryStream(blob))
            {
                SetBodyParameters(ms, returnBody,
                    out bitmap,
                    out exCap,
                    out name,
                    out tid,
                    out cid,
                    out vid,
                    out mUTCWriteTime,
                    out mcType,
                    out mCBaseType,
                    out contentDirty,
                    out bodyCount);

                ms.Close();
            }
        }

        public Entity(Stream ms, bool returnBody)
        {
            isEmpty = false;
            SetBodyParameters(ms, returnBody,
                    out bitmap,
                    out exCap,
                    out name,
                    out tid,
                    out cid,
                    out vid,
                    out mUTCWriteTime,
                    out mcType,
                    out mCBaseType,
                    out contentDirty,
                    out bodyCount);
        }

        private static void SetBodyParameters(Stream ms, bool returnBody,
                out uint bitmap,
                out int exCap,
                out string name,
                out Guid tid,
                out Guid cid,
                out Guid vid,
                out DateTime mUTCWriteTime,
                out string mcType,
                out string mCBaseType,
                out bool contentDirty,
                out int bodyCount
            )
        {
            if (ms.ReadByte() != 0xD8 || ms.ReadByte() != 0xB4)
                throw new ArgumentException("Byte stream is not valid.");

            bitmap = ReadUint(ms);

            exCap = HeaderGetBits(bitmap, 4, 28);

            if (exCap > 0)
                ms.Position += exCap;

            if (HeaderGetBit(bitmap, 1))
                name = ReadString(ms, -1);
            else
                name = null;

            tid = ReadGuid(ms);
            cid = ReadGuid(ms);
            vid = ReadGuid(ms);

            if (HeaderGetBit(bitmap, 0))
                mUTCWriteTime = ReadDateTime(ms);
            else
                mUTCWriteTime = DateTime.MinValue;

            mcType = ReadString(ms, -1);

            if (HeaderGetBit(bitmap, 4))
                mCBaseType = ReadString(ms, -1);
            else
                mCBaseType = null;

            //Add the dirty status
            contentDirty = ms.ReadByte() > 0;

            //Add the body count
            bodyCount = ReadInt(ms);

        }


        #endregion // Constructors

        #region Static Helper Methods
        private static DataSet ParseBody(uint bitmap, Stream ms)
        {
            DataSet mDataContentSet = new DataSet();
            if (sPurchaseSchemaBuffer != null)
            {
                using (MemoryStream ms2 = new MemoryStream(sPurchaseSchemaBuffer))
                {
                    mDataContentSet.ReadXmlSchema(ms2);
                }
            }

            mDataContentSet.EnforceConstraints = false;

            using (Stream loadStream = GetDataStream(ms, HeaderGetBit(bitmap, 2)))
            {
                mDataContentSet.ReadXml(loadStream);
            }

            return mDataContentSet;
        }

        private static Stream GetDataStream(Stream inStream, bool compressed)
        {
            int length = ReadInt(inStream);

            if (inStream.Length < length)
                throw new IOException("Stream does not have enough data to read the stream: [" + inStream.Length.ToString()
                    + "]-[" + length.ToString() + "]");

            if (!compressed)
                return new BufferedStream(inStream);

            return new GZipStream(inStream, CompressionMode.Decompress, false);
        }

        //public static byte[] ReadCompressedBlob(Stream inStream, bool verifyLength, int MaxLength)
        //{
        //    byte[] compressedBlob = ReadBlob(inStream, verifyLength, MaxLength);
        //    int originalLength = ReadInt(inStream);
        //    byte[] buffer = new byte[originalLength];

        //    using (MemoryStream memStream = new MemoryStream(compressedBlob))
        //    {
        //        using (GZipStream zipStream = new GZipStream(memStream, CompressionMode.Decompress, false))
        //        {
        //            int bytesRead = zipStream.Read(buffer, 0, buffer.Length);
        //            zipStream.Close();
        //        }
        //    }
        //    return buffer;
        //}

        //public static byte[] ReadBlob(Stream inStream, bool verifyLength, int MaxLength)
        //{
        //    int length = ReadInt(inStream);

        //    if (MaxLength > 0 && length > MaxLength)
        //        throw new IOException("The length is longer than the maximum permitted.");
        //    //Verify that the stream has the necessary data before creating
        //    //the byte buffer.
        //    if (verifyLength && inStream.Length < length)
        //        throw new IOException("Stream does not have enough data to read the stream: [" + inStream.Length.ToString() 
        //            + "]-[" + length.ToString() + "]");

        //    byte[] blob = new byte[length];

        //    FillBuffer(inStream, length, blob);

        //    return blob;
        //}

        /// <summary>
        /// This method fills the buffer with the required number of bytes,
        /// </summary>
        /// <param name="inStream">The stream.</param>
        /// <param name="numBytes">The number of bytes.</param>
        /// <param name="buffer">The buffer to use.</param>
        private static void FillBuffer(Stream inStream, int numBytes, byte[] buffer)
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

        public static uint ReadUint(Stream inStream)
        {
            byte[] mBuffer = new byte[4];
            inStream.Read(mBuffer, 0, 4);
            return (uint)((((mBuffer[0] & 0xff) | (mBuffer[1] << 8)) | (mBuffer[2] << 0x10)) | (mBuffer[3] << 0x18));
        }

        public static Guid ReadGuid(Stream inStream)
        {
            byte[] mBuffer = new byte[16];
            inStream.Read(mBuffer, 0, 16);
            return new Guid(mBuffer);
        }

        public static int ReadInt(Stream inStream)
        {
            return (int)((((inStream.ReadByte() & 0xff) | (inStream.ReadByte() << 8)) | (inStream.ReadByte() << 0x10)) | (inStream.ReadByte() << 0x18));
        }

        public static string ReadString(Stream inStream, int MaxLength)
        {
            int num1 = 0;
            if (inStream == null)
                throw new ArgumentNullException("Stream cannot be null.");

            int num3 = ReadInt(inStream);

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
                int num5 = Encoding.UTF8.GetChars(m_charBytes, 0, num2, m_charBuffer, 0);
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

        /// <summary>
        /// The method gets a range of bytes from the header.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="bitMarker"></param>
        /// <returns></returns>
        public static int HeaderGetBits(uint headerUint, int range, int bitMarker)
        {
            uint moved = (headerUint >> bitMarker) & (uint)range;

            return (int)moved;
        }
        /// <summary>
        /// This method gets the boolean value from the header bit.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <returns>Returns the boolean value for the bit.</returns>
        public static bool HeaderGetBit(uint headerUint, int bit)
        {
            return HeaderGetBits(headerUint, 1, bit + 19) > 0;
        }

        public static long ReadLong(Stream inStream)
        {
            byte[] mBuffer = new byte[8];
            inStream.Read(mBuffer, 0, 8);
            uint a = (uint)(((mBuffer[0] | (mBuffer[1] << 8)) | (mBuffer[2] << 16)) | (mBuffer[3] << 24));
            uint b = (uint)(((mBuffer[4] | (mBuffer[5] << 8)) | (mBuffer[6] << 16)) | (mBuffer[7] << 24));
            return (long)(((long)b << 32) | a);
        }

        public static DateTime ReadDateTime(Stream inStream)
        {
            long item = ReadLong(inStream);
            return new DateTime(item);
        }
        #endregion // Static Helper Methods
    }
}

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
    [CLSCompliant(false)]
    public static partial class StreamHelper
    {
        #region bool
        public static bool ReadBool(Stream str)
        {
            return str.ReadByte() != 0;
        }

        public static void Write(Stream str, bool value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // bool

        #region byte
        public static byte ReadByte(Stream str)
        {
            return (byte)str.ReadByte();
        }

        public static void Write(Stream str, byte value)
        {
            str.WriteByte(value);
        }
        #endregion // byte

        #region sbyte
        public static sbyte ReadSByte(Stream str)
        {
            return (sbyte)str.ReadByte();
        }

        public static void Write(Stream str, sbyte value)
        {
            str.WriteByte((byte)value);
        }
        #endregion // sbyte

        #region short
        public static short ReadInt16(Stream str)
        {
            return (short)(str.ReadByte() | (str.ReadByte() << 8));
        }

        public static void Write(Stream str, short value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // short

        #region ushort
        public static ushort ReadUInt16(Stream str)
        {
            return (ushort)(str.ReadByte() | (str.ReadByte() << 8));
        }

        public static void Write(Stream str, ushort value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // ushort

        #region int
        public static int ReadInt32(Stream str)
        {
            return (int)(str.ReadByte() | (str.ReadByte() << 8) | (str.ReadByte() << 0x10) | (str.ReadByte() << 0x18));
        }

        public static void Write(Stream str, int value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // int

        #region uint
        public static uint ReadUInt32(Stream str)
        {
            return (uint)ReadInt32(str);
        }

        public static void Write(Stream str, uint value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // uint

        #region long
        public static long ReadInt64(Stream str)
        {
            byte[] data = new byte[8];
            str.Read(data, 0, 8);
            return BitConverter.ToInt64(data, 0);
        }

        public static void Write(Stream str, long value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // long

        #region ulong
        public static ulong ReadUInt64(Stream str)
        {
            byte[] data = new byte[8];
            str.Read(data, 0, 8);
            return BitConverter.ToUInt64(data, 0);
        }

        public static void Write(Stream str, ulong value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // ulong

        #region float / single
        public static float ReadSingle(Stream str)
        {
            byte[] single = new byte[4];
            str.Read(single, 0, 4);
            return BitConverter.ToSingle(single,0);
        }

        public static void Write(Stream str, float value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // float / single

        #region double
        public static double ReadDouble(Stream str)
        {
            byte[] single = new byte[8];
            str.Read(single, 0, 8);
            return BitConverter.ToDouble(single, 0);
        }

        public static void Write(Stream str, double value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // double

        #region decimal
        public static decimal ReadDecimal(Stream str)
        {
            int[] bits = new int[4];

            bits[0] = ReadInt32(str);//lo
            bits[1] = ReadInt32(str);//mid
            bits[2] = ReadInt32(str);//hi
            bits[3] = ReadInt32(str);//flags

            return new decimal(bits);
        }


        public static void Write(Stream str, decimal value)
        {
            int[] bits = decimal.GetBits(value);
            bits.ForEach(i => Write(str, i));
        }
        #endregion // decimal

        #region char
        public static char ReadChar(Stream str)
        {
            return (char)ReadInt16(str);
        }

        public static void Write(Stream str, char value)
        {
            byte[] blob = BitConverter.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }
        #endregion // char

        #region string
        public static string ReadString(Stream str)
        {
            return ReadString(str, Encoding.UTF8);
        }

        public static string ReadString(Stream str, Encoding encoding)
        {
            int capacity = Read7BitEncodedInt(str);

            byte[] blob = new byte[capacity];

            if (str.Read(blob, 0, capacity) != capacity)
                throw new FormatException("Stream has been truncated.");

            return encoding.GetString(blob,0,capacity);
        }

        public static void Write(Stream str, string value)
        {
            Write(str, value, Encoding.UTF8);
        }

        public static void Write(Stream str, string value, Encoding encoding)
        {
            int byteCount = encoding.GetByteCount(value);
            Write7BitEncodedInt(str, byteCount);

            byte[] blob = encoding.GetBytes(value);
            str.Write(blob, 0, blob.Length);
        }

        private static void Write7BitEncodedInt(Stream str, int value)
        {
            uint num = (uint)value;
            while (num >= 0x80)
            {
                Write(str, (byte)(num | 0x80));
                num = num >> 7;
            }
            Write(str, (byte)num);
        }

        private static int Read7BitEncodedInt(Stream str)
        {
            byte num3;
            int num = 0;
            int num2 = 0;
            do
            {
                if (num2 == 0x23)
                {
                    throw new FormatException("Format_Bad7BitInt32");
                }
                num3 = ReadByte(str);
                num |= (num3 & 0x7f) << num2;
                num2 += 7;
            }
            while ((num3 & 0x80) != 0);

            return num;
        }
        #endregion // string
    }
}

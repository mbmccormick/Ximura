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
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    public static partial class LinqHelper
    {
        #region StreamRead<T>(this Stream str)
        /// <summary>
        /// This method reads a set of items from a stream, based on the type parameter.
        /// </summary>
        /// <typeparam name="T">The type to read from the stream and to return in the enumeration.</typeparam>
        /// <param name="str">The stream to read from.</param>
        /// <returns>Returns an enumeration of the items deserialized from the stream.</returns>
        public static IEnumerable<T> StreamRead<T>(this Stream str)
        {
            Func<Stream, T> conv = null;

            if (typeof(T) == typeof(int))
                conv = (s) => (T)(object)StreamHelper.ReadInt32(s);
            else if (typeof(T) == typeof(uint))
                conv = (s) => (T)(object)StreamHelper.ReadUInt32(s);
            else if (typeof(T) == typeof(short))
                conv = (s) => (T)(object)StreamHelper.ReadInt16(s);
            else if (typeof(T) == typeof(UInt16))
                conv = (s) => (T)(object)StreamHelper.ReadUInt16(s);
            else if (typeof(T) == typeof(sbyte))
                conv = (s) => (T)(object)StreamHelper.ReadSByte(s);
            else if (typeof(T) == typeof(byte))
                conv = (s) => (T)(object)StreamHelper.ReadByte(s);
            else if (typeof(T) == typeof(long))
                conv = (s) => (T)(object)StreamHelper.ReadInt64(s);
            else if (typeof(T) == typeof(UInt64))
                conv = (s) => (T)(object)StreamHelper.ReadUInt64(s);
            else if (typeof(T) == typeof(double))
                conv = (s) => (T)(object)StreamHelper.ReadDouble(s);
            else if (typeof(T) == typeof(decimal))
                conv = (s) => (T)(object)StreamHelper.ReadDecimal(s);
            else if (typeof(T) == typeof(float))
                conv = (s) => (T)(object)StreamHelper.ReadSingle(s);
            else if (typeof(T) == typeof(bool))
                conv = (s) => (T)(object)StreamHelper.ReadBool(s);
            else if (typeof(T) == typeof(char))
                conv = (s) => (T)(object)StreamHelper.ReadChar(s);
            else if (typeof(T) == typeof(string))
                conv = (s) => (T)(object)StreamHelper.ReadString(s);
            else
                throw new NotSupportedException(typeof(T).Name + " is not supported.");

            return str.StreamRead<T>(conv);
        }
        #endregion
        #region StreamRead<T>(this Stream str, Func<BinaryReader, T> conv)
        /// <summary>
        /// This method reads a set of items from a stream, based on the type parameter.
        /// </summary>
        /// <typeparam name="T">The type to read from the stream and to return in the enumeration.</typeparam>
        /// <param name="str">The stream to read from.</param>
        /// <param name="conv">The conversion function to read from the stream.</param>
        /// <returns>Returns an enumeration of the items deserialized from the stream.</returns>
        public static IEnumerable<T> StreamRead<T>(this Stream str, Func<Stream, T> conv)
        {
            if (str == null) throw new ArgumentNullException("str", "The stream cannot be null.");
            if (conv == null) throw new ArgumentNullException("conv", "The conv function cannot be null.");

            while (str.CanRead && (!str.CanSeek || (str.Position < str.Length)))
            {
                yield return conv(str);
            }
        }
        #endregion

        #region StreamWrite<T>(this IEnumerable<T> coll, Stream str)
        /// <summary>
        /// This extension method writes a collection to the stream.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="coll">The collection.</param>
        /// <param name="str">The stream.</param>
        public static void StreamWrite<T>(this IEnumerable<T> coll, Stream str)
        {
            Action<Stream, T> conv = null;

            if (typeof(T) == typeof(int))
                conv = (stm, s) => StreamHelper.Write(stm, (int)(object)s);
            else if (typeof(T) == typeof(uint))
                conv = (stm, s) => StreamHelper.Write(stm, (uint)(object)s);
            else if (typeof(T) == typeof(short))
                conv = (stm, s) => StreamHelper.Write(stm, (short)(object)s);
            else if (typeof(T) == typeof(UInt16))
                conv = (stm, s) => StreamHelper.Write(stm, (UInt16)(object)s);
            else if (typeof(T) == typeof(sbyte))
                conv = (stm, s) => StreamHelper.Write(stm, (sbyte)(object)s);
            else if (typeof(T) == typeof(byte))
                conv = (stm, s) => StreamHelper.Write(stm, (byte)(object)s);
            else if (typeof(T) == typeof(long))
                conv = (stm, s) => StreamHelper.Write(stm, (long)(object)s);
            else if (typeof(T) == typeof(UInt64))
                conv = (stm, s) => StreamHelper.Write(stm, (UInt64)(object)s);
            else if (typeof(T) == typeof(double))
                conv = (stm, s) => StreamHelper.Write(stm, (double)(object)s);
            else if (typeof(T) == typeof(decimal))
                conv = (stm, s) => StreamHelper.Write(stm, (decimal)(object)s);
            else if (typeof(T) == typeof(float))
                conv = (stm, s) => StreamHelper.Write(stm, (float)(object)s);
            else if (typeof(T) == typeof(bool))
                conv = (stm, s) => StreamHelper.Write(stm, (bool)(object)s);
            else if (typeof(T) == typeof(char))
                conv = (stm, s) => StreamHelper.Write(stm, (char)(object)s);
            else if (typeof(T) == typeof(string))
                conv = (stm, s) => StreamHelper.Write(stm, (string)(object)s);
            else
                throw new NotSupportedException(typeof(T).Name + " is not supported.");

            coll.StreamWrite(str, conv);
        }
        #endregion // StreamWrite<T>(this IEnumerable<T> coll, Stream str)
        #region StreamWrite<T>(this IEnumerable<T> coll, Stream str, Action<Stream, T> act)
        /// <summary>
        /// This extension method writes a collection to the stream.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="coll">The collection.</param>
        /// <param name="str">The stream.</param>
        /// <param name="act">The action that writes the data to the stream.</param>
        public static void StreamWrite<T>(this IEnumerable<T> coll, Stream str, Action<Stream, T> act)
        {
            if (coll == null) throw new ArgumentNullException("coll", "The collection cannot be null.");
            if (str == null) throw new ArgumentNullException("str", "The stream cannot be null.");
            if (act == null) throw new ArgumentNullException("conv", "The conv action cannot be null.");

            coll.ForEach(i => act(str, i));
        }
        #endregion // StreamWrite<T>(this IEnumerable<T> coll, Stream str, Action<Stream, T> act)
    }
}

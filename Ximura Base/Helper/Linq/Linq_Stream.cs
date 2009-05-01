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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Helper
{
    public static partial class LinqHelper
    {
        #region StreamFrom<T>(this Stream str)
        /// <summary>
        /// This method reads a set of items from a stream, based on the type parameter.
        /// </summary>
        /// <typeparam name="T">The type to read from the stream and to return in the enumeration.</typeparam>
        /// <param name="str">The stream to read from.</param>
        /// <returns>Returns an enumeration of the items deserialized from the stream.</returns>
        public static IEnumerable<T> StreamFrom<T>(this Stream str)
        {
            Func<BinaryReader, T> conv = null;

            if (typeof(T) == typeof(int))
                conv = (s) => (T)(object)(s.ReadInt32());
            else if (typeof(T) == typeof(uint))
                conv = (s) => (T)(object)(s.ReadUInt32());
            else if (typeof(T) == typeof(short))
                conv = (s) => (T)(object)(s.ReadInt16());
            else if (typeof(T) == typeof(UInt16))
                conv = (s) => (T)(object)(s.ReadUInt16());
            else if (typeof(T) == typeof(sbyte))
                conv = (s) => (T)(object)(s.ReadSByte());
            else if (typeof(T) == typeof(byte))
                conv = (s) => (T)(object)(s.ReadByte());
            else if (typeof(T) == typeof(long))
                conv = (s) => (T)(object)(s.ReadInt64());
            else if (typeof(T) == typeof(UInt64))
                conv = (s) => (T)(object)(s.ReadUInt64());
            else if (typeof(T) == typeof(double))
                conv = (s) => (T)(object)(s.ReadDouble());
            else if (typeof(T) == typeof(decimal))
                conv = (s) => (T)(object)(s.ReadDecimal());
            else if (typeof(T) == typeof(float))
                conv = (s) => (T)(object)(s.ReadSingle());
            else if (typeof(T) == typeof(bool))
                conv = (s) => (T)(object)(s.ReadBoolean());
            else if (typeof(T) == typeof(char))
                conv = (s) => (T)(object)(s.ReadChar());
            else if (typeof(T) == typeof(string))
                conv = (s) => (T)(object)(s.ReadString());
            else
                throw new NotSupportedException(typeof(T).Name + " is not supported.");

            return str.StreamFrom<T>(conv);
        }
        #endregion // IEnumerable<T> StreamFrom<T>(this Stream str)

        #region StreamFrom<T>(this Stream str, Func<BinaryReader, T> conv)
        /// <summary>
        /// This method reads a set of items from a stream, based on the type parameter.
        /// </summary>
        /// <typeparam name="T">The type to read from the stream and to return in the enumeration.</typeparam>
        /// <param name="str">The stream to read from.</param>
        /// <param name="conv">The conversion function to read from the stream.</param>
        /// <returns>Returns an enumeration of the items deserialized from the stream.</returns>
        public static IEnumerable<T> StreamFrom<T>(this Stream str, Func<BinaryReader, T> conv)
        {
            str.Position = 0;
            BinaryReader br = new BinaryReader(str);

            while (str.CanRead && str.Position<str.Length)
            {
                yield return conv(br);
            }
        }
        #endregion // StreamFrom<T>(this Stream str, Func<BinaryReader, T> conv)
    }
}

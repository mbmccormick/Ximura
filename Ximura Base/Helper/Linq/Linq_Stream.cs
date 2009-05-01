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
        public static IEnumerable<T> StreamFrom<T>(this Stream str)
        {
            Func<Stream, T> conv = null;

            if (typeof(T) == typeof(int))
                conv = (s) => (T)(object)(((s.ReadByte() | (s.ReadByte() << 8)) | (s.ReadByte() << 0x10)) | (s.ReadByte() << 0x18));
            else if (typeof(T) == typeof(short))
                conv = (s) => (T)(object)(s.ReadByte() | (s.ReadByte() << 8));
            else if (typeof(T) == typeof(byte))
                conv = (s) => (T)(object)(s.ReadByte());
            else if (typeof(T) == typeof(long))
                conv = (s) =>
                    {
                        uint num = (uint)(((s.ReadByte() | (s.ReadByte() << 8)) | (s.ReadByte() << 0x10)) | (s.ReadByte() << 0x18));
                        uint num2 = (uint)(((s.ReadByte() | (s.ReadByte() << 8)) | (s.ReadByte() << 0x10)) | (s.ReadByte() << 0x18));
                        return (T)(object)((long)((num2 << 0x20) | num));
                    };
            else
                throw new NotSupportedException(typeof(T).Name + " is not supported.");

            return str.StreamFrom<T>(conv);
        }

        public static IEnumerable<T> StreamFrom<T>(this Stream str, Func<Stream, T> conv)
        {
            str.Position = 0;

            while (str.CanRead && str.Position<str.Length)
            {
                yield return conv(str);
            }
        }
    }
}

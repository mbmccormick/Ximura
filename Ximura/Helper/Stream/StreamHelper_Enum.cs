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
using System.Collections.Generic;


using Ximura;
#endregion // using
namespace Ximura
{
    public static partial class StreamHelper
    {
        #region Enum(this Stream data)
        /// <summary>
        /// This method turns a stream in to an enumerable byte array.
        /// </summary>
        /// <param name="data">The stream to read from.</param>
        /// <returns>Returns a stream of bytes from the stream.</returns>
        public static IEnumerable<byte> Enum(this Stream data)
        {
            if (data == null) throw new ArgumentNullException("The data stream cannot be null.");

            while (data.CanRead)
            {
                int item = data.ReadByte();
                if (item > -1)
                    yield return (byte)item;
                else
                    break;
            }
        }
        #endregion
        #region Enum(this Stream data, byte[] buffer, int start, int length)
        /// <summary>
        /// This method turns a stream in to an enumerable byte array. 
        /// This method also inserts an byte buffer before the stream in enumerated. This may be useful when reading from a readonly stream
        /// when bytes have already been read.
        /// </summary>
        /// <param name="data">The stream to read from.</param>
        /// <param name="buffer">A data buffer to insert before the stream.</param>
        /// <param name="start">The buffer offset.</param>
        /// <param name="length">The number of bytes to read from the buffer.</param>
        /// <returns>Returns a stream of bytes from the byte array and then the stream.</returns>
        public static IEnumerable<byte> Enum(this Stream data, byte[] buffer, int start, int length)
        {
            //Boundary checks.
            if (buffer == null) throw new ArgumentNullException("Buffer cannot be null.");
            if (start >= buffer.Length) throw new ArgumentOutOfRangeException("Start is after the end of the buffer array.");
            if (start + length >= buffer.Length) throw new ArgumentOutOfRangeException("The length specified is after the end of the buffer array.");

            //Loop through the byte array.
            for(int i = 0; i<length; i++)
                yield return buffer[start+i];

            //Now loop through the stream.
            foreach(var item in data.Enum())
                yield return item;
        }
        #endregion // Enum(this Stream data, byte[] buffer, int start, int length)
    }
}

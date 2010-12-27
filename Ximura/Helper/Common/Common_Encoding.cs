#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2010 Paul Stancer.
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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The <b>Common</b> class includes a number of useful utilities.
    /// </summary>
    public static partial class Common
    {
        #region Hex Encoding
        /// <summary>
        /// This function converts a binary array in to a hexadecimal string.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <returns>A string that represents the byte array as hexadecimal</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">This exception 
        /// is thrown if the offset and the length and greater than the 
        /// binary array length.</exception>
        public static string Enc_EncodeByteToHex(byte[] data)
        {
            return Enc_EncodeByteToHex(data, 0, data.Length, false);
        }
        /// <summary>
        /// This function converts a binary array in to a hexadecimal string.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <param name="offset">The starting point in the byte array.</param>
        /// <param name="length">The number of bytes to convert.</param>
        /// <returns>A string that represents the byte array as hexadecimal</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">This exception 
        /// is thrown if the offset and the length and greater than the 
        /// binary array length.</exception>
        public static string Enc_EncodeByteToHex(byte[] data, int offset, int length, bool lower)
        {
            if (data.Length < offset + length)
                throw new ArgumentOutOfRangeException("length", string.Format("Length [{0}] is too long.", length));

            StringBuilder output = new StringBuilder(length * 2);
            string id;
            if (lower)
                id = "x2";
            else
                id = "X2";

            for (int loopthru = 0; loopthru < length; loopthru++)
            {
                output.Append(data[offset + loopthru].ToString(id));
            }
            return output.ToString();

        }
        #endregion

        #region Hex Decoding
        /// <summary>
        /// This method decodes a hex string and inserts it into a byte array.
        /// </summary>
        /// <param name="HexString">The hex string that you wish to parse.</param>
        /// <param name="data">The byte array that you wish to be updated</param>
        /// <param name="offset">The point within the byte array that the method should start inserting encoded data.</param>
        /// <param name="length">The number of decoded bytes that the method should insert in the byte array.</param>
        /// <exception cref="System.ArgumentException">This exception is thrown if the hex 
        /// string is not divisible by 2. All strings should be left padded with 0 to ensure
        /// that they are divisible by 2.</exception>
        public static void Enc_DecodeHexAsByte(string HexString,
            byte[] data, int offset, int length)
        {
            if ((HexString.Length % 2) > 0)
                throw new ArgumentException("The Hex string must be divisible by 2");

            for (int strPoint = 0; strPoint < length; strPoint++)
            {
                byte temp = byte.Parse(HexString.Substring(strPoint * 2, 2),
                    NumberStyles.HexNumber);

                data[strPoint + offset] = temp;
            }
        }
        /// <summary>
        /// This method decodes a hex string and returns it as a byte array.
        /// </summary>
        /// <param name="HexString">The hex string to decode.</param>
        /// <returns>A byte array containing the bytes corresponding to the decoded string.</returns>
        /// <exception cref="System.ArgumentException">This exception is thrown if the hex 
        /// string is not divisible by 2. All strings should be left padded with 0 to ensure
        /// that they are divisible by 2.</exception>
        public static byte[] Enc_DecodeHexString(string HexString)
        {
            if ((HexString.Length % 2) > 0)
                throw new ArgumentException("The Hex string must be divisible by 2");

            int length = HexString.Length / 2;
            byte[] data = new byte[length];

            Enc_DecodeHexAsByte(HexString, data, 0, length);
            return data;
        }

        #endregion
    }
}

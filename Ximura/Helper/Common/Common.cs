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

#if (!SILVERLIGHT)
        #region Write Bytes Functions

        public static void WriteBytes(MemoryStream Message, string strData)
        {
            byte[] byData = Encoding.ASCII.GetBytes(strData);
            Message.Write(byData, 0, byData.Length);
        }

        public static void WriteBytes(MemoryStream Message, StringBuilder strbData)
        {
            WriteBytes(Message, strbData, true);
        }

        public static void WriteBytes(MemoryStream Message, StringBuilder strbData, bool blnClearSTRB)
        {
            if (strbData.Length > 0)
            {
                byte[] byData = Encoding.ASCII.GetBytes(strbData.ToString());
                Message.Write(byData, 0, byData.Length);
            }
            if (blnClearSTRB) strbData.Length = 0;
        }

        public static void WriteBytes(MemoryStream Message, byte[] byData)
        {
            Message.Write(byData, 0, byData.Length);
        }

        /// <summary>
        /// This converts a string in to an ASCII byte array. This method
        /// adds a CRLF at the end of the string by default.
        /// </summary>
        /// <param name="strData">The string you wish to convert.</param>
        /// <returns>A byte array containing an ASCII representation of the string.</returns>
        public static byte[] ASCByt(string strData)
        {
            return ASCByt(strData, true);
        }
        /// <summary>
        /// This converts a string in to an ASCII byte array.
        /// </summary>
        /// <param name="strData">The string you wish to convert.</param>
        /// <param name="blnAddCRLF">Select true if you want a new line appended at the end.</param>
        /// <returns>A byte array containing an ASCII representation of the string.</returns>
        public static byte[] ASCByt(string strData, bool blnAddCRLF)
        {
            if (blnAddCRLF) return Encoding.ASCII.GetBytes(strData + Environment.NewLine);

            return Encoding.ASCII.GetBytes(strData);
        }

        #endregion
#endif




        #region IsNumeric(string value)
        /// <summary>
        /// This method tests whether a string is numeric.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            if (value == null || value == "")
                return false;

            double tryIt;
            return double.TryParse(value, out tryIt);
        }



        #endregion // Helper Methods



    }
}


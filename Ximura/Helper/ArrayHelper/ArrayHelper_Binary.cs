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
    public static partial class ArrayHelper
    {
        private static int FindCharCaseInsensitive(byte[] byData, byte byteSearch, int intStart)
        {
            return FindCharCaseInsensitive(byData,byteSearch, intStart, false);
        }

        private static int FindCharCaseInsensitive(byte[] byData, byte byteSearch, int intStart, bool blnCaseInsensitive)
        {
            if (!blnCaseInsensitive)
                return Array.IndexOf(byData, (byte)byteSearch, intStart);

            //OK, it's a bit more complex as we have to check for a lowercase char
            byte charLow = 0;
            //Encoding.ASCII.GetBytes(Char.ToLower(Convert.ToChar(bySearch)));
            byte charHigh = 0;
            //Encoding.ASCII.GetBytes(Char.ToUpper(Convert.ToChar(bySearch)));

            int intLower = Array.IndexOf(byData, charLow, intStart, 1);
            int intUpper = Array.IndexOf(byData, charHigh, intStart, 1);

            if (intLower < intUpper && intLower > -1)
            {
                return intLower;
            }
            else
            {
                return intUpper;
            }

        }

        public static int BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition)
        {
            return byData.BinarySearchExt(bySearch, intSearchPosition, byData.Length, false);
        }

        public static int BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition, int intLength)
        {
            return byData.BinarySearchExt(bySearch, intSearchPosition, intLength, false);
        }

        public static int BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition, int intLength, bool blnIgnoreCase)
        {

            if (intSearchPosition < 0)
                throw new ArgumentOutOfRangeException("intSearchPosition", @"Argument must be greater than -1");

            if (intSearchPosition > byData.Length) return -1;

            int intPointer = 0;

            //OK, let's see if we can find the first instance of the first 
            //character of bySearch
            intPointer = FindCharCaseInsensitive(byData,
                bySearch[0], intSearchPosition, blnIgnoreCase);

            //No luck, or has it passed the max length (intLength) that we 
            //should look in?
            if (intPointer < 0 || intPointer > intLength - 1)
                return -1;

            bool blnFailCheck = false;
            while (intPointer >= 0 || intPointer < intLength)
            {
                for (int intLoop = 1; intLoop <= bySearch.Length - 1; intLoop++)
                {
                    if ((intPointer + intLoop) >= intLength)
                    {
                        blnFailCheck = true;
                        break;
                    }

                    if (blnIgnoreCase)
                    {
                        blnFailCheck = blnFailCheck ||
                            (Char.ToLower(Convert.ToChar(bySearch[intLoop])) !=
                            Char.ToLower(Convert.ToChar(byData[intPointer + intLoop])));
                    }
                    else
                    {
                        blnFailCheck = blnFailCheck ||
                            (bySearch[intLoop] != byData[intPointer + intLoop]);
                    }

                    if (blnFailCheck) break;
                }

                if (!blnFailCheck) break;

                //intPointer = Array.IndexOf(byData, bySearch(0), intPointer + 1, intLength - intPointer - 1)
                intPointer = FindCharCaseInsensitive(byData, bySearch[0], intPointer + 1, blnIgnoreCase);

                if (intPointer > -1 || intPointer < intLength)
                    blnFailCheck = false;
            }

            if (blnFailCheck) return -1;

            return intPointer;
        }

    }
}

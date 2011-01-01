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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Ximura;
using Ximura.Data;
#endregion
namespace Ximura
{
    /// <summary>
    /// This is the CSV row item structure.
    /// </summary>
    public struct CSVRowItem: IEnumerable<string>
    {        
        #region Declarations
        private char[] mData;
        private Dictionary<string, int> mHeaders;

        private int[] mStarts;
        private int[] mLengths;
        private int mHeaderCount;
        #endregion  

        #region SkipPreamble(byte[] preAmb, byte[] buffer, int start, int length)
        /// <summary>
        /// This method checks whether the data contains an encoding preamble at the begining and if so it strips them out.
        /// </summary>
        /// <param name="preAmb">The encoding preamble.</param>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="start">The start position.</param>
        /// <param name="length">The available bytes.</param>
        /// <returns>Returns true if the preamble should be skipped, false otherwise.</returns>
        private static bool SkipPreamble(byte[] preAmb, byte[] buffer, int start, int length)
        {
            //Do the boundary checks
            if (length < preAmb.Length)
                return false;

            //We need to check the preamble
            for (int i = 0; i < preAmb.Length; i++)
            {
                if (preAmb[i] != buffer[start + i])
                    return false;
            }

            return true;
        }
        #endregion  

        #region PositionSet(int item, int start, int lenght)
        /// <summary>
        /// This method sets the start and end positions of the various line items.
        /// </summary>
        /// <param name="item">The item number.</param>
        /// <param name="start">The start position.</param>
        /// <param name="length">The length in characters.</param>
        private void PositionSet(int item, int start, int length)
        {
            if (item >= mHeaderCount)
            {
                mHeaderCount = item+1;
                Array.Resize(ref mStarts,  mHeaderCount);
                Array.Resize(ref mLengths, mHeaderCount);
            }

            mStarts[item] = start;
            mLengths[item] = length;
        }
        #endregion
        #region PositionGet(int item, out int start, out int length)
        /// <summary>
        /// This method retrieves the item positions with the particular array.
        /// </summary>
        /// <param name="item">The item position.</param>
        /// <param name="start">The start position in the char array.</param>
        /// <param name="length">The number of characters in the array.</param>
        /// <returns>Returns true if the item exists.</returns>
        private bool PositionGet(int item, out int start, out int length)
        {
            start = -1;
            length = -1;

            if (item >= mHeaderCount)
                return false;

            start = mStarts[item];
            length = mLengths[item];

            return true;
        }
        #endregion  
 

        #region Constructors
        /// <summary>
        /// This is the byte constructor. The data will be converted in to a string using UTF8 encoding.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="start">The start position.</param>
        /// <param name="length">The data length.</param>
        public CSVRowItem(Dictionary<string, int> headers, byte[] buffer, int start, int length)
            : this(headers, UTF8Encoding.UTF8, buffer, start, length) { }
        /// <summary>
        /// This is the byte constructor using a custom encoding method.
        /// </summary>
        /// <param name="enc">The byte encoding used to transform the data in to a string.</param>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="start">The start position.</param>
        /// <param name="length">The data length.</param>
        public CSVRowItem(Dictionary<string, int> headers, Encoding enc, byte[] buffer, int start, int length)
        {
            mHeaderCount = -1;
            mHeaders = headers;
            mStarts = null;
            mLengths = null;

            byte[] preAmb = enc.GetPreamble();

            if (SkipPreamble(preAmb, buffer, start, length))
            {
                start += preAmb.Length;
                length -= preAmb.Length;
            }

            mData = enc.GetString(buffer, start, length).ToCharArray();
        }
        /// <summary>
        /// This is the string constructor.
        /// </summary>
        /// <param name="data">The CSV data line.</param>
        public CSVRowItem(Dictionary<string, int> headers, string data)
        {
            mHeaderCount = -1;
            mHeaders = headers;
            mStarts = null;
            mLengths = null; 
            
            mData = data.ToCharArray();
        }
        #endregion  
        #region Data
        /// <summary>
        /// This is the raw data from the string.
        /// </summary>
        public string Data { get { return new string(mData); } }
        #endregion  

        #region BuildItems()
        /// <summary>
        /// This method is used to parse and build the items from the data string.
        /// </summary>
        private void BuildItems()
        {
            if (mStarts != null)
                return;

            //Set the boundary buffers to their initial values.
            if (mHeaders == null)
            {
                mStarts = new int[10];
                mLengths = new int[10];
            }
            else
            {
                mStarts = new int[mHeaders.Count];
                mLengths = new int[mHeaders.Count];
            }

            int startPos = 0;
            int pos = -1;
            int start = 0;
            int scanStart = 0;
            int item = 0;

            try
            {
                bool inSpeechMarks = false;

                //OK, we will scan through the characters for the boundary markers.
                do
                {
                    pos = mData.FindFirstPosition(scanStart, mData.Length - start,
                        c => c == '\"' || c == ',' || c == '\r' || c == '\n');

                    if (pos == -1)
                    {
                        //This is the boundary condition for either a single record with no line end
                        //or the last record with a line end.
                        PositionSet(item, start, mData.Length - start);
                        item++;
                        return;
                    }

                    switch (mData[pos])
                    {
                        case '\"':
                            if (!inSpeechMarks)
                            {
                                inSpeechMarks = true;
                                start = pos +1;
                                scanStart = start;
                            }
                            else
                            {
                                //First check if this is a double speech mark
                                //if (pos+1 < mData.Length -1)

                            }
                            break;
                        case ',':
                            if (!inSpeechMarks)
                            {
                                PositionSet(item, start, pos - start);
                                item++;
                                start = pos + 1;
                                scanStart = start;
                            }
                            break;
                        case '\r':
                        case '\n':
                            if (!inSpeechMarks)
                            {
                                PositionSet(item, start, pos - start);
                                item++;
                                return;
                            }
                            break;
                    }

                }
                while (pos != -1 && pos < mData.Length);
            }
            finally
            {
                //OK, we need to check for a boundary condition where a line ends
                //without a carriage return. This can happed
                mHeaderCount = item;
            }
        }
        #endregion  
        #region GetEnumerator()
        /// <summary>
        /// This enumerator returns the data values.
        /// </summary>
        /// <returns>A string collection containing the individual items.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            BuildItems();

            for (int i = 0; i < mStarts.Length; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion  

        #region HeadersSupported
        /// <summary>
        /// This property identifies whether there is a header collection available.
        /// </summary>
        public bool HeadersSupported
        {
            get
            {
                return mHeaders != null;
            }
        }
        #endregion  

        #region this[string header]
        /// <summary>
        /// This iterator returns an item based on it's column name.
        /// </summary>
        /// <param name="header">The header name which is case sensitive.</param>
        /// <returns>Returns the value of the item or null if the header is not found.</returns>
        public string this[string header]
        {
            get
            {
                if (!HeadersSupported)
                    throw new NotSupportedException("Headers are not available in the structure.");

                if (!mHeaders.ContainsKey(header))
                    return null;

                return this[mHeaders[header]];
            }
        }
        #endregion  
        #region this[int header]
        /// <summary>
        /// This iterator returns an item based on it's column position.
        /// </summary>
        /// <param name="header">The header position.</param>
        /// <returns>Returns the value of the item or null if the header is not found.</returns>
        public string this[int header]
        {
            get
            {
                int start, length;

                if (!PositionGet(header, out start, out length))
                    throw new ArgumentOutOfRangeException("The header identifier is greater than the number of available columns.");

                if (length == 0)
                    return string.Empty;

                return new string(mData, start, length);
            }
        }
        #endregion  
    }
}

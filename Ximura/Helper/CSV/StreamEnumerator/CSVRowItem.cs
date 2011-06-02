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
using System.Diagnostics;

using Ximura;
#endregion
namespace Ximura
{
    /// <summary>
    /// This is the CSV row item structure.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public struct CSVRowItem: IEnumerable<string>
    {        
        #region Declarations
        /// <summary>
        /// This class holds the configuration options for the CSV files.
        /// </summary>
        private CSVStreamOptions mOptions;

        /// <summary>
        /// This array holds the decoded character data for the specific line number.
        /// </summary>
        private char[] mData;

        /// <summary>
        /// This array holds the start position for the specific header. 
        /// The last value is the end position of the last value. This is needed
        /// as the char array may be bigger than the actual data for the line item.
        /// </summary>
        private int[] mPositions;

        /// <summary>
        /// This is the line ID.
        /// </summary>
        private long mLineID;
        #endregion  
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="options">The enumeration options.</param>
        /// <param name="data">The character data array.</param>
        /// <param name="positions">The positions collection.</param>
        public CSVRowItem(CSVStreamOptions options,
            char[] data, int[] positions, long lineID)
        {
            mOptions = options;
            mData = data;

            mLineID = lineID;

            mPositions = positions;
        }
        #endregion

        #region GetEnumerator()
        /// <summary>
        /// This enumerator returns the data values.
        /// </summary>
        /// <returns>A string collection containing the individual items.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
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
                return mOptions.Headers != null;
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

                if (!mOptions.Headers.ContainsKey(header))
                    return null;

                return this[mOptions.Headers[header]];
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

        #region Length(int header)
        /// <summary>
        /// This is the length of the specific fiels.
        /// </summary>
        /// <param name="header">The header position.</param>
        /// <returns>Returns the number of characters for the field.</returns>
        public int Length(int header)
        {
            int start, length;

            if (!PositionGet(header, out start, out length))
                throw new ArgumentOutOfRangeException("The header identifier is greater than the number of available columns.");

            return length;
        }
        #endregion
        #region Length(string header)
        /// <summary>
        /// This method returns the length of the specific named header.
        /// </summary>
        /// <param name="header">The header name.</param>
        /// <returns>The length of the specific header.</returns>
        public int Length(string header)
        {
            if (!HeadersSupported)
                throw new NotSupportedException("Headers are not available in the structure.");

            if (!mOptions.Headers.ContainsKey(header))
                throw new ArgumentOutOfRangeException("The header is not recognized.");

            return Length(mOptions.Headers[header]);
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

            if (item >= mPositions.Length)
                return false;

            if (item == 0)
            {
                start = 0;
                length = mPositions[0];
            }
            else
            {
                start = mPositions[item - 1];
                length = mPositions[item] - start;
            }

            return true;
        }
        #endregion  

        #region Count
        /// <summary>
        /// This is the number of items in the csv line.
        /// </summary>
        public int Count
        {
            get
            {
                return mPositions.Length;
            }
        }
        #endregion
        #region CharCount
        /// <summary>
        /// This is the number of characters in the CSV line.
        /// </summary>
        public int CharCount
        {
            get
            {
                return mPositions[mPositions.Length - 1];
            }
        }
        #endregion  

        #region LineID
        /// <summary>
        /// This is the CSV line ID.
        /// </summary>
        public long LineID { get { return mLineID; } }
        #endregion

        #region ToString()
        /// <summary>
        /// This override returns the string representation of the line.
        /// </summary>
        /// <returns>Returns a string with the data.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                int start, length;

                if (!PositionGet(i, out start, out length))
                    throw new ArgumentOutOfRangeException("The header identifier is greater than the number of available columns.");

                if (length > 0)
                {
                    sb.Append(mData, start, length);

                    if ((Count - i) > 1)
                    {
                        sb.Append(mOptions.CSVSeperator);
                    }
                }
            }

            return sb.ToString();
        }
        #endregion

        #region Data
        /// <summary>
        /// This enumerator returns a set of keyvalue pairs with the Header name and value.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Data
        {
            get
            {
                string[] keys = new string[Count];
                mOptions.Headers.Keys.CopyTo(keys, 0);
                for (int i = 0; i < Count; i++)
                    yield return new KeyValuePair<string, string>(keys[i], this[i]);
            }
        }
        #endregion
    }
}

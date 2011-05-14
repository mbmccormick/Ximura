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
        private char mSeperator;

        private int[] mStarts;
        private int[] mLengths;
        private int mHeaderCount;
        #endregion  
        #region Constructors
        /// <summary>
        /// This is the byte constructor using a custom encoding method.
        /// </summary>
        /// <param name="enc">The byte encoding used to transform the data in to a string.</param>
        /// <param name="data">The character collection.</param>
        /// <param name="start">The start position.</param>
        /// <param name="length">The data length.</param>
        public CSVRowItem(char seperator, Dictionary<string, int> headers, char[] data, IEnumerable<KeyValuePair<int,int>> positions)
        {
            mHeaders = headers;
            mData = data;
            mSeperator = seperator;

            int headerCount = headers==null?-1:headers.Count;
            int[] starts    = headerCount == -1 ? null : new int[headerCount];
            int[] lengths   = headerCount == -1 ? null : new int[headerCount];

            //The the char boundaries for the items.
            positions
                .OrderBy(k => k.Key)
                .ForIndex((i, k) =>
                {
                    if (i >= headerCount)
                    {
                        headerCount = i + 1;
                        Array.Resize(ref starts, headerCount);
                        Array.Resize(ref lengths, headerCount);
                    }

                    starts[i] = k.Key;
                    lengths[i] = k.Value;
                }
                );

            mStarts = starts;
            mLengths = lengths;
            mHeaderCount = headerCount;
        }
        #endregion

        #region Data
        /// <summary>
        /// This is the raw data from the string.
        /// </summary>
        public string Data { get { return new string(mData); } }
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

        #region Count
        /// <summary>
        /// This is the number of items in the csv line.
        /// </summary>
        public int Count
        {
            get
            {
                return mStarts.Length;
            }
        }
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
                        sb.Append(mSeperator);
                    }
                }
            }

            return sb.ToString();
        }
        #endregion // ToString()

    }
}

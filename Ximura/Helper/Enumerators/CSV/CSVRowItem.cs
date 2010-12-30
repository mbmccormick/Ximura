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
        private string mData;
        #endregion  
        #region Constructors
        /// <summary>
        /// This is the byte constructor. The data will be converted in to a string using UTF8 encoding.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="start">The start position.</param>
        /// <param name="length">The data length.</param>
        public CSVRowItem(byte[] buffer, int start, int length)
            : this(UTF8Encoding.UTF8, buffer, start, length){}
        /// <summary>
        /// This is the byte constructor using a custom encoding method.
        /// </summary>
        /// <param name="enc">The byte encoding used to transform the data in to a string.</param>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="start">The start position.</param>
        /// <param name="length">The data length.</param>
        public CSVRowItem(Encoding enc, byte[] buffer, int start, int length):this(enc.GetString(buffer, start, length)){}
        /// <summary>
        /// This is the string constructor.
        /// </summary>
        /// <param name="data">The CSV data line.</param>
        public CSVRowItem(string data)
        {
            mData = data;
        }
        #endregion  
        #region Data
        /// <summary>
        /// This is the raw data from the string.
        /// </summary>
        public string Data { get { return mData; } }
        #endregion  
        #region GetEnumerator()
        /// <summary>
        /// This enumerator returns the data values.
        /// </summary>
        /// <returns>A string collection containing the individual items.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            //TODO:this is a naive interpretation of CSV line logic.
            string[] items = mData.Split(new char[]{','});

            foreach(string item in items)
            {
                if (item.EndsWith("\r\n"))
                    yield return item.Substring(0, item.Length-2).Trim(new char[] { '"' });
                else
                    yield return item.Trim(new char[]{'"'});
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion  

    }
}

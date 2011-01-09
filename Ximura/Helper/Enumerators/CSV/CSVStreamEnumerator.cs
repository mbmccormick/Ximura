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
    #region CSVStreamEnumerator
    /// <summary>
    /// This is the default CSV parser class.
    /// </summary>
    public class CSVStreamEnumerator : CSVStreamEnumerator<CSVRowItem>
    {
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="headerInFirstRow">A boolean value that indicates whether the headers are in the first row.</param>
        public CSVStreamEnumerator(Stream data, bool headerInFirstRow)
            : base(data, headerInFirstRow, Encoding.UTF8, (i) => i)
        {

        }
    }
    #endregion  

    #region CSVStreamEnumerator<O>
    /// <summary>
    /// This class enumerates a CSV data stream and returns a set of data objects for each individual row record.
    /// </summary>
    /// <typeparam name="O">The output item type.</typeparam>
    public class CSVStreamEnumerator<O> : IntermediateObjectEnumerator<Stream, CSVRowItem, O>
    {
        #region Declarations
        private Dictionary<string, int> mHeaders;

        private Encoding mEncoding;
        private bool mScanPreamble;
        #endregion 

        #region Constructor
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="headerInFirstRow">A boolean value that indicates whether the headers are in the first row.</param>
        /// <param name="enc">The character encoding. If this is null, then it will be set to UTF8.</param>
        /// <param name="convert">A function to convert the CSVRowItem structure in to the output structure.</param>
        public CSVStreamEnumerator(Stream data, bool headerInFirstRow, 
            Encoding enc, Func<CSVRowItem, O> convert)
            : base(data, null, convert)
        {
            if (enc == null)
                mEncoding = Encoding.UTF8;
            else
                mEncoding = enc;

            //Set the preamble flag.
            byte[] preAmb = enc.GetPreamble();
            mScanPreamble = preAmb != null & preAmb.Length > 0;

            if (headerInFirstRow)
            {
                Tuple<CSVRowItem, Stream>? result = Parse(data);
                if (!result.HasValue)
                    throw new ArgumentOutOfRangeException("The headers cannot be read from the stream.");

                mHeaders = HeadersParse(result.Value.Item1);
            }
            else
                mHeaders = null;
        }
        #endregion

        #region HeadersParse(CSVRowItem headers)
        /// <summary>
        /// This method parses the headers and creates a header collection.
        /// </summary>
        /// <param name="headers">The csv items from the first row.</param>
        /// <returns>The dictionary of headers.</returns>
        private Dictionary<string, int> HeadersParse(CSVRowItem headers)
        {
            Dictionary<string, int> data = new Dictionary<string,int>();

            headers.ForIndex((i, d) => data.Add(d, i));

            return data;
        }
        #endregion  

        #region Headers
        /// <summary>
        /// This enumeration will return a header collection enumeration.
        /// </summary>
        public Dictionary<string,int> Headers
        {
            get
            {
                if (mHeaders == null)
                    return null;

                return mHeaders; ;
            }
        }
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

        #region Parse(Stream data)
        /// <summary>
        /// This method converts the stream data in to an individual row item.
        /// </summary>
        /// <param name="data">The stream to read from.</param>
        /// <returns>Returns the intermediate item or null if not more items can be read.</returns>
        protected override Tuple<CSVRowItem, Stream>? Parse(Stream data)
        {
            if (!data.CanRead)
                return null;

            //if (mScanPreamble)
            //{
            //    byte[] preAmb = enc.GetPreamble();
            //    int preAmbLength = preAmb.Length;
            //    while (data.CanRead && preAmbLength > 0)
            //    {
                    
            //    }

            //    mScanPreamble = false;
            //}

            MatchState<byte> matchState = new MatchState<byte>(true);
            
            //TODO: Naive interpretation - should match on CRLF and LF and not CRLF/LF in speech marks.
            var match = data.Enum().MatchSequence(new byte[] { 13, 10 }, matchState);

            if (match.IsMatch)
                return new Tuple<CSVRowItem, Stream>(
                    new CSVRowItem(mHeaders, matchState.Data, 0, matchState.DataPosition + 1)
                    , data);

            return null;
        }
        #endregion  
    }
    #endregion  
}

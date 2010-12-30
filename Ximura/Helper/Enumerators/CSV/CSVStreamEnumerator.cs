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
            : base(data, headerInFirstRow, (i) => i)
        {

        }
    }
    #endregion  

    /// <summary>
    /// This class 
    /// </summary>
    /// <typeparam name="O">The output item type.</typeparam>
    public class CSVStreamEnumerator<O> : IntermediateObjectEnumerator<Stream, CSVRowItem, O>
    {
        #region Declarations
        private Dictionary<string, int> mHeaders;
        #endregion 
        #region Constructor
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="headerInFirstRow">A boolean value that indicates whether the headers are in the first row.</param>
        /// <param name="convert">A function to convert the CSVRowItem structure in to the output structure.</param>
        public CSVStreamEnumerator(Stream data, bool headerInFirstRow, Func<CSVRowItem, O> convert)
            : base(data, null, convert)
        {
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

        #region Parse(Stream data)
        /// <summary>
        /// This method converts the stream data in to an individual row item.
        /// </summary>
        /// <param name="data">The stream to read from.</param>
        /// <returns>Returns the intermediate item or null if not more items can be read.</returns>
        protected override Tuple<CSVRowItem, Stream>? Parse(Stream data)
        {
            MatchState<byte> matchState = new MatchState<byte>(true);

            var match = data.Enum().MatchSequence(new byte[] { 13, 10 }, matchState);

            if (match.IsMatch)
                return new Tuple<CSVRowItem, Stream>(
                    new CSVRowItem(matchState.Data, 0, matchState.DataPosition + 1)
                    , data);

            return null;
        }
        #endregion  
    }
}

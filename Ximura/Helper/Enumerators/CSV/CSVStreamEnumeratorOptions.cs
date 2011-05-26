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
    /// This structure holds the enumerator options.
    /// </summary>
    public class CSVStreamEnumeratorOptions
    {
        #region Static -> Default
        /// <summary>
        /// This is the empty job signature.
        /// </summary>
        public static readonly CSVStreamEnumeratorOptions Default;

        /// <summary>
        /// This is the static constructor.
        /// </summary>
        static CSVStreamEnumeratorOptions()
        {
            Default = new CSVStreamEnumeratorOptions();

            Default.HeadersInFirstRow = true;
            Default.Headers = null;

            Default.CSVSeperator = ',';

            Default.DataRowsToSkip = 0;
            Default.HeaderRowsToSkip = 0;

            Default.Encoding = Encoding.UTF8;
            Default.SkipCharacters = null;
        }
        #endregion

        #region Constructor
        private CSVStreamEnumeratorOptions()
        {

        }
        /// <summary>
        /// This is the options constructor.
        /// </summary>
        /// <param name="HeadersInFirstRow">This boolean values specifies whether the headers are in the first row.</param>
        /// <param name="SkipEmptyRows">This property specifies whether empty rows should be skipped.</param>
        public CSVStreamEnumeratorOptions(bool HeadersInFirstRow, bool SkipEmptyRows)
            : this(Encoding.UTF8, ',', HeadersInFirstRow, SkipEmptyRows, null, null) { }

        /// <summary>
        /// This is the options constructor.
        /// </summary>
        /// <param name="CSVSeperator">This is the CSV seperator character.</param>
        /// <param name="HeadersInFirstRow">This boolean values specifies whether the headers are in the first row.</param>
        /// <param name="SkipEmptyRows">This property specifies whether empty rows should be skipped.</param>
        public CSVStreamEnumeratorOptions(char CSVSeperator, bool HeadersInFirstRow, bool SkipEmptyRows)
            : this(Encoding.UTF8, CSVSeperator, HeadersInFirstRow, SkipEmptyRows, null, null) { }

        /// <summary>
        /// This is the options constructor.
        /// </summary>
        /// <param name="encoding">This is the character encoding for the stream.</param>
        /// <param name="CSVSeperator">This is the CSV seperator character.</param>
        /// <param name="HeadersInFirstRow">This boolean values specifies whether the headers are in the first row.</param>
        /// <param name="SkipEmptyRows">This property specifies whether empty rows should be skipped.</param>
        /// <param name="skipCharacters">This predicate can be used to remove invalid characters from the incoming stream. 
        /// By default if this is left null all characters are processed.</param>
        public CSVStreamEnumeratorOptions(Encoding encoding, char CSVSeperator,
            bool HeadersInFirstRow, bool SkipEmptyRows, Predicate<char> skipCharacters, Dictionary<string, int> Headers)
        {
            this.HeadersInFirstRow = HeadersInFirstRow;
            this.CSVSeperator = CSVSeperator;
            this.IgnoreEmptyDataRows = SkipEmptyRows;
            this.Encoding = encoding;
            this.SkipCharacters = skipCharacters;
            this.Headers = Headers;
        }
        #endregion

        #region Encoding
        /// <summary>
        /// This is the encoding used by the stream enumerator.
        /// </summary>
        public Encoding Encoding
        {
            get;
            private set;
        }
        #endregion // Encoding

        #region HeaderRowsToSkip
        /// <summary>
        /// This is the number of header rows that should be skipped before the header row is processed.
        /// </summary>
        public int HeaderRowsToSkip { get; private set; }
        #endregion
        #region DataRowsToSkip
        /// <summary>
        /// This is the number of data rows after the header rows that should be skipped before data is returned.
        /// </summary>
        public int DataRowsToSkip { get; private set; }
        #endregion

        #region HeadersInFirstRow
        /// <summary>
        /// This property specifies whether headers are in the first row.
        /// </summary>
        public bool HeadersInFirstRow
        {
            get;
            private set;
        }
        #endregion // HeadersInFirstRow

        #region Headers
        /// <summary>
        /// This is the header collection. This must be supplied if HeadersInFirstRow is set to false;
        /// </summary>
        public Dictionary<string,int> Headers
        {
            get;
            private set;
        }
        #endregion // Headers

        #region CSVSeperator
        /// <summary>
        /// This is the CSV seperator character.
        /// </summary>
        public char CSVSeperator
        {
            get;
            private set;
        }
        #endregion // CSVSeperator

        #region TextDelimiter
        /// <summary>
        /// This is the text delimiter character.
        /// </summary>
        public char TextDelimiter
        {
            get;
            private set;
        }
        #endregion

        #region IgnoreEmptyDataRows
        /// <summary>
        /// This identifies whether empty rows will be ignored or whether a data exception will be thrown.
        /// </summary>
        public bool IgnoreEmptyDataRows
        {
            get;
            private set;
        }
        #endregion // SkipEmptyRows

        #region SkipCharacters
        /// <summary>
        /// This predicate can be used to ignore certain character in the incoming data stream.
        /// </summary>
        public Predicate<char> SkipCharacters
        {
            get;
            private set;
        }
        #endregion // SkipCharacters

    }
}

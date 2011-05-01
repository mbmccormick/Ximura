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
    public class CSVStreamEnumerator<O> : IntermediateObjectEnumerator<Stream, UnicodeCharEnumerator, CSVRowItem, O>
    {
        #region Static declaration
        /// <summary>
        /// This is the default char array growth factor
        /// </summary>
        public static readonly int ARRAYGROWTHFACTOR = 10;
        #endregion  

        #region Declarations
        private Dictionary<string, int> mHeaders;

        private char? mOverflow;
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
            Encoding enc, Func<CSVRowItem, O> convert):this(data, headerInFirstRow, false, enc, convert)
        {

        }
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="headerInFirstRow">A boolean value that indicates whether the headers are in the first row.</param>
        /// <param name="skipEmptyRows">This boolean value indicates whether empty rows can be skipped without throwing an error.</param>
        /// <param name="enc">The character encoding. If this is null, then it will be set to UTF8.</param>
        /// <param name="convert">A function to convert the CSVRowItem structure in to the output structure.</param>
        public CSVStreamEnumerator(Stream data, bool headerInFirstRow, bool skipEmptyRows,
            Encoding enc, Func<CSVRowItem, O> convert)
            : base(data, null, convert, null)
        {
            if (enc == null)
                Enc = Encoding.UTF8;
            else
                Enc = enc;

            mOverflow = null;

            if (headerInFirstRow)
            {
                Tuple<CSVRowItem, UnicodeCharEnumerator>? result = Parse(mDataConverted);
                if (!result.HasValue)
                    throw new ArgumentOutOfRangeException("The headers cannot be read from the stream.");

                mHeaders = HeadersParse(result.Value.Item1);
            }
            else
                mHeaders = null;
        }
        #endregion

        #region ConvertSource(Stream data)
        /// <summary>
        /// This method converts the incoming byte stream in to a unicode char enumerator.
        /// </summary>
        /// <param name="data">The incoming byte stream.</param>
        /// <returns>The character enumerator.</returns>
        protected override UnicodeCharEnumerator ConvertSource(Stream data)
        {
            return new UnicodeCharEnumerator(data, Enc);
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
                return mHeaders;
            }
        }
        #endregion  

        #region WriteChar(ref char[] data, int position, char value)
        /// <summary>
        /// This method is used to set the value and to 
        /// autogrow the the char array.
        /// </summary>
        /// <param name="data">The array</param>
        /// <param name="position">The char position.</param>
        /// <param name="value">The value to set.</param>
        private int WriteChar(ref char[] data, int position, char value)
        {
            if (position >= data.Length)
                Array.Resize(ref data, position + ARRAYGROWTHFACTOR);

            data[position] = value;

            return position + 1;
        }
        #endregion  

        #region Parse(UnicodeCharEnumerator data)
        /// <summary>
        /// This method converts the stream data in to an individual row item.
        /// </summary>
        /// <param name="data">The unicode enumerator to read from.</param>
        /// <returns>Returns the intermediate item or null if not more items can be read.</returns>
        protected override Tuple<CSVRowItem, UnicodeCharEnumerator>? Parse(UnicodeCharEnumerator data)
        {
            IEnumerator<char> enChar = data.GetEnumerator();
            
            //Check that we can get a new character from the stream.
            if (!mOverflow.HasValue && !enChar.MoveNext())
                return null;

            //Ok, set the initial state.
            int pos = 0, item = 0, start = 0;
            //Create the character array to hold the data.
            char[] cData = new char[ARRAYGROWTHFACTOR];
            List<KeyValuePair<int, int>> positions = new List<KeyValuePair<int, int>>();

            bool inSpeechMarks = false, firstSpeech = false, scan = true;

            try
            {
                char val;
                //OK, we will scan through the characters for the boundary markers.
                do
                {
                    if (mOverflow.HasValue)
                        val = mOverflow.Value;
                    else
                        val = enChar.Current;

                    switch (val)
                    {
                        case '\"':
                            if (!inSpeechMarks)
                                inSpeechMarks = true;
                            //Is this a double speech mark within existing speech marks?
                            else if (firstSpeech)
                            {
                                pos = WriteChar(ref cData, pos, val);
                                firstSpeech = false;
                            }
                            else
                                firstSpeech = true;
                            break;

                        case ',':
                            if (!inSpeechMarks)
                            {
                                //PositionSet(item, start, pos - start);
                                positions.Add(new KeyValuePair<int, int>(start, pos-start));
                                item++;
                                start = pos;
                            }
                            else
                                pos = WriteChar(ref cData, pos, val);
                            break;

                        case '\r':
                        case '\n':
                            if (inSpeechMarks)
                                pos = WriteChar(ref cData, pos, val);
                            else
                            {
                                scan = false;
                            }
                            break;

                        default:
                            pos = WriteChar(ref cData, pos, val);
                            break;
                    }

                    if (mOverflow.HasValue)
                    {
                        mOverflow = null;
                    }
                }
                while (scan && enChar.MoveNext());
            }
            catch (Exception ex)
            {
                //Not sure what we would be catching here, but put a breakpoint in first.
                throw ex;
            }
            finally
            {
                //OK, we need to check for a boundary condition where a line ends
                //without a carriage return. This can happed
                //mHeaderCount = item;
            }

            CSVRowItem line = new CSVRowItem(null, cData, positions);

            return new Tuple<CSVRowItem, UnicodeCharEnumerator>(line, data);
        }
        #endregion

        #region Enc
        /// <summary>
        /// This is the encoding used by the stream enumerator.
        /// </summary>
        public Encoding Enc
        {
            get;
            private set;
        }
        #endregion  

    }
    #endregion  
}

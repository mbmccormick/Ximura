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
        public static readonly int ARRAYGROWTHFACTOR = 20;
        #endregion  

        #region Declarations
        private Dictionary<string, int> mHeaders;
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
            : base(data, null, convert, null)
        {
            if (enc == null)
                Enc = Encoding.UTF8;
            else
                Enc = enc;


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
                if (mHeaders == null)
                    return null;

                return mHeaders; ;
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
                Array.Resize(ref data, position + 1);

            data[position] = value;

            return position + 1;
        }
        #endregion  

        /// <summary>
        /// This method converts the stream data in to an individual row item.
        /// </summary>
        /// <param name="data">The stream to read from.</param>
        /// <returns>Returns the intermediate item or null if not more items can be read.</returns>
        protected override Tuple<CSVRowItem, UnicodeCharEnumerator>? Parse(UnicodeCharEnumerator data)
        {
            char[] cData = new char[10];
            IEnumerator<char> enChar = data.GetEnumerator();
            
            //Check that we can get a new character from the stream.
            if (!enChar.MoveNext())
                return null;

            //Ok, set the initial state.
            int start;
            int scanStart;
            //This is the character position in the char array.
            int pos = 0;
            int item = 0;
            bool inData = false;
            bool inSpeechMarks = false;
            bool firstSpeech = false;
            bool isLastCharSpeech = false;
            bool scan = true;
            try
            {
                //OK, we will scan through the characters for the boundary markers.
                do
                {
                    char current = enChar.Current;

                    switch (current)
                    {
                        case '\"':
                            if (!inSpeechMarks)
                            {
                                inSpeechMarks = true;
                                start = pos + 1;
                                scanStart = start;
                            }
                            //Is this a double speech mark within existing speech marks?
                            else if (firstSpeech)
                            {
                                //First check if this is a double speech mark
                                //if (pos+1 < mData.Length -1)

                            }
                            else
                                firstSpeech = true;
                            break;

                        case ',':
                            if (!inSpeechMarks)
                            {
                                //PositionSet(item, start, pos - start);
                                item++;
                                start = pos + 1;
                                scanStart = start;
                            }
                            break;

                        case '\r':
                        case '\n':
                            if (inSpeechMarks)
                                pos = WriteChar(ref cData, pos, current);
                            else
                                scan = false;
                            break;

                        default:
                            pos = WriteChar(ref cData, pos, current);
                            break;
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

            return null;
            //return new Tuple<CSVRowItem,UnicodeCharEnumerator>(
            //    new CSVRowItem(mHeaders, cData, positions),data);
        }
         


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

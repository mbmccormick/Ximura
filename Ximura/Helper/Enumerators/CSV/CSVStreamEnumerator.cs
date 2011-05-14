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
        public CSVStreamEnumerator(Stream data)
            : this(data, CSVStreamEnumeratorOptions.Default)
        {

        }
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="options">This is the parse options for the CSV file..</param>
        public CSVStreamEnumerator(Stream data, CSVStreamEnumeratorOptions options)
            : base(data, (i) => i, options)
        {

        }
    }
    #endregion  

    #region CSVStreamEnumerator<O>
    /// <summary>
    /// This class enumerates a CSV data stream and returns a set of data objects for each individual row record.
    /// </summary>
    /// <typeparam name="O">The output item type.</typeparam>
    public class CSVStreamEnumerator<O> : 
        IntermediateObjectEnumerator<Stream, UnicodeCharEnumerator, CSVRowItem, O>
    {
        #region Static declaration
        /// <summary>
        /// This is the default char array growth factor
        /// </summary>
        public static readonly int ARRAYGROWTHFACTOR = 10;
        #endregion  

        #region Declarations
        /// <summary>
        /// This is the header collection for the CSV file.
        /// </summary>
        private Dictionary<string, int> mHeaders;

        /// <summary>
        /// This is the overflow character. This is held when looking ahead in the data stream.
        /// </summary>
        private char? mOverflow;

        private CSVStreamEnumeratorOptions mOptions;

        private long mCurrentLine;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="convert">A function to convert the CSVRowItem structure in to the output structure.</param>
        public CSVStreamEnumerator(Stream data, Func<CSVRowItem, O> convert)
            : this(data, convert, CSVStreamEnumeratorOptions.Default){}
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="convert">A function to convert the CSVRowItem structure in to the output structure.</param>
        /// <param name="options">This is the enumerator options.</param>
        public CSVStreamEnumerator(Stream data, Func<CSVRowItem, O> convert, CSVStreamEnumeratorOptions options)
            : base(data, null, convert, null)
        {
            mOverflow = null;
            mOptions = options;
            mCurrentLine = 0;
            //If the headers are in the first row then parse the first line.
            if (options.HeadersInFirstRow)
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
        private int WriteValueAutoGrow<I>(ref I[] data, int position, I value) where I: struct
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
            bool discardWhitespace = false;
            
            try
            {
                char val;
                //OK, we will scan through the characters for the boundary markers.
                do
                {
                    if (mOverflow.HasValue)
                    {
                        val = mOverflow.Value;
                        mOverflow = null;
                    }
                    else
                        val = enChar.Current;

                    if (val == mOptions.CSVSeperator)
                    {
                        if (!inSpeechMarks || (inSpeechMarks && firstSpeech))
                        {
                            //PositionSet(item, start, pos - start);
                            positions.Add(new KeyValuePair<int, int>(start, pos - start));
                            inSpeechMarks = false;
                            firstSpeech = false;
                            item++;
                            start = pos;
                        }
                        else
                            pos = WriteValueAutoGrow(ref cData, pos, val);

                        firstSpeech = false;
                    }
                    else if (!(discardWhitespace && char.IsWhiteSpace(val)))
                        switch (val)
                        {
                            case '\"':
                                if (!inSpeechMarks)
                                {
                                    inSpeechMarks = true;
                                }
                                //Is this a double speech mark within existing speech marks?
                                else if (firstSpeech)
                                {
                                    pos = WriteValueAutoGrow(ref cData, pos, val);
                                    firstSpeech = false;
                                }
                                else
                                    firstSpeech = true;

                                break;

                            case '\r':
                            case '\n':
                                if (inSpeechMarks)
                                    pos = WriteValueAutoGrow(ref cData, pos, val);
                                else
                                {
                                    scan = false;
                                    mCurrentLine++;
                                    mOverflow = ScanAhead(enChar);

                                    if (mOverflow.HasValue
                                        && mOverflow.Value == '\n')
                                        mOverflow = null;
                                }

                                firstSpeech = false;
                                break;

                            default:
                                pos = WriteValueAutoGrow(ref cData, pos, val);
                                firstSpeech = false;
                                break;
                        }
                    else
                        //We are in a strange state.
                        throw new Exception();
                }
                while (scan && (mOverflow.HasValue || enChar.MoveNext()));
            }
            catch (Exception ex)
            {
                //Not sure what we would be catching here, but put a breakpoint in first.
                throw ex;
            }
            //finally
            //{
            //    //OK, we need to check for a boundary condition where a line ends
            //    //without a carriage return. This can happed
            //    //mHeaderCount = item;
            //}


            positions.Add(new KeyValuePair<int, int>(start, pos - start));
            item++;

            CSVRowItem line = new CSVRowItem(mOptions.CSVSeperator, null, cData, positions);

            return new Tuple<CSVRowItem, UnicodeCharEnumerator>(line, data);
        }
        #endregion

        #region ScanAhead(IEnumerator<char> enChar)
        /// <summary>
        /// This generic method is used to scana ahead of a byte stream.
        /// </summary>
        /// <param name="enChar">The character enumerator.</param>
        /// <returns></returns>
        private char? ScanAhead(IEnumerator<char> enChar)
        {
            if (!enChar.MoveNext())
            {
                return null;
            }

            return enChar.Current;
        }
        #endregion // ScanAhead(IEnumerator<char> enChar)

        #region Enc
        /// <summary>
        /// This is the encoding used by the stream enumerator.
        /// </summary>
        public Encoding Enc
        {
            get{return mOptions.Encoding;}
        }
        #endregion  

    }
    #endregion  
}

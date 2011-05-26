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
    /// This is the CSV stream enumerator class.
    /// </summary>
    public class CSVStreamEnumerator : CSVStreamEnumerator<CSVRowItem>
    {
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        public CSVStreamEnumerator(Stream data)
            : this(data, CSVStreamEnumeratorOptions.Default)
        {

        }
        /// <summary>
        /// This is the default constructor for the CSV enumerator.
        /// </summary>
        /// <param name="data">The data stream which will be read from.</param>
        /// <param name="options">This is the configuration options for the CSV file.</param>
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
        private Dictionary<string,int> mHeaders;

        /// <summary>
        /// This is the overflow character. This is held when looking ahead in the data stream.
        /// </summary>
        private char? mOverflow;

        private CSVStreamEnumeratorOptions mOptions;

        private long mCurrentLine;

        private long mCharacterTotal;

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
            : base(data, null, convert, (d) => { return new UnicodeCharEnumerator(data, options.Encoding); })
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
                mHeaders = options.Headers;
        }
        #endregion

        #region Dispose(bool disposing)
        /// <summary>
        /// This method checks cleans up anu large properties.
        /// </summary>
        /// <param name="disposing">Set to true if being disposed for the first time.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                mHeaders = null;
            }
            base.Dispose(disposing);
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
                DisposedCheck();
                return mHeaders;
            }
        }
        #endregion  

        #region WriteValueAutoGrow<I>(ref I[] data, int position, I value)
        /// <summary>
        /// This method is used to set the value and to 
        /// autogrow the the char array.
        /// </summary>
        /// <param name="data">The array</param>
        /// <param name="position">The char position.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Returns the new position in the array.</returns>
        private int WriteValueAutoGrow<I>(ref I[] data, int position, I value) where I : struct
        {
            return WriteValueAutoGrow<I>(ref data, position, value, null);
        }
        /// <summary>
        /// This method is used to set the value and to 
        /// autogrow the the char array.
        /// </summary>
        /// <param name="data">The array</param>
        /// <param name="position">The char position.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="toGrow">The array growth amount. If null then the default settings will be used.</param>
        /// <returns>Returns the new position in the array.</returns>
        private int WriteValueAutoGrow<I>(ref I[] data, int position, I value, int? toGrow) where I : struct
        {
            if (position >= data.Length)
            {
                int growBy = data.Length / 10;
                Array.Resize(ref data, position + (growBy > ARRAYGROWTHFACTOR ? growBy : ARRAYGROWTHFACTOR));
            }
            //Array.Resize(ref data, position + ARRAYGROWTHFACTOR);

            data[position] = value;

            return position + 1;
        }
        #endregion  
        
        #region CharArrayCreate()
        /// <summary>
        /// This method creates the character array to its default size.
        /// </summary>
        /// <returns>Returns a new character array.</returns>
        private char[] CharArrayCreate()
        {
            char[] cData;
            long average = (mCurrentLine == 0) ? ARRAYGROWTHFACTOR : mCharacterTotal / mCurrentLine;

            if (average < ARRAYGROWTHFACTOR)
                cData = new char[ARRAYGROWTHFACTOR];
            else
                cData = new char[average + average / 10];

            return cData;
        }
        #endregion // CharArrayCreate()
        #region ParseV2(UnicodeCharEnumerator data)
        /// <summary>
        /// This method converts the stream data in to an individual row item.
        /// </summary>
        /// <param name="data">The unicode enumerator to read from.</param>
        /// <returns>Returns the intermediate item or null if not more items can be read.</returns>
        protected override Tuple<CSVRowItem, UnicodeCharEnumerator>? 
            Parse(UnicodeCharEnumerator data)
        {
            IEnumerator<char> enChar = data.GetEnumerator();

            //Check that we can get a new character from the stream.
            if (!mOverflow.HasValue && !enChar.MoveNext())
                return null;

            //Create the character array to hold the data.
            char[] cData = CharArrayCreate();

            //Ok, set the initial state.
            int pos = 0, start = 0;

            bool inSpeechMarks = false, firstSpeech = false, scan = true;
            bool discardWhitespace = false;

            int headerPos = 0;
            int[] posEnds = new int[mOptions.Headers.Count];

            try
            {
                char val;
                //OK, we will scan through the characters for the boundary markers.
                do
                {
                    //Do we have an overflow character from a previous line look-ahead.
                    if (mOverflow.HasValue)
                    {
                        val = mOverflow.Value;
                        mOverflow = null;
                    }
                    else
                        val = enChar.Current;

                    //Seperator character
                    if (val == mOptions.CSVSeperator)
                    {
                        if (!inSpeechMarks || (inSpeechMarks && firstSpeech))
                        {
                            headerPos = WriteValueAutoGrow(ref posEnds, headerPos, pos, 1);

                            inSpeechMarks = false;
                            firstSpeech = false;
                            start = pos;
                        }
                        else
                            pos = WriteValueAutoGrow(ref cData, pos, val);

                        firstSpeech = false;
                    }
                    else if (!(discardWhitespace && char.IsWhiteSpace(val)))
                    {
                        switch (val)
                        {
                            //Speechmark
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
                            //EOL
                            case '\r':
                            case '\n':
                                if (inSpeechMarks && !firstSpeech)
                                {
                                    pos = WriteValueAutoGrow(ref cData, pos, val);
                                }
                                else
                                {
                                    scan = false;
                                    mCurrentLine++;
                                    mOverflow = ScanAhead(enChar);

                                    if (mOverflow.HasValue
                                        && mOverflow.Value == '\n')
                                        mOverflow = null;
                                    inSpeechMarks = false;
                                }

                                firstSpeech = false;
                                break;
                            //Valid character
                            default:
                                pos = WriteValueAutoGrow(ref cData, pos, val);
                                firstSpeech = false;
                                break;
                        }
                    }
                }
                while (scan && (mOverflow.HasValue || enChar.MoveNext()));
            }
            catch (Exception ex)
            {
                //Not sure what we would be catching here, but put a breakpoint in first.
                throw ex;
            }

            headerPos = WriteValueAutoGrow(ref posEnds, headerPos, pos, 1);

            CSVRowItem line = new CSVRowItem(mOptions, cData, posEnds, mCurrentLine);

            mCharacterTotal += line.CharCount;

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
            get
            {
                DisposedCheck();
                return mOptions.Encoding;
            }
        }
        #endregion  

        #region DataSource
        /// <summary>
        /// This is the data source stream for the enumerator.
        /// </summary>
        public virtual Stream BaseStream
        {
            get
            {
                DisposedCheck();
                return base.mDataSource;
            }
        }
        #endregion
    }
    #endregion  
}

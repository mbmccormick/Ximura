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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Ximura;
#endregion
namespace Ximura
{
    #region IntermediateObjectEnumerator<D, I, O>
    /// <summary>
    /// This method is used to parse the data stream with an intermediate data type prior to conversion.
    /// </summary>
    /// <typeparam name="D">The source data type, i.e. stream</typeparam>
    /// <typeparam name="I">The intermediate data type.</typeparam>
    /// <typeparam name="O">The output record data type, i.e. int</typeparam>
    public abstract class IntermediateObjectEnumerator<D, I, O> : IntermediateObjectEnumerator<D, D, I, O>
    {
        #region Constructor
        /// <summary>
        /// This is the protected constructor.
        /// </summary>
        /// <param name="data">The data source.</param>
        /// <param name="parse">The parsing function.</param>
        /// <param name="convert">The conversion function to convert the intermediate type in to the output type.</param>
        protected IntermediateObjectEnumerator(D data, Func<D, Tuple<I, D>?> parse, Func<I, O> convert)
            : base(data, parse, convert, (d) => d)
        {
        }
        #endregion
    }
    #endregion  

    #region IntermediateObjectEnumerator<D, C, I, O>
    /// <summary>
    /// This method is used to parse the data stream with an intermediate data type prior to conversion.
    /// </summary>
    /// <typeparam name="D">The source data type, i.e. stream</typeparam>
    /// <typeparam name="C">The converted source data type, i.e. stream</typeparam>
    /// <typeparam name="I">The intermediate data type.</typeparam>
    /// <typeparam name="O">The output record data type, i.e. int</typeparam>
    public abstract class IntermediateObjectEnumerator<D, C, I, O> : IEnumerable<O>
    {
        #region Declarations
        /// <summary>
        /// This function is used to convert the intermediate type in to the output type.
        /// </summary>
        protected Func<I, O> mConvertOutput;
        /// <summary>
        /// This function is used to convert the data in to the intermediate data type.
        /// </summary>
        protected Func<C, Tuple<I, C>?> mParse;
        /// <summary>
        /// This enumerator encapsulates the base object with the intermediate type.
        /// </summary>
        protected ObjectEnumerator<C, I> mIntermediateOE;

        /// <summary>
        /// The original data source.
        /// </summary>
        protected D mDataSource;
        /// <summary>
        /// The converted data source.
        /// </summary>
        protected C mDataConverted;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the protected constructor.
        /// </summary>
        /// <param name="data">The data source.</param>
        /// <param name="parse">The parsing function.</param>
        /// <param name="convertOutput">The conversion function to convert the intermediate type in to the output type.</param>
        /// <param name="convertSource">The conversion function to convert the incoming data source in to the outgoing data source.</param>
        protected IntermediateObjectEnumerator(D data, Func<C, Tuple<I, C>?> parse, 
            Func<I, O> convertOutput, Func<D, C> convertSource)
        {
            if (data == null)
                throw new ArgumentNullException("The data cannot be null.");

            mDataSource = data;

            //Set the converted data source.
            if (convertSource == null)
                mDataConverted = ConvertSource(data);
            else
                mDataConverted = convertSource(data);

            if (mDataConverted == null)
                throw new ArgumentNullException("The converted data source cannot be null.");

            if (parse == null)
                mParse = Parse;
            else
                mParse = parse;

            if (convertOutput == null)
                mConvertOutput = ConvertOutput;
            else
                mConvertOutput = convertOutput;

            mIntermediateOE = new ObjectEnumerator<C, I>(mDataConverted, mParse);

        }
        #endregion

        #region Parse(C data)
        /// <summary>
        /// This method can be used to parse the data instead of using a function.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns an individual record.</returns>
        protected virtual Tuple<I, C>? Parse(C data)
        {
            throw new NotImplementedException("Parse is not implemented.");
        }
        #endregion

        #region ConvertOutput(I item)
        /// <summary>
        /// This method converts an intermediate item in to the output item.
        /// </summary>
        /// <param name="item">The intermediate item to convert.</param>
        /// <returns>Returns a converted output record.</returns>
        protected virtual O ConvertOutput(I item)
        {
            throw new NotImplementedException("ConvertOutput is not implemented.");
        }
        #endregion
        #region ConvertSource(I item)
        /// <summary>
        /// This method converts one data source in to another.
        /// </summary>
        /// <param name="data">The data source to convert.</param>
        /// <returns>Returns a converted data source.</returns>
        protected virtual C ConvertSource(D data)
        {
            throw new NotImplementedException("Convert is not implemented.");
        }
        #endregion

        #region GetEnumerator()
        /// <summary>
        /// This enumerator returns the converted objects.
        /// </summary>
        /// <returns>returns a collection of converted objects.</returns>
        public IEnumerator<O> GetEnumerator()
        {
            return mIntermediateOE.Convert(mConvertOutput).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
    #endregion
}

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
using Ximura.Data;
#endregion
namespace Ximura
{
    /// <summary>
    /// This method is used to parse the data stream with an intermediate data type prior to conversion.
    /// </summary>
    /// <typeparam name="D">The source data type, i.e. stream</typeparam>
    /// <typeparam name="I">The intermediate data type.</typeparam>
    /// <typeparam name="O">The output record data type, i.e. int</typeparam>
    public abstract class IntermediateObjectEnumerator<D, I, O> : IEnumerable<O>
    {
        #region Declarations
        /// <summary>
        /// This function is used to convert the intermediate type in to the output type.
        /// </summary>
        protected Func<I, O> mConvert;
        /// <summary>
        /// This function is used to convert the data in to the intermediate data type.
        /// </summary>
        protected Func<D, Tuple<I, D>?> mParse;
        /// <summary>
        /// This enumerator encapsulates the base object with the intermediate type.
        /// </summary>
        protected ObjectEnumerator<D, I> mIntermediateOE;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the protected constructor.
        /// </summary>
        /// <param name="data">The data source.</param>
        /// <param name="parse">The parsing function.</param>
        /// <param name="convert">The conversion function to convert the intermediate type in to the output type.</param>
        protected IntermediateObjectEnumerator(D data, Func<D, Tuple<I, D>?> parse, Func<I, O> convert)
        {
            if (parse == null)
                mParse = Parse;
            else
                mParse = parse;

            mIntermediateOE = new ObjectEnumerator<D, I>(data, mParse);

            if (convert == null)
                mConvert = Convert;
            else
                mConvert = convert;
        }
        #endregion

        #region Parse(S data)
        /// <summary>
        /// This method can be used to parse the data instead of using a function.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns an individual record.</returns>
        protected virtual Tuple<I, D>? Parse(D data)
        {
            throw new NotImplementedException("Parse is not implemented.");
        }
        #endregion

        #region Convert(I item)
        /// <summary>
        /// This method converts an intermediate item in to the output item.
        /// </summary>
        /// <param name="item">The intermediate item to convert.</param>
        /// <returns>Returns a converted output record.</returns>
        protected virtual O Convert(I item)
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
            return mIntermediateOE.Convert(mConvert).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

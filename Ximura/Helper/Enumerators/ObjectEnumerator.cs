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
    /// <summary>
    /// This generic class is used to convert a data source (D) in to a set of discrete data items (O).
    /// </summary>
    /// <typeparam name="D">The source data type, i.e. stream</typeparam>
    /// <typeparam name="O">The output record data type, i.e. int</typeparam>
    public class ObjectEnumerator<D, O> : DisposableBase, IEnumerable<O>
    {
        #region Declarations
        /// <summary>
        /// This is the data source.
        /// </summary>
        protected D mData;
        /// <summary>
        /// This is the parsing function that converts the data collection in to a record.
        /// </summary>
        protected Func<D, Tuple<O, D>?> mParse;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the protected constructor.
        /// </summary>
        /// <param name="data">The data source.</param>
        /// <param name="parse">The parsing function.</param>
        public ObjectEnumerator(D data, Func<D, Tuple<O, D>?> parse)
        {
            mData = data;

            if (parse == null)
                mParse = Parse;
            else
                mParse = parse;
        }
        #endregion
        #region Dispose(bool disposing)
        /// <summary>
        /// This method checks cleans up and 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!mData.Equals(default(D)) && mData is IDisposable)
                    ((IDisposable)mData).Dispose();
                mData = default(D);
                mParse = null;
            }
        }
        #endregion

        #region Parse(S data)
        /// <summary>
        /// This method can be used to parse the data instead of using a function.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns an individual record.</returns>
        protected virtual Tuple<O, D>? Parse(D data)
        {
            throw new NotImplementedException("Parse is not implemented.");
        }
        #endregion

        #region GetEnumerator()
        /// <summary>
        /// This is the default object enumerator.
        /// </summary>
        /// <returns>Returns the object collection.</returns>
        public virtual IEnumerator<O> GetEnumerator()
        {
            DisposedCheck();
            return mData.Unfold(mParse).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            DisposedCheck();
            return GetEnumerator();
        }
        #endregion

    }
}

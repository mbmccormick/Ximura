#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2011 Paul Stancer.
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
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura
{
    /// <summary>
    /// This fragment holds the individual line items from the CSV file.
    /// </summary>
    public class CSVBaseFragment : MessageFragment<MessageTerminatorCSV>
    {
        #region Declarations
        Encoding mDefaultEncoding;
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public CSVBaseFragment()
            : base()
        {
        }
        #endregion

        #region Write(byte[] buffer, int offset, int count)
        /// <summary>
        /// This override writes the data and also shrinks the buffer to fit the actual size of the data
        /// as this will generally not be participating as part of a pool.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="offset">The byte offset.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>Returns the number of bytes read from the incoming buffer.</returns>
        public override int Write(byte[] buffer, int offset, int count)
        {
            int value = base.Write(buffer, offset, count);

            if (!this.CanWrite)
                ShrinkCapacityToFit();

            return value;
        }
        #endregion  

        #region Items
        /// <summary>
        /// This enumeration returns the items in the CSV line.
        /// </summary>
        public virtual IEnumerable<string> Items
        {
            get
            {
                yield break;
            }
        }
        #endregion  

        #region Reset()
        /// <summary>
        /// This override resets the default encoding.
        /// </summary>
        public override void Reset()
        {
            mDefaultEncoding = null;
            base.Reset();
        }
        #endregion // Reset()

        #region DataString
        /// <summary>
        /// This is the string representation of the data using the default encoding.
        /// </summary>
        public virtual string DataString
        {
            get
            {
                if (InternalBuffer == null)
                    return null;
                return DefaultEncoding.GetString(InternalBuffer, 0, (int)Length);
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The DataString cannot be set when the fragment is not initializing.");
                InternalBuffer = DefaultEncoding.GetBytes(value);
            }
        }
        #endregion // DataString
        #region DefaultEncoding
        /// <summary>
        /// This is the default encoding for the message
        /// </summary>
        public virtual Encoding DefaultEncoding
        {
            get
            {
                if (mDefaultEncoding == null)
                    return Encoding.UTF8;
                return mDefaultEncoding;
            }
            protected set { mDefaultEncoding = value; }
        }
        #endregion // DefaultEncoding

    }
}

#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// For more details see http://ximura.org
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This class is a base sentinel for the linked list.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public class VertexClassDataSentinel<T> : VertexClass<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassDataSentinel()
            : base()
        {
        }
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassDataSentinel(int hashID)
            : base()
        {
            HashID = hashID;
        }
        #endregion

        #region Value
        /// <summary>
        /// This override sets the value to the default of the type.
        /// </summary>
        public override T Value
        {
            get { return default(T); }
            set { }
        }
        #endregion // Value
        #region IsSentinel
        /// <summary>
        /// This property specifies that the data item is a sentinel.
        /// </summary>
        public override bool IsSentinel { get { return true; } }
        #endregion // IsSentinel

    }
}

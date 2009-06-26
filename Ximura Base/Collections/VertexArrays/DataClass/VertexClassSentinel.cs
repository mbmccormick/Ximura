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
    /// 
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public class VertexClassSentinel<T> : VertexClass<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassSentinel():base()
        {
            Up = null;
            Down = null;
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassSentinel(int hashID, VertexClass<T> down)
            : base()
        {
            Up = null;
            Down = down;
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

        #region Up
        /// <summary>
        /// The Up sentinel.
        /// </summary>
        public override VertexClass<T> Up { get; set; }
        #endregion // Up
        #region Down
        /// <summary>
        /// The down item.
        /// </summary>
        public override VertexClass<T> Down { get; set; }
        #endregion // Down

        #region IDisposable Members
        /// <summary>
        /// This method removes and object references.
        /// </summary>
        public override void Dispose()
        {
            Up = null;
            Down = null;

            base.Dispose();
        }
        #endregion
    }
}

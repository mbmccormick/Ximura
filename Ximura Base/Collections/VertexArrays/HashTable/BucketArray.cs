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
using System.Runtime.Serialization;
using System.Threading;

using Ximura;
using Ximura.Helper;
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// The bucket array holds optimized data for the bucket array.
    /// </summary>
    public class BucketArray<T> : FineGrainedLockArray<CollectionVertexStruct<T>>
    {
        #region Constructor
        /// <summary>
        /// This constructor sets the array capacity and the array offset integer.
        /// </summary>
        /// <param name="capacity">The array capacity.</param>
        /// <param name="offset">The array offset, this is the initial position of the array, the default should be 0.</param>
        public BucketArray(int capacity):base(capacity)
        {
        }
        #endregion // Constructor

        //#region Offset
        ///// <summary>
        ///// The offset for the bucket array is always 0, as this is handled elsewhere.
        ///// </summary>
        //public override int Offset{get{return 0;}}
        //#endregion // Offset
    }

    ///// <summary>
    ///// The bucket array holds optimized data for the bucket array.
    ///// </summary>
    //public class BucketArray2 : FineGrainedLockArray<int>
    //{
    //    #region Constructor
    //    /// <summary>
    //    /// This constructor sets the array capacity and the array offset integer.
    //    /// </summary>
    //    /// <param name="capacity">The array capacity.</param>
    //    /// <param name="offset">The array offset, this is the initial position of the array, the default should be 0.</param>
    //    public BucketArray2(int capacity)
    //        : base(capacity, 0)
    //    {
    //    }
    //    #endregion // Constructor

    //    #region Offset
    //    /// <summary>
    //    /// The offset for the bucket array is always 0, as this is handled elsewhere.
    //    /// </summary>
    //    public override int Offset { get { return 0; } }
    //    #endregion // Offset
    //}
}

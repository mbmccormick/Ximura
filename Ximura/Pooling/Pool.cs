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
﻿#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the base object pool.
    /// </summary>
    /// <typeparam name="T">The pool type.</typeparam>
    public class Pool<T> : PoolInvocator<T>
        where T : class, IXimuraPoolableObject, new()
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public Pool()
            : base(null)
        {

        }
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="remoteInvocator">This delegate can be used to create a new pool object.</param>
        public Pool(Func<T> remoteInvocator)
            : base(remoteInvocator)
        {
        }
        /// <summary>
        /// This is the pool constructor with default arguments.
        /// </summary>
        /// <param name="min">This is the minimum size.</param>
        /// <param name="max">This is the maximum size.</param>
        /// <param name="prefer">This is the prefered size.</param>
        public Pool(int min, int max, int prefer)
            : base(null, min, max, prefer)
        {

        }
        /// <summary>
        /// This is the pool constructor with default arguments.
        /// </summary>
        /// <param name="min">This is the minimum size.</param>
        /// <param name="max">This is the maximum size.</param>
        /// <param name="prefer">This is the prefered size.</param>
        /// <param name="remoteInvocator">This delegate can be used to create a new pool object.</param>
        public Pool(Func<T> remoteInvocator, int min, int max, int prefer)
            : base(remoteInvocator, min, max, prefer)
        {
        }
        #endregion // Constructors

        #region CreateNewPoolObject
        /// <summary>
        /// This method creates a new pool object of type T.
        /// </summary>
        /// <returns>Returns the new object.</returns>
        protected override T CreateNewPoolObject()
        {
            T obj;

            if (mRemoteInvocator == null)
                obj = new T();
            else
                obj = mRemoteInvocator();

            return obj;
        }
        #endregion // CreateNewPoolObject
    }
}

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
    public class PoolInvocator<T> : PoolBase<T>, IXimuraPool<T>
        where T : class, IXimuraPoolableObject
    {
        #region Declarations
        /// <summary>
        /// This is the pool invocator that can be used to invoke a new object.
        /// </summary>
        protected Func<T> mRemoteInvocator;
        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="remoteInvocator">This delegate can be used to create a new pool object.</param>
        public PoolInvocator(Func<T> remoteInvocator)
            : base()
        {
            mRemoteInvocator = remoteInvocator;
        }
        /// <summary>
        /// This is the pool constructor with default arguments.
        /// </summary>
        /// <param name="min">This is the minimum size.</param>
        /// <param name="max">This is the maximum size.</param>
        /// <param name="prefer">This is the prefered size.</param>
        /// <param name="remoteInvocator">This delegate can be used to create a new pool object.</param>
        public PoolInvocator(Func<T> remoteInvocator, int min, int max, int prefer)
            : base(min, max, prefer)
        {
            mRemoteInvocator = remoteInvocator;
        }
        #endregion // Constructors

        #region CreateNewPoolObject
        /// <summary>
        /// This method creates a new pool object of type T.
        /// </summary>
        /// <returns>Returns the new object.</returns>
        protected override T CreateNewPoolObject()
        {
            if (mRemoteInvocator == null)
                throw new ArgumentNullException("The remote invocator delegate is null.");
            T obj = mRemoteInvocator();

            return obj;
        }
        #endregion // CreateNewPoolObject
    }
}

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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the base class for object that can be used by the pool. Although you do not need to inherit 
    /// from this class, this class simplifies the construction of poolable objects.
    /// </summary>
    public class PoolableObjectBase : IXimuraPoolableObject
    {
        #region Declarations
        private Guid mTrackID = Guid.NewGuid();
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor. It is called when the object is created by the pool.
        /// Poolable objects must implement a public default constructor.
        /// </summary>
        public PoolableObjectBase()
        {
            Reset();
        }
        #endregion // Constructor

        #region CanPool
        /// <summary>
        /// This method returns true when the object can be pooled. 
        /// Override this property if you require more finegrained control.
        /// </summary>
        public virtual bool CanPool
        {
            get { return true; }
        }
        #endregion

        #region TrackID
        /// <summary>
        /// This is the track ID for the poolable object. This value will not change between the various
        /// uses of the object through the application.
        /// </summary>
        public Guid TrackID
        {
            get { return mTrackID; }
        }
        #endregion // TrackID

        #region Reset()
        /// <summary>
        /// This method is called when the object is first created or when the object is 
        /// returned to the pool. You should override this method and use it to reset all
        /// poolable object values to their default value and remeove any reference to external
        /// objects and/or data.
        /// </summary>
        public virtual void Reset()
        {

        }
        #endregion // Reset()

        #region IDisposable Members/Finalize
        ///// <summary>
        ///// The finalizer
        ///// </summary>
        //~PoolableObjectBase()
        //{
        //    Dispose(false);
        //}
        /// <summary>
        /// The dispose method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// The overrided dispose method
        /// </summary>
        /// <param name="disposing">True if this is called by dispose, false if this
        /// is called by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion
    }
}

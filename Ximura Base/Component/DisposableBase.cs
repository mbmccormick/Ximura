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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This base class implements the IDisposable interface.
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        #region Declarations
        /// <summary>
        /// This variables determines whether the collection has been disposed.
        /// </summary>
        private bool mDisposed = false;
        #endregion // Declarations

        #region Finalizer
        /// <summary>
        /// This is the finalizer for the collection.
        /// </summary>
        ~DisposableBase()
        {
            this.Dispose(false);
        }
        #endregion
        #region DisposedCheck()
        /// <summary>
        /// This method identifies when the collection has been disposed and throws an ObjectDisposedException.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">This exception is thrown when the collection has been disposed.</exception>
        protected internal void DisposedCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException(GetType().ToString(), "Collection has been disposed.");
        }
        #endregion // DisposedCheck()
        #region Dispose()
        /// <summary>
        /// This method disposes of the collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            mDisposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion // Dispose()
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of the data in the collection. You should override this method if you need to add
        /// custom dispose logic to your collection.
        /// </summary>
        /// <param name="disposing">The class is disposing, i.e. this is called by Dispose and not the finalizer.</param>
        protected abstract void Dispose(bool disposing);
        #endregion // Dispose(bool disposing)
    }
}

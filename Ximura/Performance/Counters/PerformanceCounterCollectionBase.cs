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
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Performance
{
    /// <summary>
    /// The abstract PerformanceCounterCollectionBase class is used to provide group functionality
    /// for related performance counters.
    /// </summary>
    public abstract class PerformanceCounterCollectionBase : IXimuraPerformanceCounterCollection
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PerformanceCounterCollectionBase()
        {

        }
        #endregion // Constructors


        #region ICollection<IXimuraPerformanceCounter> Members

        public void Add(IXimuraPerformanceCounter item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public bool Contains(IXimuraPerformanceCounter item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void CopyTo(IXimuraPerformanceCounter[] array, int arrayIndex)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public int Count
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public bool Remove(IXimuraPerformanceCounter item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<IXimuraPerformanceCounter> Members

        public IEnumerator<IXimuraPerformanceCounter> GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IXimuraPerformanceCounterCollection Members

        public virtual Guid AppID
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public virtual Guid CommandID
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public virtual Guid PCID
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public virtual string Name
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public virtual string Category
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        #endregion
    }
}

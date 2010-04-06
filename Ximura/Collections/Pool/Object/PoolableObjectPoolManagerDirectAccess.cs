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
    /// This object is an example of an object that implements the IXimuraPoolManagerDirectAccess interface.
    /// This interface allows a poolable object to use other object from the base pool manager. This
    /// is useful for poolable object that need to create a large number of child objects. The functionality
    /// for this is implemented in the base class, but is only activated if the pool manager detects 
    /// that the object implements this interface.
    /// </summary>
    public class PoolableObjectPoolManagerDirectAccess : PoolableReturnableObjectBase, IXimuraPoolManagerDirectAccess
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        public PoolableObjectPoolManagerDirectAccess():base()
        {

        }
    }
}

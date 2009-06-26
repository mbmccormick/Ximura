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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract class ConcurrentTreeBase : DisposableBase
    {
        #region IsReadOnly
        /// <summary>
        /// This value specifies whether items can be added or removed from the tree. The default is false.
        /// </summary>
        public virtual bool IsReadOnly { get { return false; } }
        #endregion // IsReadOnly

    }
}

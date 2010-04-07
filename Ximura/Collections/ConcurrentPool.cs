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
using System.Security.Permissions;
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This is the lock-free implementation of the Pool class using a hash-table based array.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    [DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public sealed class ConcurrentPool<T>
        where T: class
    {
    }
}

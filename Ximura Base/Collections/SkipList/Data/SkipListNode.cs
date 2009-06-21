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
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    #region SkipListNode<T>
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SkipListNode<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public SkipListNode() : this(default(T)) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SkipListNode(T value)
        {
            Value = value;
            Right = null;
            Up = null;
            Down = null;
        }
        /// <summary>
        /// 
        /// </summary>
        public T Value;
        /// <summary>
        /// 
        /// </summary>
        public SkipListNode<T> Right;
        /// <summary>
        /// 
        /// </summary>
        public SkipListNode<T> Up;
        /// <summary>
        /// 
        /// </summary>
        public SkipListNode<T> Down;
    }
    #endregion // SkipListNode<T>
}

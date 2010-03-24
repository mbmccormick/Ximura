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

using Ximura;
using Ximura.Helper;
using Ximura.Framework;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The context pool holds the collection of context objects.
    /// </summary>
    /// <typeparam name="CNTX">The context type.</typeparam>
    public class ContextPool<CNTX> : Pool<CNTX>
        where CNTX : class, IXimuraFSMContext, new()
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ContextPool():base()
        {

        }
        /// <summary>
        /// This is the pool constructor with default arguments.
        /// </summary>
        /// <param name="min">This is the minimum size.</param>
        /// <param name="max">This is the maximum size.</param>
        /// <param name="prefer">This is the prefered size.</param>
        public ContextPool(int min, int max, int prefer)
            : base(min, max, prefer)
        {

        }
        #endregion // Constructors
    }
}

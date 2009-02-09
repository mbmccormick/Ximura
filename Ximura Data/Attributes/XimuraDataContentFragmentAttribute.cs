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
using System.Reflection;
using System.IO;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// Summary description for XimuraDataContentFragmentAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class XimuraDataContentFragmentAttribute : Attribute
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public XimuraDataContentFragmentAttribute()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}

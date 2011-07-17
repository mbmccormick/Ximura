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
using System.Text;
using System.Linq;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// Provide special functionality to deal with Attribute.
    /// </summary>
    public static partial class AttributeHelper
    {
        #region Attribute<A>(this Type baseType)
        /// <summary>
        /// This extension method returns the first attribute of the specified type.
        /// </summary>
        /// <typeparam name="A">The attribute type.</typeparam>
        /// <param name="baseType">The base type to search.</param>
        /// <returns>Returns the first attribute or null if no attribute can be found.</returns>
        public static A Attribute<A>(this Type baseType) where A:Attribute
        {
            return baseType
                .Attributes<A>()
                .FirstOrDefault();
        }
        #endregion  
        #region Attributes<A>(this Type baseType)
        /// <summary>
        /// This extension method returns a list of the specified attribute for the base type specified.
        /// </summary>
        /// <typeparam name="A">The attribute type.</typeparam>
        /// <param name="baseType">The base type to search.</param>
        /// <returns>Returns a list of attributes.</returns>
        public static IEnumerable<A> Attributes<A>(this Type baseType) where A:Attribute
        {
            return baseType
                .GetCustomAttributes(typeof(A), true)
                .OfType<A>();
        }
        #endregion  
   
        #region PropertyAttributes<T>(Type baseType)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<PropertyInfo, T>> PropertyAttributes<T>(this Type baseType)
            where T : Attribute
        {
            foreach (PropertyInfo pi in baseType.GetProperties())
            {
                foreach (Attribute attr in pi.GetCustomAttributes(typeof(T), true))
                {
                    if (attr.GetType() != typeof(T))
                        continue;

                    yield return new KeyValuePair<PropertyInfo, T>(pi, attr as T);
                }
            }
        }
        #endregion  
    }
}

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
using System.Text;
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
    public static partial class ArrayHelper
    {
        #region Contains<T>(this IEnumerable<T> items, Predicate<T> action)
        /// <summary>
        /// This method scans a collection and returns true when an item is matched.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="items">The enumeration.</param>
        /// <param name="predic">The predicate that returns true when there is a match.</param>
        /// <returns>Returns true if an item is matched in the collection.</returns>
        public static bool Contains<T>(this IEnumerable<T> items, Predicate<T> predic)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predic == null) throw new ArgumentNullException("action");

            foreach (T item in items)
                if (predic(item))
                    return true;

            return false;
        }
        #endregion  
    }
}

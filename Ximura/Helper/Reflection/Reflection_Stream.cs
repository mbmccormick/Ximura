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
using System.Linq;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The reflection helper provides help on creating objects using reflection.
    /// </summary>
    public static partial class Reflection
    {

        #region ResourceLoadStream(string resourceName)
        /// <summary>
        /// This method reads a binary definition from an assembly based on the resource name.
        /// Note: this method will attempt to load the assembly if it is not loaded.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <returns>Returns an unmanaged stream containing the data.</returns>
        public static Stream ResourceLoadStream(string resourceName)
        {
            string[] items = resourceName.Split(new char[] { ',' }).Select(i => i.Trim()).ToArray();

            Assembly ass = Assembly.GetExecutingAssembly();

#if (!SILVERLIGHT)
            if (items.Length > 1)
            {
                var asses = AppDomain.CurrentDomain.GetAssemblies();
                ass = asses.SingleOrDefault(a => a.FullName.ToLowerInvariant() == items[1].ToLowerInvariant());

                if (ass == null)
                {
                    ass = AppDomain.CurrentDomain.Load(items[1]);

                    if (ass == null)
                        throw new ArgumentOutOfRangeException(
                            string.Format("The assembly cannot be resolved: {0}", items[1]));
                }
            }
#else
            if (ass == null)
                return null;
#endif
            return ass.GetManifestResourceStream(items[0]);
        }
        #endregion 

    }
}

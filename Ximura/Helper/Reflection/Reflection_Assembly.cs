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
        public static Assembly GetAssemblyFromName(AssemblyName theAssemblyName)
        {
            Assembly toReturn = null;

#if (!SILVERLIGHT)
            Assembly[] theAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly loopAssembly in theAssemblies)
            {
                if (theAssemblyName.FullName == loopAssembly.FullName)
                {
                    toReturn = loopAssembly;
                    break;
                }
            }

            if (toReturn == null)
            {
                toReturn = AppDomain.CurrentDomain.Load(theAssemblyName);
            }
#endif
            return toReturn;
        }
    }
}

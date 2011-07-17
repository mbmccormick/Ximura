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
        #region CreateTypeFromString
        /// <summary>
        /// This method returns the relevant type resolved from the nale
        /// </summary>
        /// <param name="typeName">The type name to resolve.</param>
        /// <returns>The type or null if the name cannot be resolved.</returns>
        public static Type CreateTypeFromString(string typeName)
        {
            return CreateTypeFromString(typeName, null, true);
        }
        /// <summary>
        /// This method returns the relevant type resolved from the nale
        /// </summary>
        /// <param name="typeName">The type name to resolve.</param>
        /// <param name="callingType">The type of the calling object.</param>
        /// <returns>The type or null if the name cannot be resolved.</returns>
        public static Type CreateTypeFromString(string typeName, Type callingType)
        {
            return CreateTypeFromString(typeName, callingType, true);
        }
        /// <summary>
        /// This method returns the relevant type resolved from the nale
        /// </summary>
        /// <param name="typeName">The type name to resolve.</param>
        /// <param name="callingType">The type of the calling object.</param>
        /// <param name="allowRelative">Set this to true if you allow relative paths to the type, i.e. less specific references.</param>
        /// <returns>The Type for the object or null if the type cannot be found.</returns>
        public static Type CreateTypeFromString(string typeName, Type callingType, bool allowRelative)
        {
            if (StringTypeLookupCache.ContainsKey(typeName))
                return StringTypeLookupCache[typeName];

            if (allowRelative)
                if (StringTypeRelativeLookupCache.ContainsKey(typeName))
                    return StringTypeRelativeLookupCache[typeName];

            Type toReturn = CreateTypeFromStringNoCache(typeName, callingType);

            if (toReturn != null || (toReturn == null && !allowRelative))
                return toReturn;

            if (toReturn == null)
                toReturn = SearchTypeByName(typeName);

            lock (syncReflection)
            {
                if (toReturn != null && !StringTypeRelativeLookupCache.ContainsKey(typeName))
                    StringTypeRelativeLookupCache.Add(typeName, toReturn);
            }

            return toReturn;
        }

        private static Type SearchTypeByName(string ContentType)
        {
            int point = ContentType.IndexOf(',', 0);
            int point2 = ContentType.IndexOf(',', point + 1);

            Type tryit = null;

            if (point2 > 0)
            {
                string reduced = ContentType.Substring(0, point2);
                tryit = CreateTypeFromString(reduced);
            }

            if (tryit == null && point > 0)
            {
                string reduced2 = ContentType.Substring(0, point);
                tryit = CreateTypeFromString(reduced2);
            }

            return tryit;
        }
        /// <summary>
        /// This method creates an object from the type name passed.
        /// </summary>
        /// <param name="typeName">The tpye name.</param>
        /// <param name="callingType">The calling type used as a reference.</param>
        /// <returns>Returns a new object of the required type 
        /// or null if the object cannot be resolved.</returns>
        public static Type CreateTypeFromStringNoCache(string typeName, Type callingType)
        {
            try
            {
#pragma warning disable
                Type classType = null;
                try
                {
                    classType = Type.GetType(typeName, false, true);
                }
                catch (Exception ex)
                {
                    //XimuraAppTrace.WriteLine("Type error: " + ex.Message, "Reflection", EventLogEntryType.Information);
                }

                if (classType == null)
                {
                    classType = searchType(typeName);
                }
#if (!SILVERLIGHT)
                if (classType == null && callingType != null)
                {
                    classType = searchType(typeName, callingType.Assembly.GetReferencedAssemblies());
                }
#endif
                lock (syncReflection)
                {
                    if (classType != null && !StringTypeLookupCache.ContainsKey(typeName))
                        StringTypeLookupCache.Add(typeName, classType);
                }
#pragma warning restore
                return classType;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region searchType
        private static Type searchType(string typeName)
        {
            //Assembly fredo = Assembly.LoadWithPartialName("XimuraWinLib");
            Type typeToReturn = null;

#if (!SILVERLIGHT)
            typeToReturn = searchType(typeName, AppDomain.CurrentDomain.GetAssemblies());

            if (typeToReturn == null && Assembly.GetEntryAssembly() != null)
                typeToReturn = searchType(typeName, Assembly.GetEntryAssembly().GetReferencedAssemblies());
#endif
            return typeToReturn;
        }

        private static Type searchType(string typeName, AssemblyName[] assemblyNames)
        {
            Type typeToReturn = null;

            foreach (AssemblyName thisAssName in assemblyNames)
            {
                Assembly thisAss = GetAssemblyFromName(thisAssName);

                if (thisAss != null)
                {
                    typeToReturn = searchType(typeName, thisAss);
                    if (typeToReturn != null) break;
                }
            }

            return typeToReturn;
        }

        private static Type searchType(string typeName, Assembly[] assemblies)
        {
            Type typeToReturn = null;

            foreach (Assembly thisAss in assemblies)
            {
                typeToReturn = searchType(typeName, thisAss);

                if (typeToReturn != null) break;
            }

            return typeToReturn;
        }

        private static Type searchType(string typeName, Assembly thisAss)
        {
            try
            {
                return thisAss.GetType(typeName, false);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

    }
}

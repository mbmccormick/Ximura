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
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
        #region Static declarations
        static Dictionary<string, Type> StringTypeLookupCache;
        static Dictionary<string, Type> StringTypeRelativeLookupCache;
        static Dictionary<Type, Dictionary<Type, bool>> sTypeValidationCache;
        static object syncReflection = new object();
        /// <summary>
        /// Binding Flags constant to be reused for all Reflection access methods.
        /// </summary>
        public const BindingFlags MemberAccess =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;
        #endregion // Static declarations.
        #region Static constructor
        /// <summary>
        /// This static constructor creates the Type/string lookup cache.
        /// </summary>
        static Reflection()
        {
            StringTypeLookupCache = new Dictionary<string, Type>();
            StringTypeRelativeLookupCache = new Dictionary<string, Type>();
            sTypeValidationCache = new Dictionary<Type, Dictionary<Type, bool>>();
        }
        #endregion // Static constructor.




        #region getTypesfromObjectArray
        /// <summary>
        /// This method returns a array of System types corresponsing to the object passed
        /// </summary>
        /// <param name="theObjects">An array of objects</param>
        /// <returns>An array of ooject types</returns>
        public static Type[] getTypesfromObjectArray(object[] theObjects)
        {
            Type[] theTypes = new Type[theObjects.Length];
            for (int loopit = 0; loopit < theObjects.Length; loopit++)
            {
                object item = theObjects[loopit];
                theTypes[loopit] = item == null ? null : item.GetType();
            }

            return theTypes;
        }
        #endregion

        #region ValidateInterface
        /// <summary>
        /// This public static method caches interface implementation for type checking. This is 
        /// used to boost performance when checking large amounts of types.
        /// </summary>
        /// <param name="objectType">The type for the object you wish to check.</param>
        /// <param name="interfaceType">The type for the interface you wish to check.</param>
        /// <returns>Returns true if the interface is implemented.</returns>
        public static bool ValidateInterface(Type objectType, Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("Parameter interfaceType must be an interface.", "interfaceType");

            if (sTypeValidationCache.ContainsKey(objectType) &&
                sTypeValidationCache[objectType].ContainsKey(interfaceType))
                return sTypeValidationCache[objectType][interfaceType];

            lock (syncReflection)
            {
                if (!sTypeValidationCache.ContainsKey(objectType))
                    sTypeValidationCache.Add(objectType, new Dictionary<Type, bool>());

                Dictionary<Type, bool> implementCache = sTypeValidationCache[objectType];

                try
                {
                    InterfaceMapping map = objectType.GetInterfaceMap(interfaceType);
                    implementCache[interfaceType] = true;
                    return true;
                }
                catch (ArgumentNullException sanex)
                {
                    implementCache[interfaceType] = false;
                    return false;
                }  
                catch (ArgumentException saex)
                {
                    implementCache[interfaceType] = false;
                    return false;
                }
                catch (InvalidOperationException iopex)
                {
                    implementCache[interfaceType] = false;
                    return false;
                }
                catch (Exception ex)
                {
                    //XimuraAppTrace.WriteLine("ValidateIsPoolable(Type objectType) -> Unexpected error: "
                    //    + ex.Message, "PoolManager", EventLogEntryType.Warning);
                    implementCache[interfaceType] = false;
                    return false;
                }
            }

            //return sTypeValidationCache[objectType][checkType];;
        }
        #endregion // ValidateInterface

        private static string StripSlash(string input)
        {
            return input.Trim('/');
        }

    }
}

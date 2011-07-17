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
        #region CreateObjectFromType
        /// <summary>
        /// This method will create an instance of a class object from the typeName.
        /// </summary>
        /// <param name="typeName">The typeName to create an instance of.</param>
        /// <returns>The new object.</returns>
        public static object CreateObjectFromType(string typeName)
        {
            return CreateObjectFromType(typeName, (object[])null, (Type)null);
        }
        /// <summary>
        /// This method will create an instance of a class object from the typeName.
        /// </summary>
        /// <param name="typeName">The typeName to create an instance of.</param>
        /// <param name="parameters">A parameter array of object type for the class constructor</param>
        /// <returns>The new object.</returns>
        public static object CreateObjectFromType(string typeName, object[] parameters)
        {
            return CreateObjectFromType(typeName, parameters, (Type)null);
        }
        /// <summary>
        /// This method will create an instance of a class object from the typeName.
        /// </summary>
        /// <param name="typeName">The typeName to create an instance of.</param>
        /// <param name="parameters">A parameter array of object type for the class constructor</param>
        /// <param name="callingType">The calling type. This is used to search referenced assemblies.</param>
        /// <returns>The new object.</returns>
        public static object CreateObjectFromType(string typeName, object[] parameters, Type callingType)
        {
            Type classType = CreateTypeFromString(typeName, callingType);

            if (classType == null)
                return null;
            else
                return CreateObjectFromType(classType, parameters);
        }
        /// <summary>
        /// This method will create an instance of a class object from the class type.
        /// </summary>
        /// <param name="classType">The class type to create an instance of.</param>
        /// <returns>The new object.</returns>
        public static object CreateObjectFromType(Type classType)
        {
            return CreateObjectFromType(classType, null, null);
        }
        /// <summary>
        /// This method will create an instance of a class object from the class type.
        /// </summary>
        /// <param name="classType">The class type to create an instance of.</param>
        /// <param name="parameters">A parameter array of object type for the class constructor</param>
        /// <returns>The new object.</returns>
        public static object CreateObjectFromType(Type classType, object[] parameters)
        {
            return CreateObjectFromType(classType, parameters, parameters == null ? null : getTypesfromObjectArray(parameters));
        }
        /// <summary>
        /// This method will create an instance of a class object from the class type.
        /// </summary>
        /// <param name="classType">The class type to create an instance of.</param>
        /// <param name="parameters">A parameter array of object type for the class constructor</param>
        /// <param name="types">The types collection.</param>
        /// <returns>The new object.</returns>
        public static object CreateObjectFromType(Type classType, object[] parameters, Type[] types)
        {
            try
            {
                if (classType == null || !classType.IsClass)
                    throw new ArgumentException("classType must be a class and cannot be null", "classType");

                if (parameters == null || parameters.Length == 0)
                {
                    ConstructorInfo TypeConstruct = classType.GetConstructor(System.Type.EmptyTypes);
                    return TypeConstruct.Invoke(null);
                }
                else
                {
                    ConstructorInfo TypeConstruct = classType.GetConstructor(types);

                    object obj = TypeConstruct.Invoke(parameters);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}

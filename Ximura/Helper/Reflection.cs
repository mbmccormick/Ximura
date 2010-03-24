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
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The reflection helper provides help on creating objects using reflection.
    /// </summary>
    public static class Reflection
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
        #endregion
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
                if (classType == null && callingType != null)
                {
                    classType = searchType(typeName, callingType.Assembly.GetReferencedAssemblies());
                }

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
            Type typeToReturn = searchType(typeName, AppDomain.CurrentDomain.GetAssemblies());


            if (typeToReturn == null && Assembly.GetEntryAssembly() != null)
                typeToReturn = searchType(typeName, Assembly.GetEntryAssembly().GetReferencedAssemblies());

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
                return thisAss.GetType(typeName, false, true);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Assembly GetAssemblyFromName(AssemblyName theAssemblyName)
        {
            Assembly[] theAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly toReturn = null;
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

            return toReturn;
        }

        public static Stream ResolveResourceFromName(string resourceName)
        {
            Stream toReturn = ResolveResourceFromName(resourceName, AppDomain.CurrentDomain.GetAssemblies());
            if (toReturn == null)
                toReturn = ResolveResourceFromName(resourceName, Assembly.GetExecutingAssembly().GetReferencedAssemblies());

            return toReturn;
        }

        public static Stream ResolveResourceFromName(string resourceName, AssemblyName[] Asses)
        {
            Stream streamResx = null;

            foreach (AssemblyName thisAssName in Asses)
            {
                Assembly thisAss = GetAssemblyFromName(thisAssName);

                if (thisAss != null)
                {
                    streamResx = thisAss.GetManifestResourceStream(resourceName);
                    if (streamResx != null)
                        return streamResx;
                }
            }

            return null;
        }
        public static Stream ResolveResourceFromName(string resourceName, Assembly[] Asses)
        {
            Stream streamResx = null;

            foreach (Assembly thisAss in Asses)
            {
                if (thisAss != null)
                {
                    streamResx = thisAss.GetManifestResourceStream(resourceName);
                    if (streamResx != null)
                        return streamResx;
                }
            }

            return null;
        }
        #endregion

        #region CreateObjectFromType
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

        #region Get/SetProperty()


        /// <summary>
        /// Retrieve a property value from an object dynamically. This is a simple version
        /// that uses Reflection calls directly. It doesn't support indexers.
        /// </summary>
        /// <param name="Object">Object to make the call on</param>
        /// <param name="Property">Property to retrieve</param>
        /// <returns>Object - cast to proper type</returns>
        public static object GetProperty(object Object, string Property)
        {
            return Object.GetType().GetProperty(Property, Reflection.MemberAccess).GetValue(Object, null);
        }

        /// <summary>
        /// Sets the property on an object. This is a simple method that uses straight Reflection 
        /// and doesn't support indexers.
        /// </summary>
        /// <param name="Object">Object to set property on</param>
        /// <param name="Property">Name of the property to set</param>
        /// <param name="Value">value to set it to</param>
        public static void SetProperty(object Object, string Property, object Value)
        {
            Object.GetType().GetProperty(Property, Reflection.MemberAccess).SetValue(Object, Value, null);
        }
        #endregion
        #region Get/SetPropertyEx()
        /// <summary>
        /// Returns a property or field value using a base object and sub members including . syntax.
        /// For example, you can access: this.oCustomer.oData.Company with (this,"oCustomer.oData.Company")
        /// This method also supports indexers in the Property value such as:
        /// Customer.DataSet.Tables["Customers"].Rows[0]
        /// </summary>
        /// <param name="Parent">Parent object to 'start' parsing from. Typically this will be the Page.</param>
        /// <param name="Property">The property to retrieve. Example: 'Customer.Entity.Company'</param>
        /// <returns></returns>
        public static object GetPropertyEx(object Parent, string Property)
        {
            Type Type = Parent.GetType();

            int lnAt = Property.IndexOf(".");
            if (lnAt < 0)
            {
                // *** Complex parse of the property    
                return GetPropertyInternal(Parent, Property);
            }

            // *** Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
            string Main = Property.Substring(0, lnAt);
            string Subs = Property.Substring(lnAt + 1);

            // *** Retrieve the next . section of the property
            object Sub = GetPropertyInternal(Parent, Main);

            // *** Now go parse the left over sections
            return GetPropertyEx(Sub, Subs);
        }
        /// <summary>
        /// Sets a value on an object. Supports . syntax for named properties
        /// (ie. Customer.Entity.Company) as well as indexers.
        /// </summary>
        /// <param name="Object Parent">
        /// Object to set the property on.
        /// </param>
        /// <param name="String Property">
        /// Property to set. Can be an object hierarchy with . syntax and can 
        /// include indexers. Examples: Customer.Entity.Company, 
        /// Customer.DataSet.Tables["Customers"].Rows[0]
        /// </param>
        /// <param name="Object Value">
        /// Value to set the property to
        /// </param>
        public static object SetPropertyEx(object Parent, string Property, object Value)
        {
            Type Type = Parent.GetType();

            // *** no more .s - we got our final object
            int lnAt = Property.IndexOf(".");
            if (lnAt < 0)
            {
                SetPropertyInternal(Parent, Property, Value);
                return null;
            }

            // *** Walk the . syntax
            string Main = Property.Substring(0, lnAt);
            string Subs = Property.Substring(lnAt + 1);

            object Sub = GetPropertyInternal(Parent, Main);

            SetPropertyEx(Sub, Subs, Value);

            return null;
        }
        #endregion
        #region Get/SetPropertyInternal()
        /// <summary>
        /// Parses Properties and Fields including Array and Collection references.
        /// Used internally for the 'Ex' Reflection methods.
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Property"></param>
        /// <returns></returns>
        private static object GetPropertyInternal(object Parent, string Property)
        {
            if (Property == "this" || Property == "me")
                return Parent;

            object Result = null;
            string PureProperty = Property;
            string Indexes = null;
            bool IsArrayOrCollection = false;

            // *** Deal with Array Property
            if (Property.IndexOf("[") > -1)
            {
                PureProperty = Property.Substring(0, Property.IndexOf("["));
                Indexes = Property.Substring(Property.IndexOf("["));
                IsArrayOrCollection = true;
            }

            // *** Get the member
            MemberInfo Member = Parent.GetType().GetMember(PureProperty, Reflection.MemberAccess)[0];
            if (Member.MemberType == MemberTypes.Property)
                Result = ((PropertyInfo)Member).GetValue(Parent, null);
            else
                Result = ((FieldInfo)Member).GetValue(Parent);

            if (IsArrayOrCollection)
            {
                Indexes = Indexes.Replace("[", "").Replace("]", "");

                if (Result is Array)
                {
                    int Index = -1;
                    int.TryParse(Indexes, out Index);
                    Result = CallMethod(Result, "GetValue", Index);
                }
                else if (Result is ICollection)
                {
                    if (Indexes.StartsWith("\""))
                    {
                        // *** String Index
                        Indexes = Indexes.Trim('\"');
                        Result = CallMethod(Result, "get_Item", Indexes);
                    }
                    else
                    {
                        // *** assume numeric index
                        int Index = -1;
                        int.TryParse(Indexes, out Index);
                        Result = CallMethod(Result, "get_Item", Index);
                    }
                }

            }

            return Result;
        }
        /// <summary>
        /// Parses Properties and Fields including Array and Collection references.
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Property"></param>
        /// <returns></returns>
        private static object SetPropertyInternal(object Parent, string Property, object Value)
        {
            if (Property == "this" || Property == "me")
                return Parent;

            object Result = null;
            string PureProperty = Property;
            string Indexes = null;
            bool IsArrayOrCollection = false;

            // *** Deal with Array Property
            if (Property.IndexOf("[") > -1)
            {
                PureProperty = Property.Substring(0, Property.IndexOf("["));
                Indexes = Property.Substring(Property.IndexOf("["));
                IsArrayOrCollection = true;
            }

            if (!IsArrayOrCollection)
            {
                // *** Get the member
                MemberInfo Member = Parent.GetType().GetMember(PureProperty, Reflection.MemberAccess)[0];
                if (Member.MemberType == MemberTypes.Property)
                    ((PropertyInfo)Member).SetValue(Parent, Value, null);
                else
                    ((FieldInfo)Member).SetValue(Parent, Value);
                return null;
            }
            else
            {
                // *** Get the member
                MemberInfo Member = Parent.GetType().GetMember(PureProperty, Reflection.MemberAccess)[0];
                if (Member.MemberType == MemberTypes.Property)
                    Result = ((PropertyInfo)Member).GetValue(Parent, null);
                else
                    Result = ((FieldInfo)Member).GetValue(Parent);
            }
            if (IsArrayOrCollection)
            {
                Indexes = Indexes.Replace("[", "").Replace("]", "");

                if (Result is Array)
                {
                    int Index = -1;
                    int.TryParse(Indexes, out Index);
                    Result = CallMethod(Result, "SetValue", Value, Index);
                }
                else if (Result is ICollection)
                {
                    if (Indexes.StartsWith("\""))
                    {
                        // *** String Index
                        Indexes = Indexes.Trim('\"');
                        Result = CallMethod(Result, "set_Item", Indexes, Value);
                    }
                    else
                    {
                        // *** assume numeric index
                        int Index = -1;
                        int.TryParse(Indexes, out Index);
                        Result = CallMethod(Result, "set_Item", Index, Value);
                    }
                }

            }

            return Result;
        }
        #endregion
        #region CallMethod()
        /// <summary>
        /// Calls a method on an object dynamically.
        /// </summary>
        /// <param name="Params"></param>
        /// 1st - Method name, 2nd - 1st parameter, 3rd - 2nd parm etc.
        /// <returns></returns>
        public static object CallMethod(object Object, string Method, params object[] Params)
        {
            return Object.GetType().InvokeMember(Method, Reflection.MemberAccess | BindingFlags.InvokeMethod, null, Object, Params);
        }
        #endregion

        #region ResourceLoadFromUri(Uri location)
        /// <summary>
        /// This method loads a resource from the xmrres resource location specified.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Returns a byte array containing the resource.</returns>
        /// <exception cref="System.ArgumentException">This exception is thrown when the resource cannot be resolved.</exception>
        public static byte[] ResourceLoadFromUri(Uri location)
        {
            ResourceCheck(location);

            Type locationType = ResourceResolveType(location);
            byte[] data = ResourceResolve(locationType, location.Segments[2]);

            return data;
        }
        #endregion // ResourceLoadFromUri(Uri location)
        #region ResourceResolveType(Uri location)
        /// <summary>
        /// This method will return the type specified in the xmrres resource string.
        /// </summary>
        /// <param name="location">The resource location.</param>
        /// <returns>Returns the type specified.</returns>
        public static Type ResourceResolveType(Uri location)
        {
            ResourceCheck(location);
            string typeName = StripSlash(location.Segments[1]) + ", " + location.Host;
            Type theType = Type.GetType(typeName, false, true);

            if (theType == null)
                theType = CreateTypeFromString(typeName);

            return theType;
        }
        #endregion // ResourceResolveType(Uri location)
        #region ResourceCheck(Uri location)
        /// <summary>
        /// This method checks whether the xmrres resource uri is correctly formatted.
        /// </summary>
        /// <param name="location">The xmrres resource location.</param>
        public static void ResourceCheck(Uri location)
        {
            if (location.Scheme.ToLower() != "xmrres")
                throw new NotSupportedException(@"The scheme """ + location.Scheme + @""" is not supported.");

            if (location.Segments.Length < 3)
                throw new ArgumentException(
                    "location does not have the required number of parts: " + location.ToString(), "location");
        }
        #endregion // ResourceCheck(Uri location)
        #region ResourceResolve(Type locationType, string resource)
        /// <summary>
        /// This method resolves an embedded resource as a byte array.
        /// </summary>
        /// <param name="locationType">The type</param>
        /// <param name="resource">The resource to resolve.</param>
        /// <returns>Returns a byte array.</returns>
        /// <exception cref="System.ArgumentException">This exception is thrown when the resource cannot be resolved.</exception>
        public static byte[] ResourceResolve(Type locationType, string resource)
        {
            byte[] blob;

            using (Stream data = locationType.Assembly.GetManifestResourceStream(resource))
            {
                if (data == null)
                    throw new ArgumentException(@"The resource """ + resource + @""" cannot be resolved.");

                blob = new byte[data.Length];
                data.Read(blob, 0, (int)data.Length);
            }

            return blob;
        }
        #endregion // ResourceResolve(Type locationType, string resource)

        public static Stream ResourceLoadFromUriAsStream(Uri location)
        {
            ResourceCheck(location);

            Type locationType = ResourceResolveType(location);
            return ResourceResolveAsStream(locationType, location.Segments[2]);
        }

        public static Stream ResourceResolveAsStream(Type locationType, string resource)
        {
            Stream data = locationType.Assembly.GetManifestResourceStream(resource);

            if (data == null)
                throw new ArgumentException(@"The resource """ + resource + @""" cannot be resolved.");

            return data;
        }


        private static string StripSlash(string input)
        {
            return input.Trim('/');
        }

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

            return ass.GetManifestResourceStream(items[0]);
        }
        #endregion 

    }
}

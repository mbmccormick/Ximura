﻿#region Copyright
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
    }
}

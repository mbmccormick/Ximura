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
using System.Security.Cryptography;
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// Provide special functionality to deal with Attribute.
    /// </summary>
    public static class AttributeHelper
    {
        public static A Attribute<A>(this Type baseType) where A:Attribute
        {
            return baseType
                .GetCustomAttributes(typeof(A), true)
                .OfType<A>()
                .FirstOrDefault();
        }

        public static IEnumerable<A> Attributes<A>(this Type baseType) where A : Attribute
        {
            return baseType
                .GetCustomAttributes(typeof(A), true)
                .OfType<A>();
        }

        public static T GetAttribute<T>(Type baseType)
            where T : Attribute
        {
            object[] attrs = baseType.GetCustomAttributes(typeof(T), true);
            if (attrs.Length == 0)
                return null;

            Attribute attrToReturn = null;

            foreach (object attr in attrs)
            {
                if (attr.GetType() != typeof(T))
                    continue;
                return (T)attr;
            }

            return null;
        }

        public static T[] GetAttributes<T>(Type baseType)
            where T : Attribute
        {
            object[] attrs = baseType.GetCustomAttributes(typeof(T), true);

            List<T> attrsToReturn = new List<T>();

            foreach (object attr in attrs)
            {
                if (attr.GetType() != typeof(T))
                    continue;
                attrsToReturn.Add((T)attr);
            }

            return attrsToReturn.ToArray();
        }

        public static List<KeyValuePair<PropertyInfo, T>> GetPropertyAttributes<T>(Type baseType)
            where T : Attribute
        {
            List<KeyValuePair<PropertyInfo, T>> data = new List<KeyValuePair<PropertyInfo, T>>();
            try
            {
                foreach (PropertyInfo pi in baseType.GetProperties())
                {
                    foreach (Attribute attr in pi.GetCustomAttributes(typeof(T), true))
                    {
                        if (attr.GetType() != typeof(T))
                            continue;
                        data.Add(new KeyValuePair<PropertyInfo, T>(pi, attr as T));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return data;
        }



    }
}

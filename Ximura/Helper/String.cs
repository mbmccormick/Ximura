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

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// Provide special functionality to deal with string
    /// </summary>
    public static class StringHelper
    {
        #region XmlDecode()
        /// <summary>
        /// Handle the special xml decode.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string XmlDecode(string source)
        {
            return string.IsNullOrEmpty(source) ? source : source.Replace("&lt;", "<").Replace("&gt;", ">").Replace(@"\n", "\n");
        }
        #endregion
        #region StringToTypedValue
        /// <summary>
        /// Turns a string into a typed value. Useful for auto-conversion routines
        /// like form variable or XML parsers.
        /// <seealso>Class wwUtils</seealso>
        /// </summary>
        /// <param name="SourceString">
        /// The string to convert from
        /// </param>
        /// <param name="TargetType">
        /// The type to convert to
        /// </param>
        /// <param name="Culture">
        /// Culture used for numeric and datetime values.
        /// </param>
        /// <returns>object. Throws exception if it cannot be converted.</returns>
        public static object StringToTypedValue(string SourceString, Type TargetType, CultureInfo Culture)
        {
            object Result = null;

            if (TargetType == typeof(string))
                Result = SourceString;
            else if (TargetType == typeof(int))
                Result = int.Parse(SourceString, NumberStyles.Integer, Culture.NumberFormat);
            else if (TargetType == typeof(byte))
                Result = Convert.ToByte(SourceString);
            else if (TargetType == typeof(decimal))
                Result = Decimal.Parse(SourceString, NumberStyles.Any, Culture.NumberFormat);
            else if (TargetType == typeof(double))
                Result = Double.Parse(SourceString, NumberStyles.Any, Culture.NumberFormat);
            else if (TargetType == typeof(bool))
            {
                if (SourceString.ToLower() == "true" || SourceString.ToLower() == "on" || SourceString == "1")
                    Result = true;
                else
                    Result = false;
            }
            else if (TargetType == typeof(DateTime))
                Result = Convert.ToDateTime(SourceString, Culture.DateTimeFormat);
            else if (TargetType.IsEnum)
                Result = Enum.Parse(TargetType, SourceString);
            else
            {
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(TargetType);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                    Result = converter.ConvertFromString(null, Culture, SourceString);
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Type Conversion not handled in StringToTypedValue for " +
                                                    TargetType.Name + " " + SourceString);
                    throw (new ApplicationException("Type Conversion not handled in StringToTypedValue"));
                }
            }

            return Result;
        }

        /// <summary>
        /// Turns a string into a typed value. Useful for auto-conversion routines
        /// like form variable or XML parsers.
        /// </summary>
        /// <param name="SourceString">The input string to convert</param>
        /// <param name="TargetType">The Type to convert it to</param>
        /// <returns>object reference. Throws Exception if type can not be converted</returns>
        public static object StringToTypedValue(string SourceString, Type TargetType)
        {
            return StringToTypedValue(SourceString, TargetType, CultureInfo.CurrentCulture);
        }
        #endregion
    }
}

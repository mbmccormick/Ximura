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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class provides extension methods for classes that implement IXimuraContent.
    /// </summary>
    public static partial class ContentHelper
    {
        #region FormatterResolve
        /// <summary>
        /// This method resolves the correct formatter for the content and the blob.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter FormatterResolve(Type contentType)
        {
            try
            {
                XimuraContentSerializationAttribute csAttr =
                    AH.GetAttribute<XimuraContentSerializationAttribute>(contentType);

                if (csAttr == null || csAttr.Formatter == null)
                    return new ContentFormatter();

                return csAttr.Formatter;
            }
            catch (Exception ex)
            {
                return new ContentFormatter();
            }
        }
        /// <summary>
        /// This method resolves the formatter for the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter FormatterResolve(this IXimuraContent content)
        {
            Type contentType = content.GetType();
            IXimuraFormatter formatter = FormatterResolve(contentType);
            return formatter;
        }
        /// <summary>
        /// This method resolves the formatter for the blob.
        /// </summary>
        /// <param name="blob">The blob.</param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter FormatterResolve(byte[] blob)
        {
            return new ContentFormatter();
        }
        /// <summary>
        /// This method resolves the formatter for the blob.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter FormatterResolve(Stream stream)
        {
            return new ContentFormatter();
        }
        #endregion

    }
}

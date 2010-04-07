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
using System.Runtime.Serialization;

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
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <returns>A content object</returns>
        public static IXimuraContent DeserializeToContent(byte[] blob, IXimuraPoolManager pMan)
        {
            return DeserializeToContent(blob, 0, blob.Length, pMan);
        }
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>A content object</returns>
        public static IXimuraContent DeserializeToContent(byte[] blob, IXimuraFormatter formatter, IXimuraPoolManager pMan)
        {
            return DeserializeToContent(blob, 0, blob.Length, formatter, pMan);
        }
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <returns>A content object</returns>
        public static IXimuraContent DeserializeToContent(byte[] blob, int index, int length, IXimuraPoolManager pMan)
        {
            IXimuraFormatter formatter = ContentHelper.FormatterResolve(blob);

            return DeserializeToContent(blob, index, length, formatter, pMan);
        }
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>A content object</returns>
        public static IXimuraContent DeserializeToContent(byte[] blob, int index, int length,
            IXimuraFormatter formatter, IXimuraPoolManager pMan)
        {
            using (MemoryStream memStream = new MemoryStream(blob, index, length))
            {
                return DeserializeToContent(memStream, formatter, pMan);
            }
        }
        /// <summary>
        /// This method deserializes the stream in to the correct content entity.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <returns>The content entity.</returns>
        public static IXimuraContent DeserializeToContent(Stream stream, IXimuraPoolManager pMan)
        {
            IXimuraFormatter formatter = ContentHelper.FormatterResolve(stream);
            return DeserializeToContent(stream, formatter, pMan);
        }
        /// <summary>
        /// This method deserializes the stream in to the correct content entity.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="pMan">The pool manager to retrieve a new object.</param>
        /// <returns>The content entity.</returns>
        public static IXimuraContent DeserializeToContent(Stream stream, IXimuraFormatter formatter, IXimuraPoolManager pMan)
        {
            IXimuraContent content = formatter.Deserialize(stream, pMan) as IXimuraContent;

            return content;
        }

    }
}

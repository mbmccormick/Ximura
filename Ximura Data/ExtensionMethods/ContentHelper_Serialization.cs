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
        #region SerializeToStream(this IXimuraContent content, Stream stream)
        /// <summary>
        /// This method serializes an entity to a stream.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <param name="stream">The stream to serialize to.</param>
        public static void SerializeToStream(this IXimuraContent content, Stream stream)
        {
            IFormatter formatter = content.FormatterResolve();
            SerializeToStream(content, stream, formatter);
        }
        #endregion // SerializeToStream(this IXimuraContent content, Stream stream)
        #region  SerializeToStream(this IXimuraContent content, Stream stream, IFormatter formatter)
        /// <summary>
        /// This method serializes an entity to a stream.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="formatter">The formatter to use for the serialization.</param>
        public static void SerializeToStream(this IXimuraContent content, Stream stream, IFormatter formatter)
        {
            formatter.Serialize(stream, content);
        }
        #endregion //  SerializeToStream(this IXimuraContent content, Stream stream, IFormatter formatter)

        #region Serialize(this IXimuraContent content)
        /// <summary>
        /// This method serializes an entity in to a blob.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <returns>A byte array containing the serialized entity.</returns>
        public static byte[] Serialize(this IXimuraContent content)
        {
            Type contentType = content.GetType();
            IFormatter formatter = ContentHelper.FormatterResolve(contentType);
            return Serialize(content, formatter);
        }
        #endregion
        #region Serialize(this IXimuraContent content, IFormatter formatter)
        /// <summary>
        /// This method serializes an entity in to a blob.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <param name="formatter">The formatter to serialize the entity with.</param>
        /// <returns>A byte array containing the serialized entity.</returns>
        public static byte[] Serialize(this IXimuraContent content, IFormatter formatter)
        {
            byte[] returnByte = null;

            using (MemoryStream memStream = new MemoryStream())
            {
                if (content is Content)
                {
                    SerializeToStream(content as Content, memStream, formatter);
                }

                long length = memStream.Length;
                returnByte = new byte[length];

                Array.Copy(memStream.GetBuffer(), 0, returnByte, 0, length);
            }
            return returnByte;
        }
        #endregion
    }
}

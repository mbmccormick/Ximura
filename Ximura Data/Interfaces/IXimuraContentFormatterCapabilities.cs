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
using System.IO;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;

#endregion
namespace Ximura.Data
{
    /// <summary>
    /// Summary description for IXimuraContentFormatterCapabilities.
    /// </summary>
    public interface IXimuraContentFormatterCapabilities
    {
        /// <summary>
        /// The maximum version value up to a maximum of 63.
        /// </summary>
        int Max { get;set;}
        /// <summary>
        /// The minimum version number, between 0 and 63.
        /// </summary>
        int Min { get;set;}
        /// <summary>
        /// The revision number between 0 and 63.
        /// </summary>
        int Revision { get;set;}
        /// <summary>
        /// The Type ID
        /// </summary>
        Guid TypeID { get;}
        /// <summary>
        /// The Content ID
        /// </summary>
        Guid ContentID { get;}
        /// <summary>
        /// The Version ID
        /// </summary>
        Guid VersionID { get;}
        /// <summary>
        /// The content type specified.
        /// </summary>
        string ContentType { get;}
        /// <summary>
        /// The content base type specified.
        /// </summary>
        string ContentBaseType { get;}
        /// <summary>
        /// This specifies whether the deserializer can use the best match for creating
        /// the object from the type.
        /// </summary>
        bool AllowRelativeType { get;}
        /// <summary>
        /// This method identifies whether the content supports a base type.
        /// </summary>
        bool SupportsBaseType { get;}
        /// <summary>
        /// This method loads the header from the stream.
        /// </summary>
        /// <param name="inStream">The stream to read from.</param>
        void Load(Stream inStream);
        /// <summary>
        /// This method outputs the header for the stream.
        /// </summary>
        /// <param name="outStream">The stream to write to.</param>
        /// <param name="entity">The entity specified.</param>
        void Output(Stream outStream, IXimuraContent entity, SerializationInfo info);
        /// <summary>
        /// This is the helper used to read and write objects to the stream.
        /// </summary>
        IXimuraContentSerializationReaderWriter RWHelper { get;}
        /// <summary>
        /// This property indicates whether the body data is compressed.
        /// </summary>
        bool StreamCompressed { get;}
    }
}
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
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Xml;
using System.Data;
using System.Reflection;
using System.IO.Compression;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This class is used to serialize and deserialize the content.
    /// </summary>
    public class ContentFormatter : FormatterBase
    {
        #region Declarations
        /// <summary>
        /// The default binary writer.
        /// </summary>
        protected BinaryWriter mWriter;
        /// <summary>
        /// The default binary reader.
        /// </summary>
        protected BinaryReader mReader;
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ContentFormatter()
            : base()
        {
        }
        #endregion // Constructor

        #region Serialize
        /// <summary>
        /// This method serializes the content to the stream.
        /// </summary>
        /// <param name="outStream">The stream.</param>
        /// <param name="graph">The content.</param>
        public override void Serialize(Stream outStream, object graph)
        {
            Content entity = graph as Content;
            if (entity == null)
                throw new ContentFormatterException("Only Content can be serialized.");

            SerializationInfo info = new SerializationInfo(entity.GetType(), new FormatterConverter());

            //Get the bit values from the relevant fields
            entity.GetObjectData(info, Context);

            WriteByteMarkers(outStream);

            //Get the header object
            IXimuraContentFormatterCapabilities header = GetHeader();

            //Write the header bytes to the stream.
            header.Output(outStream, entity, info);

            //Serialize the content to the stream. The output streamis in the header ReaderWriter helper.
            ContentSerializeInternal(header, entity, info);
        }
        #endregion // Serialize

        #region Deserialize
        /// <summary>
        /// This method deserializes the stream to a content object.
        /// </summary>
        /// <param name="inStream">The stream containing the data.</param>
        /// <returns>The deserialized object.</returns>
        public override object Deserialize(Stream inStream)
        {
            return Deserialize(inStream, null);
        }
        /// <summary>
        /// This method deserializes the stream to a content object.
        /// </summary>
        /// <param name="inStream">The stream containing the data.</param>
        /// <param name="pMan">The pool manaer containing the required pool.</param>
        /// <returns>The deserialized object.</returns>
        public override object Deserialize(Stream inStream, IXimuraPoolManager pMan)
        {
            if (!VerifyByteMarkers(inStream))
                throw new ContentFormatterException
                    ("The serialization byte markers are not present at the beginning of the stream.");

            IXimuraContentFormatterCapabilities header = GetHeader();

            header.Load(inStream);

            return ContentDeserializeInternal(header, pMan);
        }
        #endregion // Deserialize

        #region GetHeader()
        #region Helper method
        /// <summary>
        /// This method is used to get the header object which can 
        /// parse or write to the stream.
        /// </summary>
        /// <returns>A header object.</returns>
        protected virtual IXimuraContentFormatterCapabilities GetHeader()
        {
            return GetHeader(null);
        }
        #endregion // Helper method
        /// <summary>
        /// This method is used to get the header object which can 
        /// parse or write to the stream.
        /// </summary>
        /// <param name="entity">The entity type.</param>
        /// <returns>A header object.</returns>
        protected virtual IXimuraContentFormatterCapabilities GetHeader(Content entity)
        {
            IXimuraContentFormatterCapabilities header = new ContentFormatterCapabilities();

            return header;
        }
        #endregion

        #region ContentSerializeInternal
        /// <summary>
        /// This method serializes the serialization info.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="info">The serialization info.</param>
        protected virtual void ContentSerializeInternal(
            IXimuraContentFormatterCapabilities header, Content entity, SerializationInfo info)
        {
            IXimuraContentSerializationReaderWriter rwHelper = header.RWHelper;

            int bodyCount = info.GetInt32("bodycount");
            bool contentDirty = info.GetBoolean("dirty");

            //Write the content dirty status
            rwHelper.Write(contentDirty);

            //Write the blob count.
            rwHelper.Write(bodyCount);

            //Loop through each blob and save it to the stream.
            for (int loop = 0; loop < bodyCount; loop++)
            {
                byte[] blob =
                    (byte[])info.GetValue("body" + loop.ToString(), typeof(byte[]));

                if (header.StreamCompressed)
                    rwHelper.WriteCompressedBlob(blob);
                else
                    rwHelper.WriteBlob(blob);

            }
            rwHelper.BaseStream.Flush();
        }
        #endregion // ContentSerializeInternal

        #region ContentDeserializeInternal
        /// <summary>
        /// This method deserializes the stream in to a content object.
        /// </summary>
        /// <param name="header">The header</param>
        /// <param name="pMan">The pool manaer containing the required pool.</param>
        /// <returns>Returns a deserialized object.</returns>
        protected virtual Content ContentDeserializeInternal(
            IXimuraContentFormatterCapabilities header, IXimuraPoolManager pMan)
        {
            IXimuraContentSerializationReaderWriter rwHelper = header.RWHelper;

            Type contentType = ResolveType(header.ContentType, header.AllowRelativeType);

            if (contentType == null && header.SupportsBaseType)
                contentType = ResolveType(header.ContentBaseType, header.AllowRelativeType);

            //OK, the content type is still null
            if (contentType == null)
                throw new ContentFormatterException("The content type cannot be created.");

            SerializationInfo info = PrepareSerializationInfo(header, contentType);

            Content content = null;
            if (pMan != null)
            {
                content = (Content)pMan.GetPoolManager(contentType).Get(info, Context);
            }
            else
            {
                object[] parameters = new object[] { info, Context };
                Type[] typesArray = RH.getTypesfromObjectArray(parameters);
                ConstructorInfo TypeConstruct = contentType.GetConstructor(typesArray);
                content = TypeConstruct.Invoke(parameters) as Content;
                content.OnDeserialization(null);
            }

            return content as Content;
        }

        private Type ResolveType(string assemblyName, bool AllowRelativeType)
        {
            return RH.CreateTypeFromString(assemblyName, null, true);
        }

        /// <summary>
        /// This method prepares the serialization info.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        protected static SerializationInfo PrepareSerializationInfo(
            IXimuraContentFormatterCapabilities header, Type contentType)
        {
            IXimuraContentSerializationReaderWriter rwHelper = header.RWHelper;
            SerializationInfo info = new SerializationInfo(contentType, new FormatterConverter());

            //Add the ids
            info.AddValue("tid", header.TypeID);
            info.AddValue("cid", header.ContentID);
            info.AddValue("vid", header.VersionID);

            //Add the dirty status
            bool contentDirty = rwHelper.ReadBool();
            info.AddValue("dirty", contentDirty);

            //Add the body count
            int bodyCount = rwHelper.ReadInt();
            info.AddValue("bodycount", bodyCount);

            //Loop through each blob and save it to the stream.
            for (int loop = 0; loop < bodyCount; loop++)
            {
                byte[] blob;
                if (header.StreamCompressed)
                    blob = rwHelper.ReadCompressedBlob();
                else
                    blob = rwHelper.ReadBlob();

                info.AddValue("body" + loop.ToString(), blob, typeof(byte[]));
            }

            return info;
        }
        #endregion

        #region ByteMarkers methods
        /// <summary>
        /// This protected method writes the initial byte headers that identify the start
        /// of the DataContent serialization stream.
        /// </summary>
        /// <param name="outStream"></param>
        public static void WriteByteMarkers(Stream outStream)
        {
            //This is the two magic bytes that are written to the start of the serialization stream
            outStream.WriteByte(0xD8);
            outStream.WriteByte(0xB4);
        }

        /// <summary>
        /// This method returns true if it has found the start of the serialization 
        /// byte stream which is identified by the 2 bytes D8 B4. This method will
        /// read 1 or 2 bytes from the stream, depending on whether the first byte
        /// is equal to D8.
        /// </summary>
        /// <param name="inStream">The stream to read from.</param>
        /// <returns>True if the two byte header marker has been found.</returns>
        public static bool VerifyByteMarkers(Stream inStream)
        {
            return inStream.ReadByte() == 0xD8 && inStream.ReadByte() == 0xB4;
        }
        #endregion // ByteMarkers

        #region GetSerializationInfo(Stream inStream)
        /// <summary>
        /// This method converts the incoming stream to a searialization information.
        /// </summary>
        /// <param name="inStream">The incoming data stream.</param>
        /// <returns>Returns the serialization information.</returns>
        public static SerializationInfo GetSerializationInfo(Stream inStream)
        {
            if (!VerifyByteMarkers(inStream))
                throw new ContentFormatterException
                    ("The serialization byte markers are not present at the beginning of the stream.");

            IXimuraContentFormatterCapabilities header = new ContentFormatterCapabilities();

            header.Load(inStream);

            IXimuraContentSerializationReaderWriter rwHelper = header.RWHelper;

            Type contentType = RH.CreateTypeFromString(header.ContentType, null, true);

            if (contentType == null && header.SupportsBaseType)
                contentType = RH.CreateTypeFromString(header.ContentBaseType, null, true);

            //OK, the content type is still null
            if (contentType == null)
                throw new ContentFormatterException("The content type cannot be created.");

            SerializationInfo info = PrepareSerializationInfo(header, contentType);

            return info;
        }
        #endregion
    }
}
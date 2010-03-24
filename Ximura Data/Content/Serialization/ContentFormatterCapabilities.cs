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
using System.Text;
using System.Data;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// The ContentFormatterCapabilities is used to read and write the serialized 
    /// data header.
    /// </summary>
    public class ContentFormatterCapabilities : IXimuraContentFormatterCapabilities
    {
        #region Declarations
        /// <summary>
        /// This is the max incoming length for variable byte arrays.
        /// </summary>
        public const int cMaxHeaderLength = 0xFFFF;
        uint headerUint = 0;

        Guid tid = Guid.Empty;
        Guid cid = Guid.Empty;
        Guid vid = Guid.Empty;

        DateTime mUTCWriteTime = DateTime.MinValue;
        string mCType = null;
        string mCBaseType = null;
        string mHeaderObjectName = null;

        ContentSerializationReaderWriter rwHelper;

        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ContentFormatterCapabilities()
        {
            rwHelper = new ContentSerializationReaderWriter();
            SetDefaultProperties();
        }
        #endregion // Constructor

        #region SetDefaultProperties()
        /// <summary>
        /// This method sets the formatter revision number.
        /// </summary>
        protected virtual void SetDefaultProperties()
        {
            Max = 1;
            Min = 0;
            Revision = 1;
            SupportsBaseType = false;
            WriteDate = true;
            HeaderName = true;
            AllowRelativeType = true;
            StreamCompressed = true;
        }
        #endregion // SetVersionNumber()

        #region Version properties
        #region Max
        /// <summary>
        /// The maximum version value up to a maximum of 63.
        /// </summary>
        public int Max
        {
            get
            {
                return HeaderGetBits(0x7F, 12);
            }
            set
            {
                if (value < 0 || value > 0x7F)
                    throw new ArgumentOutOfRangeException("Max can only be between 0 and 127.");
                HeaderSetBits(value, 0x7F, 12);
            }
        }
        #endregion
        #region Min
        /// <summary>
        /// The minimum version number, between 0 and 63.
        /// </summary>
        public int Min
        {
            get
            {
                return HeaderGetBits(0x3F, 6);
            }
            set
            {
                if (value < 0 || value > 0x3F)
                    throw new ArgumentOutOfRangeException("Min can only be between 0 and 63.");
                HeaderSetBits(value, 0x3F, 6);
            }
        }
        #endregion // Min
        #region Revision
        /// <summary>
        /// The revision number between 0 and 63.
        /// </summary>
        public int Revision
        {
            get
            {
                return HeaderGetBits(0x3F, 0);
            }
            set
            {
                if (value < 0 || value > 0x3F)
                    throw new ArgumentOutOfRangeException("Version can only be between 0 and 63.");
                HeaderSetBits(value, 0x3F, 0);
            }
        }
        #endregion // Revision
        #endregion // Version properties
        #region Content Properties
        /// <summary>
        /// The Type ID
        /// </summary>
        public Guid TypeID { get { return tid; } }
        /// <summary>
        /// The Content ID
        /// </summary>
        public Guid ContentID { get { return cid; } }
        /// <summary>
        /// The Version ID
        /// </summary>
        public Guid VersionID { get { return vid; } }
        /// <summary>
        /// The content type specified.
        /// </summary>
        public string ContentType { get { return mCType; } }
        /// <summary>
        /// The content type specified.
        /// </summary>
        public string ContentBaseType { get { return mCBaseType; } }
        /// <summary>
        /// This is the time the header was written to the stream.
        /// </summary>
        public DateTime UTCWriteTime { get { return mUTCWriteTime; } }
        /// <summary>
        /// This is the name of the object that created the header.
        /// </summary>
        public string HeaderObjectName { get { return mHeaderObjectName; } }
        #endregion // Content Properties

        #region CapabilityProperties
        /// <summary>
        /// This method sets the header bits.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="range">The range</param>
        /// <param name="bitMarker">The begining bit marker.</param>
        protected virtual void HeaderSetBits(int value, int range, int bitMarker)
        {
            uint map = (uint)value << bitMarker;
            uint mask = ~((uint)range << bitMarker);
            headerUint &= mask;
            headerUint |= map;
        }
        /// <summary>
        /// This method sets the capability bit for the value.
        /// </summary>
        /// <param name="bit">The bit to set.</param>
        /// <param name="value">The boolean value to set.</param>
        protected virtual void HeaderSetBit(int bit, bool value)
        {
            HeaderSetBits(value ? 1 : 0, 1, bit + 19);
        }
        /// <summary>
        /// The method gets a range of bytes from the header.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="bitMarker"></param>
        /// <returns></returns>
        protected virtual int HeaderGetBits(int range, int bitMarker)
        {
            uint moved = (headerUint >> bitMarker) & (uint)range;

            return (int)moved;
        }
        /// <summary>
        /// This method gets the boolean value from the header bit.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <returns>Returns the boolean value for the bit.</returns>
        protected virtual bool HeaderGetBit(int bit)
        {
            return HeaderGetBits(1, bit + 19) > 0;
        }

        /// <summary>
        /// This method identifies whether the content supports a base type.
        /// </summary>
        public bool SupportsBaseType
        {
            get { return HeaderGetBit(4); }
            set
            {
                HeaderSetBit(4, value);
            }
        }
        /// <summary>
        /// This specifies whether the deserializer can use the best match for creating
        /// the object from the type.
        /// </summary>
        public bool AllowRelativeType
        {
            get { return HeaderGetBit(3); }
            set
            {
                HeaderSetBit(3, value);
            }
        }
        /// <summary>
        /// This property specifies that the following stream is compressed.
        /// </summary>
        public bool StreamCompressed
        {
            get { return HeaderGetBit(2); }
            set
            {
                HeaderSetBit(2, value);
            }
        }
        /// <summary>
        /// This property indicates whether the object will write its name in the header.
        /// </summary>
        public bool HeaderName
        {
            get { return HeaderGetBit(1); }
            set
            {
                HeaderSetBit(1, value);
            }
        }
        /// <summary>
        /// This property indicates whether the header will write the UTC date in the header
        /// </summary>
        public bool WriteDate
        {
            get { return HeaderGetBit(0); }
            set
            {
                HeaderSetBit(0, value);
            }
        }
        #endregion // StreamCompressed
        #region Load
        /// <summary>
        /// This method loads the header from the stream.
        /// </summary>
        /// <param name="inStream">The stream to read from.</param>
        public void Load(Stream inStream)
        {
            //Set the stream for the helper.
            rwHelper.BaseStream = inStream;

            HeaderRead();
            ContentDetailsRead();
        }
        #endregion // Load
        #region Output()
        /// <summary>
        /// This method outputs the header for the stream.
        /// </summary>
        /// <param name="outStream">The stream to write to.</param>
        /// <param name="entity">The entity specified.</param>
        /// <param name="info">The SerializationInfo object.</param>
        public void Output(Stream outStream, IXimuraContent entity, SerializationInfo info)
        {
            if (outStream == null)
                throw new ArgumentNullException("Stream cannot be null.");
            if (info == null)
                throw new ArgumentNullException("SerializationInfo cannot be null.");
            if (entity == null)
                throw new ContentFormatterCapabilitiesException("The ContentFormatterCapabilities header has not been initialized.");

            //Set the stream for the helper.
            rwHelper.BaseStream = outStream;

            SetContentHeaderProperties(entity);

            //Write the header.
            HeaderWrite();
            //Write the capabilities.
            ContentDetailsWrite(entity, info);
        }
        #endregion // Output

        #region SetContentHeaderProperties
        /// <summary>
        /// This method is used to set any specific header properties based on the entity type.
        /// </summary>
        /// <param name="entity">The entity being serialized.</param>
        protected virtual void SetContentHeaderProperties(IXimuraContent entity)
        {
            IXimuraContentEntityFragment cef = entity as IXimuraContentEntityFragment;
            if (cef != null)
                this.SupportsBaseType = cef.SupportsFragmentBaseType();
        }
        #endregion // SetContentHeaderProperties

        #region ContentDetailsWrite
        /// <summary>
        /// This method writes the content details from the serialization info in 
        /// to the stream.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="info">The serialization info.</param>
        protected virtual void ContentDetailsWrite(IXimuraContent entity, SerializationInfo info)
        {
            Guid cid = (Guid)info.GetValue("cid", typeof(Guid));
            Guid vid = (Guid)info.GetValue("vid", typeof(Guid));
            Guid tid = (Guid)info.GetValue("tid", typeof(Guid));
            rwHelper.Write(tid);
            rwHelper.Write(cid);
            rwHelper.Write(vid);

            if (WriteDate)
                rwHelper.Write(DateTime.UtcNow);

            mHeaderObjectName = entity.GetType().AssemblyQualifiedName;
            rwHelper.Write(mHeaderObjectName);

            if (SupportsBaseType)
            {
                IXimuraContentEntityFragment cef = entity as IXimuraContentEntityFragment;
                rwHelper.Write(cef.FragmentBaseType() == null ? "" : cef.FragmentBaseType().AssemblyQualifiedName);
            }
        }
        #endregion // ContentDetailsWrite
        #region ContentDetailsRead
        /// <summary>
        /// This method reads the content details from the stream.
        /// </summary>
        protected virtual void ContentDetailsRead()
        {
            tid = rwHelper.ReadGuid();
            cid = rwHelper.ReadGuid();
            vid = rwHelper.ReadGuid();

            if (WriteDate)
                mUTCWriteTime = rwHelper.ReadDateTime();

            //Get the object type string.
            mCType = rwHelper.ReadString();

            //Check whether there is a base type string.
            if (SupportsBaseType)
            {
                mCBaseType = rwHelper.ReadString();
            }
        }
        #endregion // ContentDetailsRead
        #region HeaderRead
        /// <summary>
        /// This method reads the header from the stream.
        /// </summary>
        protected virtual void HeaderRead()
        {
            headerUint = rwHelper.ReadUint();

            int extendedCapabilityBytes = this.HeaderGetBits(4, 28);

            if (extendedCapabilityBytes > 0)
            {
                ReadExtendedCapabilityBytes(extendedCapabilityBytes);
            }

            if (HeaderName)
            {
                rwHelper.ReadString();
            }
        }
        #endregion // HeaderRead
        #region HeaderWrite
        /// <summary>
        /// This method writes the header to the stream
        /// </summary>
        protected virtual void HeaderWrite()
        {
            //We would set the extended bitmaps here. 
            //The first three bytes identify whether there are extended property bytes
            rwHelper.Write(headerUint);

            if (HeaderName)
            {
                rwHelper.Write(this.GetType().AssemblyQualifiedName);
            }
        }
        #endregion // HeaderWrite

        #region ReadExtendedCapabilityBytes
        /// <summary>
        /// This method should be overriden to process any extra capability 
        /// bytes specified in the header.
        /// </summary>
        /// <param name="extendedCapabilityBytes">The number of capability bytes to read</param>
        protected virtual void ReadExtendedCapabilityBytes(int extendedCapabilityBytes)
        {
            if (extendedCapabilityBytes >= cMaxHeaderLength)
                throw new ContentFormatterException("Stream corruption. CapabilityBytes exceeds the maximum.");

            byte[] buffer = new byte[extendedCapabilityBytes];
            rwHelper.BaseStream.Read(buffer, 0, extendedCapabilityBytes);

        }
        #endregion // ProcessExtendedCapabilityBytes

        #region RWHelper
        /// <summary>
        /// This is the read write helper for the stream.
        /// </summary>
        public IXimuraContentSerializationReaderWriter RWHelper
        {
            get { return rwHelper; }
        }
        #endregion // RWHelper
    }
}
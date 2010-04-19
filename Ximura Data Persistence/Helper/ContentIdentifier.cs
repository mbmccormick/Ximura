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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Text;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
using RH = Ximura.Reflection;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This structure holds the collection of content ID and their associated methods.
    /// </summary>
    public struct ContentIdentifier: IEquatable<ContentIdentifier>
    {
        #region Declarations
        private static readonly ContentIdentifier mEmpty = 
            new ContentIdentifier(false);
        Guid tid;
        Guid cid;
        Guid vid;
        string contentType;
        byte[] byCombined;
        Type internalType;
        #endregion // Declarations

        #region Static helper methods
        #region ExtractTypeID
        /// <summary>
        /// This method resolves the Type ID from the XimuraContentTypeIDAttribute attribute.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static Guid ExtractTypeID(Type contentType)
        {
            //if (contentType.
            return contentType.GetContentTypeAttributeID(); ;
        }
        #endregion // ExtractTypeID
        #region ExtractContentID
        /// <summary>
        /// This method resolves the content ID from the XimuraContentIDAttribute attribute.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static Guid ExtractContentID(Type contentType)
        {
            XimuraContentIDAttribute attr = AH.GetAttribute<XimuraContentIDAttribute>(contentType);

            if (attr != null)
                return attr.ID;

            throw new ArgumentOutOfRangeException("The XimuraContentIDAttribute is missing.");
        }
        #endregion // ExtractContentID
        #region AQNTruncate
        /// <summary>
        /// This method truncates the Assembly Qualified Name (AQN), in to the distinct
        /// format without the version number.
        /// </summary>
        /// <param name="aqn">The string.</param>
        /// <returns>The truncated string.</returns>
        public static string AQNTruncate(string aqn)
        {
            int point = aqn.IndexOf(", Version");
            if (point < 0)
                return aqn;

            return aqn.Substring(0, point);
        }
        #endregion // AQNTruncate

        #region Empty
        /// <summary>
        /// This is the empty identifier;
        /// </summary>
        public static ContentIdentifier Empty
        {
            get
            {
                return mEmpty;
            }
        }
        #endregion // Empty
        #endregion // Static helper methods

        #region Constructors
        /// <summary>
        /// This is the type constructor. This constructor can be used by content items that implement
        /// both the XimuraContentTypeIDAttribute and XimuraContentIDAttribute attributes.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        public ContentIdentifier(Type contentType):
            this(ExtractTypeID(contentType), ExtractContentID(contentType), Guid.Empty, contentType){}
        /// <summary>
        /// This is the type constructor. This constructor can be used by content items that implement
        /// both the XimuraContentTypeIDAttribute and XimuraContentIDAttribute attributes.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="contentID">The content ID.</param>
        public ContentIdentifier(Type contentType, Guid contentID):
            this(ExtractTypeID(contentType), contentID, Guid.Empty, contentType){}
        /// <summary>
        /// This is the type constructor. This constructor can be used by content items that implement
        /// both the XimuraContentTypeIDAttribute and XimuraContentIDAttribute attributes.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="contentID">The content ID.</param>
        /// <param name="versionID">The version ID.</param>
        public ContentIdentifier(Type contentType, Guid contentID, Guid versionID)
            :
            this(ExtractTypeID(contentType), contentID, versionID, contentType) { }
        /// <summary>
        /// This is the content constructor.
        /// </summary>
        /// <param name="data">The content data.</param>
        public ContentIdentifier(IXimuraContent data)
            :this(data.IDType, data.IDContent, data.IDVersion, data.GetType()){ }
        /// <summary>
        /// This is the specific constructor.
        /// </summary>
        /// <param name="tid">The content type id.</param>
        /// <param name="cid">The content id.</param>
        /// <param name="vid">The content version id.</param>
        /// <param name="contentType">This the content object type.</param>
        public ContentIdentifier(Guid tid, Guid cid, Guid vid, Type contentType)
		{
            this.tid = tid;
            this.cid = cid;
            this.vid = vid;
            this.contentType = AQNTruncate(contentType.AssemblyQualifiedName);
            this.internalType = contentType;
            byCombined = null;
  
        }
		/// <summary>
		/// This is the specific constructor.
		/// </summary>
		/// <param name="tid">The content type id.</param>
		/// <param name="cid">The content id.</param>
		/// <param name="vid">The content version id.</param>
		/// <param name="contentType">This the content object type.</param>
		public ContentIdentifier(Guid tid, Guid cid, Guid vid, string contentType)
		{
			this.tid = tid;
			this.cid = cid;
			this.vid = vid;
			this.contentType = contentType;
            this.internalType = RH.CreateTypeFromString(contentType);
            byCombined = null;
		}

        private ContentIdentifier(bool handler)
        {
            this.tid = Guid.Empty;
            this.cid = Guid.Empty;
            this.vid = Guid.Empty;
            this.contentType = null;
            this.internalType = null;
            byCombined = null;
        }

		#endregion // Constructors

        #region TypeID
        /// <summary>
        /// This is the content type ID.
        /// </summary>
        public Guid TypeID
        {
            get { return tid; }
        }
        #endregion // TypeID
        #region ContentID
        /// <summary>
        /// This is the content ID>
        /// </summary>
        public Guid ContentID
        {
            get { return cid; }
        }
        #endregion // ContentID
        #region VersionID
        /// <summary>
        /// This is the version ID
        /// </summary>
        public Guid VersionID
        {
            get
            {
                return vid;
            }
            set
            {
                vid = value;
            }
        }
        #endregion // VersionID
        #region ContentType
        /// <summary>
        /// This is the content type>
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
        }
        #endregion // ContentType
        #region InternalType
        /// <summary>
        /// This is the content type
        /// </summary>
        public Type InternalType
        {
            get { return internalType; }
        }
        #endregion // ContentType

        #region ContentTypeAQNTruncated
        /// <summary>
        /// This is the truncated object type.
        /// </summary>
        public string ContentTypeAQNTruncated
        {
            get
            {
                return AQNTruncate(ContentType);
            }
        }
        #endregion // ContentTypeAQNTruncated

        #region CombinedID
        /// <summary>
        /// This is the combined byte array.
        /// </summary>
        public byte[] CombinedID
        {
            get
            {
                if (byCombined == null)
                    BuildCombined();

                return byCombined;
            }
        }
        private void BuildCombined()
        {
            byte[] byContentType = Encoding.ASCII.GetBytes(contentType);

            byCombined = new byte[48 + byContentType.Length];
            tid.ToByteArray().CopyTo(byCombined, 0);
            cid.ToByteArray().CopyTo(byCombined, 16);
            vid.ToByteArray().CopyTo(byCombined, 32);
            byContentType.CopyTo(byCombined, 48);
        }
        #endregion // CombinedID

        #region Cacheable()
        /// <summary>
        /// This helper method identifies whether the content can be cached. 
        /// It basically returns true if the Guid is not empty.
        /// </summary>
        /// <returns></returns>
        public bool Cacheable()
        {
            return vid != Guid.Empty;
        }
        #endregion // Cacheable()

        #region public string CreateItemIDString()
        /// <summary>
        /// This method returns a unique identifier string for the content collection.
        /// </summary>
        /// <returns></returns>
        public string CreateItemIDString()
        {
            return tid.ToString("N") + @"/" + cid.ToString("N") + @"/" + vid.ToString("N") + ".bin";
        }
        #endregion // public string CreateItemIDString()

        #region Equals
        /// <summary>
        /// This method combines the other ContentIdentifier with this one.
        /// </summary>
        /// <param name="other">The content identifier to compare against.</param>
        /// <returns>Returns true if the identifiers are equal.</returns>
        public override bool Equals(object other)
        {
            if ((other == null) || !(other is ContentIdentifier))
            {
                return false;
            }

            return Equals((ContentIdentifier)other);
        }

        /// <summary>
        /// This method combines the other ContentIdentifier with this one.
        /// </summary>
        /// <param name="other">The content identifier to compare against.</param>
        /// <returns>Returns true if the identifiers are equal.</returns>
        public bool Equals(ContentIdentifier other)
        {
            if (other.CombinedID.Length != this.CombinedID.Length)
                return false;


            for (int i = 0; i < this.CombinedID.Length; i++)
            {
                if (other.CombinedID[i] != this.CombinedID[i])
                    return false;
            }

            return true;
        }

        #endregion // Equals

        #region ToString()
        /// <summary>
        /// This override string identifies the content uniquely.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ContentType + @"/" + CreateItemIDString();
        }
        #endregion // ToString()

        #region GetHashCode()
        /// <summary>
        /// This override is required for some reason.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 0;
            if (contentType != null) hashCode ^= contentType.GetHashCode();
            hashCode ^= tid.GetHashCode();
            hashCode ^= cid.GetHashCode();
            hashCode ^= vid.GetHashCode();
            return hashCode;
        }
        #endregion // GetHashCode()
    }
}

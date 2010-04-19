#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the base content holder for storing and munipulating binary content
    /// </summary>
    [XimuraContentTypeID("2CE06633-AA48-474A-A315-C5B55F1B6859")]
    [XimuraDataContentSchema("http://schema.ximura.org/binary/1.0",
        "xmrres://XimuraCommEntities/Ximura.Communication.BinaryContent/Ximura.Communication.Resources.BinaryContent.xsd")]
    [XimuraDataContentDefault(
        "xmrres://XimuraCommEntities/Ximura.Communication.BinaryContent/Ximura.Communication.Resources.BinaryContent_NewData.xml", false)]
    public class BinaryContent : DublinCore
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public BinaryContent() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public BinaryContent(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:BinaryContent";
            mappingShortcuts.Add("r", basePath);
            mappingShortcuts.Add("rp", basePath + "/r:properties");
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
        #region NamespaceDefaultShortName
        /// <summary>
        /// This is the short name used in the namespace manager to refer to the root namespace.
        /// </summary>
        protected override string NamespaceDefaultShortName
        {
            get { return "r"; }
        }
        #endregion // NamespaceDefaultShortName

        #region FileName
        /// <summary>
        /// This is the filename that the binary file is known by. If the filename is null,
        /// the file will be known by {VID}.bin where VID is a string representation of the VersionID
        /// </summary>
        [CDSReference("name")]
        public virtual string FileName
        {
            get { return XmlMappingGetToString(XPSc("r", "filename")); }
            set { XmlMappingSet(XPSc("r", "filename"), value); }
        }
        #endregion // FileName

        #region Data
        /// <summary>
        /// This is the binary content data.
        /// </summary>
        public virtual byte[] Data
        {
            get { return XmlMappingGetToByteArray(XPSc("r", "data")); }
            set
            {
                XmlMappingSet(XPSc("r", "data"), value);
                Length = value.Length;
                ETag = GetETag(value);
                MimeType = GetMimeType(value);
            }
        }
        #endregion // ETag
        #region ETag
        /// <summary>
        /// This is the default ETag which is a MD5 hash of the byte array.
        /// </summary>
        [CDSAttribute("Binary", "ETag")]
        public virtual string ETag
        {
            get { return XmlMappingGetToString(XPSc("r", "etag")); }
            protected set { XmlMappingSet(XPSc("r", "etag"), value); }
        }
        #endregion // ETag
        #region Length
        /// <summary>
        /// This is the length of the internal byte array.
        /// </summary>
        [CDSAttribute("Binary", "Length")]
        public virtual long Length
        {
            get
            {
                long? length = XmlMappingGetToInt64Nullable(XPSc("r", "length"));
                return (length.HasValue) ? length.Value : 0;
            }
            protected set { XmlMappingSet(XPSc("r", "length"), value); }

        }
        #endregion // Length
        #region MimeType
        /// <summary>
        /// This abstract method should be overriden by all base classes.
        /// </summary>
        [CDSAttribute("Binary", "MimeType")]
        public virtual string MimeType
        {
            get { return XmlMappingGetToString(XPSc("r", "mimetype")); }
            set { XmlMappingSet(XPSc("r", "mimetype"), value); }
        }
        #endregion // MimeType
        #region GetETag()
        /// <summary>
        /// This method prepares the ETag value from the byte array
        /// </summary>
        /// <param name="buffer">The buffer to hash and set as the ETag.</param>
        protected virtual string GetETag(byte[] buffer)
        {
            using (HashAlgorithm mhash = new MD5CryptoServiceProvider())
            {
                //Create the hash value from the array of bytes.
                byte[] hashValue = mhash.ComputeHash(buffer);
                return Convert.ToBase64String(hashValue);
            }
        }
        #endregion // PrepareETag()
        #region GetMimeType()
        /// <summary>
        /// This method returns the default mime type for a binary file. You should override this method if
        /// you require a more specific mime type.
        /// </summary>
        /// <returns>Returns the default mime type: application/octet-stream</returns>
        protected virtual string GetMimeType(byte[] buffer)
        {
            return "application/octet-stream";
        }
        #endregion // GetMimeType()

        #region GetReadOnlyStream()
        /// <summary>
        /// This method returns the inner byte array as a read only stream.
        /// </summary>
        /// <returns></returns>
        public Stream GetReadOnlyStream()
        {
            return new MemoryStream(Data, false);
        }
        #endregion // GetReadOnlyStream()

        #region ToArray()
        /// <summary>
        /// This override returns a copy of the internally stored byte array without
        /// any additional meta data.
        /// </summary>
        /// <returns>Returns a byte array of the internal image, i.e. the stored binary image.</returns>
        public override byte[] ToArray()
        {
            byte[] blob = new byte[Data.Length];
            Buffer.BlockCopy(Data, 0, blob, 0, Data.Length);

            return blob;
        }
        #endregion // ToArray()

        #region Properties
        /// <summary>
        /// This enumeration returns the metadata properties for the binary entity.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Properties
        {
            get
            {
                XmlNodeList nodeProperties =
                    this.XmlDataDoc.SelectNodes(XPSc("r", "properties", "property"), NSM);

                foreach (XmlNode node in nodeProperties)
                {
                    yield return new KeyValuePair<string, string>(node.Attributes["id"].Value, node.InnerText);
                }
            }
        }
        #endregion // Properties
        #region PropertySet(string key, string value)
        /// <summary>
        /// This method sets the specific property with the value specified. If the property does not exist it 
        /// is created and added to the collection.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        public void PropertySet(string key, string value)
        {
            XmlNode node = PropertyNode(key);

            if (node != null)
            {
                node.InnerText = value;
            }

            XmlNode parent = this.XmlDataDoc.SelectSingleNode(XPSc("rp"), NSM);
            XmlElementAdd(parent, "property", value, XmlAttributeCreate("id", key));
        }
        #endregion // PropertySet(string key, string value)
        #region PropertyExists(string key)
        /// <summary>
        /// This method checks whether a specific property exists in the collection.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>Returns true if the property exists.</returns>
        public bool PropertyExists(string key)
        {
            return PropertyNode(key) != null;
        }
        #endregion // PropertyExists(string key)
        #region PropertyRemove(string key)
        /// <summary>
        /// This method removes the specific property from the content if it exists.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>Returns true if the property exists and has been removed.</returns>
        public bool PropertyRemove(string key)
        {
            XmlNode node = PropertyNode(key);
            if (node == null)
                return false;

            node.ParentNode.RemoveChild(node);
            return true;
        }
        #endregion // PropertyRemove(string key)
        #region PropertyGet(string key)
        /// <summary>
        /// This method returns the property for the specified key.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>Returns the property value or null if the property cannot be found.</returns>
        public string PropertyGet(string key)
        {
            XmlNode node = PropertyNode(key);
            return node == null ? (string)null : node.InnerText;
        }
        #endregion // PropertyGet(string key)
        #region PropertyNode(string key)
        /// <summary>
        /// This method returns the property node for the specific key.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The specified node or null if the node cannot be found.</returns>
        private XmlNode PropertyNode(string key)
        {
            return this.XmlDataDoc.SelectSingleNode(XPSc("rp", "property") + "[@id='" + key + "']", NSM);
        }
        #endregion // PropertyNode(string key)
    }

    #region BinaryContentRaw
    /// <summary>
    /// This is the raw binary content that does not store the binary data as an XML representation.
    /// </summary>
    [XimuraContentTypeID("2CE06633-AA48-474a-A315-C5B55F1B6859")]
    public class BinaryContentRaw : Content
    {
        #region Declarations
        private string mMimeType;
        /// <summary>
        /// This is the internal byte array used to hold the content.
        /// </summary>
        protected byte[] mData;
        /// <summary>
        /// This is the Etag daclarations
        /// </summary>
        protected string mETag;
        /// <summary>
        /// This is the internal FileName
        /// </summary>
        protected string mFileName;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public BinaryContentRaw() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public BinaryContentRaw(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region Reset()
        /// <summary>
        /// This is the Reset method for the content.
        /// </summary>
        public override void Reset()
        {
            mFileName = null;
            mMimeType = null;
            mETag = null;
            mData = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Load(System.IO.Stream data) -> FileStream parse
        /// <summary>
        /// This override parses the filename when a filestream is passed as a stream.
        /// </summary>
        /// <param name="data">The data stream.</param>
        /// <returns>Returns the number of bytes read from the stream.</returns>
        public override int Load(System.IO.Stream data)
        {
            int result = base.Load(data);
            if (data is FileStream)
            {
                FileInfo fi = new FileInfo(((FileStream)data).Name);
                FileName = fi.Name;
                switch (fi.Extension.ToLowerInvariant())
                {
                    case ".png":
                        mMimeType = "image/png";
                        break;
                    case ".jpg":
                        mMimeType = "image/jpeg";
                        break;
                    case ".gif":
                        mMimeType = "image/gif";
                        break;
                    default:
                        //mMimeType = "image/" + fi.Extension.ToLowerInvariant().Substring(1);
                        break;
                }
                this.IDContent = Guid.NewGuid();
                this.IDVersion = Guid.NewGuid();
            }
            return result;
        }
        #endregion // Load(System.IO.Stream data) -> FileStream parse
        #region Load(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method loads the content from the byte array.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <param name="offset">The data byte offset.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>Returns the actual number of bytes read.</returns>
        public override int Load(byte[] buffer, int offset, int count)
        {
            if (Loaded)
                throw new ContentLoadException("The content is already loaded.");

            mData = new byte[count - offset];
            Buffer.BlockCopy(buffer, offset, mData, 0, count);

            PrepareETag();

            mCanLoad = false;
            return count;
        }
        #endregion // Load(byte[] buffer, int offset, int count)

        #region PrepareETag()
        /// <summary>
        /// This method prepares the ETag value from the byte array
        /// </summary>
        protected virtual void PrepareETag()
        {
            if (mETag != null)
                return;

            using (HashAlgorithm mhash = new MD5CryptoServiceProvider())
            {
                //Create the hash value from the array of bytes.
                byte[] HashValue = mhash.ComputeHash(mData);
                mETag = Convert.ToBase64String(HashValue);
            }
        }
        #endregion // PrepareETag()

        #region ContentBody
        /// <summary>
        /// This overriden method returns the content body for serialization.
        /// </summary>
        protected override byte[] ContentBody
        {
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8))
                    {
                        bw.Write(FileName);
                        bw.Write(MimeType);
                        bw.Write(ETag);
                        ms.Write(mData, 0, mData.Length);

                        return ms.ToArray();
                    }
                }
            }
        }
        #endregion // ContentBody

        #region BodyDataProcess(byte[] blob)
        /// <summary>
        /// This method sets the specific properties from the binary data.
        /// </summary>
        /// <param name="blob">The data.</param>
        protected virtual void BodyDataProcess(byte[] blob)
        {
            using (MemoryStream ms = new MemoryStream(blob))
            {
                using (BinaryReader br = new BinaryReader(ms, Encoding.UTF8))
                {
                    mFileName = br.ReadString();
                    mMimeType = br.ReadString();
                    mETag = br.ReadString();

                    Load(ms);
                }
            }
        }
        #endregion 

        #region ETag
        /// <summary>
        /// This is the default ETag which is a MD5 hash of the byte array.
        /// </summary>
        [CDSAttribute("Binary", "ETag")]
        public virtual string ETag
        {
            get
            {
                LoadCheck();
                if (mETag == null)
                    return string.Empty;
                return mETag;
            }
        }
        #endregion // ETag

        #region Length
        /// <summary>
        /// This is the length of the internal byte array.
        /// </summary>
        [CDSAttribute("Binary", "Length")]
        public virtual string Length
        {
            get
            {
                LoadCheck();
                if (mData == null)
                    return "0";
                return mData.Length.ToString();
            }
        }
        #endregion // Length

        #region MimeType
        /// <summary>
        /// This abstract method should be overriden by all base classes.
        /// </summary>
        [CDSAttribute("Binary", "MimeType")]
        public virtual string MimeType
        {
            get
            {
                LoadCheck();
                if (mMimeType == null)
                    mMimeType = GetMimeType();
                return mMimeType;
            }
            set
            {
                mMimeType = value;
            }
        }
        #endregion // MimeType

        #region GetMimeType()
        /// <summary>
        /// This method makes an system call to determine the Mime type from the byte array.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetMimeType()
        {
            //if (mData == null || mData.Length == 0)
            return "";

            //try
            //{
            //    System.UInt32 mimeType;
            //    System.UInt32 returnValue = FindMimeFromData(0, null, mData, 256, null, 0, out mimeType, 0);
            //    System.IntPtr mimeTypePointer = new IntPtr(mimeType);

            //    string mimeStr = Marshal.PtrToStringUni(mimeTypePointer);
            //    return mimeStr;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion // GetMimeType()

        #region FileName
        /// <summary>
        /// This is the filename that the binary file is known by. If the filename is null,
        /// the file will be known by {VID}.bin where VID is a string representation of the VersionID
        /// </summary>
        [CDSReference("name")]
        public virtual string FileName
        {
            get
            {
                if (mFileName == null)
                    return "";
                return mFileName;
            }
            set { mFileName = value; }
        }
        #endregion // FileName

        #region GetReadOnlyStream()
        /// <summary>
        /// This method returns the inner byte array as a read only stream.
        /// </summary>
        /// <returns></returns>
        public Stream GetReadOnlyStream()
        {
            return new MemoryStream(mData, false);
        }
        #endregion // GetReadOnlyStream()

        #region ToArray()
        /// <summary>
        /// This override returns a copy of the internally stored byte array without
        /// any additional meta data.
        /// </summary>
        /// <returns>Returns a byte array of the internal image, i.e. the stored binary image.</returns>
        public override byte[] ToArray()
        {
            byte[] blob = new byte[mData.Length];
            Buffer.BlockCopy(mData, 0, blob, 0, mData.Length);

            return blob;
        }
        #endregion // ToArray()
    }
    #endregion // BinaryContentRaw
}

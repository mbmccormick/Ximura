#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

using System.Text;
using System.IO;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class StylesheetBase : BinaryContent
    {
        #region Declarations
        XslCompiledTransform mTran;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public StylesheetBase() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public StylesheetBase(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region Reset()
        /// <summary>
        /// This is the reset method.
        /// </summary>
        public override void Reset()
        {
            mTran = null;
            base.Reset();
        }
        #endregion // Reset()

        #region MimeType
        /// <summary>
        /// This is the MIME type for a stylesheet
        /// </summary>
        [CDSAttribute("Binary", "MimeType")]
        public override string MimeType
        {
            get { return "application/xslt+xml"; }
        }
        #endregion // MimeType

        #region Compile(XmlResolver xsltResolver)
        /// <summary>
        /// This method compiles the XSLT Stylesheet, so that it is ready to 
        /// transform data.
        /// </summary>
        /// <param name="xsltResolver">The resolver to resolve external resources.</param>
        /// <returns></returns>
        public virtual void Compile(XmlResolver xsltResolver)
        {
            try
            {
#if (DEBUG)
                mTran = new XslCompiledTransform(true);
#else
                mTran = new XslCompiledTransform();
#endif
                //byte[] blob = System.Text.Encoding.UTF8.GetBytes(XmlDataDoc.InnerXml);
                byte[] blob = Data;

                using (MemoryStream ms = new MemoryStream(blob))
                {
                    using (XmlReader xmlr = XmlReader.Create(ms))
                    {
                        mTran.Load(xmlr, XsltSettings.Default, xsltResolver);
                    }
                }
            }
            catch (XsltCompileException xcex)
            {
                //throw xcex;
            }
            catch (XsltException xex)
            {
                //throw xex;
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            //XslTransform transform = new XslTransform();
            //transform.Load(

        }
        #endregion // Compile(XmlResolver xsltResolver)

        #region Transform(IXPathNavigable nav)
        /// <summary>
        /// This method transform the incoming data using the transform.
        /// </summary>
        /// <param name="nav">The navigable data.</param>
        /// <returns>Returns the byte array of the transformed data.</returns>
        public virtual byte[] Transform(IXPathNavigable nav)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                mTran.Transform(nav, null, ms);
                return ms.ToArray();
            }
        }

        public virtual byte[] Transform(XmlReader reader)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                mTran.Transform(reader, null, ms);
                return ms.ToArray();
            }
        }
        #endregion // Transform(IXPathNavigable nav)

        public override int Load(Stream data)
        {
            int result = base.Load(data);
            if (data is FileStream)
            {
                FileInfo fi = new FileInfo(((FileStream)data).Name);
                if (fi.DirectoryName.ToLowerInvariant().EndsWith("include"))
                {
                    FileName = @"Include/" + fi.Name;
                }
                else
                    FileName = fi.Name;

                this.IDContent = Guid.NewGuid();
                this.IDVersion = Guid.NewGuid();
            }
            return result;
        }

        #region FileName
        /// <summary>
        /// This is the filename that the binary file is known by. If the filename is null,
        /// the file will be known by {VID}.bin where VID is a string representation of the VersionID
        /// </summary>
        [CDSReference("name")]
        public override string FileName
        {
            get
            {
                return base.FileName;
            }
            set { base.FileName = value; }
        }
        #endregion // FileName

        #region IDContent
        /// <summary>
        /// This is the ID of the Entity.
        /// </summary>
        [Browsable(false)]
        public override Guid IDContent
        {
            get
            {
                Guid? idAttr = GetContentAttributeID();
                if (!idAttr.HasValue)
                    return mIDContent;

                return idAttr.Value;
            }
            set
            {
                if (!GetContentAttributeID().HasValue)
                    mIDContent = value;
            }
        }
        #endregion // ID
        #region IDVersion
        /// <summary>
        /// This is the version ID of the entity.
        /// </summary>
        [Browsable(false)]
        public override Guid IDVersion
        {
            get
            {
                return mIDVersion;
            }
            set
            {
                mIDVersion = value;
            }
        }
        #endregion // Version

        
    }
}

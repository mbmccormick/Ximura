#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ResourceManagerMessageFragmentBody : InternetMessageFragmentBody
    {
        #region Declarations
        private string mETag;
        private string mExpires;
        private string mContentType;
        private string mContentEncoding;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ResourceManagerMessageFragmentBody()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the message so that it can be reused.
        /// </summary>
        public override void Reset()
        {
            mExpires = null;
            mETag = null;
            mContentType = null;
            mContentEncoding = null;
            base.Reset();
        }
        #endregion // Reset()

        #region ContentType
        public override bool HasContentType
        {
            get { return mContentType != null; }
        }

        public override string ContentType
        {
            get
            {
                return mContentType;
            }
            set
            {
                mContentType = value;
            }
        }
        #endregion // ContentType

        public override bool HasContentEncoding
        {
            get
            {
                return mContentEncoding != null;
            }
        }

        public override string ContentEncoding
        {
            get
            {
                return mContentEncoding;
            }
            set
            {
                mContentEncoding = value;
            }
        }

        public override bool HasETag
        {
            get
            {
                return mETag != null;
            }
        }

        public override string ETag
        {
            get
            {
                return mETag;
            }
            set
            {
                mETag = value;
            }
        }

        public override bool HasExpires
        {
            get
            {
                return mExpires != null;
            }
        }

        public override string Expires
        {
            get
            {
                return mExpires;
            }
            set
            {
                mExpires = value;
            }
        }
    }
}

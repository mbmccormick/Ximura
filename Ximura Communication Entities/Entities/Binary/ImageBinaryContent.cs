#region using
using System;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the base content holder for storing and munipulating binary content
    /// </summary>
    [XimuraContentTypeID("7806DA64-20DA-447E-BECD-F8DF6E7D9E76")]
    [XimuraContentCachePolicy(ContentCacheOptions.VersionCheck | ContentCacheOptions.Cacheable, 86400)] //1 days
    public class ImageBinaryContent : BinaryContent
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ImageBinaryContent() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ImageBinaryContent(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region FileName
        /// <summary>
        /// This override sets the MimeType for the specific extension.
        /// </summary>
        public override string FileName
        {
            get
            {
                return base.FileName;
            }
            set
            {
                base.FileName = value;
                FileInfo fi = new FileInfo(value);

                switch (fi.Extension.ToLowerInvariant())
                {
                    case ".png":
                        MimeType = "image/png";
                        break;
                    case ".jpg":
                        MimeType = "image/jpeg";
                        break;
                    case ".gif":
                        MimeType = "image/gif";
                        break;
                    default:
                        //mMimeType = "image/" + fi.Extension.ToLowerInvariant().Substring(1);
                        break;
                }
            }
        }
        #endregion // FileName
    }
}

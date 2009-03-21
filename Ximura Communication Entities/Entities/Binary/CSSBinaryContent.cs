#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the base content holder for storing and munipulating binary content
    /// </summary>
    [XimuraContentTypeID("9FCDE441-C438-46e3-86F7-16C8A20752E7")]
    [XimuraContentCachePolicy(ContentCacheOptions.VersionCheck | ContentCacheOptions.Cacheable, 86400)] //! day
    public class CSSBinaryContent : BinaryContent
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public CSSBinaryContent() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public CSSBinaryContent(System.ComponentModel.IContainer container)
            :
            base(container) { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public CSSBinaryContent(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        protected override string GetMimeType(byte[] buffer)
        {
            return "text/css";
        }
    }
}

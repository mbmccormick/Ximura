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
    [XimuraContentTypeID("B1BB9AE5-950F-4F08-B748-54B1CB5D7F40")]
    [XimuraContentCachePolicy(ContentCacheOptions.VersionCheck | ContentCacheOptions.Cacheable, 1200)] //20 minutes
    public class ScriptBinaryContent : BinaryContent
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ScriptBinaryContent()  { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ScriptBinaryContent(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        protected override string GetMimeType(byte[] buffer)
        {
            return "application/x-javascript";
        }
    }
}

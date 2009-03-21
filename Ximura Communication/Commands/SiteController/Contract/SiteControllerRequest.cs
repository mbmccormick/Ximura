#region using
using System;
using System.Runtime.Serialization;
using System.IO;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class is the base class for holding the parameters for the Site Manager request.
    /// </summary>
    public class SiteControllerRequest : RQServer
    {
        #region Declarations
        private Message mMessage;
        private Uri mMessageUri;
        private Uri mURILocal;
        private Uri mURIRemote;
        private string mServerType;
        private string mMessageMethod;
        private string mMessageUserAgent;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public SiteControllerRequest()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public SiteControllerRequest(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SiteControllerRequest(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Reset
        /// <summary>
        /// This override reset the specific data for the Site Manager Request.
        /// </summary>
        public override void Reset()
        {
            mServerType = null;
            mURILocal = null;
            mURIRemote = null;
            mMessage = null;
            mMessageUri = null;
            mMessageMethod = null;
            mMessageUserAgent = null;
            SessionID = null;
            base.Reset();
        }
        #endregion // Reset

        #region SessionID
        /// <summary>
        /// This is the session ID stored in the server.
        /// </summary>
        public Guid? SessionID
        {
            get;
            set;
        }
        #endregion // SessionID

        #region MessageUserAgent
        /// <summary>
        /// This is the message user agent.
        /// </summary>
        public string MessageUserAgent
        {
            get { return mMessageUserAgent; }
            set { mMessageUserAgent = value; }
        }
        #endregion // MessageUserAgent
        #region MessageMethod
        /// <summary>
        /// This is the request uri passed to the site controller command.
        /// </summary>
        public string MessageMethod
        {
            get { return mMessageMethod; }
            set { mMessageMethod = value; }
        }
        #endregion // MessageMethod
        #region MessageUri
        /// <summary>
        /// This is the request uri passed to the site controller command.
        /// </summary>
        public Uri MessageUri
        {
            get { return mMessageUri; }
            set { mMessageUri = value; }
        }
        #endregion // MessageUri
        #region Message
        /// <summary>
        /// This is the HTTP request.
        /// </summary>
        public Message Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }
        #endregion // Message

        #region URIRemote
        /// <summary>
        /// The remote path.
        /// </summary>
        public Uri URIRemote
        {
            get { return mURIRemote; }
            set { mURIRemote = value; }
        }
        #endregion // URIRemote
        #region URILocal
        /// <summary>
        /// The local path/
        /// </summary>
        public Uri URILocal
        {
            get { return mURILocal; }
            set { mURILocal = value; }
        }
        #endregion // URIRemote

        #region ServerType
        /// <summary>
        /// This is the request protocol, used to identify the calling party.
        /// </summary>
        public string ServerType
        {
            get { return mServerType; }
            set { mServerType = value; }
        }
        #endregion // Protocol
    }
}

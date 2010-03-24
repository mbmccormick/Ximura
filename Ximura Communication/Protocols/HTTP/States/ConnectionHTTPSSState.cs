#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state manages connection requests.
    /// </summary>
    public class ConnectionHTTPSSState : HTTPSSState
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ConnectionHTTPSSState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public ConnectionHTTPSSState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region IN --> ConnectionRequest
        /// <summary>
        /// This state manages connection requests from HTTP browsers.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The data in the request.</param>
        /// <returns>Returns false if the connection is successful, true if the connection should be reset.</returns>
        public override bool ConnectionRequest(SiteServerContext<HTTPSiteServerRQ, HTTPSiteServerRS> context, 
            SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            RQCallbackServer Request = Data.ContractRequest;
            RSCallbackServer Response = Data.ContractResponse;

            Response.ServerContextID = context.SignatureID;
            Response.Status = CH.HTTPCodes.OK_200;
            Response.MessageResponse = null;
            Response.MessageRequestType = typeof(HTTPRequestMessage);

            context.URILocal = Request.LocalUri;
            context.URIRemote = Request.RemoteUri;
            context.ProtocolCommandID = Data.Sender;
            context.ProtocolContextID = Request.ProtocolContextID;

            context.ExpiryTimeSet();
            context.ChangeState("AwaitRequest");

            return false;
        }
        #endregion // ConnectionRequest

    }
}

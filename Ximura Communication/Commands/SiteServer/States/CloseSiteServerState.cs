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
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    public class CloseSiteServerState<RQ, RS> : SiteServerState<RQ, RS>
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public CloseSiteServerState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public CloseSiteServerState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region Close
        protected IXimuraRQRSEnvelope EnvTransportClose(SiteServerContext<RQ, RS> context)
        {
            IXimuraRQRSEnvelope EnvClose = context.CreateSystemRequest(context.ProtocolCloseRequestAddress);
            IXimuraTransportRequest Request = EnvClose.Request as IXimuraTransportRequest;
            Request.ProtocolContextID = context.ProtocolContextID;
            Request.MessageType = null;
            Request.ServerContextID = context.SignatureID;
            Request.SignalClose = true;

            return EnvClose;
        }

        public override void Close(SiteServerContext<RQ, RS> context)
        {
            if (context.ClosePending)
                return;

            context.ClosePending = true;

            IXimuraRQRSEnvelope EnvClose = null;
            try
            {
                EnvClose = EnvTransportClose(context);
                context.ProcessSession.ProcessRequest(EnvClose);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (EnvClose != null && EnvClose.ObjectPoolCanReturn)
                    EnvClose.ObjectPoolReturn();
            }
        }

        /// <summary>
        /// This close method is called by the remote transport in response to a transport close.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true.</returns>
        public override bool Close(SiteServerContext<RQ, RS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            if (context.ClosePending)
                return true;

            context.ClosePending = true;

            return true;
        }
        #endregion // Close(SiteServerContext<RQ, RS> context)

        public override bool Receive(SiteServerContext<RQ, RS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            return base.Receive(context, job, Data);
        }

        public override bool ConnectionRequest(SiteServerContext<RQ, RS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            return base.ConnectionRequest(context, job, Data);
        }

        public override bool Transmit(SiteServerContext<RQ, RS> context, ProtocolConnectionIdentifiers identifier, IXimuraMessageStream messageOut, bool SignalClose)
        {
            return base.Transmit(context, identifier, messageOut, SignalClose);
        }


    }
}
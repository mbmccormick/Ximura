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
    public class SiteServerState<RQ, RS> : State
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Declaration
        /// <summary>
        /// This is the delegate used to pipe access to a particular command.
        /// </summary>
        /// <param name="context">The ftpcontext.</param>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns the status indicating whether the connection should remain open.</returns>
        protected delegate bool CommandDirect(
            SiteServerContext<RQ, RS> context, Message messageRQ,
            SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data);

        #endregion // Declaration

        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public SiteServerState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public SiteServerState(IContainer container):base(container)
		{
		}
		#endregion // Constructors

        //#region CreateSystemRequest
        ///// <summary>
        ///// This method creates the system request.
        ///// </summary>
        ///// <returns>A new envelope with the correct destination address.</returns>
        //protected IXimuraRQRSEnvelope CreateSystemRequest(EnvelopeAddress address)
        //{
        //    IXimuraRQRSEnvelope Env = RQRSEnvelopeHelper.Get(address.command);
        //    Env.Request.ID = Guid.NewGuid();
        //    Env.DestinationAddress = address;
        //    return Env;
        //}
        //#endregion // CreateSystemRequest

        #region Close
        /// <summary>
        /// This method switches the context to the close state and then calls the correct close method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="job"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public virtual bool Close(SiteServerContext<RQ, RS> context,
            SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            context.ChangeState("Close");
            return context.Close(job, Data);
        }
        /// <summary>
        /// This method switches the context to the close state and then calls the correct close method.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual void Close(SiteServerContext<RQ, RS> context)
        {
            context.ChangeState("Close");
            context.Close();
        }
        #endregion // Close

        public virtual bool Listen(SiteServerContext<RQ, RS> context, Uri listenOn)
        {
            throw new NotImplementedException("Listen is not implemented.");
        }

        public virtual bool ListenConfirm(SiteServerContext<RQ, RS> context, IXimuraRQRSEnvelope Env)
        {
            throw new NotImplementedException("ListenConfirm is not implemented.");
        }

        public virtual void Initialize(SiteServerContext<RQ,RS> context)
        {
            throw new NotImplementedException("Initialize is not implemented.");
        }

        public virtual bool ConnectionRequest(SiteServerContext<RQ,RS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            throw new NotImplementedException("ConnectionRequest is not implemented.");
        }

        public virtual bool Receive(SiteServerContext<RQ,RS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            throw new NotImplementedException("Receive is not implemented.");
        }

        public virtual bool Transmit(SiteServerContext<RQ,RS> context,
            ProtocolConnectionIdentifiers identifier, IXimuraMessageStream messageOut, bool SignalClose)
        {
            throw new NotImplementedException("Transmit is not implemented.");
        }


    }
}

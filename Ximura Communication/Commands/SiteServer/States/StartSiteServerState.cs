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
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the start state used to initialize a new connection or a listening connection.
    /// </summary>
    /// <typeparam name="RQ">The server request type.</typeparam>
    /// <typeparam name="RS">The server response type.</typeparam>
    public class StartSiteServerState<RQ, RS> : SiteServerState<RQ, RS>
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public StartSiteServerState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public StartSiteServerState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region Listen(SiteServerContext<RQ, RS> context, Uri listenOn)
        /// <summary>
        /// This method initializes a listen request for the specified location.
        /// </summary>
        /// <param name="context">The listen context</param>
        /// <param name="listenOn">The listen location.</param>
        /// <returns>Returns true if the listen request was successful.</returns>
        public override bool Listen(SiteServerContext<RQ, RS> context, Uri listenOn)
        {
            IXimuraTransportRequest Request = null;
            IXimuraTransportResponse Response = null;

            bool success = context.ListenerSchemeSet(listenOn);

            if (success)
            {
                //Ok, we will send a listen request to the protocol.
                IXimuraRQRSEnvelope Env = null;
                try
                {
                    Env = CreateSystemRequest(context.ProtocolListenRequestAddress);

                    Request = Env.Request as IXimuraTransportRequest;
                    Response = Env.Response as IXimuraTransportResponse;

                    Request.ProtocolUri = listenOn;
                    Request.ProtocolUriConnectionLimit = null;
                    Request.ServerAddress = context.ServerConnectionRequestAddress;

                    context.ProcessSession.ProcessRequest(Env);

                    switch (Env.Response.Status)
                    {
                        case "200": //OK, listener was successful. We need to confirm the listen request.
                            switch (Response.ConnectionType)
                            {
                                case TransportConnectionType.Connectionful:
                                    context.ChangeState("ListenConnectionful");
                                    break;
                                case TransportConnectionType.Connectionless:
                                    context.ChangeState("ListenConnectionless");
                                    break;
                                default:
                                    throw new NotSupportedException(
                                        string.Format("The TransportConnectionType ({0}) is not supported.",Response.ConnectionType));
                            }

                            //Return the confirmation status, just in case something goes wrong.
                            return context.ListenConfirm(Env);
                        default: //Error
                            break;
                    }
                }
                catch (Exception ex)
                {
                    XimuraAppTrace.WriteLine(ex.Message, "StartSiteServerState/Listen", EventLogEntryType.Warning);
                }
                finally
                {
                    if (Env != null && Env.ObjectPoolCanReturn)
                    {
                        //Return the envelope to the pool as we have finished with it.
                        Env.ObjectPoolReturn();
                    }
                }
            }
            //There is some form of error, so we will quit.
            context.ChangeState("Close");
            return false;
        }
        #endregion // Listen(SiteServerContext<RQ, RS> context, Uri listenOn)

        #region Initialize(SiteServerContext<RQ, RS> context)
        /// <summary>
        /// This method initializes the context for a new connection request.
        /// </summary>
        /// <param name="context"></param>
        public override void Initialize(SiteServerContext<RQ, RS> context)
        {

        }
        #endregion // Initialize(SiteServerContext<RQ, RS> context)

        #region ConnectionRequest
        /// <summary>
        /// This method is called when a listening context passes the request to a new context for processing.
        /// </summary>
        /// <param name="context">The new connection context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The protocol data.</param>
        /// <returns>Returns true is the context should be reset and returned to the pool, otherwise returns false and the connection will be persisted.</returns>
        public override bool ConnectionRequest(SiteServerContext<RQ, RS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            //Ok, change the context to the connection state and pass control to that state.
            context.ChangeState("Connection");

            return context.ConnectionRequest(job, Data);
        }
        #endregion // ConnectionRequest


    }
}

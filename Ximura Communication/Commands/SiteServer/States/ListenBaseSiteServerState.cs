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
    public class ListenBaseSiteServerState<RQ, RS> : SiteServerState<RQ, RS>
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ListenBaseSiteServerState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public ListenBaseSiteServerState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region ListenerConfirm(SiteServerContext<RQ, RS> listener)
        /// <summary>
        /// This method is called when the listener is confirmed.
        /// </summary>
        /// <param name="listener"></param>
        public override bool ListenConfirm(SiteServerContext<RQ, RS> listener, IXimuraRQRSEnvelope Env)
        {
            IXimuraRQRSEnvelope EnvConfirm = null;

            try
            {
                IXimuraTransportRequest EnvRQ = Env.Request as IXimuraTransportRequest;
                IXimuraTransportResponse EnvRS = Env.Response as IXimuraTransportResponse;

                listener.ProtocolContextID = EnvRS.ProtocolContextID;
                listener.ProtocolUri = EnvRS.ProtocolUri;

                EnvConfirm = listener.CreateSystemRequest(listener.ProtocolListenConfirmAddress);
                IXimuraTransportRequest Request = EnvConfirm.Request as IXimuraTransportRequest;
                Request.ProtocolContextID = listener.ProtocolContextID;
                Request.MessageType = listener.InitialMessageType;
                Request.ProtocolUriConnectionLimit = listener.ListenerConnectionLimit;
                Request.ServerContextID = listener.SignatureID;

                listener.ProcessSession.ProcessRequest(EnvConfirm);

                IXimuraTransportResponse Response = EnvConfirm.Response as IXimuraTransportResponse;

                string status = EnvConfirm.Response.Status;
                switch (status)
                {
                    case "200": //OK, listener was successful.
                        return true;
                    default: //Error, something has gone wrong.
                        XimuraAppTrace.WriteLine(string.Format("ListenerConfirm error -->{0}/{1}", status, EnvConfirm.Response.Substatus),
                            listener.CommandName, EventLogEntryType.Warning);
                        return false;
                }
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine(string.Format("ListenerConfirm exception --> {0}", ex.Message),
                    listener.CommandName, EventLogEntryType.Error);
                return false;
            }
            finally
            {
                if (EnvConfirm != null && EnvConfirm.ObjectPoolCanReturn)
                {
                    //Return the envelope to the pool as we have finished with it.
                    EnvConfirm.ObjectPoolReturn();
                }
            }

        }
        #endregion // ListenConfirm

        public override void Close(SiteServerContext<RQ, RS> context)
        {
            //There is nothing to do here. We should just close.
        }

        public override bool Close(SiteServerContext<RQ, RS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            return base.Close(context, job, Data);
        }
    }
}

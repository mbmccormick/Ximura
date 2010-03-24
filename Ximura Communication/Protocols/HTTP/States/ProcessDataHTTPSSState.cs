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

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    public class ProcessDataHTTPSSState : HTTPSSState
    {
        #region Declarations
        private Guid mSiteManager = new Guid("62A74574-7425-4cc2-B2F1-74EF38689725");
        #endregion // Declarations
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ProcessDataHTTPSSState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public ProcessDataHTTPSSState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region IN --> Receive
        /// <summary>
        /// This method receives requests from the HTTP agent and passes the request to the Site Controller.
        /// </summary>
        /// <param name="httpContext">The current context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the connection should be reset.</returns>
        public override bool Receive(SiteServerContext<HTTPSiteServerRQ, HTTPSiteServerRS> context, 
            SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            RQCallbackServer Request = Data.ContractRequest;
            RSCallbackServer Response = Data.ContractResponse;

            HTTPRequestMessage httpRQ = null;
            RQRSContract<SiteControllerRequest, SiteControllerResponse> Env = null;
            try
            {
                httpRQ = Request.Message as HTTPRequestMessage;

                Env = context.EnvelopeHelper.Get(mSiteManager) as
                    RQRSContract<SiteControllerRequest, SiteControllerResponse>;

                context.SenderIdentitySet((IXimuraRQRSEnvelope)Env);
                Env.DestinationAddress = new EnvelopeAddress(mSiteManager, "Receive");

                //HTTPServerContext httpContext = context as HTTPServerContext;

                Env.ContractRequest.MessageMethod = httpRQ.Instruction.Verb;
                Env.ContractRequest.Message = httpRQ;
                Env.ContractRequest.MessageUri = ExtractUri(httpRQ, context.URILocal);
                Env.ContractRequest.MessageUserAgent = ExtractUserAgent(httpRQ);

                Env.ContractRequest.ServerType = "http";

                Env.ContractRequest.URILocal = context.URILocal;
                Env.ContractRequest.URIRemote = context.URIRemote;

                context.ExpiryTime = null;

                Guid signatureID = context.SignatureID.Value;
                //Send the request synchronously to the server using the current job, 
                //so that it is processed on the current thread.
                job.ProcessRequest((IXimuraRQRSEnvelope)Env);

                //Check whether the context has been reset during the call to the Site Controller.
                if (context == null || !context.SignatureID.HasValue || context.SignatureID.Value != signatureID)
                {
                    Response.Status = CH.HTTPCodes.InternalServerError_500;
                    Response.CloseNotify = true;
                    return false;
                }

                context.ExpiryTimeSet();

                Response.MessageResponse = Env.ContractResponse.Message;
                Response.MessageRequestType = typeof(HTTPRequestMessage);
                Response.MaxLength = 2000000;

                Response.Status = CH.HTTPCodes.OK_200;
            }
            catch (Exception ex)
            {
                Response.Status = CH.HTTPCodes.InternalServerError_500;
                Response.Substatus = ex.Message;
            }
            finally
            {
                Response.ProtocolContextID = Request.ProtocolContextID;
                Response.ServerContextID = context.SignatureID;

                if (httpRQ != null || httpRQ.ObjectPoolCanReturn)
                {
                    httpRQ.ObjectPoolReturn();
                }
                if (Env != null || Env.ObjectPoolCanReturn)
                {
                    Env.ObjectPoolReturn();
                }

                context.ChangeState("AwaitRequest");
            }
            return false;
        }
        #endregion // Receive

        public override void Close(SiteServerContext<HTTPSiteServerRQ, HTTPSiteServerRS> context)
        {
            base.Close(context);
        }

        public override bool Close(SiteServerContext<HTTPSiteServerRQ, HTTPSiteServerRS> context, SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            return base.Close(context, job, Data);
        }
    }
}

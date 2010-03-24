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
    /// <summary>
    /// This state is used to handle connectionful protocol request for a listening context.
    /// </summary>
    /// <typeparam name="RQ">The request type.</typeparam>
    /// <typeparam name="RS">The response type.</typeparam>
    public class ListenConnectionfulSSState<RQ, RS> : ListenBaseSiteServerState<RQ, RS>
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ListenConnectionfulSSState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public ListenConnectionfulSSState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region ConnectionRequest
        /// <summary>
        /// This method processes an incoming connection request and assigns it to a new context.
        /// </summary>
        /// <param name="context">The listening context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request/response data.</param>
        /// <returns>Always returns false as true would close the listening connection.</returns>
        public override bool ConnectionRequest(SiteServerContext<RQ, RS> context,
            SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            //Check that we can access the context pool to get a new context to handle the connection?
            if (!context.ContextPoolAccessGranted)
            {
                //No, then we have to decline the connection as we cannot retrieve a context to process the connection.
                Data.ContractResponse.Status = CH.HTTPCodes.BadRequest_400;
                Data.ContractResponse.Substatus = "No contexts available: context pool access not granted.";
                return false;
            }

            bool resetNewContext = false;
            SiteServerContext<RQ, RS> connContext = null;

            try
            {
                connContext = context.ContextPoolAccess.ContextGetGeneric() as SiteServerContext<RQ, RS>;

                if (connContext == null)
                {
                    Data.ContractResponse.Status = CH.HTTPCodes.BadRequest_400;
                    Data.ContractResponse.Substatus = "No contexts available: ContextGetGeneric() returned null.";
                    return false;
                }

                //Ok, initialize the context.
                connContext.ProtocolContextID = Data.ContractRequest.ProtocolContextID;
                connContext.ProtocolCommandID = context.ProtocolCommandID;

                connContext.Initialize();
                connContext.ChangeState("Connection");
                resetNewContext = connContext.ConnectionRequest(job, Data);
            }
            catch (Exception ex)
            {
                resetNewContext = true;
                XimuraAppTrace.WriteLine(ex.Message, "ListenConnectionfulSSState/ConnectionRequest", EventLogEntryType.Warning);
            }
            finally
            {
                if (resetNewContext && connContext!=null)
                {
                    connContext.Close();
                    context.ContextPoolAccess.ContextReturnGeneric(connContext);
                }
            }

            return false;
        }

        #endregion // ConnectionRequest
    }
}

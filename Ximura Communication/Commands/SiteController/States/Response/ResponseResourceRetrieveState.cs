#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ResponseResourceRetrieveState : SiteControllerState
    {
        #region Declarations
        private Guid mResourceManager = new Guid("40BC9B93-92F8-4f66-8698-E59A9631876D");
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResponseResourceRetrieveState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResponseResourceRetrieveState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region RequestProcess()
        /// <summary>
        /// This method processes the actual request.
        /// </summary>
        /// <param name="context">The active context.</param>
        public override bool RequestProcess(SiteControllerContext context)
        {
            RQRSContract<ResourceManagerRequest, ResourceManagerResponse> Env = null;
            //Ok, we need to call the Content Compiler to build the output content.
            try
            {
                Env = RQRSEnvelopeHelper.Get<ResourceManagerRequest, ResourceManagerResponse>();
                Env.DestinationAddress = new EnvelopeAddress(mResourceManager, "Receive");

                Env.ContractRequest.Data = context.ScriptRequest;
                Env.ContractRequest.Settings = context.ScriptSettings;

                context.SenderIdentitySet((IXimuraRQRSEnvelope)Env);

                context.Job.ProcessRequest(Env);

                context.ScriptRequest.ResponseStatus = Env.ContractResponse.Status;
                context.ScriptRequest.ResponseStatusMessage = Env.ContractResponse.Substatus;
                context.ProtocolResponse.Body = Env.ContractResponse.Body;
            }
            catch (Exception ex)
            {
                context.ChangeState("Error");
            }
            finally
            {
                if (Env != null)
                    Env.ObjectPoolReturn();
            }

            return false;
        }
        #endregion
    }
}

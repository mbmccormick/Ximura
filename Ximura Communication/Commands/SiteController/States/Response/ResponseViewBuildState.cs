#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state is used to redirect the request to the content compiler command, which
    /// processes the page.
    /// </summary>
    public class ResponseViewBuildState : SiteControllerState
    {
        #region Declarations
        private Guid mContentCompiler = new Guid("A33D715F-6960-45ad-B4C9-A3BF7FBFD15C");
        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResponseViewBuildState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResponseViewBuildState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region RequestProcess()
        /// <summary>
        /// This method processes the actual request.
        /// </summary>
        /// <param name="context">The active context</param>
        public override bool RequestProcess(SiteControllerContext context)
        {
            RQRSContract<ContentCompilerRequest, ContentCompilerResponse> Env = null;
            //Ok, we need to call the Content Compiler to build the output content.
            try
            {
                Env = context.EnvelopeHelper.Get<ContentCompilerRequest, ContentCompilerResponse>();

                Env.ContractRequest.Data = context.ScriptRequest;
                Env.ContractRequest.Settings = context.ScriptSettings;

                context.SenderIdentitySet((IXimuraRQRSEnvelope)Env);

                Env.DestinationAddress = new EnvelopeAddress(mContentCompiler, "Receive");

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

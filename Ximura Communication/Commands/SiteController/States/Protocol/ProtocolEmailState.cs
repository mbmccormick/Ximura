#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ProtocolEmailState : ProtocolBaseState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ProtocolEmailState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ProtocolEmailState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        public override bool MessageDecode(SiteControllerContext context)
        {
            //Set the HTTP response message.
            context.ProtocolResponse = context.GetObjectPool<InternetMessageResponse>().Get();
            context.ProtocolResponse.BeginInit();

            ControllerRequest contRQ = context.ScriptRequest;

            contRQ.RequestURI = context.RequestURI;

            contRQ.RequestVerb = context.RequestMethod;

            return context.ScriptRequestResolved;
        }

        public override void ResponsePrepare(SiteControllerContext context)
        {

        }

        public override void ResponseComplete(SiteControllerContext context)
        {
            TranslateResponseCode(context); 
            
            context.ProtocolResponse.Instruction.Protocol = "EMAIL";
            context.ProtocolResponse.Instruction.Version = "1.0";

            context.ProtocolResponse.HeaderAdd("Server", context.ContextSettings.ServerID);
            context.ProtocolResponse.HeaderAdd("Date", DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT");

            context.ProtocolResponse.EndInit();

            context.Response.Status = context.ScriptRequest.ResponseStatus;
        }

        protected override void SetErrorBody(SiteControllerContext context, string errorCode, string errorMessage, string errorDescription)
        {
            //We don't care.
        }

        public override void Log(SiteControllerContext context)
        {
        }
    }
}

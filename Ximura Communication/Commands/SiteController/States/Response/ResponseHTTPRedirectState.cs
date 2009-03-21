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
    /// <summary>
    /// This state is used to send a HTTP response to the browser.
    /// </summary>
    public class ResponseHTTPRedirectState : SiteControllerState
    {
        #region Declarations

        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResponseHTTPRedirectState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResponseHTTPRedirectState(IContainer container)
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
            //Ok, we need to call the Content Compiler to build the output content.
            try
            {
                string redirect = context.ScriptSettings.OutputColl[0].OutputType;

                switch (redirect)
                {
                    case "relative":
                        UriBuilder uri = new UriBuilder(context.ScriptRequest.RequestProtocol
                            , context.ScriptRequest.RequestHost, context.ScriptRequest.RequestPort.Value
                            , context.ScriptSettings.OutputColl[0].Output);
                        context.ScriptRequest.ResponseHeaderAdd("Location", uri.ToString());
                        break;
                    case "absolute":
                        context.ScriptRequest.ResponseHeaderAdd("Location", context.ScriptSettings.OutputColl[0].Output);
                        break;
                    default:
                        context.ScriptRequest.ResponseStatus = CH.HTTPCodes.InternalServerError_500;
                        context.ScriptRequest.ResponseStatusMessage = "Internal Server Error";
                        context.ChangeState("Error");
                        return false;
                }

                context.ScriptRequest.ResponseStatus = CH.HTTPCodes.SeeOther_303;
                context.ScriptRequest.ResponseStatusMessage = "See Other";
                context.ProtocolResponse.Body = null;
            }
            catch (Exception ex)
            {
                context.ScriptRequest.ResponseStatus = CH.HTTPCodes.InternalServerError_500;
                context.ScriptRequest.ResponseStatusMessage = "Internal Server Error";
                context.ChangeState("Error");
            }

            return false;
        }
        #endregion
    }
}

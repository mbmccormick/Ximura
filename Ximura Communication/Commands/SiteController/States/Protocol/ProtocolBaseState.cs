#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;

using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state implements the base protocol functions.
    /// </summary>
    public abstract class ProtocolBaseState : SiteControllerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ProtocolBaseState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ProtocolBaseState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region IN --> SessionResolve(SiteControllerContext context)
        /// <summary>
        /// This method resolves the session.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns true if the session can be resolved.</returns>
        public override bool SessionResolve(SiteControllerContext context)
        {
            ControllerSession session;

            switch (context.CDSHelper.Read<ControllerSession>(context.ScriptRequest.SessionID, null, out session))
            {
                case CDSResponse.OK:
                    context.ScriptSession = session;
                    context.ScriptRequest.SessionID = session.IDContent;

                    ProcessMemberSecurity(context);
                    return true;
            }

            return false;
        }
        #endregion // SessionResolve(SiteManagerContext context)
        #region ProcessMemberSecurity(SiteControllerContext context)
        /// <summary>
        /// This method processes any security parameters retrieved from the Member Session.
        /// </summary>
        /// <param name="context">The current context.</param>
        protected virtual void ProcessMemberSecurity(SiteControllerContext context)
        {
        }
        #endregion // ProcessMemberSecurity(SiteControllerContext context)

        #region TranslateResponseCode(SiteControllerContext context)
        /// <summary>
        /// This method translates in outgoing error code and sets the reponse instruction
        /// as well as setting the response message body for error conditions.
        /// </summary>
        /// <param name="context">The current context.</param>
        protected virtual void TranslateResponseCode(SiteControllerContext context)
        {
            //If the body has already been set, then there is nothing to do here.

            string code = context.ScriptRequest.ResponseStatus;
            context.ProtocolResponse.Instruction.Verb = code;
            //context.ProtocolResponse.Instruction.Instruction = context.ScriptRequest.ResponseStatusMessage;
            switch (code)
            {
                case "":
                case "100":
                    context.ProtocolResponse.Instruction.Verb = "404";
                    context.ProtocolResponse.Instruction.Instruction = "Not found";
                    SetErrorBody(context, CH.HTTPCodes.NotFound_404);
                    return;
                case "200":
                    context.ProtocolResponse.Instruction.Instruction = "OK";
                    return;
                case "201":
                    context.ProtocolResponse.Instruction.Instruction = "Created";
                    return;
                case "202":
                    context.ProtocolResponse.Instruction.Instruction = "Accepted";
                    return;
                case "203":
                    context.ProtocolResponse.Instruction.Instruction = "Non-authoritative Information";
                    return;
                case "204":
                    context.ProtocolResponse.Instruction.Instruction = "No Content";
                    return;
                case "205":
                    context.ProtocolResponse.Instruction.Instruction = "Reset Content";
                    return;
                case "206":
                    context.ProtocolResponse.Instruction.Instruction = "Partial Content";
                    return;

                case "300":
                    context.ProtocolResponse.Instruction.Instruction = "Multiple Choices";
                    return;
                case "301":
                    context.ProtocolResponse.Instruction.Instruction = "Moved Permanently";
                    return;
                case "302":
                    context.ProtocolResponse.Instruction.Instruction = "Found";
                    return;
                case "303":
                    context.ProtocolResponse.Instruction.Instruction = "See Other";
                    if (context.ProtocolResponse.Body == null)
                        SetErrorBody(context, "303", "Just for Firefox.", "Oh Firefox, Firefox, why! Why do you taunt me so? Why can't you be nice and behaved like that lovely IE?");
                    return;
                case "304":
                    context.ProtocolResponse.Instruction.Instruction = "Not Modified";
                    return;
                case "305":
                    context.ProtocolResponse.Instruction.Instruction = "Use Proxy";
                    return;
                case "306":
                    context.ProtocolResponse.Instruction.Instruction = "What the F**K?";
                    return;
                case "307":
                    context.ProtocolResponse.Instruction.Instruction = "Moved Temporarily";
                    if (context.ProtocolResponse.Body == null)
                        SetErrorBody(context, "307");
                    return;
            }

            //Ok, we are now in error response codes, so we may need to set an error body.
            switch (code)
            {
                case "400":
                    context.ProtocolResponse.Instruction.Instruction = "Bad Request";
                    break;
                case "401":
                    context.ProtocolResponse.Instruction.Instruction = "Unauthorized";
                    break;
                case "402":
                    context.ProtocolResponse.Instruction.Instruction = "Payment Required - apparently";
                    break;
                case "403":
                    context.ProtocolResponse.Instruction.Instruction = "Forbidden";
                    break;
                case "404":
                    context.ProtocolResponse.Instruction.Instruction = "Not Found";
                    break;
                case "405":
                    context.ProtocolResponse.Instruction.Instruction = "Method Not Allowed";
                    break;
                case "406":
                    context.ProtocolResponse.Instruction.Instruction = "Not Acceptable";
                    break;
                case "407":
                    context.ProtocolResponse.Instruction.Instruction = "Proxy Authentication Required";
                    break;
                case "408":
                    context.ProtocolResponse.Instruction.Instruction = "Request Time-out";
                    break;
                case "409":
                    context.ProtocolResponse.Instruction.Instruction = "Conflict";
                    break;
                case "410":
                    context.ProtocolResponse.Instruction.Instruction = "Gone";
                    break;
                case "411":
                    context.ProtocolResponse.Instruction.Instruction = "Length Required";
                    break;
                case "412":
                    context.ProtocolResponse.Instruction.Instruction = "Precondition Failed";
                    break;
                case "413":
                    context.ProtocolResponse.Instruction.Instruction = "Request Entity Too Large";
                    break;
                case "414":
                    context.ProtocolResponse.Instruction.Instruction = "Request URL Too Long";
                    break;
                case "415":
                    context.ProtocolResponse.Instruction.Instruction = "Unsupported Media Type";
                    break;
                case "416":
                    context.ProtocolResponse.Instruction.Instruction = "Request Range Not Satisfiable";
                    break;
                case "417":
                    context.ProtocolResponse.Instruction.Instruction = "Expectation Failed";
                    break;
                case "500":
                    context.ProtocolResponse.Instruction.Instruction = "Internal Server Error";
                    break;
                case "501":
                    context.ProtocolResponse.Instruction.Instruction = "Not Implemented";
                    break;
                case "502":
                    context.ProtocolResponse.Instruction.Instruction = "Bad Gateway";
                    break;
                case "503":
                    context.ProtocolResponse.Instruction.Instruction = "Service Unavailable";
                    break;
                case "504":
                    context.ProtocolResponse.Instruction.Instruction = "Gateway Time-out";
                    break;
                case "505":
                    context.ProtocolResponse.Instruction.Instruction = "HTTP Version Not Supported";
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (context.ProtocolResponse.Body == null)
                SetErrorBody(context, code);
        }
        #endregion // TranslateResponseCode(SiteControllerContext context)
        #region SetErrorBody(SiteManagerContext context, string errorCode)
        /// <summary>
        /// This method will set the error body.
        /// </summary>
        /// <param name="ResponseCode">The error response code.</param>
        protected virtual void SetErrorBody(SiteControllerContext context, string errorCode)
        {
            SetErrorBody(context, errorCode, null, null);
        }

        protected virtual void SetErrorBody(SiteControllerContext context, string errorCode, string errorMessage)
        {
            SetErrorBody(context, errorCode, errorMessage, null);
        }

        protected virtual void SetErrorBody(SiteControllerContext context, string errorCode, string errorMessage, string errorDescription)
        {
            HTMLErrorBodyFragment newFrag = context.GetObjectPool<HTMLErrorBodyFragment>().Get();
            newFrag.BeginInit();
            newFrag.ErrorTypeSet(errorCode, errorMessage, errorDescription);
            context.ProtocolResponse.Body = newFrag;
        }
        #endregion

    }
}

#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state selects the appropraite output method state.
    /// </summary>
    public class OutputCDSRMState : OutputRMState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputCDSRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputCDSRMState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region OutputPrepare(ContentCompilerContext context)
        /// <summary>
        /// This method selects the appropriate node and creates a Body content.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void OutputPrepare(ResourceManagerContext context)
        {
            Content content = null;
            try
            {
                string refType = "name";// context.Request.Data.VariableGet("resourcetype");
                string refValue = context.Request.Data.VariableGet("resourcevalue");

                Type entityType = RH.CreateTypeFromString(context.Request.Settings.OutputColl[0].Output);

                string status= context.CDSHelper.Execute(entityType, 
                    CDSData.Get(CDSStateAction.Read, refType, refValue)
                    , out content);

                if (status == CH.HTTPCodes.OK_200 && content is BinaryContent)
                {
                    string statusBody;
                    context.Response.Body = PrepareBody(context, (BinaryContent)content, out statusBody);
                    context.Response.Status = statusBody;
                }
                else
                {
                    context.Response.Body = null;
                    context.Response.Status = CH.HTTPCodes.NotFound_404;
                }
            }
            catch (Exception ex)
            {
                context.Response.Body = null;
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
            }
            finally
            {
                if (content != null && content.ObjectPoolCanReturn)
                    content.ObjectPoolReturn();
                content = null;
            }
        }
        #endregion // OutputPrepare(ContentCompilerContext context)

        public override void OutputComplete(ResourceManagerContext context)
        {
            //Do nothing
        }
    }
}

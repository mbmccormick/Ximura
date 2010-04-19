#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class OutputXPathCCState : OutputBaseCCState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputXPathCCState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputXPathCCState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region OutputPrepare(ContentCompilerContext context)
        /// <summary>
        /// This method selects the appropriate node and creates a Body content.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void OutputPrepare(ContentCompilerContext context)
        {
            try
            {
                string path = context.Request.Settings.OutputColl[0].Output;
                string outputType = context.Request.Settings.OutputColl[0].OutputType;
                string data;
                if (path == null || outputType == "xslt")
                    data = context.ModelData.Payload.InnerXml;
                else
                {
                    data = context.ModelData.ExtractPathData(path);
                }

                if (data == null)
                {
                    context.Response.Status = CH.HTTPCodes.ExpectationFailed_417;
                    context.Response.Substatus = "Node resolution error -> " + context.Request.Data.ResponseOutput;
                    return;
                }

                context.Response.Body = PrepareBody(context, data);
                context.Response.Body.ContentType = context.Request.Settings.OutputColl[0].OutputMIMEType;
                context.Response.Status = CH.HTTPCodes.OK_200;
            }
            catch (Exception ex)
            {
                context.Response.Body = null;
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
            }
        }
        #endregion // OutputPrepare(ContentCompilerContext context)


    }
}

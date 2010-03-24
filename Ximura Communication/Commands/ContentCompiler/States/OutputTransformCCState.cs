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
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class OutputTransformCCState : OutputBaseCCState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputTransformCCState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputTransformCCState(IContainer container)
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
            string stylesheetRef = null;
            try
            {
                if (context.Request.Data.OutputRawXML)
                {
                    string data = context.ModelData.XmlDataDoc.InnerXml; ;
                    context.Response.Body = PrepareBody(context, data);
                    context.Response.Body.ContentType = "application/xml; charset=utf-8";
                    context.Response.Status = CH.HTTPCodes.OK_200;
                    return;
                }

                 //string stylesheetRef = context.Request.Data.ResponseStylesheet;
                stylesheetRef = context.Request.Settings.OutputColl[0].Output;

                Stylesheet stylesheet = context.StylesheetGet("name", stylesheetRef);
                if (stylesheet == null)
                    throw new InvalidStylesheetCCException(string.Format("Stylesheet cannot be found: {0}", stylesheetRef));

                byte[] blob = stylesheet.Transform(context.ModelData);
                context.Response.Body = PrepareBody(context, blob);
                context.Response.Body.ContentType = context.Request.Settings.OutputColl[0].OutputMIMEType;

                context.Request.Data.ResponseHeaderAdd("ETag", PreparePageETag(context.ModelData));
                context.Request.Data.ResponseHeaderAdd("Cache-control", "must-revalidate");
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

        private string PreparePageETag(Model data)
        {
            Guid ID = data.IDContent;
            Guid Version = data.IDVersion;

            return @"""" + Version.ToString("N").ToUpper() + @"""";
        }
    }
}

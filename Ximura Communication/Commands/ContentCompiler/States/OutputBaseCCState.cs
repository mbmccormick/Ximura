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
using System.IO.Compression;
using System.ComponentModel;
using System.Text;
using System.Xml;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class OutputBaseCCState : ContentCompilerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputBaseCCState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputBaseCCState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors


        public override void OutputComplete(ContentCompilerContext context)
        {
            //Nothing set
        }
        
        #region PrepareBody(ContentCompilerContext context, string data)
        /// <summary>
        /// This method prepares the HTTP body based on the browser content encoding settings.
        /// </summary>
        /// <param name="context">The current compile context.</param>
        /// <param name="data">The transformed data.</param>
        /// <returns>Returns a ContentCompilerMessageFragmentBody object containing the prepared output binary data.</returns>
        protected virtual ContentCompilerMessageFragmentBody PrepareBody(ContentCompilerContext context, string data)
        {
            byte[] blob = Encoding.UTF8.GetBytes(data);
            return PrepareBody(context, blob);
        }
        /// <summary>
        /// This method prepares the HTTP body based on the browser content encoding settings.
        /// </summary>
        /// <param name="context">The current compile context.</param>
        /// <param name="blob">The UTF-8 formatted byte array.</param>
        /// <returns>Returns a ContentCompilerMessageFragmentBody object containing the prepared output binary data.</returns>
        protected virtual ContentCompilerMessageFragmentBody PrepareBody(ContentCompilerContext context, byte[] blob)
        {
            ContentCompilerMessageFragmentBody
                body = context.GetObjectPool<ContentCompilerMessageFragmentBody>().Get();

            switch (context.Request.Data.RequestPreferredCompression)
            {
                case "gzip":
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Stream gz = new GZipStream(ms, CompressionMode.Compress, true))
                        {
                            gz.Write(blob, 0, blob.Length);
                            gz.Close();
                        }
                        ms.Position = 0;
                        body.Load(ms);
                    }
                    body.ContentEncoding = "gzip";
                    break;
                case "deflate":
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Stream gz = new DeflateStream(ms, CompressionMode.Compress, true))
                        {
                            gz.Write(blob, 0, blob.Length);
                            gz.Close();
                        }
                        ms.Position = 0;
                        body.Load(ms);
                    }
                    body.ContentEncoding = "deflate";
                    break;
                default:
                    body.Load(blob, 0, blob.Length);
                    break;
            }

            blob = null;

            body.ContentType = context.Request.Data.ResponseOutputMIMEType;

            return body;
        }
        #endregion // PrepareBody(ContentCompilerContext context, string data)

    }
}

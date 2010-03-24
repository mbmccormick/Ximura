#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state selects the appropraite output method state.
    /// </summary>
    public class OutputRMState : ResourceManagerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputRMState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region PrepareBodyOutput(ResourceManagerContext context, BinaryContent content)
        /// <summary>
        /// This method prepares the binary body for output from the content.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="content">The content to be prepared.</param>
        /// <returns>Returns a ResourceManagerMessageFragmentBody object with the binary representation of the content.</returns>
        protected virtual ResourceManagerMessageFragmentBody PrepareBodyOutput(ResourceManagerContext context, BinaryContent content)
        {
            ResourceManagerMessageFragmentBody
                body = context.GetObjectPool<ResourceManagerMessageFragmentBody>().Get();

            byte[] blob = content.ToArray();

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

            if (content.MimeType == null || content.MimeType == "")
            {
                if (context.Request.Settings.OutputColl.Count > 0)
                    body.ContentType = context.Request.Settings.OutputColl[0].OutputMIMEType;
                else
                    body.ContentType = "application/octet-stream";
            }
            else
                body.ContentType = content.MimeType;

            return body;
        }
        #endregion // PrepareBodyOutput(ResourceManagerContext context, BinaryContent content)

        #region PrepareBody(ContentCompilerContext context, string data)
        /// <summary>
        /// This method prepares the HTTP body based on the browser content encoding settings.
        /// </summary>
        /// <param name="context">The current compile context.</param>
        /// <param name="blob">The UTF-8 formatted byte array.</param>
        /// <returns>Returns a ContentCompilerMessageFragmentBody object containing the prepared output binary data.</returns>
        protected virtual ResourceManagerMessageFragmentBody PrepareBody(
            ResourceManagerContext context, BinaryContent content, out string status)
        {
            DateTime? expiry = null;

            string cacheControl = context.Request.Data.RequestHeaderGet("Cache-Control");
            bool allowNotModified = cacheControl == null || cacheControl.ToLowerInvariant() != "no-cache";

            
            int? expiryIns = content.CacheExpiry;
            ResourceManagerMessageFragmentBody body = null;

            string eTag = content.IDVersion.ToString("N").ToUpperInvariant();
            string eTagValidate = context.Request.Data.RequestIfNoneMatch.Trim(new char[]{'"',' '});

            if ((content.CacheOptions & ContentCacheOptions.Cacheable)>0)
                expiry = DateTime.Now.AddSeconds(expiryIns.Value);

            context.ContextSettings.ETagAdd(content.IDVersion, content.IDContent, expiry, content.GetType(), expiryIns);

            //Next check whether we have an etag in the request, and whether it matches the content.
            if (!allowNotModified || eTagValidate == "" || eTag != eTagValidate)
            {
                //Ok, get the body.
                body = PrepareBodyOutput(context, content);

                body.ETag = string.Format(@"""{0}""", eTag);
                if (expiry.HasValue)
                    body.Expires = CH.ConvertToRFC1123DateString(expiry.Value);

                status = CH.HTTPCodes.OK_200;
            }
            else
            {
                if (expiry.HasValue)
                    context.Request.Data.ResponseHeaderAdd("Expires", CH.ConvertToRFC1123DateString(expiry.Value));
                context.Request.Data.ResponseHeaderAdd("ETag", string.Format(@"""{0}""", eTag));

                status = CH.HTTPCodes.NotModified_304;
            }

            if (expiry.HasValue)
                context.Request.Data.ResponseHeaderAdd("Cache-control", "public");
            else if ((content.CacheOptions & ContentCacheOptions.VersionCheck) > 0)
                context.Request.Data.ResponseHeaderAdd("Cache-control", "must-revalidate");

            return body;
        }
        #endregion

    }
}

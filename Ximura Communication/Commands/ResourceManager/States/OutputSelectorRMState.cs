#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Helper;
using Ximura.Persistence;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state selects the appropraite output method state.
    /// </summary>
    public class OutputSelectorRMState : ResourceManagerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputSelectorRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputSelectorRMState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region OutputSelect(ContentCompilerContext context)
        /// <summary>
        /// This method selects the appropriate output style.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void OutputSelect(ResourceManagerContext context)
        {
            string id = context.Request.Settings.OutputColl[0].OutputType;
            switch (id)
            {
                case "file":
                    context.CheckChangeState("RS_OutputFile");
                    break;
                case "resource":
                    context.CheckChangeState("RS_OutputResource");
                    break;
                case "cds":
                    context.CheckChangeState("RS_OutputCDS");
                    break;
                //case "app":
                //    context.CheckChangeState("RS_OutputAPP");
                //    break;
                default:
                    throw new InvalidOutputCCException(@"Output format """ + id + @""" not recognised.");
            }
        }
        #endregion // OutputSelect(ContentCompilerContext context)

        #region RequestValidate(ResourceManagerContext context)
        /// <summary>
        /// This method determines whether the entity has not been modified, and if so
        /// simply instructs the calling party that that is the case.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns false if the output logic should not be processed.</returns>
        public override bool RequestValidate(ResourceManagerContext context)
        {
            //Check whether the request has an ETag.
            string ETag = context.Request.Data.RequestIfNoneMatch;
            context.CheckChangeState("OutputSelector");

            if (ETag == null || ETag == "")
                return true;

            try
            {
                Guid vid = new Guid(ETag.Trim(new char[] { '"' }));
                Guid? cid;
                DateTime? expiry;
                Type contentType;


                if (!context.ContextSettings.ETagValidate(vid, out cid, out expiry, out contentType))
                    return true;


                if (expiry.HasValue && expiry.Value > DateTime.Now)
                {
                    context.Request.Data.ResponseHeaderAdd("Expires", CH.ConvertToRFC1123DateString(expiry.Value));
                    //set the cache control to public as we want these resources to be as cached as possible.
                    context.Request.Data.ResponseHeaderAdd("Cache-control", "public");
                }
                else if (!RevalidateAndUpdateETag(context, vid, cid.Value, contentType)) //OK, check the CDS to see if the content has changed.
                {
                    context.Request.Data.ResponseHeaderAdd("X-debug", "cachehit revalidate fail");

                    return true;
                }
                else
                    context.Request.Data.ResponseHeaderAdd("Cache-control", "must-revalidate");

                context.Request.Data.ResponseHeaderAdd("ETag", ETag);
        
                context.Response.Status = CH.HTTPCodes.NotModified_304;
                context.Response.Substatus = "Not Modified";
                return false;
            }
            catch (Exception ex)
            {
                context.Request.Data.ResponseHeaderAdd("X-debug", "cachehit exception");
                return true;
            }
        }
        #endregion // RequestValidate(ResourceManagerContext context)

        #region RevalidateAndUpdateETag
        /// <summary>
        /// This method revalidates the etag versionID values with the Content Data Store. 
        /// This method also resets the content expiry time, or removes the ETag from the cache if the version is invalid.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="vidIn">The Etag version ID.</param>
        /// <param name="cidIn">The content ID.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>Returns true if the ETag is valid.</returns>
        protected virtual bool RevalidateAndUpdateETag(ResourceManagerContext context,
            Guid vidIn, Guid cidIn, Type contentType)
        {
            Guid? cid, vid;
            string status = context.CDSHelper.Execute(contentType,
                CDSData.Get(CDSStateAction.VersionCheck, cidIn, null), out cid, out vid);

            if (status != CH.HTTPCodes.OK_200)
                return false;

            bool success = vidIn == vid;

            if (success)
                context.ContextSettings.ETagExpiryReset(vidIn);
            else
                context.ContextSettings.ETagCacheFlush(cidIn);

            return success;
        }
        #endregion // RevalidateETag

    }
}

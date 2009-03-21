#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using Ximura;
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
    public class OutputResourceRMState : OutputRMState
    {
        #region Declarations
        private Dictionary<Uri, GuidHolder> mInternalResourceCache;

        private object syncLockCache = new object();

        private struct GuidHolder
        {
            public GuidHolder(Guid ID, Guid Version, byte[] blob)
            {
                this.ID=ID;
                this.Version = Version;
                this.blob = blob;
            }

            public Guid ID;
            public Guid Version;
            public byte[] blob;
        }
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputResourceRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputResourceRMState(IContainer container)
            : base(container)
        {
            mInternalResourceCache = new Dictionary<Uri, GuidHolder>();
        }
        #endregion // Constructors

        #region OutputPrepare(ContentCompilerContext context)
        /// <summary>
        /// This method selects the appropriate node and creates a Body content.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void OutputPrepare(ResourceManagerContext context)
        {
            BinaryContent content = null;
            try
            {
                Uri resource = new Uri(context.Request.Settings.OutputColl[0].Output);

                GuidHolder holder;
                lock (syncLockCache)
                {
                    byte[] blob;
                    if (!mInternalResourceCache.ContainsKey(resource))
                    {
                        blob = RH.ResourceLoadFromUri(resource);
                        holder = new GuidHolder(Guid.NewGuid(), Guid.NewGuid(), blob);
                        mInternalResourceCache.Add(resource, holder);
                    }
                    else
                        holder = mInternalResourceCache[resource];
                }

                content = context.GetObjectPool<BinaryContent>().Get();
                content.Load();
                content.Data = holder.blob;
                //content.Load(holder.blob, 0, holder.blob.Length);
                content.ID = holder.ID;
                content.Version = holder.Version;

                string statusBody;
                context.Response.Body = PrepareBody(context, content, out statusBody);
                context.Response.Status = statusBody;
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

        #region OutputComplete(ResourceManagerContext context)
        /// <summary>
        /// This method does not do anything, but is reserved for future expansion.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void OutputComplete(ResourceManagerContext context)
        {
            //Do nothing
        }
        #endregion // OutputComplete(ResourceManagerContext context)
    }
}

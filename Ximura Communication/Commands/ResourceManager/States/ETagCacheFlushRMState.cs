#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ETagCacheFlushRMState : ResourceManagerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ETagCacheFlushRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ETagCacheFlushRMState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors


        public override bool RequestValidate(ResourceManagerContext context)
        {
            Guid? cid = context.Request.ETagFlushID;
            if (cid.HasValue && context.ContextSettings.ETagCacheFlush(cid.Value))
            {
                context.Response.Status = CH.HTTPCodes.OK_200;
            }
            else
                context.Response.Status = CH.HTTPCodes.NotFound_404;

            return false;
        }



    }
}

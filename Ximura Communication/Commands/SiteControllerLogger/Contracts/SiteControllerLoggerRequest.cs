#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class SiteControllerLoggerRequest : RQServer
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public SiteControllerLoggerRequest()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public SiteControllerLoggerRequest(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SiteControllerLoggerRequest(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the class for reuse in the pool
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Address = null;
            CachePruneMax = null;
            CachePrunePercent = null;
            Registries = null;
        }
        #endregion // Reset()

        #region Address
        /// <summary>
        /// This is the IP address that requires resolution.
        /// </summary>
        public IPAddress Address
        {
            get;
            set;
        }
        #endregion // Address

        #region CachePruneMax
        /// <summary>
        /// This is the number of records for the IP address cache table
        /// </summary>
        public int? CachePruneMax
        {
            get;
            set;
        }
        #endregion // CachePruneMax
        #region CachePrunePercent
        /// <summary>
        /// This is the cache prune percent.
        /// </summary>
        public int? CachePrunePercent
        {
            get;
            set;
        }
        #endregion // CachePrunePercent

        #region Registries
        /// <summary>
        /// This property contains the remote registries.
        /// </summary>
        public IEnumerable<KeyValuePair<string, Uri>> Registries { get; set; }
        #endregion // Registries
    }
}

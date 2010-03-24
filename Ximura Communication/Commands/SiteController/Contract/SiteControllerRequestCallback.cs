#region using
using System;
using System.Runtime.Serialization;
using System.IO;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the site manager request class and contains the data to process the request.
    /// </summary>
    public class SiteControllerRequestCallback : RQCallbackServer
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public SiteControllerRequestCallback()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public SiteControllerRequestCallback(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SiteControllerRequestCallback(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

    }
}

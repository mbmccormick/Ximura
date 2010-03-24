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
    public class SiteControllerResponseCallback : RSCallbackServer
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public SiteControllerResponseCallback()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public SiteControllerResponseCallback(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SiteControllerResponseCallback(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}

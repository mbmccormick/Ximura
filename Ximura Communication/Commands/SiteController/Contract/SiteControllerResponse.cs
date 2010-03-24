#region using
using System;
using System.Runtime.Serialization;
using System.IO;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class SiteControllerResponse: RSServer
    {
        #region Declarations
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public SiteControllerResponse()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public SiteControllerResponse(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SiteControllerResponse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Reset
        /// <summary>
        /// This override reset the specific data for the Site Manager Request.
        /// </summary>
        public override void Reset()
        {
            Message = null;
            SessionID = null;
            base.Reset();
        }
        #endregion // Reset

        #region Message
        /// <summary>
        /// This is the HTTP response.
        /// </summary>
        public Message Message
        {
            get;
            set;
        }
        #endregion // Response

        #region SessionID
        /// <summary>
        /// This is the session ID returned by the Site Controller.
        /// </summary>
        public Guid? SessionID
        {
            get;
            set;
        }
        #endregion // SessionID
    }
}

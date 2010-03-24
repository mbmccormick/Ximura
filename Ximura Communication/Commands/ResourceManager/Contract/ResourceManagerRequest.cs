#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ResourceManagerRequest : ContentCompilerRequest
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public ResourceManagerRequest()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public ResourceManagerRequest(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ResourceManagerRequest(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        /// <summary>
        /// This reset override clears the ETagFlushID
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            ETagFlushID = null;
        }

        /// <summary>
        /// This is the ID passed through an ETagFlush Request.
        /// </summary>
        public Guid? ETagFlushID { get; set; }

    }
}

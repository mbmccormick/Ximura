#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

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
    public class SiteControllerLoggerResponse : RSServer
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public SiteControllerLoggerResponse()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public SiteControllerLoggerResponse(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SiteControllerLoggerResponse(SerializationInfo info, StreamingContext context)
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
            CountryCode = null;
        }
        #endregion // Reset()

        #region CountryCode
        /// <summary>
        /// This is the country code that corresponds to the IPAddress in the request.
        /// </summary>
        public string CountryCode
        {
            get;
            set;
        }
        #endregion // CountryCode

    }
}

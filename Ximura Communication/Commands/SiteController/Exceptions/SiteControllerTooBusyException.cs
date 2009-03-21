#region using
using System;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The Site Manager Exception is thrown by base elements with the Process chain.
    /// </summary>
    public class SiteControllerTooBusyException : SiteControllerException
    {
        /// <summary>
        /// Initializes a new instance of the ProtocolException class.
        /// </summary>
        public SiteControllerTooBusyException() : base() { }
        /// <summary>
        /// Initializes a new instance of the ProtocolException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SiteControllerTooBusyException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the ProtocolException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception</param>
        public SiteControllerTooBusyException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected SiteControllerTooBusyException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #region ResponseCode
        /// <summary>
        /// This is the HTTP Response code.
        /// </summary>
        public override int ResponseCode { get { return 503; } }
        #endregion // ResponseCode
    }
}

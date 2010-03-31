#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The stylesheet class provides the functionality for the XSLT objects.
    /// </summary>
    [XimuraContentTypeID("D5AA1AFD-057D-46de-A0D1-06D633BB4584")]
    public class Stylesheet : StylesheetBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public Stylesheet() { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public Stylesheet(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        protected override string GetMimeType(byte[] buffer)
        {
            return "application/xslt+xml";
        }
    }
}

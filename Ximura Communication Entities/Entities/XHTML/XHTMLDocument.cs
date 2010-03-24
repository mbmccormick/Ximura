#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

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
    [XimuraDataContentSchemaReference("http://www.w3.org/1999/xhtml",
        "xmrres://XimuraCommEntities/Ximura.Communication.XHTMLDocument/Ximura.Communication.Resources.XHTMLStrict.xsd")]
    public class XHTMLDocumentFragment : DublinCore
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public XHTMLDocumentFragment() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public XHTMLDocumentFragment(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
    }
}

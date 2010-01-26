#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The data model template holds the Model Template that accepts the ControllerRequest to turn in to a Data Model.
    /// </summary>
    [XimuraContentTypeID("C1B9C360-55E0-4394-B9D5-C6E25F8009C1")]
    [XimuraDataContentSchemaReference("http://schema.ximura.org/controller/model/1.0",
        "xmrres://XimuraCommEntities/Ximura.Communication.Model/Ximura.Communication.Resources.Model.xsd")]
    public class ModelTemplate : StylesheetBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ModelTemplate() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ModelTemplate(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        protected override string GetMimeType(byte[] buffer)
        {
            return "application/x-atf+xml";
        }
    }
}

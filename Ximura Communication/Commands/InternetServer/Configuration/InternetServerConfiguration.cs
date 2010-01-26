#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using CDS = Ximura.Persistence.CDSHelper;
using Ximura.Persistence;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The InternetServerConfiguration class contains the base settings needed to configure the composite InternetServerCommand.
    /// </summary>
    public class InternetServerConfiguration : CommandConfiguration
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetServerConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public InternetServerConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
    }
}

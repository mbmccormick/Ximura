#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the base class for the Atom Publishing Protocol.
    /// </summary>
    public class AtomPPBase : AtomBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AtomPPBase() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AtomPPBase(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

    }
}

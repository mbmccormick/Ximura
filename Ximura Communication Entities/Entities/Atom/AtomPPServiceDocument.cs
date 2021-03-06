﻿#region using
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
    /// This class holds the Atom publishng protocol service document.
    /// </summary>
    public class AtomPPServiceDocument: AtomPPBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AtomPPServiceDocument() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AtomPPServiceDocument(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
    }
}

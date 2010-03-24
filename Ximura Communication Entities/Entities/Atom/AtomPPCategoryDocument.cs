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
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class AtomPPCategoryDocument : AtomPPBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AtomPPCategoryDocument() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AtomPPCategoryDocument(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
    }
}

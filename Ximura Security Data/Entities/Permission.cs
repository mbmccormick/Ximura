#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net.Mail;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Security
{
    /// <summary>
    /// This class contains the security information for a particular system permission.
    /// </summary>
    [XimuraContentTypeID("{6C5D43D2-B6FB-4e2f-B1EB-63A34C7630CE}")]
    [DataContract]
    [Serializable()]
    public class Permission : Content
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Permission()
            : base()
        {
        }
        #endregion // Constructor
    }
}

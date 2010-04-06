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
    [XimuraContentTypeID("{01B50216-DFE4-4acf-8663-F2220D93D049}")]
    [Serializable()]
    public class Permission : SecurityContentBase
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

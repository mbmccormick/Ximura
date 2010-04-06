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
    /// This class contains the security information for a particular user role.
    /// </summary>
    [XimuraContentTypeID("{3F1FF5D7-FF3A-4240-8AD2-3D263431EB8D}")]
    [Serializable()]
    public class Realm : SecurityContentBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Realm()
            : base()
        {
        }
        #endregion // Constructor

        #region Active
        /// <summary>
        /// This is a boolean value indicating whether the role is active.
        /// </summary>
        public bool Active { get; set; }
        #endregion // Active

    }
}

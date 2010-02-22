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
    /// This data holds the suthentication information for a particular user.
    /// </summary>
    [XimuraContentTypeID("{B5FFCC86-83FB-4813-91DF-CFFA0EBEF7E6}")]
    [DataContract]
    [Serializable()]
    public class UserAuth : Content
    {
        #region Declaration
        //[DataMember]
        //private Dictionary<string, byte[]> mAuthData;
        #endregion // Declaration
        #region Constructor
        /// <summary>
        /// This is the user authentication constructor.
        /// </summary>
        public UserAuth()
            : base()
        {
            //mAuthData = new Dictionary<string, byte[]>();
            AuthData = new List<UserAuthInformation>();
        }
        #endregion // Constructor

        #region UserName
        /// <summary>
        /// This is the username on the external system.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        #endregion // Assignee
        #region AuthType
        /// <summary>
        /// This is the auth type.
        /// </summary>
        [DataMember]
        public string AuthType { get; set; }
        #endregion // AuthType
        #region UserID
        /// <summary>
        /// This is the internal user IDContent.
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }
        #endregion // AssigneeID
        #region DateExpiry
        /// <summary>
        /// This is the expiry date of the auth.
        /// </summary>
        [DataMember]
        public DateTime? DateExpiry { get; set; }
        #endregion // DateExpiry
        #region Active
        /// <summary>
        /// This boolean value indicates whether the auth is active.
        /// </summary>
        [DataMember]
        public bool Active { get; set; }
        #endregion // Active

        #region AuthData
        /// <summary>
        /// This is the auth data collection for the User Auth entity.
        /// </summary>
        [DataMember]
        public List<UserAuthInformation> AuthData { get; protected set; }
        #endregion
    }

    /// <summary>
    /// This class holds the authorization data for a user auth collection.
    /// </summary>
    public class UserAuthInformation 
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public UserAuthInformation()
        {
            Key = null;
            Value = null;
        }
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        /// <param name="key">The auth key.</param>
        /// <param name="value">The auth binary array.</param>
        public UserAuthInformation(string key, byte[] value)
        {
            Key = key;
            Value = value;
        }
        #endregion // Constructor

        /// <summary>
        /// The authorization identifier.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The authorization byte value.
        /// </summary>
        public byte[] Value { get; set; }


    }
}
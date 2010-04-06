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
    [XimuraContentTypeID("{598D74FE-6C1C-4d1a-93E0-E7D599908BC5}")]
    [Serializable()]
    public class UserAuth : SecurityContentBase
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
            AuthData = new List<UserAuthData>();
        }
        #endregion // Constructor

        #region Type
        /// <summary>
        /// This is the auth type.
        /// </summary>
        public string Type { get; set; }
        #endregion // AuthType
        #region UserID
        /// <summary>
        /// This is the user ID.
        /// </summary>
        public Guid UserID { get; set; }
        #endregion // AssigneeID
        #region DateExpiry
        /// <summary>
        /// This is the expiry date.
        /// </summary>
        public DateTime? DateExpiry { get; set; }
        #endregion // DateExpiry
        #region Enabled
        /// <summary>
        /// This boolean value indicates whether the auth is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        #endregion // Active

        #region AuthData
        /// <summary>
        /// This collection holds the authorization data for the entity.
        /// </summary>
        public List<UserAuthData> AuthData { get; protected set; }
        #endregion
    }

    /// <summary>
    /// This class holds the authorization data for a user auth collection.
    /// </summary>
    public class UserAuthData
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public UserAuthData()
        {
            Key = null;
            Value = null;
        }
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        /// <param name="key">The auth key.</param>
        /// <param name="value">The auth binary array.</param>
        public UserAuthData(string key, byte[] value)
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
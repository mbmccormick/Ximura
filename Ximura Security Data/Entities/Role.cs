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
    [XimuraContentTypeID("{C5FC7234-BB81-43d2-9F40-D783E18AAB9C}")]
    [DataContract]
    [Serializable()]
    public class Role : Content
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Role()
            : base()
        {
            Permissions = new List<RolePermission>();
        }
        #endregion // Constructor

        #region Active
        /// <summary>
        /// This is a boolean value indicating whether the role is active.
        /// </summary>
        [DataMember]
        public bool Active { get; set; }
        #endregion // Active

        /// <summary>
        /// This collection holds the permission.
        /// </summary>
        [DataMember]
        public List<RolePermission> Permissions { get; protected set; }

    }

    /// <summary>
    /// This class holds the role permission information.
    /// </summary>
    [Serializable]
    [DataContract]
    public class RolePermission 
    {
        #region Constructor
        /// <summary>
        /// This is the default empty constructor.
        /// </summary>
        public RolePermission()
        {
            this.PermissionID = null;
            this.PermissionValue = null;
            this.Description = null;
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="PermissionID">The permission id.</param>
        /// <param name="PermissionValue">The specific bitmap permission.</param>
        /// <param name="Description">An optional description for the permissions.</param>
        public RolePermission(Guid PermissionID, long PermissionValue, string Description)
        {
            this.PermissionID = PermissionID;
            this.PermissionValue = PermissionValue;
            this.Description = Description;
        }
        #endregion

        #region PermissionID
        /// <summary>
        /// This is the permission IDContent.
        /// </summary>
        public Guid? PermissionID { get; set; }
        #endregion // PermissionID
        #region PermissionValue
        /// <summary>
        /// This is the permission value which is a combination of the specific permission bits.
        /// </summary>
        public long? PermissionValue { get; set; }
        #endregion // PermissionValue
        #region Name
        /// <summary>
        /// This is an optional description for the value.
        /// </summary>
        public string Description { get; set; }
        #endregion // Name

    }
}

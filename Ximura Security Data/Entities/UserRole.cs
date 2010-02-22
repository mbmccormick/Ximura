#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net.Mail;

using Ximura;
using Ximura.Data;
using System.Data.SqlTypes;
#endregion // using
namespace Ximura.Security
{
    /// <summary>
    /// The SearchResult class is the base class for returning lists of data
    /// based on a set of search parameters.
    /// </summary>
    [XimuraContentTypeID("{3A4BC2AD-D750-47f7-9795-342D8EC160D4}")]
    [DataContract]
    [Serializable()]
    public class UserRole : SecurityContentBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public UserRole()
            : base()
        {
            UserRoles = new List<UserRole>();
            Active = true;
        }
        #endregion // Constructor

        #region Active
        /// <summary>
        /// This property specifies whether the user role is active.
        /// </summary>
        [DataMember]
        public bool Active { get; set; }
        #endregion // Active

        #region Roles
        /// <summary>
        /// This is the a list of user roles.
        /// </summary>
        [DataMember]
        public List<UserRole> UserRoles
        {
            get;
            set;
        }
        #endregion
    }

    ///// <summary>
    ///// This class holds a reduced set of user role information.
    ///// </summary>
    //[Serializable]
    //[DataContract]
    //public class UserRole
    //{
    //    #region Constructors
    //    /// <summary>
    //    /// This is the empty constructor.
    //    /// </summary>
    //    public UserRole()
    //    {
    //        this.IDContent = null;
    //        this.ID = "";
    //        this.Type = null;
    //    }
    //    /// <summary>
    //    /// This is the cloneable constructor.
    //    /// </summary>
    //    /// <param name="IDContent">The role id</param>
    //    /// <param name="CombineType">The client name</param>
    //    public UserRole(Guid? ID, string CombineType)
    //    {
    //        this.IDContent = ID;
    //        this.ID = "";
    //        this.Type = CombineType;
    //    }
    //    /// <summary>
    //    /// This is the cloneable constructor.
    //    /// </summary>
    //    /// <param name="IDContent">The role id.</param>
    //    /// <param name="Name">The role name.</param>
    //    /// <param name="CombineType">The combine type.</param>
    //    public UserRole(Guid? ID, string Name, string CombineType)
    //    {
    //        this.IDContent = ID;
    //        this.ID = Name;
    //        this.Type = CombineType;
    //    }
    //    /// <summary>
    //    /// This is the sql persistence constructor.
    //    /// </summary>
    //    /// <param name="IDContent">The client identifier.</param>
    //    /// <param name="CombineType">The client name.</param>
    //    public UserRole(SqlGuid RoleID, SqlString RoleName, SqlString CombineType)
    //    {
    //        this.IDContent = RoleID.Value;
    //        this.ID = RoleName.Value;
    //        this.Type = CombineType.Value;
    //    }
    //    #endregion // Constructors

    //    #region IDContent
    //    /// <summary>
    //    /// The role combine type.
    //    /// </summary>
    //    [DataMember]
    //    public Guid? IDContent { get; set; }
    //    #endregion // CombineType

    //    #region ID
    //    /// <summary>
    //    /// The role combine type.
    //    /// </summary>
    //    [DataMember]
    //    public string ID { get; set; }
    //    #endregion // CombineType

    //    #region Type
    //    /// <summary>
    //    /// The role combine type.
    //    /// </summary>
    //    [DataMember]
    //    public string Type { get; set; }
    //    #endregion // CombineType




    //}
}
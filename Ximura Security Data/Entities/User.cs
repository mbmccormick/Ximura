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
    /// The SearchResult class is the base class for returning lists of data
    /// based on a set of search parameters.
    /// </summary>
    [XimuraContentTypeID("{17A05C32-8565-43e3-8CF0-A8BB996095E2}")]
    [DataContract]
    [Serializable]
    public class User : Content
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
        }
        #endregion // Constructor

        #region Name
        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public override string Name
        {
            get { return string.Format("{0}{1} {2}", (NameTitle == null || NameTitle.Trim() == "") ? "" : NameTitle + " ", NameGiven, NameFamily); }
            set { throw new NotImplementedException("Name is not implemented"); }
        }
        #endregion // Name

        #region JobTitle
        /// <summary>
        /// Gets or sets the job title.
        /// </summary>
        /// <value>The job title.</value>
        [DataMember]
        public string JobTitle { get; set; }
        #endregion

        #region NameTitle
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title, eg. Mr, Ms, Dr.</value>
        [DataMember]
        public string NameTitle { get; set; }
        #endregion // NameTitle
        #region NameGiven
        /// <summary>
        /// Gets or sets the given names.
        /// </summary>
        /// <value>The given names.</value>
        [DataMember]
        public string NameGiven { get; set; }
        #endregion // NameGiven
        #region NameFamily
        /// <summary>
        /// Gets or sets the family name.
        /// </summary>
        /// <value>The family name.</value>
        [DataMember]
        public string NameFamily { get; set; }
        #endregion // NameFamily

        #region EmailAddress
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [DataMember]
        public string EmailAddress { get; set; }
        #endregion // EmailAddress

        #region PhoneNumber
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        [DataMember]
        public string PhoneNumber { get; set; }
        #endregion // PhoneNumber
        #region MobileNumber
        /// <summary>
        /// Gets or sets the mobile number.
        /// </summary>
        /// <value>The mobile number.</value>
        [DataMember]
        public string MobileNumber { get; set; }
        #endregion // MobileNumber
    }
}

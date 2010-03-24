#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

using CH = Ximura.Common;
using System.Security.Principal;
using System.Configuration;
using System.Security.Permissions;
#endregion // using
namespace Ximura.Auth
{
    /// <summary>
    /// This membership provider uses a digest based storage approach to user passwords.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DigestMembershipProvider : MembershipProvider, IAuthenticationProviderDigest
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public DigestMembershipProvider()
        {

        }
        #endregion // Constructor


        public override void Initialize(string name, 
            System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }


        #region ApplicationName
        /// <summary>
        /// This is the application name and realm used for authentication.
        /// </summary>
        public override string ApplicationName
        {
            get;
            set;
        }
        #endregion // ApplicationName

        #region Realm
        /// <summary>
        /// This is the realm as reported to the client, and used in the hashing process.
        /// </summary>
        public virtual string Realm
        {
            get
            {
                return ApplicationName;
            }
        }
        #endregion // Realm



        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, 
            string email, string passwordQuestion, string passwordAnswer, 
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.ProviderError;
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return false;
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        #region Find users
        public override MembershipUserCollection FindUsersByEmail(
            string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            return new MembershipUserCollection();
        }

        public override MembershipUserCollection FindUsersByName(
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            return new MembershipUserCollection();
        }

        public override MembershipUserCollection GetAllUsers(
            int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            return new MembershipUserCollection();
        }
        #endregion // Find users


        public override int GetNumberOfUsersOnline()
        {
            return 0;
        }

        #region GetPassword(string username, string answer)
        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Password retrieval is not uspported.");
        }
        #endregion // GetPassword(string username, string answer)


        #region Find user
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }
        #endregion // Find user


        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        #region PasswordFormat
        /// <summary>
        /// This method specifies that the password is hashed.
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }
        #endregion // PasswordFormat


        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}

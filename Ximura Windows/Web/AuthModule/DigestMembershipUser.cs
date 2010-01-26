#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
#endregion // using
namespace Ximura.Auth
{
    /// <summary>
    /// This is the extended membership class used by the DigestMembershipProvider
    /// </summary>
    public class DigestMembershipUser : MembershipUser, IMembershipUserDigest
    {

    }
}

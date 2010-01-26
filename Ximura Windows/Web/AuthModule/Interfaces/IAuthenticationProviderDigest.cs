#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion // using
namespace Ximura.Auth
{
    /// <summary>
    /// This interface specifies additional properties to authenticate using digest authentication.
    /// </summary>
    public interface IAuthenticationProviderDigest
    {
        /// <summary>
        /// This is the realm used for authentication.
        /// </summary>
        string Realm { get; }
    }
}

#region using
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Net;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;


using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This structure holds the realm authentication settings.
    /// </summary>
    public class RealmAuthentication
    {
        #region Public properties
        /// <summary>
        /// Identifies whether the user is authenticated.
        /// </summary>
        public bool Authenticated;
        /// <summary>
        /// The realm.
        /// </summary>
        public string Realm;
        /// <summary>
        /// The username for the realm.
        /// </summary>
        public string Username;
        /// <summary>
        /// This value determines whether the auth settings should persist.
        /// </summary>
        public bool CookiePersist = false;
        #endregion // Public properties

        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="Realm">The realm.</param>
        /// <param name="Username">The username for the realm.</param>
        /// <param name="Authenticated">Identifies whether the user is authenticated.</param>
        public RealmAuthentication(string Realm, string Username, bool Authenticated, bool CookiePersist)
        {
            this.Authenticated = Authenticated;
            this.Realm = Realm;
            this.Username = Username;
            this.CookiePersist = CookiePersist;
        }
        #endregion // Constructor
    }
}

#region using
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.Serialization;
using System.IO;
using System.ServiceProcess;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using Ximura.Windows;
#endregion // using
namespace Ximura.Samples.SocialNetworking
{
    /// <summary>
    /// This is the installer class that install the service.
    /// </summary>
    [RunInstaller(true)]
    [ServiceInstaller(
        typeof(SocialNetworkingService),
        ServiceStartMode.Automatic,
        "XSSocNet",
        "Ximura Samples Social Networking",
        "This is the Ximura samples social networking service.",
        ServiceAccount.NetworkService,null,null
        )]
    public class ProjectInstaller : ServiceAppInstaller
    {
        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ProjectInstaller()
        {
        }
        #endregion // Constructor
    }
}

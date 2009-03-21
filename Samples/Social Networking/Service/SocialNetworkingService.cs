#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.IO;

using Ximura;
using Ximura.Windows;
#endregion // using
namespace Ximura.Samples.SocialNetworking
{
    /// <summary>
    /// This is the Budubu analysis service.
    /// </summary>
    [XimuraInstallerEventLogger("XSSocNet", "Ximura Sample Social Networking")]
    [XimuraInstallerAppServerConfig(typeof(SocialNetworkingAppServer))]
    public class SocialNetworkingService : Ximura.Windows.AppServerService
    {
        #region Main entry point
        /// <summary>
        /// The main process entry point.
        /// </summary>
        static void Main()
        {
            System.ServiceProcess.ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new SocialNetworkingService() };

            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }
        #endregion
    }
}

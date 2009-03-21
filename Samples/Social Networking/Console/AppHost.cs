#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Ximura;
using Ximura.Windows;
#endregion // using
namespace Ximura.Samples.SocialNetworking
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [AppServer(1, "Ximura Samples SocialNetworking Site", typeof(SocialNetworkingAppServer))]
    public class AppHost : Ximura.Windows.ApplicationHost
    {
        [STAThread]
        public static void Main()
        {
            AppHost app = new AppHost();
            app.InitializeComponent();
            app.Run();
        }
    }
}
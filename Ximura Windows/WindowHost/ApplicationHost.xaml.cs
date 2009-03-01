#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Ximura;
using Ximura.Helper;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class ApplicationHost : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            HostAppServerAttribute[] attrs = AH.GetAttributes<HostAppServerAttribute>(GetType());

            WindowAppServers primaryWin = new Ximura.Windows.WindowAppServers(attrs);


            
            primaryWin.Show();
        }
    }
}

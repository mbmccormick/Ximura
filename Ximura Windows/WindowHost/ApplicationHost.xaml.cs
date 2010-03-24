#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Ximura;

using AH = Ximura.AttributeHelper;
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
            AppServerAttribute[] attrs = AH.GetAttributes<AppServerAttribute>(GetType());

            WindowAppServers primaryWin = new Ximura.Windows.WindowAppServers(attrs);

            primaryWin.Show();
        }
    }
}

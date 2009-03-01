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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// The windows helper class is used to provide additional functions for a windows environment.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// This static method provides a shortcut method to updating a UI control on the current thread, 
        /// or invoking the update on another thread. 
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action to update the UI element.</param>
        public static void InvokeOrCallDirect(this Control control, Action action)
        {
            if (control.Dispatcher.CheckAccess())
                action();
            else
                control.Dispatcher.BeginInvoke(action);
        }
    }
}

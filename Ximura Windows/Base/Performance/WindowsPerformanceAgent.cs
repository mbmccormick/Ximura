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

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// The WindowsPerformanceAgent class connects the Ximura performance architecture with the windows performance architecture by 
    /// means of the PerformanceCounter class in the System.Diagnostics namespace.
    /// </summary>
    public class WindowsPerformanceAgent : PerformanceAgentBase<WindowsPerformanceAgentConfiguration>
    {
        #region Constructor
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public WindowsPerformanceAgent():this(null){}
		/// <summary>
		/// This is the base constructor for a Ximura command
		/// </summary>
		/// <param name="container">The container to be added to</param>
        public WindowsPerformanceAgent(System.ComponentModel.IContainer container)
            : base(container) 
        {
        }
        #endregion


    }
}

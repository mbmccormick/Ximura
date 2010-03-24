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
using Ximura.Framework;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This attribute is used to specify the appservers that can be started by the host application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AppServerAttribute : System.Attribute
    {
        /// <summary>
        /// This constructor sets the basic parameters for the application.
        /// </summary>
        /// <param name="Priority">The server priority.</param>
        /// <param name="Name">The server friendly name.</param>
        /// <param name="ServerType">The server type.</param>
        public AppServerAttribute(int Priority, string Name, Type ServerType)
            : this(Priority, Name, ServerType, true)
        {
        }
        /// <summary>
        /// This constructor sets the basic parameters for the application.
        /// </summary>
        /// <param name="Priority">The server priority.</param>
        /// <param name="Name">The server friendly name.</param>
        /// <param name="ServerType">The server type.</param>
        /// <param name="Enabled">This property determines whether the appserver is started by default.</param>
        public AppServerAttribute(int Priority, string Name, Type ServerType, bool Enabled)
        {
            this.Priority = Priority;
            this.Name = Name;
            this.ServerType = ServerType;
            this.Enabled = Enabled;
        }
        /// <summary>
        /// The server priority.
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// The server friendly name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The server type.
        /// </summary>
        public Type ServerType { get; private set; }
        /// <summary>
        /// This property determines whether the appserver is started by default.
        /// </summary>
        public bool Enabled { get; private set; }
    }
}

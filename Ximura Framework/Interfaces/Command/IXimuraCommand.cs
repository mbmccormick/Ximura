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

using Ximura;


#endregion // using
namespace Ximura
{
    /// <summary>
    /// IXimuraCommand is the base interface for the command object.
    /// </summary>
    public interface IXimuraCommand
    {
        /// <summary>
        /// The command unique identifier.
        /// </summary>
        Guid CommandID { get;}
        /// <summary>
        /// The command name. This is used in to the config file to retrieve the
        /// settings.
        /// </summary>
        string CommandName { get;set;}
        /// <summary>
        /// The command friendly description
        /// </summary>
        string CommandDescription { get;set;}
    }

    /// <summary>
    /// This interface is implemented by commands that support notification.
    /// </summary>
    public interface IXimuraCommandNotification
    {
        /// <summary>
        /// This boolean property specifies that the object supports system notification.
        /// </summary>
        bool SupportsNotifications { get; }
        /// <summary>
        /// This method is called when there is a system notification.
        /// </summary>
        /// <param name="notificationType">The notification object type.</param>
        /// <param name="notification">The notification object.</param>
        void Notify(Type notificationType, object notification);
    }
}

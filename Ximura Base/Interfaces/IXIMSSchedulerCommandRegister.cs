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
using System.Threading;
using System.Collections;
using System.ComponentModel;

using XIMS;
using XIMS.Helper;
using XIMS.Applications;
#endregion // using
namespace XIMS.Applications.Command
{
	/// <summary>
    /// The IXIMSSchedulerCommandRegister interface provides an interaction between the scheduler and the 
    /// application commands.
	/// </summary>
	public interface IXIMSSchedulerCommandRegister
	{
        /// <summary>
        /// This method is used by a command to register with the scheduler, a valid Guid identifies that the 
        /// scheduler is able to process the schedules.
        /// </summary>
        /// <param name="CommandID">Returns true if the command was registered successfully.</param>
        void RegisterCommand(Guid CommandID);
        /// <summary>
        /// This method is used to register a subcommand. The scheduler will then call the command based
        /// on the schedule set in entity.
        /// </summary>
        /// <param name="registerID">The register ID.</param>
        /// <param name="subCommandID">The sub command id, this can be null if it is the default command.</param>
        /// <param name="description">The sub command description.</param>
        /// <param name="subCommand">The subcommand object that will be sent to the command as a subcommand parameter.</param>
        /// <param name="priority">The priority set for the request.</param>
        /// <returns>Returns true if the command is registered correctly.</returns>
        bool RegisterSubCommand(Guid registerID, string subCommandID,
            string name, string description, object subCommand, JobPriority priority);
        /// <summary>
        /// This method is used to register a subcommand. The scheduler will then call the command based
        /// on the schedule set in entity.
        /// </summary>
        /// <param name="registerID">The register ID.</param>
        /// <param name="subCommandID">The sub command id, this can be null if it is the default command.</param>
        /// <param name="description">The sub command description.</param>
        /// <param name="subCommand">The subcommand object that will be sent to the command as a subcommand parameter.</param>
        /// <param name="priority">The priority set for the request.</param>
        /// <param name="active">This method specifies whether the subcommand is active. This property can be used to remove a subcommand
        /// that has been depreciated in a new version of a command. If this is set to false
        /// the Scheduler will remove all references to this command if they exist in the schedule.</param>
        /// <returns>Returns true if the command is registered correctly.</returns>
        bool RegisterSubCommand(Guid registerID, string subCommandID,
            string name, string description, object subCommand, JobPriority priority, bool active);
        /// <summary>
        /// This method is used to specifically disable/unregister a subcommand that has already been registered.
        /// </summary>
        /// <param name="registerID">The registerID previously passed back from the scheduler.</param>
        /// <param name="subCommandID">The subcommand id.</param>
        /// <returns>Returns true if the command was successfully unregistered.</returns>
        bool UnregisterSubCommand(Guid registerID, string subCommandID);
        /// <summary>
        /// This method is used by the command to signal that it is completed/stopped and no longer wishes
        /// to receive shedule requests.
        /// </summary>
        /// <param name="schedulerID"></param>
        void UnregisterCommand(Guid schedulerID);
	}
}

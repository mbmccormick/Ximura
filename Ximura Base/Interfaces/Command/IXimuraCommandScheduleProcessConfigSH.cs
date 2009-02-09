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
using System.Collections;
using System.ComponentModel;
using System.Configuration;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the default interface for a scheduler config section handler
    /// </summary>
    public interface IXimuraCommandScheduleProcessConfigSH : IConfigurationSectionHandler
    {
        /// <summary>
        /// Get Next Schedules within interval(in minutes)
        /// </summary>
        /// <param name="interval">interval in minutes</param>
        /// <returns>return all upcoming schedules in time/schedule pairs</returns>
        ArrayList GetNextSchedules(double interval);
    }
}

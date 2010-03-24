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
using System.ComponentModel.Design;

using Ximura;
using Ximura.Framework;
using Ximura.Helper;
#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// This interface is registered as a service and is used by application 
	/// commands to communicate with the Application Dispatcher
	/// </summary>
	public interface IXimuraCommandBridge
	{
		/// <summary>
		/// This method is used to register a command in the command collection.
		/// </summary>
		/// <param name="command">The command to register.</param>
		/// <returns>Returns true if the command is registered successfully.</returns>
		bool Register(IXimuraCommandRQ command);
		/// <summary>
		/// This method is used to unregister a command.
		/// </summary>
		/// <param name="command">The commands to remove.</param>
		/// <returns>Returns true if the command is successfully removed
		/// from the collection. Returns false if the command was not a member
		/// of the collection.</returns>
		bool Unregister(IXimuraCommandRQ command);

        /// <summary>
        /// This is the envelope helper.
        /// </summary>
        IXimuraEnvelopeHelper EnvelopeHelper { get; }
	}
}

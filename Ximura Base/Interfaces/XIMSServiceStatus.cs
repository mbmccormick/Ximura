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

using XIMS;
#endregion // using
namespace XIMS.Helper
{
	/// <summary>
	/// The CatalogProcessStatus shows the current status of the Catalog Process.
	/// </summary>
	public enum XIMSServiceStatus
	{
		/// <summary>
		/// The status is undefined.
		/// </summary>
		Undefined=0x0,
		/// <summary>
		/// The process has not started.
		/// </summary>
		NotStarted=0x1,
		/// <summary>
		/// The process is starting up.
		/// </summary>
		Starting=0x2,
		/// <summary>
		/// The process is processing.
		/// </summary>
		Started=0x4,
		/// <summary>
		/// The process is pausing.
		/// </summary>
		Pausing=0x8,
		/// <summary>
		/// The process is paused.
		/// </summary>
		Paused=0x10,
		/// <summary>
		/// The process is resuming.
		/// </summary>
		Resuming=0x20,
		/// <summary>
		/// The process is stopping.
		/// </summary>
		Stopping=0x40,
		/// <summary>
		/// The process has stopped.
		/// </summary>
		Stopped=0x80,
		/// <summary>
		/// The process has completed.
		/// </summary>
		Completed=0x100,
        /// <summary>
        /// The process has resumed.
        /// </summary>
        Resumed =0x200
	}

}

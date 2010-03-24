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
using System.Timers;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura
{
	/// <summary>
	/// IXimuraFSMConfigSH is used to provide specific settings for the context pool.
	/// </summary>
	public interface IXimuraFSMConfigSH: IXimuraConfigSH
	{
		/// <summary>
		/// This is the context pool minimum value. 
		/// </summary>
		int PoolMin{get;}
		/// <summary>
		/// This is the context pool maximum value. 
		/// </summary>
		int PoolMax{get;}
		/// <summary>
		/// This is the context pool preferred value. 
		/// </summary>
		int PoolPrefer{get;}
		/// <summary>
		/// This is the timeout value for the context. Requests which exceed this value will be cancelled.
		/// </summary>
		int ContextTimeOutInMs{get;}
	}
}

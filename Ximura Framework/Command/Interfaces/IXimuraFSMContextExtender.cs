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
using Ximura.Helper;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura
{
	/// <summary>
	/// IXimuraFSMContextExtender generic interface.
	/// </summary>
	public interface IXimuraFSMContextExtender<CNTX, ST, SET, CONF, PERF>
        where CNTX : class, IXimuraFSMContext, new()
        where ST : class,IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
		/// <summary>
		/// This boolean value indicates whether there are contexts available
		/// in the pool for the default context type.
		/// </summary>
		bool ContextAvailable();
		/// <summary>
		/// This property returns the number of contexts currently active for the default type.
		/// </summary>
		int ContextsCurrent();
		/// <summary>
		/// This method returns a new context of the default type.
		/// </summary>
		/// <returns>A Context object or null if there are no 
		/// objects available.</returns>
        CNTX ContextGet();
		/// <summary>
		/// This method returns a context back to the pool.
		/// </summary>
		/// <param name="conn">The context to return.</param>
        void ContextReturn(CNTX conn);
	}
}

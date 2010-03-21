﻿#region Copyright
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
	/// <summary>
	/// IXimuraAppSite is an interface that inherits from ISite and provides 
	/// specific component model method for the Ximura Application framework.
	/// </summary>
	public interface IXimuraAppSite : ISite
	{
		/// <summary>
		/// An additional property which is the parent of the component
		/// </summary>
		object parent{get;}
		/// <summary>
		/// The service container.
		/// </summary>
		IServiceContainer ServiceContainer{get;}
	}
}

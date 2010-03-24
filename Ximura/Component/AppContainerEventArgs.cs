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
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

using Ximura;
using Ximura.Helper;
using Ximura.Framework;
#endregion
namespace Ximura
{
	/// <summary>
	/// This object contains the argument passed when a component id added or removed.
	/// </summary>
	public class XimuraAppContainerEventArgs : CancelEventArgs
	{
		#region Declarations
		private IComponent mComponent;
		private string mName;
		#endregion
		#region Constructor
		/// <summary>
		/// This is the event argument constructor.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="name"></param>
		public XimuraAppContainerEventArgs(IComponent component, string name)
		{
			this.mComponent=component;
			this.mName=name;
		}

		#endregion
		#region Properties
		/// <summary>
		/// This is the component that is added or removed from the collection.
		/// </summary>
		public IComponent Component
		{
			get
			{
				return mComponent;
			}
		}
		/// <summary>
		/// This is the component name. This is only used when a component
		/// is added to the collection. If the name is not set this value 
		/// is null.
		/// </summary>
		public string Name
		{
			get
			{
				return mName;
			}
		}
		#endregion
	}
}
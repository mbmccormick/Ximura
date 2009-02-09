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
using System;

using Ximura.Server;

namespace Ximura.Data
{
	/// <summary>
	/// This delegate is used to signal a data change
	/// </summary>
	public delegate void DataContentDataChange(object sender, DataChangeEventArgs args);
	
	public delegate void ContentEvent(object sender, ContentEventArgs args);

	/// <summary>
	/// DataChangedEventArgs is used to transmit data change details..
	/// </summary>
	public class DataChangeEventArgs : ContentEventArgs
	{
		#region Declarations

		#endregion

		#region Constructors
		public DataChangeEventArgs()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion
	}


	public class DataContentEventArgs: ContentEventArgs
	{
        public IXimuraDataEntityServer ParentDataServer = null;


	}

	public class ContentEventArgs : EventArgs
	{
        


	}
}

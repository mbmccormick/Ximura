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
using System.Data;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Data
{
	/// <summary>
	/// IXimuraDataEntityServer is used by DataContent Entities to load
	/// their datasets from a parent container.
	/// </summary>
	public interface IXimuraDataEntityServer
	{
		/// <summary>
		/// This method allows satelite DataContent to retrieve the 
		/// parent dataset.
		/// </summary>
		/// <param name="dataContentType">The content type.</param>
		/// <returns>The parent dataset or null if it does not exist.</returns>
		DataSet getParentDataSet(Type dataContentType);
		/// <summary>
		/// This method will link the DataContent with it's parent 
		/// content in the model folder.
		/// </summary>
		/// <param name="item">The DataContent Entity to link.</param>
		DataSet getParentDataSet(DataContent item);
	}
}

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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
	/// <summary>
	/// This attribute is used to set the default command configurations setting.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
	public sealed class XimuraAppModuleConfigAttribute : System.Attribute
	{
		#region Declarations
		private string mResource, mConfigHelper;

		#endregion // Declarations
		#region Constructor
		/// <summary>
		/// This attribute is used to specify the default XML for the parent configuration settings.
		/// </summary>
		/// <param name="ResourceFile">The name of the resource file that contains the base XML fragment.</param>
		/// <param name="ConfigHelperType">This is the .NET type name of the command config section handler.</param>
		public XimuraAppModuleConfigAttribute(string ResourceFile,string ConfigHelperType)
		{
			mResource=ResourceFile;
			mConfigHelper=ConfigHelperType;
		}
		#endregion // Constructor

		#region Property Fields
		/// <summary>
		/// The name of the resource file that contains the base XML fragment.
		/// </summary>
		public string ResourceFile
		{
			get{return mResource;}
		}

		/// <summary>
		/// The >NET type name of the command config section handler.
		/// </summary>
		public string ConfigHelperType
		{
			get{return mConfigHelper;}
		}

		#endregion
	}
}

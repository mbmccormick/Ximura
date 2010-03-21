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

using XIMS;
using XIMS.Helper;
using CH=XIMS.Helper.Common;
#endregion // using
namespace XIMS.Applications
{
	/// <summary>
	/// This attribute is used to define command properties.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
	public sealed class XIMSAppModuleAttribute : System.Attribute
	{
		#region Private Field Declaration

		private string m_name, m_description;
		private Guid m_cmdID = Guid.Empty;

		#endregion
		
		#region Class Constructors
		/// <summary>
		/// The XIMSAppCommandIDAttribute is used to uniquely identify the XIMS command.
		/// </summary>
		/// <param name="ID">A string representation of a GUID in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
		public XIMSAppModuleAttribute(string ID):this(ID,"",""){}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ID">A string representation of a GUID in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
		/// <param name="name">The friendly name of the command. 
		/// This will be used in the config file when parsing command set up paramters.</param>
		public XIMSAppModuleAttribute(string ID,string name):this(ID,name,""){}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ID">A string representation of a GUID in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
		/// <param name="name">The friendly name of the command. 
		/// This will be used in the config file when parsing command set up paramters.</param>
		/// <param name="description">A human readable description explaining what the command does.</param>
		public XIMSAppModuleAttribute(string ID,string name,string description)
		{
			m_cmdID= new Guid(ID);
			m_name= name;
			m_description=description;
		}
		#endregion

		#region Property Fields

		/// <summary>
		/// The Guid of the module
		/// </summary>
		public Guid ID
		{
			get{return m_cmdID;}
		}

		/// <summary>
		/// The name of the module. This is used to retrieve app settings.
		/// </summary>
		public string Name
		{
			get{return m_name;}
		}

		/// <summary>
		/// The friendly description of the module.
		/// </summary>
		public string Description
		{
			get{return m_description;}
		}

		#endregion
	}
}
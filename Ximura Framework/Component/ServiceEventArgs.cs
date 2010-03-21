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
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;

using XIMS;
using XIMS.Data;
using XIMS.Helper;
using CH=XIMS.Helper.Common;

using XIMS.Applications;
using XIMS.Applications.Command;
#endregion // using
namespace XIMS.Helper
{
	/// <summary>
	/// The ServiceEventArgs class is used by the component to pass notification 
	/// of the service status
	/// </summary>
	public class ServiceEventArgs : EventArgs
	{
		private XIMSServiceStatus m_status;

		/// <summary>
		/// The default constructor. The service is defined as Undefined
		/// </summary>
		public ServiceEventArgs():this(XIMSServiceStatus.Undefined)
		{}

		/// <summary>
		/// The main constructor.
		/// </summary>
		/// <param name="status">The status of the service.</param>
		public ServiceEventArgs(XIMSServiceStatus status)
		{
			m_status=status;
		}

		/// <summary>
		/// The status of the service
		/// </summary>
		public XIMSServiceStatus Status
		{
			get{return m_status;}
		}
	}
}

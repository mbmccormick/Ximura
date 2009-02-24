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
using System.ComponentModel;
using System.ComponentModel.Design;

using XIMS;
using XIMS.Applications;
#endregion // using
namespace XIMS.Helper
{
	/// <summary>
	/// Summary description for AppHelper.
	/// </summary>
	public class AppHelper
	{
		/// <summary>
		/// This method can start, stop, resume or pause a group of components.
		/// </summary>
		/// <param name="action">The action required</param>
		/// <param name="components">The components to which the action should be provided</param>
		public static void ComponentsStatusChange(XIMSServiceStatusAction action, 
			ICollection components)
		{
			if (components == null)
				return;

			//AppDomain hello = System.AppDomain.CreateDomain("hello");

			foreach(object objService in components)
			{
				IXIMSService service = objService as IXIMSService;
				if (service != null)
					try
					{
						IXIMSAppServer appServer = objService as IXIMSAppServer;
					
						switch (action)
						{
							case XIMSServiceStatusAction.Start:
								service.Start();
								break;
							case XIMSServiceStatusAction.Stop:
								service.Stop();
								break;
							case XIMSServiceStatusAction.Pause:
								service.Pause();
								break;
							case XIMSServiceStatusAction.Continue:
								service.Continue();
								break;
						}
					}
					catch (Exception ex)
					{
						throw ex;
					}
			}
		}
	}
}
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
using Ximura.Server;
#endregion
namespace Ximura
{
	/// <summary>
	/// AppContainer is the base container for the Ximura Application framework. 
	/// The container is used to share services between the components within an application.
	/// </summary>
	public class XimuraAppContainer : System.ComponentModel.Container
	{
		#region Declarations
		private IServiceContainer m_ServiceProvider = null;
		private object m_parent = null;
		#endregion
		#region Constructor
		/// <summary>
		/// This is the default container constructor
		/// </summary>
		public XimuraAppContainer():this(null,null){}
		/// <summary>
		/// This is container constructor with a service provider
		/// </summary>
		/// <param name="theProvider"></param>
		public XimuraAppContainer(IServiceContainer theProvider):this(theProvider, null){}
		/// <summary>
		/// This is the expanded constructor including the parent object for the 
		/// expanded ElementSite item.
		/// </summary>
		/// <param name="theProvider"></param>
		/// <param name="parent"></param>
		public XimuraAppContainer(IServiceContainer theProvider,object parent):base()
		{
			m_ServiceProvider = theProvider;
			m_parent=parent;
		}
		#endregion	
		#region Events
		/// <summary>
		/// This is the event delegate used to monitor component addition/deletion
		/// for the XimuraAppContainer object.
		/// </summary>
		public delegate void AppContainerEventHandler(object sender, XimuraAppContainerEventArgs args);
		/// <summary>
		/// This event will be fired when a component is added to the collection.
		/// </summary>
		public event CancelEventHandler ComponentAdding;
		/// <summary>
		/// This event will be fired when a component is added.
		/// </summary>
		public event EventHandler ComponentAdded;
		/// <summary>
		/// This event will be fired when a component is removed from the
		/// collection
		/// </summary>
		public event CancelEventHandler ComponentRemoving;
		/// <summary>
		/// This event will be fired when a component is removed.
		/// </summary>
		public event EventHandler ComponentRemoved;
		#endregion
		#region ISite overloads
		/// <summary>
		/// This method creates a new ElementSite compoent
		/// </summary>
		/// <param name="component">The component</param>
		/// <param name="name">The name</param>
		/// <returns>The ElementSite object.</returns>
		protected override ISite CreateSite(IComponent component, string name)
		{
			return new ElementSite(this,component,m_ServiceProvider,name,false,m_parent);
		}
		#endregion
		#region IServiceProvider
		/// <summary>
		/// This is the service provider.
		/// </summary>
		public IServiceProvider ServiceProvider
		{
			get
			{
				return m_ServiceProvider as IServiceProvider;
			}
			//			set
			//			{
			//				m_ServiceProvider=value;
			//			}
		}
		/// <summary>
		/// This is the service container.
		/// </summary>
		public IServiceContainer ServiceContainer
		{
			get
			{
				return m_ServiceProvider;
			}
			set
			{
				m_ServiceProvider=value;
			}
		}
		#endregion

		#region Overriden Add method
		/// <summary>
		/// This overriden method adds event support to the Add event
		/// </summary>
		/// <param name="component">The component to add.</param>
		/// <param name="name">The name of the component 
		/// or null if this is not set</param>
		public override void Add(IComponent component, string name)
		{
			if (ComponentAdding!= null)
			{
				XimuraAppContainerEventArgs CancelArgs = 
					new XimuraAppContainerEventArgs(component,name);
				ComponentAdding(this,CancelArgs);
				if (CancelArgs.Cancel) return;
			}

			base.Add (component, name);
			if (ComponentAdded!= null)
				ComponentAdded(this,new XimuraAppContainerEventArgs(component,name));	
		}
		#endregion
		#region Overriden Remove method
		/// <summary>
		/// This overriden method adds event support to the Remove method.
		/// </summary>
		/// <param name="component">The component to remove.</param>
		public override void Remove(IComponent component)
		{
			if (ComponentRemoving!= null)
			{
				XimuraAppContainerEventArgs CancelArgs = 
					new XimuraAppContainerEventArgs(component, null);
				ComponentRemoving(this,CancelArgs);
				if (CancelArgs.Cancel) return;
			}
			
			base.Remove (component);
			if (ComponentRemoved!= null)
				ComponentRemoved(this,new XimuraAppContainerEventArgs(component,null));
		}
		#endregion
	}
}
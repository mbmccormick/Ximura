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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;

using Ximura;

#endregion // using
namespace Ximura
{
	/// <summary>
	/// ElementSite is used by the Ximura.Data component model to link elements
	/// within an aggreagate Content/Element object.
	/// </summary>
	public class ElementSite : IXimuraAppSite
	{
		#region Declarations

		/// <summary>
		/// This is the component.
		/// </summary>
		protected IComponent m_Component;
		/// <summary>
		/// This is the container
		/// </summary>
		protected IContainer m_Container;
		/// <summary>
		/// This is the internal variable that determines whether the 
		/// system is in design mode.
		/// </summary>
		protected bool m_bDesignMode;
		/// <summary>
		/// This is the site instance name
		/// </summary>
		protected string m_Name;
		/// <summary>
		/// This is the document service container.
		/// </summary>
		protected IServiceContainer m_theServices = null;

		/// <summary>
		/// This is the parent object that created collection
		/// </summary>
		protected object m_parent = null;

		#endregion
		#region Constructor / Destructors / IDisposable
		/// <summary>
		/// This is a default constructor for the ElementSite
		/// </summary>
		/// <param name="actvCntr">The container.</param>
		/// <param name="prntCmpnt">The component.</param>
		public ElementSite(IContainer actvCntr, IComponent prntCmpnt):
			this(actvCntr, prntCmpnt,(IServiceContainer)null,(string)null,false,null){}
		/// <summary>
		/// This constructor allows a service container to be set for the site
		/// </summary>
		/// <param name="actvCntr">The container.</param>
		/// <param name="prntCmpnt">The component.</param>
		/// <param name="theServices">The Service Container</param>
		public ElementSite(IContainer actvCntr, IComponent prntCmpnt,
			IServiceContainer theServices):
			this(actvCntr, prntCmpnt,theServices,(string)null,false,null){}
		/// <summary>
		/// This constructor
		/// </summary>
		/// <param name="actvCntr">The container.</param>
		/// <param name="prntCmpnt">The component.</param>
		/// <param name="theServices">The Service Container</param>
		/// <param name="Name">The name of the site</param>
		/// <param name="bDesignMode">
		/// A boolean value which specifies whether the 
		/// system is in design mode
		/// </param>
		public ElementSite(IContainer actvCntr, IComponent prntCmpnt,
			IServiceContainer theServices,string Name,bool bDesignMode):
			this(actvCntr, prntCmpnt,theServices,Name,bDesignMode,null){}
		/// <summary>
		/// This constructor
		/// </summary>
		/// <param name="actvCntr">The container.</param>
		/// <param name="prntCmpnt">The component.</param>
		/// <param name="theServices">The Service Container</param>
		/// <param name="Name">The name of the site</param>
		/// <param name="bDesignMode"></param>
		/// <param name="parent">The parent object that created this component</param>
		public ElementSite(IContainer actvCntr, IComponent prntCmpnt,
			IServiceContainer theServices,string Name,bool bDesignMode,
			object parent)
		{
			m_Component = prntCmpnt;
			m_Container = actvCntr;
			m_bDesignMode = bDesignMode;
			m_theServices = theServices;
			m_Name = Name;
			m_parent = parent;
		}
		#endregion
		#region ISite Members
		/// <summary>
		/// The component
		/// </summary>
		public virtual IComponent Component
		{
			get
			{
				return m_Component;
			}
		}
		/// <summary>
		/// The container
		/// </summary>
		public virtual IContainer Container
		{
			get
			{
				return m_Container;
			}
		}

		/// <summary>
		/// The design mode of the container
		/// </summary>
		public virtual bool DesignMode
		{
			get
			{
				return m_bDesignMode;
			}
		}

		/// <summary>
		/// The name of the container
		/// </summary>
		public virtual string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}


		#endregion
		#region IServiceProvider Members

		/// <summary>
		/// This method returns an object that represents the service specified
		/// in the serviceType parameter.
		/// </summary>
		/// <param name="serviceType">The service type to retrieve.</param>
		/// <returns>An object that represents the service or null is the service
		/// cannot be found.</returns>
		public virtual object GetService(Type serviceType)
		{
            if (m_theServices == null)
            {
                

                return null;
            }
			return m_theServices.GetService(serviceType);
		}

		#endregion
		#region IXimuraAppSite implementation
		/// <summary>
		/// An additional property which is the parent of the component
		/// </summary>
		public object parent
		{
			get{return m_parent;}
		}
	
		/// <summary>
		/// The service container.
		/// </summary>
		public IServiceContainer ServiceContainer
		{
			get{return m_theServices;}
		}
		#endregion
	}
}

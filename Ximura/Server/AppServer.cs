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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;

using Ximura.Performance;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Server
{
    /// <summary>
    /// The AppServerBase class is the class that all server applications derive from.
    /// </summary>
    public class AppServer : AppServer<AppServerSystemConfiguration, AppServerCommandConfiguration, AppServerPerformance>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the service.
        /// </summary>
        public AppServer()
            : this((IContainer)null)
        {
        }
        /// <summary>
        /// This constructor is called by the .Net component model when adding it to a container
        /// </summary>
        /// <param name="container">The container to add the component to.</param>
        public AppServer(IContainer container)
            : base(container)
        {
        }
        #endregion
    }

    /// <summary>
    /// The AppServerBase class is the class that all server applications derive from.
    /// </summary>
    [ToolboxBitmap(typeof(XimuraResourcePlaceholder), "Ximura.Resources.AppServer.bmp")]
    //[Designer("Ximura.Design.Applications.AppServerDesigner, XimuraDesign")]
    public partial class AppServer<CONFSYS, CONFCOM, PERF> : AppBase<CONFCOM, PERF>, IXimuraAppServer
        where CONFSYS : AppServerSystemConfiguration, new()
        where CONFCOM : AppServerCommandConfiguration, new()
        where PERF : AppServerPerformance, new()
    {
        #region Declarations
        #region Containers/Service containers
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// This is the container for the control classes.
        /// </summary>
        protected XimuraAppContainer mControlContainer = null;
        /// <summary>
        /// The service container contains all the components for the container.
        /// </summary>
        protected IServiceContainer mControlServiceContainer = null;
        #endregion // Containers
        #region Application Components
        private IXimuraSessionManagerRegistration mSecurityMan = null;
        private IXimuraSessionManager mSessionMan = null;
        #endregion // Components
        #region Attributes
        private XimuraApplicationIDAttribute mApplicationIDAttribute = null;
        /// <summary>
        /// This method holds the appserver attribute settings.
        /// </summary>
        private XimuraAppServerAttribute mAppServerAttribute = null;
        #endregion // Attributes
        #region Configuration/Settings
        private Guid? mAppID = null;
        #endregion // Settings
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the service.
        /// </summary>
        public AppServer() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by the .Net component model when adding it to a container
        /// </summary>
        /// <param name="container">The container to add the component to.</param>
        public AppServer(IContainer container)
            : base(container)
        {
            //Set the default value for the domain status.
            //mSeperateDomain = AppServerAttribute.DomainRequired;

            InitializeComponents();
            RegisterContainer(components);
        }
        #endregion
        #region Dispose(bool disposing)
        /// <summary> 
        /// This is an override of the IDisposable method which cleans up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (mControlContainer != null)
                {
                    mControlContainer.Dispose();
                }

                XimuraAppTrace.Close();
            }
        }
        #endregion

        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion // InitializeComponents()
        #region InitializeControlContainer()
        /// <summary>
        /// This method initializes the base components container 
        /// and the Application control container
        /// </summary>
        protected virtual void InitializeControlContainer()
        {
            //Set up the control container
            if (mControlServiceContainer == null)
                mControlServiceContainer = new XimuraServiceContainer();

            //Check that we only do this once
            if (mControlContainer == null)
                mControlContainer = new XimuraAppContainer(mControlServiceContainer, this);
        }
        #endregion

        #region ControlServiceContainer
        /// <summary>
        /// This is the control service container.
        /// </summary>
        protected virtual IServiceContainer ControlServiceContainer
        {
            get
            {
                return mControlServiceContainer;
            }
        }
        #endregion // ControlServiceContainer
        #region ControlContainer
        /// <summary>
        /// This is the control container.
        /// </summary>
        protected virtual IContainer ControlContainer
        {
            get
            {
                return mControlContainer;
            }
        }
        #endregion // ControlContainer

        #region ApplicationID
        /// <summary>
        /// This property returns the internal application ID
        /// </summary>
        [ReadOnly(true)]
        public virtual Guid ApplicationID
        {
            get
            {
                // Get the application ID from the assembly
                //First check whether we have got it from the application
                //previously
                if (!mAppID.HasValue)
                {
                    mAppID = GetAppGuid();
                }

                return mAppID.HasValue ? mAppID.Value : Guid.Empty;
            }
        }

        private Guid? GetAppGuid()
        {
            XimuraApplicationIDAttribute attr = AH.GetAttribute<XimuraApplicationIDAttribute>(GetType());

            if (attr != null)
                return attr.ApplicationID;

            return null;
        }
        #endregion
        #region AppServerAttribute/ApplicationIDAttribute
        /// <summary>
        /// This property is the XimuraAppServerAttribute applied to the application.
        /// </summary>
        protected XimuraAppServerAttribute AppServerAttribute
        {
            get
            {
                if (mAppServerAttribute == null)
                {
                    object[] attrs = this.GetType().GetCustomAttributes(typeof(XimuraAppServerAttribute), true);
                    if (attrs.Length > 0)
                        mAppServerAttribute = attrs[0] as XimuraAppServerAttribute;

                    if (mAppServerAttribute == null)
                        mAppServerAttribute = GetDefaultAttributeSettings();
                }
                return mAppServerAttribute;
            }
        }
        private XimuraAppServerAttribute GetDefaultAttributeSettings()
        {
            return new XimuraAppServerAttribute(this.GetType().Name);
        }

        /// <summary>
        /// This property is the XimuraApplicationIDAttribute applied to the application.
        /// </summary>
        protected XimuraApplicationIDAttribute ApplicationIDAttribute
        {
            get
            {
                if (mApplicationIDAttribute == null)
                {
                    object[] attrs = this.GetType().GetCustomAttributes(typeof(XimuraApplicationIDAttribute), true);
                    if (attrs.Length > 0)
                        mApplicationIDAttribute = attrs[0] as XimuraApplicationIDAttribute;
                }
                return mApplicationIDAttribute;
            }
        }

        #endregion

        #region SiteBeforeChange
        /// <summary>
        /// This overriden method does nothing. AppServer does not care about the site property.
        /// </summary>
        /// <param name="newSite">The new site value.</param>
        /// <param name="oldSite">The old site value.</param>
        protected override void SiteBeforeChange(ISite oldSite, ISite newSite)
        {
            //Do nothing
        }
        #endregion // SiteBeforeChange
        #region SiteChanged
        /// <summary>
        /// This overriden method does nothing. AppServer does not care about the site property.
        /// </summary>
        /// <param name="oldSite">The old site.</param>
        /// <param name="newSite">The new site.</param>
        protected override void SiteChanged(ISite oldSite, ISite newSite)
        {
            //Do nothing.
        }
        #endregion // SiteChanged

        #region AgentsAdd<A>
        /// <summary>
        /// This method is used to add a collection of agent holders with a particular service.
        /// </summary>
        /// <typeparam name="A">The agent attribute type.</typeparam>
        /// <param name="AgentsDefault">The default agent collection.</param>
        /// <param name="Service">The service to add the agent holder to.</param>
        protected virtual void AgentsAdd<A>(IEnumerable<XimuraServerAgentHolder> AgentsDefault, IXimuraAppServerAgentService Service)
            where A : XimuraAppServerAgentAttributeBase
        {
            AgentsDefault
                .Union(AH.GetAttributes<A>(GetType()).Select(t => t.Agent))
                .ForEach(a => Service.AgentAdd(a));
        }
        #endregion // AgentsAdd<A>
        #region AgentsRemove<A>
        /// <summary>
        /// This method is used to remove a collection of agent holders from a particular service.
        /// </summary>
        /// <typeparam name="A">The agent attribute type.</typeparam>
        /// <param name="AgentsDefault">The default agent collection.</param>
        /// <param name="Service">The service to remove the agent holder from.</param>
        protected virtual void AgentsRemove<A>(IEnumerable<XimuraServerAgentHolder> AgentsDefault, IXimuraAppServerAgentService Service)
            where A : XimuraAppServerAgentAttributeBase
        {
            AgentsDefault
                .Union(AH.GetAttributes<A>(GetType()).Select(t => t.Agent))
                .Reverse()
                .ForEach(a => Service.AgentRemove(a));
        }
        #endregion // AgentsRemove<A>
    }
}
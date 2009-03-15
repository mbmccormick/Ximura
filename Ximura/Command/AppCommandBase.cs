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
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;

using AH = Ximura.Helper.AttributeHelper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Command
{
    /// <summary>
    /// AppCommandBase is the base object for an Application command.
    /// </summary>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    /// <typeparam name="PERF">The command performance monitor object.</typeparam>
    public class AppCommandBase<CONF, PERF> : AppCommandBase<CONF, PERF, CONF>
        where CONF : CommandConfiguration, new()
        where PERF : CommandPerformance, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public AppCommandBase() : this(null) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="container">The container to be added to</param>
        public AppCommandBase(System.ComponentModel.IContainer container) : base(container) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandBase(Guid? commandID, System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion
    }

    /// <summary>
    /// AppCommandBase is the base object for an Application command.
    /// </summary>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    /// <typeparam name="PERF">The command performance monitor object.</typeparam>
    /// <typeparam name="EXTCONF">The external command object that contains a set of user configurable settings.</typeparam>
	[ToolboxBitmap(typeof(XimuraResourcePlaceholder),"Ximura.Resources.XimuraComponent.bmp")]
    public class AppCommandBase<CONF, PERF, EXTCONF> : AppBase<CONF, PERF, EXTCONF>, IXimuraCommandBase
        where CONF : CommandConfiguration, new()
        where PERF : CommandPerformance, new()
        where EXTCONF : CommandConfiguration, new()
    {
		#region Declarations

        private XimuraAppModuleAttribute m_AppCommandAttribute = null;

        private XimuraAppConfigurationAttribute mAppConfigurationAttribute = null;

        /// <summary>
        /// The dispatcherCollection is the command gateway to the dispatcher. 
        /// </summary>
        private IXimuraCommandBridge mCommandBridge = null;

        private IXimuraCommand mCommandDefinition;
        /// <summary>
        /// The debug name is used to accurately identify the command within the application.
        /// </summary>
        private string mDebugName = null;

		private Guid? mID = null;
		/// <summary>
		/// This is the command name
		/// </summary>
		private string mName = null;
		/// <summary>
		/// This is the command description
		/// </summary>
		private string mDescription = null;
		/// <summary>
		/// The settings provider for the base command container.
		/// </summary>
		protected IXimuraAppSettings settingsProvider = null;

        private string mParentCommandName = "";

        private bool mSupportsNotifications = false;
		#endregion
		#region Constructor
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public AppCommandBase():this(null){}
		/// <summary>
		/// This is the base constructor for a Ximura command
		/// </summary>
		/// <param name="container">The container to be added to</param>
        public AppCommandBase(System.ComponentModel.IContainer container) : base(container) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandBase(Guid? commandID, System.ComponentModel.IContainer container) : base(container) 
        {
            mID = commandID;
        }
        #endregion
		#region Dispose
		/// <summary>
		/// This overriden Dispose method removes any services or service references
		/// </summary>
		/// <param name="disposing">True is the method is disposing</param>
		protected override void Dispose( bool disposing )
		{
			//Remove any references to any services
			if (disposing) 
			{

                if (this.ServiceStatus != XimuraServiceStatus.Stopped)
                {
                    ConfigurationStop();
                    ServicesDereference();

                    ServicesRemove();
                }

				//Remove any references to other services.
				ServicesDereference();
			}

			base.Dispose(disposing);
		}
		#endregion // Dispose

        #region AppCommandAttribute
        /// <summary>
		/// This property is the XimuraAppCommandAttribute applied to the application.
		/// </summary>
		protected XimuraAppModuleAttribute AppCommandAttribute
		{
			get
			{
				if (m_AppCommandAttribute == null)
				{
//					Type meType = this.GetType();
//					object[] meattrs = meType.GetCustomAttributes(true);
					object[] attrs = this.GetType().GetCustomAttributes(typeof(XimuraAppModuleAttribute),true);
					if (attrs.Length>0)
						m_AppCommandAttribute=attrs[0] as XimuraAppModuleAttribute;
					else
						m_AppCommandAttribute=new XimuraAppModuleAttribute(Guid.Empty.ToString());
				}
				return m_AppCommandAttribute;
			}
		}
		#endregion
        #region AppConfigurationAttribute
        /// <summary>
        /// This property is the XimuraAppCommandAttribute applied to the application.
        /// </summary>
        protected XimuraAppConfigurationAttribute AppConfigurationAttribute
        {
            get
            {
                if (mAppConfigurationAttribute == null)
                {
                    object[] attrs = this.GetType().GetCustomAttributes(typeof(XimuraAppConfigurationAttribute), true);
                    if (attrs.Length > 0)
                        mAppConfigurationAttribute = attrs[0] as XimuraAppConfigurationAttribute;
                    else
                        mAppConfigurationAttribute = new XimuraAppConfigurationAttribute(ConfigurationLocation.None, "");
                }
                return mAppConfigurationAttribute;
            }
        }
        #endregion

		#region IXimuraCommand Members
		#region CommandID
		/// <summary>
		/// This is the command ID.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("This is the command ID.")]
		[Category("Command")]
		public Guid CommandID
		{
			get
			{
				if (mID.HasValue)
					return mID.Value;
				else
                    return AppCommandAttribute.ID;
			}
		}
		#endregion // CommandID
		#region CommandName
		/// <summary>
		/// This is command name used to route requests to it in an application.
		/// </summary>
		/// 		
		[
		Description("This is command name used to route requests to it in an application."),
		Category("Command"),
		DefaultValue(null)
		]
		public virtual string CommandName
		{
			get
			{
				if (mName == null || mName == "")
					return AppCommandAttribute.Name;
				else
					return mName;
			}
			set
			{
				mName = value;
			}
		}
		#endregion // CommandName
		#region CommandDescription
		/// <summary>
		/// This is the command friendly description
		/// </summary>
		[
		Description("This is the command description."),
		Category("Command"),
		DefaultValue(null)
		]
		public string CommandDescription
		{
			get
			{
				if (mDescription == null)
					return AppCommandAttribute.Description;
				else
					return mDescription;
			}
			set
			{
				mDescription = value;
			}
		}
		#endregion // CommandDescription
		#endregion

        #region ConfigClass
        /// <summary>
        /// This property contains the prefered config class type. If this property is set
        /// the config class must inherit from this class.
        /// </summary>
        protected virtual Type ConfigClass
        {
            get
            {
                string mConfigClass = AppCommandAttribute.ConfigClass;

                if (mConfigClass == null)
                    return null;
                else
                    return RH.CreateTypeFromString(AppCommandAttribute.ConfigClass);
            }
        }
        #endregion // ConfigClass
        #region ConfigClassForce
        /// <summary>
        /// This property determines whether the config class should be forced to a specific type.
        /// </summary>
        protected virtual bool ConfigClassForce
        {
            get
            {
                return AppCommandAttribute.ForceConfigClass;
            }
        }
        #endregion // ConfigClassForce
        #region GetSettings methods
        /// <summary>
		/// This protected member returns the settings for the command
		/// </summary>
		/// <param name="key">The key that specifies the specific settings object</param>
		/// <returns>The settings object</returns>
		protected virtual object GetSettings(string key)
		{
			if (this.Site == null)
				throw new ApplicationException("The command has not been joined to the container.");

            //If we are in design mode there are no settings.
            if (this.Site.DesignMode)
                return null;

			//Get the settings provider
			if (settingsProvider == null)
			{
				GetSettingsProvider();

				//If it's still null then exit
				if (settingsProvider == null)
					throw new ApplicationException("The Settings provider is null");
			}

            object set;
            if (ConfigClassForce && ConfigClass != null)
                set = settingsProvider.GetConfigExtended(key, ConfigClass, true);
            else
                set = settingsProvider.GetConfig(key);

            if (set == null)
                throw new ApplicationException("The command settings are not available.");

            if (ConfigClass != null && set.GetType()!=ConfigClass 
                && !set.GetType().IsInstanceOfType(ConfigClass))
                throw new ApplicationException("Settings class is not of the correct type.");

			return set;
		}

		#endregion
		#region GetSettingsProvider
		/// <summary>
		/// This method gets the settings for the command.
		/// </summary>
		protected virtual void GetSettingsProvider()
		{
            if (settingsProvider == null)
			    settingsProvider = GetService<IXimuraAppSettings>();
		}
		#endregion // GetSettingsProvider
        #region BaseSettings
        /// <summary>
        /// This is the base settings class. It is primarily used when loading the configuration in hybrid mode. 
        /// This default method returns null;
        /// </summary>
        protected virtual IXimuraConfigSH BaseSettings
        {
            get { return null; }
        }
        #endregion // BaseSettings

        #region ConfigurationLoad
        /// <summary>
        /// This method loads the command configuration.
        /// </summary>
        /// <param name="commandConfiguration"></param>
        protected override bool ConfigurationLoad(CONF commandConfiguration)
        {
            byte[] blob = null;
            bool applySettings = false;

            try
            {
                switch (AppConfigurationAttribute.ConfigType)
                {
                    case ConfigurationLocation.None:
                        return true;
                    case ConfigurationLocation.ConfigurationManager:
                        //blob = RH.ResourceLoadFromUri(new Uri(AppConfigurationAttribute.ConfigLocation));
                        break;
                    case ConfigurationLocation.Hybrid:
                        blob = RH.ResourceLoadFromUri(new Uri(AppConfigurationAttribute.ConfigLocation));
                        applySettings = true;
                        break;
                    case ConfigurationLocation.Resource:
                        blob = RH.ResourceLoadFromUri(new Uri(AppConfigurationAttribute.ConfigLocation));
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return commandConfiguration.Load(ApplicationDefinition, CommandDefinition,
                blob != null ? new MemoryStream(blob) : (Stream)null, applySettings?BaseSettings:(IXimuraConfigSH)null);
        }
        #endregion // commandConfiguration
        #region ConfigurationManager
        /// <summary>
        /// This override gets the ConfigurationManager from the IXimuraConfigurationManager service.
        /// </summary>
        protected override IXimuraConfigurationManager ConfigurationManager
        {
            get
            {
                if (base.ConfigurationManager == null)
                    base.ConfigurationManager = GetService<IXimuraConfigurationManager>();

                return base.ConfigurationManager;
            }
            set
            {
                throw new NotSupportedException("ConfigurationManager cannot be set in the command object.");
            }
        }
        #endregion

        #region PerformanceStart()
        /// <summary>
        /// This method creates and registers the command performance object.
        /// </summary>
        protected override void PerformanceStart()
        {
            base.PerformanceStart();

            Performance.ID = this.CommandID;
            Performance.PCID = this.CommandID;
            Performance.Name = this.CommandName;
            Performance.Category = "Command";

            if (PerformanceManager != null)
                PerformanceManager.PerformanceCounterCollectionRegister(Performance);
        }
        #endregion
        #region PerformanceStop()
        /// <summary>
        /// This method unregisters the command performance object.
        /// </summary>
        protected override void PerformanceStop()
        {
            if (PerformanceManager != null)
                PerformanceManager.PerformanceCounterCollectionUnregister(Performance);

            base.PerformanceStop();
        }
        #endregion
        #region PerformanceManager
        /// <summary>
        /// This override gets the performance manager object form the IXimuraPerformanceManager service
        /// </summary>
        protected override IXimuraPerformanceManager PerformanceManager
        {
            get
            {
                if (base.PerformanceManager == null)
                    base.PerformanceManager = GetService<IXimuraPerformanceManager>();

                return base.PerformanceManager;
            }
            set
            {
                throw new NotSupportedException("PerformanceManager cannot be set in the command object.");
            }
        }
        #endregion // PerformanceManager

        #region PoolManagerStart()
        /// <summary>
        /// This override gets a reference to the pool manager.
        /// </summary>
        protected override void PoolManagerStart()
        {
            PoolManager = GetService<IXimuraPoolManager>();
        }
        #endregion // PoolManagerStart()

        #region ApplicationDefinition
        /// <summary>
        /// This private method contains the unique application identifiers.
        /// </summary>
        protected override IXimuraApplicationDefinition ApplicationDefinition
        {
            get
            {
                if (mApplicationDefinition == null)
                    mApplicationDefinition = GetService<IXimuraApplicationDefinition>();

                return mApplicationDefinition;
            }
        }
        #endregion // ApplicationDefinition
        #region CommandDefinition
        /// <summary>
        /// This private method contains the unique application identifiers.
        /// </summary>
        protected IXimuraCommand CommandDefinition
        {
            get
            {
                if (mCommandDefinition == null)
                    mCommandDefinition = new CommandDefinition(this);

                return mCommandDefinition;
            }
        }
        #endregion // ApplicationDefinition

        #region DebugName
        /// <summary>
        /// The debug name can be used to accurately determine the unique name for the 
        /// application and the command.
        /// </summary>
        protected string DebugName
        {
            get
            {
                if (mDebugName == null)
                {
                    if (ParentCommandName == null || ParentCommandName == "")
                        mDebugName = ApplicationDefinition.ApplicationName + @"\" + CommandName;
                    else
                        mDebugName = ApplicationDefinition.ApplicationName + @"\" + ParentCommandName + @"\" + CommandName;
                }
                return mDebugName;
            }
        }
        #endregion // DebugName

        #region IXimuraServiceParentSettings Members
        /// <summary>
        /// This method is used by nested commands to get their command settings.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string ParentCommandName
        {
            get
            {
                return mParentCommandName; ;
            }
            set
            {
                mParentCommandName = value;
            }
        }
        #endregion
        #region IXimuraCommandNotification Members
        /// <summary>
        /// This base method always returns false, as notifications are not supported. Override this method and return true if you wish 
        /// to support notifications.
        /// </summary>
        [Category("Command Settings")]
        [DefaultValue(false)]
        [Description("This method returns determines whether the command supports system notifications.")]
        public virtual bool SupportsNotifications
        {
            get { return mSupportsNotifications; }
            set { mSupportsNotifications = value; }
        }
        /// <summary>
        /// This method is called when there is a system notification.
        /// </summary>
        /// <param name="notificationType">The notification object type.</param>
        /// <param name="notification">The notification object.</param>
        public virtual void Notify(Type notificationType, object notification)
        {

        }
        #endregion

        #region CommandBridge
        /// <summary>
        /// The command bridge is the bridge to the security manager and the dispatcher.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual IXimuraCommandBridge CommandBridge
        {
            get
            {
                return mCommandBridge;
            }
        }
        #endregion // CommandBridge
        #region CommandBridgeStart()
        /// <summary>
        /// This method gets a reference to the command bridge and registers the command.
        /// </summary>
        protected virtual void CommandBridgeStart()
        {
            //Get the dispatcher collection.
            mCommandBridge = GetService<IXimuraCommandBridge>();

            CommandBridgeRegister(true);
        }
        #endregion // CommandBridgeStart()
        #region CommandBridgeRegister(bool register)
        /// <summary>
        /// This method determines whether the command is registered or unregistered with the command bridge.
        /// </summary>
        /// <param name="register">A boolean value indicating whether this command should be registered or unregistered.</param>
        protected virtual void CommandBridgeRegister(bool register)
        {

        }
        #endregion // CommandBridgeRegister(bool register)
        #region CommandBridgeStop()
        /// <summary>
        /// This method unregisters the command and removes the reference to the command bridge.
        /// </summary>
        protected virtual void CommandBridgeStop()
        {
            CommandBridgeRegister(false);

            mCommandBridge = null;
        }
        #endregion // CommandBridgeStop()
    }
}
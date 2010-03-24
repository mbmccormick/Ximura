#region Copyright
//*******************************************************************************
//Copyright (c) 2000-2009 Paul Stancer.
//All rights reserved. This program and the accompanying materials
//are made available under the terms of the Eclipse Public License v1.0
//which accompanies this distribution, and is available at
//http://www.eclipse.org/legal/epl-v10.html

//Contributors:
//    Paul Stancer - initial implementation
//*******************************************************************************
#endregion
#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;
using System.Linq;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The performance manager manages the interaction between the performance system and the commands.
    /// </summary>
    [XimuraAppModule("5E609DAA-AD76-4eb5-B0CC-18C3A3ECD9C0", "PerformanceManager")]
    public partial class PerformanceManager:
        AppServerAgentManager<IXimuraPerformanceAgent, PerformanceManagerConfiguration, PerformanceCounterCollection>, IXimuraPerformanceManagerService
	{
		#region Declarations
		#endregion
		#region Constructor
		/// <summary>
		/// The default Ximura Application constructor
		/// </summary>
		/// <param name="container">The container the services should be added to.</param>
        public PerformanceManager(System.ComponentModel.IContainer container)
            : base(container) 
        {
        }
		#endregion
        #region ServicesProvide/ServicesRemove
        /// <summary>
        /// This override adds the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            AddService<IXimuraPerformanceManagerService>(this);
        }
        /// <summary>
        /// This override removes the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesRemove()
        {
            RemoveService<IXimuraPerformanceManagerService>();

            base.ServicesRemove();
        }
        #endregion // ServicesProvide/ServicesRemove

        #region InternalStart/InternalStop
        /// <summary>
        /// This override creates the performance counter collection.
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();

            ManagerInternal = new PerformanceCounterCollection();
        }
        /// <summary>
        /// This override removes the performance collection.
        /// </summary>
        protected override void InternalStop()
        {
            ManagerInternal.Clear();
            ManagerInternal = null;

            base.InternalStop();
        }
        #endregion // InternalStart/InternalStop

		#region Provider Handling
        /// <summary>
        /// This method creates the performance agent.
        /// </summary>
        /// <param name="holder">The agent holder.</param>
        /// <returns>The performance agent.</returns>
        /// <exception cref="Ximura.Framework.AgentInvalidTypeException">This exception is thrown if the agent type does not implement the IXimuraPerformanceAgent interface.</exception>
        protected override IXimuraPerformanceAgent AgentCreate(XimuraServerAgentHolder holder)
        {
            if (!RH.ValidateInterface(holder.AgentType, typeof(IXimuraPerformanceAgent)))
                throw new AgentInvalidTypeException(
                    string.Format("The agent type '{0}' does not implement the IXimuraPerformanceAgent interface", holder.AgentType.Name));

            return (IXimuraPerformanceAgent)RH.CreateObjectFromType(holder.AgentType);
        }
        #endregion

        #region ManagerInternal
        /// <summary>
        /// This is the performance counter collection.
        /// </summary>
        protected PerformanceCounterCollection ManagerInternal
        {
            get;
            set;
        }
        #endregion // ManagerInternal
        #region Manager
        /// <summary>
        /// This is the performance manager used to register performance services.
        /// </summary>
        public IXimuraPerformanceManager Manager
        {
            get { return ManagerInternal; }
        }
        #endregion // Manager
    }
}

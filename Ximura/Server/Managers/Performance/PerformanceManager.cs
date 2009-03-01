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
using Ximura.Server;

using Ximura.Command;

#endregion // using
namespace Ximura.Server
{
    [XimuraAppModule("5E609DAA-AD76-4eb5-B0CC-18C3A3ECD9C0", "PerformanceManager")]
    public partial class PerformanceManager:
        AppServerAgentManager<IXimuraPerformanceAgent, PerformanceManagerConfiguration, PerformanceCounterCollection>, IXimuraPerformanceManagerService
	{
		#region Declarations

		#endregion
		#region Constructors / Destructor
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

        protected override void InternalStart()
        {
            base.InternalStart();

            ManagerInternal = new PerformanceCounterCollection();
        }

        protected override void InternalStop()
        {
            ManagerInternal.Clear();
            ManagerInternal = null;

            base.InternalStop();
        }

		#region Provider Handling

        protected override IXimuraPerformanceAgent AgentCreate(XimuraServerAgentHolder holder)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected PerformanceCounterCollection ManagerInternal
        {
            get;
            set;
        }

        /// <summary>
        /// This is the performance manager used to register performance services.
        /// </summary>
        public IXimuraPerformanceManager Manager
        {
            get { return ManagerInternal; }
        }
    }
}

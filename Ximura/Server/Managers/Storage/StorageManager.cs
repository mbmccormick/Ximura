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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
    [XimuraAppModule("834F904B-21B2-4721-ACCA-F2B3D67FF311", "StorageManager")]
    public class StorageManager : 
        AppServerAgentManager<StorageAgentBase, StorageManagerConfiguration, StorageManagerPerformance>, IXimuraStorageManagerService
        //where CONF : CommandConfiguration, new()<CONF>
    {
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public StorageManager():this(null){}
		/// <summary>
		/// The Ximura Application component model constructor
		/// </summary>
		/// <param name="container">The container that the services should be added to.</param>
        public StorageManager(IContainer container): base(container)
		{
		}
		#endregion

        protected override StorageAgentBase AgentCreate(XimuraServerAgentHolder holder)
        {
            throw new NotImplementedException();
        }

        #region ServicesProvide/ServicesRemove
        /// <summary>
        /// This override adds the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            AddService<IXimuraStorageManagerService>(this);
        }
        /// <summary>
        /// This override removes the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesRemove()
        {
            RemoveService<IXimuraStorageManagerService>();

            base.ServicesRemove();
        }
        #endregion // ServicesProvide/ServicesRemove
    }
}

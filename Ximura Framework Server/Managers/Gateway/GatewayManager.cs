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

using RH=Ximura.Reflection;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The gateway manager manages interactions with the external agents that interact with the system.
    /// </summary>
    [XimuraAppModule("8A1A0F0D-A903-4f7a-BC3C-B697D971C517", "GatewayManager")]
    public class GatewayManager :
        AppServerAgentManager<IXimuraGatewayAgent, GatewayManagerConfiguration, GatewayManagerPerformance>, IXimuraGatewayManagerService
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public GatewayManager() : this((IContainer)null) { }
        /// <summary>
        /// The Ximura Application component model constructor
        /// </summary>
        /// <param name="container">The container that the services should be added to.</param>
        public GatewayManager(IContainer container) : base(container) { }
        #endregion

        #region AgentCreate(XimuraServerAgentHolder holder)
        /// <summary>
        /// This method is used to create the Gateway agent.
        /// </summary>
        /// <param name="holder">The agent holder.</param>
        /// <returns>Returns the gateway agent.</returns>
        protected override IXimuraGatewayAgent AgentCreate(XimuraServerAgentHolder holder)
        {
            if (!RH.ValidateInterface(holder.AgentType, typeof(IXimuraGatewayAgent)))
                throw new AgentInvalidTypeException(
                    string.Format("The agent type '{0}' does not implement the IXimuraGatewayAgent interface", holder.AgentType.Name));

            return (IXimuraGatewayAgent)RH.CreateObjectFromType(holder.AgentType);
        }
        #endregion // AgentCreate(XimuraServerAgentHolder holder)


        #region ServicesProvide/ServicesRemove
        /// <summary>
        /// This override adds the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            AddService<IXimuraGatewayManagerService>(this);
        }
        /// <summary>
        /// This override removes the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesRemove()
        {
            RemoveService<IXimuraGatewayManagerService>();

            base.ServicesRemove();
        }
        #endregion // ServicesProvide/ServicesRemove
    }
}

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
using Ximura.Framework;
using Ximura.Framework;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This class provides common functionality for agent based process services.
    /// </summary>
    /// <typeparam name="AGENT">The agent type.</typeparam>
    /// <typeparam name="CONF">The agent manager configuration.</typeparam>
    /// <typeparam name="PERF">The performance counter type.</typeparam>
    public abstract class AppServerAgentManager<AGENT, CONF, PERF> : 
        AppServerProcessBase<CONF, PERF>, IXimuraAppServerAgentService
        where AGENT : class, IXimuraServerAgent
        where CONF : ConfigurationBase, new()
        where PERF : PerformanceCounterCollection, new()
    {
        #region Declarations
        /// <summary>
        /// This is the agent collection.
        /// </summary>
        protected Dictionary<string, AGENT> mAgents;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AppServerAgentManager() : this((IContainer)null) { }
        /// <summary>
        /// The Ximura Application component model constructor
        /// </summary>
        /// <param name="container">The container that the services should be added to.</param>
        public AppServerAgentManager(IContainer container)
            : base(container)
        {
            mAgents = new Dictionary<string, AGENT>();
        }
        #endregion

        #region AgentAdd
        /// <summary>
        /// This method creates an agent based on the type passed in the holder and adds it to the agent collection.
        /// </summary>
        /// <param name="holder">The agent metadata holder.</param>
        public virtual void AgentAdd(XimuraServerAgentHolder holder)
        {
            AGENT agent = AgentCreate(holder);

            AgentAdd(agent, holder.AgentID);
        }
        /// <summary>
        /// This method adds an agent to the collection.
        /// </summary>
        /// <param name="agent">The system agent.</param>
        /// <param name="id">The agent id.</param>
        public virtual void AgentAdd(AGENT agent, string id)
        {
            mAgents.Add(id, agent);
        }
        #endregion

        #region AgentCreate(XimuraServerAgentHolder holder);
        /// <summary>
        /// This abstract method creates the specific agent.
        /// </summary>
        /// <param name="holder">THe agent holder information.</param>
        /// <returns>Returns an agent class.</returns>
        protected abstract AGENT AgentCreate(XimuraServerAgentHolder holder);
        #endregion // AgentCreate(XimuraServerAgentHolder holder);
    }
}

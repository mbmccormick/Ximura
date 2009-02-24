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

using Ximura;
using Ximura.Helper;

using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This attribute class is used to identify specific agents for the application.
    /// </summary>
    public class XimuraAppServerAgentAttributeBase: System.Attribute
    {
        #region Declarations
        XimuraServerAgentHolder holder;
        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        public XimuraAppServerAgentAttributeBase(string type, string ID, string Name)
            : this(RH.CreateTypeFromString(type), ID, Name){ }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        public XimuraAppServerAgentAttributeBase(Type type, string ID, string Name)
        {
            holder = new XimuraServerAgentHolder(type, ID, Name);
        }

        #endregion
        /// <summary>
        /// The agent.
        /// </summary>
        public XimuraServerAgentHolder Agent { get { return holder; } }

        //public Type AgentType { get { return holder.AgentType; } }

        //public string AgentID { get { return holder.AgentID; } }

        //public string AgentName { get { return holder.AgentName; } }

    }
}

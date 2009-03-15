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
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        /// <param name="name">The agent name.</param>
        public XimuraAppServerAgentAttributeBase(string type, string ID, string Name)
            : this(RH.CreateTypeFromString(type), ID, Name){ }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        /// <param name="name">The agent name.</param>
        public XimuraAppServerAgentAttributeBase(Type type, string ID, string Name)
        {
            holder = new XimuraServerAgentHolder(type, ID, Name);
        }

        #endregion
        /// <summary>
        /// The agent holder.
        /// </summary>
        public XimuraServerAgentHolder Agent { get { return holder; } }

    }
}

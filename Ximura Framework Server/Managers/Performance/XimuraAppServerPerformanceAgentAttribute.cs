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
namespace Ximura.Framework
{
    /// <summary>
    /// The agent is used to set the performance agents for the application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class XimuraAppServerPerformanceAgentAttribute : XimuraAppServerAgentAttributeBase
    {
        #region Constructors
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        /// <param name="name">The agent name.</param>
        public XimuraAppServerPerformanceAgentAttribute(string type, string id, string name)
            : base(RH.CreateTypeFromString(type), id, name) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        /// <param name="name">The agent name.</param>
        public XimuraAppServerPerformanceAgentAttribute(Type type, string id, string name)
            : base(type, id, name) { }
        #endregion
    }
}

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
﻿#region using
using System;

using Ximura;


using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The XimuraAppServerAttribute attribute is used to set friendly names and descriptions for
    /// the server which will be used in the server performance counters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class XimuraSessionManagerAttribute : XimuraAppServerAgentAttributeBase
    {
        #region Constructor
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        public XimuraSessionManagerAttribute(string type)
            : base(type, null, null) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        public XimuraSessionManagerAttribute(Type type)
            : base(type, null, null) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        public XimuraSessionManagerAttribute(string type, string id)
            : base(type, id, null) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        public XimuraSessionManagerAttribute(Type type, string id)
            : base(type, id, null) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        /// <param name="name">The agent name.</param>
        public XimuraSessionManagerAttribute(string type, string id, string name)
            : base(type, id, name) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="type">The agent type.</param>
        /// <param name="id">The agent id.</param>
        /// <param name="name">The agent name.</param>
        public XimuraSessionManagerAttribute(Type type, string id, string name)
            : base(type, id, name) { }
        #endregion // Constructor
    }
}

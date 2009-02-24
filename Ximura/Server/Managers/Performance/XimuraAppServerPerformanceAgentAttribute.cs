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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class XimuraAppServerPerformanceAgentAttribute : XimuraAppServerAgentAttributeBase
    {
        #region Constructors

        public XimuraAppServerPerformanceAgentAttribute(string AgentType, string AgentID, string AgentName)
            : base(RH.CreateTypeFromString(AgentType), AgentID, AgentName){ }

        public XimuraAppServerPerformanceAgentAttribute(Type AgentType, string AgentID, string AgentName)
            : base(AgentType, AgentID, AgentName){ }

        #endregion


    }
}

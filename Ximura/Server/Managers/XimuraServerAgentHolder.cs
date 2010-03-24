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
    /// The agent holder class is used by the agent based attributes to pass common information to the 
    /// server processes.
    /// </summary>
    public class XimuraServerAgentHolder
    {
        public Type AgentType;
        public string AgentID;
        public string AgentName;

        public XimuraServerAgentHolder(Type type, string ID, string Name)
        {
            this.AgentType = type;
            this.AgentID = ID;
            this.AgentName = Name;
        }

    }
}

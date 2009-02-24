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
    /// The XimuraAppServerAttribute attribute is used to set friendly names and descriptions for
    /// the server which will be used in the server performance counters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class XimuraAppServerStorageAttribute : XimuraAppServerAgentAttributeBase
    {
        #region Declarations
        #endregion

        #region Constructors

        public XimuraAppServerStorageAttribute(string type, string id, string name)
            : base(RH.CreateTypeFromString(type), id, name)
        { }

        public XimuraAppServerStorageAttribute(Type type, string id, string name)
            : base(type, id, name)
        {
        }

        #endregion

    }

}

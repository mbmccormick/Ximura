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
    public sealed class XimuraAppServerLoggerAttribute : XimuraAppServerAgentAttributeBase
    {
        #region Declarations
        #endregion

        #region Constructors

        public XimuraAppServerLoggerAttribute(string LoggerType, string LoggerID, string LoggerName)
            : base(RH.CreateTypeFromString(LoggerType), LoggerID, LoggerName)
        { }

        public XimuraAppServerLoggerAttribute(Type LoggerType, string LoggerID, string LoggerName)
            : base(LoggerType, LoggerID, LoggerName)
        {
        }

        #endregion



        #region IXimuraLoggerSettings Members

        public int GetSwitchValue(string Type)
        {
            return 0;
        }

        public int LogLevel
        {
            get { return 0; }
        }

        #endregion

        #region IXimuraLoggerSettings Members


        public string GetSetting(string Type)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}

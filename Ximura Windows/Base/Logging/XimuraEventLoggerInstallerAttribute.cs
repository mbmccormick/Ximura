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
using System.Diagnostics;
using System.Security.Permissions;

using Ximura;

using RH=Ximura.Reflection;
using Ximura.Framework;

#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// The XimuraAppServerAttribute attribute is used to set friendly names and descriptions for
    /// the server which will be used in the server performance counters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class XimuraInstallerEventLoggerAttribute : System.Attribute, IXimuraLoggerSettings
    {
        #region Declarations
        #endregion

        #region Constructors

        public XimuraInstallerEventLoggerAttribute(string LoggerID, string LoggerName)
        {
            this.LoggerID = LoggerID;
            this.LoggerName = LoggerName;
        }

        #endregion

        public Type LoggerType { get {return typeof(EventLogLogger);} }

        public string LoggerID { get; private set; }

        public string LoggerName { get; private set; }



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

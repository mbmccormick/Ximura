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
namespace Ximura.Framework
{
    /// <summary>
    /// The XimuraAppServerAttribute attribute is used to set friendly names and descriptions for
    /// the server which will be used in the server performance counters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class XimuraAppServerAttribute : Attribute
    {
        #region Declarations
        private string mAppName;
        private string mAppDescription;
        private bool mDomainRequired = false;
        #endregion

        #region Constructors
        /// <overloads></overloads>
        /// <summary>
        /// This Attributes identifies an Ximura Application.
        /// </summary>
        /// <param name="AppServerName">The ID of the Ximura application.</param>
        public XimuraAppServerAttribute(string AppServerName) : this(AppServerName, "") { }
        /// <summary>
        /// The Attributes identifies an Ximura Application.
        /// </summary>
        /// <param name="AppServerName">The ID of the Ximura application.</param>
        /// <param name="AppDescription">A short description of what the application does.</param>
        public XimuraAppServerAttribute(string AppServerName, string AppDescription)
        {
            mAppName = AppServerName;
            mAppDescription = AppDescription;
        }

        #endregion

        #region Properties
        /// <summary>
        /// This property returns true if the application requires a seperate 
        /// domain to run in.
        /// </summary>
        public bool DomainRequired { get { return mDomainRequired; } }
        /// <summary>
        /// The application instance of this specific part of an application.
        /// </summary>
        public string AppServerName { get { return mAppName; } }

        /// <summary>
        /// The Description of the application. This is used in the performance counter.
        /// </summary>
        public string AppDescription { get { return mAppDescription; } }
        #endregion
    }


}

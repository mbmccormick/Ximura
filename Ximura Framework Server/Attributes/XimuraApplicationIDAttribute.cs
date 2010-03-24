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
    /// The XimuraApplicationIDAttribute attribute is used to define a unique Guid for the 
    /// server application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple = false)]
    public sealed class XimuraApplicationIDAttribute : Attribute
    {
        #region Declarations
        private Guid mID;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The Attributes identifies an Ximura Application.
        /// </summary>
        /// <param name="ApplicationID">The ID of the Ximura application.</param>
        public XimuraApplicationIDAttribute(string ApplicationID)
        {
            mID = new Guid(ApplicationID);
        }
        #endregion // Constructor

        #region ApplicationID
        /// <summary>
        /// This is the ID of the Ximura application.
        /// </summary>
        public Guid ApplicationID
        {
            get
            {
                return mID;
            }
        }
        #endregion // ApplicationID
    }
}

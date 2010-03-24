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
using System.Threading;
using System.Timers;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the authorization handler metadata container used for the AuthHandlerExtender.
    /// </summary>
    public class AuthHandlerMetadataContainer : CommunicationMetadataContainer
    {
        #region Declarations
        private int mPriority;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        public AuthHandlerMetadataContainer()
        {
        }
        #endregion // Constructors


        #region Priority
        /// <summary>
        /// This is the priority property used to specify the order in which 
        /// </summary>
        public int Priority
        {
            get { return mPriority; }
            set { mPriority = value; }
        }
        #endregion // Priority

    }
}

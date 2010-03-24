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
using System.Collections.Generic;
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
    /// This extender class is used to interact with the various protocol authentication handlers.
    /// </summary>
    [ProvideProperty("Priority", "Ximura.Communication.AuthHandler, XimuraComm")]
    [ProvideProperty("Enabled", "Ximura.Communication.AuthHandler, XimuraComm")]
    [ProvideProperty("Identifier", "Ximura.Communication.AuthHandler, XimuraComm")]
    public class AuthHandlerExtender : 
        CommunicationExtenderBase<AuthHandler,AuthHandlerMetadataContainer>
    {
        #region Declarations
        private bool mEnabled;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        public AuthHandlerExtender():this(null)
        {
        }
        /// <summary>
		/// The component model constructor.
		/// </summary>
		/// <param name="container">The container.</param>
        public AuthHandlerExtender(IContainer container)
            : base(container)
		{
		}
        #endregion // Constructors

        #region Priority
        /// <summary>
        /// This property is used to return the enabled object settings.
        /// </summary>
        /// <param name="state">The object.</param>
        /// <returns>The identifier string value of the object.</returns>
        [DefaultValue(0), Category("Communication")]
        public virtual int GetPriority(AuthHandler item)
        {
            return ItemGet(item).Priority;
        }
        /// <summary>
        /// This property is used to set the enabled state.
        /// </summary>
        /// <param name="state">The object.</param>
        /// <param name="value">The identifier value.</param>
        public virtual void SetPriority(AuthHandler item, int value)
        {
            ItemGet(item).Priority = value;
        }
        #endregion

        
    }
}

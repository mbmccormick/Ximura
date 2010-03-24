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
    /// This is the base extender for the protocol and server containers.
    /// </summary>
    /// <typeparam name="C">The meta data container</typeparam>
    public class CommunicationExtenderBase<T, C> : MetadataExtender<T, C>
        where C : CommunicationMetadataContainer, new()
        where T : class
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public CommunicationExtenderBase() : this(null) { }
        /// <summary>
        /// The component model constructor.
        /// </summary>
        /// <param name="container">The container.</param>
        public CommunicationExtenderBase(IContainer container)
            : base(container)
        {
        }
        #endregion

        #region Enabled
        /// <summary>
        /// This property is used to return the enabled object settings.
        /// </summary>
        /// <param name="state">The object.</param>
        /// <returns>The enabled boolean value of the object.</returns>
        [DefaultValue(false), Category("Communication")]
        public virtual bool GetEnabled(T item)
        {
            return ItemGet(item).Enabled;
        }
        /// <summary>
        /// This property is used to set the enabled state.
        /// </summary>
        /// <param name="state">The object.</param>
        /// <param name="value">The enabled value.</param>
        public virtual void SetEnabled(T item, bool value)
        {
            ItemGet(item).Enabled = value;
        }
        #endregion

        #region Identifier
        /// <summary>
        /// This property is used to return the enabled object settings.
        /// </summary>
        /// <param name="state">The object.</param>
        /// <returns>The identifier string value of the object.</returns>
        [DefaultValue(null), Category("Communication")]
        public virtual string GetIdentifier(T item)
        {
            return ItemGet(item).Identifier;
        }
        /// <summary>
        /// This property is used to set the enabled state.
        /// </summary>
        /// <param name="state">The object.</param>
        /// <param name="value">The identifier value.</param>
        public virtual void SetIdentifier(T item, string value)
        {
            ItemGet(item).Identifier = value;
        }
        #endregion
    }
}

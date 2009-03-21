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
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This extender class is used to interact with the various protocol authentication handlers.
    /// </summary>
    [ProvideProperty("Enabled", "Ximura.Communication.MessageResolver, XimuraComm")]
    [ProvideProperty("Identifier", "Ximura.Communication.MessageResolver, XimuraComm")]
    public class MessageResolverExtender :
        CommunicationExtenderBase<MessageResolver, MessageResolverMetadataContainer>
    {
        #region Declarations
        private Dictionary<Type, MessageResolver> mResolver;
        private bool mEnabled = true;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        public MessageResolverExtender()
            : this(null)
        {
        }
        /// <summary>
        /// The component model constructor.
        /// </summary>
        /// <param name="container">The container.</param>
        public MessageResolverExtender(IContainer container)
            : base(container)
        {
            mResolver = new Dictionary<Type, MessageResolver>();
        }
        #endregion // Constructors

        #region Enabled
        /// <summary>
        /// This property determines whether the message resolver is active. 
        /// </summary>
        [DefaultValue(true)]
        [Description("This property determines whether the message resolver will map incoming stateless messages to their context.")]
        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }
        #endregion // Enabled

        #region Resolve(IXimuraMessageStream Message)
        /// <summary>
        /// This method attempts to resolve the relevant context based on the message contents.
        /// </summary>
        /// <param name="Message">The message to resolve.</param>
        /// <returns>Returns the Guid of the context, or null if the context cannot be resolved.</returns>
        public virtual Guid? Resolve(IXimuraMessageStream Message)
        {
            return null;
        }
        #endregion

    }
}

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
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the protocol request folder.
    /// </summary>
    public class RQServer : RQRSFolder
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public RQServer()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public RQServer(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public RQServer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
    /// <summary>
    /// This is the protocol response folder
    /// </summary>
    public class RSServer : RQRSFolder
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public RSServer()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public RSServer(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public RSServer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}

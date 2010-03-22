﻿#region Copyright
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

using System.Diagnostics;

using Ximura;
using Ximura.Helper;
using Ximura.Data;

using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Communication
{
    public class HTTPResponseMessage : InternetMessage
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HTTPResponseMessage()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This is the reset method to set the content.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
        #endregion // Reset()
        #region FragmentInitialType
        /// <summary>
        /// This method returns the initial fragment type for the class.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(HTTPResponseHeaderFragment);
            }
        }
        #endregion // FragmentInitialType
    }
}
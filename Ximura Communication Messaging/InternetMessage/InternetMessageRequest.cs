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
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using Ximura;

using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura.Communication
{
    public class InternetMessageRequest : InternetMessage
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetMessageRequest()
            : base()
        {
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This is the fragment type for the outgoing message.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(RequestHeaderFragment);
            }
        }
        #endregion // FragmentInitialType



    }
}

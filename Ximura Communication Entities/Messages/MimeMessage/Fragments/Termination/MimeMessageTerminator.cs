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
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;

using Ximura;
using Ximura.Helper;
using Ximura.Data;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This class is used to terminate a mime based message.
    /// </summary>
    public class MimeMessageTerminator : MessageGenericTerminatorBase<MimeMatchCollectionState>
    {

        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeMessageTerminator()
            : base()
        {
        }
        #endregion


        #region Initialize(string boundary)
        /// <summary>
        /// This method initializes the message terminator with the boundary string.
        /// </summary>
        /// <param name="boundary">The boundary data within the mime seperator.</param>
        public virtual void Initialize(string boundary)
        {
            base.mState = new MimeMatchCollectionState(boundary);


            Initialized = true;
        }
        #endregion


         
    }
}

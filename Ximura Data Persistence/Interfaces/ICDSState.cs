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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Data;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This interface is implemented by CDS States
    /// </summary>
    public interface ICDSState: IXimuraFSMState
    {
        /// <summary>
        /// This method can be used to initialize the request.
        /// </summary>
        /// <param name="context">The job context.</param>
        void Initialize(CDSContext context);
        /// <summary>
        /// This method is used to process the specific REST actions for the CDS State/
        /// </summary>
        /// <param name="action">The request action, i.e. Read, Create etc</param>
        /// <param name="context">The current context.</param>
        /// <returns>
        /// The boolean return value indicates whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        /// <exception cref="Ximura.Data.CDSStateException">
        /// A CDSStateException will be thrown if a request is made that this persistence manager does not support.
        /// </exception>
        bool ProcessAction(CDSStateAction action, CDSContext context);
        /// <summary>
        /// This method can be used to modify a response before it is sent back to a user.
        /// </summary>
        /// <param name="context">The job context.</param>
        void Finish(CDSContext context);
        /// <summary>
        /// This is the priority group for the CDSState. This property determines the grouping order in which 
        /// states will be polled for incoming requests.
        /// </summary>
        CDSStatePriorityGroup PriorityGroup { get; set; }
        /// <summary>
        /// This property determines the polling order for CDSStates within the specific group.
        /// </summary>
        short PriorityGroupID { get; set; }
        /// <summary>
        /// This method should return true when the action and entity are supported. This method is used by the CDS to build
        /// the Execution plan for specific Entity types and actions.
        /// </summary>
        /// <returns>Returns -1 is the action is not supported, otherwise the combined order is returned.</returns>
        int SupportsEntityAction(CDSStateAction action, Type objectType);
    }
}

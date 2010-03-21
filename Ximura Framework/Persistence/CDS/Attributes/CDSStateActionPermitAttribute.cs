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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;


using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// This attribute specifies which actions the CDSState implements. These attributes
    /// only specify the default behavior and can be overriden in specific implementation by 
    /// adding attributes with a higher priority or specifically in code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CDSStateActionPermitAttribute : Attribute
    {
        #region Declarations
        CDSStateAction mAction;
        bool mDeny;
        int mPriority;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This attribute specifies which actions the CDSState implements. These attributes
        /// only specify the default behavior and can be overriden in specific implementation by 
        /// adding attributes with a higher priority or specifically in code.
        /// </summary>
        /// <param name="action">
        /// This is the CDSStateAction that the attribute defines.
        /// </param>
        public CDSStateActionPermitAttribute(CDSStateAction action)
            : this(action, false, 0)
        {

        }

        /// <summary>
        /// This attribute specifies which actions the CDSState implements. These attributes
        /// only specify the default behavior and can be overriden in specific implementation by 
        /// adding attributes with a higher priority or specifically in code.
        /// </summary>
        /// <param name="action">
        /// This is the CDSStateAction that the attribute defines.
        /// </param>
        /// <param name="deny">
        /// This property defines whether the action is specifically denied. Deny will override a permit when they have the same priority. By default this value is set to false.
        /// </param>
        public CDSStateActionPermitAttribute(CDSStateAction action, bool deny):this(action,deny,0)
        {

        }

        /// <summary>
        /// This attribute specifies which actions the CDSState implements. These attributes
        /// only specify the default behavior and can be overriden in specific implementation by 
        /// adding attributes with a higher priority or specifically in code.
        /// </summary>
        /// <param name="action">
        /// This is the CDSStateAction that the attribute defines.
        /// </param>
        /// <param name="deny">
        /// This property defines whether the action is specifically denied. Deny will override a permit when they have the same priority. By default this value is set to false.
        /// </param>
        /// <param name="priority">
        /// This is the priority of the attribute. Higher value attributes will override lower value attributes.
        /// By default this value will be 0.
        /// </param>
        public CDSStateActionPermitAttribute(CDSStateAction action, bool deny, int priority)
        {
            mAction=action;
            mDeny=deny;
            mPriority=priority;
        }
        #endregion // Constructors

        #region Action
        /// <summary>
        /// This is the CDSStateAction that the attribute is specifying.
        /// </summary>
        public CDSStateAction Action
        {
            get { return mAction; }
        }
        #endregion // Action
        #region Deny
        /// <summary>
        /// This property defines whether the action is specifically denied. Deny will override a permit when they have the same priority. 
        /// By default this value is set to false.
        /// </summary>
        public bool Deny
        {
            get { return mDeny; }
        }
        #endregion // Deny
        #region Priority
        /// <summary>
        /// This is the priority of the attribute. Higher value attributes will override lower value attributes.
        /// By default this value will be 0.
        /// </summary>
        public int Priority
        {
            get { return mPriority; }
        }
        #endregion // Priority
    }
}

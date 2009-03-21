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
#region Copyright
// *****************************************************************************
// 
//  Loyalty Points System
// 
//  (c) SLA Ltd 2003-2004 
//
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of 
//  StancerLee Associates Limited (SLA Ltd), Central, Hong Kong 
//  and are supplied subject to the Non-Disclosure Agreement agreed between 
//  the distributed party and SLA Ltd.
//
//  This code cannot be distributed in part or as a whole to any third party 
//  without the express written permission of SLA Ltd.
//
// *****************************************************************************
#endregion // Copyright
#region using
using System;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using XIMS;
using XIMS.Data;
using CH = XIMS.Helper.Common;
using XIMS.Helper;
using XIMS.Applications;
using XIMS.Applications.Data;
using XIMS.Applications.Security;
using XIMS.Applications.Command;

#endregion // using
namespace XIMS.Communication
{
    public class SIPProtocolContext : BNFProtocolContext
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public SIPProtocolContext() : base() { }
        #endregion

    }
}

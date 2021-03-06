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

using XIMS;
using XIMS.Helper;
using XIMS.Data;
using CH = XIMS.Helper.Common;
#endregion
namespace XIMS.Communication
{
    /// <summary>
    /// This class is used to serialize and deserialize SIP messages
    /// </summary>
    public class SIPInternetMessageFormatter : InternetMessageFormatterBase
    {
        #region Constructor
		/// <summary>
		/// The default constructor
		/// </summary>
        public SIPInternetMessageFormatter() { }
		#endregion // Construcutor
    }
}

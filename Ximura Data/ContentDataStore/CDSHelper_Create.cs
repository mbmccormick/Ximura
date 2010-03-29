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
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Data;
using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{    /// <summary>
    /// This static class allows for dynamic updates to the CDS persistence service.
    /// </summary>
    public static partial class CDSHelper
    {
        #region CDSCreate(this IXimuraSessionRQ SessionRQ, Content inData)
        /// <summary>
        /// This is a shortcut method for creating an entity.
        /// </summary>
        /// <param name="inData">The content to create in the CDS.</param>
        /// <returns>Returns the CDS status</returns>
        public static CDSResponse CDSCreate(this IXimuraSessionRQ SessionRQ, Content inData)
        {
            return SessionRQ.CDSExecute(CDSData.Get(CDSAction.Create), inData);
        }
        #endregion // Create(Content inData)
    }
}

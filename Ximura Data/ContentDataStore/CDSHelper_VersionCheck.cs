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
using Ximura.Data;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{    /// <summary>
    /// This static class allows for dynamic updates to the CDS persistence service.
    /// </summary>
    public static partial class CDSHelper
    {
        /// <summary>
        /// This method initializes the entity from the persistence store.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="svc">The persistence service.</param>
        /// <returns>Returns the entity.</returns>
        public static CDSResponse CDSVersionCheck<T>(this IXimuraSessionRQ SessionRQ, string refType, string refValue, 
            out Guid? cid, out Guid? vid) where T : Content
        {
            return SessionRQ.CDSExecute<T>(CDSData.Get(CDSAction.VersionCheck, refType, refValue), out cid, out vid);
        }

        public static CDSResponse CDSVersionCheck<T>(this IXimuraSessionRQ SessionRQ, Guid? CID, Guid? VID, 
            out Guid? cid, out Guid? vid) where T : Content
        {
            return SessionRQ.CDSExecute<T>(CDSData.Get(CDSAction.VersionCheck, CID, VID), out cid, out vid);
        }

        public static CDSResponse CDSVersionCheck(this IXimuraSessionRQ SessionRQ, Type objectType, string refType, string refValue, 
            out Guid? cid, out Guid? vid)
        {
            return SessionRQ.CDSExecute(objectType, CDSData.Get(CDSAction.VersionCheck, refType, refValue), out cid, out vid);
        }

        public static CDSResponse CDSVersionCheck(this IXimuraSessionRQ SessionRQ, Type objectType, Guid? CID, Guid? VID, 
            out Guid? cid, out Guid? vid)
        {
            return SessionRQ.CDSExecute(objectType, CDSData.Get(CDSAction.VersionCheck, CID, VID), out cid, out vid);
        }
    }
}

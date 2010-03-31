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
        public static CDSResponse CDSRead<T>(this IXimuraSessionRQ SessionRQ, string refType, string refValue, out T data) where T : Content
        {
            return SessionRQ.CDSExecute<T>(CDSData.Get(CDSAction.Read, refType, refValue), out data);
        }

        public static CDSResponse CDSRead<T>(this IXimuraSessionRQ SessionRQ, Guid? CID, Guid? VID, out T data) where T : Content
        {
            return SessionRQ.CDSExecute<T>(CDSData.Get(CDSAction.Read, CID, VID), out data);
        }

        public static CDSResponse CDSRead(this IXimuraSessionRQ SessionRQ, Type outType, string refType, string refValue, out Content data)
        {
            return SessionRQ.CDSExecute(outType, CDSData.Get(CDSAction.Read, refType, refValue), out data);
        }

        public static CDSResponse CDSRead(this IXimuraSessionRQ SessionRQ, Type outType, Guid? CID, Guid? VID, out Content data)
        {
            return SessionRQ.CDSExecute(outType, CDSData.Get(CDSAction.Read, CID, VID), out data);
        }

    }
}

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
        /// <summary>
        /// This method initializes the entity from the persistence store.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="svc">The persistence service.</param>
        /// <returns>Returns the entity.</returns>
        public static CDSResponse CDSVersionCheck<E>(this IXimuraSessionRQ SessionRQ) where E : Content
        {
            //CDSContext pc = PersistenceContext.Initialize(typeof(E));
            //svc.Execute(typeof(E));

            //return (E)pc.ResponseData;
            return CDSResponse.NotImplemented;
        }
        //#region VersionCheck/VersionCheck<T>
        //public static CDSResponse VersionCheck<T>(string refType, string refValue, out Guid? cid, out Guid? vid) where T : Content
        //{
        //    return TranslateResponseCode(Execute<T>(CDSData.Get(CDSStateAction.VersionCheck, refType, refValue), out cid, out vid));
        //}

        //public static CDSResponse VersionCheck<T>(Guid? CID, Guid? VID, out Guid? cid, out Guid? vid) where T : Content
        //{
        //    return TranslateResponseCode(Execute<T>(CDSData.Get(CDSStateAction.VersionCheck, CID, VID), out cid, out vid));
        //}

        //public static CDSResponse VersionCheck(Type objectType, string refType, string refValue, out Guid? cid, out Guid? vid)
        //{
        //    return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSStateAction.VersionCheck, refType, refValue), out cid, out vid));
        //}

        //public static CDSResponse VersionCheck(Type objectType, Guid? CID, Guid? VID, out Guid? cid, out Guid? vid)
        //{
        //    return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSStateAction.VersionCheck, CID, VID), out cid, out vid));
        //}
        //#endregion
    }
}

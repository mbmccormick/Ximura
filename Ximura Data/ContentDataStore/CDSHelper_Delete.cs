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
        public static CDSResponse CDSDelete<E>(this IXimuraSessionRQ SessionRQ) where E : Content
        {
            //CDSContext pc = PersistenceContext.Initialize(typeof(E));
            //svc.Execute(typeof(E));

            //return (E)pc.ResponseData;
            return CDSResponse.NotImplemented;
        }

        //#region Delete/Delete<T>

        //public static CDSResponse Delete<T>(string refType, string refValue) where T : Content
        //{
        //    return TranslateResponseCode(Execute<T>(CDSData.Get(CDSStateAction.Delete, refType, refValue)));
        //}

        //public static CDSResponse Delete<T>(Guid? CID, Guid? VID) where T : Content
        //{
        //    return TranslateResponseCode(Execute<T>(CDSData.Get(CDSStateAction.Delete, CID, VID)));
        //}

        //public static CDSResponse Delete(Type objectType, string refType, string refValue)
        //{
        //    return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSStateAction.Delete, refType, refValue)));
        //}

        //public static CDSResponse Delete(Type objectType, Guid? CID, Guid? VID)
        //{
        //    return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSStateAction.Delete, CID, VID)));
        //}

        //#endregion
    }
}

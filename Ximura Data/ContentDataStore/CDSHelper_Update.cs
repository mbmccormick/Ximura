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
        public static CDSResponse CDSUpdate<E>(this IXimuraSessionRQ SessionRQ) where E : Content
        {
            //CDSContext pc = PersistenceContext.Initialize(typeof(E));
            //svc.Execute(typeof(E));

            //return (E)pc.ResponseData;
            return CDSResponse.NotImplemented;
        }
        //#region Update/Update<T>
        ///// <summary>
        ///// This is a shortcut method for creating an entity.
        ///// </summary>
        ///// <param name="inData">The content to create in the CDS.</param>
        ///// <returns>Returns the CDS status</returns>
        //public static CDSResponse Update(Content inData, out Content outData)
        //{
        //    return TranslateResponseCode(Execute(inData.GetType(), CDSData.Get(CDSStateAction.Update), inData, out outData));
        //}

        //public static CDSResponse Update<T>(T inData, out T outData) where T : Content
        //{
        //    return TranslateResponseCode(Execute(CDSData.Get(CDSStateAction.Update), inData, out outData));
        //}

        //public static CDSResponse Update(Content inData)
        //{
        //    return TranslateResponseCode(Execute(inData.GetType(), CDSData.Get(CDSStateAction.Update), inData));
        //}

        //public static CDSResponse Update<T>(T inData) where T : Content
        //{
        //    return TranslateResponseCode(Execute(CDSData.Get(CDSStateAction.Update), inData));
        //}
        //#endregion
    }
}

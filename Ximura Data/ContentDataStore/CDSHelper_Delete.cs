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
        #region Delete/Delete<T>

        public static CDSResponse CDSDelete<T>(this IXimuraSessionRQ SessionRQ, string refType, string refValue) where T : Content
        {
            return SessionRQ.CDSExecute<T>(CDSData.Get(CDSAction.Delete, refType, refValue));
        }

        public static CDSResponse CDSDelete<T>(this IXimuraSessionRQ SessionRQ, Guid? CID, Guid? VID) where T : Content
        {
            return SessionRQ.CDSExecute<T>(CDSData.Get(CDSAction.Delete, CID, VID));
        }

        public static CDSResponse CDSDelete(this IXimuraSessionRQ SessionRQ, Type objectType, string refType, string refValue)
        {
            return SessionRQ.CDSExecute(objectType, CDSData.Get(CDSAction.Delete, refType, refValue));
        }

        public static CDSResponse CDSDelete(this IXimuraSessionRQ SessionRQ, Type objectType, Guid? CID, Guid? VID)
        {
            return SessionRQ.CDSExecute(objectType, CDSData.Get(CDSAction.Delete, CID, VID));
        }

        #endregion
    }
}

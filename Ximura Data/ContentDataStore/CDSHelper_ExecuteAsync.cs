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
{
    /// <summary>
    /// This static class allows for dynamic updates to the CDS persistence service.
    /// </summary>
    public static partial class CDSHelper
    {
        #region ExecuteAsync(IXimuraSessionRQ SessionRQ, CDSData rq, CDSCallback rsCallback, object state)
        public static void ExecuteAsync(this IXimuraSessionRQ SessionRQ, CDSData rq, CDSCallback rsCallback, object state)
        {
            //RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;

            //try
            //{
            //    rqEnv = EnvelopeRequest(scContentDataStore.Read);
            //    //Get the request
            //    CDSRequestFolder rq = rqEnv.ContractRequest;
            //    //Set the value type
            //    rq.DataType = itemType;

            //    switch (idtype)
            //    {
            //        case IDType.ContentID:
            //            //Add the ID if it is set.
            //            if (itemID != Guid.Empty)
            //                rq.DataContentID = itemID;
            //            break;
            //        case IDType.VersionID:
            //            if (itemID != Guid.Empty)
            //                rq.DataVersionID = itemID;
            //            break;
            //        default:
            //            throw new NotSupportedException("TypeID is not supported.");
            //    }

            //    if (priority == JobPriority.NotSet)
            //        SessionRQ.ProcessRequest(rqEnv);
            //    else
            //        SessionRQ.ProcessRequest(rqEnv, priority);

            //    CDSResponseFolder rs = rqEnv.ContractResponse;

            //    status = rs.Status;

            //    return rs.Data as DataContent;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    EnvelopeReturn(rqEnv);
            //}
        }
        #endregion
        #region ExecuteAsync<T>(IXimuraSessionRQ SessionRQ,CDSData rq, CDSCallback<T> rsCallback, object state)
        public static void ExecuteAsync<T>(this IXimuraSessionRQ SessionRQ, CDSData rq, CDSCallback<T> rsCallback, object state)
            where T : Content
        {
            //RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;

            //try
            //{
            //    rqEnv = EnvelopeRequest(scContentDataStore.Read);
            //    //Get the request
            //    CDSRequestFolder rq = rqEnv.ContractRequest;
            //    //Set the value type
            //    rq.DataType = itemType;

            //    switch (idtype)
            //    {
            //        case IDType.ContentID:
            //            //Add the ID if it is set.
            //            if (itemID != Guid.Empty)
            //                rq.DataContentID = itemID;
            //            break;
            //        case IDType.VersionID:
            //            if (itemID != Guid.Empty)
            //                rq.DataVersionID = itemID;
            //            break;
            //        default:
            //            throw new NotSupportedException("TypeID is not supported.");
            //    }

            //    if (priority == JobPriority.NotSet)
            //        SessionRQ.ProcessRequest(rqEnv);
            //    else
            //        SessionRQ.ProcessRequest(rqEnv, priority);

            //    CDSResponseFolder rs = rqEnv.ContractResponse;

            //    status = rs.Status;

            //    return rs.Data as DataContent;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    EnvelopeReturn(rqEnv);
            //}
        }
        #endregion
    }
}

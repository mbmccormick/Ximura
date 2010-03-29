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
{   
    /// <summary>
    /// This static class allows for dynamic updates to the CDS persistence service.
    /// </summary>
    public static partial class CDSHelper
    {
        #region Execute(this IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        public static string Execute(this IXimuraSessionRQ SessionRQ, Type itemType,
            CDSData rqH, out Guid? cid, out Guid? vid)
        {
            cid = null;
            vid = null;
            RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;
            Content rsData = null;
            try
            {
                rqEnv = EnvelopeRequest(
                    new EnvelopeAddress(CDSCommandID, rqH.Action));
                //Get the request
                CDSRequestFolder rq = rqEnv.ContractRequest;
                //Set the value type
                rq.DataType = itemType;
                rq.ByReference = rqH.ByReference;
                rq.DataReferenceType = rqH.RefType;
                rq.DataReferenceValue = rqH.RefValue;

                if (rqH.IDContent.HasValue)
                    rq.DataContentID = rqH.IDContent.Value;
                if (rqH.IDVersion.HasValue)
                    rq.DataVersionID = rqH.IDVersion.Value;

                rq.Data = null;

                SessionRQ.ProcessRequest(rqEnv, rqH.Priority);

                CDSResponseFolder rs = rqEnv.ContractResponse;
                if (rqEnv.ContractRequest.InternalCall &&
                    rqH.Action == CDSAction.ResolveReference)
                {
                    cid = rq.DataContentID;
                    vid = rq.DataVersionID;
                }
                else
                {
                    cid = rs.CurrentContentID;
                    vid = rs.CurrentVersionID;
                }

                rsData = rs.Data;

                return rs.Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rsData != null && rsData.ObjectPoolCanReturn)
                    rsData.ObjectPoolReturn();
                rqH.ObjectPoolReturn();
                EnvelopeReturn(rqEnv);
            }
        }
        #endregion
        #region Execute(this IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        public static string Execute(this IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        {
            return Execute(SessionRQ, itemType, rqH, null, out outData);
        }
        #endregion // Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        #region Execute(this IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, Content inData, out Content outData)
        public static string Execute(this IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, Content inData, out Content outData)
        {
            RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;

            try
            {
                rqEnv = EnvelopeRequest(
                    new EnvelopeAddress(CDSCommandID, rqH.Action));
                //Get the request
                CDSRequestFolder rq = rqEnv.ContractRequest;
                //Set the value type
                rq.DataType = itemType;
                rq.ByReference = rqH.ByReference;
                rq.DataReferenceType = rqH.RefType;
                rq.DataReferenceValue = rqH.RefValue;

                if (rqH.IDContent.HasValue)
                    rq.DataContentID = rqH.IDContent.Value;
                if (rqH.IDVersion.HasValue)
                    rq.DataVersionID = rqH.IDVersion.Value;

                rq.Data = inData;

                SessionRQ.ProcessRequest(rqEnv, rqH.Priority);

                CDSResponseFolder rs = rqEnv.ContractResponse;

                outData = rs.Data;

                return rs.Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rqH.ObjectPoolReturn();
                EnvelopeReturn(rqEnv);
            }
        }
        #endregion // Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, Content inData, out Content outData)

        #region Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, out T outData)
        public static string Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, out T outData)
            where T : Content
        {
            return Execute<T>(SessionRQ, rqH, null, out outData);
        }
        #endregion
        #region Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, T inData)
        public static string Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, T inData)
            where T : Content
        {
            Content intContent = null;
            try
            {
                return Execute(SessionRQ, typeof(T), rqH, inData, out intContent);
            }
            finally
            {
                if (intContent != null && intContent.ObjectPoolCanReturn)
                    intContent.ObjectPoolReturn();
            }
        }
        #endregion // Execute<T>(IXimuraSessionRQ SessionRQ,
        #region Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, T inData, out T outData)
        public static string Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, T inData, out T outData)
            where T : Content
        {
            Content intContent;
            string status = Execute(SessionRQ, typeof(T), rqH, inData, out intContent);
            outData = intContent as T;
            return status;
        }
        #endregion

        #region Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, out Guid? cid, out Guid? vid)
        public static string Execute<T>(this IXimuraSessionRQ SessionRQ, CDSData rqH, out Guid? cid, out Guid? vid)
            where T : Content
        {
            return Execute(SessionRQ, typeof(T), rqH, out cid, out vid);
        }
        #endregion
    }
}

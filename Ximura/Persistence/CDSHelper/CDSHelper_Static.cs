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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Server;
using Ximura.Command;


#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// This static class is used to simplify access to the Content Data Store
    /// </summary>
    public partial class CDSHelper
    {
        private struct CDSJobHolder
        {
            public object State;
        }

        #region Declarations
        /// <summary>
        /// The job tracker.
        /// </summary>
        private static Dictionary<Guid, CDSJobHolder> mAsyncJobTracker;
        /// <summary>
        /// The Version Check callback.
        /// </summary>
        private static CommandRSCallback cbVersionCheck;
        #endregion

        #region Constructor - Static
        static CDSHelper()
        {
            mAsyncJobTracker = new Dictionary<Guid, CDSJobHolder>();

            cbVersionCheck = new CommandRSCallback(AsyncCDSCallback);
        }
        #endregion // Constructor - Static

        #region Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        public static string Execute(IXimuraSessionRQ SessionRQ, Type itemType, 
            CDSData rqH, out Guid? cid, out Guid? vid)
        {
            cid = null;
            vid = null;
            RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;
            Content rsData = null;
            try
            {
                rqEnv = EnvelopeRequest(
                    new EnvelopeAddress(scContentDataStore.CDSCommandID, rqH.Action));
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
                    rqH.Action == CDSStateAction.ResolveReference)
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
        #region Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        public static string Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        {
            return Execute(SessionRQ, itemType, rqH, null, out outData);
        }
        #endregion // Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, out Content outData)
        #region Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, Content inData, out Content outData)
        public static string Execute(IXimuraSessionRQ SessionRQ, Type itemType, CDSData rqH, Content inData, out Content outData)
        {
            RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;

            try
            {
                rqEnv = EnvelopeRequest(
                    new EnvelopeAddress(scContentDataStore.CDSCommandID,rqH.Action));
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

        #region Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, out T outData)
        public static string Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, out T outData)
            where T : Content
        {
            return Execute<T>(SessionRQ, rqH, null, out outData);
        }
        #endregion
        #region Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, T inData)
        public static string Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, T inData)
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
        #region Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, T inData, out T outData)
        public static string Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, T inData, out T outData)
            where T: Content
        {
            Content intContent;
            string status = Execute(SessionRQ, typeof(T), rqH, inData, out intContent);
            outData = intContent as T;
            return status;
        }
        #endregion

        #region Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, out Guid? cid, out Guid? vid)
        public static string Execute<T>(IXimuraSessionRQ SessionRQ, CDSData rqH, out Guid? cid, out Guid? vid)
            where T : Content
        {
            return Execute(SessionRQ, typeof(T), rqH, out cid, out vid);
        }
        #endregion 

        #region CDSAccessAsync
        public static void ExecuteAsync(IXimuraSessionRQ SessionRQ,
            CDSData rq, CDSCallback rsCallback, object state)
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
        #region CDSAccessAsync
        public static void ExecuteAsync<T>(IXimuraSessionRQ SessionRQ,
            CDSData rq, CDSCallback<T> rsCallback, object state)
            where T:Content
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

        #region AsyncCDSCallback
        /// <summary>
        /// This is the Read call back.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void AsyncCDSCallback(object sender, CommandRSEventArgs args)
        {
            try
            {

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EnvelopeReturn(args);
            }
        }
        #endregion
        #region EnvelopeRequest/EnvelopeReturn
        /// <summary>
        /// This methos is used to get an envelope for the request from the pool.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private static RQRSContract<CDSRequestFolder, CDSResponseFolder> EnvelopeRequest(EnvelopeAddress addr)
        {
            return RQRSEnvelopeHelper.Get(addr) as RQRSContract<CDSRequestFolder, CDSResponseFolder>;
        }
        /// <summary>
        /// This method is used by sync methods to return the envelope back to the pool.
        /// </summary>
        /// <param name="env"></param>
        private static void EnvelopeReturn(IXimuraRQRSEnvelope env)
        {
            if (env != null && env.ObjectPoolCanReturn)
                env.ObjectPoolReturn();
        }
        /// <summary>
        /// This method is used by async callbacks to return the envelope back to the pool.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        private static void EnvelopeReturn(CommandRSEventArgs args)
        {
            if (args == null || args.Data == null)
                return;

            EnvelopeReturn(args.Data);
            args.Reset();
        }
        #endregion // EnvelopeRequest/EnvelopeReturn

        #region Create(Content inData)
        ///// <summary>
        ///// This is a shortcut method for creating an entity.
        ///// </summary>
        ///// <param name="inData">The content to create in the CDS.</param>
        ///// <returns>Returns the CDS status</returns>
        //public static CDSResponse Create(IXimuraSessionRQ SessionRQ, Content inData)
        //{
        //    return TranslateResponseCode(Execute(SessionRQ, inData.GetType(), CDSData.Get(CDSStateAction.Create), inData));
        //}
        //#endregion // Create(Content inData)
        //#region Read<T>

        //public static CDSResponse Read<T>(string refType, string refValue, out T data) where T : Content
        //{
        //    return TranslateResponseCode(Execute<T>(CDSData.Get(CDSStateAction.Read, refType, refValue), out data));
        //}

        //public static CDSResponse Read<T>(Guid? CID, Guid? VID, out T data) where T : Content
        //{
        //    return TranslateResponseCode(Execute<T>(CDSData.Get(CDSStateAction.Read, CID, VID), out data));
        //}

        //#endregion // Create(Content inData)
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
        //#endregion // Create(Content inData)
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

        //#endregion // Create(Content inData)VersionCheck
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
        #endregion // VersionCheck

        #region Browse<T>
        public static BrowseContext<T> Browse<T>(IXimuraSessionRQ SessionRQ) where T : Content
        {
            return Browse<T>(SessionRQ, CDSBrowseConstraints.Relational);
        }

        public static BrowseContext<T> Browse<T>(IXimuraSessionRQ SessionRQ, CDSBrowseConstraints constraints) where T : Content
        {
            return new BrowseContext<T>(SessionRQ, constraints);
        }
        #endregion // Browse<T>

    }
}

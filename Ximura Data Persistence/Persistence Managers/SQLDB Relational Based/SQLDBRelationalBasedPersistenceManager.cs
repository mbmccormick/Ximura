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
﻿#region using
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;

using CH = Ximura.Helper.Common;
using AH = Ximura.Helper.AttributeHelper;
#endregion
namespace Ximura.Data
{
    public class SQLDBRelationalBasedPersistenceManager<CONT, DCONT> : SQLDBPersistenceManager<CONT, DCONT>
        where CONT : DCONT
        where DCONT : Content
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SQLDBRelationalBasedPersistenceManager()
            : this((IContainer)null)
        {

        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container"></param>
        public SQLDBRelationalBasedPersistenceManager(IContainer container)
            : base(container)
        {
        }
        #endregion

        protected virtual bool ResolveID(CDSContext context, Guid tid, Guid cid, Guid vid, out Guid? newVid)
        {
            throw new NotImplementedException("ResolveIDs is not implemented.");
        }

        protected virtual bool ResolveID(CDSContext context, Guid tid, string refType, string refValue, out Guid? cid, out Guid? vid)
        {
            throw new NotImplementedException("ResolveIDs is not implemented.");
        }

        protected virtual int ReadData(CDSContext context, Guid tid, Guid? cid, Guid? vid, CONT data)
        {
            throw new NotImplementedException("ReadData is not implemented.");
        }

        #region ResolveReference(CDSContext context)
        /// <summary>
        /// This method resolves the reference.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ResolveReference(CDSContext context)
        {
            try
            {
                string refType = context.Request.DataReferenceType;
                string refValue = context.Request.DataReferenceValue;
                Guid tid = Content.GetContentTypeAttributeID(context.Request.DataType);
                Guid? cid, vid;

                if (!ResolveID(context, tid, refType, refValue, out cid, out vid))
                    return false;

                context.Request.DataContentID = cid;
                context.Request.DataVersionID = vid;
                context.Response.Status = CH.HTTPCodes.Continue_100;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion // ResolveReference(CDSContext context)

        #region VersionCheck(CDSContext context)
        /// <summary>
        /// This method checks the data store to see whether the references to the entity are current.
        /// </summary>
        /// <param name="context">The current CDS context.</param>
        /// <returns>
        /// The status codes for the response are as follows:
        ///     200 = OK, the contentID and versionID are correct.
        ///     400 = missing parameter, either the contentID or versionID is null
        ///     404 = the content ID was not found
        ///     412 = the version ID is not the current version.
        ///     500 = there has been an internal system error. check the SubStatus parameter for the exception description.
        /// </returns>
        protected override bool VersionCheck(CDSContext context)
        {
            try
            {
                Guid? cid, vid;
                Guid tid = Content.GetContentTypeAttributeID(context.Request.DataType);

                cid = context.Request.DataContentID;
                vid = context.Request.DataVersionID;

                if (!cid.HasValue || !vid.HasValue)
                {
                    context.Response.Status = CH.HTTPCodes.BadRequest_400;
                    return true;
                }

                Guid? newVid;
                if (!ResolveID(context, tid, cid.Value, vid.Value, out newVid))
                {
                    context.Response.Status = CH.HTTPCodes.NotFound_404;
                    return true;
                }

                context.Response.CurrentContentID = cid;
                context.Response.CurrentVersionID = newVid;

                if (!newVid.HasValue || newVid.Value != vid.Value)
                {
                    context.Response.Status = CH.HTTPCodes.PreconditionFailed_412;
                    return true;
                }

                context.Response.Status = CH.HTTPCodes.OK_200;
            }
            catch (Exception ex)
            {
                context.Response.CurrentVersionID = null;
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
            }

            return true;
        }
        #endregion // VersionCheck(CDSContext context)

        #region Read(CDSContext context)
        /// <summary>
        /// This method reads the data from the datastore.
        /// </summary>
        /// <param name="context">The CDS context.</param>
        /// <returns>
        /// The status codes for the response are as follows:
        ///     200 = OK, the contentID and versionID are correct.
        ///     400 = missing parameter, the contentID is null
        ///     404 = the content ID was not found
        ///     500 = there has been an internal system error. check the SubStatus parameter for the exception description.
        /// </returns>
        protected override bool Read(CDSContext context)
        {
            Guid? cid, vid;
            Guid tid = Content.GetContentTypeAttributeID(context.Request.DataType);
            cid = context.Request.DataContentID;
            vid = context.Request.DataVersionID;

            if (!cid.HasValue)
            {
                context.Response.Status = CH.HTTPCodes.BadRequest_400;
                return true;
            }

            CONT data = null;
            int response = 100;
            try
            {
                data = context.GetObject<CONT>();

                response = ReadData(context, tid, cid, vid, data);
            }
            catch (Exception ex)
            {
                response = 500;
                context.Response.Substatus = ex.Message;
            }
            finally
            {
                context.Response.Status = response.ToString();

                if (response == 200)
                {
                    context.Response.Data = data;
                }
                else
                {
                    context.Response.Data = null;
                    if (data != null && data.ObjectPoolCanReturn)
                        data.ObjectPoolReturn();
                }
            }

            return true;
        }
        #endregion // Read(CDSContext context)
    }
}

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

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This Persistence manager can be used to convert an entity to another type.
    /// </summary>
    /// <typeparam name="CONT">The base entity for the Persistence Manager.</typeparam>
    /// <typeparam name="CNVR">The entity that is being converted.</typeparam>
    public class ConverterPMCDSState<CONT, CNVR, CONF> : PersistenceManagerCDSState<CONT, CONT, CONF>
        where CONT : Content
        where CNVR : Content
        where CONF : CommandConfiguration, new()
    {
        #region Declarations

        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ConverterPMCDSState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ConverterPMCDSState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region Read(CDSContext context)
        /// <summary>
        /// Reads and converts a ListItem in to an EntryList object.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Always returns true</returns>
        protected override bool Read(CDSContext context)
        {
            CNVR original = null;
            CONT newContent = null;

            try
            {
                CDSResponse response = context.CDSHelperDirect.Read<CNVR>(
                    context.Request.DataContentID, context.Request.DataVersionID, out original);

                switch (response)
                {
                    case CDSResponse.OK:
                        break;
                    default:
                        context.Response.Data = null;
                        context.Response.CurrentContentID = null;
                        context.Response.CurrentVersionID = null;
                        context.Response.Status = ((int)response).ToString();
                        return true;
                }

                newContent = context.GetObject<CONT>();

                ReadConvert(context, newContent, original);

                context.Response.Data = newContent;
                context.Response.Status = CH.HTTPCodes.OK_200;
            }
            catch (Exception ex)
            {
                if (newContent != null && newContent.ObjectPoolCanReturn)
                    newContent.ObjectPoolReturn();

                context.Response.Data = null;
                context.Response.CurrentContentID = null;
                context.Response.CurrentVersionID = null;

                if (ex is CDSStateException)
                {
                    context.Response.Status = ((CDSStateException)ex).ResponseCode.ToString();
                    context.Response.Substatus = ((CDSStateException)ex).Message;
                }
                else
                    context.Response.Status = CH.HTTPCodes.InternalServerError_500;
            }
            finally
            {
                if (original != null && original.ObjectPoolCanReturn)
                    original.ObjectPoolReturn();
            }

            return true;
        }
        #endregion // Read(CDSContext context)
        #region ResolveReference(CDSContext context)
        /// <summary>
        /// This is a passthru method to the ListItem ResolveReference method.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Always returns true.</returns>
        protected override bool ResolveReference(CDSContext context)
        {
            Guid? cid, vid;
            CDSResponse response = context.CDSHelperDirect.ResolveReference<CNVR>(
                context.Request.DataReferenceType, context.Request.DataReferenceValue, out cid, out vid);

            context.Request.DataContentID = cid;
            context.Request.DataVersionID = vid;
            context.Response.Status = ((int)response).ToString();

            return true;
        }
        #endregion // ResolveReference(CDSContext context)
        #region VersionCheck(CDSContext context)
        /// <summary>
        /// This is a passthru method to the ListItem VersionCheck method.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Always returns true.</returns>
        protected override bool VersionCheck(CDSContext context)
        {
            Guid? cid, vid;
            CDSResponse response = context.CDSHelperDirect.VersionCheck<CNVR>(
                context.Request.DataContentID, context.Request.DataVersionID, out cid, out vid);

            context.Response.CurrentContentID = cid;
            context.Response.CurrentVersionID = vid;
            context.Response.Status = ((int)response).ToString();

            return true;
        }
        #endregion // VersionCheck(CDSContext context)

        #region ReadConvert(CDSContext context, CONT newContent, CONV original)
        /// <summary>
        /// This method loads the newContent with the original content.
        /// To throw a custom error with a status code and description, throw a CDSStateException.
        /// </summary>
        /// <param name="context">The current request context.</param>
        /// <param name="newContent">The new content to load.</param>
        /// <param name="original">The original content to read from.</param>
        /// <exception cref="System.NotImplementedException">This method throws a NotImplementedException exception. 
        /// Override this method with your conversion logic.</exception>
        protected virtual void ReadConvert(CDSContext context, CONT newContent, CNVR original)
        {
            throw new NotImplementedException("ReadConvert is not implemented.");
        }
        #endregion // ReadConvert(CDSContext context, CONT newContent, CONV original)

    }
}

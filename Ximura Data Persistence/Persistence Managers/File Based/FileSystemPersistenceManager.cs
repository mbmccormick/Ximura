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
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    [CDSStateActionPermit(CDSStateAction.Create)]
    [CDSStateActionPermit(CDSStateAction.Read)]
    [CDSStateActionPermit(CDSStateAction.Update)]
    [CDSStateActionPermit(CDSStateAction.Delete)]
    [CDSStateActionPermit(CDSStateAction.VersionCheck)]
    [CDSStateActionPermit(CDSStateAction.ResolveReference)]
    [CDSStateActionPermit(CDSStateAction.Browse)]
    public class FileSystemPersistenceManager<CONT, DCONT, CONF> : FileSystemBasePMCDSState<CONT, DCONT, CONF>
        where CONT : Content, DCONT
        where DCONT : Content
        where CONF : CommandConfiguration, new()
    {
        private Dictionary<ContentIdentifier, byte[]> mLookUpTable;

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public FileSystemPersistenceManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public FileSystemPersistenceManager(IContainer container)
            : base(container)
        {
            mLookUpTable = new Dictionary<ContentIdentifier, byte[]>();
        }
        #endregion // Constructors

        protected override bool Read(CDSContext context)
        {
            if (!DirectoryScan(context).Exists)
            {
                context.Response.Status = CH.HTTPCodes.NotFound_404;
                return false;
            }
            if (ReferenceTypeNotValid(context.Request.DataReferenceType))
            {
                context.Response.Status = CH.HTTPCodes.NotFound_404;
                return false;
            }
            FileInfo[] fi =
                DirectoryScan(context).GetFiles(
                    NameFormat(context.Request.DataReferenceValue), SearchOption.TopDirectoryOnly);


            if (fi.Length == 0)
            {
                context.Response.Status = CH.HTTPCodes.NotFound_404;
                return false;
            }

            CONT data = context.GetObjectPool<CONT>().Get();

            try
            {
                using (FileStream strmFile = fi[0].OpenRead())
                {
                    data.Load(strmFile);
                }
            }
            catch (Exception ex)
            {
                data.ObjectPoolReturn();
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                return false;
            }

            context.Response.Data = data;
            context.Response.Status = CH.HTTPCodes.OK_200;
            return true;
        }

        protected override bool ResolveReference(CDSContext context)
        {
            if (!DirectoryScan(context).Exists)
                return false;

            if (ReferenceTypeNotValid(context.Request.DataReferenceType))
                return false;

            FileInfo[] fi =
                DirectoryScan(context).GetFiles(
                    NameFormat(context.Request.DataReferenceValue), SearchOption.TopDirectoryOnly);

            return fi.Length > 0;
        }

        protected virtual bool ReferenceTypeNotValid(string refType)
        {
            return refType.ToLower() != "name";
        }

        protected virtual string NameFormat(string refValue)
        {
            return refValue + ".xml";
        }

        protected override bool VersionCheck(CDSContext context)
        {
            return false;
        }

        protected virtual DirectoryInfo DirectoryScan(CDSContext context)
        {
            throw new NotImplementedException("DirectoryScan is not implemented.");
        }
    }
}

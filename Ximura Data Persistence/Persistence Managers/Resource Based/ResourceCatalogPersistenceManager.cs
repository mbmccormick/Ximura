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

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This state allows embedded resources to be acessed using the Persistence Management architecture.
    /// You should override the ResourceCatalog property to return the location of the resource catalog binary.
    /// Alternatively, you can override ResourceStream and returns a stream containing the binary data for the catalog.
    /// </summary>
    public class ResourceCatalogPersistenceManager<CONT, DCONT, CONF> : ResourceBasePersistenceManager<CONT, DCONT, CONF>
        where CONT : DCONT
        where DCONT : Content
        where CONF : CommandConfiguration, new()
    {
        #region Declarations
        /// <summary>
        /// This collection holds the data for the specific domain datasets.
        /// </summary>
        protected Dictionary<int, byte[]> mDomainData;

        //private object syncCatalog = new object();

        private ResourceCatalog mCatalog = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResourceCatalogPersistenceManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResourceCatalogPersistenceManager(IContainer container)
            : base(container)
        {
            mDomainData = new Dictionary<int, byte[]>();
            ResourceCatalogInitialize();
        }
        #endregion // Constructors

        #region ResourceCatalogInitialize()
        /// <summary>
        /// This method initializes the catalog for the Resource based Persistence Manager.
        /// </summary>
        protected virtual void ResourceCatalogInitialize()
        {
            mCatalog = new ResourceCatalog();

            using (Stream io = ResourceCatalogStream)
            {
                mCatalog.Load(io);
            }
        }
        #endregion // InitializeCatalog()
        #region ResourceCatalogStream
        /// <summary>
        /// This is the resource stream.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Stream ResourceCatalogStream
        {
            get
            {
                byte[] blob = RH.ResourceLoadFromUri(ResourceCatalogLocation);
                return new MemoryStream(blob);
            }
        }
        #endregion // ResourceStream
        #region ResourceCatalogLocation --> MUST OVERRIDE
        /// <summary>
        /// This method returns the base resource location Uri.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Uri ResourceCatalogLocation
        {
            get
            {
                throw new NotImplementedException("ResourceBasedPMCDSState->ResourceCatalog is not implemented.");
            }
        }
        #endregion // ResourceBase

        #region Catalog
        /// <summary>
        /// This is the resource catalog.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual ResourceCatalog Catalog
        {
            get
            {
                return mCatalog;
            }
        }
        #endregion // Catalog

        #region Read(CDSContext context)
        /// <summary>
        /// This method is called when requesting the controller script.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool Read(CDSContext context)
        {
            string id = null;
            Guid vid;
            bool success = false;

            try
            {
                if (context.Request.DataVersionID.HasValue)
                    success = Catalog.ResolveObject(
                        context.Request.DataVersionID.Value, true, out id, out vid);

                if (!success && context.Request.DataContentID.HasValue)
                    success = Catalog.ResolveObject(
                        context.Request.DataContentID.Value, false, out id, out vid);

                if (!success)
                    return false;

                CONT cScript = context.GetObjectPool<CONT>().Get();

                using (Stream schemaStream = new MemoryStream(RH.ResourceLoadFromUri(new Uri(id))))
                {
                    cScript.Load(schemaStream);
                }
                context.Response.Data = cScript;
                context.Response.Status = CH.HTTPCodes.OK_200;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        #endregion // Read(CDSContext context)
        #region VersionCheck(CDSContext context)
        /// <summary>
        /// This method resolves the resource reference to its constituent IDs
        /// </summary>
        /// <param name="context">The current CDS context.</param>
        /// <returns>Returns true if the resource was successfully resolved.</returns>
        protected override bool VersionCheck(CDSContext context)
        {
            string id = null;
            Guid vid = Guid.Empty;
            bool success = false;

            if (context.Request.DataVersionID.HasValue)
                success = Catalog.ResolveObject(
                    context.Request.DataVersionID.Value, true,
                    out id, out vid);

            if (!success && context.Request.DataContentID.HasValue)
            {
                success = Catalog.ResolveObject(
                    context.Request.DataContentID.Value, false,
                    out id, out vid);
            }

            if (!success)
            {
                context.Response.Status = CH.HTTPCodes.NotFound_404;
                return false;
            }

            context.Response.CurrentVersionID = vid;
            context.Response.Status = CH.HTTPCodes.OK_200;
            return true;
        }
        #endregion // VersionCheck(CDSContext context)
        #region ResolveReference(CDSContext context)
        /// <summary>
        /// This method resolves the entity reference from the resource catalog.
        /// </summary>
        /// <param name="context">The current CDS context.</param>
        /// <returns>Returns true as this is the authoritize destination for resource based references.</returns>
        protected override bool ResolveReference(CDSContext context)
        {
            Guid tid, cid, vid;
            DateTime vidExpire;

            bool success = mCatalog.ResolveReference(
                context.Request.DataReferenceType, context.Request.DataReferenceValue,
                    out tid, out cid, out vid, out vidExpire);

            if (!success)
                return false;

            context.Request.DataTypeID = tid;
            context.Request.DataContentID = cid;
            context.Request.DataVersionID = vid;

            return true;
        }
        #endregion

    }
}

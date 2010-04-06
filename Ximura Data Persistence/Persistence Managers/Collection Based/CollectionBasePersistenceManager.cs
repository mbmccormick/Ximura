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
using Ximura.Framework;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This base Persistence Manager class is used for PMs that require a collection of resources to be stored in the PM.
    /// </summary>
    /// <typeparam name="CONT">The entity type.</typeparam>
    /// <typeparam name="DCONT">The entity scan type.</typeparam>
    public class CollectionBasePersistenceManager<CONT, DCONT, CONF> : PersistenceManagerCDSState<CONT, DCONT, CONF>
        where CONT : class, DCONT
        where DCONT : class, IXimuraContent
        where CONF : CommandConfiguration, new()
    {
        #region Declarations
        /// <summary>
        /// This collection holds the content collection for the persistence manager.
        /// </summary>
        protected Dictionary<ContentIdentifier, CacheItemContainer<CONT>> mDataCache;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CollectionBasePersistenceManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public CollectionBasePersistenceManager(IContainer container)
            : base(container)
        {
            mDataCache = new Dictionary<ContentIdentifier, CacheItemContainer<CONT>>();
        }
        #endregion // Constructors

        #region ProcessIdentifier(CDSContext context, out ContentIdentifier identifier)
        /// <summary>
        /// This method extracts the ids from the context.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="identifier">The output identifier.</param>
        /// <returns>Returns true if the identifier can be extracted.</returns>
        protected bool ProcessIdentifier(CDSContext context, out ContentIdentifier? identifier)
        {
            if (!context.Request.DataVersionID.HasValue ||
                !context.Request.DataContentID.HasValue ||
                context.Request.DataType == null)
            {
                identifier = null;
                return false;
            }

            //Note that we do not add the version ID to the key.
            identifier = new ContentIdentifier(
                context.Request.DataType,
                context.Request.DataContentID.Value,
                context.Request.DataVersionID.Value);
            return true;
        }
        #endregion // ProcessIdentifier(CDSContext context, out ContentIdentifier identifier)
    }
}

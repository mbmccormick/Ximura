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
//#region using
//using System;
//using System.Collections;
//using System.Collections.Generic;
//#endregion // using
//namespace Ximura.Helper
//{
//    /// <summary>
//    /// ContextMetadataContainer is used to hold the context information.
//    /// </summary>
//    public class ContextMetadataContainer<CNTX>
//        where CNTX : class, IXimuraFSMContext, new()
//    {
//        #region Declarations
//        private CNTX mKey = null;

//        /// <summary>
//        /// This is the pool that holds the collection of Contexts for the particular type.
//        /// </summary>
//        IXimuraPool<CNTX> pool = null;

//        private Dictionary<object,object> extendedProperties = null;
//        /// <summary>
//        /// This boolean property determines whether the particular state is enabled for the pool.
//        /// </summary>
//        public bool Enabled = false;
//        /// <summary>
//        /// This boolean property determines whether the settings should be taken from the application config.
//        /// </summary>
//        public bool UseConfigSettings = true;
//        /// <summary>
//        /// This is the min pool.
//        /// </summary>
//        public int PoolMin = 0;
//        /// <summary>
//        /// This is the max pool.
//        /// </summary>
//        public int PoolMax = 0;
//        /// <summary>
//        /// This is the pool prefer value.
//        /// </summary>
//        public int PoolPrefer = 0;
//        #endregion // Declarations
//        #region Constructors
//        /// <summary>
//        /// This is the default constructor.
//        /// </summary>
//        public ContextMetadataContainer()
//        {

//        }
//        /// <summary>
//        /// This is the default constructor.
//        /// </summary>
//        /// <param name="key">The context object.</param>
//        public ContextMetadataContainer(CNTX key)
//        {
//            mKey = key;
//        }
//        #endregion // Constructors

//        #region ContextKey
//        /// <summary>
//        /// This is the context.
//        /// </summary>
//        public CNTX ContextKey
//        {
//            get
//            {
//                return mKey;
//            }
//            set
//            {
//                mKey = value;
//            }
//        }
//        #endregion // ContextKey

//        #region this[object key]
//        /// <summary>
//        /// This is the public accessor for the extended properties.
//        /// </summary>
//        public object this[object key]
//        {
//            get
//            {
//                if (extendedProperties == null ||
//                    !extendedProperties.ContainsKey(key))
//                    return null;

//                return extendedProperties[key];
//            }
//            set
//            {
//                if (extendedProperties == null)
//                    extendedProperties = new Dictionary<object, object>();

//                extendedProperties[key]=value;
//            }		
//        }
//        #endregion // this[object key]
//    }
//}
//#region Copyright
//// *******************************************************************************
//// Copyright (c) 2000-2009 Paul Stancer.
//// All rights reserved. This program and the accompanying materials
//// are made available under the terms of the Eclipse Public License v1.0
//// which accompanies this distribution, and is available at
//// http://www.eclipse.org/legal/epl-v10.html
////
//// Contributors:
////     Paul Stancer - initial implementation
//// *******************************************************************************
//#endregion
//#region using
//using System;
//using System.Data;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Security;
//using System.Security.Cryptography;
//using System.Globalization;
//using System.Runtime.Serialization;

//using Ximura;
//using Ximura.Data;

//using CH = Ximura.Common;
//using RH = Ximura.Reflection;
//using AH = Ximura.AttributeHelper;
//using Ximura.Data;
//using Ximura.Framework;
//using Ximura.Framework;
//#endregion // using
//namespace Ximura.Data
//{
//    /// <summary>
//    /// The CDSData class is used by the CDSHelper to hold the information for a request.
//    /// </summary>
//    public class CDSData : CDSDataBase, IXimuraPoolReturnable
//    {
//        #region Static Declarations
//        private static readonly Guid sCDSCommandGuid = new Guid("FE21CBF6-2CDC-4549-9F13-49385CAE8DDA");
//        private static IXimuraPool<CDSData> sCDSRequestPool;
//        #endregion // Declarations
//        #region Static Constructor
//        static CDSData()
//        {
//            sCDSRequestPool = new PoolInvocator<CDSData>(delegate(){ return new CDSData(); });
//        }
//        #endregion // Static Constructor


//        #region Declarations
//        private readonly Guid mTrackID = Guid.NewGuid();
//        #endregion 

//        #region Constructor - Private
//        private CDSData()
//        {
//            Reset();
//        }
//        #endregion
//        #region IDisposable Members/Finalize
//        private bool disposed = false;
//        /// <summary>
//        /// The dispose method.
//        /// </summary>
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//        /// <summary>
//        /// The overrided dispose method
//        /// </summary>
//        /// <param name="disposing">True if this is called by dispose, false if this
//        /// is called by the finalizer.</param>
//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposed && disposing)
//            {
//                RefValue = null;
//                RefType = null;
//                disposed = true;
//            }
//        }
//        #endregion

//        #region IXimuraPoolReturnable Members
//        #region ObjectPool
//        /// <summary>
//        /// This property is not supported.
//        /// </summary>
//        public IXimuraPool ObjectPool
//        {
//            get
//            {
//                throw new NotSupportedException("Access to the internal object pool is not supported.");
//            }
//            set
//            {
//                //throw new NotSupportedException("Access to the internal object pool is not supported.");
//            }
//        }
//        #endregion // ObjectPool
//        #region ObjectPoolCanReturn
//        /// <summary>
//        /// This property will always be true as objects will always be part of a pool.
//        /// </summary>
//        public bool ObjectPoolCanReturn
//        {
//            get { return true; }
//        }
//        #endregion // ObjectPoolCanReturn
//        #region ObjectPoolReturn()
//        /// <summary>
//        /// This method returns the request to the pool for re-use.
//        /// </summary>
//        public void ObjectPoolReturn()
//        {
//            sCDSRequestPool.Return(this);
//        }
//        #endregion // ObjectPoolReturn()
//        #endregion
//        #region IXimuraPoolableObject Members
//        /// <summary>
//        /// This will always return true for this object type.
//        /// </summary>
//        public bool CanPool
//        {
//            get { return true; }
//        }
//        /// <summary>
//        /// This is the internal track ID.
//        /// </summary>
//        public Guid TrackID
//        {
//            get { return mTrackID; }
//        }
//        #endregion
//    }
//}

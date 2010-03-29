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
using Ximura.Framework;


#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This static class is used to simplify access to the Content Data Store
    /// </summary>
    public partial class CDSHelper : ICDSHelperLegacy
    {
        #region Declarations
		/// <summary>
		/// This is the session that the process command will run under.
		/// </summary>
		private IXimuraSessionRQ mSession;

		#endregion // Declarations 
		#region Constructors
        public CDSHelper(): this(null)
        {
        }
		/// <summary>
		/// This is the main constructor.
		/// </summary>
		/// <param name="session">The session object to wrap.</param>
        public CDSHelper(IXimuraSessionRQ session)
            : base()
		{
            //if (session == null)
            //    throw new ArgumentNullException("session","Session cannot be null.");
			mSession=session;
		}
		#endregion // Constructors

        #region Execute(Type contentType, CDSData rq, Content inData, out Content outData)
        public string Execute(Type contentType, CDSData rq, Content inData, out Content outData)
        {
            return Execute(mSession, contentType, rq, inData, out outData);
        }
        #endregion

        #region Execute(Type contentType, CDSData rq)
        public string Execute(Type contentType, CDSData rq)
        {
            Guid? cid, vid;
            return Execute(mSession, contentType, rq, out cid, out vid);
        }
        #endregion
        #region Execute(Type contentType, CDSData rq, Content inData)
        public string Execute(Type contentType, CDSData rq, Content inData)
        {
            Content outData = null;
            try
            {
                return Execute(mSession, contentType, rq, inData, out outData);
            }
            finally
            {
                if (outData != null && outData.ObjectPoolCanReturn)
                    outData.ObjectPoolReturn();
            }
        }
        #endregion
        #region Execute(Type contentType, CDSData rq, out Content outData)
        public string Execute(Type contentType, CDSData rq, out Content outData)
        {
            return Execute(mSession, contentType, rq, null, out outData);
        }
        #endregion
        #region Execute(Type contentType, CDSData rq, out Guid? cid, out Guid? vid)
        public string Execute(Type contentType, CDSData rq, out Guid? cid, out Guid? vid)
        {
            return Execute(mSession, contentType, rq, out cid, out vid);
        }
        #endregion

        #region Execute<T>(CDSData rq)
        public string Execute<T>(CDSData rq)
            where T : Content
        {
            Guid? cid, vid;
            return Execute<T>(mSession, rq, out cid, out vid);
        }
        #endregion
        #region Execute<T>(CDSData rq, T inData)
        public string Execute<T>(CDSData rq, T inData)
            where T : Content
        {
            T outData=null;
            try
            {
                return Execute<T>(mSession, rq, inData, out outData);
            }
            finally
            {
                if (outData != null && outData.ObjectPoolCanReturn)
                    outData.ObjectPoolReturn();
            }
        }
        #endregion
        #region Execute<T>(CDSData rq, out T outData)
        public string Execute<T>(CDSData rq, out T outData)
            where T : Content
        {
            return Execute<T>(mSession, rq, null, out outData);
        }
        #endregion
        #region Execute<T>(CDSData rq, T inData, out T outData)
        public string Execute<T>(CDSData rq, T inData, out T outData)
            where T : Content
        {
            return Execute<T>(mSession, rq, inData, out outData);
        }
        #endregion
        #region Execute<T>(CDSData rq, T inData, out Guid? newVersionID)
        public string Execute<T>(CDSData rq, T inData, out Guid? newVersionID)
            where T : Content
        {
            T outData=null;
            newVersionID = null;
            try
            {
                string status = Execute<T>(mSession, rq, inData, out outData);
                if (outData != null)
                    newVersionID = outData.IDVersion;
                return status;
            }
            finally
            {
                if (outData!=null && outData.ObjectPoolCanReturn)
                    outData.ObjectPoolReturn();
            }
        }
        #endregion
        #region Execute<T>(CDSData rq, out Guid? cid, out Guid? vid)
        public string Execute<T>(CDSData rq, out Guid? cid, out Guid? vid)
            where T : Content
        {
            return Execute<T>(mSession, rq, out cid, out vid);
        }
        #endregion

        #region Session
        /// <summary>
        /// This property is the internal session.
        /// </summary>
        public IXimuraSessionRQ Session
        {
            get { return mSession; }
            set { mSession = value; }
        }
        #endregion // Session

        #region Create(Content inData)
        /// <summary>
        /// This is a shortcut method for creating an entity.
        /// </summary>
        /// <param name="inData">The content to create in the CDS.</param>
        /// <returns>Returns the CDS status</returns>
        public CDSResponse Create(Content inData)
        {
            return TranslateResponseCode(Execute(inData.GetType(), CDSData.Get(CDSAction.Create), inData));
        }
        #endregion // Create(Content inData)
        #region Read<T>

        public CDSResponse Read<T>(string refType, string refValue, out T data) where T : Content
        {
            return TranslateResponseCode(Execute<T>(CDSData.Get(CDSAction.Read, refType, refValue), out data));
        }

        public CDSResponse Read<T>(Guid? CID, Guid? VID, out T data) where T : Content
        {
            return TranslateResponseCode(Execute<T>(CDSData.Get(CDSAction.Read, CID, VID), out data));
        }

        #endregion // Create(Content inData)
        #region Update/Update<T>
        /// <summary>
        /// This is a shortcut method for creating an entity.
        /// </summary>
        /// <param name="inData">The content to create in the CDS.</param>
        /// <returns>Returns the CDS status</returns>
        public CDSResponse Update(Content inData, out Content outData)
        {
            return TranslateResponseCode(Execute(inData.GetType(), CDSData.Get(CDSAction.Update), inData, out outData));
        }

        public CDSResponse Update<T>(T inData, out T outData) where T : Content
        {
            return TranslateResponseCode(Execute(CDSData.Get(CDSAction.Update), inData, out outData));
        }

        public CDSResponse Update(Content inData)
        {
            return TranslateResponseCode(Execute(inData.GetType(), CDSData.Get(CDSAction.Update), inData));
        }

        public CDSResponse Update<T>(T inData) where T : Content
        {
            return TranslateResponseCode(Execute(CDSData.Get(CDSAction.Update), inData));
        }
        #endregion // Create(Content inData)
        #region Delete/Delete<T>

        public CDSResponse Delete<T>(string refType, string refValue) where T : Content
        {
            return TranslateResponseCode(Execute<T>(CDSData.Get(CDSAction.Delete, refType, refValue)));
        }

        public CDSResponse Delete<T>(Guid? CID, Guid? VID) where T : Content
        {
            return TranslateResponseCode(Execute<T>(CDSData.Get(CDSAction.Delete, CID, VID)));
        }

        public CDSResponse Delete(Type objectType, string refType, string refValue)
        {
            return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSAction.Delete, refType, refValue)));
        }

        public CDSResponse Delete(Type objectType, Guid? CID, Guid? VID)
        {
            return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSAction.Delete, CID, VID)));
        }

        #endregion // Create(Content inData)VersionCheck
        #region VersionCheck/VersionCheck<T>
        public CDSResponse VersionCheck<T>(string refType, string refValue, out Guid? cid, out Guid? vid) where T : Content
        {
            return TranslateResponseCode(Execute<T>(CDSData.Get(CDSAction.VersionCheck, refType, refValue), out cid, out vid));
        }

        public CDSResponse VersionCheck<T>(Guid? CID, Guid? VID, out Guid? cid, out Guid? vid) where T : Content
        {
            return TranslateResponseCode(Execute<T>(CDSData.Get(CDSAction.VersionCheck, CID, VID), out cid, out vid));
        }

        public CDSResponse VersionCheck(Type objectType, string refType, string refValue, out Guid? cid, out Guid? vid)
        {
            return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSAction.VersionCheck, refType, refValue), out cid, out vid));
        }

        public CDSResponse VersionCheck(Type objectType, Guid? CID, Guid? VID, out Guid? cid, out Guid? vid)
        {
            return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSAction.VersionCheck, CID, VID), out cid, out vid));
        }
        #endregion // VersionCheck
        #region Browse<T>
        public BrowseContext<T> Browse<T>() where T : Content
        {
            return Browse<T>(CDSBrowseConstraints.Relational);
        }

        public BrowseContext<T> Browse<T>(CDSBrowseConstraints constraints) where T : Content
        {
            return new BrowseContext<T>(mSession, constraints);
        }
        #endregion // Browse<T>

        #region ResponseTranslate
        /// <summary>
        /// This method translates the response code in to a CDSResponse enumeration.
        /// </summary>
        /// <param name="responseCode">The string response code.</param>
        /// <returns>Returns the CDSResponse enumeration value, or Unknown if the value is not recognised.</returns>
        protected CDSResponse TranslateResponseCode(string responseCode)
        {
            if (responseCode == null || responseCode == "")
                return CDSResponse.ResponseCodeNotSet;

            int value;
            if (!int.TryParse(responseCode, out value))
                return CDSResponse.ResponseCodeUnknown;

            switch (value)
            {
                case 100:
                    return CDSResponse.Continue;
                case 200:
                    return CDSResponse.OK;
                case 400:
                    return CDSResponse.BadRequest;
                case 404:
                    return CDSResponse.NotFound;
                case 412:
                    return CDSResponse.VersionIDInvalid;
                case 500:
                    return CDSResponse.SystemError;
                case 501:
                    return CDSResponse.NotImplemented;
                case 503:
                    return CDSResponse.CDSNotStarted;
                default:
                    return CDSResponse.ResponseCodeUnknown;
            }      
        }
        #endregion
    }
}

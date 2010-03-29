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
using System.Globalization;
using System.Runtime.Serialization;
using System.ComponentModel;

using Ximura;
using Ximura.Data;

using HTTPCD = Ximura.Common.HTTPCodes;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// RQRSFolder is a composite content object that can contain multiple entity object along with numerous other
    /// named parameters.
    /// </summary>
    public class RQRSFolder : PoolableReturnableObjectBase//, IXimuraPoolableObject
    {
        #region Declarations
        private Guid mID;
        /// <summary>
        /// This is the internal HTTP status code.
        /// </summary>
        protected string mStatus = HTTPCD.Continue_100;
        /// <summary>
        /// This is the internal substatus code/description
        /// </summary>
        protected string mSubstatus = "";
        /// <summary>
        /// The culture for the request.
        /// </summary>
        protected CultureInfo mCulture = null;
        #endregion
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public RQRSFolder()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public RQRSFolder(System.ComponentModel.IContainer container)//: base(container)
        {
        }

        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public RQRSFolder(SerializationInfo info, StreamingContext context)//:base(info,context)
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This method resets the object.
        /// </summary>
        public override void Reset()
        {
            mID = Guid.Empty;
            mStatus = HTTPCD.Continue_100;
            mSubstatus = "";
            mCulture = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Status and SubStatus
        /// <summary>
        /// The Status is the HTTP status code that is used to define the object's status.
        /// </summary>
        public string Status
        {
            get { return mStatus; }
            set
            {
                mStatus = value;
            }
        }
        /// <summary>
        /// This is the response substatus. This can be used to return a substatus
        /// field or a text description.
        /// </summary>
        public string Substatus
        {
            get { return mSubstatus; }
            set
            {
                mSubstatus = value;
            }
        }
        #endregion

        #region IXimuraPoolableObject Members
        #region CanPool
        /// <summary>
        /// This method returns true if the object can be pooled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool CanPool
        {
            get { return true; }
        }
        #endregion // CanPool
        #endregion

        #region ID
        /// <summary>
        /// This is the request ID.
        /// </summary>
        public Guid ID
        {
            get { return mID; }
            set { mID = value; }
        }
        #endregion // ID
    }
}
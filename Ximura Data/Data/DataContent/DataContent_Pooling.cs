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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Data
{
    public partial class DataContent
    {
        #region Reset()
        /// <summary>
        /// This method resets the data content.
        /// </summary>
        public override void Reset()
        {
            evhOnRowChanging = null;
            evhOnRowChanged = null;
            evhOnRowDeleted = null;
            evhOnRowDeleting = null;
            DataChanged = null;
            DataChanging = null;
            ConnectToParentDataSource = null;

            mLinkType = DataContentLinkType.MergeIfPresent;
            mIsNewRecord = false;
            mSatelliteEntity = null;
            mLinkTypeIdentifier = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Dispose(bool disposing)
        private bool mDisposed = false;
        /// <summary>
        /// This is the dispose override.
        /// </summary>
        /// <param name="disposing">True when this is called by the dispose method.</param>
        protected override void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (disposing)
                {
                    evhOnRowChanging = null;
                    evhOnRowChanged = null;
                    evhOnRowDeleted = null;
                    evhOnRowDeleting = null;
                    DataChanged = null;
                    DataChanging = null;
                    ConnectToParentDataSource = null; 
                    attrsSummary = null;

                    mLinkTypeIdentifier = null;
                    mSatelliteEntity = null;
                    base.Dispose(disposing);
                }
                mDisposed = true;
            }
        }
        #endregion // Dispose(bool disposing)

    }
}

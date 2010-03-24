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
using Ximura.Framework;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Data
{
    public partial class DataContent
    {
        #region Declarations
        private DataContentLinkType mLinkType = DataContentLinkType.MergeIfPresent;
        private string mLinkTypeIdentifier = null;

        /// <summary>
        /// This boolean value identifies whether the content is chained to a parent dataset
        /// </summary>
        protected bool? mSatelliteEntity = null;
        #endregion // Declarations

        #region LinkType
        /// <summary>
        /// This is the link type.
        /// </summary>
        public DataContentLinkType LinkType
        {
            get { return mLinkType; }
            set { mLinkType = value; }
        }
        #endregion // LinkType

        #region LinkTypeIdentifier
        /// <summary>
        /// This property identifies the specific object to link to, when the LinkType property
        /// is set to Link.
        /// </summary>
        public string LinkTypeIdentifier
        {
            get
            {
                return mLinkTypeIdentifier;
            }
            set
            {
                mLinkTypeIdentifier = value;
            }
        }
        #endregion

        #region LoadFromParentEntityServer()
        /// <summary>
        /// This method is used to load the entity from the IXimuraDataEntityServer
        /// contained in the current Site object.
        /// </summary>
        /// <returns></returns>
        public virtual bool LoadFromParentEntityServer()
        {
            //DataContentEventArgs dataArgs = new DataContentEventArgs();

            //if (ConnectToParentDataSource != null)
            //{
            //    ConnectToParentDataSource(this, dataArgs as ContentEventArgs);
            //}
            //IXimuraDataEntityServer parentDataServer;

            //if (dataArgs.ParentDataServer != null)
            //    parentDataServer = dataArgs.ParentDataServer;
            //else
            //    parentDataServer = GetService(typeof(IXimuraDataEntityServer)) as IXimuraDataEntityServer;

            ////Are we satellite content, we will check this by trying our parent.
            //if (parentDataServer != null)
            //{
            //    return LoadFromParentEntityServer(parentDataServer);
            //}
            return false;
        }
        /// <summary>
        /// This method loads the entity from the parent server.
        /// </summary>
        /// <param name="parentDataServer"></param>
        /// <returns></returns>
        public virtual bool LoadFromParentEntityServer(IXimuraDataEntityServer parentDataServer)
        {
            DataSet parentDataSet = parentDataServer.getParentDataSet(this);
            if (parentDataSet == null || (mSatelliteEntity ?? false))
                return false;

            switch (this.LinkType)
            {
                case DataContentLinkType.MergeIfPresent:
                    Load(parentDataSet, false);
                    break;
                case DataContentLinkType.Link:
                    Load(parentDataSet, true);
                    break;
                default:
                    return false;
            }

            //EntityLoad(parentDataSet, true);
            mSatelliteEntity = true;
            return true;
        }
        #endregion // LoadFromParentEntityServer()

        #region SatelliteEntity property
        /// <summary>
        /// This property defines whether the content is a satellite entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SatelliteEntity
        {
            get { return mSatelliteEntity ?? false; }
        }
        #endregion // SatelliteEntity property

    }
}

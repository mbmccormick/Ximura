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
using Ximura.Server;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Data
{
    public partial class DataContent
    {
        #region Dirty
        /// <summary>
        /// This overriden method ties the dataset to the content state.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Dirty
        {
            get
            {
                if (mDataContentSet == null) return base.Dirty;

                return this.mDataContentSet.HasChanges() || base.Dirty;
            }
            set
            {
                base.Dirty = value;
            }
        }
        #endregion

        #region ID
        /// <summary>
        /// This is the DataContent ID.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Guid IDContent
        {
            get
            {
                if (GetContentAttributeID().HasValue || attrContentMappingContentID == null)
                    return base.IDContent;

                return MappingHelperGet(attrContentMappingContentID);
            }
            set
            {
                //We only allow this to be changed in the parent entity.
                if ((mSatelliteEntity ?? false) || GetContentAttributeID().HasValue)
                    return;

                if (attrContentMappingContentID != null)
                {
                    MappingHelperSet(attrContentMappingContentID, value);
                }

                base.IDContent = value;
            }
        }
        #endregion
        #region Version
        /// <summary>
        /// This is the DataContent Version ID
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Guid IDVersion
        {
            get
            {
                if (attrContentMappingVersionID == null)
                    return base.IDVersion;

                return MappingHelperGet(attrContentMappingVersionID);
            }
            set
            {
                //We only allow this to be changed in the parent entity.
                if (mSatelliteEntity ?? false)
                    return;

                if (attrContentMappingVersionID != null)
                {
                    DataTable theTable = mDataContentSet.Tables[attrContentMappingVersionID.Table];

                    if (theTable != null)
                    {
                        if (theTable.Rows.Count == 0)
                        {
                            DataRow newRow = theTable.NewRow();
                            newRow[attrContentMappingVersionID.Field] = value;
                            theTable.Rows.Add(newRow);
                        }
                        else
                        {
                            DataRow theRow = theTable.Rows[0];
                            theRow[attrContentMappingVersionID.Field] = value;
                        }
                    }
                }

                base.IDVersion = value;
            }
        }
        #endregion

        #region GetDataSet
        /// <summary>
        /// This is the internal dataset for the binding
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DataSet GetDataSet
        {
            get { return mDataContentSet; }
        }
        #endregion
    }
}

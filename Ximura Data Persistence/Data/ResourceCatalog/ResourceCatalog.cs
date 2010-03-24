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
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class holds the resource collection for the Resource based Persistence Manager.
    /// </summary>
    [XimuraContentTypeID("B7D0B27E-DCCF-4ED7-8CD8-413E40379144")]
    [XimuraDataContentSchema("http://schema.ximura.org/persistence/resourcecatalog/1.0",
       "xmrres://Ximura/Ximura.Data.ResourceCatalog/Ximura.Data.CDS.Data.ResourceCatalog.DataSet.ResourceCatalog.xsd")]
    public class ResourceCatalog: DataContent
    {
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
		public ResourceCatalog(){}

		/// <summary>
		/// This is the deserialization constructor. 
		/// </summary>
		/// <param name="info">The Serialization info object that contains all the relevant data.</param>
		/// <param name="context">The serialization context.</param>
        public ResourceCatalog(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
		#endregion // Deserialization Constructor

        #region ResolveObject(Guid vid, out string id, out string path, out string type)
        /// <summary>
        /// This method resolves an embedded object location from the VID.
        /// </summary>
        /// <param name="vid">The version ID of the object.</param>
        /// <param name="id">The object resource id.</param>
        /// <param name="path">The object resource location.</param>
        /// <returns>Returns true if the object has been sucessfully resolved.</returns>
        public bool ResolveObject(Guid vid, bool useVersionID, out string id, out Guid versionID)
        {
            id = null;
            versionID = Guid.Empty;

            try
            {
                DataTable dtResources = GetDataSet.Tables["Resource"];

                DataRow[] rows;

                if (useVersionID)
                    rows = dtResources.Select(@"vid='" + vid.ToString() + "'");
                else
                    rows = dtResources.Select(@"cid='" + vid.ToString() + "'");

                if (rows.Length > 0)
                {
                    DataRow row = rows[0];

                    id = (string)row["id"];

                    versionID = new Guid((string)row["vid"]);
                }
                return rows.Length > 0;
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Error");
            }
            return false;
        }
        #endregion // ResolveObject(Guid vid, out string id, out string path, out string type)

        #region ResolveReference(string refType, string refValue, out Guid tid, out Guid cid, out Guid vid, out DateTime vidExpire)
        /// <summary>
        /// This method resolves the reference in to it's correct id values.
        /// </summary>
        /// <param name="refType">The reference type.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="tid">The type id.</param>
        /// <param name="cid">The content id.</param>
        /// <param name="vid">The version id.</param>
        /// <param name="vidExpire">The time that the version ID will expire. This can be used to cache
        /// the lookup for further requests.</param>
        /// <returns></returns>
        public bool ResolveReference(string refType, string refValue, 
            out Guid tid, out Guid cid, out Guid vid, out DateTime vidExpire)
        {
            tid = Guid.Empty;
            cid = Guid.Empty;
            vid = Guid.Empty;
            vidExpire = DateTime.Now;

            try
            {
                DataTable dtResources = GetDataSet.Tables["Resource"];

                DataRow[] rows = dtResources.Select(@"reftype='" + refType + "' and refvalue='" + refValue + "'");

                if (rows.Length > 0)
                {
                    DataRow row = rows[0];

                    tid = new Guid((string)row["tid"]);
                    cid = new Guid((string)row["cid"]);
                    vid = new Guid((string)row["vid"]);

                    vidExpire = DateTime.Now + TimeSpan.FromDays(1);
                }
                return rows.Length>0;
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Error");
            }
            return false;
        }
        #endregion // ResolveReference(string refType, string refValue, out Guid tid, out Guid cid, out Guid vid, out DateTime vidExpire)
    }
}

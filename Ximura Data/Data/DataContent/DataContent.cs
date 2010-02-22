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
	/// <summary>
	/// This is the default data content object. A data content object 
	/// differs from a content object as it is specifically designed to be loaded 
	/// and saved from a database based on the ID and Version number.
	/// </summary>
	[DefaultProperty("DataSource"),DefaultEvent("DataChanged")]
	[XimuraContentSerialization("Ximura.Data.Serialization.DataContentFormatter, Ximura")]
    [XimuraDataContentSchemaReference("http://www.w3.org/XML/1998/namespace", 
        "xmrres://XimuraData/Ximura.Data.DataContent/Ximura.Data.Resources.Xml.xsd")]
    [XimuraDataContentSchemaReference("http://schema.ximura.org/core", 
        "xmrres://XimuraData/Ximura.Data.DataContent/Ximura.Data.Resources.Ximura.xsd")]
    [XimuraDataContentIDMapping("entity","cid")]
    [XimuraDataContentVersionMapping("entity", "vid")]
    public partial class DataContent : XmlContentBase, IXimuraDataEntity,
        IDataContentSummary, IXimuraDataContentLinkage
	{
		#region Declarations

        private object syncDataContent = new object();
		/// <summary>
		/// Property that defines whether the record set is a new record.
		/// </summary>
		protected bool mIsNewRecord = false;

		#endregion
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
		public DataContent()
        {
			InitTableListeners();
        }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public DataContent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //This method sets the delegates for the DataTable event listeners
            InitTableListeners();
        }
        #endregion // Deserialization Constructor
    }
}

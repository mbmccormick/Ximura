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
        #region Event Handling
        /// <summary>
        /// This event is fired when a row is changing within a table.
        /// </summary>
        protected DataRowChangeEventHandler evhOnRowChanging;
        /// <summary>
        /// This event is fired when a row is changed in a table.
        /// </summary>
        protected DataRowChangeEventHandler evhOnRowChanged;
        /// <summary>
        /// This event is fired when a row is deleted in a table.
        /// </summary>
        protected DataRowChangeEventHandler evhOnRowDeleted;
        /// <summary>
        /// This event is fired when a row is being deleted in a table.
        /// </summary>
        protected DataRowChangeEventHandler evhOnRowDeleting;
        /// <summary>
        /// This event will be fired when the data has changed.
        /// </summary>
        public event DataContentDataChange DataChanged;
        /// <summary>
        /// This event is fired when data within the entity is changing.
        /// </summary>
        public event DataContentDataChange DataChanging;

        public event ContentEvent ConnectToParentDataSource;

        #endregion

        #region DataTable Event Handling
        protected virtual void SetChangeEvents(DataSet dataSet)
        {
            if (dataSet == null) return;

            //First of all set the bindings for the default table, if true
            if (attrContentMappingContentID != null && attrContentMappingContentID.listenEvents)
            {
                DataTable primaryTable = this.mDataContentSet.Tables[attrContentMappingContentID.Table];
                AddTableListener(primaryTable);
                SetTableShortcuts();
            }
        }
        /// <summary>
        /// This method sets the table shortcuts.
        /// </summary>
        protected virtual void SetTableShortcuts()
        {
        }

        /// <summary>
        /// This method creates the DataTable event handlers
        /// </summary>
        protected void InitTableListeners()
        {
            //evhOnRowChanging =new DataRowChangeEventHandler(OnRowChanging);
            //evhOnRowChanged =new DataRowChangeEventHandler(OnRowChanged);
            //evhOnRowDeleted =new DataRowChangeEventHandler(OnRowDeleted);
            //evhOnRowDeleting =new DataRowChangeEventHandler(OnRowDeleting);
        }

        protected void AddTableListener(DataTable theTable)
        {
            //theTable.RowChanging+=evhOnRowChanging;
            //theTable.RowChanged+=evhOnRowChanged;
            //theTable.RowDeleted+=evhOnRowDeleted;
            //theTable.RowDeleting+=evhOnRowDeleting;
        }

        protected void RemoveTableListener(DataTable theTable)
        {
            //theTable.RowChanging-=evhOnRowChanging;
            //theTable.RowChanged-=evhOnRowChanged;
            //theTable.RowDeleted-=evhOnRowDeleted;
            //theTable.RowDeleting-=evhOnRowDeleting;
        }

        protected virtual void OnRowChanging(object sender, DataRowChangeEventArgs e)
        {

        }

        protected virtual void OnRowChanged(object sender, DataRowChangeEventArgs e)
        {

        }

        protected virtual void OnRowDeleted(object sender, DataRowChangeEventArgs e)
        {

        }

        protected virtual void OnRowDeleting(object sender, DataRowChangeEventArgs e)
        {

        }
        #endregion

    }
}

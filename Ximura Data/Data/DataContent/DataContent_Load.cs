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
        #region Declarations
        //private DataSet internalDataContentSet;

        #endregion
        #region mDataContentSet protected data set
        /// <summary>
        /// This is the internal DataSet that hold the internal data for the 
        /// DataContent object.
        /// </summary>
        protected DataSet mDataContentSet
        {
            //get { return internalDataContentSet; }
            //set
            //{
            //    lock (syncDataContent)
            //    {
            //        internalDataContentSet = value;
            //    }
            //}
            get { return ((XmlDataDocument)XmlDataDoc).DataSet; }
            //set
            //{
            //    lock (syncDataContent)
            //    {
            //        internalDataContentSet = value;
            //    }
            //}
        }
        #endregion // mDataContentSet protected data set


        #region Load(DataSet data)
        /// <summary>
        /// This method loads the DataContent from a DataSet.
        /// </summary>
        /// <param name="data">The DataSet this content should hold.</param>
        public virtual void Load(DataSet data)
        {
            Load(data, false);
        }
        #endregion
        #region Load(DataSet data, bool replace)
        /// <summary>
        /// This method loads the DataContent from a DataSet.
        /// </summary>
        /// <param name="data">The DataSet this content should hold.</param>
        /// <param name="replace">This method will replace the dataset.</param>
        public virtual void Load(DataSet data, bool replace)
        {
            XmlDataDoc = new XmlDataDocument(data);
            mCanLoad = false;

        }
        #endregion // EntityLoad(DataSet data, bool replace)

        #region Load(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method converts the byte buffer in to a dataset and loads it in to the object.
        /// </summary>
        /// <param name="buffer">The byte incoming buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The number of bytes to read from the buffer.</param>
        /// <returns>Returns the number of bytes read from the buffer.</returns>
        public override int Load(byte[] buffer, int offset, int count)
        {
            if (Loaded)
                throw new ContentLoadException("The content is already loaded.");

            using (MemoryStream ms = new MemoryStream(buffer, offset, count, false))
            {
                try
                {
                    XmlDataDoc = new XmlDataDocument();
                    XmlReader xmlSchema = XmlTextReader.Create(ms, SchemaXmlReaderSettings);
                    mDataContentSet.EnforceConstraints = false;
                    mDataContentSet.ReadXmlSchema(xmlSchema);

                    ms.Position = 0;
                    XmlReader xmlData = XmlReader.Create(ms, SchemaXmlReaderSettings);
                    XmlDataDoc.Load(xmlData);
                }
                catch (Exception ex)
                {
                    try
                    {
                        XmlDataDoc = new XmlDataDocument();

                        ms.Position = 0;
                        XmlReader xmlData = XmlReader.Create(ms, SchemaXmlReaderSettings);
                        XmlDataDoc.Load(xmlData);
                    }
                    catch (Exception ex2)
                    {
                        throw ex2;
                    }
                }
            }

            mCanLoad = false;
            return count;
        }
        #endregion // Load(byte[] buffer, int offset, int count)
        #region Load(XmlDataDocument doc, bool replace)
        /// <summary>
        /// This method loads the DataContent from a DataSet.
        /// </summary>
        /// <param name="data">The DataSet this content should hold.</param>
        /// <param name="replace">This method will replace the dataset.</param>
        public virtual void Load(XmlDataDocument doc, bool replace)
        {
            XmlDataDoc = doc;
            mCanLoad = false;
        }
        #endregion // EntityLoad(DataSet data, bool replace)

        #region IsNewRecord
        /// <summary>
        /// Returns true if this is a new record.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNewRecord
        {
            get { return this.IDVersion == Guid.Empty; }
        }
        #endregion // IsNewRecord

        #region InitializeDataset
        /// <summary>
        /// This method is used to initialize the dataset with any 
        /// predefined value for a new record.
        /// </summary>
        protected virtual void InitializeDataset()
        {

        }
        #endregion

        #region SetInitialRecordSet
        /// <summary>
        /// This method loads the empty data set from the embedded resource
        /// </summary>
        //protected virtual bool SetInitialRecordSet()
        //{
        //    if (attrSchemaPrimary == null || mDataContentSet == null)
        //        return false;

        //    byte[] buffer;

        //    if (!GetAttrBuffer(GetType(), NewEntityCache, out buffer, DelGetNewData))
        //        return false;

        //    //Load the empty record if present
        //    MemoryStream ms = new MemoryStream(buffer);

        //    mDataContentSet.Clear();

        //    try
        //    {
        //        mDataContentSet.ReadXml(ms);
        //    }
        //    catch (System.Data.ConstraintException cex)
        //    {
        //        mDataContentSet.Clear();
        //        XimuraAppTrace.WriteLine("Constraint Exception: ",
        //            this.GetType().AssemblyQualifiedName, EventLogEntryType.Information);
        //        mDataContentSet.EnforceConstraints = false;
        //        ms.Position = 0;
        //        mDataContentSet.ReadXml(ms); ;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return true;
        //}
        #endregion // SetInitialRecordSet
    }
}

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
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    public partial class DataContent
    {
        #region Declarations
        /// <summary>
        /// This is the Schema attribute.
        /// </summary>
        protected XimuraDataContentSummaryAttribute[] attrsSummary = null;
        #endregion // Declarations

        #region SetAttributes()
        /// <summary>
        /// This protected method sets the mapping attributes for the content.
        /// </summary>
        protected override void SetAttributes()
        {
            base.SetAttributes();

            attrsSummary = AH.GetAttributes<XimuraDataContentSummaryAttribute>(GetType());
        }
        #endregion

        //#region ProcessSchemaAttribute
        ///// <summary>
        ///// This method loads the design time schema in to the dataset. This is 
        ///// used for the design time processing.
        ///// </summary>
        //protected virtual void ProcessSchemaAttribute()
        //{
        //    ProcessSchemaAttribute(false, true);
        //}
        ///// <summary>
        ///// This method loads the design time schema in to the dataset. This is 
        ///// used for the design time processing.
        ///// </summary>
        ///// <param name="noParentConnection">Set this to true if you do not wish the entity to connect to a parent data server.</param>
        //protected virtual void ProcessSchemaAttribute(bool noParentConnection, bool createNew)
        //{

        //    try
        //    {
        //        //If there is not an attribute present, quit.
        //        if (attrSchemaPrimary == null)
        //            return;

        //        if (!LoadSchemaFromAttrMetadata())
        //            return;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(this.GetType().Name + " : " + ex.Message);
        //        return;
        //    }

        //    try
        //    {
        //        if (!noParentConnection && LoadFromParentEntityServer())
        //        {
        //            SetChangeEvents(mDataContentSet);
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(this.GetType().Name + " : " + ex.Message);
        //        return;
        //    }

        //    if (createNew)
        //    {
        //        try
        //        {
        //            //Set the entity as a new record if present
        //            Load(true);
        //            SetChangeEvents(mDataContentSet);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(this.GetType().Name + " : " + ex.Message);
        //        }
        //    }
        //}

        //#endregion
        //#region GetAttrBuffer
        //private bool GetAttrBuffer(Type thisType, Dictionary<Type, byte[]> collection,
        //    out byte[] buffer, DelGetAttrStream DelGet)
        //{
        //    buffer = null;

        //    if (attrSchemaPrimary == null)
        //    {
        //        return false;
        //    }

        //    byte[] blob = attrSchemaPrimary.ResPathData;


        //    if (collection.ContainsKey(thisType))
        //    {
        //        buffer = collection[thisType];
        //        return true;
        //    }
        //    else
        //    {
        //        using (Stream schemaStream = DelGet(thisType, this.attrSchemaPrimary))
        //        {
        //            if (schemaStream == null)
        //            {
        //                return false;
        //            }

        //            lock (syncDataContent)
        //            {
        //                if (!collection.ContainsKey(thisType))
        //                {
        //                    buffer = new byte[schemaStream.Length];

        //                    schemaStream.Read(buffer, 0, buffer.Length);

        //                    collection.Add(thisType, buffer);
        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}
        //#endregion // GetAttrBuffer

        //#region LoadSchemaFromAttrMetadata()
        ///// <summary>
        ///// This method loads the content schema from the associated metadata.
        ///// </summary>
        ///// <returns></returns>
        //protected virtual bool LoadSchemaFromAttrMetadata()
        //{
        //    if (mDataContentSet != null)
        //        return false;

        //    Type thisType = GetType();
        //    mDataContentSet = new DataSet(thisType.Name);

        //    byte[] buffer;

        //    //            if (!GetAttrBuffer(thisType, SchemaCache, out buffer, DelGetSchema))
        //    if (!GetAttrBuffer(thisType, SchemaCache, out buffer, DelGetNewData))
        //        return false;

        //    try
        //    {
        //        XmlReader xmlR = XmlReader.Create(new MemoryStream(buffer), SchemaXmlReaderSettings);
        //        //XmlReader xmlR = XmlReader.Create("ximura://hello.mom", SchemaXmlReaderSettings);
        //        //OK, we will load the schema from the attribute
        //        //mDataContentSet.ReadXmlSchema(xmlR);
        //        mDataContentSet.ReadXml(xmlR);

        //    }
        //    catch (XmlSchemaException ex)
        //    {
        //        throw ex;
        //    }

        //    return true;
        //}
        //#endregion // LoadSchemaFromAttrMetadata()
    }
}

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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This abstract class wraps a content based entity around a XML base object.
    /// </summary>
    /// <typeparam name="T">An XML object that implements IXPathNavigable.</typeparam>
    public abstract partial class XmlContentHolder<T>
    {
        #region Load()
        /// <summary>
        /// This method resets the entity to it's default value.
        /// </summary>
        /// <returns>True is the entity is reset.</returns>
        public virtual bool Load()
        {
            return Load(false);
        }
        #endregion
        #region Load(bool force)
        /// <summary>
        /// This method resets the entity to it's default value.
        /// </summary>
        /// <param name="force">Set this to true if you wish to lose any data changes.</param>
        /// <returns>True is the entity is reset.</returns>
        public virtual bool Load(bool force)
        {
            if (Loaded && !force)
                throw new ContentLoadException("The content is already loaded. Set force to true if you wish to reset the content.");

            //If the content does not have any default data then quit
            if (attrDefaultData == null)
                return false;

            byte[] blob = attrDefaultData.ResPathData;
            Load(blob, 0, blob.Length);
            return true;
        }
        #endregion // EntityCreateNew(bool force)
        #region Load(Stream strmData)
        /// <summary>
        /// This method loads the dataset from a stream.
        /// </summary>
        /// <param name="strmData">The data stream to read.</param>
        /// <returns>Returns the number of bytes read from the stream.</returns>
        public override int Load(Stream strmData)
        {
            try
            {
                byte[] blob = new byte[strmData.Length];
                strmData.Read(blob, 0, blob.Length);
                return Load(blob, 0, blob.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // Load(Stream strmData)

        #region SchemaXmlReaderSettings
        /// <summary>
        /// This is the XMLReader setting used when loading the XSD file. You should override this class
        /// to add any specific namespace XSD files.
        /// </summary>
        protected virtual XmlReaderSettings SchemaXmlReaderSettings
        {
            get
            {
                XmlSchemaSet set = new XmlSchemaSet();
                XmlResolver xmlR = XmlContentXsdResolver.GetResolver(GetType());

                set.XmlResolver = xmlR;
                set.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandlerError);
                SchemaAdd(set);
                set.Compile();

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = XmlContentXsdResolver.GetResolver(GetType());
                settings.ValidationFlags =
                    XmlSchemaValidationFlags.AllowXmlAttributes |
                    //XmlSchemaValidationFlags.ProcessInlineSchema | 
                    XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationType = ValidationType.Schema;
                //settings.XmlResolver = xmlR;
                settings.Schemas = set;

                return settings;
            }
        }
        #endregion // SchemaXmlReaderSettings
        #region SchemaAdd(XmlSchemaSet set)
        /// <summary>
        /// This default method will add the schemas defined in the metadata, you should override this
        /// method to add specific schemas not covered in the attributes.
        /// </summary>
        /// <param name="set">The schema set.</param>
        protected virtual void SchemaAdd(XmlSchemaSet set)
        {
            if (attrSchemaPrimary != null)
                using (MemoryStream ms = new MemoryStream(attrSchemaPrimary.ResPathData))
                {
                    XmlSchema s = XmlSchema.Read(ms, ValidationEventHandlerError);
                    set.Add(s);
                }

            foreach (XimuraDataContentSchemaReferenceAttribute attr in attrsSchemaReference)
                using (MemoryStream ms = new MemoryStream(attr.ResPathData))
                {
                    XmlSchema s = XmlSchema.Read(ms, ValidationEventHandlerError);
                    set.Add(s);
                }

            //set.Add(attr.UriPath.ToString(), attr.ResPathDataXmlReader);


            //if (attrSchemaPrimary != null)
            //    set.Add(attrSchemaPrimary.UriPath.ToString() ,attrSchemaPrimary.ResPathDataXmlReader);

            //foreach (XimuraDataContentSchemaReferenceAttribute attr in attrsSchemaReference)
            //    set.Add(attr.UriPath.ToString(), attr.ResPathDataXmlReader);


            //set.Add("http://www.w3.org/XML/1998/namespace", "xmrres://Ximura/Ximura.Data.DataContent/Ximura.Data.Contents.DataSet.xml.xsd");
            //set.Add("http://schema.ximura.org/core", "xmrres://Ximura/Ximura.Data.DataContent/Ximura.Data.Contents.DataSet.Ximura.xsd");
            //set.Add("http://purl.org/dc/elements/1.1/", "xmrres://Ximura/Ximura.Data.DataContent/Ximura.Data.Contents.DataSet.dc.xsd");
        }
        #endregion // SchemaAdd(XmlSchemaSet set)

        #region ValidationEventHandlerError(object sender, ValidationEventArgs e)
        protected virtual void ValidationEventHandlerError(object sender, ValidationEventArgs e)
        {


        }
        #endregion // ValidationEventHandlerError(object sender, ValidationEventArgs e)
    }
}

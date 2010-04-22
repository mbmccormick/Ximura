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
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class is used to resolve the XSD 
    /// </summary>
    public partial class XmlContentXsdResolver : XmlResolver
    {
        #region Declarations
        private Type mObjectType;
        /// <summary>
        /// This is the Schema attribute.
        /// </summary>
        protected XimuraDataContentSchemaAttribute attrSchemaPrimary;
        /// <summary>
        /// This is the collection of reference schemas for the object.
        /// </summary>
        protected XimuraDataContentSchemaReferenceAttribute[] attrsSchemaReference;
        private System.Net.ICredentials mCredentials;
        #endregion // Declarations
        #region Factory Constructor
        /// <summary>
        /// This class is used to resolve the system resources.
        /// </summary>
        private XmlContentXsdResolver(Type objectType)
            : base()
        {
            mObjectType = objectType;

            attrSchemaPrimary =
                AH.GetAttribute<XimuraDataContentSchemaAttribute>(objectType);

            attrsSchemaReference =
                AH.GetAttributes<XimuraDataContentSchemaReferenceAttribute>(objectType);
        }

        #endregion // Constructor

        #region DataContentType
        /// <summary>
        /// This is the resolver base object type.
        /// </summary>
        public Type DataContentType
        {
            get { return mObjectType; }
        }
        #endregion // DataContentType

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if ((ofObjectToReturn != null) && (ofObjectToReturn != typeof(Stream)))
            {
                throw new XmlException("Xml_UnsupportedClass");
            }

            return GetStream(absoluteUri);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri tempUri = base.ResolveUri(baseUri, relativeUri);
            return tempUri;
        }

        public override System.Net.ICredentials Credentials
        {
            set
            {
                mCredentials = value;
            }
        }

        protected virtual Stream GetStream(Uri location)
        {
            switch (location.Scheme.ToLower())
            {
                case "xmrres":
                    return ResolveEmbeddedResourceToStream(location, DataContentType);
                case "http":
                    return AttributeResolveEmbeddedResource(location);
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual Stream AttributeResolveEmbeddedResource(Uri location)
        {
            //OK, we are going to scan the attributes for the URI mapping
            Uri resUri = ScanRefAttributes(location);

            if (resUri != null)
                return ResolveEmbeddedResourceToStream(resUri, DataContentType);

            if (DataContentType == typeof(DataContent))
                return null;

            return XmlContentXsdResolver.GetResolver(DataContentType.BaseType).AttributeResolveEmbeddedResource(location);
        }

        protected virtual Uri ScanRefAttributes(Uri location)
        {
            Attribute attrToReturn = null;

            object[] attrs = DataContentType.GetCustomAttributes(typeof(XimuraDataContentSchemaReferenceAttribute), false);
            if (attrs.Length == 0)
                return null;

            foreach (object attr in attrs)
            {
                if (!(attr is XimuraDataContentSchemaReferenceAttribute))
                    continue;

                XimuraDataContentSchemaReferenceAttribute refAttr = attr as XimuraDataContentSchemaReferenceAttribute;

                if (refAttr.UriPath == location)
                    return refAttr.ResPath;
            }

            return null;
        }
    }
}

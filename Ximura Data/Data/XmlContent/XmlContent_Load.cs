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
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The XMLContent object wraps basic persistence functionality around the XMl Data object.
    /// </summary>
    public partial class XmlContent
    {
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
                    XmlDataDoc = new XmlDocument();
                    XmlReader xmlSchemaReader = XmlReader.Create(ms, SchemaXmlReaderSettings);

                    XmlDataDoc.Load(xmlSchemaReader);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            mCanLoad = false;
            return count;
        }
        #endregion // Load(byte[] buffer, int offset, int count)

        #region Load(XmlDocument doc, bool replace)
        /// <summary>
        /// This method loads the DataContent from a DataSet.
        /// </summary>
        /// <param name="data">The DataSet this content should hold.</param>
        /// <param name="replace">This method will replace the dataset.</param>
        public virtual void Load(XmlDocument doc, bool replace)
        {
            XmlDataDoc = doc;
            mCanLoad = false;
        }
        #endregion // EntityLoad(DataSet data, bool replace)
    }
}

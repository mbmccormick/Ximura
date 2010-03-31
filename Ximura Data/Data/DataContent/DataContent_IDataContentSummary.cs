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
        #region GetDataSummary()
        /// <summary>
        /// This protected method will return a text summary of the data
        /// if a stylesheet has been defined.
        /// </summary>
        /// <returns>
        /// A string containing a text summary, or null if a stylesheet is
        /// not defined.
        /// </returns>
        protected virtual string GetDataSummary()
        {
            if (attrsSummary == null || attrsSummary.Length == 0)
                return null;

            //Resolve aummary Attr
            XimuraDataContentSummaryAttribute summaryAttr = attrsSummary[0] as XimuraDataContentSummaryAttribute;

            Stream xsltStream = new MemoryStream(summaryAttr.ResPathData);

            return GetDataSummary(xsltStream);
        }

        protected virtual string GetDataSummary(Stream xsltStream)
        {
            DataSet data = this.GetDataSet;
            if (xsltStream == null || data == null)
                return null;

            Stream dsStream = new MemoryStream();
            data.WriteXml(dsStream);

            TextWriter tw = new StringWriter();

            try
            {
                XslTransform xsl = new XslTransform();

                XmlDocument XSLTDoc = new XmlDocument();
                XSLTDoc.Load(xsltStream);
                XmlDocument dataDoc = new XmlDocument();
                dataDoc.LoadXml(data.GetXml());

                XPathNavigator stylesheet = XSLTDoc.CreateNavigator();
                xsl.Load(stylesheet, null, null);
                XPathNavigator dataNav = dataDoc.CreateNavigator();

                xsl.Transform(dataNav, null, tw, null);
            }
            catch (Exception ex)
            {
                return null;
            }

            return tw.ToString();
        }
        #endregion // getDataSummary()
        #region IDataContentSummary Members

        public virtual object GetSummary(DataContentSummaryType type)
        {
            // TODO:  Add DataContent.getSummary implementation
            return null;
        }

        public virtual object GetSummary(DataContentSummaryType type, string id)
        {
            // TODO:  Add DataContent.Ximura.Data.IDataContentSummary.getSummary implementation
            return null;
        }

        #endregion

    }
}

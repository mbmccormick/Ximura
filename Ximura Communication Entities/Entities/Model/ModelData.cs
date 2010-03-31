#region using
using System;
using System.Xml;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The model class provides the basic functionality for the data model for the presentation classes.
    /// </summary>
    [XimuraContentTypeID("59D17FCA-CD2A-47fe-A541-D71E5804615B")]
    [XimuraDataContentDefault(
        "xmrres://XimuraCommEntities/Ximura.Communication.Model/Ximura.Communication.Resources.Model_DefaultData.xml")]
    [XimuraDataContentSchema("http://schema.ximura.org/controller/model/1.0",
        "xmrres://XimuraCommEntities/Ximura.Communication.Model/Ximura.Communication.Resources.Model.xsd")]
    public class Model : XimuraCore
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public Model() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public Model(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region NamespaceDefaultShortName
        /// <summary>
        /// This is the short name used in the namespace manager to refer to the root namespace.
        /// </summary>
        protected override string NamespaceDefaultShortName
        {
            get
            {
                return "r";
            }
        }
        #endregion // NamespaceDefaultShortName

        #region ExtractPathData(string xPath)
        /// <summary>
        /// This method is used to extract XML by means of an xpath declaration.
        /// </summary>
        /// <param name="xPath">THe xPath of the data to extract.</param>
        /// <returns>Returns the XML as a string, or null if the xPath does not resolve.</returns>
        public string ExtractPathData(string xPath)
        {
            try
            {
                XmlNode node = this.XmlDataDoc.SelectSingleNode(xPath, NSM);
                if (node == null)
                    return null;
                return node.InnerXml;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion // ExtractPathData(string xPath)

        #region DataNodes()
        /// <summary>
        /// This method returns a enumerator for the data nodes.
        /// </summary>
        /// <returns>Returns an enumerator of XmlElement nodes</returns>
        public IEnumerable<XmlElement> DataNodes()
        {
            XmlNodeList nodeInserts =
                XmlDataDoc.SelectNodes(
                    @"//r:model/r:data[@type='entity' and @action='insert']", NSM);
            foreach (XmlNode node in nodeInserts)
                yield return node as XmlElement;
        }
        #endregion // DataNodes()


        ///// <summary>
        ///// This class returns the inserts for the document.
        ///// </summary>
        //public IEnumerable<Insert> Inserts
        //{
        //    get
        //    {
        //        Insert insert = null;

        //        try
        //        {
        //            XmlNodeList nodeInserts =
        //                XmlDataDoc.SelectNodes(
        //                    @"//r:model/r:data[@type='entity' and @action='insert']", NSM);
        //            foreach (XmlNode node in nodeInserts)
        //                yield return new Insert(node as XmlElement);
        //        }
        //        finally
        //        {
        //            if (insert != null && insert.ObjectPoolCanReturn())
        //                insert.ObjectPoolReturn();
        //        }
        //    }
        //}
        
    }
}

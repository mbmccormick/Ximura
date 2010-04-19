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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the base class for documents that use Atom feeds.
    /// </summary>
    /// <remarks>
    /// atomFeed =
    /// atomFeed =
    /// element atom:feed {
    ///  atomCommonAttributes,
    ///  (atomAuthor* --
    ///   & atomCategory* --
    ///   & atomContributor* --
    ///   & atomGenerator? F
    ///   & atomIcon? F
    ///   & atomId --
    ///   & atomLink* --
    ///   & atomLogo? F
    ///   & atomRights? --
    ///   & atomSubtitle? F
    ///   & atomTitle --
    ///   & atomUpdated --
    ///   & extensionElement*),
    ///  atomEntry*}
    /// </remarks>
    public class AtomFeed : AtomBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AtomFeed() //: this((IContainer)null) 
        { }
        ///// <summary>
        ///// This constructor is called by .NET when it added as new to a container.
        ///// </summary>
        ///// <param name="container">The container this component should be added to.</param>
        //public AtomFeed(System.ComponentModel.IContainer container)
        //    :
        //    base(container) { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AtomFeed(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            mappingShortcuts.Add("r", "//r:feed");
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
        #region NamespaceDefaultShortName
        /// <summary>
        /// This override sets the default namespace to "r"
        /// </summary>
        protected override string NamespaceDefaultShortName
        {
            get
            {
                return "r";
            }
        }
        #endregion // NamespaceDefaultShortName

        #region AtomLogoSet
        public void AtomLogoSet(Uri logoUri)
        {
            AtomLogoSet(logoUri, null, null);
        }
        public void AtomLogoSet(Uri logoUri, string baseUri, string lang)
        {
            AtomUriConstructSet("logo", logoUri, baseUri, lang, false);
        }
        #endregion // AtomLogoSet

        #region AtomIconSet
        public void AtomIconSet(Uri iconUri)
        {
            AtomIconSet(iconUri, null, null);
        }
        public void AtomIconSet(Uri iconUri, string baseUri, string lang)
        {
            AtomUriConstructSet("icon", iconUri, baseUri, lang, false);
        }
        #endregion // AtomIconSet

        #region AtomSubtitleSet
        public void AtomSubtitleSet(AtomTextType type, string text)
        {
            AtomSubtitleSet(type, text, null, null);
        }
        public void AtomSubtitleSet(AtomTextType type, string text,
            string baseUri, string lang)
        {
            AtomTextConstructSet("subtitle", type, text, baseUri, lang);
        }
        #endregion // AtomTitleSet

        #region EntryAdd(AtomEntry entry)
        /// <summary>
        /// This method adds an entry record to an existing feed.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
        public virtual void EntryAdd(AtomEntry entry)
        {
            try
            {
                XmlDocumentFragment frag = Payload.CreateDocumentFragment();

                XmlNode dataNode = entry.Payload.FirstChild;

                while (dataNode.NodeType != XmlNodeType.Element)
                    dataNode = dataNode.NextSibling;

                frag.InnerXml = dataNode.OuterXml;

                XmlNodeList data = Payload.SelectNodes("//r:entry", NSM);

                if (data.Count == 0)
                {
                    XmlNode rootNode = Payload.SelectSingleNode(XPSc("r"), NSM);
                    rootNode.AppendChild(frag);
                }
                else
                {
                    XmlNode sibling = data[data.Count - 1];
                    sibling.ParentNode.InsertAfter(frag, sibling);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // EntryAdd(AtomEntry entry)

        #region EntryCollection<T>()
        /// <summary>
        /// This property returns the entry collection from the feed document.
        /// </summary>
        /// <param name="objectPoolAutoReturn">Set this value to true if you want the iteration loop
        /// to automatically return the entry object back to the pool.</param>
        /// <returns>Returns an entry collection.</returns>
        public IEnumerable<T> EntryCollection<T>(bool objectPoolAutoReturn) where T : AtomEntry
        {
            XmlNodeList data = Payload.SelectNodes("//r:entry", NSM);
            foreach (XmlNode node in data)
            {
                T entry = EntryCollectionGetEntry<T>(node);
                yield return entry;

                if (entry.ObjectPoolCanReturn)
                    entry.ObjectPoolReturn();
            }
        }
        #endregion // EntryCollection()
        #region EntryCollectionGetEntry<T>(XmlNode node)
        /// <summary>
        /// This method returns a new entry abject from the object pool.
        /// </summary>
        /// <param name="node">The node containing the data for the entry object.</param>
        /// <returns>An AtomEntry object.</returns>
        protected T EntryCollectionGetEntry<T>(XmlNode node) where T : AtomEntry
        {
            T entry = this.ObjectPool.PoolManager.GetPoolManager<T>().Get();
            entry.Load(node.OuterXml);
            return entry;
        }
        #endregion // EntryCollectionGetEntry(XmlNode node)
        #region EntryGet<T>(int ordinal)
        /// <summary>
        /// This method returns the node at the specific position.
        /// </summary>
        /// <typeparam name="T">The entry object type.</typeparam>
        /// <param name="ordinal">The entry record position.</param>
        /// <returns>Returns the entry object.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The ArgumentOutOfRangeException will be thrown if the ordinal value cannot be found in the collection.</exception>
        public T EntryGet<T>(int ordinal) where T : AtomEntry
        {
            string path = "//r:feed/r:entry[osmeta:osrelevance='" + ordinal.ToString() + "']";
            XmlNode node = Payload.SelectSingleNode(path, NSM);
            if (node == null)
                throw new ArgumentOutOfRangeException("ordinal", string.Format("Ordinal ({0}) is out of range.", ordinal));
            T entry = EntryCollectionGetEntry<T>(node);
            return entry;
        }
        #endregion // EntryGet<T>(int ordinal)
    }
}

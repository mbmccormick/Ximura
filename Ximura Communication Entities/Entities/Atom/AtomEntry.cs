#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the base class for documents that use Atom feeds.
    /// </summary>
    /// <remarks>
    ///atomEntry =
    ///  element atom:entry {
    ///     atomCommonAttributes,
    ///     (atomAuthor* --
    ///      & atomCategory* --
    ///      & atomContent? E 
    ///      & atomContributor* --
    ///      & atomId --
    ///      & atomLink* --
    ///      & atomPublished? E
    ///      & atomRights? --
    ///      & atomSource? E
    ///      & atomSummary? E
    ///      & atomTitle --
    ///      & atomUpdated --
    ///      & extensionElement*)
    ///  }
    /// </remarks>
    public class AtomEntry : AtomBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AtomEntry() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public AtomEntry(System.ComponentModel.IContainer container)
            :
            base(container) { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AtomEntry(SerializationInfo info, StreamingContext context)
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
            mappingShortcuts.Add("r", "//r:entry");
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

        #region AtomPublished/Set
        /// <summary>
        /// The published date
        /// </summary>
        public virtual DateTime? AtomPublished
        {
            get { return this.XmlMappingGet<DateTime?>(XPSc("r", "published"), ParseDateGet); }
        }
        public virtual void AtomPublishedSet(DateTime value)
        {
            AtomDateConstructSet("published", value);
        }
        #endregion
        #region AtomSourceSet
        ///// <summary>
        ///// This is the feed subtitle.
        ///// </summary>
        //public virtual void AtomSourceSet()
        //{
        //    throw new NotSupportedException();
        //}
        #endregion
        #region AtomSummarySet
        /// <summary>
        /// This is the feed subtitle.
        /// </summary>
        public void AtomSummarySet(AtomTextType type, string text)
        {
            AtomSummarySet(type, text, null, null);
        }
        public virtual void AtomSummarySet(AtomTextType type, string text,
            string baseUri, string lang)
        {
            AtomTextConstructSet("summary", type, text, baseUri, lang);
        }
        #endregion
        #region AtomContentSet
        /// <summary>
        /// This is the feed subtitle.
        /// </summary>
        public void AtomContentSet(AtomTextType type, string text)
        {
            AtomContentSet(type, text, null, null);
        }
        public virtual void AtomContentSet(AtomTextType type, string text,
            string baseUri, string lang)
        {
            AtomTextConstructSet("content", type, text, baseUri, lang);
        }
        public void AtomContentSet(string type, byte[] data)
        {
            AtomContentSet(type, data, null, null);
        }
        public virtual void AtomContentSet(string type, byte[] data,
            string baseUri, string lang)
        {
            throw new NotImplementedException();
        }

        public void AtomContentSet(string type, Uri src)
        {
            AtomContentSet(type, src, null, null);
        }

        public virtual void AtomContentSet(string type, Uri src,
            string baseUri, string lang)
        {
            AtomTextConstructSet("content", type, src==null?"":src.OriginalString, baseUri, lang);
        }
        #endregion

        #region ContentConstructWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="baseUri"></param>
        /// <param name="lang"></param>
        /// <remarks>
        /// atomInlineTextContent =
        ///   element atom:content {
        ///      atomCommonAttributes,
        ///      attribute type { "text" | "html" }?,
        ///      (text)*
        ///   }
        ///
        /// atomInlineXHTMLContent =
        ///   element atom:content {
        ///      atomCommonAttributes,
        ///      attribute type { "xhtml" },
        ///      xhtmlDiv
        ///   }
        ///
        /// atomInlineOtherContent =
        ///   element atom:content {
        ///      atomCommonAttributes,
        ///      attribute type { atomMediaType }?,
        ///      (text|anyElement)*
        ///   }
        ///
        /// atomOutOfLineContent =
        ///   element atom:content {
        ///      atomCommonAttributes,
        ///      attribute type { atomMediaType }?,
        ///      attribute src { atomUri },
        ///      empty
        ///   }
        ///
        /// atomContent = atomInlineTextContent
        ///  | atomInlineXHTMLContent
        ///  | atomInlineOtherContent
        ///  | atomOutOfLineContent
        /// </remarks>
        protected void ContentConstructWrite(XmlWriter writer, 
            AtomTextType type, string text,
            string baseUri, string lang)
        {
            writer.WriteStartElement("content");

            CommonAttributesWrite(writer, baseUri, lang);

            switch (type)
            {
                case AtomTextType.Text:
                case AtomTextType.Html:
                    writer.WriteAttributeString("type",
                        (string)((type == AtomTextType.Text) ? "text" : "html"));
                    writer.WriteString(text);
                    break;
                case AtomTextType.Xhtml:
                    writer.WriteAttributeString("type", "xhtml");
                    writer.WriteRaw(text);
                    break;
            }

            writer.WriteEndElement();
        }
        #endregion // TextConstructWrite
    }
}

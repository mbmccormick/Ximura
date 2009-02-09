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
using System.Net.Mail;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the base object for the Atom feed class.
    /// </summary>
    /// <remarks>
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
    /// atomEntry =
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
    [XimuraDataContentSchema("http://www.w3.org/2005/Atom",
        "xmrres://XimuraData/Ximura.Data.AtomBase/Ximura.Data.Atom.Atom.xsd")]
    public class AtomBase : XimuraCore
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AtomBase() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public AtomBase(System.ComponentModel.IContainer container)
            :
            base(container) { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AtomBase(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region AtomXmlLang
        /// <summary>
        /// This property sets the base language declaration for the atom entity.
        /// </summary>
        public virtual string AtomXmlLang
        {
            get { return XmlMappingGetToString(XPScA("r", "xml:lang")); }
            set 
            { 
                string mAtomLanguage = value; 
            }
        }
        #endregion // AtomLanguage
        #region AtomXmlBase
        /// <summary>
        /// This property sets the base uri declaration for the atom entity.
        /// </summary>
        public virtual string AtomXmlBase
        {
            get { return XmlMappingGetToString(XPScA("r", "xml:base")); }
            set 
            { 
                string mAtomBaseUri = value; 
            }
        }
        #endregion // AtomBaseUri

        #region AtomID/Set
        public virtual string AtomID
        {
            get { return XmlMappingGetToString(XPSc("r", "id")); }
        }

        public virtual void AtomIDSet(Uri id)
        {
            AtomIDSet(id, null, null);
        }

        public virtual void AtomIDSet(Uri id, string baseUri, string lang)
        {
            AtomUriConstructSet("id", id, baseUri, lang, false);
        }
        #endregion // AtomID/Set

        #region AtomUpdated/Set
        public virtual DateTime? AtomUpdated
        {
            get { return this.XmlMappingGet<DateTime?>(XPSc("r", "updated"), ParseDateGet); }
        }

        public void AtomUpdatedSet(DateTime value)
        {
            AtomDateConstructSet("updated", value);
        }
        #endregion // AtomUpdated/Set

        #region AtomTitleSet
        public void AtomTitleSet(AtomTextType type, string text)
        {
            AtomTitleSet(type, text, null, null);
        }
        public void AtomTitleSet(AtomTextType type, string text,
            string baseUri, string lang)
        {
            AtomTextConstructSet("title", type, text, baseUri, lang);
        }
        #endregion // AtomTitleSet

        #region AtomAuthorAdd
        public void AtomAuthorAdd(string name, Uri uri, MailAddress email)
        {
            AtomAuthorAdd(name, uri, email, null, null);
        }

        public void AtomAuthorAdd(string name, Uri uri, MailAddress email,
            string baseUri, string lang)
        {
            AtomPersonConstructSet("author", name, uri, email, baseUri, lang, true);
        }
        #endregion // AtomAuthorAdd
        #region AtomContributorAdd
        public void AtomContributorAdd(string name, Uri uri, MailAddress email)
        {
            AtomContributorAdd(name, uri, email, null, null);
        }

        public void AtomContributorAdd(string name, Uri uri, MailAddress email,
            string baseUri, string lang)
        {
            AtomPersonConstructSet("contributor", name, uri, email, baseUri, lang, true);
        }
        #endregion // AtomAuthorAdd

        #region AtomCategoryAdd
        public void AtomCategoryAdd(string name, Uri scheme, string label)
        {
            AtomCategoryAdd(name, scheme, label, null, null);
        }

        public void AtomCategoryAdd(string term, Uri scheme, string label, string baseUri, string lang)
        {
            AtomCategoryConstructSet(term, scheme, label, baseUri, lang, true);
        }
        #endregion // AtomAuthorAdd

        #region AtomLinkAdd
        public void AtomLinkAdd(Uri href, string rel)
        {
            AtomLinkConstructSet(href, rel, null, null, null, null, null, null);
        }

        public void AtomLinkAdd(Uri href, string rel, string type)
        {
            AtomLinkConstructSet(href, rel, type, null, null, null, null, null);
        }

        public void AtomLinkAdd(Uri href,
            string rel, string type, string hreflang, string title, string length,
            string baseUri, string lang)
        {
            AtomLinkConstructSet(href, rel, type, hreflang, title, length, baseUri, lang);
        }
        #endregion // AtomLinkAdd

        #region AtomLinkConstructSet
        protected virtual bool AtomLinkConstructSet(Uri href,
            string rel, string type, string hreflang, string title, string length,
            string baseUri, string lang)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    LinkWrite(writer, href, rel, type, hreflang, title, length, baseUri, lang);
                });
            return FragmentSet(XPSc("r", "link"), true, frag);
        }
        #endregion // AtomPersonConstructSet

        #region AtomUriConstructSet
        protected virtual bool AtomUriConstructSet(string elementName, Uri uri, 
            string baseUri, string lang, bool append)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    UriConstructWrite(writer, elementName, uri, baseUri, lang);
                });
            return FragmentSet(XPSc("r", elementName), append, frag);
        }
        #endregion // AtomPersonConstructSet

        #region AtomCategoryConstructSet
        protected virtual bool AtomCategoryConstructSet(string term, Uri scheme, string label,
            string baseUri, string lang, bool append)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    CategoryWrite(writer, term, scheme, label, baseUri, lang);
                });
            return FragmentSet(XPSc("r", "category"), append, frag);
        }
        #endregion // AtomCategoryConstructSet

        #region AtomPersonConstructSet
        protected virtual bool AtomPersonConstructSet(string elementName, 
            string name, Uri uri, MailAddress email,
            string baseUri, string lang, bool append)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    PersonConstructWrite(writer, elementName, name, uri, email, baseUri, lang);
                });
            return FragmentSet(XPSc("r", elementName), append, frag);
        }
        #endregion // AtomPersonConstructSet

        #region AtomTextConstructSet
        protected virtual bool AtomTextConstructSet(string elementName,
            string type, string src, string baseUri, string lang)
        {
            return AtomTextConstructSet(elementName, type, src, baseUri, lang, false);
        }

        protected virtual bool AtomTextConstructSet(string elementName,
            string type, string src, string baseUri, string lang, bool append)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    TextConstructWrite(writer, elementName, type, src, baseUri, lang);
                });
            return FragmentSet(XPSc("r", elementName), append, frag);
        }

        protected virtual bool AtomTextConstructSet(string elementName,
            AtomTextType type, string text,
            string baseUri, string lang)
        {
            return AtomTextConstructSet(elementName, type, text, baseUri, lang, false);
        }

        protected virtual bool AtomTextConstructSet(string elementName,
            AtomTextType type, string text, string baseUri, string lang, bool append)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    TextConstructWrite( writer, elementName, type, text, baseUri, lang);
                });
            return FragmentSet(XPSc("r", elementName), append, frag);
        }
        #endregion // AtomTextConstructSet

        #region AtomDateConstructSet
        protected virtual bool AtomDateConstructSet(string elementName, DateTime time)
        {
            return AtomDateConstructSet(elementName, time, null, null, false);
        }

        protected virtual bool AtomDateConstructSet(string elementName, DateTime time, 
            string baseUri, string lang, bool append)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer) {
                    DateConstructWrite(writer, elementName, time, baseUri, lang);
                });
            return FragmentSet(XPSc("r", elementName), append, frag);
        }
        #endregion // AtomDateConstructSet

        #region AtomConstructAdd(string xPath, bool append, CreateFragmentAction action)
        /// <summary>
        /// This method adds a custom action to the entry documents.
        /// </summary>
        /// <param name="xPath">The xpath of the reference node.</param>
        /// <param name="append">This boolean property determines whether the node should be appended or replaced.</param>
        /// <param name="action">The action delegate that writes the node XML.</param>
        /// <returns>Returns true if the node is created successfully.</returns>
        public virtual bool AtomConstructAdd(string xPath, bool append, CreateFragmentAction action)
        {
            XmlDocumentFragment frag = CreateFragment(action);
            return FragmentSet(xPath, append, frag);
        }
        #endregion // AtomConstructAdd(string xPath, bool append, CreateFragmentAction action)

        #region XmlWriter Helpers

        #region UriConstructWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="text"></param>
        /// <param name="uri"></param>
        /// <param name="version"></param>
        /// <value>
        ///atomGenerator = element atom:generator {
        ///   atomCommonAttributes,
        ///   attribute uri { atomUri }?,
        ///   attribute version { text }?,
        ///   text
        ///}
        /// </value>
        protected void UriConstructWrite(XmlWriter writer,
            string elementName, Uri uri, 
            string baseUri, string lang)
        {
            writer.WriteStartElement(elementName, "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            writer.WriteString(uri.OriginalString);

            writer.WriteEndElement();
        }
        #endregion // GeneratorWrite
        #region GeneratorConstructWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="text"></param>
        /// <param name="uri"></param>
        /// <param name="version"></param>
        /// <value>
        ///atomGenerator = element atom:generator {
        ///   atomCommonAttributes,
        ///   attribute uri { atomUri }?,
        ///   attribute version { text }?,
        ///   text
        ///}
        /// </value>
        protected void GeneratorWrite(XmlWriter writer, 
            string text, Uri uri, string version,
            string baseUri, string lang)
        {
            writer.WriteStartElement("generator", "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            if (uri != null)
                writer.WriteAttributeString("uri", uri.ToString());
            if (version != null && version!="")
                writer.WriteAttributeString("version", version);

            writer.WriteString(text);

            writer.WriteEndElement();
        }
        #endregion // GeneratorWrite
        #region CategoryWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="term"></param>
        /// <param name="scheme"></param>
        /// <param name="label"></param>
        /// <param name="baseUri"></param>
        /// <param name="lang"></param>
        /// <remarks>
        ///atomCategory =
        ///   element atom:category {
        ///      atomCommonAttributes,
        ///      attribute term { text },
        ///      attribute scheme { atomUri }?,
        ///      attribute label { text }?,
        ///      undefinedContent
        ///   }
        /// </remarks>
        protected void CategoryWrite(XmlWriter writer, 
            string term, Uri scheme, string label,
            string baseUri, string lang)
        {
            writer.WriteStartElement("category", "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            writer.WriteAttributeString("term", term);

            if (scheme != null)
                writer.WriteAttributeString("scheme", scheme.ToString());

            if (label != null)
                writer.WriteAttributeString("label", label);

            writer.WriteEndElement();
        }
        #endregion // CategoryWrite
        #region LinkWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        /// <param name="type"></param>
        /// <param name="hreflang"></param>
        /// <param name="title"></param>
        /// <param name="length"></param>
        /// <param name="baseUri"></param>
        /// <param name="lang"></param>
        /// <remarks>
        /// atomLink =
        ///   element atom:link {
        ///      atomCommonAttributes,
        ///      attribute href { atomUri },
        ///      attribute rel { atomNCName | atomUri }?,
        ///      attribute type { atomMediaType }?,
        ///      attribute hreflang { atomLanguageTag }?,
        ///      attribute title { text }?,
        ///      attribute length { text }?,
        ///      undefinedContent
        ///   }
        /// </remarks>
        protected void LinkWrite(XmlWriter writer, Uri href,
            string rel, string type, string hreflang, string title, string length,
            string baseUri, string lang)
       {
           if (href == null)
               throw new ArgumentNullException("href");

            writer.WriteStartElement("link", "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            writer.WriteAttributeString("href", href.ToString());

            if (rel!=null && rel!="")
                writer.WriteAttributeString("rel", rel);
            if (type != null && type != "")
                writer.WriteAttributeString("type", type);
            if (hreflang != null && hreflang != "")
                writer.WriteAttributeString("hreflang", hreflang);
            if (title != null && title != "")
                writer.WriteAttributeString("title", title);
            if (length != null && length != "")
                writer.WriteAttributeString("length", length);

            writer.WriteEndElement();
        }
        #endregion // LinkWrite

        #region TextConstructWrite
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
        /// atomPlainTextConstruct =
        ///   atomCommonAttributes,
        ///   attribute type { "text" | "html" }?,
        ///   text
        ///
        /// atomXHTMLTextConstruct =
        ///   atomCommonAttributes,
        ///   attribute type { "xhtml" },
        ///   xhtmlDiv
        ///
        /// atomTextConstruct = atomPlainTextConstruct | atomXHTMLTextConstruct
        /// </remarks>
        protected void TextConstructWrite(XmlWriter writer, string elementName,
            AtomTextType type, string text,
            string baseUri, string lang)
        {
            writer.WriteStartElement(elementName, "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            switch (type)
            {
                case AtomTextType.Text:
                case AtomTextType.Html:
                    writer.WriteAttributeString("type", 
                        (string)((type==AtomTextType.Text)?"text":"html"));
                    writer.WriteString(text);
                    break;
                case AtomTextType.Xhtml:
                    writer.WriteAttributeString("type", "xhtml");
                    writer.WriteRaw(text);
                    break;
            }

            writer.WriteEndElement();
        }

        protected void TextConstructWrite(XmlWriter writer, string elementName,
            string type, string src)
        {
            TextConstructWrite(writer, elementName, type, src, null, null);
        }

        protected void TextConstructWrite(XmlWriter writer, string elementName,
            string type, string src, string baseUri, string lang)
        {
            writer.WriteStartElement(elementName, "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            writer.WriteAttributeString("type", type);
            writer.WriteAttributeString("src", src);

            writer.WriteEndElement();
        }
        #endregion // TextConstructWrite

        #region DateConstructWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="identifier"></param>
        /// <param name="time"></param>
        /// <param name="baseUri"></param>
        /// <param name="lang"></param>
        /// <remarks>
        /// atomDateConstruct =
        ///   atomCommonAttributes,
        ///   xsd:dateTime
        /// </remarks>
        protected void DateConstructWrite(XmlWriter writer,
            string identifier, DateTime time,
            string baseUri, string lang)
        {
            writer.WriteStartElement(identifier, "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            writer.WriteValue(ParseDateSet(time));

            writer.WriteEndElement();
        }
        #endregion // DateWrite
        #region DateConstruct Helper methods
        protected DateTime? ParseDateGet(string value)
        {
            if (value == null)
                return (DateTime?)null;

            return DateTime.Parse(value);

        }

        protected string ParseDateSet(DateTime? dateValue)
        {
            if (!dateValue.HasValue)
                return null;
            string offsetStr = "Z";
            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(dateValue.Value);
            if (offset != TimeSpan.Zero)
            {
                if (offset.Hours < 0)
                    offsetStr = "-";
                else
                    offsetStr = "+";
                offsetStr += offset.Hours.ToString("00") + ":" + offset.Minutes.ToString("00");
            }
            return CH.ConvertToISO8601DateString(dateValue.Value) + offsetStr;
        }
        #endregion // Data Handling methods
        #region PersonConstructWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="identifier"></param>
        /// <param name="name"></param>
        /// <param name="uri"></param>
        /// <param name="email"></param>
        /// <param name="baseUri"></param>
        /// <param name="lang"></param>
        /// <remarks>
        /// atomPersonConstruct =
        ///   atomCommonAttributes,
        ///   (element atom:name { text }
        ///    & element atom:uri { atomUri }?
        ///    & element atom:email { atomEmailAddress }?
        ///    & extensionElement*)
        /// </remarks>
        protected void PersonConstructWrite(XmlWriter writer,
            string identifier, string name, Uri uri, MailAddress email, 
            string baseUri, string lang)
        {
            writer.WriteStartElement(identifier, "http://www.w3.org/2005/Atom");

            CommonAttributesWrite(writer, baseUri, lang);

            writer.WriteElementString("name", name);

            if (uri != null)
                writer.WriteElementString("uri", uri.ToString());

            if (email != null)
                writer.WriteElementString("email", email.Address);

            writer.WriteEndElement();
        }
        #endregion // PersonConstructWrite

        #region CommonAttributesWrite
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="baseUri"></param>
        /// <param name="lang"></param>
        /// <remarks>
        /// atomCommonAttributes =
        ///   attribute xml:base { atomUri }?,
        ///   attribute xml:lang { atomLanguageTag }?,
        ///   undefinedAttribute*
        /// </remarks>
        public void CommonAttributesWrite(XmlWriter writer, string baseUri, string lang)
        {
            if (baseUri != null && baseUri != "")
            {
                writer.WriteAttributeString("xml", "base", "http://www.w3.org/XML/1998/namespace", baseUri);
            }
            if (lang != null && lang != "")
            {
                writer.WriteAttributeString("xml", "lang", "http://www.w3.org/XML/1998/namespace", lang);
            }
        }
        #endregion // CommonAttributesWrite

        #endregion // XmlWriter Helpers
    }
}

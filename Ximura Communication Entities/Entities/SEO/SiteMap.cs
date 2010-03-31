#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// this enumeration defines the change frequencies as specified in the sitemaps.org specification.
    /// </summary>
    public enum SiteMapChangeFrequency
    {
        always,
        hourly,
        daily,
        weekly,
        monthly,
        yearly,
        never
    }

    [XimuraContentTypeID("6348A21B-F8CF-496a-A963-50EBB41DD4A6")]
    [XimuraDataContentDefault("xmrres://XimuraCommEntities/Ximura.Communication.SiteMap/Ximura.Communication.Resources.SiteMap_NewData.xml")]
    [XimuraDataContentSchema("http://www.sitemaps.org/schemas/sitemap/0.9",
      "xmrres://XimuraCommEntities/Ximura.Communication.SiteMap/Ximura.Communication.Resources.SiteMap.xsd")]
    [XimuraContentCachePolicy(ContentCacheOptions.VersionCheck)]
    public class SiteMap : XimuraCore
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SiteMap() 
        { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public SiteMap(SerializationInfo info, StreamingContext context)
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
            mappingShortcuts.Add("r", "//r:urlset");
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

        public bool EntryAdd(Uri location)
        {
            return EntryAdd(location, (DateTime?)null, (SiteMapChangeFrequency?)null, (decimal?)null);
        }

        public bool EntryAdd(Uri location, DateTime? lastModified)
        {
            return EntryAdd(location, lastModified, (SiteMapChangeFrequency?)null, (decimal?)null);
        }

        public bool EntryAdd(Uri location, DateTime? lastModified, SiteMapChangeFrequency? changeFreq, decimal? priority)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    writer.WriteStartElement("url", "http://www.sitemaps.org/schemas/sitemap/0.9");

                        writer.WriteStartElement("loc", "http://www.sitemaps.org/schemas/sitemap/0.9");
                        writer.WriteValue(location.ToString());
                        writer.WriteEndElement();

                        if (lastModified.HasValue)
                        {
                            writer.WriteStartElement("lastmod", "http://www.sitemaps.org/schemas/sitemap/0.9");
                            writer.WriteValue(CH.ConvertToISO8601DateString(lastModified.Value));
                            writer.WriteEndElement();
                        }
                        if (changeFreq.HasValue)
                        {
                            writer.WriteStartElement("changefreq", "http://www.sitemaps.org/schemas/sitemap/0.9");
                            writer.WriteValue(changeFreq.Value.ToString());
                            writer.WriteEndElement();
                        }

                        if (priority.HasValue)
                        {
                            writer.WriteStartElement("priority", "http://www.sitemaps.org/schemas/sitemap/0.9");
                            writer.WriteValue(priority.Value.ToString());
                            writer.WriteEndElement();
                        }

                    writer.WriteEndElement();
                });

            return FragmentSet(XPSc("r", "urlset"), true, frag);
        }

    }
}

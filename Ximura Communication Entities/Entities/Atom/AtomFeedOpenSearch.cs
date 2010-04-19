#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    [XimuraDataContentSchemaReference("http://a9.com/-/spec/opensearch/1.1/",
          "xmrres://XimuraCommEntities/Ximura.Communication.Entities.AtomFeedOpenSearch/Ximura.Communication.Entities.Resources.OpenSearch.xsd")]
    public class AtomFeedOpenSearch: AtomFeed
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AtomFeedOpenSearch() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AtomFeedOpenSearch(SerializationInfo info, StreamingContext context)
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
            mappingShortcuts.Add("os", "//r:feed");
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
        #region NamespaceManagerAdd(XmlNamespaceManager nsm)
        /// <summary>
        /// This override adds the openSearch namespace to the default Namespace manager.
        /// </summary>
        /// <param name="nsm">The system default namespace manager.</param>
        protected override void NamespaceManagerAdd(XmlNamespaceManager nsm)
        {
            base.NamespaceManagerAdd(nsm);
            nsm.AddNamespace("openSearch", "http://a9.com/-/spec/opensearch/1.1/");
            nsm.AddNamespace("osmeta", "http://schema.ximura.org/osmeta/1.0");
        }
        #endregion // NamespaceManagerAdd(XmlNamespaceManager nsm)

        #region OpenSearchTotalResults
        /// <summary>
        /// This is the results total
        /// </summary>
        public int? OpenSearchTotalResults
        {
            get { return XmlMappingGetToInt32Nullable(XPSc("os", "openSearch:totalResults")); }
            set { XmlMappingSet(XPSc("os", "openSearch:totalResults"), value); }
        }
        #endregion // OpenSearchTotalResults
        #region OpenSearchStartIndex
        /// <summary>
        /// This is the results start index
        /// </summary>
        public int? OpenSearchStartIndex
        {
            get { return XmlMappingGetToInt32Nullable(XPSc("os", "openSearch:startIndex")); }
            set { XmlMappingSet(XPSc("os", "openSearch:startIndex"), value); }
        }
        #endregion // OpenSearchStartIndex
        #region OpenSearchItemsPerPage
        /// <summary>
        /// This is the total number of items per page.
        /// </summary>
        public int? OpenSearchItemsPerPage
        {
            get { return XmlMappingGetToInt32Nullable(XPSc("os", "openSearch:itemsPerPage")); }
            set { XmlMappingSet(XPSc("os", "openSearch:itemsPerPage"), value); }
        }
        #endregion // OpenSearchItemsPerPage
        #region OpenSearchAdultContent
        /// <summary>
        /// This flag specifies whether the feed contains adult content.
        /// </summary>
        public virtual bool OpenSearchAdultContent
        {
            get { return XmlMappingGetToBool(XPSc("os", "openSearch:AdultContent")); }
            set { XmlMappingSet(XPSc("os", "openSearch:AdultContent"), value); }
        }
        #endregion // OpenSearchAdultContent

        #region OpenSearchEntryAdd(int relevance, Guid version, string createDate, string title, string atomID)
        /// <summary>
        /// This reduced format entry is used to add cached search results.
        /// </summary>
        /// <param name="relevance">The entry relevance.</param>
        /// <param name="version">The item unique identifier.</param>
        /// <param name="createDate">The entry create date.</param>
        /// <param name="title">The entry title.</param>
        /// <param name="atomID">The entry id.</param>
        public virtual void OpenSearchEntryAdd(int relevance, Guid version, string createDate, 
            string title, string atomID, int? score, string ep1, string ep2, string ep3)
        {
            XmlDocumentFragment frag = this.Payload.CreateDocumentFragment();
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartElement("entry", "http://www.w3.org/2005/Atom");

                writer.WriteAttributeString("xmlns", "osmeta", null, "http://schema.ximura.org/osmeta/1.0");

                if (title != null)
                {
                    writer.WriteStartElement("title");
                    writer.WriteAttributeString("type", "text");
                    writer.WriteString(title);
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("id");
                writer.WriteString(atomID);
                writer.WriteEndElement();

                writer.WriteStartElement("updated");
                writer.WriteString(createDate);
                writer.WriteEndElement();

                writer.WriteStartElement("osmeta", "osversion", "http://schema.ximura.org/osmeta/1.0");
                writer.WriteString(version.ToString().ToUpperInvariant());
                writer.WriteEndElement();

                writer.WriteStartElement("osmeta", "osrelevance", "http://schema.ximura.org/osmeta/1.0");
                writer.WriteString(relevance.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("osmeta", "osscore", "http://schema.ximura.org/osmeta/1.0");
                writer.WriteString(score.HasValue?score.Value.ToString():"0");
                writer.WriteEndElement();

                if (ep1 != null)
                {
                    writer.WriteStartElement("osmeta", "ep1", "http://schema.ximura.org/osmeta/1.0");
                    writer.WriteString(ep1);
                    writer.WriteEndElement();
                }
                if (ep2 != null)
                {
                    writer.WriteStartElement("osmeta", "ep2", "http://schema.ximura.org/osmeta/1.0");
                    writer.WriteString(ep2);
                    writer.WriteEndElement();
                }
                if (ep3 != null)
                {
                    writer.WriteStartElement("osmeta", "ep3", "http://schema.ximura.org/osmeta/1.0");
                    writer.WriteString(ep3);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.Close();
            }

            frag.InnerXml = sb.ToString();

            XmlNode rootNode = this.Payload.SelectSingleNode(XPSc("r"), NSM);
            rootNode.AppendChild(frag);

        }
        #endregion // OpenSearchEntryAdd(string order, string id, string createDate)

        #region Load()
        /// <summary>
        /// This override ensures that the correct type id is set in the base dataset.
        /// </summary>
        /// <returns>Returns true if the records was loaded successfully.</returns>
        public override bool Load()
        {
            bool response = base.Load();
            if (response)
                IDType = GetContentTypeAttributeID();

            return response;
        }
        #endregion // Load()

        public EntryHolder? EntryHolderGet(int relevance)
        {
            XmlNode node = null;
            string path = "//r:feed/r:entry[osmeta:osrelevance='" + relevance.ToString() + "']";
            node = Payload.SelectSingleNode(path, NSM);

            if (node != null)
                return new EntryHolder(node, NSM);

            return null;
        }

        public EntryHolder? EntryHolderGet(Guid versionID)
        {
            XmlNode node = null;
            string path = "//r:feed/r:entry[osmeta:osversion='" + versionID.ToString().ToUpperInvariant() + "']";
            node = Payload.SelectSingleNode(path, NSM);

            if (node != null)
                return new EntryHolder(node, NSM);

            return null;
        }

        public IEnumerable<EntryHolder> EntryHolders
        {
            get
            {
                XmlNodeList nodes = null;
                string path = "//r:feed/r:entry";
                nodes = Payload.SelectNodes(path, NSM);

                foreach (XmlNode node in nodes)
                    yield return new EntryHolder(node, NSM);
            }
        }
    }

    #region Struct --> EntryHolder
    /// <summary>
    /// The entry holder contains the necessary data to retrieve an entry for the feed.
    /// </summary>
    public struct EntryHolder
    {
        /// <summary>
        /// This is the score for the entry item.
        /// </summary>
        public int Score;
        /// <summary>
        /// The relevance order. A lower value is more relevant.
        /// </summary>
        public int Relevance;
        /// <summary>
        /// The version ID of the entry.
        /// </summary>
        public Guid VersionID;
        /// <summary>
        /// The title of the entry. This may be shortened as it is not display but is used for sorting.
        /// </summary>
        public string Title;
        /// <summary>
        /// The uri containing extended information on the entry.
        /// </summary>
        public Uri ID;
        /// <summary>
        /// The create date of the entry. This is used for sorting.
        /// </summary>
        public string CreateDate;

        /// <summary>
        /// Extended property 1
        /// </summary>
        public string Ep1;
        /// <summary>
        /// Extended property 2
        /// </summary>
        public string Ep2;
        /// <summary>
        /// Extended property 3
        /// </summary>
        public string Ep3;

        /// <summary>
        /// This constructor sets the relevant values from the collection node.
        /// </summary>
        /// <param name="node">The collection node.</param>
        /// <param name="NSM">The collection namespace manager.</param>
        public EntryHolder(XmlNode node, XmlNamespaceManager NSM)
        {
            Relevance = int.Parse(node.SelectSingleNode("osmeta:osrelevance", NSM).InnerText);
            Score = int.Parse(node.SelectSingleNode("osmeta:osscore", NSM).InnerText);
            VersionID = new Guid(node.SelectSingleNode("osmeta:osversion", NSM).InnerText);
            ID = new Uri(node.SelectSingleNode("r:id", NSM).InnerText);
            CreateDate = node.SelectSingleNode("r:updated", NSM).InnerText;

            XmlNode title = node.SelectSingleNode("r:title", NSM);
            Title = title != null ? title.InnerText : null;

            XmlNode nep1 = node.SelectSingleNode("osmeta:ep1", NSM);
            Ep1 = nep1 != null ? nep1.InnerText : null;

            XmlNode nep2 = node.SelectSingleNode("osmeta:ep2", NSM);
            Ep2 = nep2 != null ? nep2.InnerText : null;

            XmlNode nep3 = node.SelectSingleNode("osmeta:ep3", NSM);
            Ep3 = nep3 != null ? nep3.InnerText : null;
        }
    }
    #endregion // EntryHolder

    #region PaginationHolder
    /// <summary>
    /// This class provides a convenient pagination holder, that implemenents from the 
    /// base poolable object.
    /// </summary>
    public class PaginationHolder : PoolableReturnableObjectBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. It is called when the object is created by the pool.
        /// Poolable objects must implement a public default constructor.
        /// </summary>
        public PaginationHolder()
            : base()
        {
        }
        #endregion // Constructor

        public virtual int Score(EntryHolder eh, int totalRecords)
        {
            return eh.Relevance;
        }

        public virtual bool SupportsScoring
        {
            get
            {
                return false;
            }
        }
    }
    #endregion // PaginationHolder

    #region PaginationFilterSortType
    /// <summary>
    /// This is the sort type for the pagination filter.
    /// </summary>
    public enum PaginationFilterSortType
    {
        /// <summary>
        /// The results are sorted by the relevance calculation.
        /// </summary>
        Relevance,
        /// <summary>
        /// The results are sorted by the alphabetical title name.
        /// </summary>
        Alphabetical,
        /// <summary>
        /// The results are sorted by the create date.
        /// </summary>
        CreateDate
    }
    #endregion // PaginationFilterSortType

    #region PaginationFilter
    /// <summary>
    /// The pagination filter class contains the filter parameters for the pagination.
    /// You can override this class to provide specific filter options.
    /// </summary>
    public class PaginationFilter : PoolableReturnableObjectBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. It is called when the object is created by the pool.
        /// Poolable objects must implement a public default constructor.
        /// </summary>
        public PaginationFilter()
            : base()
        {
        }
        #endregion // Constructor
        #region Reset()
        /// <summary>
        /// This override resets the filter to its default values.
        /// </summary>
        public override void Reset()
        {
            SortAscending = false;
            SortType = PaginationFilterSortType.Relevance;
            Start = 0;
            Total = 0;
            PerPage = 0;
            Path = null;
            Host = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Start
        /// <summary>
        /// The pagination start parameter.
        /// </summary>
        public virtual int Start { get; set; }
        #endregion // Start
        #region Total
        /// <summary>
        /// The pagination total records.
        /// </summary>
        public virtual int Total { get; set; }
        #endregion // Total
        #region PerPage
        /// <summary>
        /// The pagination records per page.
        /// </summary>
        public virtual int PerPage { get; set; }
        #endregion // PerPage

        #region Path
        /// <summary>
        /// The request path
        /// </summary>
        public virtual Uri Path { get; set; }
        #endregion // PerPage

        #region Host
        public virtual void HostSet(string path)
        {
            Host = new Uri(path);
        }
        /// <summary>
        /// The request host
        /// </summary>
        public virtual Uri Host { get; private set; }
        #endregion // PerPage

        #region FormatPagination(int page)
        protected virtual Uri FormatPagination(int page)
        {
            UriBuilder builder = new UriBuilder(Host);
            if (builder.Query != null && builder.Query != "")
            {
                string item = builder.Query + "&p=" + page.ToString();

                if (item.StartsWith("?"))
                    item = item.Substring(1);
                builder.Query = item;
            }
            else
                builder.Query = "p=" + page.ToString();

            return builder.Uri;
        }
        #endregion // FormatPagination(int page)

        #region HostFirst
        /// <summary>
        /// The request host
        /// </summary>
        public virtual Uri HostFirst 
        {
            get
            {
                int p = 1;
                return FormatPagination(p);
            }
        }
        #endregion
        #region HostNext
        /// <summary>
        /// The request host
        /// </summary>
        public virtual Uri HostNext 
        {
            get
            {
                int p = Start + PerPage;
                if (p >= Total)
                    p = Total - (Total % PerPage) + 1;
                if (p == Total)
                    p = Total - PerPage + 1;
                if (p <1)
                    p = 1;

                return FormatPagination(p);
            }
        }
        #endregion
        #region HostPrevious
        /// <summary>
        /// The request host
        /// </summary>
        public virtual Uri HostPrevious 
        {
            get
            {
                int p = Start - PerPage;
                if (p>Total)
                    p = Total - PerPage + 1;
                if (p < 1)
                    p = 1;

                return FormatPagination(p);
            }
        }
        #endregion
        #region HostLast
        /// <summary>
        /// The request host
        /// </summary>
        public virtual Uri HostLast 
        {
            get
            {
                int p = Total - (Total % PerPage) + 1;
                if (p == Total)
                    p = Total - PerPage +1;
                if (p < 1)
                    p = 1;

                return FormatPagination(p);
            }
        }
        #endregion

        #region SupportsPagination
        /// <summary>
        /// This method returns true if the filter supports pagination.
        /// </summary>
        public bool SupportsPagination
        {
            get
            {
                return Host != null;
            }
        }
        #endregion // SupportsPagination

        #region SortAscending
        /// <summary>
        /// This method determines whether the sort order is ascending. The default is true.
        /// </summary>
        public virtual bool SortAscending { get; set; }
        #endregion // SortAscending
        #region SortType
        /// <summary>
        /// This is the sort type for the pagination records.
        /// </summary>
        public virtual PaginationFilterSortType SortType { get; set; }

        #endregion // SortType

        #region PaginationData
        /// <summary>
        /// This method returns an enumeration of EntryHolder structures based on the filter parameters.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="holder"></param>
        /// <returns></returns>
        public IEnumerable<EntryHolder> PaginationData(
            AtomFeedOpenSearch collection, PaginationHolder holder)
        {
            IEnumerable<EntryHolder> results;

            if (holder != null && holder.SupportsScoring)
                results =
                    collection.EntryHolders
                    .Where(d => (holder.Score(d, collection.OpenSearchTotalResults.Value) >= Start))
                    .OrderBy(d => holder.Score(d, collection.OpenSearchTotalResults.Value))
                    .Take(PerPage);
            else
                results =
                    collection.EntryHolders
                    .Where(d => (d.Relevance >= Start))
                    .Take(PerPage);

            return results;
        }
        #endregion // PaginationData

    }
    #endregion // PaginationFilter

}

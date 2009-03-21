#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class contains the configuration for the SiteControllerLoggerCommand.
    /// </summary>
    [XimuraContentTypeID("BF0EB25E-31C1-4d17-806A-DA6DFD290D2C")]
    [XimuraDataContentSchema("http://schema.ximura.org/configuration/command/1.0",
        "xmrres://Ximura/Ximura.Command.CommandConfiguration/Ximura.Command.Configuration.CommandConfiguration.xsd")]
    public class SiteControllerLoggerConfiguration : CommandConfiguration<SiteControllerTimerPollJob>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SiteControllerLoggerConfiguration() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public SiteControllerLoggerConfiguration(System.ComponentModel.IContainer container)
            :
            base(container) 
        {
        }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public SiteControllerLoggerConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
    }

    #region Class -> SiteControllerTimerPollJob
    /// <summary>
    /// This override adds the specific tasks for the SiteControllerLogger.
    /// </summary>
    public class SiteControllerTimerPollJob : TimerPollJob
    {
        #region Constructor
        /// <summary>
        /// This constructor sets the specific properties for the poll job request.
        /// </summary>
        /// <param name="data">The data node.</param>
        /// <param name="NSM">The namespace manager.</param>
        public SiteControllerTimerPollJob(){}
        #endregion // Constructor

        #region Configure(XmlElement element, XmlNamespaceManager NSM, object subCommand)
        /// <summary>
        /// This override sets the extended parameters for the timer poll job.
        /// </summary>
        /// <param name="element">The xml configuration element.</param>
        /// <param name="NSM">The namespace manager.</param>
        /// <param name="subCommand">The subcommand object.</param>
        public override void Configure(XmlElement element, XmlNamespaceManager NSM, object subCommand)
        {
            base.Configure(element, NSM, subCommand);
            RequestFormat = null;
            switch (ID)
            {
                case "cacheprune":
                    Subcommand = SiteControllerLoggerPollJob.CachePrune;
                    RequestFormat = FormatCachePrune;
                    break;
                case "datareload":
                    Subcommand = SiteControllerLoggerPollJob.DataReload;
                    RequestFormat = FormatDataReload;
                    break;
                case "dblogqueue":
                    Subcommand = SiteControllerLoggerPollJob.DBLogQueue;
                    break;
                case "dbupdate":
                    Subcommand = SiteControllerLoggerPollJob.DBUpdate;
                    break;
                default:
                    Subcommand = null;
                    break;
            }
        }
        #endregion // Configure(XmlElement element, XmlNamespaceManager NSM, object subCommand)

        #region FormatCachePrune(RQRSFolder folder)
        /// <summary>
        /// This method sets the request properties for the cache prune timer poll job.
        /// </summary>
        /// <param name="folder">The request folder to configure.</param>
        protected virtual void FormatCachePrune(RQRSFolder folder)
        {
            SiteControllerLoggerRequest rqLogger = folder as SiteControllerLoggerRequest;
            rqLogger.CachePruneMax = int.Parse(mValues["prunemax"]);
            rqLogger.CachePrunePercent = int.Parse(mValues["prunepercent"]);
        }
        #endregion // FormatCachePrune(RQRSFolder folder)

        #region FormatDataReload(RQRSFolder folder)
        /// <summary>
        /// This method sets the properties for the NIC data reload timer poll job.
        /// </summary>
        /// <param name="folder">The request folder to configure.</param>
        protected virtual void FormatDataReload(RQRSFolder folder)
        {
            SiteControllerLoggerRequest rqLogger = folder as SiteControllerLoggerRequest;
            List<KeyValuePair<string, Uri>> registries = new List<KeyValuePair<string, Uri>>();

            try
            {
                mValues.ForEach(v => registries.Add(new KeyValuePair<string, Uri>(v.Key, new Uri(v.Value))));
            }
            catch (Exception ex)
            {
                registries.Clear();
            }

            rqLogger.Registries = registries;
        }
        #endregion // FormatDataReload(RQRSFolder folder)

    }
    #endregion // Class -> SiteControllerTimerPollJob


    #region Enum --> SiteControllerLoggerPollJob
    /// <summary>
    /// This enumeration contains the polling jobs currently supported by the command.
    /// </summary>
    public enum SiteControllerLoggerPollJob
    {
        /// <summary>
        /// The cache pruning.
        /// </summary>
        CachePrune,
        /// <summary>
        /// The database log process empties the logging queue and persists it to the database.
        /// </summary>
        DBLogQueue,
        /// <summary>
        /// The database update.
        /// </summary>
        DBUpdate,
        /// <summary>
        /// The data reload.
        /// </summary>
        DataReload
    }
    #endregion // Enum --> PollJob

}

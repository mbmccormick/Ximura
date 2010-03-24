#region using
using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This command resolves country locations from their IP addresses, and logs requests to the site controller to the data store.
    /// </summary>
    [XimuraAppModule(SiteControllerLoggerCommand.ID, SiteControllerLoggerCommand.Name)]
    [XimuraAppConfiguration(ConfigurationLocation.Resource,
        "xmrres://XimuraComm/Ximura.Communication.SiteControllerLoggerCommand/Ximura.Communication.Commands.SiteControllerLogger.Configuration.SiteControllerLoggerConfiguration_Default.xml")]
    public class SiteControllerLoggerCommand : AppCommandProcess<SiteControllerLoggerRequest, SiteControllerLoggerResponse, 
        RQRSFolder, RQRSFolder, SiteControllerLoggerConfiguration, CommandPerformance>
    {
        #region Declarations
        #region Constants
        /// <summary>
        /// The command ID
        /// </summary>
        public const string ID = "B8ABB1A1-98A2-419b-A0AD-CB70BAB8FF01";
        /// <summary>
        /// The command name.
        /// </summary>
        public const string Name = "SiteControllerLoggerCommand";
        #endregion // Constants
        /// <summary>
        /// This class contains the ipresolver class and is also used to provides the IServiceIPAddressLocationResolver service.
        /// </summary>
        protected InternalServiceRedirection ResolverService = null;
        #endregion // Declarations
        #region Constructors
		/// <summary>
		/// This is the empty constructor
		/// </summary>
		public SiteControllerLoggerCommand():this((IContainer)null){}
		/// <summary>
		/// This is the constrcutor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The command container.</param>
        public SiteControllerLoggerCommand(System.ComponentModel.IContainer container)
            : base(container)
		{
		}
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public SiteControllerLoggerCommand(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
		#endregion

        #region MAIN ENTRY POINT --> ProcessRequest
        /// <summary>
        /// This method processes the incoming requests.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request/response data to process.</param>
        protected override void ProcessRequest(SecurityManagerJob job,
            RQRSContract<SiteControllerLoggerRequest, SiteControllerLoggerResponse> Data)
        {
            if (Data.DestinationAddress.SubCommand is SiteControllerLoggerPollJob)
            {
                SiteControllerLoggerPollJob pollJob =
                    (SiteControllerLoggerPollJob)Data.DestinationAddress.SubCommand;

                switch (pollJob)
                {
                    case SiteControllerLoggerPollJob.CachePrune:
                        PR_PruneCache(job, Data);
                        return;
                    case SiteControllerLoggerPollJob.DBUpdate:
                        PR_DBUpdate(job, Data);
                        return;
                    case SiteControllerLoggerPollJob.DBLogQueue:
                        PR_DBLogQueue(job, Data);
                        return;
                    case SiteControllerLoggerPollJob.DataReload:
                        PR_DataReload(job, Data);
                        return;
                }
            }

            PR_ResolveAddress(job, Data);
        }
        #endregion // ProcessRequest
        #region SERVICE ENTRY POINT --> ServiceResolveAddress(IPAddress address, out string isoCountryCode)
        /// <summary>
        /// This class is used to provide the IServiceIPAddressLocationResolver service to the application.
        /// </summary>
        protected class InternalServiceRedirection : ISiteControllerLogger
        {
            #region Declarations
            private ReaderWriterLockSlim mLock;
            private IPLocationResolver mResolver = null;

            private Queue<SiteControllerRequestInfo> mLoggingQueue = null;

            /// <summary>
            /// this object is used to sync the logging queue.
            /// </summary>
            private object syncQueue = new object();

            private bool mQueueOverflow = false;
            private int mQueueHighWaterMark;

            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This constructor creates the internal logging objects.
            /// </summary>
            /// <param name="resolver"></param>
            public InternalServiceRedirection(int queueHighWaterMark, IPLocationResolver resolver)
            {
                mQueueHighWaterMark = queueHighWaterMark;
                mLock = new ReaderWriterLockSlim();
                mResolver = resolver;
                mLoggingQueue = new Queue<SiteControllerRequestInfo>();
            }
            #endregion // InternalServiceRedirection

            #region Resolver
            /// <summary>
            /// This property contains the location resolver collection.
            /// </summary>
            public IPLocationResolver Resolver 
            {
                get
                {
                    try
                    {
                        mLock.EnterReadLock();
                        return mResolver;
                    }
                    finally
                    {
                        mLock.ExitReadLock();
                    }
                }
                set
                {
                    try
                    {
                        mLock.EnterWriteLock();
                        mResolver = value;
                    }
                    finally
                    {
                        mLock.ExitWriteLock();
                    }

                }
            }
            #endregion // Resolver

            #region IAddressResolution Members
            public IPLocationResolver.ResolverResponse CacheResolveAddress(IPAddress address, out string isoCountryCode)
            {
                try
                {
                    mLock.EnterReadLock();
                    if (mResolver != null)
                    {
                        try
                        {
                            return mResolver.CacheResolveAddress(address, out isoCountryCode);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    isoCountryCode = null;
                    return IPLocationResolver.ResolverResponse.IPAddressError;
                }
                finally
                {
                    mLock.ExitReadLock();
                }
            }

            public bool ResolveAddress(IPAddress address, out string isoCountryCode)
            {
                return CacheResolveAddress(address, out isoCountryCode) == IPLocationResolver.ResolverResponse.OK;
            }

            #endregion

            #region QueueOverflowPoll()
            /// <summary>
            /// This property identifies whether the queue overflowed since the last poll and resets the poll value to false.
            /// </summary>
            /// <returns></returns>
            public bool QueueOverflowPoll()
            {
                lock (syncQueue)
                {
                    bool oldVal = mQueueOverflow;
                    mQueueOverflow = false;
                    return oldVal;
                }
            }
            #endregion // QueueOverflowPoll()

            #region LogRequestEnqueue/LogRequestDequeue
            /// <summary>
            /// This method adds the request info to the queue.
            /// </summary>
            /// <param name="info">The info to log.</param>
            public void LogRequestEnqueue(SiteControllerRequestInfo info)
            {
                lock (syncQueue)
                {
                    //Stop the queue from overflowing.
                    while (mLoggingQueue.Count >= mQueueHighWaterMark)
                    {
                        SiteControllerRequestInfo oldInfo = mLoggingQueue.Dequeue();
                        mQueueOverflow = true;
                        if (oldInfo != null && oldInfo.ObjectPoolCanReturn)
                            oldInfo.ObjectPoolReturn();
                    }

                    mLoggingQueue.Enqueue(info);
                }
            }
            /// <summary>
            /// This method removes a log request from the queue.
            /// </summary>
            /// <returns>Returns a info object or null if the queue is empty.</returns>
            public SiteControllerRequestInfo LogRequestDequeue()
            {
                lock (syncQueue)
                {
                    if (mLoggingQueue.Count == 0)
                        return null;

                    return mLoggingQueue.Dequeue();
                }
            }
            #endregion
        }
        #endregion

        #region ENTRY POINT --> PR_ResolveAddress
        /// <summary>
        /// This method resolves the IPAddress passed to it.
        /// </summary>
        /// <param name="job">the current job.</param>
        /// <param name="Data">The data to process</param>
        protected virtual void PR_ResolveAddress(SecurityManagerJob job,
            RQRSContract<SiteControllerLoggerRequest, SiteControllerLoggerResponse> Data)
        {
            SiteControllerLoggerRequest rq = Data.ContractRequest;
            SiteControllerLoggerResponse rs = Data.ContractResponse;

            IPAddress address = rq.Address;

            string isoCode;
            int status = (int)ResolverService.CacheResolveAddress(address, out isoCode);

            rs.Status = status.ToString();
            if (status == 200)
                rs.CountryCode = isoCode;
        }
        #endregion // PR_ResolveAddress
        #region ENTRY POINT --> PR_PruneCache
        /// <summary>
        /// This method prunes the cache in the resolver.
        /// </summary>
        /// <param name="job">the current job.</param>
        /// <param name="Data">The data to process</param>
        protected virtual void PR_PruneCache(SecurityManagerJob job,
            RQRSContract<SiteControllerLoggerRequest, SiteControllerLoggerResponse> Data)
        {
            SiteControllerLoggerRequest rq = Data.ContractRequest;
            SiteControllerLoggerResponse rs = Data.ContractResponse;

            try
            {
                if (ResolverService.Resolver == null)
                {
                    rs.Status = CH.HTTPCodes.NotAcceptable_406;
                }
                else
                {
                    int records = ResolverService.Resolver.CachePrune(
                        rq.CachePruneMax.Value, rq.CachePrunePercent.Value);

                    if (records > 0)
                    {
                        XimuraAppTrace.WriteLine(
                            string.Format("{0} cache records were removed from the IP location cache.", records)
                            , this.CommandName, EventLogEntryType.Information, "EventLog");
                    }
                    rs.Status = CH.HTTPCodes.OK_200;
                }
            }
            catch (Exception ex)
            {
                rs.Status = CH.HTTPCodes.InternalServerError_500;
                rs.Substatus = ex.Message;
            }
        }
        #endregion // PR_PruneCache
        #region ENTRY POINT --> PR_DBUpdate (not implemented)
        /// <summary>
        /// This method is not implemented. You should override this method in your own command.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="Data">The job data,</param>
        protected virtual void PR_DBUpdate(SecurityManagerJob job,
            RQRSContract<SiteControllerLoggerRequest, SiteControllerLoggerResponse> Data)
        {
            SiteControllerLoggerRequest rq = Data.ContractRequest;
            SiteControllerLoggerResponse rs = Data.ContractResponse;

            rs.Status = CH.HTTPCodes.NotImplemented_501;
        }
        #endregion // PR_DBUpdate
        #region ENTRY POINT --> PR_DBLogQueue
        /// <summary>
        /// This method is not implemented. You should override this method in your own command.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="Data">The job data,</param>
        protected virtual void PR_DBLogQueue(SecurityManagerJob job,
            RQRSContract<SiteControllerLoggerRequest, SiteControllerLoggerResponse> Data)
        {
            SiteControllerLoggerRequest rq = Data.ContractRequest;
            SiteControllerLoggerResponse rs = Data.ContractResponse;

            //set a loop maximum counter to stop a never ending loop, which could happen if exceptions are thrown.
            int loopMax = 500000;

            if (ResolverService.QueueOverflowPoll())
            {
                XimuraAppTrace.WriteLine("The logging queue has overflowed. Logging data has been lost.", this.CommandName, EventLogEntryType.Warning);
            }

            while (true)
            {
                SiteControllerRequestInfo info = null;

                try
                {
                    info = ResolverService.LogRequestDequeue();
                    if (info == null)
                        break;

                    LogRequest(info);
                }
                finally
                {
                    if (info != null && info.ObjectPoolCanReturn)
                        info.ObjectPoolReturn();
                }
                loopMax--;

                if (loopMax <= 0)
                {
                    XimuraAppTrace.WriteLine("PR_DBLogQueue: loopMax has been reached.", this.CommandName, EventLogEntryType.Warning);
                    break;
                }
            }

            rs.Status = CH.HTTPCodes.OK_200;
        }
        #endregion // PR_DBLogQueue
        #region ENTRY POINT --> PR_DataReload
        /// <summary>
        /// This request reloads the NIC registry data from the remote NIC registries.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="Data">The job data,</param>
        protected virtual void PR_DataReload(SecurityManagerJob job,
            RQRSContract<SiteControllerLoggerRequest, SiteControllerLoggerResponse> Data)
        {
            SiteControllerLoggerRequest rq = Data.ContractRequest;
            SiteControllerLoggerResponse rs = Data.ContractResponse;

            IPLocationResolver newIPResolver = null;

            try
            {
                newIPResolver = new IPLocationResolver();
                List<IPLocationResolver.NICLoadReport> results = new List<IPLocationResolver.NICLoadReport>();

                WebClient client = new WebClient();
                DateTime start = DateTime.Now;
                foreach (var item in rq.Registries)
                {
                    using (Stream dataStream = client.OpenRead(item.Value))
                    {
                        results.Add(newIPResolver.Load(item.Key, dataStream));
                    }
                }

                TimeSpan length = DateTime.Now - start;

                ResolverService.Resolver = newIPResolver;

                DataLoadLog(results, start);

            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine(string.Format("NIC registry load has failed: /r/n{0}", ex.Message)
                    , this.CommandName, EventLogEntryType.Warning);

                rs.Status = CH.HTTPCodes.InternalServerError_500;
                rs.Substatus = ex.Message;
                return;
            }

            rs.Status = CH.HTTPCodes.OK_200;
        }
        #endregion // PR_DataReload

        #region LogRequest(SiteControllerRequestInfo info)
        /// <summary>
        /// This methdo logs the info record to the appropriate persistence store. 
        /// You can override this method to add additional stores.
        /// By default this method calls the SQLWrite_SiteControllerRequestInfo method.
        /// </summary>
        /// <param name="info">The info to log.</param>
        protected virtual void LogRequest(SiteControllerRequestInfo info)
        {
            SQLWrite_SiteControllerRequestInfo(info);
        }
        #endregion // LogRequest(SiteControllerRequestInfo info)

        #region LoggingDBConnection
        /// <summary>
        /// This is the conection to the loggin database. You should override this method.
        /// </summary>
        public virtual string LoggingDBConnection
        {
            get
            {
                throw new NotImplementedException("SiteControllerLoggerCommand/LoggingDBConnection is not implemented");
            }
        }
        #endregion // LoggingDBConnection
        #region SQLWrite_SiteControllerRequestInfo
        /// <summary>
        /// This method writes the request info to the database.
        /// </summary>
        /// <param name="sr">The SiteControllerRequestInfo object.</param>
        /// <returns>Returns true if the record was written successfully.</returns>
        protected virtual bool SQLWrite_SiteControllerRequestInfo(SiteControllerRequestInfo sr)
        {
            return SQLWrite_SiteControllerRequestInfo(LoggingDBConnection, sr);
        }
        /// <summary>
        /// This method writes the request info to the database.
        /// </summary>
        /// <param name="dbConn">The SQL connection string.</param>
        /// <param name="sr">The SiteControllerRequestInfo object.</param>
        /// <returns>Returns true if the record was written successfully.</returns>
        protected virtual bool SQLWrite_SiteControllerRequestInfo(string dbConn, SiteControllerRequestInfo sr)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConn))
                {
                    conn.Open();

                    SqlCommand sqlCmd = new SqlCommand("spx_LogRequest", conn);
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    sqlCmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = sr.DateTimeLog.Value;
                    sqlCmd.Parameters.Add("@ClientAddress", SqlDbType.VarChar, 15).Value = sr.AddressClient.Address.ToString();
                    sqlCmd.Parameters.Add("@ClientAddressPort", SqlDbType.Int).Value = sr.AddressClient.Port;
                    sqlCmd.Parameters.Add("@ServerAddress", SqlDbType.VarChar, 15).Value = sr.AddressServer.Address.ToString();
                    sqlCmd.Parameters.Add("@ServerAddressPort", SqlDbType.Int).Value = sr.AddressServer.Port;
                    sqlCmd.Parameters.Add("@UserName", SqlDbType.VarChar, 150).Value = sr.UserName;
                    sqlCmd.Parameters.Add("@ServiceName", SqlDbType.VarChar, 50).Value = sr.ServiceName;
                    sqlCmd.Parameters.Add("@ServerName", SqlDbType.VarChar, 50).Value = sr.ServerName;
                    sqlCmd.Parameters.Add("@Method", SqlDbType.VarChar, 250).Value = sr.ProtocolMethod;
                    sqlCmd.Parameters.Add("@URIStem", SqlDbType.VarChar, 250).Value = sr.URIStem;
                    sqlCmd.Parameters.Add("@URIQuery", SqlDbType.VarChar, 500).Value = sr.URIQuery;
                    sqlCmd.Parameters.Add("@Status", SqlDbType.VarChar, 3).Value = sr.ProtocolStatus;
                    sqlCmd.Parameters.Add("@BytesSent", SqlDbType.BigInt).Value = sr.BytesSent;
                    sqlCmd.Parameters.Add("@BytesReceived", SqlDbType.BigInt, 50).Value = sr.BytesReceived;
                    sqlCmd.Parameters.Add("@TimeTaken", SqlDbType.BigInt).Value = sr.TimeTaken;
                    sqlCmd.Parameters.Add("@ProtocolVersion", SqlDbType.VarChar, 50).Value = sr.ProtocolVersion;
                    sqlCmd.Parameters.Add("@Host", SqlDbType.VarChar, 250).Value = sr.Host;
                    sqlCmd.Parameters.Add("@UserAgent", SqlDbType.VarChar, 250).Value = sr.UserAgent;
                    sqlCmd.Parameters.Add("@Cookie", SqlDbType.VarChar, 150).Value = sr.Cookie;
                    sqlCmd.Parameters.Add("@Referer", SqlDbType.VarChar, 500).Value = sr.Referer;
                    sqlCmd.Parameters.Add("@Debug", SqlDbType.VarChar, 500).Value = sr.Debug;

                    sqlCmd.Parameters.Add("@BrowserID", SqlDbType.UniqueIdentifier).Value = sr.BrowserID.HasValue ? (object)sr.BrowserID.Value : System.DBNull.Value;
                    sqlCmd.Parameters.Add("@PageID", SqlDbType.UniqueIdentifier).Value = sr.PageID.HasValue ? (object)sr.PageID.Value : System.DBNull.Value;
                    sqlCmd.Parameters.Add("@SessionID", SqlDbType.UniqueIdentifier).Value = sr.SessionID.HasValue ? (object)sr.SessionID.Value : System.DBNull.Value;
                    sqlCmd.Parameters.Add("@UserID", SqlDbType.VarChar, 50).Value = sr.UserID;
                    sqlCmd.Parameters.Add("@MappingID", SqlDbType.VarChar, 50).Value = sr.MappingID;
                    sqlCmd.Parameters.Add("@ISOCountryCode", SqlDbType.VarChar, 5).Value = sr.ISOCountryCode;

                    sqlCmd.ExecuteNonQuery();
                    conn.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion // SiteControllerRequestInfo_SQLWrite(string dbConn, SiteControllerRequestInfo sr)

        #region InternalStart()
        /// <summary>
        /// This override starts the command and loads the IP data.
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();
            long start = Environment.TickCount;
            DataLoad();
            long length = Environment.TickCount-start;

            //IPTests();
        }

        private void IPTests()
        {
            try
            {
                string isoCountryCode;
                //199.43.183.195
                ResolverService.ResolveAddress(IPAddress.Parse("199.43.183.195"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("209.234.171.41"), out isoCountryCode);

                ResolverService.ResolveAddress(IPAddress.Parse("192.96.243.44"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("192.96.244.44"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("192.96.245.44"), out isoCountryCode);

                ResolverService.ResolveAddress(IPAddress.Parse("212.149.48.44"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("212.149.148.216"), out isoCountryCode);

                ResolverService.ResolveAddress(IPAddress.Parse("10.0.0.52"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("24.254.10.157"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("74.6.18.247"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("218.102.221.225"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("210.17.242.1"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("62.56.96.200"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("93.97.181.247"), out isoCountryCode);
                ResolverService.ResolveAddress(IPAddress.Parse("135.196.224.164"), out isoCountryCode);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion // InternalStart()
        #region InternalStop()
        /// <summary>
        /// This method stops the command and unloads the data.
        /// </summary>
        protected override void InternalStop()
        {
            DataUnload();
            base.InternalStop();
        }
        #endregion // InternalStop()

        #region LogQueueMaxSize
        /// <summary>
        /// This is the maximum size for the logging queue. After the queue reaches this number of records, old data will be discarded.
        /// </summary>
        protected virtual int LogQueueMaxSize
        {
            get
            {
                return 100000;
            }
        }
        #endregion // LogQueueMaxSize

        #region ServicesProvide()
        /// <summary>
        /// This override adds the IServiceIPAddressLocationResolver sercvice.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            if (ResolverService == null)
                ResolverService = new InternalServiceRedirection(LogQueueMaxSize, null);

            AddService(typeof(ISiteControllerLogger), ResolverService, true);
        }
        #endregion // ServicesProvide()
        #region ServicesRemove()
        /// <summary>
        /// This override removes the IServiceIPAddressLocationResolver service.
        /// </summary>
        protected override void ServicesRemove()
        {
            RemoveService(typeof(ISiteControllerLogger));

            base.ServicesRemove();
        }
        #endregion // ServicesRemove()

        #region DataLoad()
        /// <summary>
        /// This method loads the initial IPaddress data
        /// </summary>
        protected virtual void DataLoad()
        {
            IPLocationResolver ipResolver = new IPLocationResolver();
            List<IPLocationResolver.NICLoadReport> results = new List<IPLocationResolver.NICLoadReport>();
            DateTime start = DateTime.Now;

            results.Add(DataProcessResource(ipResolver, "apnic"));
            results.Add(DataProcessResource(ipResolver, "arin"));
            results.Add(DataProcessResource(ipResolver, "lacnic"));
            results.Add(DataProcessResource(ipResolver, "ripencc"));
            results.Add(DataProcessResource(ipResolver, "afrinic"));
            results.Add(DataProcessResource(ipResolver, "iana"));

            //set the service resolver to the new resolver.
            ResolverService.Resolver = ipResolver;

            DataLoadLog(results, start);
        }
        #endregion // DataLoad()
        #region DataUnload()
        /// <summary>
        /// This method unloads the data.
        /// </summary>
        protected virtual void DataUnload()
        {
            ResolverService.Resolver.Clear();
            ResolverService.Resolver = null;
        }
        #endregion // DataUnload()
        #region DataLoadLog(IEnumerable<IPLocationResolver.NICLoadReport> results, DateTime startTime)
        /// <summary>
        /// This method logs the load results to the logging providers.
        /// </summary>
        /// <param name="results">the results collection</param>
        protected virtual void DataLoadLog(IEnumerable<IPLocationResolver.NICLoadReport> results, DateTime startTime)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("NIC registry load at {0}", startTime);
            sb.AppendLine();
            sb.AppendLine();

            results.ForEach(r => r.ToString(sb));

            sb.AppendLine();
            sb.AppendFormat("Completed at {0}", DateTime.Now);
            sb.AppendLine();

            XimuraAppTrace.WriteLine(sb.ToString(), this.CommandName, EventLogEntryType.Information, "EventLog");
        }
        #endregion // DataLoadLog(IEnumerable<IPLocationResolver.NICLoadReport> results, DateTime startTime)

        #region DataProcessResource(string filepart)
        /// <summary>
        /// This method processes a particular ipaddress resource.
        /// </summary>
        /// <param name="filepart">The filepart name.</param>
        protected virtual IPLocationResolver.NICLoadReport DataProcessResource(IPLocationResolver ipResolver, string filepart)
        {
            try
            {
                Uri location = new Uri(string.Format(
                    "xmrres://AegeaConnectCMS/Aegea.Connect.SiteControllerLoggerCommand/Aegea.Connect.Commands.SiteControllerLogger.Data.{0}",
                        filepart));
                using (Stream dataStrm = RH.ResourceLoadFromUriAsStream(location))
                {
                    return ipResolver.Load(filepart, dataStrm);
                }
            }
            catch (Exception ex){}

            return null;
        }
        #endregion // DataProcessResource(string filepart) 
    }
}

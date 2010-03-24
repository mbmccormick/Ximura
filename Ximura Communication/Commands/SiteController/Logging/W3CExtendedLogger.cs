#region using
using System;
using System.Runtime.Serialization;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class is used to log site manager protocol request.
    /// </summary>
    public class W3CExtendedLogger : LoggerAgentBase
    {
        private string path = null;
        private string dbConn = null;

        private bool canWriteDB = false;
        private bool canWriteFile = false;

        private DateTime? dbConnHoldOff;
        private int dbFileCount = 0;
        private object syncHoldOff = new object();

        public override void Initialize(IXimuraLoggerSettings settings)
        {
            base.Initialize(settings);

            path = settings.GetSetting("filepath");
            dbConn = settings.GetSetting("dbconnection");

            canWriteDB = dbConn != null && dbConn != "";
            canWriteFile = path != null && path != "";
        }

        public override void Write(string message)
        {
            //Do nothing
        }

        public override void WriteLine(string message)
        {
            //Do nothing
        }

        public override void Write(object o, string category)
        {
            if (!(o is SiteControllerRequestInfo))
                return;

            WriteInternal((SiteControllerRequestInfo)o);
        }

        public override void Write(object o)
        {
            if (!(o is SiteControllerRequestInfo))
                return;

            WriteInternal((SiteControllerRequestInfo)o);
        }

        private void WriteInternal(SiteControllerRequestInfo sr)
        {
            if (canWriteFile)
                WriteFile(sr);

            if (canWriteDB)
                WriteDB(sr);
        }

        private void WriteFile(SiteControllerRequestInfo sr)
        {

        }
        private void WriteDB(SiteControllerRequestInfo sr)
        {
            lock (syncHoldOff)
            {
                if (dbConnHoldOff.HasValue)
                    if (dbConnHoldOff.Value > DateTime.Now)
                    {
                        return;
                    }
                    else
                        dbConnHoldOff = null;
            }

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
                }
            }
            catch (Exception ex)
            {
                lock (syncHoldOff)
                {
                    dbFileCount++;
                    dbConnHoldOff = DateTime.Now.AddMinutes(2);
                    if (dbFileCount > 100)
                        canWriteDB = false;
                }
            }
        }

        public override void WriteLine(object o)
        {
            Write(o);
        }

        #region AcceptCategory(string category, EventLogEntryType type)
        /// <summary>
        /// This method inform the Logging Manager whether it will accept the 
        /// category for logging.
        /// </summary>
        /// <param name="category">The logging category.</param>
        /// <param name="type">The envent log entry type category.</param>
        /// <returns>A boolean value. True indicates the category is accepted.</returns>
        public override bool AcceptCategory(string category, EventLogEntryType type)
        {
            return category=="W3C";
        }
        #endregion // AcceptCategory(string category, EventLogEntryType type)

    }
}

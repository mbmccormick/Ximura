#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
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
    public partial class IPLocationResolver
    {
        public class NICLoadReport
        {
            private class NICTotal
            {
                public int Total;
                public int Count;

                public NICTotal(int total)
                {
                    Total = total;
                    Count = 0;
                }
            }

            public int Records { get; protected set; }
            public string Registry { get; protected set; }
            public string SerialID { get; protected set; }
            public DateTime EndDate { get; protected set; }

            private Dictionary<string, NICTotal> itemRecords;

            public void ParseHeader(string line)
            {
                string[] item = line.Split(new char[] { '|' }, StringSplitOptions.None);
                Registry = item[1];
                SerialID = item[2];
                EndDate = DateTime.Parse(string.Format("{0}-{1}-{2}"
                    , item[5].Substring(0,4)
                    , item[5].Substring(4,2)
                    , item[5].Substring(6,2)
                    ));
                int noRecords;
                if (int.TryParse(item[3], out noRecords))
                    Records = noRecords;

                itemRecords = new Dictionary<string, NICTotal>();
            }

            public void ParseSummary(string line)
            {
                string[] item = line.Split(new char[] { '|' }, StringSplitOptions.None);


                itemRecords.Add(item[2], new NICTotal(int.Parse(item[4])));
            }

            public void AddRecord(string id)
            {
                itemRecords[id].Count = itemRecords[id].Count + 1;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                ToString(sb);

                return sb.ToString();
            }

            public virtual void ToString(StringBuilder sb)
            {
                sb.AppendFormat("{0} {1} ({2} records) - {3:yyyy-MM-dd}"+Environment.NewLine, Registry, SerialID, Records, EndDate);
                itemRecords.ForEach(r => sb.AppendFormat("   {0} -> {1}/{2}" + Environment.NewLine, r.Key, r.Value.Total, r.Value.Count));
            }
        }

        #region Load(string filepart, Stream strmData)
        /// <summary>
        /// This method reads through each line in the stream and processes it.
        /// </summary>
        /// <param name="id">The NIC id.</param>
        public virtual NICLoadReport Load(string id, Stream strmData)
        {
            DisposeCheck();

            NICLoadReport report = new NICLoadReport();

            if (strmData == null)
                throw new ArgumentNullException("strmData is null");

            if (strmData.CanRead)
            {
                using (StreamReader sr = new StreamReader(strmData))
                {
                    bool firstLineProcessed = false;

                    while (strmData.CanRead && !sr.EndOfStream)
                    {
                        try
                        {
                            string line = sr.ReadLine();

                            if (line == null)
                                break;

                            //Skip commented lines
                            if (line.StartsWith("#"))
                                continue;

                            //Skip empty lines
                            if (line.Trim() == "")
                                continue;

                            //Process the first line.
                            if (!firstLineProcessed)
                            {
                                firstLineProcessed = true;
                                report.ParseHeader(line);
                                continue;
                            }

                            DataProcessLine(line, report);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            return report;
        }
        #endregion // DataProcessStream(string filepart, Stream strmData)


        #region DataProcessLine(string line, NICLoadReport report)
        /// <summary>
        /// This method processes each line.
        /// </summary>
        /// <param name="line">The data line.</param>
        /// <param name="report">The load report.</param>
        /// <returns>Returns true if the line was processes successfully.</returns>
        protected virtual void DataProcessLine(string line, NICLoadReport report)
        {
            try
            {
                string[] item = line.Split(new char[] { '|' }, StringSplitOptions.None);

                if (item[1] == "*")
                {
                    report.ParseSummary(line);
                    return;
                }

                if (item.Length < 4)// || item[6]=="ieft")
                    return;

                bool success = false;

                switch (item[2])
                {
                    case "ipv4":
                        //case "ipv6":
                        success = DataProcessRecord(item[3], item[1], item[4]);
                        break;
                }

                if (success)
                    report.AddRecord(item[2]);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // DataProcessLine(string line)
        #region DataProcessRecord(string ipAddress, string isoCountryCode, string netMask)
        /// <summary>
        /// This method processes both IPV4 and IPV6 addresses and loads them in to the collection.
        /// </summary>
        /// <param name="ipAddress">The IP address to load.</param>
        /// <param name="isoCountryCode">The country code for the address.</param>
        /// <param name="netMask">The netmask.</param>
        protected virtual bool DataProcessRecord(string ipAddress, string isoCountryCode, string netMask)
        {
            IPAddress address;
            if (!IPAddress.TryParse(ipAddress, out address))
                return false;

            Add(address, isoCountryCode, netMask);
            return true;
        }
        #endregion
    }
}

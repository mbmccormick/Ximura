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
        #region Class -> IPCache
        /// <summary>
        /// This structure holds the results for resolved IP addresses.
        /// </summary>
        protected class IPCache
        {
            public readonly IPAddress Address;
            public readonly string ISOCountryCode;
            public readonly bool Resolved;

            public long HitCount;

            public DateTime LastHit { get; private set; }

            public IPCache(IPAddress Address, string ISOCountryCode, bool Resolved)
            {
                this.Address = Address;
                this.ISOCountryCode = ISOCountryCode;
                this.Resolved = Resolved;

                this.HitCount = 1;
                this.LastHit = DateTime.Now;
            }

            public void Increment()
            {
                Interlocked.Increment(ref HitCount);
                LastHit = DateTime.Now;
            }

            /// <summary>
            /// The Weight value is used to determine which records should be removed from the collection.
            /// </summary>
            public long Weight
            {
                get
                {
                    TimeSpan span = DateTime.Now - LastHit;
                    return ((long)span.TotalMilliseconds) / HitCount;
                }
            }
        }
        #endregion // IPCache

        #region Declarations
        private Dictionary<IPAddress, IPCache> mCache = null;
        private object syncIPCache = new object();
        #endregion // Declarations

        #region CacheConstruct()
        /// <summary>
        /// This method initializes the cache and is called by the constructor.
        /// </summary>
        protected virtual void CacheConstruct()
        {
            if (mCache == null)
                mCache = new Dictionary<IPAddress, IPCache>();
        }
        #endregion // CacheConstruct()

        #region Enum --> ResolveResponse
        /// <summary>
        /// This is the response code for the CacheResolveAddress.
        /// </summary>
        public enum ResolverResponse : int
        {
            /// <summary>
            /// The IP address is null or the wrong type.
            /// </summary>
            IPAddressError = 400,
            /// <summary>
            /// The IP address cannot be resolved.
            /// </summary>
            NotFound = 404,
            /// <summary>
            /// Success.
            /// </summary>
            OK = 200,
        }
        #endregion // Enum --> ResolveResponse

        #region CacheResolveAddress(IPAddress address, out string isoCountryCode)
        /// <summary>
        /// This method resolves the IPAddress with the internal collection and caches common results.
        /// </summary>
        /// <param name="address">The address to resolve.</param>
        /// <param name="isoCountryCode">The out parameter containing the country code, or null if the address cannot be resolved.</param>
        /// <returns>Returns 200 if the request is successful.</returns>
        public ResolverResponse CacheResolveAddress(IPAddress address, out string isoCountryCode)
        {
            DisposeCheck();

            if (address == null || address.AddressFamily != AddressFamily.InterNetwork)
            {
                isoCountryCode = null;
                return ResolverResponse.IPAddressError;
            }

            IPCache cache = null;
            bool isCached = false;

            lock (syncIPCache)
            {
                if (mCache.ContainsKey(address))
                {
                    cache = mCache[address];
                    cache.Increment();
                    isCached = true;
                }
            }

            if (cache == null)
                cache = ResolveAddress(address);

            if (!isCached)
                lock (syncIPCache)
                {
                    if (!mCache.ContainsKey(address))
                    {
                        mCache.Add(address, cache);
                    }
                }

            if (cache.Resolved)
            {
                isoCountryCode = cache.ISOCountryCode;
                return ResolverResponse.OK;
            }
            else
            {
                isoCountryCode = null;
                return ResolverResponse.NotFound;
            }
        }
        #endregion // ResolveAddress(IPAddress address, out string isoCountryCode)
        #region CachePrune/CachePrune(int percent)
        /// <summary>
        /// This method prunes the cache and removes old records
        /// </summary>
        /// <returns>Returns the number of records pruned from the collection.</returns>
        public int CachePrune()
        {
            return CachePrune(100, 10);
        }
        /// <summary>
        /// this method prunes the cache and removes old records
        /// </summary>
        /// <param name="maxRecords">the maximum number of records permitted in the cache.</param>
        /// <param name="percent">
        /// The prune percent. This is the percentage of records to free up under the maxrecords value.
        /// e.g. if the collection holds 11000 records and the maxRecords=10000 and the prunePercent = 10
        /// the CachePrune command will first remove the oldest top 1000 records and then remove a further 10% of the remaining records.
        /// If however the collection had under 9000 records, then nothing would be pruned from the collection.
        /// </param>
        /// <returns>Returns the number of records pruned from the collection.</returns>
        public int CachePrune(int maxRecords, int prunePercent)
        {
            DisposeCheck();

            if (maxRecords < 0 || prunePercent < 0)
                throw new ArgumentOutOfRangeException();

            int newRecords = maxRecords - (((maxRecords * 100) / prunePercent) / 100);
            if (newRecords == 0)
                newRecords = maxRecords;

            lock (syncIPCache)
            {
                int removeCount = mCache.Count - newRecords;
                //Check whether we have anything to prune.
                if (removeCount<=0)
                    return 0 ;

                //Find the records that need to be removed.
                IPCache[] toRemove = mCache.Values.OrderByDescending(c => c.Weight).Take(removeCount).ToArray();
                //remove the records from the cache.
                toRemove.ForEach(c => mCache.Remove(c.Address));

                return toRemove.Length;
            }
        }
        #endregion // CachePrune/CachePrune(int percent)

        #region ResolveAddress(IPAddress address)
        /// <summary>
        /// This method resolves the IPAddress from the collection. See RFC 3330.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected virtual IPCache ResolveAddress(IPAddress address)
        {
            if (IPAddress.IsLoopback(address))
                return new IPCache(address, "--LBK", true);

            byte[] byaddress = address.GetAddressBytes();

            string isoCountryCode;
            if (IsIPV4RestrictedNetwork(byaddress, out isoCountryCode) 
                || Resolve(address, out isoCountryCode))
                return new IPCache(address, isoCountryCode, true);

            return new IPCache(address, "", false);
        }
        #endregion // DataResolveAddress(IPAddress address)
        #region IsIPV4RestrictedNetwork(byte[] address)
        /// <summary>
        /// This method resolves the address and checks whether it is within private address space.
        /// </summary>
        /// <param name="address">The address bytes to resolve.</param>
        /// <returns>Returns true if the address is private.</returns>
        protected bool IsIPV4RestrictedNetwork(byte[] address, out string network)
        {
            network = "";
            if (address.Length != 4)
                return false;

            switch (address[0])
            {
                case 10:
                    network = "--P24";
                    return true;
                case 127:
                    network = "--LBK";
                    return true;
                case 169:
                    network = "--LLC";
                    return address[1] == 254;
                case 172:
                    network = "--P20";
                    return address[1] >= 16 && address[1] <= 31;
                case 192:
                    network = "--P16";
                    return address[1] == 168;
                case 224:
                    network = "--MUL";
                    return true;
                case 240:
                    network = "--RES";
                    return true;
                default:
                    return false;
            }
        }
        #endregion // IsIPV4PrivateNetwork(byte[] address)

    }
}

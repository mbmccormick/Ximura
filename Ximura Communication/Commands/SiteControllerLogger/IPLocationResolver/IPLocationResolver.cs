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

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class loads the data from the registry.
    /// </summary>
    public partial class IPLocationResolver: IDisposable
    {
        #region Declaration
        private bool mDisposed;
        private object msyncDisposed = new object();

        private Dictionary<AddressFamily, IPLocationNode> mRootNodeCollection;
        private Dictionary<byte,string> mLocationCollection;
        #endregion // Declaration
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public IPLocationResolver()
        {
            mDisposed = false;

            mRootNodeCollection = new Dictionary<AddressFamily, IPLocationNode>();

            mLocationCollection = new Dictionary<byte,string>();
            mLocationCollection.Add((byte)0, "");

            CacheConstruct();
        }
        #endregion // Constructor

        #region IDisposable Members
        /// <summary>
        /// This method disposes of the class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// This method provides specific dispose code for the collection.
        /// </summary>
        /// <param name="disposing">Set to true if this is the first time this is called.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (msyncDisposed)
            {
                if (!mDisposed && disposing)
                {

                    Clear();
                    mDisposed = true;
                }
            }
        }

        #region DisposeCheck()
        /// <summary>
        /// This method checks whether the collection has been disposed, and if so throws an exception.
        /// </summary>
        protected void DisposeCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException("IPLocationResolver");
        }
        #endregion // DisposeCheck()
        #endregion

        #region Add(IPAddress address, string isoCountryCode, string netMask)
        /// <summary>
        /// This method adds a specific record data from the Registry file.
        /// </summary>
        /// <param name="address">The IP address.</param>
        /// <param name="isoCountryCode">The country code as it appears in the database.</param>
        /// <param name="netMask">The netmask string as it appears in the database.</param>
        public void Add(IPAddress address, string isoCountryCode, string netMask)
        {
            DisposeCheck();


            if (!mLocationCollection.ContainsValue(isoCountryCode))
            {
                mLocationCollection.Add(((byte)mLocationCollection.Count), isoCountryCode);
            }

            byte key = mLocationCollection.Where(k => k.Value == isoCountryCode).Select(k => k.Key).Single();


            IPLocationNode rootNode;

            RegistryRecordParser data = new RegistryRecordParser(address, netMask);

            if (!mRootNodeCollection.ContainsKey(address.AddressFamily))
            {
                rootNode = new IPLocationNode();
                mRootNodeCollection.Add(address.AddressFamily, rootNode);
            }
            else
                rootNode = mRootNodeCollection[address.AddressFamily];

            rootNode.Add(data, key);
        }
        #endregion // Add(IPAddress address, string isoCountryCode, string netMask)

        #region Contains(IPAddress address)
        /// <summary>
        /// This method resolves the IPaddress but does not return the country code.
        /// </summary>
        /// <param name="address">The address to resolve.</param>
        /// <returns>Returns true if the address was resolved successfully.</returns>
        public bool Contains(IPAddress address)
        {
            DisposeCheck();
            if (!mRootNodeCollection.ContainsKey(address.AddressFamily))
            {
                return false;
            }

            IPLocationNode rootNode = mRootNodeCollection[address.AddressFamily];

            return rootNode.Contains(address.GetAddressBytes());
        }
        #endregion // Contains(IPAddress address)
        #region Resolve(IPAddress address, out string isoCountryCode)
        /// <summary>
        /// This method attemps to resolve an IP address and returns the ISO county code.
        /// </summary>
        /// <param name="address">The address to resolve.</param>
        /// <param name="isoCountryCode">The ISO country code, or null if the address cannot be resolved.</param>
        /// <returns>Returns true if the address was resolved successfully.</returns>
        public bool Resolve(IPAddress address, out string isoCountryCode)
        {
            DisposeCheck();
            if (!mRootNodeCollection.ContainsKey(address.AddressFamily))
            {
                isoCountryCode = null;
                return false;
            }

            IPLocationNode rootNode = mRootNodeCollection[address.AddressFamily];
            byte key;
            bool success = rootNode.Resolve(address.GetAddressBytes(), out key);
            isoCountryCode = mLocationCollection[key];
            return success;
        }
        #endregion // Resolve(IPAddress address, out string isoCountryCode)

        #region Clear()
        /// <summary>
        /// this method resets all IPLocationNodes and clears to collection, which allows the GC to recycle them.
        /// </summary>
        public void Clear()
        {
            DisposeCheck();
            mRootNodeCollection.Values.ForEach(d => d.Reset());
            mRootNodeCollection.Clear();
            mRootNodeCollection = null;
            mCache.Clear();
            mCache = null;

        }
        #endregion // Clear()
    }
}

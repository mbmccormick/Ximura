#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ResourceManagerSettings : ContextSettings<ResourceManagerState, ResourceManagerConfiguration, ResourceManagerPerformance>
    {
        #region Class -> ETagHolder
        /// <summary>
        /// This class holds the ETag information.
        /// </summary>
        protected class ETagHolder
        {
            public Guid VID;
            public Guid CID;
            public DateTime? Expiry;
            public Type ContentType;
            public int? ExpiryTimeinSeconds;

            internal ETagHolder(Guid VID, Guid CID, DateTime? Expiry, Type contentType, int? expiryTimeinSeconds)
            {
                this.VID = VID;
                this.CID = CID;
                this.Expiry = Expiry;
                this.ContentType = contentType;
                this.ExpiryTimeinSeconds = expiryTimeinSeconds;
            }
        }
        #endregion // ETagHolder
        #region Declaration
        private Dictionary<Guid, ETagHolder> mCacheCollection;
        private object syncObject = new object();
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This constructor is called by the FSM when initiating the settings.
        /// </summary>
        public ResourceManagerSettings()
            : base()
        {
            mCacheCollection = new Dictionary<Guid, ETagHolder>();
        }
        #endregion


        public void ETagCollectionClear()
        {
            lock (syncObject)
            {
                mCacheCollection.Clear();
            }
        }

        public void ETagExpiryReset(Guid ETag)
        {
            lock (syncObject)
            {
                if (mCacheCollection.ContainsKey(ETag))
                {
                    ETagHolder holder = mCacheCollection[ETag];
                    if (holder.ExpiryTimeinSeconds.HasValue)
                        holder.Expiry = DateTime.Now.AddSeconds(holder.ExpiryTimeinSeconds.Value);
                }
            }
        }

        public bool ETagValidate(Guid ETag, out Guid? cid, out DateTime? expiry, out Type contentType)
        {
            lock (syncObject)
            {
                ETagHolder holder = null;
                bool match = mCacheCollection.ContainsKey(ETag);
                if (match)
                {
                    holder = mCacheCollection[ETag];
                    cid = holder.CID;
                    expiry = holder.Expiry;
                    contentType = holder.ContentType;
                }
                else
                {
                    cid = null;
                    expiry = null;
                    contentType = null;
                }
                return match;
            }
        }

        public void ETagAdd(Guid ETag, Guid cid, DateTime? expiry, Type contentType, int? expiryTimeinSeconds)
        {
            lock (syncObject)
            {
                ETagHolder holder = new ETagHolder(ETag, cid, expiry, contentType, expiryTimeinSeconds);
                if (!mCacheCollection.ContainsKey(ETag))
                    mCacheCollection.Add(ETag, holder);
                else
                    mCacheCollection[ETag] = holder;
            }
        }

        public bool ETagCacheFlush(Guid cid)
        {
            bool success = false;
            try
            {
                lock (syncObject)
                {
                    var holders = mCacheCollection.Values.Where(h => h.CID == cid).ToList();
                    foreach (var holder in holders)
                    {
                        success = true;
                        mCacheCollection.Remove(holder.VID);
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

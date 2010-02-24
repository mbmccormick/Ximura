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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Text;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;


using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    #region CacheItemContainer
    /// <summary>
    /// This class structure is used to hold a cache entry and its 
    /// associated statistics.
    /// </summary>
    public class CacheItemContainer
    {
        #region Declarations
        protected ContentIdentifier mID;
        protected DateTime? mLastAccessed = null;
        protected byte[] mBlob;
        protected string mFilePath;
        protected long mTotalHitCount;
        protected long mCacheCount = 0;
        protected DateTime? mExpiryDateTime = null;
        protected DateTime mCreateDateTime;
        protected object syncLock = new object();
        protected long mSize = 0;
        protected bool mIsInterned = false;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The cache item container is used to hold an internal entity in to the cache memory queue.
        /// </summary>
        /// <param name="id">The content identifier.</param>
        /// <param name="blob">The binary blob.</param>
        /// <param name="hitCount">The historical hit count for the item.</param>
        public CacheItemContainer(ContentIdentifier id, byte[] blob, long hitCount)
            : this(id, blob, null, hitCount){}
        /// <summary>
        /// The cache item container is used to hold an internal entity in to the cache memory queue.
        /// </summary>
        /// <param name="id">The content identifier.</param>
        /// <param name="path">The relative file path.</param>
        /// <param name="hitCount">The historical hit count for the item.</param>
		public CacheItemContainer(ContentIdentifier id, string path, long hitCount)
            : this(id, null, path, hitCount){}
        /// <summary>
        /// The cache item container is used to hold an internal entity in to the cache memory queue.
        /// </summary>
        /// <param name="id">The content identifier.</param>
        /// <param name="blob">The binary blob.</param>
        /// <param name="path">The relative file path.</param>
        /// <param name="hitCount">The historical hit count for the item.</param>
		public CacheItemContainer(ContentIdentifier id, byte[] blob, string path, long hitCount)
        {
            this.mID = id;
            this.mBlob = blob;
            this.mFilePath = path;
            this.mTotalHitCount = hitCount;
            mCreateDateTime = DateTime.Now;
        }
        #endregion // Constructors

        #region Identifier
        /// <summary>
        /// This is the identifier.
        /// </summary>
        public ContentIdentifier Identifier
        {
            get
            {
                return mID;
            }
        }
        #endregion // Identifier
        #region VersionID
        /// <summary>
        /// This is the version ID of the content from the identifier.
        /// </summary>
        public Guid VersionID
        {
            get
            {
                return mID.VersionID;
            }
        }
        #endregion // VersionID

        #region Blob
        /// <summary>
        /// This is the binary blob for the content.
        /// </summary>
        public virtual byte[] Blob
        {
            get
            {
                return mBlob;
            }
            //set
            //{
            //    lock (syncLock)
            //    {
            //        mBlob = value;
            //    }
            //}
        }
        #endregion // Blob
        #region FilePath
        /// <summary>
        /// This is the filepath.
        /// </summary>
        public string FilePath
        {
            get
            {
                return mFilePath;
            }
            set
            {
                mFilePath = value;
            }
        }
        #endregion // FilePath
        #region Size
        /// <summary>
        /// This is the size of the cache item.
        /// </summary>
        public long Size
        {
            get
            {
                lock (syncLock)
                {
                    if (IsInterned)
                        return Blob.Length;
                    return mSize;
                }
            }
            set
            {
                mSize = value;
            }
        }
        #endregion // Size
        #region IsInterned
        /// <summary>
        /// This method determines whether the cached item is loaded in to the class in memory.
        /// </summary>
        public bool IsInterned
        {
            get
            {
                return mIsInterned;
            }
        }
        #endregion // IsInterned

        #region CacheHitUpdate()
        /// <summary>
        /// This method updates the cache statistics.
        /// </summary>
        public void CacheHit()
        {
            lock (syncLock)
            {
                CacheHitInternal();
            }
        }
        /// <summary>
        /// This protected method is used to update the cache settings. This method does not modify and thread locks.
        /// </summary>
        protected void CacheHitInternal()
        {
            TimeSpan stale = StaleTime;
            mLastAccessed = DateTime.Now;
            mCacheCount++;
            UpdateWaitTime((decimal)stale.TotalMilliseconds);
        }
        #endregion // CacheHit()
        #region CacheRevalidate()
        /// <summary>
        /// This method revalidates the cached items for the cache expiry time.
        /// </summary>
        public void CacheRevalidate()
        {
            lock (syncLock)
            {
                mExpiryDateTime = DateTime.Now.AddSeconds(CacheExpiry);
            }
        }
        #endregion
        #region CacheOptions
        /// <summary>
        /// This is the cache options for the content.
        /// </summary>
        public virtual ContentCacheOptions CacheOptions
        {
            get;
            protected set;
        }
        #endregion // CacheOptions
        #region CacheExpiry
        /// <summary>
        /// This is the cache options for the content.
        /// </summary>
        public virtual int CacheExpiry
        {
            get;
            protected set;
        }
        #endregion // CacheOptions

        #region AverageWait
        decimal mAverageWait = -1;
        private void UpdateWaitTime(decimal waitTime)
        {
            if (CacheCount <= 1)
            {
                mAverageWait = waitTime;
                return;
            }

            decimal cc=(decimal)CacheCount;

            //Calculate the new average wait time before the cache is accessed.
            mAverageWait = ((mAverageWait * (cc - 1)) + waitTime) / cc;
        }
        /// <summary>
        /// This is the average wait time between cache hits in milliseconds. -1 determines that the content item
        /// has never been hit.
        /// </summary>
        public long AverageWait
        {
            get
            {
                return (long)mAverageWait;
            }
        }
        #endregion // AverageWait
        #region StaleTime
        /// <summary>
        /// The stale time is the length of time since the cache item was last used as a TimeSpan object.
        /// </summary>
        public TimeSpan StaleTime
        {
            get
            {
                if (!mLastAccessed.HasValue)
                    return DateTime.Now.Subtract(mCreateDateTime);

                return DateTime.Now.Subtract(mLastAccessed.Value);
            }
        }
        #endregion // StaleTime
        #region CacheCount
        /// <summary>
        /// The cache count determines the number of times that the cache has been hit during the 
        /// current service run time.
        /// </summary>
        public long CacheCount
        {
            get
            {
                return mCacheCount;
            }
        }
        #endregion // CacheCount
        #region TotalHitCount
        /// <summary>
        /// The total hit count is the maximum number of hit for the content within the lifetime
        /// of the cache collection.
        /// </summary>
        public long TotalHitCount
        {
            get
            {
                return mCacheCount + mTotalHitCount;
            }
        }
        #endregion // TotalHitCount
        #region LastAccessed
        /// <summary>
        /// This is the time that the cache has last been accessed.
        /// If the cache item has never been accessed, this value is null.
        /// </summary>
        public DateTime? LastAccessed
        {
            get
            {
                if (mCacheCount == 0)
                    return null;
                return mLastAccessed;
            }
        }
        #endregion // LastAccessed
        #region ExpiryDateTime
        /// <summary>
        /// This is the expiry date time of the cache item. If this is null, the item does not 
        /// have an expiry date.
        /// </summary>
        public DateTime? ExpiryDateTime
        {
            get
            {
                return mExpiryDateTime;
            }
            set
            {
                mExpiryDateTime = value;
            }
        }
        #endregion // ExpiryDateTime

        #region HasExpired
        /// <summary>
        /// This is the expiry time for the content.
        /// </summary>
        public bool CacheHasExpired
        {
            get
            {
                lock (syncLock)
                {
                    return (mExpiryDateTime.HasValue && mExpiryDateTime.Value < DateTime.Now);
                }
            }
        }
        #endregion // HasExpired

        #region ExpiryRating
        /// <summary>
        /// This is the expiry rating for the cacheitem. You should override this class if you wish a more finegrained ranking algorithm.
        /// </summary>
        public virtual decimal ExpiryRating
        {
            get
            {
                if (!IsInterned)
                    return long.MinValue;

                long thc = TotalHitCount;
                if (thc == 0)
                    return long.MaxValue;

                long awt = AverageWait;
                if (awt <= 0)
                    return long.MaxValue;

                long sz = Size;
                if (sz == 0)
                    return long.MinValue;


                return sz*awt/thc;
            }
        }
        #endregion // ExpiryRating


        public static bool Serialize(CacheItemContainer Container, Guid ApplicationID, DirectoryInfo dInfo)
        {
            Rfc2898DeriveBytes pwdGen =
                new Rfc2898DeriveBytes(Container.Identifier.CombinedID, ApplicationID.ToByteArray(), 1000);

            RijndaelManaged rjn = new RijndaelManaged();

            string dir = dInfo.FullName;

            using (ICryptoTransform eTransform = rjn.CreateEncryptor(pwdGen.GetBytes(32), pwdGen.GetBytes(16)))
            {
                using (FileStream ms = new FileStream(Path.Combine(dir, Container.Identifier.CreateItemIDString()), FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(ms, eTransform, CryptoStreamMode.Write))
                    {
                        int value = Container.mBlob.Length;
                        cs.WriteByte((byte)value);
                        cs.WriteByte((byte)(value >> 8));
                        cs.WriteByte((byte)(value >> 0x10));
                        cs.WriteByte((byte)(value >> 0x18));
                        cs.Write(Container.mBlob, 0, value);
                        cs.FlushFinalBlock();
                    }
                }
            } 
            
            return true;
        }

        public static byte[] Deserialize(ContentIdentifier Identifier, Guid ApplicationID, DirectoryInfo dInfo)
        {
            Rfc2898DeriveBytes pwdGen =
                new Rfc2898DeriveBytes(Identifier.CombinedID, ApplicationID.ToByteArray(), 1000);

            RijndaelManaged rjn = new RijndaelManaged();

            string dir = dInfo.FullName;

            using (ICryptoTransform eTransform = rjn.CreateDecryptor(pwdGen.GetBytes(32), pwdGen.GetBytes(16)))
            {
                using (FileStream ms = new FileStream(Path.Combine(dir, Identifier.CreateItemIDString()), FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(ms, eTransform, CryptoStreamMode.Read))
                    {
                        int len = (((cs.ReadByte() | (cs.ReadByte() << 8)) | (cs.ReadByte() << 0x10)) | (cs.ReadByte() << 0x18));

                        byte[] data = new byte[len];
                        cs.Read(data, 0, data.Length);

                        return data;
                    }
                }
            }
        }
    }
    #endregion // CacheItemContainer
    #region CacheItemContainer<C>
    /// <summary>
    /// This is the generic container used to hold content in the Ximura system.
    /// </summary>
    /// <typeparam name="C">The content type.</typeparam>
    public class CacheItemContainer<C> : CacheItemContainer
        where C : Content
    {
        #region Declarations
        /// <summary>
        /// This is the content item.
        /// </summary>
        protected C mItem;
        /// <summary>
        /// This is the symetric algorithm used to safely store the content in the file system.
        /// </summary>
        protected SymmetricAlgorithm alg = null;
        /// <summary>
        /// This is the directory information for the folder containing the cache items.
        /// </summary>
        protected DirectoryInfo dInfo = null;

        protected ContentCacheOptions? mContentCacheOptions = null;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The cache item container is used to hold an internal entity in to the cache memory queue.
        /// </summary>
        /// <param name="data">The content.</param>
        /// <param name="hitCount">The historical hit count for the item.</param>
        public CacheItemContainer(C data, bool internData, long hitCount)
            : this(data, internData, null, hitCount) { }

        /// <summary>
        /// The cache item container is used to hold an internal entity in to the cache memory queue.
        /// </summary>
        /// <param name="data">The content.</param>
        /// <param name="path">The relative file path.</param>
        /// <param name="hitCount">The historical hit count for the item.</param>
        public CacheItemContainer(C data, bool internData, string path, long hitCount) :
            base(new ContentIdentifier(data),null,null,hitCount)
        {
            if (data == null)
                throw new ArgumentNullException("data", "The content cannot be null");

            CacheOptions = data.CacheOptions;
            CacheExpiry = data.CacheExpiry;

            mIsInterned = internData;
            if (mIsInterned)
            {
                mItem = null;
                mBlob = Content.SerializeEntity(data);
            }
            else
                mItem = data;

            mLastAccessed = DateTime.Now;
            mExpiryDateTime = mLastAccessed.Value.AddSeconds(CacheExpiry);
        }
        #endregion // Constructors

        #region Item
        /// <summary>
        /// This property returns the cached item.
        /// </summary>
        public C Item
        {
            get
            {
                if (IsInterned)
                    throw new NotSupportedException("Item is not supported when the data is interned. You can get a copy from ItemCloned");

                lock (syncLock)
                {
                    CacheHitInternal();
                    return mItem;
                }
            }
        }
        #endregion // Item
        #region ItemCloned()
        /// <summary>
        /// This property returns the cached item.
        /// </summary>
        public C ItemCloned(IXimuraPoolManager pMan)
        {
            lock (syncLock)
            {
                CacheHitInternal();
                if (IsInterned)
                    return (C)Content.DeserializeEntity(this.Blob, pMan);
                else
                    return (C)mItem.Clone();
            }
        }
        #endregion // Item

        #region CacheClear()
        /// <summary>
        /// This method clears the cached content item and returns it to the pool.
        /// </summary>
        public virtual void CacheClear()
        {
            lock (syncLock)
            {
                if (mItem != null && mItem.ObjectPoolCanReturn)
                    mItem.ObjectPoolReturn();

                mItem = null;
                mBlob = null;
            }
        }
        #endregion
    }
    #endregion // CacheItemContainer<C>
}

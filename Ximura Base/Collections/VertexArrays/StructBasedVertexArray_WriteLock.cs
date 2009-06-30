#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// For more details see http://ximura.org
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text;

using Ximura;
using Ximura.Helper;
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
//    public abstract partial class StructBasedVertexArray<T>
//    {
//        #region Static declarations
//        /// <summary>
//        /// This value holds an individual incremental ID for each thread.
//        /// </summary>
//        [ThreadStatic]
//        static int sLockIndex;
//        /// <summary>
//        /// This value holds the maximum number of thread registered with the collection.
//        /// </summary>
//        static int sLockCounter = 0;
//        #endregion // Static declarations
//        #region Declarations
//        /// <summary>
//        /// This array holds the write thread status.
//        /// </summary>
//        private int[] mWriteThreadStatus;
//        /// <summary>
//        /// This is the current lock position which allows threads to enter and exit in order.
//        /// </summary>
//        private int mWriteLockPosition = 0;
//        /// <summary>
//        /// This property specifies whether threads can modify data in the collection.
//        /// </summary>
//        private volatile int mWriteLock;
//        #endregion // Declarations

//        #region MaxSupportedConcurrentThreads
//        /// <summary>
//        /// This protected property returns the maximum supported number of threads. You should override this
//        /// method if you require the collection to support more threads.
//        /// </summary>
//        protected virtual int MaxSupportedConcurrentThreads { get { return Environment.ProcessorCount * 25; } }
//        #endregion // MaxConcurrentThreads

//        #region WriteLockInitialize()
//        /// <summary>
//        /// This method initializes the write lock logic.
//        /// </summary>
//        private void WriteLockInitialize()
//        {
//            mWriteThreadStatus = new int[MaxSupportedConcurrentThreads];
//            mWriteLock = 0;
//        }
//        #endregion // WriteLockInitialize()

//        #region WriteLockAcquire()
//        /// <summary>
//        /// This method acquires the write lock.
//        /// </summary>
//        private void WriteLockAcquire()
//        {
//            if (mWriteLock == 1)
//                Console.WriteLine("ERROR: Multiple accesses of WriteLockAcquire()");
//#if (DEBUG)
//            Console.WriteLine("{0} WriteLock: Waiting for Write Lock...", Interlocked.Increment(ref mDebugCounter));
//#endif
//            //Set the lock to stop new threads from entering.
//            mWriteLock = 1;
//            mWriteLockPosition = 0;

//#if (DEBUG)
//            Console.WriteLine("{0} WriteLock: Flag set to 1", Interlocked.Increment(ref mDebugCounter));
//#endif
//            //Wait for all threads to leave the critical section.
//            int id = sLockCounter;
//            int count = 0;
//            for (; --id >= 0; )
//            {
//                count++;
//#if (DEBUG)
//                Console.WriteLine("{0} WriteLock: Checking thread {1} => {2}", Interlocked.Increment(ref mDebugCounter), id, mWriteThreadStatus[id]);
//#endif
//                if (mWriteThreadStatus[id] == 1)
//                {
//                    ThreadingHelper.ThreadWait();
//                    id = sLockCounter;
//                    continue;
//                }
//            }
//#if (DEBUG)
//            Console.WriteLine("{0} WriteLock: Write Lock acquired in {1} hops", Interlocked.Increment(ref mDebugCounter), count);
//#endif
//        }
//        #endregion // WriteLockAcquire()
//        #region WriteLockRelease()
//        /// <summary>
//        /// This method releases the write lock.
//        /// </summary>
//        private void WriteLockRelease()
//        {
//            //Unset the lock. Threads can now enter write sections.
//            mWriteLock = 0;
//#if (DEBUG)
//            Console.WriteLine("{0} WriteLock: Write Lock Released.", Interlocked.Increment(ref mDebugCounter));
//#endif
//        }
//        #endregion // WriteLockRelease()

//        #region WriteEnter()
//        /// <summary>
//        /// This method is called when a thread enters a critical section that can modify the slot data.
//        /// </summary>
//        private void WriteEnter()
//        {
//            if (sLockIndex == 0)
//                sLockIndex = Interlocked.Increment(ref sLockCounter);

//            //Wait for lock release before entering the critical section.
//            if (mWriteLock == 1)
//            {
//                int myLock = Interlocked.Increment(ref mWriteLockPosition) - 1;
//#if (DEBUG)
//                Console.WriteLine("{0} WriteLock: Thread {1} locked: {2}/{3} -> {4}", Interlocked.Increment(ref mDebugCounter), sLockIndex, myLock, mWriteLockPosition, Thread.CurrentThread.ManagedThreadId);
//#endif
//                while (myLock < mWriteLockPosition)
//                    ThreadingHelper.ThreadWait();

//#if (DEBUG)
//                Console.WriteLine("{0} WriteLock: Thread {1} released: {2}/{3} -> {4}", Interlocked.Increment(ref mDebugCounter), sLockIndex, myLock, mWriteLockPosition, Thread.CurrentThread.ManagedThreadId);
//#endif
//                Interlocked.Increment(ref mWriteLockPosition);
//            }

//            mWriteThreadStatus[sLockIndex] = 1;
//        }
//        #endregion // WriteEnter()
//        #region WriteExit()
//        /// <summary>
//        /// This method is called when a thread leaves a critical section that can modify the slot data.
//        /// </summary>
//        private void WriteExit()
//        {
//            mWriteThreadStatus[sLockIndex] = 0;

//#if (DEBUG)
//            if (mWriteLock == 1)
//                Console.WriteLine("{0} WriteLock: Thread {1} leaving WriteExit()", Interlocked.Increment(ref mDebugCounter), sLockIndex);
//#endif
//        }
//        #endregion // WriteExit()

//        #region ItemIsLockedInternal(int index)
//        /// <summary>
//        /// This method checks whether an item in the collection is locked.
//        /// </summary>
//        /// <param name="index">The index of the item to check.</param>
//        /// <returns>Returns true if the item is locked.</returns>
//        private bool ItemIsLockedInternal(int index)
//        {
//            if (mIsFixedSize)
//                return mSlots[index].Locked == 1;

//            WriteEnter();
//            try
//            {
//                return mSlots[index].Locked == 1;
//            }
//            finally
//            {
//                WriteExit();
//            }
//        }
//        #endregion // ItemIsLocked(int index)
//        #region ItemLockWaitInternal(int index)
//        /// <summary>
//        /// This method waits for a locked item to become available.
//        /// </summary>
//        /// <param name="index">The index of the item to wait for.</param>
//        /// <returns>Returns the number of lock cycles during the wait.</returns>
//        public virtual void ItemLockWaitInternal(int index)
//        {
//            if (mIsFixedSize)
//            {
//                while (mSlots[index].Locked == 1)
//                {
//                    ThreadingHelper.ThreadWait();
//                }
//            }

//            while (true)
//            {
//                WriteEnter();
//                try
//                {
//                    if (Interlocked.CompareExchange(ref mSlots[index].Locked, 1, 0) == 0)
//                        break;

//                    ThreadingHelper.ThreadWait();
//                }
//                finally
//                {
//                    WriteExit();
//                }
//            }
//        }
//        #endregion // ItemLockWait(int index)
//        #region ItemLockInternal(int index)
//        /// <summary>
//        /// This method locks the item specified.
//        /// </summary>
//        /// <param name="index">The index of the item to lock.</param>
//        private void ItemLockInternal(int index)
//        {
//#if (LOCKDEBUG)
//            try
//            {
//                Console.WriteLine("{0} Lock: S{1} Locking slot -> on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
//#endif
//                if (mIsFixedSize)
//                {
//                    while (Interlocked.CompareExchange(ref mSlots[index].Locked, 1, 0) == 1)
//                        ThreadingHelper.ThreadWait();
//                    return;
//                }

//                while (true)
//                {
//                    WriteEnter();
//                    try
//                    {
//                        if (Interlocked.CompareExchange(ref mSlots[index].Locked, 1, 0) == 1)
//                            break;

//                        ThreadingHelper.ThreadWait();
//                    }
//                    finally
//                    {
//                        WriteExit();
//                    }
//                }
//#if (LOCKDEBUG)
//            }
//            finally
//            {
//                Console.WriteLine("{0} Lock: S{1} Locking complete -> on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
//            }
//#endif
//        }
//        #endregion // ItemLockInternal(int index)
//        #region ItemTryLockInternal(int index)
//        /// <summary>
//        /// This method attempts to lock the item specified.
//        /// </summary>
//        /// <param name="index">The index of the item you wish to lock..</param>
//        /// <returns>Returns true if the item was successfully locked.</returns>
//        private bool ItemTryLockInternal(int index)
//        {
//#if (LOCKDEBUG)
//            try
//            {
//                Console.WriteLine("{0} Lock: S{1} TryLocking slot -> on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
//#endif
//                if (mIsFixedSize)
//                    return (Interlocked.CompareExchange(ref mSlots[index].Locked, 1, 0) == 0);

//                WriteEnter();
//                try
//                {
//                    return (Interlocked.CompareExchange(ref mSlots[index].Locked, 1, 0) == 0);
//                }
//                finally
//                {
//                    WriteExit();
//                }
//#if (LOCKDEBUG)
//            }
//            finally
//            {
//                Console.WriteLine("{0} Lock: S{1} TryLocking complete -> on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
//            }
//#endif
//        }
//        #endregion // ItemTryLock(int index)
//        #region ItemUnlockInternal(int index)
//        /// <summary>
//        /// The method unlocks the item.
//        /// </summary>
//        /// <param name="index">The index of the item you wish to unlock.</param>
//        private void ItemUnlockInternal(int index)
//        {
//#if (LOCKDEBUG)
//            try
//            {
//                Console.WriteLine("{0} Lock: S{1} Unlocking slot on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
//#endif
//                if (mIsFixedSize)
//                {
//                    mSlots[index].Locked = 0;
//                    return;
//                }

//                WriteEnter();
//                try
//                {
//                    mSlots[index].Locked = 0;
//                }
//                finally
//                {
//                    WriteExit();
//                }
//#if (LOCKDEBUG)
//            }
//            finally
//            {
//                Console.WriteLine("{0} Lock: S{1} Unlocking complete on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
//            }
//#endif
//        }
//        #endregion // ItemUnlock(int index)
//        #region ItemSetValueInternal(int index, CollectionVertexStruct<T> value)
//        /// <summary>
//        /// This method sets the internal value for the item.
//        /// </summary>
//        /// <param name="index">The index position.</param>
//        /// <param name="value">The value to set</param>
//        private void ItemSetValueInternal(int index, CollectionVertexStruct<T> value)
//        {
//            if (mIsFixedSize)
//            {
//                mSlots[index].Value = value;
//                return;
//            }

//            WriteEnter();
//            try
//            {
//                mSlots[index].Value = value;
//            }
//            finally
//            {
//                WriteExit();
//            }
//        }
//        #endregion
//        #region ItemGetValueInternal(int index)
//        /// <summary>
//        /// This method sets the internal value for the item.
//        /// </summary>
//        /// <param name="index">The index position.</param>
//        /// <param name="value">The value to set</param>
//        private CollectionVertexStruct<T> ItemGetValueInternal(int index)
//        {
//            if (mIsFixedSize)
//                return mSlots[index].Value;

//            WriteEnter();
//            try
//            {
//                return mSlots[index].Value;
//            }
//            finally
//            {
//                WriteExit();
//            }
//        }
//        #endregion

//    }
}

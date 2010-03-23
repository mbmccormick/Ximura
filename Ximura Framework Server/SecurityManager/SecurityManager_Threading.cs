//#region Copyright
//// *******************************************************************************
//// Copyright (c) 2000-2009 Paul Stancer.
//// All rights reserved. This program and the accompanying materials
//// are made available under the terms of the Eclipse Public License v1.0
//// which accompanies this distribution, and is available at
//// http://www.eclipse.org/legal/epl-v10.html
////
//// Contributors:
////     Paul Stancer - initial implementation
//// *******************************************************************************
//#endregion
//﻿#region using
//using System;
//using System.ComponentModel;
//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.Security;
//using System.Security.Cryptography;
//using System.Security.Principal;
//using System.Security.Permissions;

//using Ximura;
//using Ximura.Helper;
//using Ximura.Server;

//using Ximura.Command;
//#endregion // using
//namespace Ximura.Server
//{
//    public partial class SecurityManager
//    {
//        #region Declarations
//        /// <summary>
//        /// List of all worker threads at the disposal of the thread pool.
//        /// </summary>
//        private object syncThreadPool = new object();

//        private List<Thread> mWorkerThreads;

//        private int mInUseThreads, mTotalThreads;

//        /// <summary>
//        /// Queue of all the callbacks waiting to be executed.
//        /// </summary>
//        private IXimuraDispatcherJobPriorityQueue mWaitingJobs;

//        /// <summary>
//        /// Used to signal that a worker thread is needed for processing.  Note that multiple
//        /// threads may be needed simultaneously and as such we use a semaphore instead of
//        /// an auto reset event.
//        /// </summary>
//        private SecurityManager.Semaphore mWorkerThreadNeeded;

//        #endregion

//        #region ThreadPoolInitiate()
//        /// <summary>
//        /// This method initializes the thread pool
//        /// </summary>
//        protected virtual void ThreadPoolInitiate()
//        {
//            mWaitingJobs = NewJobPriorityQueue();
//            mWorkerThreads = new List<Thread>(Configuration.ThreadPoolMin);
//            mInUseThreads = 0;

//            // Create our "thread needed" event
//            mWorkerThreadNeeded = new SecurityManager.Semaphore(0);

//            // Create the initial worker threads
//            for (int i = 0; i < Configuration.ThreadPoolMin; i++)
//            {
//                ThreadAddNew();
//            }
//        }
//        #endregion // ThreadPoolInitiate()
//        #region ThreadPoolShutdown()
//        /// <summary>
//        /// This method shuts down the thread pool.
//        /// </summary>
//        protected virtual void ThreadPoolShutdown()
//        {
//            QueuedItemsClear();

//            if (mWorkerThreads != null)
//            {
//                for (int i = this.mWorkerThreads.Count; i > 0; i--)
//                    mWorkerThreadNeeded.AddOne();
//            }
//        }
//        #endregion // ThreadPoolShutdown()

//        #region PerformanceUpdateCounters()
//        /// <summary>
//        /// This method is used to update the performance counter
//        /// after the status has changed.
//        /// </summary>
//        protected virtual void PerformanceUpdateCounters()
//        {

//        }
//        #endregion // UpdateCounters()

//        #region ThreadNeedNew
//        /// <summary>
//        /// This property returns true if the number of active threads reaches
//        /// the total number of threads in the pool.
//        /// </summary>
//        private bool ThreadNeedNew
//        {
//            get
//            {
//                return mInUseThreads >= mWorkerThreads.Count;
//            }
//        }
//        #endregion // NeedNewThread
//        #region ThreadAddNew()
//        private void ThreadAddNew()
//        {
//            Thread newThread;

//            lock (syncThreadPool)
//            {
//                //Make sure that we don't exceed the maximum thread count
//                if (Configuration.ThreadPoolMax.HasValue
//                    && mWorkerThreads.Count >= Configuration.ThreadPoolMax.Value)
//                    return;

//                // Create a new thread and add it to the list of threads.
//                newThread = new Thread(QueuedItemsProcess);

//                mWorkerThreads.Add(newThread);
//            }

//            Guid threadID = Guid.NewGuid();
//            //Configure the new thread and start it
//            //We add a guid to the end of the thread to ensure that it is 
//            //unique.
//            string id = DateTime.Now.ToString("s");
//            newThread.Name = CommandName + " #" + id + "-" + threadID.ToString("N");
//            newThread.IsBackground = true;
//            newThread.Start();
//        }
//        #endregion // AddNewThread()
//        #region ThreadRemove()
//        /// <summary>
//        /// This method aborts a thread pool thread. We use abort because this method 
//        /// will only be called by a thread that is not executing.
//        /// </summary>
//        private void ThreadRemove()
//        {
//            lock (syncThreadPool)
//            {
//                //Remove the thread from the collection
//                mWorkerThreads.Remove(Thread.CurrentThread);


//                try
//                {
//                    Thread.CurrentThread.Abort();
//                }
//                catch (ThreadAbortException)
//                {
//                    //Do nothing
//                }
//                catch (Exception ex)
//                {
//                    XimuraAppTrace.WriteLine(ex.Message);
//                }
//            }
//        }
//        #endregion // RemoveThisThread()

//        #region QueuedItemsClear
//        ///<summary>
//        ///This method empties the work queue of any queued work items.
//        ///</summary>
//        private void QueuedItemsClear()
//        {
//            // Clear all waiting items and reset the number of worker threads currently needed
//            // to be 0 (there is nothing for threads to do)
//            if (mWaitingJobs != null)
//                mWaitingJobs.Clear();

//            if (mWorkerThreadNeeded != null)
//                mWorkerThreadNeeded.Reset(0);

//            //Update the counters with the new value
//            PerformanceUpdateCounters();
//        }
//        #endregion // EmptyQueue
//        #region QueuedItemsProcess()
//        /// <summary>
//        /// A thread worker function that processes items from the work queue.
//        /// </summary>
//        private void QueuedItemsProcess()
//        {
//            Interlocked.Increment(ref mTotalThreads);
//            // Process indefinitely
//            while (true)
//            {
//                // Get the next item in the queue.  If there is nothing there, go to sleep
//                // for a while until we're woken up when a callback is waiting.
//                SecurityManagerJob job = null;

//                while (job == null)
//                {
//                    // Try to get the next callback available.  We need to lock on the 
//                    // queue in order to make our count check and retrieval atomic.
//                    if (this.ServiceStatus == XimuraServiceStatus.Stopping ||
//                        this.ServiceStatus == XimuraServiceStatus.Stopped)
//                        ThreadRemove();

//                    if (mWaitingJobs.Count > 0)
//                    {
//                        try
//                        {
//                            job = mWaitingJobs.Pop();
//                        }
//                        catch { } // We should not fail here.
//                    }

//                    // If we can't get one, go to sleep.
//                    if (job == null)
//                        mWorkerThreadNeeded.WaitOne();
//                }

//                // We now have a callback.  Execute it.  Make sure to accurately
//                // record how many callbacks are currently executing.
//                try
//                {
//                    Interlocked.Increment(ref mInUseThreads);

//                    job.ActiveThread = Thread.CurrentThread;

//                    switch (job.JobType)
//                    {
//                        case DJobType.Command:
//                            //This is the standard entry point for a job request.
//                            job.Command.ProcessRequestSCM(job);
//                            break;
//                        case DJobType.Callback:
//                            job.JobCallback(job);
//                            break;
//                    }

//                    JobComplete(job);

//                    job.ActiveThread = null;
//                }
//                catch (Exception ex)
//                {
//                    // Make sure we don't throw here, but warn the security manager
//                    // that the job has thrown an unhandled exception.
//                    JobException(job, ex, Thread.CurrentThread);
//                }
//                finally
//                {
//                    Interlocked.Decrement(ref mInUseThreads);
//                }
//            }

//            Interlocked.Decrement(ref mTotalThreads);
//        }
//        #endregion


//        #region Class -> Semaphore
//        /// <summary>
//        /// Implementation of Dijkstra's PV Semaphore based on the Monitor class.
//        /// </summary>
//        public class Semaphore
//        {
//            #region Declarations
//            /// <summary>
//            /// The number of units alloted by this semaphore.
//            /// </summary>
//            private int m_count;
//            #endregion
//            #region Constructor
//            /// <overloads>
//            /// Initializes the semaphore.
//            /// </overloads>
//            /// <summary>
//            /// Initialize the semaphore as a binary semaphore.
//            /// </summary>
//            public Semaphore() : this(1) { }

//            /// <summary>
//            /// Initialize the semaphore as a counting semaphore.
//            /// </summary>
//            /// <param name="count">Initial number of threads that can take out units from this semaphore.</param>
//            /// <exception cref="ArgumentException">Throws if the count argument is less than 1.</exception>
//            public Semaphore(int count)
//            {
//                if (count < 0)
//                    throw new ArgumentException("Semaphore must have a count of at least 0.", "count");
//                m_count = count;
//            }
//            #endregion

//            #region Synchronization Operations
//            //			/// <summary>
//            //			/// V the semaphore (add 1 unit to it).
//            //			/// </summary>
//            //			public void AddOne() 
//            //			{
//            //				V();
//            //			}

//            //			/// <summary>
//            //			/// P the semaphore (take out 1 unit from it).
//            //			/// </summary>
//            //			public void WaitOne()
//            //			{
//            //				P();
//            //			}
//            #endregion

//            #region WaitOne
//            /// <summary>
//            /// P the semaphore (take out 1 unit from it).
//            /// </summary>
//            public void WaitOne()//P() 
//            {
//                // Lock so we can work in peace.  This works because lock is actually
//                // built around Monitor.
//                lock (this)
//                {
//                    // Wait until a unit becomes available.  We need to wait
//                    // in a loop in case someone else wakes up before us.  This could
//                    // happen if the Monitor.Pulse statements were changed to Monitor.PulseAll
//                    // statements in order to introduce some randomness into the order
//                    // in which threads are woken.
//                    while (m_count <= 0)
//                        Monitor.Wait(this, Timeout.Infinite);
//                    m_count--;
//                }
//            }
//            #endregion // WaitOne
//            #region AddOne
//            /// <summary>
//            /// V the semaphore (add 1 unit to it).
//            /// </summary>
//            public void AddOne()//V() 
//            {
//                // Lock so we can work in peace.  This works because lock is actually
//                // built around Monitor.
//                lock (this)
//                {
//                    // Release our hold on the unit of control.  Then tell everyone
//                    // waiting on this object that there is a unit available.
//                    m_count++;
//                    Monitor.Pulse(this);
//                }
//            }
//            #endregion // AddOne
//            #region Reset
//            /// <summary>
//            /// Resets the semaphore to the specified count. Should be used cautiously.
//            /// </summary>
//            public void Reset(int count)
//            {
//                lock (this) { m_count = count; }
//            }
//            #endregion // Reset
//        }
//        #endregion

//    }
//}

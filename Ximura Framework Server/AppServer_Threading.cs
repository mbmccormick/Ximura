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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Framework;
using Ximura.Framework;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Framework
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region Declarations
        /// <summary>
        /// List of all worker threads at the disposal of the thread pool.
        /// </summary>
        private object syncThreadPool = new object();

        private List<Thread> mWorkerThreads;

        private int mInUseThreads, mTotalThreads;

        /// <summary>
        /// Queue of all the callbacks waiting to be executed.
        /// </summary>
        private IXimuraDispatcherJobPriorityQueue mWaitingJobs;

        /// <summary>
        /// Used to signal that a worker thread is needed for processing.  Note that multiple
        /// threads may be needed simultaneously and as such we use a semaphore instead of
        /// an auto reset event.
        /// </summary>
        private ThreadingHelper.Semaphore mWorkerThreadNeeded;

        #endregion

        #region ThreadPoolStart()
        /// <summary>
        /// This method initializes the thread pool
        /// </summary>
        protected virtual void ThreadPoolStart()
        {
            int count = System.Environment.ProcessorCount;

            mWaitingJobs = JobPriorityQueueCreate();
            mWorkerThreads = new List<Thread>(ConfigurationSystem.ThreadPoolMin);
            mInUseThreads = 0;

            // Create our "thread needed" event
            mWorkerThreadNeeded = new ThreadingHelper.Semaphore(0);

            // Create the initial worker threads
            for (int i = 0; i < ConfigurationSystem.ThreadPoolMin; i++)
            {
                ThreadAddNew();
            }
        }
        #endregion // ThreadPoolInitiate()
        #region ThreadPoolStop()
        /// <summary>
        /// This method shuts down the thread pool.
        /// </summary>
        protected virtual void ThreadPoolStop()
        {
            QueuedItemsClear();

            if (mWorkerThreads != null)
            {
                for (int i = this.mWorkerThreads.Count; i > 0; i--)
                    mWorkerThreadNeeded.AddOne();
            }
        }
        #endregion // ThreadPoolShutdown()

        #region PerformanceUpdateCounters()
        /// <summary>
        /// This method is used to update the performance counter
        /// after the status has changed.
        /// </summary>
        protected virtual void PerformanceUpdateCounters()
        {

        }
        #endregion // UpdateCounters()

        #region ThreadNeedNew
        /// <summary>
        /// This property returns true if the number of active threads reaches
        /// the total number of threads in the pool.
        /// </summary>
        private bool ThreadNeedNew
        {
            get
            {
                return mInUseThreads >= mWorkerThreads.Count;
            }
        }
        #endregion // NeedNewThread
        #region ThreadAddNew()
        private void ThreadAddNew()
        {
            Thread newThread;

            lock (syncThreadPool)
            {
                //Make sure that we don't exceed the maximum thread count
                if (ConfigurationSystem.ThreadPoolMax.HasValue
                    && mWorkerThreads.Count >= ConfigurationSystem.ThreadPoolMax.Value)
                    return;

                // Create a new thread and add it to the list of threads.
                newThread = new Thread(QueuedItemsProcess);

                mWorkerThreads.Add(newThread);
            }

            //Configure the new thread and start it
            //We add a guid to the end of the thread to ensure that it is unique.
            newThread.Name = string.Format("{0} #{1}-{2}",
                ConfigurationSystem.Name, DateTime.Now.ToString("s"), Guid.NewGuid().ToString("N"));

            newThread.IsBackground = true;
            newThread.Start();
        }
        #endregion // AddNewThread()
        #region ThreadRemove()
        /// <summary>
        /// This method aborts a thread pool thread. We use abort because this method 
        /// will only be called by a thread that is not executing.
        /// </summary>
        private void ThreadRemove()
        {
            lock (syncThreadPool)
            {
                //Remove the thread from the collection
                mWorkerThreads.Remove(Thread.CurrentThread);

                try
                {
                    Thread.CurrentThread.Abort();
                }
                catch (ThreadAbortException)
                {
                    //Do nothing
                }
                catch (Exception ex)
                {
                    XimuraAppTrace.WriteLine(ex.Message);
                }
            }
        }
        #endregion // RemoveThisThread()

        #region QueuedItemsClear
        ///<summary>
        ///This method empties the work queue of any queued work items.
        ///</summary>
        private void QueuedItemsClear()
        {
            // Clear all waiting items and reset the number of worker threads currently needed
            // to be 0 (there is nothing for threads to do)
            if (mWaitingJobs != null)
                mWaitingJobs.Clear();

            if (mWorkerThreadNeeded != null)
                mWorkerThreadNeeded.Reset(0);

            //Update the counters with the new value
            PerformanceUpdateCounters();
        }
        #endregion // EmptyQueue
        #region QueuedItemsProcess()
        /// <summary>
        /// A thread worker function that processes items from the work queue.
        /// </summary>
        private void QueuedItemsProcess()
        {
            Interlocked.Increment(ref mTotalThreads);
            // Process indefinitely
            while (true)
            {
                // Get the next item in the queue.  If there is nothing there, go to sleep
                // for a while until we're woken up when a callback is waiting.
                SecurityManagerJob job = null;

                while (job == null)
                {
                    // Try to get the next callback available.  We need to lock on the 
                    // queue in order to make our count check and retrieval atomic.
                    if (this.ServiceStatus == XimuraServiceStatus.Stopping ||
                        this.ServiceStatus == XimuraServiceStatus.Stopped)
                        ThreadRemove();

                    if (mWaitingJobs.Count > 0)
                    {
                        try
                        {
                            job = mWaitingJobs.Pop();
                        }
                        catch { } // We should not fail here.
                    }

                    // If we can't get one, go to sleep.
                    if (job == null)
                        mWorkerThreadNeeded.WaitOne();
                }

                // We now have a callback. Execute it. Make sure to accurately
                // record how many callbacks are currently executing.
                try
                {
                    Interlocked.Increment(ref mInUseThreads);

                    job.ActiveThread = Thread.CurrentThread;

                    switch (job.JobType)
                    {
                        case DJobType.Command:
                            //This is the standard entry point for a job request.
                            job.Command.ProcessRequestSCM(job);
                            break;
                        case DJobType.Callback:
                            job.JobCallback(job);
                            break;
                    }

                    JobComplete(job);

                    job.ActiveThread = null;
                }
                catch (Exception ex)
                {
                    // Make sure we don't throw here, but warn the security manager
                    // that the job has thrown an unhandled exception.
                    JobException(job, ex, Thread.CurrentThread);
                }
                finally
                {
                    Interlocked.Decrement(ref mInUseThreads);
                    PerformanceUpdateCounters();
                }
            }

            Interlocked.Decrement(ref mTotalThreads);
            PerformanceUpdateCounters();
        }
        #endregion

        #region JobPriorityQueueCreate()
        /// <summary>
        /// This method returns a new job priority queue class.
        /// </summary>
        protected virtual IXimuraDispatcherJobPriorityQueue JobPriorityQueueCreate()
        {
            return new DispatcherJobPriorityQueue(ConfigurationSystem.JobPriorityCapacity);
        }
        #endregion // JobPriorityQueueCreate()


    }
}

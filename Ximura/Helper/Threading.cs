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
using System.Text;
using System.Linq;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Collections;
using Ximura.Helper;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This static class provides threading help.
    /// </summary>
    public static class ThreadingHelper
    {
        #region ThreadWait()
        /// <summary>
        /// This method pauses the current thread for the minimum amount of time.
        /// </summary>
        public static void ThreadWait()
        {
            if ((Environment.ProcessorCount > 1))
                Thread.SpinWait(20);
            else
                Thread.Sleep(0);
        }
        #endregion

        #region Func<Action, Thread> fnActionExecute
        /// <summary>
        /// This function creates a new thread and executes the action on that thread.
        /// </summary>
        public static Func<ThreadStart, Thread> fnActionExecute =
            (act) =>
            {
                Thread t = new Thread(act);
                t.Start();
                return t;
            };
        #endregion // Func<Action, Thread> fnActionExecute

        #region ExecuteParallel(IEnumerable<Action> ts, int maxThreads)
        /// <summary>
        /// Executes a number of actions in parallel and then waits until they are complete.
        /// </summary>
        /// <param name="ts">The action enumeration.</param>
        /// <returns>Returns a timespan containing the time taken to execute the job.</returns>
        public static TimeSpan ExecuteParallel(IEnumerable<Action> ts)
        {
            return ts.Execute(Environment.ProcessorCount);
        }        
        /// <summary>
        /// Executes a number of actions in parallel and then waits until they are complete.
        /// </summary>
        /// <param name="ts">The action enumeration.</param>
        /// <param name="maxThreads">The maximum number of threads to use during execution.</param>
        /// <returns>Returns a timespan containing the time taken to execute the job.</returns>
        public static TimeSpan ExecuteParallel(IEnumerable<Action> ts, int maxThreads)
        {
            return ts.Execute(maxThreads);
        }
        #endregion // ThreadCheck(IEnumerable<ThreadStart> ts)
        #region Execute(this IEnumerable<Action> ts, int maxThreads)
        /// <summary>
        /// This method enumerates the actions and executes them in parallel and waits until they are complete.
        /// By default this method sets the number of parallel jobs to the number of processors in the machine.
        /// </summary>
        /// <param name="ts">The action enumeration.</param>
        /// <returns>Returns a timespan containing the time taken to execute the job.</returns>
        public static TimeSpan Execute(this IEnumerable<Action> ts)
        {
            return ts.Execute(Environment.ProcessorCount);
        }
        /// <summary>
        /// This method enumerates the actions and executes them in parallel and waits until they are complete.
        /// </summary>
        /// <param name="ts">The action enumeration.</param>
        /// <param name="maxThreads">The maximum number of parallel executions.</param>
        /// <returns>Returns a timespan containing the time taken to execute the job.</returns>
        public static TimeSpan Execute(this IEnumerable<Action> ts, int maxThreads)
        {
            DateTime start = DateTime.Now;

            int threadsActive = 0;

            Func<Action, ThreadStart> fnCountWrapper = (j) => new ThreadStart(() =>
            {
                Interlocked.Increment(ref threadsActive);
                j();
                Interlocked.Decrement(ref threadsActive);
            });

            var threads = ts.Select(a =>
                    {
                        Thread t = fnActionExecute(fnCountWrapper(a));

                        //This loop ensures that we keep the number of active threads to the threadLimit.
                        while (maxThreads > 0 && threadsActive >= maxThreads)
                            ThreadWait();

                        return t;
                    }
                ).ToArray();

            //Ok, wait for any running threads to complete execution.
            threads.Where(t => t.IsAlive).ForEach(t => t.Join());

            //Calculate the timespan for the process.
            return DateTime.Now - start;
        }
        #endregion

        #region Class -> Semaphore
        /// <summary>
        /// Implementation of Dijkstra's PV Semaphore based on the Monitor class.
        /// </summary>
        public class Semaphore
        {
            #region Declarations
            /// <summary>
            /// The number of units alloted by this semaphore.
            /// </summary>
            private int m_count;
            #endregion
            #region Constructor
            /// <overloads>
            /// Initializes the semaphore.
            /// </overloads>
            /// <summary>
            /// Initialize the semaphore as a binary semaphore.
            /// </summary>
            public Semaphore() : this(1) { }

            /// <summary>
            /// Initialize the semaphore as a counting semaphore.
            /// </summary>
            /// <param name="count">Initial number of threads that can take out units from this semaphore.</param>
            /// <exception cref="ArgumentException">Throws if the count argument is less than 1.</exception>
            public Semaphore(int count)
            {
                if (count < 0)
                    throw new ArgumentException("Semaphore must have a count of at least 0.", "count");
                m_count = count;
            }
            #endregion

            #region Synchronization Operations
            //			/// <summary>
            //			/// V the semaphore (add 1 unit to it).
            //			/// </summary>
            //			public void AddOne() 
            //			{
            //				V();
            //			}

            //			/// <summary>
            //			/// P the semaphore (take out 1 unit from it).
            //			/// </summary>
            //			public void WaitOne()
            //			{
            //				P();
            //			}
            #endregion

            #region WaitOne
            /// <summary>
            /// P the semaphore (take out 1 unit from it).
            /// </summary>
            public void WaitOne()//P() 
            {
                // Lock so we can work in peace.  This works because lock is actually
                // built around Monitor.
                lock (this)
                {
                    // Wait until a unit becomes available.  We need to wait
                    // in a loop in case someone else wakes up before us.  This could
                    // happen if the Monitor.Pulse statements were changed to Monitor.PulseAll
                    // statements in order to introduce some randomness into the order
                    // in which threads are woken.
                    while (m_count <= 0)
                        Monitor.Wait(this, Timeout.Infinite);
                    m_count--;
                }
            }
            #endregion // WaitOne
            #region AddOne
            /// <summary>
            /// V the semaphore (add 1 unit to it).
            /// </summary>
            public void AddOne()//V() 
            {
                // Lock so we can work in peace.  This works because lock is actually
                // built around Monitor.
                lock (this)
                {
                    // Release our hold on the unit of control.  Then tell everyone
                    // waiting on this object that there is a unit available.
                    m_count++;
                    Monitor.Pulse(this);
                }
            }
            #endregion // AddOne
            #region Reset
            /// <summary>
            /// Resets the semaphore to the specified count. Should be used cautiously.
            /// </summary>
            public void Reset(int count)
            {
                lock (this) { m_count = count; }
            }
            #endregion // Reset
        }
        #endregion
    }
}

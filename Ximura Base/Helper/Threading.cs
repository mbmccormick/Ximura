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
using Ximura.Server;
#endregion // using
namespace Ximura.Helper
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
    }
}

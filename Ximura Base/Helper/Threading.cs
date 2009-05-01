﻿#region Copyright
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
    public static class Threading
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

        //public static Func<Action<T1>, Thread> ActionExecuteParam = t1 => ActionExecute.Curry()(t1);

        public static Func<Action, Thread> ActionExecute =
            (act) =>
            {
                Thread t = new Thread(new ThreadStart(act));
                t.Start();
                return t;
            };


        #region ExecuteActionsInt(IEnumerable<Action> ts, int threadLimit)
        private static TimeSpan ExecuteActionsInt(IEnumerable<Action> ts, int threadLimit)
        {
            DateTime start = DateTime.Now;

            int active = 0;

            Func<Action, ThreadStart> fnCountWrapper = (j) => new ThreadStart(() =>
                {
                    Interlocked.Increment(ref active);
                    j();
                    Interlocked.Decrement(ref active);
                });

            var threads = ts.Select(a =>
                {
                    Thread t = ActionExecute(a);

                    while (threadLimit > 0 && active >= threadLimit)
                        ThreadWait();

                    return t;
                }
                ).ToArray();

            threads.Where(t => t.IsAlive).ForEach(t => t.Join());

            return DateTime.Now - start;
        }
        #endregion // ExecuteParallelInt(IEnumerable<Action> ts)

        #region ExecuteParallel(IEnumerable<Action> ts, int maxThreads)
        /// <summary>
        /// Executes a number of actions in parallel and then waits until they are complete.
        /// </summary>
        /// <param name="ts">The action enumeration.</param>
        /// <returns>Returns a timespan containing the time taken to execute the job.</returns>
        public static TimeSpan ExecuteParallel(IEnumerable<Action> ts, int maxThreads)
        {
            return ExecuteActionsInt(ts, maxThreads);
        }
        #endregion // ThreadCheck(IEnumerable<ThreadStart> ts)
        #region Execute(this IEnumerable<Action> ts, int maxThreads)
        /// <summary>
        /// This method enumerates the actions and executes them in parallel and waits until they are complete.
        /// </summary>
        /// <param name="ts">The action enumeration.</param>
        /// <returns>Returns a timespan containing the time taken to execute the job.</returns>
        public static TimeSpan Execute(this IEnumerable<Action> ts)
        {
            return ExecuteActionsInt(ts, -1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="maxThreads"></param>
        /// <returns></returns>
        public static TimeSpan Execute(this IEnumerable<Action> ts, int maxThreads)
        {
            return ExecuteActionsInt(ts, maxThreads);
        }
        #endregion





    }






}
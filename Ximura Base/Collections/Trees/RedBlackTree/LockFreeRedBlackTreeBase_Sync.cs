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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public partial class LockFreeRedBlackTreeBase<TKey, TVal, TVert>
    {
        #region Declarations
        private int mThreadsCurrent = 0;
        private readonly int mThreadsMax = Environment.ProcessorCount * 2;
        #endregion // Declarations

        #region TreeAction
        /// <summary>
        /// This enumeration is used to specify the action type.
        /// </summary>
        [Flags()]
        protected enum TreeAction : int
        {
            /// <summary>
            /// The thread is inserting data.
            /// </summary>
            Insert = 1,
            /// <summary>
            /// The thread is removing data.
            /// </summary>
            Remove = 2,
            /// <summary>
            /// The thread is checking data is in the tree.
            /// </summary>
            Contains = 4
        }
        #endregion // TreeAction

        #region ThreadEnter(TreeAction act)
        /// <summary>
        /// This method registers a thread when it enters the tree.
        /// </summary>
        /// <param name="act">The thread action.</param>
        protected void ThreadEnter(TreeAction act)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            Interlocked.Increment(ref mThreadsCurrent);

            //Wait if there are too many threads executing.
            while (mThreadsCurrent > mThreadsMax)
                Threading.ThreadWait();

        }
        #endregion // ThreadEnter(TreeAction act)

        #region ThreadExit()
        /// <summary>
        /// This method unregisters the thread from the collection.
        /// </summary>
        protected void ThreadExit()
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            Interlocked.Decrement(ref mThreadsCurrent);

        }
        #endregion // ThreadExit()


    }
}

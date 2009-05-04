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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract class LockableBase: ILockable
    {
        #region Declarations
        private int mLocked;
        private int? mLockingThread = null;
        #endregion // Declarations


        #region ILockable Members

        public bool IsLocked()
        {
            return mLocked == 1;
        }

        public void Lock()
        {
            while (!TryLock())
                ThreadWait();
        }

        public void LockWait()
        {
            while (IsLocked())
                ThreadWait();
        }

        public bool TryLock()
        {
            bool result = (Interlocked.CompareExchange(ref mLocked, 1, 0) == 0);

            if (result)
                mLockingThread = Thread.CurrentThread.ManagedThreadId;

            return result;
        }

        public void Unlock()
        {
            mLocked = 0;
            mLockingThread = null;
        }

        #endregion

        #region ThreadWait()
        protected void ThreadWait()
        {
            if ((Environment.ProcessorCount > 1))
            {
                Thread.SpinWait(20);
            }
            else
            {
                Thread.Sleep(0);
            }
        }
        #endregion

    }
}

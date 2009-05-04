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
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class LockFreeCollectionBase<T>
    {
        //#region WorldStop()
        ///// <summary>
        ///// The world stop functionality allows for 'Stop The World' scenarios where all incoming threads are halted to 
        ///// allow a large atomic operation to execute. This method is not implemented in the base collection class.
        ///// </summary>
        ///// <returns></returns>
        //protected virtual bool WorldStop()
        //{
        //    DisposedCheck();
        //    return false;
        //}
        //#endregion // WorldStop()
        //#region WorldRelease()
        ///// <summary>
        ///// This method should be called to release the global lock. This method is not implemented in the base collection class.
        ///// </summary>
        //protected virtual void WorldRelease()
        //{

        //}
        //#endregion // WorldRelease()
        //#region WorldEnter()
        ///// <summary>
        ///// This method is called when a thread enters the class.
        ///// </summary>
        //protected virtual void WorldEnter()
        //{
        //    DisposedCheck();
        //}
        //#endregion // WorldEnter()
        //#region WorldLeave()
        ///// <summary>
        ///// This method is called when a thread leaves the class.
        ///// </summary>
        //protected virtual void WorldLeave()
        //{

        //}
        //#endregion // WorldLeave()
    }
}

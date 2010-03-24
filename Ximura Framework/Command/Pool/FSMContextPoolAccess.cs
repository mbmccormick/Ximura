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
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    public delegate CNTX delFSMContextGet<CNTX>()
    where CNTX : class, IXimuraFSMContext, new();

    public delegate void delFSMContextReturn<CNTX>(CNTX buffer)
        where CNTX : class, IXimuraFSMContext, new();

    /// <summary>
    /// This class is used by the Transport class to allow transport buffers access to the buffer pool 
    /// for listening connections that require a new buffer.
    /// </summary>
    public class FSMContextPoolAccess<CNTX> : IXimuraFSMContextPoolAccess
        where CNTX : class, IXimuraFSMContext, new()
    {
        private delFSMContextGet<CNTX> mContextGet;
        private delFSMContextReturn<CNTX> mContextReturn;

        public CNTX ContextGet()
        {
            return mContextGet.Invoke();
        }

        public void ContextReturn(CNTX context)
        {
            mContextReturn.Invoke(context);
        }

        public FSMContextPoolAccess(delFSMContextGet<CNTX> contextGet, delFSMContextReturn<CNTX> contextReturn)
        {
            this.mContextGet = contextGet;
            this.mContextReturn = contextReturn;
        }

        #region IXimuraFSMContextPoolAccess Members

        public IXimuraFSMContext ContextGetGeneric()
        {
            return ContextGet();
        }

        public void ContextReturnGeneric(IXimuraFSMContext context)
        {
            CNTX cntx = context as CNTX;
            if (cntx == null)
                throw new ArgumentOutOfRangeException("Context is not of the correct type.");

            ContextReturn(cntx);
        }

        #endregion
    }

    /// <summary>
    /// This interface is used to allow non-generic implementations of the class due to the .Net 2.0 generic limitations.
    /// </summary>
    public interface IXimuraFSMContextPoolAccess
    {
        IXimuraFSMContext ContextGetGeneric();
        void ContextReturnGeneric(IXimuraFSMContext context);
    }
}
